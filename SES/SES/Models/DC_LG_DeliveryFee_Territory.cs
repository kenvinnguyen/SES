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
    public class DC_LG_DeliveryFee_Territory
    {

        public int DeliveryFeeID { get; set; }
        public string ProvinceID { get; set; }
        public string DistrictID { get; set; }
        public string PickingProvinceID { get; set; }
        public string PickingDistrictID { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        [Ignore]
        public string ProvinceName { get; set; }
        [Ignore]
        public string DistrictName { get; set; }
        [Ignore]
        public string PickingProvinceName { get; set; }
        [Ignore]
        public string PickingDistrictName { get; set; }
        [Ignore]
        public string DeliveryFeeName { get; set; }
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_DeliveryFee_Territory",param);
            var lst = new List<DC_LG_DeliveryFee_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_DeliveryFee_Territory();
                item.DeliveryFeeID = !row.IsNull("DeliveryFeeID") ? int.Parse(row["DeliveryFeeID"].ToString()) : 0;
                item.ProvinceID = !row.IsNull("ProvinceID") ? row["ProvinceID"].ToString() : "";
                item.DistrictID = !row.IsNull("DistrictID") ? row["DistrictID"].ToString() : "";
                item.PickingProvinceID = !row.IsNull("PickingProvinceID") ? row["PickingDistrictID"].ToString() : "";
                item.PickingDistrictID = !row.IsNull("PickingDistrictID") ? row["PickingDistrictID"].ToString() : "";
                item.ProvinceName = !row.IsNull("ProvinceName") ? row["ProvinceName"].ToString() : "";
                item.DistrictName = !row.IsNull("DistrictName") ? row["DistrictName"].ToString() : "";
                item.PickingProvinceName = !row.IsNull("PickingProvinceName") ? row["PickingProvinceName"].ToString() : "";
                item.PickingDistrictName = !row.IsNull("PickingDistrictName") ? row["PickingDistrictName"].ToString() : "";
                item.DeliveryFeeName = !row.IsNull("DeliveryFeeName") ? row["DeliveryFeeName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows.Count) : 0;
            return result;
        }
        public List<DC_LG_DeliveryFee_Territory> GetList(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_DeliveryFee_Territory", param);
            var lst = new List<DC_LG_DeliveryFee_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new DC_LG_DeliveryFee_Territory();
                item.DeliveryFeeID = !row.IsNull("DeliveryFeeID") ? int.Parse(row["DeliveryFeeID"].ToString()) : 0;
                item.ProvinceID = !row.IsNull("ProvinceID") ? row["ProvinceID"].ToString() : "";
                item.DistrictID = !row.IsNull("DistrictID") ? row["DistrictID"].ToString() : "";
                item.PickingProvinceID = !row.IsNull("PickingProvinceID") ? row["PickingProvinceID"].ToString() : "";
                item.PickingDistrictID = !row.IsNull("PickingDistrictID") ? row["PickingDistrictID"].ToString() : "";
                item.ProvinceName = !row.IsNull("ProvinceName") ? row["ProvinceName"].ToString() : "";
                item.DistrictName = !row.IsNull("DistrictName") ? row["DistrictName"].ToString() : "";
                item.PickingProvinceName = !row.IsNull("PickingProvinceName") ? row["PickingProvinceName"].ToString() : "";
                item.PickingDistrictName = !row.IsNull("PickingDistrictName") ? row["PickingDistrictName"].ToString() : "";
                item.DeliveryFeeName = !row.IsNull("DeliveryFeeName") ? row["DeliveryFeeName"].ToString() : "";

                lst.Add(item);
            }
       
            return lst;
        }

    }
}