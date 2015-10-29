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
    public class DC_LG_DeliveryFee
    {
        public int DeliveryFeeID { get; set; }
        public string Name { get; set; }
        public string TransporterID { get; set; }
        public string Descr { get; set; }
        public double MinDay { get; set; }
        public double MaxDay { get; set; }
        public double MinTime { get; set; }
        public double MaxTime { get; set; }
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string DeliveryName { get; set; }
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_DC_DeliveryFee", param);
            var lst = new List<DC_LG_DeliveryFee>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_DeliveryFee();
                item.DeliveryFeeID = !row.IsNull("DeliveryFeeID") ? int.Parse(row["DeliveryFeeID"].ToString()): 0; 
                item.Name = !row.IsNull("Name") ? row["Name"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? row["TransporterID"].ToString() : "";
                item.Descr = !row.IsNull("Descr") ? row["Descr"].ToString() : "";
                item.MinDay = !row.IsNull("MinDay") ? Convert.ToDouble(row["MinDay"].ToString()) : 0;
                item.MaxDay = !row.IsNull("MaxDay") ? Convert.ToDouble(row["MaxDay"].ToString()) : 0;
                item.MinTime = !row.IsNull("MinTime") ? Convert.ToDouble(row["MinTime"].ToString()) : 0;
                item.MaxTime = !row.IsNull("MaxTime") ? Convert.ToDouble(row["MaxTime"].ToString()) : 0;
                item.MinWeight = !row.IsNull("MinWeight") ? Convert.ToDouble(row["MinWeight"].ToString()) : 0;
                item.MaxWeight = !row.IsNull("MaxWeight") ? Convert.ToDouble(row["MaxWeight"].ToString()) : 0;
                item.Price = !row.IsNull("Price") ? Convert.ToDouble(row["Price"].ToString()) : 0;
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false; ;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.DeliveryName = !row.IsNull("DeliveryName") ? row["DeliveryName"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows.Count) : 0;
            return result;
        }
        public List<DC_LG_DeliveryFee> GetListDeliveryFee(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_DC_DeliveryFee", param);
            var lst = new List<DC_LG_DeliveryFee>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_DeliveryFee();
                item.DeliveryFeeID = !row.IsNull("DeliveryFeeID") ? int.Parse(row["DeliveryFeeID"].ToString()) : 0;
                item.Name = !row.IsNull("Name") ? row["Name"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? row["TransporterID"].ToString() : "";
                item.Descr = !row.IsNull("Descr") ? row["Descr"].ToString() : "";
                item.MinDay = !row.IsNull("MinDay") ? Convert.ToDouble(row["MinDay"].ToString()) : 0;
                item.MaxDay = !row.IsNull("MaxDay") ? Convert.ToDouble(row["MaxDay"].ToString()) : 0;
                item.MinTime = !row.IsNull("MinTime") ? Convert.ToDouble(row["MinTime"].ToString()) : 0;
                item.MaxTime = !row.IsNull("MaxTime") ? Convert.ToDouble(row["MaxTime"].ToString()) : 0;
                item.MinWeight = !row.IsNull("MinWeight") ? Convert.ToDouble(row["MinWeight"].ToString()) : 0;
                item.MaxWeight = !row.IsNull("MaxWeight") ? Convert.ToDouble(row["MaxWeight"].ToString()) : 0;
                item.Price = !row.IsNull("Price") ? Convert.ToDouble(row["Price"].ToString()) : 0;
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false; ;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.DeliveryName = !row.IsNull("DeliveryName") ? row["DeliveryName"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");

                lst.Add(item);
            }
           
            return lst;
        }
    }
}