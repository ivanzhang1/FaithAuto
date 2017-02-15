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
    public class Modules {
        [DisplayName("Enabled")]
        public bool ENABLED { get; set; }

        [DisplayName("Module ID")]
        public int MODULE_ID { get; set; }

        [DisplayName("Module Name")]
        public string MODULE_NAME { get; set; }

        [DisplayName("Description")]
        public string DESCRIPTION { get; set; }

        public DateTime? CREATED_DATE { get; set; }
        public int? CREATED_BY_USER_ID { get; set; }
        public DateTime? LAST_UPDATED_DATE { get; set; }
        public int? LAST_UPDATED_BY_USER_ID { get; set; }
        public string CREATED_BY_LOGIN { get; set; }
        public string LAST_UPDATED_BY_LOGIN { get; set; }
    }

    public class Services {
        public string SYSTEMNAME { get; set; }
        public string SERVER { get; set; }
        public string NAME { get; set; }
        public string STATUS { get; set; }
        public string STATE { get; set; }
        public string STARTNAME { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class ApplicationPool {
        [DisplayName("Application Pool Name")]
        public string NAME { get; set; }

        [DisplayName("Server Name")]
        public string SERVER { get; set; }
    }
}