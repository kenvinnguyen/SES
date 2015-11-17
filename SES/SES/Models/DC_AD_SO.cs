using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using SES.Service;
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
    public class SOHeader
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
    public class SODetail
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

        [Ignore]
        public string Size { get; set; }
        [Ignore]
        public string Type { get; set; }
        [Ignore]
        public string ShapeTemplate { get; set; }
        public DataSourceResult GetPage(DataSourceRequest request, string whereCondition, string SONumber)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", request.Page));
            param.Add(new SqlParameter("@PageSize", request.PageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            param.Add(new SqlParameter("@Sort", CustomModel.GetSortStringFormRequest(request)));
            //param.Add(new SqlParameter("@SONumber", SONumber));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Select_DC_AD_SO_Detail_DynamicView", param);
            var lst = new List<SODetail>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new SODetail();
                item.ItemName = !row.IsNull("ItemName") ? row["ItemName"].ToString() : "";
                item.ItemCode = !row.IsNull("ItemCode") ? row["ItemCode"].ToString() : "";
                item.UnitName = !row.IsNull("UnitName") ? row["UnitName"].ToString() : "";
                item.Qty = !row.IsNull("Qty") ? int.Parse(row["Qty"].ToString()) : 0;
                item.Price = !row.IsNull("Price") ? double.Parse(row["Price"].ToString()) : 0;
                item.Size = !row.IsNull("Size") ? row["Size"].ToString() : "";
                item.Type = !row.IsNull("Type") ? row["Type"].ToString() : "";
                item.ShapeTemplate = !row.IsNull("ShapeTemplate") ? row["ShapeTemplate"].ToString() : "";
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.TotalAmt = !row.IsNull("TotalAmt") ? double.Parse(row["TotalAmt"].ToString()) : 0;
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.UnitID = !row.IsNull("UnitID") ? row["UnitID"].ToString() : "";
                item.SONumber = !row.IsNull("SONumber") ? row["SONumber"].ToString() : "";

                lst.Add(item);
            }
            request.Filters = null;
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;

        }

    }
    public class DC_OCM_Merchant
    {
        [AutoIncrement]
        public int PKMerchantID { get; set; }
        public string MerchantID { get; set; }
        public string MerchantName { get; set; }
        [Ignore]
        public int Qty { get; set; }
        [Ignore]
        public double Price { get; set; }
        [Ignore]
        public double TotalAmt { get; set; }
        [Ignore]
        public string ItemName { get; set; }
        [Ignore]
        public string PrinterName { get; set; }
        [Ignore]
        public DateTime TransactionDate { get; set; }
        [Ignore]
        public string EnglishName { get; set; }
    }

    public class DC_Parameter
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}