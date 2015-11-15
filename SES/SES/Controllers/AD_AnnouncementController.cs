using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SES.Service;
using SES.Models;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using System.Collections;
using System.Configuration;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;
using System.Globalization;

namespace SES.Controllers
{
    [Authorize]
    [NoCache]
    public class AD_AnnouncementController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Announcement().GetPage(request.Page, request.PageSize, whereCondition, currentUser.UserID);
            return Json(data);
        }

       [HttpPost]
        public ActionResult Create(Master_Announcement item)
        {

            //if (form.AllKeys.Contains("TextContent"))
            //{
            //    item.TextContent = form.Get("TextContent");
            //}
           
            //CHECK IS NULL VALUE
            if (string.IsNullOrEmpty(item.TextContent))
            {
                item.TextContent = "";
            }
            if (string.IsNullOrEmpty(item.HTMLContent))
            {
                item.HTMLContent = "";
            }
            if (string.IsNullOrEmpty(item.Title))
            {
                item.Title = "";
            }
           
            IDbConnection dbConn = new OrmliteConnection().openConn();

            try
            {
                    var isExist = dbConn.GetByIdOrDefault<Master_Announcement>(item.AnnouncementID);

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                        {
                            return Json(new { success = false, message = "Đối tượng này đã tồn tại." });
                        }
                        item.CreatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;

                        dbConn.Insert<Master_Announcement>(item);
                        long lastInsertId = dbConn.GetLastInsertId();
                        dbConn.Close();
                        return Json(new { success = true, AnnouncementID = lastInsertId, createdat = item.CreatedAt, createdby = item.CreatedBy });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        dbConn.Update<Master_Announcement>(item);
                        dbConn.Close();
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "You don't have permission" });
            }
            catch (Exception ex)
            {
                log.Error("AD_Announcement - Create - " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                dbConn.Close();
            }

        }
      
        [HttpPost]
        public ActionResult Deactive(string data)
        {
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                try
                {
                    var newdata = data.Split(',');
                    foreach (string id in newdata)
                    {
                        dbConn.Delete<Master_Announcement>(p => p.AnnouncementID == Convert.ToInt32(id));
                    }

                    return Json(new { success = true, });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });

                }
                finally
                {
                    dbConn.Close();
                }
            }
            else
            {
                return Json(new { success = false, message = "You don't have permission" });
            }
        }


        //=====================================================================================================        

        public ActionResult PartialAnnouncement()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return PartialView("_AD_Announcement", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }


        [HttpPost]
        public ActionResult GetByID(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Master_Announcement>(id);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
    
    }
}