using CLZ.CLZService;
using CLZ.Common;
using CLZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class SysUserController : BaseController
    {
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }


        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager, string queryStr)
        {
            ViewBag.Perm = GetPermission();
            CommonServiceClient client = new CommonServiceClient();
            List<SysUser> list = client.GetSysUserList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = list

            };

            return Json(json);
        }

        #region 设置用户角色
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetRoleByUser(string userId)
        {
            ViewBag.UserId = userId;
            ViewBag.Perm = GetPermission();
            return View();
        }

        [SupportFilter(ActionName = "Allot")]
        public JsonResult GetRoleListByUser(GridPager pager, string userId)
        {
            CommonServiceClient client=new CommonServiceClient();
            if (string.IsNullOrWhiteSpace(userId))
                return Json(0);
            var userList = client.GetRoleByUserId(ref pager, userId);
            var jsonData = new
            {
                total = pager.totalRows,
                rows = (
                    from r in userList
                    select new
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Flag = r.flag == "0" ? "0" : "1",
                    }
                ).ToArray()
            };
            return Json(jsonData);
        }
        #endregion
        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByUserId(string userId, string roleIds)
        {
            CommonServiceClient client = new CommonServiceClient();
            string[] arr = roleIds.Split(',');


            if (client.UpdateSysRoleSysUser(userId, arr.ToList())>0)
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + roleIds, "成功", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + roleIds, "失败", "分配角色", "用户设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.SetFail), JsonRequestBehavior.AllowGet);
            }

        }

        #region 创建
        [SupportFilter]
        public ActionResult Create()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysUser model)
        {
            CommonServiceClient client = new CommonServiceClient();
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {
                model.CreatePerson = GetUserId();
                model.CreateTime = DateTime.Now;
                model.Password = ValueConvert.MD5("123456");
                model.State = true;
                if (client.CreateSysUser(model,ref error) == 1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("创建用户,用户Id:{0},用户Name:{1}", model.Id,model.UserName), "成功", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("创建用户,用户Id:{0},用户Name:{1}", model.Id, model.UserName), "失败", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail+Environment.NewLine+error.ErrorMessage));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
        }
        #endregion

    }
}
