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



namespace SES.Models
{
    public class PO_Header
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string PONumber { set; get; }
        public DateTime PODate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string VendorName { get; set; }
        public string VendorID {get;set;}
        public int TotalQty { get; set; }
        public double TotalAmt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
    public class PO_Detail
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string PONumber { set; get; }
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
        public string Status { get; set; }
        public string Note { get; set; }

    }

    public class InventoryTransaction
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string TransactionID { set; get; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string RefID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double TotalAmt { get; set; }
        public string WHID { get; set; }
        public string WHLID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }

    } 
}