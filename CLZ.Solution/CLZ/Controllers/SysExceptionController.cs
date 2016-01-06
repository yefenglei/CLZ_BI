using CLZ.CLZService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class SysExceptionController : BaseController
    {
        //
        // GET: /SysException/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id) 
        {
            SysException entity = new SysException();
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                entity = client.GetSysExceptionById(id);
            }
            catch (Exception ex)
            {             
                throw ex;
            }
            return View(entity);
        }

        public JsonResult GetList(GridPager pager,string queryStr)
        {
            object json=null;
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                List<SysException> list = client.GetSysExceptionList(ref pager, queryStr);
                json = new { total = pager.totalRows, rows = list };
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
            return Json(json,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error() 
        {
            BaseException ex = new BaseException();
            return View(ex);
        }
    }
}
