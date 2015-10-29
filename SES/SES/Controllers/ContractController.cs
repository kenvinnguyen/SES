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
    public class ContractController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialContract()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listTranporter"] = dbConn.Select<DC_LG_Transporter>(p => p.Status == true);
                //dict["listContractTransporter"] = new DC_LG_Contract().GetList(" AND 1=1");
                dbConn.Close();
                return PartialView("_Contract", dict);
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
            var  data = new DC_LG_Contract().GetList(request.Page,request.PageSize,whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(DC_LG_Contract item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.ContractID) &&
                    !string.IsNullOrEmpty(item.ContractName)
                    )
                {
                    var isExist = db.SingleOrDefault<DC_LG_Contract>("ContractID={0}", item.ContractID);
                    var data = Request["TransporterID"];
                    //string data = !string.IsNullOrEmpty(item.TransporterID) ? item.TransporterID : "";
                    double n;
                    item.StartDate = item.StartDate != null ? item.StartDate : DateTime.Now;
                    item.EndDate = item.EndDate != null ? item.EndDate : DateTime.Now;
                    item.DiscountPercent = double.TryParse(item.DiscountPercent.ToString(),out n) ? item.DiscountPercent/100 : 0;
                    if(item.StartDate>item.EndDate)
                    {
                        return Json(new { success = false, message = "Ngày kết thúc không thể lớn hơn " + item.StartDate });
                    }
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.DiscountPercent = !string.IsNullOrEmpty(item.DiscountPercent.ToString()) ? item.DiscountPercent : 0;
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã hợp đồng đã tồn tại" });
                        item.ContractName = !string.IsNullOrEmpty(item.ContractName) ? item.ContractName : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                        item.CreatedBy = currentUser.UserID;
                        db.Insert(item);
                        db.Delete<DC_LG_Contract_Transporter>(p => p.ContractID ==item.ContractID);
                        if (!string.IsNullOrEmpty(data))
                        {
                            string[] arr = data.Split(',');
                            foreach (string ite in arr)
                            {
                                var detail = new DC_LG_Contract_Transporter();
                                detail.ContractID = item.ContractID;
                                detail.TransporterID = int.Parse(ite);
                                detail.Note = "";
                                detail.UpdatedAt = DateTime.Now;
                                detail.CreatedAt = DateTime.Now;
                                detail.CreatedBy = currentUser.UserID;
                                detail.UpdatedBy = currentUser.UserID;
                                db.Insert<DC_LG_Contract_Transporter>(detail);
                            }
                        }
                        return Json(new { success = true, ContractID = item.ContractID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.ContractName = !string.IsNullOrEmpty(item.ContractName) ? item.ContractName : "";
                        item.CreatedBy = isExist.CreatedBy;
                        item.CreatedAt = isExist.CreatedAt;
                        item.UpdatedAt = DateTime.Now;
                        item.UpdatedBy = currentUser.UserID;
                        db.Update(item);
                        db.Delete<DC_LG_Contract_Transporter>(p => p.ContractID == item.ContractID);
                        if (!string.IsNullOrEmpty(data))
                        {
                            string[] arr = data.Split(',');
                            foreach (string ite in arr)
                            {
                                var detail = new DC_LG_Contract_Transporter();
                                detail.ContractID = item.ContractID;
                                detail.TransporterID = int.Parse(ite);
                                detail.Note = "";
                                detail.UpdatedAt = DateTime.Now;
                                detail.CreatedAt = DateTime.Now;
                                detail.CreatedBy = currentUser.UserID;
                                detail.UpdatedBy = currentUser.UserID;
                                db.Insert<DC_LG_Contract_Transporter>(detail);
                            }
                        }
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
                log.Error("Contract - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetTransByID(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var listStran = dbConn.Select<DC_LG_Contract_Transporter>("ContractID={0}",id);
                return Json(new { success = true, listtran = listStran });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        [HttpPost]
        public ActionResult AddTranporterToContract(string id, string data)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                db.Delete<DC_LG_Contract_Transporter>(p => p.ContractID == id);
                if (!string.IsNullOrEmpty(data))
                {
                    string[] arr = data.Split(',');
                    foreach (string item in arr)
                    {
                        var detail = new DC_LG_Contract_Transporter();
                        detail.ContractID = id;
                        detail.TransporterID = int.Parse(item);
                        detail.Note = "";
                        detail.UpdatedAt = DateTime.Now;
                        detail.CreatedAt = DateTime.Now;
                        detail.CreatedBy = currentUser.UserID;
                        detail.UpdatedBy = currentUser.UserID;
                        db.Insert<DC_LG_Contract_Transporter>(detail);
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }
        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/HopDongVanChuyen.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                IDbConnection db= new OrmliteConnection().openConn();
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                var lstResult = new DC_LG_Contract().GetList(1, 9999999, whereCondition).ToList();
                //var lstResult = db.Select<DC_LG_Contract>(whereCondition).ToList();
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    
                    ws.Cells["A" + rowNum].Value = item.ContractID;
                    ws.Cells["B" + rowNum].Value = item.ContractName;
                    ws.Cells["C" + rowNum].Value = item.TransporterID;
                    ws.Cells["D" + rowNum].Value = item.TransporterName;
                    ws.Cells["E" + rowNum].Value = item.DiscountPercent;
                    ws.Cells["F" + rowNum].Value = DateTime.Parse(item.StartDate.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["G" + rowNum].Value = DateTime.Parse(item.EndDate.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["H" + rowNum].Value = item.CreatedBy;
                    ws.Cells["I" + rowNum].Value = DateTime.Parse(item.CreatedAt.ToString()).ToString("dd/MM/yyyy");
                    ws.Cells["J" + rowNum].Value = item.UpdatedBy;
                    ws.Cells["K" + rowNum].Value = DateTime.Parse(item.UpdatedAt.ToString()).ToString("dd/MM/yyyy");
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
                        "HopDongVanChuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
