using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DecaInsight.Models;
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
using System.Globalization;
using SES.Service;
using SES.Models;


namespace DecaInsight.Controllers
{
    public class AD_PickingListController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialPickingList()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<DC_AD_Unit>(p => p.Status == true);
                dict["ListPrinter"] = dbConn.Select<DC_AD_Printer>(p => p.Status == true);
                dbConn.Close();
                return PartialView("PickingList", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult PartialDetail(string id)
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["listItem"] = dbConn.Select<Products>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<DC_AD_Unit>(p => p.Status == true);
                dict["PickingNumber"] = id;
                return PartialView("PickingListDetail", dict);
            }
            else
            {
                return Json(new { succsess = false, message = "Không có quyền tạo." });
            }
        }
        public ActionResult ReadHeader([DataSourceRequest] DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            var data = new List<DC_AD_Picking_Header>();
            if (request.Filters.Any())
            {
                var where = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                data = dbConn.Select<DC_AD_Picking_Header>(where);
            }
            else
            {
                data = dbConn.Select<DC_AD_Picking_Header>();
            }
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult ReadDetail([DataSourceRequest] DataSourceRequest request, string text)
        {
            var dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == text).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult GetByPickingNumber(string id)
        {
            try
            {
                var dbConn = new OrmliteConnection().openConn();
                var data = dbConn.Select<DC_AD_Picking_Header>(s => s.PickingNumber == id).FirstOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        public ActionResult UpdateDetail([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<DC_AD_Picking_Detail> list)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                if (list != null && ModelState.IsValid)
                {
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item.PickingNumber))
                        {
                            ModelState.AddModelError("", "Số picking không tồn tại");
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }
                        if (item.Qty <= 0)
                        {
                            ModelState.AddModelError("", "Số lượng phải lớn hơn 0.");
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }
                        dbConn.Update<DC_AD_Picking_Detail>(set: "Qty = '" + item.Qty + "', TotalAmt = '" + item.Price * item.Qty + "'", where: "ID = '" + item.Id + "'");
                        var success = dbConn.Execute(@"UPDATE DC_AD_Picking_Header Set TotalQty = @TotalQty, TotalAmt =@TotalAmt
                            WHERE PickingNumber = '" + item.PickingNumber +"'", new
                            {
                                TotalQty = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == item.PickingNumber).Sum(s => s.Qty),
                                TotalAmt = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == item.PickingNumber).Sum(s => s.TotalAmt),
                            }) == 1;
                        if (!success)
                        {
                            return Json(new { success = false, message = "Không thể lưu" });
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e.Message);
                return Json(list.ToDataSourceResult(request, ModelState));
            }
            return Json(new { sussess = true });           
        }

        public ActionResult PickingOut(string data, string WHID, string WHLID)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                string[] separators = { "@@" };
                var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                string TransactionID = "";
                string datetimeSO = DateTime.Now.ToString("yyMMdd");
                var existSO = dbConn.SingleOrDefault<DC_AD_Transaction>("SELECT id, TransactionID FROM DC_AD_Transaction ORDER BY Id DESC");
                var detail = new DC_AD_Transaction();
                if (existSO != null)
                {
                    var nextNo = Int32.Parse(existSO.TransactionID.Substring(8, 5)) + 1;
                    TransactionID = "TS" + datetimeSO + String.Format("{0:00000}", nextNo);
                }
                else
                {
                    TransactionID = "TS" + datetimeSO + "00001";
                }
                foreach (var item in listdata)
                {
                    if (dbConn.Select<DC_AD_Picking_Header>(s => s.Status != "Mới" && s.PickingNumber == item).Count() > 0)
                    {
                        return Json(new { success = false, message = item + " đã được nhập kho rồi." });
                    }
                    foreach (var po in dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == item).ToList())
                    {
                        detail.TransactionID = TransactionID;
                        detail.TransactionType = "Out";
                        detail.TransactionDate = DateTime.Now;
                        detail.RefID = item;
                        detail.ItemCode = !string.IsNullOrEmpty(po.ItemCode) ? po.ItemCode : "";
                        detail.ItemName = !string.IsNullOrEmpty(po.ItemName) ? po.ItemName : "";
                        detail.UnitID = !string.IsNullOrEmpty(po.UnitID) ? po.UnitID : "";
                        detail.UnitName = !string.IsNullOrEmpty(po.UnitName) ? po.UnitName : "";
                        detail.Qty = po.Qty != null ? po.Qty : 0;
                        detail.Price = po.Price != null ? po.Price : 0;
                        detail.TotalAmt = po.TotalAmt != null ? po.TotalAmt : 0;
                        detail.WHID = !string.IsNullOrEmpty(WHID) ? WHID : "";
                        detail.WHLID = !string.IsNullOrEmpty(WHLID) ? WHLID : "";
                        detail.Note = !string.IsNullOrEmpty(po.Note) ? po.Note : "";
                        detail.Status = "Đang giao hàng";
                        detail.CreatedBy = currentUser.UserID;
                        detail.CreatedAt = DateTime.Now;
                        detail.UpdatedBy = "";
                        detail.UpdatedAt = DateTime.Parse("1900-01-01");
                        detail.PrinterID = dbConn.Select<DC_AD_Picking_Header>(s => s.PickingNumber == item).FirstOrDefault().PrinterID;
                        detail.PrinterName = dbConn.Select<DC_AD_Picking_Header>(s => s.PickingNumber == item).FirstOrDefault().PrinterName;
                        dbConn.Insert<DC_AD_Transaction>(detail);
                        dbConn.Update<SOHeader>(set: "Status = N'Đang giao hàng'", where: "SONumber = '" + po.SONumber + "'");
                    }
                    dbConn.Update<DC_AD_Picking_Header>(set: "Status = N'Đang giao hàng'", where: "PickingNumber = '" + item + "'");
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            return Json(new { success = true });
        }
        public ActionResult CompletePicking(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<DC_AD_Picking_Header>(s => s.Status == "Hoàn thành" && s.PickingNumber == item).Count() > 0)
                        {
                            return Json(new { success = false, message = item + " đã được hoàn thành trước đó." });
                        }

                        if (dbConn.Select<DC_AD_Picking_Header>(s =>s.Status != "Đang giao hàng" && s.PickingNumber==item).Count() > 0)
                        {
                            return Json(new { success = false, message = item + " vui lòng nhập kho trước khi hoàn thành." });
                        }
                        

                        dbConn.Update<DC_AD_Picking_Header>(set: "Status = N'Hoàn thành', UpdatedBy ='" + currentUser.UserID + "', UpdatedAt= '"+DateTime.Now+"'", where: "PickingNumber = '" + item + "'");
                        foreach (var so in dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == item).ToList())
                        {
                            dbConn.Update<SOHeader>(set: "Status = N'Hoàn thành'", where: "SONumber = '" + so.SONumber + "'");
                        }
                        //dbConn.Update<DC_AD_SO_Header>(set: "Status = N'Hoàn thành'", where: "SONumber = '" + item + "'");
                    }
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Bạn không có quyền hủy." });
            }
        }
	}
}