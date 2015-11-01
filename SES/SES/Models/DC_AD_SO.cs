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
                item.RowUpdatedAt = !row.IsNull("RowUpdatedAt") ? DateTime.Parse(row["RowUpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.RowUpdatedUser = !row.IsNull("RowUpdatedUser") ? row["RowUpdatedUser"].ToString() : "";
                item.RowCreatedUser = !row.IsNull("RowCreatedUser") ? row["RowCreatedUser"].ToString() : "";
                //item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;

                lst.Add(item);
            }

            return lst;
        }
    }
}