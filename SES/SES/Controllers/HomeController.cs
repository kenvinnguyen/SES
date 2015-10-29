using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SES.Models;
using SES.Service;

using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.IO;

namespace SES.Controllers
{
    [Authorize]
    //[NoCache]
    public class HomeController : CustomController
    {        
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {                
                //IDbConnection dbConn = new OrmliteConnection().openConn();
                //var dict = new Dictionary<string, object>();
                //dict["asset"] = userAsset;
                //dict["activestatus"] = new CommonLib().GetActiveStatus();
                //var data = new Config_Announcement().GetPage(1,int.MaxValue,"");
                //dbConn.Close();
                return View();
            }
            else
                return RedirectToAction("NoAccess", "Error");
          
        }

        public ActionResult Partial()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewData["listConfig_Announcement"] = new Master_Announcement().GetAllSort("", "CreatedAt", "DESC");
                dbConn.Close();
                return PartialView("_Home", dict);
            }
            else
                return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Download(string urlFolder, string file)
        {
            //string fileName = "ExcelReport\\Exported\\InventoryStock\\" + file;
            string fileName = urlFolder + file;
            string fullPath = Path.Combine(Server.MapPath("~/"), fileName);
            //string fullPath = "D:\\HongHanh\\Source\\" + file;
            return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }
    
        public ActionResult PartialChangePass()
        {
            return PartialView("_ChangePass");
        }


    }
}