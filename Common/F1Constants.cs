using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace FTTests {
    public struct Navigation {
        public struct InFellowship {
            public const string Your_Profile = "//ul[@id='home_links']/li[@id='update_profile']/a";
            public const string Privacy_Settings = "//ul[@id='home_links']/li[@id='privacy_settings']/a";
            public const string Change_Your_EmailPassword = "//ul[@id='home_links']/li[@id='update_username']/a";
            public const string Church_Directory = "//ul[@id='home_links']/li[@id='Li1']/a";
            public const string Your_Groups = "//ul[@id='home_links']/li[@id='your_groups']/a";
            public const string Find_A_Group = "//ul[@id='home_links']/li[@id='find_a_group']/a";
            //public const string Your_Giving = "//ul[@id='home_links']/li[@id='your_giving' and contains(a/text()[2], 'Your Giving')]/a";
            //Testign with new Html change
            public const string Your_Giving = "//ul[@id='home_links']/li[@id='your_giving']/a";

            public const string Your_Giving_GiveNow = "link=Give Now";
            public const string Your_Giving_ScheduleGiving = "link=Schedule Giving";
            public const string Your_Giving_ScheduleGiving_WebDriver = "Schedule Giving";
            public const string Your_Giving_GivingSchedules_Tab = "Schedules";


        }

        public struct Portal {
            public struct Giving {
                public struct Contributions {
                    public const string Search = "Giving › Contributions › Search";
                    public const string Batches_All = "Giving › Contributions › Batches";
                    public const string Batches_General = "Giving › Contributions › Batches › General";
                    public const string Batches_CreditCardBatches = "Giving › Contributions › Batches › Credit Card";
                    public const string Batches_ScannedContributions = "Giving › Contributions › Scanned Contributions";
                    public const string Unmatched = "Giving › Contributions › Unmatched";

                    public const string Batches_Scanned = "Giving › Contributions › Batches > Scanned";                                        
                    public const string Enter_Contributions = "Giving › Contributions › Enter Contributions";
                }

                public struct Statements {
                    public const string Statement_Builder = "Giving › Statements › Statement Builder";
                    public const string Queue = "Giving › Statements › Queue";
                    public const string Custom_Styles = "Giving › Statements › Custom Styles";
                    public const string Online_Statement = "Giving › Statements › Online Statement";
                }

                public struct Setup {
                    public const string Funds = "Giving › Setup › Funds";
                    public const string Pledge_Drives = "Giving › Setup › Pledge Drives";
                    public const string Contribution_Attributes = "Giving › Setup › Contribution Attributes";
                    public const string Sub_Funds = "Giving › Setup › Sub Funds";
                    public const string Sub_Types = "Giving › Setup › Sub Types";
                    public const string Account_References = "Giving › Setup › Account References";
                }
            }

            public struct Admin {
                public struct Security_Setup {
                    public const string PortalUsers = "Admin › Security Setup › Portal Users";
                }
                public struct Church_Setup {
                    public const string PortalUsers = "Admin › Security Setup › Portal Users";
                    public const string ChurchContacts = "Admin › Church Setup › Church Contacts";
                    public const string News_Announcements = "Admin › Church Setup › News & Announcements";
                    public const string Campuses = "Admin › Church Setup › Campuses";
                }
                public struct Ministry_Setup {
                    public const string Volunteer_Types = "Admin › Ministry Setup › Volunteer Types";
                }
            }

            public struct WebLink {
                public struct Event_Registration {
                    public const string Manage_Forms = "WebLink › Event Registration › Manage Forms";
                    public const string Aggregate_Forms = "WebLink › Event Registration › Aggregate Forms";
                    public const string View_Submissions = "WebLink › Event Registration › View Submissions";

                    //////////

                    public struct ManageForms
                    {
                        #region Manage Forms

                        #region Form creation
                        public const string Add_Edit_FormName = "ctl00_ctl00_MainContent_content_txtFormName_textBox";
                        public const string Active_Form = "ctl00_ctl00_MainContent_content_chkActive";
                        public const string Save_Form = "ctl00_ctl00_MainContent_content_btnSaveSettings";
                        public const string Confirmation_Save = "ctl00_ctl00_MainContent_content_btnSaveSettings";
                        #endregion form creation

                        #region Form select
                      

                        #endregion Form select
                        public const string Clone_Form = "ctl00_ctl00_MainContent_content_lnkbtnCopyForm";
                        public const string Restriction_Availability = "ctl00_ctl00_MainContent_content_lnkbtnRestrictions";
                        public const string Associate_Fund = "ctl00_ctl00_MainContent_content_lnkAccountReference";
                        
                        public const string Modify_Restriction = "ctl00_ctl00_MainContent_content_lnkWarningEditRestrictions";
                        public const string Preview = "ctl00_ctl00_MainContent_content_lnkQuickView";
                        public const string QuestionAndAnswer = "ctl00_ctl00_MainContent_content_hyplnkModifyQuestions";
                        public const string Schedule_Time_Choices = "ctl00_ctl00_MainContent_content_lnkScheduleTime";
                        public const string Modify_Activity_Association = "ctl00_ctl00_MainContent_content_lnkActivityAssoc";
                        public const string Age_Restrictions = "ctl00_ctl00_MainContent_content_lnkFormAgeRestriction";

                        #endregion
                        #region Question And Answer
                        public const string Add_New_header = "Add new header";
                        public const string Form_Header = "ctl00_ctl00_MainContent_content_txtFormHeader_textBox";
                        public const string Add_Header = "ctl00_ctl00_MainContent_content_btnAddHeader";
                        public const string Shared_Header = "ctl00_ctl00_MainContent_content_chkShared";
                        public const string Save_Header = "ctl00_ctl00_MainContent_content_btnDone";
                        public const string Search_Header = "ctl00_ctl00_MainContent_content_btnSearch";
                        public const string Shared_Question_Checkbox = "ctl00_ctl00_MainContent_content_chkShared";
                        public const string Add_Question = "ctl00_ctl00_MainContent_content_rptHeaders_ctl00_lnkAddQuestion";
                        public const string Question_Text = "ctl00_ctl00_MainContent_content_txtQuestionName_textBox";
                        public const string Answer_Required = "ctl00_ctl00_MainContent_content_chkAnswerRequired";
                        public const string Answer_Textbox = "ctl00_ctl00_MainContent_content_rdbtnText";
                        public const string Default_Answer_Textbox = "ctl00_ctl00_MainContent_content_txtOptionalDefaultText_textBox";
                        public const string Singleline_Answer_Textbox = "ctl00_ctl00_MainContent_content_rdbtnSingleText";
                        public const string Multiline_Answer_Textbox = "ctl00_ctl00_MainContent_content_rdbtnMultiLine";
                        public const string Dropdown_Answer = "ctl00_ctl00_MainContent_content_rdbtnDropDown";
                        public const string Save_Question = "ctl00_ctl00_MainContent_content_btnAddQuestion";
                        public const string Search_Question = "ctl00_ctl00_MainContent_content_btnSearch";
                        public const string Delete_Question = "ctl00_ctl00_MainContent_content_dgQuestions_ctl02_lnkDelete";
                        public const string Add_Answer_Choice = "ctl00_ctl00_MainContent_content_rptHeaders_ctl00_dlQuestions_ctl00_rptQuestionAnswerChoices_ctl00_lnkAddNewAnswerChoice";
                        public const string Answer_Choice_Textbox = "ctl00_ctl00_MainContent_content_txtAnswerChoice_textBox";
                        public const string Answer_Price_Textbox = "ctl00_ctl00_MainContent_content_txtPrice_textBox";
                        public const string Answer_Default = "ctl00_ctl00_MainContent_content_txtPrice_textBox";
                        public const string Answer_Save = "ctl00_ctl00_MainContent_content_btnAddAnswer";
                        public const string Validation_Type = "ctl00_ctl00_MainContent_content_ddlValidation_dropDownList";

                        #endregion
                        #region Schedule Activity Selection
                        public const string Establish_Activity = "ctl00_ctl00_MainContent_content_lnkActivityAssoc";
                        public const string Associate_Activity = "ctl00_ctl00_MainContent_content_rdbtnActivity";
                        public const string Ministry_Dropdown = "ctl00_ctl00_MainContent_content_ddlMinistry_dropDownList";
                        public const string Activity_Dropdown = "ctl00_ctl00_MainContent_content_ddlActivity_dropDownList";
                        public const string BreakoutGroup_Dropdown = "ctl00_ctl00_MainContent_content_ddlBreakoutGroup_dropDownList";
                        public const string ActivityGroup_Dropdown = "ctl00_ctl00_MainContent_content_ddlActivityGroup_dropDownList";
                        public const string RoomLocation_Dropdown = "ctl00_ctl00_MainContent_content_ddlRoomLocation_dropDownList";
                        public const string Save_Activity_Settings = "ctl00_ctl00_MainContent_content_btnSaveSettings";

                        #endregion
                        #region Schedule Assignment
                        public const string Select_Schedule = "ctl00_ctl00_MainContent_content_rdbtnSelectSchedule";
                        public const string Schedule_Source = "ctl00_ctl00_MainContent_content_lbScheduleSource";
                        public const string Schedule_Dest = "ctl00_ctl00_MainContent_content_lbScheduleDest";
                        public const string Schedule_Add = "ctl00_ctl00_MainContent_content_btnScheduleAdd";
                        public const string Schedule_Add_All = "ctl00_ctl00_MainContent_content_btnScheduleAddAll";
                        public const string Schedule_Remove = "ctl00_ctl00_MainContent_content_btnScheduleRemove";
                        public const string Schedule_Remove_All = "ctl00_ctl00_MainContent_content_btnScheduleRemoveAll";

                        public const string Select_Instance = "ctl00_ctl00_MainContent_content_rdbtnSelectInstance";
                        public const string Schedule_DropdownList = "ctl00_ctl00_MainContent_content_ddlActivitySchedules_dropDownList";
                        public const string Instance_Source = "ctl00_ctl00_MainContent_content_lbInstanceSource";
                        public const string Instance_Dest = "ctl00_ctl00_MainContent_content_lbInstanceDest";
                        public const string Instance_Add = "ctl00_ctl00_MainContent_content_btnInstanceAdd";
                        public const string Instance_Add_All = "ctl00_ctl00_MainContent_content_btnInstanceAddAll";
                        public const string Instance_Remove = "ctl00_ctl00_MainContent_content_btnInstanceRemove";
                        public const string Instance_Remove_All = "ctl00_ctl00_MainContent_content_btnInstanceRemoveAll";
                        public const string Schedule_Save = "ctl00_ctl00_MainContent_content_btnDone";

                        #endregion
                        #region Confirmation Message
                        public const string Create_Confirmation_Message = "ctl00_ctl00_MainContent_content_lnkConfirmationMessage";
                        public const string Confirmation_Message_Email = "ctl00_ctl00_MainContent_content_txtEmailFromAddress_textBox";
                        #endregion Confirmation Message
                        #region Account_Fund_Association
                        public const string Account_Association = "ctl00_ctl00_MainContent_content_lnkAccountReference";
                        public const string Fund_Select_Radio_button = "ctl00_ctl00_MainContent_content_rbtnFund";
                        public const string Fund_Select_Dropdown = "ctl00_ctl00_MainContent_content_ddlFund_dropDownList";
                        public const string Fund_Select_Save = "ctl00_ctl00_MainContent_content_btnSaveSettings";
                        public const string Price_Date_Range = "ctl00_ctl00_MainContent_content_lnkDateRangePricing";
                        #endregion Account_Fund_Association
                        #region Pricing
                        public const string Price_TextBox = "ctl00_ctl00_MainContent_content_txtPrice_textBox";
                        public const string Price_StartDate = "ctl00_ctl00_MainContent_content_ctl01_DateTextBox";
                        public const string Price_EndDate = "ctl00_ctl00_MainContent_content_ctl02_DateTextBox";
                        public const string Price_Settings_Save = "ctl00_ctl00_MainContent_content_btnAddPricing";
                        #endregion Pricing
                        #region Promocodes
                        public const string Promocode_Selet = "ctl00_ctl00_MainContent_content_lnkPromoCodes";
                        public const string Promocode_Name_Textbox = "ctl00_ctl00_MainContent_content_txtPromoCodeName_textBox";
                        public const string Promocode_Textbox = "ctl00_ctl00_MainContent_content_txtManualCode_textBox";
                        public const string Promocode_Discount_Amount_Select = "ctl00_ctl00_MainContent_content_rdbtnAmount";
                        public const string Promocode_Discount_Amount_Textbox = "ctl00_ctl00_MainContent_content_txtAmount_textBox";
                        public const string Promocode_Discount_Percentage_Select = "ctl00_ctl00_MainContent_content_rdbtnPercentage";
                        public const string Promocode_Discount_Percentage_Textbox = "ctl00_ctl00_MainContent_content_txtPercentage_textBox";
                        public const string Promocode_StartDate = "ctl00_ctl00_MainContent_content_ctl01_DateTextBox";
                        public const string Promocode_EndDate = "ctl00_ctl00_MainContent_content_ctl02_DateTextBox";
                        public const string Promocode_Settings_Save = "ctl00_ctl00_MainContent_content_btnAddPromoCode";
                        public const string Promocode_Done = "ctl00_ctl00_MainContent_content_btnDone";
                        #endregion Promocodes
                        #region DownPayment
                        public const string DownPayment_Select = "ctl00_ctl00_MainContent_content_lnkbtnPaymentSettings";
                        public const string DownPayment_Amount_Textbox = "ctl00_ctl00_MainContent_content_txtPaymentAmount_textBox";
                        public const string DownPayment_NoCutoffDate_Select = "ctl00_ctl00_MainContent_content_rdbtnNoCutOffDate";
                        public const string DownPayment_CutoffDate_Select = "ctl00_ctl00_MainContent_content_rdbtnCutOffDate";
                        public const string DownPayment_CutOffDate_Textbox = "ctl00_ctl00_MainContent_content_ctl02_DateTextBox";
                        public const string DownPayment_BalanceDueDate_Textbox = "ctl00_ctl00_MainContent_content_ctl03_DateTextBox";
                        public const string DownPayment_Setting_Save = "ctl00_ctl00_MainContent_content_btnSave";
                        #endregion DownPayment
                    }
                }
                //add by grace for Enable or Disable self-checkin
                public struct InFellowship 
                {
                    public struct Features
                    {
                        public const string Features_Link = "WebLink › InFellowship › Features";
                    }
                }
            }

            public struct Ministry {

                public struct ActivityRoomSetup {
                    public const string RLC = "Ministry › Activities (Legacy) › Rooms, Locations & Classes";
                }

                // changes "Ministry › Activities (New) › View All" to "Ministry › Activities › View All" since "(new)" was removed.(FO-4800)
                public struct Activities {
                    public const string View_All = "Ministry › Activities › View All";
                }

                public struct Volunteers {
                    public const string Staffing_Assignment = "Ministry › Volunteers (Legacy) › Staffing Assignment";
                }
            }

            public struct People {
                public struct Search {
                    public const string Add_Household = "People › Search › Add Household";
                    public const string My_Saved_Queries = "People › Search › My Saved Queries";
                    public const string People_Query = "People › Search › People Query";
                    public const string Check_All = "checkAll";
                }
            }
        }

        public struct People {
            public struct Search {
                public const string Find_a_Person = "People › Search › Find a Person";
                public const string Find_a_Person_Dateofbirth = "People › Search › Find a Person › Date of birth";
            }

            public struct FindAPerson {
                public struct PrivacySettings {
                    public const string Slider_Address = "//table[@id='slider_table']/tbody/tr[1]/td/div/a";
                    public const string Slider_Birthdate = "//table[@id='slider_table']/tbody/tr[2]/td/div/a";
                    public const string Slider_Email = "//table[@id='slider_table']/tbody/tr[3]/td/div/a";
                    public const string Slider_Phone = "//table[@id='slider_table']/tbody/tr[4]/td/div/a";
                    public const string Slider_Websites = "//table[@id='slider_table']/tbody/tr[5]/td/div/a";
                    public const string Slider_Social = "//table[@id='slider_table']/tbody/tr[6]/td/div/a";
                }
            }

            public struct GroupEmail {
                public const string Compose = "People › Group Email › Compose";

                public const string Drafts = "People › Group Email › Drafts";
                public const string Sent_Items = "People › Group Email › Sent Items";
                public const string Templates = "People › Group Email › Templates";
                public const string Delegates = "People › Group Email › Delegates";
            }

            public struct Volunteer_Pipeline {
                public const string Submit_Application = "People › Volunteer Pipeline › Submit Application";
                public const string Review_Applications = "People › Volunteer Pipeline › Review Applications";
                public const string Ministry_Review = "People › Volunteer Pipeline › Ministry Review";
                public const string Verify_Requirements = "People › Volunteer Pipeline › Verify Requirements";
                public const string Background_Checks = "People › Volunteer Pipeline › Background Checks";
                public const string Approved_Volunteers = "People › Volunteer Pipeline › Approved Volunteers";
                public const string Rejected_Volunteers = "People › Volunteer Pipeline › Rejected Volunteers";
            }

            public struct Data_Integrity {
                public const string Duplicate_Finder = "People › Data Integrity › Duplicate Finder";
                public const string Merge_Individual = "People › Data Integrity › Merge Individual";
                public const string Move_Individual = "People › Data Integrity › Move Individual";
                public const string Split_Household = "People › Data Integrity › Split Household";
                public const string Duplicate_Queue = "People › Data Integrity › Duplicate Queue";
                public const string Mass_Action_Queue = "People › Data Integrity › Mass Action Queue";
            }

            public struct CounselingMentoring {
                public const string View_Relationships = "People › Counseling/Mentoring › View Relationships";
            }
        }

        public struct Groups {
            public struct Administration {
                public const string Group_Types = "Groups › Administration › Group Types";
                public const string Custom_Fields = "Groups › Administration › Custom Fields";
                public const string Search_Categories = "Groups › Administration › Search Categories";
                public const string Span_Of_Care = "Groups › Administration › Span of Care";
            }

            public struct GroupsByGroupType {
                public const string View_All = "Groups › Groups by Group Type › View All";
            }

            

        }

        public struct Ministry {
            public struct Contacts {
                public const string My_Contacts = "Ministry › Contacts › My Contacts";
                public const string Edit_Contact_Forms = "Ministry › Contacts › Edit Contact Forms";
                public const string Monitor_Efficiency = "Ministry › Contacts › Monitor Efficiency";
                public const string Monitor_Statistics = "Ministry › Contacts › Monitor Statistics";
            }

            // All of the ActivityRoom_Setup test cases are commented by ivan.zhang. 09/24/2015. FO-4800
            public struct ActivityRoom_Setup {
                public const string Activities = "Ministry › Activities (Legacy) › Activities";
                public const string Activity_Schedules = "Ministry › Activities (Legacy) › Activity Schedules";
                public const string Activity_RLC_Groups = "Ministry › Activities (Legacy) › Activity RLC Groups";
                public const string Rooms_Locations_Classes = "Ministry › Activities (Legacy) › Rooms, Locations & Classes";
                public const string Breakout_Groups = "Ministry › Activities (Legacy) › Breakout Groups";
                public const string Activity_Requirements = "Ministry › Activities (Legacy) › Activity Requirements";
                public const string Group_Finder_Properties = "Ministry › Activities (Legacy) › Group Finder Properties";
            }

            public struct Activities {
                public const string View_All = "Ministry › Activities › View All";
            }

            public struct Assignments {
                public const string View_All = "Ministry › Assignments › View All";
                public const string Add_Assignment = "Ministry › Assignments › Add Assignment";
            }

            // All of the Participants test cases are commented by ivan.zhang. 09/24/2015. FO-4800
            public struct Participants {
                public const string Assignments = "Ministry › Participants (Legacy) › Assignments";
                public const string Manage_Assignments = "Ministry › Participants (Legacy) › Manage Assignments";
            }

            // All of the Volunteers test cases are commented by ivan.zhang. 09/24/2015. FO-4800
            public struct Volunteers {
                public const string Jobs = "Ministry › Volunteers (Legacy) › Jobs";
                public const string Schedules = "Ministry › Volunteers (Legacy) › Schedules";
                public const string VSO_Funnel_Report = "Ministry › Volunteers (Legacy) › VSO Funnel Report";
            }

            public struct Attendance {
                public const string Post_Attendance = "Ministry › Attendance › Post Attendance";
                public const string Head_Count = "Ministry › Attendance › Head Count";
                public const string Automated_Attendance_Rules = "Ministry › Attendance › Automated Attendance Rules";
            }

            public struct Checkin {
                public const string Live_Checkins = "Ministry › Check-in › Live Check-ins";
                public const string Super_Checkins = "Ministry › Check-in › Super Check-ins";
                public const string Theme_Manager = "Ministry › Check-in › Theme Manager";
            }
        }

        public struct WebLink {
            public struct Volunteer_Application {
                public const string Manage_Forms = "WebLink › Volunteer Application › Manage Forms";
            }

            public struct Online_Giving {
                public const string Confirmation_Messages = "WebLink › Online Giving › Confirmation Messages";
            }

            public struct Small_Group_Manager {
                public const string Questions = "WebLink › Small Group Manager › Questions";
                public const string Associate_Questions = "WebLink › Small Group Manager › Associate Questions";
            }

            public struct WebLink_Setup {
                public const string Church_Information = "WebLink › WebLink Setup › Church Information";
                public const string Skin_Manager = "WebLink › WebLink Setup › Skin Manager";
                public const string Integration_Codes = "WebLink › WebLink Setup › Integration Codes";
            }

            public struct InFellowship {
                public const string Branding = "WebLink › InFellowship › Branding";
                public const string Features = "WebLink › InFellowship › Features";
                public const string Links = "WebLink › InFellowship › Links";
                public const string Contact = "WebLink › InFellowship › Contact";
                public const string Privacy = "WebLink › InFellowship › Privacy";
                public const string Conversion = "WebLink › InFellowship › Online Giving Conversion";
            }
        }

        public struct Giving {
            public struct Contributions {
                public const string Contributor_Details = "Giving › Contributions › Contributor Details";
                public const string ContributorDetails_Scheduled = "Giving › Contributions › Contributor Details › Scheduled";
                public const string Batches = "Giving › Contributions › Batches";
                public const string Batches_RemoteDepositCapture = "Giving › Contributions › Batches › Remote Deposit Capture";
                public const string Batches_ScannedContributions = "Giving › Contributions › Scanned Contributions";
            }

            public struct Setup {
                public const string Pledge_Drives = "Giving › Setup › Pledge Drives";
                public const string Organizations = "Giving › Setup › Organizations";
            }
        }

        public struct Admin {
            public struct Security_Setup {
                public const string Portal_Users = "Admin › Security Setup › Portal Users";
                public const string Security_Roles = "Admin › Security Setup › Security Roles";
            }

            public struct People_Setup {
                public const string Status = "Admin › People Setup › Status";
                public const string Individual_Attributes = "Admin › People Setup › Individual Attributes";
                public const string Individual_Note_Types = "Admin › People Setup › Individual Note Types";
                public const string Individual_Requirements = "Admin › People Setup › Individual Requirements";
                public const string Relationship_Types = "Admin › People Setup › Relationship Types";
                public const string Schools = "Admin › People Setup › Schools";
            }

            public struct Ministry_Setup {
                public const string Ministries = "Admin › Ministry Setup › Ministries";
                public const string Head_Count_Attributes = "Admin › Ministry Setup › Head Count Attributes";
                public const string Activity_Types = "Admin › Ministry Setup › Activity Types";
                public const string Job_Attributes = "Admin › Ministry Setup › Job Attributes";
                public const string Job_Information = "Admin › Ministry Setup › Job Information";
            }

            public struct Contact_Setup {
                public const string Form_Names = "Admin › Contact Setup › Form Names";
                public const string Manage_Items = "Admin › Contact Setup › Manage Items";
                public const string Build_Forms = "Admin › Contact Setup › Build Forms";
                public const string Contact_Dispositions = "Admin › Contact Setup › Contact Dispositions";
            }

            public struct Church_Setup {
                public const string Church_Contacts = "Admin › Church Setup › Church Contacts";
                public const string Buildings = "Admin › Church Setup › Buildings";
                public const string Rooms = "Admin › Church Setup › Rooms";
                public const string Departments = "Admin › Church Setup › Departments";
                public const string Mailing_Address = "Admin › Church Setup › Mailing Address";
                public const string TWA_Census = "Admin › Church Setup › TWA Census";
                public const string Giftedness_Programs = "Admin › Church Setup › Giftedness Programs";
            }

            public struct Image_Upload {
                public const string View_Upload_Status = "Admin › Image Upload › View Upload Status";
            }

            public struct Integration {
                public const string Applications = "Admin › Integration › Applications";
                public const string Application_Keys = "Admin › Integration › Application Keys";
                public const string Data_Exchange_Export = "Admin › Integration › Data Exchange Export";
                public const string Data_Exchange_Import = "Admin › Integration › Data Exchange Import";
            }
        }
    }

    public struct InFellowshipConstants {
        public struct Groups {
            public struct GroupsConstants {
                public struct GroupManagement {
                    public struct NavigationLinks {
                        public const string Link_Dashboard = "link=Dashboard";
                        public const string Link_Roster = "link=Roster";
                        public const string Link_Prospects = "css=li#tab_group_prospects a";
                        public const string Link_Settings = "link=Settings";
                        public const string Link_CancelToRoster = "css=p > a.cancel";
                        public const string Link_TabBack = "tab_back";

                        public const string Gear_Menu = "nav_gear";
                        public const string Gear_SendAnEmail = "css=span.nav_gear_menu_email > a";
                        public const string Gear_InviteSomeone = "//a[contains(@href, '/Group/Invite/')]";
                        public const string Gear_TellAFriend = "//a[contains(@href, '/Group/TellAFriend/')]";
                        public const string Gear_FindAGroup = "link=Find a group";
                    }
                }

                public struct SpanOfCareDashboard {
                    public const string Link_Switch_SOC = "switch_group_trigger";
                    public const string Link_Groups = "link=Groups";
                    public const string Link_Groups_Numerical = "//a[contains(@href, '/GroupSoc/Groups')]";
                    public const string Link_Leaders_Numerical = "//a[contains(@href, '/GroupSoc/Groups')]";
                    public const string Link_Members_Numerical = "//a[contains(@href, '/GroupSoc/Groups')]";

                    public const string Link_EmailAllLeaders = "link=Email leaders";

                    public const string Link_Prospects = "link=Prospects";
                    public const string Link_Prospects_Numerical = "//a[contains(@href, '/GroupSoc/Prospects')]";

                    public const string Gear_GearMenu = "nav_gear";

                    public struct EmailAllLeaders {
                        public const string TextField_Subject = "subject";
                        public const string TextField_Message = "email_body";
                    }
                }

                public struct FindAGroupPage {
                    public const string TextField_ZipCode = "zipcode";
                    public const string Dropdown_Campus = "campus";
                    public const string Dropdown_Weekday = "weekday";
                    public const string Dropdown_StartTime = "start_time";
                    public const string Dropdown_Category = "category";
                    public const string Checkbox_ChildcareProvided = "childcare";
                }

                public struct IndividualProspectPage
                {
                    public const string Link_SendAnEmail = "css=a.task_send_email";
                    public const string Link_MakeAPhoneCall = "css=a.task_phone_call";
                    public const string Link_MeetFaceToFace = "css=a.task_face_to_face";
                    public const string Link_MakeAComment = "css=a.task_leave_comment";

                    public const string Link_SendAnEmail_WD = "Send an email";
                    public const string Link_MakeAPhoneCall_WD = "Make a phone call";
                    public const string Link_MeetFaceToFace_WD = "Met face-to-face";
                    public const string Link_MakeAComment_WD = "Leave a comment";

                    public const string TextField_Email = "email_message";

                    public const string TextField_Comment = "comment_message";

                    public const string TextField_MeetingInfo = "f2f_message";

                    public const string Link_PhoneAdd = "link=Add";
                    public const string Link_PhoneAdd_WD = "Add";
                    public const string TextField_PhoneNumber = "add_phone";
                    public const string DropDown_PhoneStatus = "phone_call_status";
                    public const string TextField_PhoneMessage = "phone_message";

                    public const string Button_SaveComment = "btn_save_comment";
                    public const string Button_SaveMeetin = "btn_save_face";
                    public const string Button_SavePhone = "btn_save_phone";
                    public const string Button_SendEmail = "btn_send_email";

                    public const string Link_AllowProspect = "button_allow";
                    public const string Link_DenyProspect = "button_deny";

                    public const string Button_ConfirmDenyProspect = "submit_deny";
                }

            }
        }

        public struct People {
            public struct ProfileEditorConstants {
                public const string TextField_BioArea = "profile";
                public const string TextField_AddressOne = "street1";
                public const string TextField_AddressTwo = "street2";
                public const string TextField_City = "city";
                public const string TextField_ZipCode = "postal_code";
                public const string TextField_County = "county";

                public const string DropDown_HouseholdPositon = "household_position";
                public const string DropDown_MaritalStatus = "marital_status";
                public const string DropDown_Gender = "gender";
                public const string DropDown_Country = "country_code";
                public const string DropDown_State = "state";
                public const string DropDown_Carrier = "carrier";

                public const string Link_AddAnotherWebsite = "link=Add another";
                public const string Link_EditProfile = "link=Update your profile";
                public const string Link_PrivacySettings = "link=Privacy settings";

                public const string DateControl_DateOfBirth = "dob";
            }

            public struct PrivacySettingsConstants {
                public const string Slider_Address = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[1]/td/div/a";
                public const string Slider_Birthdate = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[2]/td/div/a";
                public const string Slider_Email = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[3]/td/div/a";
                public const string Slider_Phone = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[4]/td/div/a";
                public const string Slider_Websites = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[5]/td/div/a";
                public const string Slider_Social = "//div[@id='content']/div/div[1]/form/table[1]/tbody/tr[6]/td/div/a";

                public const string Checkbox_IncludeInDirectory = "include_in_church_directory";
                public const string Button_Submit = "//input[@value='Save privacy settings']";
            }
        }
    }

    public struct TableIds {
        public const string Admin_ActivityTypes = "//table[@id='ctl00_ctl00_MainContent_content_dgActivityTypes']";
        public const string Admin_AttributeGroups = "//table[@id='ctl00_ctl00_MainContent_content_dgAttributeGroupNames']";
        public const string Admin_Buildings = "//table[@id='ctl00_ctl00_MainContent_content_dgBuildings']";
        public const string Admin_Campuses = "//table[@class='grid']";
        public const string Admin_ContactDispositions = "//table[@id='ctl00_ctl00_MainContent_content_dgContactDispositions']";
        public const string Admin_Departments = "//table[@id='ctl00_ctl00_MainContent_content_dgDepartments']";
        public const string Admin_FormNames = "//table[@class='grid']";
        public const string Admin_HeadCountAttributes = "//table[@class='grid']";
        public const string Admin_IndividualAttributes = "//table[@id='ctl00_ctl00_MainContent_content_dgIndividualAttributes']";
        public const string Admin_JobAttributeGroups = "//table[@id='ctl00_ctl00_MainContent_content_dgAttributeGroups']";
        public const string Admin_JobAttributes = "//table[@id='ctl00_ctl00_MainContent_content_dgJobAttributes']";
        public const string Admin_ManageItems = "//table[@id='ctl00_ctl00_MainContent_content_dgContactItems']";
        public const string Admin_PortalUsers = "//table[@id='ctl00_ctl00_MainContent_content_dgUsers']";
        public const string Admin_Rooms = "//table[@id='ctl00_ctl00_MainContent_content_dgRooms']";
        public const string Admin_Schools = "//table[@id='ctl00_ctl00_MainContent_content_dgSchools']";
        public const string Admin_VolunteerTypes = "//table[@id='ctl00_ctl00_MainContent_content_dgParticipantTypes']";
        public const string Admin_ApplicationKeys = "//table[@id='douglasP']";

        public const string Giving_Search = "//table[@id='ctl00_ctl00_MainContent_content_dgContributions']";
        public const string Giving_ContributorDetails_Scheduled = "//table[@id='ctl00_ctl00_MainContent_content_dgContributions']";
        public const string Giving_ScannedContribution = "//table[@id='batches_grid']";
        public const string Giving_Batches = "//table[@id='batches_grid']";
        public const string Giving_RemoteDepositCapture = "//table[@id='batches_grid']";
        public const string Giving_Unmatched_All = "//table[@id='unmatched_grid']";
        public const string Giving_Unmatched_Batches = "//table[@id='unmatched_grid']";
        public const string Giving_ImportedBatches = "//table[@id='ctl00_ctl00_MainContent_content_importedBatchesGrid']";
        public const string Giving_AccountReferences = "//table[@id='ctl00_ctl00_MainContent_content_dgAccountReference']";
        public const string Giving_Funds = "//table[@id='ctl00_ctl00_MainContent_content_dgFunds']";
        public const string Giving_SubFunds = "//table[@id='ctl00_ctl00_MainContent_content_dgSubFund']";
        public const string Giving_SubTypes = "//table[@id='ctl00_ctl00_MainContent_content_dgContributionType']";

        public const string Giving_ContributionAttributes = "//table[@id='ctl00_ctl00_MainContent_content_dgAttrs']";
        public const string Giving_Contributions = "//table[@id='repTable']";
        public const string Giving_Schedules = "//table[@class='grid']";
        public const string Giving_Pledges = "//table[@id='ctl00_ctl00_MainContent_content_dgPledges']";
        public const string Giving_Accounts = "//table[@class='grid']";
        public const string Giving_RDC_RDCBatchItemList = "//table[@id='ctl00_content_RDCBatchItemList_grdBatcheItems']";
        public const string Giving_RDC_PotentialMatches = "//table[@class='grid select_row']";
        //Some bozo removed the id in the tables so now using @class=grid
        //public const string Giving_RDC_IndividualMatches = "//table[@id='']";
        public const string Giving_RDC_IndividualMatches = "//table[@class='grid select_row']";

        public const string Groups_CustomFields = "//table[@id='_callback_zebra']";
        public const string Groups_Group_ActiveProspects = "//table[@id='_callback_open_prospects']";
        public const string Groups_Group_ClosedProspects = "//table[@id='_callback_closed_prospects']";
        public const string Groups_Group_AddIndividualToGroup = "//table[@class='grid']";
        //public const string Groups_Group_Members = "//table[@id='ctl00_ctl00_MainContent_content_grdGroupMembers']";
        public const string Groups_Group_Members = "ctl00_ctl00_MainContent_content_grdGroupMembers";
        public const string Groups_Group_GroupTypes = "//table[@class='grid faux_iphone']";
        public const string Groups_Group_Permissions = "//table[@id='group_permissions_table']";
        public const string Groups_Group_Edit_Details = "//table[@class='info']";
        public const string Groups_GroupTypes = "//table[@id='ctl00_ctl00_MainContent_content_grdGroupTypes']";
        //Some bozo removed the id in the tables so now using @class=grid
        //public const string Groups_GroupType_Permissions = "//table[@id='']";
        public const string Groups_GroupType_Permissions = "//table[@class='grid']";
        //public const string Groups_GroupTypes_CustomFields = "//table[@id='']";
        public const string Groups_GroupTypes_CustomFields = "//table[@class='grid']";
        //Some bozo removed the id in the tables so now using @class=grid
        //public const string Groups_GroupTypes_LandingPage_CustomFields = "//div[1]/div[2]/div[4]/table[@id='']";
        public const string Groups_GroupTypes_LandingPage_CustomFields = "//div[1]/div[2]/div[4]/table[@class='grid']";
        public const string Groups_GroupTypes_MoveGroup = "//table[@class='grid']";
        public const string Groups_SearchCategories = "//table[@class='grid']";
        public const string Groups_SearchCategories_Index = "//table[@id='ctl00_ctl00_MainContent_content_grdCategories']";
        public const string Groups_SearchCategories_Criteria = "//table[@class='grid']";
        //Some bozo removed the id in the tables so now using @class=grid
        //public const string Groups_SpanOfCare = "//table[@id='']";
        public const string Groups_SpanOfCare = "//table[@class='grid']";
        public const string Groups_SpanOfCare_All = "//table[@class='grid']";
        //Some bozo removed the id in the tables so now using @class=grid
        //public const string Groups_SpanOfCare_GroupTypes = "//*[@id='']";
        public const string Groups_SpanOfCare_GroupTypes = "//*[@class='grid']";
        public const string Groups_SpanOfCare_Campuses = "//table[@id='']";
        //public const string Groups_SpanOfCare_Owners = "//table[@id='' and tbody/tr[1]/th[position()=3 and normalize-space(text())='Name']]";
        public const string Groups_SpanOfCare_Owners = "//table[@class='grid' and tbody/tr[1]/th[position()=3 and normalize-space(text())='Name']]";
        //public const string Groups_SpanOfCare_GroupList = "//table[@id='' and tbody/tr[1]/th[position()=1 and normalize-space(text())='Groups']]";
        public const string Groups_SpanOfCare_GroupList = "//table[@class='grid' and tbody/tr[1]/th[position()=1 and normalize-space(text())='Groups']]";
        public const string Groups_ViewAll_GroupList_AllTab = "//table[@id='ctl00_ctl00_MainContent_content_ucGrpGrid_grdGroups']";
        public const string Groups_ViewAll_GroupList_GroupTab = "//table[@id='ctl00_ctl00_MainContent_content_ucGrpGrid_grdGroups']";

        public const string Groups_ViewAll_GroupList_SearchResults = "//table[@id='ctl00_ctl00_MainContent_content_grdGroups']";
        public const string Groups_ViewAll_PeopleList_PeopleListTab = "//table[@id='ctl00_ctl00_MainContent_content_ucGrpGrid_grdPeopleList']";
        public const string Groups_ViewAll_PeopleList_SearchResults = "//table[@id='ctl00_ctl00_MainContent_content_grdPeopleList']";
        public const string Groups_ViewAll_GroupList_SoCTab = "//div[@style='']/table[@id='']";
        public const string Groups_ViewAll_SavedSearches = "//table[@class='list full']";
        public const string Groups_ViewAll_ShowLeadersAndMembers_List = "//table[@id='ctl00_ctl00_MainContent_content_grdLeadersMembers']";
        public const string Groups_ViewAll_AttendanceDashboard = "//table[@id='attendance_table']";

        public const string Ministry_ActivityRoomSetup_Activities = "//table[@id='ctl00_ctl00_MainContent_content_dgActivity']";
        public const string Ministry_Activities_ViewAll = "//table[@id='activities_grid']";
        public const string Ministry_Activities_ViewAll_Requirements = "//table[@id='requirementTable']";
        public const string Ministry_Activities_ViewAll_Schedules = "//div[@id='activitySchedulesTableDiv']";
        public const string Ministry_ActivitySchedules = "//table[@id='ctl00_ctl00_MainContent_content_Listtimes1_activitySchedulesDataGrid']";
        public const string Ministry_ActivityRLCGroups = "//table[@class='grid increase_leading']";
        public const string Ministry_Activity_PropertiesTable = "//div[@id='sidebar']/div/table";
        public const string Ministry_Activities_BreakoutGroups = "//table[@id='breakoutGroupsTable']";
        public const string Ministry_Activity_CheckInTable = "//div[@id='sidebar']/div/table[2]";
        public const string Ministry_Assignments = "ctl00_ctl00_MainContent_content_dgIndividualPrefs";
        public const string Ministry_Assignments_ViewAll = "//table[@id='assignments_grid']";
        public const string Ministry_AutomatedAttendanceRules = "//table[@id='ctl00_ctl00_MainContent_content_dgAttendanceRules']";
        public const string Ministry_GroupFinderProperties = "//table[@id='ctl00_ctl00_MainContent_content_dgActivityProperties']";
        public const string Ministry_GroupFinderProperties_Choices = "//table[@id='ctl00_ctl00_MainContent_content_dgActivityChoice']";
        public const string Ministry_ManageStaffingAssignments = "//table[@class='info staffing']";
        public const string Ministry_PostAttendance_RLCListing = "//table[@id='ctl00_ctl00_MainContent_content_grdAttendanceHistory']";
        public const string Ministry_PostAttendance_SearchResults = "//table[@id='search_results_table']";
        public const string Ministry_PostAttendance_Attendees = "//table[@id='ctl00_ctl00_MainContent_content_dgResults']";
        public const string Ministry_Schedules = "//table[@class='grid']";
        public const string Ministry_NewSchedules = "//table[@class='grid padded_header']";
        public const string Ministry_Recurrences = "//table[@id='recurrences_listing']";
        public const string Ministry_SuperCheckins = "//table[@class='grid']";
        public const string Ministry_Themes = "//table[@id='ctl00_ctl00_MainContent_content_dgThemes']";
        public const string Ministry_BreakoutGroups = "//[@id='breakoutGroupsTable']";

        public const string People_Delegates = "//table[@id='ctl00_ctl00_MainContent_content_grdDelegates']";
        public const string People_Drafts = "//table[@id='ctl00_ctl00_MainContent_content_grdDrafts']";
        public const string People_Individuals = "//table[@id='ctl00_ctl00_MainContent_content_grdIndividuals']";
        public const string People_SentItems = "//table[@id='ctl00_ctl00_MainContent_content_grdSentItems']";
        public const string People_Templates = "//table[@id='ctl00_ctl00_MainContent_content_grdTemplates']";
        public const string People_GroupsWidget = "//table[@class='grid gutter_bottom_none']";
        public const string People_Individual_Requirements = "//table[@id='ind_requirements']";

        public const string WebLink_InFellowship_Links = "//table[@class='grid']";
        public const string WebLink_InFellowship_ChurchDirectory_MemberStatusTable = "//table[@id='member_table']";
        public const string WebLink_InFellowship_ChurchDirectory_AttendeeStatusTable = "//table[@id='attendee_table']";
        public const string WebLink_InFellowship_FormsTable = "//table[@id='ctl00_ctl00_MainContent_content_dgForms']";

        // Begin InFellowship tables...
        public const string InFellowship_FindAGroup_Results = "//table[@class='grid hidden-xs']";
        public const string InFellowship_ActiveProspects = "//table[@class='grid hidden-xs']";  //modify by grace zhang
        public const string InFellowship_SpanOfCareGroups = "//table[@class='grid full decrease']";
        public const string InFellowship_SpanOfCare_Prospects_GroupList = "//table[@id='prospects_by_group']";
        public const string InFellowship_ChurchDirectory = "//table[@class='grid']";
        public const string InFellowship_ChurchDirectory_IndividualInfoCard = "//table[@class='info_card']";
        public const string InFellowship_Groups_Roster = "//table[@class='grid align_middle hidden-xs']"; //Updated by Jim
        public const string InFellowship_Groups_Roster_Profile = "//table[@class='info_card']";
        public const string InFellowship_Groups_MatchedProspects = "//table[@class='grid']";
        public const string InFellowship_Group_Attendance = "//table[@class='grid']";
        public const string InFellowship_Group_Attendance_Roster_Responsive = "//table[@class='grid']";
        public const string InFellowship_Groups_Members_With_Attendance = "//table[@id='members_attendance']";
        public const string InFellowship_Groups_Members_Without_Attendance = "//table[@id='members_missing_attendance']";
        public const string InFellowship_GivingHistory = "//div[@id='giving_history']/table[@class='grid']";
        public const string InFellowship_GivingSchedules = "//table[@class='grid']";
        public const string InFellowship_GiveNowTable = "//ul[@id='giving_table']";
        public const string InFellowship_ContributionDetails_GiveNowWithoutAccount = "//table[@class='grid']";
        public const string InFellowship_Giving_FundSubFundAmount = "//ul[@id='giving_table']";
        public const string InFellowship_ScheduledGiving_FundSubFundAmount = "//table[@id='giving_table']";
        public const string InFellowship_ScheduleGivingTable = "//ul[@id='giving_table']";


        public struct Portal {            
            //public const string Giving_Batches = "ctl00_ctl00_MainContent_content_grdBatches";
            public const string Giving_Batches = "batches_grid";
            public const string Giving_ContributionAttributes = "ctl00_ctl00_MainContent_content_dgAttrs";
            public const string Giving_Funds = "ctl00_ctl00_MainContent_content_dgFunds";
            public const string Giving_PledgeDrives = "ctl00_ctl00_MainContent_content_dgPledgeDrives";
            public const string Giving_Search = "ctl00_ctl00_MainContent_content_dgContributions";
            public const string Giving_StatementQueue = "ctl00_ctl00_MainContent_content_dgStatements";
            public const string Giving_SubFunds = "ctl00_ctl00_MainContent_content_dgSubFund";
            public const string Giving_SubTypes = "ctl00_ctl00_MainContent_content_dgContributionType";
            public const string Giving_AccountReferences = "ctl00_ctl00_MainContent_content_dgAccountReference";
            public const string Giving_ImportedBatches = "ctl00_ctl00_MainContent_content_importedBatchesGrid";
            public const string Giving_All = "delete_batch_id";
            public const string Giving_Accounts = "//table[@class='grid']";

            public const string Groups_Group_Members = "ctl00_ctl00_MainContent_content_grdGroupMembers";
            public const string Groups_ViewAll_GroupList_TemporaryTab = "ctl00_ctl00_MainContent_content_ucGrpGrid_grdTemporary";

            public const string Ministry_Volunteers_StaffingAssignments = "//table[@id='']";
            

            public const string People_PeopleQuery = "ctl00_ctl00_MainContent_content_grdIndividuals";
            public const string People_PeopleSearchModal = "//div[@id='search_results_inner']/table";
            public const string People_DuplicateQueue = "ctl00_ctl00_MainContent_content_dgMergeQueue";
            public const string People_Individuals = "ctl00_ctl00_MainContent_content_grdIndividuals";
            public const string People_Individual_Requirements = "ind_requirements";

            public const string WebLink_ManageForms = "ctl00_ctl00_MainContent_content_dgForms";
            public const string WebLink_AggregateForms = "ctl00_ctl00_MainContent_content_dgForms";
            public const string WebLink_ViewSubmissions = "ctl00_ctl00_MainContent_content_dgIndividualForms";
            public const string WebLink_ViewSubmissions_AddPaymentRefund = "ctl00_ctl00_MainContent_content_dgPayments";
            public const string WebLink_Validation_Summary = "ValidationSummary";

        }

        public struct Weblink {
            public const string ContributionSchedule = "dgSchedule";
        }
            public struct EventRegistration
            {
              
        }

        public struct ReportLibrary {
            public const string LabelStyles = "ctl00_MainContent_grdLabelStyles";
            public const string Queue = "ctl00_MainContent_grdReports";
            public const string Myreport = "ctl00_ctl00_MainContent_LeftColumn_grdReports";
        }
    }

    public struct GeneralPeople
    {
        public struct IndividualEdit
        {
            public const string Prefix = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtPrefix";
            public const string FirstName = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFirstName";
            public const string MiddleName = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMiddleName";
            public const string LastName = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtLastName";
            public const string Suffix = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtSuffix";
            public const string HouseholdPosition = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlHsdPosition";
            public const string GenderDropDown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlGender";
            public const string MaritalStatusDropDown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlMaritalStatus";
            public const string BirthDate = "_dob_control_";
            public const string StatusDropdown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlStatus";
            public const string SubStatusDropDown = "ddlSubStatus";
            public const string StatusDate = "_status_control_";
            public const string StatusComment = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtStatusComment";
            public const string FaceBook = "ctl00_ctl00_MainContent_content_ctlIndividualFull_facebook";
            public const string Linkedin = "ctl00_ctl00_MainContent_content_ctlIndividualFull_linkedin";
            public const string Twitter = "ctl00_ctl00_MainContent_content_ctlIndividualFull_twitter";
            public const string SaveButton = "ctl00_ctl00_MainContent_content_btnSave";
            
            #region More Fields
            public const string MoreFieldsLink = "More fields";
            public const string Title = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTitle";
            public const string GoesByName = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtGoesByName";
            public const string FormerName = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerName";
            public const string Employer = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtEmployer";
            public const string OccupationDropDown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlOccupation";
            public const string OccupationDescription = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtOccupationDesc";
            public const string FormerDenominationDropDown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlDenomination";
            public const string FormerChurch = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtFormerChurch";
            public const string SchoolDropDown = "ctl00_ctl00_MainContent_content_ctlIndividualFull_ddlSchool";
            public const string BarCode = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtBarCode";
            public const string TagComment = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtTagComment";
            public const string MemberEnvNumber = "ctl00_ctl00_MainContent_content_ctlIndividualFull_txtMemberEnvNo";
            
            #endregion More Fields
        }
        public struct PeopleQuery
        {
            public const string Check_All = "checkAll";
        }
    }

    public struct GeneralAdmin
    {
        public struct ApplicationKeys
        {
            public const string Edit_Application_Name = "application_name";
            public const string Edit_Version = "application_version";
            public const string Edit_Description = "intended_use";
            public const string Edit_Contact_Email = "contact_email";
            public const string Edit_Home_Page_URI = "home_page_uri";
            public const string Edit_Download_URI = "download_uri";
            public const string Edit_Callback_URI = "callback_uri";
            public const string Edit_Save_Button = "submitQuery";
            public const string Back_Button = "tab_back";
        }

    }

    public struct GeneralInFellowship
    {
        public struct Giving
        {
            public const string Fund = "fund";
            public const string SubFund = "sub_fund";
            public const string Amount = "amount";
            public const string Contributor_FirstName = "giver_first_name";
            public const string Contributor_LastName = "giver_last_name";
            public const string Contributor_Email = "giver_email";
            public const string PersonalCheck_Bullet = "payment_method_check";
            public const string CreditCard_Bullet = "payment_method_cc";
            //public const string PhoneNumber = "phone";
            public const string PhoneNumber = "//div[@data-toggle-target='check']/fieldset/div/div/div/div/input[@id='phone']";
            //public const string RoutingNumber = "routing_number";
            public const string RoutingNumber = "//div[@data-toggle-target='check']/fieldset/div/div/div[2]/div/input[@id='routing_number']";
            //public const string AccountNumber = "account_number";
            public const string AccountNumber = "//div[@data-toggle-target='check']/fieldset/div/div/div[3]/div/input[@id='account_number']";
            public const string Cardholder_FirstName = "FirstName";
            public const string Cardholder_LastName = "LastName";
            public const string CreditCard_Type = "payment_type_id";
            public const string CreditCard_Number = "cc_account_number";
            public const string CreditCard_ExpireMonth = "expiration_month";
            public const string CreditCard_ExpireYear = "expiration_year";
            public const string CreditCard_SecurityCode = "CVC";
            public const string CreditCard_ValidFromMonth = "valid_from";
            public const string CreditCard_ValidFromYear = "valid_from_year";
            public const string Country = "Country";
            public const string AddressLine1 = "Address1";
            public const string AddressLine2 = "Address2";
            public const string City = "City";
            public const string State = "state";
            public const string StProvince = "StProvince";
            public const string ZipCode = "PostalCode";
            public const string County = "County";

            public const string GIVE_NOW = "GIVE NOW";
            public const string Add_Another = "add_row";
            public const string Continue = "//button[@class='btn btn-primary btn-lg next hidden-xs']";
            public const string Continue_Responsive = "//button[@class='btn btn-primary btn-md next visible-xs']";
            public const string Back = "« Back";
            public const string Captcha = "VerificationCaptcha_CaptchaDiv";
            public const string CreateAnAccount_CheckBox = "checkbox";
            public const string Process_Payment = "//button[@class='btn btn-primary btn-lg next hidden-xs']";
            public const string Process_Payment_Responsive = "//button[@class='btn btn-primary btn-md next visible-xs']";
            public const string Cancel = "Cancel";
            public const string Payment_Succes_Message = "success_message_inner";
            public const string Error_Message = "//div[@class='error_msgs_for']";
            public const string Edit_ContributionDetails = "//div[@class='col-xs-12']/table[1]/thead/tr/th/a[@class='float_right normal' and text()='Edit']";
            public const string Edit_PaymentInformation = "//div[@class='col-xs-12']/table[2]/tbody/tr/th/a[@class='float_right normal' and text()='Edit']";
            public const string GivingHistory_View = "submit";
            public const string f = "£";

        }

        public struct EventRegistration
        {
            public const string FormName = "page_header";

            #region Select Individual page
            public const string SelectAddPersonButton = "//li[@id='add_person']/a";
            public const string SelectViewGuests = "expand_link";
            public const string ChurchLogo = "Dynamic Church";
            public const string Instance = "selectedScheduleOrInstanceId";
            public const string ContinueButton = "continue";
            public const string ResponsiveContinueButton = "continue";
            public const string SubmitPayment = "submitPayment";
            public const string ResponsiveSubmitPayment = "submitPayment";

            #region Modals
            public const string AddModalFirstName = "first_name";
            public const string AddModalLastName = "last_name";
            public const string AddModalDOB = "dob";
            public const string AddModalMaritalStatus = "marital_status";
            public const string AddModalGender = "gender";
            public const string AddModalHouseholdPosition = "household_member_type";
            public const string ModalSave = "//button[@class='btn btn-primary']";


            #endregion Modals

            #endregion Select Individual page
            #region ErrorMessages
            public struct TextValidationErrorMessage
        {
            public const string Question_Required = "An answer for this question is required";
            public const string URL = "Please enter only valid URL";
            public const string Alpha = "Please enter only letters";
            public const string Alphanumeric = "Please enter only letters and numbers";
            public const string CurrencyPlus = "Please enter a positive currency amount";
            public const string CurrencyMinus = "Please enter a valid currency amount";
            public const string DateTime = "Please enter a valid date, time or date and time";
            public const string Email = "Please enter a valid email address";
            public const string IntegerPlus = "Please enter a non-zero positive whole number";
            public const string IntegerMinus = "Please enter only numbers";
            public const string IntergerZero = "Please enter a positive whole number";
            public const string MultiEmail = "Please enter only valid email addresses";
            public const string Phone = "Please enter a valid phone number";
            public const string PhoneACH = "Please enter a valid phone number";
        }
        #endregion Error Messages */ 
            #region Payments
            public const string CC_Payment_Method = "payment_method_cc";
            public const string CC_FirstName = "cc_first_name";
            public const string CC_LastName = "cc_last_name";
            public const string CC_Type = "payment_type_id";
            public const string CC_Number = "cc_account_number";
            public const string CC_Expiration_Month = "expiration_month";
            public const string CC_Expiration_Year = "expiration_year";
            public const string CC_Security_Code = "cvc";
            public const string Echeck_Payment_Method = "payment_method_echeck";
            public const string CCPrevious_Payment_Method = "payment_method_previous_cc";
            public const string Cash_Payment_Method = "payment_method_cash";
            public const string Check_Payment_Method = "payment_method_check";
            public const string Echeck_FirstName = "echeck_first_name";
            public const string Echeck_LastName = "echeck_last_name";
            public const string BankRouting_Number_Name = "routing_number";
            public const string Echeck_AccountNumber = "account_number";
            public const string Echeck_PhoneNumber = "echeck_phone_number";
            
            #endregion Payments

        }
        
        public struct Account
        {
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string LoginEmail = "emailaddress";
            public const string Password = "password";
            public const string ConfrimPassword = "confirmpassword";
            public const string CreateAccountButton = "submit";
            public const string DateOfBirth = "dob";
            public const string MaleRadio = "//input[@name='gender' and @value='Male']";
            public const string FemaleRadio = "//input[@name='gender' and @value='Female']";
            public const string CountryDropDown = "country_code";
            public const string Address1 = "street1";
            public const string Address2 = "street2";
            public const string City = "city";
            public const string StateDropDown = "state";
            public const string ZipCode = "postal_code";
            public const string County = "county";
            public const string HomePhone = "home_phone";
            public const string MobilePhone = "mobile_phone";
            public const string SaveAccountButton = "submitQuery";
            public const string UpdateLoginEmail = "NewEmailAddress";
            public const string UpdateConfirmEmail = "EmailAddressConfirm";
            public const string NewPassword = "NewPassword";
            public const string NewPasswordConfirm = "PasswodConfirm";
            public const string CurrentPassword = "CurrentPassword";
            public const string UpdateSaveButton = "btn_save";

        }
    }
    
    public struct GeneralMinistry
    {
        public struct Activities
        {
            #region View All
            //public const string Add_Link = "add_activity_button";
            public const string Reset_Link = "Reset";
            public const string Apply_Button = "//button[@type='submit'] [text()='Apply']";
            public const string Delete_Button = "//img[@class='uiClose']";

            public const string Activity_Name = "activity_name";
            public const string Ministry_DropDown = "show_ministry";
            
            public const string Activity_Type_Filter = "//a[@data-toggle='#activity_type_wrapper'] [text()='Activity Type']";
            public const string Activity_Type_DropDown = "//select[@name='activityType']";

            public const string Occurence = "//a[@data-toggle='#occurs_wrapper'] [text()='Occurence']";
            public const string Mon_CheckBox = "day_of_week_0";
            public const string Tue_CheckBox = "day_of_week_1";
            public const string Wed_CheckBox = "day_of_week_2";
            public const string Thu_CheckBox = "day_of_week_3";
            public const string Fri_CheckBox = "day_of_week_4";
            public const string Sat_CheckBox = "day_of_week_5";
            public const string Sun_CheckBox = "day_of_week_6";
            public const string DateRange_DropDown = "//select[@name='date_range']";
            public const string DateFrom = "date_from";
            public const string DateTo = "date_to";

            public const string Age_Range = "//a[@data-toggle='#age_range_wrapper'] [text()='Age Range']";
            public const string Age_Range_Min_DropDown = "age_range_min";
            public const string Age_Range_Max_DropDown= "age_range_max";

            public const string Location = "//a[@data-toggle='#location_wrapper'] [text()='Location']";
            public const string Buildings_DropDown = "//select[@name='buildingsFilter']";
            public const string Rooms_DropDown = "//select[@name='roomsFilter']";
            
            public const string Confidential = "//a[@data-toggle='#confidential_wrapper'] [text()='Confidential']";
            public const string Confidential_Checkbox = "confidential_yes";
            public const string Normal_Checkbox = "confidential_no";
            
            public const string Check_In_Enabled = "//a[@data-toggle='#checkin_wrapper'] [text()='Check-in Enabled']";
            public const string Check_In_Enabled_CheckBox = "checkin_yes";
            public const string Check_In_Code = "checkin_code";
            public const string Check_In_NotEnabled_CheckBox = "checkin_no";

            public const string Active = "//a[@data-toggle='#active_wrapper'] [text()='Active']";
            public const string Active_CheckBox = "active_yes";
            public const string Inactive_CheckBox = "active_no";

            public const string More_Button = "load_button";

            #endregion View All

            #region Activity Side Bar

            public const string Pop_Over_Module = "//div[@class='popover fade left in']";
            public const string Pop_Over_Title = "//h3[@class='popover-title']";
            public const string Pop_Over_Text = "//div[@class='popover-content']";

            public const string Add_Activity = "add_activity_button";
            public const string View_Assignments = "//div[@id='sidebar']/div/ul/li[1]/a";
            public const string Manage_Staffing_Needs = "//div[@id='sidebar']/div/ul/li[2]/a";
            //public const string Delete_This_Activity = "//div[@id='sidebar']/div/ul/li[4]/a";

            public const string Properties_Edit = "//div[@id='sidebar']/div/h6[2]/a";
            public const string Check_In_Settings_Edit = "//div[@id='sidebar']/div/h6[3]/a";
            public const string Check_In_Settings_Renew = "renew_checkincode";

            #endregion Activity Side Bar

            #region Add Activity

            public const string Ministry = "ministry";
            public const string Activity = "activity_name";
            public const string Description = "description";
            public const string Is_Confidential = "is_confidential";
            public const string Activity_Type = "activity_type";
            public const string EnableCheckin_Box = "enable_checkin";
            public const string PrintNameTag_Box = "print_name_tag";
            public const string PrintParentReceipt_Box = "print_receipt";
            public const string AllowedClosedRoom_Box = "assignments_override";
            public const string RequireAssignment_Box = "require_assignment";
            public const string RequireAssignment_Prevent = "activity_ar_always";
            public const string RequireAssignment_Alert = "activity_ar_optional";
            public const string AgeRange_From = "age_range_from";
            public const string AgeRange_To = "age_range_to";
            public const string ParticipantAssignment_Schedule = "assignment_participants_schedule";
            public const string ParticipantAssignment_DateTime = "assignment_participants_datetime";
            public const string ParticipantAssignment_Always = "assignment_participants_always";
            public const string StaffAssignment_Schedule = "assignment_staff_schedule";
            public const string StaffAssignment_DateTime = "assignment_staff_datetime";
            public const string StaffAssignment_Always = "assignment_staff_always";
            public const string AutoCreation_FirstAttendance = "assignment_update_first";
            public const string AutoCreation_LastAttendance = "assignment_update_last";
            public const string UseAsWebLinkGroups_Box = "use_as_wl_groups";
            public const string WebLink_Contact_Item = "contact_item";
            public const string Activity_Schedule_Name = "schedule_name";
            public const string Activity_StartTime = "start_time";
            public const string Activity_EndTime = "end_time";
            public const string ScheduleRecurrence_Once = "frequency_0";
            public const string ScheduleRecurrence_Daily = "frequency_1";
            public const string ScheduleRecurrence_Weekly = "frequency_2";
            public const string ScheduleRecurrence_Monthly = "frequency_3";
            public const string ScheduleRecurrence_Yearly = "frequency_4";
            public const string RecurrenceOnce_StartDate = "once_start_date_from";
            public const string RecurrenceDaily_StartDate = "daily_start_date_from";
            public const string RecurrenceDaily_DailyPattern = "daily_pattern_0";
            public const string RecurrenceDaily_WeekdayPattern = "daily_pattern_1";
            public const string RecurrenceDaily_EveryNthDay = "daily_nth_day";
            public const string RecurrenceDaily_Ending_Box = "daily_ends";
            public const string RecurrenceDaily_EndsDate_Option = "daily_ends_date";
            public const string RecurrenceDaily_EndDate = "recurrence_daily_ending_date";
            public const string RecurrenceDaily_EndsOccurrence_Option = "daily_ends_nth";
            public const string RecurrenceDaily_EndsNthTimes = "daily_ends_nth";
            public const string RecurrenceWeekly_StartDate = "weekly_start_date_from";
            public const string RecurrenceWeekly_EveryNthWeek = "weekly_nth_week";
            public const string RecurrenceWeekly_Sunday = "weekly_sunday";
            public const string RecurrenceWeekly_Monday = "weekly_monday";
            public const string RecurrenceWeekly_Tuesday = "weekly_tuesday";
            public const string RecurrenceWeekly_Wednesday = "weekly_wednesday";
            public const string RecurrenceWeekly_Thursday = "weekly_thursday";
            public const string RecurrenceWeekly_Friday = "weekly_friday";
            public const string RecurrenceWeekly_Saturday = "weekly_saturday";
            public const string RecurrenceWeekly_Ending_Box = "weekly_ends";
            public const string RecurrenceWeekly_EndsDate_Option = "weekly_ends_date";
            public const string RecurrenceWeekly_EndDate = "recurrence_weekly_ending_date";
            public const string RecurrenceWeekly_EndsOccurrence_Option = "weekly_ends_nth";
            public const string RecurrenceWeekly_EndsNthTimes = "weekly_ends_nth";
            public const string RecurrenceMonthly_StartDate = "monthly_start_date_from";
            public const string RecurrenceMonthly_NthDayOfMonth = "monthly_0";
            public const string RecurrenceMonthly_MonthlyNthDay = "monthly_nth_day";
            public const string RecurrenceMonthly_MonthlyNthMonth = "monthly_nth_month";
            public const string RecurrenceMonthly_SpecificWeekDateOfMonth = "monthly_1";
            public const string RecurrenceMonthly_SpecificWeekDateNthDay = "monthly_ordinal_nth";
            public const string RecurrenceMonthly_SpecificWeekDateWeekDay = "monthly_weekday";
            public const string RecurrenceMonthly_SpecificWeekDateNthMonth = "monthly_nth_month_2";
            public const string RecurrenceMonthly_Ending_Box = "monthly_ends";
            public const string RecurrenceMonthly_EndsDate_Option = "monthly_ends_date";
            public const string RecurrenceMonthly_EndDate = "recurrence_monthly_ending_date";
            public const string RecurrenceMonthly_EndsOccurrence_Option = "monthly_ends_nth";
            public const string RecurrenceMonthly_EndsNthTimes = "monthly_ends_nth";
            public const string RecurrenceYearly_StartDate = "yearly_start_date_from";
            public const string RecurrenceYearly_SameDayEveryYear = "yearly_day";
            public const string RecurrenceYearly_SpecificDayOfYear = "yearly_nth";
            public const string RecurrenceYearly_SpecificNthWeek = "yearly_ordinal_nth";
            public const string RecurrenceYearly_SpecificDayOfWeek = "yearly_weekday";
            public const string RecurrenceYearly_SpecificMonthOfYear = "yearly_nth_month";
            public const string RecurrenceYearly_Ending_Box = "yearly_ends";
            public const string RecurrenceYearly_EndsDate_Option = "yearly_ends_date";
            public const string RecurrenceYearly_EndDate = "recurrence_yearly_ending_date";
            public const string RecurrenceYearly_EndsOccurrence_Option = "yearly_ends_nth";
            public const string RecurrenceYearly_EndsNthTimes = "yearly_ends_nth";
            
            
            public const string Error_Message = "error_message";
            public const string Next_Step = "commit";
            public const string Create_Activity = "create_activity";


            #endregion Add Activity

            #region Activity Schedules

            public const string Activity_Schedule_Add = "Add Schedule";
            public const string Activity_Schedule_Past_CheckBox = "filterOptionPast";
            public const string Activity_Schedule_Current_CheckBox = "filterOptionCurrent";
            public const string Activity_Schedule_Future = "filterOptionFuture";

            #endregion Activity Schedules

            #region Properties Tab
            public const string Add_Requirement = "Add";
            public const string Active_RequirementsCheckBox = "show_active";
            public const string Inactive_Requirements_CheckBox = "show_inactive";
            public const string Requirements_Schedule_DropDown = "schedulesDropDown";
            public const string Requirement_Name_Input = "RequirementName";
            public const string Requirement_Delete_Confirm = "ConfirmDelete";
            public const string Requirement_Delete_Cancel = "Don't delete";
            public const string Requirement_Save_Button = "//button[@type='submit'][text()='Save requirement']";
            public const string Requirement_Save_And_Add_Button = "//button[@type='submit'][text()='Save and add another']";
            public const string Requirement_Save_Cancel = "Cancel";
            

            #endregion Properties Tab

            public const string UI_Collapsed = "uiToggle";
            public const string UI_Expanded = "uiToggle uiToggleExpanded";
            public const string Im_Done = "commit";
            public const string Check_Icon = "/portal/images/check.gif";
            public const string UnCheck_Icon = "/portal/images/close.gif";

            #region Activity Settings
            public const string Activity_Active = "active_activity";
            public const string Activity_Inactive = "inactive_activity";
            public const string Activity_Description = "description";
            public const string Activity_IsConfidential = "is_confidential";
            public const string EditActivity_Type = "activity_type";
            public const string Activity_EnableCheckin = "enable_checkin";
            public const string Activity_PrintNameTag = "print_name_tag";
            public const string Activity_PrintParentReceipt = "print_receipt";
            public const string Activity_AssignmentOverride = "assignments_override";
            public const string Activity_RequireAssignment = "require_assignment";
            public const string Activity_RequirePrevent = "activity_ar_always";
            public const string Activity_RequireAlert = "activity_ar_optional";
            public const string Activity_EnableAgeRestrictions = "enable_age_restrictions";
            public const string Activity_AgeRangeFrom = "age_range_from";
            public const string Activity_AgeRangeTo = "age_range_to";
            public const string Activity_EnforceAssignmentCreation = "enforce_assignment";
            public const string Activity_EnforceAssignmentParticipants = "enforce_participants";
            public const string Activity_EnforceAssignmentParticipantSchedule = "assignment_participants_schedule";
            public const string Activity_EnforceAssignmentParticipantDateTime = "assignment_participants_datetime";
            public const string Activity_EnforceAssignmentParticipantAlways = "assignment_participants_always";
            public const string Activity_EnforceAssignmentVolStaff = "enforce_staff";
            public const string Activity_EnforceAssignmentVolStaffSchedule = "assignment_staff_schedule";
            public const string Activity_EnforceAssignmentVolStaffDateTime = "assignment_staff_datetime";
            public const string Activity_EnforceAssignmentVolStaffAlways = "assignment_staff_always";
            public const string Activity_AutoCreateAssignment = "auto_update_assignments";
            public const string Activity_AutoCreateFirstAttendance = "assignment_update_first";
            public const string Activity_AutoCreateLastAttendance = "assignment_update_last";
            public const string Activity_UseAsWebLinkGroups = "use_as_wl_groups";
            public const string Activity_WebLinkContactItem = "contact_item";
            public const string Activity_PropertiesTable = "//div[@id='sidebar']/div/table";
            public const string Activity_CheckInTable = "//div[@id='sidebar']/div/table[2]";
            #endregion Activity Settings

            #region Activity Schedule
            public const string Schedule_Error_Message = "//dl[@id='error_message']";
            #endregion Activity Schedule

            #region Add/Edit Roster
            public const string Roster_Name = "activity_name";
            public const string Roster_Active = "active_activity";
            public const string Roster_Inactive = "inactive_activity";
            public const string Roster_Folder = "roster_folder";
            public const string Roster_Building = "building_room";
            public const string Roster_Capacity = "roster_capacity";
            public const string Roster_StartAsClosed = "start_as_closed";
            public const string Roster_UseRecommendedAges = "enable_age_restrictions";
            public const string Roster_SetAgeRange = "roster_age";
            public const string Roster_StartAge = "start_age_range";
            public const string Roster_EndAge = "end_age_range";
            public const string Roster_AgeRangeType = "month_year_select";
            public const string Roster_SetBirthdateRange = "roster_birth_date_range";
            public const string Roster_Birthdate_Start = "birth_date_start";
            public const string Roster_Birthdate_End = "birth_date_end";
            public const string Roster_HiddenInSmallGroupFinder = "roster_hidden_in_activity_finder";
            public const string Roster_ShareContactsWithGroup = "roster_share_contacts_with_groups";
            public const string Roster_Save = "commit";
            public const string Roster_SaveAndAddAnother = "commit_again";
            #endregion Add/Edit Roster

            #region Add/Edit Roster Folders
            public const string RosterFolder_Add = "Add Folder";
            public const string RosterFolder_Name = "roster_folder_name";
            public const string RosterFolder_Save = "submitQuery";
            #endregion Add/Edit Roster Folders

            #region Breakout Groups
            public const string BreakoutGroups_Add = "Add Breakout Group";
            public const string BreakoutGroups_Save = "btn";
            public const string BreakoutGroups_SaveAddAnother = "submit";
            public const string BreakoutGroups_Name = "breakout_group_name";
            public const string BreakoutGroups_Description = "breakout_group_description";
            public const string BreakoutGroups_TagCode = "breakout_group_tag_code";
            public const string BreakoutGroups_Gear_Edit = "Edit";
            public const string BreakoutGroups_Gear_ViewAssignments = "View Assignments";
            public const string BreakoutGroups_Gear_Delete = "Delete";
            public const string BreakoutGroups_Delete = "ConfirmDelete";
            #endregion Breakout Groups

        }

        public struct PostAttendance
        {
            public const string StaffAssignment_Radio = "search_staff";
            public const string ParticipantAssignment_Radio = "search_participant";
            public const string SearchAll_Radio = "search_all";
            public const string SearchAll_TextBox = "ctl00_ctl00_MainContent_content_basicsearch";
            public const string PostAttendance_Button = "post_attendance";
            public const string Search_Button = "ctl00_ctl00_MainContent_content_btnSearch";
            public const string MinistryName = "active_ministry_name";
            public const string Ministry_DropDown = "ctl00_ctl00_MainContent_ddlMinistryTemplateSelection";
            public const string Activity_DropDown = "ctl00_ctl00_MainContent_content_ddlActivity_dropDownList";
            public const string Group_DropDown = "ctl00_ctl00_MainContent_content_ddlActivityGroup_dropDownList";
            public const string Schedule_DropDown = "ctl00_ctl00_MainContent_content_ddlSchedule_dropDownList";
            public const string DateRange = "ctl00_ctl00_MainContent_content_dtMYRange";
            public const string Time_DropDown = "ctl00_ctl00_MainContent_content_ddlActivityTime_dropDownList";
            public const string Action_DropDown = "ctl00_ctl00_MainContent_content_ddlAction_dropDownList";



        }

        public struct AutoAttendanceRules
        {
            public const string HH_Head = "ctl00_ctl00_MainContent_content_chkBoxHouseholdHead";
            public const string HH_Spouse = "ctl00_ctl00_MainContent_content_chkBoxHouseholdSpouse";
            public const string HH_Child = "ctl00_ctl00_MainContent_content_chkBoxHouseholdChild";
            public const string Contributor_Spouse = "ctl00_ctl00_MainContent_content_chkBoxContributorSpouse";
            public const string Contributor_Children = "ctl00_ctl00_MainContent_content_chkBoxContributorChildren";
            public const string Child_Parents = "ctl00_ctl00_MainContent_content_chkBoxChildParents";
            public const string Child_Siblings = "ctl00_ctl00_MainContent_content_chkBoxSiblings";
        }

        public struct Assignments
        {

            public const string Reset_Link = "Reset";
            public const string Add_Assignment = "//div[@id='sidebar']/div/div/ul/li/a";
            public const string Apply_Button = "//button[@type='submit'] [text()='Apply']";
            public const string Delete_Button = "//img[@class='uiClose']";
            public const string Table_Results = "all_assign_wrapper";
            
            public const string Roles = "//a[@data-toggle='#roleCriteriaSection'] [text()='Roles']";
            public const string Participant_Checkbox = "showParticipants";
            public const string Volunteer_Checkbox = "showActiveStaff";
            public const string Inactive_Staff_Checkbox = "showInactiveStaff";

            public const string Ministry = "ministry_dropdown_-1";
            public const string Ministry_DropDown = "ministry_dropdown_";
            public const string Ministry_Loading_Image = "show_ministry_loading";

            public const string Activity = "activity_dropdown_-1";
            public const string Activity_Dropdown = "activity_dropdown_";
            public const string Activity_Loading_Image = "show_activity_loading";

            public const string Rosters = "//a[@data-toggle='#rosterCriteriaSection'] [text()='Rosters / Breakout Groups']";
            public const string Roster_Folders_Dropdown = "activityGroup_dropdown_";
            public const string Roster_DropDown = "activityDetail_dropdown_";
            public const string Breakout_Group_DropDown = "individualGroup_dropdown_";
            public const string Roster_Folder_Loading_Image = "show_activityGroup_loading";

            public const string Activity_Schedules = "//a[@data-toggle='#activityTimeCriteriaSection'] [text()='Activity Schedules']";
            public const string Activity_Schedules_DropDown = "activityTime_dropdown_";
            public const string Activity_Schedules_Loading_Image = "show_activityTime_loading";

            public const string Date_Time = "//a[@data-toggle='#activityDateTimesCriteriaSection'] [text()='Dates & Times']";
            //public const string Date_Range_DropDown = "date_range";
            public const string Date_From = "dateTimesDateFrom";
            public const string Date_To = "dateTimesDateTo";
            public const string DateTime_DropDown = "activityInstance_dropdown_";
            public const string Date_Time_Loading_Image = "activityDateTimesCriteriaSection";

            public const string Jobs = "//a[@data-toggle='#jobCriteriaSection'] [text()='Volunteer / Staff Jobs']";
            public const string Jobs_DropDown = "job_dropdown_";

            public const string Staff_Schedule = "//a[@data-toggle='#staffingScheduleCriteriaSection'] [text()='Volunteer / Staff Schedules']";
            public const string Staff_Schedule_DropDown = "staffingSchedule_dropdown_";
            
            public const string DateOfBirth = "//a[@data-toggle='#birthDatesCriteriaSection'] [text()='Date of Birth / Age']";
            public const string DateOfBirth_Named_Date = "birthDateRange";
            public const string DateOfBirth_Start_Date = "birthDateDateFrom";
            public const string DateOfBirth_End_Date = "birthDateDateTo";
            public const string DateOfBirth_Age_Start = "age_from";
            public const string DateOfBirth_Age_End = "age_to";
            public const string DateOfBirth_Age_Type = "age_type";
            public const string DateOfBirth_Date_Radio = "birthdate_type_dates";
            public const string DateOfBirth_Age_Radio = "birthdate_type_ages";

            public const string Success_Message = "success_middle_messages";
            public const string More_Button = "load_button";
            public const string UI_Collapsed = "uiToggle";
            public const string UI_Expanded = "uiToggle uiToggleExpanded";
            public const string No_Assignments_Found = "//table[@id='assignments_grid']/tbody/tr/td[@style='text-align: center;'] [text()='No assignments found ']";
            public const string Tab_Back_Arrow = "tab_back";

            #region Actions
            public const string Select_All_CheckBox = "//input[@data-grid-action='check-all'] [@name='checkAll']";
            public const string Add_To_Group_DropDown = "addToGroupDropdown";
            public const string Add_To_Group_Modal = "add_to_group_modal";
            public const string Add_To_Group_Modal_Who_DropDown = "assign_to_who";
            public const string Add_To_Group_Modal_New_Radio_Button = "new_group";
            public const string Add_To_Group_Modal_Existing_Radio_Button = "existing_group";
            public const string Add_To_Group_Modal_Type_DropDown = "type";
            public const string Add_To_Group_Modal_Group_DropDown = "group";
            public const string Add_To_Group_Modal_Name = "group_name";
            public const string Add_To_Group_Modal_Save = "addToGroupSubmit";
            public const string Add_To_Group_Modal_Exit = "fancybox-close";
            public const string Delete_DropDown = "DeleteButton";
            public const string Email = "massEmail";
            #endregion Actions

            #region Add To Group Options
            public const string Add_To_Assigned = "Add assigned to group";
            public const string Add_To_Parents = "Add parents to group";
            public const string Add_To_FamilyHousehold = "Add family / household to group";
            public const string Add_To_Children = "Add children to group";
            public const string Add_To_Group_Error_Ok = "//div[@class='modal_actions gutter_top']/button";
            #endregion Add To Group Options

        }

        public struct Activity
        {

            public struct Property
            {

                public const string Ministry = "Ministry";
                public const string Activity_Type = "Activity Type";
                public const string Age = "Age";
                public const string Confidential = "Confidential";            
                public const string Volunteers_Staff = "Volunteers/Staff";                
                public const string Participants = "Participants";                
                public const string Auto_Assignment = "Auto Assignment";                
                public const string Weblink_Groups = "WebLink Groups\r\nContact Item";
                public const string Active = "Active";
                public const string Yes = "Yes";
                public const string No = "No";

            }

            public struct CheckIn
            {


            }

        }
    }

    public struct GeneralWebLink
    {
        public struct Forms
        {
            public const string Weblink_CreateForm_Restrictions_Submissions = "ctl00_ctl00_MainContent_content_txtMaxCapacity_textBox";
            public const string Weblink_CreateForm_Restrictions_StartDate = "ctl00_ctl00_MainContent_content_ctl01_DateTextBox";
            public const string Weblink_CreateForm_Restrictions_EndDate = "ctl00_ctl00_MainContent_content_ctl02_DateTextBox";
            public const string Weblink_CreateForm_Restrictions_DateRangeRadio = "ctl00_ctl00_MainContent_content_rdbtnDateRestriction";
            public const string Weblink_CreateForm_Restrictions_GenderDropDown = "ctl00_ctl00_MainContent_content_ddlGenderRestriction_dropDownList";
            public const string Weblink_CreateForm_Restrictions_GenderRadio = "ctl00_ctl00_MainContent_content_rdbtnGenderRestriction";
            public const string Weblink_CreateForm_Restrictions_StartAge = "ctl00_ctl00_MainContent_content_txtLowAge_textBox";
            public const string Weblink_CreateForm_Restrictions_EndAge = "ctl00_ctl00_MainContent_content_txtHighAge_textBox";
            public const string Weblink_CreateForm_Restrictions_AgeRangeRadio = "ctl00_ctl00_MainContent_content_rdbtnAgeRangeRestriction";
            public const string Weblink_CreateForm_Restrictions_AgeMonthsRadio = "ctl00_ctl00_MainContent_content_rdbtnAgeMonths";
            public const string Weblink_CreateForm_Restrictions_AgeYearsRadio = "ctl00_ctl00_MainContent_content_rdbtnAgeYears";
            public const string Weblink_CreateForm_Restrictions_AgeByDate = "ctl00_ctl00_MainContent_content_ctl03_DateTextBox";
            public const string Weblink_CreateForm_Restrictions_SaveButton = "ctl00_ctl00_MainContent_content_btnSaveSettings";
            public const string Weblink_ManageForms_SearchButton = "ctl00_ctl00_MainContent_content_btnSearch";
        }
        public struct InFellowship
        {
            public struct Features
            {
                public const string Church_Directory = "//a[@class='feature_section' and span[position()=2 and text()='Church Directory']]";
                public const string Groups = "//a[@class='feature_section' and span[position()=2 and text()='Groups']]";
                public const string Online_Giving = "//a[@class='feature_section' and span[position()=2 and text()='Online Giving']]";
                public const string Profile_Editor = "//a[@class='feature_section' and span[position()=2 and text()='Profile Editor']]";

                public struct OnlineGiving
                {
                    public const string EnableGiveNow = "enable_give_now";
                    public const string EnableScheduledGiving = "enable_scheduled_giving";
                    public const string EnableGiveWithoutAccount = "enable_give_without_account";
                    public const string Save_Changes = "save_online_giving";
                }
            }
        }
    }

    public struct GeneralEnumerations {
        public enum Status {
            Inactive,
            Active
        }

        public enum Gender {
            Coed = 0,
            Male = 1,
            Female = 2,
            NA = 100
        }

        public enum MaritalStatus {
            ChildYouth,
            Divorced,
            Married,
            Seperated,
            Single,
            Widow,
            Widower,
            NA
        }

        public enum GroupMaritalStatus {
            MarriedOrSingle = 0,
            Married = 1,
            Single = 2,
            NA = 100
        }

        public enum HouseholdPositonWeb {
            Husband,
            Wife,
            SingleAdult,
            Child,
            Other,
            FriendOfFamily,
            NA
        }

        public enum WeeklyScheduleDays {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Any
        }

        public enum StartTimes {
            Morning,
            Midday,
            Afternoon,
            Evening,
            Any
        }

        public enum TimeSetting {
            TwelveHour,
            TwentyFourHour
        }

        public enum AssignmentRole {
            Staff,
            Participant
        }

        public enum InFellowshipBrandingColors {
            blue_light,
            blue_dark,
            green_light,
            green_dark,
            red_light,
            red_dark,
            brown_light,
            brown_dark,
            gray
        }

        public enum CustomFieldType {
            SingleSelect,
            MultiSelect
        }

        public enum IndividualGroupRole {
            Leader,
            Member,
        }

        public enum GroupRolesIndividualHas {
            Leader,
            Member,
            Both
        }

        public enum GroupRolesPortalUser {
            Admin = 1,
            Manager = 2,
            Viewer = 3
        }

        public enum GroupTypeLeaderAdminRights {
            None,
            EmailGroup,
            InviteSomeone,
            EditRecords,
            EditDetails,
            ChangeScheduleLocation,
            TakeAttendance,
            AddSomeone
        }

        public enum GroupTypeMemberAdminRights {
            EmailGroup,
            ViewRoster
        }

        public enum GroupTypeLeaderViewRights {
            NotSpecified,
            Limited,
            Basic,
            Full
        }

        public enum GroupTypeMemberViewRights {
            NotSpecified,
            Limited,
            Basic
        }

        public enum SearchCategoryCriteria {
            AgeRange,
            Gender,
            MaritalStatus,
            Childcare
        }

        public enum SpanOfCareCriteria {
            NotSpecified,
            Coed,
            Female,
            Male,
            MarriedSingle,
            Married,
            Single
        }

        public enum PrivacyLevels {
            ChurchStaff,
            Leaders,
            Members,
            Everyone
        }

        public enum PrivacySettingTypes {
            Address,
            Birthdate,
            Email,
            Phone,
            Websites,
            Social
        }

        public enum CommunicationTypes {
            Home = 1,
            Mobile = 3,
            Work = 2,
            Personal = 4,
            Emergency = 138,
            InFellowship = 6,
            HomeEmail = 5,
            Facebook = 201,
            Linkedin = 202,
            Twitter = 203
        }

        public enum SocialMediaTypes {
            Facebook,
            Twitter,
            LinkedIn
        }

        public enum GiveByTypes {
            Credit_card,
            Personal_check
        }

        public enum GivingFrequency {
            Once,
            Monthly,
            TwiceMonthly,
            Weekly,
            EveryTwoWeeks
        }

        public enum ProspectTaskStates {
            ExpressInterest = 5,
            Invited = 6,
            Joined = 7,
            Declined = 9,
            Denied = 8
        }

        public enum SpanOfCareChildCareSettings {
            HasChildCare = 1,
            NoChildCare = 0,
            NA
        }

        public enum GroupScheduleFrequency {
            OneTime = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
        }

        public enum RemoteDepositeCaptureSearchFilters {
            All,
            Saved,
            Pending
        }

        public enum BatchTypes {
            Standard = 1,
            CreditCard = 2,
            RDC = 3,
            Scanned = 4
        }

        public enum ActivityScheduleFrequency {
            Once,
            Daily,
            Monthly,
            Weekly,
            Yearly
        }

        public enum ActivityCreationParticipantAssignmentTypes {
            Always,
            Activity,
            Schedule,
            DateTime
        }
    
        public enum ActivityCreationAutomaticAssignmentTypes {
            None,
            FirstAttendance,
            MostRecentAttendance,
            LastAttendance
        }

        public enum ActivityCreationStaffAssignmentType {
            Always,
            Activity,
            Schedule,
            DateTime
        }

        public enum ActivityCreationCheckinRequirements {
            Never,
            Always,
            Optional
        }

        public enum ActivitiesRequireAssignment
        {
            None,
            Prevent,
            Alert
        }

        public enum ActivityCreationDefaultActivityCheckinSettings {
            UseDefault,
            OverrideDefault
        }

        public enum ActivityCreationCheckinBestFit {
            NoAgeRestriction,
            AgeRange,
            BirthDateRange
        }

        public enum ActivityEnforceAssignmentCreationType
        {
            Schedule,
            DateTime,
            Always
        }

        public enum ActivityEnforceAssignmentAutoCreate
        {
            FirstAttendance,
            LastAttendance,
            None
        }

        public enum EventRegistrationTextValidationTypes
        {
           

        }


    }

    public struct GeneralInFellowshipConstants {
        public struct GeneralLinks {
            public const string Link_FindAGroup = "link=FIND A GROUP";
            public const string Link_SignIn = "link=Sign In";
            public const string Link_Logout = "link=Logout";
            public const string Link_ChurchName = "link=QA Enterprise Unlimited #2 - In Fellowship";
            public const string Link_Home = "link=HOME";
            public const string Link_ListView = "toggle_list";
            public const string Link_MapView = "toggle_map";
            public const string Link_Directory = "link=DIRECTORY";
        }

        public struct LandingPage {
           // public const string Link_YourProfile = "link=Your Profile";
            //This change is for f1-4313
            public const string Link_YourProfile = "link=Update Profile";
            public const string Link_PrivacySettings = "link=Privacy Settings";
            public const string Link_ChangeYourEmailPassword = "link=Change Login/Password";
            public const string Link_YourGroups = "link=Your Groups";
            public const string Link_YourGroups_WebDriver = "Your Groups";
            public const string Link_FindAGroup = "link=Find A Group";
            public const string Link_ChurchDirectory = "link=Church Directory";
        }

        public struct LoginPage {
            public const string Link_CreateAnAccount = "link=Register";
            public const string Link_FindAGroupHere = GeneralLinks.Link_FindAGroup;
            public const string Email = "username";
        }

        public struct GivingSchedule {
            public const string OnceOption = "Once";
            public const string MonthlyOption = "Monthly";
            public const string TwiceMonthlyOption = "Twice monthly";
            public const string WeeklyOption = "Weekly";
            public const string EveryTwoWeeksOption = "Every two weeks";
        }

        public struct CreateAccount
        {
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string LoginEmail = "emailaddress";
            public const string Password = "password";
            public const string ConfirmPassword = "confirmpassword";
            public const string SaveAndCreateAccount = "submit";
        }
    }

    public static class JavaScriptMethods {
        private const string basescript = "selenium.browserbot.getCurrentWindow()";

        /// <summary>
        /// Returns a JavaScript string to determine if an element is present
        /// </summary>
        /// <param name="id">Id of the element</param>
        /// <returns></returns>
        public static string IsElementPresent(string id) {
            return string.Format("{0}.document.getElementById('{1}')", basescript, id);
        }

        /// <summary>
        /// Determines if a given element is present based on a CSS selector.
        /// </summary>
        /// <param name="cssSelector">The css selector expression</param>
        /// <returns></returns>
        public static string IsElementPresentSelector(string cssSelector) {
            return string.Format("{0}.document.querySelector('{1}')", basescript, cssSelector);
        }

        /// <summary>
        /// Returns a JavaScript string to determine if select has elements.
        /// </summary>
        /// <param name="id">Id of the select</param>
        /// <returns></returns>
        public static string DoesSelectHaveElements(string id) {
            return string.Format("{0}.document.getElementById('{1}').length", basescript, id);
        }

        /// <summary>
        /// Closes the current window.
        /// </summary>
        /// <returns></returns>
        public static string CloseCurrentWindow() {
            return string.Format("{0}.close()", basescript);
        }

        /// <summary>
        /// Gets the number of rows in a given table.
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <returns></returns>
        public static string GetNumberOfTableRows(string tableId) {
            return string.Format("{0}.document.getElementByID('{1}').rows.length", basescript, tableId);
        }

        /// <summary>
        /// Gets the number of columns for a given row.
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <param name="tableRow">Current table row</param>
        /// <returns></returns>
        public static string GetNumberOfTableColumns(string tableID, string tableRow) {
            return string.Format("{0}.document.getElementByID('{1}').rows['{2}'].cells.length", basescript, tableID, tableRow);
        }

        /// <summary>
        /// Determines if the specified option exists in the given select element.
        /// </summary>
        /// <param name="selectId">Select Id</param>
        /// <param name="optionText">Option text</param>
        /// <returns></returns>
        public static string OptionExistsInSelect(string selectId, string optionText) {
            StringBuilder script = new StringBuilder();

            script.Append(" {");
            script.AppendFormat(" var selectElement = {0}.document.getElementById('{1}'); ", basescript, selectId);
            script.Append(" var currentOption; ");
            script.Append(" var foundText = 'blah'; ");
            script.Append(" for (index = 0; index < selectElement.options.length; index++) ");
            script.Append(" {");
            script.Append(" currentOption = selectElement.options[index]; ");
            script.AppendFormat(" if (currentOption.text.match('{0}'))", optionText);
            script.Append(" {");
            script.Append(" foundText = currentOption.text; ");
            script.Append(" break; ");
            script.Append(" }");
            script.Append(" }");
            script.AppendFormat(" foundText.match('{0}'); ", optionText);
            script.Append(" }");

            return script.ToString();
        }
    }

    #region Links
    public struct GeneralLinks {
        public const string Actions = "link=Actions";
        public const string Add = "link=Add";
        public const string Add_WebDriver = "Add";
        public const string Back = "link=Back";
        public const string Edit = "link=Edit";
        public const string Cancel = "link=Cancel";
        public const string Close = "link=Close";
        public const string Home = "link=Home";
        public const string Options = "link=Options";
        public const string Find_person = "link=Find person";
        public const string RETURN = "link=RETURN";
        public const string RETURN_WebDriver = "RETURN";
        //WHO CHANGED THIS?!?
        //public const string RETURN = "minimal_return_arrow"; // //a[@class='minimal_return_arrow']
    }

    public struct GeneralLinksWebDriver
    {
        public const string Actions = "Actions";
        public const string Add = "Add";
        public const string Back = "Back";
        public const string Edit = "Edit";
        public const string Cancel = "Cancel";
        public const string Close = "Close";
        public const string Home = "Home";
        public const string Options = "Options";
        public const string Find_person = "Find person";
        public const string RETURN = "RETURN";
    }

    public struct GeneralButtons {
        public const string Save = "ctl00_ctl00_MainContent_content_btnSave";
        public const string Search = "ctl00_ctl00_MainContent_content_btnSearch";
        public const string submitQuery = "submitQuery";
        //SP--Changing this button id 
        public const string Attendance_submitQuery = "btn_save";
        public const string Attendance_submitQuery_XPath = "//button[contains(@id,'btn_save') and contains(@class,'hidden-xs')]";
        public const string Yes = "ctl00_ctl00_MainContent_content_btnYes";
        public const string Submit = "ctl00_ctl00_MainContent_content_btnSubmit";
        public const string Next = "//input[@value='Next →']";      
    }

    public enum Gender {
        Coed,
        Male,
        Female
    }

    public enum MaritalStatus {
        Married_or_Single,
        Married,
        Single
    }

    public enum Childcare {
        Childcare_is_provided,
        Childcare_is_not_provided,
        Not_applicable
    }

    public enum SearchByConstants {
        Individual,
        MemberEnv,
        Date_of_birth,

        Household_Address,
        Household_Communication,
        Individual_Address,
        Individual_Attributes,
        Individual_Communication,
        Individual_Information,
        Participant_Assignment,
        Participant_Attendance,
        Staff_Assignment,
        Staff_Attendance,

        NULL
    }

    public enum FieldConstants {
        Address,
        City,
        Comment,
        Country,
        State_Province,
        Type,
        Zip,

        Listed, /*Type,*/
        Value,

        Bar_Code,
        Member_Env_Code,
        Date_Of_Birth,
        Employer,
        First_Name,
        Former_Church,
        Former_Name,
        Gender,
        Goes_By,
        Household_Member_Type,
        Last_Name,
        Marital_Status,
        Middle_Name,
        Occupation,
        Occupation_Description,
        Prefix,
        School,
        Status_Group,
        Status,
        Status_Comment,

        Attribute,
        Attribute_Group,
        End_Date,
        Staff_Pastor,
        Start_Date,

        Activity,
        Activity_Group,
        Activity_Schedule,
        Breakout_Group,
        Date_Time,
        Ministry,
        Room_Location_Class,

        NULL
    }

    public enum ComparisonConstants {
        Contains,
        Does_Not_Contain,
        Starts_With,
        Ends_With,
        Equal_To,
        Not_Equal_To,
        Is_Null,

        Is_Equal_To,

        NULL
    }

    public struct HouseholdPositionConstants {
        public const string Head = "Head";
        public const string Spouse = "Spouse";
        public const string Child = "Child";
        public const string Other = "Other (extended family)";
        public const string Visitor = "Visitor";
    }

    public struct TextConstants {
        public const string ErrorHeadingPlural = "The following errors occurred…";
        public const string ErrorHeadingSingular = "The following error occurred…";
    }
    #endregion Links

    // enumeration containing all the environments
    public enum F1Environments {
        LOCAL,
        QA,
        LV_QA,
        LV_UAT,
        INT,
        INT2,
        INT3,
        LV_PROD,
        STAGING,
        PRODUCTION,
    }

    public struct DataConstants {
        public const string Activity = "A Test Activity";
        public const string Ministry = "A Test Ministry";
    }

    public static class TestConstants {
        private const string _htmlEditor = "//body[@id='tinymce' or @class='gecko secure os']";

        public static string HtmlEditor {
            get { return _htmlEditor; }
        }
    }

    public struct TestCategories {
        public struct Services {
            public const string PaymentProcessor = "PaymentProcessor";
            public const string RDCChecker = "RDCChecker";
            public const string PaymentReconciliation = "PaymentReconciliation";
            public const string SmokeTest = "SmokeTest";
        }
    }

    public struct EmailMailBox
    {
        public struct GMAIL
        {
            public const string INBOX = "Inbox";
            public const string SPAM = "[Gmail]/Spam";
            public const string TRASH = "[Gmail]/Trash";
        }

        public struct SearchParse
        {
            public const string ALL = "ALL";
            public const string UNSEEN = "UNSEEN";
        }

    }

    public struct CreditCard
    {
        public struct Images
        {
            public const string Visa = "visa.png";
            public const string Master_Card = "mastercard.png";
            public const string AMEX = "american_express.png";
            public const string Discover = "discover.png";
            public const string Diners_Club = "diners_club.png";
            public const string JCB = "jcb.png";
            public const string Switch = "switch.png";
            public const string Solo = "solo.png";
            public const string Visa_Electron = "visa_electron.png";
            public const string Dankort = "dankort.png";
            public const string Laser = "laser.png";
            public const string Carte_Bleue = "carte_bleue.png";
            public const string Carte_Blanche = "diners_club.png";
            public const string Carta_Si = "carta_si.png";
            public const string Maestro = "maestro.png";
            public const string Switch_Maestro = "switch.png";
        }

        public struct Names
        {
            //Have Images
            public const string Visa = "Visa";
            public const string Master_Card = "Master Card";
            public const string MasterCard = "MasterCard";
            public const string AMEX = "American Express";
            public const string Discover = "Discover";
            public const string Diners_Club = "Diners Club";
            public const string JCB = "JCB";
            public const string Switch = "Switch";
            public const string Solo = "Solo";
            public const string Visa_Electron = "Visa Electron";
            public const string Dankort = "Dankort";
            public const string Laser = "Laser";
            public const string Carte_Bleue = "Carte Bleue";
            public const string Carte_Blanche = "Carte Blanche";
            public const string Carta_Si = "Carta Si";
            public const string Maestro = "Maestro";
            public const string Switch_Maestro = "Switch / Maestro";

            //Don't Have Images
            public const string Optima = "Optima";
            public const string EnRoute = "EnRoute";
            public const string JAL = "JAL";
            public const string Bill_Me_Later = "Bill Me Later";
            public const string eCheck = "eCheck";
            public const string Check = "Check";
            public const string Delta = "Delta";
            public const string None = "--";
            public const string Cash = "Cash";
            public const string Empty = "";

        }
    }

}