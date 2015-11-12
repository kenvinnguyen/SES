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
    public class Promotion
    {
        public string PromotionID { get; set; }
        public string PromotionName { get; set; }
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
        [Ignore]
        public string MerchantName { get; set; }
        [Ignore]
        public string MerchantID { get; set; }
        [Ignore]
        public string Description { get; set; }
        [Ignore]
        public string ProvinceID { get; set; }
        [Ignore]
        public string DistrictID { get; set; }
        [Ignore]
        public string ProvinceName { get; set; }
        [Ignore]
        public string DistrictName{ get; set; }
        [Ignore]
        public string conNote { get; set; }
        [Ignore]
        public double DecaMinOrdAmt { get; set; }
        [Ignore]
        public double DecaMaxOrdAmt { get; set; }
        [Ignore]
        public double DecaPercentAmt { get; set; }
        [Ignore]
        public double MerchantMinOrdAmt { get; set; }
        [Ignore]
        public double MerchantMaxOrdAmt { get; set; }
        [Ignore]
        public double MerchantPercentAmt { get; set; }
        [Ignore]
        public double DecaMinOrdQty { get; set; }
        [Ignore]
        public double DecaMaxOrdQty { get; set; }
        [Ignore]
        public double DecaPercentQty { get; set; }
        [Ignore]
        public double MerchantMinOrdQty { get; set; }
        [Ignore]
        public double MerchantMaxOrdQty { get; set; }
        [Ignore]
        public double MerchantPercentQty { get; set; }
        public List<Promotion> GetList(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_Promotion_ApplyFor", param);
            var lst = new List<Promotion>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Promotion();
                item.PromotionID = !row.IsNull("PromotionID") ? row["PromotionID"].ToString() : "";
                item.TransporterName = !row.IsNull("TransporterName") ? row["TransporterName"].ToString() : "";
                item.PromotionName = !row.IsNull("PromotionName") ? row["PromotionName"].ToString() : "";
                //item.ProvinceID = !row.IsNull("ProvinceID") ? row["ProvinceID"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? row["TransporterID"].ToString() : "";
                //item.ProvinceName = !row.IsNull("ProvinceName") ? row["ProvinceName"].ToString() : "";
                item.StartDate = !row.IsNull("StartDate") ? DateTime.Parse(row["StartDate"].ToString()) : DateTime.Parse("1900-01-01");
                item.EndDate = !row.IsNull("EndDate") ? DateTime.Parse(row["EndDate"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.conNote = !row.IsNull("conNote") ? row["conNote"].ToString() : "";
                //item.Description = !row.IsNull("Description") ? row["Description"].ToString() : "";
                item.DecaMinOrdAmt = !row.IsNull("DecaMinOrdAmt") ? Double.Parse(row["DecaMinOrdAmt"].ToString()) : 0;
                item.DecaMaxOrdAmt = !row.IsNull("DecaMaxOrdAmt") ? Double.Parse(row["DecaMaxOrdAmt"].ToString()) : 0;
                item.DecaPercentAmt = !row.IsNull("DecaPercentAmt") ? Double.Parse(row["DecaPercentAmt"].ToString()) : 0;
                item.MerchantMinOrdAmt = !row.IsNull("MerchantMinOrdAmt") ? Double.Parse(row["MerchantMinOrdAmt"].ToString()) : 0;
                item.MerchantMaxOrdAmt = !row.IsNull("MerchantMaxOrdAmt") ? Double.Parse(row["MerchantMaxOrdAmt"].ToString()) : 0;
                item.MerchantPercentAmt = !row.IsNull("MerchantPercentAmt") ? Double.Parse(row["MerchantPercentAmt"].ToString()) : 0;
                item.DecaMinOrdQty = !row.IsNull("DecaMinOrdQty") ? Double.Parse(row["DecaMinOrdQty"].ToString()) : 0;
                item.DecaMaxOrdQty = !row.IsNull("DecaMaxOrdQty") ? Double.Parse(row["DecaMaxOrdQty"].ToString()) : 0;
                item.DecaPercentQty = !row.IsNull("DecaPercentQty") ? Double.Parse(row["DecaPercentQty"].ToString()) : 0;
                item.MerchantMinOrdQty = !row.IsNull("MerchantMinOrdQty") ? Double.Parse(row["MerchantMinOrdQty"].ToString()) : 0;
                item.MerchantMaxOrdQty = !row.IsNull("MerchantMaxOrdQty") ? Double.Parse(row["MerchantMaxOrdQty"].ToString()) : 0;
                item.MerchantPercentQty = !row.IsNull("MerchantPercentQty") ? Double.Parse(row["MerchantPercentQty"].ToString()) : 0;
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;
                item.IsAllMerchant = !row.IsNull("IsAllMerchant") ? Convert.ToBoolean(row["IsAllMerchant"].ToString()) : false;
                if (item.IsAllMerchant)
                {
                    item.MerchantName = "Áp dụng cho tất cả các gian hàng";
                    item.MerchantID = "";
                }
                else
                {
                    item.MerchantID = !row.IsNull("MerchantID") ? row["MerchantID"].ToString() : "";
                    item.MerchantName = !row.IsNull("MerchantName") ? row["MerchantName"].ToString() : "";
                }
                item.IsAllDistric = !row.IsNull("IsAllDistric") ? Convert.ToBoolean(row["IsAllDistric"].ToString()) : false;
                if (item.IsAllDistric)
                {
                    item.DistrictName = "Áp dụng cho tất cả quận huyện";
                    item.DistrictID = "";
                }
                else
                {
                    item.DistrictID = !row.IsNull("DistrictID") ? row["DistrictID"].ToString() : "";
                    item.DistrictName = !row.IsNull("DistrictName") ? row["DistrictName"].ToString() : "";
                }
                lst.Add(item);
            }
            return lst;
        }
    }
}