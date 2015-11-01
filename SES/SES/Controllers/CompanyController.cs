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
using Dapper;

namespace SES.Controllers
{
    public class CompanyController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Company/
        public ActionResult PartialCompany()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listCompany"] = dbConn.Select<Company>(p => p.Status == true);
                dbConn.Close();
                return PartialView("_Company", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Company().GetList(request.Page, request.PageSize, whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create(Company item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.CompanyID) &&
                    !string.IsNullOrEmpty(item.CompanyName)
                    )
                {
                    var isExist = db.SingleOrDefault<Company>("CompanyID={0}", item.CompanyID);
                  
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã công ty đã tồn tại" });
                        item.CompanyName = !string.IsNullOrEmpty(item.CompanyName) ? item.CompanyName : "";
                        item.ShortName = !string.IsNullOrEmpty(item.ShortName) ? item.ShortName : "";
                        item.EnglishName = !string.IsNullOrEmpty(item.EnglishName) ? item.EnglishName : "";
                        item.SubDomain = !string.IsNullOrEmpty(item.SubDomain) ? item.SubDomain : "";
                        item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                        item.MobilePhone = !string.IsNullOrEmpty(item.MobilePhone) ? item.MobilePhone : "";
                        item.Fax = !string.IsNullOrEmpty(item.Fax) ? item.Fax : "";
                        item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                        item.PersonalEmail = !string.IsNullOrEmpty(item.PersonalEmail) ? item.PersonalEmail : "";
                        item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                        item.Website = !string.IsNullOrEmpty(item.Website) ? item.Website : "";
                        item.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = currentUser.UserID;
                        db.Insert<Company>(item);

                        return Json(new { success = true, CompanyID = item.CompanyID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.CompanyName = !string.IsNullOrEmpty(item.CompanyName) ? item.CompanyName : "";
                        item.ShortName = !string.IsNullOrEmpty(item.ShortName) ? item.ShortName : "";
                        item.EnglishName = !string.IsNullOrEmpty(item.EnglishName) ? item.EnglishName : "";
                        item.SubDomain = !string.IsNullOrEmpty(item.SubDomain) ? item.SubDomain : "";
                        item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                        item.MobilePhone = !string.IsNullOrEmpty(item.MobilePhone) ? item.MobilePhone : "";
                        item.Fax = !string.IsNullOrEmpty(item.Fax) ? item.Fax : "";
                        item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                        item.PersonalEmail = !string.IsNullOrEmpty(item.PersonalEmail) ? item.PersonalEmail : "";
                        item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                        item.Website = !string.IsNullOrEmpty(item.Website) ? item.Website : "";
                        item.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                        item.CreatedBy = isExist.CreatedBy;
                        item.CreatedAt = isExist.CreatedAt;
                        item.UpdatedAt = DateTime.Now;
                        item.UpdatedBy = currentUser.UserID;
                        db.Update(item);
                        
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" });
                }
                else
                {
                    return Json(new { success = false, message = "Chưa nhập đủ giá trị" });
                }
            }
            catch (Exception e)
            {
                log.Error("Company - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
    }
}
