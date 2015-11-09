using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using SES.Service;
using System.Data.SqlClient;
namespace SES.Models
{
    public class CustomerHirerachyViewModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get { return true; } }
        public List<CustomerHirerachyViewModel> items { get; set; }
        public bool @checked { get; set; }
    }
    public class CustomerHirerachy
    {
        public string CustomerHirerachyID { get; set; }
        public string ParentCustomerHirerachyID { get; set; }
        public string CustomerHirerachyName { get; set; }
        public int CustomerHirerachyIndex { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public List<CustomerHirerachy> GetAllCustomerHirerachy()
        {
            IDbConnection db = new OrmliteConnection().openConn();
            var lst = db.Select<CustomerHirerachy>().Where(p => p.Status == true).ToList();
            db.Close();
            return lst;
        }

    }

    public class CustomerHirerachyIsAllowed
    {
        public string CustomerHirerachyID { get; set; }
        public string ParentCustomerHirerachyID { get; set; }
        public string CustomerHirerachyName { get; set; }
        public int CustomerHirerachyIndex { get; set; }
        public string ControllerName { get; set; }
        public bool IsAllowed { get; set; }
    }

}