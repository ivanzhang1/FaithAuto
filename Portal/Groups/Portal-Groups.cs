using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;

namespace FTTests.Portal.Groups {
	public struct GroupsByGroupTypeConstants {
		public struct ViewAllTabs {
			public const string GroupsTab = "//a[contains(@href, '/Groups/Group/ListAll.aspx?search_context=1')]";
			public const string PeopleListTab = "link=People Lists";
			public const string TempGroupTab = "link=Temporary";
            public const string TempGroupTabLink = "Temporary";
			public const string SpanOfCareTab = "link=Spans of Care";
		}

		public struct ViewAll {
			// Header links
			public const string HeaderLink_NameAndStartDate = "link=Name and start date";
			public const string HeaderLink_Name = "link=Name";
			public const string HeaderLink_GroupTypesAndCustomFields = "link=Group types and custom fields";
			public const string HeaderLink_StandardFields = "link=Standard fields";
			public const string HeaderLink_Demographics = "link=Standard Fields";

            public const string HeaderLink_NameAndStartDate_Link = "Name and start date";
            public const string HeaderLink_Name_Link = "link=Name";
            public const string HeaderLink_GroupTypesAndCustomFields_Link = "Group types and custom fields";
            public const string HeaderLink_StandardFields_Link = "Standard fields";
            public const string HeaderLink_Demographics_Link = "Standard Fields";

			// Sidebar links
			public const string Link_NewGroup = "//a[contains(@href, '/Groups/Group/Step1.aspx')]";
			public const string Link_NewPeopleList = "link=Add a people list";

			// Search options and fields
			public const string NameField = "group_name";
			public const string IndividualNameField = "individual_name";
            public const string StartDateFrom = "start_date_from";
            public const string StartDateTo = "start_date_to";
			public const string CheckBox_Coed = "gender_0";
			public const string CheckBox_Female = "gender_1";
			public const string CheckBox_Male = "gender_2";
			public const string CheckBox_Married = "marital_status_1";
			public const string CheckBox_Single = "marital_status_2";
			public const string CheckBox_MarriedOrSingle = "marital_status_0";
			public const string DropDown_AgeRangeMin = "age_range_min";
			public const string DropdDown_AgeRangeMax = "age_range_max";
			public const string CheckBox_Sunday = "day_of_week_1";
			public const string CheckBox_Monday = "day_of_week_2";
			public const string CheckBox_Tuesday = "day_of_week_3";
			public const string CheckBox_Wednesday = "day_of_week_4";
			public const string CheckBox_Thursday = "day_of_week_5";
			public const string CheckBox_Friday = "day_of_week_6";
			public const string CheckBox_Saturday = "day_of_week_7";

			public const string SearchButton = "commit";

            // Show Leaders Members Gear
            public const string Gear_GroupsTab = "link=Actions";
            public const string Gear_Link_ShowLeadersMembers = "link=Show Leaders and Members";
            public const string Gear_Link_Export = "link=Export...";
            
            //Export
            public const string Export_Mail_Merge_XLS = "link=XLS";
		}

		public struct SaveASearch {
			public const string Field_Name = "new_search_name";
			public const string Field_Description = "new_description";
			public const string Link_Drawer = "//img[@alt='Search_pull_tab']";
			public const string Button_SaveSearch = "btn_save_search";
		}

		public struct GroupManagement {
			public const string Button_Add_Member_Save = "add_new_member";
			public const string Button_EditDetailsSaveChanges = "submitQuery";
			public const string Button_NewLocationSaveChanges = "submitQuery";
			public const string Button_NewScheduleSaveChanges = "submitQuery";

            public const string Checkbox_PrivateLocation = "is_private";

			public const string Dropdown_Member_Role = "GroupMemberType";
			public const string Dropdown_GroupTimeZone = "time_zone";
			public const string DropDown_State = "ddlStates";

			public const string Link_NewLocation = "link=New location";
			public const string Link_EditLocation = "link=Edit location";
			public const string Link_NewSchedule = "link=New schedule";
			public const string Link_AddCustomField = "link=Add custom field";

