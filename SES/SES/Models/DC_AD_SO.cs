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

<<<<<<< .mine        [Ignore]
        public string Size { get; set; }
        [Ignore]
        public string Type { get; set; }
        [Ignore]
        public string ShapeTemplate { get; set; }
        public DataSourceResult GetPage(DataSourceRequest request, string whereCondition, string SONumber)
=======    }
    public class DC_OCM_Merchant
    {
        [AutoIncrement]
        public string MerchantID { get; set; }
        public string MerchantName { get; set; }
        public string ShortName { get; set; }
        public string EnglishName { get; set; }
        public string SubDomain { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PersonalEmail { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string SignOffDate { get; set; }
        public string ActiveDate { get; set; }
        public int LegalPage { get; set; }
        public string LegalPageReason { get; set; }
        public int CreateMerchantStatus { get; set; }
        public int TrainingMerchantManager { get; set; }
        public int PublishedStatus { get; set; }
        public int ApprovedStatus { get; set; }
        public string UpdatedUser { get; set; }
        public string PublishedUser { get; set; }
        public string AprrovedStatus { get; set; }
        public string UpdatedDate { get; set; }
        public string PublishedDate { get; set; }
        public string ApprovedDate { get; set; }
        public string Url { get; set; }
        public string Hotline { get; set; }
        public string Descr { get; set; }
        public string Keyword { get; set; }
        public string ShortDescr { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedUser { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedUser { get; set; }
        public int IsNew { get; set; }
    
        public List<DC_OCM_Merchant> GetList(int page, int pageSize, string whereCondition)
>>>>>>> .theirs        {
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
<<<<<<< .mine                var item = new SODetail();
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
=======                var item = new DC_OCM_Merchant();
                item.MerchantID = !row.IsNull("MerchantID") ? row["MerchantID"].ToString() : "";
                item.MerchantName = !row.IsNull("MerchantName") ? row["MerchantName"].ToString() : "";
                item.RowUpdatedAt = !row.IsNull("RowUpdatedAt") ? DateTime.Parse(row["RowUpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.RowUpdatedUser = !row.IsNull("RowUpdatedUser") ? row["RowUpdatedUser"].ToString() : "";
                item.RowCreatedUser = !row.IsNull("RowCreatedUser") ? row["RowCreatedUser"].ToString() : "";
                //item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;
>>>>>>> .theirs
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
    }

    public class DC_Parameter
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}