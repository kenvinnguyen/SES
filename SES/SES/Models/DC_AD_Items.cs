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
    public class DC_AD_Items
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
        
    }
}