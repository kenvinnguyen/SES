using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class Auth_User_Merchant
    {
        public string UserID { get; set; }
        public int PKMerchantID { get; set; }
        public string MerchantID { get; set; }
        public string MerchantName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}