			public const string Link_ViewProspects = "link=Prospects";
			public const string Link_ViewProspects_Active = "link=//a[contains(@href, 'showActive=True')]";
			public const string Link_ViewProspects_Closed = "//*[@id='closed_count']";
            public const string Link_ViewProspects_Closed_WD = "closed_count";
			public const string Link_ViewGroupSettings = "link=View group settings";

			public const string Link_ChangePermissions = "link=Change permissions";

			public const string Link_EditGroupDetails = "link=Edit group details";
			public const string Link_EditGroupCustomFields = "link=Manage custom fields";

			public const string Link_DeleteGroup = "link=Delete this group";
			public const string Link_DeleteSchedule = "link=Delete schedule";

			public const string Gear_ProspectsMenu = "link=Actions";
			public const string Gear_ProspectsMenu_ExportToCSV = "link=Export to CSV";


			public const string DateControl_GroupStartDate = "start_date";
			public const string DateControl_ScheduleStartDate = "start_date";

			public const string TextField_ScheduleStartTime = "start_time";
			public const string TextField_GroupName = "group_name";
			public const string TextField_GroupDescription = "group_description";
			public const string TextField_LocationName = "location_name";
			public const string TextField_LocationDescription = "location_description";
			public const string TextField_Address1 = "address_address1";
			public const string TextField_City = "address_city";
			public const string TextField_ZipCode = "address_postalcode";

			public const string RadioButton_WeeklyEvent = "recurrence_weekly";
			public const string RadioButton_MonthlyEvent = "recurrence_monthly";

			// Custom Field UI at the group level
			public const string CustomFields_SectionLandingPage = "//div[@id='main_content']/div[1]/div[2]/div[4]";
			public const string CustomFields_PageHeaders = "//div[@id='main_content']/div[1]/div[2]/form/h3";
		}

        public struct ShowLeadersAndMembers {
            public const string RadioButton_Filter_All = "filter_all";
            public const string RadioButton_Filter_Leaders = "filter_leaders";
            public const string RadioButton_Filter_Members = "filter_members";

            public const string Button_Filter = GeneralButtons.submitQuery;
        }

		public struct Wizard_ExportCSV {
			public const string Link_Back = "link=← Back";
			public const string Button_Next = "button_submit";

			public const string Checkbox_All = "//input[@type='checkbox']";
			public const string Checkbox_IndividualName = "//input[@name='column_name' and @value='Individual Name']";
			public const string Checkbox_CommunicationEmail = "//input[@name='column_name' and @value='Communication - Email Count']";
			public const string Checkbox_CommunicationPhone = "//input[@name='column_name' and @value='Communication – Phone Count']";
			public const string Checkbox_CommunicationMeeting = "//input[@name='column_name' and @value='Communication – Meeting Count']";
			public const string Checkbox_CommunicationComment = "//input[@name='column_name' and @value='Communication – Comment Count']";
			public const string Link_ReturnToProspects = "link=Return to prospects";
		}

		public struct Wizard_GroupCreation {
			public const string Link_EditStep1 = "//a[contains(@href, '/Groups/Group/Step1.aspx')]";
			public const string Link_EditStep2 = "//a[contains(@href, '/Groups/Group/Step2.aspx')]";
			public const string Link_EditStep3 = "//a[contains(@href, '/Groups/Group/Step3.aspx')]";
			public const string Link_EditStep4 = "//a[contains(@href, '/Groups/Group/Step4.aspx')]";

			public const string Link_AddGroup_Active_Group = "active_group_add";

			public const string Step2_TextField_GroupName = "group_name";
			public const string Step2_TextField_GroupDescription = "group_description";
			public const string Step2_DropDown_GroupTimeZone = "time_zone";
			public const string Step2_DropDown_Campus = "group_campus";
			public const string Step2_DateControl_StartDate = "start_date";
			public const string Step2_DropDown_Gender = "group_gender";
			public const string Step2_DropDown_MaritalStatus = "marital_status";
			public const string Step2_Dropdown_AgeRangeLower = "age_from";
			public const string Step2_Dropdown_AgeRangeUpper = "age_to";
            public const string Step2_Checkbox_Childcare = "childcare";
			public const string Step2_Checkbox_Unlockable = "unlock_group";
			public const string Step2_Checkbox_Searchable = "is_searchable";
			public const string Step2_Button_Next = "submitQuery";
			public const string Step2_Link_Back = "link=← Back";

