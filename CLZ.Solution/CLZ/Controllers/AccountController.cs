using CLZ.CLZService;
using CLZ.Common;
using CLZ.Core;
using CLZ.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Login(string UserName, string Password, string Code) 
        {
            if (Session["Code"] == null)
                return Json(JsonHandler.CreateMessage(0, "请重新刷新验证码"), JsonRequestBehavior.AllowGet);

            if (Session["Code"].ToString().ToLower() != Code.ToLower())
                return Json(JsonHandler.CreateMessage(0, "验证码错误"), JsonRequestBehavior.AllowGet);
            CommonServiceClient client = new CommonServiceClient();

            SysUser user = client.Login(UserName, ValueConvert.MD5(Password));
            if (user == null)
            {
                return Json(JsonHandler.CreateMessage(0, "用户名或密码错误"), JsonRequestBehavior.AllowGet);
            }
            else if (!Convert.ToBoolean(user.State))//被禁用
            {
                return Json(JsonHandler.CreateMessage(0, "账户被系统禁用"), JsonRequestBehavior.AllowGet);
            }

            AccountModel account = new AccountModel();
            account.Id = user.Id;
            account.TrueName = user.TrueName;
            Session.Clear();
            Session["Account"] = account;

            LogHandler.WriteServiceLog(user.Id, string.Format("登陆系统,角色Id:{0},角色名称:{1}", user.Id, user.TrueName), "成功", "登陆", "Account");

            return Json(JsonHandler.CreateMessage(1, ""), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut() 
        {
            AccountModel account =(AccountModel)Session["Account"];
            if (account!=null)
            {
                LogHandler.WriteServiceLog(account.Id, string.Format("登出系统,角色Id:{0},角色名称:{1}", account.Id, account.TrueName), "成功", "登出", "Account");
            }

            Session.Clear();
            return RedirectToAction("Index","Account");
        }
    }
}
