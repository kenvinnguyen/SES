using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DecaInsight.Service;

namespace DecaInsight.Models
{
    public class DC_LG_DeliverFee_Support
    {
        public string SupportDLFeeID { get; set; }
        public string SupportDLFeeName { get; set; }
        public bool IsAllMerchant { get; set; }
        public bool IsAllDistric { get; set; }
        public DateTime? StartDate { get; set; }
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

    }
}