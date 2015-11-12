using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class Promotion_Condition
    {
        public string PromotionID { get; set; }
        public int PromotionType { get; set; }
        public double DecaMinOrdQty { get; set; }
        public double DecaMaxOrdQty { get; set; }
        public double DecaMinOrdAmt { get; set; }
        public double DecaMaxOrdAmt { get; set; }
        public double DecaPercent { get; set; }
        public double MerchantMinOrdQty { get; set; }
        public double MerchantMaxOrdQty { get; set; }
        public double MerchantMinOrdAmt { get; set; }
        public double MerchantMaxOrdAmt { get; set; }
        public double MerchantPercent { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}