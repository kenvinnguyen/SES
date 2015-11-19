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
using System.Globalization;
using DecaInsight.Models;

namespace SES.Controllers
{
    public class OP_SalesOrderController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialDetail(string id)
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listVendor"] = dbConn.Select<Vendor>("select VendorID, VendorName from Vendor");
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<INUnit>(p => p.Status == true);
                dict["SONumber"] = id;
                if (string.IsNullOrEmpty(id))
                {
                    return PartialView("_OP_CreateOrderIndex", dict);
                }
                else
                {
                    return PartialView("_OP_CreateOrder", dict);
                }
            }
            else
            {
                return RedirectToAction("NoAccess", "Error");
            }
        }
        public ActionResult PartialCreate(string id)
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listVendor"] = dbConn.Select<Vendor>("select VendorID,VendorName from Vendor");
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<INUnit>(p => p.Status == true);
                dict["SONumber"] = id;
                return PartialView("_OP_CreateOrder", dict);
            }
            else
            {
                return RedirectToAction("NoAccess", "Error");
            }
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            var data = new List<Products>();
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                data = dbConn.Select<Products>(whereCondition).ToList();
            }
            else
            {
                data = dbConn.Select<Products>().ToList();
            }
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult ReadDetail([DataSourceRequest] DataSourceRequest request, string SONumber)
        {
            var dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Select<SODetail>("SELECT * FROM SODetail WHERE SONumber = '" + SONumber + "' ");
            return Json(data.ToDataSourceResult(request));
        }
        //public ActionResult ReadDetail([DataSourceRequest] DataSourceRequest request, string SONumber)
        //{
        //    log4net.Config.XmlConfigurator.Configure();
        //    string whereCondition = "";
        //    if (request.Filters.Count > 0)
        //    {
        //        whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
        //    }
        //    var data = new DC_AD_SO_Detail().GetPage(request, whereCondition, SONumber);
        //    return Json(data);
        //}
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Products> list, string SONumber)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                if (list != null && ModelState.IsValid)
                {
                    foreach (var item in list)
                    {
                        if (item.Qty > 0)
                        {
                            //string SONumber = Request["SONumber"];
                            var header = new SOHeader();
                            var detail = new SODetail();

                            if (dbConn.Select<SODetail>(s => s.SONumber == SONumber && s.ItemCode == item.Code).Count() > 0)
                            {
                                dbConn.Update<SODetail>(set: "Qty = '" + item.Qty + "', TotalAmt = '" + item.Qty * item.Price + "'", where: "SONumber = '" + SONumber + "'");
                            }
                            else
                            {
                                var data = new SODetail();
                                data.ItemName = item.Name;
                                data.ItemCode = item.Code;
                                data.Note = "";
                                data.Price = item.VATPrice;
                                data.Qty = item.Qty;
                                data.SONumber = SONumber;
                                data.UnitID = item.Unit;
                                data.UnitName = dbConn.Select<INUnit>(s => s.UnitID == item.Unit).FirstOrDefault().UnitName;
                                data.TotalAmt = item.Qty * item.VATPrice;
                                data.Status = "";
                                data.CreatedAt = DateTime.Now;
                                data.CreatedBy = currentUser.UserID;
                                data.UpdatedAt = DateTime.Parse("1900-01-01");
                                data.UpdatedBy = "";
                                dbConn.Insert<SODetail>(data);
                            }
                            dbConn.Update<SOHeader>(set: "TotalQty ='" + dbConn.Select<SODetail>(s => s.SONumber == SONumber).Sum(s => s.Qty) + "', TotalAmt = '" + +dbConn.Select<SODetail>(s => s.SONumber == SONumber).Sum(s => s.TotalAmt) + "'", where: "SONumber ='" + SONumber + "'");

                        }
                        else
                        {
                            dbConn.Delete<SOHeader>(s => s.SONumber == SONumber);
                            ModelState.AddModelError("error", "Đơn hàng được tạo khi số lượng > 0");
                            return Json(list.ToDataSourceResult(request, ModelState));
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
        public ActionResult CreateHeader(SOHeader item)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                try
                {
                    string SONumber = Request["SONumber"];
                    var header = new SOHeader();
                    var detail = new SODetail();
                    if (string.IsNullOrEmpty(SONumber))
                    {
                        string datetimeSO = DateTime.Now.ToString("yyMMdd");
                        var existSO = dbConn.SingleOrDefault<SOHeader>("SELECT id, SONumber FROM SOHeader ORDER BY Id DESC");
                        if (existSO != null)
                        {
                            var nextNo = Int32.Parse(existSO.SONumber.Substring(8, 5)) + 1;
                            SONumber = "SO" + datetimeSO + String.Format("{0:00000}", nextNo);
                        }
                        else
                        {
                            SONumber = "SO" + datetimeSO + "00001";
                        }
                    }
                    if (string.IsNullOrEmpty(Request["VendorID"]))
                    {
                        return Json(new { message = "Nhà cung cấp không tồn tai." });
                    }
                    if (!string.IsNullOrEmpty(Request["SODate"]))
                    {
                        DateTime fromDateValue;
                        if (!DateTime.TryParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                        {
                            return Json(new { message = "Ngày tạo không đúng." });
                        }

                    }
                    header.SONumber = SONumber;
                    header.SODate = !string.IsNullOrEmpty(Request["SODate"]) ? DateTime.Parse(DateTime.ParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")) : DateTime.Now;
                    header.VendorID = !string.IsNullOrEmpty(Request["VendorID"]) ? Request["VendorID"] : "";
                    header.Note = !string.IsNullOrEmpty(Request["Note"]) ? Request["Note"] : "";
                    header.TotalQty = 0;
                    header.WHID = "";
                    header.Status = "Mới";
                    header.WHLID = "";
                    header.TotalAmt = 0;
                    header.CreatedBy = currentUser.UserID;
                    header.CreatedAt = DateTime.Now;
                    header.UpdatedBy = "";
                    header.UpdatedAt = DateTime.Parse("1900-01-01");
                    dbConn.Insert<SOHeader>(header);
                    return Json(new { success = true, SONumber = SONumber });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Không có quyền tạo." });
            }
        }
        public ActionResult UpdateDetail([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SODetail> list)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {

                if (list != null && ModelState.IsValid)
                {
                    foreach (var item in list)
                    {
                        if (dbConn.Select<SOHeader>(s => s.SONumber == item.SONumber && s.Status != "Mới").Count() > 0)
                        {
                            return Json(new { success = false, message = "Đơn hàng đã xác nhận nên không được xóa." });
                        }
                        else if (item.Qty > 0)
                        {
                            dbConn.Update<SODetail>(set: "Qty = '" + item.Qty + "', TotalAmt = '" + item.Qty * item.Price + "',UpdatedAt = '" + DateTime.Now + "', UpdatedBy ='" + currentUser.UserID + "'", where: "SONumber = '" + item.SONumber + "' AND ItemCode ='" + item.ItemCode + "'");
                            dbConn.Update<SOHeader>(set: "TotalQty ='" + dbConn.Select<SODetail>(s => s.SONumber == item.SONumber).Sum(s => s.Qty) + "', TotalAmt = '" + +dbConn.Select<SODetail>(s => s.SONumber == item.SONumber).Sum(s => s.TotalAmt) + "'", where: "SONumber ='" + item.SONumber + "'");
                        }
                        else
                        {
                            ModelState.AddModelError("error", "Đơn hàng được tạo khi số lượng > 0");
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                return Json(new { sussess = true });
            }
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                return Json(list.ToDataSourceResult(request, ModelState));
            }
        }
        public ActionResult DeleteDetail(string data, string SONumber)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (dbConn.Select<SOHeader>(s => s.SONumber == SONumber && s.Status != "Mới").Count() > 0)
                    {
                        return Json(new { success = false, message = "Đơn hàng đã xác nhận nên không được xóa." });
                    }
                    foreach (var item in listdata)
                    {
                        dbConn.Delete<SODetail>(s => s.Id == int.Parse(item));
                    }
                    if (dbConn.Select<SODetail>(s => s.SONumber == SONumber).Count() <= 0)
                    {
                        dbConn.Delete<SOHeader>(s => s.SONumber == SONumber);
                    }
                    return Json(new { success = true });
                }

                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }

            }
            else
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa dữ liệu." });
            }
        }

        public ActionResult GetBySONumber(string id)
        {
            try
            {
                var dbConn = new OrmliteConnection().openConn();
                var data = dbConn.Select<SOHeader>("SELECT * FROM SOHeader WHERE SONumber ='" + id + "'").FirstOrDefault();
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
	}
}