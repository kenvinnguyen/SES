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

        [HttpPost]
        public ActionResult GetCompanyByID(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var listCompany = dbConn.Select<Company>("CompanyID={0}", id);
                return Json(new { success = true, listcompany = listCompany });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/CongTy.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                IDbConnection db= new OrmliteConnection().openConn();
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                var lstResult = new Company().GetList(1, 9999999, whereCondition).ToList();
                //var lstResult = db.Select<DC_LG_Contract>(whereCondition).ToList();
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    
                    ws.Cells["A" + rowNum].Value = item.CompanyID;
                    ws.Cells["B" + rowNum].Value = item.CompanyName;
                    ws.Cells["C" + rowNum].Value = item.Phone;
                    ws.Cells["D" + rowNum].Value = item.Fax;
                    ws.Cells["E" + rowNum].Value = item.Email;
                    ws.Cells["F" + rowNum].Value = item.Address;
                    ws.Cells["G" + rowNum].Value = item.Website;
                    ws.Cells["H" + rowNum].Value = item.CreatedBy;
                    ws.Cells["I" + rowNum].Value = DateTime.Parse(item.CreatedAt.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["J" + rowNum].Value = item.UpdatedBy;
                    ws.Cells["K" + rowNum].Value = DateTime.Parse(item.UpdatedAt.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["L" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
                    rowNum++;
                }
                db.Close();
            }
            else
            {
                ws.Cells["A2:E2"].Merge = true;
                ws.Cells["A2"].Value = "You don't have permission to export data.";
            }
            MemoryStream output = new MemoryStream();
            pck.SaveAs(output);
            return File(output.ToArray(), //The binary data of the XLS file
                        "application/vnd.ms-excel", //MIME type of Excel files
                        "CongTy" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
