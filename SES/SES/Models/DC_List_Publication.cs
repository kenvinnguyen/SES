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
    public class DC_List_Publication
    {
        
        [AutoIncrement]
        public int Id { get; set; }
        public double Price { get; set; }
        public string Uint { set; get; }
        public string PublicationName { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }
        public string PublicationID { get; set; }
    }
}