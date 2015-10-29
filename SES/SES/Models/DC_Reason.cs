using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SES.Models
{
    public class DC_Reason
    {
        public string ReasonID { get; set; }
        public string ReasonType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }
    }
}