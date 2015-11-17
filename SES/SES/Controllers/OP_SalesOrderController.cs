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
    public class OP_SalesOrder : CustomController
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
                dict["listUnit"] = dbConn.Select<DC_AD_Unit>(p => p.Status == true);
                dict["listVendor"] = dbConn.Select<Vendor>();
                dbConn.Close();
                return PartialView("SalesOrderHeader", dict);
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
                dict["listUnit"] = dbConn.Select<DC_AD_Unit>(p => p.Status == true);
                dict["SONumber"] = id;
                return PartialView("SalesOrderDetail", dict);
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
                    DateTime fromDateValue;
                    var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
                    
                    var header = new SOHeader();
                    var detail = new SODetail();
                    var itemcode = dbConn.SingleOrDefault<Products>("SELECT * FROM Products WHERE Code = '" + Request["ItemCode"] + "'");
                    var itemunit = dbConn.SingleOrDefault<DC_AD_Unit>("SELECT * FROM DC_AD_Unit WHERE UnitID = '" + itemcode.Unit + "'");
                    var checkheader =  dbConn.SingleOrDefault<SOHeader>("SELECT * FROM SOHeader Where SONumber = '" + SONumber + "' AND MerchantID = '" + Request["MerchantID"] + "'");
                    if (checkheader == null)
                    {
                        if (!string.IsNullOrEmpty(Request["SODate"]))
                        {
                            if (DateTime.TryParseExact(Request["SODate"], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                            {
                                header.SODate = DateTime.Parse(DateTime.ParseExact(Request["SODate"], "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                return Json(new { message = "Ngày tạo không đúng." });
                            }
                        }
                        header.SONumber = SONumber;
                        //header.SODate = DateTime.Now;
                        header.MerchantID = !string.IsNullOrEmpty(Request["MerchantID"]) ? Request["MerchantID"] : "";
                        header.Note = !string.IsNullOrEmpty(Request["Note"]) ? Request["Note"] : "";
                        header.TotalQty = !string.IsNullOrEmpty(Request["Qty"]) ? int.Parse(Request["Qty"]) : 0;
                        header.WHID = Request["WHID"];
                        header.Status = "New";
                        header.WHLID = !string.IsNullOrEmpty(Request["WHLID"]) ? Request["WHLID"] : ""; 
                        header.TotalAmt = itemcode.VATPrice * int.Parse(Request["Qty"]);
                        header.CreatedBy = currentUser.UserID;
                        header.CreatedAt = DateTime.Now;
                        header.UpdatedBy = "";
                        header.UpdatedAt = DateTime.Parse("1900-01-01");
                        dbConn.Insert<SOHeader>(header);
                    }
                    else
                    {
                        var success = dbConn.Execute(@"UPDATE SOHeader Set TotalQty = @TotalQty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy
                        WHERE SONumber = '" + SONumber + "'", new
                        {
                            TotalQty = checkheader.TotalQty + int.Parse(Request["Qty"]),
                            TotalAmt = itemcode.VATPrice * (checkheader.TotalQty +int.Parse(Request["Qty"])),
                            UpdatedBy = currentUser.UserID,
                            UpdatedAt = DateTime.Now,
                        }) == 1;
                        if (!success)
                        {
                            return Json(new { success = false, message = "Không thể lưu" });
                        }
                    }
                    var checkdetail = dbConn.SingleOrDefault<SODetail>("SELECT * FROM SODetail WHERE SONumber = '" + SONumber + "' AND ItemCode = '" + Request["ItemCode"] + "'");
                    if (checkdetail == null)
                    {
                        detail.SONumber = SONumber;
                        detail.ItemCode = !string.IsNullOrEmpty(Request["ItemCode"]) ? Request["ItemCode"] : "";
                        detail.ItemName = !string.IsNullOrEmpty(itemcode.Name)? itemcode.Name :"" ;
                        detail.Price = itemcode.VATPrice;
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
                    else
                    {
                        var success = dbConn.Execute(@"UPDATE SODetail Set Qty = @Qty, TotalAmt =@TotalAmt ,UpdatedAt = @UpdatedAt, UpdatedBy =  @UpdatedBy, Price = @Price
                        WHERE SONumber = '" + SONumber + "' AND ItemCode = '" + Request["ItemCode"] + "'", new
                        {
                            Qty = checkdetail.Qty + int.Parse(Request["Qty"]),
                            TotalAmt = itemcode.VATPrice * (checkdetail.Qty + int.Parse(Request["Qty"])),
                            Price = itemcode.VATPrice,
                            UpdatedBy = currentUser.UserID,
                            UpdatedAt = DateTime.Now,
                        }) == 1;
                        if (!success)
                        {
                            return Json(new { success = false, message = "Không thể lưu" });
                        }
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
                //return Json(new { succsess = true, dataItem = data });
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
                if (text.Length >= 4)
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
                if (text.Length >= 4)
                {
                    data = dbConn.Query<Products>("SELECT TOP 5 Name, Code, size FROM Products WHERE Name COLLATE Latin1_General_CI_AI LIKE N'%" + text + "%'");
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
	}
}