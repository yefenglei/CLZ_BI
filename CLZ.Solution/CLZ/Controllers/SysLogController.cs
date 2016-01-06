using CLZ.CLZService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class SysLogController : BaseController
    {
        //
        // GET: /SysLog/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 根据id获取系统日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id) 
        {
            SysLog log = new SysLog();
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                log = client.GetSysLogById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(log);
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public JsonResult GetList(GridPager pager,string queryStr) 
        {
            var json=new object();
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                List<SysLog> list = client.GetSysLogList(ref pager,queryStr);
                json = new { total = pager.totalRows, rows = list };
                
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}
