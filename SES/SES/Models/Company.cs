using SES.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class Company
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
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
        public string Descr { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }

        public List<Company> GetList(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_Company", param);
            var lst = new List<Company>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Company();
                item.CompanyID = !row.IsNull("CompanyID") ? row["CompanyID"].ToString() : "";
                item.CompanyName = !row.IsNull("CompanyName") ? row["CompanyName"].ToString() : "";
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                //item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;

                lst.Add(item);
            }

            return lst;
        }
    }
}