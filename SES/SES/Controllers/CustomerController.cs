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
    public class CustomerController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult PartialCustomer()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listprovince"] = dbConn.Select<Master_Territory>("Select TerritoryID,TerritoryName From Master_Territory where Level ='Province'");
                dict["listdistrict"] = dbConn.Select<Master_Territory>("Select TerritoryID,TerritoryName From Master_Territory where Level ='District'");
                dbConn.Close();
                return PartialView("_Customer", dict);
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
            var data = dbConn.Select<Customer>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }
        public ActionResult Create(Customer item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.CustomerName)
                    )
                {
                    var isExist = db.SingleOrDefault<Customer>("CustomerID={0}", item.CustomerID);
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.Shoptype = !string.IsNullOrEmpty(item.Shoptype) ? item.Shoptype : "";
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                    item.Fax = !string.IsNullOrEmpty(item.Fax) ? item.Fax : "";
                    item.Agent = !string.IsNullOrEmpty(item.Agent) ? item.Agent : "";
                    item.Birthday = !string.IsNullOrEmpty(item.Birthday) ? item.Birthday : "";
                    item.Gender = !string.IsNullOrEmpty(item.Gender) ? item.Gender : "";
                    item.LevelHirerachy1 = !string.IsNullOrEmpty(item.LevelHirerachy1) ? item.LevelHirerachy1 : "";
                    item.LevelHirerachy2 = !string.IsNullOrEmpty(item.LevelHirerachy2) ? item.LevelHirerachy2 : "";
                    item.LevelHirerachy3 = !string.IsNullOrEmpty(item.LevelHirerachy3) ? item.LevelHirerachy3 : "";
                    item.LevelHirerachy4 = !string.IsNullOrEmpty(item.LevelHirerachy4) ? item.LevelHirerachy4 : "";
                    item.Desc = !string.IsNullOrEmpty(item.Desc) ? item.Desc : "";
                    item.ProvinceID = !string.IsNullOrEmpty(item.ProvinceID) ? item.ProvinceID : "";
                    item.DistrictID = !string.IsNullOrEmpty(item.DistrictID) ? item.DistrictID : "";
                    if (item.ProvinceID=="")
                        item.DistrictID = "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã khách hàng đã tồn tại" });
                        string id = "";
                        var checkID = db.SingleOrDefault<Customer>("SELECT CustomerID, Id FROM dbo.Customer ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.CustomerID.Substring(2, checkID.CustomerID.Length - 2)) + 1;
                            id = "C" + String.Format("{0:00000000}", nextNo);
                        }
                        else
                        {
                            id = "C00000001";
                        }
                        item.CustomerID = id;
                        item.CustomerName = !string.IsNullOrEmpty(item.CustomerName) ? item.CustomerName : "";
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedBy = "";
                        item.UpdatedAt = DateTime.Parse("1900-01-01");
                        item.CreatedBy = currentUser.UserID;
                        db.Insert(item);
                        return Json(new { success = true, CustomerID = item.CustomerID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.CustomerName = !string.IsNullOrEmpty(item.CustomerName) ? item.CustomerName : "";
                        item.CreatedAt = item.CreatedAt;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = isExist.CreatedBy;
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
                log.Error("Customerion - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult GetDisByProvinceID(string ProvinceID)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<Master_Territory>("select TerritoryID,TerritoryName from Master_Territory where ParentID ={0}" , ProvinceID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }
        public JsonResult GetCustomerHirerachy(string customerid)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                //select list menu cha
                List<CustomerHirerachy> lstFirstMenu = db.Select<CustomerHirerachy>("Status = 1 AND ParentCustomerHirerachyID ='' ").OrderBy(p => p.CustomerHirerachyIndex).ToList();
                List<CustomerHirerachy> allCustomerHirerachy = db.Select<CustomerHirerachy>("Status = 1 ").OrderBy(p => p.CustomerHirerachyIndex).ToList();

                var listCustomer = new List<CustomerHirerachy>();
                if (!string.IsNullOrEmpty(customerid))
                {
                    listCustomer = db.SqlList<CustomerHirerachy>("p_CustomerHirerachy_Select '" + customerid+"'");
                }

                List<CustomerHirerachyViewModel> lstMenuView = new List<CustomerHirerachyViewModel>();
                foreach (CustomerHirerachy der in lstFirstMenu)
                {
                    CustomerHirerachyViewModel node = new CustomerHirerachyViewModel();
                    node.id = der.CustomerHirerachyID;
                    node.text = der.CustomerHirerachyName;
                    node.items = new List<CustomerHirerachyViewModel>();
                    AddChildrenNode(ref node, allCustomerHirerachy, listCustomer);
                    lstMenuView.Add(node);
                }
                return Json(new { success = true, Data = lstMenuView });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        private void AddChildrenNode(ref CustomerHirerachyViewModel node, List<CustomerHirerachy> allCustomerHirerachy, List<CustomerHirerachy> listCustomer)
        {
            try
            {
                string parentID = node.id;
                var obj = listCustomer.Find(p => p.CustomerHirerachyID == parentID);
                if (obj != null)
                {
                    node.@checked = true;
                }
                List<CustomerHirerachy> lstChildMenu = allCustomerHirerachy.Where(p => p.ParentCustomerHirerachyID == parentID).ToList();// db.Select<CustomerHirerachy>("Status = {0} AND ParentCustomerHirerachyID ={1}", true, parentID).OrderBy(p => p.CustomerHirerachyIndex).ToList();//Danh sách menu con của parentID 
                foreach (CustomerHirerachy der in lstChildMenu)
                {
                    CustomerHirerachyViewModel n = new CustomerHirerachyViewModel();
                    n.id = der.CustomerHirerachyID;
                    n.text = der.CustomerHirerachyName;

                    var check = listCustomer.Find(p => p.CustomerHirerachyID == der.CustomerHirerachyID);
                    if (check != null)
                    {
                        n.@checked = true;
                    }

                    n.items = new List<CustomerHirerachyViewModel>();
                    AddChildrenNode(ref n, allCustomerHirerachy, listCustomer);
                    node.items.Add(n);
                }
            }
            catch (Exception)
            {

            }
        }
        [HttpPost]
        public ActionResult SaveCustomerHirerachy(string CustomerID, string CustomerHirerachyIDs)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (string.IsNullOrEmpty(CustomerHirerachyIDs))
                {
                    db.UpdateOnly(new CustomerHirerachyDetail() { Status = false, UpdatedAt = DateTime.Now, UpdatedBy = currentUser.UserID },
                        onlyFields: p => new { p.Status, p.UpdatedAt, p.UpdatedBy },
                        where: p => p.CustomerID == CustomerID);
                }
                else
                {
                    db.ExecuteNonQuery("p_CustomerHirerachyDetail_Save_By_CustomerID @CustomerID, @UserID, @CustomerHirerachyIDs", new
                    {
                        CustomerID = CustomerID,
                        UserID = currentUser.UserID,
                        CustomerHirerachyIDs = CustomerHirerachyIDs
                    });
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetCustomerCode(string CustomerID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.SingleOrDefault<Customer>("CustomerID={0}", CustomerID);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/KhachHang.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<Customer>(whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {

                    ws.Cells["A" + rowNum].Value = item.CustomerID;
                    ws.Cells["B" + rowNum].Value = item.CustomerName;
                    ws.Cells["C" + rowNum].Value = item.Agent;
                    ws.Cells["D" + rowNum].Value = item.Address;
                    ws.Cells["E" + rowNum].Value = item.Email;
                    ws.Cells["F" + rowNum].Value = item.Phone;
                    ws.Cells["G" + rowNum].Value = item.Fax;
                    ws.Cells["H" + rowNum].Value = item.Birthday;
                    ws.Cells["I" + rowNum].Value = item.ProvinceID;
                    ws.Cells["J" + rowNum].Value = item.DistrictID;
                    ws.Cells["K" + rowNum].Value = item.Note;
                    ws.Cells["L" + rowNum].Value = item.CreatedBy;
                    ws.Cells["M" + rowNum].Value = Convert.ToDateTime(item.CreatedAt).ToString("dd/MM/yyyy");
                    ws.Cells["N" + rowNum].Value = item.UpdatedBy;
                    ws.Cells["O" + rowNum].Value = Convert.ToDateTime(item.UpdatedAt).ToString("dd/MM/yyyy");
                    ws.Cells["P" + rowNum].Value = item.Status ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "KhachHang" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}
