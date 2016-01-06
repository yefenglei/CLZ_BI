using CLZ.CLZService;
using CLZ.Common;
using CLZ.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class DataAnalysisController : BaseController
    {
        //
        // GET: /DataAnalysis/

        public ActionResult Index()
        {
            return View();
        }



        public JsonResult GetChartOrderInfo(string startDate, string endDate, int compareDay) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = default(DateTime);
                DateTime ed = default(DateTime);
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
                
                int cd = compareDay;
                List<sp_wx_getOrderInfo_Result> result = client.GetWXProductOrderInfo(sd, ed, cd).OrderBy(n => n.time).ToList();
                List<string> xCategories = new List<string>();
                List<int> order = new List<int>();
                List<double> sale = new List<double>();
                foreach (sp_wx_getOrderInfo_Result obj in result)
                {
                    xCategories.Add(obj.time.ToShortDateString());
                    order.Add((int)obj.orderCount);
                    sale.Add((double)obj.sale);
                }
                var jsondata = new
                {
                    categories = xCategories,
                    series = new { orderList = order, saleList = sale }
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            
        }

        public JsonResult GetProductOrderInfoByAddressId(string startDate, string endDate, string addressName)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate);
                List<ProductOrder> result = client.GetWXProductOrderInfoByAddressId(sd, ed, addressName).OrderBy(n => n.time).ToList();
                List<string> xCategories = new List<string>();
                List<int> order = new List<int>();
                List<double> sale = new List<double>();
                foreach (ProductOrder obj in result)
                {
                    xCategories.Add(obj.time.ToShortDateString());
                    order.Add((int)obj.count);
                    sale.Add((double)obj.price);
                }
                var jsondata = new
                {
                    categories = xCategories,
                    series = new { orderList = order, saleList = sale }
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }

        public JsonResult GetWXChartOrderTableInfo(GridPager pager, string startDate, string endDate, int compareDay) 
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            DateTime sd = ConvertDate(startDate);
            DateTime ed = ConvertDate(endDate,DateTime.Now.ToShortDateString());
            int cd = compareDay;
            CommonServiceClient client = new CommonServiceClient();
            List<sp_wx_getOrderInfo_Result> result = client.GetWXProductOrderTableInfo(ref pager,sd, ed, cd).ToList();
            
            var jsondata = new
            {
                total = pager.totalRows,
                rows = result
            };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GettWXCustomerInfo(string startDate, string endDate, int compareDay, int pageSize = 100, string customerName="")
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                int cd = compareDay;
                List<sp_wx_getCustomerInfo_Result> result = client.GetWXCustomerInfo(sd, ed, cd).Take(pageSize).ToList();
                if (!string.IsNullOrEmpty(customerName))
                {
                    result= result.Where(r=>r.truename==customerName).ToList();
                }
                List<string> xCategories = new List<string>();
                List<int> order = new List<int>();
                List<double> count = new List<double>();
                List<double> price = new List<double>();
                foreach (sp_wx_getCustomerInfo_Result obj in result)
                {
                    xCategories.Add(obj.truename);
                    order.Add((int)obj.totalOrder);
                    count.Add((double)obj.totalCount);
                    price.Add((double)obj.totalPrice);
                }
                var jsondata = new
                {
                    categories = xCategories,
                    series = new { orderList = order, countList = count,priceList=price }
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }

        public JsonResult GetWXCustomerPurchaseRateInfo(string startDate, string endDate) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());

                List<PurchaseRate> result = client.GetWXCustomerRateOfPurchaseInfo(sd,ed);
                if(result==null)
                {
                    return null;
                }
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

        public JsonResult GetWXCustomerInfoByBuyTimes(string startDate, string endDate,int buyTimes) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());

                List<CustomerOrder> result = client.GetWXCustomerInfoByBuyTimes(sd, ed, buyTimes);
                if (result == null)
                {
                    return null;
                }
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

        public JsonResult GettWXSingleCustomerInfo(string customerName,string startDate, string endDate)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<wx_customer_order> result = client.GetWXSingleCustomerInfo(customerName, sd, ed).OrderBy(t => t.time).ToList();

                int tc = (int)(result.Sum(n => n.totalCount));
                int to = (int)(result.Sum(n => n.totalOrder));
                double tp =  (double)(result.Sum(n => n.totalPrice));
                var jsondata = new
                {
                    total = result.Count,
                    rows = result,
                    footer = new object[] { new { time = "合计", totalCount = tc, totalPrice = tp,totalOrder=to } }
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }

        public JsonResult GettWXCustomerTableInfo(string startDate, string endDate, int compareDay, int pageSize = 100, string customerName = "") 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                int cd = compareDay;
                List<sp_wx_getCustomerInfo_Result> result = client.GetWXCustomerInfo(sd, ed, cd).Take(pageSize).ToList();
                if (!string.IsNullOrEmpty(customerName))
                {
                    result = result.Where(r => r.truename == customerName).ToList();
                }
                var jsondata = new
                {
                    total = result.Count(),
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


        public JsonResult GetWXProductSaleInfo(string startDate, string endDate, int compareDay, int limitRowNumber = 0,string sort="totalCount",string order="desc")
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                int cd = compareDay;
                List<sp_wx_getProductInfo_Result> result = client.GetWXProductSaleInfo(sd, ed, cd);

                if (order == "desc")
                {
                    switch (sort)
                    {
                        case "totalPrice":
                            result = result.OrderByDescending(c => c.totalPrice).ToList();
                            break;
                        case "totalCount":
                            result = result.OrderByDescending(c => c.totalCount).ToList();
                            break;
                        default:
                            result = result.OrderByDescending(c => c.name).ToList();
                            break;
                    }
                }
                else
                {

                    switch (sort)
                    {
                        case "totalPrice":
                            result = result.OrderBy(c => c.totalPrice).ToList();
                            break;
                        case "totalCount":
                            result = result.OrderBy(c => c.totalCount).ToList();
                            break;
                        default:
                            result = result.OrderBy(c => c.name).ToList();
                            break;
                    }
                }
                if(limitRowNumber!=0)
                {
                    result = result.Take(limitRowNumber).ToList();
                }

                List<string> xCategories = new List<string>();
                List<int> totalCount = new List<int>();
                List<double> totalPrice = new List<double>();
                foreach (sp_wx_getProductInfo_Result obj in result)
                {
                    xCategories.Add(obj.name);
                    totalCount.Add((int)obj.totalCount);
                    totalPrice.Add((double)obj.totalPrice);
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


        public JsonResult GetWXProductSaleTableInfo(string startDate, string endDate, int compareDay, int pageSize = 1000,string sort="totalCount",string order="desc")
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                int cd = compareDay;
                List<sp_wx_getProductInfo_Result> result=null;
                if (pageSize != 1000)
                {
                    result = client.GetWXProductSaleInfo(sd, ed, cd).Take(pageSize).ToList();
                }
                else
                {
                    result = client.GetWXProductSaleInfo(sd, ed, cd).Take(pageSize).OrderBy(r => r.name).ToList();
                }
                if (order == "desc")
                {
                    switch (sort)
                    {
                        case "totalPrice":
                            result = result.OrderByDescending(c => c.totalPrice).ToList();
                            break;
                        case "totalCount":
                            result = result.OrderByDescending(c => c.totalCount).ToList();
                            break;
                        default:
                            result = result.OrderByDescending(c => c.name).ToList();
                            break;
                    }
                }
                else
                {

                    switch (sort)
                    {
                        case "totalPrice":
                            result = result.OrderBy(c => c.totalPrice).ToList();
                            break;
                        case "totalCount":
                            result = result.OrderBy(c => c.totalCount).ToList();
                            break;
                        default:
                            result = result.OrderBy(c => c.name).ToList();
                            break;
                    }
                }

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

        public JsonResult GetWXPagingProductSaleTableInfo(GridPager pager, string startDate, string endDate, int compareDay, int limitRowNumber=0)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();

                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                int cd = compareDay;
                List<sp_wx_getProductInfo_Result> result = null;
                result = client.GetWXPagingProductSaleInfo(ref pager,sd,ed,cd);
                int totalCount = pager.totalRows;
                if (limitRowNumber!=0)
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
        /// <returns></returns>
        public JsonResult GetWXProductCategorySaleInfo(string startDate, string endDate)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getProductCategoryInfo_Result> result = client.GetWXProductCategoryInfo(sd, ed);
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
        /// <returns></returns>
        public JsonResult GetWXSingleProductSaleInfo(string startDate, string endDate,string name) 
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> result = client.GetWXSingleProductSaleInfoByNameAndDate(name,sd, ed).OrderBy(t=>t.time).ToList();

                int tc = result.Sum(n=>n.totalCount)==null?0:(int)(result.Sum(n=>n.totalCount));
                double tp = result.Sum(n => n.totalPrice) == null ? 0 : (double)(result.Sum(n => n.totalPrice));
                var jsondata = new
                {
                    total = result.Count,
                    rows = result,
                    footer = new object[]{ new { time = "合计",totalCount=tc,totalPrice=tp}}
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }


        public JsonResult GetWXRegionOrderInfo(string startDate, string endDate)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                sd = new DateTime(sd.Year, sd.Month, 1);// 计算获取该月第一天的日期
                ed = new DateTime(ed.Year,ed.Month+1,1).AddDays(-1);// 计算获取该月最后一天的日期
                List<sp_wx_getRegionProductInfo_Result> result = client.GetWXRegionProductInfo(sd, ed);
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


        public JsonResult GetWXMonthlyOrderInfoByAddressId(string startDate, string endDate,string addressName)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                sd = new DateTime(sd.Year, sd.Month, 1);// 计算获取该月第一天的日期
                ed = new DateTime(ed.Year, ed.Month + 1, 1).AddDays(-1);// 计算获取该月最后一天的日期
                int addressId = client.get_wx_service_points_ByName(addressName).Id;
                List<sp_wx_getMonthlyProductInfoByAddressId_Result> result = client.GetWXMonthlyProductOrderInfo(sd, ed, addressId);
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


        public ActionResult SingleProductSaleInfo()
        {
            return View();
        }

        public ActionResult CustomerInfo(string startDate="", string endDate="",string customerName = "") 
        {
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.customerName = customerName;
            return View();
        }

        public ActionResult ProductSaleInfo() 
        {
            return View();
        }

        public ActionResult RegionOrderInfo() 
        {
            return View();
        }

        public ActionResult RateOfPurchase() 
        {
            return View();
        }

        public ActionResult PaymentTerms() 
        {
            return View();
        }

        /// <summary>
        /// 获取指定时间范围内的支付方式信息
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="payTypes">支付类型 如alipay,wcpay</param>
        /// <returns></returns>
        public JsonResult GetWXPaymentTermsInfo(string startDate, string endDate,string payTypes)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<wx_product_cart> result = client.GetWXProductCartList();
                string[] types = payTypes.ToLower().Split(',');
                List<object> list = new List<object>();
                foreach(string payType in types)
                {
                    var data = from q in result
                               where q.time >= sd && q.time <= ed && q.pay_type == payType
                               group q by new { q.time, q.pay_type }
                                   into g
                                   orderby g.Key.time descending
                                   select new
                                   {
                                       g.Key.pay_type,
                                       time = new DateTime(g.Key.time.Year, g.Key.time.Month, g.Key.time.Day),
                                       count = g.Count()
                                   };
                    list.Add(data.ToList());
                }
                

                var jsondata = new
                {
                    total = list.Count(),
                    rows = list.Count>1?list.ToArray():list[0]
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
        /// 获取指定时间内的各个支付方式的占比
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public JsonResult GetWXPaymentTermsPercentageInfo(string startDate, string endDate)
        {
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<wx_product_cart> result = client.GetWXProductCartList();
                var data = from q in result
                           where q.time >= sd && q.time <= ed
                           group q by new { q.pay_type }
                               into g
                               select new
                               {
                                   payType=g.Key.pay_type,
                                   count = g.Count()
                               };

                var jsondata = new
                {
                    total = data.Count(),
                    rows = data
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }


        #region 导出excel文件

        /// <summary>
        /// 导出所有服务点的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportAllServicePointOrders(string startDate, string endDate)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                sd = new DateTime(sd.Year, sd.Month, 1);// 计算获取该月第一天的日期
                ed = new DateTime(ed.Year, ed.Month + 1, 1).AddDays(-1);// 计算获取该月最后一天的日期
                List<sp_wx_getRegionProductInfo_Result> result = client.GetWXRegionProductInfo(sd, ed);
                DataTable dt = MakeDataTable(result);
                NPOIHelper.Export(dt, "服务点数据分析统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "服务点数据分析统计表.xls");
        }

        /// <summary>
        /// 导出所有服务点的订单数量占比数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportAllServicePointRate(string startDate, string endDate)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                sd = new DateTime(sd.Year, sd.Month, 1);// 计算获取该月第一天的日期
                ed = new DateTime(ed.Year, ed.Month + 1, 1).AddDays(-1);// 计算获取该月最后一天的日期
                List<sp_wx_getRegionProductInfo_Result> result = client.GetWXRegionProductInfo(sd, ed);
                DataTable dt = MakeDataTableForServicePointRate(result);
                NPOIHelper.Export(dt, "服务点订单数占比统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "服务点订单数占比统计表.xls");
        }

        /// <summary>
        /// 导出单个服务点的销售数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="addressName"></param>
        /// <returns></returns>
        public FileResult ExportSingleServicePoint(string startDate, string endDate, string addressName)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                sd = new DateTime(sd.Year, sd.Month, 1);// 计算获取该月第一天的日期
                ed = new DateTime(ed.Year, ed.Month + 1, 1).AddDays(-1);// 计算获取该月最后一天的日期
                int addressId = client.get_wx_service_points_ByName(addressName).Id;
                List<sp_wx_getMonthlyProductInfoByAddressId_Result> result = client.GetWXMonthlyProductOrderInfo(sd, ed, addressId);
                DataTable dt = MakeDataTableForSingleServicePoint(result);
                NPOIHelper.Export(dt, addressName+"统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", addressName + "统计表.xls");
        }


        /// <summary>
        /// 导出微信商城订单的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportOrderInfo(string startDate, string endDate,int compareDay)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getOrderInfo_Result> result = client.GetWXProductOrderInfo(sd, ed, compareDay).OrderBy(n => n.time).ToList();
                DataTable dt = MakeDataTableForWXOrder(result);
                NPOIHelper.Export(dt, "订单数据统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "订单数据统计表.xls");
        }


        /// <summary>
        /// 导出商品销售的数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportProductSaleInfo(string startDate, string endDate, int compareDay)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getProductInfo_Result> result = client.GetWXProductSaleInfo(sd, ed, compareDay).OrderByDescending(n=>n.totalCount).ToList();
                DataTable dt = MakeDataTableForWXProductSale(result);
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
        public FileResult ExportProductSaleCategoryInfo(string startDate, string endDate)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getProductCategoryInfo_Result> result = client.GetWXProductCategoryInfo(sd, ed);
                DataTable dt = MakeDataTableForWXProductSaleCategory(result);
                NPOIHelper.Export(dt, "商品销售分类占比统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", "商品销售分类占比统计表.xls");
        }

        /// <summary>
        /// 导出单个商品的销售数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public FileResult ExportSingleProductInfo(string startDate, string endDate,string name)
        {
            string filepath = Server.MapPath("~\\Temp") + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                DateTime sd = ConvertDate(startDate);
                DateTime ed = ConvertDate(endDate, DateTime.Now.ToShortDateString());
                List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> result = client.GetWXSingleProductSaleInfoByNameAndDate(name, sd, ed).OrderByDescending(t => t.time).ToList();
                DataTable dt = MakeDataTableForWXSingleProduct(result);
                NPOIHelper.Export(dt, name+"销售数据统计表", filepath);
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
            return File(filepath, "application/ms-excel", name+"销售数据统计表.xls");
        }




        private DataTable MakeDataTable(List<sp_wx_getRegionProductInfo_Result> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("服务点ID", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("服务点", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("总销售额", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("总订单数", typeof(String));

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            foreach (sp_wx_getRegionProductInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["服务点ID"] = obj.address_id;
                row["服务点"] = obj.service_name;
                row["总销售额"] = obj.totalPrice;
                row["总订单数"] = obj.totalCount;
                dt.Rows.Add(row);
            }
            return dt;
        }


        private DataTable MakeDataTableForServicePointRate(List<sp_wx_getRegionProductInfo_Result> sourceList)
        {
            int totalOrder = sourceList.Sum(s => s.totalCount) == null ? 0 : (int)sourceList.Sum(s => s.totalCount);
            DataTable dt = new DataTable();
            if (totalOrder == 0)
            {
                return dt;
            }
            DataColumn dc1 = new DataColumn("服务点ID", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("服务点", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("占比", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("总订单数", typeof(String));

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            foreach (sp_wx_getRegionProductInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["服务点ID"] = obj.address_id;
                row["服务点"] = obj.service_name;
                row["占比"] = string.Format("{0:F2}", (obj.totalCount * 1.0 / totalOrder));
                row["总订单数"] = obj.totalCount;
                dt.Rows.Add(row);
            }
            return dt;
        }


        private DataTable MakeDataTableForSingleServicePoint(List<sp_wx_getMonthlyProductInfoByAddressId_Result> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc3 = new DataColumn("销售额", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("订单数", typeof(String));
            DataColumn dc5 = new DataColumn("时间", Type.GetType("System.String"));

            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            foreach (sp_wx_getMonthlyProductInfoByAddressId_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["销售额"] = obj.totalPrice;
                row["订单数"] = obj.totalCount;
                row["时间"] = obj.newDate;
                dt.Rows.Add(row);
            }
            return dt;
        }


        private DataTable MakeDataTableForWXOrder(List<sp_wx_getOrderInfo_Result> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("时间", typeof(DateTime));
            DataColumn dc2 = new DataColumn("订单数", typeof(Int32));
            DataColumn dc3 = new DataColumn("销售额", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            foreach (sp_wx_getOrderInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["时间"] = obj.time;
                row["订单数"] = obj.orderCount;
                row["销售额"] = obj.sale;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable MakeDataTableForWXProductSale(List<sp_wx_getProductInfo_Result> sourceList)
        {
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("商品名称", typeof(string));
            DataColumn dc2 = new DataColumn("销售总数", typeof(Int32));
            DataColumn dc3 = new DataColumn("销售总额", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            foreach (sp_wx_getProductInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["商品名称"] = obj.name;
                row["销售总数"] = obj.totalCount;
                row["销售总额"] = obj.totalPrice;
                dt.Rows.Add(row);
            }
            return dt;
        }


        private DataTable MakeDataTableForWXProductSaleCategory(List<sp_wx_getProductCategoryInfo_Result> sourceList)
        {
            int totalCount = sourceList.Sum(n => n.totalCount) == null ? 0 : (int)sourceList.Sum(n => n.totalCount);
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("商品分类名称", typeof(string));
            DataColumn dc2 = new DataColumn("占比", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            foreach (sp_wx_getProductCategoryInfo_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["商品分类名称"] = obj.name;
                row["占比"] = obj.totalCount * 1.0 / totalCount;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable MakeDataTableForWXSingleProduct(List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> sourceList)
        {
            int tc = sourceList.Sum(n => n.totalCount) == null ? 0 : (int)(sourceList.Sum(n => n.totalCount));
            double tp = sourceList.Sum(n => n.totalPrice) == null ? 0 : (double)(sourceList.Sum(n => n.totalPrice));
            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn("时间", typeof(string));
            DataColumn dc2 = new DataColumn("销售数量", typeof(Int32));
            DataColumn dc3 = new DataColumn("销售额", typeof(decimal));
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            foreach (sp_wx_getSingleProductSaleInfoByNameAndDate_Result obj in sourceList)
            {
                DataRow row = dt.NewRow();
                row["时间"] = obj.time;
                row["销售数量"] = obj.totalCount;
                row["销售额"] = obj.totalPrice;
                dt.Rows.Add(row);
            }
            DataRow r = dt.NewRow();
            r["时间"] = "合计";
            r["销售数量"] = tc;
            r["销售额"] = tp;
            dt.Rows.Add(r);

            return dt;
        }

        #endregion


        

        /// <summary>
        /// 日期转换，失败则返回defaultDate
        /// </summary>
        /// <param name="date">需要转换的日期字符串</param>
        /// <returns></returns>
        private DateTime ConvertDate(string date, string defaultDate="1970/01/01") 
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
