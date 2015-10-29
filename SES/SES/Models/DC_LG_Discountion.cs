using Kendo.Mvc.UI;
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
    public class DC_LG_Discountion
    {
        public string DiscountionID { get; set; }
        public string DiscountionName { get; set; }
        public string DiscountionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string TransporterID { get; set; }
        [Ignore]
        public string TransporterName { get; set; }
        [Ignore]
        public string MerchangeMerchantID { get; set; }
        [Ignore]
        public string MerchantName { get; set; }
        [Ignore]
        public string TransporterLocationID { get; set; }
        [Ignore]
        public string TransporterLocationName { get; set; }
        
    }
     
}