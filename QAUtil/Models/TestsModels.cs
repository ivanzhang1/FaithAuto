using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QAUtil.Models {
    public class Test {
        [DisplayName("Duration")]
        public string Duration { get; set; }

        [DisplayName("Class Name")]
        public string ClassName { get; set; }

        [DisplayName("Test Name")]
        public string TestName { get; set; }

        [DisplayName("Skipped")]
        public bool Skipped { get; set; }

        [DisplayName("Failed Since")]
        public int FailedSince { get; set; }
    }

    public class TestSuite {
        [DisplayName("File")]
        public string File { get; set; }

        [DisplayName("Namespace")]
        public string Name { get; set; }

        [DisplayName("Duration")]
        public string Duration { get; set; }

        [DisplayName("Test Cases")]
        public int TestCases { get; set; }
    }
}