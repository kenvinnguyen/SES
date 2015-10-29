using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class DC_AD_Printer
    {
        public string PrinterID { get; set; }
        public string PrinterName { get; set; }
        public string DfltAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string WHAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}