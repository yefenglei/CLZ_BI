using CLZ.Common;
using CLZ.Core;
using CLZ.ZHSettleService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class ZHSettleController : BaseController
    {
        //
        // GET: /ZHSettle/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 交易数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public JsonResult GetMainDealInfo(string startDate, string endDate)
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = default(DateTime);
                DateTime ed = default(DateTime);
                sd = GetStartDate(startDate, format);
                ed = GetEndDate(endDate,format);

                List<ZH_SETTLE_DEAL_MAIN> result = client.GetMainSettleDealInfo(sd, ed).ToList();
                var jsondata = new
                {
                    total = result.Count,
                    rows = result
                };
                return MyJson(jsondata, JsonRequestBehavior.AllowGet,"yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }

        /// <summary>
        /// 商品交易数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="limitRowNumber"></param>
        /// <param name="tradeMode">1 代表论斤卖 2 代表论份卖</param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public JsonResult GetGoodsDealInfo(string startDate, string endDate,int tradeMode=1, int limitRowNumber = 0, string sort = "COUNT", string order = "desc")
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = GetStartDate(startDate, format);
                DateTime ed = GetEndDate(endDate, format);
                List<sp_zhsettle_getGoodsDealInfo_Result> result = client.GetZH_DAILY_GOODS_DEALInfo(sd, ed, limitRowNumber, sort, order, tradeMode);

                List<string> xCategories = new List<string>();
                List<double> totalCount = new List<double>();
                List<double> totalPrice = new List<double>();
                foreach (sp_zhsettle_getGoodsDealInfo_Result obj in result)
                {
                    xCategories.Add(obj.PRODUCT_NAME);
                    totalCount.Add((double)obj.COUNT);
                    totalPrice.Add((double)obj.AMOUNT);
                }
                var jsondata = new
                {
                    categories = xCategories,
                    series = new { countList = totalCount, priceList = totalPrice }
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }

        /// <summary>
        /// 获取商品交易分页数据
        /// </summary>
        /// <param name="pager">分页参数</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="limitRowNumber">限制返回的记录条数</param>
        /// <param name="tradeMode">1 代表论斤卖 2 代表论份卖</param>
        /// <returns></returns>
        public JsonResult GetGoodsDealPagingInfo(GridPager pager, string startDate, string endDate,int tradeMode=1, int limitRowNumber = 0)
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = GetStartDate(startDate, format);
                DateTime ed = GetEndDate(endDate, format);

                List<sp_zhsettle_getGoodsDealInfo_Result> result = client.GetZH_DAILY_GOODS_DEALPagingInfo(ref pager, sd, ed, tradeMode);
                int totalCount = pager.totalRows;
                if (limitRowNumber != 0)
                {
                    totalCount = limitRowNumber;
                    result = result.Take(limitRowNumber).ToList();
                }
                var jsondata = new
                {
                    total = totalCount,
                    rows = result
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }


        /// <summary>
        /// 获取指定时间范围内的商品分类销售数据
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="tradeMode">1 代表论斤卖 2 代表论份卖</param>
        /// <returns></returns>
        public JsonResult GetGoodsDealCategoryInfo(string startDate, string endDate,int tradeMode=1)
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = GetStartDate(startDate, format);
                DateTime ed = GetEndDate(endDate, format);
                List<sp_zhsettle_getGoodsDealCategoryInfo_Result> result = client.GetZH_GOODS_DEAL_CategoryInfo(sd, ed, tradeMode);
                var jsondata = new
                {
                    total = result.Count,
                    rows = result
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }


        /// <summary>
        /// 根据商品名称以及时间范围，获取商品销售信息
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="name"></param>
        /// <param name="tradeMode">1 代表论斤卖 2 代表论份卖</param>
        /// <returns></returns>
        public JsonResult GetSingleGoodDealInfo(string startDate, string endDate, string name,int tradeMode=1)
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = GetStartDate(startDate, format);
                DateTime ed = GetEndDate(endDate, format);
                List<ZH_DAILY_GOODS_DEAL> result = client.GetZH_Single_GOODS_DEALInfo(name, sd, ed, tradeMode).OrderBy(t => t.DEAL_DATE).ToList();

                //var q =
                //from p in result
                //group p by new { p.DEAL_DATE,p.PRODUCT_NAME} into g
                //select new ZH_DAILY_GOODS_DEAL
                //{
                //    PRODUCT_NAME = g.Key.PRODUCT_NAME,
                //    AMOUNT = g.Sum(p => p.AMOUNT),
                //    COUNT = g.Sum(p => p.COUNT),
                //    DEAL_DATE=g.Key.DEAL_DATE
                //};
                //result = q.ToList();

                double tc = (double)(result.Sum(n => n.COUNT));
                double tp =  (double)(result.Sum(n => n.AMOUNT));
                var jsondata = new
                {
                    total = result.Count,
                    rows = result,
                    footer = new object[] { new { DEAL_DATE = "合计", COUNT = tc, AMOUNT = tp } }
                };
                return MyJson(jsondata, JsonRequestBehavior.AllowGet,"yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取所有商品列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public JsonResult GetProductInfo_Deprecated(GridPager pager, string searchText) 
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                List<ZH_DIC_PRODUCT> result = client.GetZH_DIC_PRODUCTPagingInfo(ref pager,searchText).OrderBy(p=>p.PRODUCT_NAME).ToList();
                var jsondata = new
                {
                    total = pager.totalRows,
                    rows = result
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }


        /// <summary>
        /// 获取所有商品列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public JsonResult GetProductInfo(GridPager pager, string searchText)
        {
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                List<ZHProductModel> result = client.GetPRODUCTOrderByCountPagingInfo(ref pager, searchText).OrderByDescending(n => n.PRODUCT_TOTAL).ToList();
                var jsondata = new
                {
                    total = pager.totalRows,
                    rows = result
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }

        public ActionResult MainDealInfo()
        {
            return View();
        }

        public ActionResult GoodsDealInfo() 
        {
            return View();
        }

        public ActionResult SingleDealInfo() {
            return View();
        }
        public ActionResult CommercialMatch()
        {
            return View();
        }

        public ActionResult SelectProduct() 
        {
            return View();
        }


        #region 导出excel

        /// <summary>
        /// 导出所有服务点的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportMainDeals(string startDate, string endDate)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());

                List<ZH_SETTLE_DEAL_MAIN> result = client.GetMainSettleDealInfo(sd, ed).ToList();
                DataTable dt = MakeDataTable(result);
                NPOIHelper.Export(dt, "交易数据分析统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "交易数据分析统计表.xls");
        }

        /// <summary>
        /// 导出单个商品的销售数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportSingleProductInfo(string startDate, string endDate, string name, int tradeMode = 1)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<ZH_DAILY_GOODS_DEAL> result = client.GetZH_Single_GOODS_DEALInfo(name, sd, ed, tradeMode).OrderBy(t => t.DEAL_DATE).ToList();
                DataTable dt = MakeDataTableForSingleProduct(result);
                NPOIHelper.Export(dt, name + "销售数据统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", name + "销售数据统计表.xls");
        }

        /// <summary>
        /// 导出商品销售的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportProductSaleInfo(string startDate, string endDate, int tradeMode = 1, int limitRowNumber = 0, string sort = "COUNT", string order = "desc")
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_zhsettle_getGoodsDealInfo_Result> result = client.GetZH_DAILY_GOODS_DEALInfo(sd, ed, limitRowNumber, sort, order, tradeMode);
                DataTable dt = MakeDataTableForProductSale(result);
                NPOIHelper.Export(dt, "商品销售统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "商品销售统计表.xls");
        }

        /// <summary>
        /// 导出商品销售分类的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportProductSaleCategoryInfo(string startDate, string endDate, int tradeMode=1)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                ZHSettleServiceClient client = new ZHSettleServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_zhsettle_getGoodsDealCategoryInfo_Result> result = client.GetZH_GOODS_DEAL_CategoryInfo(sd, ed, tradeMode);
                DataTable dt = MakeDataTableForProductSaleCategory(result);
                NPOIHelper.Export(dt, "商品销售分类占比统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "商品销售分类占比统计表.xls");
        }

        private DataTable MakeDataTable(List<ZH_SETTLE_DEAL_MAIN> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("时间", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("交易数量", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("交易额", Type.GetType("System.String"));

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);

            foreach (ZH_SETTLE_DEAL_MAIN obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["时间"] = obj.DEAL_TIME;
                row["交易数量"] = obj.DEAL_COUNT;
                row["交易额"] = obj.GOODS_CASH;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable MakeDataTableForSingleProduct(List<ZH_DAILY_GOODS_DEAL> sourceList)
        {
            double tc = (double)(sourceList.Sum(n => n.COUNT));
            double tp = (double)(sourceList.Sum(n => n.AMOUNT));
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("时间", typeof(string));
            DataColumn dc2 = new DataColumn("销售重量", typeof(decimal));
            DataColumn dc3 = new DataColumn("销售额", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            foreach (ZH_DAILY_GOODS_DEAL obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["时间"] = obj.DEAL_DATE;
                row["销售重量"] = obj.COUNT;
                row["销售额"] = obj.AMOUNT;
                dt.Rows.Add(row);
            }
            DataRow r = dt.NewRow();
            r["时间"] = "合计";
            r["销售重量"] = tc;
            r["销售额"] = tp;
            dt.Rows.Add(r);

            return dt;
        }

        private DataTable MakeDataTableForProductSale(List<sp_zhsettle_getGoodsDealInfo_Result> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("商品名称", typeof(string));
            DataColumn dc2 = new DataColumn("销售总重量", typeof(decimal));
            DataColumn dc3 = new DataColumn("销售总额", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            foreach (sp_zhsettle_getGoodsDealInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["商品名称"] = obj.PRODUCT_NAME;
                row["销售总重量"] = obj.COUNT;
                row["销售总额"] = obj.AMOUNT;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable MakeDataTableForProductSaleCategory(List<sp_zhsettle_getGoodsDealCategoryInfo_Result> sourceList)
        {
            decimal totalCount = sourceList.Sum(n => n.realCount) == null ? 0 : (decimal)sourceList.Sum(n => n.realCount);
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("商品分类名称", typeof(string));
            DataColumn dc2 = new DataColumn("占比", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            foreach (sp_zhsettle_getGoodsDealCategoryInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["商品分类名称"] = obj.PRODUCT_TYPE_NAME;
                row["占比"] = obj.realCount / totalCount;
                dt.Rows.Add(row);
            }
            return dt;
        }

        #endregion

        private DateTime GetStartDate(string startDate, System.IFormatProvider format) 
        {
            DateTime sd = default(DateTime);
            if (string.IsNullOrEmpty(startDate))
            {
                sd = Convert.ToDateTime("1970/01/01", format);
            }
            else
            {
                try
                {
                    sd = Convert.ToDateTime(startDate, format);
                }
                catch (Exception ex)
                {
                    sd = Convert.ToDateTime("1970/01/01", format);
                    ExceptionHelper.WriteException(new Exception("开始日期格式不对", ex));
                }
            }
            return sd;
        }


        private DateTime GetEndDate(string endDate, System.IFormatProvider format)
        {
            DateTime ed = default(DateTime);
            if (string.IsNullOrEmpty(endDate))
            {
                ed = DateTime.Now;
            }
            else
            {
                try
                {
                    ed = Convert.ToDateTime(endDate, format);
                }
                catch (Exception ex)
                {
                    ed = DateTime.Now;
                    ExceptionHelper.WriteException(new Exception("结束日期格式不对", ex));
                }
            }
            return ed;
        }



        /// <summary>
        /// 日期转换，失败则返回defaultDate
        /// </summary>
        /// <param name="date">需要转换的日期字符串</param>
        /// <returns></returns>
        private DateTime ConvertDate(string date, string defaultDate = "1970/01/01")
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            if (string.IsNullOrEmpty(date))
            {
                return Convert.ToDateTime(defaultDate, format);
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(date, format);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.WriteException(new Exception("开始日期格式不对", ex));
                    return Convert.ToDateTime(defaultDate, format);
                }
            }
        }
    }
}
