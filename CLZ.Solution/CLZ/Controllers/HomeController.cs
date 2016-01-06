using CLZ.CLZService;
using CLZ.Core;
using CLZ.Model.Sys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //AccountModel account = new AccountModel();
            //account.Id = "admin";
            //account.TrueName = "管理员";
            //Session["Account"] = account;
            return View();
        }


        /// <summary>
        /// 获取导航菜单
        /// </summary>
        /// <param name="id">所属</param>
        /// <returns>树</returns>
        public JsonResult GetTree(string id)
        {
            try
            {
                if (Session["Account"] != null)
                {
                    AccountModel account = (AccountModel)Session["Account"];
                    CLZService.CommonServiceClient client = new CLZService.CommonServiceClient();
                    List<SysModule> menus = client.GetMenuByModuleIdAndUserId(id, account.Id);
                    var jsonData = (
                            from m in menus
                            select new
                            {
                                id = m.Id,
                                text = m.Name,
                                value = m.Url,
                                showcheck = false,
                                complete = false,
                                isexpand = false,
                                checkstate = 0,
                                hasChildren = m.IsLast ? false : true,
                                Icon = m.Iconic
                            }
                        ).ToArray();
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else 
                {
                    return Json(0,JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return Json(0,JsonRequestBehavior.AllowGet);
            }
            

        }


        /// <summary>
        /// 获取导航菜单
        /// </summary>
        /// <param name="id">所属</param>
        /// <returns>树</returns>
        public JsonResult GetTreeByEasyui(string id)
        {
            //加入本地化
            CultureInfo info = Thread.CurrentThread.CurrentCulture;
            string infoName = info.Name;
            if (Session["Account"] != null)
            {
                //加入本地化
                AccountModel account = (AccountModel)Session["Account"];
                CLZService.CommonServiceClient client = new CLZService.CommonServiceClient();
                List<SysModule> menus = client.GetMenuByModuleIdAndUserId(id, account.Id);
                var json = from r in menus
                           select new SysModuleNavModel()
                           {
                               id = r.Id,
                               text = infoName.IndexOf("zh") > -1 || infoName == "" ? r.Name : r.EnglishName,     //text
                               attributes = r.Url,
                               iconCls = r.Iconic,
                               state = (client.GetMenuByModuleIdAndUserId(r.Id, account.Id).Count > 0) ? "closed" : "open"

                           };


                return Json(json);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
