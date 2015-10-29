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
    public class DeliveryFeeController :CustomController 
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialDeliveryFee()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE IsActive = 1");
                dbConn.Close();
                return PartialView("_DeliveryFee", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read_DeliveryPackage([DataSourceRequest]DataSourceRequest request)
        {
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new DC_LG_DeliveryFee().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }
        public ActionResult Read_DeliveryFeeTerritory([DataSourceRequest]DataSourceRequest request)
        {
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new DC_LG_DeliveryFee_Territory().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }
        public FileResult Export_DeliveryPackage([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/GoiCuocVanChuyen.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " +new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new DC_LG_DeliveryFee().GetListDeliveryFee(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.DeliveryFeeID;
                    ws.Cells["B" + rowNum].Value = item.Name;
                    ws.Cells["C" + rowNum].Value = item.TransporterID;
                    ws.Cells["D" + rowNum].Value = item.DeliveryName;
                    ws.Cells["E" + rowNum].Value = item.Descr;
                    ws.Cells["F" + rowNum].Value = item.MinDay;
                    ws.Cells["G" + rowNum].Value = item.MaxDay;
                    ws.Cells["H" + rowNum].Value = item.MinTime;
                    ws.Cells["I" + rowNum].Value = item.MaxTime;
                    ws.Cells["J" + rowNum].Value = item.MinWeight    ;
                    ws.Cells["K" + rowNum].Value = item.MaxWeight;
                    ws.Cells["L" + rowNum].Value = item.Price;
                    ws.Cells["M" + rowNum].Value = item.Note;
                    ws.Cells["N" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "GoiCuocVanChuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_DeliveryTerritory([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/GoiCuocVanChuyenTheoVungMien.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new DC_LG_DeliveryFee_Territory().GetList(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.DeliveryFeeID;
                    ws.Cells["B" + rowNum].Value = item.DeliveryFeeName;
                    ws.Cells["C" + rowNum].Value = item.ProvinceID;
                    ws.Cells["D" + rowNum].Value = item.ProvinceName;
                    ws.Cells["E" + rowNum].Value = item.DistrictID;
                    ws.Cells["F" + rowNum].Value = item.DistrictName;
                    ws.Cells["G" + rowNum].Value = item.PickingProvinceID;
                    ws.Cells["H" + rowNum].Value = item.PickingProvinceName;
                    ws.Cells["I" + rowNum].Value = item.PickingDistrictID;
                    ws.Cells["J" + rowNum].Value = item.PickingDistrictName;
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
                        "GoiCuocVanChuyenTheoVungMien" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
