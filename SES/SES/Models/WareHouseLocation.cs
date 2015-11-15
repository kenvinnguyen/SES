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
    public class WareHouseLocation
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string WHID { set; get; }
        public string WHLID { set; get; }
        public string WHLName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public Boolean Status { get; set; }
        public string Note { get; set; }
        [Ignore]
        public string WHName { get; set; }
    }
}