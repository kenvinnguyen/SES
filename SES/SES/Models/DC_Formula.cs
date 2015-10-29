using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class DC_Formula
    {
        public string FormulaID { get; set; }
        public string FormulaName { get; set; }
        public string Formula { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}