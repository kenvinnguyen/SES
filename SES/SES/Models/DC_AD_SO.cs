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
    public class DC_AD_SO_Header
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string SONumber { set; get; }
        public DateTime SODate { get; set; }
        public string MerchantID { get; set; }
        public string WHID { get; set; }
        public string WHLID { get; set; }
        public int TotalQty { get; set; }
        public double TotalAmt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        [Ignore]
        public string WHName { get; set; }
        [Ignore]
        public string WHLName { get; set; }
    }
    public class DC_AD_SO_Detail
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string SONumber { set; get; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double TotalAmt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }

    }
    public class DC_OCM_Merchant
    {
        [AutoIncrement]
        public int PKMerchantID { get; set; }
        public string MerchantID { get; set; }
        public string MerchantName { get; set; }
    }
}