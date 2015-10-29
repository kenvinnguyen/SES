﻿using Kendo.Mvc.UI;
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
    public class DC_LG_Transporter_Location
    {
        public int TransporterLocationID { get; set; }
        public string TransporterLocationName { get; set; }
        public bool IsAllMerchant { get; set; }
        public int TransporterID { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string TransporterName { get; set; }
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_DC_TransporterLocation", param);
            var lst = new List<DC_LG_Transporter_Location>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_Transporter_Location();
                item.TransporterLocationID = !row.IsNull("TransporterLocationID") ? int.Parse(row["TransporterLocationID"].ToString()) : 0;
                item.TransporterLocationName = !row.IsNull("TransporterLocationName") ? row["TransporterLocationName"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? int.Parse(row["TransporterID"].ToString()) : 0;
                item.IsAllMerchant = !row.IsNull("IsAllMerchant") ? Convert.ToBoolean(row["IsAllMerchant"]) : false; ;
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false; 
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.TransporterName = !row.IsNull("TransporterName") ? row["TransporterName"].ToString() : "";
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
        public List<DC_LG_Transporter_Location> Getlist(string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 999999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_DC_TransporterLocation", param);
            var lst = new List<DC_LG_Transporter_Location>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_Transporter_Location();
                item.TransporterLocationID = !row.IsNull("TransporterLocationID") ? int.Parse(row["TransporterLocationID"].ToString()) : 0;
                item.TransporterLocationName = !row.IsNull("TransporterLocationName") ? row["TransporterLocationName"].ToString() : "";
                item.TransporterID = !row.IsNull("TransporterID") ? int.Parse(row["TransporterID"].ToString()) : 0;
                item.IsAllMerchant = !row.IsNull("IsAllMerchant") ? Convert.ToBoolean(row["IsAllMerchant"]) : false; ;
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.TransporterName = !row.IsNull("TransporterName") ? row["TransporterName"].ToString() : "";
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