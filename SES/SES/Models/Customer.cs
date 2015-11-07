using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Agent { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Shoptype { get; set; }
        public string LevelHirerachy1 { get; set; }
        public string LevelHirerachy2 { get; set; }
        public string LevelHirerachy3 { get; set; }
        public string LevelHirerachy4 { get; set; }
        public string Desc { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Birthday { get; set; }
        public string ProvinceID { get; set; }
        public string DistrictID { get; set; }
        public string Gender { get; set; }

    }
}