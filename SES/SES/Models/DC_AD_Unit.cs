using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class DC_AD_Unit
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string UnitID { set; get; }
        public string UnitName { set; get; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public Boolean Status { get; set; }
        public string Note { get; set; }
    }
}