			public const string Step3_Link_SkipThisStep = "link=Skip this step →";
			public const string Step3_DateControl_ScheduleStartDate = "start_date";
			public const string Step3_TextField_ScheduleStartTime = "start_time";
            public const string Step3_TextField_ScheduleEndTime = "end_time";
			public const string Step3_Checkbox_ScheduleEndTime = "end_toggle";
			public const string Step3_RadioButton_OneTimeEvent = "recurrence_never";
			public const string Step3_RadioButton_WeeklyEvent = "recurrence_weekly";
			public const string Step3_RadioButton_MonthlyEvent = "recurrence_monthly";
			public const string Step3_Checkbox_EndDate = "end_toggle_date";
			public const string Step3_Button_Next = "submitQuery";
			public const string Step3_Link_Back = "link=← Back";

            public const string Step3_Checkbox_Sunday = "recurrence_weekly_sunday";
            public const string Step3_Checkbox_Monday = "recurrence_weekly_monday";
            public const string Step3_Checkbox_Tuesday = "recurrence_weekly_tuesday";
            public const string Step3_Checkbox_Wednesday = "recurrence_weekly_wednesday";
            public const string Step3_Checkbox_Thursday = "recurrence_weekly_thursday";
            public const string Step3_Checkbox_Friday = "recurrence_weekly_friday";
            public const string Step3_Checkbox_Saturday = "recurrence_weekly_saturday";
            public const string Step3_DropDown_Reoccurance_Weekly = "recurrence_every";
            public const string Step3_TextField_ScheduleEndDate = "end_date";

			public const string Step4_Link_SkipThisStep = "link=Skip this step →";
			public const string Step4_TextField_LocationName = "location_name";
			public const string Step4_TextField_LocationDescription = "location_description";
			public const string Step4_RadioButton_MeetsInPerson = "location_physical";
			public const string Step4_RadioButton_MeetsOnline = "location_online";
            public const string Step4_TextField_URL = "location_url";
			public const string Step4_Dropdown_Country = "ddlCountries";
			public const string Step4_TextField_Address1 = "address_address1";
			public const string Step4_TextField_Address2 = "address_address2";
			public const string Step4_TextField_City = "address_city";
			public const string Step4_Dropdown_State = "ddlStates";
			public const string Step4_TextField_Zipcode = "address_postalcode";
			public const string Step4_Button_Next = "submitQuery";
			public const string Step4_Link_Back = "link=← Back";
            public const string Step4_Checkbox_Private_Location = "is_private";

			public const string Step5_Checkbox_PublicGroup = "group_permissions_check";
			public const string Step5_Button_Next = "submitQuery";
			public const string Step5_Link_Back = "link=← Back";
		}

		public struct Wizard_PeopleListCreation {
			public const string TextField_PeopleListName = "group_name";
			public const string TextField_PeopleListDescription = "group_description";
			public const string DateControl_StartDate = "start_date";
            public const string Checkbox_Unlock_Group = "accepting_members";
            public const string Button_CreatePeopleList = "submitQuery";

            public const string TextField_IndividualSearch = "txtSearch";
            public const string Button_IndividualSearch = "btnAdvSearch";
            public const string Button_Save = "add_new_member";
            public const string Button_SaveAddAnother = "add_new_member_another";
            public const string Link_CancelLinkName = "Cancel";
		}
	}

	public struct GroupsAdministrationConstants {
		public struct GroupTypeManagement {
			public const string Link_NewGroupType = "link=New group type";
            public const string Link_NewGroupType_Text = "New group type";
			public const string Link_EditAdminRights = "//a[contains(@href, '/Groups/GroupType/EditRights.aspx')]";
			public const string Link_EditViewRights = "//a[contains(@href, '/Groups/GroupType/EditViews.aspx')]";
			public const string Link_EditPermissions = "//a[contains(@href, '/Groups/GroupType/EditPermissions.aspx')]";

			public const string Button_EditCustomFields_SaveChanges = "update_customfields";

			public const string TextField_GroupTypeName = "group_type_name";
			public const string TextField_GroupTypeDescription = "group_type_description";
			public const string CheckBox_GroupsPublic = "group_type_web_enabled";
			public const string CheckBox_GroupsSearchable = "group_type_searchable";
		}

