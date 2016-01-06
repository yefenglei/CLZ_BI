using CLZ.CLZService;
using CLZ.Common;
using CLZ.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class SampleController : BaseController
    {
        //
        // GET: /Sample/
        ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        [SupportFilter(ActionName = "Index")]
        public ActionResult Index()
        {
            CommonServiceClient client = new CommonServiceClient();
            List<SysSample> list = client.getAllSamples();
            return View(list);
        }
        [HttpPost]
        [SupportFilter(ActionName = "Index")]
        public JsonResult GetList(GridPager pager) 
        {
            CommonServiceClient client = new CommonServiceClient();
            List<SysSample> list = client.GetList(ref pager);
            var json = new
            {
                total = pager.totalRows,
                rows = list
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [SupportFilter(ActionName = "Create")]
        public ActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [SupportFilter(ActionName = "Create")]
        public JsonResult Create(SysSample model)
        {
            CLZService.CommonServiceClient client = new CLZService.CommonServiceClient();
            //todo:服务端校验
            model.CreateTime = DateTime.Now;
            if (client.AddSample(model,ref error) == 1)
            {
                LogHandler.WriteServiceLog(TrueName,"新增样例记录 Id"+model.Id,"成功","创建","样例程序");
                return Json(JsonHandler.CreateMessage(1,"新增成功！"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                LogHandler.WriteServiceLog(TrueName, "新增样例记录 Id" + model.Id, "失败", "创建", "样例程序");
                return Json(JsonHandler.CreateMessage(0, "新增失败！," + error), JsonRequestBehavior.AllowGet);
            }
        }
        [SupportFilter(ActionName = "Edit")]
        public ActionResult Edit(int id)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                SysSample model = client.getSysSampleById(id);
                return View(model);
            }
            catch (Exception ex)
            {
                //todo:写入日志 log4net
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }
        
        [HttpPost]
        [SupportFilter(ActionName = "Edit")]
        public JsonResult Edit(SysSample model) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                int result = client.Edit(model,ref error);
                if (1 == result)
                {
                    LogHandler.WriteServiceLog(TrueName, "修改样例记录 id：" + model.Id, "成功", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(1, "修改成功！"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHandler.WriteServiceLog(TrueName, "修改样例记录 id：" + +model.Id, "失败", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(0, "修改失败！," + error), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                //todo:写入日志 log4net
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }
        [SupportFilter(ActionName = "Delete")]
        public JsonResult Delete(int id) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                int result = client.Delete(id,ref error);
                if (1 == result)
                {
                    LogHandler.WriteServiceLog(TrueName, "删除样例记录 id："+id, "成功", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(1, "删除成功！"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    LogHandler.WriteServiceLog(TrueName, "删除样例记录 id：" + id, "失败", "删除", "样例程序");
                    return Json(JsonHandler.CreateMessage(0, "删除失败！," + error), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //todo:写入日志 log4net
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }
        [SupportFilter(ActionName = "Details")]
        public ActionResult Details(int id)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                SysSample entity = client.getSysSampleById(id);
                return View(entity);

            }
            catch (Exception ex)
            {
                //todo:写入日志 log4net
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }
        [SupportFilter(ActionName = "test")]
        public ActionResult test() 
        {
            return View();
        }
        
    }
}
