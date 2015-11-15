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
    public class WareHouseController : CustomController
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
                dict["user"] = dbConn.Select<Auth_User>(p => p.IsActive == true);
                dict["listWH"] = dbConn.Select<WareHouse>(p => p.Status == true);
                dbConn.Close();
                return PartialView("_WareHouse", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult ReadWH([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<WareHouse>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult ReadWHL([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<WareHouseLocation>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult ReadUnit([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<DC_AD_Unit>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult CreateWH(WareHouse item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<WareHouse>("SELECT WHID, Id FROM dbo.DC_AD_WH Where WHID ='" + item.WHID + "'");
                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                {
                    if (isExist != null)
                    {
                        return Json(new { success = false, message = "Kho đã tồn tại." });
                    }
                    string id = "";
                    var checkID = db.SingleOrDefault<WareHouse>("SELECT WHID, Id FROM dbo.DC_AD_WH ORDER BY Id DESC");
                    if (checkID != null)
                    {
                        var nextNo = int.Parse(checkID.WHID.Substring(2, checkID.WHID.Length - 2)) + 1;
                        id = "WH" + String.Format("{0:00000000}", nextNo);
                    }
                    else
                    {
                        id = "WH00000001";
                    }
                    item.WHID = id;
                    item.WHName = !string.IsNullOrEmpty(item.WHName) ? item.WHName.Trim() : "";
                    item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address.Trim() : ""; 
                    item.WHKeeper = !string.IsNullOrEmpty(item.WHKeeper) ? item.WHKeeper.Trim() : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : ""; 
                    item.CreatedAt = DateTime.Now;
                    item.CreatedBy = currentUser.UserID;
                    item.UpdatedAt = DateTime.Parse("1900-01-01");
                    item.UpdatedBy = "";
                    item.Status = item.Status;
                    db.Insert<WareHouse>(item);

                    return Json(new { success = true, Code = item.WHID, createdate = item.CreatedAt, createdby = item.CreatedBy });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    var success = db.Execute(@"UPDATE WareHouse SET Status = @Status, Address=@Address,WHKeeper=@WHKeeper,
                    Note = @Note,  UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy, WHName=@WHName
                    WHERE WHID = '" + item.WHID + "'", new
                    {
                        Status = item.Status,
                        //WHName = !string.IsNullOrEmpty(item.WHName) ? item.WHName.Trim() : "",
                        Address = !string.IsNullOrEmpty(item.Address) ? item.Address.Trim() : "",
                        WHKeeper = !string.IsNullOrEmpty(item.WHKeeper) ? item.WHKeeper.Trim() : "",
                        Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : "",
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = currentUser.UserID,
                        WHName = !string.IsNullOrEmpty(item.WHName) ? item.WHName.Trim() : "",
                    }) == 1;
                    if (!success)
                    {
                        return Json(new { success = false, message = "Cập nhật không thành công." });
                    }
                    
                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });
            }
            catch (Exception e)
            {
                log.Error(" ListPublication - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult CreateWHL(WareHouseLocation item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<WareHouseLocation>("SELECT WHLID, Id FROM dbo.WareHouseLocation Where WHLID ='" + item.WHLID + "'");
                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                {
                    if (isExist != null)
                    {
                        return Json(new { success = false, message = "Vị trí kho đã tồn tại." });
                    }
                    string id = "";
                    var checkID = db.SingleOrDefault<WareHouseLocation>("SELECT WHLID, Id FROM dbo.WareHouseLocation ORDER BY Id DESC");
                    if (checkID != null)
                    {
                        var nextNo = int.Parse(checkID.WHLID.Substring(3, checkID.WHLID.Length - 3)) + 1;
                        id = "WHL" + String.Format("{0:00000000}", nextNo);
                    }
                    else
                    {
                        id = "WHL00000001";
                    }
                    item.WHLID = id;
                    item.WHLName = !string.IsNullOrEmpty(item.WHLName) ? item.WHLName.Trim() : "";
                    item.WHID = !string.IsNullOrEmpty(item.WHID) ? item.WHID.Trim() : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : "";
                    item.CreatedAt = DateTime.Now;
                    item.CreatedBy = currentUser.UserID;
                    item.UpdatedAt = DateTime.Parse("1900-01-01");
                    item.UpdatedBy = "";
                    item.Status = item.Status;
                    db.Insert<WareHouseLocation>(item);

                    return Json(new { success = true, Code = item.WHLID, createdate = item.CreatedAt, createdby = item.CreatedBy });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    var success = db.Execute(@"UPDATE WareHouseLocation SET Status = @Status,
                    Note = @Note,  UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy, WHLName = @WHLName, WHID = @WHID
                    WHERE WHLID = '" + item.WHLID + "'", new
                        {
                            Status = item.Status,
                            //WHName = !string.IsNullOrEmpty(item.WHName) ? item.WHName.Trim() : "",
                            Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : "",
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = currentUser.UserID,
                            WHLName = !string.IsNullOrEmpty(item.WHLName) ? item.WHLName.Trim() : "",
                            WHID = !string.IsNullOrEmpty(item.WHID) ? item.WHID.Trim() : "",
                        }) == 1;
                    if (!success)
                    {
                        return Json(new { success = false, message = "Cập nhật không thành công." });
                    }

                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });
            }
            catch (Exception e)
            {
                log.Error(" ListPublication - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult CreateUnit(DC_AD_Unit item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<DC_AD_Unit>("SELECT UnitID, Id FROM dbo.DC_AD_Unit Where UnitID ='" + item.UnitID + "'");
                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy ==null)
                {
                    if (isExist != null)
                    {
                        return Json(new { success = false, message = "Đơn vị tính đã tồn tại." });
                    }
                    string id = "";
                    var checkID = db.SingleOrDefault<DC_AD_Unit>("SELECT UnitID, Id FROM dbo.DC_AD_Unit ORDER BY Id DESC");
                    if (checkID != null)
                    {
                        var nextNo = int.Parse(checkID.UnitID.Substring(3, checkID.UnitID.Length - 3)) + 1;
                        id = "UIT" + String.Format("{0:00000000}", nextNo);
                    }
                    else
                    {
                        id = "UIT00000001";
                    }
                    item.UnitID = id;
                    item.UnitName = !string.IsNullOrEmpty(item.UnitName) ? item.UnitName.Trim() : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : "";
                    item.CreatedAt = DateTime.Now;
                    item.CreatedBy = currentUser.UserID;
                    item.UpdatedAt = DateTime.Parse("1900-01-01");
                    item.UpdatedBy = "";
                    item.Status = item.Status;
                    db.Insert<DC_AD_Unit>(item);

                    return Json(new { success = true, Code = item.UnitID, createdate = item.CreatedAt, createdby = item.CreatedBy });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    var success = db.Execute(@"UPDATE DC_AD_Unit SET Status = @Status,
                    Note = @Note,  UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy, UnitName = @UnitName
                    WHERE UnitID = '" + item.UnitID + "'", new
                    {
                        Status = item.Status,
                        //WHName = !string.IsNullOrEmpty(item.WHName) ? item.WHName.Trim() : "",
                        Note = !string.IsNullOrEmpty(item.Note) ? item.Note.Trim() : "",
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = currentUser.UserID,
                        UnitName = !string.IsNullOrEmpty(item.UnitName) ? item.UnitName.Trim() : "",
                    }) == 1;
                    if (!success)
                    {
                        return Json(new { success = false, message = "Cập nhật không thành công." });
                    }

                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });
            }
            catch (Exception e)
            {
                log.Error(" ListPublication - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult ExportWH([DataSourceRequest]DataSourceRequest request)
        {
            if (userAsset["Export"])
            {
                using (var dbConn = new OrmliteConnection().openConn())
                {
                    //using (ExcelPackage excelPkg = new ExcelPackage())
                    FileInfo fileInfo = new FileInfo(Server.MapPath(@"~\ExportTemplate\ThongTinKho.xlsx"));
                    var excelPkg = new ExcelPackage(fileInfo);

                    string fileName = "ThongTinKho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var data = new List<WareHouse>();
                    if (request.Filters.Any())
                    {
                        var where = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                        data = dbConn.Select<WareHouse>(where);
                    }
                    else
                    {
                        data = dbConn.Select<WareHouse>();
                    }

                    ExcelWorksheet expenseSheet = excelPkg.Workbook.Worksheets["Data"];

                    int rowData = 1;

                    foreach (var item in data)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.WHID;
                        expenseSheet.Cells[rowData, i++].Value = item.WHName;
                        expenseSheet.Cells[rowData, i++].Value = item.Address;
                        expenseSheet.Cells[rowData, i++].Value = item.WHKeeper;
                        expenseSheet.Cells[rowData, i++].Value = item.Note;
                        if (item.Status == true)
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Đang hoạt động";
                        }
                        else
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Ngưng hoạt động";
                        }
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
                        //expenseSheet.Cells[rowData, i++].Value = item.RowLastUpdatedTime;
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
        public ActionResult ExportWHL([DataSourceRequest]DataSourceRequest request)
        {
            if (userAsset["Export"])
            {
                using (var dbConn = new OrmliteConnection().openConn())
                {
                    //using (ExcelPackage excelPkg = new ExcelPackage())
                    FileInfo fileInfo = new FileInfo(Server.MapPath(@"~\ExportTemplate\ViTriKho.xlsx"));
                    var excelPkg = new ExcelPackage(fileInfo);

                    string fileName = "ViTriKho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var data = new List<WareHouseLocation>();
                    if (request.Filters.Any())
                    {
                        var where = new KendoApplyFilter().ApplyFilter(request.Filters[0],"data.");
                        data = data = dbConn.Query<WareHouseLocation>("p_SelectWareHouseLocation_Export", new { WhereCondition = where }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    }
                    else
                    {
                        data = data = dbConn.Query<WareHouseLocation>("p_SelectWareHouseLocation_Export", new { WhereCondition = "1=1" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    }

                    ExcelWorksheet expenseSheet = excelPkg.Workbook.Worksheets["Data"];

                    int rowData = 1;

                    foreach (var item in data)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.WHLID;
                        expenseSheet.Cells[rowData, i++].Value = item.WHLName;
                        expenseSheet.Cells[rowData, i++].Value = item.WHName+"/"+item.WHID;
                        expenseSheet.Cells[rowData, i++].Value = item.Note;
                        if (item.Status == true)
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Đang hoạt động";
                        }
                        else
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Ngưng hoạt động";
                        }
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
                    }
                    expenseSheet = excelPkg.Workbook.Worksheets["Kho"];
                    var listWH = dbConn.Select<WareHouse>("SELECT * FROM WareHouse WHERE Status = 1");
                    rowData = 1;
                    foreach (var item in listWH)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.WHName+"/" + item.WHID;
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
        public ActionResult ExportUnit([DataSourceRequest]DataSourceRequest request)
        {
            if (userAsset["Export"])
            {
                using (var dbConn = new OrmliteConnection().openConn())
                {
                    //using (ExcelPackage excelPkg = new ExcelPackage())
                    FileInfo fileInfo = new FileInfo(Server.MapPath(@"~\ExportTemplate\DonViTinh.xlsx"));
                    var excelPkg = new ExcelPackage(fileInfo);

                    string fileName = "DonViTinh_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var data = new List<DC_AD_Unit>();
                    if (request.Filters.Any())
                    {
                        var where = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                        data = dbConn.Select<DC_AD_Unit>(where);
                    }
                    else
                    {
                        data = dbConn.Select<DC_AD_Unit>();
                    }

                    ExcelWorksheet expenseSheet = excelPkg.Workbook.Worksheets["Data"];

                    int rowData = 1;

                    foreach (var item in data)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.UnitID;
                        expenseSheet.Cells[rowData, i++].Value = item.UnitName;
                        //expenseSheet.Cells[rowData, i++].Value = item.WHID;
                        expenseSheet.Cells[rowData, i++].Value = item.Note;
                        if (item.Status == true)
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Đang hoạt động";
                        }
                        else
                        {
                            expenseSheet.Cells[rowData, i++].Value = "Ngưng hoạt động";
                        }
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
        public ActionResult ImportUnit()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension =
                        System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);

                    if (fileExtension == ".xlsx" || fileExtension == ".xls")
                    {
                        
                        
                            string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + Request.Files["FileUpload"].FileName);
                            string errorFileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName);
                            string linkerror = "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName;

                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);

                            Request.Files["FileUpload"].SaveAs(fileLocation);

                            var rownumber = 2;
                            var total = 0;
                            FileInfo fileInfo = new FileInfo(fileLocation);
                            var excelPkg = new ExcelPackage(fileInfo);
                            //FileInfo template = new FileInfo(Server.MapPath(errorFileLocation));
                            //template.CopyTo(errorFileLocation);
                            //FileInfo _fileInfo = new FileInfo(errorFileLocation);
                            //var _excelPkg = new ExcelPackage(_fileInfo);
                            ExcelWorksheet oSheet = excelPkg.Workbook.Worksheets["Data"];
                            //ExcelWorksheet eSheet = _excelPkg.Workbook.Worksheets["Data"];
                            ExcelPackage pck = new ExcelPackage(new FileInfo(errorFileLocation));
                            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
                            int totalRows = oSheet.Dimension.End.Row;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                string ID = oSheet.Cells[i, 1].Value != null ? oSheet.Cells[i, 1].Value.ToString() : "";
                                string Name = oSheet.Cells[i, 2].Value != null ? oSheet.Cells[i, 2].Value.ToString() : "";
                                string Note = oSheet.Cells[i, 3].Value != null ? oSheet.Cells[i, 3].Value.ToString() : "";
                                //string Status = oSheet.Cells[i, 4].Value == "Đang hoạt động" ? "true" : "false";
                                string Status = "false";
                                if (oSheet.Cells[i, 6].Value != null)
                                {
                                    if (oSheet.Cells[i, 4].Value.ToString() == "Đang hoạt động")
                                    {
                                        Status = "true";
                                    }
                                }
                                try
                                {
                                    if (string.IsNullOrEmpty(Name))
                                    {
                                        ws.Cells["A" + 2].Value = Name;
                                        ws.Cells[rownumber, 14].Value = "Vui lòng nhập (*).";
                                        rownumber++;
                                    }
                                    else
                                    {
                                        var checkexists = dbConn.SingleOrDefault<DC_AD_Unit>("SELECT * FROM DC_AD_Unit WHERE UnitID = '" + ID + "'");
                                        if (checkexists != null)
                                        {
                                            checkexists.UnitID = ID;
                                            checkexists.UnitName = Name;
                                            checkexists.Status = Boolean.Parse(Status);
                                            checkexists.UpdatedAt = DateTime.Now;
                                            checkexists.UpdatedBy = currentUser.UserID;
                                            dbConn.Update<DC_AD_Unit>(checkexists);
                                        }
                                        else
                                        {
                                            string id = "";
                                            var checkID = dbConn.SingleOrDefault<DC_AD_Unit>("SELECT UnitID, Id FROM dbo.DC_AD_Unit ORDER BY Id DESC");
                                            if (checkID != null)
                                            {
                                                var nextNo = int.Parse(checkID.UnitID.Substring(3, checkID.UnitID.Length - 3)) + 1;
                                                id = "UIT" + String.Format("{0:00000000}", nextNo);
                                            }
                                            else
                                            {
                                                id = "UIT00000001";
                                            }
                                            var item = new DC_AD_Unit();
                                            item.UnitID = id;
                                            item.UnitName = !string.IsNullOrEmpty(Name) ? Name.Trim() : "";
                                            item.Note = !string.IsNullOrEmpty(Note) ? Note.Trim() : "";
                                            item.CreatedAt = DateTime.Now;
                                            item.CreatedBy = currentUser.UserID;
                                            item.UpdatedAt = DateTime.Parse("1900-01-01");
                                            item.UpdatedBy = "";
                                            item.Status = Boolean.Parse(Status);
                                            dbConn.Insert<DC_AD_Unit>(item);
                                        }
                                        total++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    return Json(new { success = false, message = e.Message });
                                }
                            }
                            return Json(new { success = true, total = total, totalError = rownumber - 2, link = linkerror });
                        }
   
                    else
                    {
                        return Json(new { success = false, message = "Không phải là file Excel. *.xlsx" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        public ActionResult ImportWHL()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension =
                        System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);

                    if (fileExtension == ".xlsx" || fileExtension == ".xls")
                    {


                        string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + Request.Files["FileUpload"].FileName);
                        string errorFileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName);
                        string linkerror = "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName;

                        if (System.IO.File.Exists(fileLocation))
                            System.IO.File.Delete(fileLocation);

                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        var rownumber = 2;
                        var total = 0;
                        FileInfo fileInfo = new FileInfo(fileLocation);
                        var excelPkg = new ExcelPackage(fileInfo);
                        //FileInfo template = new FileInfo(Server.MapPath(errorFileLocation));
                        //template.CopyTo(errorFileLocation);
                        //FileInfo _fileInfo = new FileInfo(errorFileLocation);
                        //var _excelPkg = new ExcelPackage(_fileInfo);
                        ExcelWorksheet oSheet = excelPkg.Workbook.Worksheets["Data"];
                        //ExcelWorksheet eSheet = _excelPkg.Workbook.Worksheets["Data"];
                        ExcelPackage pck = new ExcelPackage(new FileInfo(errorFileLocation));
                        ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
                        int totalRows = oSheet.Dimension.End.Row;
                        for (int i = 2; i <= totalRows; i++)
                        {
                            string ID = oSheet.Cells[i, 1].Value != null ? oSheet.Cells[i, 1].Value.ToString() : "";
                            string Name = oSheet.Cells[i, 2].Value != null ? oSheet.Cells[i, 2].Value.ToString() : "";
                            string WHName = oSheet.Cells[i, 3].Value != null ? oSheet.Cells[i, 3].Value.ToString() : "";
                            string[] WHID = WHName.Split('/');
                            string Note = oSheet.Cells[i, 4].Value != null ? oSheet.Cells[i, 4].Value.ToString() : "";
                            //string Status = oSheet.Cells[i, 5].Value == "Đang hoạt động" ? "true" : "false";
                            string Status = "false";
                            if (oSheet.Cells[i, 5].Value != null)
                            {
                                if (oSheet.Cells[i, 6].Value.ToString() == "Đang hoạt động")
                                {
                                    Status = "true";
                                }
                            }
                            try
                            {
                                if (string.IsNullOrEmpty(Name))
                                {
                                    ws.Cells["A" + 2].Value = Name;
                                    ws.Cells[rownumber, 14].Value = "Vui lòng nhập (*).";
                                    rownumber++;
                                }
                                else
                                {
                                    var checkexists = dbConn.SingleOrDefault<WareHouseLocation>("SELECT * FROM WareHouseLocation WHERE WHLID = '" + ID + "'");
                                    if (checkexists != null)
                                    {
                                        checkexists.WHLID = ID;
                                        checkexists.WHLName = Name;
                                        checkexists.Note = Note;
                                        checkexists.WHID = !string.IsNullOrEmpty(WHName) ? WHID[WHID.Count() - 1] : "";
                                        checkexists.Status = Boolean.Parse(Status);
                                        checkexists.UpdatedAt = DateTime.Now;
                                        checkexists.UpdatedBy = currentUser.UserID;
                                        dbConn.Update<WareHouseLocation>(checkexists);
                                    }
                                    else
                                    {
                                        string id = "";
                                        var checkID = dbConn.SingleOrDefault<WareHouseLocation>("SELECT WHLID, Id FROM dbo.WareHouseLocation ORDER BY Id DESC");
                                        if (checkID != null)
                                        {
                                            var nextNo = int.Parse(checkID.WHLID.Substring(3, checkID.WHLID.Length - 3)) + 1;
                                            id = "WHL" + String.Format("{0:00000000}", nextNo);
                                        }
                                        else
                                        {
                                            id = "WHL00000001";
                                        }
                                        var item = new WareHouseLocation();
                                        item.WHLID = id;
                                        item.WHLName = !string.IsNullOrEmpty(Name) ? Name.Trim() : "";
                                        item.Note = !string.IsNullOrEmpty(Note) ? Note.Trim() : "";
                                        item.WHID = !string.IsNullOrEmpty(WHName) ? WHID[WHID.Count() - 1] : "";
                                        item.CreatedAt = DateTime.Now;
                                        item.CreatedBy = currentUser.UserID;
                                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                                        item.UpdatedBy = "";
                                        item.Status = Boolean.Parse(Status);
                                        dbConn.Insert<WareHouseLocation>(item);
                                    }
                                    total++;
                                }
                            }
                            catch (Exception e)
                            {
                                return Json(new { success = false, message = e.Message });
                            }
                        }
                        return Json(new { success = true, total = total, totalError = rownumber - 2, link = linkerror });
                    }

                    else
                    {
                        return Json(new { success = false, message = "Không phải là file Excel. *.xlsx" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        public ActionResult ImportWH()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension =
                        System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);

                    if (fileExtension == ".xlsx" || fileExtension == ".xls")
                    {


                        string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + Request.Files["FileUpload"].FileName);
                        string errorFileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName);
                        string linkerror = "[" + currentUser.UserID + "-" + datetime + "-Error]" + Request.Files["FileUpload"].FileName;

                        if (System.IO.File.Exists(fileLocation))
                            System.IO.File.Delete(fileLocation);

                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        var rownumber = 2;
                        var total = 0;
                        FileInfo fileInfo = new FileInfo(fileLocation);
                        var excelPkg = new ExcelPackage(fileInfo);
                        //FileInfo template = new FileInfo(Server.MapPath(errorFileLocation));
                        //template.CopyTo(errorFileLocation);
                        //FileInfo _fileInfo = new FileInfo(errorFileLocation);
                        //var _excelPkg = new ExcelPackage(_fileInfo);
                        ExcelWorksheet oSheet = excelPkg.Workbook.Worksheets["Data"];
                        //ExcelWorksheet eSheet = _excelPkg.Workbook.Worksheets["Data"];
                        ExcelPackage pck = new ExcelPackage(new FileInfo(errorFileLocation));
                        ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
                        int totalRows = oSheet.Dimension.End.Row;
                        for (int i = 2; i <= totalRows; i++)
                        {
                            string ID = oSheet.Cells[i, 1].Value != null ? oSheet.Cells[i, 1].Value.ToString() : "";
                            string Name = oSheet.Cells[i, 2].Value != null ? oSheet.Cells[i, 2].Value.ToString() : "";
                            string Address = oSheet.Cells[i, 3].Value != null ? oSheet.Cells[i, 3].Value.ToString() : "";
                            string Keeper = oSheet.Cells[i, 4].Value != null ? oSheet.Cells[i, 4].Value.ToString() : "";
                            string Note = oSheet.Cells[i, 5].Value != null ? oSheet.Cells[i, 5].Value.ToString() : "";
                            string Status = "false";
                            if (oSheet.Cells[i, 6].Value != null)
                            {
                                if (oSheet.Cells[i, 6].Value.ToString() == "Đang hoạt động")
                                {
                                    Status = "true";
                                }
                            }

                            try
                            {
                                if (string.IsNullOrEmpty(Name))
                                {
                                    ws.Cells["A" + 2].Value = Name;
                                    ws.Cells[rownumber, 14].Value = "Vui lòng nhập (*).";
                                    rownumber++;
                                }
                                else
                                {
                                    var checkexists = dbConn.SingleOrDefault<WareHouse>("SELECT * FROM DC_AD_WH WHERE WHID = '" + ID + "'");
                                    if (checkexists != null)
                                    {
                                        checkexists.WHID = ID;
                                        checkexists.WHName = Name;
                                        checkexists.Note = Note;
                                        checkexists.Address = !string.IsNullOrEmpty(Address) ? Address : "";
                                        checkexists.WHKeeper = Keeper;
                                        checkexists.Status = Boolean.Parse(Status);
                                        checkexists.UpdatedAt = DateTime.Now;
                                        checkexists.UpdatedBy = currentUser.UserID;
                                        dbConn.Update<WareHouse>(checkexists);
                                    }
                                    else
                                    {
                                        string id = "";
                                        var checkID = dbConn.SingleOrDefault<WareHouse>("SELECT WHID, Id FROM dbo.DC_AD_WH ORDER BY Id DESC");
                                        if (checkID != null)
                                        {
                                            var nextNo = int.Parse(checkID.WHID.Substring(2, checkID.WHID.Length - 2)) + 1;
                                            id = "WH" + String.Format("{0:00000000}", nextNo);
                                        }
                                        else
                                        {
                                            id = "WH00000001";
                                        }
                                        var item = new WareHouse();
                                        item.WHID = id;
                                        item.WHName = !string.IsNullOrEmpty(Name) ? Name.Trim() : "";
                                        item.Note = !string.IsNullOrEmpty(Note) ? Note.Trim() : "";
                                        item.Address = !string.IsNullOrEmpty(Address) ? Address : "";
                                        item.WHKeeper = Keeper;
                                        item.CreatedAt = DateTime.Now;
                                        item.CreatedBy = currentUser.UserID;
                                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                                        item.UpdatedBy = "";
                                        item.Status = Boolean.Parse(Status);
                                        dbConn.Insert<WareHouse>(item);
                                    }
                                    total++;
                                }
                            }
                            catch (Exception e)
                            {
                                return Json(new { success = false, message = e.Message });
                            }
                        }
                        return Json(new { success = true, total = total, totalError = rownumber - 2, link = linkerror });
                    }

                    else
                    {
                        return Json(new { success = false, message = "Không phải là file Excel. *.xlsx" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
	}
}