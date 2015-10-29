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
    public class DC_LG_Contract
    {
        public string ContractID { get; set; }
        public string ContractName { get; set; }
        [Ignore]
        public string TransporterID { get; set; }
        public double DiscountPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string TransporterName { get; set; }
        public List<DC_LG_Contract> GetList(int page, int pageSize,string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_Contract_Transporter", param);
            var lst = new List<DC_LG_Contract>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_Contract();
                item.ContractID = !row.IsNull("ContractID") ? row["ContractID"].ToString() : "";
                item.TransporterName = !row.IsNull("TransporterName") ? row["TransporterName"].ToString() : "";
                item.ContractName = !row.IsNull("ContractName") ? row["ContractName"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? row["TransporterID"].ToString() : "";
                item.DiscountPercent = !row.IsNull("DiscountPercent") ? double.Parse(row["DiscountPercent"].ToString()) : 0;
                item.StartDate = !row.IsNull("StartDate") ? DateTime.Parse(row["StartDate"].ToString()) : DateTime.Parse("1900-01-01");
                item.EndDate = !row.IsNull("EndDate") ? DateTime.Parse(row["EndDate"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;

                lst.Add(item);
            }

            return lst;
        }
    }
}