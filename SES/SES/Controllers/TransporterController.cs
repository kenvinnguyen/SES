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
    public class TransporterController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialTransporter()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                // ViewBag.listRegion = dbConn.Select<Master_Territory>("Select TerritoryID, TerritoryName from Master_Territory where Level='Region' ");
                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE Status = 1");
                dbConn.Close();
                return PartialView("_Transporter", dict);
               
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        //
        // GET: /DeliveryManage/Details/5
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition =  new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<DC_LG_Transporter>(whereCondition).OrderBy(p=>p.Weight);
           
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Read_TransporterLocation([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new DC_LG_Transporter_Location().GetPage(request.Page,request.PageSize, whereCondition);
            return Json(data);
        }
        public ActionResult Read_TransporterLocation_Territory([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new DC_LG_Transporter_Location_Territory().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }
        //
        // GET: /DeliveryManage/Create
        public ActionResult Create(DC_LG_Transporter item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (//!string.IsNullOrEmpty(item.TransporterID) &&
                    !string.IsNullOrEmpty(item.TransporterName) 
                    )
                {
                    int n;
                    var isExist = db.SingleOrDefault<DC_LG_Transporter>("TransporterID={0}",item.TransporterID);
                    item.Weight = int.TryParse(item.Weight.ToString(),out n) ? item.Weight : 0;
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if(isExist != null)
                            return Json(new { success = false, message = "Mã đơn vị vận chuyển đã tồn tại!" });
                        item.TransporterName = !string.IsNullOrEmpty(item.TransporterName) ? item.TransporterName : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = currentUser.UserID;
                        db.Insert(item);
                        return Json(new { success = true, TransporterID = item.TransporterID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.TransporterName = !string.IsNullOrEmpty(item.TransporterName) ? item.TransporterName : "";
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
                    return Json(new { success = false, message = "Chưa nhập giá trị" });
                }
            }
            catch (Exception e)
            {
                log.Error("Transporter - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetDeliveryByCode(string TransporterID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.SingleOrDefault<DC_LG_Transporter>("TransporterID={0}", TransporterID); 
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
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/DeliveryUOM.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition =  new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<DC_LG_Transporter>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TransporterID;
                    ws.Cells["B" + rowNum].Value = item.TransporterName;
                    ws.Cells["C" + rowNum].Value = item.Weight;
                    ws.Cells["D" + rowNum].Value = item.Note;
                    ws.Cells["E" + rowNum].Value = item.CreatedBy;
                    ws.Cells["F" + rowNum].Value = Convert.ToDateTime(item.CreatedAt).ToString("dd/MM/yyyy");
                    ws.Cells["G" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "DanhSachDonViVanChuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_TransporterLocation([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/DiaBanVanChuyen.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new DC_LG_Transporter_Location().Getlist(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TransporterLocationID;
                    ws.Cells["B" + rowNum].Value = item.TransporterLocationName;
                    ws.Cells["C" + rowNum].Value = item.IsAllMerchant? 1 : 0;
                    ws.Cells["D" + rowNum].Value = item.TransporterID;
                    ws.Cells["E" + rowNum].Value = item.TransporterName;
                    ws.Cells["F" + rowNum].Value = item.Note;
                    ws.Cells["G" + rowNum].Value = item.CreatedBy;
                    ws.Cells["H" + rowNum].Value = Convert.ToDateTime(item.CreatedAt).ToString("dd/MM/yyyy");
                    ws.Cells["I" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "DiaBanVanChuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_TransporterLocation_Territory([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/DiaBanVanChuyenTheoVung.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new DC_LG_Transporter_Location_Territory().Getlist(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TransporterLocationID;
                    ws.Cells["B" + rowNum].Value = item.TransporterLocationName;
                    ws.Cells["C" + rowNum].Value = item.ProvinceID;
                    ws.Cells["D" + rowNum].Value = item.ProvinceName;
                    ws.Cells["E" + rowNum].Value = item.DistrictID;
                    ws.Cells["F" + rowNum].Value = item.DistrictName;
                    ws.Cells["G" + rowNum].Value = item.Note;
                    ws.Cells["H" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";

                    //ws.Cells["F" + rowNum].Value = item.PickingProvinceID + "-" + item.PickingProvinceName;
                    //ws.Cells["G" + rowNum].Value = item.CreatedBy;
                    //ws.Cells["H" + rowNum].Value = Convert.ToDateTime(item.CreatedAt).ToString("dd/MM/yyyy");
                    //ws.Cells["I" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "DiaBanVanChuyenTheoVung" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
