//using Kendo.Mvc.UI;
//using Kendo.Mvc.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using SES.Service;
//using SES.Models;
//using System.Text.RegularExpressions;
//using OfficeOpenXml;
//using System.IO;
//using System.Collections;
//using System.Configuration;
//using log4net;
//using System.Data;
//using ServiceStack.OrmLite;
//using ServiceStack.OrmLite.SqlServer;
//using System.Data.SqlClient;

//namespace SES.Controllers
//{
//    public class DeliveryPromotionController : CustomController
//    {
//        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//        public ActionResult PartialDeliveryPromotion()
//        {
//            if (userAsset.ContainsKey("View") && userAsset["View"])
//            {
//                IDbConnection dbConn = new OrmliteConnection().openConn();
//                var dict = new Dictionary<string, object>();
//                dict["asset"] = userAsset;
//                dict["activestatus"] = new CommonLib().GetActiveStatus();
//                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE IsActive = 1");
//                dbConn.Close();
//                return PartialView("_DeliveryPromotion", dict);
//            }
//            else
//                return RedirectToAction("NoAccess", "Error");
//        }
//        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
//        {
//            var dbConn = new OrmliteConnection().openConn();
//            string whereCondition = "";
//            if (request.Filters.Count > 0)
//            {
//                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
//            }
//            var data = dbConn.Select<DC_LG_Promotion>(whereCondition);
//            return Json(data.ToDataSourceResult(request));
//        }

//        //
//        // GET: /DeliveryManage/Create
//        public ActionResult Create(DC_LG_Promotion item)
//        {
//            IDbConnection db = new OrmliteConnection().openConn();
//            try
//            {
//                if (!string.IsNullOrEmpty(item.PromotionID) &&
//                    !string.IsNullOrEmpty(item.PromotionName)
//                    )
//                {
//                    var isExist = db.SingleOrDefault<DC_LG_Promotion>("PromotionID={0}", item.PromotionID);
//                    item.FromDate = item.FromDate!=null ? item.FromDate : DateTime.Now;
//                    item.EndDate = item.EndDate!= null? item.EndDate : DateTime.Now;
//                    if (item.FromDate > item.EndDate)
//                    {
//                        return Json(new { success = false, message = "Ngày kết thúc không thể lớn hơn " + item.FromDate });
//                    }
//                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
//                    item.PromotionType = !string.IsNullOrEmpty(item.PromotionType) ? item.PromotionType : "";
//                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
//                    {
//                        if (isExist != null)
//                            return Json(new { success = false, message = "Mã chương trình khuyến mãi đã tồn tại" });
//                        item.PromotionName = !string.IsNullOrEmpty(item.PromotionName) ? item.PromotionName : "";
//                        item.CreatedAt = DateTime.Now;
//                        item.UpdatedAt = DateTime.Now;
//                        item.CreatedBy = currentUser.UserID;
//                        db.Insert(item);
//                        return Json(new { success = true, PromotionID = item.PromotionID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
//                    }
//                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
//                    {
//                        item.PromotionName = !string.IsNullOrEmpty(item.PromotionName) ? item.PromotionName : "";
//                        item.CreatedAt = item.CreatedAt;
//                        item.UpdatedAt = DateTime.Now;
//                        item.CreatedBy = currentUser.UserID;
//                        db.Update(item);
//                        return Json(new { success = true });
//                    }
//                    else
//                        return Json(new { success = false, message = "Bạn không có quyền" });
//                }
//                else
//                {
//                    return Json(new { success = false, message = "Chưa nhập đủ giá trị" });
//                }
//            }
//            catch (Exception e)
//            {
//                log.Error("DeliveryPromotion - Create - " + e.Message);
//                return Json(new { success = false, message = e.Message });
//            }
//            finally { db.Close(); }
//        }
//        [HttpPost]
//        public ActionResult GetPromotionCode(string PromotionID)
//        {
//            IDbConnection dbConn = new OrmliteConnection().openConn();
//            try
//            {
//                var data = dbConn.SingleOrDefault<DC_LG_Promotion>("PromotionID={0}", PromotionID);
//                return Json(new { success = true, data = data });
//            }
//            catch (Exception e)
//            {
//                return Json(new { success = false, message = e.Message });
//            }
//            finally { dbConn.Close(); }
//        }
//        public FileResult Export([DataSourceRequest]DataSourceRequest request)
//        {
//            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ChuongTrinhKhuyenMai.xlsx")));
//            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
//            if (userAsset["Export"])
//            {
//                string whereCondition = "";
//                if (request.Filters.Count > 0)
//                {
//                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
//                }
//                IDbConnection db = new OrmliteConnection().openConn();
//                var lstResult = db.Select<DC_LG_Promotion>(whereCondition);
//                int rowNum = 2;
//                foreach (var item in lstResult)
//                {
                    
//                    ws.Cells["A" + rowNum].Value = item.PromotionID;
//                    ws.Cells["B" + rowNum].Value = item.PromotionName;
//                    ws.Cells["C" + rowNum].Value = item.PromotionType;
//                    ws.Cells["D" + rowNum].Value = Convert.ToDateTime(item.FromDate).ToString("dd/MM/yyyy");
//                    ws.Cells["E" + rowNum].Value = Convert.ToDateTime(item.EndDate).ToString("dd/MM/yyyy");
//                    ws.Cells["F" + rowNum].Value = item.Note;
//                    ws.Cells["G" + rowNum].Value = item.CreatedBy;
//                    ws.Cells["H" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
//                    rowNum++;
//                }
//                db.Close();
//            }
//            else
//            {
//                ws.Cells["A2:E2"].Merge = true;
//                ws.Cells["A2"].Value = "You don't have permission to export data.";
//            }
//            MemoryStream output = new MemoryStream();
//            pck.SaveAs(output);
//            return File(output.ToArray(), //The binary data of the XLS file
//                        "application/vnd.ms-excel", //MIME type of Excel files
//                        "ChuongTrinhKhuyenMai" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
//        }
//    }
//}
