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
namespace DecaInsight.Models
{
    public class DC_AD_Picking_Header
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string PickingNumber { set; get; }
        public DateTime PickingDate { get; set; }
        public string PrinterID { get; set; }
        public string PrinterName { get; set; }
        public int TotalQty { get; set; }
        public double TotalAmt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
    }
    public class DC_AD_Picking_Detail
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string PickingNumber { set; get; }
        public string SONumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double TotalAmt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        [Ignore]
        public int InvenNumber { get; set; }
    }
}