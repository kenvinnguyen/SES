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
    public class RuleCheckController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialRuleCheck()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return PartialView("RuleCheck", dict);
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
            var data = dbConn.Select<DC_RuleCheck>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(DC_RuleCheck item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.RuleName) && !string.IsNullOrEmpty(item.Value))
                {
                    var isExist = db.SingleOrDefault<DC_RuleCheck>("RuleID={0}", item.RuleID);
                    item.RuleName = !string.IsNullOrEmpty(item.RuleName) ? item.RuleName : "";
                    item.RuleType = !string.IsNullOrEmpty(item.RuleType) ? item.RuleType : "";
                    item.Value = !string.IsNullOrEmpty(item.Value) ? item.Value : "";
                    item.FromDate = item.FromDate != null ? item.FromDate : DateTime.Now;
                    item.EndDate = item.EndDate != null ? item.EndDate : DateTime.Now;

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã Rule đã tồn tại!" });

                        string id = "";
                        var checkID = db.SingleOrDefault<DC_RuleCheck>("SELECT RuleID FROM DC_RuleCheck ORDER BY RuleID DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.RuleID.Substring(2, checkID.RuleID.Length - 2)) + 1;
                            id = "RC" + String.Format("{0:000000}", nextNo);
                        }
                        else
                        {
                            id = "RC000001";
                        }
                        item.RuleID = id;
                        item.RuleName = !string.IsNullOrEmpty(item.RuleName) ? item.RuleName : "";
                        item.RuleType = !string.IsNullOrEmpty(item.RuleType) ? item.RuleType : "";
                        item.Value = !string.IsNullOrEmpty(item.Value) ? item.Value : "";
                        item.CreatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedAt= DateTime.Parse("1900-01-01");
                        item.UpdatedBy = "";
                        db.Insert(item);
                        return Json(new { success = true, RuleID = item.RuleID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.RuleName = !string.IsNullOrEmpty(item.RuleName) ? item.RuleName : "";
                        item.RuleType = !string.IsNullOrEmpty(item.RuleType) ? item.RuleType : "";
                        item.Value = !string.IsNullOrEmpty(item.Value) ? item.Value : "";
                        item.Status = item.Status;
                        item.FromDate = item.FromDate!=null ? item.FromDate : DateTime.Now;
                        item.EndDate = item.EndDate!= null? item.EndDate : DateTime.Now;
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
                log.Error("Rule" + item.RuleID + " - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetRuleByID(string RuleID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.SingleOrDefault<DC_RuleCheck>("RuleID={0}", RuleID);
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
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/RuleKiemtra.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<DC_RuleCheck>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {

                    ws.Cells["A" + rowNum].Value = item.RuleID;
                    ws.Cells["B" + rowNum].Value = item.RuleName;
                    ws.Cells["C" + rowNum].Value = item.RuleType;
                    ws.Cells["D" + rowNum].Value = item.Value; 
                    ws.Cells["E" + rowNum].Value = item.CreatedAt;
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
                        "RuleKiemtra" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        


    }
}
