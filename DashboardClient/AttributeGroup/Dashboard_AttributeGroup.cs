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
using System.Collections;

namespace FTTests.Dashboard.AttributeGroup
{
    [TestFixture]
    class Dashboard_AttributeGroup : FixtureBaseWebDriver
    {
        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify new attribute group and attribute whould be shown, inactive attribute group and attribute would not be shown on page")]
        public void Dashboard_AttributeGroup_Settings_NewAttributeGroup_And_NewAttribute()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            string activeAttributeGroup = utility.GetUniqueName("aag");
            string inactiveAttributeGroup = utility.GetUniqueName("iag");
            string activeAttribute = utility.GetUniqueName("aa");
            string inactiveAttribute = utility.GetUniqueName("ia");

            try
            {
                test.SQL.Admin_IndividualAttributeGroups_Create(churchId, activeAttributeGroup, true);
                test.SQL.Admin_IndividualAttributeGroups_Create(churchId, inactiveAttributeGroup, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, activeAttribute, true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, inactiveAttribute, true, true, false, false, false);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                int widgetsTotal = settings.getWidgetsTotalOnPage();
                bool hasCustomerizedActiveAttributeGroup = false;
                bool hasCustomerizedInactiveAttributeGroup = false;
                bool hasCustomerizedActiveAttribute = false;
                bool hasCustomerizedInactiveAttribute = false;
                ArrayList allWidgetsOnPage = settings.getAllWidgetsNamesOnPage(settings.getWidgetsTotalOnPage());

                foreach (string widget in allWidgetsOnPage)
                {
                    if (widget.Contains(activeAttributeGroup))
                    {
                        TestLog.WriteLine(string.Format("The customerized active attribute group '{0}' found", widget));
                        hasCustomerizedActiveAttributeGroup = true;
                    }

                    if (widget.Contains(inactiveAttributeGroup))
                    {
                        TestLog.WriteLine(string.Format("The customerized inactive attribute group '{0}' found", widget));
                        hasCustomerizedInactiveAttributeGroup = true;
                    }
                }

                Assert.IsTrue(hasCustomerizedActiveAttributeGroup, "Active attribute group should can be shown on page");
                Assert.IsFalse(hasCustomerizedInactiveAttributeGroup, "Inactive attribute group should cannot be shown on page");

                for (int i = 1; i <= widgetsTotal; i++)
                {
                    if (settings.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        ArrayList items = settings.getWidgetSubItemsOnPage(i);
                        foreach (string item in items)
                        {
                            if (item.Contains(activeAttribute))
                            {
                                TestLog.WriteLine(string.Format("The customerized active attribute '{0}' found", item));
                                hasCustomerizedActiveAttribute = true;
                            }

                            if (item.Contains(inactiveAttribute))
                            {
                                TestLog.WriteLine(string.Format("The customerized inactive attribute '{0}' found", item));
                                hasCustomerizedInactiveAttribute = true;
                            }
                        }
                        Assert.IsTrue(hasCustomerizedActiveAttribute, "Active attribute should can be shown on page");
                        Assert.IsFalse(hasCustomerizedInactiveAttribute, "Inactive attribute should cannot be shown on page");
                        break;
                    }
                }

            }
            finally
            {
                //clear test data
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, activeAttribute);
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, inactiveAttribute);
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, activeAttributeGroup);
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, inactiveAttributeGroup);
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify active attribute without 'start date' would not be shown on page")]
        public void Dashboard_AttributeGroup_Settings_NewAttribute_WithoutStartDate()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            string activeAttributeGroup = utility.GetUniqueName("aag");
            string activeAttributeWithoutStartDate = utility.GetUniqueName("aawsd");

            try
            {
                test.SQL.Admin_IndividualAttributeGroups_Create(churchId, activeAttributeGroup, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, activeAttributeWithoutStartDate, true, false, false, false, true);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                int widgetsTotal = settings.getWidgetsTotalOnPage();
                bool hasCustomerizedActiveAttributeWithoutStartDate = false;

                for (int i = 1; i <= widgetsTotal; i++)
                {
                    if (settings.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        ArrayList items = settings.getWidgetSubItemsOnPage(i);
                        foreach (string item in items)
                        {
                            if (item.Contains(activeAttributeWithoutStartDate))
                            {
                                TestLog.WriteLine(string.Format("The customerized active attribute '{0}' without 'start date' is found", item));
                                hasCustomerizedActiveAttributeWithoutStartDate = true;
                            }
                        }

                        Assert.IsFalse(hasCustomerizedActiveAttributeWithoutStartDate, "Active attribute without 'start date' should cannot be shown on page");
                        break;
                    }
                }

            }
            finally
            {
                //clear test data
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, activeAttributeWithoutStartDate);
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, activeAttributeGroup);
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify active attribute named in ('salvation','baptism','dedication','re-dedication','rededication','baby dedication') would be checked by default")]
        public void Dashboard_AttributeGroup_Settings_SpecialAttributes()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            string activeAttributeGroup = utility.GetUniqueName("aag");
            bool is_attribute_salvation_checked = false;
            bool is_attribute_baptism_checked = false;
            bool is_attribute_dedication_checked = false;
            bool is_attribute_re_dedication_checked = false;
            bool is_attribute_rededication_checked = false;
            bool is_attribute_baby_dedication_checked = false;

            try
            {
                test.SQL.Dashboard_ClearData(churchId, test.Dashboard.DashboardUsername);
                test.SQL.Admin_IndividualAttributeGroups_Create(churchId, activeAttributeGroup, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "salvation", true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "baptism", true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "dedication", true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "re-dedication", true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "rededication", true, true, false, false, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "baby dedication", true, true, false, false, true);

                test.Dashboard.LoginWebDriver();
                new DashboardHomePage(test.Driver, test.GeneralMethods).openSettingsPage();
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                int widgetsTotal = settings.getWidgetsTotalOnPage();

                for (int i = 1; i <= widgetsTotal; i++)
                {
                    if (settings.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        object[] items = settings.getWidgetSubItemsOnPage(i).ToArray();
                        for (int j = 0; j < items.Length; j++)
                        {
                            if (items[j].ToString().ToLower().Contains("salvation"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_salvation_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_salvation_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                                continue;
                            }

                            if (items[j].ToString().ToLower().Contains("baptism"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_baptism_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_baptism_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                                continue;
                            }

                            if (items[j].ToString().ToLower().Contains("dedication"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_dedication_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_dedication_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                                continue;
                            }

                            if (items[j].ToString().ToLower().Contains("re-dedication"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_re_dedication_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_re_dedication_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                                continue;
                            }

                            if (items[j].ToString().ToLower().Contains("rededication"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_rededication_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_rededication_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                                continue;
                            }

                            if (items[j].ToString().ToLower().Contains("baby dedication"))
                            {
                                TestLog.WriteLine(string.Format("The active attribute '{0}' is found", items[j]));
                                is_attribute_baby_dedication_checked = settings.isWidgetItemChecked(i, j + 1);
                                Assert.IsTrue(is_attribute_baby_dedication_checked, string.Format("Active attribute '{0}' should be checked by default", items[j]));
                            }
                        }

                    }
                }

            }
            finally
            {
                //clear test data
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "salvation");
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "baptism");
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "dedication");
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "re-dedication");
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "rededication");
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "baby dedication");
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, activeAttributeGroup);
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the big number of newly created attribute group is 0")]
        public void Dashboard_AttributeGroup_Home_BigNumber_NewAttributeGroup()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            string activeAttributeGroup = utility.GetUniqueName("aag");

            try
            {
                test.SQL.Admin_IndividualAttributeGroups_Create(churchId, activeAttributeGroup, true);
                test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, "salvation", true, true, false, false, true);

                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetsTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetsTotal);

                for (int i = 1; i <= widgetsTotal; i++)
                {
                    if (settings.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        Assert.IsFalse(settings.isWidgetItemChecked(i, 1), "Active attribute 'salvation' should not be checked");
                        if (turnedOnWidgetTotal == 6)
                        {
                            settings.turnOffMultiWidgets(widgetsTotal, 1);
                        }
                        settings.turnOnWidget(i);

                        settings.checkWidgetItem(i, 1);

                        break;
                    }
                }

                settings.closeSettingsPage();
                home.selectView("year");

                for (int i = 1; i <= turnedOnWidgetTotal; i++)
                {
                    if (home.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        string bigNumber = home.getWidgetBigNumber(i);
                        int subTotalOnPage = int.Parse(bigNumber.Replace(",", ""));
                        Assert.AreEqual(subTotalOnPage, 0, "The big number of attribute group should be 0");
                        break;
                    }
                }
            }
            finally
            {
                //clear test data
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, "salvation");
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, activeAttributeGroup);
            }
        }

        [Test, MultipleAsserts]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Jim Jin")]
        [Description("Verify the calculation of big number will take newly created individual into account")]
        public void Dashboard_AttributeGroup_Home_BigNumber_NewIndividual()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            GeneralMethods utility = test.GeneralMethods;

            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            int churchId = test.SQL.FetchChurchID(test.Dashboard.ChurchCode);
            int userId = test.SQL.User_FetchID(churchId, test.Dashboard.DashboardEmail, test.Dashboard.DashboardUsername);
            string activeAttributeGroup = utility.GetUniqueName("aag");
            string activeAttributeWithStartDate = utility.GetUniqueName("aa1");
            string activeAttributeWithoutStartDate = utility.GetUniqueName("aa2");
            string individual_1 = utility.GetUniqueName("ind1");
            string individual_2 = utility.GetUniqueName("ind2");
            string individual_3 = utility.GetUniqueName("ind3");
            string individual_4 = utility.GetUniqueName("ind4");

            test.SQL.Admin_IndividualAttributeGroups_Create(churchId, activeAttributeGroup, true);
            test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, activeAttributeWithStartDate, false, true, false, false, true);
            test.SQL.Admin_IndividualAttributes_Create(churchId, activeAttributeGroup, activeAttributeWithoutStartDate, false, false, false, false, true);
            test.SQL.People_Individual_Create(churchId, "AutoTest", individual_1);
            test.SQL.People_Individual_Create(churchId, "AutoTest", individual_2);
            test.SQL.People_Individual_Create(churchId, "AutoTest", individual_3);
            test.SQL.People_Individual_Create(churchId, "AutoTest", individual_4);
            int individual_1_id = test.SQL.People_Individuals_FetchID(churchId, "AutoTest " + individual_1);
            int individual_2_id = test.SQL.People_Individuals_FetchID(churchId, "AutoTest " + individual_2);
            int individual_3_id = test.SQL.People_Individuals_FetchID(churchId, "AutoTest " + individual_3);
            int individual_4_id = test.SQL.People_Individuals_FetchID(churchId, "AutoTest " + individual_4);

            int attributeGroupId = test.SQL.Admin_Fetch_AttributeGroupID(churchId, activeAttributeGroup);
            int attributeId_withStartDate = test.SQL.Admin_Fetch_AttributeID(churchId, attributeGroupId, activeAttributeWithStartDate);
            int attributeId_withoutStartDate = test.SQL.Admin_Fetch_AttributeID(churchId, attributeGroupId, activeAttributeWithoutStartDate);

            test.SQL.Dashboard_Insert_IndividualAttribute(churchId, individual_1_id, attributeId_withStartDate, now, now.AddYears(100));
            test.SQL.Dashboard_Insert_IndividualAttribute(churchId, individual_2_id, attributeId_withStartDate, now.AddDays(-370), now.AddYears(-1));
            test.SQL.Dashboard_Insert_IndividualAttribute(churchId, individual_3_id, attributeId_withoutStartDate, now.AddYears(100), now.AddYears(100));
            test.SQL.Dashboard_Insert_IndividualAttribute(churchId, individual_4_id, attributeId_withStartDate, now.AddDays(-2), now);

            try
            {
                test.Dashboard.LoginWebDriver();
                DashboardHomePage home = new DashboardHomePage(test.Driver, test.GeneralMethods);
                DashboardSettingsPage settings = new DashboardSettingsPage(test.Driver, test.GeneralMethods, test.SQL);

                home.openSettingsPage();
                int widgetsTotal = settings.getWidgetsTotalOnPage();
                int turnedOnWidgetTotal = settings.getTurnedOnWidgetsTotalOnPage(widgetsTotal);

                for (int i = 1; i <= widgetsTotal; i++)
                {
                    if (settings.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        if (turnedOnWidgetTotal == 6)
                        {
                            settings.turnOffMultiWidgets(widgetsTotal, 1);
                        }
                        settings.turnOnWidget(i);

                        settings.checkWidgetItem(i, 1);
                        break;
                    }
                }

                settings.closeSettingsPage();
                home.selectView("year");

                DateTime[] dateRange = test.Dashboard.getDateRange(now, "year");
                int subTotalInDb = test.SQL.Dashboard_AttributeGroup_GetPeriodSum(churchId, userId, attributeGroupId, dateRange[0], dateRange[1]);

                for (int i = 1; i <= turnedOnWidgetTotal; i++)
                {
                    if (home.getWidgetName(i).Contains(activeAttributeGroup))
                    {
                        string bigNumber = home.getWidgetBigNumber(i);
                        int subTotalOnPage = int.Parse(bigNumber.Replace(",", ""));


                        //Verify only individual_1 will be counted
                        TestLog.WriteLine(string.Format("Page: {0}|Db: {1}", subTotalOnPage, subTotalInDb));
                        Assert.AreEqual(subTotalOnPage, subTotalInDb, string.Format("The big number of attribute group should equal to {0}", subTotalInDb));
                        break;
                    }
                }

            }
            finally
            {
                //clear test data
                test.SQL.Dashboard_Delete_IndividualAttribute(churchId, individual_1_id, attributeId_withStartDate);
                test.SQL.Dashboard_Delete_IndividualAttribute(churchId, individual_2_id, attributeId_withStartDate);
                test.SQL.Dashboard_Delete_IndividualAttribute(churchId, individual_3_id, attributeId_withoutStartDate);
                test.SQL.Dashboard_Delete_IndividualAttribute(churchId, individual_4_id, attributeId_withStartDate);
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, activeAttributeWithoutStartDate);
                test.SQL.Admin_IndividualAttributes_Delete(churchId, activeAttributeGroup, activeAttributeWithStartDate);
                test.SQL.Admin_IndividualAttributeGroups_Delete(churchId, activeAttributeGroup);
                test.SQL.People_Individual_Delete(churchId, "AutoTest", individual_1);
                test.SQL.People_Individual_Delete(churchId, "AutoTest", individual_2);
                test.SQL.People_Individual_Delete(churchId, "AutoTest", individual_3);
                test.SQL.People_Individual_Delete(churchId, "AutoTest", individual_4);
            }
        }
    }
}