		public struct SearchCategoryManagement {
			public const string Link_AddSearchCategory = "link=Add a search category";
			public const string Link_EditSearchCategoryDetails = "link=Edit details";
			public const string Link_EditSeachCategoryCriteria = "link=Edit search criteria";
			public const string Link_DeleteSearchCategory = "link=Delete this category";

			public const string RadioButton_ShowAllCategories = "show_all_categories";
			public const string RadioButton_ShowUnpublishedCategories = "show_unpublished_categories";
			public const string RadioButton_ShowPublishedCategories = "show_published_categories";

			public const string TextField_SearchCategoryName = "category_name";
			public const string TextField_SearchCategoryDescription = "category_description";

			public const string Button_SaveChanges_EditDetails = GeneralButtons.submitQuery;
			public const string Button_SaveChanges_EditCriteria = GeneralButtons.submitQuery;
			public const string Button_Filter_Categories = "//input[@value='Filter']";

			public const string Checkbox_Published = "category_published";
		}

        public struct SearchCategoryManagementWebDriver
        {
            public const string Link_AddSearchCategory = "Add a search category";
            public const string Link_EditSearchCategoryDetails = "Edit details";
            public const string Link_EditSeachCategoryCriteria = "Edit search criteria";
            public const string Link_DeleteSearchCategory = "Delete this category";

            public const string RadioButton_ShowAllCategories = "show_all_categories";
            public const string RadioButton_ShowUnpublishedCategories = "show_unpublished_categories";
            public const string RadioButton_ShowPublishedCategories = "show_published_categories";

            public const string TextField_SearchCategoryName = "category_name";
            public const string TextField_SearchCategoryDescription = "category_description";

            public const string Button_SaveChanges_EditDetails = GeneralButtons.submitQuery;
            public const string Button_SaveChanges_EditCriteria = GeneralButtons.submitQuery;
            public const string Button_Filter_Categories = "//input[@value='Filter']";

            public const string Checkbox_Published = "category_published";
        }

		public struct SpanOfCareManagement {
			public const string Button_Owners_AddNew = "add_new_owner";
			public const string Button_Owners_AdvancedSearch = "btn_search_adv";
			public const string Dropdown_Owners_AttributeGroup = "attribute_group";
			public const string Dropdown_Owners_IndividualAttribute = "attribute";
			public const string Link_Owners_Add = "//a[contains(@href, '/Groups/GroupSoc/NewOwner.aspx')]";
			public const string Link_Owners_AdvancedSearch = "link=Advanced search";
		}

		public struct Wizard_CustomField {
			public const string Step2_Text_CustomFieldDescription = "//div[@id='main_content']/div[1]/div[2]/form/p[1]";
		}

		public struct Wizard_GroupType {
			public const string Step2_Link_Back = "link=← Back";
			public const string Step3_Link_Back = "link=← Back";
			public const string Step4_Link_Back = "link=← Back";
			public const string Step5_Link_Back = "link=← Back";

			public const string Link_EditStep1 = "//a[contains(@href, '/Groups/GroupType/Step1.aspx')]";
			public const string Link_EditStep2 = "//a[contains(@href, '/Groups/GroupType/Step2.aspx')]";
			public const string Link_EditStep3Link = "//a[contains(@href, '/Groups/GroupType/Step3.aspx')]";
			public const string Link_EditStep4 = "//a[contains(@href, '/Groups/GroupType/Step4.aspx')]";

			// Name, Description, and Viewable/Searchable properties
			public const string Step1_TextField_GroupTypeName = "group_type_name";
			public const string Step1_TextField_GroupTypeDescription = "group_type_description";
			public const string Step1_CheckBox_GroupsPublic = "group_type_web_enabled";
			public const string Step1_CheckBox_GroupsSearchable = "group_type_searchable";
			public const string Step1_Button_Next = GeneralButtons.submitQuery;

