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
    public class ProductsController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult PartialProducts()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["user"] = dbConn.Select<Auth_User>(p => p.IsActive == true);
                dict["listWH"] = dbConn.Select<DC_AD_WH>(p => p.Status == true);
                dict["listWHL"] = dbConn.Select<DC_AD_WHL>(p => p.Status == true);
                dict["listUnit"] = dbConn.Select<DC_AD_Unit>(p => p.Status == true);
                dbConn.Close();

                return PartialView("_Products", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<Products>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(Products item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<Products>("SELECT Code, Id FROM dbo.Products Where Code ='" + item.Code + "'");
                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                {
                    if (isExist != null)
                    {
                        return Json(new { success = false, message = "Sản phẩm đã tồn tại." });
                    }
                    string id = "";
                    var checkID = db.SingleOrDefault<Products>("SELECT Code, Id FROM dbo.Products ORDER BY Id DESC");
                    if (checkID != null)
                    {
                        var nextNo = int.Parse(checkID.Code.Substring(2, checkID.Code.Length - 2)) + 1;
                        id = "PR" + String.Format("{0:00000000}", nextNo);
                    }
                    else
                    {
                        id = "PR00000001";
                    }
                    item.Code = id;
                    item.Name = !string.IsNullOrEmpty(item.Name) ? item.Name.Trim() : "";
                    item.Price = item.VATPrice / 1.1;
                    item.VATPrice = item.VATPrice;
                    item.Size = !string.IsNullOrEmpty(item.Size) ? item.Size.Trim() : ""; ;
                    item.Unit = !string.IsNullOrEmpty(item.Unit) ? item.Unit.Trim() : ""; ;
                    item.Type = !string.IsNullOrEmpty(item.Type) ? item.Type.Trim() : ""; ;
                    item.WHID = !string.IsNullOrEmpty(item.WHID) ? item.WHID : "";
                    item.WHLID = !string.IsNullOrEmpty(item.WHLID) ? item.WHLID : "";
                    item.Desc = !string.IsNullOrEmpty(item.Desc) ? item.Desc.Trim() : "";
                    item.ShapeTemplate = !string.IsNullOrEmpty(item.ShapeTemplate) ? item.ShapeTemplate.Trim() : "";
                    item.CreatedAt = DateTime.Now;
                    item.CreatedBy = currentUser.UserID;
                    item.UpdatedAt = DateTime.Parse("1900-01-01");
                    item.UpdatedBy = "";
                    item.Status = item.Status;
                    db.Insert<Products>(item);

                    return Json(new { success = true, Code = item.Code, createdat = item.CreatedAt, createdby = item.CreatedBy });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    var success = db.Execute(@"UPDATE Products SET Status = @Status, VATPrice = @VATPrice, Size= @Size, Unit=@Unit,Type=@Type, WHID=@WHID, WHLID=@WHLID, 
                    ShapeTemplate = @ShapeTemplate, UpdatedAt = @UpdatedAt,UpdatedBy =@UpdatedBy, Price=@Price,[Desc]=@Desc, Name = @Name  WHERE Code = '" + item.Code + "'", new
                {
                    Status = item.Status,
                    Price = item.VATPrice / 1.1,
                    VATPrice = item.VATPrice,
                    Size = !string.IsNullOrEmpty(item.Size) ? item.Size.Trim() : "",
                    Unit = !string.IsNullOrEmpty(item.Unit) ? item.Unit.Trim() : "",
                    Type = !string.IsNullOrEmpty(item.Type) ? item.Type.Trim() : "",
                    WHID = !string.IsNullOrEmpty(item.WHID) ? item.WHID : "",
                    WHLID = !string.IsNullOrEmpty(item.WHLID) ? item.WHLID : "",
                    ShapeTemplate = !string.IsNullOrEmpty(item.ShapeTemplate) ? item.ShapeTemplate.Trim() : "",
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = currentUser.UserID,
                    Desc = !string.IsNullOrEmpty(item.Desc) ? item.Desc.Trim() : "",
                    Name = !string.IsNullOrEmpty(item.Name) ? item.Name.Trim() : "",
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
                log.Error(" ListProducts - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult GetWH()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<DC_AD_WH>("Select * from DC_AD_WH");
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        public ActionResult Export([DataSourceRequest]DataSourceRequest request)
        {
            if (userAsset["Export"])
            {
                using (var dbConn = new OrmliteConnection().openConn())
                {
                    //using (ExcelPackage excelPkg = new ExcelPackage())
                    FileInfo fileInfo = new FileInfo(Server.MapPath(@"~\ExportTemplate\DanhMucAnPham.xlsx"));
                    var excelPkg = new ExcelPackage(fileInfo);

                    string fileName = "ThongTinKho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var data = new List<Products>();
                    if (request.Filters.Any())
                    {
                        var where = new KendoApplyFilter().ApplyFilter(request.Filters[0], "data.");
                        //data = dbConn.Select<Products>(where);
                        data = dbConn.Query<Products>("p_SelectDC_AD_Item_Export", new { WhereCondition = where }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    }
                    else
                    {
                        //data = dbConn.Select<Products>();
                        data = dbConn.Query<Products>("p_SelectDC_AD_Item_Export", new { WhereCondition = "1=1" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    }

                    ExcelWorksheet expenseSheet = excelPkg.Workbook.Worksheets["Data"];

                    int rowData = 1;

                    foreach (var item in data)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.Code;
                        expenseSheet.Cells[rowData, i++].Value = item.Name;
                        expenseSheet.Cells[rowData, i++].Value = item.Size;
                        expenseSheet.Cells[rowData, i++].Value = item.VATPrice;
                        expenseSheet.Cells[rowData, i++].Value = item.Type;
                        expenseSheet.Cells[rowData, i++].Value = item.UnitName + "/" + item.UnitID;
                        expenseSheet.Cells[rowData, i++].Value = item.WHName + "/" + item.WHID;
                        expenseSheet.Cells[rowData, i++].Value = item.WHLName + "/" + item.WHLID;
                        expenseSheet.Cells[rowData, i++].Value = item.ShapeTemplate;
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
                    expenseSheet = excelPkg.Workbook.Worksheets["Warehouse"];
                    var listWH = dbConn.Select<DC_AD_WH>("SELECT * FROM DC_AD_WH WHERE Status = 1");
                    rowData = 1;
                    foreach (var item in listWH)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.WHName + "/" + item.WHID;
                    }
                    expenseSheet = excelPkg.Workbook.Worksheets["Location"];
                    var listWHL = dbConn.Select<DC_AD_WHL>("SELECT * FROM DC_AD_WHL WHERE Status = 1");
                    rowData = 1;
                    foreach (var item in listWHL)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.WHLName + "/" + item.WHLID;
                    }
                    expenseSheet = excelPkg.Workbook.Worksheets["Unit"];
                    var listUnit = dbConn.Select<DC_AD_Unit>("SELECT * FROM DC_AD_Unit WHERE Status = 1");
                    rowData = 1;
                    foreach (var item in listUnit)
                    {
                        int i = 1;
                        rowData++;
                        expenseSheet.Cells[rowData, i++].Value = item.UnitName + "/" + item.UnitID;
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
        public ActionResult ImportData()
        {
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension =
                        System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);

                    if (fileExtension == ".xlsx" || fileExtension == ".xls")
                    {
                        IDbConnection dbConn = new OrmliteConnection().openConn();
                        using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
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
                                string Size = oSheet.Cells[i, 3].Value != null ? oSheet.Cells[i, 3].Value.ToString() : "";
                                string Priece = oSheet.Cells[i, 4].Value != null ? oSheet.Cells[i, 4].Value.ToString() : "0";
                                string Type = oSheet.Cells[i, 5].Value != null ? oSheet.Cells[i, 5].Value.ToString() : "";
                                string Unit = oSheet.Cells[i, 6].Value != null ? oSheet.Cells[i, 6].Value.ToString() : "";
                                string[] UnitID = Unit.Split('/');
                                string WH = oSheet.Cells[i, 7].Value != null ? oSheet.Cells[i, 7].Value.ToString() : "";
                                string[] WHID = WH.Split('/');
                                string WHL = oSheet.Cells[i, 8].Value != null ? oSheet.Cells[i, 8].Value.ToString() : "";
                                string[] WHLID = WHL.Split('/');
                                string Templete = oSheet.Cells[i, 9].Value != null ? oSheet.Cells[i, 9].Value.ToString() : "";
                                //string Status = oSheet.Cells[i, 9].Value != null ? oSheet.Cells[i, 9].Value.ToString() : "Ngưng hoạt động";
                                string Status = "false";
                                if (oSheet.Cells[i, 10].Value != null)
                                {
                                    if (oSheet.Cells[i, 10].Value.ToString() == "Đang hoạt động")
                                    {
                                        Status = "true";
                                    }
                                }
                                try
                                {
                                    if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Size) || string.IsNullOrEmpty(Priece))
                                    {
                                        ws.Cells["A" + 2].Value = Name;
                                        ws.Cells[rownumber, 14].Value = "Vui lòng nhập (*).";
                                        rownumber++;
                                    }
                                    else
                                    {
                                        var checkexists = dbConn.SingleOrDefault<Products>("SELECT * FROM Products WHERE Code = '" + ID + "'");
                                        if (checkexists != null)
                                        {
                                            checkexists.Code = ID;
                                            checkexists.Name = Name;
                                            checkexists.Size = Name;
                                            checkexists.Price = int.Parse(Priece) / 1.1;
                                            checkexists.VATPrice = int.Parse(Priece);
                                            checkexists.Type = Type;
                                            checkexists.Unit = Unit != null ? UnitID[UnitID.Count() - 1] : "";
                                            checkexists.WHID = WH != null ? WHID[WHID.Count() - 1] : "";
                                            checkexists.WHLID = WHL != null ? WHLID[WHLID.Count() - 1] : "";
                                            checkexists.ShapeTemplate = Templete;
                                            checkexists.Status = Boolean.Parse(Status);
                                            checkexists.UpdatedAt = DateTime.Now;
                                            checkexists.UpdatedBy = currentUser.UserID;
                                            dbConn.Update<Products>(checkexists);
                                        }
                                        else
                                        {
                                            string id = "";
                                            var checkID = dbConn.SingleOrDefault<Products>("SELECT Code, Id FROM dbo.Products ORDER BY Id DESC");
                                            if (checkID != null)
                                            {
                                                var nextNo = int.Parse(checkID.Code.Substring(2, checkID.Code.Length - 2)) + 1;
                                                id = "PR" + String.Format("{0:00000000}", nextNo);
                                            }
                                            else
                                            {
                                                id = "PR00000001";
                                            }
                                            var item = new Products();
                                            item.Code = ID;
                                            item.Name = Name;
                                            item.Size = Name;
                                            item.Price = int.Parse(Priece) / 1.1;
                                            item.VATPrice = int.Parse(Priece);
                                            item.Type = Type;
                                            item.Unit = Unit != null ? UnitID[UnitID.Count() - 1] : "";
                                            item.WHID = WH != null ? WHID[WHID.Count() - 1] : "";
                                            item.WHLID = WHL != null ? WHLID[WHLID.Count() - 1] : "";
                                            item.ShapeTemplate = Templete;
                                            item.Status = Boolean.Parse(Status);
                                            item.CreatedAt = DateTime.Now;
                                            item.CreatedBy = currentUser.UserID;
                                            item.UpdatedAt = DateTime.Parse("1900-01-01");
                                            item.UpdatedBy = "";
                                            item.Status = Boolean.Parse(Status);
                                            dbConn.Insert<Products>(item);
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