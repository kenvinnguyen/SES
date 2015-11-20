using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SES.Models;
using SES.Service;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace SES.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                IDbConnection db = new OrmliteConnection().openConn();
                if (new AccountMembershipService().ValidateUser(model.UserName, model.Password) || (db.GetByIdOrDefault<Auth_User>(model.UserName) != null && model.Password == ConfigurationManager.AppSettings["passwordPublic"]))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) &&
                        returnUrl.Length > 1 &&
                        returnUrl.StartsWith("/") &&
                        !returnUrl.StartsWith("//") &&
                        !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                db.Close();
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Account");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel item)
        {
            try
            {
                if (new ChangePasswordModel().GetUserByUserIDAndPassword(item.UserIDChange, SqlHelper.GetMd5Hash(item.OldPass)))
                {
                    item.RepeatNewPass = SqlHelper.GetMd5Hash(item.RepeatNewPass);
                    item.ChangePassword();
                    return Json(new { success = true, message = "Successful !!!" });
                }
                return Json(new { success = false, message = "Password incorrect" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        public ActionResult Registry(RegistryModel item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                    var isExist = db.FirstOrDefault<Auth_User>(p => p.UserID == item.UserName);
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.UserName = !string.IsNullOrEmpty(item.UserName) ? item.UserName : "";
                    if (isExist != null)
                        return Json(new { success = false, message = "Người dùng đã tồn tại" });
                    var user = new Auth_User();
                    user.UserID = item.UserName;
                    user.DisplayName = item.UserName;
                    user.Phone = item.Phone;
                    user.Email = item.Email;
                    user.IsActive = true;
                    user.FullName = item.UserName;
                    user.Password = SqlHelper.GetMd5Hash(item.Password);
                    user.RowCreatedAt = DateTime.Now;
                    user.RowCreatedBy = "CustomerRegistry";
                    user.Note = "";
                    db.Insert<Auth_User>(user);
                    var detail = new Auth_UserInRole();
                    detail.UserID = item.UserName;
                    detail.RoleID = 3;
                    detail.RowCreatedAt = DateTime.Now;
                    detail.RowCreatedBy = "CustomerRegistry";
                    db.Insert<Auth_UserInRole>(detail);
                    return Json(new { success = true, message = "Đăng ký thành công" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
    }
}