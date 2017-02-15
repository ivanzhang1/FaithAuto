using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections;

namespace FTTests.Dashboard.Login
{
    [TestFixture]
    public class Dashboard_Settings : FixtureBaseWebDriver
    {
        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the maximum turned on widgets is 6")]
        public void Dashboard_Setting_MaximumTurnedOnWidget()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            int maximumTurnedOnWidgets = 6;

            try
            {
                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                int totalWidgets = settings.getWidgetsTotalOnPage();
                int totalturnedOnWidgets = settings.getTurnedOnWidgetsTotalOnPage(totalWidgets);

                //verify number of turned on widgets is not exceed 6
                Assert.IsTrue(totalturnedOnWidgets <= maximumTurnedOnWidgets, string.Format("The number of total turned on widgets should not exceed {0}", maximumTurnedOnWidgets));

                settings.turnOnMultiWidgets(totalWidgets, maximumTurnedOnWidgets - (totalturnedOnWidgets - 1));

                //verify the pop-up will be shown if try to select seventh widget
                Assert.IsTrue(settings.isAlertDisplayed());
                settings.closeAlert();
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the default checked funds are correct")]
        public void Dashboard_Setting_DefaultTurnedOnFunds()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                test.SQL.Dashboard_ClearData(churchId, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                ArrayList turnOnFundsSql = test.SQL.Dashboard_Giving_GetDefaultTurnedOnFundsName(churchId, -5);
                ArrayList turnOnFundsOnPage = settings.getWidgetTurnedOnSubItemsOnPage(1);

                TestLog.WriteLine(turnOnFundsOnPage.Count);
                Assert.AreEqual(turnOnFundsSql.Count, turnOnFundsOnPage.Count);

                foreach (var itemOnPage in turnOnFundsOnPage)
                {
                    bool flag = false;
                    TestLog.WriteLine(itemOnPage.ToString());
                    foreach (var itemInSql in turnOnFundsSql)
                    {
                        if(itemInSql.ToString().Replace(" ","").Contains(itemOnPage.ToString().Replace(" ","")))
                        {
                            flag=true;
                            TestLog.WriteLine(itemInSql.ToString() + "|" + itemOnPage.ToString());
                        }
                    }
                    Assert.IsTrue(flag);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the default turned on attribute groups are correct")]
        public void Dashboard_Setting_DefaultTurnedOnAttributeGroups()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                test.SQL.Dashboard_ClearData(churchId, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                ArrayList canBeTurnedOnAttributeGroupsInSql = test.SQL.Dashboard_AttributeGroup_GetGroupsCanBeTurnedOnByDefault(churchId);
                ArrayList turnOnWidgetsOnPage = settings.getTurnedOnWidgetsNamesOnPage(settings.getWidgetsTotalOnPage());

                TestLog.WriteLine(canBeTurnedOnAttributeGroupsInSql.Count);
                Assert.IsTrue(canBeTurnedOnAttributeGroupsInSql.Count >= turnOnWidgetsOnPage.Count - 2);

                foreach (string widget in turnOnWidgetsOnPage)
                {
                    bool flag = false;
                    TestLog.WriteLine(widget.ToString());
                    foreach (string wedgetInDb in canBeTurnedOnAttributeGroupsInSql)
                    {
                        if ( widget.Contains("Giving")|| widget.Contains("Attendance") || widget.Replace(" ", "").Contains(wedgetInDb.Replace(" ", "")))
                        {
                            flag = true;
                            TestLog.WriteLine(widget.ToString() + "|" + wedgetInDb.ToString());
                        }
                    }
                    Assert.IsTrue(flag);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify all the attribute groups on settings page are correct")]
        public void Dashboard_Setting_AllAttributeGroups()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                ArrayList allAttributeGroupsInSql = test.SQL.Dashboard_AttributeGroup_GetAllAttributeGroups(churchId);
                ArrayList allWidgetsOnPage = settings.getAllWidgetsNamesOnPage(settings.getWidgetsTotalOnPage());

                TestLog.WriteLine(allAttributeGroupsInSql.Count);
                Assert.IsTrue(allWidgetsOnPage.Count >= allAttributeGroupsInSql.Count + 1);

                foreach (string widget in allWidgetsOnPage)
                {
                    bool flag = false;
                    TestLog.WriteLine(widget.ToString());
                    foreach (string wedgetInDb in allAttributeGroupsInSql)
                    {
                        if (widget.Contains("Giving") || widget.Contains("Attendance") || widget.Replace(" ", "").Contains(wedgetInDb.Replace(" ", "")))
                        {
                            flag = true;
                            TestLog.WriteLine(widget.ToString() + "|" + wedgetInDb.ToString());
                        }
                    }
                    Assert.IsTrue(flag);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the number of funds on page is correct, inactive funds name will be ended with 'inactive'")]
        public void Dashboard_Setting_DefaultFunds()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            try
            {
                int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
                test.SQL.Dashboard_ClearData(churchId, test.Dashboard.DashboardUsername);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                ArrayList fundsInSql = test.SQL.Dashboard_Giving_GetAllFundsName(churchId);
                ArrayList fundsOnPage = settings.getWidgetSubItemsOnPage(1);

                TestLog.WriteLine(fundsOnPage.Count);
                Assert.AreEqual(fundsInSql.Count, fundsOnPage.Count);

                foreach (var itemOnPage in fundsOnPage)
                {
                    bool flag = false;
                    TestLog.WriteLine(itemOnPage.ToString());
                    object[] funds = fundsInSql.ToArray();
                    for (int i = 0; i < funds.Length; i++ )
                    {
                        if (funds[i].ToString().Replace(" ", "").Contains(itemOnPage.ToString().Replace(" ", "")))
                        {
                            flag = true;
                            TestLog.WriteLine(funds[i].ToString() + "|" + itemOnPage.ToString());
                            if (funds[i].ToString().Contains("inactive"))
                            {
                                Assert.IsTrue(settings.isWidgetItemInactive(1, i+1));
                            }
                        }
                    }
                    Assert.IsTrue(flag);
                }
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify any update on settings page will be saved after closed page")]
        public void Dashboard_Setting_Saved_After_closed()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;
            Random rand = new Random();
            bool checkOruncheckGivingItem = false;
            
            try
            {
                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                int numberOfSubItem = settings.getWidgetSubItemsOnPage(1).Count;
                int randomSubItemIndex = rand.Next(numberOfSubItem);
                int numberOfTurnedOnWidget = settings.getTurnedOnWidgetsTotalOnPage(settings.getWidgetsTotalOnPage());

                bool isGivingWidgetTurnedOn = settings.isWidgetTurnedOn(1);
                bool isAttendanceWidgetTurnedOn = settings.isWidgetTurnedOn(2);

                #region make modification on settings page
                if (isGivingWidgetTurnedOn)
                {
                    if (numberOfSubItem > 0)
                    {
                        if (settings.isWidgetItemChecked(1, randomSubItemIndex))
                        {
                            settings.uncheckWidgetItem(1, randomSubItemIndex);
                            checkOruncheckGivingItem = false;
                        }
                        else
                        {
                            settings.checkWidgetItem(1, randomSubItemIndex);
                            checkOruncheckGivingItem = true;
                        }
                    }

                    settings.turnOffWidget(1);
                }
                else 
                {
                    if (numberOfTurnedOnWidget < 6)
                    {
                        settings.turnOnWidget(1);
                    }
                    else 
                    {
                        isGivingWidgetTurnedOn = !isGivingWidgetTurnedOn;
                        numberOfTurnedOnWidget++;
                    }

                    if (numberOfSubItem > 0)
                    {
                        if (settings.isWidgetItemChecked(1, randomSubItemIndex))
                        {
                            settings.uncheckWidgetItem(1, randomSubItemIndex);
                            checkOruncheckGivingItem = false;
                        }
                        else
                        {
                            settings.checkWidgetItem(1, randomSubItemIndex);
                            checkOruncheckGivingItem = true;
                        }
                    }
                }


                if (isAttendanceWidgetTurnedOn)
                {
                    settings.turnOffWidget(2);
                }
                else 
                {
                    if (numberOfTurnedOnWidget < 6)
                    {
                        settings.turnOnWidget(2);
                    }
                    else
                    {
                        isAttendanceWidgetTurnedOn = !isAttendanceWidgetTurnedOn;
                    }
                }
                #endregion

                settings.closeSettingsPage();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                //verify all update on giving widget is saved
                Assert.AreEqual(settings.isWidgetTurnedOn(1), !isGivingWidgetTurnedOn);
                Assert.AreEqual(settings.isWidgetItemChecked(1, randomSubItemIndex), checkOruncheckGivingItem);
                //verify all update on attendance widget is saved
                Assert.AreEqual(settings.isWidgetTurnedOn(2), !isAttendanceWidgetTurnedOn);
            }
            finally
            {
                //clear test data
            }
        }

        [Test, MultipleAsserts]
        [Author("Jim Jin")]
        [Description("Verify newly created fund can be displayed on setting page after page refresh")]
        public void Dashboard_Setting_NewFund_Refresh()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            string fund_name_1 = utility.GetUniqueName("fund1");
            string fund_name_2 = utility.GetUniqueName("fund2");
            int church_id = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);

            try
            {
                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                ArrayList fundsOnPage = settings.getWidgetSubItemsOnPage(1);

                TestLog.WriteLine(fundsOnPage.Count);

                test.SQL.Giving_Funds_Create(church_id, fund_name_1, true, null, 1, true, "Auto Testing");
                test.SQL.Giving_Funds_Create(church_id, fund_name_2, true, null, 2, true, "Auto Testing");

                test.Driver.Navigate().Refresh();
                utility.WaitForPageIsLoaded();

                ArrayList fundsOnPageNew = settings.getWidgetSubItemsOnPage(1);

                //verify newly added fund has been shown on the page
                Assert.AreEqual(fundsOnPageNew.Count, fundsOnPage.Count+1);
                Assert.IsTrue(fundsOnPageNew.Contains((object)fund_name_1));
            }
            finally
            {
                //clear test data
                test.SQL.Giving_Funds_Delete(church_id, fund_name_1);
                test.SQL.Giving_Funds_Delete(church_id, fund_name_2);
            }
        }

        [Test, MultipleAsserts, Timeout(8000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving widget will be removed from setting page if has no related report rights")]
        public void Dashboard_Setting_Giving_ReportRights_NotShow()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);

            try
            {
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                
                test.Dashboard.uncheckAllRoles(userId);
                test.Dashboard.uncheckAccessRight(userId, "Contribution");
                test.Dashboard.uncheckAccessRight(userId, "Contributor Visibility");
                test.Dashboard.uncheckAccessRight(userId, "Contributor Summaries");

                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                Assert.IsFalse(home.getWidgetName(1).ToLower().Contains("giving"));

                home.openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                //verify the giving widget is removed
                Assert.IsFalse(settings.getWidgetName(1).ToLower().Contains("giving"));

                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAccessRight(userId, "Contribution");
                test.Dashboard.checkAccessRight(userId, "Contributor Visibility");
                test.Dashboard.checkAccessRight(userId, "Contributor Summaries");
                //test.Dashboard.LoginWebDriver();
                test.Portal.LogoutWebDriver();
                test.Dashboard.OpenLoginWebDriver();

                //verify the giving widget is back
                home.openSettingsPage();
                Assert.IsTrue(settings.getWidgetName(1).ToLower().Contains("giving"));
                settings.turnOnWidget(1);

                //verify the giving widget is closed
                Assert.IsFalse(settings.isWidgetTurnedOn(1));
            }
            finally
            {
                //clear test data
                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAllRoles(userId);
            }
        }

        [Test, MultipleAsserts, Timeout(8000)]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the giving widget can be shown if has any of report rights in ('Contribution','Contributor Visibility','Contributor Summaries')")]
        public void Dashboard_Setting_Giving_ReportRights_Show()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);

            try
            {
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);

                test.Dashboard.uncheckAllRoles(userId);
                test.Dashboard.checkAccessRight(userId, "Contribution");
                test.Dashboard.uncheckAccessRight(userId, "Contributor Visibility");
                test.Dashboard.uncheckAccessRight(userId, "Contributor Summaries");
                test.Portal.LogoutWebDriver();
                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);
                //verify giving widget is shown
                Assert.IsTrue(settings.getWidgetName(1).ToLower().Contains("giving"));
                settings.turnOnWidget(1);

                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAccessRight(userId, "Contributor Visibility");
                test.Dashboard.uncheckAccessRight(userId, "Contribution");
                test.Portal.LogoutWebDriver();
                //test.Dashboard.LoginWebDriver();
                test.Dashboard.OpenLoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                Assert.IsTrue(settings.getWidgetName(1).ToLower().Contains("giving"));

                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAccessRight(userId, "Contributor Summaries");
                test.Dashboard.uncheckAccessRight(userId, "Contributor Visibility");
                test.Portal.LogoutWebDriver();
                //test.Dashboard.LoginWebDriver();
                test.Dashboard.OpenLoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                Assert.IsTrue(settings.getWidgetName(1).ToLower().Contains("giving"));

                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAccessRight(userId, "Contribution");
                test.Dashboard.checkAccessRight(userId, "Contributor Visibility");
                //test.Dashboard.checkAccessRight(userId, "Contributor Summaries");
                test.Portal.LogoutWebDriver();
                test.Dashboard.OpenLoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                Assert.IsTrue(settings.getWidgetName(1).ToLower().Contains("giving"));
            }
            finally
            {
                //clear test data
                test.Driver.Manage().Cookies.DeleteAllCookies();
                test.Portal.LoginWebDriver(test.Dashboard.DashboardUsername, test.Dashboard.DashboardPassword, test.Dashboard.ChurchCode);
                test.Dashboard.checkAllRoles(userId);
            }
        }
    }
}
