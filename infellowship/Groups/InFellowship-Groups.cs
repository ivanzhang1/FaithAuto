using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Selenium;

namespace FTTests.infellowship.Groups {
    public struct GroupsConstants {
        public struct GroupManagement {
            public struct NavigationLinks {
                public const string Link_Dashboard = "link=Dashboard";
                public const string Link_Dashboard_WebDriver = "Dashboard";
                public const string Link_Roster = "link=Roster";
                public const string Link_Roster_WebDriver = "Roster";
                public const string Link_Prospects = "css=li#tab_group_prospects a";
                public const string Link_Settings = "link=Settings";
                public const string Link_Attendance = "link=Attendance";
                public const string Link_CancelToRoster = "css=div > a.cancel";
                public const string Link_TabBack = "tab_back";

                public const string Gear_Menu = "css=span.nav_gear";                
                public const string Gear_SendAnEmail = "css=span.nav_gear_menu_email > a";
                public const string Gear_InviteSomeone = "//a[contains(@href, '/Group/Invite/')]";
                public const string Gear_TellAFriend = "//a[contains(@href, '/Group/TellAFriend/')]";
                public const string Gear_FindAGroup = "link=Find a group";
            }

            public struct InviteSomeonePage {
                public const string TextField_FirstName = "FirstName";
                public const string TextField_LastName = "LastName";
                public const string TextField_Email = "Email";
                public const string TextField_PersonalNote = "Note";
                public const string TextField_PhoneNumber = "Phone";
                public const string TextField_Message = "Message";

                public const string Button_Invite = "btn_invite";
            }

            public struct SendAnEmailPage {
                public const string RadioButton_SendToEveryone = "all_recipients";
                public const string RadioButton_LetMeChoose = "choose_recipients";

                public const string TextField_Subject = "subject";
                public const string TextField_Message = "email_body";

                public const string Link_AttachAFile = "link=Attach a file…";
            }

            public struct TellAFriendPage {
                public const string TextField_FirstName = "FirstName";
                public const string TextField_LastName = "LastName";
                public const string TextField_Email = "Email";
                public const string TextField_PersonalNote = "Note";
                public const string URL_GroupPublic = "//a[contains(@href, '/GroupTypes')]";
            }

            public struct SettingsPage {
                public const string Link_EditDetails = "link=Edit details";
                public const string Link_EditDetails_WebDriver = "Edit details";
                public const string Link_CreateEditSchedule = "//a[contains(@href, '/GroupSchedule/')]";
                public const string Link_CreateEditLocation = "//a[contains(@href, '/GroupLocation/')]";
                public const string Link_UpdateBulletinBoard = "//a[contains(@href, '/Group/BulletinBoard/')]";


                public struct UpdateBulletinBoardPage {
                    public const string TextField_BulletinBoardPost = "post";
                    public const string CheckBox_Publish = "publish";
                    //public const string Link_Cancel_BulletinBoard = "css= a.cancel";
                    public const string Link_Cancel_BulletinBoard = "css=p > a.cancel.nudge_left";
                    
                }

                public struct EditDetailsPage {
                    public const string TextField_GroupName = "name";
                    public const string TextField_Description = "description";
                    public const string TextField_LowerAgeRange = "age_range_min";
                    public const string TextField_UpperAgeRange = "age_range_max";

                    public const string DropDown_MaritalStatus = "marital_preference";
                    public const string DropDown_Gender = "gender_preference";
                    public const string DropDown_Timezone = "time_zone";

                    public const string Checkbox_Childcare = "childcare";
                    public const string Checkbox_Searchable = "is_searchable";

                    public const string Link_Cancel_EditDetails = "Cancel";
                }

                public struct CreateEditSchedulePage {
                    public const string DateControl_StartDate = "start_date";

                    public const string DropDown_Reoccurance = "recurrence_every";

                    public const string TextField_StartMeetingTime = "start_time";
                    public const string TextField_EndMeetingTime = "end_time";

                    public const string RadioButton_OneTimeEvent = "recurrence_never";
                    public const string RadioButton_WeeklyEvent = "recurrence_weekly";
                    public const string RadioButton_MonthlyEvent = "recurrence_monthly";

                    public const string CheckBox_EndTime = "end_toggle";
                    public const string CheckBox_NotifyMembers = "notify";
                    public const string CheckBox_Sunday = "recurrence_weekly_sunday";
                    public const string CheckBox_Monday = "recurrence_weekly_monday";
                    public const string CheckBox_Tuesday = "recurrence_weekly_tuesday";
                    public const string CheckBox_Wednesday = "recurrence_weekly_wednesday";
                    public const string CheckBox_Thursday = "recurrence_weekly_thursday";
                    public const string CheckBox_Friday = "recurrence_weekly_friday";
                    public const string CheckBox_Saturday = "recurrence_weekly_saturday";

                    public const string Link_DeleteThisSchedule = "link=Delete this schedule";
                    //public const string Link_Cancel_CreateSchedule = "link=Cancel";
                    public const string Link_Cancel_CreateSchedule = "css=p > a.cancel";

                    public const string Button_CreateSchedule = "submitQuery";
                }

                public struct CreateEditLocationPage {
                    public const string Link_Cancel_EditDetails = "//a[contains(@href, '/GroupSettings/Show/')]";

                    public const string TextField_LocationName = "location_name";
                    public const string TextField_Description = "location_description";
                    public const string TextField_AddressOne = "street1";
                    public const string TextField_AddressTwo = "street2";
                    public const string TextField_City = "city";
                    public const string TextField_ZipCode = "postal_code";
                    public const string TextField_County = "county";

                    public const string RadioButton_MeetsInPerson = "location_physical";
                    public const string RadioButton_MeetsOnline = "location_online";

                    public const string DropDown_Country = "country_code";
                    public const string DropDown_State = "state";

                    public const string Button_CreateLocation = "submitQuery";

                    //public const string Link_Cancel_CreateLocation = "link=Cancel";
                    public const string Link_Cancel_CreateLocation = "css=p > a.cancel";
                }
            }

            public struct DashboardPage {
                public const string Link_UpdateBulletinBoard_Wrench = "//a[contains(@href, '/Group/BulletinBoard/')]";
                public const string Link_ViewRoster = "link=View roster";
                public const string Link_ViewSettings = "link=View settings";
                public const string Link_UpdateBulletinBoard = "link=Update bulletin board";
                public const string Link_UpdateBulletinBoard_WebDriver = "Update bulletin board";
                public const string Link_ProspectsNumLink = "//a[contains(@href, '/Group/Prospects/')]";

            }

            public struct RosterPage {
                public const string Link_InviteSomeone = "link=Add or Invite someone";
                public const string Link_SendAnEmail = "link=Send an email";
                public const string Link_ViewProspects = "link=View prospects";
				public const string Link_ExportToCSV = "link=Download CSV";
            }


            public struct IndividualMemberPage {
                public const string Link_EditMember = "link=Edit this person";
                public const string Link_EditMemberWebDriver = "Edit this person";
                public const string Link_RemoveMember = "link=Remove from group";
            }

            
        }

        public struct GroupPublicPage {
            public const string TextField_FirstName = "FirstName";
            public const string TextField_LastName = "LastName";
            public const string TextField_Email = "EmailAddress";
            public const string TextField_Message = "Message";

            public const string Button_SubmitExpressInterest = "btn_save";
        }
    }
}
