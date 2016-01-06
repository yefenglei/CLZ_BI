using CLZ.Common;
using CLZ.Core;
using CLZ.Model;
using CLZ.Model.common;
using CLZ.Model.ZHService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CLZ.BLLService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“ZHSettleService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 ZHSettleService.svc 或 ZHSettleService.svc.cs，然后开始调试。
    public class ZHSettleService : IZHSettleService
    {
        #region ZH_SETTLE_DEAL_MAIN

        public List<ZH_SETTLE_DEAL_MAIN> getAllZH_SETTLE_DEAL_MAIN()
        {
            try
            {
                DBContainer db = new DBContainer();
                return db.ZH_SETTLE_DEAL_MAIN.ToList();
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }


        public int AddZH_SETTLE_DEAL_MAIN(ZH_SETTLE_DEAL_MAIN model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_SETTLE_DEAL_MAIN.Add(model);
                    db.Entry(model).State = EntityState.Added;
                    return (db.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }

        }


        public ZH_SETTLE_DEAL_MAIN getZH_SETTLE_DEAL_MAINById(Int64 ID)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_SETTLE_DEAL_MAIN obj = db.ZH_SETTLE_DEAL_MAIN.Where(n => n.ID==ID).SingleOrDefault();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public int EditZH_SETTLE_DEAL_MAIN(ZH_SETTLE_DEAL_MAIN model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_SETTLE_DEAL_MAIN.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }


        }

        public int DeleteZH_SETTLE_DEAL_MAIN(Int64 ID, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_SETTLE_DEAL_MAIN obj = db.ZH_SETTLE_DEAL_MAIN.Where(n => n.ID == ID).SingleOrDefault();
                    db.ZH_SETTLE_DEAL_MAIN.Remove(obj);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                error.ErrorMessage = ex.Message;
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }




        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public List<ZH_SETTLE_DEAL_MAIN> GetZH_SETTLE_DEAL_MAINList(ref GridPager pager)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<ZH_SETTLE_DEAL_MAIN> queryData = null;
                    queryData = db.ZH_SETTLE_DEAL_MAIN;
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }



        public List<ZH_SETTLE_DEAL_MAIN> GetMainSettleDealInfo(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<ZH_SETTLE_DEAL_MAIN> list = db.ZH_SETTLE_DEAL_MAIN.Where(n => n.DEAL_TIME >= startDate && n.DEAL_TIME <= endDate).OrderBy(n=>n.DEAL_TIME).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        #endregion

        #region ZH_DAILY_GOODS_DEAL

        public List<ZH_DAILY_GOODS_DEAL> getAllZH_DAILY_GOODS_DEAL()
        {
            try
            {
                DBContainer db = new DBContainer();
                return db.ZH_DAILY_GOODS_DEAL.ToList();
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }


        public int AddZH_DAILY_GOODS_DEAL(ZH_DAILY_GOODS_DEAL model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_DAILY_GOODS_DEAL.Add(model);
                    db.Entry(model).State = EntityState.Added;
                    return (db.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }

        }


        public ZH_DAILY_GOODS_DEAL getZH_DAILY_GOODS_DEALById(String ID)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_DAILY_GOODS_DEAL obj = db.ZH_DAILY_GOODS_DEAL.Where(n => n.ID == new Guid(ID)).SingleOrDefault();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public int EditZH_DAILY_GOODS_DEAL(ZH_DAILY_GOODS_DEAL model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_DAILY_GOODS_DEAL.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }


        }

        public int DeleteZH_DAILY_GOODS_DEAL(String ID, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_DAILY_GOODS_DEAL obj = db.ZH_DAILY_GOODS_DEAL.Where(n => n.ID == new Guid(ID)).SingleOrDefault();
                    db.ZH_DAILY_GOODS_DEAL.Remove(obj);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                error.ErrorMessage = ex.Message;
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }




        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public List<ZH_DAILY_GOODS_DEAL> GetZH_DAILY_GOODS_DEALList(ref GridPager pager)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<ZH_DAILY_GOODS_DEAL> queryData = null;
                    queryData = db.ZH_DAILY_GOODS_DEAL;
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        /// <summary>
        /// 根据时间范围获取商品销售西信息
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="limitRowNumber">限制返回的记录条数</param>
        /// <param name="sort">排序的字段</param>
        /// <param name="order">排序的方向</param>
        /// <returns></returns>
        public List<sp_zhsettle_getGoodsDealInfo_Result> GetZH_DAILY_GOODS_DEALInfo(DateTime startDate, DateTime endDate, int limitRowNumber, string sort, string order, int tradeMode)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<sp_zhsettle_getGoodsDealInfo_Result> queryData = null;
                    queryData = db.sp_zhsettle_getGoodsDealInfo(startDate,endDate,tradeMode).AsQueryable();
                    if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
                    {
                        queryData = LinqHelper.DataSorting(queryData, sort, order);
                    }
                    if (limitRowNumber>0)
                    {
                        queryData = queryData.Take(limitRowNumber);
                    }
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取某时间段内的商品销售数据
        /// </summary>
        /// <param name="pager">分页数据</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<sp_zhsettle_getGoodsDealInfo_Result> GetZH_DAILY_GOODS_DEALPagingInfo(ref GridPager pager, DateTime startDate, DateTime endDate, int tradeMode)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<sp_zhsettle_getGoodsDealInfo_Result> queryData = db.sp_zhsettle_getGoodsDealInfo(startDate, endDate, tradeMode).AsQueryable();
                    IQueryable<sp_zhsettle_getGoodsDealInfo_Result> totalData = db.sp_zhsettle_getGoodsDealInfo(startDate, endDate, tradeMode).AsQueryable();
                    pager.totalRows = totalData.Count();
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }


        /// <summary>
        /// 获取商品分类的销售数据
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<sp_zhsettle_getGoodsDealCategoryInfo_Result> GetZH_GOODS_DEAL_CategoryInfo(DateTime startDate, DateTime endDate, int tradeMode)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_zhsettle_getGoodsDealCategoryInfo_Result> queryData = null;
                    queryData = db.sp_zhsettle_getGoodsDealCategoryInfo(startDate, endDate, tradeMode).ToList();
                    return queryData;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取单个商品的销售数据
        /// </summary>
        /// <param name="name">商品名称</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<ZH_DAILY_GOODS_DEAL> GetZH_Single_GOODS_DEALInfo(string name, DateTime startDate, DateTime endDate, int tradeMode)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<ZH_DAILY_GOODS_DEAL> queryData = null;
                    queryData = db.ZH_DAILY_GOODS_DEAL.Where(n=>n.PRODUCT_NAME==name&&n.DEAL_DATE>=startDate&&n.DEAL_DATE<=endDate &&n.TRADE_MODE==tradeMode).ToList();
                    return queryData;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        #endregion


        #region ZH_DIC_PRODUCT

        public List<ZH_DIC_PRODUCT> getAllZH_DIC_PRODUCT()
        {
            try
            {
                DBContainer db = new DBContainer();
                return db.ZH_DIC_PRODUCT.ToList();
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }


        public int AddZH_DIC_PRODUCT(ZH_DIC_PRODUCT model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_DIC_PRODUCT.Add(model);
                    db.Entry(model).State = EntityState.Added;
                    return (db.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }

        }


        public ZH_DIC_PRODUCT getZH_DIC_PRODUCTById(String PRODUCT_SN)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_DIC_PRODUCT obj = db.ZH_DIC_PRODUCT.Where(n => n.PRODUCT_SN == PRODUCT_SN).SingleOrDefault();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public int EditZH_DIC_PRODUCT(ZH_DIC_PRODUCT model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.ZH_DIC_PRODUCT.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                error.ErrorMessage = ex.Message;
                return 0;
            }


        }

        public int DeleteZH_DIC_PRODUCT(String PRODUCT_SN, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    ZH_DIC_PRODUCT obj = db.ZH_DIC_PRODUCT.Where(n => n.PRODUCT_SN == PRODUCT_SN).SingleOrDefault();
                    db.ZH_DIC_PRODUCT.Remove(obj);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                error.ErrorMessage = ex.Message;
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }




        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public List<ZH_DIC_PRODUCT> GetZH_DIC_PRODUCTList(ref GridPager pager)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<ZH_DIC_PRODUCT> queryData = null;
                    queryData = db.ZH_DIC_PRODUCT;
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public List<ZH_DIC_PRODUCT> GetZH_DIC_PRODUCTPagingInfo(ref GridPager pager, string searchText)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<ZH_DIC_PRODUCT> queryData = null;
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        queryData = db.ZH_DIC_PRODUCT.Where(p => p.PRODUCT_NAME.Contains(searchText));
                    }
                    else 
                    {
                        queryData = db.ZH_DIC_PRODUCT;
                    }
                    
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取按销量排序后的商品名称列表
        /// </summary>
        /// <param name="pager">分页参数</param>
        /// <param name="searchText">查询商品</param>
        /// <returns></returns>
        public List<ZHProductModel> GetPRODUCTOrderByCountPagingInfo(ref GridPager pager, string searchText)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<ZH_DAILY_GOODS_DEAL> queryData = db.ZH_DAILY_GOODS_DEAL;
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        queryData = queryData.Where(p => p.PRODUCT_NAME.Contains(searchText));
                    }
                    var newQueryData = from q in queryData
                                group q by new { q.PRODUCT_SN, q.PRODUCT_NAME }
                                    into g
                                    select new ZHProductModel
                                    {
                                        PRODUCT_SN=g.Key.PRODUCT_SN,
                                        PRODUCT_NAME=g.Key.PRODUCT_NAME,
                                        PRODUCT_TOTAL=g.Sum(n=>n.COUNT)
                                    };

                    int total = newQueryData.Count();
                    pager.totalRows = total;
                    newQueryData = LinqHelper.SortingAndPaging(newQueryData, pager.sort, pager.order, pager.page, pager.rows);
                    return newQueryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        #endregion

    }
}
