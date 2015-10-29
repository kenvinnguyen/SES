using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SES.Service;

namespace SES.Models
{
    public class Master_Announcement
    {
        [AutoIncrement]
        public int AnnouncementID { get; set; }
        public string Title { get; set; }
        public string HTMLContent { get; set; }
        public string TextContent { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Default (typeof(string),"")]
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Default(typeof(string), "")]
        public string UpdatedBy { get; set; }
        [Ignore]
        public bool isNewPost { get; set; }
        [Ignore]
        public string UserName { get; set; }
        [Ignore]
        public string DateFormatString { get; set; }
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition, string userID)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            param.Add(new SqlParameter("@UserID", userID));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Announcement_Select_By_Page", param);
            var lst = new List<Master_Announcement>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Announcement();
                item.AnnouncementID =!row.IsNull("AnnouncementID") ? Convert.ToInt32( row["AnnouncementID"].ToString() ): 0;
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.HTMLContent = !row.IsNull("HTMLContent") ? row["HTMLContent"].ToString() : "";
                item.TextContent = !row.IsNull("TextContent") ? row["TextContent"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false;
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                
                //reference
                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        public List<Master_Announcement> GetAllSort(string whereCondition,string orderBy, string ordervalue)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@OrderBy", orderBy));
            param.Add(new SqlParameter("@Ordervalue", ordervalue));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Announcement_SelectAll", param);
            var lst = new List<Master_Announcement>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Announcement();
                item.AnnouncementID = !row.IsNull("AnnouncementID") ? Convert.ToInt32(row["AnnouncementID"].ToString()) : 0;
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.HTMLContent = !row.IsNull("HTMLContent") ? row["HTMLContent"].ToString() : "";
                item.TextContent = !row.IsNull("TextContent") ? row["TextContent"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false;
                item.HTMLContent = !row.IsNull("HTMLContent") ? row["HTMLContent"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                lst.Add(item);

                //reference
                DateTime dateNow = DateTime.Now;
                DateTime dateCreate = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                double subdate = dateNow.Subtract(dateCreate).TotalDays;
                if (subdate <= 7)
                {
                    item.isNewPost = true;
                }
                else
                {
                    item.isNewPost = false;
                }
                item.UserName = !row.IsNull("UserName") ? row["UserName"].ToString() : "";
                item.DateFormatString = String.Format("{0:dd/MM/yyyy}", dateCreate); 
            }
          
            return lst;
        }


    }
}