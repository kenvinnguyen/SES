using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SES.Controllers
{    
    public class ErrorController : Controller
    {        
        public ActionResult NoAccess()
        {
            return PartialView("NoAccess");
        }

        public ActionResult NotFound()
        {
            return PartialView("NotFound");
        }

        public ActionResult ErrorPage()
        {
            return PartialView("ErrorPage");
        }

        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/ExcelImport"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
	}
}