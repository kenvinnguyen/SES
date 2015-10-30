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
        public bool Status { get; set; }

        public List<DC_OCM_Merchant> GetList(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_Merchant", param);
            var lst = new List<DC_OCM_Merchant>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_OCM_Merchant();
                item.MerchantID = !row.IsNull("MerchantID") ? row["MerchantID"].ToString() : "";
                item.MerchantName = !row.IsNull("MerchantName") ? row["MerchantName"].ToString() : "";
                //item.ContractName = !row.IsNull("ContractName") ? row["ContractName"].ToString() : "";
                //item.TransporterID = !row.IsNull("TransporterID") ? row["TransporterID"].ToString() : "";
                //item.DiscountPercent = !row.IsNull("DiscountPercent") ? double.Parse(row["DiscountPercent"].ToString()) : 0;
                //item.StartDate = !row.IsNull("StartDate") ? DateTime.Parse(row["StartDate"].ToString()) : DateTime.Parse("1900-01-01");
                //item.EndDate = !row.IsNull("EndDate") ? DateTime.Parse(row["EndDate"].ToString()) : DateTime.Parse("1900-01-01");
                //item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                //item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                //item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                //item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                //item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;

                lst.Add(item);
            }

            return lst;
        }
    }
}