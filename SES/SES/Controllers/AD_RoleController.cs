using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SES.Service;
using SES.Models;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;

namespace SES.Controllers
{
    [Authorize]
    [NoCache]
    public class AD_RoleController : CustomController
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
            var data = new Auth_Role().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(form["RoleName"]))
                {
                    var item = new Auth_Role();
                    item.RoleName = form["RoleName"];
                    item.IsActive = form["IsActive"] != null ? Convert.ToBoolean(form["IsActive"]) : false;
                    item.Note = !string.IsNullOrEmpty(form["Note"]) ? form["Note"] : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] &&
                        string.IsNullOrEmpty(form["RoleID"]))    // Tạo mới
                    {
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_Role>(item);
                        long lastID = db.GetLastInsertId();
                        if (lastID > 0)
                        {
                            // Thêm Role vào Auth_Action
                            db.ExecuteSql("EXEC p_Auth_Role_GenerateAction_By_RoleID " + lastID + "," + currentUser.UserID);
                        }
                        return Json(new { success = true, insert = true, RoleID = lastID, createdat = item.RowCreatedAt, createdby = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Insert") && userAsset["Insert"] &&
                            Convert.ToInt32(form["RoleID"]) > 0 &&
                            Convert.ToInt32(form["IsCopy"]) == 1)  // Sao chép
                    {
                        item.RoleID = Convert.ToInt32(form["RoleID"]);
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_Role>(item);
                        long lastID = db.GetLastInsertId();
                        if (lastID > 0)
                        {
                            // Sao chép Action RoleID đã chọn vào RoleID vừa tạo                            
                            db.ExecuteSql("p_Auth_Role_CopyAction_By_RoleID " + item.RoleID + "," + lastID + "," + currentUser.UserID);
                        }
                        return Json(new { success = true, insert = true, RoleID = lastID, createdat = item.RowCreatedAt, createdby = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] &&
                            Convert.ToInt32(form["RoleID"]) > 0)    // Cập nhật
                    {
                        item.RoleID = Convert.ToInt32(form["RoleID"]);
                        item.RowCreatedAt = DateTime.Parse(form["RowCreatedAt"]);
                        item.RowCreatedBy = form["RowCreatedBy"];
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowUpdatedBy = currentUser.UserID;
                        if (item.RowCreatedBy != "system")
                        {
                            db.Update<Auth_Role>(item);
                        }
                      
                        return Json(new { success = true, RoleID = item.RoleID });
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
                log.Error("HOAdminAuthRole - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        //========================================== Tab Phân quyền chức năng =================================

        public ActionResult ReadAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid)
        {
            var data = new List<Auth_Action>();
            if (roleid > 0 && !string.IsNullOrEmpty(menuid))
            {
                IDbConnection db = new OrmliteConnection().openConn();
                data = db.Select<Auth_Action>(p => p.RoleID == roleid && p.MenuID == menuid &&
                                                    p.Action != "View" &&
                                                    p.Action != "Insert" &&
                                                    p.Action != "Update" &&
                                                    p.Action != "Delete" &&
                                                    p.Action != "Export" &&
                                                    p.Action != "Import");
                db.Close();
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid, [Bind(Prefix = "models")]IEnumerable<Auth_Action> lst)
        {
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                IDbConnection db = new OrmliteConnection().openConn();
                foreach (var item in lst)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.Action))
                        {
                            ModelState.AddModelError("er", "Xin nhập quyền");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        var isExist = db.SingleOrDefault<Auth_Action>("RoleID = {0} AND MenuID = {1} AND Action = {2}", roleid, menuid, item.Action);
                        if (isExist != null)
                        {
                            ModelState.AddModelError("er", "Quyền đã tồn tại");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        try
                        {
                            item.RoleID = roleid;
                            item.MenuID = menuid;
                            item.Note = "";
                            item.RowCreatedAt = DateTime.Now;
                            item.RowCreatedBy = currentUser.UserID;
                            db.Insert<Auth_Action>(item);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("er", ex.Message);
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                db.Close();
                return Json(lst.ToDataSourceResult(request));
            }
            else
            {
                ModelState.AddModelError("er", "Bạn không có quyền tạo mới");
                return Json(lst.ToDataSourceResult(request, ModelState));
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid, [Bind(Prefix = "models")]IEnumerable<Auth_Action> lst)
        {
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                IDbConnection db = new OrmliteConnection().openConn();
                foreach (var item in lst)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.Action))
                        {
                            ModelState.AddModelError("er", "Xin nhập quyền");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        var isExist = db.SingleOrDefault<Auth_Action>("RoleID = {0} AND MenuID = {1} AND Action = {2}", roleid, menuid, item.Action);
                        if (isExist != null)
                        {
                            try
                            {
                                item.RoleID = roleid;
                                item.MenuID = menuid;
                                item.Note = "";
                                item.RowCreatedAt = isExist.RowCreatedAt;
                                item.RowCreatedBy = isExist.RowCreatedBy;
                                item.RowUpdatedAt = DateTime.Now;
                                item.RowUpdatedBy = currentUser.UserID;
                                db.Update<Auth_Action>(item, p => p.RoleID == roleid && p.MenuID == menuid && p.Action == item.Action);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("er", ex.Message);
                                return Json(lst.ToDataSourceResult(request, ModelState));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("er", "Quyền không tồn tại");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                db.Close();
                return Json(lst.ToDataSourceResult(request));
            }
            else
            {
                ModelState.AddModelError("er", "Bạn không có quyền cập nhật");
                return Json(lst.ToDataSourceResult(request, ModelState));
            }

        }

        //=====================================================================================================        

        public ActionResult PartialRole()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["user"] = dbConn.Select<Auth_User>(p => p.IsActive == true);
                dbConn.Close();
                return PartialView("_AD_Role", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        [HttpPost]
        public ActionResult GetByID(int id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Auth_Role>(id);
                var listUserRole = dbConn.Select<Auth_UserInRole>(p => p.RoleID == id);
                return Json(new { success = true, data = data, listuser = listUserRole });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        [HttpPost]
        public ActionResult AddUserToGroup(int id, string data)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                // Delete All User in Role
                db.Delete<Auth_UserInRole>(p => p.RoleID == id);

                // Add User Role
                if (!string.IsNullOrEmpty(data))
                {
                    string[] arr = data.Split(',');
                    foreach (string item in arr)
                    {
                        var detail = new Auth_UserInRole();
                        detail.UserID = item;
                        detail.RoleID = id;
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

        //public JsonResult GetMenu([DataSourceRequest] DataSourceRequest request)
        //{
        //    IDbConnection db = new OrmliteConnection().openConn();
        //    try
        //    {
        //        var listMenu = db.Select<Auth_Menu>(p => p.IsVisible == true).OrderBy(p => p.MenuIndex).ToList();
        //        DataSourceResult dsr = new DataSourceResult();
        //        dsr.Data = listMenu;
        //        return Json(dsr, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message });
        //    }
        //    finally { db.Close(); }
        //}
        
        //[HttpPost]
        public JsonResult GetMenu(string action, int roleID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {                
                //select list menu cha
                List<Auth_Menu> lstFirstMenu = db.Select<Auth_Menu>("IsVisible = 1 AND ParentMenuID ='' AND MenuID <> 'Home'").OrderBy(p => p.MenuIndex).ToList();
                List<Auth_Menu> allAuthMenu = db.Select<Auth_Menu>("IsVisible = 1  AND MenuID <> 'Home'").OrderBy(p => p.MenuIndex).ToList();

                var listAction = new List<Auth_Menu>();
                if (!string.IsNullOrEmpty(action))
                {
                    listAction = db.SqlList<Auth_Menu>("p_Auth_Menu_Select_By_Action '" + action + "', " + roleID);
                }

                List<AuthMenuViewModel> lstMenuView = new List<AuthMenuViewModel>();
                foreach (Auth_Menu der in lstFirstMenu)
                {
                    AuthMenuViewModel node = new AuthMenuViewModel();
                    node.id = der.MenuID;
                    node.text = der.MenuName;
                    node.items = new List<AuthMenuViewModel>();
                    AddChidrenNode(ref node, allAuthMenu,listAction);
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

        private void AddChidrenNode(ref AuthMenuViewModel node, List<Auth_Menu> allAuthMenu, List<Auth_Menu> listAction)
        {
            try
            {
                string parentID = node.id;
                var obj = listAction.Find(p => p.MenuID == parentID);
                if(obj != null)
                {
                    node.@checked = true;
                }
                List<Auth_Menu> lstChildMenu = allAuthMenu.Where(p => p.ParentMenuID == parentID).ToList();// db.Select<Auth_Menu>("IsVisible = {0} AND ParentMenuID ={1}", true, parentID).OrderBy(p => p.MenuIndex).ToList();//Danh sách menu con của parentID 
                foreach (Auth_Menu der in lstChildMenu)
                {
                    AuthMenuViewModel n = new AuthMenuViewModel();
                    n.id = der.MenuID;
                    n.text = der.MenuName;

                    var check = listAction.Find(p => p.MenuID == der.MenuID);
                    if (check != null)
                    {
                        n.@checked = true;
                    }

                    n.items = new List<AuthMenuViewModel>();                    
                    AddChidrenNode(ref n, allAuthMenu, listAction);
                    node.items.Add(n);
                }                
            }
            catch (Exception)
            {

            }
        }

        [HttpPost]
        public ActionResult SavePermission(int RoleID, string Action, string MenuIDs)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if(RoleID == 0 || string.IsNullOrEmpty(Action))
                    return Json(new { success = false, message = "Có dữ liệu rỗng" });
                if (string.IsNullOrEmpty(MenuIDs))
                {
                    db.UpdateOnly(new Auth_Action() { IsAllowed = false, RowUpdatedAt = DateTime.Now, RowUpdatedBy = currentUser.UserID },
                        onlyFields: p => new { p.IsAllowed, p.RowUpdatedAt, p.RowUpdatedBy },
                        where: p => p.RoleID == RoleID && p.Action == Action);
                }
                else
                {
                    db.ExecuteNonQuery("p_Auth_Action_Save_By_RoleID @RoleID, @UserID, @Action, @MenuIDs", new
                    {
                        RoleID = RoleID,
                        UserID = currentUser.UserID,
                        Action = Action,
                        MenuIDs = MenuIDs
                    });
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }
    }
}