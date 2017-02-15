using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Gallio.Framework;
using MbUnit.Framework;

using System.Security.Cryptography;
using System.Linq;

using log4net;
using log4net.Config;
using System.Collections;

namespace FTTests {

    public class SQL {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _dbConnectionString;
        private int _individualID;
        private int _householdID;
        private Dictionary<int, string> _amsChurches = new Dictionary<int, string>();
        private Dictionary<int, string> _nonAmsChurches = new Dictionary<int, string>();
        private Dictionary<string, Boolean> _ams = new Dictionary<string, Boolean>();

        #region Properties
        public int IndividualID {
            get { return _individualID; }
        }

        public int HouseholdID {
            get { return _householdID; }
        }

        public Dictionary<string, Boolean> AMS
        {
            //AMS["ChurchId"]
            get { return _ams; }
        }

        public Dictionary<int, string> AMSChurches
        {
            get { return this._amsChurches; }
        }

        public Dictionary<int, string> NonAMSChurches
        {
            get { return this._nonAmsChurches; }
        }

        #endregion Properties

        #region Constructors
        public SQL(string dbConnectionString) {

            log.Debug("Enter Instantiate SQL");
            //TODO: 
            //FGJ Remove hard core value here
            //Set in config app
            this._dbConnectionString = dbConnectionString;
            this._individualID = this.People_Individuals_FetchID(15, "Matthew Sneeden");
            this._householdID = this.People_Households_FetchID(15, _individualID);

            //FGJ, seeting it up now in SeleniumTestBase
            //Set AMS status for all provisioned churches
            //this._ams = this.AMS_Enable_Status_Add();

            log.Debug("Exit Instantiate SQL");

        }
        #endregion Constructors

        #region Admin
        /// <summary>
        /// This method creates an activity type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityTypeName">The activity type name.</param>
        /// <param name="isActive">The active setting for the activity type.</param>
        public void Admin_ActivityTypesCreate(int churchId, string activityTypeName, bool isActive) {
            this.Execute(string.Format("INSERT INTO ChmActivity.dbo.ACTIVITY_TYPE (CHURCH_ID, ACTIVITY_TYPE_NAME, IS_ACTIVE) VALUES({0}, '{1}', {2})", churchId, activityTypeName, Convert.ToInt16(isActive)));
        }

        /// <summary>
        /// This method deletes an activity type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityTypeName">The activity type name.</param>
        public void Admin_ActivityTypesDelete(int churchId, string activityTypeName) {
            this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_TYPE WHERE CHURCH_ID = {0} AND ACTIVITY_TYPE_NAME = '{1}'", churchId, activityTypeName));
        }

        /// <summary>
        /// This method creates a building.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="buildingName">The building name.</param>
        public void Admin_BuildingsCreate(int churchId, string buildingName) {
            this.Execute(string.Format("INSERT INTO ChmChurch.dbo.BUILDING (CHURCH_ID, BUILDING_NAME) VALUES({0}, '{1}')", churchId, buildingName));
        }

        /// <summary>
        /// This method deletes a building.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="buildingName">The name of the building to be deleted.</param>
        public void Admin_BuildingsDelete(int churchId, string buildingName) {
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.BUILDING WHERE CHURCH_ID = {0} AND BUILDING_NAME = '{1}'", churchId, buildingName));
        }

        /// <summary>
        /// This method creates a campus.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="campusName">The name of the campus.</param>
        public void Admin_CampusCreate(int churchId, string campusName) {
            this.Execute(string.Format("INSERT INTO ChmPeople.dbo.ChurchCampus (ChurchID, CampusName) VALUES ({0}, '{1}')", churchId, campusName));
        }

        /// <summary>
        /// This method deletes a campus.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="campusName">The name of the campus.</param>
        public void Admin_CampusDelete(int churchId, string campusName) {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.ChurchCampus WHERE ChurchID = {0} AND CampusName = '{1}'", churchId, campusName));
        }

        /// <summary>
        /// This method creates a contact item.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="
        /// Name">The name of the contact item.</param>
        /// <param name="contactItemType">The type of the contact item.</param>
        /// <param name="ministryName">The name of the ministry the contact item will be tied to.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="active">The active flag.</param>
        /// <param name="multipleClose">The multiple close flag.</param>
        /// <param name="dispositionRequiredOnClose">The volunteer flag.</param>
        public void Admin_ContactItems_Create(int churchId, string contactItemName, int contactItemType, string ministryName, int? userId, bool active, bool multipleClose, bool dispositionRequiredOnClose) {
            string column = string.Empty;
            string userIdVal = string.Empty;
            StringBuilder query = new StringBuilder();
            query.AppendFormat("INSERT INTO ChmChurch.dbo.CONTACT_ITEM (CHURCH_ID, CONTACT_ITEM_NAME, CONTACT_ITEM_TYPE_ID, MINISTRY_ID,{0} IS_ACTIVE, MULTIPLE_CLOSE, DISPOSITION_REQUIRED_ON_CLOSE) ", column = userId.HasValue ? " USER_ID," : "");
            query.AppendFormat("SELECT {0}, '{1}', {2}, MINISTRY_ID,{3} {4}, {5}, {6} FROM ChmActivity.dbo.MINISTRY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND MINISTRY_NAME = '{7}'", churchId, contactItemName, contactItemType, userIdVal = userId.HasValue ? string.Format(" {0},", userId.ToString()) : "", Convert.ToInt16(active), Convert.ToInt16(multipleClose), Convert.ToInt16(dispositionRequiredOnClose), ministryName);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a contact item.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contactItemName">The name of the contact item.</param>
        public void Admin_ContactItems_Delete(int churchId, string contactItemName) {
            //Modify by Clark.Peng, if DataTable is empty, don't excute the delete sql
            DataTable dtResult = this.Execute(string.Format("SELECT CONTACT_ITEM_ID FROM ChmChurch.dbo.CONTACT_ITEM WHERE CHURCH_ID = {0} AND CONTACT_ITEM_NAME = '{1}' ORDER BY CREATED_DATE", churchId, contactItemName));
            int itemId;
            if (dtResult.Rows.Count > 0)
                itemId = Convert.ToInt32(dtResult.Rows[0]["CONTACT_ITEM_ID"].ToString());
            else
                return;
            
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.CONTACT_INSTANCE_ITEM WHERE CHURCH_ID = {0} AND CONTACT_ITEM_ID = {1}", churchId, itemId));
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.CONTACT_ITEM WHERE CHURCH_ID = {0} AND CONTACT_ITEM_ID = {1}", churchId, itemId));
        }

        public string Admin_ContactItems_FetchTheLatestContactInstanceId(int churchId, string contactItemName)
        {
            int itemId = Convert.ToInt32(this.Execute(string.Format("SELECT CONTACT_ITEM_ID FROM ChmChurch.dbo.CONTACT_ITEM WHERE CHURCH_ID = {0} AND CONTACT_ITEM_NAME = '{1}' ORDER BY CREATED_DATE", churchId, contactItemName)).Rows[0]["CONTACT_ITEM_ID"].ToString());
            return this.Execute(string.Format("SELECT CONTACT_INSTANCE_ITEM_ID FROM ChmChurch.dbo.CONTACT_INSTANCE_ITEM WHERE CHURCH_ID = {0} AND CONTACT_ITEM_ID = {1} ORDER BY CREATED_DATETIME", churchId, itemId)).Rows[0]["CONTACT_INSTANCE_ITEM_ID"].ToString();
        }

        public void Admin_ContactItems_ContactItemInstance_Delete(int churchId, int contactInstanceId)
        {
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.CONTACT_INSTANCE_ITEM WHERE CHURCH_ID = {0} AND CONTACT_INSTANCE_ITEM_ID = {1}", churchId, contactInstanceId));
        }

        /// <summary>
        /// This method creates a contact disposition.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contactDispositionName">The name of the contact disposition.</param>
        /// <param name="active">The active flag.</param>
        /// <param name="portal">Flag designating visibility in portal.</param>
        /// <param name="weblink">Flag designation visibility in weblink.</param>
        public void Admin_ContactDispositionsCreate(int churchId, string contactDispositionName, bool active, bool portal, bool weblink) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmChurch.dbo.CONTACT_DISPOSITION (CHURCH_ID, CONTACT_DISPOSITION_NAME, IS_ACTIVE) ");
            query.AppendFormat("VALUES({0}, '{1}', {2}) ", churchId, contactDispositionName, Convert.ToInt16(active));

            query.Append("DECLARE @contactDispositionID INT ");
            query.Append("SET @contactDispositionID = (SELECT SCOPE_IDENTITY()) ");

            if (portal) {
                query.Append("INSERT INTO ChmChurch.dbo.CONTACT_DISPOSITION_APPLICATION (CONTACT_DISPOSITION_ID, APPLICATION_ID, CHURCH_ID) ");
                query.AppendFormat("VALUES(@contactDispositionID, 0, {0}) ", churchId);
            }

            if (weblink) {
                query.Append("INSERT INTO ChmChurch.dbo.CONTACT_DISPOSITION_APPLICATION (CONTACT_DISPOSITION_ID, APPLICATION_ID, CHURCH_ID) ");
                query.AppendFormat("VALUES(@contactDispositionID, 17, {0}) ", churchId);
            }

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a contact disposition.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contactDispositionName">The name of the contact disposition.</param>
        public void Admin_ContactDispositionsDelete(int churchId, string contactDispositionName) {
            StringBuilder query = new StringBuilder("DECLARE @contactDispositionID INT ");
            query.AppendFormat("SET @contactDispositionID = (SELECT CONTACT_DISPOSITION_ID FROM ChmChurch.dbo.CONTACT_DISPOSITION WITH (NOLOCK) WHERE CHURCH_ID = {0} AND CONTACT_DISPOSITION_NAME = '{1}') ", churchId, contactDispositionName);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.CONTACT_DISPOSITION_APPLICATION WHERE CONTACT_DISPOSITION_ID = @contactDispositionID AND APPLICATION_ID IN (0, 17) AND CHURCH_ID = {0} ", churchId);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.CONTACT_DISPOSITION WHERE CONTACT_DISPOSITION_ID = @contactDispositionID AND CHURCH_ID = {0}", churchId);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method creates a department.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="departmentName">The name of the department.</param>
        /// <param name="departmentCode">The code of the department.</param>
        public void Admin_DepartmentsCreate(int churchId, string departmentName, string departmentCode) {
            if (departmentCode == string.Empty) {
                this.Execute(string.Format("INSERT INTO ChmChurch.dbo.DEPT (CHURCH_ID, DEPT_NAME) VALUES ({0}, '{1}')", churchId, departmentName));
            }
            else {
                this.Execute(string.Format("INSERT INTO ChmChurch.dbo.DEPT (CHURCH_ID, DEPT_NAME, DEPT_CODE) VALUES ({0}, '{1}', '{2}')", churchId, departmentName, departmentCode));
            }
        }

        /// <summary>
        /// This method deletes a department.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="departmentName">The name of the department.</param>
        public void Admin_DepartmentsDelete(int churchId, string departmentName) {
            StringBuilder query = new StringBuilder();
            query.Append("DECLARE @deptName varchar(255), @deptId int, @userId int, @churchId int ");
            query.AppendFormat("SET @deptName = '{0}' ", departmentName);
            query.AppendFormat("SET @churchId = {0} ", churchId);
            query.Append("SELECT @deptId = DEPT_ID FROM  ChmChurch.dbo.DEPT WHERE CHURCH_ID = @churchId AND DEPT_NAME = @deptName ");

            query.Append("DELETE FROM ChmChurch.dbo.USER_ROLE WHERE user_id in (SELECT user_id FROM ChmChurch.dbo.USERS WHERE DEPT_ID = @deptId) ");
            query.Append("DELETE FROM ChmChurch.dbo.USERS WHERE DEPT_ID = @deptId ");
            query.Append("DELETE FROM ChmChurch.dbo.DEPT WHERE CHURCH_ID = @churchId AND DEPT_NAME = @deptName ");
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method creates a head count attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="headCountAttributeName">The name of the head count attribute.</param>
        /// <param name="outside">Flag for outside.</param>
        public void Admin_HeadCountAttributes_Create(int churchId, string headCountAttributeName, bool outside) {
            this.Execute(string.Format("INSERT INTO ChmActivity.dbo.ACTIVITY_ATTRIBUTE (CHURCH_ID, ACTIVITY_ATTRIBUTE_NAME, IS_PERIPHERAL) VALUES({0}, '{1}', {2})", churchId, headCountAttributeName, Convert.ToInt16(outside)));
        }

        /// <summary>
        /// This method deletes a head count attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="headCountAttributeName">The head count attribute name.</param>
        public void Admin_HeadCountAttributes_Delete(int churchId, string headCountAttributeName) {
            this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_ATTRIBUTE WHERE CHURCH_ID = {0} AND ACTIVITY_ATTRIBUTE_NAME = '{1}'", churchId, headCountAttributeName));
        }

        /// <summary>
        /// This method creates an individual attribute group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualAttributeGroupName">The name of the individual attribute group.</param>
        /// <param name="enabled">Flag for enabled.</param>
        public void Admin_IndividualAttributeGroups_Create(int churchId, string individualAttributeGroupName, bool enabled) {
            this.Execute(string.Format("INSERT INTO ChmPeople.dbo.ATTRIBUTE_GROUP (CHURCH_ID, ATTRIBUTE_GROUP_NAME, ENABLED) VALUES({0}, '{1}', {2})", churchId, individualAttributeGroupName, Convert.ToInt16(enabled)));
        }

        /// <summary>
        /// This method deletes an individual attribute group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualAttributeGroupName">The name of the individual attribute group.</param>
        public void Admin_IndividualAttributeGroups_Delete(int churchId, string individualAttributeGroupName) {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.ATTRIBUTE_GROUP WHERE CHURCH_ID = {0} AND ATTRIBUTE_GROUP_NAME = '{1}'", churchId, individualAttributeGroupName));
        }

        /// <summary>
        /// This method get an attribute group id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualAttributeGroupName">The name of the individual attribute group.</param>
        public int Admin_Fetch_AttributeGroupID(int churchId, string individualAttributeGroupName)
        {
            string query = string.Format("SELECT ATTRIBUTE_GROUP_ID FROM ChmPeople.dbo.ATTRIBUTE_GROUP WITH(NOLOCK) WHERE CHURCH_ID = {0} AND ATTRIBUTE_GROUP_NAME = '{1}'", churchId, individualAttributeGroupName);
            return Convert.ToInt32(this.Execute(query).Rows[0]["ATTRIBUTE_GROUP_ID"].ToString());
        }

        /// <summary>
        /// This method get an attribute id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualAttributeGroupName">The name of the individual attribute group.</param>
        public int Admin_Fetch_AttributeID(int churchId, int attributeGroupId, string attributeName)
        {
            string query = string.Format("select top 1 ATTRIBUTE_ID from ChmPeople..ATTRIBUTE with(nolock) where church_id={0} and attribute_group_id={1} and attribute_name='{2}'", churchId, attributeGroupId, attributeName);
            return Convert.ToInt32(this.Execute(query).Rows[0]["ATTRIBUTE_ID"].ToString());
        }

        /// <summary>
        /// This method creates an individual attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="attributeGroupName">The name of the attribute group the attribute will be tied to.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="hasStaff">Flag for staff.</param>
        /// <param name="hasStartDate">Flag for start date.</param>
        /// <param name="hasEndDate">Flag for end date.</param>
        /// <param name="hasComment">Flag for comment.</param>
        /// <param name="enabled">Flag for enabled.</param>
        public void Admin_IndividualAttributes_Create(int churchId, string attributeGroupName, string attributeName, bool hasStaff, bool hasStartDate, bool hasEndDate, bool hasComment, bool enabled) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.ATTRIBUTE (CHURCH_ID, ATTRIBUTE_GROUP_ID, ATTRIBUTE_NAME, HAS_STAFF, HAS_START_DATE, HAS_END_DATE, HAS_COMMENT, ENABLED) ");
            query.AppendFormat("SELECT {0}, ATTRIBUTE_GROUP_ID, '{1}', {2}, {3}, {4}, {5}, {6} FROM ChmPeople.dbo.ATTRIBUTE_GROUP WITH (NOLOCK) WHERE CHURCH_ID = {7} AND ATTRIBUTE_GROUP_NAME = '{8}'", churchId, attributeName, Convert.ToInt16(hasStaff), Convert.ToInt16(hasStartDate), Convert.ToInt16(hasEndDate), Convert.ToInt16(hasComment), Convert.ToInt16(enabled), churchId, attributeGroupName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an individual attribute.
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="attributeGroupName"></param>
        /// <param name="attributeName"></param>
        public void Admin_IndividualAttributes_Delete(int churchId, string attributeGroupName, string attributeName) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmPeople.dbo.ATTRIBUTE WHERE CHURCH_ID = {0} AND ", churchId);
            query.AppendFormat("ATTRIBUTE_GROUP_ID = (SELECT TOP 1 ATTRIBUTE_GROUP_ID FROM ChmPeople.dbo.ATTRIBUTE_GROUP WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ATTRIBUTE_GROUP_NAME = '{1}') ", churchId, attributeGroupName);
            query.AppendFormat("AND ATTRIBUTE_NAME = '{0}'", attributeName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method creates a job attribute group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="jobAttributeGroupName">The name of the job attribute group.</param>
        public void Admin_JobAttributeGroups_Create(int churchId, string jobAttributeGroupName) {
            this.Execute(string.Format("INSERT INTO ChmActivity.dbo.JOB_ATTRIBUTE_GROUP (CHURCH_ID, JOB_ATTRIBUTE_GROUP_NAME, CREATED_DATE) VALUES({0}, '{1}', CURRENT_TIMESTAMP)", churchId, jobAttributeGroupName));
        }

        /// <summary>
        /// This method deletes a job attribute group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="jobAttributeGroupName">The name of the job attribute group.</param>
        public void Admin_JobAttributeGroups_Delete(int churchId, string jobAttributeGroupName) {
            this.Execute(string.Format("DELETE FROM ChmActivity.dbo.JOB_ATTRIBUTE_GROUP WHERE CHURCH_ID = {0} AND JOB_ATTRIBUTE_GROUP_NAME = '{1}'", churchId, jobAttributeGroupName));
        }

        /// <summary>
        /// This method creates a job attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="jobAttributeGroupName">The name of the job attribute group the job atribute will belong to.</param>
        /// <param name="jobAttributeName">The name of the job attribute.</param>
        /// <param name="userId">The user id who created the job attribute.</param>
        public void Admin_JobAttributes_Create(int churchId, string jobAttributeGroupName, string jobAttributeName, int userId) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.JOB_ATTRIBUTE (JOB_ATTRIBUTE_GROUP_ID, CHURCH_ID, JOB_ATTRIBUTE_NAME, SORT, CREATED_DATE, CREATED_BY_USER_ID) ");
            query.AppendFormat("SELECT JOB_ATTRIBUTE_GROUP_ID, {0}, '{1}', 1, CURRENT_TIMESTAMP, {2} FROM ChmActivity.dbo.JOB_ATTRIBUTE_GROUP WITH (NOLOCK) WHERE CHURCH_ID = {0} AND JOB_ATTRIBUTE_GROUP_NAME = '{3}'", churchId, jobAttributeName, userId, jobAttributeGroupName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a job attribute
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="jobAttributeGroupName"></param>
        /// <param name="jobAttributeName"></param>
        public void Admin_JobAttributes_Delete(int churchId, string jobAttributeGroupName, string jobAttributeName) {
            this.Execute("DELETE FROM ChmActivity.dbo.JOB_ATTRIBUTE WHERE JOB_ATTRIBUTE_GROUP_ID = (SELECT TOP 1 JOB_ATTRIBUTE_GROUP_ID FROM ChmActivity.dbo.JOB_ATTRIBUTE_GROUP WITH (NOLOCK) WHERE CHURCH_ID = 15 AND JOB_ATTRIBUTE_GROUP_NAME = '') AND CHURCH_ID = 15 AND JOB_ATTRIBUTE_NAME = ''");
        }

        /// <summary>
        /// This method creates a room.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="roomCode">The room code.</param>
        /// <param name="buildingName">The name of the builing the room will be tied to.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <param name="roomDescription">The description for the room.</param>
        public void Admin_RoomsCreate(int churchId, string roomCode, string buildingName, string roomName, string roomDescription) {
            StringBuilder query = new StringBuilder("DECLARE @buildingID INT ");
            query.AppendFormat("SET @buildingID = {0} ", Convert.ToInt32(this.Execute(string.Format("SELECT TOP 1 BUILDING_ID FROM ChmChurch.dbo.BUILDING WHERE CHURCH_ID = {0} AND BUILDING_NAME = '{1}'", churchId, buildingName)).Rows[0]["BUILDING_ID"]));
            query.AppendFormat("INSERT INTO ChmChurch.dbo.ROOM (CHURCH_ID, BUILDING_ID, ROOM_CODE, ROOM_DESC, ROOM_NAME) VALUES({0}, @buildingID, '{1}', '{2}', '{3}')", churchId, roomCode, roomDescription, roomName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a room.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="roomName">The room name.</param>
        public void Admin_RoomsDelete(int churchId, string roomName) {
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.ROOM WHERE CHURCH_ID = {0} AND ROOM_NAME = '{1}'", churchId, roomName));
        }

        /// <summary>
        /// This method creates a school.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="schoolName">The name of the school.</param>
        /// <param name="schoolTypeId">The type for the school.</param>
        public void Admin_Schools_Create(int churchId, string schoolName, int schoolTypeId) {
            this.Execute(string.Format("INSERT INTO ChmPeople.dbo.SCHOOL (CHURCH_ID, SCHOOL_NAME, SCHOOL_TYPE_ID) VALUES({0}, '{1}', {2})", churchId, schoolName, schoolTypeId));
        }

        /// <summary>
        /// This method deletes a school.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="schoolName">The name of the school.</param>
        public void Admin_Schools_Delete(int churchId, string schoolName) {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.SCHOOL WHERE CHURCH_ID = {0} AND SCHOOL_NAME = '{1}'", churchId, schoolName));
        }

        /// This method creates a contact form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the contact form.</param>
        /// <param name="isActive">Flag designating active status for the form.</param>
        public void Admin_FormNamesCreate(int churchId, string formName, bool isActive) {
            this.Execute(string.Format("INSERT INTO ChmChurch.dbo.CONTACT_TYPE (CHURCH_ID, CONTACT_TYPE_NAME, IS_ACTIVE) VALUES({0}, '{1}', {2})", churchId, formName, Convert.ToInt16(isActive)));
        }

        /// <summary>
        /// This method deletes a contact form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the contact form.</param>
        public void Admin_FormNamesDelete(int churchId, string formName) {
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.CONTACT_TYPE WHERE CHURCH_ID = {0} AND CONTACT_TYPE_NAME = '{1}'", churchId, formName));
        }

        /// <summary>
        /// This method deletes a user.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        public void Admin_Users_Delete(int churchId, string login) {
            this.Execute(string.Format("DELETE FROM ChmChurch.dbo.USERS WHERE CHURCH_ID = {0} AND LOGIN = '{1}'", churchId, login));
        }

        /// <summary>
        /// Set a User to Active or InActive (
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <param name="login">Portal Login</param>
        /// <param name="enableStatus">Enable Status (0 = Active or 1 = InActive)</param>
        public void Admin_Set_User_ActiveInActive(int churchId, string login, int enableStatus)
        {
            this.Execute(string.Format("UPDATE [ChmChurch].[dbo].[USERS] SET ENABLED = {0} WHERE CHURCH_ID = {1} AND LOGIN = '{2}'", enableStatus, churchId, login));
        }

        /// <summary>
        /// Sets a User failed login attempt retry count
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="login"></param>
        /// <param name="retryCount"></param>
        public void Admin_Set_User_RetryCount(int churchId, string login, int retryCount)
        {
            this.Execute(string.Format("UPDATE [ChmChurch].[dbo].[USERS] SET RetryCount = {0} WHERE CHURCH_ID = {1} AND LOGIN = '{2}';", retryCount, churchId, login));
        }

        /// <summary>
        /// Sets the Last failed login time for the user
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="login"></param>
        /// <param name="dateTime"></param>
        public void Admin_Set_User_LastFailLoginDate(int churchId, string login, string dateTime)
        {
            this.Execute(string.Format("UPDATE [ChmChurch].[dbo].[USERS] SET LastFailLoginDate = '{0}' WHERE CHURCH_ID = {1} AND LOGIN = '{2}';", dateTime, churchId, login));
        }

        /// <summary>
        /// This method deletes all security roles for a user based the specified security group id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <param name="securityGroupId">The security type id.</param>
        public void Admin_Users_DeleteSecurityRolesBySecurityGroup(int churchId, string login, int securityGroupId) {
            StringBuilder query = new StringBuilder("DECLARE @roleID INT ");
            query.AppendFormat("SET @roleID = {0} ", this.FetchUserSecurityRole(churchId, login));

            query.Append("DELETE r_s_t FROM ChmChurch.dbo.ROLE_SECURITY_TYPE r_s_t WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmChurch.dbo.SECURITY_TYPE s_t WITH (NOLOCK) ");
            query.Append("ON r_s_t.SECURITY_TYPE_ID = s_t.SECURITY_TYPE_ID ");
            query.AppendFormat("WHERE ROLE_ID = @roleID AND SECURITY_GROUP_ID = {0}", securityGroupId);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method creates a user security type for the given login.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <param name="securityGroupId">The security group id.</param>
        /// <param name="securityTypeName">The security type name.</param>
        public void Admin_Users_CreateSecurityRoles(int churchId, string login, int securityGroupId, string[] securityTypeNames) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @roleID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @roleID = {0} ", this.FetchUserSecurityRole(churchId, login));

            foreach (string securityTypeName in securityTypeNames) {
                StringBuilder insertQuery = new StringBuilder();
                insertQuery.Append(query);
                insertQuery.Append("INSERT INTO ChmChurch.dbo.ROLE_SECURITY_TYPE (ROLE_ID, SECURITY_TYPE_ID) ");
                insertQuery.AppendFormat("SELECT @roleID, SECURITY_TYPE_ID FROM ChmChurch.dbo.SECURITY_TYPE WHERE SECURITY_GROUP_ID = {0} AND CHURCH_ID IN (0, @churchID) AND SECURITY_TYPE_NAME = '{1}'", securityGroupId, securityTypeName);
                this.Execute(insertQuery.ToString());
            }
        }

        /// <summary>
        /// Creates a status.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="statusGroupName">The status group this status will belong to (Member, Attendee, Inactive, Deceased, System)</param>
        /// <param name="statusName">The name of the status</param>
        /// <param name="enabled">Is the status enabled?</param>
        public void Admin_Status_Create(int churchId, string statusGroupName, string statusName, bool enabled) {
            var active = enabled == true ? 1 : 0;
            var statusGroupID = 0;
            switch (statusGroupName) {
                case "Member":
                    statusGroupID = 4;
                    break;
                case "Attendee":
                    statusGroupID = 5;
                    break;
                case "Inactive":
                    statusGroupID = 6;
                    break;
                case "Deceased":
                    statusGroupID = 101;
                    break;
                case "System":
                    statusGroupID = 102;
                    break;
                default:
                    throw new Exception(string.Format("Unknown status group name {0} ", statusGroupName));
            }

            StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.Status (church_id, status_group_id, status_name, enabled) ");
            query.Append(string.Format("VALUES ({0}, {1}, '{2}', {3})", churchId, statusGroupID, statusName, active));

            this.Execute(query.ToString());
        }
               
        /// <summary>
        /// Deletes a status.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="statusName">The name of the status.</param>
        public void Admin_Status_Delete(int churchId, string statusName) {
            StringBuilder query = new StringBuilder("DELETE FROM ChmPeople.dbo.Status ");
            query.Append(string.Format("WHERE CHURCH_ID = {0} and STATUS_NAME = '{1}'", churchId, statusName));

            this.Execute(query.ToString());
        }

        /// <summary>
        /// Fetches the role id for a given church id and login.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The user login.</param>
        /// <returns>The role id for the user.</returns>
        private int FetchUserSecurityRole(int churchId, string login) {
            StringBuilder query = new StringBuilder("SELECT u_r.ROLE_ID FROM ChmChurch.dbo.USERS usr WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmChurch.dbo.USER_ROLE u_r WITH (NOLOCK) ");
            query.Append("ON usr.USER_ID = u_r.USER_ID ");
            query.AppendFormat("WHERE usr.CHURCH_ID = {0} AND LOGIN = '{1}' AND ENABLED = 1", churchId, login);
            return Convert.ToInt32(this.Execute(query.ToString()).Rows[0]["ROLE_ID"]);
        }

        //Fetch Login User First Name and Last Name
        public string FetchUserName(int churchId, string login)
        {
            DataTable dt = this.Execute(string.Format("SELECT TOP 1 FIRST_NAME, LAST_NAME FROM ChmChurch.dbo.USERS  WITH (NOLOCK) WHERE CHURCH_ID = {0} AND LOGIN = '{1}'", churchId, login));
            string userName = string.Format("{0} {1}", dt.Rows[0]["FIRST_NAME"], dt.Rows[0]["LAST_NAME"]);
            return userName;

        }

        /// <summary>
        /// Creates a status with Specified characters.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="statusGroupName">The status group this status will belong to (Member, Attendee, Inactive, Deceased, System)</param>
        /// <param name="statusName">The name of the status</param>
        /// <param name="enabled">Is the status enabled?</param>
        /// created by ivan.zhang
        public void Admin_Status_Create_SpecCharater(int churchId, string statusGroupName, string statusName, bool enabled)
        {
            var active = enabled == true ? 1 : 0;
            var statusGroupID = 0;
            switch (statusGroupName)
            {
                case "Member":
                    statusGroupID = 4;
                    break;
                case "Attendee":
                    statusGroupID = 5;
                    break;
                case "Inactive":
                    statusGroupID = 6;
                    break;
                case "Deceased":
                    statusGroupID = 101;
                    break;
                case "System":
                    statusGroupID = 102;
                    break;
                default:
                    throw new Exception(string.Format("Unknown status group name {0} ", statusGroupName));
            }

            //StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.Status (church_id, status_group_id, status_name, enabled) ");
            //query.Append(string.Format("VALUES ({0}, {1}, '{2}', {3})", churchId, statusGroupID, statusName, active));

            List<SqlParameter> paramlist = new List<SqlParameter>();
            StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.Status (church_id, status_group_id, status_name, enabled) ");
            query.Append("VALUES (@ChurchID, @StatusGroupID, @StatusName, @Enabled)");

            paramlist.Add(new SqlParameter() { ParameterName = "@ChurchID", Value = churchId, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@StatusGroupID", Value = statusGroupID, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@StatusName", Value = statusName, DbType = DbType.String });
            paramlist.Add(new SqlParameter() { ParameterName = "@Enabled", Value = enabled, DbType = DbType.Boolean });

            this.ExecuteNonQuery(query.ToString(), paramlist.ToArray());
        }

        /// <summary>
        /// Deletes a status with Specified characters.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="statusName">The name of the status.</param>
        /// created by ivan.zhang
        public void Admin_Status_Delete_SpecCharater(int churchId, string statusName)
        {
            //StringBuilder query = new StringBuilder("DELETE FROM ChmPeople.dbo.Status ");
            //query.Append(string.Format("WHERE CHURCH_ID = {0} and STATUS_NAME = '{1}'", churchId, statusName));

            //this.Execute(query.ToString());

            List<SqlParameter> paramlist = new List<SqlParameter>();
            StringBuilder query = new StringBuilder("DELETE FROM ChmPeople.dbo.Status ");
            query.Append("WHERE CHURCH_ID = @ChurchID and STATUS_NAME = @StatusName");

            paramlist.Add(new SqlParameter() { ParameterName = "@ChurchID", Value = churchId, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@StatusName", Value = statusName, DbType = DbType.String });

            this.ExecuteNonQuery(query.ToString(), paramlist.ToArray());
        }

        /// <summary>
        /// Executes a SQL query.
        /// </summary>
        /// <param name="query">String representing the query to be executed.</param>
        /// <returns>Return an int value</returns>
        /// created by ivan.zhang
        public int ExecuteNonQuery(string query, params SqlParameter[] args)
        {
            log.Debug("Enter Execute " + query);

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                try
                {
                    dbConnection.Open();
                    var dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = query;
                    if (args != null && args.Length > 0)
                    {
                        dbCommand.Parameters.AddRange(args);
                    }

                    return dbCommand.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error in executing query: {0} {1}", query, e.Message));
                    dbConnection.Close();
                    throw new Exception(e.StackTrace);
                }
            }


        }

        /// <summary>
        /// Fetches the church id for a given church code
        /// </summary>
        /// <param name="churchCode">Church Code</param>
        /// <returns>The church id</returns>
        public int FetchChurchID(string churchCode)
        {
            StringBuilder query = new StringBuilder("SELECT [CHURCH_ID] FROM [ChmChurch].[dbo].[CHURCH] WITH (NOLOCK) ");
            query.AppendFormat("WHERE CHURCH_CODE = '{0}' ", churchCode);

            return (int)this.Execute(query.ToString()).Rows[0]["CHURCH_ID"];

        }
        /// <summary>
        /// Fetches the church code for a given church id
        /// </summary>
        /// <param name="churchCode">Church Code</param>
        /// <returns>The church id</returns>
        public string FetchChurchCode(int churchId)
        {
            StringBuilder query = new StringBuilder("SELECT [CHURCH_CODE] FROM [ChmChurch].[dbo].[CHURCH] WITH (NOLOCK) ");
            query.AppendFormat("WHERE CHURCH_ID = {0} ", churchId);

            return (string)this.Execute(query.ToString()).Rows[0]["CHURCH_CODE"];

        }

        /// <summary>
        /// Fetches the church name by church code
        /// </summary>
        /// <param name="churchCode">Church Code</param>
        /// <returns>The church Name</returns>
        public string FetchChurchName(string churchCode)
        {
            StringBuilder query = new StringBuilder("SELECT church_name FROM ChmChurch..CHURCH WITH (NOLOCK) ");
            query.AppendFormat("WHERE CHURCH_CODE = '{0}' ", churchCode);

            return (string)this.Execute(query.ToString()).Rows[0]["CHURCH_NAME"]; 

        }

        /// <summary>
        /// This method deletes a volunteer type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="volunteerTypeName">The name of the volunteer type.</param>
        public void Admin_VolunteerTypes_Delete(int churchId, string volunteerTypeName) {
            try
            {
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_TYPE WHERE CHURCH_ID = {0} AND INDIVIDUAL_TYPE_NAME = '{1}'", churchId, volunteerTypeName);
            }
            catch (System.Exception e)
            {
                int individualTypeId;
                StringBuilder query = new StringBuilder("select individual_type_id FROM ChmActivity.dbo.INDIVIDUAL_TYPE ");
                query.AppendFormat("WHERE CHURCH_ID = {0} ", churchId);
                query.AppendFormat("AND INDIVIDUAL_TYPE_NAME = '{0}' ", volunteerTypeName);

                individualTypeId = (int)this.Execute(query.ToString()).Rows[0]["individual_type_id"];

                this.Execute("DELETE FROM ChmActivity.dbo.staffing_pref WHERE CHURCH_ID = {0} AND individual_type_id = {1}", churchId, individualTypeId);
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_TYPE WHERE CHURCH_ID = {0} AND INDIVIDUAL_TYPE_NAME = '{1}'", churchId, volunteerTypeName);
            }
        }
        #endregion Admin

        #region Giving
        /// <summary>
        /// Polls the database checking for a range of reason codes for a payment based on the payment type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentAmount">The amount of the payment.</param>
        /// <param name="dateTimeFrom">The starting date range.</param>
        /// <param name="clientApplication">The application that generated the payment.</param>
        /// <param name="paymentType">The type of payment.</param>
        /// <returns>The payment status.</returns>
        public int Giving_WaitUntilPaymentProcessed(int churchId, double paymentAmount, DateTime dateTimeFrom, string clientApplication, int paymentType, GeneralMethods generalMethods = null) {
            // Find the payment
            StringBuilder query = new StringBuilder();
            query.Append("SELECT TOP 1 pay.PaymentID FROM ChmContribution.dbo.Payment pay WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.PaymentInformation payinf WITH (NOLOCK) ");
            query.Append("ON pay.PaymentInformationID = payinf.PaymentInformationID ");
            query.AppendFormat("WHERE pay.ChurchID = {0} AND pay.Amount = {1} AND pay.CreatedDate > '{2}' AND pay.ClientApplication = '{3}' AND payinf.PaymentTypeID = {4} ", churchId, paymentAmount, dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss"), clientApplication, paymentType);
            query.Append("ORDER BY pay.CreatedDate DESC");

            int paymentId = (int)this.Execute(query.ToString()).Rows[0]["PaymentID"];
            int paymentStatusId = int.MinValue;
            List<int> expectedPaymentStatusIds = (paymentType == 998 || paymentType == 999) ? new List<int> { 14, 15 } : new List<int> { 5, 7, 8, 17, 18 }; // If non-transactional, wait for status of 14 or 15, if transactional, wait for status of 5, 7, or 8

            // Wait until the payment reason code matches one of the expected types
            Retry.WithPolling(5000).WithTimeout(360000).WithFailureMessage("The payment did not process in the allotted time.")
                .DoBetween(() => paymentStatusId = Giving_GetPaymentStatusID(churchId, paymentId, generalMethods, clientApplication)
                                 //Convert.ToInt16(this.Execute("SELECT PaymentStatusID FROM ChmContribution.dbo.Payment WITH (NOLOCK) WHERE ChurchID = {0} AND PaymentID = {1}", churchId, paymentId).Rows[0]["PaymentStatusID"])                                 
                                 //this._generalMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.View_Submissions);
                                 )
                .Until(() => expectedPaymentStatusIds.Contains(paymentStatusId));

            // Return the payment status id
            return paymentStatusId;
        }

        public int Giving_GetPaymentStatusID(int churchId, int paymentId, GeneralMethods generalMethods, string clientApplication)
        {
            if( (generalMethods != null) && (!clientApplication.Contains("Infellowship")) )
            generalMethods.Navigate_Portal(Navigation.Portal.WebLink.Event_Registration.View_Submissions);

            //Wait a few before getting pmt status id
            Thread.Sleep(TimeSpan.FromSeconds(5));

            try
            {
                string statusQuery = string.Format("SELECT PaymentStatusID FROM ChmContribution.dbo.Payment WITH (NOLOCK) WHERE ChurchID = {0} AND PaymentID = {1}", churchId, paymentId);
                DataTable dt = this.Execute(statusQuery);
                return Convert.ToInt16(dt.Rows[0]["PaymentStatusID"]);
            }
            catch (System.IndexOutOfRangeException)
            {
                //Log error and return neg value. Looks like this method is used with a retry polling
                throw new Exception(string.Format("Error getting Payment Status for ChurchID = {0} AND PaymentID = {1}", churchId, paymentId));
                //return -1;
            }
        }

        /// <summary>
        /// Returns the payment status id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentAmount">The amount of the payment.</param>
        /// <param name="dateTimeFrom">The starting date range.</param>
        /// <param name="clientApplication">The application that generated the payment.</param>
        /// <param name="paymentType">The type of payment.</param>
        /// <returns>The payment status.</returns>
        public int Giving_GetPaymentStatusID(int churchId, double paymentAmount, DateTime dateTimeFrom, string clientApplication, int paymentType) {
            StringBuilder query = new StringBuilder();
            int paymentId = -1;
            string statusQuery = string.Empty;

            query.Append("SELECT TOP 1 pay.PaymentID FROM ChmContribution.dbo.Payment pay WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.PaymentInformation payinf WITH (NOLOCK) ");
            query.Append("ON pay.PaymentInformationID = payinf.PaymentInformationID ");
            query.AppendFormat("WHERE pay.ChurchID = {0} AND pay.Amount = {1} AND pay.CreatedDate > '{2}' AND pay.ClientApplication = '{3}' AND payinf.PaymentTypeID = {4} ", churchId, paymentAmount, dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss"), clientApplication, paymentType);
            query.Append("ORDER BY pay.CreatedDate DESC");

            //Get Payment ID
            try
            {
                //Wait a few before getting pmt id
                Thread.Sleep(TimeSpan.FromSeconds(5));
                paymentId = (int)this.Execute(query.ToString()).Rows[0]["PaymentID"];
            }
            catch (System.IndexOutOfRangeException)
            {
                TestLog.WriteLine(string.Format("Error getting payment id using following query: {0}", query.ToString()));
                throw new Exception(string.Format("Error getting payment id using following query: {0}", query.ToString()));
            }


            //Return Payment Status ID
            try
            {
                statusQuery = string.Format("SELECT PaymentStatusID FROM ChmContribution.dbo.Payment WITH (NOLOCK) WHERE ChurchID = {0} AND PaymentID = {1}", churchId, paymentId);
                DataTable dt = this.Execute(statusQuery);
                return Convert.ToInt16(dt.Rows[0]["PaymentStatusID"]);
            }
            catch (System.IndexOutOfRangeException)
            {
                //Log error and return neg value. Looks like this method is used with a retry polling
                TestLog.WriteLine(string.Format("Error getting Payment Status using following query: {0}", statusQuery));
                throw new Exception(string.Format("Error getting Payment Status for ChurchID = {0} AND PaymentID = {1}", churchId, paymentId));
                //return -1;
            }

        }

        /// <summary>
        /// This method deletes the non-transactional contribution receipts.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id the receipts are tied to.</param>
        /// <param name="householdId">The household id the receipts are tied to.</param>
        public void Giving_DeleteNonTransactionalContributionReceipts(int churchId, int? individualId, int? householdId) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} ", churchId);

            if (individualId.HasValue) {
                query.AppendFormat("AND INDIVIDUAL_ID = {0} ", individualId);
            }

            if (householdId.HasValue) {
                query.AppendFormat("AND HOUSEHOLD_ID = {0} ", householdId);
            }

            query.Append("AND ISNUMERIC(ACCOUNT_REFERENCE) = 1");
            this.Execute(query.ToString());

            /* MIGHT NEED TO INCORPORATE THIS 
            DELETE dbo.PaymentContributionReceipt WHERE ContributionReceiptID IN (select ContributionReceiptID from dbo.Contribution_Receipt where Church_ID = 15 and Household_ID = 17201018 and ISNUMERIC(Account_Reference) = 1 )
             */


        }

        /// <summary>
        /// This method deletes schedule giving information.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="login">The login the giving is tied to.</param>
        public void Giving_DeleteScheduledGivingByChurchAndLogin(int churchId, string login) {
            StringBuilder query = new StringBuilder();
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method Inactivate schedule giving by individual first name and last name.
        /// </summary>
        /// <param name="first_name">Individual's first name.</param>
        /// <param name="login">individual's last name.</param>
        public void Giving_InactivateScheduledGivingByName(string firstName, string lastName)
        {
            StringBuilder query = new StringBuilder("update chmContribution.dbo.ScheduledGiving set IsActive = 0 where IndividualID in (");
            query.AppendFormat("select individual_id from chmPeople.dbo.individual where first_name = '{0}' and last_name = '{1}')", firstName, lastName);
            this.Execute(query.ToString());
        }
        /// <summary>
        /// This method calculates the total amount of contributions made on behalf an individual or household.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="householdID">The household id.</param>
        /// <param name="individualID">The individual id.</param>
        /// <param name="receivedDateFrom">The starting date to calculate the total from.</param>
        /// <returns>The total amount of contributions for the given starting date.</returns>
        public double Giving_GetContributionReceiptTotalForIndividualOrHousehold(int churchId, int? householdID, int? individualID, string receivedDateFrom) {
            DataTable dt;
            StringBuilder query = new StringBuilder("SELECT SUM(AMOUNT) FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT cr WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.FUND f WITH (NOLOCK) ");
            query.Append("ON f.FUND_ID = cr.FUND_ID ");

            if (individualID > 0) {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND INDIVIDUAL_ID = {2} AND RECEIVED_DATE > '{3}'", churchId, householdID, individualID, receivedDateFrom);
            }
            else if (individualID == int.MinValue) {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND RECEIVED_DATE > '{2}'", churchId, householdID, receivedDateFrom);
            }
            else {
                query.AppendFormat("WHERE f.FUND_TYPE = 1 AND cr.CHURCH_ID = {0} AND HOUSEHOLD_ID = {1} AND INDIVIDUAL_ID IS NULL AND RECEIVED_DATE > '{2}'", churchId, householdID, receivedDateFrom);
            }

            TestLog.WriteLine("Giving_GetContributionReceiptTotalForIndividualOrHousehold QUERY: " + query.ToString());

            dt = this.Execute(query.ToString());

            if (!Convert.IsDBNull(dt.Rows[0][0]))
            {
                return Convert.ToDouble(dt.Rows[0][0]);
            }
            else
            {
                return Convert.ToDouble(0);
            }

        }

        /// <summary>
        /// This method calculates the total amount of contributions made on behalf an individual or household using stored proc
        /// </summary>
        /// <param name="churchID">Church ID</param>
        /// <param name="individualID">Individual ID</param>
        /// <param name="now">DateTime Now to calculate current start/end year dates</param>
        /// <param name="individualIDList">Individual ID List (i.e. "0, _individualID" "_individualID")</param>
        /// <returns>Total Amount</returns>
        public string Giving_GetContributionReceiptTotalForIndividualOrHousehold_Proc(int churchID, int individualID, DateTime now, string individualIDList = "")
        {
            string proc = "ChmContribution.dbo.Weblink_FetchGiving";
            SqlDataReader dr;
            decimal total = 0.00m;
            DateTime startRcvDate = new DateTime(now.Year, 1, 1);
            DateTime endRcvDate = new DateTime(now.Year, 12, 31);

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));
                    cmd.Parameters.Add(new SqlParameter("@IndividualID", individualID));
                    cmd.Parameters.Add(new SqlParameter("@StartReceivedDate", startRcvDate));
                    cmd.Parameters.Add(new SqlParameter("@EndReceivedDate", endRcvDate));
                    cmd.Parameters.Add(new SqlParameter("@IndividualIDList", individualIDList));
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        total = total + Convert.ToDecimal(dr["Amount"]);
                    }
                }

                dbConnection.Close();
            }

            //{0:C}
            string sumDecimal = total.ToString("#,##0.00");
            TestLog.WriteLine(string.Format("Amount Total: {0}", sumDecimal));

            return sumDecimal;

        }

        /// <summary>
        /// This method gets Organization name of which has multiple addresses
        /// </summary>
        /// <param name="churchID">Church ID</param>        
        public string Giving_GetMultipleAddressesOrganizationForSearchInContributorDetails(int churchID)
        {
            StringBuilder query = new StringBuilder("SELECT TOP 1 ChmPeople.dbo.HOUSEHOLD_ADDRESS.HOUSEHOLD_ID FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS ");
            query.Append("INNER JOIN ChmPeople.dbo.HOUSEHOLD ");
            query.Append("ON ChmPeople.dbo.HOUSEHOLD_ADDRESS.HOUSEHOLD_ID= ChmPeople.dbo.HOUSEHOLD.HOUSEHOLD_ID ");
            query.AppendFormat("WHERE ChmPeople.dbo.HOUSEHOLD.PARTY_TYPE_ID>1 AND ChmPeople.dbo.HOUSEHOLD.CHURCH_ID={0} ", churchID);
            query.Append("GROUP BY ChmPeople.dbo.HOUSEHOLD_ADDRESS.HOUSEHOLD_ID HAVING COUNT(*)>1 ");
            
            int household_id = Convert.ToInt32(this.Execute(query.ToString()).Rows[0]["HOUSEHOLD_ID"]);
            query.Clear();
            query.AppendFormat("SELECT ChmPeople.dbo.HOUSEHOLD.HOUSEHOLD_NAME FROM ChmPeople.dbo.HOUSEHOLD WHERE HOUSEHOLD_ID = {0} ", household_id);
            
            return this.Execute(query.ToString()).Rows[0][0].ToString();
        }
        
        /// <summary>
        /// This method fetches the most recent provider reply response code for the specified payment amount.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="paymentAmount">The amount of the payment.</param>
        /// <param name="dateTimeFrom">The starting date for the created date.</param>
        /// <param name="clientApplication">The client application that generated the payment.</param>
        /// <returns>The reason code for the payment.</returns>
        public int Giving_GetPaymentReasonCode(int churchId, double paymentAmount, DateTime dateTimeFrom, string clientApplication) {
            StringBuilder query = new StringBuilder("SELECT TOP 1 pay_rea.ReasonCode FROM ChmContribution.dbo.PaymentReason pay_rea WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.ProviderRequest pro_req WITH (NOLOCK) ");
            query.Append("ON pro_req.PaymentReasonID = pay_rea.PaymentReasonID ");
            query.Append("INNER JOIN ChmContribution.dbo.Payment pay WITH (NOLOCK) ");
            query.Append("ON pay.PaymentID = pro_req.PaymentID ");
            query.AppendFormat("WHERE pay.ChurchID = {0} AND pay.Amount = {1} AND pay.CreatedDate > '{2}' AND pay.ClientApplication = '{3}' ORDER BY pay.PaymentID DESC", churchId, paymentAmount, dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ss"), clientApplication);

            return Convert.ToInt16(this.Execute(query.ToString()).Rows[0]["ReasonCode"]);
        }

        /// <summary>
        /// This method fetches all of the fund names in the church tied to the specified merchant account.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="merchantAccountCode">The merchant account code.</param>
        /// <returns>Data table containing the names of the funds belonging to the merchant account, sorted in ascending order.</returns>
        public DataTable Giving_FetchFundNamesTiedToMerchantAccount(int churchId, string merchantAccountCode) {
            StringBuilder query = new StringBuilder("SELECT fund.FUND_NAME FROM ChmContribution.dbo.FUND fund WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmContribution.dbo.PP_ACCOUNT_REFERENCE acc_ref WITH (NOLOCK) ");
            query.Append("ON fund.PP_ACCOUNT_REFERENCE_ID = acc_ref.PP_ACCOUNT_REFERENCE_ID ");
            query.Append("INNER JOIN ChmContribution.dbo.PP_MERCHANT_ACCOUNT mer_acc WITH (NOLOCK) ");
            query.Append("ON acc_ref.PP_MERCHANT_ACCOUNT_ID = mer_acc.PP_MERCHANT_ACCOUNT_ID ");
            query.AppendFormat("WHERE fund.CHURCH_ID = {0} AND fund.IS_ACTIVE = 1 AND fund.IS_WEB_ACTIVE = 1 AND fund.FUND_TYPE IN (1, 2) AND mer_acc.PP_MERCHANT_ACCOUNT_CODE = '{1}' ", churchId, merchantAccountCode);
            query.Append("ORDER BY fund.FUND_NAME ASC");
            return this.Execute(query.ToString());
        }

        #region Account References
        /// <summary>
        /// This method creates an account reference.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="accountReferenceName">The name of the account reference.</param>
        /// <param name="description">The description of the account reference.</param>
        /// <param name="majorAccountCode">The major account code.</param>
        /// <param name="paymentGatewayId">The payment gateway id.</param>
        /// <param name="active">Flag for active.</param>
        public void Giving_AccountReferences_Create(int churchId, string accountReferenceName, string description, string majorAccountCode, string paymentGatewayId, bool active) {
            string column = string.Empty;
            string mac = string.Empty;
            StringBuilder query = new StringBuilder();
            query.AppendFormat("INSERT INTO ChmContribution.dbo.PP_ACCOUNT_REFERENCE (CHURCH_ID, ACCOUNT_CODE, ACCOUNT_DESC, {0} PP_MERCHANT_ACCOUNT_ID, IS_ACTIVE) ", column = string.IsNullOrEmpty(majorAccountCode) ? "" : "MAJOR_ACCOUNT_CODE,");
            query.AppendFormat("SELECT TOP 1 {0}, '{1}', '{2}',{3} PP_MERCHANT_ACCOUNT_ID, {4} FROM ChmContribution.dbo.PP_MERCHANT_ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND PP_MERCHANT_ACCOUNT_CODE = '{5}' AND IS_ACTIVE = 1", churchId, accountReferenceName, description, mac = string.IsNullOrEmpty(majorAccountCode) ? "" : string.Format("'{0}',", majorAccountCode), Convert.ToInt16(active), paymentGatewayId);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an account reference.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="accountReferenceName">The account reference name.</param>
        public void Giving_AccountReferences_Delete(int churchId, string accountReferenceName) {
            this.Execute(string.Format("DELETE FROM ChmContribution.dbo.PP_ACCOUNT_REFERENCE WHERE CHURCH_ID = {0} AND ACCOUNT_CODE = '{1}'", churchId, accountReferenceName));
        }
        #endregion Account References

        #region Accounts
        public void Giving_Accounts_Create(int churchId, int accountTypeId, int householdId, int? individualId, string account, [Optional] int? routingNumber) {
            string indVal;
            StringBuilder query = null;

            if (routingNumber != null) {
                query = new StringBuilder("INSERT INTO ChmContribution.dbo.ACCOUNT (CHURCH_ID, ACCOUNT_TYPE_ID, HOUSEHOLD_ID, INDIVIDUAL_ID, ACCOUNT, ROUTING_NO ) ");
                query.AppendFormat("VALUES({0}, {1}, {2}, {3}, '{4}', {5})", churchId, accountTypeId, householdId, indVal = individualId != null ? individualId.ToString() : "NULL", account, routingNumber);
            }

            else {
                query = new StringBuilder("INSERT INTO ChmContribution.dbo.ACCOUNT (CHURCH_ID, ACCOUNT_TYPE_ID, HOUSEHOLD_ID, INDIVIDUAL_ID, ACCOUNT ) ");
                query.AppendFormat("VALUES({0}, {1}, {2}, {3}, '{4}')", churchId, accountTypeId, householdId, indVal = individualId != null ? individualId.ToString() : "NULL", account);
            }
            
            this.Execute(query.ToString());
        }

        public void Giving_Accounts_Delete(int churchId, int accountTypeId, int individualId, string account) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} AND ACCOUNT_ID IN (SELECT ACCOUNT_ID FROM ChmContribution.dbo.ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} AND ACCOUNT_TYPE_ID = {2} AND ACCOUNT = '{3}') ", churchId, individualId, accountTypeId, account);
            query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT_TYPE_ID = {1} AND INDIVIDUAL_ID = {2} AND ACCOUNT = '{3}'", churchId, accountTypeId, individualId, account);

            this.Execute(query.ToString());
        }
        #endregion Accounts

        #region Batches
        /// <summary>
        /// This method authorizes a credit card batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="individualId">The individual id who is authorizing the batch.</param>
        public void Giving_Batches_Authorize(int churchId, string batchName, int individualId) {
            using (SqlConnection conn = new SqlConnection(this._dbConnectionString)) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("ChmContribution.dbo.BatchContribution_SaveBatchForProcessing", conn)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChurchID", churchId);
                    cmd.Parameters.AddWithValue("@BatchID", this.Execute(string.Format("SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = 2", churchId, batchName)).Rows[0][0]);
                    cmd.Parameters.AddWithValue("@CreatedByIndividualID", individualId);
                    cmd.ExecuteReader();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// This method creates a batch (regular or credit card).
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="batchAmount">The amount of the batch.</param>
        /// <param name="batchTypeId">The type of the batch.</param>
        public void Giving_Batches_Create(int churchId, string batchName, double batchAmount, int batchTypeId, [Optional, DefaultParameterValue(99)] int batchStatusId) {
            int batchStatusIdDB = 0;
            if (batchStatusId == 99) {
                batchStatusIdDB = batchTypeId == 2 ? 1 : 0;
            }
            else {
                batchStatusIdDB = batchStatusId;
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("INSERT INTO ChmContribution.dbo.BATCH(CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, CREATED_DATE, BatchStatusID,{0} BatchTypeID) ", batchTypeId == 2 ? " ReceivedDate," : "");
            query.AppendFormat("VALUES({0}, '{1}', CURRENT_TIMESTAMP, {2}, CURRENT_TIMESTAMP, {3},{4} {5}) ", churchId, batchName, batchAmount, batchStatusIdDB, batchTypeId == 2 ? "CURRENT_TIMESTAMP," : "", batchTypeId);
            if (batchTypeId == 2) {
                query.Append("DECLARE @batchID INT ");
                query.Append("SET @batchID = (SELECT SCOPE_IDENTITY()) ");
                query.Append("INSERT INTO ChmContribution.dbo.BatchHistory(ChurchID, BatchID, CreatedDate, CreatedByUserID, CreatedByLogin, BatchHistoryNote, CreatedByIndividualID) ");
                query.AppendFormat("VALUES({0}, @batchID, CURRENT_TIMESTAMP, 65211, 'chm_system', 'created <em>{1}</em> totaling <span class=''text_green''>{2:c}</span>.', {3})", churchId, batchName, batchAmount, _individualID);
            }
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates a Scanned Contribtion batch and Scanned Batch items.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="savedBatch">Specifies if this Scanned batch is saved. By default, the batch will not be pending.</param>
        /// <param name="scannedBatchItemAmounts">A list of amounts to use for each batch item.  This is optional.  If no items are specified, the Scanned Batch will have 0 items.</param>
        /// <param name="individualsToMatchToForItems">A list of individuals that will be matched to the Scanned Batch Item.  The order of the individuals will match up to the order of the Scanned Batch Items.</param>
        /// <param name="accountNumbers">The list of account numbers tied to each Scanned batch item.  The order of the account numbers will match up to the order of the Scanned batch items and the individual the batch item is tied to.</param>
        /// <param name="routingNumbers">The list of routing numbers tied to each Scanned batch item.  The order of the routing numbers will match up to the order of the Scanned batch items.</param>
        /// <param name="fund">The fund this Scanned Batch is tied to when saved. This is optional and, by default, the General Fund will be used.</param>
        /// <pparam name="householdOnly">Specifies if the Scanned Batch items are tied to the household only.  By default, they will not.</param>
        public void Giving_Batches_Create_Scanned(int churchId, string batchName, [Optional, DefaultParameterValue(false)] bool savedBatch,
            [Optional, DefaultParameterValue(null)] List<double> scannedBatchItemAmounts, [Optional, DefaultParameterValue(null)] List<string> individualsToMatchToForItems,
            [Optional, DefaultParameterValue(null)] List<string> accountNumbers, [Optional, DefaultParameterValue(null)] List<string> routingNumbers,
            [Optional, DefaultParameterValue(false)] bool isOrganization,
            [Optional, DefaultParameterValue("1 - General Fund")] string fund, [Optional, DefaultParameterValue(false)] bool isHousehold)
            //bool isOrganization = false
        {
            var batchStatusID = 0;
            if (savedBatch) {
                batchStatusID = 5;
            }
            else {
                batchStatusID = 4;
            }

            // The total amount for the batch.
            var totalCount = scannedBatchItemAmounts != null ? scannedBatchItemAmounts.Count : 0;
            var totalAmount = scannedBatchItemAmounts != null ? scannedBatchItemAmounts.Sum() : 0;
            //int individualIDTemp = this.People_Individuals_FetchID(churchId, individualsToMatchToForItems[0]);
            //TestLog.WriteLine("Individual ID: " + individualIDTemp);
            //TestLog.WriteLine("Individual: " + individualsToMatchToForItems[0]);
            TestLog.WriteLine("Amount total: " + totalAmount);
            TestLog.WriteLine("Count total: " + totalCount);

            // Create the Batch and the Scanned Batch
            StringBuilder query = new StringBuilder("DECLARE @batchName VARCHAR(50) ");
            query.Append("DECLARE @amount MONEY, @totalAmount MONEY ");
            query.Append("DECLARE @batchId INT, @churchID INT, @referenceImageID INT, @index INT, @IndividualID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @batchName = '{0}' ", batchName);
            query.AppendFormat("SET @totalAmount = '{0}' ", totalAmount);
            //query.AppendFormat("SET @IndividualID = '{0}' ", individualIDTemp);
            query.Append("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = @churchID) ");
            query.Append("INSERT INTO ChmContribution.dbo.BATCH (CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, BatchStatusID, BatchTypeID) ");
            query.AppendFormat("VALUES(@churchID, @batchName, CURRENT_TIMESTAMP, @totalAmount, {0}, 4) ", batchStatusID);
            query.Append("SET @batchId = (SELECT SCOPE_IDENTITY()) ");
            query.Append("SELECT top 1 @IndividualID = Individual_ID from chmpeople.dbo.individual where church_ID = @churchID ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatch (ChurchID, BatchID, BatchName, BatchCreatedDate, ItemCount, BatchAmount, LastUpdatedByIndividualID) ");
            query.AppendFormat("VALUES(@churchID, @batchId, @batchName, CURRENT_TIMESTAMP, {0}, @totalAmount, @IndividualID) ", totalCount);

            this.Execute(query.ToString());

            // Create the Scanned batch items
            if (scannedBatchItemAmounts != null) {
                var batchID = this.Giving_Batches_Fetch_ID(churchId, GeneralEnumerations.BatchTypes.Scanned, batchName);
                int fundId = this.Giving_Funds_FetchID(churchId, fund);
                foreach (var scannedBatchItemAmount in scannedBatchItemAmounts) {
                    // Match the Scanned Batch item to the first individual, if provided

                    StringBuilder query2 = new StringBuilder("DECLARE @referenceImageID INT ");
                    query2.AppendFormat("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = {0}) ", churchId);
                    // Store the account number
                    var indexOfCurrentItem = scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount);
                    var accountNumber = accountNumbers != null ? accountNumbers[scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount)] : "1234567890";
                    var routingNumber = routingNumbers != null ? routingNumbers[scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount)] : "111000025";

                    if (individualsToMatchToForItems != null) {

                        if (isOrganization) {
                            var householdID = this.Execute(string.Format("SELECT HOUSEHOLD_ID FROM ChmPeople.dbo.HOUSEHOLD WHERE CHURCH_ID = 15 AND PARTY_TYPE_ID IN (2,3) AND HOUSEHOLD_NAME = '{0}'", individualsToMatchToForItems[scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount)])).Rows[0][0];
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, CreatedDate, CheckNumber, HouseholdID) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, 'O:65H8W0ORG', CURRENT_TIMESTAMP, {5}, {6}) ", churchId, batchID, accountNumber, routingNumber, scannedBatchItemAmount, scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount) + 500, householdID);
                        }
                        else {
                            var individualID = this.People_Individuals_FetchID(churchId, individualsToMatchToForItems[scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount)]);
                            var householdID = this.People_Households_FetchID(churchId, individualID);
                            //check householdID DB.NULL
                            if (isHousehold) {
                                TestLog.WriteLine("RDC Household - Matched");
                                query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, CreatedDate, CheckNumber, HouseholdID) ");
                                query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, 'M:HookEm05', CURRENT_TIMESTAMP, {5}, {6}) ", churchId, batchID, accountNumber, routingNumber, scannedBatchItemAmount, scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount) + 1997, householdID);

                            }
                            else {
                                query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, CreatedDate, CheckNumber, IndividualID, HouseholdID) ");
                                query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, 'T:65H8W0X7w', CURRENT_TIMESTAMP, {5}, {6}, {7}) ", churchId, batchID, accountNumber, routingNumber, scannedBatchItemAmount, scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount) + 500, individualID, householdID);
                            }
                        }
                    }
                    else {
                        if (isHousehold) {
                            TestLog.WriteLine("RDC Household - Unmatched");
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, CreatedDate, CheckNumber) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, 'U:HookEm2005', CURRENT_TIMESTAMP, {5}) ", churchId, batchID, accountNumber, routingNumber, scannedBatchItemAmount, scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount) + 1998);

                        }
                        else {
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, CreatedDate, CheckNumber) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, 'T:65H8W0X7w', CURRENT_TIMESTAMP, {5}) ", churchId, batchID, accountNumber, routingNumber, scannedBatchItemAmount, scannedBatchItemAmounts.IndexOf(scannedBatchItemAmount) + 500);

                        }
                    }

                    try {
                        this.Execute(query2.ToString());
                    }
                    catch (Exception e) {
                        throw new Exception(e.Message);
                    }


                    //query.AppendFormat("DELETE c_r FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT c_r WITH (NOLOCK) INNER JOIN ChmContribution.dbo.ACCOUNT ac WITH (NOLOCK) ON c_r.ACCOUNT_ID = ac.ACCOUNT_ID WHERE c_r.CHURCH_ID = {0} AND ac.ACCOUNT = '{1}' AND ac.ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
                    //query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT = '{1}' AND ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);


                }

                // Create a contribution receipt for each item if we are creating a saved batch
                if (savedBatch) {
                    log.DebugFormat("Batch ID: {0}", batchID);
                    this.Giving_RDCBatch_Save_Proc(churchId, Convert.ToInt32(batchID), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), fundId);
                    // query2.Append("INSERT INTO ChmContribution.dbo.CONTRIBUTION_RECEIPT(Church_ID, Fund_ID, Amount, Reference_Image_ID, Received_Date, Batch_ID, Is_Split, Address_Verification, Contribution_Type_ID, Thank) ");
                    // query2.AppendFormat("VALUES({0}, {1}, {2}, @referenceImageID, CURRENT_TIMESTAMP, {3}, 0, 0, 5, 1) ", churchId, fundId, scannedBatchItemAmount, batchID);
                }
                
            }
        }

        // Converts to DBNull, if Null
        public static object ToDBNull(object value) {
            if (null != value)
                return value;
            return DBNull.Value;
        }

        public void Giving_RDCBatch_Save_Proc(int churchID, int batchId, DateTime receivedDate, int fundId, int? subFundId = null, int? pledgeDriveId = null) {
           

            string proc = "ChmContribution.Pmt.RDCBatch_Save";
            SqlDataReader dr;
           
            log.Debug(string.Format("EXECUTE PROC {0} with following parms. ChurchID: {1}, BatchID: {2}, ReceivedDate: {3}, FundID: {4}, SubFundID: {5}, PledgeDriveID: {6}",
                proc, churchID, batchId, receivedDate.ToString(), fundId, subFundId, pledgeDriveId));

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString)) {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));
                    cmd.Parameters.Add(new SqlParameter("@BatchID", batchId));
                    cmd.Parameters.Add(new SqlParameter("@ReceivedDate", receivedDate));
                    cmd.Parameters.Add(new SqlParameter("@FundID", fundId));
                    cmd.Parameters.Add(new SqlParameter("@SubFundID", ToDBNull(subFundId)));
                    cmd.Parameters.Add(new SqlParameter("@PledgeDriveID", ToDBNull(pledgeDriveId)));
                    dr = cmd.ExecuteReader();

                }

                dbConnection.Close();
            }

        }

        /// <summary>
        /// Creates a remote deposit capture batch and RDC Batch items.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="savedBatch">Specifies if this RDC batch is saved. By default, the batch will not be saved.</param>
        /// <param name="rdcBatchItemAmounts">A list of amounts to use for each batch item.  This is optional.  If no items are specified, the RDC Batch will have 0 items.</paparam>
        /// <param name="individualsToMatchToForItems">A list of individuals that will be matched to the RDC Batch Item.  The order of the individuals will match up to the order of the RDC Batch Items.</param>
        /// <param name="accountNumbers">The list of account numbers tied to each RDC batch item.  The order of the account numbers will match up to the order of the RDC batch items and the individual the batch item is tied to.</param>
        /// <param name="routingNumbers">The list of routing numbers tied to each RDC batch item.  The order of the routing numbers will match up to the order of the RDC batch items.</param>
        /// <param name="fund">The fund this RDC Batch is tied to when saved. This is optional and, by default, the General Fund will be used.</param>
        /// <pparam name="householdOnly">Specifies if the RDC Batch items are tied to the household only.  By default, they will not.</pparam>
        public void Giving_Batches_Create_RDC(int churchId, string batchName, [Optional, DefaultParameterValue(false)] bool savedBatch, 
            [Optional, DefaultParameterValue(null)] List<double> rdcBatchItemAmounts, [Optional, DefaultParameterValue(null)] List<string> individualsToMatchToForItems, 
            [Optional, DefaultParameterValue(null)] List<string> accountNumbers, [Optional, DefaultParameterValue(null)] List<string> routingNumbers, 
            [Optional, DefaultParameterValue(false)] bool isOrganization,
            [Optional, DefaultParameterValue("1 - General Fund")] string fund, [Optional, DefaultParameterValue(false)] bool isHousehold, string referenceNumber = "O:65H8W0ORG")            
            //bool isOrganization = false
        {
            var batchStatusID = 0;
            if (savedBatch) {
                batchStatusID = 5;
            }
            else {
                batchStatusID = 4;
            }

            // The total amount for the batch.
            var totalAmount = rdcBatchItemAmounts != null ? rdcBatchItemAmounts.Sum() : 0;

            // Create the Batch and the RDC Batch
            StringBuilder query = new StringBuilder("DECLARE @batchName VARCHAR(50) ");
            query.Append("DECLARE @amount MONEY, @totalAmount MONEY ");
            query.Append("DECLARE @batchId INT, @churchID INT, @ppMerchantAccountID INT, @locationID INT, @referenceImageID INT, @index INT, @IndividualID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @batchName = '{0}' ", batchName);
            query.AppendFormat("SET @totalAmount = '{0}' ", totalAmount);
            query.Append("SET @ppMerchantAccountID = (SELECT TOP 1 PP_MERCHANT_ACCOUNT_ID FROM ChmContribution.dbo.PP_MERCHANT_ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND PP_MERCHANT_ACCOUNT_CODE = 'ftone') ");
            query.Append("SET @locationID = (SELECT TOP 1 Location_ID FROM ChmContribution.dbo.PP_ProfitStars_Merchant_Location WITH (NOLOCK) WHERE Church_ID = @churchID) ");
            query.Append("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = @churchID) ");
            query.Append("INSERT INTO ChmContribution.dbo.BATCH (CHURCH_ID, BATCH_NAME, BATCH_DATE, BATCH_AMOUNT, BatchStatusID, BatchTypeID) ");
            query.AppendFormat("VALUES(@churchID, @batchName, CURRENT_TIMESTAMP, @totalAmount, {0}, 3) ", batchStatusID);
            query.Append("SET @batchId = (SELECT SCOPE_IDENTITY()) ");
            query.Append("SELECT top 1 @IndividualID = Individual_ID from chmpeople.dbo.individual where church_ID = @churchID ");
            query.Append("INSERT INTO ChmContribution.Pmt.RDCBatch (ChurchID, BatchID, BatchName, BatchCreatedDate, ItemCount, BatchAmount, LastUpdatedByIndividualID) ");
            query.AppendFormat("VALUES(@churchID, @batchId, @batchName, CURRENT_TIMESTAMP, {0}, @totalAmount, @IndividualID) ", rdcBatchItemAmounts != null ? rdcBatchItemAmounts.Count : 0);

            this.Execute(query.ToString());

            // Create the RDC batch items
            if (rdcBatchItemAmounts != null) {
                var batchID = this.Giving_Batches_Fetch_ID(churchId, GeneralEnumerations.BatchTypes.RDC, batchName);
                var fundId = this.Giving_Funds_FetchID(churchId, fund);
                
                foreach (var rdcBatchItemAmount in rdcBatchItemAmounts) {
                    // Match the RDC Batch item to the first individual, if provided
                    StringBuilder query2 = new StringBuilder("DECLARE @ppMerchantAccountID INT, @referenceImageID INT, @locationID INT ");
                    query2.AppendFormat("SET @ppMerchantAccountID = (SELECT TOP 1 PP_MERCHANT_ACCOUNT_ID FROM ChmContribution.dbo.PP_MERCHANT_ACCOUNT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND PP_MERCHANT_ACCOUNT_CODE = 'ftone') ", churchId);
                    query2.AppendFormat("SET @referenceImageID = (SELECT TOP 1 reference_image_id FROM ChmContribution.dbo.REFERENCE_IMAGE WITH (NOLOCK) WHERE church_Id = {0}) ", churchId);
                    query2.AppendFormat("SET @locationID = (SELECT TOP 1 Location_ID FROM ChmContribution.dbo.PP_ProfitStars_Merchant_Location WITH (NOLOCK) WHERE Church_ID = {0}) ", churchId);

                    // Store the account number
                    var accountNumber = accountNumbers != null ? accountNumbers[rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount)] : "1234567890";
                    var routingNumber = routingNumbers != null ? routingNumbers[rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount)] : "111000025";

                    if (individualsToMatchToForItems != null)
                    {

                        if (isOrganization)
                        {
                            var householdID = this.Execute(string.Format("SELECT HOUSEHOLD_ID FROM ChmPeople.dbo.HOUSEHOLD WHERE CHURCH_ID = 15 AND PARTY_TYPE_ID IN (2,3) AND HOUSEHOLD_NAME = '{0}'", individualsToMatchToForItems[rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount)])).Rows[0][0];
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber, HouseholdID) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, '{7}', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, {5}, {6}) ", churchId, batchID, accountNumber, routingNumber, rdcBatchItemAmount, rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount) + 500, householdID, referenceNumber);
                        }
                        else
                        {
                            var individualID = this.People_Individuals_FetchID(churchId, individualsToMatchToForItems[rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount)]);
                            var householdID = this.People_Households_FetchID(churchId, individualID);
                            if (isHousehold)
                            {
                                TestLog.WriteLine("RDC Household - Matched");
                                referenceNumber = "M:GigEmAg97";
                                query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber, HouseholdID) ");
                                query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, '{7}', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, {5}, {6}) ", churchId, batchID, accountNumber, routingNumber, rdcBatchItemAmount, rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount) + 1997, householdID, referenceNumber);

                            }
                            else
                            {
                                referenceNumber = "T:65H8W0X7w";
                                query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber, IndividualID, HouseholdID) ");
                                query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, '{8}', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, {5}, {6}, {7}) ", churchId, batchID, accountNumber, routingNumber, rdcBatchItemAmount, rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount) + 500, individualID, householdID, referenceNumber);
                            }
                        }
                    }
                    else
                    {
                        if (isHousehold)
                        {
                            TestLog.WriteLine("RDC Household - Unmatched");
                            referenceNumber = "U:GigEm1999";
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, '{6}', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, {5}) ", churchId, batchID, accountNumber, routingNumber, rdcBatchItemAmount, rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount) + 1998, referenceNumber);

                        }
                        else
                        {
                            referenceNumber = "T:65H8W0X7w";
                            query2.Append("INSERT INTO ChmContribution.Pmt.RDCBatchItem (ChurchID, BatchID, ItemCreatedDate, AccountNumber, RoutingNumber, Amount, ReferenceImageID, ReferenceNumber, PPMerchantAccountID, LocationID, CreatedDate, CheckNumber) ");
                            query2.AppendFormat("VALUES({0}, {1}, CURRENT_TIMESTAMP, '{2}', '{3}', '{4}', @referenceImageID, '{6}', @ppMerchantAccountID, @locationID, CURRENT_TIMESTAMP, {5}) ", churchId, batchID, accountNumber, routingNumber, rdcBatchItemAmount, rdcBatchItemAmounts.IndexOf(rdcBatchItemAmount) + 500, referenceNumber);

                        }
                    }

                    

                    //query.AppendFormat("DELETE c_r FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT c_r WITH (NOLOCK) INNER JOIN ChmContribution.dbo.ACCOUNT ac WITH (NOLOCK) ON c_r.ACCOUNT_ID = ac.ACCOUNT_ID WHERE c_r.CHURCH_ID = {0} AND ac.ACCOUNT = '{1}' AND ac.ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
                    //query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT = '{1}' AND ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
                    try {
                        this.Execute(query2.ToString());
                    }
                    catch (Exception e) {
                        throw new Exception(e.Message);
                    }

                }

                // Create a contribution receipt for each item if we are creating a saved batch
                if (savedBatch) {
                    log.DebugFormat("Batch ID: {0}", batchID);
                    this.Giving_RDCBatch_Save_Proc(churchId, Convert.ToInt32(batchID), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), fundId);
                }
            }
        }

        public void Giving_Batches_Delete_RDC(int churchID, string batchName, string accountNumber, string routingNumber)
        {

            TestLog.WriteLine("Delete RDC");
            StringBuilder query = new StringBuilder();             
            query.AppendFormat("DELETE c_r FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT c_r WITH (NOLOCK) INNER JOIN ChmContribution.dbo.ACCOUNT ac WITH (NOLOCK) ON c_r.ACCOUNT_ID = ac.ACCOUNT_ID WHERE c_r.CHURCH_ID = {0} AND ac.ACCOUNT = '{1}' AND ac.ROUTING_NO = '{2}'", churchID, accountNumber, routingNumber);
            query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT = '{1}' AND ROUTING_NO = '{2}'", churchID, accountNumber, routingNumber);

            this.Execute(query.ToString());

        }

        /// <summary>
        /// Adds a contribution to a credit card batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="fundName">The name of the fund the contribution should be attributed to.</param>
        /// <param name="amount">The amount of the contribution.</param>
        public void Giving_Batches_AddContributionToCreditCardBatch(int churchId, string batchName, string fundName, double amount, DateTime expirationDate) {
            StringBuilder query = new StringBuilder("DECLARE @batchID INT, @creditCardInformationID INT, @fundID INT, @paymentgroup INT, @paymentInfoID INT ");
            query.AppendFormat("SET @batchID = (SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeId = 2) ", churchId, batchName);
            query.Append("INSERT INTO ChmContribution.dbo.CreditCardInformation (ChurchID, BatchID, PaymentTypeID, HouseholdID, IndividualID, FirstName, LastName, Address1, City, StProvince, PostalCode, Country, AccountNumber, ExpirationDate, Email) ");
            query.AppendFormat("VALUES ({0}, @batchID, 1, {1}, {2}, 'Matthew', 'Sneeden', '9616 Armour Dr', 'Fort Worth', 'TX', '76244-6085', 'US', 'aQWpXh9WlGpMDtpFHcU8o/jz9IWaBX7SzfptyqJmxYc=', '{3}', 'msneeden@fellowshiptech.com') ", churchId, _householdID, _individualID, expirationDate.ToString("yyyy-MM-dd"));
            query.Append("SET @creditCardInformationID = (SELECT SCOPE_IDENTITY()) ");
            query.AppendFormat("SET @fundID = (SELECT FUND_ID FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}') ", churchId, fundName);
            

            if (IsAMSEnabled(churchId))
            {
                //Retrieving the PaymentInformation Id from PaymentInformation table. PaymentInfromationId is a required field for new AMS process.
                //query.AppendFormat("SET @pInfoID = (SELECT top 1 PaymentInformationId FROM ChmContribution.dbo.PaymentInformation where PaymentInformationId is not null AND ChurchID = {0} AND HouseholdID = {1} AND IndividualID = {2} AND ExpirationDate = {3} AND PaymentTypeID = 1 order by createddate desc ) ", churchId, _householdID, _individualID, expirationDate.ToString("yyyy-MM-dd"));
                query.AppendFormat("SET @paymentgroup = (SELECT top 1 PaymentId FROM ChmContribution.dbo.Payment where PaymentId is not null AND ChurchID = {0} AND HouseholdID = {1} AND IndividualID = {2}  order by createddate desc ) ", churchId, _householdID, _individualID);
                query.AppendFormat("SET @paymentInfoID = (SELECT top 1 PaymentInformationId FROM ChmContribution.dbo.Payment where PaymentInformationId is not null AND ChurchID = {0} AND HouseholdID = {1} AND IndividualID = {2}  order by createddate desc ) ", churchId, _householdID, _individualID);
                query.Append("INSERT INTO ChmContribution.dbo.Contribution (ChurchID, BatchID, CreditCardInformationID, FundID, Amount, CreatedByUserID, CreatedByIndividualID, PaymentGroup, PaymentInformationId, ContributionStatusId) ");
                query.AppendFormat("VALUES ({0}, @batchID, @creditCardInformationID, @fundID, {1}, 65211, 27103418, @paymentgroup, @paymentInfoID, 0)", churchId, amount);

               
            }
            else
            {
                query.Append("INSERT INTO ChmContribution.dbo.Contribution (ChurchID, BatchID, CreditCardInformationID, FundID, Amount, CreatedByUserID, CreatedByIndividualID) ");
                query.AppendFormat("VALUES ({0}, @batchID, @creditCardInformationID, @fundID, {1}, 65211, 27103418)", churchId, amount);

            }

            this.Execute(query.ToString());

        }

        /// <summary>
        /// This method deletes a batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <param name="batchTypeId">The type of the batch.</param>
        public void Giving_Batches_Delete(int churchId, string batchName, int batchTypeId) {
            StringBuilder query = new StringBuilder();
            
            if (batchTypeId == 2) {
                string batchSubQuery = string.Format("SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = 2", churchId, batchName);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.PaymentContributionReceipt WHERE ChurchID = {0} AND ContributionReceiptID IN (SELECT CONTRIBUTION_RECEIPT_ID FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_ID IN ({1})) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND BATCH_ID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.BatchHistory WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.Contribution WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.CreditCardInformation WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
            }
            else if (batchTypeId == 3) {
                string batchSubQuery = string.Format("SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = 3", churchId, batchName);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND BATCH_ID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatchItemDetail WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery); 
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatchItem WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatch WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
            }
            else if (batchTypeId == 4) {
                string batchSubQuery = string.Format("SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = 4", churchId, batchName);
                query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND BATCH_ID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatchItemDetail WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatchItem WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
                query.AppendFormat("DELETE FROM ChmContribution.Pmt.RDCBatch WHERE ChurchID = {0} AND BatchID IN ({1}) ", churchId, batchSubQuery);
            }
            else {
                query.AppendFormat("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND BATCH_ID IN (SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = 1) ", churchId, batchName.Replace("'", "''"));
            }
            query.AppendFormat("DELETE FROM ChmContribution.dbo.BATCH WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = {2}", churchId, batchName.Replace("'", "''"), batchTypeId);
            this.Execute(query.ToString());
        }

        public void Giving_Batches_Delete_All(int churchID, string fromDate = "01/01/2000", int batchTypeID = 3)
        {

            StringBuilder query = new StringBuilder("CREATE TABLE #Batches(ID INT IDENTITY(1,1), BatchID INT, processed INT DEFAULT 0) ");
            query.Append("DECLARE @churchID INT, @batchID INT, @batchTypeID INT, @startid INT, @endid INT, @date DATETIME ");
            query.AppendFormat("SET @churchID = {0} ", churchID);
            query.AppendFormat("SET @date = '{0}' ", fromDate);
            query.AppendFormat("SET @batchTypeID = '{0}' ", batchTypeID);
            query.Append("INSERT INTO #Batches (BatchID) SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND BATCH_DATE > @date AND BatchTypeID = @batchTypeID ORDER BY BATCH_DATE DESC ");
            query.Append("SET @startid = (SELECT MIN(ID) FROM #Batches WHERE processed = 0) ");
            query.Append("SET @endid = (SELECT MAX(ID) FROM #Batches WHERE processed = 0) ");
            query.Append("WHILE (@startid <= @endid) ");
            query.Append("BEGIN ");
            query.Append("SELECT @batchID = BatchID FROM #Batches WHERE ID = @startid ");
            query.Append("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = @churchID AND BATCH_ID = @batchID ");

            if (batchTypeID == 3)
            {
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatchItemDetail WHERE ChurchID = @churchID AND BatchID = @batchID ");
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatchItem WHERE ChurchID = @churchID AND BatchID = @batchID ");
                query.Append("DELETE FROM ChmContribution.Pmt.RDCBatch WHERE ChurchID = @churchID AND BatchID = @batchID ");
            }

            query.Append("DELETE FROM ChmContribution.dbo.BATCH WHERE CHURCH_ID = @churchID AND BATCH_ID = @batchID ");
            query.Append("UPDATE #Batches SET processed = 1 WHERE id = @startid ");
            query.Append("SET @startid = @startid + 1 ");
            query.Append("END ");
            query.Append("DROP TABLE #Batches ");

            //if (deleteAccounts)
            //{
            //    query.AppendFormat("DELETE c_r FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT c_r WITH (NOLOCK) INNER JOIN ChmContribution.dbo.ACCOUNT ac WITH (NOLOCK) ON c_r.ACCOUNT_ID = ac.ACCOUNT_ID WHERE c_r.CHURCH_ID = {0} AND ac.ACCOUNT = '{1}' AND ac.ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
            //    query.AppendFormat("DELETE FROM ChmContribution.dbo.ACCOUNT WHERE CHURCH_ID = {0} AND ACCOUNT = '{1}' AND ROUTING_NO = '{2}'", churchID, form["accountNumber"], form["routingNumber"]);
            //}

            DataTable results = this.Execute(query.ToString());
            
            
        }

        /// <summary>
        /// Fetches a batch's ID.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchType">The type of batch.</param>
        /// <param name="batchName">The name of the batch.</param>
        /// <returns></returns>
        public object Giving_Batches_Fetch_ID(int churchId, GeneralEnumerations.BatchTypes batchType, string batchName) {
            return this.Execute("SELECT TOP 1 BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' and BatchTypeID = {2}", churchId, batchName, (int)batchType).Rows[0]["BATCH_ID"];
        }

        #endregion Batches

        #region Contribution Attributes
        /// <summary>
        /// This method creates a contribution attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionAttributeName">The name of the contribution attribute.</param>
        public void Giving_ContributionAttributes_Create(int churchId, string contributionAttributeName, int createdByUserId) {
            this.Execute(string.Format("INSERT INTO ChmContribution.dbo.CONTRIBUTION_ATTRIBUTE (CHURCH_ID, CONTRIBUTION_ATTRIBUTE_NAME, CREATED_BY_USER_ID) VALUES({0}, '{1}', {2})", churchId, contributionAttributeName, createdByUserId));
        }

        /// <summary>
        /// This method deletes a contribution attribute.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionAttributeName">The name of the contribution attribute to be deleted.</param>
        public void Giving_ContributionAttributes_Delete(int churchId, string contributionAttributeName) {
            this.Execute(string.Format("DELETE FROM ChmContribution.dbo.CONTRIBUTION_ATTRIBUTE WHERE CHURCH_ID = {0} AND CONTRIBUTION_ATTRIBUTE_NAME = '{1}'", churchId, contributionAttributeName));
        }
        #endregion Contribution Attributes

        /// <summary>
        /// This method deletes contribution receipts tied to a batch.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="batchName">The batch name.</param>
        /// <param name="batchTypeId">The batch type id.</param>
        public void Giving_ContributionReceipts_DeleteByBatchName(int churchId, string batchName, int batchTypeId) {
            this.Execute(string.Format("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND BATCH_ID IN (SELECT BATCH_ID FROM ChmContribution.dbo.BATCH WITH (NOLOCK) WHERE CHURCH_ID = {0} AND BATCH_NAME = '{1}' AND BatchTypeID = {2})", churchId, batchName, batchTypeId));
        }

        /// <summary>
        /// This method deletes contribution receipts tied to a pledge drive.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="pledgeDriveName">The pledge drive name.</param>
        public void Giving_ContributionReceipts_DeleteByPledgeDriveName(int churchId, string pledgeDriveName)
        {
            this.Execute(string.Format("DELETE FROM ChmContribution.dbo.CONTRIBUTION_RECEIPT WHERE CHURCH_ID = {0} AND PLEDGE_DRIVE_ID IN (SELECT PLEDGE_DRIVE_ID FROM ChmContribution.dbo.PLEDGE_DRIVE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND PLEDGE_DRIVE_NAME = '{1}')", churchId, pledgeDriveName));
        }

        #region Funds
        /// <summary>
        /// This method fetches the CONTRIBUTION_RECEIPT_ID for a CONTRIBUTION_RECEIPT.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundId">The Id of the fund.</param>
        /// <returns>Integer representing the CONTRIBUTION_RECEIPT_ID.</returns>
        public int Giving_Contribution_Receipt_FetchID(int churchId, int fundId) {
            return Convert.ToInt32(this.Execute("SELECT TOP 1 CONTRIBUTION_RECEIPT_ID FROM chmcontribution.dbo.contribution_receipt WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_ID = '{1}'", churchId, fundId).Rows[0]["CONTRIBUTION_RECEIPT_ID"]);
        }

        /// <summary>
        /// This method fetches the id for a fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund.</param>
        /// <returns>Integer representing the fund id.</returns>
        public int Giving_Funds_FetchID(int churchId, string fundName) {
            return Convert.ToInt32(this.Execute("SELECT TOP 1 FUND_ID FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}'", churchId, fundName).Rows[0]["FUND_ID"]);
        }

        /// <summary>
        /// This method returns all of the fund names in the specified church.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="isActive">Flag deisgnating active funds.</param>
        /// <param name="isWebActive">Flag designating web active funds.</param>
        /// <param name="hasAccountReferenceIdSet">Flag designating funds that have an account reference.</param>
        /// <returns>DataTable containing the names of the funds belonging to the church.</returns>
        public DataTable Giving_Funds_FetchNames(int churchId, bool isActive, bool? isWebActive, bool? hasAccountReferenceIdSet) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT FUND_NAME FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND IS_ACTIVE = {1} ", churchId, Convert.ToInt16(isActive));
            if (isWebActive.HasValue) {
                if ((bool)isWebActive) {
                    query.AppendFormat("AND IS_WEB_ACTIVE = {0} ", Convert.ToInt16(isWebActive));
                }
            }

            if (hasAccountReferenceIdSet.HasValue) {
                if ((bool)hasAccountReferenceIdSet) {
                    query.AppendFormat("AND PP_ACCOUNT_REFERENCE_ID IS NOT NULL ");
                }
            }
            query.Append(" ORDER BY FUND_NAME ASC");

            return this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates a fund
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund.</param>
        /// <param name="active">Flag for active.</param>
        /// <param name="glAccount">The general ledger account.</param>
        /// <param name="fundType">The fund type.</param>
        /// <param name="webActive">Flag for web active.</param>
        /// <param name="accountReferenceDescription">The name of the account reference the fund will be tied to.</param>
        public void Giving_Funds_Create(int churchId, string fundName, bool active, string glAccount, int fundType, bool webActive, string accountReferenceDescription) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmContribution.dbo.FUND (CHURCH_ID, FUND_NAME, IS_ACTIVE, GL_ACCOUNT, FUND_TYPE, IS_WEB_ACTIVE, PP_ACCOUNT_REFERENCE_ID) ");
            query.AppendFormat("SELECT {0}, '{1}', {2}, '{3}', {4}, {5}, PP_ACCOUNT_REFERENCE_ID FROM ChmContribution.dbo.PP_ACCOUNT_REFERENCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACCOUNT_DESC = '{6}'", churchId, fundName, Convert.ToInt16(active), glAccount, fundType, Convert.ToInt16(webActive), accountReferenceDescription);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Deletes a fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund.</param>
        public void Giving_Funds_Delete(int churchId, string fundName) {
            try
            {
                this.Execute(string.Format("DELETE FROM ChmContribution.dbo.FUND WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}'", churchId, fundName));
            } catch (System.Exception) {
                this.Execute(string.Format("DELETE from chmcontribution.dbo.PaymentContributionReceipt where Contributionreceiptid={0}", this.Giving_Contribution_Receipt_FetchID(churchId, Giving_Funds_FetchID(churchId, fundName))));
                this.Execute(string.Format("DELETE FROM chmcontribution.dbo.contribution_receipt WHERE CHURCH_ID = {0} AND FUND_ID = {1}", churchId, Giving_Funds_FetchID(churchId, fundName)));
                this.Execute(string.Format("DELETE FROM ChmContribution.dbo.FUND WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}'", churchId, fundName));
            }
            
        }
        #endregion Funds

        /// <summary>
        /// This method marks a scheduled contribution for weblink as inactive and deleted.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="amount">The amount of the scheduled contribution.</param>
        public void Giving_ScheduledContributions_Delete_PortalORWeblink(int churchId, int individualId, double amount, int applicationId) {
            StringBuilder query = new StringBuilder("UPDATE ChmContribution.dbo.SCHEDULED_CONTRIBUTION ");
            query.Append("SET IS_ACTIVE = 0, IS_DELETED = 1 ");
            query.AppendFormat("WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} AND AMOUNT = {2} AND IS_ACTIVE = 1 AND IS_DELETED = 0 AND ORIGINAL_FT_APPLICATION_ID = {3}", churchId, individualId, amount, applicationId);
            this.Execute(query.ToString());
        }

        #region Pledge Drives
        //INSERT INTO ChmContribution.dbo.PLEDGE_DRIVE (CHURCH_ID, PLEDGE_DRIVE_NAME, FUND_ID, START_DATE, END_DATE, GOAL, SUB_FUND_ID, IS_WEB_ACTIVE)
        //VALUES(15, 'name', 123, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 100, 123, 1)

        /// <summary>
        /// This method deletes a pledge drive.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="pledgeDriveName">The name of the pledge drive.</param>
        public void Giving_PledgeDrives_Delete(int churchId, string pledgeDriveName) {
            Giving_ContributionReceipts_DeleteByPledgeDriveName(churchId, pledgeDriveName);
            this.Execute("DELETE FROM ChmContribution.dbo.PLEDGE_DRIVE WHERE CHURCH_ID = {0} AND PLEDGE_DRIVE_NAME = '{1}'", churchId, pledgeDriveName);
        }

        /// <summary>
        /// This method get the pledge drive id in a pledge drive.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="pledgeDriveName">The name of the pledge drive.</param>
        public int Giving_PledgeDrives_FetchID(int churchId, string pledgeDriveName) {
            return Convert.ToInt16(this.Execute("SELECT TOP 1 PLEDGE_DRIVE_ID FROM ChmContribution.dbo.PLEDGE_DRIVE WHERE CHURCH_ID = {0} AND PLEDGE_DRIVE_NAME = '{1}'", churchId, pledgeDriveName));
        }

        /// <summary>
        /// Deletes all the pledges for an individual and their household.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the individual.</param>
        public void Giving_PledgeDrives_Pledge_Delete(int churchId, string individualName) {
            var individualID = this.People_Individuals_FetchID(churchId, individualName);
            var householdId = this.People_Households_FetchID(churchId, individualID);

            if (householdId > 0)
            {
                this.Execute("DELETE FROM ChmContribution.dbo.PLEDGE WHERE CHURCH_ID = {0} and HOUSEHOLD_ID = {1}", churchId, householdId);
            }
        }

        #endregion Pledge Drives

        #region Sub Funds
        /// <summary>
        /// This method creates a sub fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund the sub fund will be tied to.</param>
        /// <param name="subFundName">The name of the sub fund.</param>
        /// <param name="subFundCode">The sub fund code.</param>
        /// <param name="active">Flag for active.</param>
        /// <param name="glAccount">The general ledger account.</param>
        /// <param name="webActive">Flag for web active.</param>
        public void Giving_SubFunds_Create(int churchId, string fundName, string subFundName, string subFundCode, bool active, string glAccount, bool webActive) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmContribution.dbo.SUB_FUND (CHURCH_ID, FUND_ID, SUB_FUND_NAME, SUB_FUND_CODE, IS_ACTIVE, GL_ACCOUNT, IS_WEB_ACTIVE) ");
            query.AppendFormat("SELECT {0}, FUND_ID, '{1}', '{2}', {3}, '{4}', {5} FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_NAME = '{6}'", churchId, subFundName, subFundCode, Convert.ToInt16(active), glAccount, Convert.ToInt16(webActive), fundName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a sub fund.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="fundName">The name of the fund the sub fund is tied to.</param>
        /// <param name="subFundName">The name of the sub fund.</param>
        public void Giving_SubFunds_Delete(int churchId, string fundName, string subFundName) {

            //add by Grace Zhang,to  prevent foreign Key conflict
            StringBuilder queryadd = new StringBuilder();
            queryadd.AppendFormat("DELETE FROM ChmContribution.dbo.ScheduledGivingDetail WHERE CHURCHID = {0} ", churchId);
            queryadd.AppendFormat("AND FUNDID = (SELECT TOP 1 FUND_ID FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}')", churchId, fundName, subFundName);
            this.Execute(queryadd.ToString());
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmContribution.dbo.SUB_FUND WHERE CHURCH_ID = {0} ", churchId);
            query.AppendFormat("AND FUND_ID = (SELECT TOP 1 FUND_ID FROM ChmContribution.dbo.FUND WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FUND_NAME = '{1}') AND SUB_FUND_NAME = '{2}'", churchId, fundName, subFundName);
            this.Execute(query.ToString());
        }
        #endregion Sub Funds

        #region Sub Types
        /// <summary>
        /// This method creates a sub type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionTypeId">The id of the parent type.</param>
        /// <param name="contributionSubTypeName">The name of the sub type.</param>
        /// <param name="isActive">Flag for active.</param>
        public void Giving_SubTypes_Create(int churchId, int contributionTypeId, string contributionSubTypeName, bool isActive){
            StringBuilder query = new StringBuilder("INSERT INTO ChmContribution.dbo.CONTRIBUTION_SUB_TYPE (CHURCH_ID, CONTRIBUTION_TYPE_ID, CONTRIBUTION_SUB_TYPE_NAME, IS_ACTIVE) ");
            query.AppendFormat("VALUES({0}, {1}, '{2}', 1)", churchId, contributionTypeId, contributionSubTypeName);
            this.Execute(query.ToString());
        }
        
        /// <summary>
        /// This method deletes a sub type.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="contributionSubTypeName">The name of the sub type.</param>
        public void Giving_SubTypes_Delete(int churchId, string contributionSubTypeName){
            this.Execute("DELETE FROM ChmContribution.dbo.CONTRIBUTION_SUB_TYPE WHERE CHURCH_ID = {0} AND CONTRIBUTION_SUB_TYPE_NAME = '{1}'", churchId, contributionSubTypeName);
        }
        #endregion Sub Types

        /// <summary>
        /// Deletes all the scheduled giving for an infellowship user.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="infellowshipLogin">The login for the user's infellowship account.</param>
        public void Giving_ScheduleGiving_DeleteAll(int churchId, string infellowshipLogin) {
            this.Execute(string.Format("UPDATE ChmContribution.dbo.ScheduledGiving SET DeletedDate = CURRENT_TIMESTAMP WHERE ChurchID = {0} AND CreatedByLogin = '{1}'", churchId, infellowshipLogin));
        }

        /// <summary>
        /// Determines if a given schedule is active or inactive.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="amount">The amount of the contribution</param>
        /// <returns>boolean</returns>
        public bool Giving_ScheduledGiving_IsScheduleActive(int churchId, double amount) {
            StringBuilder isActiveQuery = new StringBuilder("SELECT TOP 1 SG.ISACTIVE FROM CHMCONTRIBUTION.DBO.PAYMENTINFORMATION PIF WITH (NOLOCK) ");
            isActiveQuery.Append("INNER JOIN CHMCONTRIBUTION.DBO.SCHEDULEDGIVING SG WITH (NOLOCK) ON SG.PAYMENTINFORMATIONID=PIF.PAYMENTINFORMATIONID ");
            isActiveQuery.Append("INNER JOIN CHMCONTRIBUTION.DBO.SCHEDULEDGIVINGDETAIL SGD WITH (NOLOCK) ON SGD.SCHEDULEDGIVINGID=SG.SCHEDULEDGIVINGID ");
            isActiveQuery.AppendFormat("WHERE SGD.CHURCHID={0} AND SGD.AMOUNT={1} ORDER BY SGD.CREATEDDATE DESC", churchId, amount);
            
            bool returnValue = Convert.ToInt16(this.Execute(isActiveQuery.ToString()).Rows[0][0]) == 0 ? false : true;
            return returnValue;
        }

        /// <summary>
        /// Pulls back the credit card ID number associated with card type
        /// </summary>
        /// <param name="creditCardType">The name of the credit card</param>
        /// <returns>integer</returns>
        public int Giving_CreditCardTypeID(string creditCardType)
        {
            DataTable results_cardTypeID = this.Execute("SELECT TOP 1 PP_Type_ID FROM ChmContribution.dbo.PP_TYPE where PP_TYPE_NAME = '{0}'", creditCardType);
            int cardType_ID = Convert.ToInt32(results_cardTypeID.Rows[0]["PP_Type_ID"]);


            return (cardType_ID);
        }
        #endregion Giving

        #region Groups
        /// <summary>
        /// Creates a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The name of the group type the group will belong to.</param>
        /// <param name="individualName">The individual name who will be creating the group.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="groupDescription">The description of the group.</param>
        /// <param name="startDate">The start date for the group.</param>
        /// <param name="gender">The gender type for the group.  By default the group will be coed.</param>
        /// <param name="maritalStatus">The marital status for the group.  By default the group will be married or single.</param>
        /// <param name="hasChildcare">Specifies if the group provides childcare.  By default the group will not provide childcare.</param>
        /// <param name="isSearchable">Specifies if the group is searchable. By default the group will be searchable.</param>
        /// <param name="campusName">The name of the campus for this group.  By default no campus will tied to this group.</param>
        /// <param name="isPublic">Specifies if this group is public and accessible in InFellowship.</param>
        public void Groups_Group_Create(int churchId, string groupTypeName, string individualName, string groupName, string groupDescription, string startDate,            
            [Optional, DefaultParameterValue(GeneralEnumerations.Gender.Coed)] 
            GeneralEnumerations.Gender gender, [Optional, DefaultParameterValue(GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle)] 
            GeneralEnumerations.GroupMaritalStatus maritalStatus, [Optional, DefaultParameterValue(false)] bool hasChildcare, 
            [Optional, DefaultParameterValue(true)] bool isSearchable, [Optional, DefaultParameterValue(null)] string campusName, 
            [Optional, DefaultParameterValue(false)] bool isPublic, [Optional, DefaultParameterValue(0)] int startAgeRange, 
            [Optional, DefaultParameterValue(0)] int endAgeRange)
        {

            // Store the campus ID 
            var campusID = 0;
            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
            
            // If a campus name is provided, look up that campus and get the id
            if (!string.IsNullOrEmpty(campusName)) {
                campusID = Convert.ToInt16(this.Execute(string.Format("SELECT ChurchCampusID FROM ChmPeople.dbo.ChurchCampus WITH (NOLOCK) WHERE ChurchId = {0} and CampusName = '{1}'", churchId, campusName)).Rows[0]["ChurchCampusID"]);
            }
        
            StringBuilder createGroupQuery = new StringBuilder("DECLARE @Group_Type_ID INT ");
            createGroupQuery.AppendFormat("SET @Group_Type_ID = (SELECT TOP 1 Group_Type_ID FROM ChmPeople.dbo.Group_Type WITH (NOLOCK) WHERE Church_ID = {0} AND Group_Type_Name = '{1}' AND Deleted_Date IS NULL) ", churchId, groupTypeName);
            createGroupQuery.Append("DECLARE @Individual_ID INT ");
            //createGroupQuery.AppendFormat("SET @Individual_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}') ", churchId, individualName);
            createGroupQuery.AppendFormat("SET @Individual_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}') ", churchId, name[0], name[1]);
            


            // If there is a campus, specify the ChurchCampusID
            if (!string.IsNullOrEmpty(campusName)) {

                createGroupQuery.Append("INSERT INTO ChmPeople.dbo.Groups (Church_ID, Group_Type_ID, Group_Name, Description, Is_Open, Is_Public, Start_Date, Created_By_Individual_ID, Gender_ID, Marital_Status_ID, HasChildcare, IsSearchable, ChurchCampusID, Start_Age_Range, End_Age_Range, Created_Date) ");
                
                if (startAgeRange > 0 && endAgeRange > 0) {
                    createGroupQuery.AppendFormat("VALUES ({0}, @Group_Type_ID, '{1}', '{2}', 1, {3}, '{4}', @Individual_ID, {5}, {6}, {7}, {8}, {9}, {10}, {11}, '{12}')", churchId, groupName, groupDescription, Convert.ToInt16(isPublic), startDate, (int)gender, (int)maritalStatus, Convert.ToInt16(hasChildcare), Convert.ToInt16(isSearchable), campusID, startAgeRange, endAgeRange, startDate);
                }
                else {
                    createGroupQuery.AppendFormat("VALUES ({0}, @Group_Type_ID, '{1}', '{2}', 1, {3}, '{4}', @Individual_ID, {5}, {6}, {7}, {8}, {9}, NULL, NULL, '{10}')", churchId, groupName, groupDescription, Convert.ToInt16(isPublic), startDate, (int)gender, (int)maritalStatus, Convert.ToInt16(hasChildcare), Convert.ToInt16(isSearchable), campusID, startDate);
                }

                this.Execute(createGroupQuery.ToString());
            }

            else {
                createGroupQuery.Append("INSERT INTO ChmPeople.dbo.Groups (Church_ID, Group_Type_ID, Group_Name, Description, Is_Open, Is_Public, Start_Date, Created_By_Individual_ID, Gender_ID, Marital_Status_ID, HasChildcare, IsSearchable, Start_Age_Range, End_Age_Range, Created_Date) ");
                if (startAgeRange > 0 && endAgeRange > 0) {
                    createGroupQuery.AppendFormat("VALUES ({0}, @Group_Type_ID, '{1}', '{2}', 1, {3}, '{4}', @Individual_ID, {5}, {6}, {7}, {8}, {9}, {10}, '{11}')", churchId, groupName, groupDescription, Convert.ToInt16(isPublic), startDate, (int)gender, (int)maritalStatus, Convert.ToInt16(hasChildcare), Convert.ToInt16(isSearchable), startAgeRange, endAgeRange, startDate);
                }
                else {
                    createGroupQuery.AppendFormat("VALUES ({0}, @Group_Type_ID, '{1}', '{2}', 1, {3}, '{4}', @Individual_ID, {5}, {6}, {7}, {8}, NULL, NULL, '{9}')", churchId, groupName, groupDescription, Convert.ToInt16(isPublic), startDate, (int)gender, (int)maritalStatus, Convert.ToInt16(hasChildcare), Convert.ToInt16(isSearchable), startDate);
                }
                this.Execute(createGroupQuery.ToString());
            }

            //Temp Group Update
            // If Temporary Group set exp date [ExpirationDate] = 2014-04-08 16:07:39.903
            // Add a year to keep it simple since we are going to delete it anyway. Or we should be
            if (groupTypeName == "Temporary")
            {

                string expDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddYears(1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                int tempGrpId = this.Groups_GroupTypes_FetchID(churchId, "Temporary");

                StringBuilder query = new StringBuilder();
                query.Append("UPDATE [ChmPeople].[dbo].[Groups] ");
                query.AppendFormat("SET [ExpirationDate] =  '{0}' ", expDate);
                query.AppendFormat("WHERE Church_ID = {0} and Group_Type_ID = {1} and Group_Name = '{2}'", churchId, tempGrpId, groupName);
                this.Execute(query.ToString());
            }

            //Set Time Zone
            if (churchId == 15)
            {
                int grpId = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);

                StringBuilder query = new StringBuilder();
                query.Append("UPDATE [ChmPeople].[dbo].[Groups] ");
                query.AppendFormat("SET [Time_Zone_ID] =  {0} ", 115);
                query.AppendFormat("WHERE Church_ID = {0} and Group_Type_ID = {1} and Group_Name = '{2}' and Deleted_Date IS NULL ", churchId, grpId, groupName);
                this.Execute(query.ToString());

            }

            //SOC Updates
            var groupId = this.Groups_Group_FetchID(churchId, groupName);
            var groupTypeId = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);
            this.Groups_SOC_Lookup_Proc(churchId, "GroupID", groupId);
            this.Groups_SOC_Lookup_Proc(churchId, "GroupTypeID", groupTypeId);

            TestLog.WriteLine("Create Group Query: {0}", createGroupQuery);

        }

        /// <summary>
        /// Adds a leader or member to a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The group the leader or member will belong to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <param name="leaderOrMember">Leader or member.</param>
        /// <param name="createDate">Create Date</param>
        /// <param name="joinDate">Join Date</param>
        public void Groups_Group_AddLeaderOrMember(int churchId, string groupName, string individualName, string leaderOrMember, string createDate = null, string joinDate = null) {

            // Figure out if we are adding a leader or a member
            var groupMemberSystemTypeValue = leaderOrMember == "Leader" ? 1 : 2;

            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
            
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT, @individualID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            //query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND FIRST_NAME = '{0}' AND LAST_NAME = '{1}') ", name[0], name[1]);            
            query.AppendFormat("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = {0}) ", groupMemberSystemTypeValue);
            query.AppendFormat("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = '{0}' AND Deleted_Date IS NULL) ", groupName);


            if(!string.IsNullOrEmpty(createDate) && (string.IsNullOrEmpty(joinDate)))
            {
                query.Append("INSERT INTO ChmPeople.dbo.Group_Member (Group_ID, Church_ID, Individual_ID, Group_Member_Type_ID, Created_By_Individual_ID, Created_Date) ");
                query.AppendFormat("VALUES (@groupID, @churchID, @individualID, @groupMemberTypeID, @individualID, '{0}')", createDate);

            }
            else if (string.IsNullOrEmpty(createDate) && (!string.IsNullOrEmpty(joinDate)))
            {
                query.Append("INSERT INTO ChmPeople.dbo.Group_Member (Group_ID, Church_ID, Individual_ID, Group_Member_Type_ID, Created_By_Individual_ID, JoinDate) ");
                query.AppendFormat("VALUES (@groupID, @churchID, @individualID, @groupMemberTypeID, @individualID, '{0}')", joinDate);

            }
            else if (!string.IsNullOrEmpty(createDate) && (!string.IsNullOrEmpty(joinDate)))
            {
                query.Append("INSERT INTO ChmPeople.dbo.Group_Member (Group_ID, Church_ID, Individual_ID, Group_Member_Type_ID, Created_By_Individual_ID, Created_Date, JoinDate) ");
                query.AppendFormat("VALUES (@groupID, @churchID, @individualID, @groupMemberTypeID, @individualID, '{0}', '{1}')", createDate, joinDate);

            }
            else
            {
                query.Append("INSERT INTO ChmPeople.dbo.Group_Member (Group_ID, Church_ID, Individual_ID, Group_Member_Type_ID, Created_By_Individual_ID) ");
                query.Append("VALUES (@groupID, @churchID, @individualID, @groupMemberTypeID, @individualID)");

            }

            this.Execute(query.ToString());
        }

        /// <summary>
        /// Updates a person in the group to a leader/member.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="individualName">The name of the individual in the group.</param>
        /// <param name="leaderOrMember">The new role for the individual</param>
        public void Groups_Group_UpdateLeaderOrMemberRole(int churchId, string groupName, string individualName, string leaderOrMember) {
            // Figure out if we are adding a leader or a member
            var groupMemberSystemTypeValue = leaderOrMember == "Leader" ? 1 : 2;
            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));


            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT, @individualID INT, @groupMemberTypeID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            //query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND FIRST_NAME = '{0}' AND LAST_NAME = '{1}') ", name[0], name[1]);
            query.AppendFormat("SET @groupMemberTypeID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = {0}) ", groupMemberSystemTypeValue);
            query.AppendFormat("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = '{0}' AND Deleted_Date IS NULL) ", groupName);
            query.Append("UPDATE ChmPeople.dbo.Group_Member ");
            query.Append("SET Group_Member_Type_ID = @groupMemberTypeID ");
            query.Append("WHERE Group_ID = @groupID and Church_ID = @churchID and Individual_ID = @individualID");
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates a group type, sets an individual as an admin, and applies the appropriate permissions.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <param name="groupTypeName">The name of the group type.</param>
        /// <param name="leaderAdminRights">An array of rights to be configured.</param>
        /// <param name="isPublic">Specifies if this group type is enabled in InFellowship.</param>
        /// <param name="isSearchable">Specifies if groups under this group type are searchable.</param>
        //public void Groups_CreateGroupType(int churchId, string individualName, string groupTypeName, Portal.Groups.GroupsEnumerations.GroupTypeLeaderAdminRights[] leaderAdminRights) {
        public void Groups_GroupType_Create(int churchId, string individualName, string groupTypeName, List<int> leaderAdminRights, 
            [Optional, DefaultParameterValue(true)] bool isPublic, [Optional, DefaultParameterValue(true)] bool isSearchable) {

            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
            
            // Add the leader view right
            leaderAdminRights.Add(7);

            // Build the query
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @individualID INT, @groupTypeID INT, @groupMemberTypeLeaderID INT, @groupMemberTypeMemberID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            //Cleaned up query since individual name is not indexed
            //query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND FIRST_NAME = '{0}' AND LAST_NAME = '{1}') ", name[0], name[1]);
                                                   //SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = 15        AND FIRST_NAME = 'Matthew' AND LAST_NAME = 'Sneeden'
            query.Append("SET @groupMemberTypeLeaderID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 1) ");
            query.Append("SET @groupMemberTypeMemberID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 2) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Type (Church_ID, Group_Type_Name, Description, Created_By_Individual_ID, Is_Web_Enabled, Is_Searchable, GroupSystemTypeID) ");
            query.AppendFormat("VALUES (@churchID, '{0}', null, @individualID, {1}, {2}, 3) ", groupTypeName, Convert.ToInt16(isPublic), Convert.ToInt16(isSearchable));
            query.Append("SET @groupTypeID = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @individualID, @individualID, 1) ");

            // Append the insert statements for each security right.
            for (int i = 0; i < leaderAdminRights.Count; i++) {
                if (leaderAdminRights[i] == 11 || leaderAdminRights[i] == 3) {
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    // Members can view the roster / email the group
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeMemberID, {0}, @individualID) ", leaderAdminRights[i]));
                }
                query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", leaderAdminRights[i]));
            }

            // Execute the query
            this.Execute(query.ToString());

            //SOC Updates   
            var groupTypeId = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);
            
            this.Groups_SOC_Lookup_Proc(churchId, "GroupTypeID", groupTypeId);

        }

        /// <summary>
        /// Creates a group type with permissions.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The individual who is creating the group type.</param>
        /// <param name="groupTypeName">The name of the group type.</param>
        /// <param name="leaderAdminRights">The admin rights for leaders you wish to enable for the group type.</param>
        /// <param name="leaderViewRights">The view rights for leaders you wish to enable for the group type.</param>
        /// <param name="memberAdminRights">The admin rights for memberes you wish to enable for the group type.</param>
        /// <param name="memberViewRights">The view rights for members you wish to enable for the group type.</param>
        public void Groups_CreateGroupType(int churchId, string individualName, string groupTypeName, List<GeneralEnumerations.GroupTypeLeaderAdminRights> leaderAdminRights, GeneralEnumerations.GroupTypeLeaderViewRights leaderViewRight, List<GeneralEnumerations.GroupTypeMemberAdminRights> memberAdminRights, GeneralEnumerations.GroupTypeMemberViewRights memberViewRight) {

            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
            
            // Build the query
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @individualID INT, @groupTypeID INT, @groupMemberTypeLeaderID INT, @groupMemberTypeMemberID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            //query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND INDIVIDUAL_NAME = '{0}') ", individualName);
            query.AppendFormat("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND FIRST_NAME = '{0}' AND LAST_NAME = '{1}') ", name[0], name[1]);
            query.Append("SET @groupMemberTypeLeaderID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 1) ");
            query.Append("SET @groupMemberTypeMemberID = (SELECT Group_Member_Type_ID FROM ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Member_System_Type_ID = 2) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Type (Church_ID, Group_Type_Name, Description, Created_By_Individual_ID, Is_Web_Enabled, Is_Searchable, GroupSystemTypeID) ");
            query.AppendFormat("VALUES (@churchID, '{0}', null, @individualID, 1, 1, 3) ", groupTypeName);
            query.Append("SET @groupTypeID = (SELECT SCOPE_IDENTITY()) ");
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.Append("VALUES (@churchID, @groupTypeID, @individualID, @individualID, 1) ");

            // Leader admin rights
            foreach (var leaderAdminRight in leaderAdminRights) {
                switch (leaderAdminRight) {
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.None:
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.EmailGroup:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 3));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.InviteSomeone:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 1));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.EditRecords:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 2));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.EditDetails:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 9));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.ChangeScheduleLocation:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 4));
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 5));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.TakeAttendance:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 10));
                        break;
                    case GeneralEnumerations.GroupTypeLeaderAdminRights.AddSomeone:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 12));
                        break;
                    default:
                        break;
                }
            }

            // Leader view rights
            switch (leaderViewRight) {
                case GeneralEnumerations.GroupTypeLeaderViewRights.NotSpecified:
                    break;
                case GeneralEnumerations.GroupTypeLeaderViewRights.Limited:
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 6));
                    break;
                case GeneralEnumerations.GroupTypeLeaderViewRights.Basic:
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 7));
                    break;
                case GeneralEnumerations.GroupTypeLeaderViewRights.Full:
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeLeaderID, {0}, @individualID) ", 8));
                    break;
                default:
                    break;
            }

            // Member admin rights
            foreach (var memberAdminRight in memberAdminRights) {
                switch (memberAdminRight) {
                    case GeneralEnumerations.GroupTypeMemberAdminRights.EmailGroup:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeMemberID, {0}, @individualID) ", 3));
                        break;
                    case GeneralEnumerations.GroupTypeMemberAdminRights.ViewRoster:
                        query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                        query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeMemberID, {0}, @individualID) ", 11));
                        break;
                    default:
                        break;
                }
            }

            // Member view rights
            switch (memberViewRight) {
                case GeneralEnumerations.GroupTypeMemberViewRights.NotSpecified:
                    break;
                case GeneralEnumerations.GroupTypeMemberViewRights.Limited:
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeMemberID, {0}, @individualID) ", 6));
                    break;
                case GeneralEnumerations.GroupTypeMemberViewRights.Basic:
                    query.Append("INSERT INTO ChmPeople.dbo.GroupTypeMemberTypePermission (ChurchID, GroupTypeID, MemberTypeID, PermissionID, CreatedByIndividualID) ");
                    query.Append(string.Format("VALUES (@churchID, @groupTypeID, @groupMemberTypeMemberID, {0}, @individualID) ", 7));
                    break;
                default:
                    break;
            }

            // Execute the query
            this.Execute(query.ToString());


            //SOC Updates
            var groupTypeId = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);
            this.Groups_SOC_Lookup_Proc(churchId, "GroupTypeID", groupTypeId);
        }

        /// <summary>
        /// Deletes all Attendance Summaries for a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>s
        public void Groups_Group_DeleteAttendanceSummary(int churchId, string groupName) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @groupID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.Append(string.Format("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND group_name = '{0}' and deleted_date is null) ", groupName));
            query.Append("delete from ChmPeople.dbo.Attendancesummary WHERE AttendanceContextValueId = @groupid");
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates an attendance record for an individual.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group that the individual attendned.</param>
        /// <param name="individual">The name of the individual you are adding the record for.</param>
        /// <param name="didIndividualAttend">Specifies if the individual was there or not.</param>
        public void Groups_Group_CreateAttendanceRecord(int churchId, string groupName, string individual, int didIndividualAttend) {

            string[] name = individual.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
            
            StringBuilder query = new StringBuilder();
            query.Append("DECLARE @attendancecontext_value_id INT ");
            query.Append("DECLARE @attendance_summary_id INT ");
            query.Append("DECLARE @individualID INT ");

            // Get the individual id
            //query.Append(string.Format("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_NAME = '{1}') ", churchId, individual));
            query.Append(string.Format("SET @individualID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}') ", churchId, name[0], name[1]));

            // Get the attendance summary
            query.Append(string.Format("SET @attendancecontext_value_id = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups where Church_ID = {0} and Group_Name = '{1}') ", churchId, groupName));
            query.Append(string.Format("SET @attendance_summary_id = (SELECT TOP 1 attendancesummaryid FROM ChmPeople.dbo.attendancesummary WHERE ChurchId = {0} and attendancecontextvalueid = @attendancecontext_value_id) ", churchId));

            // Create the Attendance Record
            query.Append("INSERT INTO ChmPeople.dbo.Attendance (churchid, individualid, attendancesummaryid, attendancedate, isPresent) ");
            query.Append(string.Format("VALUES({0}, @individualID, @attendance_summary_id, getdate(), {1})", churchId, didIndividualAttend));

            this.Execute(query.ToString());
        }

        /// <summary>
        /// Deletes a group type in the database along with all groups tied to it.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The group type name.</param>
        public void Groups_GroupType_Delete(int churchId, string groupTypeName) {
            TestLog.WriteLine(string.Format("Delete Group Type -- ChurchID: {0} GroupTypeName: {1} ", churchId, groupTypeName));
            DataTable groupTypes = this.Execute(string.Format("SELECT Group_Type_ID FROM ChmPeople.dbo.Group_Type WITH (NOLOCK) WHERE Church_ID = {0} AND Group_Type_Name = '{1}' AND Deleted_Date IS NULL", churchId, groupTypeName));
            if (groupTypes.Rows.Count > 0) {
                this.Execute(string.Format("UPDATE ChmPeople.dbo.Groups SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Type_ID = '{2}' AND Deleted_Date IS NULL", this._individualID, churchId, groupTypes.Rows[0]["Group_Type_ID"]));
                this.Execute(string.Format("UPDATE ChmPeople.dbo.Group_Type SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Type_Name = '{2}' AND Deleted_Date IS NULL", this._individualID, churchId, groupTypeName));
            }
        }

        /// <summary>
        /// Deletes a group from the database.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The group name.</param>
        public void Groups_Group_Delete(int churchId, string groupName) {
            this.Execute(string.Format("UPDATE ChmPeople.dbo.Groups SET Deleted_Date = CURRENT_TIMESTAMP, Deleted_By_Individual_ID = {0} WHERE Church_ID = {1} AND Group_Name = '{2}' AND Deleted_Date IS NULL", this._individualID, churchId, groupName));
            
            //SOC Updates
            try
            {
                int groupId = this.Groups_Group_FetchID(churchId, groupName);
                this.Groups_SOC_Lookup_Proc(churchId, "GroupID", groupId);
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with Groups Group Delete: {0}", e.Message);
            }

        }

        /// <summary>
        /// This method returns the most recent activation code for the specified user.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>The activation code for the user.</returns>
        public string Groups_FetchUserActivationCode(int churchId, string email) {
            return this.Execute(string.Format("SELECT * FROM ChmPeople.dbo.UserLoginActivation WITH (NOLOCK) WHERE ChurchID = {0} AND Email = '{1}' ORDER BY CreatedDate DESC", churchId, email)).Rows[0]["ActivationCode"].ToString();
        }

        public string Groups_GetProspectActivationCode(int churchID, string email, string groupName) {
            // Get the group id
            var groupID = this.Execute(string.Format("SELECT TOP 1 * FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE CHURCH_ID = {0} AND Group_Name = '{1}' AND DELETED_DATE IS NULL", churchID, groupName)).Rows[0]["Group_ID"];

            StringBuilder query = new StringBuilder("SELECT TOP 1 CT_ID.Email, ULAC.ActionCode, TASK.GroupId FROM [ChmPeople].[dbo].[CommunicationTarget] CT_ID WITH (NOLOCK) ");
            query.Append("JOIN [ChmPeople].[dbo].[Task] TASK with (NOLOCK) ");
            query.Append("ON CT_ID.CommunicationTargetID = TASK.CommunicationTargetID and CT_ID.ChurchID = TASK.ChurchID ");
            query.Append("JOIN [ChmPeople].[dbo].[UserLoginActionCode] ULAC with (NOLOCK) ");
            query.Append("ON TASK.TaskID = ULAC.ContextTypeDataID ");
            query.AppendFormat("WHERE ULAC.ChurchID = {0} and Email = '{1}' AND TASK.GroupID = {2} ORDER BY ULAC.CreatedDate DESC", churchID, email, groupID);

            int rowCount = 0;
            // Do not attempt to retrieve the value until it's there
            Retry.WithTimeout(30000).WithPolling(500).DoBetween(() => rowCount = this.Execute(query.ToString()).Rows.Count).Until(() => rowCount > 0);

            return this.Execute(query.ToString()).Rows[0]["ActionCode"].ToString();
        }

        /// <summary>
        /// Gets the activation code for a prospect.
        /// </summary>
        /// <param name="churchID"></param>
        /// <param name="email"></param>
        /// <param name="individualName"></param>
        /// <param name="groupName"></param>
        /// <returns>The activation code for the prospect</returns>
        public string Groups_GetProspectActivationCode(int churchID, string email, string individualName, string groupName) {
            // Get the group id
            var groupID = this.Execute(string.Format("SELECT TOP 1 * FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE CHURCH_ID = {0} AND Group_Name = '{1}' AND DELETED_DATE IS NULL", churchID, groupName)).Rows[0]["Group_ID"];

            // Get the individual id
            var individualID = this.People_Individuals_FetchID(churchID, individualName);

            StringBuilder query = new StringBuilder("SELECT TOP 1 CT_ID.Email, ULAC.ActionCode, TASK.GroupId FROM [ChmPeople].[dbo].[CommunicationTarget] CT_ID WITH (NOLOCK) ");
            query.Append("JOIN [ChmPeople].[dbo].[Task] TASK with (NOLOCK) ");
            query.Append("ON CT_ID.CommunicationTargetID = TASK.CommunicationTargetID and CT_ID.ChurchID = TASK.ChurchID ");
            query.Append("JOIN [ChmPeople].[dbo].[UserLoginActionCode] ULAC with (NOLOCK) ");
            query.Append("ON TASK.TaskID = ULAC.ContextTypeDataID ");
            query.AppendFormat("WHERE ULAC.ChurchID = {0} and Email = '{1}' AND TASK.GroupID = {2} and CT_ID.IndividualID = {3} ORDER BY ULAC.CreatedDate DESC", churchID, email, groupID, individualID);

            int rowCount = 0;
            // Do not attempt to retrieve the value until it's there
            Retry.WithTimeout(30000).WithPolling(500).DoBetween(() => rowCount = this.Execute(query.ToString()).Rows.Count).Until(() => rowCount > 0);

            return this.Execute(query.ToString()).Rows[0]["ActionCode"].ToString();
        }

        /// <summary>
        /// Updates a group member's join date to a set number of dates in the past or future.
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="individual">The individual you wish to update.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="addOrSubtract">Operation you wish to perform.  Use "+" or "-"</param>
        /// <param name="numberOfDays">The number of days you wish to adjust.</param>
        public void Groups_Group_UpdateMemberJoinDate(int churchId, string individual, string groupName, string addOrSubtract, int numberOfDays) {
            var operation = "+";

            if (addOrSubtract == "-") {
                operation = "-";
            }
            // Update group member's createddate, e.g. join date
            StringBuilder query = new StringBuilder("declare @individualid int ");
            query.Append("declare @groupid int ");
            query.Append(string.Format("set @groupid = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups where group_name = '{0}' and Deleted_Date is null and church_id = {1}) ", groupName, churchId));
            query.Append(string.Format("set @individualid = (SELECT TOP 1 Individual_ID FROM ChmPeople.dbo.Individual WITH (NOLOCK) where Individual_Name = '{0}' and church_id = {1}) ", individual, churchId));

            query.Append("UPDATE ChmPeople.dbo.Group_Member ");
            query.Append(string.Format("SET Created_Date = GETDATE(){0}{1} ", operation, numberOfDays));
            query.Append(string.Format("WHERE Group_ID = @groupid and Individual_ID = @individualid and church_id = {0}", churchId));
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates an unposted attendance record for a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="individualsInGroup">A collection of individuals in the group.</param>
        /// <param name="isPosted">Specifies if the attendance date is posted.</param>
        /// <param name="hasEndTime">Specifies if you want to have an end time for the attendance posting.</param>
        /// <param name="attendanceHistoryDays">The number of attendance days in past or future (i.e. -5 is 5 days past from NOW() date, 5 days in future from NOW() date</param>
        /// <returns>The date that will be present in the drop down.</returns>
        public string Groups_Group_CreateAttendanceRecord(int churchId, string groupName, List<string> individualsInGroup, bool isPosted, 
            [Optional, DefaultParameterValue(false)] bool hasEndTime, string startDate = null, string startTime = null) {

            // Initial Items
            var totalMembers = individualsInGroup.Count;

            StringBuilder query = new StringBuilder("DECLARE @churchid INT ");

            // Variables;
            query.Append("DECLARE @groupid INT ");
            query.Append("DECLARE @group_name VARCHAR(50) ");
            query.Append("DECLARE @attendancedate DATETIME ");
            query.Append("DECLARE @endTimeForDate DATETIME ");
            query.Append("DECLARE @attendancesummaryid INT ");
            query.Append("DECLARE @individualid INT ");
            query.Append("DECLARE @attendanceID INT ");
            query.Append("DECLARE @groupmembertypeid INT ");
            query.Append("DECLARE @groupmembersystemtypeid INT ");
            query.Append("DECLARE @daDate DATETIME ");

            // Set variables
            query.AppendFormat("SET @churchid = {0} ", churchId);
            query.AppendFormat("SET @group_name = '{0}' ", groupName);
            query.Append("SET @groupid = (SELECT group_id FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE group_name = @group_name AND church_id = @churchid AND deleted_date is null) ");


            if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(startTime))
            {
                //Set Attendance Date with current datetime
                query.Append("SET @attendancedate = GETDATE() ");
            }
            else
            { 
                //Hard code attendance date using passed in variables
                //query.AppendFormat("SET @attendancedate = DATEADD(dd,{0},@daDate) ", attendanceHistoryDays);            
                query.AppendFormat("DECLARE @smalldatetime smalldatetime = '{0} {1}' ", startDate, startTime);
                query.Append("SET @attendancedate = @smalldatetime ");
            }

            query.Append("SET @endTimeForDate = DATEADD(hh,5,@attendancedate) ");

            // Create the attendance summary record
            query.Append("INSERT INTO ChmPeople.dbo.AttendanceSummary (churchid, attendancecontexttypeid, attendancecontextvalueid, startdatetime, enddatetime, presentCount, absentcount, totalcount, met, comments, isPosted, isScheduled) ");
            if (hasEndTime) {
                query.AppendFormat("VALUES (@churchid, 1, @groupid, @attendancedate, @endTimeForDate, 0, 0, {0}, 0, '', {1}, 1) ", totalMembers, Convert.ToInt16(isPosted));
            }
            else {
                query.AppendFormat("VALUES (@churchid, 1, @groupid, @attendancedate, @attendancedate, 0, 0, {0}, 0, '', {1}, 1) ", totalMembers, Convert.ToInt16(isPosted));
            }

            // Create the attendance record and a group attendance detail record for each individual
            query.Append("SET @attendancesummaryid = (SELECT TOP 1 AttendanceSummaryId from ChmPeople.dbo.AttendanceSummary WITH (NOLOCK) WHERE AttendanceContextValueID = @groupid and churchid = @churchid) ");
            foreach (var individual in individualsInGroup) {

                // Attendance record for the individual
                query.AppendFormat("SET @individualid = (SELECT ChmPeople.dbo.Individual.individual_id FROM ChmPeople.dbo.Individual WITH (NOLOCK) INNER JOIN ChmPeople.dbo.Group_Member ON ChmPeople.dbo.Individual.church_id = @churchid AND ChmPeople.dbo.Individual.individual_name = '{0}' AND ChmPeople.dbo.Individual.individual_id = ChmPeople.dbo.Group_Member.individual_id AND ChmPeople.dbo.Group_Member.Group_ID = @groupid)", individual);
                query.Append("INSERT INTO ChmPeople.dbo.Attendance (churchid, individualid, attendancesummaryid, attendancedate, ispresent) ");
                query.Append("VALUES (@churchid, @individualid, @attendancesummaryid, @attendancedate, 0) ");

                // GroupAttendanceDetail record for the individual
                //query.Append("SET @attendanceID = (SELECT TOP 1 attendanceid FROM ChmPeople.dbo.Attendance WITH (NOLOCK) WHERE churchid = @churchid AND individualid = @individualid AND attendancesummaryid = @attendancesummaryid AND attendancedate = @attendancedate order by CreatedDate DESC)");
                query.Append("SET @attendanceID = (SELECT TOP 1 attendanceid FROM ChmPeople.dbo.Attendance WITH (NOLOCK) WHERE churchid = @churchid AND individualid = @individualid AND attendancesummaryid = @attendancesummaryid AND attendancedate = @attendancedate)");
                query.Append("SET @groupmembertypeid = (SELECT Group_Member_Type_ID from ChmPeople.dbo.Group_Member WITH (NOLOCK) WHERE Group_ID = @groupid AND Church_ID = @churchid and Individual_ID = @individualid) ");
                query.Append("SET @groupmembersystemtypeid = (SELECT Group_Member_System_Type_ID from ChmPeople.dbo.Group_Member_Type WITH (NOLOCK) WHERE Church_ID = @churchid and group_member_type_id = @groupmembertypeid) ");

                query.Append("INSERT INTO ChmPeople.dbo.GroupAttendanceDetail (churchid, attendanceid, groupmembersystemtypeid) ");
                query.Append("VALUES (@churchid, @attendanceID, @groupmembersystemtypeid) ");
            }

            // Execute
            this.Execute(query.ToString());

            // Get the date generated
            StringBuilder query2 = new StringBuilder("DECLARE @churchid INT ");
            query2.Append("DECLARE @group_name VARCHAR(50) ");
            query2.Append("DECLARE @groupid INT ");
            query2.AppendFormat("SET @churchid = {0} ", churchId);
            query2.AppendFormat("SET @group_name = '{0}' ", groupName);
            query2.Append("SET @groupid = (SELECT group_id FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE group_name = @group_name AND church_id = @churchid AND deleted_date is null) ");
            query2.Append("SELECT startdatetime FROM ChmPeople.dbo.AttendanceSummary WITH (NOLOCK) WHERE ChurchId = @churchid and AttendanceContextValueID = @groupid");
            DateTime date = Convert.ToDateTime(this.Execute(query2.ToString()).Rows[0]["StartDateTime"]);

            // Formate the date so it matches the value to be selected in the drop down
            string attendanceDate = string.Empty;
            if (hasEndTime) {
                attendanceDate = string.Format("{0} at {1} - {2}", date.ToString("dddd, MMMM dd, yyyy"), date.ToString("h:mm tt"), date.AddHours(5).ToString("h:mm tt"));
            }
            else {
                attendanceDate = string.Format("{0} at {1}", date.ToString("dddd, MMMM dd, yyyy"), date.ToString("h:mm tt"));
            }

            return attendanceDate;
        }


        /// <summary>
        /// Creates a Span of Care.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="spanOfCareName">The name of the span of care.</param>
        /// <param name="individualName">The name of the individual creating the span of care.</param>
        /// <param name="ownerName">The owner of the span of care.</param>
        /// <param name="groupTypesInSpanOfCare">A list of groups to be included in the span of care. Any groups under this group types will automatically fall in to the Span of Care through DB triggers.</param>
        /// <param name="campusNames">An optional list of campuses that define this span of care.  By default no campuses will be used.</param>
        /// <param name="maritalStatusTypes">An optional marital status that defines this span of care.  By default, marital status will not be used to define this span of care.</param>
        /// <param name="genderType">An optional marital status that defines this span of care.  By default, gender will not be used to define this span of care.</param>
        /// <param name="hasChildCare">An optional childcare setting that defines this span of care.  By default, childcare will not be used to define this span of care.</param>
        public void Groups_SpanOfCare_Create(int churchId, string spanOfCareName, string individualName, string ownerName, List<string> groupTypesInSpanOfCare, 
            [Optional, DefaultParameterValue(null)] List<string> campusNames, 
            [Optional, DefaultParameterValue(GeneralEnumerations.Gender.NA)] GeneralEnumerations.Gender genderType, 
            [Optional, DefaultParameterValue(GeneralEnumerations.GroupMaritalStatus.NA)] GeneralEnumerations.GroupMaritalStatus maritalStatusType, [Optional, DefaultParameterValue(GeneralEnumerations.SpanOfCareChildCareSettings.NA)] GeneralEnumerations.SpanOfCareChildCareSettings hasChildCare) {
            // Store who is creating this SOC
            var createdByIndividualID = this.People_Individuals_FetchID(churchId, individualName);
            
            // Create the Span of Care
            StringBuilder queryCreate = new StringBuilder();
            queryCreate.Append("INSERT INTO ChmPeople.dbo.GroupSoc (churchid, groupSocName, createdByIndividualID, createdByLogin) ");
            queryCreate.AppendFormat("VALUES ({0}, '{1}', {2}, 'chm_system')", churchId, spanOfCareName, createdByIndividualID);

            this.Execute(queryCreate.ToString());

            // Store the ID
            var groupSocID = this.Groups_SpanOfCare_FetchID(churchId, spanOfCareName);

            // Add an owner, if one is provided
            if (!string.IsNullOrEmpty(ownerName)) {
                StringBuilder queryAddOwner = new StringBuilder("DECLARE @ownerIndividualID INT ");
                queryAddOwner.AppendFormat("SET @ownerIndividualID = {0} ", this.People_Individuals_FetchID(churchId, ownerName));
                queryAddOwner.Append("INSERT INTO ChmPeople.dbo.GroupSoc_Owner (churchid, groupSocID, individualID, CreatedDate, createdByIndividualID, createdByLogin) ");
                queryAddOwner.AppendFormat("VALUES ({0}, {1}, @ownerIndividualID, GETDATE(), {2}, 'chm_system')", churchId, groupSocID, createdByIndividualID);

                this.Execute(queryAddOwner.ToString());
            }

            // Add the group types
            StringBuilder queryAddGroupTypes = new StringBuilder();
            foreach (var groupType in groupTypesInSpanOfCare) {
                queryAddGroupTypes.Append("INSERT INTO ChmPeople.dbo.GroupSoc_GroupType (churchid, groupSocID, groupTypeID, CreatedDate, createdByIndividualID, createdByLogin) ");
                queryAddGroupTypes.AppendFormat("VALUES ({0}, {1}, {2}, GETDATE(), {3}, 'chm_system') ", churchId, groupSocID, this.Groups_GroupTypes_FetchID(churchId, groupType), createdByIndividualID);
            }

            this.Execute(queryAddGroupTypes.ToString());

            // Standard fields
            StringBuilder queryAddStandardFields = new StringBuilder();

            // Add the campuses
            var campusID = 0;

            if (campusNames != null) {
                foreach (var campusName in campusNames) {
                    campusID = Convert.ToInt16(this.Execute(string.Format("SELECT ChurchCampusID FROM ChmPeople.dbo.ChurchCampus WITH (NOLOCK) WHERE ChurchId = {0} and CampusName = '{1}'", churchId, campusName)).Rows[0]["ChurchCampusID"]);
                    queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_ChurchCampus (churchid, groupSocID, ChurchCampusID, CreatedDate, createdByIndividualID, createdByLogin) ");
                    queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, {2}, GETDATE(), {3}, 'chm_system') ", churchId, groupSocID, campusID, createdByIndividualID);
                }
            }

            // Add the gender settings
            queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_Gender (churchid, groupSocID, GenderID, CreatedDate, createdByIndividualID, createdByLogin) ");
            queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, {2}, GETDATE(), {3}, 'chm_system') ", churchId, groupSocID, Convert.ToInt16(genderType), createdByIndividualID);

            // Add the marital status settings
            queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_MaritalStatus (churchid, groupSocID, MaritalStatusID, CreatedDate, createdByIndividualID, createdByLogin) ");
            queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, {2}, GETDATE(), {3}, 'chm_system') ", churchId, groupSocID, Convert.ToInt16(maritalStatusType), createdByIndividualID);

            // Add the child care setting
            if (hasChildCare == GeneralEnumerations.SpanOfCareChildCareSettings.NA) {
                queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_HasChildCare(churchid, groupSocID, HasChildCareId, CreatedDate, createdByIndividualID, createdByLogin) ");
                queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, 0, GETDATE(), {2}, 'chm_system') ", churchId, groupSocID, createdByIndividualID);
                queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_HasChildCare(churchid, groupSocID, HasChildCareId, CreatedDate, createdByIndividualID, createdByLogin) ");
                queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, 1, GETDATE(), {2}, 'chm_system') ", churchId, groupSocID, createdByIndividualID);
            }
            else {
                queryAddStandardFields.Append("INSERT INTO ChmPeople.dbo.GroupSoc_HasChildCare(churchid, groupSocID, HasChildCareId, CreatedDate, createdByIndividualID, createdByLogin) ");
                queryAddStandardFields.AppendFormat("VALUES ({0}, {1}, {2}, GETDATE(), {3}, 'chm_system') ", churchId, groupSocID, Convert.ToInt16(hasChildCare), createdByIndividualID);
            }


            this.Execute(queryAddStandardFields.ToString());

            //SOC Updates
            this.Groups_SOC_Lookup_Proc(churchId, "GroupSocID", groupSocID);

        }

        /// <summary>
        ///  Deletes a Span of Care.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="spanOfCareName">The name of the Span of Care.</param>
        public void Groups_SpanOfCare_Delete(int churchId, string spanOfCareName) {
            StringBuilder query = new StringBuilder("UPDATE ChmPeople.dbo.GroupSoc SET DeletedDate = GETDATE() ");
            query.AppendFormat("WHERE ChurchId = {0} and GroupSocName = '{1}'", churchId, spanOfCareName);
            this.Execute(query.ToString());



        }

        /// <summary>
        /// Gets the span of care id for a Span of Care.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="spanOfCareName">The name of the span of care.</param>
        /// <returns>The id for the span of care.</returns>
        public int Groups_SpanOfCare_FetchID(int churchId, string spanOfCareName) {
            return Convert.ToInt32(this.Execute(string.Format("SELECT GroupSocID from ChmPeople.dbo.GroupSoc WITH (NOLOCK) WHERE ChurchID = {0} and GroupSocName = '{1}' and DeletedDate is null", churchId, spanOfCareName)).Rows[0]["GroupSocID"]);
        }

        /// <summary>
        /// Gets the group type id for a Group Type
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The name of the group type.</param>
        /// <returns>The id for the group type.</returns>
        public int Groups_GroupTypes_FetchID(int churchId, string groupTypeName) {

            return Convert.ToInt32(this.Execute(string.Format("SELECT Group_Type_ID from ChmPeople.dbo.Group_Type WITH (NOLOCK) WHERE Church_ID = {0} and Group_Type_Name = '{1}' and Deleted_Date is null", churchId, groupTypeName)).Rows[0]["Group_Type_ID"]);
        }

        /// <summary>
        /// Gets the group type id for a Group Type
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeId">The name of the group type.</param>
        /// <returns>The group type custom field id for the group type.</returns>
        public int Groups_GroupTypes_FetchGroupTypeCustomFieldID(int churchId, int groupTypeId)
        {
            return Convert.ToInt32(this.Execute(string.Format("SELECT GroupTypeCustomFieldID from ChmPeople.dbo.GroupTypeCustomField WITH (NOLOCK) WHERE ChurchID = {0} and GroupTypeID = '{1}' and DeletedDate is null", churchId, groupTypeId)).Rows[0]["GroupTypeCustomFieldID"]);
        }

        /// <summary>
        /// Gets the custom field id for a Group Type
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeId">The name of the group type.</param>
        /// <returns>The custom field id for the group type.</returns>
        public int Groups_GroupTypes_FetchCustomFieldID(int churchId, int groupTypeId)
        {
            return Convert.ToInt32(this.Execute(string.Format("SELECT CustomFieldID from ChmPeople.dbo.GroupTypeCustomField WITH (NOLOCK) WHERE ChurchID = {0} and GroupTypeID = '{1}' and DeletedDate is null", churchId, groupTypeId)).Rows[0]["CustomFieldID"]);
        }

        /// <summary>
        /// Gets the custom field id for a Custom Field.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="customFieldName">The name of the custom field.</param>
        /// <returns>The id for the custom field.</returns>
        public int Groups_CustomField_FetchID(int churchId, string customFieldName) {
            try {
                return Convert.ToInt32(this.Execute(string.Format("SELECT CustomFieldID from ChmPeople.dbo.CustomField WITH (NOLOCK) WHERE ChurchID = {0} and CustomFieldText = '{1}' and DeletedDate is null", churchId, customFieldName)).Rows[0]["CustomFieldID"]);
            }
            catch (System.IndexOutOfRangeException) {
                return 0;
            }
        }


        /// <summary>
        /// Fetches a group id for a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns></returns>
        public int Groups_Group_FetchID(int churchId, string groupName) {
            return Convert.ToInt32(this.Execute(string.Format("SELECT TOP 1 Group_ID from ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = {0} and Group_Name = '{1}' and Deleted_Date is null", churchId, groupName)).Rows[0]["Group_ID"]);
        }

        /// <summary>
        /// Fetches a group type id for a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns></returns>
        public int Groups_Group_FetchGroupTypeID(int churchId, string groupName)
        {
            return Convert.ToInt32(this.Execute(string.Format("SELECT TOP 1 Group_Type_ID from ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = {0} and Group_Name = '{1}' and Deleted_Date is null", churchId, groupName)).Rows[0]["Group_Type_ID"]);
        }

        /// <summary>
        /// Gets the ID for a custom field's option
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="customFieldName">The name of the custom field that has the option.</param>
        /// <param name="optionValue">The name of option.</param>
        /// <returns>The id for the option.</returns>
        public int Groups_CustomFieldOption_FetchID(int churchId, string customFieldName, string optionValue) {
            int customFieldID = this.Groups_CustomField_FetchID(churchId, customFieldName);
            return Convert.ToInt32(this.Execute(string.Format("SELECT OptionFieldID from ChmPeople.dbo.OptionField WITH (NOLOCK) WHERE ChurchID = {0} and CustomFieldID = {1} and OptionText = '{2}' and DeletedDate is null", churchId, customFieldID, optionValue)).Rows[0]["OptionFieldID"]);
        }

        /// <summary>
        /// Creates a custom field.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="customFieldType">The type of custom field.</param>
        /// <param name="customFieldName">The name of the custom field.</param>
        /// <param name="customFieldDescription">The description of the customn field.</param>
        /// <param name="customFieldChoices"></param>
        public void Groups_CustomField_Create(int churchId, GeneralEnumerations.CustomFieldType customFieldType, string customFieldName, string customFieldDescription, List<string> customFieldChoices) {
            var dataTypeID = customFieldType == GeneralEnumerations.CustomFieldType.MultiSelect ? 6 : 8;
            var customFieldID = 0;

            // Create the custom field
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ChmPeople.dbo.CustomField (ChurchId, DataTypeID, CustomFieldText, OptionalText, SpanOfCare, Search) ");
            query.AppendFormat("VALUES ({0}, {1}, '{2}', '{3}', 0, 0)", churchId, dataTypeID, customFieldName, customFieldDescription);
            this.Execute(query.ToString());

            // Get the id
            customFieldID = this.Groups_CustomField_FetchID(churchId, customFieldName);

            // Create the choices.  Store a counter for the sort order column.  The order will be the same as the order in the list.
            var counter = 0;
            StringBuilder query2 = new StringBuilder();
            foreach (var choice in customFieldChoices) {
                query2.Append("INSERT INTO ChmPeople.dbo.OptionField (churchid, customfieldid, optiontext, isdefault, sortorder) ");
                query2.AppendFormat("VALUES ({0}, {1}, '{2}', 0, {3}) ", churchId, customFieldID, choice, counter);
                counter++;
            }

            this.Execute(query2.ToString());

        }

        /// <summary>
        ///  Deletes a custom field.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="customFieldName">The name of the custom field.</param>
        public void Groups_CustomField_Delete(int churchId, string customFieldName) {
            // TODO Use PROC to DELETE
            // ChmPeople.dbo.CustomField_Destroy @ChurchID = ###, @CustomFieldID = ###   
            var customFieldID = this.Groups_CustomField_FetchID(churchId, customFieldName);

            // Delete the options and the custom field, if it exists
            if (customFieldID > 0) {
                StringBuilder query = new StringBuilder();
                query.Append("UPDATE ChmPeople.dbo.OptionField ");
                query.Append("SET DeletedDate = GETDATE() ");
                query.AppendFormat("WHERE ChurchID = {0} and CustomFieldID = {1}", churchId, customFieldID);

                this.Execute(query.ToString());

                // Delete the custom field
                StringBuilder query2 = new StringBuilder();
                query2.Append("UPDATE ChmPeople.dbo.CustomField ");
                query2.Append("SET DeletedDate = GETDATE() ");
                query2.AppendFormat("WHERE ChurchID = {0} and CustomFieldID = {1}", churchId, customFieldID);

                this.Execute(query2.ToString());
            }
        }

        /// <summary>
        /// Adds a custom field to a group type.  This will make the custom field have the status of "in use".
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The group type name to gain the custom field.</param>
        /// <param name="customFieldName">The name of the custom field to be added to the group type.</param>
        /// <param name="individualName">The individual adding this custom field.</param>
        /// <param name="defaulFieldValue">The default custom field value for this group type.  This is optional.</param>
        public void Groups_GroupTypes_AddCustomField(int churchId, string groupTypeName, string customFieldName,[Optional, DefaultParameterValue(null)] string individualName, [Optional, DefaultParameterValue(null)] string defaulFieldValue) {
            var customFieldID = this.Groups_CustomField_FetchID(churchId, customFieldName);
            var groupTypeID = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);

            // Add the custom field to the group type
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ChmPeople.dbo.GroupTypeCustomField (ChurchID, CustomFieldID, GroupTypeID, SortOrder, IsActive) ");
            query.AppendFormat("VALUES ({0}, {1}, {2}, 999, 1)", churchId, customFieldID, groupTypeID);

            this.Execute(query.ToString());

            // Create a record in the option field list table.  Insert the default value into the option field value table.
            if (!string.IsNullOrEmpty(defaulFieldValue)) {
                StringBuilder optionFieldValueListQuery = new StringBuilder();
                optionFieldValueListQuery.Append("INSERT INTO ChmPeople.dbo.OptionFieldValueList (churchid) ");
                optionFieldValueListQuery.AppendFormat("VALUES ({0}) ", churchId);
                optionFieldValueListQuery.Append("SELECT SCOPE_IDENTITY() ");

                var optionFieldValueListID = this.Execute(optionFieldValueListQuery.ToString()).Rows[0][0];
                var optionFieldID = this.Groups_CustomFieldOption_FetchID(churchId, customFieldName, defaulFieldValue);
                var individualID = this.People_Individuals_FetchID(churchId, individualName);

                StringBuilder query2 = new StringBuilder();
                query2.Append("INSERT INTO ChmPeople.dbo.OptionFieldValue (ChurchID, OptionFieldValueListID, OptionFieldID, CustomFieldID, CreatedByIndividualID) ");
                query2.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4}) ", churchId, optionFieldValueListID, optionFieldID, customFieldID, individualID);
                this.Execute(query2.ToString());

                // Set the group type to have the option field list value that points to the default value
                StringBuilder query3 = new StringBuilder();
                query3.Append("UPDATE ChmPeople.dbo.GroupTypeCustomField ");
                query3.AppendFormat("SET OptionFieldValueListID = {0} ", optionFieldValueListID);
                query3.AppendFormat("WHERE ChurchID = {0} and GroupTypeID = {1}", churchId, groupTypeID);

                this.Execute(query3.ToString());
            }
        }

        /// <summary>
        /// Grants a linked portal user manager or viewer rights to a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupTypeName">The name of the group type name the group belongs to.</param>
        /// <param name="groupName">The name of the group that the individual will gain rights to.</param>
        /// <param name="individualName">The name of the individual to gain rights.</param>
        /// <param name="groupRole">The portal security right the individual will gain.</param>
        public void Groups_Group_AddManagerOrViewer(int churchId, string groupTypeName, string groupName, string individualName, 
            GeneralEnumerations.GroupRolesPortalUser groupRole,
            string createdBy = "Bryan Mikaelian") 
        {
            var groupTypeID = this.Groups_GroupTypes_FetchID(churchId, groupTypeName);
            var groupID = this.Groups_Group_FetchID(churchId, groupName);
            var individualID = this.People_Individuals_FetchID(churchId, individualName);
            var createdByIndividualID = this.People_Individuals_FetchID(churchId, createdBy);

            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ChmPeople.dbo.Group_Admin (Church_ID, Group_Type_ID, Group_ID, Individual_ID, Created_By_Individual_ID, Group_Admin_System_Type_ID) ");
            query.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4}, {5}) ", churchId, groupTypeID, groupID, individualID, createdByIndividualID, Convert.ToInt16(groupRole));

            this.Execute(query.ToString());
        }

        /// <summary>
        /// Creates a people list.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="peopleListName">The name of the people list.</param>
        /// <param name="peopleListDescription">The description of the people list.</param>
        /// <param name="indiviudalName">The name of the individual creating the people list.</param>
        public void Groups_PeopleLists_Create(int churchId, string peopleListName, string peopleListDescription, string individualName, [Optional, DefaultParameterValue(false)] bool isViewableByAll) {
            var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToShortDateString();
            var groupTypeName = "People List";

            // Create a "Group"
            this.Groups_Group_Create(churchId, groupTypeName, individualName, peopleListName, peopleListDescription, startDate, GeneralEnumerations.Gender.Coed, GeneralEnumerations.GroupMaritalStatus.MarriedOrSingle, false, false, null, isViewableByAll);
        }

        /// <summary>
        /// Deletes a people list.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="peopleListName">The name of the people list.</param>
        public void Groups_PeopleLists_Delete(int churchId, string peopleListName) {
            // Delete a "Group"
            this.Groups_Group_Delete(churchId, peopleListName);
        }

        /// <summary>
        /// Adds a manager or a viewer to a people list.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="peopleListName">The name of the people list.</param>
        /// <param name="individualName">The name of the individual to gain the role.</param>
        /// <param name="role">The role to give the individual.</param>
        public void Groups_PeopleLists_AddManagerOrViewer(int churchId, string peopleListName, string individualName, GeneralEnumerations.GroupRolesPortalUser role) {
            // Add a manager or a leader to a "Group"
            this.Groups_Group_AddManagerOrViewer(churchId, "People List", peopleListName, individualName, role);
        }

        /// <summary>
        /// Adds an individual to a people list.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="peopleListName">The name of the people list.</param>
        /// <param name="individualName">The name of the individual to be added.</param>
        public void Groups_PeopleLists_AddMember(int churchId, string peopleListName, string individualName) {
            this.Groups_Group_AddLeaderOrMember(churchId, peopleListName, individualName, "Member");
        }


        /// <summary>
        /// Adds a prospect to a group.  The individual does not have to exist.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="firstName">The first name of the prospect.</param>
        /// <param name="lastName">The last name of the prospect.</param>
        /// <param name="email">The email address of the prospect.</param>
        /// <param name="phone">The phone number of the prospect.</param>
        /// <param name="status">The status of the prospect.</param>
        public void Groups_Group_AddProspect(int churchId, string groupName, string firstName, string lastName, string email, string phone, GeneralEnumerations.ProspectTaskStates status) {
            var individualId = 0;
            try {
                individualId = this.People_Individuals_FetchID(churchId, string.Format("{0} {1}", firstName, lastName));
            }
            catch (System.IndexOutOfRangeException) {
            }

            var groupId = this.Groups_Group_FetchID(churchId, groupName);

            // Create the communication target
            StringBuilder query = new StringBuilder("DECLARE @communicationTargetID INT ");
            query.Append("INSERT INTO ChmPeople.dbo.CommunicationTarget (churchid, individualid, groupid, firstname, lastname, email, phone) ");
            if (individualId > 0) {
                query.AppendFormat("VALUES ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}') ", churchId, individualId, groupId, firstName, lastName, email, phone);
            }
            else {
                query.AppendFormat("VALUES ({0}, NULL, {1}, '{2}', '{3}', '{4}', '{5}') ", churchId, groupId, firstName, lastName, email, phone);
            }
            query.Append("SELECT SCOPE_IDENTITY() ");

            var communicationTargetID = Convert.ToInt32(this.Execute(query.ToString()).Rows[0][0]);

            // Create the task records

            // Prospect
            StringBuilder query2 = new StringBuilder();
            query2.Append("INSERT INTO ChmPeople.dbo.Task (churchid, TaskTypeID, TaskStatusID, groupid, communicationtargetid, taskname) ");
            // Declined has the same TaskStatusID as Denied but not the same Task Event ID
            if (status == GeneralEnumerations.ProspectTaskStates.Declined) {
                query2.AppendFormat("VALUES ({0}, 2, 8, {1}, {2}, 'Automated Test Generated Task') ", churchId, groupId, communicationTargetID);
            }
            else {
                query2.AppendFormat("VALUES ({0}, 2, {1}, {2}, {3}, 'Automated Test Generated Task') ", churchId, Convert.ToInt16(status), groupId, communicationTargetID);
            }
            query2.Append("SELECT SCOPE_IDENTITY() ");

            var taskID = Convert.ToInt32(this.Execute(query2.ToString()).Rows[0][0]);

            // Communication
            StringBuilder query3 = new StringBuilder();
            query3.Append("INSERT INTO ChmPeople.dbo.Task (churchid, TaskTypeID, TaskStatusID, ParentTaskID, groupid, communicationtargetid, taskname) ");
            query3.AppendFormat("VALUES ({0}, 1, 1, {1}, {2}, {3}, 'New Communication Task') ", churchId, taskID, groupId, communicationTargetID);
            this.Execute(query3.ToString());

            // Create the task event record based on the status.  Some status require two task events.
            StringBuilder query4 = new StringBuilder();
            if (status == GeneralEnumerations.ProspectTaskStates.ExpressInterest) {
                var taskEventID1 = 13;
                var taskEventID2 = 14;
                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID1, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID1);
                }

                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID2, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID2);
                }
            }
            else if (status == GeneralEnumerations.ProspectTaskStates.Denied || status == GeneralEnumerations.ProspectTaskStates.Declined) {
                var taskEventID1 = status == GeneralEnumerations.ProspectTaskStates.Denied ? 17 : 21;
                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID1, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID1);
                }
            }
            else if (status == GeneralEnumerations.ProspectTaskStates.Joined) {
                var taskEventID1 = 20;
                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID1, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID1);
                }
            }
            // Invited
            else {
                var taskEventID1 = 16;
                var taskEventID2 = 18;
                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID1, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID1);
                }

                query4.Append("INSERT INTO ChmPeople.dbo.TaskEvent(churchid, taskid, TaskEventTypeID, IndividualID, IsConfidential) ");
                if (individualId > 0) {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}, 0) ", churchId, taskID, taskEventID2, individualId);
                }
                else {
                    query4.AppendFormat("VALUES ({0}, {1}, {2}, NULL, 0) ", churchId, taskID, taskEventID2);
                }
            }

            this.Execute(query4.ToString());
        }


        /// <summary>
        /// Creates a schedule for a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="reoccurence">The schedule reoccurence</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="individualName">The individual creating the schedule.</param>
        /// <param name="scheduleStartDate">The start date for the schedule.</param>
        /// <param name="scheduleStartTime">The start time for the schedule.</param>
        /// <param name="scheduleEndDate">The end date for the schedule.  This is optional.</param>
        /// <param name="scheduleEndTime">The end time for the schedule.  This is optional.</param>
        /// <param name="occursOnDays">The days the schedule occurs on. This is only used for Weekly Schedules.  By Default, the schedule will occur on all days.</param>
        public void Groups_Group_CreateSchedule(int churchId, GeneralEnumerations.GroupScheduleFrequency reoccurence, string groupName, string individualName, string scheduleStartDate, string scheduleStartTime, string scheduleEndDate, string scheduleEndTime, [Optional, DefaultParameterValue(null)] List<GeneralEnumerations.WeeklyScheduleDays> occursOnDays) {
            var groupID = this.Groups_Group_FetchID(churchId, groupName);
            var individualID = this.People_Individuals_FetchID(churchId, individualName);

            // Create the schedule
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ChmPeople.dbo.Schedule (ChurchID, RecurrenceTypeID, ScheduleName, StartTime, EndTime, IsShared, StartDate, EndDate, CreatedByIndividualID) ");
            if (string.IsNullOrEmpty(scheduleEndDate) && string.IsNullOrEmpty(scheduleEndTime)) {
                query.AppendFormat("VALUES ({0}, {1}, 'Default Schedule 1', '{2}', NULL, 0, '{3}', NULL, {4}) ", churchId, Convert.ToInt16(reoccurence), scheduleStartTime, scheduleStartDate, individualID);
            }
            else {
                query.AppendFormat("VALUES ({0}, {1}, 'Default Schedule 1', '{2}', '{3}', 0, '{4}', '{5}', {6}) ", churchId, Convert.ToInt16(reoccurence), scheduleStartTime, scheduleEndTime, scheduleStartDate, scheduleEndDate, individualID);
            }
            query.Append("SELECT SCOPE_IDENTITY() ");

            var scheduleID = this.Execute(query.ToString()).Rows[0][0];

            // Create the event
            StringBuilder query2 = new StringBuilder();
            query2.Append("INSERT INTO ChmPeople.dbo.Events (ChurchID, EventName, Description, EventTypeID, CreatedByIndividualID) ");
            query2.AppendFormat("VALUES ({0}, '{1} - Default Event', 'Default event for group: {2}', 0, {3}) ", churchId, groupName, groupName, individualID);
            query2.Append("SELECT SCOPE_IDENTITY() ");

            var eventID = this.Execute(query2.ToString()).Rows[0][0];

            // Create the group event
            StringBuilder query3 = new StringBuilder();
            query3.Append("INSERT INTO ChmPeople.dbo.GroupEvent (ChurchID, EventID, GroupID, CreatedByIndividualID) ");
            query3.AppendFormat("VALUES ({0}, {1}, {2}, {3}) ", churchId, eventID, groupID, individualID);
            this.Execute(query3.ToString());

            // Create the event schedule
            StringBuilder query4 = new StringBuilder();
            query4.Append("INSERT INTO ChmPeople.dbo.EventSchedule (ChurchID, EventID, ScheduleID, CreatedByIndividualID) ");
            query4.AppendFormat("VALUES ({0}, {1}, {2}, {3}) ", churchId, eventID, scheduleID, individualID);
            this.Execute(query4.ToString());

            // Set up the recurrence
            if (reoccurence == GeneralEnumerations.GroupScheduleFrequency.Weekly) {
                StringBuilder query5 = new StringBuilder();
                query5.Append("INSERT INTO ChmPeople.dbo.RecurrenceWeekly (ChurchID, ScheduleID, RecurrenceFrequency, OccurOnSunday, OccurOnMonday, OccurOnTuesday, OccurOnWednesday, OccurOnThursday, OccurOnFriday, OccurOnSaturday, CreatedByIndividualID) ");
                // If we didn't specify any days, all days will be selected
                if (occursOnDays == null) {
                    query5.AppendFormat("VALUES ({0}, {1}, 1, 1, 1, 1, 1, 1, 1, 1, {2})", churchId, scheduleID, individualID);
                }
                else {
                    var sunday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Sunday) ? 1 : 0;
                    var monday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Monday) ? 1 : 0;
                    var tuesday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Tuesday) ? 1 : 0;
                    var wednesday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Wednesday) ? 1 : 0;
                    var thursday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Thursday) ? 1 : 0;
                    var friday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Friday) ? 1 : 0;
                    var saturday = occursOnDays.Contains(GeneralEnumerations.WeeklyScheduleDays.Saturday) ? 1 : 0;

                    query5.AppendFormat("VALUES ({0}, {1}, 1, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})", churchId, scheduleID, sunday, monday, tuesday, wednesday, thursday, friday, saturday, individualID);
                }

                this.Execute(query5.ToString());
            }
        }

        /// <summary>
        /// Deletes Temporary Group by updating the exp date. Let's the system clean things up for us.
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <param name="tempGrpName">Temmporary Group Name</param>
        public void Groups_Temporary_Delete(int churchId, string tempGrpName)
        {

            int tempGrpId = this.Groups_GroupTypes_FetchID(churchId, "Temporary");

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE [ChmPeople].[dbo].[Groups] ");
            query.Append("SET [ExpirationDate] = GetDate() ");
            query.AppendFormat("WHERE Church_ID = {0} and Group_Type_ID = {1} and Group_Name = '{2}'", churchId, tempGrpId, tempGrpName);

            try
            {
                DataTable dt = this.Execute(query.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error [{0}] Deleting Temporary Group: [{1}] for Church ID [{2}]", e.Message, tempGrpName, churchId));   
            }

        }

        /// <summary>
        /// Gets the task id for a prospect
        /// </summary>
        /// <param name="churchID"></param>
        /// <param name="noteText"></param>
        /// <returns></returns>
        public int Groups_Prospect_GetTaskId(int churchId, string noteText)
        {           
            return Convert.ToInt32(this.Execute(string.Format("SELECT TaskID FROM ChmPeople.dbo.TaskEvent WHERE ChurchID = {0} AND Note = '{1}'", churchId, noteText)).Rows[0]["TaskID"]);
        }


        /// <summary>
        /// This method sets Prospect tasks to expired. 
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="noteText"></param>
        public string Groups_Prospect_Expire_Created_Date(int churchId, string noteText)
        {
            int taskId = this.Groups_Prospect_GetTaskId(churchId, noteText);
            TestLog.WriteLine(taskId);

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).AddDays(-30);
            string expiredDate = now.ToString("yyyy-MM-dd HH:mm:ss.000");

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE [ChmPeople].[dbo].[TaskEvent] ");
            query.AppendFormat("SET [CreatedDate] = '{0}' ", expiredDate);
            query.AppendFormat("WHERE [ChurchID] = {0} AND [TaskID] = {1} ", churchId, taskId);
            this.Execute(query.ToString());

            StringBuilder query2 = new StringBuilder();
            query2.Append("UPDATE [ChmPeople].[dbo].[Task] ");
            query2.AppendFormat("SET [CreatedDate] = '{0}' ", expiredDate);
            query2.AppendFormat("WHERE [ChurchID] = {0} AND [TaskID] = {1} ", churchId, taskId);
            this.Execute(query2.ToString());

            StringBuilder query3 = new StringBuilder();
            query3.Append("UPDATE [ChmPeople].[dbo].[Task] ");
            query3.AppendFormat("SET [CreatedDate] = '{0}' ", expiredDate);
            query3.AppendFormat("WHERE [ChurchID] = {0} AND [ParentTaskID] = {1} ", churchId, (taskId));
            this.Execute(query3.ToString());

            StringBuilder query4 = new StringBuilder();
            query4.Append("UPDATE [ChmPeople].[dbo].[TaskIndividual] ");
            query4.AppendFormat("SET [CreatedDate] = '{0}' ", expiredDate);
            query4.AppendFormat("WHERE [ChurchID] = {0} AND [TaskID] = {1} ", churchId, taskId);
            this.Execute(query4.ToString());

            /*
            try
            {
                DataTable dt = this.Execute(query.ToString());
                DataTable dt2 = this.Execute(query2.ToString());
                DataTable dt3 = this.Execute(query3.ToString());
                DataTable dt4 = this.Execute(query4.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error: [{0}] Failed to update Task: [{1}] for Church ID [{2}]", e.Message, taskId, churchId));
            }
            */

            this.ExecuteDBProc("ChmPeople.dbo.Prospects_ExpireProspectTasks");
            Thread.Sleep(TimeSpan.FromSeconds(5));

            log.DebugFormat("Expired Date: {0}", now.ToShortTimeString());
            return now.ToShortTimeString();
        }

        #endregion Groups

        #region People
        /// <summary>
        /// This method fetches the individual id for an individual.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <returns>Integer representing the individual.</returns>
        public int People_Individuals_FetchID(int churchId, string individualName) {

            string[] name = individualName.Split(' ');
            
                TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
                //return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND Individual_Name = '{1}'", churchId, individualName).Rows[0]["INDIVIDUAL_ID"]);
                return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' AND STATUS_ID != 45960 ORDER BY CREATED_DATE desc", churchId, name[0], name[1]).Rows[0]["INDIVIDUAL_ID"]);           
        }

        /// <summary>
        /// This method fetches the individual id for an individual.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <returns>Integer representing the individual.</returns>
        public string[] People_Individuals_FetchIDList(int churchId, string individualName)
        {

            string[] name = individualName.Split(' ');
            TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));

            DataTable dt = new DataTable();

            dt = this.Execute("SELECT INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' ORDER BY CREATED_DATE desc", churchId, name[0], name[1]);

            string[] result = new string[dt.Rows.Count];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index]["INDIVIDUAL_ID"].ToString();
            }

            return result;
        }

        public int People_Individuals_FetchID_ByEmail(int churchId, string email)
        {
            StringBuilder query = new StringBuilder("SELECT TOP 1 IndividualID FROM [ChmPeople].[dbo].[UserLogin] WITH (NOLOCK) ");
            query.AppendFormat("WHERE [ChurchID] = {0} AND [Email] = '{1}'", churchId, email);
            return Convert.ToInt32(this.Execute(query.ToString()).Rows[0]["IndividualID"].ToString());
        }

        /// <summary>
        /// This method fetches the individual id for an individual.
        /// </summary>
        /// <param name="churchId">Church Id</param>
        /// <param name="firstName">First Name</param>
        /// <param name="LastName">Last Name</param>
        /// <returns></returns>
        public int People_Individuals_FetchID(int churchId, string firstName, string LastName)
        {

            DataTable dt = new DataTable();

            dt = this.Execute("SELECT INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' order by last_update desc", churchId, firstName, LastName);

            if (dt.Rows.Count > 1)
            {
                TestLog.WriteLine("Warning: There are multiple record existent with given individual name.");
            }
            else if (dt.Rows.Count == 0)
            {
                return -1;
            }

            return Convert.ToInt32(dt.Rows[0]["INDIVIDUAL_ID"]);

        }

        /// <summary>
        /// Fetch Status Id for a particular Individual Status
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <param name="statusName">Status Name</param>
        /// <returns></returns>
        public int People_Individual_FetchStatusID(int churchId, string statusName)
        {
            DataTable dtStatusId = new DataTable();
            StringBuilder queryStatusId = new StringBuilder("SELECT [STATUS_ID], [CHURCH_ID], [STATUS_NAME], [ENABLED] FROM [ChmPeople].[dbo].[STATUS] WITH (NOLOCK) ");
            queryStatusId.AppendFormat("WHERE (church_id = 0 or church_id = {0}) and status_name = '{1}'", churchId, statusName);
            dtStatusId = this.Execute(queryStatusId.ToString());

            if (dtStatusId.Rows.Count < 0)
            {
                throw new Exception(string.Format("{0} does not exist for church id [{1}]", statusName, churchId));
            }
            else
            {
                return Convert.ToInt32(dtStatusId.Rows[0]["STATUS_ID"]);
            }

        }

        /// <summary>
        /// This method fetches the household id for an individual.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <returns>Integer representing the household.</returns>
        public int People_Households_FetchID(int churchId, int individualId) {
            if (individualId > 0)
            {
                return Convert.ToInt32(this.Execute("SELECT TOP 1 HOUSEHOLD_ID FROM ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD WITH (NOLOCK) WHERE INDIVIDUAL_ID = {0} AND CHURCH_ID = {1}", individualId, churchId).Rows[0]["HOUSEHOLD_ID"]);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// This method  Fetches UNIQUE_ID for an individual.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <returns>Integer representing the individual.</returns>
        public string People_Individuals_UniqueID(int churchId, int individualID)
        {

            //return Convert.ToString(this.Execute("SELECT TOP 1 UNIQUE_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualID));
            return Convert.ToString(this.Execute(string.Format("SELECT TOP 1 UNIQUE_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualID)));
            
            
        }

        /// <summary>
        /// Update Status for an Individual
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <param name="statusName">Status Name to Change to</param>
        /// <param name="firstName">Individual First Name</param>
        /// <param name="lastName">Individual Last Name</param>
        public void People_Individual_Update_Status(int churchId, string statusName, string firstName, string lastName)
        {
            StringBuilder updateStatus = new StringBuilder();
            int statusId = this.People_Individual_FetchStatusID(churchId, statusName);
            int individualId = this.People_Individuals_FetchID(churchId, firstName, lastName);

            updateStatus.Append("UPDATE ChmPeople.dbo.Individual ");
            updateStatus.AppendFormat("SET STATUS_ID = {0} " , statusId);
            updateStatus.AppendFormat("WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId);
            this.Execute(updateStatus.ToString());

        }

        /// <summary>
        /// This method delete the individual record with given parameters
        /// </summary>
        /// <param name="churchId">Church Id</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <returns></returns>
        public void People_Individual_Delete(int churchId, string firstName, string lastName)
        {
            if ((firstName + lastName).Equals("FT Tester") || (lastName + firstName).Equals("FT Tester"))
            {
                Assert.Fail("FT Tester SHOULD NEVER BE MERGED");
            }

            int individualId = People_Individuals_FetchID(churchId, firstName, lastName);

            if (individualId == -1)
            {
                TestLog.WriteLine("No individual with given name found from DB, no need to delete");
;
                return;
            }

            try
            {
                this.Execute(string.Format("DELETE FROM ChmPeople.dbo.INDIVIDUAL WHERE Church_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
            }
            catch (Exception)
            {
                this.Execute(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
                this.Execute(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_COMMUNICATION WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
                this.Execute(string.Format("DELETE FROM ChmPeople.dbo.Group_Member WHERE Church_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
                this.Execute(string.Format("DELETE FROM ChmPeople.dbo.INDIVIDUAL WHERE Church_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
            }
        }


        /// <summary>
        /// This method delete the individual record with given parameters
        /// </summary>
        /// <param name="churchId">Church Id</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <returns></returns>
        public void People_Individual_DeleteBulk(int churchId, string firstName, string lastName)
        {
            if ((firstName + lastName).Equals("FT Tester") || (lastName + firstName).Equals("FT Tester"))
            {
                Assert.Fail("FT Tester SHOULD NEVER BE MERGED");
            }

            int individualId = People_Individuals_FetchID(churchId, firstName, lastName);

            while (individualId != -1)
            {
                try
                {
                    this.Execute("DELETE FROM ChmPeople.dbo.INDIVIDUAL WHERE Church_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId);
                }
                catch (Exception)
                {
                    this.Execute(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));
                    this.Execute(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_COMMUNICATION WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId));

                    this.Execute("DELETE FROM ChmPeople.dbo.INDIVIDUAL WHERE Church_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId);
                }

                individualId = People_Individuals_FetchID(churchId, firstName, lastName);
            }
            
        }

        /// <summary>
        /// This method get the column value of individual with given parameters
        /// </summary>
        /// <param name="churchId">Church Id</param>
        /// <param name="individualId">Individual Id of one individual</param>
        /// <param name="columnName">Column Name</param>
        /// <returns></returns>
        public String People_Individual_FetchColumnValue(int churchId, int individualId, string columnName)
        {
            DataTable dt = new DataTable();

            dt = this.Execute("SELECT TOP 1 * FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} order by Created_Date desc", churchId, individualId);

            if (dt.Rows.Count > 1)
            {
                throw new Exception(string.Format("There were [{0}] individual(s) found in church id {1} by the individual_id {2}", dt.Rows.Count, churchId, individualId));
            }
            else
            {
                return Convert.ToString(dt.Rows[0][columnName]);
            }
        }

        /// <summary>
        /// This method Fetches Xml Request to CIA for an individual for background check.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <returns>Integer representing the individual.</returns>
        public string People_Individuals_xmlRequest(int individualID)
        {

            return Convert.ToString(this.Execute(string.Format("SELECT TOP 1 REPORT_XML FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE  INDIVIDUAL_ID = {0}", individualID)));


        }

        /// <summary>
        /// Deletes a prospect from a group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupName">The name of the group the prospect is tied to.</param>
        /// <param name="firstName">The prospect's first name.</param>
        /// <param name="lastName">The prospect's last name.</param>
        /// <param name="email">The prospect's email.</param>
        public void People_DeleteProspectInfo(int churchId, string groupName, string firstName, string lastName, string email) {
            StringBuilder script = new StringBuilder("DECLARE @churchID INT, @groupName VARCHAR(50), @groupID INT, @firstName VARCHAR(25), @lastName VARCHAR(25), @email VARCHAR(50) ");
            script.AppendFormat("SET @churchID = {0} ", churchId);
            script.AppendFormat("SET @groupName = '{0}' ", groupName);
            script.Append("SET @groupID = (SELECT TOP 1 Group_ID FROM ChmPeople.dbo.Groups WITH (NOLOCK) WHERE Church_ID = @churchID AND Group_Name = @groupName AND Deleted_Date IS NULL) ");
            script.AppendFormat("SET @firstName = '{0}' ", firstName);
            script.AppendFormat("SET @lastName = '{0}' ", lastName);
            script.AppendFormat("SET @email = '{0}' ", email);
            script.Append("CREATE TABLE #TaskIDs (ID INT IDENTITY(1,1), task_ID INT) ");
            script.Append("INSERT INTO #TaskIDs (task_ID) SELECT TaskID FROM ChmPeople.dbo.Task WITH (NOLOCK) WHERE ChurchID = @churchID AND CommunicationTargetID IN (SELECT CommunicationTargetID FROM ChmPeople.dbo.CommunicationTarget WITH (NOLOCK) WHERE ChurchID = @churchID AND GroupID = @groupID AND FirstName = @firstName AND LastName = @lastName AND Email = @email) ");
            script.Append("DECLARE @startid INT, @endid INT, @taskID INT ");
            script.Append("SET @startid = (SELECT MIN(ID) FROM #TaskIDs) ");
            script.Append("SET @endid = (SELECT MAX(ID) FROM #TaskIDs) ");
            script.Append("WHILE (@startid <= @endid) ");
            script.Append("BEGIN ");
            script.Append("SELECT @taskID = task_ID FROM #TaskIDs WHERE ID = @startid ");
            script.Append("DELETE FROM ChmPeople.dbo.TaskIndividual WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.TaskEvent WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.UserLoginActionCode WHERE ChurchID = @churchID AND ContextTypeDataID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.Task WHERE ChurchID = @churchID AND TaskID = @taskID ");
            script.Append("DELETE FROM ChmPeople.dbo.CommunicationTarget WHERE ChurchID = @churchID AND CommunicationTargetID = (SELECT CommunicationTargetID FROM ChmPeople.dbo.Task WITH (NOLOCK) WHERE ChurchID = @churchID AND TaskID = @taskID) ");
            script.Append("SET @startid = @startid + 1 ");
            script.Append("END ");
            script.Append("DROP TABLE #TaskIDs");

            this.Execute(script.ToString());
        }

        /// <summary>
        /// Deletes a template.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="templateName">The name of the template.</param>
        public void People_DeleteTemplate(int churchId, string templateName) {
            this.Execute(string.Format("DELETE FROM ChmEmail.dbo.EMAIL_TEMPLATE WHERE CHURCH_ID = {0} AND TEMPLATE_NAME = '{1}'", churchId, templateName));
        }

        /// <summary>
        /// Deletes a individual.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The id of the individual.</param>
        public void People_DeleteIndividual(int churchId, int individualId)
        {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.Individual WHERE CHURCH_ID = {0} AND individual_id = {1}", churchId, individualId));
        }

        public void People_DeleteIndividual(int churchId, string firstName, string lastName)
        {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.Individual WHERE CHURCH_ID = {0} AND First_Name = '{1}' and Last_Name = '{2}'", churchId, firstName, lastName));
        }

        /// <summary>
        /// Deletes a individual from give household member.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The id of the individual.</param>
        /// <param name="householdId">The id of the individual.</param>
        public void People_DeleteIndividualFromHousehold(int churchId, int individualId, int householdId)
        {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.Individual_Household WHERE CHURCH_ID = {0} AND individual_id = {1} AND household_id = {2}", churchId, individualId, householdId));
        }


        /// <summary>
        /// This method creates an email delegate.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userLogin">The login of the user.</param>
        /// <param name="delegateLogin">The login of the delgate to be created.</param>
        public void People_Delegates_Create(int churchId, string userLogin, string delegateLogin) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @userID INT, @delegateForUserID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @userID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", delegateLogin);
            query.AppendFormat("SET @delegateForUserID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", userLogin);
            query.Append("INSERT INTO ChmEmail.dbo.EMAIL_USER_PROXY (CHURCH_ID, USER_ID, PROXY_FOR_USER_ID, LAST_UPDATED_DATE) ");
            query.Append("VALUES (@churchID, @userID, @delegateForUserID, CURRENT_TIMESTAMP)");
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an email delegate.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userLogin">The login of the user.</param>
        /// <param name="delegateLogin">The login of the delegate to be deleted.</param>
        public void People_Delegates_Delete(int churchId, string userLogin, string delegateLogin) {
            StringBuilder query = new StringBuilder("DECLARE @churchID INT, @userID INT, @delegateForUserID INT ");
            query.AppendFormat("SET @churchID = {0} ", churchId);
            query.AppendFormat("SET @userID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", delegateLogin);
            query.AppendFormat("SET @delegateForUserID = (SELECT TOP 1 USER_ID FROM ChmChurch.dbo.USERS WITH (NOLOCK) WHERE CHURCH_ID = @churchID AND LOGIN = '{0}') ", userLogin);
            query.Append("DELETE FROM ChmEmail.dbo.EMAIL_USER_PROXY WHERE CHURCH_ID = @churchID AND USER_ID = @userID AND PROXY_FOR_USER_ID = @delegateForUserID");
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a people query.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="savedQueryName">The name of the people query.</param>
        public void People_PeopleQuery_Delete(int churchId, string savedQueryName) {
            this.Execute(string.Format("DELETE FROM ChmPortal.dbo.SavedQuery WHERE ChurchID = {0} AND SavedQueryName = '{1}'", churchId, savedQueryName));
        }

        /// <summary>
        /// This method returns the most recent activation code for the specified user.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>The activation code for the user.</returns>
        public string People_FetchUserActivationCode(int churchId, string email) {
            return this.Execute(string.Format("SELECT * FROM ChmPeople.dbo.UserLoginActivation WITH (NOLOCK) WHERE ChurchID = {0} AND Email = '{1}' ORDER BY CreatedDate DESC", churchId, email)).Rows[0]["ActivationCode"].ToString();
        }

        public string People_FetchUserPasswordResetCode(int churchId, string email)
        {
            string userLoginid = this.People_FetchUserLoginId(churchId, email);
            
            StringBuilder query = new StringBuilder("SELECT * FROM [ChmPeople].[dbo].[UserLoginActionCode] WITH (NOLOCK) ");
            query.AppendFormat("WHERE [ChurchID] = {0} AND [UserLoginID] = {1} AND [DateUsed] IS NULL", churchId, userLoginid);
            
            DataTable dt = this.Execute(query.ToString());
            
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["ActionCode"].ToString();
            }
            else
            {
                throw new Exception(string.Format("No Unused Password Reset Code Found for Infellowship user [{0}] in church [{1}]", email, churchId)); 
            }

            
        }

        /// <summary>
        /// Returns the User Login Id for a user
        /// </summary>
        /// <param name="churchId">church id</param>
        /// <param name="email">Infellowship Email</param>
        /// <returns>UserLoginId</returns>
        public string People_FetchUserLoginId(int churchId, string email)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM [ChmPeople].[dbo].[UserLogin] WITH (NOLOCK) ");
            query.AppendFormat("WHERE [ChurchID] = {0} AND [Email] = '{1}'", churchId, email);
            return this.Execute(query.ToString()).Rows[0]["UserLoginId"].ToString();
        }

        /// <summary>
        /// This method fetches the individual and household ids for the given email address.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="email">The email address of the individual.</param>
        /// <returns>DataTable containing the individual and household ids.</returns>
        public DataTable People_GetIndividualHouseholdIdFromEmail(int churchId, string email) {
            StringBuilder query = new StringBuilder("SELECT H.HOUSEHOLD_ID, IN_H.INDIVIDUAL_ID FROM ChmPeople.dbo.HOUSEHOLD H WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD IN_H WITH (NOLOCK) ");
            query.Append("ON H.HOUSEHOLD_ID = IN_H.HOUSEHOLD_ID ");
            query.Append("INNER JOIN ChmPeople.dbo.UserLogin UL WITH (NOLOCK) ");
            query.Append("ON UL.IndividualID = IN_H.INDIVIDUAL_ID ");
            query.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL", churchId, email);

            //StringBuilder filter = new StringBuilder();
            //if (email.Length == 1) {
            //    //filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL)", churchID, emails[0]);
            //    filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email = '{1}' AND UL.DeletedDate IS NULL", churchId, email[0]);
            //}
            //else {
            //    filter.AppendFormat("WHERE UL.ChurchID = {0} AND UL.Email IN (", churchId);

            //    for (int i = 0; i < email.Length; i++) {
            //        filter.AppendFormat("'{0}'", email[i]);
            //        if (i != email.Length - 1) {
            //            filter.Append(", ");
            //        }
            //        else {
            //            //filter.Append(") AND UL.DeletedDate IS NULL)");
            //            filter.Append(") AND UL.DeletedDate IS NULL");
            //        }
            //    }
            //}

            //query.Append(filter.ToString());

            //StringBuilder householdsQuery = new StringBuilder("SELECT H.HOUSEHOLD_ID, IN_H.INDIVIDUAL_ID,UL.Email FROM ChmPeople.dbo.HOUSEHOLD H WITH (NOLOCK) ");
            //householdsQuery.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD IN_H WITH (NOLOCK) ON H.HOUSEHOLD_ID = IN_H.HOUSEHOLD_ID ");
            //householdsQuery.Append("INNER JOIN ChmPeople.dbo.UserLogin UL WITH (NOLOCK) ON UL.IndividualID = IN_H.INDIVIDUAL_ID ");
            //householdsQuery.Append("WHERE UL.ChurchID = 15 AND UL.Email in ");
            //householdsQuery.Append("('msneeden@fellowshiptech.com','ljiang@fellowshiptech.com','rubyzhou2018@yahoo.com','sunnyjiang2018@yahoo.com','kerryyo2018@yahoo.com','rosemarry2018@yahoo.com') order by UL.Email ASC");
            //hhResults = this.ExecuteDBQuery(householdsQuery.ToString());

            return this.Execute(query.ToString());
        }

        /// <summary>
        /// This method returns the names of all of the individuals in the specified household.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="householdId">The household id.</param>
        /// <returns>DataTable containing the individual names belonging to the household.</returns>
        public DataTable People_FetchIndividualsInHouseholdByHouseholdID(int churchId, int householdId) {
            StringBuilder query = new StringBuilder("SELECT INDIVIDUAL_NAME FROM ChmPeople.dbo.INDIVIDUAL_HOUSEHOLD ih WITH (NOLOCK) ");
            query.Append("INNER JOIN ChmPeople.dbo.INDIVIDUAL ind WITH (NOLOCK) ");
            query.Append("ON ih.INDIVIDUAL_ID = ind.INDIVIDUAL_ID ");
            query.AppendFormat("WHERE ind.CHURCH_ID = {0} AND ih.HOUSEHOLD_ID = {1}", churchId, householdId);

            return this.Execute(query.ToString());
        }

        /// <summary>
        /// This method calls the MergeIndividual stored procedure.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualFrom">The name of the duplicate individual.</param>
        /// <param name="individualTo">The name of the master individual.</param>
        public void People_MergeIndividual(int churchId, string individualFrom, string individualTo) {

            if (individualFrom.Equals("FT Tester"))
            {
                Assert.Fail("FT Tester SHOULD NEVER BE MERGED");
            }

            try {
                int toINDId = this.People_Individuals_FetchID(churchId, individualTo);
                int toHHId = this.People_Households_FetchID(churchId, toINDId);

                int fromINDId = this.People_Individuals_FetchID(churchId, individualFrom);
                int fromHHId = this.People_Households_FetchID(churchId, fromINDId);


                using (SqlConnection conn = new SqlConnection(this._dbConnectionString)) {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("ChmPeople.dbo.MergeIndividual", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MovingToHouseholdID", toHHId);
                        cmd.Parameters.AddWithValue("@MovingFromHouseholdID", fromHHId);
                        cmd.Parameters.AddWithValue("@MovingToIndividualID", toINDId);
                        cmd.Parameters.AddWithValue("@MovingFromIndividualID", fromINDId);
                        cmd.Parameters.AddWithValue("@ChurchID", churchId);
                        cmd.ExecuteReader();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex) {
                if (ex is SqlException || ex is IndexOutOfRangeException) { }
                else { throw; }
            }
        }


        /// <summary>
        /// This method calls the MergeIndividual stored procedure.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualFrom">The name of the duplicate individual.</param>
        /// <param name="individualTo">The name of the master individual.</param>
        public void People_MergeIndividualBulk(int churchId, string individualFrom, string individualTo)
        {

            if (individualFrom.Equals("FT Tester"))
            {
                Assert.Fail("FT Tester SHOULD NEVER BE MERGED");
            }

            try
            {
                int toINDId = this.People_Individuals_FetchID(churchId, individualTo);
                int toHHId = this.People_Households_FetchID(churchId, toINDId);

                string[] indiList = this.People_Individuals_FetchIDList(churchId, individualFrom);
                int fromINDId;
                int fromHHId;

                for (int index = 0; index < indiList.Length; index++)
                {
                    fromINDId = Convert.ToInt32(indiList[index]);
                    if (fromINDId == -1) { continue; }
                    fromHHId = this.People_Households_FetchID(churchId, fromINDId);


                    using (SqlConnection conn = new SqlConnection(this._dbConnectionString))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand("ChmPeople.dbo.MergeIndividual", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MovingToHouseholdID", toHHId);
                            cmd.Parameters.AddWithValue("@MovingFromHouseholdID", fromHHId);
                            cmd.Parameters.AddWithValue("@MovingToIndividualID", toINDId);
                            cmd.Parameters.AddWithValue("@MovingFromIndividualID", fromINDId);
                            cmd.Parameters.AddWithValue("@ChurchID", churchId);
                            cmd.ExecuteReader();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlException || ex is IndexOutOfRangeException) { }
                else { throw; }
            }
        }


        public void People_Update_Member_Envelope_Number(int churchId, string individualName, string memberEnvelopeNumber)
        {
            /*
             UPDATE [ChmPeople].[dbo].[INDIVIDUAL]
             SET [MEMBER_ENV_CODE] = 'DC01-0100'      
             WHERE CHURCH_ID = '15' and INDIVIDUAL_ID = '29974717'             
             */

            int individualId = this.People_Individuals_FetchID(churchId, individualName);
            this.Execute(string.Format("UPDATE ChmPeople.dbo.INDIVIDUAL SET MEMBER_ENV_CODE = '{0}' WHERE CHURCH_ID = '{1}' AND INDIVIDUAL_ID = '{2}'", memberEnvelopeNumber, churchId, individualId));

        }
        /// <summary>
        /// This method creates an address.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="address1">Address line 1.</param>
        /// <param name="city">City.</param>
        /// <param name="stateProvince">State/province.</param>
        /// <param name="postalCode">Postal code.</param>
        /// <param name="country">Country.</param>
        public void People_Addresses_Create(int churchId, string individualName, string address1, string city, string stateProvince, string postalCode, string country, int addressTypeId = 7) {
            int individualId = this.People_Individuals_FetchID(churchId, individualName);
            int householdId = this.People_Households_FetchID(churchId, individualId);
            StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.HOUSEHOLD_ADDRESS (HOUSEHOLD_ID, CHURCH_ID, ADDRESS_TYPE_ID, INDIVIDUAL_ID, ADDRESS_1, CITY, POSTAL_CODE, COUNTRY, ST_PROVINCE) ");
            query.AppendFormat("VALUES({0}, {1}, {8}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}')", householdId, churchId, individualId, address1, city, postalCode, country, stateProvince, addressTypeId);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an address.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id the address is tied to.</param>
        /// <param name="address1">Address line 1.</param>
        public void People_Addresses_Delete(int churchId, int individualId, string address1) {
            this.Execute(string.Format("DELETE FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1} AND ADDRESS_1 = '{2}'", churchId, individualId, address1));
        }

        /// <summary>
        /// Gets the individual name for someone.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns>The full name of the individual.  Includes Goes By name and middle initial.</returns>
        public string People_GetIndividualName(int churchId, string firstName, string lastName) {
            return Convert.ToString(this.Execute(string.Format("SELECT TOP 1 Individual_Name FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}'", churchId, firstName, lastName)).Rows[0]["Individual_Name"]);
        }

        /// <summary>
        /// Updates a communication value's validation status id.
        /// </summary>
        /// <param name="churchID">The church id.</param>
        /// <param name="individualName">The individual who has a communication value to be updated.</param>
        /// <param name="communicationValue">The communication value to be updated.</param>
        /// <param name="validationStatusId">The status id for this communcation value.  0 is invalid.  Null is valid.</param>
        public void People_UpdateCommunicationValidationStatus(int churchID, string individualName, string communicationValue, int? validationStatusId) {
            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            // Update the communication value's validation status to the value provided.  This will also set the communication's listed value to 0.
            StringBuilder query = new StringBuilder("UPDATE ChmPeople.dbo.HOUSEHOLD_COMMUNICATION ");
            string setQuery = validationStatusId == null ? "SET VALIDATION_STATUS_ID = NULL " : "SET VALIDATION_STATUS_ID = {0} ";
            query.AppendFormat(setQuery, validationStatusId);
            query.AppendFormat("WHERE (INDIVIDUAL_ID = {0} OR HOUSEHOLD_ID = {1}) and CHURCH_ID = {2} and COMMUNICATION_VALUE = '{3}'", individualId, householdId, churchID, communicationValue);
            
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Gets the household communication information for an individual with a specific value.
        /// </summary>
        /// <param name="churchID">The church id.</param>
        /// <param name="individualName">The individual name.</param>
        /// <param name="communicationValue">The communication value.</param>
        /// <returns>DataTable containing the communication values.</returns>
        public DataTable People_GetCommunicationValueInformation(int churchID, string individualName, string communicationValue) {
            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            // Update the communication value's validation status to the value provided.  This will also set the communication's listed value to 0.
            StringBuilder query = new StringBuilder("SELECT TOP 1 * FROM ChmPeople.dbo.HOUSEHOLD_COMMUNICATION WITH (NOLOCK) ");
            query.AppendFormat("WHERE (INDIVIDUAL_ID = {0} OR HOUSEHOLD_ID = {1}) AND CHURCH_ID = {2} AND COMMUNICATION_VALUE = '{3}'", individualId, householdId, churchID, communicationValue);

            return this.Execute(query.ToString());
        }

        public DataTable People_GetHouseholdIndividual_Modal_Information(int churchID, string individualName){


            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            /*
             SELECT HOUSEHOLD_ID, HOUSEHOLD_MEMBER_TYPE_NAME, STATUS_NAME, LIST_NAME, INDIVIDUAL_NAME, DATE_OF_BIRTH
               FROM [ChmPeople].[dbo].[INDIVIDUAL_HOUSEHOLD] AS Peeps
               INNER JOIN [ChmPeople].[dbo].[HOUSEHOLD_MEMBER_TYPE] AS HouseType
                 ON Peeps.HOUSEHOLD_MEMBER_TYPE_ID = HouseType.HOUSEHOLD_MEMBER_TYPE_ID
               LEFT JOIN [ChmPeople].[dbo].[INDIVIDUAL] AS Ind
                 ON Peeps.Individual_ID = Ind.Individual_ID
               RIGHT JOIN [ChmPeople].[dbo].[STATUS] AS S
                 ON S.STATUS_ID = Ind.STATUS_ID
               RIGHT OUTER JOIN [ChmPeople].[dbo].[STATUS_SELECTLIST] AS SubStatus
                 ON SubStatus.STATUS_SELECTLIST_ID = Ind.STATUS_SELECTLIST_ID
               where
                household_id = '18950808' 
             */

            /*
            StringBuilder query = new StringBuilder("SELECT HOUSEHOLD_ID, HOUSEHOLD_MEMBER_TYPE_NAME, STATUS_NAME, LIST_NAME, INDIVIDUAL_NAME, DATE_OF_BIRTH ");
            query.Append("FROM [ChmPeople].[dbo].[INDIVIDUAL_HOUSEHOLD] AS Peeps WITH (NOLOCK) ");
            query.Append("  INNER JOIN [ChmPeople].[dbo].[HOUSEHOLD_MEMBER_TYPE] AS HouseType ");
            query.Append("    ON Peeps.HOUSEHOLD_MEMBER_TYPE_ID = HouseType.HOUSEHOLD_MEMBER_TYPE_ID ");
            query.Append("  LEFT JOIN [ChmPeople].[dbo].[INDIVIDUAL] AS Ind ");
            query.Append("    ON Peeps.Individual_ID = Ind.Individual_ID ");
            query.Append("  RIGHT JOIN [ChmPeople].[dbo].[STATUS] AS S ");
            query.Append("    ON S.STATUS_ID = Ind.STATUS_ID ");
            query.Append("  RIGHT OUTER JOIN [ChmPeople].[dbo].[STATUS_SELECTLIST] AS SubStatus ");
            query.Append("    ON SubStatus.STATUS_SELECTLIST_ID = Ind.STATUS_SELECTLIST_ID ");
            query.AppendFormat("WHERE Peeps.HOUSEHOLD_ID = {0} ORDER BY HouseType.HOUSEHOLD_MEMBER_TYPE_ID ASC, Ind.DATE_OF_BIRTH ASC", householdId);
            */

            StringBuilder query = new StringBuilder("SELECT HOUSEHOLD_ID, HOUSEHOLD_MEMBER_TYPE_NAME, STATUS_NAME, LIST_NAME, INDIVIDUAL_NAME, DATE_OF_BIRTH ");
            query.Append("FROM     [ChmPeople].dbo.INDIVIDUAL AS i WITH (NOLOCK) ");
            query.Append("JOIN      [ChmPeople].dbo.INDIVIDUAL_HOUSEHOLD AS ih  ");
            query.Append("ON          i.INDIVIDUAL_ID = ih.INDIVIDUAL_ID  ");
            query.Append("JOIN      [ChmPeople].dbo.HOUSEHOLD_MEMBER_TYPE AS hmt  ");
            query.Append("ON          ih.HOUSEHOLD_MEMBER_TYPE_ID = hmt.HOUSEHOLD_MEMBER_TYPE_ID  ");
            query.Append("JOIN      [ChmPeople].dbo.STATUS AS s  ");
            query.Append("ON          i.STATUS_ID = s.STATUS_ID  ");
            query.Append("LEFT JOIN [ChmPeople].dbo.STATUS_SELECTLIST AS ss  ");
            query.Append("ON          i.STATUS_SELECTLIST_ID = ss.STATUS_SELECTLIST_ID  ");
            query.Append("AND i.CHURCH_ID = ss.CHURCH_ID  ");
            query.AppendFormat("WHERE  ih.HOUSEHOLD_ID = {0}  ORDER BY ih.HOUSEHOLD_MEMBER_TYPE_ID ASC, i.DATE_OF_BIRTH ASC", householdId);

            return this.Execute(query.ToString());

        }

        public DataTable People_GetHouseholdInformation(int churchID, string individualName)
        {

            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            TestLog.WriteLine("HouseholdID: " + householdId);

            //[ChmPeople].[dbo].[HOUSEHOLD]
            //Get Household Information
            StringBuilder query = new StringBuilder("SELECT TOP 1 * FROM ChmPeople.dbo.HOUSEHOLD (NOLOCK) ");
            query.AppendFormat("WHERE HOUSEHOLD_ID = {0} AND CHURCH_ID = {1}", householdId, churchID);

            return this.Execute(query.ToString());

        }

        //1 Primary
        //2 Secondary
        public DataTable People_GetHouseholdAddress(int churchID, string individualName, string addressTypeID)
        {

            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            //[ChmPeople].[dbo].[HOUSEHOLD]
            // Update the communication value's validation status to the value provided.  This will also set the communication's listed value to 0.
            StringBuilder query = new StringBuilder("SELECT TOP 1 * FROM ChmPeople.dbo.HOUSEHOLD_ADDRESS (NOLOCK) ");
            query.AppendFormat("WHERE HOUSEHOLD_ID = {0} AND CHURCH_ID = {1} AND ADDRESS_TYPE_ID = {2}", householdId, churchID, addressTypeID);

            return this.Execute(query.ToString());

        }

        //1 Home Phone
        //4 E-mail
        public DataTable People_GetHouseholdCommunication(int churchID, string individualName)
        {
            // Get the individual id for the person
            var individualId = this.People_Individuals_FetchID(churchID, individualName);
            var householdId = this.People_Households_FetchID(churchID, individualId);

            // Get Household Communication Values
            StringBuilder query = new StringBuilder("SELECT * FROM ChmPeople.dbo.HOUSEHOLD_COMMUNICATION WITH (NOLOCK) ");
            query.AppendFormat("WHERE INDIVIDUAL_ID is null and  HOUSEHOLD_ID = {0} AND CHURCH_ID = {1}", householdId, churchID);

            return this.Execute(query.ToString());
        }


        /// <summary>
        /// Gets an individual's unsubscribed status.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The name of the individual.</param>
        public object People_GetIndividualUnsubscribedStatus(int churchId, string individualName) {
            return this.Execute("SELECT TOP 1 UnsubscribeAllChurchEmail FROM ChmPeople.dbo.Individual WITH (NOLOCK) WHERE CHURCH_ID = {0} AND Individual_Name = '{1}'", churchId, individualName).Rows[0]["UnsubscribeAllChurchEmail"];
        }

        /// <summary>
        /// Creates an infellowship account with a desired username and password.
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="individualName">The name of the individual that will have the account.</param>
        /// <param name="desiredUserName">The desired username.</param>
        /// <param name="desiredPassword">The desired password.</param>
        public void People_InFellowshipAccount_Create(int churchId, string individualName, string desiredUserName, string desiredPassword) {
            // Get the individual id
            var individualID = this.People_Individuals_FetchID(churchId, individualName);

            // Create a hash and salt
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[24];
            rng.GetBytes(buff);
            var salt = Convert.ToBase64String(buff);

            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = ASCIIEncoding.ASCII.GetBytes("7AB3996B-EF9C-4835-8D0A-59B10B4E0150");
            hmacsha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(desiredPassword + salt));

            var hash = ASCIIEncoding.ASCII.GetString(hmacsha1.Hash);
            using (SqlConnection conn = new SqlConnection(this._dbConnectionString)) {
                conn.Open();
                using (SqlCommand query = new SqlCommand("INSERT INTO ChmPeople.dbo.UserLogin (churchId, IndividualId, Email, UserLoginStatusId, IsUserLoggedIn, PasswordHash, PasswordSalt) VALUES(@ChurchId, @IndividualID, @Email, @UserLoginStatusID, @IsUserLoggedIn, @PasswordHash, @PasswordSalt)", conn)) {
                    query.CommandType = CommandType.Text;
                    query.Parameters.AddWithValue("ChurchId", churchId);
                    query.Parameters.AddWithValue("IndividualID", individualID);
                    query.Parameters.AddWithValue("Email", desiredUserName);
                    query.Parameters.AddWithValue("UserLoginStatusID", 1);
                    query.Parameters.AddWithValue("IsUserLoggedIn", 1);
                    query.Parameters.AddWithValue("PasswordHash", hash);
                    query.Parameters.AddWithValue("PasswordSalt", salt);
                    SqlDataReader reader = query.ExecuteReader();

                }
                conn.Close();
            }

            // Add the user name as a infellowship comm value
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmPeople.dbo.HOUSEHOLD_COMMUNICATION (CHURCH_ID, INDIVIDUAL_ID, HOUSEHOLD_ID, COMMUNICATION_TYPE_ID, COMMUNICATION_VALUE) ");
            query2.AppendFormat("VALUES ({0}, {1}, {2}, 6, '{3}')", churchId, individualID, this.People_Households_FetchID(churchId, individualID), desiredUserName);
            this.Execute(query2.ToString());
        }


        /// <summary>
        /// Deletes an InFellowship account.
        /// </summary>
        /// <param name="churchID">The church id.</param>
        /// <param name="accountEmailAddress">The account to delete.</param>
        public void People_InFellowshipAccount_Delete(int churchID, string accountEmailAddress) {
            this.Execute("DELETE FROM ChmPeople.dbo.UserLogin WHERE ChurchID = {0} and Email = '{1}' ", churchID, accountEmailAddress);
        }

        /// <summary>
        /// Creates a new individual.  Also creates a new household for this individual.  The household name will be the first and last name concatenated.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name for this individual.</param>
        /// <param name="lastName">The last name for this individual.</param>
        /// <param name="indStatus">[Optional]The status of the individual</param>
        public void People_Individual_Create(int churchId, string firstName, string lastName, [Optional, DefaultParameterValue(1)] int indStatus) {
            // Variables
            var houseHoldName = firstName + " " + lastName;

            // Household
            StringBuilder query1 = new StringBuilder("INSERT INTO ChmPeople.dbo.Household (church_id, household_name) ");
            query1.AppendFormat("VALUES ({0}, '{1}') ", churchId, houseHoldName);
            query1.Append("SELECT SCOPE_IDENTITY()");
            var houseHoldID = this.Execute(query1.ToString()).Rows[0][0];

            // Individual
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual (church_id, status_id, last_name, first_name) ");
            query2.AppendFormat("VALUES ({0}, {1}, '{2}', '{3}') ", churchId, indStatus, lastName, firstName);
            query2.Append("SELECT SCOPE_IDENTITY()");
            var individualID = this.Execute(query2.ToString()).Rows[0][0];

            // Individual Household
            StringBuilder query3 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual_Household (church_id, household_id, individual_id, household_type_id, household_member_type_id, is_authorized) ");
            query3.AppendFormat("VALUES ({0}, {1}, {2}, 1, 1, 1) ", churchId, houseHoldID, individualID);
            this.Execute(query3.ToString());
        }

        /// <summary>
        /// Creates a new individual in a household
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name for this individual.</param>
        /// <param name="lastName">The last name for this individual.</param>
        /// <param name="indStatus">[Optional]The status of the individual</param>
        public void People_Individual_Create_In_Household(int churchId, string firstName, string lastName, string houseHoldID, [Optional, DefaultParameterValue(1)] int indStatus)
        {
            // Individual
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual (church_id, status_id, last_name, first_name) ");
            query2.AppendFormat("VALUES ({0}, {1}, '{2}', '{3}') ", churchId, indStatus, lastName, firstName);
            query2.Append("SELECT SCOPE_IDENTITY()");
            var individualID = this.Execute(query2.ToString()).Rows[0][0];

            // Individual Household
            StringBuilder query3 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual_Household (church_id, household_id, individual_id, household_type_id, household_member_type_id, is_authorized) ");
            query3.AppendFormat("VALUES ({0}, {1}, {2}, 1, 1, 1) ", churchId, houseHoldID, individualID);
            this.Execute(query3.ToString());
        }

        /// <summary>
        /// Creates a new individual with given property.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name for this individual.</param>
        /// <param name="lastName">The last name for this individual.</param>
        /// <param name="middlename">[optional]The middle name for this individual, default is empty</param>
        /// <param name="goesBy">[optional]The goes by person name for this individual, default is empty</param>
        public int People_Individual_Only_Create(int churchId, string firstName, string lastName, string middlename = "", string goesBy = "")
        {

            // Individual
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual (church_id, status_id, last_name, first_name, middle_name, goes_by) ");
            query2.AppendFormat("VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}') ", churchId, 1, lastName, firstName, middlename, goesBy);
            query2.Append("SELECT SCOPE_IDENTITY()");
            
            return Convert.ToInt32(this.Execute(query2.ToString()).Rows[0][0]);
        }
        
        /// <summary>
        /// Creates a new individual with given property.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="firstName">The first name for this individual.</param>
        /// <param name="lastName">The last name for this individual.</param>
        /// <param name="middleName">[optional]The middle name for this individual, default is empty</param>
        /// <param name="goesBy">[optional]The goes by person name for this individual, default is empty</param>
        /// <returns></returns>
        public int People_Individual_Only_Create_Param(int churchId, string firstName, string lastName, string middleName = "", string goesBy = "", string birthday = "")
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ChmPeople.dbo.Individual (church_id, status_id, last_name, first_name, middle_name, goes_by, date_of_birth) ");
            query.Append("VALUES (@ChurchID, @StatusID, @LastName, @FirstName, @MiddleName, @GoesBy , @Birthday) ");
            query.Append("SELECT SCOPE_IDENTITY()");

            paramlist.Add(new SqlParameter() { ParameterName = "@ChurchID", Value = churchId, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@StatusID", Value = 1, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@LastName", Value = lastName, DbType = DbType.String });
            paramlist.Add(new SqlParameter() { ParameterName = "@FirstName", Value = firstName, DbType = DbType.String });
            paramlist.Add(new SqlParameter() { ParameterName = "@MiddleName", Value = middleName, DbType = DbType.String });
            paramlist.Add(new SqlParameter() { ParameterName = "@GoesBy", Value = goesBy, DbType = DbType.String });

            if (string.IsNullOrEmpty(birthday))
            {
                paramlist.Add(new SqlParameter() { ParameterName = "@Birthday", Value = DBNull.Value, DbType = DbType.DateTime });
            }
            else
            {
                paramlist.Add(new SqlParameter() { ParameterName = "@Birthday", Value = DateTime.Parse(birthday), DbType = DbType.DateTime });
            }

            DataTable dt = this.ExecuteSql(query.ToString(), paramlist.ToArray());
            return Convert.ToInt32(dt.Rows[0][0]);
        }


        public void People_Individual_UpdateNameByID(int churchId, int individualId, string firstName = "", string lastName = "", string middleName = "")
        {
            var paramlist = new List<SqlParameter>();
            var setList = new List<string>();
            bool isExcute = false;
            if (!string.IsNullOrEmpty(firstName))
            {
                setList.Add(string.Format(" first_name = @FirstName "));
                paramlist.Add(new SqlParameter() { ParameterName = "@FirstName", Value = firstName, DbType = DbType.String });
                isExcute = true;
                
            }
            if (!string.IsNullOrEmpty(middleName))
            {
                setList.Add(string.Format(" middle_name = @MiddleName "));
                paramlist.Add(new SqlParameter() { ParameterName = "@MiddleName", Value = middleName, DbType = DbType.String });
                isExcute = true;
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                setList.Add(string.Format(" last_name = @LastName "));
                paramlist.Add(new SqlParameter() { ParameterName = "@LastName", Value = lastName, DbType = DbType.String });
                isExcute = true;
            }
            
            if (!isExcute) return;
            
            var query = new StringBuilder();
            query.Append(" UPDATE ChmPeople.dbo.Individual SET ");
            query.Append(string.Join(",", setList));
            query.Append(" WHERE individual_id = @IndividualID AND church_id = @ChurchID ");            

            paramlist.Add(new SqlParameter() { ParameterName = "@ChurchID", Value = churchId, DbType = DbType.Int32 });
            paramlist.Add(new SqlParameter() { ParameterName = "@IndividualID", Value = individualId, DbType = DbType.Int32 });
            
            DataTable dt = this.ExecuteSql(query.ToString(), paramlist.ToArray());
        }

        /// <summary>
        /// Bundle an individual to given household.  Also creates a new household for this individual.  The household name will be the first and last name concatenated.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="householdId">The household id to join in.</param>
        /// <param name="individualId">The individual id will be joined.</param>
        /// <param name="memberType">[optional]Membership the individual will join as, default is 3 as Child</param>
        /// <param name="isFamilyMember">[optional]Whether the individual is regular member in family, default is yes</param>
        public void People_Individual_Join_Household(int churchId, int householdId, int individualId, int memberType = 3, int isFamilyMember = 1)
        {
            // Join individual to household
            StringBuilder query3 = new StringBuilder("INSERT INTO ChmPeople.dbo.Individual_Household (church_id, household_id, individual_id, household_type_id, household_member_type_id, is_authorized) ");
            query3.AppendFormat("VALUES ({0}, {1}, {2}, 1, {3}, {4}) ", churchId, householdId, individualId, memberType, isFamilyMember);
            this.Execute(query3.ToString());
        }

        /// <summary>
        /// Adds a communication value to a household and, if specified, an individual.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualName">The individual gaining the communication value.</param>
        /// <param name="commType">The comm type you wish to add</param>
        /// <param name="commValue">The value for the communication item.</param>
        /// <param name="isIndividualCommValue">Specifies if this is an individual level comm value.</param>
        public void People_CommunicationValue_Add(int churchId, string individualName, GeneralEnumerations.CommunicationTypes commType, string commValue, bool isIndividualCommValue) {
            var individualId = this.People_Individuals_FetchID(churchId, individualName);
            var houseHoldId = this.People_Households_FetchID(churchId, individualId);


            StringBuilder query = new StringBuilder("INSERT INTO ChmPeople.dbo.Household_Communication (HOUSEHOLD_ID, CHURCH_ID, COMMUNICATION_TYPE_ID, INDIVIDUAL_ID, COMMUNICATION_VALUE, LISTED) ");
            if (isIndividualCommValue) {
                query.AppendFormat("VALUES ({0}, {1}, {2}, {3}, '{4}', 1) ", houseHoldId, churchId, Convert.ToInt32(commType), individualId, commValue);
            }
            else {
                query.AppendFormat("VALUES ({0}, {1}, {2}, NULL, '{3}', 1) ", houseHoldId, churchId, Convert.ToInt32(commType), commValue);
            }

            this.Execute(query.ToString());

        }

        #endregion People

        #region Ministry

        #region Ministries
        /// <summary>
        /// This method fetches the id for a ministry.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="ministryName">The name of the ministry.</param>
        /// <returns>The integer id for the ministry.</returns>
        public int Ministry_Ministries_FetchID(int churchId, string ministryName) {
            return Convert.ToInt32(this.Execute("SELECT MINISTRY_ID FROM ChmActivity.dbo.MINISTRY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND MINISTRY_NAME = '{1}'", churchId, ministryName).Rows[0]["MINISTRY_ID"]);
        }
        #endregion Ministries

        #region Activities
        /// <summary>
        /// This method fetches the id for an activity.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity.</param>
        /// <returns>The integer id for the activity.</returns>
        public int Ministry_Activities_FetchID(int churchId, string activityName)
        {
            DataTable result = this.Execute("SELECT ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1", churchId, activityName);
            if (result.Rows.Count < 1)
            {
                return -1;
            }

            return Convert.ToInt32(result.Rows[0]["ACTIVITY_ID"]);
        }

        public int[] Ministry_Activities_FetchIDS(int churchId, string activityName)
        {

            try
            {
                using (DataTable dt = this.Execute("SELECT ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1", churchId, activityName))
                {
                    
                    int[] activityIds = new int[dt.Rows.Count];

                    for (int i = 0; i < activityIds.Length; i++)
                    {
                        activityIds[i] = Convert.ToInt32(dt.Rows[i]["ACTIVITY_ID"].ToString());
                    }
                   
                    return activityIds;
                }

            }
            catch (System.IndexOutOfRangeException) 
            {       
                return null;
            }
        }
        public string[] Ministry_Activities_FetchNameList(int churchId, string actNamePrefix)
        {
            DataTable dt = this.Execute("SELECT ACTIVITY_NAME FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME like '{1}%' AND IS_ACTIVE = 1", churchId, actNamePrefix);
            string[] result = new string[dt.Rows.Count];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index]["ACTIVITY_NAME"].ToString();
            }

            return result;
        }

        public string Ministry_Activities_FetchCreatedDate(int churchId, string activityName)
        {
            DataTable result = this.Execute("SELECT CREATED_DATE FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1 order by CREATED_DATE desc", churchId, activityName);
            if (result.Rows.Count < 1)
            {
                return "";
            }

            return result.Rows[0]["CREATED_DATE"].ToString();
        }

        // add by Grace Zhang start
        /// <summary>
        /// This method fetches the id for an instance of activity.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityId">The id of the activity.</param>
        /// <returns>The integer id for the instance.</returns>
        public int Ministry_Activities_FetchInstanceID(int churchId, int activityId)
        {
            try
            {
                return Convert.ToInt32(this.Execute("SELECT TOP 1 ACTIVITY_INSTANCE_ID FROM ChmActivity.dbo.ACTIVITY_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1} ORDER BY START_DATE_TIME DESC", churchId, activityId).Rows[0]["ACTIVITY_INSTANCE_ID"]);
            } 
            catch (System.IndexOutOfRangeException)
            {
                return -1;
            }

        }
        public int Ministry_Activities_FetchInstanceID(int churchId, int activityId, DateTime startDate)
        {
            try
            {
                return Convert.ToInt32(this.Execute("SELECT ACTIVITY_INSTANCE_ID FROM ChmActivity.dbo.ACTIVITY_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1} and START_DATE_TIME = '{2}'", churchId, activityId, startDate.ToString("yyyy-MM-dd HH:mm")).Rows[0]["ACTIVITY_INSTANCE_ID"]);
            }
            catch (System.IndexOutOfRangeException)
            {
                return -1;
            }

        }
        public int[] Ministry_Activities_FetchMultyInstanceID(int churchId, int activityId)
        {
            try
            {
                using (DataTable dt = this.Execute("SELECT ACTIVITY_INSTANCE_ID FROM ChmActivity.dbo.ACTIVITY_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1}", churchId, activityId))
                {
                    int[] activityInstanceIds = new int[dt.Rows.Count];

                    for (int i = 0; i < activityInstanceIds.Length; i++)
                    {
                        activityInstanceIds[i] = Convert.ToInt32(dt.Rows[i]["ACTIVITY_INSTANCE_ID"].ToString());
                    }

                    return activityInstanceIds;
                }

            }
            catch (System.IndexOutOfRangeException)
            {
                int[] activityInstanceIds = { -1 };
                return activityInstanceIds; 
            }
        }
        

        /// <summary>
        /// This method fetches the id for an roser of activity.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityId">The id of the activity.</param>
        /// <returns>The integer id for the roster.</returns>
        public int Ministry_Activities_FetchRosterID(int churchId, int activityId)
        {
            return Convert.ToInt32(this.Execute("SELECT ACTIVITY_DETAIL_ID FROM ChmActivity.dbo.ACTIVITY_DETAIL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1} ", churchId, activityId).Rows[0]["ACTIVITY_DETAIL_ID"]);
        }

        /// <summary>
        /// This method fetches the id for an Individual TypeId.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        ///  <param name="individualId">The id of the individual.</param>
        /// <param name="activityInstanceId">The id of the activity instance.</param>
        /// <returns>The integer id for the roster.</returns>
        public int Ministry_Activities_FetchIndividualTypeId(int churchId, int individualId, int activityInstanceId)
        {
            string id = string.Empty;
            DataTable requirementDT = this.Execute("SELECT INDIVIDUAL_TYPE_ID FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID =  {1} AND ACTIVITY_INSTANCE_ID = {2} ", churchId, individualId, activityInstanceId);
            if (requirementDT.Rows.Count > 0)
            {
                id = requirementDT.Rows[0]["INDIVIDUAL_TYPE_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("individualTypeId [{0}] for church id [{1}] and activityInstance id [{2}] was not found", individualId, churchId, activityInstanceId));
            }
            return Convert.ToInt32(id);
        }

        /// <summary>
        /// This method fetches the Name for an IndividualTypeId.
        /// </summary>
        ///  <param name="individualTypeId">The id of the individualTypeId.</param>
        /// <returns>The integer id for the roster.</returns>
        public String Ministry_Activities_FetchIndividualTypeName(int individualTypeId)
        {
            return Convert.ToString(this.Execute("SELECT INDIVIDUAL_TYPE_NAME FROM ChmActivity.dbo.INDIVIDUAL_TYPE WITH (NOLOCK) WHERE INDIVIDUAL_TYPE_ID = {0} ", individualTypeId).Rows[0]["INDIVIDUAL_TYPE_NAME"]);
        }

        /// <summary>
        /// This method fetches many individualIds.
        /// </summary>
        ///  <param name="individualNumber">The id of the individualTypeId.</param>
        /// <returns>The integer id for the roster.</returns>
        public string[] Ministry_Activities_FetchMultipleIndividualIDs(int churchId, int individualNumber)
        {
            using (DataTable dt = this.Execute("SELECT TOP {0} INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {1} ", individualNumber, churchId))
            {
                string[] individuals = new string[dt.Rows.Count];

                for (int i = 0; i < individuals.Length; i++)
                {
                    individuals[i] = dt.Rows[i]["INDIVIDUAL_ID"].ToString();
                }

                return individuals;
            }
        }
        /// <summary>
        /// This method update  individual info in ChmPeople.dbo.UserLogin.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        ///  <param name="individualId">The id of the individual.</param>
        ///  <param name="userLoginId">The id of the userLogin.</param>
        ///   <param name="dateStr">The value of the DELETEDDATE field of table ChmPeople.dbo.USERLOGIN .</param>
        ///  add for :FO-3026
        public void InfellowShipIndividual_data_Update(int churchId, String email, String dateStr)
        {

            StringBuilder updateStr = new StringBuilder("UPDATE ChmPeople.dbo.USERLOGIN  SET ");

            try
            {
                if (dateStr != null)
                {

                    updateStr.AppendFormat("DeletedDate = '" + dateStr + "' WHERE CHURCHID = {0} and Email = '{1}' ", churchId, email);

                }
                else if (dateStr == null)
                {
                    updateStr.AppendFormat("DeletedDate = null WHERE CHURCHID = {0} and Email = '{1}' ", churchId, email);

                }
                this.Execute(updateStr.ToString());
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table USERLOGIN update: {0}", e.Message);
            }

        }
        /// <summary>
        /// This method update  IndividualInstance info in ChmActivity.dbo.INDIVIDUAL_INSTANCE.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityInstanceId">The activityInstance id.</param>
        ///  <param name="individualId">The id of the individual.</param>
        ///  <param name="checkInMachineId">The id of the Machine.</param>
        ///   <param name="checkInTime">The vtime of checkin.</param>

        public void IndividualInstance_Checkin_Update(int churchId, int activityInstanceId, int individualId, int checkInMachineId, String checkInTime)
        {
            StringBuilder updateStr = new StringBuilder("UPDATE ChmActivity.dbo.INDIVIDUAL_INSTANCE  SET ");
            try
            {
                updateStr.AppendFormat("CHECK_IN_MACHINE_ID = {0} WHERE CHURCH_ID = {1} and INDIVIDUAL_ID = {2} and ACTIVITY_INSTANCE_ID = {3} ", checkInMachineId, churchId, individualId, activityInstanceId);
                this.Execute(updateStr.ToString());
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table INDIVIDUAL_INSTANCE update: {0}", e.Message);
            }
            StringBuilder updateStr2 = new StringBuilder("UPDATE ChmActivity.dbo.INDIVIDUAL_INSTANCE  SET ");
            try
            {
                updateStr2.AppendFormat("CHECK_IN_TIME = '{0}' WHERE CHURCH_ID = {1} and INDIVIDUAL_ID = {2} and ACTIVITY_INSTANCE_ID = {3} ", checkInTime, churchId, individualId, activityInstanceId);
                this.Execute(updateStr2.ToString());
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table INDIVIDUAL_INSTANCE update: {0}", e.Message);
            }

        }

        /// <summary>
        /// This method creates an activity with multy rosters and ties it to a ministry
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="ministryId">The ministry ID</param>
        /// <param name="activityName">The name of the activity</param>
        /// <param name="rosterNameArray">The name of the roster Array</param>
        public void Ministry_Activities_CreateForCheckIn(int churchId, int ministryId, string activityName, string[] rosterNameArray, string rosterGroupName = "All Rosters")
        {
            // Create the activity
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY (CHURCH_ID, ACTIVITY_NAME, HAS_CHECKIN, CHECKIN_MINUTES_BEFORE, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, PAGER_ENABLED,CREATED_DATE, CREATED_BY_LOGIN,CHECKIN_CODE,FIXED_TIMES,HAS_SECURITY_AUTHORIZATION,TRACK_ATTENDANCE_BY_INDIVIDUAL) ");
            query.AppendFormat("VALUES({0}, '{1}', 1, 0, 1, 1, 1, 1, GetDate(), 'ft.ministrytest',null,0,1,null)", churchId, activityName);

            this.Execute(query.ToString());

            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            TestLog.WriteLine("The new check in activity created as activityId: " + activityId);
            // Tie it to a ministry
            query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_MINISTRY (MINISTRY_ID, ACTIVITY_ID, CHURCH_ID, IS_PRIMARY, CREATED_DATE, CREATED_BY_LOGIN)");
            query.AppendFormat("VALUES({0}, {1}, {2}, 0, GetDate(), 'ft.ministrytest')", ministryId, activityId, churchId);

            this.Execute(query.ToString());

            // Create Roster Group
            this.Ministry_ActivityRLCGroups_Create(churchId, activityName, rosterGroupName);

            // Store roster group ID
            int rosterGroupId = this.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);

            // Create  rosters
            for (int i = 0; i < rosterNameArray.Length; i++)
            {
                query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, ACTIVITY_GROUP_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED)");
                query.AppendFormat("VALUES({0}, '{1}', {2}, {3}, 1, 1, 0, 0, 1, 0)", churchId, rosterNameArray[i], activityId, rosterGroupId);

                this.Execute(query.ToString());

            }


        }
        
        /// <summary>
        /// This method fetches the id for an ActivityCheckIn Id.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        ///  <param name="individualId">The id of the individual.</param>
        /// <param name="activityInstanceId">The id of the activity instance.</param>
        /// <returns>The integer id for the roster.</returns>
        public int Ministry_Fetch_Activity_CheckInId(int churchId, int activityInstanceId)
        {
            string id = string.Empty;
            DataTable requirementDT = this.Execute("select ACTIVE_CHECKIN_ID from ChmActivity.dbo.ACTIVE_CHECKIN WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1}", churchId, activityInstanceId);
            if (requirementDT.Rows.Count > 0)
            {
                return Convert.ToInt32(requirementDT.Rows[0]["ACTIVE_CHECKIN_ID"].ToString());
            }

            return -1;
        }
        public int Ministry_fetch_Activity_DetailId(int churchId, int activityID, string rosterName)
        {
            string id = string.Empty;
            DataTable requirementDT = this.Execute("select  ACTIVITY_DETAIL_ID from ChmActivity.dbo.ACTIVITY_DETAIL  where CHURCH_ID ={0} and ACTIVITY_ID = {1} and ACTIVITY_DETAIL_NAME ='{2}' ", churchId, activityID, rosterName);
            //select * from ChmActivity.dbo.ACTIVITY_DETAIL where ACTIVITY_ID = 122763 and ACTIVITY_DETAIL_NAME ='RosterCheckInTest003001'
            if (requirementDT.Rows.Count > 0)
            {
                id = requirementDT.Rows[0]["ACTIVITY_DETAIL_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("ACTIVITY_DETAIL_ID [{0}]  was not found", id));
            }
            return Convert.ToInt32(id);


        }

        public string[] Ministry_Fetch_Activity_DetailIds(int churchId, int activityID)
        {
            DataTable dt = this.Execute("select ACTIVITY_DETAIL_ID from ChmActivity.dbo.ACTIVITY_DETAIL (NOLOCK) where CHURCH_ID ={0} and ACTIVITY_ID = {1}", churchId, activityID);

            string[] result = new string[dt.Rows.Count];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index]["ACTIVITY_DETAIL_ID"].ToString();
            }

            return result;
        }

        public int Ministry_Fetch_Activity_TimeId(int churchId, int activityID, string scheduleName)
        {

            string id = string.Empty;
            DataTable requirementDT = this.Execute("select ACTIVITY_TIME_ID from ChmActivity.dbo.ACTIVITY_TIME  where CHURCH_ID ={0} and ACTIVITY_ID = {1} and ACTIVITY_TIME_NAME ='{2}'", churchId, activityID, scheduleName);
            //select * from ChmActivity.dbo.ACTIVITY_DETAIL where ACTIVITY_ID = 122763 and ACTIVITY_DETAIL_NAME ='RosterCheckInTest003001'
            if (requirementDT.Rows.Count > 0)
            {
                id = requirementDT.Rows[0]["ACTIVITY_TIME_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("ACTIVITY_TIME_ID [{0}]  was not found", id));
            }
            return Convert.ToInt32(id);

        }

        public string[] Ministry_Fetch_Activity_TimeIds(int churchId, int activityID)
        {
            DataTable dt = this.Execute("select ACTIVITY_TIME_ID from ChmActivity.dbo.ACTIVITY_TIME (NOLOCK) where CHURCH_ID ={0} and ACTIVITY_ID = {1}", churchId, activityID);
            string[] result = new string[dt.Rows.Count];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = dt.Rows[index]["ACTIVITY_TIME_ID"].ToString();
            }

            return result;

        }

        public int Ministry_fetch_Activity_CheckInId(int churchId, int activityInstanceId)
        {
            string id = string.Empty;
            DataTable requirementDT = this.Execute("select ACTIVE_CHECKIN_ID from ChmActivity.dbo.ACTIVE_CHECKIN  where CHURCH_ID ={0} and ACTIVITY_INSTANCE_ID = {1} ", churchId, activityInstanceId);            
            if (requirementDT.Rows.Count > 0)
            {
                id = requirementDT.Rows[0]["ACTIVE_CHECKIN_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("ACTIVE_CHECKIN_ID [{0}]  was not found", id));
            }
            return Convert.ToInt32(id);

        }
        /// <summary>
        /// This method creates an activity schedule for check in.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the schedule is tied to.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="endDate">The end date for the schedule.</param>
        public void Ministry_ActivitySchedules_CreateForCheckIn(int churchId, string activityName, string activityScheduleName, string[] rosterNameArray, DateTime startDate, DateTime endDate)
        {
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE ACTIVITY_TIME_NAME = '{0}'", activityScheduleName);
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_TIME (CHURCH_ID, ACTIVITY_ID, ACTIVITY_TIME_NAME, ACTIVITY_START_TIME, ACTIVITY_END_TIME) ");

            query.AppendFormat("VALUES({0},{1}, '{2}', '{3}', '{4}')", churchId, activityId, activityScheduleName, startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"));
            query.ToString();
            this.Execute(query.ToString());

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_INSTANCE WHERE ACTIVITY_ID = {0}", activityId);
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_INSTANCE (CHURCH_ID, ACTIVITY_ID, START_DATE_TIME,START_CHECKIN, END_CHECKIN,CREATED_DATE) ");
            query2.AppendFormat("VALUES({0},{1}, '{2}', '{3}', '{4}','{5}')", churchId, activityId, startDate.ToString("yyyy-MM-dd HH:mm"), startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"), startDate.ToString("yyyy-MM-dd HH:mm"));
            this.Execute(query2.ToString());

            /*
            string proc = "[ChmActivity].[dbo].[insertActiveCheckinByChurch]";
            SqlDataReader dr;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@churchid", churchId));
                    dr = cmd.ExecuteReader();
                }

                dbConnection.Close();
            }
            */
            
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVE_CHECKIN WHERE ACTIVITY_INSTANCE_ID = {0}", activityInstanceId);
            StringBuilder query3 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVE_CHECKIN (CHURCH_ID, ACTIVITY_INSTANCE_ID,CREATED_DATE,CREATED_BY_LOGIN) ");
            query3.AppendFormat("VALUES({0},{1}, '{2}', '{3}')", churchId, activityInstanceId, startDate.ToString("yyyy-MM-dd HH:mm"), "DEV\f1qaportal");
            this.Execute(query3.ToString());

            for (int i = 0; i < rosterNameArray.Length; i++)
            {
                int activityDetailId = Ministry_fetch_Activity_DetailId(churchId, activityId, rosterNameArray[i]);
                int activityCheckInId = Ministry_fetch_Activity_CheckInId(churchId, activityInstanceId);
                int isIpen = 1;
                int capacityNum = 200;
                StringBuilder query4 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVE_DETAIL_CHECKIN (ACTIVITY_DETAIL_ID,ACTIVE_CHECKIN_ID,CHURCH_ID,IS_OPEN,CAPACITY,CREATED_DATE,CREATED_BY_LOGIN) ");
                query4.AppendFormat("VALUES({0},{1}, {2}, {3},{4},'{5}','{6}')", activityDetailId, activityCheckInId, churchId, isIpen, capacityNum, startDate.AddHours(0.1).ToString("yyyy-MM-dd HH:mm"), "DEV\f1qaportal");
                query4.ToString();
                this.Execute(query4.ToString());
            }
           
            int activityTimeId = Ministry_Fetch_Activity_TimeId(churchId, activityId, activityScheduleName);
            StringBuilder query5 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_TIME_ACTIVITY_INSTANCE (ACTIVITY_TIME_ID,ACTIVITY_INSTANCE_ID,CREATED_DATE,CREATED_BY_LOGIN) ");
            query5.AppendFormat("VALUES({0},{1},'{2}','{3}')", activityTimeId, activityInstanceId, startDate.AddHours(0.2).ToString("yyyy-MM-dd HH:mm"), "DEV\f1qaportal");
            query5.ToString();
            this.Execute(query5.ToString());

        }

        public void Ministry_Rosters_CreateForCheckIn(int churchId, int ministryId, string activityName, string[] rosterNameArray, string rosterGroupName = "All Rosters")
        {
            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            // Store roster group ID
            int rosterGroupId = this.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);

            // Create  rosters
            for (int i = 0; i < rosterNameArray.Length; i++)
            {
                StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, ACTIVITY_GROUP_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED)");
                query.AppendFormat("VALUES({0}, '{1}', {2}, {3}, 1, 1, 0, 0, 1, 0)", churchId, rosterNameArray[i], activityId, rosterGroupId);

                this.Execute(query.ToString());

            }
        }
        public void Ministry_Active_Detail_CheckIn(int churchId, string activityName, string rosterName, DateTime startDate)
        {

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            int activityDetailId = Ministry_fetch_Activity_DetailId(churchId, activityId, rosterName);
            int activityCheckInId = Ministry_Fetch_Activity_CheckInId(churchId, activityInstanceId);
            int isIpen = 1;
            int capacityNum = 200;
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVE_DETAIL_CHECKIN (ACTIVITY_DETAIL_ID,ACTIVE_CHECKIN_ID,CHURCH_ID,IS_OPEN,CAPACITY,CREATED_DATE,CREATED_BY_LOGIN) ");
            query.AppendFormat("VALUES({0},{1}, {2}, {3},{4},'{5}','{6}')", activityDetailId, activityCheckInId, churchId, isIpen, capacityNum, startDate.AddHours(0.1).ToString("yyyy-MM-dd HH:mm"), "DEV\f1qaportal");
            query.ToString();
            this.Execute(query.ToString());

        }
        public int Ministry_Activities_FetchRosterIDByName(int churchId, int activityId, String rosterName)
        {
            return Convert.ToInt32(this.Execute("SELECT ACTIVITY_DETAIL_ID FROM ChmActivity.dbo.ACTIVITY_DETAIL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1} AND ACTIVITY_DETAIL_NAME ='{2}'", churchId, activityId, rosterName).Rows[0]["ACTIVITY_DETAIL_ID"]);
        }

        /// <summary>
        /// This method creates an activity schedule for check in.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the schedule is tied to.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="endDate">The end date for the schedule.</param>
        public void Ministry_Activity_CreateLastAttendance(int churchId, string activityName, string studentName, string rosterName, DateTime startDate, DateTime endDate)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);           
           
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_INSTANCE (CHURCH_ID, ACTIVITY_ID, START_DATE_TIME,START_CHECKIN, END_CHECKIN,CREATED_DATE) ");
            query2.AppendFormat("VALUES({0},{1}, '{2}', '{3}', '{4}','{5}')", churchId, activityId, startDate.ToString("yyyy-MM-dd HH:mm"), startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"), startDate.ToString("yyyy-MM-dd HH:mm"));
            this.Execute(query2.ToString());
           
            TestLog.WriteLine("-startDate.ToString= {0}", startDate.ToString("yyyy-MM-dd HH:mm"));
            
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId, startDate);
            int individualId =People_Individuals_FetchIDByName(churchId, studentName);
            int rosterId =Ministry_Activities_FetchRosterIDByName(churchId, activityId, rosterName);

            StringBuilder query3 = new StringBuilder("INSERT INTO ChmActivity.dbo.INDIVIDUAL_INSTANCE ( INDIVIDUAL_ID, INDIVIDUAL_TYPE_ID, ACTIVITY_INSTANCE_ID,ACTIVITY_DETAIL_ID,CHURCH_ID,CHECK_IN_MACHINE_ID,CHECK_IN_TIME) ");
            query3.AppendFormat("VALUES({0}, 1, {1}, {2}, {3}, 16729, '{4}')", individualId, activityInstanceId, rosterId, churchId,startDate.AddSeconds(1).ToString("yyyy-MM-dd HH:mm"));
            this.Execute(query3.ToString());

        }

        /// <summary>
        /// This method creates an activity schedule for check in.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="studentName">The name of the student.</param>
        public void Ministry_Activity_DeleteLastAttendance(int churchId, string studentName)
        {
            int individualId = People_Individuals_FetchIDByName(churchId, studentName);

            StringBuilder query3 = new StringBuilder("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE");
            query3.AppendFormat(" WHERE INDIVIDUAL_ID = {0} AND CHURCH_ID = {1}", individualId, churchId);
            this.Execute(query3.ToString());

        }

        public void Ministry_ActivitySchedules_DeleteActivity(int churchId, string activityName)
        {
            //int activityId = Ministry_Activities_FetchID(churchId, activityName);
            int[] activityIds = Ministry_Activities_FetchIDS(churchId, activityName);
            if (Ministry_Activities_FetchID(churchId, activityName) == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return;
            }
            for (int k = 0; k < activityIds.Length; k++)
            {
                TestLog.WriteLine("The actvity id want to delete is: " + activityIds[k]);
                int activityId = activityIds[k];
                //int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
                int activityInstanceId = -1;
                int[] activityInstancesId = Ministry_Activities_FetchMultyInstanceID(churchId, activityId);
                for (int i = 0; i < activityInstancesId.Length; i++)
                {
                    TestLog.WriteLine("The actvity instance id want to delete is: " + activityInstanceId);
                    activityInstanceId = activityInstancesId[i];
                    if (activityInstanceId != -1)
                    {
                        string[] activityTimeIds = Ministry_Fetch_Activity_TimeIds(churchId, activityId);
                        foreach (string timeId in activityTimeIds)
                        {
                            TestLog.WriteLine("Try to delete activity instance of timeId: " + timeId + ", instance:" + activityInstanceId);
                            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME_ACTIVITY_INSTANCE WHERE ACTIVITY_TIME_ID = {0} and ACTIVITY_INSTANCE_ID ={1}", timeId, activityInstanceId);
                        }
                    }
                }

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1}", churchId, activityId);

                for (int i = 0; i < activityInstancesId.Length; i++)
                {
                    activityInstanceId = activityInstancesId[i];
                    if (activityInstanceId != -1)
                    {
                        this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID ={1}", activityInstanceId, churchId);

                        string[] activityDetailIds = Ministry_Fetch_Activity_DetailIds(churchId, activityId);
                        foreach (string detailId in activityDetailIds)
                            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVE_DETAIL_CHECKIN WHERE ACTIVITY_DETAIL_ID = {0} and CHURCH_ID ={1}", detailId, churchId);
                    }
                }

                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_PREFS WHERE ACTIVITY_ID ={0} and CHURCH_ID ={1} ", activityId, churchId);

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_DETAIL WHERE ACTIVITY_ID ={0} and CHURCH_ID ={1} ", activityId, churchId);

                for (int i = 0; i < activityInstancesId.Length; i++)
                {
                    activityInstanceId = activityInstancesId[i];
                    if (activityInstanceId != -1)
                    {
                        this.Execute("DELETE FROM ChmActivity.dbo.ACTIVE_CHECKIN WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID ={1} ", activityInstanceId, churchId);
                    }
                }

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_INSTANCE WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_GROUP WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_MINISTRY WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

                this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);
            }
            
        }
        public void Ministry_ActivitySchedules_DeleteActivityForSelfCheckIn(int churchId, string activityName)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            TestLog.WriteLine("The actvity id want to delete is: " + activityId);
            if (activityId == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return;
            }

            int[] activityInstancesId = Ministry_Activities_FetchMultyInstanceID(churchId, activityId);
            
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1}", churchId, activityId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_DETAIL WHERE ACTIVITY_ID ={0} and CHURCH_ID ={1} ", activityId, churchId);
          
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_INSTANCE WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_GROUP WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_MINISTRY WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1}", activityId, churchId);

        }
        public void Ministry_ActivitySchedules_DeleteIndividualCheckedIn(int churchId, string activityName, string individualName)
        {
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            if (individualId == -1)
            {
                TestLog.WriteLine("No individual with given name: " + individualName + " is found");
                return;
            }

            Ministry_ActivitySchedules_DeleteIndividualCheckedIn(churchId, activityName, individualId);
        }

        public void Ministry_ActivitySchedules_DeleteIndividualCheckedIn(int churchId, string activityName, int individualId)
        {
            TestLog.WriteLine("The individual id want to delete is: " + individualId);

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            if (activityId == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return;
            }
            TestLog.WriteLine("The actvity id want to delete is: " + activityId);

            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            TestLog.WriteLine("The actvity instance id want to delete is: " + activityInstanceId);
            if (activityInstanceId != -1)
            {
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID = {1} and INDIVIDUAL_ID = {2}",
                    activityInstanceId, churchId, individualId);
            }

            this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_PREFS WHERE ACTIVITY_ID = {0} and CHURCH_ID = {1} and INDIVIDUAL_ID = {2}", activityId, churchId, individualId);

        }

        /// <summary>
        /// This method fetches the Name of A Church TimeZone
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <returns>The Name of A Church TimeZone.</returns>
        public String Ministry_Activity_Instance_TimeZone(int churchId)
        {
            int timeZoneInfoId = -1;
            string proc = "[ChmPortal].[dbo].[Config_GetSettingConfigByKeys]";
            SqlDataReader dr;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@SettingID", 2));
                    cmd.Parameters.Add(new SqlParameter("@ConfigKeyTypeID1", 1));
                    cmd.Parameters.Add(new SqlParameter("@ConfigKeyValue1", null));
                    cmd.Parameters.Add(new SqlParameter("@ConfigKeyTypeID2", 2));
                    cmd.Parameters.Add(new SqlParameter("@ConfigKeyValue2", churchId));
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dr["VALUE"] != null) timeZoneInfoId = Convert.ToInt32(dr["VALUE"].ToString());
                        break;
                    }
                }

                dbConnection.Close();
            }

            //return Convert.ToString(this.Execute("SELECT STANDARD_NAME FROM ChmChurch.dbo.TIME_ZONE (NOLOCK) where TIME_ZONE_ID = (SELECT TOP 1 TIME_ZONE_ID FROM ChmChurch.dbo.CHURCH (NOLOCK) where CHURCH_ID ={0} )", churchId).Rows[0]["STANDARD_NAME"]);
            return Convert.ToString(this.Execute("SELECT STANDARD_NAME FROM ChmChurch.dbo.TIME_ZONE_INFO (NOLOCK) where TIME_ZONE_INFO_ID ={0} ", timeZoneInfoId).Rows[0]["STANDARD_NAME"]);

        }
        /// <summary>
        /// This method fetches the IndividualID of A LoginUser
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="Email">The User Account.</param>
        /// <returns>The IndividualID of A LoginUser.</returns>
        public int People_Individuals_FetchIDByEmail(int churchId, String Email)
        {           
            return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUALID FROM ChmPeople.dbo.USERLOGIN WITH (NOLOCK) WHERE CHURCHID = {0} AND EMAIL = '{1}'", churchId, Email).Rows[0]["INDIVIDUALID"]);

        }
        /// <summary>
        /// Gets the individual name for someone.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="intindividualID">The intindividualID.</param>
        /// <returns>The full name of the individual.  Includes Goes By name and middle initial.</returns>
        public string People_GetIndividualName(int churchId, int individualId )
        {
            return Convert.ToString(this.Execute(string.Format("SELECT TOP 1 Individual_Name FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_ID = {1}", churchId, individualId)).Rows[0]["Individual_Name"]);
        }
        /// <summary>
        /// This method fetches the individual id for an individual.
        /// </summary>
        /// <param name="churchId">The church id the individual belongs to.</param>
        /// <param name="individualName">The name of the individual.</param>
        /// <returns>Integer representing the individual.</returns>
        public int People_Individuals_FetchIDByName(int churchId, string individualName)
        {
            try
            {
                string[] name = individualName.Split(' ');
                if (name.Length == 2)
                {
                    TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
                    return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' ORDER BY CREATED_DATE desc", churchId, name[0], name[1]).Rows[0]["INDIVIDUAL_ID"]);
                }
                else if (name.Length == 3)
                {
                    TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1], name[2]));
                    return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND MIDDLE_NAME = '{2}' AND LAST_NAME = '{3}' ORDER BY CREATED_DATE desc", churchId, name[0], name[1], name[2]).Rows[0]["INDIVIDUAL_ID"]);

                }
                else
                {
                    return -1;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }
        public String People_Individuals_FetchBirthdayByName(int churchId, string individualName)
        {
            String birthDay = "";
            string[] name = individualName.Split(' ');
            if (name.Length == 2)
            {
                TestLog.WriteLine(string.Format("Individual Name: {0} {1}", name[0], name[1]));
                birthDay = Convert.ToString(this.Execute("SELECT CONVERT(varchar(20), DATE_OF_BIRTH, 20) AS DATE_OF_BIRTH  FROM ChmPeople.dbo.INDIVIDUAL WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' ORDER BY CREATED_DATE desc)", churchId, name[0], name[1]).Rows[0]["DATE_OF_BIRTH"]);
            }
            else if (name.Length == 3)
            {
                TestLog.WriteLine(string.Format("Individual Name: {0} {1} {2}", name[0], name[1], name[2]));
                birthDay = Convert.ToString(this.Execute("SELECT CONVERT(varchar(20), DATE_OF_BIRTH, 20) AS DATE_OF_BIRTH  FROM ChmPeople.dbo.INDIVIDUAL WHERE INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND MIDDLE_NAME = '{2}' AND LAST_NAME = '{3}' ORDER BY CREATED_DATE desc)", churchId, name[0], name[1], name[2]).Rows[0]["DATE_OF_BIRTH"]);

            }
            else
            {
                birthDay = "";
            }

            return birthDay;
        }
        public void People_Individuals_UpdateBirthdayByName(int churchId, string individualName, string birthDay)
        {
            string[] name = individualName.Split(' ');
            StringBuilder updateStr = new StringBuilder("UPDATE ChmPeople.dbo.INDIVIDUAL SET ");
            try
            {
                if (name.Length == 2)
                {
                    updateStr.AppendFormat("DATE_OF_BIRTH ='{0}'  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {1} AND FIRST_NAME = '{2}' AND LAST_NAME = '{3}' ORDER BY CREATED_DATE desc) ", birthDay, churchId, name[0], name[1]);
                    this.Execute(updateStr.ToString());
                }
                if (name.Length == 3)
                {
                    updateStr.AppendFormat("DATE_OF_BIRTH ='{0}'  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {1} AND FIRST_NAME = '{2}' AND MIDDLE_NAME = '{3}' AND LAST_NAME = '{4}' ORDER BY CREATED_DATE desc) ", birthDay, churchId, name[0], name[1], name[2]);
                    this.Execute(updateStr.ToString());
                }
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table ChmPeople.dbo.INDIVIDUAL update: {0}", e.Message);
                throw e;
            }


        }
        public void People_Individuals_UpdateTagCommentByName(int churchId, string individualName, string tagComment)
        {
            string[] name = individualName.Split(' ');
            StringBuilder updateStr = new StringBuilder("UPDATE ChmPeople.dbo.INDIVIDUAL SET ");
            try
            {
                if (name.Length == 2)
                {
                    if (tagComment == null || tagComment.Equals(""))
                    {
                        updateStr.AppendFormat("DEFAULT_TAG_COMMENT =null  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND LAST_NAME = '{2}' ORDER BY CREATED_DATE desc) ", churchId, name[0], name[1]);
                    }
                    else
                    {
                        updateStr.AppendFormat("DEFAULT_TAG_COMMENT ='{0}'  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {1} AND FIRST_NAME = '{2}' AND LAST_NAME = '{3}' ORDER BY CREATED_DATE desc) ", tagComment, churchId, name[0], name[1]);
                    }
                        this.Execute(updateStr.ToString());
                }
                if (name.Length == 3)
                {
                    if (tagComment == null || tagComment.Equals(""))
                    {
                        updateStr.AppendFormat("DEFAULT_TAG_COMMENT =null  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FIRST_NAME = '{1}' AND MIDDLE_NAME = '{2}' AND LAST_NAME = '{3}' ORDER BY CREATED_DATE desc) ", churchId, name[0], name[1], name[2]);
                    }
                    else
                    {
                        updateStr.AppendFormat("DEFAULT_TAG_COMMENT ='{0}'  WHERE  INDIVIDUAL_ID = (SELECT TOP 1 INDIVIDUAL_ID FROM ChmPeople.dbo.INDIVIDUAL WITH (NOLOCK) WHERE CHURCH_ID = {1} AND FIRST_NAME = '{2}' AND MIDDLE_NAME = '{3}' AND LAST_NAME = '{4}' ORDER BY CREATED_DATE desc) ", tagComment, churchId, name[0], name[1], name[2]);
                    }
                        this.Execute(updateStr.ToString());
                }
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table ChmPeople.dbo.INDIVIDUAL update: {0}", e.Message);
                throw e;
            }


        }
        public void People_Individuals_UpdateTagCode(int churchId, String activityName,string individualName, string tagCodeValue)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);            
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            StringBuilder updateStr = new StringBuilder("UPDATE ChmActivity.dbo.INDIVIDUAL_INSTANCE SET ");           
            try
            {
                updateStr.AppendFormat("TAG_CODE ='{0}'  WHERE CHURCH_ID = {1} AND ACTIVITY_INSTANCE_ID = {2} and INDIVIDUAL_ID ={3} ", tagCodeValue, churchId, activityInstanceId, individualId);
                this.Execute(updateStr.ToString());                
            }
            catch (Exception e)
            {
                TestLog.WriteLine("Error occured with table ChmActivity.dbo.INDIVIDUAL_INSTANCE update: {0}", e.Message);
                throw e;
            }
        }
        public String Individual_Instance_FetchCheckInTime(int churchId, String activityName, string individualName)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);           
            return Convert.ToString(this.Execute("SELECT CHECK_IN_TIME  FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} and  INDIVIDUAL_ID = {2}", churchId, activityInstanceId, individualId).Rows[0]["CHECK_IN_TIME"]);
        }
        public String Individual_Instance_FetchPresentTime(int churchId, String activityName, string individualName)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            return Convert.ToString(this.Execute("SELECT PRESENT_TIME  FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} and  INDIVIDUAL_ID = {2}", churchId, activityInstanceId, individualId).Rows[0]["PRESENT_TIME"]);
        }
        public String Individual_Instance_FetchCheckOutTime(int churchId, String activityName, string individualName)
        {
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            StringBuilder selectStr = new StringBuilder("SELECT CHECK_OUT_TIME  FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK)  ");
            selectStr.AppendFormat("WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} and  INDIVIDUAL_ID = {2}", churchId, activityInstanceId, individualId);
            TestLog.WriteLine("selectStr==", selectStr.ToString());
            //return Convert.ToString(this.Execute("SELECT CHECK_OUT_TIME  FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} and  INDIVIDUAL_ID = {2}", churchId, activityInstanceId, individualId).Rows[0]["CHECK_OUT_TIME"]);
            return Convert.ToString(this.Execute(selectStr.ToString()).Rows[0]["CHECK_OUT_TIME"]);
        }
        public String Ministry_Church_FetchName(int churchId)
        {
            return Convert.ToString(this.Execute("SELECT CHURCH_NAME FROM ChmChurch.dbo.CHURCH WITH (NOLOCK) WHERE CHURCH_ID = '{0}' ", churchId).Rows[0]["CHURCH_NAME"]);
        }
        public void Ministry_Activities_CreateForSelfCheckIn(int churchId, int ministryId, string activityName, DateTime createDate, int individualPrefsOverrideCloseRoom, string[] rosterNameArray, string rosterGroupName = "All Rosters")
        {
            // Create the activity
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY (CHURCH_ID, ACTIVITY_NAME, HAS_CHECKIN, CHECKIN_MINUTES_BEFORE, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, PAGER_ENABLED,CREATED_DATE, CREATED_BY_LOGIN,CHECKIN_CODE,FIXED_TIMES,HAS_SECURITY_AUTHORIZATION,TRACK_ATTENDANCE_BY_INDIVIDUAL,INDIVIDUAL_PREFS_OVERRIDE_CLOSED_ROOM) ");
            query.AppendFormat("VALUES({0}, '{1}', 1, 0, 1, 1, 1, 1, '{2}', 'ft.ministrytest',null,0,1,null,{3})", churchId, activityName, createDate.ToString("yyyy-MM-dd HH:mm"), individualPrefsOverrideCloseRoom);

            this.Execute(query.ToString());

            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            TestLog.WriteLine("The new check in activity created as activityId: " + activityId);
            // Tie it to a ministry
            query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_MINISTRY (MINISTRY_ID, ACTIVITY_ID, CHURCH_ID, IS_PRIMARY, CREATED_DATE, CREATED_BY_LOGIN)");
            query.AppendFormat("VALUES({0}, {1}, {2}, 0, '{3}', 'ft.ministrytest')", ministryId, activityId, churchId, createDate.ToString("yyyy-MM-dd HH:mm"));

            this.Execute(query.ToString());

            // Create Roster Group
            this.Ministry_ActivityRLCGroups_Create(churchId, activityName, rosterGroupName);

            // Store roster group ID
            int rosterGroupId = this.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);

            // Create  rosters
            for (int i = 0; i < rosterNameArray.Length; i++)
            {
                query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, ACTIVITY_GROUP_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED)");
                query.AppendFormat("VALUES({0}, '{1}', {2}, {3}, 1, 1, 0, 0, 1, 0)", churchId, rosterNameArray[i], activityId, rosterGroupId);

                this.Execute(query.ToString());

            }


        }
        public void PartiAssignment_CreateForSelfCheckIn(int churchId, string activityName, string activityScheduleName, string rosterName, string[] individualNameArray, DateTime createDate)
        {

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_TIME (CHURCH_ID, ACTIVITY_ID, ACTIVITY_TIME_NAME, ACTIVITY_START_TIME, ACTIVITY_END_TIME) ");
            
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);

            int activityTimeId = Ministry_Fetch_Activity_TimeId(churchId, activityId,activityScheduleName);

            int activityDetailId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterName);

            //individualGroupId

            this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_PREFS WHERE ACTIVITY_ID = {0} and CHURCH_ID ={1}", activityId, churchId);
            StringBuilder query1 = new StringBuilder("");
            for(int i= 0; i<individualNameArray.Length; i++)
            {
                query1.Clear();
                query1.Append("");
                int individualId = People_Individuals_FetchIDByName(churchId, individualNameArray[i]);
                if(individualId!=-1)
                {
                    query1.Append("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS (INDIVIDUAL_ID,CHURCH_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, ACTIVITY_TIME_ID, ACTIVITY_INSTANCE_ID,INDIVIDUAL_GROUP_ID,CREATED_DATE) ");
                    query1.AppendFormat("VALUES({0},{1}, {2}, {3}, {4}, {5}, null, '{6}')", individualId,churchId,activityId, activityDetailId, activityTimeId,activityInstanceId,createDate.ToString("yyyy-MM-dd HH:mm"));
                    this.Execute(query1.ToString());
                }
            }
           
        }
        public void PartiAssignment_CreateForSelfCheckInAssignRule(int churchId, string activityName, string activityScheduleName, String[] rosterArray, string[] individualNameArray, DateTime createDate)
        {

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            
            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);

            int activityTimeId = Ministry_Fetch_Activity_TimeId(churchId, activityId, activityScheduleName);

            int activityDetailId0 = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterArray[0]);
            int activityDetailId1 = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterArray[1]);
            int activityDetailId2 = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterArray[2]);

            //individualGroupId
            StringBuilder query1 = new StringBuilder("");
            for (int i = 0; i < individualNameArray.Length; i++)
            {
                query1.Clear();
                query1.Append("");
                int individualId = People_Individuals_FetchIDByName(churchId, individualNameArray[i]);
                if (individualId != -1)
                {
                    query1.Append("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS (INDIVIDUAL_ID,CHURCH_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, ACTIVITY_TIME_ID, ACTIVITY_INSTANCE_ID,INDIVIDUAL_GROUP_ID,CREATED_DATE) ");
                    query1.AppendFormat("VALUES({0},{1}, {2}, {3}, null, null, null, '{4}')", individualId, churchId, activityId, activityDetailId0, createDate.ToString("yyyy-MM-dd HH:mm"));
                    this.Execute(query1.ToString());
                    query1.Clear();
                    query1.Append("");
                    query1.Append("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS (INDIVIDUAL_ID,CHURCH_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, ACTIVITY_TIME_ID, ACTIVITY_INSTANCE_ID,INDIVIDUAL_GROUP_ID,CREATED_DATE) ");
                    query1.AppendFormat("VALUES({0},{1}, {2}, {3}, {4}, null, null, '{5}')", individualId, churchId, activityId, activityDetailId1, activityTimeId, createDate.ToString("yyyy-MM-dd HH:mm"));
                    this.Execute(query1.ToString());
                    query1.Clear();
                    query1.Append("");
                    query1.Append("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS (INDIVIDUAL_ID,CHURCH_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, ACTIVITY_TIME_ID, ACTIVITY_INSTANCE_ID,INDIVIDUAL_GROUP_ID,CREATED_DATE) ");
                    query1.AppendFormat("VALUES({0},{1}, {2}, {3}, null, {4}, null, '{5}')", individualId, churchId, activityId, activityDetailId2, activityInstanceId, createDate.ToString("yyyy-MM-dd HH:mm"));
                    this.Execute(query1.ToString());
                }
            }

        }
        public void VolStaffAssignment_CreateForSelfCheckIn(int churchId, string activityName, string activityScheduleName, string rosterName, string[] individualNameArray, int individualTypeId, DateTime createDate)
        {

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_TIME (CHURCH_ID, ACTIVITY_ID, ACTIVITY_TIME_NAME, ACTIVITY_START_TIME, ACTIVITY_END_TIME) ");

            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);

            int activityTimeId = Ministry_Fetch_Activity_TimeId(churchId, activityId, activityScheduleName);

            int activityDetailId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterName);

            int staffingScheduleId = this.SelfChekin_FetchStaffingScheduleID(churchId, activityId);
            //individualGroupId

            this.Execute("DELETE FROM ChmActivity.dbo.STAFFING_PREF WHERE ACTIVITY_ID = {0} and CHURCH_ID ={1}", activityId, churchId);
            StringBuilder query1 = new StringBuilder("");
            for (int i = 0; i < individualNameArray.Length; i++)
            {
                query1.Clear();
                query1.Append("");
                int individualId = People_Individuals_FetchIDByName(churchId, individualNameArray[i]);
                if (individualId != -1)
                {
                    query1.Append("INSERT INTO ChmActivity.dbo.STAFFING_PREF (INDIVIDUAL_ID,INDIVIDUAL_TYPE_ID,CHURCH_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, ACTIVITY_TIME_ID, ACTIVITY_INSTANCE_ID,STAFFING_SCHEDULE_ID,INDIVIDUAL_GROUP_ID,CREATED_DATE) ");
                    query1.AppendFormat("VALUES({0},{1}, {2}, {3}, {4}, {5}, {6}, {7}, null, '{8}')", individualId, individualTypeId, churchId, activityId, activityDetailId, activityTimeId, activityInstanceId, staffingScheduleId, createDate.ToString("yyyy-MM-dd HH:mm"));
                    this.Execute(query1.ToString());
                }
            }

        }
        public void Ministry_ActivitySchedules_DeleteActivitiesForSelfChekIn(int churchId, string activityName)
        {
            int[] activityIds = Ministry_Activities_FetchIDS(churchId, activityName);
            for (int index = 0; index < activityIds.Length; index++ )
            {
                int activityId = activityIds[index];
                this.Execute("DELETE FROM ChmActivity.dbo.STAFFING_PREF WHERE ACTIVITY_ID = {0} and CHURCH_ID ={1}", activityId, churchId);
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_PREFS WHERE ACTIVITY_ID = {0} and CHURCH_ID ={1}", activityId, churchId);
                Ministry_ActivitySchedules_DeleteActivity(churchId, activityName);
            }

        }

        public int SelfChekin_FetchStaffingScheduleID(int churchId, int activityId)
        {
            try
            {
                return Convert.ToInt32(this.Execute("SELECT TOP 1 STAFFING_SCHEDULE_ID FROM ChmActivity.dbo.STAFFING_SCHEDULE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1}", churchId, activityId).Rows[0]["STAFFING_SCHEDULE_ID"]);
            }
            catch (System.IndexOutOfRangeException)
            {
                return -1;
            }

        }

        public void Set_Feature_DarkOrLight(int featureToggleID, int featureToggleModeID)
        {
            DataTable requirementDT = this.Execute("SELECT FeatureToggleID FROM [ChmPortal].[dbo].[FeatureToggle] WITH (NOLOCK) WHERE FeatureToggleID = {0}", featureToggleID);

            if (requirementDT.Rows.Count > 0)
            {
                this.Execute(string.Format("UPDATE [ChmPortal].[dbo].[FeatureToggle] SET FeatureToggleModeID = {0} WHERE FeatureToggleID = {1} ", featureToggleModeID, featureToggleID));
            }
            else
            {
                throw new Exception(string.Format("Requirement id for FeatureToggle id [{1}] was not found", featureToggleID));
            }
            
        }
        public void Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(int churchId, string activityName, string individualName)
        {
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            if (individualId == -1)
            {
                TestLog.WriteLine("No individual with given name: " + individualName + " is found");
                return;
            }

            Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(churchId, activityName, individualId);
        }

        public void Ministry_ActivitySchedules_DeleteIndividualSelfCheckedIn(int churchId, string activityName, int individualId)
        {
            TestLog.WriteLine("The individual id want to delete is: " + individualId);

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            if (activityId == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return;
            }
            TestLog.WriteLine("The actvity id want to delete is: " + activityId);

            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            TestLog.WriteLine("The actvity instance id want to delete is: " + activityInstanceId);
            if (activityInstanceId != -1)
            {
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID = {1} and INDIVIDUAL_ID = {2}",
                    activityInstanceId, churchId, individualId);
            }
        }
        public void Ministry_ActivitySchedules_DeleteAllSelfCheckedIn(int churchId, string activityName)
        {
            
            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            if (activityId == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return;
            }
            TestLog.WriteLine("The actvity id want to delete is: " + activityId);

            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            TestLog.WriteLine("The actvity instance id want to delete is: " + activityInstanceId);
            if (activityInstanceId != -1)
            {
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID = {1}",
                    activityInstanceId, churchId);
            }
        }
        public bool Ministry_ActivitySchedules_IsSuccSelfCheckedIn(int churchId, string activityName, String individualName)
        {
            bool isSuccflag = false;
            int count = 0;
            int individualId = People_Individuals_FetchIDByName(churchId, individualName);
            if (individualId == -1)
            {
                TestLog.WriteLine("No individual with given name: " + individualName + " is found");
                return false;
            }

            int activityId = Ministry_Activities_FetchID(churchId, activityName);
            if (activityId == -1)
            {
                TestLog.WriteLine("No activity with given name: " + activityName + " is found");
                return false;
            }

            int activityInstanceId = Ministry_Activities_FetchInstanceID(churchId, activityId);
            
            if (activityInstanceId != -1)
            {
                this.Execute("SELECT count(*)  AS COUNT FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID = {1} and INDIVIDUAL_ID = {2}",
                    activityInstanceId, churchId, individualId);
                try
                {
                    count = Convert.ToInt32(this.Execute("SELECT count(*)  AS COUNT FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE ACTIVITY_INSTANCE_ID = {0} and CHURCH_ID = {1} and INDIVIDUAL_ID = {2}", activityInstanceId, churchId, individualId).Rows[0]["COUNT"]);
                    TestLog.WriteLine("count==" + count);
                    if (count > 0)
                    {
                        isSuccflag = true;
                    }
                    else if (count == 0) 
                    {
                        isSuccflag = false;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    return false;
                }

            }
            return isSuccflag;
        }

        public int Ministry_Activities_FetchIndividualInstanceID(int churchId,int activityInstanceId, int individualId, int activityDetailId)
        {

            //SELECT * FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = 15 AND ACTIVITY_INSTANCE_ID = 34196066  and  INDIVIDUAL_ID = 30073238 and ACTIVITY_DETAIL_ID = 568679
            TestLog.WriteLine(string.Format("Select = SELECT TOP 1 INDIVIDUAL_INSTANCE_ID FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} AND INDIVIDUAL_ID = {2} AND ACTIVITY_DETAIL_ID = {3} ", churchId, activityInstanceId, individualId, activityDetailId));
            try
            {
                return Convert.ToInt32(this.Execute("SELECT TOP 1 INDIVIDUAL_INSTANCE_ID FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} AND INDIVIDUAL_ID = {2} AND ACTIVITY_DETAIL_ID = {3} ", churchId, activityInstanceId, individualId, activityDetailId).Rows[0]["INDIVIDUAL_INSTANCE_ID"]);
            }
            catch (System.IndexOutOfRangeException)
            {
                return -1;
            }

        }
        public void  Ministry_Activities_DeleteIndividualInstanceID(int churchId, int activityInstanceId, int individualId, int activityDetailId)
        {
            try
            {
               TestLog.WriteLine(string.Format("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} AND INDIVIDUAL_ID = {2} AND ACTIVITY_DETAIL_ID = {3} ", churchId, activityInstanceId, individualId, activityDetailId));
                this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_INSTANCE WHERE CHURCH_ID = {0} AND ACTIVITY_INSTANCE_ID = {1} AND INDIVIDUAL_ID = {2} AND ACTIVITY_DETAIL_ID = {3} ", churchId, activityInstanceId, individualId, activityDetailId);
            }
            catch (Exception e)
            {
                throw new Exception("No INDIVIDUAL_INSTANCE to be deleteed");
            }
            

        }
        // add by Grace Zhang end
        /// <summary>
        /// This method fetches the id for an church.
        /// </summary>
        /// <param name="churchCode">The code of the church.</param>
        /// <returns>The integer id for the roster.</returns>
        public int Ministry_Church_FetchID(String churchCode)
        {
            return Convert.ToInt32(this.Execute("SELECT CHURCH_ID FROM ChmChurch.dbo.CHURCH WITH (NOLOCK) WHERE CHURCH_CODE = '{0}' ", churchCode).Rows[0]["CHURCH_ID"]);
        }
        
        /// <summary>
        /// This method fetches the id for a requirement
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="requirementName">The name of the requirement</param>
        /// <returns>The integer id for the requirement</returns>
        public int Ministry_Activities_Requirement_FetchID(int churchId, string requirementName)
        {
            string id = string.Empty;
            DataTable requirementDT = this.Execute("SELECT REQUIREMENT_ID FROM ChmPeople.dbo.REQUIREMENT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND REQUIREMENT_NAME = '{1}' AND IS_ACTIVE = 1", churchId, requirementName);

            if (requirementDT.Rows.Count > 0)
            {
                id = requirementDT.Rows[0]["REQUIREMENT_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Requirement [{0}] for church id [{1}] was not found", requirementName, churchId));
            }

            return Convert.ToInt32(id);
        }

        /// <summary>
        /// This method will delete a single activity
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="activityName"></param>
        public void Ministry_Activities_Delete_Activity(int churchId, string activityName)
        {
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            
            if (activityId == -1) {
                log.Warn("Didn't find given activity when deletion, activity name: " + activityName);
                return;
            }

            this.Execute("DELETE FROM ChmActivity.dbo.individual_prefs WHERE church_id = {0} and activity_id = {1}", churchId, activityId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_DETAIL WHERE church_id = {0} and activity_id = {1}", churchId, activityId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_GROUP WHERE church_id = {0} and activity_id = {1}", churchId, activityId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_MINISTRY WHERE church_id = {0} and activity_id = {1}", churchId, activityId);

            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY WHERE church_id = {0} and activity_id = {1}", churchId, activityId);
            
        }

        /// <summary>
        /// This method deletes all activities under a particular ministry within a particular church.
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="ministryName">The name of the ministry</param>
        public void Ministry_Activities_DeleteAll(int churchId, string ministryName)
        {
            int ministryId;

            try
            {
                ministryId = Ministry_Ministries_FetchID(churchId, ministryName);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("No ministry ID found for church {0} and ministry name {1}", churchId, ministryName));
            }
            log.Debug(string.Format("Ministry ID = {0}", ministryId));

            StringBuilder query = new StringBuilder("DECLARE @MinistryID INT ");
            query.AppendFormat("SELECT @MinistryID = {0} ", ministryId);

            query.Append("CREATE TABLE #Activity (ActivityID INT) ");
            query.Append("INSERT #Activity ( ActivityID ) ");
            query.Append("SELECT Activity_ID FROM ChmActivity.dbo.ACTIVITY_MINISTRY AS am WITH (NOLOCK) WHERE MINISTRY_ID = @MinistryID ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_TIME_ACTIVITY_INSTANCE ");
            query.Append("WHERE ACTIVITY_INSTANCE_ID IN (SELECT ACTIVITY_INSTANCE_ID FROM ChmActivity.dbo.ACTIVITY_INSTANCE AS ai WITH (NOLOCK) WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a)) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_INSTANCE ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_TIME ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_DETAIL ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_GROUP ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DELETE ChmActivity.dbo.ACTIVITY_MINISTRY ");
            query.Append("WHERE ACTIVITY_ID IN (SELECT ActivityID FROM #Activity AS a) ");

            query.Append("DROP TABLE #Activity ");

            this.Execute(query.ToString());
        }


        /// <summary>
        /// Retrieve Requirement Counters for Missing, Pending, Approved, NotApproved, Expired, Completed, and Conditional status
        /// </summary>
        /// <param name="churchID">The Church ID</param>
        /// <param name="activityName">Activity Name</param>
        /// <param name="requirementName">Requirement Name</param>
        /// <returns>Returns a Dictionary so user can get value based on keys: Missing, Pending, Approved, NotApproved, Expired, Completed, and Conditional</returns>
        public Dictionary<string, string> Ministry_Activities_Activity_Requirements(int churchID, string activityName, string requirementName)
        {
            string proc = "[ChmPeople].[dbo].[Individual_Activity_Requirements]";
            SqlDataReader dr;
            Dictionary<string, string> requirementStatusCount = new Dictionary<string, string>();

            //Requirement Status ID Value Count
            int missingCount = 0; //0
            int pendingCount = 0; //1
            int approvedCount = 0; //2
            int notApprovedCount = 0; //3
            int expiredCount = 0; //4
            int completedCount = 0; //5
            int conditionalCount = 0; //6

            int activityID = Ministry_Activities_FetchID(15, activityName);
            int requirementID = Ministry_Activities_Requirement_FetchID(churchID, requirementName);

            //TestLog.WriteLine("Requirement ID: {0}", requirementID);

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));
                    cmd.Parameters.Add(new SqlParameter("@ActivityID", activityID));

                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        //Check for requirement ID and then 
                        //Get the value in req status id
                        
                            //TestLog.WriteLine("DB Requirement ID: {0}", dr["REQUIREMENT_ID"]);
                            if (dr["REQUIREMENT_ID"].ToString() == requirementID.ToString())
                            {
                                //Go through value and count them
                                //TestLog.WriteLine("Requirement Status ID: {0}", dr["REQUIREMENT_STATUS_ID"].ToString());
                                switch (dr["REQUIREMENT_STATUS_ID"].ToString())
                                {
                                    case "1":
                                        pendingCount++;
                                        break;
                                    case "2":
                                        //TestLog.WriteLine("Approved {0}", approvedCount);
                                        approvedCount++;
                                        break;
                                    case "3":
                                        notApprovedCount++;
                                        break;
                                    case "4":
                                        expiredCount++;
                                        break;
                                    case "5":
                                        completedCount++;
                                        break;
                                    case "6":
                                        conditionalCount++;
                                        break;
                                    default:
                                        missingCount++;
                                        break;
                                        
                                }//End of switch
                            }//End of requirement check

                    }//end of while read

                }//end of using call

                dbConnection.Close();

            }//end of using call

            if (missingCount == 0)
            {
                requirementStatusCount.Add("Missing", "");
            }
            else
            {
                requirementStatusCount.Add("Missing", missingCount.ToString());
            }

            if (pendingCount == 0)
            {
                requirementStatusCount.Add("Pending", "");
            }
            else
            {
                requirementStatusCount.Add("Pending", pendingCount.ToString());
            }

            if (approvedCount == 0)
            {
                requirementStatusCount.Add("Approved", "");
            }
            else 
            { 
                requirementStatusCount.Add("Approved", approvedCount.ToString()); 
            }

            if (notApprovedCount == 0)
            {
                requirementStatusCount.Add("NotApproved", "");
            }
            else
            {
                requirementStatusCount.Add("NotApproved", notApprovedCount.ToString());
            }
            
            if (expiredCount == 0)
            {
                requirementStatusCount.Add("Expired", "");
            }
            else
            {
                requirementStatusCount.Add("Expired", expiredCount.ToString());
            }
            
            if (completedCount == 0)
            {
                requirementStatusCount.Add("Completed", "");
            }
            else
            {
                requirementStatusCount.Add("Completed", completedCount.ToString());
            }

            if (conditionalCount == 0)
            {
                requirementStatusCount.Add("Conditional", "");
            }
            else 
            {
                requirementStatusCount.Add("Conditional", conditionalCount.ToString());
            }

            return requirementStatusCount;

        }

        /// <summary>
        /// This method fetches the checkin code for an activity.
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="activityName"></param>
        /// <returns>The integer check-in code for the activity</returns>
        public int Ministry_CheckIn_FetchCheckInCode(int churchId, string activityName)
        {
            return Convert.ToInt32(this.Execute("SELECT CHECKIN_CODE FROM [ChmActivity].[dbo].[ACTIVITY] WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}'", churchId, activityName).Rows[0]["CHECKIN_CODE"]);
        }

        /// <summary>
        /// This method creates an activity with a roster and ties it to a ministry
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="ministryId">The ministry ID</param>
        /// <param name="activityName">The name of the activity</param>
        /// <param name="rosterName">The name of the roster</param>
        public void Ministry_Activities_Create(int churchId, int ministryId, string activityName, string rosterName, string rosterGroupName = "All Rosters")
        {
            // Create the activity
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY (CHURCH_ID, ACTIVITY_NAME, HAS_CHECKIN, CHECKIN_MINUTES_BEFORE, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, CREATED_DATE, CREATED_BY_LOGIN) ");
            query.AppendFormat("VALUES({0}, '{1}', 0, 0, 0, 0, 1, GetDate(), 'ft.ministrytest')", churchId, activityName);

            this.Execute(query.ToString());

            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            // Tie it to a ministry
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_MINISTRY (MINISTRY_ID, ACTIVITY_ID, CHURCH_ID, IS_PRIMARY, CREATED_DATE, CREATED_BY_LOGIN)");
            query2.AppendFormat("VALUES({0}, {1}, {2}, 0, GetDate(), 'ft.ministrytest')", ministryId, activityId, churchId);

            this.Execute(query2.ToString());

            // Create Roster Group
            this.Ministry_ActivityRLCGroups_Create(churchId, activityName, rosterGroupName);

            // Store roster group ID
            int rosterGroupId = this.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);

            // Create a roster
            StringBuilder query3 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, ACTIVITY_GROUP_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED)");
            query3.AppendFormat("VALUES({0}, '{1}', {2}, {3}, 0, 0, 0, 0, 1, 0)", churchId, rosterName, activityId, rosterGroupId);

            this.Execute(query3.ToString());

        }
        /// <summary>
        /// This method creates an activity with multy rosters and ties it to a ministry
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="ministryId">The ministry ID</param>
        /// <param name="activityName">The name of the activity</param>
        /// <param name="rosterNameArray">The name of the roster Array</param>
        public void Ministry_Activities_Create(int churchId, int ministryId, string activityName, string[] rosterNameArray, string rosterGroupName = "All Rosters")
        {
            // Create the activity
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY (CHURCH_ID, ACTIVITY_NAME, HAS_CHECKIN, CHECKIN_MINUTES_BEFORE, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, CREATED_DATE, CREATED_BY_LOGIN) ");
            query.AppendFormat("VALUES({0}, '{1}', 0, 0, 0, 0, 1, GetDate(), 'ft.ministrytest')", churchId, activityName);

            this.Execute(query.ToString());

            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            // Tie it to a ministry
            StringBuilder query2 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_MINISTRY (MINISTRY_ID, ACTIVITY_ID, CHURCH_ID, IS_PRIMARY, CREATED_DATE, CREATED_BY_LOGIN)");
            query2.AppendFormat("VALUES({0}, {1}, {2}, 0, GetDate(), 'ft.ministrytest')", ministryId, activityId, churchId);

            this.Execute(query2.ToString());

            // Create Roster Group
            this.Ministry_ActivityRLCGroups_Create(churchId, activityName, rosterGroupName);

            // Store roster group ID
            int rosterGroupId = this.Ministry_RosterFolders_FetchID(churchId, rosterGroupName, activityId);

            // Create  rosters
            for (int i = 0; i < rosterNameArray.Length; i++)
            {
                StringBuilder query3 = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, ACTIVITY_GROUP_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED)");
                query3.AppendFormat("VALUES({0}, '{1}', {2}, {3}, 0, 0, 0, 0, 1, 0)", churchId, rosterNameArray[i], activityId, rosterGroupId);

                this.Execute(query3.ToString());

            }
            

        }

        #endregion Activities

        #region Activity Schedules
        /// <summary>
        /// This method fetches the id of an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        /// <returns>Integer id for the activity schedule.</returns>
        public int Ministry_ActivitySchedules_FetchID(int churchId, string activityScheduleName) {
            return Convert.ToInt32(this.Execute("SELECT ACTIVITY_TIME_ID FROM ChmActivity.dbo.ACTIVITY_TIME WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_NAME = '{1}'", churchId, activityScheduleName).Rows[0]["ACTIVITY_TIME_ID"]);
        }

        /// <summary>
        /// This method fetches the id of an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        /// <returns>Integer id for the activity schedule.</returns>
        public int Ministry_ActivitySchedules_FetchID_Single(int churchId, string activityScheduleName)
        {
            return Convert.ToInt32(this.Execute("SELECT TOP 1 ACTIVITY_TIME_ID FROM ChmActivity.dbo.ACTIVITY_TIME WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_NAME = '{1}' ORDER BY ACTIVITY_TIME_ID DESC", churchId, activityScheduleName).Rows[0]["ACTIVITY_TIME_ID"]);
        }

        /// <summary>
        /// This method creates an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the schedule is tied to.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        /// <param name="startDate">The start date for the schedule.</param>
        /// <param name="endDate">The end date for the schedule.</param>
        public void Ministry_ActivitySchedules_Create(int churchId, string activityName, string activityScheduleName, DateTime startDate, DateTime endDate) {
            StringBuilder query = new StringBuilder("DECLARE @activityID INT ");
            query.AppendFormat("SET @activityID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1) ", churchId, activityName);
            query.Append("INSERT INTO ChmActivity.dbo.ACTIVITY_TIME (CHURCH_ID, ACTIVITY_ID, ACTIVITY_TIME_NAME, ACTIVITY_START_TIME, ACTIVITY_END_TIME) ");
            query.AppendFormat("VALUES({0}, @activityID, '{1}', '{2}', '{3}')", churchId, activityScheduleName, startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"));
            
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityScheduleNames">The name list of the activity schedule.</param>
        public void Ministry_ActivitySchedules_BulkDelete(int churchId, string[] activityScheduleNames)
        {
            for (int index = 0; index < activityScheduleNames.Length; index++)
            {
                Ministry_ActivitySchedules_Delete(churchId,  activityScheduleNames[index]);
            }
        }

        /// <summary>
        /// This method deletes an activity schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityScheduleName">The name of the activity schedule.</param>
        public void Ministry_ActivitySchedules_Delete(int churchId, string activityScheduleName) {
            try
            {
                this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_NAME = '{1}'", churchId, activityScheduleName));
            }
            catch (Exception)
            {
                this.Execute(string.Format("DELETE FROM ChmActivity.dbo.RG_ACTIVITY_REGISTRATION_SCHEDULE WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_ID in ({1})", churchId, 
                    this.Ministry_ActivitySchedules_FetchID(churchId, activityScheduleName)));
                
                this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_TIME WHERE CHURCH_ID = {0} AND ACTIVITY_TIME_NAME = '{1}'", churchId, activityScheduleName));
            }
        }
        #endregion Activity Schedules

        #region Breakout Groups
        /// <summary>
        /// Fetches the ID for a breakout group
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="breakoutGroupName">The breakout group name</param>
        /// <param name="activityName">The activity name</param>
        /// <returns>The integer ID for the breakout group</returns>
        public int Ministry_Activities_BreakoutGroups_FetchID(int churchId, string breakoutGroupName, string activityName)
        {
            int activityId = this.Ministry_Activities_FetchID(15, activityName);

            return Convert.ToInt32(this.Execute("SELECT INDIVIDUAL_GROUP_ID FROM ChmActivity.dbo.INDIVIDUAL_GROUP WITH (NOLOCK) WHERE CHURCH_ID = {0} AND INDIVIDUAL_GROUP_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, breakoutGroupName, activityId).Rows[0]["INDIVIDUAL_GROUP_ID"]);
        }

        /// <summary>
        /// Creates a breakout group
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="activityName">The activity name</param>
        /// <param name="breakoutGroupName">The breakout group name</param>
        public void Ministry_Activities_BreakoutGroups_Create(int churchId, string activityName, string breakoutGroupName)
        {
            // Store the activity ID
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            // Create the breakout group
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.INDIVIDUAL_GROUP (CHURCH_ID, INDIVIDUAL_GROUP_NAME, ACTIVITY_ID, CREATED_DATE, CREATED_BY_LOGIN) ");
            query.AppendFormat("VALUES({0}, '{1}', {2}, GetDate(), 'ft.ministrytest')", churchId, breakoutGroupName, activityId);

            this.Execute(query.ToString());

        }

        /// <summary>
        /// Deletes a breakout group
        /// </summary>
        /// <param name="churchId">The Church ID</param>
        /// <param name="activityName">The activity name</param>
        /// <param name="breakoutGroupName">The breakout group name</param>
        public void Ministry_Activities_BreakoutGroups_Delete(int churchId, string activityName, string breakoutGroupName)
        {
            int activityId = -1;
            try
            {
                // Store activity ID
                activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            }
            catch (IndexOutOfRangeException e)
            {
                log.Warn("Didn't find given activity when deletion, activity name: " + activityName);
                return;
            }

            // Delete breakout group
            this.Execute("DELETE FROM ChmActivity.dbo.INDIVIDUAL_GROUP WHERE CHURCH_ID = {0} AND INDIVIDUAL_GROUP_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, breakoutGroupName, activityId);

        }
        #endregion Breakout Groups

        #region Activity RLC Groups
        /// <summary>
        /// This method creates an activity rlc group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the activity rlc group will be tied to.</param>
        /// <param name="activityRLCGroupName">The name of the activity rlc group.</param>
        public void Ministry_ActivityRLCGroups_Create(int churchId, string activityName, string activityRLCGroupName) {
            StringBuilder query = new StringBuilder("DECLARE @activityID INT ");
            query.AppendFormat("SET @activityID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1) ", churchId, activityName);
            query.Append("INSERT INTO ChmActivity.dbo.ACTIVITY_GROUP (CHURCH_ID, ACTIVITY_ID, ACTIVITY_GROUP_NAME, ACTIVITY_GROUP_SORT, CHECKIN_BALANCE_TYPE) ");
            query.AppendFormat("VALUES({0}, @activityID, '{1}', 0, 0)", churchId, activityRLCGroupName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes an activity rlc group.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the activity rlc group is tied to.</param>
        /// <param name="activityRLCGroupName">The name of the activity rlc group.</param>
        public void Ministry_ActivityRLCGroups_Delete(int churchId, string activityName, string activityRLCGroupName) {
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_GROUP WHERE CHURCH_ID = {0} AND ACTIVITY_ID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1) AND ACTIVITY_GROUP_NAME = '{2}'", churchId, activityName, activityRLCGroupName);
        }

        #endregion Activity RLC Groups

        #region Staffing Schedules
        /// <summary>
        /// This method fetches the ID for a staff schedule
        /// </summary>
        /// <param name="churchId">The church ID</param>
        /// <param name="activityId">The activity ID</param>
        /// <param name="staffScheduleName">The staff schedule name</param>
        /// <returns></returns>
        public int Ministry_StaffingSchedules_FetchID(int churchId, int activityId, string staffScheduleName)
        {
            DataTable results = this.Execute("SELECT STAFFING_SCHEDULE_ID FROM ChmActivity.dbo.STAFFING_SCHEDULE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND STAFFING_SCHEDULE_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, staffScheduleName, activityId);

            int retVal = results.Rows.Count > 0 ? Convert.ToInt32(results.Rows[0]["STAFFING_SCHEDULE_ID"]) : int.MinValue;
            return retVal;
        }

        /// <summary>
        /// This method creates a staffing schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name this schedule will be tied to.</param>
        /// <param name="staffingScheduleName">The name of the staffing schedule.</param>
        public void Ministry_StaffingSchedules_Create(int churchId, string activityName, string staffingScheduleName) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.STAFFING_SCHEDULE (CHURCH_ID, ACTIVITY_ID, STAFFING_SCHEDULE_NAME) ");
            query.AppendFormat("SELECT {0}, ACTIVITY_ID, '{1}' FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{2}' AND IS_ACTIVE = 1", churchId, staffingScheduleName, activityName);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a staffing schedule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name the schedule is tied to.</param>
        /// <param name="staffingScheduleName">The name of the staffing schedule.</param>
        public void Ministry_StaffingSchedules_Delete(int churchId, string activityName, string staffingScheduleName) {
            StringBuilder query = new StringBuilder("DECLARE @activityID INT ");
            query.AppendFormat("SET @activityID = (SELECT TOP 1 ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE ACTIVITY_NAME = '{0}' AND CHURCH_ID = {1} AND IS_ACTIVE = 1) ", activityName, churchId);
            query.AppendFormat("DELETE FROM ChmActivity.dbo.STAFFING_PREF WHERE CHURCH_ID = {0} AND ACTIVITY_ID = @activityID AND STAFFING_SCHEDULE_ID IN (SELECT STAFFING_SCHEDULE_ID FROM ChmActivity.dbo.STAFFING_SCHEDULE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND STAFFING_SCHEDULE_NAME = '{1}') ", churchId, staffingScheduleName);
            query.AppendFormat("DELETE FROM ChmActivity.dbo.STAFFING_SCHEDULE WHERE CHURCH_ID = {0} AND ACTIVITY_ID = @activityID AND STAFFING_SCHEDULE_NAME = '{1}'", churchId, staffingScheduleName);

            this.Execute(query.ToString());
        }
        #endregion Staffing Schedules

        #region Staffing Assignments
        /// <summary>
        /// This method creates a staffing assignment *** ONLY CREATES HIGH LEVEL DATA AT PRESENT!! ***
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="individualTypeId">The individual type id.</param>
        /// <param name="activityName">The name of the activity the staffing schedule is tied to.</param>
        /// <param name="staffingScheduleName">The name of the staffing schedule the assignment will be tied to.</param>
        public void Ministry_StaffingAssignments_Create(int churchId, int individualId, int individualTypeId, string activityName, string staffingScheduleName) {
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.STAFFING_PREF (INDIVIDUAL_ID, INDIVIDUAL_TYPE_ID, ACTIVITY_ID, STAFFING_SCHEDULE_ID, CHURCH_ID, IS_ACTIVE) ");
            query.AppendFormat("SELECT {0}, {1}, ACTIVITY_ID, STAFFING_SCHEDULE_ID, {2}, 1 FROM ChmActivity.dbo.STAFFING_SCHEDULE WITH (NOLOCK) WHERE CHURCH_ID = {2} AND ACTIVITY_ID = {3} AND STAFFING_SCHEDULE_NAME = '{4}'", individualId, individualTypeId, churchId, activityId, staffingScheduleName);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a staffing assignment.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="activityName">The name of the activity the assignment is tied to.</param>
        public void Ministry_StaffingAssignments_Delete(int churchId, int individualId, string activityName, string staffingScheduleName) {
            int activityId = -1;
            try
            {
                // Store activity ID
                activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            }
            catch (IndexOutOfRangeException e)
            {
                log.Warn("Didn't find given activity when deletion, activity name: " + activityName);
                return;
            }

            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmActivity.dbo.STAFFING_PREF WHERE INDIVIDUAL_ID = {0} AND ACTIVITY_ID = {1} AND CHURCH_ID = {2} ", individualId, activityId, churchId);
            query.AppendFormat("AND STAFFING_SCHEDULE_ID IN (SELECT STAFFING_SCHEDULE_ID FROM ChmActivity.dbo.STAFFING_SCHEDULE WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = {1} AND STAFFING_SCHEDULE_NAME = '{2}')", churchId, activityId, staffingScheduleName);

            this.Execute(query.ToString());
        }
        #endregion Staffing Assignments

        #region Participant Assignments
        /// <summary>
        /// Creates a Participant Assignment
        /// </summary>
        /// <param name="churchId">The Church ID</param>
        /// <param name="individualId">The individual ID</param>
        /// <param name="activityName">The name of the activity</param>
        /// <param name="rosterName">The name of the roster</param>
        /// <param name="breakoutGroupName">The name of the breakout group</param>
        public void Ministry_ParticipantAssignments_Create(int churchId, int individualId, string activityName, string rosterName, string breakoutGroupName = null)
        {
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            int rosterId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterName);

            if (!string.IsNullOrEmpty(breakoutGroupName))
            {
                int breakoutGroupId = this.Ministry_Activities_BreakoutGroups_FetchID(15, breakoutGroupName, activityName);

                StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS ");
                query.AppendFormat("(INDIVIDUAL_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, CHURCH_ID, INDIVIDUAL_GROUP_ID) ");
                query.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4})", individualId, activityId, rosterId, churchId, breakoutGroupId);

                this.Execute(query.ToString());
            }
            else
            {
                StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.INDIVIDUAL_PREFS ");
                query.AppendFormat("(INDIVIDUAL_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, CHURCH_ID) ");
                query.AppendFormat("VALUES ({0}, {1}, {2}, {3})", individualId, activityId, rosterId, churchId);

                this.Execute(query.ToString());
            }
        }

        /// <summary>
        /// This method deletes a participant assignment.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individual id.</param>
        /// <param name="activityName">The name of the activity the assignment is tied to.</param>
        /// <param name="rosterName">The name of the roster the assignment is tied to.</param>
        public void Ministry_ParticipantAssignments_Delete(int churchId, int individualId, string activityName, string rosterName)
        {

            int activityId = -1;
            try
            {
                activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            }
            catch (IndexOutOfRangeException e)
            {
                log.Warn("Didn't find given activity when deletion, activity name: " + activityName);
                return;
            }

            int rosterId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rosterName);

            StringBuilder query = new StringBuilder("DELETE FROM ChmActivity.dbo.INDIVIDUAL_PREFS ");
            query.AppendFormat("WHERE CHURCH_ID = {0} and INDIVIDUAL_ID = {1} and ACTIVITY_ID = {2} and ACTIVITY_DETAIL_ID = {3}", churchId, individualId, activityId, rosterId);

            this.Execute(query.ToString());
        }
        #endregion Participant Assignments

        #region Group Finder Properties
        /// <summary>
        /// This method creates a group finder property.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="groupFinderPropertyName">The name of the property.</param>
        /// <param name="isFilterChoice">Flag designating is filter choice.</param>
        /// <param name="showResults">Flag designating show results.</param>
        public void Ministry_GroupFinderProperties_Create(int churchId, string activityName, string groupFinderPropertyName, bool isFilterChoice, bool showResults) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_PROPERTY (CHURCH_ID, ACTIVITY_ID, ACTIVITY_PROPERTY_NAME, IS_FILTER_CHOICE, SHOW_RESULTS)");
            query.AppendFormat("SELECT {0}, ACTIVITY_ID, '{1}', {2}, {3} FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{4}' AND IS_ACTIVE = 1", churchId, groupFinderPropertyName, Convert.ToInt16(isFilterChoice), Convert.ToInt16(showResults), activityName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a group finder property.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the property is tied to.</param>
        /// <param name="groupFinderPropertyName">The name of the property.</param>
        public void Ministry_GroupFinderProperties_Delete(int churchId, string activityName, string groupFinderPropertyName) {
            this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_PROPERTY WHERE CHURCH_ID = {0} AND ACTIVITY_ID = (SELECT ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1) AND ACTIVITY_PROPERTY_NAME = '{2}'", churchId, activityName, groupFinderPropertyName));
        }

        /// <summary>
        /// This method created a group finder property choice.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the gfp is tied to.</param>
        /// <param name="groupFinderPropertyName">The name of the group finder property the choice will be tied to.</param>
        /// <param name="choiceName">The name of the choice.</param>
        public void Ministry_GroupFinderProperties_Choices_Create(int churchId, string activityName, string groupFinderPropertyName, string choiceName) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_PROPERTY_LOV (CHURCH_ID, ACTIVITY_PROPERTY_ID, ACTIVITY_PROPERTY_LOV_NAME) ");
            query.AppendFormat("SELECT {0}, ACTIVITY_PROPERTY_ID, '{1}' FROM ChmActivity.dbo.ACTIVITY_PROPERTY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = (SELECT ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{2}' AND IS_ACTIVE = 1) AND ACTIVITY_PROPERTY_NAME = '{3}'", churchId, choiceName, activityName, groupFinderPropertyName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a group finder property choice.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The name of the activity the gfp is tied to.</param>
        /// <param name="groupFinderPropertyName">The name of the group finder property the choice is tied to.</param>
        /// <param name="choiceName">The name of the choice.</param>
        public void Ministry_GroupFinderProperties_Choices_Delete(int churchId, string activityName, string groupFinderPropertyName, string choiceName){
            this.Execute(string.Format("DELETE FROM ChmActivity.dbo.ACTIVITY_PROPERTY_LOV WHERE CHURCH_ID = {0} AND ACTIVITY_PROPERTY_ID = (SELECT ACTIVITY_PROPERTY_ID FROM ChmActivity.dbo.ACTIVITY_PROPERTY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_ID = (SELECT ACTIVITY_ID FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_NAME = '{1}' AND IS_ACTIVE = 1) AND ACTIVITY_PROPERTY_NAME = '{2}') AND ACTIVITY_PROPERTY_LOV_NAME = '{3}'", churchId, activityName, groupFinderPropertyName, choiceName));
        }
        #endregion Group Finder Properties

        #region Automated Attendance Rules
        /// <summary>
        /// This method deletes an automated attendance rule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="ministryName">The name of the ministry.</param>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="rlcName">The name of the room location class.</param>
        public void Ministry_AutomatedAttendanceRules_Delete(int churchId, string ministryName, string activityName, string rlcName) {
            int ministryId = this.Ministry_Ministries_FetchID(churchId, ministryName);
            int activityId = -1;
            try
            {
                // Store activity ID
                activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            }
            catch (IndexOutOfRangeException e)
            {
                log.Warn("Didn't find given activity when deletion, activity name: " + activityName);
                return;
            }
            //int rlcId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rlcName);

            //if (ministryId != int.MinValue && activityId != int.MinValue/* && rlcId != int.MinValue*/) {
                this.Execute("DELETE FROM ChmActivity.dbo.AUTOMATIC_ATTENDANCE_RULE WHERE CHURCH_ID = {0} AND MINISTRY_ID = {1} AND ACTIVITY_ID = {2}"/* AND ACTIVITY_DETAIL_ID = {3}"*/, churchId, ministryId, activityId/*, rlcId*/);
            //}
        }

        /// <summary>
        /// This method creates an automated attendance rule.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="ministryName">The ministry name.</param>
        /// <param name="activityName">The activity name.</param>
        /// <param name="rlcName">The rlc name.</param>
        /// <param name="householdHeadInd"></param>
        /// <param name="householdSpouseInd"></param>
        /// <param name="householdChildInd"></param>
        /// <param name="contributorChildInd"></param>
        /// <param name="childParentInd"></param>
        /// <param name="childSiblingInd"></param>
        public void Ministry_AutomatedAttendanceRules_Create(int churchId, string ministryName, string activityName, string rlcName, int householdHeadInd, int householdSpouseInd, int householdChildInd, int contributorSpouseInd, int contributorChildInd, int childParentInd, int childSiblingInd) {
            int ministryId = this.Ministry_Ministries_FetchID(churchId, ministryName);
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            int activityDetailId = this.Ministry_ActivityDetails_FetchID(churchId, activityId, rlcName);

            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.AUTOMATIC_ATTENDANCE_RULE (CHURCH_ID, MINISTRY_ID, ACTIVITY_ID, ACTIVITY_DETAIL_ID, HOUSEHOLD_HEAD_IND, HOUSEHOLD_SPOUSE_IND, HOUSEHOLD_CHILD_IND, CONTRIBUTOR_SPOUSE_IND, CONTRIBUTOR_CHILD_IND, CHILD_PARENT_IND, CHILD_SIBLING_IND) ");
            query.AppendFormat("VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})", churchId, ministryId, activityId, activityDetailId, householdHeadInd, householdSpouseInd, householdChildInd, contributorSpouseInd, contributorChildInd, childParentInd, childSiblingInd);

            this.Execute(query.ToString());
        }
        #endregion Automated Attendance Rules

        #region Room Location Class
        /// <summary>
        /// This method fetches the id for a room location class.
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="activityId"></param>
        /// <param name="activityDetailName"></param>
        /// <returns>The integer id for the room location class.</returns>
        public int Ministry_ActivityDetails_FetchID(int churchId, int activityId, string activityDetailName) {
            DataTable results = this.Execute("SELECT ACTIVITY_DETAIL_ID FROM ChmActivity.dbo.ACTIVITY_DETAIL WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_DETAIL_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, activityDetailName, activityId);
            
            int retVal = results.Rows.Count > 0 ? Convert.ToInt32(results.Rows[0]["ACTIVITY_DETAIL_ID"]) : int.MinValue;
            return retVal;
        }

        /// <summary>
        /// This method creates a room location class.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityDetailName">The room location class name.</param>
        public void Ministry_ActivityDetails_Create(int churchId, string activityName, string activityDetailName) {
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);

            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.ACTIVITY_DETAIL (CHURCH_ID, ACTIVITY_DETAIL_NAME, ACTIVITY_ID, CHECKIN_AUTO_OPEN, CHECKIN_ENABLED, HAS_NAMETAG, HAS_RECEIPT, IS_ACTIVE, IS_CLOSED) ");
            query.AppendFormat("VALUES({0}, '{1}', {2}, 0, 0, 0, 0, 1, 0)", churchId, activityDetailName, activityId);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a room location class.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityDetailName">The activity detail name.</param>
        public void Ministry_ActivityDetails_Delete(int churchId, string activityName, string activityDetailName) {
            int activityId = this.Ministry_Activities_FetchID(churchId, activityName);
            TestLog.WriteLine("Activity: " + activityName);
            TestLog.WriteLine("ActivityID: " + activityId);
            this.Execute("DELETE FROM ChmActivity.dbo.ACTIVITY_DETAIL WHERE CHURCH_ID = {0} AND ACTIVITY_DETAIL_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, activityDetailName, activityId);
        }
        #endregion Room Location Class

        #region Roster Folders/Rosters

        /// <summary>
        /// This method fetches the ID of a Roster Folder
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="activityFolderName"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int Ministry_RosterFolders_FetchID(int churchId, string activityFolderName, int activityId)
        {
            DataTable results = this.Execute("SELECT ACTIVITY_GROUP_ID FROM ChmActivity.dbo.ACTIVITY_GROUP WITH (NOLOCK) WHERE CHURCH_ID = {0} AND ACTIVITY_GROUP_NAME = '{1}' AND ACTIVITY_ID = {2}", churchId, activityFolderName, activityId);

            int retVal = results.Rows.Count > 0 ? Convert.ToInt32(results.Rows[0]["ACTIVITY_GROUP_ID"]) : int.MinValue;
            return retVal;
        }
        #endregion Roster Folders/Rosters

        #region Super Check-ins
        /// <summary>
        /// This method creates a super check-in.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="superCheckinName">The name of the super check-in.</param>
        /// <param name="activityScheduleNames">The names of the activity schedules that will map to this super check-in.</param>
        public void Ministry_SuperCheckins_Create(int churchId, string superCheckinName, params string[] activityScheduleNames) {
            StringBuilder query = new StringBuilder("DECLARE @checkinCode INT, @globalActivityID INT ");
            query.Append("SET @checkinCode = (SELECT randValue FROM (SELECT CONVERT(INT, RAND() * 9000) + 1000  AS randValue) foo ");
            query.AppendFormat("WHERE NOT EXISTS (SELECT CHECKIN_CODE FROM ChmActivity.dbo.GLOBAL_ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND foo.randValue = CHECKIN_CODE) ", churchId);
            query.AppendFormat("AND NOT EXISTS (SELECT CHECKIN_CODE FROM ChmActivity.dbo.ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = 15 AND foo.randValue = CHECKIN_CODE)) ", churchId);

            query.Append("INSERT INTO ChmActivity.dbo.GLOBAL_ACTIVITY (CHURCH_ID, GLOBAL_ACTIVITY_NAME, CHECKIN_CODE) ");
            query.AppendFormat("VALUES({0}, '{1}', @checkinCode) ", churchId, superCheckinName);
            query.Append("SET @globalActivityID = (SELECT SCOPE_IDENTITY()) ");

            foreach (var activityScheduleName in activityScheduleNames) {
                int activityScheduleId = this.Ministry_ActivitySchedules_FetchID(churchId, activityScheduleName);

                query.Append("INSERT INTO ChmActivity.dbo.GLOBAL_ACTIVITY_TIME (GLOBAL_ACTIVITY_ID, ACTIVITY_TIME_ID, CHURCH_ID) ");
                query.AppendFormat("VALUES(@globalActivityID, {0}, {1}) ", activityScheduleId, churchId);
            }
            this.Execute(query.ToString());
        }


        /// <summary>
        /// This method deletes a super check-in.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="superCheckinName">The name of the super check-in.</param>
        public void Ministry_SuperCheckins_Delete(int churchId, string superCheckinName) {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM ChmActivity.dbo.GLOBAL_ACTIVITY_TIME WHERE GLOBAL_ACTIVITY_ID = (SELECT GLOBAL_ACTIVITY_ID FROM ChmActivity.dbo.GLOBAL_ACTIVITY WITH (NOLOCK) WHERE CHURCH_ID = {0} AND GLOBAL_ACTIVITY_NAME = '{1}') AND CHURCH_ID = {0} ", churchId, superCheckinName);
            query.AppendFormat("DELETE FROM ChmActivity.dbo.GLOBAL_ACTIVITY WHERE CHURCH_ID = {0} AND GLOBAL_ACTIVITY_NAME = '{1}'", churchId, superCheckinName);

            this.Execute(query.ToString());
        }
        #endregion Super Check-ins

        #region Themes
        /// <summary>
        /// This method creates a check-in theme.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="themeName">The name of the theme.</param>
        public void Ministry_Themes_Create(int churchId, string themeName) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.THEME (THEME_NAME, CHURCH_ID, COLOR_SET_ID) ");
            query.AppendFormat("VALUES('{0}', {1}, 1)", themeName, churchId);

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a check-in theme.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="themeName">The name of the theme.</param>
        public void Ministry_Themes_Delete(int churchId, string themeName) {
            this.Execute("DELETE FROM ChmActivity.dbo.THEME WHERE CHURCH_ID = {0} AND THEME_NAME = '{1}'", churchId, themeName);
        }
        #endregion Themes

        #region Assignment Counts

        /// <summary>
        /// Gets Ministry Assignments Participant and Staff Count by a church id and single activity id
        /// </summary>
        /// <param name="churchID">Church ID</param>
        /// <param name="activityID">Activity ID</param>
        /// <returns>String array where participant = [0], staff = [1]</returns>
        public string[] Ministry_Assignments_GetAssignmentCountForActivities(int churchID, int activityID)
        {
            string proc = "[ChmActivity].[dbo].[cp_GetAssignmentCountForActivities]";
            SqlDataReader dr;
            string[] participantStaffCount = new string[] {"0", "0"};

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));
                    //cmd.Parameters.Add(new SqlParameter("@ActivitiesCSV", string.Format("N'{0}'", activityID)));
                    cmd.Parameters.Add(new SqlParameter("@ActivitiesCSV", Convert.ToString(activityID)));
                    
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {                        
                        if (dr["ParticipantCount"] != null) participantStaffCount[0] = dr["ParticipantCount"].ToString();
                        if (dr["StaffCount"] != null) participantStaffCount[1] = dr["StaffCount"].ToString();
                    }

                }

                dbConnection.Close();
            }

            return participantStaffCount;

        }


        /// <summary>
        /// Gets Ministry Assignments Attendance Count
        /// </summary>
        /// <param name="churchID">Church ID</param>
        /// <param name="activityID">Activity ID</param>
        /// <returns>String attendance count</returns>
        public string Ministry_Assignments_GetAttendanceForLastActivityDay(int churchID, int activityID)
        {
            string proc = "[ChmActivity].[dbo].[cp_GetAttendanceForLastActivityDay]";
            SqlDataReader dr;
            string attendanceCount = string.Empty;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));
                    //cmd.Parameters.Add(new SqlParameter("@ActivitiesCSV", string.Format("N'{0}'", activityID)));
                    cmd.Parameters.Add(new SqlParameter("@ActivitiesCSV", Convert.ToString(activityID)));

                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (dr["Attendance"] != null) attendanceCount = dr["Attendance"].ToString();
                    }

                }

                dbConnection.Close();
            }

            return attendanceCount;

        }

        /// <summary>
        /// Gets the count for Staff Assignments for a particular activity schedule
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="activityScheduleId">The activity time ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetStaffAssignmentForScheduleCount(int activityId, int activityScheduleId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.STAFFING_PREF WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and activity_time_id = {1}", activityId, activityScheduleId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Staff count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["STAFFING_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Staff assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        /// <summary>
        /// This method update a form's setting.
        /// </summary>
        /// <param name="Form_Type">update form_type, 1 is General Form, 2 is Registration Form, 3 is Aggregate Form.</param>
        /// <param name="Form_StartDate">form date range, start date</param>
        /// <param name="Form_EndDate">form date range, end date</param>
        /// <param name="Form_MaxCapacity">max submission</param>
        /// <param name="Form_ID">Form_ID</param>
        /// ivan zhang
        public void Update_ExistingForm(string Form_StartDate, string Form_EndDate, int Form_MaxCapacity, int Form_ID)
        {
            StringBuilder updateString = new StringBuilder("update [ChmActivity].[dbo].[FORM] set ");
            List<string> updateStringPart = new List<string>();
            //updateStringPart.Add(string.Format("[FORM_TYPE_ID]={0}", Form_Type));
            if (string.IsNullOrWhiteSpace(Form_StartDate))
            {
                updateStringPart.Add("[START_DATE] = NULL");
            }
            else
            {
                updateStringPart.Add(string.Format("[START_DATE] = '{0}'", Form_StartDate));
            }
            if (string.IsNullOrWhiteSpace(Form_EndDate))
            {
                updateStringPart.Add("[END_DATE] = NULL");
            }
            else
            {
                updateStringPart.Add(string.Format("[END_DATE] = '{0}'", Form_EndDate));
            }
            updateStringPart.Add(string.Format("[MAX_CAPACITY] = {0}", Form_MaxCapacity));

            var updatestringReversion = string.Join(",", updateStringPart);
            updateString.Append(updatestringReversion);
            updateString.AppendFormat(" where [FORM_ID]={0}", Form_ID);

            //update [ChmActivity].[dbo].[FORM] set [FORM_TYPE_ID]=2,[START_DATE]='2015-06-21 00:00:00.000',[END_DATE]='2015-07-11 00:00:00.000', [MAX_CAPACITY]=13  where form_id=94070
            this.Execute(updateString.ToString());

        }


        /// <summary>
        /// Gets the count for Participant Assignments for a particular activity schedule
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="activityScheduleId">The activity time ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetParticipantAssignmentForScheduleCount(int activityId, int activityScheduleId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.INDIVIDUAL_PREFS WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and activity_time_id = {1}", activityId, activityScheduleId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Participant count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["INDIVIDUAL_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Participant assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        /// <summary>
        /// Gets the count for Participant Assignments for a particular activity schedule
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="rosterId">The activity time ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetParticipantAssignmentForRosterLevelCount(int activityId, int rosterId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.INDIVIDUAL_PREFS WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and activity_detail_id = {1}", activityId, rosterId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Participant count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["INDIVIDUAL_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Participant assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        /// <summary>
        /// Gets the count for Staff Assignments for a particular activity roster group level
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="rosterGroupId">The roster group ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetStaffAssignmentForRosterGroupLevelCount(int activityId, int rosterGroupId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.STAFFING_PREF WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and activity_group_id = {1}", activityId, rosterGroupId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Staff count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["STAFFING_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Staff assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        /// <summary>
        /// Gets the count for Staff Assignments for a particular activity roster level
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="rosterId">The roster group ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetStaffAssignmentForRosterLevelCount(int activityId, int rosterId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.STAFFING_PREF WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and activity_detail_id = {1}", activityId, rosterId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Staff count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["STAFFING_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Staff assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        /// <summary>
        /// Gets the count for Staff Assignments for a particular activity schedule
        /// </summary>
        /// <param name="activityId">The activity ID</param>
        /// <param name="staffScheduleId">The activity time ID</param>
        /// <returns></returns>
        public int Ministry_Assignments_GetStaffAssignmentForStaffScheduleCount(int activityId, int staffScheduleId)
        {
            string assignmentCount = string.Empty;

            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.STAFFING_PREF WITH (NOLOCK) ");
            query.AppendFormat("where activity_id = {0} and staffing_schedule_id = {1}", activityId, staffScheduleId);
            DataTable dt = this.Execute(query.ToString());

            TestLog.WriteLine("Staff count: {0}", dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                assignmentCount = dt.Rows[0]["STAFFING_PREF_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Staff assignment count was not greater than 0"));
            }

            return dt.Rows.Count;

        }

        #endregion Assignment Counts

        #region Jobs

        public int Ministry_Jobs_FetchID(int churchId, string jobTitle)
        {
            string jobID = string.Empty;

            StringBuilder query = new StringBuilder("SELECT TOP 1 JOB_ID FROM ChmActivity.dbo.JOB WITH (NOLOCK) ");
            query.AppendFormat("where church_id = {0} and job_title = '{1}'", churchId, jobTitle);
            DataTable dt = this.Execute(query.ToString());

            if (dt.Rows.Count > 0)
            {
                jobID = dt.Rows[0]["JOB_ID"].ToString();
            }
            else
            {
                throw new Exception(string.Format("Ministry Job ID not found for church id: [{0}] and job title: [{1}]", churchId, jobTitle));
            }

         return Convert.ToInt32(jobID);

        }
        #endregion Jobs

        #endregion Ministry

        #region WebLink
        /// <summary>
        /// This method fetches the event registration confirmation code for an event registration.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="amount">The amount of the payment.</param>
        /// <param name="paymentMethod">The method of payment.</param>
        /// <returns>The confirmation code for the event registration.</returns>
        public string WebLink_FetchEventRegistrationConfirmationCode(int churchId, double amount, string paymentMethod) {
            StringBuilder query = new StringBuilder("SELECT TOP 1 CONFIRMATION_CODE FROM ChmActivity.dbo.FORM_INDIVIDUAL_SET formIndSet ");
            query.Append("INNER JOIN ChmActivity.dbo.PAY_TRANSACTION payTrans ");
            query.Append("ON formIndSet.PAY_ORDER_SET_ID = payTrans.PAY_ORDER_SET_ID ");
            query.AppendFormat("WHERE PayTrans.CHURCH_ID = {0} AND payTrans.AMOUNT = {1} AND payTrans.PAYMENT_DESCRIPTION LIKE '{2}%' ORDER BY payTrans.CREATED_DATE DESC", churchId, amount, paymentMethod);
            return this.Execute(query.ToString()).Rows[0]["CONFIRMATION_CODE"].ToString();
        }

        /// <summary>
        /// This method fetches the event registration information for an event registration
        /// </summary>
        /// <param name="churchId">The church id</param>
        /// <param name="amount">The amount</param>
        /// <param name="paymentMethod">Payment Method</param>
        /// <param name="confirmationCode">Registration Code</param>
        /// <returns>Reference Number</returns>
        public string WebLink_FetchEventRegistrationReference(int churchId, double amount, string paymentMethod, string confirmationCode)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM ChmActivity.dbo.FORM_INDIVIDUAL_SET formIndSet ");
            query.Append("INNER JOIN ChmActivity.dbo.PAY_TRANSACTION payTrans ");
            query.Append("ON formIndSet.PAY_ORDER_SET_ID = payTrans.PAY_ORDER_SET_ID ");
            //query.AppendFormat("WHERE PayTrans.CHURCH_ID = {0} AND payTrans.AMOUNT = {1} AND payTrans.PAYMENT_DESCRIPTION LIKE '{2}%' AND formIndSet.Confirmation_Code = '{3}' ", churchId, amount, paymentMethod, confirmationCode);
            query.AppendFormat("WHERE PayTrans.CHURCH_ID = {0} AND payTrans.AMOUNT = {1} AND formIndSet.Confirmation_Code = '{2}' ", churchId, amount, confirmationCode);
            query.Append("ORDER BY payTrans.CREATED_DATE DESC");
            return this.Execute(query.ToString()).Rows[0]["PP_REQUEST_CODE"].ToString();            
        }

        /// <summary>
        /// This method creates a form name.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the form.</param>
        /// <param name="isActive">Flag designating active status of the form.</param>
        public void WebLink_FormNames_Create(int formTypeId, int churchId, string formName, bool isActive) {
            StringBuilder query = new StringBuilder("INSERT INTO ChmActivity.dbo.FORM (FORM_TYPE_ID, CHURCH_ID, FORM_NAME, IS_ACTIVE) ");
            query.AppendFormat("VALUES({0}, {1}, '{2}', {3})", formTypeId, churchId, formName, Convert.ToInt16(isActive));

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a form name.
        /// </summary>
        /// <param name="formTypeId">The type of the form.</param>
        /// <param name="churchId">The church id.</param>
        /// <param name="formName">The name of the form.</param>
        public void WebLink_FormNames_Delete(int formTypeId, int churchId, string formName) {
            StringBuilder query = new StringBuilder();
            if (formTypeId == 1) {
                query.AppendFormat("DELETE FROM ChmActivity.dbo.PAY_ITEM_FORM WHERE FORM_ID IN (SELECT FORM_ID FROM ChmActivity.dbo.FORM WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}')", churchId, formName);
                query.AppendFormat("DELETE FROM ChmActivity.dbo.FORM_PAGE WHERE FORM_ID IN (SELECT FORM_ID FROM ChmActivity.dbo.FORM WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}')", churchId, formName);
                query.AppendFormat("DELETE FROM ChmActivity.dbo.FORM_INDIVIDUAL WHERE FORM_ID IN (SELECT FORM_ID FROM ChmActivity.dbo.FORM WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}')", churchId, formName);
            }
            else if (formTypeId == 3) {
                query.AppendFormat("DELETE FROM ChmActivity.dbo.FORM_RELATIONSHIP WHERE FORM1_ID IN (SELECT FORM_ID FROM ChmActivity.dbo.FORM WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}')", churchId, formName);
            }
            query.AppendFormat("DELETE FROM ChmActivity.dbo.FORM WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}'", churchId, formName);
            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method deletes a volunteer application
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="volunteerApplicationFormName">The name of the volunteer application.</param>
        public void WebLink_VolunteerApplicationForm_Delete(int churchId, string volunteerApplicationFormName)
        {
            StringBuilder query = new StringBuilder();
            query.Append("DECLARE @volunteerApplicationId int, @volunteerOpportunityInstanceId int, @volunteerOpportunityId int, @churchId int,@applicationName varchar(30) ");
            query.AppendFormat("SET @churchId = {0} ", churchId.ToString());
            query.AppendFormat("SET @applicationName = '{0}' ", volunteerApplicationFormName);
            query.Append("select @volunteerApplicationId = VOLUNTEER_APPLICATION_ID FROM ChmActivity..VOLUNTEER_APPLICATION WHERE DESCRIPTION=@applicationName AND CHURCH_ID=@churchId ");
            query.Append("select VOLUNTEER_OPPORTUNITY_INSTANCE_ID into #tempId from ChmActivity..VOLUNTEER_OPPORTUNITY_INSTANCE WHERE VOLUNTEER_APPLICATION_ID = @volunteerApplicationId ");
            query.Append("select @volunteerOpportunityId=VOLUNTEER_OPPORTUNITY_ID FROM ChmActivity..VOLUNTEER_OPPORTUNITY WHERE CHURCH_ID=@churchId AND VOLUNTEER_APPLICATION_ID = @volunteerApplicationId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY_INSTANCE_ITEM where VOLUNTEER_OPPORTUNITY_INSTANCE_ID in (select * from #tempId) ");
            query.Append("drop table #tempId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY_INSTANCE where VOLUNTEER_APPLICATION_ID = @volunteerApplicationId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY_USER where VOLUNTEER_OPPORTUNITY_ID = @volunteerOpportunityId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY_REQUIREMENT where VOLUNTEER_OPPORTUNITY_ID = @volunteerOpportunityId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY where VOLUNTEER_APPLICATION_ID = @volunteerApplicationId AND CHURCH_ID=@churchId ");
            query.Append("delete from ChmActivity..VOLUNTEER_OPPORTUNITY WHERE CHURCH_ID=@churchId AND VOLUNTEER_APPLICATION_ID = @volunteerApplicationId ");
            query.Append("delete from ChmActivity..VOLUNTEER_APPLICATION WHERE DESCRIPTION=@applicationName AND CHURCH_ID=@churchId ");

            this.Execute(query.ToString());
        }

        /// <summary>
        /// This method gets the form Id for a weblink form.
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formName"></param>
        /// <returns></returns>
        public int Weblink_InfellowshipForm_GetFormId(int churchId, string formName)
        {
            DataTable results = this.Execute("SELECT FORM_ID FROM ChmActivity.dbo.FORM WHERE CHURCH_ID = {0} AND FORM_NAME = '{1}'", churchId, formName);

            int retVal = results.Rows.Count > 0 ? Convert.ToInt32(results.Rows[0]["FORM_ID"]) : int.MinValue;
            return retVal;
        }

        
        #endregion WebLink

        
        #region EventRegistration

        /// <summary>
        /// This method deletes a form based on Form Name
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formName"></param>
        public void Weblink_Form_Delete_Proc(int churchId, string formName)
        {

            int formId = this.Weblink_InfellowshipForm_GetFormId(15, formName);
            this.Weblink_Form_Delete_Proc(churchId, formId);

        }

        /// <summary>
        /// This method deletes payment Requirement items.
        /// </summary>
        /// <param name="chruchId"></param>
        /// <param name="fromName"></param>
        public void Weblink_Form_Delete_Payment_Requirement(int chruchId, string fromName) 
        {
            var formId = this.Weblink_InfellowshipForm_GetFormId(15, fromName);
            var sql =
                string.Format(
                    @"delete from ChmActivity.dbo.PAY_PAYMENT_REQUIREMENT where PAY_ITEM_ID in(select PAY_ITEM_ID from ChmActivity.dbo.PAY_ITEM_FORM where FORM_ID = {0} and CHURCH_ID = {1})",
                    formId, chruchId);
            Execute(sql);
        }
        /// <summary>
        /// This method delete the form from database using form id
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formName"></param>
        /// <returns></returns>
        public void Weblink_Form_Delete_Proc(int churchID, int formID)
        {

            //add by Grace Zhang: when formID is invalid,dont't execute StoredProcedure
            if ((formID == int.MinValue) || (formID ==0))
             {
                 return;
             }
            string proc = "[ChmActivity].[dbo].[DestroyForm]";
            SqlDataReader dr;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FormID", formID));
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchID));                
                    dr = cmd.ExecuteReader();


                }

                dbConnection.Close();
            }


        }

        /// <summary>
        /// This method will fetch FORM_PAGE_ID from FORM_PAGE table
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public int Weblink_InfellowshipForm_GetFormPageId(int churchId, int formID)
        {

             
             DataTable results_PageID = this.Execute("SELECT  TOP 1 FORM_PAGE_ID FROM ChmActivity.dbo.FORM_PAGE WHERE CHURCH_ID = {0} AND FORM_ID = {1}", churchId, formID);
             int formPage_ID = results_PageID.Rows.Count > 0 ? Convert.ToInt32(results_PageID.Rows[0]["FORM_PAGE_ID"]) : int.MinValue;
            // int formPage_ID = Convert.ToInt32(results_PageID.Rows[0]["FORM_PAGE_ID"]);
             TestLog.WriteLine("PAge ID {0}", formPage_ID);

             return (formPage_ID); 

          }

        /// <summary>
        /// This method will fetch FORM_HEADER_ID from FORM_HEADER table
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formID"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public int Weblink_InfellowshipForm_GetFormHeaderId(int churchId, int formID, string headerName)
        {
            
                       
             int formPage_ID = this.Weblink_InfellowshipForm_GetFormPageId(15, formID);
             DataTable results_HeaderID = this.Execute("SELECT TOP 1 FORM_HEADER_ID FROM ChmActivity.dbo.FORM_HEADER WHERE CHURCH_ID = {0} AND FORM_PAGE_ID = {1} AND FORM_HEADER_NAME = '{2}'", churchId, formPage_ID, headerName);
             int formHeader_ID = Convert.ToInt32(results_HeaderID.Rows[0]["FORM_HEADER_ID"]);
           

            return (formHeader_ID); 

        }


        public string Weblink_InfellowshipForm_GetConfirmationCode(int churchId, int Form_Individual_Set_Id)
        {
            
            StringBuilder query = new StringBuilder("SELECT [CONFIRMATION_CODE] FROM [ChmActivity].[dbo].[FORM_INDIVIDUAL_SET] WITH (NOLOCK) ");
            query.AppendFormat("WHERE CHURCH_ID = {0} AND FORM_INDIVIDUAL_SET_ID = {1} order by Created_date desc", churchId, Form_Individual_Set_Id);

            return (string)this.Execute(query.ToString()).Rows[0]["CONFIRMATION_CODE"];

        }

        /// <summary>
        /// This method will fetch FORM_ITEM_ID from FORM_ITEM table_
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="formID"></param>
        /// <param name="headerName"></param>
        /// <param name="question"></param>
        /// <returns></returns>

        public int Weblink_InfellowshipForm_GetFormItemId(int churchId, int formID, string headerName, string question)
        {
            int formHeader_ID = this.Weblink_InfellowshipForm_GetFormHeaderId(churchId, formID, headerName);

            TestLog.WriteLine("Header id {0}", formHeader_ID);

            DataTable results_ItemID = this.Execute("SELECT TOP 1 FORM_ITEM_ID FROM ChmActivity.dbo.FORM_ITEM WHERE CHURCH_ID = {0} AND FORM_HEADER_ID = {1} AND TEXT = '{2}'", churchId, formHeader_ID, question);

           int formItem_ID = results_ItemID.Rows.Count > 0 ? Convert.ToInt32(results_ItemID.Rows[0]["FORM_ITEM_ID"]) : int.MinValue;
           TestLog.WriteLine("Item Id {0}", formItem_ID);
           return (formItem_ID); 

         }

        public void Weblink_ExpireFormSubmissions_Proc(int minutesInactive)
        {
            
            string proc = "ChmActivity.dbo.ExpireFormSubmissions";
            SqlDataReader dr;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@MinutesOfInactivity", minutesInactive));
                    dr = cmd.ExecuteReader();

                }

                dbConnection.Close();
            }
        }


        #endregion

        #region Report Library
        /// <summary>
        /// This method deletes a report from the report queue.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userId">The user id that generated the report.</param>
        /// <param name="outputName">The name displayed for the report in the queue.</param>
        public void ReportLibrary_Queue_DeleteItem(int churchId, int userId, string outputName) {
            StringBuilder query = new StringBuilder("DECLARE @queueItemID INT ");
            query.AppendFormat("SET @queueItemID = (SELECT TOP 1 QUEUE_ITEM_ID FROM ChmChurch.dbo.REPORT_QUEUE_ITEM WITH (NOLOCK) WHERE CHURCH_ID = {0} AND USER_ID = {1} AND (RUNTIME_REPORT_NAME = '{2}' OR OutputName = '{2}') ORDER BY CREATED_DATE DESC) ", churchId, userId, outputName);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.REPORT_QUEUE_ITEM WHERE QUEUE_ITEM_ID = @queueItemID AND CHURCH_ID = {0} ", churchId);
            query.Append("DELETE FROM ChmChurch.dbo.QUEUE_ITEM WHERE QUEUE_ITEM_ID = @queueItemID");
            this.Execute(query.ToString());
        }
      /*  public void ReportLibrary_MyReports_DeleteReport(int churchId, int userId, string reportName)
        {
            StringBuilder query = new StringBuilder("DECLARE @userReportId INT ");
            query.AppendFormat("SET @userReportId = (SELECT TOP 1 reportId  FROM ChmChurch.dbo.USERREPORT WITH (NOLOCK) WHERE CHURCHID = {0} AND USERID = {1} AND  reportName = '{2}' ORDER BY CREATEDDATE DESC) ", churchId, userId, reportName);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.USERREPORT WHERE reportId = @userReportId AND CHURCHID = {0} ", churchId);
            this.Execute(query.ToString());
        } */


        /// <summary>
        /// Deletes a label style.
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="labelStyleName">The name of the label style.</param>
        public void Reports_DeleteLabelStyle(int churchId, string labelStyleName) {
            //int mle_format_id = (int)base.ExecuteDBQuery(string.Format("SELECT TOP 1 MLE_FORMAT_ID FROM ChmChurch.dbo.MLE_FORMAT WITH (NOLOCK) WHERE CHURCH_ID = {0} AND FORMAT_NAME = '{1}'", churchId, labelStyleName)).Rows[0]["MLE_FORMAT_ID"];

            StringBuilder query = new StringBuilder("DELETE lFormat FROM ChmChurch.dbo.MLE_LABEL_FORMAT lFormat ");
            query.Append("INNER JOIN ChmChurch.dbo.MLE_FORMAT format ");
            query.Append("ON lFormat.MLE_FORMAT_ID = format.MLE_FORMAT_ID ");
            query.AppendFormat("WHERE format.CHURCH_ID = {0} AND format.FORMAT_NAME = '{1}' ", churchId, labelStyleName);
            //query.AppendFormat("DELETE FROM ChmChurch.dbo.MLE_LABEL_FORMAT WHERE MLE_FORMAT_ID = {0} ", mle_format_id);
            query.AppendFormat("DELETE FROM ChmChurch.dbo.MLE_FORMAT WHERE CHURCH_ID = {0} AND FORMAT_NAME = '{1}'", churchId, labelStyleName);
            this.Execute(query.ToString());
        }
        #endregion Report Library


        #region SSO

        public string Get_Expired_Reset_Password_Token(int churchID, string firstName, string lastName)
        {

            string token = string.Empty;

            int individualID = this.People_Individuals_FetchID(churchID, string.Format("{0} {1}", firstName, lastName));
   
            StringBuilder query = new StringBuilder("SELECT Top 1 Token FROM ChmAPI.dbo.ApiAccessToken ");
                          query.Append("WITH (NOLOCK) ");
                          query.AppendFormat("WHERE churchid = {0} ", churchID);
                          query.AppendFormat("AND IndividualID = {1} ", individualID);
                          query.Append("ORDER BY ApiAccessTokenID desc");

            token = Convert.ToString(this.Execute(query.ToString()).Rows[0]["Token"]);

            log.Debug("Reset Password Token: " + token);

            return token;

        }

        #endregion SSO

        #region SOC Procs

        /*        EXEC Groups.SocLookup_GroupID @ChurchID = 0, @GroupID = 0 11:36 AM 
(change ChurchID and GroupID as appropriate) 11:36 AM 
EXEC Groups.SocLookup_GroupTypeID @ChurchID = 0, @GroupTypeID = 0 -- After creating a group type or changing a group type 11:37 AM 
EXEC Groups.SocLookup_CustomFieldID @ChurchID = 0, @CustomFieldID = 0 -- for Custom Fields 11:37 AM 
and ... EXEC Groups.SocLookup_GroupSocID @ChurchID = 0, @GroupSocID = 0 --For Span of Care
        */

        public void Groups_SOC_Lookup_Proc(int churchId, string lookupType, int id)
        {


            string proc = string.Empty;
            string lookupParam = string.Empty;
            DataTable results;
            SqlDataReader dr;

            switch (lookupType)
            {

                case "GroupID":
                    proc = "ChmPeople.Groups.SocLookup_GroupID";
                    lookupParam = "@GroupID";
                    break;

                case "GroupTypeID":
                    proc = "ChmPeople.Groups.SocLookup_GroupTypeID";
                    lookupParam = "@GroupTypeID";
                    break;

                case "CustomFieldID":
                    proc = "ChmPeople.Groups.SocLookup_CustomFieldID";
                    lookupParam = "@CustomFieldID";
                    break;

                case "GroupSocID":
                    proc = "ChmPeople.Groups.SocLookup_GroupSocID";
                    lookupParam = "@GroupSocID";
                    break;


                default:
                    throw new Exception(string.Format("{0} is a supported Groups SOC Lookup Proc type", lookupType));
                    
            }

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", churchId));                    
                    cmd.Parameters.Add(new SqlParameter(lookupParam, id));
                    dr = cmd.ExecuteReader();
                 
                }

                dbConnection.Close();
            }

        }


        #endregion SOC Procs


        #region Dashboard
        /// <summary>
        /// Get all funds of which has giving in last 365 days
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public ArrayList Dashboard_Giving_GetDefaultTurnedOnFundsName(int churchId, int timeZone)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT DISTINCT rt.Fund_ID INTO #Fund_IDs FROM chmcontribution..contribution_receipt rt ");
            query.Append("join ChmContribution..FUND fd on rt.Fund_ID=fd.Fund_ID and fd.Fund_type=1 ");
            query.AppendFormat("WHERE rt.CHURCH_ID={0} ", churchId);
            query.AppendFormat("and Received_date >= dateadd(day, -365, convert(datetime,convert(varchar,dateadd(hour, {0}, getutcdate()),111))) ", timeZone);
            query.AppendFormat("and Received_date < convert(datetime,convert(varchar,dateadd(hour, {0}, getutcdate()),111)) ", timeZone);
            query.Append("GROUP BY rt.Fund_ID ");
            query.Append("having count(rt.amount)>0 ");

            query.Append("SELECT DISTINCT Fund_id, fund_name, IS_ACTIVE FROM ChmContribution..FUND ");
            query.AppendFormat("WHERE CHURCH_ID={0} AND FUND_TYPE = 1 AND Fund_ID in(select Fund_ID from #Fund_IDs) ", churchId);
            query.Append("DROP TABLE #Fund_IDs ");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                //string[] funds = new string[dt.Rows.Count];
                ArrayList funds = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!bool.Parse(dt.Rows[i]["IS_ACTIVE"].ToString()))
                    {
                        //funds[i] = dt.Rows[i]["fund_name"].ToString() + "(Inactive)";
                        funds.Add(dt.Rows[i]["fund_name"].ToString() + "(Inactive)");
                    }
                    else
                    {
                        //funds[i] = dt.Rows[i]["fund_name"].ToString();
                        funds.Add(dt.Rows[i]["fund_name"].ToString());
                    }
                }

                return funds;
            }

            
        }

        /// <summary>
        /// Get all attribute groups which has at least one active attribute with "start date" property and 
        /// named in ('salvation','baptism','dedication','re-dedication','rededication','baby dedication') 
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public ArrayList Dashboard_AttributeGroup_GetGroupsCanBeTurnedOnByDefault(int churchId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("select distinct g.attribute_group_id, g.attribute_group_name ");
            query.Append("from ChmPeople..ATTRIBUTE_GROUP  g ");
            query.Append("join ChmPeople..ATTRIBUTE a on g.ATTRIBUTE_GROUP_ID = a.ATTRIBUTE_GROUP_ID ");
            query.AppendFormat("where g.church_id={0} and g.ENABLED=1 and a.ENABLED=1 and a.HAS_START_DATE =1 ", churchId);
            query.Append("and a.Attribute_Name in ('salvation','baptism','dedication','re-dedication','rededication','baby dedication') ");
            query.Append("order by g.attribute_group_name");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                ArrayList attributeGroups = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    attributeGroups.Add(dt.Rows[i]["attribute_group_name"].ToString());
                }

                return attributeGroups;
            }
        }

        /// <summary>
        /// Get all attribute groups which can be turned on by default
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public ArrayList Dashboard_AttributeGroup_GetAllAttributeGroups(int churchId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("select distinct g.attribute_group_id, g.attribute_group_name ");
            query.Append("from ChmPeople..ATTRIBUTE_GROUP  g ");
            query.Append("join ChmPeople..ATTRIBUTE a on g.ATTRIBUTE_GROUP_ID = a.ATTRIBUTE_GROUP_ID ");
            query.AppendFormat("where g.church_id={0} and g.ENABLED=1 and a.ENABLED=1 and a.HAS_START_DATE =1 ", churchId);
            query.Append("order by g.attribute_group_name");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                ArrayList attributeGroups = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    attributeGroups.Add(dt.Rows[i]["attribute_group_name"].ToString());
                }

                return attributeGroups;
            }
        }

        /// <summary>
        /// Get names of all funds which type is 'contribution'
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public ArrayList Dashboard_Giving_GetAllFundsName(int churchId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT DISTINCT Fund_id, fund_name, IS_ACTIVE FROM ChmContribution..FUND ");
            query.AppendFormat("WHERE CHURCH_ID={0} AND FUND_TYPE = 1 order by fund_name", churchId);

            using (DataTable dt = this.Execute(query.ToString()))
            {
                //string[] funds = new string[dt.Rows.Count];
                ArrayList funds = new ArrayList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!bool.Parse(dt.Rows[i]["IS_ACTIVE"].ToString()))
                    {
                        //funds[i] = dt.Rows[i]["fund_name"].ToString() + "(Inactive)";
                        funds.Add(dt.Rows[i]["fund_name"].ToString() + "(Inactive)");
                    }
                    else
                    {
                        //funds[i] = dt.Rows[i]["fund_name"].ToString();
                        funds.Add(dt.Rows[i]["fund_name"].ToString());
                    }
                }

                return funds;
            }


        }

        /// <summary>
        /// Get the sum of contributions in a given time period
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public double Dashboard_Giving_GetPeriodSum(int churchId, int userId, DateTime startDate, DateTime endDate)
        {
            StringBuilder query = new StringBuilder();

            string start = string.Format("{0} 00:00:00.000", startDate.ToString("yyyy-MM-dd"));
            string end = string.Format("{0} 00:00:00.000", endDate.AddDays(1).ToString("yyyy-MM-dd"));
            //string end = endDate.ToString("yyyy-MM-dd hh:mm:ss");

            query.Append("SELECT WidgetItemReferID into #turnedOnFundIds FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and IsActive=1 and SourceTypeId = 1 ", churchId, userId);

            query.Append("SELECT ISNULL(SUM(rt.amount),0) AS 'Total' ");
            query.Append("FROM chmcontribution..contribution_receipt rt ");
            query.Append("join ChmContribution..FUND fd on rt.Fund_ID=fd.Fund_ID and fd.Fund_type=1 ");
            query.AppendFormat("WHERE rt.Received_date >= '{0}' and rt.Received_date < '{1}' ", start, end);
            //query.Append("and rt.Fund_ID in (select Fund_ID from #Fund_IDs) ");
            query.Append("and rt.Fund_id in (select * from #turnedOnFundIds) ");

            //query.Append("DROP TABLE #Fund_IDs ");
            query.Append("DROP TABLE #turnedOnFundIds ");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                return double.Parse(dt.Rows[0]["Total"].ToString());
            }
        }

        /// <summary>
        /// Get the sum of contributions for giving chart in a given time period
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public double Dashboard_Giving_GetPeriodSum_Chart(int churchId, int userId, DateTime startDate, DateTime endDate)
        {
            StringBuilder query = new StringBuilder();

            string start = string.Format("{0} 00:00:00.000", startDate.ToString("yyyy-MM-dd"));
            string end = string.Format("{0} 00:00:00.000", endDate.AddDays(1).ToString("yyyy-MM-dd"));
            //string end = endDate.ToString("yyyy-MM-dd hh:mm:ss");

            query.Append("SELECT WidgetItemReferID into #turnedOnFundIds FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and isQueryActive=1 and SourceTypeId = 1 ", churchId, userId);

            query.Append("SELECT ISNULL(SUM(rt.amount),0) AS 'Total' ");
            query.Append("FROM chmcontribution..contribution_receipt rt ");
            query.Append("join ChmContribution..FUND fd on rt.Fund_ID=fd.Fund_ID and fd.Fund_type=1 ");
            query.AppendFormat("WHERE rt.Received_date >= '{0}' and rt.Received_date < '{1}' ", start, end);
            //query.Append("and rt.Fund_ID in (select Fund_ID from #Fund_IDs) ");
            query.Append("and rt.Fund_id in (select * from #turnedOnFundIds) ");

            //query.Append("DROP TABLE #Fund_IDs ");
            query.Append("DROP TABLE #turnedOnFundIds ");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                return double.Parse(dt.Rows[0]["Total"].ToString());
            }
        }

        /// <summary>
        /// Get the total of attribute use against specific attribute group in a time period
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public int Dashboard_AttributeGroup_GetPeriodSum(int churchId, int userId, int attributeGroupId, DateTime startDate, DateTime endDate)
        {
            StringBuilder query = new StringBuilder();

            string start = string.Format("{0} 00:00:00.000", startDate.ToString("yyyy-MM-dd"));
            string end = string.Format("{0} 00:00:00.000", endDate.AddDays(1).ToString("yyyy-MM-dd"));
            //string end = endDate.ToString("yyyy-MM-dd hh:mm:ss");

            query.Append("SELECT WidgetItemReferID into #turnedOnAttributeIds FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and isActive=1 and SourceTypeId= 4 ", churchId, userId);

            query.Append("SELECT COUNT(*) AS 'Total'  FROM ChmPeople..INDIVIDUAL_ATTRIBUTE ia ");
            query.Append("JOIN ChmPeople..ATTRIBUTE a on ia.ATTRIBUTE_ID = a.ATTRIBUTE_ID ");
            query.AppendFormat("WHERE a.attribute_group_id={0} and ia.START_DATE >= '{1}' AND ia.START_DATE < '{2}' ", attributeGroupId, start, end);
            query.Append("and a.ATTRIBUTE_ID in (select * from #turnedOnAttributeIds) ");

            query.Append("drop table #turnedOnAttributeIds");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                return int.Parse(dt.Rows[0]["Total"].ToString());
            }
        }

        /// <summary>
        /// Get the total of attribute use against specific attribute group for attribute group chart in a time period
        /// </summary>
        /// <param name="churchId">The church id.</param>
        public int Dashboard_AttributeGroup_GetPeriodSum_Chart(int churchId, int userId, int attributeGroupId, DateTime startDate, DateTime endDate)
        {
            StringBuilder query = new StringBuilder();

            string start = string.Format("{0} 00:00:00.000", startDate.ToString("yyyy-MM-dd"));
            string end = string.Format("{0} 00:00:00.000", endDate.AddDays(1).ToString("yyyy-MM-dd"));
            //string end = endDate.ToString("yyyy-MM-dd hh:mm:ss");

            query.Append("SELECT WidgetItemReferID into #turnedOnAttributeIds FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and isQueryActive=1 and SourceTypeId= 4 ", churchId, userId);

            query.Append("SELECT COUNT(*) AS 'Total'  FROM ChmPeople..INDIVIDUAL_ATTRIBUTE ia ");
            query.Append("JOIN ChmPeople..ATTRIBUTE a on ia.ATTRIBUTE_ID = a.ATTRIBUTE_ID ");
            query.AppendFormat("WHERE a.attribute_group_id={0} and ia.START_DATE >= '{1}' AND ia.START_DATE < '{2}' ", attributeGroupId, start, end);
            query.Append("and a.ATTRIBUTE_ID in (select * from #turnedOnAttributeIds) ");

            query.Append("drop table #turnedOnAttributeIds");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                return int.Parse(dt.Rows[0]["Total"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific fund is checked in settings
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="fundId">The fund id.</param>
        public bool Dashboard_Giving_Fund_Is_Checked_InSettings(int churchId, int userId, int fundId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and SourceTypeId = 1 and WidgetItemReferId={2}", churchId, userId, fundId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsActive"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific item is checked in settings
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        public bool Dashboard_Widget_Item_Is_Checked_InSettings(int userWidgetItemId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE UserWidgetItemID={0}", userWidgetItemId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsActive"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific item is checked in chart
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        public bool Dashboard_Widget_Item_Is_Checked_InChart(int userWidgetItemId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsQueryActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE UserWidgetItemID={0}", userWidgetItemId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsQueryActive"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific attribute is checked in settings
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="attributeId">The attribute id.</param>
        public bool Dashboard_AttributeGroup_Attribute_Is_Checked_InSettings(int churchId, int userId, int attributeId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and SourceTypeId = 4 and WidgetItemReferId={2}", churchId, userId, attributeId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsActive"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific fund is checked in chart
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="fundId">The fund id.</param>
        public bool Dashboard_Giving_Fund_Is_Checked_InChart(int churchId, int userId, int fundId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsQueryActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and SourceTypeId = 1 and WidgetItemReferId={2}", churchId, userId, fundId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsQueryActive"].ToString());
            }
        }

        /// <summary>
        /// This method judge if specific attribute is checked in chart
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="attributeId">The attribute id.</param>
        public bool Dashboard_AttributeGroup_Attribute_Is_Checked_InChart(int churchId, int userId, int attributeId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT IsQueryActive FROM ChmDashboard..USERWIDGETITEM ");
            query.AppendFormat("WHERE CHURCHID={0} AND USERID={1} and SourceTypeId = 4 and WidgetItemReferId={2}", churchId, userId, attributeId);
            using (DataTable dt = this.Execute(query.ToString()))
            {
                return bool.Parse(dt.Rows[0]["IsQueryActive"].ToString());
            }
        }

        /// <summary>
        /// This method fetches the user id for an individual.
        /// </summary>
        /// <param name="churchId">The church id the user belongs to.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="login">The login name of the user.</param>
        /// <returns>Integer representing the user.</returns>
        public int User_FetchID(int churchId, string email, string loginName)
        {
            return Convert.ToInt32(this.Execute("SELECT user_id FROM ChmChurch..USERS with(nolock) WHERE church_id = {0} and EMAIL LIKE '{1}' and login='{2}'", churchId, email, loginName).Rows[0]["user_id"]);
        }

        /// <summary>
        /// This method fetches the attibute group id and name.
        /// </summary>
        /// <param name="attributeId">The attribute id.</param>
        /// <returns>Array contains attribute id and name.</returns>
        public string[] Dashboard_GetAttributeGroupID(int userWidgetItemId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT ag.attribute_group_id, ag.attribute_group_name FROM ChmPeople..ATTRIBUTE a ");
            query.Append("JOIN ChmPeople..ATTRIBUTE_GROUP ag on ag.attribute_group_id = a.attribute_group_id ");
            query.Append("JOIN ChmDashboard..UserWidgetItem wi on a.attribute_id = wi.WidgetItemReferID ");
            query.AppendFormat("WHERE wi.UserWidgetItemID={0}", userWidgetItemId);

            using (DataTable dt = this.Execute(query.ToString()))
            {
                string attributeGroupName = dt.Rows[0]["attribute_group_name"].ToString();
                string attributeGroupId = dt.Rows[0]["attribute_group_id"].ToString();

                return new string[] { attributeGroupName, attributeGroupId };
            }
        }


        /// <summary>
        /// Get the count of report rights which name is 'Contribution'/'Contributor Visibility'/'Contributor Summaries' for a specific user
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="userId">The user id.</param>
        public int Dashboard_Giving_GetCountOfReportRights(int churchId, int userId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) AS 'TOTAL' FROM ChmChurch..SECURITY_TYPE AS st WITH (nolock) ");
            query.Append("INNER JOIN ChmChurch..ROLE_SECURITY_TYPE AS rst (nolock) ON st.SECURITY_TYPE_ID = rst.SECURITY_TYPE_ID ");
            query.Append("INNER JOIN ChmChurch..USER_ROLE AS ur (nolock) ON rst.ROLE_ID = ur.role_id ");
            query.Append("inner JOIN ChmChurch..SECURITY_GROUP AS sg (nolock) ON st.SECURITY_GROUP_ID = sg.SECURITY_GROUP_ID ");
            query.AppendFormat("WHERE ur.USER_ID = {0} AND ur.CHURCH_ID = {1} and sg.SECURITY_GROUP_Name = 'Report Rights' ", userId, churchId);
            query.Append("and st.SECURITY_TYPE_NAME in ('Contribution','Contributor Visibility','Contributor Summaries')");

            using (DataTable dt = this.Execute(query.ToString()))
            {
                return Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());
            }
        }

        /// <summary>
        /// Clear all data related to specific user in dashboard database
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="loginName">The login name.</param>
        public void Dashboard_ClearData(int churchId, string loginName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("DECLARE @UserId int, @ChurchId int,@UserName varchar(255) ");
            query.AppendFormat("SET @UserName = '{0}' ", loginName);
            query.AppendFormat("SET @ChurchId = {0} ", churchId);
            query.Append("SELECT @UserId = USER_ID FROM ChmChurch..Users with(nolock) WHERE church_id=@ChurchId and login=@UserName ");
            query.Append("SELECT UserWidgetID INTO #userWidgetIDs FROM ChmDashboard..UserWidget with(nolock) WHERE UserId=@UserId ");
            query.Append("DELETE FROM ChmDashboard..UserWidgetItem WHERE UserWidgetID in (SELECT * FROM #userWidgetIDs) ");
            query.Append("DELETE FROM ChmDashboard..UserWidget WHERE UserId=@UserId ");
            query.Append("DELETE FROM ChmDashboard..UserTimeframe WHERE UserId=@UserId ");
            query.Append("DROP TABLE #userWidgetIDs");
            
            this.Execute(query.ToString());
        }

        /// <summary>
        /// Insert an individual_attribute record
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individua id.</param>
        public void Dashboard_Insert_IndividualAttribute(int churchId, int individualId, int attributeId, DateTime startDate, DateTime endDate)
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            StringBuilder query = new StringBuilder();

            string start = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            string end = endDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            query.Append("INSERT INTO ChmPeople..INDIVIDUAL_ATTRIBUTE(CHURCH_ID,INDIVIDUAL_ID,ATTRIBUTE_ID,STAFF_ID,START_DATE,END_DATE,COMMENT) ");
            if(startDate == now.AddYears(100) && endDate == now.AddYears(100))
            {
                query.AppendFormat("VALUES({0}, {1}, {2}, null, null,null, null)", churchId, individualId, attributeId);
            }
            else if (startDate != now.AddYears(100) && endDate == now.AddYears(100))
            {
                query.AppendFormat("VALUES({0}, {1}, {2}, null, '{3}',null, null)", churchId, individualId, attributeId, start);
            }
            else
            {
                query.AppendFormat("VALUES({0}, {1}, {2}, null, '{3}','{4}', null)", churchId, individualId, attributeId, start, end);
            }

            this.Execute(query.ToString());
        }

        /// <summary>
        /// Delete an individual_attribute record
        /// </summary>
        /// <param name="churchId">The church id.</param>
        /// <param name="individualId">The individua id.</param>
        public void Dashboard_Delete_IndividualAttribute(int churchId, int individualId, int attributeId)
        {
            string query = string.Format("DELETE FROM ChmPeople..INDIVIDUAL_ATTRIBUTE WHERE CHURCH_ID={0} AND INDIVIDUAL_ID={1} AND ATTRIBUTE_ID={2}", churchId, individualId, attributeId);
            this.Execute(query);
        }
        #endregion
        
        /// <summary>
        /// Deletes a module from the church
        /// </summary>
        /// <param name="churchId"></param>
        /// <param name="moduleId"></param>
        public void Portal_Modules_Delete(int churchId, int moduleId)
        {
            this.Execute(string.Format("DELETE from ChmPortal.dbo.CHURCH_MODULE WHERE CHURCH_ID = {0} AND MODULE_ID = {1}", churchId, moduleId));
        }

        public void Portal_Modules_Delete(int churchId, string moduleName)
        {
            this.Execute(string.Format("DELETE from ChmPortal.dbo.CHURCH_MODULE WHERE CHURCH_ID = {0} AND MODULE_ID = (select module_id from ChmPortal..MODULE where module_name = '{1}')", 
                churchId, moduleName));
        }

        public void Portal_Modules_Add(int churchId, string moduleName)
        {
            this.Execute(string.Format("Insert into ChmPortal.dbo.CHURCH_MODULE (MODULE_ID, CHURCH_ID) values ((select module_id from ChmPortal..MODULE where module_name = '{0}'), {1})",
                moduleName, churchId));
        }

        public void Portal_Modules_Add(int churchId, int moduleId)
        {
            this.Execute(string.Format("Insert into ChmPortal.dbo.CHURCH_MODULE (MODULE_ID, CHURCH_ID) values ({0}, {1})",
                moduleId, churchId));
        }

        /// <summary>
        /// TODO Place holder for executing a stored procedure 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="indList"></param>
        public void ExecuteDBProc(string proc, string indList)
        {
            DataTable results;
            SqlDataReader dr;
            decimal total = 0.00m;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ChurchID", 15));
                    cmd.Parameters.Add(new SqlParameter("@IndividualID", 29972315));
                    cmd.Parameters.Add(new SqlParameter("@StartReceivedDate", "2013-01-01"));
                    cmd.Parameters.Add(new SqlParameter("@EndReceivedDate", "2013-12-31"));
                    cmd.Parameters.Add(new SqlParameter("@IndividualIDList", indList));
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        //for (int i = 0; i < dr.FieldCount; i++)
                        //{
                        //TestLog.WriteLine(string.Format("Column Name: {0}", Convert.ToDouble(dr["Amount"])));
                        //TestLog.WriteLine(string.Format("Amount Total: {0}", total = total + Convert.ToDouble(dr["Amount"])));
                        total = total + Convert.ToDecimal(dr["Amount"]);
                        //}
                    }
                }

                dbConnection.Close();
            }

            //{0:C}
            string sumDecimal = total.ToString("#,##0.00");
            TestLog.WriteLine(string.Format("Amount Total: {0}", sumDecimal));

        }

        /// <summary>
        /// Execute a PROC with no Params 
        /// </summary>
        /// <param name="proc">Proc Name</param>
        public void ExecuteDBProc(string proc)
        {
            DataTable results;
            SqlDataReader dr;

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                dbConnection.Open();

                using (SqlCommand cmd = new SqlCommand(proc, dbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    dr = cmd.ExecuteReader();
                }

                dbConnection.Close();
            }

        }

        /// <summary>
        /// Executes a SQL query.
        /// </summary>
        /// <param name="query">String representing the query to be executed.</param>
        /// <returns>A data table containing the results of the query.</returns>
        public DataTable Execute(string query, params Object[] args) {

            log.Debug("Enter Execute " + query);
            DataTable results;

                using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
                {
                    try{

                      dbConnection.Open();

                      string queryRTG = args == null ? query : string.Format(query, args);
                      using (SqlDataAdapter dbAdapter = new SqlDataAdapter(queryRTG, dbConnection))
                      {
                        results = new DataTable();
                        //log.Debug(string.Format("DB row result: {0}", results.Rows.Count));
                        dbAdapter.Fill(results);
                      }

                      dbConnection.Close();

                    }catch(Exception e){

                        log.Error(string.Format("Error in executing query: {0} {1}", query, e.Message));
                        dbConnection.Close();
                        throw new Exception(e.StackTrace);

                    }
                }

            return results;
        }

        /// <summary>
        /// Executes a SQL query by params.
        /// </summary>
        /// <param name="query">String representing the query to be executed.</param>
        /// <returns>A data table containing the results of the query.</returns>
        public DataTable ExecuteSql(string query, params SqlParameter[] args)
        {
            log.Debug("Enter Execute " + query);
            DataTable results = new DataTable();

            using (SqlConnection dbConnection = new SqlConnection(this._dbConnectionString))
            {
                try
                {
                    //dbConnection.Open();
                    var dbCommand = dbConnection.CreateCommand();
                    dbCommand.CommandText = query;
                    if (args != null && args.Length > 0)
                    {
                        dbCommand.Parameters.AddRange(args);
                    }
                    using (SqlDataAdapter dbAdapter = new SqlDataAdapter(dbCommand))
                    {
                        dbAdapter.Fill(results);
                        log.Debug(string.Format("Executing sql success."));
                    }
                    //dbConnection.Close();
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Error in executing query: {0} {1}", query, e.Message));
                    dbConnection.Close();
                    throw new Exception(e.StackTrace);
                }
            }

            return results;
        }

        #region AMS

        /// <summary>
        /// Adds AMS status for a Churches bases on entry in DelegateChurchToAms Table.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Boolean> Set_AMS_Church_Status()
        {

            //Instantiate variables so they at least be empty
            DataTable amdDT = new DataTable();
            Dictionary<string, Boolean> ams = new Dictionary<string, Boolean>();
            StringBuilder query = new StringBuilder("SELECT * FROM ChmContribution.dbo.DelegateChurchToAms WITH (NOLOCK)");
            bool amsStatus = false;

            //If we get any type of exception then we are defaulting AMS to DISABLED
            try
            {
                amdDT = this.Execute(query.ToString());

            }
            catch (Exception e)
            {
                log.Warn(string.Format("Delegate Church AMS Table Not Present"));
            }

            if (amdDT.Rows.Count > 0)
            {

                foreach (DataRow dr in amdDT.Rows)
                {

                    log.Debug("**********  AMS  **********");
                    string localChurchId = dr["ChurchId"].ToString();
                    string localDisableAmsTrans = dr["DisableAmsTransactions"].ToString();
                    string localDisableAmsAcct = dr["DisableAmsAccount"].ToString();
                    log.Debug(string.Format("Church ID: {0}", localChurchId));
                    log.Debug(string.Format("DisableAmsTransactions : {0}", localDisableAmsTrans));
                    log.Debug(string.Format("DisableAmsAccount : {0}", localDisableAmsAcct));

                    if (localDisableAmsTrans.Equals("True") && localDisableAmsAcct.Equals("True"))
                    {
                        //Opposite 
                        log.Debug(string.Format("AMS for Church ID {0} is DISABLED", localChurchId));
                        //ams.Add(dr["ChurchId"].ToString(), false);
                        amsStatus = false;
                        
                    }
                    else if (localDisableAmsTrans.Equals("False") && localDisableAmsAcct.Equals("False"))
                    {
                        log.Debug(string.Format("AMS for Church ID {0} is ENABLED", localChurchId));
                        //ams.Add(dr["ChurchId"].ToString(), true);
                        amsStatus = true;
                    }
                    else
                    {
                        log.Debug(string.Format("AMS for Church ID {0} is being defaulted to DISABLED", localChurchId));
                        //ams.Add(dr["ChurchId"].ToString(), false);
                        amsStatus = false;
                    }

                    //Check if already there then just update else add to Dictionary
                    bool value;
                    if(ams.TryGetValue(localChurchId, out value))
                    //if (ams.ContainsKey(dr["ChurchId"].ToString()))
                    {
                        log.Debug(string.Format("Updating AMS status for Church ID {0} to {1}", localChurchId, amsStatus));
                        ams[localChurchId] =  amsStatus;
                    }
                    else
                    {
                        ams.Add(localChurchId, amsStatus);
                    }

                    log.Debug("***************************");
                }

            }
            else
            {
                log.Info("********** AMS DISABLED for all Churches");
            }

            //Set Global AMS
            this._ams = ams;

            //Return it, what you do with it is up to you
            return ams;

        }

        /// <summary>
        /// Queries Delegate Church To AMS Table
        /// </summary>
        /// <param name="churchID"></param>
        /// <returns></returns>
        public DataTable Query_DelegateChurchToAms(int churchID)
        {
            StringBuilder queryDelgateChurchToAms = new StringBuilder("SELECT * FROM [ChmContribution].[dbo].[DelegateChurchToAms] WITH (NOLOCK) ");
            queryDelgateChurchToAms.AppendFormat("WHERE ChurchId = {0}", churchID);           
            return this.Execute(queryDelgateChurchToAms.ToString());
        }

        /// <summary>
        /// Queries Delegate Church To AMS Table
        /// </summary>
        /// <param name="churchID"></param>
        /// <returns></returns>
        public DataTable Query_DelegateChurchToAms(KeyValuePair<int, string> churchID)
        {
            StringBuilder queryDelgateChurchToAms = new StringBuilder("SELECT * FROM [ChmContribution].[dbo].[DelegateChurchToAms] WITH (NOLOCK) ");
            queryDelgateChurchToAms.AppendFormat("WHERE ChurchId = {0}", churchID.Key);
            return this.Execute(queryDelgateChurchToAms.ToString());
        }

        public void Update_DelegateChurchToAms(KeyValuePair<int, string> churchID, bool amsEnabled, bool amsMigrated)
        {
            log.DebugFormat("Updating AMS for church ID [{0}] to AMS status: [{1}] and AMS Migrated: [{2}]", churchID.Value, amsEnabled, amsMigrated);
            StringBuilder updateDelgateChurchToAms = new StringBuilder("UPDATE [ChmContribution].[dbo].[DelegateChurchToAms] SET ");
            
            if (amsMigrated)
            {

                updateDelgateChurchToAms.AppendFormat("[DisableAmsTransactions] = {0} , [DisableAmsAccount] = {1}, [Monitor] = 0, [ModifiedDate] = GetDate() WHERE ChurchId = {2}", 1, 0, churchID.Key);

            }
            else
            {
                if (amsEnabled)
                {
                    updateDelgateChurchToAms.AppendFormat("[DisableAmsTransactions] = {0} , [DisableAmsAccount] = {1}, [Monitor] = 0, [ModifiedDate] = GetDate() WHERE ChurchId = {2}", 0, 0, churchID.Key);
                }
                else
                {
                    updateDelgateChurchToAms.AppendFormat("[DisableAmsTransactions] = {0} , [DisableAmsAccount] = {1}, [Monitor] = 0, [ModifiedDate] = GetDate() WHERE ChurchId = {2}", 1, 1, churchID.Key);
                }
            }
                    
            this.Execute(updateDelgateChurchToAms.ToString());
        }

        /// <summary>
        /// Inserts a Church to DelegateChurchToAms Table
        /// </summary>
        /// <param name="churchID"></param>
        /// <param name="amsEnabled"></param>
        /// <param name="enabledDisabled"></param>
        public void Insert_DelegateChurchToAms(KeyValuePair<int, string> churchID, bool amsEnabled, bool amsMigrated)
        {
            log.DebugFormat("Adding AMS for church [{0}] to AMS status: [{1}] and AMS Migrated: [{2}]", churchID.Value, amsEnabled, amsMigrated);
            StringBuilder insertDelgateChurchToAms = new StringBuilder("INSERT INTO [ChmContribution].[dbo].[DelegateChurchToAms] (ChurchId, DisableAmsTransactions, DisableAmsAccount, CreatedDate, ModifiedDate, Monitor) VALUES ");


            if (amsMigrated)
            {
                insertDelgateChurchToAms.AppendFormat("({0}, {1}, {2}, GetDate(), GetDate(), 0)", churchID.Key, 1, 0);
            }
            else
            {
                if (amsEnabled)
                {
                    insertDelgateChurchToAms.AppendFormat("({0}, {1}, {2}, GetDate(), GetDate(), 0)", churchID.Key, 0, 0);
                }
                else
                {
                    insertDelgateChurchToAms.AppendFormat("({0}, {1}, {2}, GetDate(), GetDate(), 0)", churchID.Key, 1, 1);
                }

            }
            
            this.Execute(insertDelgateChurchToAms.ToString());
        }

        /// <summary>
        /// Deletes Church from DelegateChurchToAms Table
        /// </summary>
        /// <param name="churchID"></param>
        public void Delete_DelegateChurchToAms(int churchID)
        {

            StringBuilder deleteDelgateChurchToAms = new StringBuilder("DELETE FROM [ChmContribution].[dbo].[DelegateChurchToAms] WHERE ");
            deleteDelgateChurchToAms.AppendFormat("ChurchId = {0}", churchID);
            this.Execute(deleteDelgateChurchToAms.ToString());
            log.DebugFormat("Removed AMS entry for church [{0}]", churchID);

        }

        /// <summary>
        /// Deletes Church from DelegateChurchToAms Table
        /// </summary>
        /// <param name="churchID">Church ID</param>
        public void Delete_DelegateChurchToAms(KeyValuePair<int, string> churchID)
        {

            StringBuilder deleteDelgateChurchToAms = new StringBuilder("DELETE FROM [ChmContribution].[dbo].[DelegateChurchToAms] WHERE ");
            deleteDelgateChurchToAms.AppendFormat("ChurchId = {0}", churchID.Key);
            this.Execute(deleteDelgateChurchToAms.ToString());
            log.DebugFormat("Removed AMS entry for church [{0}]", churchID.Value);

        }

        /// <summary>
        /// Gets AMS Status for a Church ID to be used throughout the framework
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <returns>true/false</returns>
        public Boolean IsAMSEnabled(int churchId)
        {
            log.DebugFormat("Enter IsAMSEnabled for {0}", churchId);
            Boolean amsEnabled = false;

            if (this.AMS.Count > 0)
            {
                if (this.AMS.ContainsKey(Convert.ToString(churchId)))
                {
                    this.AMS.TryGetValue(Convert.ToString(churchId), out amsEnabled);
                }
            }

            return amsEnabled;
        }

        /// <summary>
        /// Gets AMS Status for a Church ID directly from DB. Used mainly for initial setup AMS status
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <returns>true/false</returns>
        public Boolean AMS_Church_Status(int churchId)
        {

            log.DebugFormat("Enter AMS_Church_Status for {0}", churchId);
            //Instantiate variables so they at least be empty
            DataTable amdDT = new DataTable();
            Dictionary<string, Boolean> ams = new Dictionary<string, Boolean>();
            Boolean amsChurchStatus = false;

            StringBuilder query = new StringBuilder(string.Format("SELECT * FROM ChmContribution.dbo.DelegateChurchToAms WITH (NOLOCK) WHERE ChurchId = {0}", churchId));

            //If we get any type of exception then we are defaulting AMS to DISABLED
            try
            {
                amdDT = this.Execute(query.ToString());

            }
            catch (Exception e)
            {
                log.Warn(string.Format("Delegate Church AMS Table Not Present"));
            }

            if (amdDT.Rows.Count > 0)
            {

                foreach (DataRow dr in amdDT.Rows)
                {

                    if (dr["DisableAmsTransactions"].ToString().Equals("False") && dr["DisableAmsAccount"].ToString().Equals("False"))
                    {
                        amsChurchStatus = true;
                    }
                    
                }
            }

            log.Debug(string.Format("Current AMS Status for Church ID [{0}] is {1}", churchId, amsChurchStatus));


            //Return status
            return amsChurchStatus;

        }

        /// <summary>
        /// Gets AMS Wallset Status for a Church ID directly from DB. Used mainly for initial setup AMS Wallet status
        /// </summary>
        /// <param name="churchId">Church ID</param>
        /// <returns>true/false/empty string</returns>
        public bool AMS_Migrated_Status(int churchId)
        {

            log.DebugFormat("Enter AMS_Migrated_Status for {0}", churchId);
            //Instantiate variables so they at least be empty
            DataTable amdDT = new DataTable();
            Dictionary<string, Boolean> ams = new Dictionary<string, Boolean>();
            bool amsMigratedStatus = false;

            StringBuilder query = new StringBuilder(string.Format("SELECT * FROM ChmContribution.dbo.DelegateChurchToAms WITH (NOLOCK) WHERE ChurchId = {0}", churchId));

            //If we get any type of exception then we are defaulting AMS to DISABLED
            try
            {
                amdDT = this.Execute(query.ToString());

            }
            catch (Exception e)
            {
                log.Warn(string.Format("Delegate Church AMS Table Not Present"));
            }

            if (amdDT.Rows.Count > 0)
            {

                foreach (DataRow dr in amdDT.Rows)
                {

                    if (dr["DisableAmsTransactions"].ToString().Equals("True") && dr["DisableAmsAccount"].ToString().Equals("False"))
                    {
                        amsMigratedStatus = true;

                    }

                }
            }

            log.Debug(string.Format("Current AMS Migrated Status for Church ID [{0}] is {1}", churchId, amsMigratedStatus));

            //Return status
            return amsMigratedStatus;

        }

        /// <summary>
        /// Queries AMS Table to find out how many churches are in the DelegateChurchToAms table.
        /// </summary>
        /// <returns></returns>
        public List<string> QueryAMSTable()
        {
            log.Debug("Enter QueryAMSTable");
            //Instantiate variables so they at least be empty
            DataTable amdDT = new DataTable();
            List<string> amsTableEntries = new List<string>();

            //If we get any type of exception then we are defaulting AMS to DISABLED
            try
            {
                amdDT = this.Execute(new StringBuilder("SELECT * FROM ChmContribution.dbo.DelegateChurchToAms WITH (NOLOCK)").ToString());

            }
            catch (Exception e)
            {
                log.Warn(string.Format("Delegate Church AMS Table Not Present"));
            }

            if (amdDT.Rows.Count > 0)
            {
                foreach (DataRow dr in amdDT.Rows)
                {
                    amsTableEntries.Add(dr["ChurchId"].ToString());
                }
            }

            return amsTableEntries;
        }

        #endregion AMS

        #region AMS Setup

        public Dictionary<int, string> TokenizeAmsChurches(string amsChurchesValue, SQL sql)
        {
            //Instantiate variables so they at least be empty
            DataTable amdDT = new DataTable();
            string[] amsChurches = null;
            Dictionary<int, string> amsChurchesDict = new Dictionary<int, string>();
            Dictionary<int, string> nonAmsChurches = new Dictionary<int, string>();

            //Set List of Test Churches
            List<string> amsTestChurches = new List<string>();
            amsTestChurches.Add("DC");
            amsTestChurches.Add("QAEUNLX0C1"); 
            amsTestChurches.Add("QAEUNLX0C2");
            amsTestChurches.Add("QAEUNLX0C3");
            amsTestChurches.Add("QAEUNLX0C4");
            amsTestChurches.Add("QAEUNLX0C6");
            amsTestChurches.Add("DCDMDFWTX");

            StringBuilder query = new StringBuilder("SELECT [CHURCH_ID], [CHURCH_NAME], [CHURCH_CODE] FROM [ChmChurch].[dbo].[CHURCH] WITH (NOLOCK)");
            amdDT = sql.Execute(query.ToString());

            log.Debug(string.Format("AMS Churches: {0}", amsChurchesValue));

            //Remove spaces before tokenize amsChurches
            amsChurchesValue.Trim().Replace(" ", "");
            amsChurches = amsChurchesValue.Split(',');

            //Sift through passed in ams church values and get the churchIDs and add them to the AMS church list
            foreach (string amsChurch in amsChurches)
            {
                string church = amsChurch.Trim();
                if (!string.IsNullOrWhiteSpace(church))
                {
                    amsChurchesDict.Add(sql.FetchChurchID(church), church);
                    log.Debug(string.Format("AMS Church Code: {0}", church));
                }
            }

            /*foreach (string amsChurch in amsChurches) 
            {                
                //log.Info(string.Format("AMS Church: {0}", amsChurch.Trim()));
                foreach (DataRow dr in amdDT.Rows)
                {
                    //log.Debug(string.Format("Church Code: {0}", dr["CHURCH_CODE"].ToString()));
                    //Add to AMS list
                    if (dr["CHURCH_CODE"].ToString().Equals(amsChurch.Trim()))
                    {
                        if (!amsChurchesDict.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"])))
                        {
                            log.Debug(string.Format("AMS Church Code: {0} [{1}] ", amsChurch.Trim(), dr["CHURCH_ID"].ToString()));
                            amsChurchesDict.Add(Convert.ToInt32(dr["CHURCH_ID"]), dr["CHURCH_CODE"].ToString());
                        }
                    }                    
                }
            }
            */

            //Add non AMS churches to the nonAmsChurch list
            //This will only be our Test Churches
            //DC, QAEUNLX0C1, QAEUNLX0C2, QAEUNLX0C3, QAEUNLX0C4, QAEUNLX0C6, DCDMDFWTX
            foreach (string amsTestChurch in amsTestChurches)
            {
                if (!amsChurchesDict.ContainsValue(amsTestChurch))
                {
                    log.Debug(string.Format("*** Non AMS Church Code: {0} ", amsTestChurch));
                    nonAmsChurches.Add(sql.FetchChurchID(amsTestChurch), amsTestChurch);
                }
            }

            /*foreach (string amsChurch in amsChurches){

                foreach(DataRow dr in amdDT.Rows)
                {
                    if (!dr["CHURCH_CODE"].ToString().Equals(amsChurch.Trim()))
                    {
                        if(!nonAmsChurches.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"])) && (!amsChurchesDict.ContainsKey(Convert.ToInt32(dr["CHURCH_ID"]))))
                        {
                            //log.Debug(string.Format("*** Non AMS Church Code: {0} [{1}] ", dr["CHURCH_CODE"].ToString(), dr["CHURCH_ID"].ToString()));
                            nonAmsChurches.Add(Convert.ToInt32(dr["CHURCH_ID"]), dr["CHURCH_CODE"].ToString());                            
                        }

                    }
                }
            }*/



            //Set it to access it later
            this._amsChurches = amsChurchesDict;
            this._nonAmsChurches = nonAmsChurches;

            //Print what we got
            foreach (KeyValuePair<int, string> amsChurch in this._amsChurches)
            {
                log.Debug(string.Format("AMS Church Code: {0} [{1}] ", amsChurch.Key, amsChurch.Value));
            }

            foreach (KeyValuePair<int, string> nonAmsChurch in this._nonAmsChurches)
            {
                log.Debug(string.Format("Non AMS Church Code: {0} [{1}] ", nonAmsChurch.Key, nonAmsChurch.Value));
            }

            //Do we really want to return it?
            return _amsChurches;
        }

        public void EnableDisableAMSChurches(bool amsEnabled, bool amsMigrated, SQL sql)
        {
            log.InfoFormat("AMS Enabled: {0}", amsEnabled);
            log.InfoFormat("AMS Migrated Enabled: {0}", amsMigrated);

            //Get Global AMS used for checking through if church is enabled or not
            //sql.AMS_Enable_Status_Add();
            if(sql.QueryAMSTable().Count > 0) { Thread.Sleep(1000); };

            //If specified to enable AMS churches but there are not then kill everything else code will remove all churches
            if ( (amsEnabled || amsMigrated) && (this._amsChurches.Count == 0)) {
                if (amsEnabled)
                {
                    throw new Exception(string.Format("AMS was set to {0} and there were no churches specified", amsEnabled));
                }
                else
                {
                    throw new Exception(string.Format("AMS Migrated was set to {0} and there were no churches specified", amsMigrated));
                }
            }

            //Let's make sure we have ams populated data to set enableDisable flag
            if (this._amsChurches.Count > 0)
            {

                foreach (KeyValuePair<int, string> churchID in _amsChurches)
                {
                    //Is ChurchId already in table                    
                    //rows = sql.Query_DelegateChurchToAms(churchID).Rows.Count;
                    //log.DebugFormat("Rows found in DelegateChurchToAms: {0}", rows);
                    //amsTableEntries = sql.QueryAMSTable();

                    //Is it in DelegateChurchToAms table
                    //Enabled/Disable it
                    DataTable dtDelegate = this.Query_DelegateChurchToAms(churchID.Key);
                    log.DebugFormat("Delegate Church [{0}] To AMS count = {1}", churchID.Key, dtDelegate.Rows.Count); 
                    if (dtDelegate.Rows.Count > 0)
                    {
                        if (amsMigrated)
                        {
                            //Set to Migrated Only
                            if (!sql.AMS_Migrated_Status(churchID.Key) == amsMigrated)
                            {
                                this.Update_DelegateChurchToAms(churchID, false, true);
                            }
                            else
                            {
                                log.DebugFormat("AMS Migrated Status for Church ID [{0}] already [{1}]", churchID.Value, amsMigrated);
                            }
                        }
                        else
                        {
                            //AMS church can be disabled by
                            // 1. trans = 1, account = 0 (Migrated) OR 
                            // 2. trans = 1, account = 1
                            if( (!sql.AMS_Church_Status(churchID.Key) == amsEnabled) || (sql.AMS_Migrated_Status(churchID.Key) == true) )
                            {
                                this.Update_DelegateChurchToAms(churchID, amsEnabled, false);
                            }
                            else
                            {
                                log.DebugFormat("AMS status for Church ID [{0}] already [{1}]", churchID.Value, amsEnabled);
                            }
                        }
                    }
                    else
                    {
                        if (amsMigrated)
                        {
                            //Set to Migrated Only
                            this.Insert_DelegateChurchToAms(churchID, false, true);
                        }
                        else
                        {
                            this.Insert_DelegateChurchToAms(churchID, amsEnabled, false);
                        }
                    }

                    //Slow it down a bit
                    Thread.Sleep(1000);
                }
            }

            //Remove all other non AMS churches from DelegateChurchToAms table
            foreach (KeyValuePair<int, string> churchID in _nonAmsChurches)
            {
                if (this.QueryAMSTable().Contains(Convert.ToString(churchID.Key)))
                {
                    this.Delete_DelegateChurchToAms(churchID.Key);
                }
            }

            //TODO Find Duplicates and remove them and leave only one
            this.Delete_Duplicate_AMS_Churches();

            //Set Global AMS used for checking through if church is enabled or not
            this.Set_AMS_Church_Status();

        }

        /// <summary>
        /// Delete any duplicate entries in DelegateChurchToAms table
        /// </summary>
        public void Delete_Duplicate_AMS_Churches()
        {

            if (this._amsChurches.Count > 0)
            {
                foreach (KeyValuePair<int, string> churchID in _amsChurches)
                {
                    DataTable dt = this.Query_DelegateChurchToAms(churchID.Key);
                    for (int r = 1; r < dt.Rows.Count; r++)
                    {
                        StringBuilder deleteQuery = new StringBuilder("DELETE FROM [ChmContribution].[dbo].[DelegateChurchToAms] WHERE ");
                        deleteQuery.AppendFormat("DelegateChurchToAmsId = {0}", dt.Rows[r]["DelegateChurchToAmsId"]);
                        log.Debug(deleteQuery);
                        this.Execute(deleteQuery.ToString());
                    }                    

                }


            }

        }

        #endregion AMS Setup
    }
}