using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QAUtil.Models {
    public class Payments {
        [DisplayName("Created Date")]
        public string CreatedDate { get; set; }

        [DisplayName("Status")]
        public string PaymentStatusName { get; set; }

        [DisplayName("Household ID")]
        public int HouseholdID { get; set; }

        [DisplayName("Individual ID")]
        public int? IndividualID { get; set; }

        [DisplayName("Amount")]
        public string Amount { get; set; }

        [DisplayName("Reason Code")]
        public string ReasonCode { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Client Application")]
        public string ClientApplication { get; set; }
    }

    public class PaymentTypes {
        [DisplayName("Enabled")]
        public bool ENABLED { get; set; }

        [DisplayName("Payment Type ID")]
        public int PP_TYPE_ID { get; set; }

        [DisplayName("Payment Type")]
        public string PaymentType { get; set; }

        [DisplayName("Feature")]
        public int FeatureID { get; set; }
    }
}