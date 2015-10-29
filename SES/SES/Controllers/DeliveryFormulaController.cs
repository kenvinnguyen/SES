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
    public class DeliveryFormulaController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialDeliveryFormula()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return PartialView("DeliveryFormula", dict);
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
            var data = dbConn.Select<DC_Formula>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(DC_Formula item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.FormulaName) && !string.IsNullOrEmpty(item.Formula))
                {
                    var isExist = db.SingleOrDefault<DC_Formula>("FormulaID={0}", item.FormulaID);
                    item.FormulaName = !string.IsNullOrEmpty(item.FormulaName) ? item.FormulaName : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.Formula = !string.IsNullOrEmpty(item.Formula) ? item.Formula : "";

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã công thức đã tồn tại!" });

                        string id = "";
                        var checkID = db.SingleOrDefault<DC_Formula>("SELECT FormulaID,ID FROM DC_Formula ORDER BY ID DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.FormulaID.Substring(2, checkID.FormulaID.Length - 2)) + 1;
                            id = "FM" + String.Format("{0:000000}", nextNo);
                        }
                        else
                        {
                            id = "FM000001";
                        }
                        item.FormulaID = id;
                        item.FormulaName = !string.IsNullOrEmpty(item.FormulaName) ? item.FormulaName : "";
                        item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                        item.Formula = !string.IsNullOrEmpty(item.Formula) ? item.Formula : "";
                        item.CreatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                        item.UpdatedBy = "";
                        db.Insert(item);
                        return Json(new { success = true, FormulaID = item.FormulaID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.FormulaName = !string.IsNullOrEmpty(item.FormulaName) ? item.FormulaName : "";
                        item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                        item.Formula = !string.IsNullOrEmpty(item.Formula) ? item.Formula : "";
                        item.Status = item.Status;
                        item.CreatedAt = item.CreatedAt;
                        item.CreatedBy = currentUser.UserID;
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
                log.Error("Formula" + item.FormulaID + " - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetFormulaByID(string FormulaID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.SingleOrDefault<DC_Formula>("FormulaID={0}", FormulaID);
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
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/CongThucTinhPhiVanChuyen.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<DC_Formula>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {

                    ws.Cells["A" + rowNum].Value = item.FormulaID;
                    ws.Cells["B" + rowNum].Value = item.FormulaName;
                    ws.Cells["C" + rowNum].Value = item.Formula;
                    ws.Cells["D" + rowNum].Value = item.Note;
                    ws.Cells["E" + rowNum].Value = Convert.ToDateTime(item.CreatedAt).ToString("dd/MM/yyyy");
                    ws.Cells["F" + rowNum].Value = item.CreatedBy;
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
                        "CongThucTinhPhiVanChuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }



    }
}
