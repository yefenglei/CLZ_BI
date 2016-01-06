using CLZ.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        [HttpPost]
        public ActionResult Index()
        {
            HttpRequestBase request = Request;
            return View();
        }

        public JsonResult test(string username)
        {
            try
            {
                if(Session["username"]==null)
                {
                    Session["username"] = username;
                }

                return Json(JsonHandler.CreateMessage(1, "测试成功！"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
