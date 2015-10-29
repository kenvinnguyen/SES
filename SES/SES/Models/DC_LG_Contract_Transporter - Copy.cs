using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DecaInsight.Models
{
    public class DC_LG_Contract_Transporter
    {
        public string  ContractID {get;set;}
	    public int TransporterID {get;set;}
        public string Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}