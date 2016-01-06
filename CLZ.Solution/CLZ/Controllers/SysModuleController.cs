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
    public class SysModuleController : BaseController
    {
        //
        // GET: /SysModuleOperate/
        [SupportFilter]
        public ActionResult Index()
        {
            ViewBag.Perm = GetPermission();
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pager">分页</param>
        /// <param name="queryStr">查询条件</param>
        /// <returns></returns>
        [SupportFilter(ActionName = "Index")]
        [HttpPost]
        public JsonResult GetList(string id)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                if (id == null)
                    id = "0";
                List<SysModule> list = client.GetModuleByParentId(id);
                var json = from r in list
                           select new 
                           {
                               Id = r.Id,
                               Name = r.Name,
                               EnglishName = r.EnglishName,
                               ParentId = r.ParentId,
                               Url = r.Url,
                               Iconic = r.Iconic,
                               Sort = r.Sort,
                               Remark = r.Remark,
                               Enable = r.Enable,
                               CreatePerson = r.CreatePerson,
                               CreateTime = r.CreateTime,
                               IsLast = r.IsLast,
                               state = (client.GetModuleByParentId(r.Id).Count > 0) ? "closed" : "open"
                           };
                return Json(json);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetOptListByModule(GridPager pager, string mid)
        {
            pager.rows = 1000;
            pager.page = 1;
            CommonServiceClient client = new CommonServiceClient();
            List<SysModuleOperate> list = client.GetModuleOperateList(ref pager, mid);
            var json = new
            {
                total = pager.totalRows,
                rows = list

            };

            return Json(json);
        }


        #region 创建模块
        [SupportFilter]
        public ActionResult Create(string id)
        {
            ViewBag.Perm = GetPermission();
            SysModule entity = new SysModule()
            {
                ParentId = id,
                Enable = true,
                Sort = 0
            };
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Create(SysModule model)
        {
            CommonServiceClient client = new CommonServiceClient();
            //model.Id = ResultHelper.NewId;
            model.CreateTime = ResultHelper.NowTime;
            model.CreatePerson = GetUserId();
            if (model != null && ModelState.IsValid)
            {
                if (client.CreateModule(model,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), "创建新模块 模块Id："+model.Id+" 模块名字："+model.Name, "成功", "创建", "模块维护");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), "创建新模块 模块Id：" + model.Id + " 模块名字：" + model.Name, "失败", "创建", "模块维护");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail+" "+error.ErrorMessage));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail));
            }
        }
        #endregion


        #region 创建
        [SupportFilter(ActionName = "Create")]
        public ActionResult CreateOpt(string moduleId)
        {
            ViewBag.Perm = GetPermission();
            SysModuleOperate sysModuleOptModel = new SysModuleOperate();
            sysModuleOptModel.ModuleId = moduleId;
            sysModuleOptModel.IsValid = true;
            return View(sysModuleOptModel);
        }


        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult CreateOpt(SysModuleOperate info)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (info != null && ModelState.IsValid)
            {
                SysModuleOperate entity = client.GetModuleOperateById(info.Id);
                if (entity != null)
                {
                    return Json(JsonHandler.CreateMessage(0, Suggestion.PrimaryRepeat), JsonRequestBehavior.AllowGet);
                }
                info.Id = info.ModuleId + info.KeyCode;

                if (client.CreateModuleOperate(info,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("新增模块操作码,模块ID:{0},操作码name:{1},操作码key:{2}",info.ModuleId,info.Name,info.KeyCode), "成功", "创建", "模块设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.InsertSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("新增模块操作码,模块ID:{0},操作码name:{1},操作码key:{2}", info.ModuleId, info.Name, info.KeyCode), "失败", "创建", "模块设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail+Environment.NewLine+error.ErrorMessage), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.InsertFail), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 修改模块
        [SupportFilter]
        public ActionResult Edit(string id)
        {
            CommonServiceClient client = new CommonServiceClient();
            ViewBag.Perm = GetPermission();
            SysModule entity = client.GetModuleById(id);
            return View(entity);
        }

        [HttpPost]
        [SupportFilter]
        public JsonResult Edit(SysModule model)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (model != null && ModelState.IsValid)
            {
                if (client.EditModule(model,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("修改模块,模块ID:{0},模块名称:{1}", model.Id,model.Name), "成功", "修改", "系统菜单");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.EditSucceed));
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("修改模块,模块ID:{0},模块名称:{1}", model.Id, model.Name), "失败", "修改", "系统菜单");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail+Environment.NewLine+error.ErrorMessage));
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.EditFail));
            }
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
                if (client.DeleteModule(id,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除模块,模块ID:{0}", id), "成功", "删除", "模块设置");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除模块,模块ID:{0}", id), "失败", "删除", "模块设置");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail+Environment.NewLine+error.ErrorMessage), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        [SupportFilter(ActionName = "Delete")]
        public JsonResult DeleteOpt(string id)
        {
            CommonServiceClient client = new CommonServiceClient();
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (client.DeleteModuleOperateById(id,ref error)==1)
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除模块操作码,模块操作码ID:{0}", id), "成功", "删除", "模块设置KeyCode");
                    return Json(JsonHandler.CreateMessage(1, Suggestion.DeleteSucceed), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHandler.WriteServiceLog(GetUserId(), string.Format("删除模块操作码,模块操作码ID:{0}", id), "失败", "删除", "模块设置KeyCode");
                    return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail+Environment.NewLine+error.ErrorMessage), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonHandler.CreateMessage(0, Suggestion.DeleteFail), JsonRequestBehavior.AllowGet);
            }


        }

        #endregion


    }
}
