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
    public class SalesPersonController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /SalesPerson/
        public ActionResult PartialSalesPerson()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listSalesPerson"] = dbConn.Select<SalesPerson>(p => p.Status == true);
                dict["listCompany"] = dbConn.Select<Company>(p => p.Status == true);
                dbConn.Close();
                return PartialView("_SalesPerson", dict);
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
            var data = new SalesPerson().GetList(request.Page, request.PageSize, whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create(SalesPerson item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.SalesPersonID) &&
                    !string.IsNullOrEmpty(item.SalesPersonName)
                    )
                {
                    var isExist = db.SingleOrDefault<SalesPerson>("SalesPersonID={0}", item.SalesPersonID);

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã nhân viên đã tồn tại" });
                        item.SalesPersonName = !string.IsNullOrEmpty(item.SalesPersonName) ? item.SalesPersonName : "";
                        item.CompanyID = !string.IsNullOrEmpty(item.CompanyID) ? item.CompanyID : "";
                        //item.Gender = item.Gender;
                        item.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : "";
                        item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                        //item.DateOfBirth = !string.IsNullOrEmpty(item.MobilePhone) ? item.MobilePhone : "";
                        item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = currentUser.UserID;
                        db.Insert<SalesPerson>(item);

                        return Json(new { success = true, SalesPersonID = item.SalesPersonID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.SalesPersonName = !string.IsNullOrEmpty(item.SalesPersonName) ? item.SalesPersonName : "";
                        item.CompanyID = !string.IsNullOrEmpty(item.CompanyID) ? item.CompanyID : "";
                        //item.Gender = item.Gender;
                        item.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : "";
                        item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                        //item.DateOfBirth = !string.IsNullOrEmpty(item.MobilePhone) ? item.MobilePhone : "";
                        item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                        item.CreatedBy = isExist.CreatedBy;
                        item.CreatedAt = isExist.CreatedAt;
                        item.UpdatedAt = DateTime.Now;
                        item.UpdatedBy = currentUser.UserID;
                        db.Update<SalesPerson>(item);

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
                log.Error("SalesPerson - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/NhanVienBanHang.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                IDbConnection db = new OrmliteConnection().openConn();
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                var lstResult = new SalesPerson().GetList(1, 9999999, whereCondition).ToList();
                //var lstResult = db.Select<DC_LG_Contract>(whereCondition).ToList();
                int rowNum = 2;
                foreach (var item in lstResult)
                {

                    ws.Cells["A" + rowNum].Value = item.SalesPersonID;
                    ws.Cells["B" + rowNum].Value = item.SalesPersonName;
                    ws.Cells["C" + rowNum].Value = item.CompanyName;
                    ws.Cells["D" + rowNum].Value = item.Gender ? "Nam" : "Nữ"; ;
                    ws.Cells["E" + rowNum].Value = item.Phone;
                    ws.Cells["F" + rowNum].Value = item.Email;
                    ws.Cells["G" + rowNum].Value = item.Address;
                    ws.Cells["H" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
                    ws.Cells["I" + rowNum].Value = item.CreatedBy;
                    ws.Cells["J" + rowNum].Value = DateTime.Parse(item.CreatedAt.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["K" + rowNum].Value = item.UpdatedBy;
                    ws.Cells["L" + rowNum].Value = DateTime.Parse(item.UpdatedAt.ToString()).ToString("dd/MM/yyyy");
                    
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
                        "NhanVienBanHang" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
	}
}