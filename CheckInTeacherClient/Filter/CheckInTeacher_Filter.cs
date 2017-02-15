using System;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using OpenQA.Selenium;
using Common.PageEntitys;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using System.Net;

namespace FTTests.Filter
{
    [TestFixture, Parallelizable]
    class CheckInTeacher_Filter : CheckInBaseWebDriver
    {
        [Test(Order = 1), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student status Onsite")]
        public void CheckInTeacher_Filter_VerifyFilterOniste()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckInByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, true, false, false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as Onsite");
                Assert.IsFalse(studentNum2 < 1, "Student number of Onsite should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 2), RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies filter with student status In")]
        public void CheckInTeacher_Filter_VerifyFilterIn()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckInByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, true, false);
            test.CheckIn.CloseIconFilter();
            try 
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as In");
                Assert.IsFalse(studentNum2 < 1, "Student number of In should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 3), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student Out")]
        public void CheckInTeacher_Filter_VerifyFilterOut()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckOutByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, false, true);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as Out");
                Assert.IsFalse(studentNum2 < 1, "Student number of Out should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 4), RepeatOnFailure]
        [Author("Mady Kou")]
        [Category(TestCategories.Services.SmokeTest)]
        [Description("Verifies filter with student In and Out")]
        public void CheckInTeacher_Filter_VerifyFilterInAndOut()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, true, false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, true);
                test.CheckIn.CloseIconFilter();

                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, true, true);
                test.CheckIn.CloseIconFilter();

                test.CheckIn.WaitForRosterDisplay();
                int studentNum3 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                Assert.IsTrue(studentNum3 == studentNum1 + studentNum2, "Student number of Onsite and In are wrong");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies search function works in roster")]
        public void CheckInTeacher_Filter_SearchStudent()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

            test.CheckIn.OpenIconSearch();
            test.CheckIn.TypeNameAndSearch("Emma Kemp");
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsTrue(studentNum2 == 1, "Student number of the name is not 1");
                Assert.IsTrue(studentNum2 < studentNum1, "Student number of matched value is not smaller than total students");
            }
            finally
            {
                test.CheckIn.TypeNameAndSearch("");
                test.CheckIn.CloseIconSearch();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Category(TestCategories.Services.SmokeTest)]
        [Author("Mady Kou")]
        [Description("Verifies search function works in roster")]
        public void CheckInTeacher_Filter_SortBy()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterWebDriver(rosterNameArray[0]);
            int studentIndex1 = test.CheckIn.GetListIndexByStudentFullName("GraceAutoDrop Zhang");
            TestLog.WriteLine("Original index of the student is: " + studentIndex1);

            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetSortBy(false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentIndex2 = test.CheckIn.GetListIndexByStudentFullName("GraceAutoDrop Zhang");
                TestLog.WriteLine("Original index of the student is: " + studentIndex2);
                Assert.AreNotEqual(studentIndex1, studentIndex2, "Student index is the same after sort by changed");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetSortBy(true);
                test.CheckIn.CloseIconFilter();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 5), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student status Onsite")]
        public void CheckInTeacher_Filter_VerifyFilterOniste_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckInByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, true, false, false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as Onsite");
                Assert.IsFalse(studentNum2 < 1, "Student number of Onsite should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 6), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student status In")]
        public void CheckInTeacher_Filter_VerifyFilterIn_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckInByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, true, false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as In");
                Assert.IsFalse(studentNum2 < 1, "Student number of In should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 7), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student Out")]
        public void CheckInTeacher_Filter_VerifyFilterOut_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
            int index = test.CheckIn.GetRandomStudentIndexFromRoster();
            test.CheckIn.CheckOutByStudentIndex(index);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, false, true);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsFalse(studentNum2 == studentNum1, "Student number of Onsite should not be the same as Out");
                Assert.IsFalse(studentNum2 < 1, "Student number of Out should be greater than 1");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                test.CheckIn.UndoByStudentIndex(index);
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test(Order = 8), RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies filter with student In and Out")]
        public void CheckInTeacher_Filter_VerifyFilterInAndOut_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetFilter(false, false, true, false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, true);
                test.CheckIn.CloseIconFilter();

                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, true, true);
                test.CheckIn.CloseIconFilter();

                test.CheckIn.WaitForRosterDisplay();
                int studentNum3 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

                Assert.IsTrue(studentNum3 == studentNum1 + studentNum2, "Student number of Onsite and In are wrong");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetFilter(false, false, false, false);
                test.CheckIn.CloseIconFilter();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies search function works in roster")]
        public void CheckInTeacher_Filter_SearchStudent_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            int studentNum1 = test.CheckIn.GetTotalNumOfStudentsFromRoster();

            test.CheckIn.OpenIconSearch();
            test.CheckIn.TypeNameAndSearch("Emma Kemp");
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentNum2 = test.CheckIn.GetTotalNumOfStudentsFromRoster();
                Assert.IsTrue(studentNum2 == 1, "Student number of the name is not 1");
                Assert.IsTrue(studentNum2 < studentNum1, "Student number of matched value is not smaller than total students");
            }
            finally
            {
                test.CheckIn.TypeNameAndSearch("");
                test.CheckIn.CloseIconSearch();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }

        [Test, RepeatOnFailure]
        [Author("Mady Kou")]
        [Description("Verifies search function works in roster")]
        public void CheckInTeacher_Filter_SortBy_Mobile()
        {
            TestBaseWebDriver test = base.TestContainer[Gallio.Framework.TestContext.CurrentContext.Test.Name];
            test.CheckIn.LoginRosterMobileWebDriver(rosterNameArray[0]);
            int studentIndex1 = test.CheckIn.GetListIndexByStudentFullName("GraceAutoDrop Zhang");
            TestLog.WriteLine("Original index of the student is: " + studentIndex1);

            test.CheckIn.OpenIconFilter();
            test.CheckIn.SetSortBy(false);
            test.CheckIn.CloseIconFilter();
            try
            {
                test.CheckIn.WaitForRosterDisplay();
                int studentIndex2 = test.CheckIn.GetListIndexByStudentFullName("GraceAutoDrop Zhang");
                TestLog.WriteLine("Original index of the student is: " + studentIndex2);
                Assert.AreNotEqual(studentIndex1, studentIndex2, "Student index is the same after sort by changed");
            }
            finally
            {
                test.CheckIn.OpenIconFilter();
                test.CheckIn.SetSortBy(true);
                test.CheckIn.CloseIconFilter();
                //Logout
                test.CheckIn.LogoutWebDriver();
            }

        }
    }
}
