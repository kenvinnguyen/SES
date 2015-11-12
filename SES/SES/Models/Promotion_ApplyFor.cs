using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SES.Service;

namespace SES.Models
{
    public class Promotion_ApplyFor
    {
        public string PromotionID { get; set; }
        public int TransporterID { get; set; }
        public int ProvinceID { get; set; }
        public int DistrictID { get; set; }
        public string MerchantID { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string ConditionName { get; set; }
        [Ignore]
        public string Description { get; set; }
    }
}