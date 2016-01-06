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
    public class SysRoleController : BaseController
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
            CommonServiceClient client = new CommonServiceClient();
            List<SysRole> list = client.GetSysRoleList(ref pager, queryStr);
            var json = new
            {
                total = pager.totalRows,
                rows = list

            };

            return Json(json);
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
        public JsonResult Create(SysRole model)
        {
            CommonServiceClient client = new CommonServiceClient();
            model.CreateTime = ResultHelper.NowTime;
            if (model != null && ModelState.IsValid)
            {
                model.CreatePerson=GetUserId();
                model.CreateTime=DateTime.Now;
                if (client.CreateSysRole(model,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("新增角色,角色Id:{0},角色名称:{1}", model.Id,model.Name), "成功", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("新增角色,角色Id:{0},角色名称:{1}", model.Id, model.Name), "失败", "创建", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
        }
        #endregion

        #region 修改
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            ViewBag.Perm = GetPermission();
            CommonServiceClient client = new CommonServiceClient();
            SysRole entity = client.GetSysRoleById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysRole model)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (model != null && ModelState.IsValid)
            {
                if (client.EditSysRole(model,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("编辑角色,角色Id:{0},角色名称:{1}", model.Id, model.Name), "成功", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("编辑角色,角色Id:{0},角色名称:{1}", model.Id, model.Name), "失败", "修改", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail+Environment.NewLine+error.ErrorMessage));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
        }
        #endregion

        #region 详细
        [SupportFilter]
        public ActionResult Details(string id)
        {
            CommonServiceClient client = new CommonServiceClient();
            ViewBag.Perm = GetPermission();
            SysRole entity = client.GetSysRoleById(id);
            return View(entity);
        }

        #endregion

        #region 删除
        [HttpPost]
        [SupportFilter]
        public JsonResult Delete(string id)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (client.DeleteSysRole(id,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除角色,角色Id:{0}", id), "成功", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除角色,角色Id:{0}", id), "失败", "删除", "SysRole");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail+Environment.NewLine+error.ErrorMessage));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail));
            }


        }
        #endregion

        #region 设置角色用户
        [SupportFilter(ActionName = "Allot")]
        public ActionResult GetUserByRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            ViewBag.Perm = GetPermission();
            return View();
        }

        [SupportFilter(ActionName = "Allot")]
        public JsonResult GetUserListByRole(GridPager pager, string roleId)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (string.IsNullOrWhiteSpace(roleId))
                return Json(0);
            var userList = client.GetUserByRoleId(ref pager, roleId);

            var jsonData = new
            {
                total = pager.totalRows,
                rows = (
                    from r in userList
                    select new
                    {
                        Id = r.Id,
                        UserName = r.UserName,
                        TrueName = r.TrueName,
                        Flag = r.flag == "0" ? "0" : "1",
                    }
                ).ToArray()
            };
            return Json(jsonData);
        }
        #endregion

        [SupportFilter(ActionName = "Save")]
        public JsonResult UpdateUserRoleByRoleId(string roleId, string userIds)
        {
            CommonServiceClient client = new CommonServiceClient();
            string[] arr = userIds.Split(',');

            if (client.UpdateSysRoleSysUserByRoleId(roleId, arr.ToList()) > 0)
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + arr.ToString(), "成功", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(1, Suggestion.SetSucceed), JsonRequestBehavior.AllowGet);
            }
            else
            {
                LogHandler.WriteServiceLog(GetUserId(), "Ids:" + arr.ToString(), "失败", "分配用户", "角色设置");
                return Json(JsonHandler.CreateMessage(0, Suggestion.SetFail+Environment.NewLine+error.ErrorMessage), JsonRequestBehavior.AllowGet);
            }



        }
    }
}
