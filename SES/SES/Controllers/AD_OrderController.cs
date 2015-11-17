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

namespace SES.Controllers
{
    public class AD_OrderController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialRole()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["user"] = dbConn.Select<Auth_User>(p => p.IsActive == true);
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<Products>(p => p.Status == true);
                dict["listMerchant"] = dbConn.Select<DC_OCM_Merchant>();
                dict["ListPrinter"] = dbConn.Select<DC_AD_Printer>(p => p.Status == true);
                dict["ListStatus"] = dbConn.Select<DC_Parameter>(s => s.Type == "SOStatus");
                dbConn.Close();
                return PartialView("AD_Order_Header", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult ReadHeader ([DataSourceRequest] DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            var data = new List<SOHeader>();
            if(request.Filters.Any())
            {
                var where = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                data = dbConn.Select<SOHeader>(where);
            }
            else{
                data = dbConn.Select<SOHeader>();
            }
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult ReadDetail([DataSourceRequest] DataSourceRequest request, string text)
        {
            var dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Select<SODetail>("SELECT * FROM SODetail WHERE SONumber ='" + text + "'");
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult PartialDetail(string id)
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listMerchant"] = dbConn.Select<DC_OCM_Merchant>("SELECT MerchantID, MerchantName FROM DC_OCM_Merchant");
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<WareHouseLocation>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<Products>(p => p.Status == true);
                dict["SONumber"] = id;
                return PartialView("AD_Order_Detail", dict);
            }
            else
            {
                return Json(new { succsess = false, message = "Không có quyền tạo." });
            }
        }
        public ActionResult ConfirmCreate()
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
                    if (!string.IsNullOrEmpty(Request["SODate"]))
                    {
                        DateTime fromDateValue;                     
                        if (!DateTime.TryParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                        {
                            return Json(new { message = "Ngày tạo không đúng." });
                        }
                       
                    }
                    if (dbConn.Select<DC_OCM_Merchant>(p => p.MerchantID == Request["MerchantID"]).Count() <= 0)
                    {
                        return Json(new { success = false, message = "Nhà cung cấp không tồn tại." });
                    }

                    if (dbConn.Select<SODetail>(p => p.SONumber == SONumber).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(Request["ItemCode"]))
                        {
                            if (dbConn.Select<Products>(p => p.Code == Request["ItemCode"]).Count() <= 0)
                            {
                                return Json(new { success = false, message = "Ấn phẩm không tồn tại." });
                            }
                            var itemcode = dbConn.Select<Products>(s => s.Code == Request["ItemCode"]).FirstOrDefault();
                            var itemunit = dbConn.Select<Products>(s => s.UnitID == itemcode.Unit).FirstOrDefault();
                            if (dbConn.Select<SODetail>(p => p.ItemCode == Request["ItemCode"] && p.SONumber == SONumber).Count() > 0)
                            {
                                var success = dbConn.Execute(@"UPDATE SODetail Set Qty = @Qty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy, Price = @Price
                                WHERE SONumber = '" + SONumber + "' AND ItemCode = '" + Request["ItemCode"] + "'", new
                                    {
                                        Qty = dbConn.Select<SODetail>(s => s.ItemCode == Request["ItemCode"] && s.SONumber == SONumber).Sum(s => s.Qty) + int.Parse(Request["Qty"]),
                                        TotalAmt = itemcode != null ? itemcode.VATPrice * (dbConn.Select<SODetail>(s => s.ItemCode == Request["ItemCode"] && s.SONumber == SONumber).Sum(s => s.Qty) + int.Parse(Request["Qty"])) : 0,
                                        Price = itemcode.VATPrice,
                                        UpdatedBy = currentUser.UserID,
                                        UpdatedAt = DateTime.Now,
                                    }) == 1;
                                if (!success)
                                {
                                    return Json(new { success = false, message = "Không thể lưu" });
                                }
                            }
                            else
                            {
                                detail.SONumber = SONumber;
                                detail.ItemCode = !string.IsNullOrEmpty(Request["ItemCode"]) ? Request["ItemCode"] : "";
                                detail.ItemName = !string.IsNullOrEmpty(itemcode.Name) ? itemcode.Name : "";
                                detail.Price = itemcode != null ? itemcode.VATPrice : 0;
                                detail.Qty = int.Parse(Request["Qty"]);
                                detail.TotalAmt = itemcode.VATPrice * int.Parse(Request["Qty"]);
                                detail.UnitID = !string.IsNullOrEmpty(itemunit.UnitID) ? itemunit.UnitID : "";
                                detail.UnitName = !string.IsNullOrEmpty(itemunit.UnitName) ? itemunit.UnitName : "";
                                detail.Note = "";
                                detail.Status = "";
                                detail.CreatedBy = currentUser.UserID;
                                detail.CreatedAt = DateTime.Now;
                                detail.UpdatedBy = "";
                                detail.UpdatedAt = DateTime.Parse("1900-01-01");
                                dbConn.Insert<SODetail>(detail);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Request["ItemCode"]) || dbConn.Select<Products>(p => p.Code == Request["ItemCode"]).Count() <= 0)
                        {
                            return Json(new { success = false, message = "Ấn phẩm không tồn tại." });
                        }
                        var itemcode = dbConn.Select<Products>(s => s.Code == Request["ItemCode"]).FirstOrDefault();
                        var itemunit = dbConn.Select<Products>(s => s.UnitID == itemcode.Unit).FirstOrDefault();
                        detail.SONumber = SONumber;
                        detail.ItemCode = !string.IsNullOrEmpty(Request["ItemCode"]) ? Request["ItemCode"] : "";
                        detail.ItemName = !string.IsNullOrEmpty(itemcode.Name) ? itemcode.Name : "";
                        detail.Price = itemcode != null ? itemcode.VATPrice :0 ;
                        detail.Qty = int.Parse(Request["Qty"]);
                        detail.TotalAmt = itemcode.VATPrice * int.Parse(Request["Qty"]);
                        detail.UnitID = !string.IsNullOrEmpty(itemunit.UnitID) ? itemunit.UnitID : "";
                        detail.UnitName = !string.IsNullOrEmpty(itemunit.UnitName) ? itemunit.UnitName : "";
                        detail.Note = "";
                        detail.Status = "";
                        detail.CreatedBy = currentUser.UserID;
                        detail.CreatedAt = DateTime.Now;
                        detail.UpdatedBy = "";
                        detail.UpdatedAt = DateTime.Parse("1900-01-01");
                        dbConn.Insert<SODetail>(detail);

                    }
                    if (dbConn.Select<SOHeader>(p => p.SONumber == SONumber).Count() > 0)
                    {
                        var success = dbConn.Execute(@"UPDATE SOHeader Set TotalQty = @TotalQty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy,
                        MerchantID = @MerchantID, SODate = @SODate , WHLID =@WHLID, WHID =@WHID WHERE SONumber = '" + SONumber + "'", new
                                {
                                    TotalQty = dbConn.Select<SODetail>(s =>s.SONumber == SONumber).Sum(s => s.Qty),
                                    TotalAmt = dbConn.Select<SODetail>(s => s.SONumber == SONumber).Sum(s => s.TotalAmt),
                                    SODate = !string.IsNullOrEmpty(Request["SODate"]) ? DateTime.Parse(DateTime.ParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")) : DateTime.Now,
                                    WHID = !string.IsNullOrEmpty(Request["WHID"]) ? Request["WHID"] : "",
                                    WHLID = !string.IsNullOrEmpty(Request["WHLID"]) ? Request["WHLID"] : "",
                                    MerchantID = !string.IsNullOrEmpty(Request["MerchantID"]) ? Request["MerchantID"] : "",
                                    UpdatedBy = currentUser.UserID,
                                    UpdatedAt = DateTime.Now,
                                }) == 1;
                        if (!success)
                        {
                            return Json(new { success = false, message = "Không thể lưu" });
                        }
                    }
                    else
                    {
                        header.SONumber = SONumber;
                        header.SODate = !string.IsNullOrEmpty(Request["SODate"]) ? DateTime.Parse(DateTime.ParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")) : DateTime.Now;
                        header.MerchantID = !string.IsNullOrEmpty(Request["MerchantID"]) ? Request["MerchantID"] : "";
                        header.Note = !string.IsNullOrEmpty(Request["Note"]) ? Request["Note"] : "";
                        header.TotalQty = dbConn.Select<SODetail>(s =>s.SONumber == SONumber).Sum(s => s.Qty);
                        header.WHID = Request["WHID"];
                        header.Status = "Mới";
                        header.WHLID = !string.IsNullOrEmpty(Request["WHLID"]) ? Request["WHLID"] : "";
                        header.TotalAmt = dbConn.Select<SODetail>(s => s.SONumber == SONumber).Sum(s => s.TotalAmt);
                        header.CreatedBy = currentUser.UserID;
                        header.CreatedAt = DateTime.Now;
                        header.UpdatedBy = "";
                        header.UpdatedAt = DateTime.Parse("1900-01-01");
                        dbConn.Insert<SOHeader>(header);
                    }                 

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
        public ActionResult CreateSONew()
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                string SONumber = String.Empty;
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
                    
                return Json(new { success = true, SONumber = SONumber });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message});
            }
        }
        public ActionResult GetMerchant(string text)
        {
            using (var dbConn = new OrmliteConnection().openConn())
            {
                var data = new List<DC_OCM_Merchant>();
                if (text.Length >= 3)
                {
                    data = dbConn.Query<DC_OCM_Merchant>("SELECT TOP 5 * FROM DC_OCM_Merchant WHERE MerchantName COLLATE Latin1_General_CI_AI LIKE N'%" + text + "%'");
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetItem(string text)
        {
            using (var dbConn = new OrmliteConnection().openConn())
            {
                var data = new List<Products>();
                if (text.Length >= 3)
                {
                    data = dbConn.Query<Products>("SELECT TOP 5 Name, Code, size FROM DC_AD_Items WHERE Name COLLATE Latin1_General_CI_AI LIKE N'%" + text + "%'");
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreatePO(string data, string printer, string delivery)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string PONumber = "";
                    string datetimeSO = DateTime.Now.ToString("yyMMdd");
                    var existSO = dbConn.SingleOrDefault<DC_AD_PO_Header>("SELECT id, PONumber FROM DC_AD_PO_Header ORDER BY Id DESC");
                    if (existSO != null)
                    {
                        var nextNo = Int32.Parse(existSO.PONumber.Substring(8, 5)) + 1;
                        PONumber = "PO" + datetimeSO + String.Format("{0:00000}", nextNo);
                    }
                    else
                    {
                        PONumber = "PO" + datetimeSO + "00001";
                    }
                    if (!string.IsNullOrEmpty(delivery))
                    {
                        DateTime fromDateValue;
                        if (!DateTime.TryParseExact(delivery, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                        {
                            return Json(new { message = "Ngày tạo không đúng." });
                        }
                    }
                    var detail = new DC_AD_PO_Detail();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<SOHeader>(s => s.Status != "Mới" && s.SONumber == item).Count() > 0)
                        {
                            return Json(new { success = false, message = "Không thể đặt hàng." });
                        }
                        foreach (var dtSO in dbConn.Select<SODetail>(s => s.SONumber == item).ToList())
                        {
                            if (dbConn.Select<DC_AD_PO_Detail>(s => s.PONumber == PONumber && s.ItemCode == dtSO.ItemCode).Count() > 0)
                            {
                                var success = dbConn.Execute(@"UPDATE DC_AD_PO_Detail Set Qty = @Qty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy, Price = @Price
                                    WHERE PONumber = '" + PONumber + "' AND ItemCode = '" + dtSO.ItemCode + "'", new
                                    {
                                        Qty = dbConn.Select<DC_AD_PO_Detail>(s => s.PONumber == PONumber && s.ItemCode == dtSO.ItemCode).Sum(s => s.Qty) + dtSO.Qty,
                                        TotalAmt = dbConn.Select<DC_AD_PO_Detail>(s => s.PONumber == PONumber && s.ItemCode == dtSO.ItemCode).Sum(s => s.TotalAmt)+dtSO.TotalAmt,
                                        Price = dtSO.Price,
                                        UpdatedBy = currentUser.UserID,
                                        UpdatedAt = DateTime.Now,
                                    }) == 1;
                                if (!success)
                                {
                                    return Json(new { success = false, message = "Không thể lưu" });
                                }
                            }
                            else
                            {
                                detail.PONumber = PONumber;
                                detail.ItemCode = !string.IsNullOrEmpty(dtSO.ItemCode) ? dtSO.ItemCode : "";
                                detail.ItemName = !string.IsNullOrEmpty(dtSO.ItemName) ? dtSO.ItemName : "";
                                detail.Price = dtSO.Price;
                                detail.Qty = dtSO.Qty;
                                detail.TotalAmt = dtSO.TotalAmt;
                                detail.UnitID = !string.IsNullOrEmpty(dtSO.UnitID) ? dtSO.UnitID : "";
                                detail.UnitName = !string.IsNullOrEmpty(dtSO.UnitName) ? dtSO.UnitName : "";
                                detail.Note = "";
                                detail.Status = "Mới";
                                detail.CreatedBy = currentUser.UserID;
                                detail.CreatedAt = DateTime.Now;
                                detail.UpdatedBy = "";
                                detail.UpdatedAt = DateTime.Parse("1900-01-01");
                                dbConn.Insert<DC_AD_PO_Detail>(detail);
                            }
                        }
                        dbConn.Update<SOHeader>(set: "Status = N'Đã đặt hàng'", where: "SONumber = '" + item + "'");
                    }
                    var header = new DC_AD_PO_Header();
                    header.PONumber = PONumber;
                    header.PODate = DateTime.Now;
                    header.DeliveryDate = !string.IsNullOrEmpty(delivery) ? DateTime.Parse(DateTime.ParseExact(delivery, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")) : DateTime.Now;
                    header.PrinterID = printer;
                    header.PrinterName = dbConn.Select<DC_AD_Printer>(s => s.PrinterID == printer).FirstOrDefault().PrinterName;
                    header.TotalQty = dbConn.Select<DC_AD_PO_Detail>(s => s.PONumber == PONumber).Sum(s => s.Qty);
                    header.TotalAmt = dbConn.Select<DC_AD_PO_Detail>(s => s.PONumber == PONumber).Sum(s => s.TotalAmt);
                    header.Note = "";
                    header.Status = "Nhà in đang xử lý";
                    header.CreatedBy = currentUser.UserID;
                    header.CreatedAt = DateTime.Now;
                    header.UpdatedBy = "";
                    header.UpdatedAt = DateTime.Parse("1900-01-01");
                    dbConn.Insert<DC_AD_PO_Header>(header);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Không có quyền tạo." });
            }
        }
        public ActionResult CreatePicking(string data, string printer, string pickingdate)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                try 
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string PickingNumber = "";
                    string datetimeSO = DateTime.Now.ToString("yyMMdd");
                    var existSO = dbConn.SingleOrDefault<DC_AD_Picking_Header>("SELECT id, PickingNumber FROM DC_AD_Picking_Header ORDER BY Id DESC");
                    if (existSO != null)
                    {
                        var nextNo = Int32.Parse(existSO.PickingNumber.Substring(8, 5)) + 1;
                        PickingNumber = "PK" + datetimeSO + String.Format("{0:00000}", nextNo);
                    }
                    else
                    {
                        PickingNumber = "PK" + datetimeSO + "00001";
                    }
                    if (!string.IsNullOrEmpty(pickingdate))
                    {
                        DateTime fromDateValue;
                        if (!DateTime.TryParseExact(pickingdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                        {
                            return Json(new {success = false, message = "Ngày tạo không đúng." });
                        }
                       
                        
                    }
                    //int TotalQty = 0;
                    //double TotalAmt = 0;
                    var detail = new DC_AD_Picking_Detail();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<SOHeader>(s => s.SONumber == item && s.Status != "Đã đặt hàng").Count() > 0)
                        {
                            return Json(new { success = false, massege = "Vui lòng đặt hàng " + item + " trước khi tạo Picking." });
                        }
                        if (dbConn.Select<DC_AD_Picking_Detail>(s => s.SONumber == item).Count() > 0)
                        {
                            return Json(new { success = false, massege = item + " Đă được đăt trước đó." });
                        }
                        foreach (var dtSO in dbConn.Select<SODetail>(s => s.SONumber == item).ToList())
                        {
                            if (dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == PickingNumber && s.ItemCode == dtSO.ItemCode && s.UnitID == dtSO.UnitID).Count() > 0)
                            {
                                var success = dbConn.Execute(@"UPDATE DC_AD_Picking_Detail Set Qty = @Qty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy, Price = @Price
                                WHERE PickingNumber = '" + PickingNumber + "' AND ItemCode = '" + dtSO.ItemCode + "' AND UnitID = '" + dtSO.UnitID+ "'", new
                                    {
                                        Qty = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == PickingNumber && s.ItemCode == dtSO.ItemCode && s.UnitID == dtSO.UnitID).Sum(s => s.Qty) + dtSO.Qty,
                                        TotalAmt = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == PickingNumber && s.ItemCode == dtSO.ItemCode && s.UnitID == dtSO.UnitID).Sum(s => s.TotalAmt) + dtSO.TotalAmt,
                                        Price = dtSO.Price,
                                        UpdatedBy = currentUser.UserID,
                                        UpdatedAt = DateTime.Now,
                                    }) == 1;
                                if (!success)
                                {
                                    return Json(new { success = false, message = "Không thể lưu" });
                                }
                            }
                            else
                            {
                                detail.PickingNumber = PickingNumber;
                                detail.SONumber = item;
                                detail.ItemCode = !string.IsNullOrEmpty(dtSO.ItemCode) ? dtSO.ItemCode : "";
                                detail.ItemName = !string.IsNullOrEmpty(dtSO.ItemName) ? dtSO.ItemName : "";
                                detail.Price = dtSO.Price;
                                detail.Qty = dtSO.Qty;
                                detail.TotalAmt = dtSO.TotalAmt;
                                detail.UnitID = !string.IsNullOrEmpty(dtSO.UnitID) ? dtSO.UnitID : "";
                                detail.UnitName = !string.IsNullOrEmpty(dtSO.UnitName) ? dtSO.UnitName : "";
                                detail.Note = "";
                                detail.Status = "";
                                detail.CreatedBy = currentUser.UserID;
                                detail.CreatedAt = DateTime.Now;
                                detail.UpdatedBy = "";
                                detail.UpdatedAt = DateTime.Parse("1900-01-01");
                                dbConn.Insert<DC_AD_Picking_Detail>(detail);
                            }
                        }
                        dbConn.Update<SOHeader>(set: "Status = N'Đang giao hàng'", where: "SONumber = '" + item + "'");
                    }
                    var header = new DC_AD_Picking_Header();
                    header.PickingNumber = PickingNumber;
                    header.PickingDate = DateTime.Parse(pickingdate);
                    header.PrinterID = printer;
                    header.PrinterName = dbConn.Select<DC_AD_Printer>(s => s.PrinterID == printer).FirstOrDefault().PrinterName;
                    header.TotalQty = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == PickingNumber).Sum(s => s.Qty);
                    header.TotalAmt = dbConn.Select<DC_AD_Picking_Detail>(s => s.PickingNumber == PickingNumber).Sum(s => s.TotalAmt);
                    header.Note = "";
                    header.Status = "Mới";
                    header.CreatedBy = currentUser.UserID;
                    header.CreatedAt = DateTime.Now;
                    header.UpdatedBy = "";
                    header.UpdatedAt = DateTime.Parse("1900-01-01");
                    dbConn.Insert<DC_AD_Picking_Header>(header);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, massege = e.Message });
                }
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Không có quyền tạo." });
            }
        }
        public ActionResult CancelSalesOrder(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<SOHeader>("Select * from SOHeader where Status <> N'Mới' AND SONumber = '"+item+"'").Count() > 0)
                        {
                            return Json(new { success = false, message = "Chỉ hủy được các mẫu tin có trạng thái Mới" });
                        }

                        dbConn.Update<SOHeader>(set: "Status = N'Hủy'", where: "SONumber = '" + item + "'");
                    }
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }
                return Json(new{success = true});
            }
            else{
                return Json(new { success = false, message = "Bạn không có quyền hủy." });
            }
        }
        public ActionResult GetLocationhByWareHouseID(string WHID)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<WareHouseLocation>(s => s.WHID == WHID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            
        }
	}
}