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
    public class Products
    {
        
        [AutoIncrement]
        public int Id { get; set; }
        public double Price { get; set; }
        public string Desc { set; get; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string WHID { get; set; }
        public string WHLID { get; set; }
          [Ignore]
        public string GroupID { get; set; }
          [Ignore]
        public string BrandID { get; set; }       
        public double VATPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public Boolean Status { get; set; }
        public string ShapeTemplate { get; set; }
        [Ignore]
        public string WHName { get; set; }
        [Ignore]
        public string WHLName { get; set; }
        [Ignore]
        public string UnitID { get; set; }
        [Ignore]
        public string UnitName { get; set; }
        [Ignore]
        public double QtyIn { get; set; }
        [Ignore]
        public double QtyOut { get; set; }
        [Ignore]
        public double QtyAvailable { get; set; }
        [Ignore]
        [Required]
        public int Qty { get; set; }

        public List<Products> GetStockReportInOut(int page, int pageSize, string whereCondition, DateTime from, DateTime end)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            param.Add(new SqlParameter("@FromDate", from));
            param.Add(new SqlParameter("@EndDate", end));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Report_StockInOut", param);
            var lst = new List<Products>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Products();
                item.Code = !row.IsNull("Code") ? row["Code"].ToString() : "";
                item.Name = !row.IsNull("Name") ? row["Name"].ToString() : "";
                item.UnitID = !row.IsNull("UnitID") ? row["UnitID"].ToString() : "";
                item.UnitName = !row.IsNull("UnitName") ? row["UnitName"].ToString() : "";
                item.WHID = !row.IsNull("WHID") ? row["WHID"].ToString() : "";
                item.WHName = !row.IsNull("WHName") ? row["WHName"].ToString() : "";
                item.WHLID = !row.IsNull("WHLID") ? row["WHLID"].ToString() : "";
                item.WHLName = !row.IsNull("WHLName") ? row["WHLName"].ToString() : "";
                item.WHLID = !row.IsNull("WHLID") ? row["WHLID"].ToString() : "";
                item.QtyIn = !row.IsNull("QtyIn") ? double.Parse(row["QtyIn"].ToString()) : 0;
                item.QtyOut = !row.IsNull("QtyOut") ? double.Parse(row["QtyOut"].ToString()) : 0;
                item.QtyAvailable = !row.IsNull("QtyAvailable") ? double.Parse(row["QtyAvailable"].ToString()) : 0;
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;

                lst.Add(item);
            }
            return lst;
        }
    }
}