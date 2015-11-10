using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class Vendor
    {
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string ProvinceID { get; set; }
        public string Website { get; set; }
        public DateTime? SignOffDate { get; set; }
        public string Url { get; set; }
        public string Hotline { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}