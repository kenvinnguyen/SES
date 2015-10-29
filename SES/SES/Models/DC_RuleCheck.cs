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
    public class DC_RuleCheck
    {
      
        public string RuleID { get; set; }
        public string RuleName { set; get; }
        public string RuleType { get; set; }
        public string Value { get; set; }
        public Boolean Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public string CreatedBy { get; set; }    
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
       
      
    }
}