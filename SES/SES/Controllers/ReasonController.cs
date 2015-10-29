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
    public class ReasonController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialReason()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return PartialView("_Reason", dict);
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
            var data = dbConn.Select<DC_Reason>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }

        //
        // GET: /DeliveryManage/Create
        public ActionResult Create(DC_Reason item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.ReasonID) && item.ReasonType!="None")
                {
                    var isExist = db.GetByIdOrDefault<DC_Reason>(item.ReasonID);
                    item.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.RowCreatedAt == null && item.RowCreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã lý do đã tồn tại!" });
                        item.ReasonType = !string.IsNullOrEmpty(item.ReasonType) ? item.ReasonType : "";
                        item.RowCreatedAt = DateTime.Now;
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<DC_Reason>(item);
                        return Json(new { success = true, ReasonID = item.ReasonID, RowCreatedBy = item.RowCreatedBy, RowCreatedAt = item.RowCreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.ReasonType = !string.IsNullOrEmpty(item.ReasonType) ? item.ReasonType : "";
                        item.RowCreatedAt = item.RowCreatedAt;
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Update<DC_Reason>(item);
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
                log.Error("DeliveryUOMManage - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetReasonyCode(string ReasonID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<DC_Reason>(ReasonID);
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
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/Reason.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<DC_Reason>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.ReasonID;
                    ws.Cells["B" + rowNum].Value = item.ReasonType;
                    ws.Cells["C" + rowNum].Value = item.Description;
                    ws.Cells["D" + rowNum].Value = item.IsActive ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "Reason" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
