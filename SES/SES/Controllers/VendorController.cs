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
using System.Globalization;
namespace SES.Controllers
{
    public class VendorController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialVendor()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listProvince"] = dbConn.Select<Master_Territory>("SELECT TerritoryID, TerritoryID +'-'+ TerritoryName as TerritoryName FROM Master_Territory WHERE Level='Province'");
                dbConn.Close();
                return PartialView("_Vendor", dict);
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
            var data = dbConn.Select<Vendor>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(Vendor item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.VendorName)
                    )
                {
                    DateTime signoff;
                    var signDate = Request["SignOffDate"];
                    if (DateTime.TryParseExact(signDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out signoff
                        ))
                    { }
                    else
                    {
                        return Json(new { success = false, message = "Định dạng ngày ký không đúng" });
                    }

                    var isExist = db.SingleOrDefault<Vendor>("VendorID={0}", item.VendorID);
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.FullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : "";
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.SignOffDate = signoff;
                    item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                    item.TaxCode = !string.IsNullOrEmpty(item.TaxCode) ? item.TaxCode : "";
                    item.Website = !string.IsNullOrEmpty(item.Website) ? item.Website : "";
                    item.Hotline = !string.IsNullOrEmpty(item.Hotline) ? item.Hotline : "";
                    item.Url = "";
                    item.Fax = !string.IsNullOrEmpty(item.Fax) ? item.Fax : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã NCC đã tồn tại!" });
                        string id = "";
                        var checkID = db.SingleOrDefault<Vendor>("SELECT VendorID, Id FROM dbo.Vendor ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.VendorID.Substring(2, checkID.VendorID.Length - 2)) + 1;
                            id = "VD" + String.Format("{0:00000000}", nextNo);
                        }
                        else
                        {
                            id = "VD00000001";
                        }
                        item.VendorID = id;
                        item.VendorName = !string.IsNullOrEmpty(item.VendorName) ? item.VendorName : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = "";
                        db.Insert(item);
                        return Json(new { success = true, VendorID = item.VendorID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.VendorName = !string.IsNullOrEmpty(item.VendorName) ? item.VendorName : "";
                        item.CreatedAt = isExist.CreatedAt;
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
                log.Error("Vendor - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult Export([DataSourceRequest]DataSourceRequest request)
        {
            if (userAsset["Export"])
            {
                using (var dbConn = new OrmliteConnection().openConn())
                {
                    //using (ExcelPackage excelPkg = new ExcelPackage())
                    FileInfo fileInfo = new FileInfo(Server.MapPath(@"~\ExportTemplate\NhaCungCap.xlsx"));
                    var excelPkg = new ExcelPackage(fileInfo);

                    string fileName = "NhaCungCap" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var data = new List<Vendor>();
                    if (request.Filters.Any())
                    {
                        var where = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                        data = dbConn.Select<Vendor>(where);
                    }
                    else
                    {
                        data = dbConn.Select<Vendor>();
                    }

                    ExcelWorksheet expenseSheet = excelPkg.Workbook.Worksheets["Data"];

                    int rowData = 1;

                    foreach (var item in data)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.VendorID;
                        expenseSheet.Cells[rowData, i++].Value = item.VendorName;
                        expenseSheet.Cells[rowData, i++].Value = item.FullName;
                        expenseSheet.Cells[rowData, i++].Value = item.Address;
                        expenseSheet.Cells[rowData, i++].Value = item.Phone;
                        expenseSheet.Cells[rowData, i++].Value = item.Fax;
                        expenseSheet.Cells[rowData, i++].Value = item.Email;
                        expenseSheet.Cells[rowData, i++].Value = item.TaxCode;
                        expenseSheet.Cells[rowData, i++].Value = item.ProvinceID;
                        expenseSheet.Cells[rowData, i++].Value = item.Website;
                        expenseSheet.Cells[rowData, i++].Value = item.Hotline;
                        expenseSheet.Cells[rowData, i++].Value = item.SignOffDate;
                        expenseSheet.Cells[rowData, i++].Value = item.CreatedBy;
                        expenseSheet.Cells[rowData, i++].Value = item.CreatedAt;
                        expenseSheet.Cells[rowData, i++].Value = item.UpdatedBy;
                        if (item.UpdatedAt != DateTime.Parse("1900-01-01"))
                        {
                            expenseSheet.Cells[rowData, i++].Value = item.UpdatedAt;
                        }
                        else
                        {
                            expenseSheet.Cells[rowData, i++].Value = "";
                        }
                        expenseSheet.Cells[rowData, i++].Value = item.Note;
                        if (item.Status == true)
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Đang hoạt động";
                        }
                        else
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Ngưng hoạt động";
                        }

                    }

                    MemoryStream output = new MemoryStream();
                    excelPkg.SaveAs(output);
                    output.Position = 0;
                    return File(output.ToArray(), contentType, fileName);
                }
            }
            else
            {
                return Json(new { success = false });
            }

        }
    }
}
