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

namespace SES.Controllers
{
    public class PrinterController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialPrinter()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE IsActive = 1");
                dbConn.Close();
                return PartialView("_Printer", dict);
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
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<DC_AD_Printer>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(DC_AD_Printer item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.PrinterName)
                    )
                {
                    var isExist = db.SingleOrDefault<DC_AD_Printer>("PrinterID={0}", item.PrinterID);
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.DfltAddress = !string.IsNullOrEmpty(item.DfltAddress) ? item.DfltAddress : "";
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.WHAddress = !string.IsNullOrEmpty(item.WHAddress) ? item.WHAddress : "";
                    item.ShippingAddress = !string.IsNullOrEmpty(item.ShippingAddress) ? item.ShippingAddress : "";
                    item.ContactPhone = !string.IsNullOrEmpty(item.ContactPhone) ? item.ContactPhone : "";
                    item.ContactName = !string.IsNullOrEmpty(item.ContactName) ? item.ContactName : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã máy in đã tồn tại" });
                        string id = "";
                        var checkID = db.SingleOrDefault<DC_AD_Printer>("SELECT PrinterID, Id FROM dbo.DC_AD_Printer ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.PrinterID.Substring(2, checkID.PrinterID.Length - 2)) + 1;
                            id = "PR" + String.Format("{0:00000000}", nextNo);
                        }
                        else
                        {
                            id = "PR00000001";
                        }
                        item.PrinterID = id;
                        item.PrinterName = !string.IsNullOrEmpty(item.PrinterName) ? item.PrinterName : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = currentUser.UserID;
                        db.Insert(item);
                        return Json(new { success = true, PrinterID = item.PrinterID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.PrinterName = !string.IsNullOrEmpty(item.PrinterName) ? item.PrinterName : "";
                        item.CreatedAt = item.CreatedAt;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
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
                log.Error("Printer - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetPrinterbycode(string PrinterID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.SingleOrDefault<DC_AD_Printer>("PrinterID={0}", PrinterID);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/DanhSachNhaIn.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<DC_AD_Printer>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {

                    ws.Cells["A" + rowNum].Value = item.PrinterID;
                    ws.Cells["B" + rowNum].Value = item.PrinterName;
                    ws.Cells["C" + rowNum].Value = item.DfltAddress;
                    ws.Cells["D" + rowNum].Value = item.ShippingAddress;
                    ws.Cells["E" + rowNum].Value = item.WHAddress;
                    ws.Cells["F" + rowNum].Value = item.Email;
                    ws.Cells["G" + rowNum].Value = item.Phone;
                    ws.Cells["H" + rowNum].Value = item.ContactName;
                    ws.Cells["I" + rowNum].Value = item.ContactPhone;
                    ws.Cells["J" + rowNum].Value = item.CreatedBy;
                    ws.Cells["K" + rowNum].Value = DateTime.Parse(item.CreatedAt.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["L" + rowNum].Value = item.Note;
                    ws.Cells["M" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "DanhSachNhaIn" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