			// Leader and members permissions
			public const string Step2_CheckBox_LeadersEmailGroup = "group_type_leaders_can_email";
			public const string Step2_CheckBox_LeadersAdministerGroup = "group_type_leaders_can_admin";
			public const string Step2_CheckBox_LeadersEditMemberRecords = "group_type_leaders_can_edit";
			public const string Step2_CheckBox_LeadersEditGroupDetails = "group_type_leaders_can_updategroup";
			public const string Step2_CheckBox_LeadersChangeSchedule = "group_type_leaders_can_schedule";
            public const string Step2_CheckBox_LeadersTakeAttendace = "group_type_leaders_can_take_attendance";
			public const string Step2_CheckBox_MembersEmailGroup = "group_type_members_can_email";
			public const string Step2_Button_Next = GeneralButtons.submitQuery;

			// Leaders and members view rights
			public const string Step3_Radio_LeadersLimitedInformation = "group_type_leaders_limited_view";
			public const string Step3_Radio_LeadersBasicInformation = "group_type_leaders_default_view";
			public const string Step3_Radio_LeadersFullInformation = "group_type_leaders_full_view";
			public const string Step3_Radio_MembersLimitedInformation = "group_type_members_limited_view";
			public const string Step3_Radio_MembersBasicInformation = "group_type_members_default_view";
			public const string Step3_Button_Next = GeneralButtons.submitQuery;

			public const string Step4_Button_Next = GeneralButtons.submitQuery;
			public const string Step5_Button_SaveGroupType = GeneralButtons.submitQuery;
		}

		public struct Wizard_SearchCategories {
			public const string Step1_TextField_SearchCategoryName = "category_name";
			public const string Step1_TextField_SearchCategoryDescription = "category_description";
			public const string Step1_Checkbox_Published = "category_published";
			public const string Step1_Button_Next = GeneralButtons.submitQuery;

			//public const string Step2_Link_Previous = "← Previous";
			public const string Step2_Button_CreateSearchCategory = GeneralButtons.submitQuery;
		}

		public struct Wizard_SpanOfCare {
			public const string Step1_TextField_SpanOfCareDescription = "soc_desc";

			public const string Step3_CheckBox_Coed = "chk_gender_coed";
			public const string Step3_CheckBox_Female = "chk_gender_female";
			public const string Step3_CheckBox_Male = "chk_gender_male";
			public const string Step3_CheckBox_MarriedOrSingle = "chk_marital_status_both";
			public const string Step3_CheckBox_Married = "chk_marital_status_married";
			public const string Step3_CheckBox_Single = "chk_marital_status_single";
			public const string Step3_DropDown_MinAgeRange = "age_range_min";
			public const string Step3_DropDown_MaxAgeRange = "age_range_max";
			public const string Step3_Radio_ChildcareProvided = "rb_childcare_yes";
			public const string Step3_Radio_ChildcareNotProvided = "rb_childcare_no";
			public const string Step3_Radio_ChildcareNotApplicable = "rb_childcare_none";

		}
	}



