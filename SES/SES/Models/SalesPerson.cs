using ServiceStack.DataAnnotations;
using SES.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class SalesPerson
    {
        public string CompanyID { get; set; }
        public string SalesPersonID { get; set; }
        public string SalesPersonName { get; set; }
        public bool Gender { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string IdentityCard { get; set; }
        public string IdentityCardPlace { get; set; }
        public DateTime? IdentityCardDate { get; set; }
        public string IdentityCardAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
        [Ignore]
        public string CompanyName { get; set; }
        public List<SalesPerson> GetList(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_get_SalesPerson", param);
            var lst = new List<SalesPerson>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new SalesPerson();
                item.CompanyID = !row.IsNull("CompanyID") ? row["CompanyID"].ToString() : "";
                item.SalesPersonID = !row.IsNull("SalesPersonID") ? row["SalesPersonID"].ToString() : "";
                item.SalesPersonName = !row.IsNull("SalesPersonName") ? row["SalesPersonName"].ToString() : "";
                item.Gender = !row.IsNull("Gender") ? Convert.ToBoolean(row["Gender"].ToString()) : false;
                item.Description = !row.IsNull("Description") ? row["Description"].ToString() : "";
                item.DateOfBirth = !row.IsNull("DateOfBirth") ? DateTime.Parse(row["DateOfBirth"].ToString()) : DateTime.Parse("1900-01-01");
                item.IdentityCard = !row.IsNull("IdentityCard") ? row["IdentityCard"].ToString() : "";
                item.IdentityCardPlace = !row.IsNull("IdentityCardPlace") ? row["IdentityCardPlace"].ToString() : "";
                item.IdentityCardDate = !row.IsNull("IdentityCardDate") ? DateTime.Parse(row["IdentityCardDate"].ToString()) : DateTime.Parse("1900-01-01");
                item.IdentityCardAddress = !row.IsNull("IdentityCardAddress") ? row["IdentityCardAddress"].ToString() : "";
                item.Email = !row.IsNull("Email") ? row["Email"].ToString() : "";
                item.Phone = !row.IsNull("Phone") ? row["Phone"].ToString() : "";
                item.Address = !row.IsNull("Address") ? row["Address"].ToString() : "";
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("1900-01-01");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"].ToString()) : false;
                item.CompanyName = !row.IsNull("CompanyName") ? row["CompanyName"].ToString() : "";
                lst.Add(item);
            }

            return lst;
        }
    }
}