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
    [Authorize]
    [NoCache]
    public class AD_UserController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Auth_User().GetPage(request, whereCondition);
            return Json(data);
        }

        [HttpPost]
        public ActionResult Create(Auth_User item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.UserID) &&
                    !string.IsNullOrEmpty(item.DisplayName) &&
                    !string.IsNullOrEmpty(item.FullName))
                {
                    var isExist = db.GetByIdOrDefault<Auth_User>(item.UserID);
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.RowCreatedAt == null && item.RowCreatedBy == null)
                    {
                        if(isExist != null)
                            return Json(new { success = false, message = "Người dùng đã tồn tại." });
                        item.Password = SqlHelper.GetMd5Hash("123456");
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_User>(item);
                        return Json(new { success = true, UserID = item.UserID, RowCreatedAt = item.RowCreatedAt, RowCreatedBy = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.Password = isExist.Password;
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowUpdatedBy = currentUser.UserID;

                        
                        if (isExist.RowCreatedBy != "system")
                        {
                            db.Update<Auth_User>(item);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Dữ liệu này không cho chỉnh sửa liên hệ admin để biết thêm chi tiết" });
                        }
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" });
                }
                else
                {
                    return Json(new { success = false, message = "Chưa nhập giá trị" });
                }
            }
            catch (Exception e)
            {
                log.Error("AD_User - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/NguoiDung.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new Auth_User().GetExport(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.UserID;
                    ws.Cells["B" + rowNum].Value = item.DisplayName;
                    ws.Cells["C" + rowNum].Value = item.FullName;
                    ws.Cells["D" + rowNum].Value = item.Email;
                    ws.Cells["E" + rowNum].Value = item.Phone;
                    ws.Cells["F" + rowNum].Value = item.Note;
                    ws.Cells["G" + rowNum].Value = item.IsActive ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "NguoiDung_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        [HttpPost]
        public ActionResult Import()
        {
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), Request.Files["FileUpload"].FileName);
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        // Save file to folder                            
                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        List<SqlParameter> param = new List<SqlParameter>();
                        param.Add(new SqlParameter("@UserID", currentUser.UserID));
                        param.Add(new SqlParameter("@FilePath", fileLocation));
                        //param.Add(new SqlParameter() { ParameterName = "@Output", Direction = ParameterDirection.InputOutput, Value = 0 });
                        DataSet ds = new SqlHelper().ExcuteQueryDataSet("p_Auth_User_Import", param);
                        if (ds.Tables.Count != 2)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = true, errorfile = ds.Tables[1].Rows[0][0].ToString() });
                        }
                        //int output = Convert.ToInt32(param[param.Count - 1].Value);
                        //if (System.IO.File.Exists(fileLocation))
                        //{
                        //    System.IO.File.Delete(fileLocation);
                        //}
                        //if (output > 0)
                        //    return Json(new { success = true, errorfile = "Loi_Nguoi_Dung.xlsx" });
                        //return Json(new { success = true });
                    }
                }
                return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
            }
            catch (Exception e)
            {
                log.Error("AD_User - Import - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
        }

        //=====================================================================================================        

        public ActionResult PartialUser()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE IsActive = 1");
                dict["listrole"] = new Auth_Role().GetDataForDropDownList();
                dbConn.Close();                
                return PartialView("_AD_User", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        [HttpPost]
        public ActionResult GetUserByID(string userID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Auth_User>(userID);
                var groupUser = dbConn.Select<Auth_UserInRole>(p => p.UserID == userID);
                return Json(new { success = true, data = data, groupuser = groupUser });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        [HttpPost]
        public ActionResult AddUserToGroup(string id, string data)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                // Delete All Group of User
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message  = "Chọn người dùng trước khi thêm nhóm."});
                }
                db.DeleteById<Auth_UserInRole>(id);

                // Add User Group
                if (!string.IsNullOrEmpty(data))
                {
                    string[] arr = data.Split(',');
                    foreach (string item in arr)
                    {
                        var detail = new Auth_UserInRole();
                        detail.UserID = id;
                        detail.RoleID = int.Parse(item);
                        detail.RowCreatedAt = DateTime.Now;
                        detail.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_UserInRole>(detail);
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }

     
        [HttpPost]
        public ActionResult ExecPermissionData(string userID)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@UserID", userID));
                new SqlHelper().ExecuteQuery("p_Generate_Auth_User_AppliedFor_Detail", param);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { }            
        }

        [HttpPost]
        public ActionResult ResetPasswordUser(string userID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(userID) && db.GetByIdOrDefault<Auth_User>(userID) != null)
                {
                    string pass = SqlHelper.GetMd5Hash("123456");
                    db.ExecuteSql("UPDATE [Auth_User] SET Password = '"+ pass +"' WHERE [UserID] = '"+ userID +"'");
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Dữ liệu trống." });
            }
            catch (Exception e)
            {
                log.Error("AD_User - ResetPasswordUser - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
    }
}