	[TestFixture]
	public class Portal_Groups_WebDriver : FixtureBaseWebDriver {
        [Test, RepeatOnFailure, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Verifies the links present under the groups menu.")]
        public void Groups() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Verify the links present
            test.Driver.FindElementByLinkText("Groups").Click();
            TestLog.Write("Groups by Group Type Header: {0}", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[1]/dt").Text);
            Assert.AreEqual("Groups by Group Type", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[1]/dt").Text.Trim());
            Assert.AreEqual("View All", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[1]/dd[1]/a").Text);

            Assert.AreEqual("Administration", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[2]/dt").Text);
            Assert.AreEqual("Group Types", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[2]/dd[1]/a").Text);
            Assert.AreEqual("Custom Fields", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[2]/dd[2]/a").Text);
            Assert.AreEqual("Search Categories", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[2]/dd[3]/a").Text);
            Assert.AreEqual("Span of Care", test.Driver.FindElementByXPath("//div[@id='nav_sub_13']/dl[2]/dd[4]/a").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        #region Groups by Group Type
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the view all page under groups.")]
        public void Groups_GroupsByGroupType_ViewAll() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Select groups->view all
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.GroupsByGroupType.View_All);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: View All", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("View All");

            // ** Create a group type
            // Create a new group
            test.Driver.FindElementByXPath(GroupsByGroupTypeConstants.ViewAll.Link_NewGroup).Click();

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Group Type", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Group Type");

            // Select a group type to display additional controls
            test.Driver.FindElementByXPath("//table[@class='grid faux_iphone']/tbody/tr[*]/td/a").Click();

            // Verify the checkbox for marking a group as searchable is present, checked
            Assert.IsTrue(test.GeneralMethods.IsElementPresentWebDriver(By.Id("is_searchable")));
            Assert.IsTrue(test.Driver.FindElementById("is_searchable").Selected);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Groups by Group Type

        #region Administration
        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the group types page under groups.")]
        public void Groups_Administration_GroupTypes() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to groups->group types
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Group_Types);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Group Types", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Group Types");

            // ** New group type
            // Select link to create a new group type
            test.Driver.FindElementByLinkText(GroupsAdministrationConstants.GroupTypeManagement.Link_NewGroupType_Text).Click();

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Step 1 Group Type Wizard", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Properties");

            // Verify name label
            Assert.AreEqual("Name *", test.Driver.FindElementByXPath("//label[@for='group_type_name']").Text);

            // Verify is_searchable checkbox is present and disabled
            test.GeneralMethods.VerifyElementPresentWebDriver(By.XPath("//input[@id='group_type_searchable' and @disabled]"));
            //Assert.GreaterThan(test.Selenium.GetElementPositionTop("//input[@id='group_type_searchable' and @disabled]"), 10);
            Assert.IsTrue(test.Driver.FindElementByXPath("//input[@id='group_type_searchable' and @disabled]").Location.Y > 10);

            // ** Back to landing page
            // Select the back button
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.RETURN).Click();

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Group Types", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Group Types");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the custom fields page under groups.")]
        public void Groups_Administration_CustomFields() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to groups->custom fields
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Custom_Fields);

            // Verify title, text
            test.GeneralMethods.WaitForElement(test.Driver, By.LinkText("Add"));
            Assert.AreEqual("Fellowship One :: Custom Fields", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Custom Fields");

            // Verify add link
            Assert.AreEqual("Add", test.Driver.FindElementByXPath("//div[@id='main_content']/div[1]/div[2]/div/a").Text);

            // Verify table headers
            Assert.AreEqual("In Use", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr/th[1]", TableIds.Groups_CustomFields)).Text);
            Assert.AreEqual("Custom Field", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr/th[2]", TableIds.Groups_CustomFields)).Text);
            Assert.AreEqual("Field Type", test.Driver.FindElementByXPath(string.Format("{0}/tbody/tr/th[3]", TableIds.Groups_CustomFields)).Text);


            // Create a custom field
            test.Driver.FindElementByLinkText(GeneralLinksWebDriver.Add).Click();

            // Verify title, text
            test.GeneralMethods.WaitForElement(test.Driver, By.Id("submit_custom_field"));
            Assert.AreEqual("Fellowship One :: Create Custom Field", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Create Custom Field");

            // Verify required field labels
            Assert.AreEqual("Field name *", test.Driver.FindElementByXPath("//label[@for='custom_field_name']").Text);
            Assert.AreEqual("Field type *", test.Driver.FindElementByXPath("//label[@for='field_type']").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Author("Matthew Sneeden")]
        [Description("Navigates to the search categories page under groups.")]
        public void Groups_Administration_SearchCategories() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to groups->search categories
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Search_Categories);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Search Categories", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Search Categories");


            // Select link to create a new search category
            test.Driver.FindElementByLinkText(GroupsAdministrationConstants.SearchCategoryManagementWebDriver.Link_AddSearchCategory).Click();

            // Verify title, text
            Assert.AreEqual("Fellowship One :: Category Details", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("Category Details");

            // Verify required field
            Assert.AreEqual("Name *", test.Driver.FindElementByXPath("//label[@for='category_name']").Text);

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Matthew Sneeden")]
        [Description("Views the Span of Care page.")]
        public void Groups_Administration_SpanOfCare() {
            // Login to portal
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.Portal.LoginWebDriver();

            // Navigate to groups->span of care
            test.GeneralMethods.Navigate_Portal(Navigation.Groups.Administration.Span_Of_Care);

            // Verify title, text
            Assert.AreEqual("Fellowship One :: View Span of Care", test.Driver.Title);
            test.GeneralMethods.VerifyTextPresentWebDriver("View Span of Care");

            // Logout of portal
            test.Portal.LogoutWebDriver();
        }
        #endregion Administration
	}
}