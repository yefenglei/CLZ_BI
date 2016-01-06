using CLZ.Common;
using CLZ.Core;
using CLZ.Model;
using CLZ.Model.CLZ;
using CLZ.Model.common;
using CLZ.Model.Sys;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace CLZ.BLLService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。

    public class CommonService : ICommonService
    {
        ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        #region 样例

        public List<SysSample> getAllSamples()
        {
            try
            {
                DBContainer db = new DBContainer();
                return db.SysSample.ToList();
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }

        }


        public int AddSample(SysSample model,ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.SysSample.Add(model);
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


        public SysSample getSysSampleById(int id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysSample obj = db.SysSample.Where(n => n.Id == id).SingleOrDefault();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public int Edit(SysSample model,ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.SysSample.Attach(model);
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

        public int Delete(int id,ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysSample obj = db.SysSample.Where(n => n.Id == id).SingleOrDefault();
                    db.SysSample.Remove(obj);
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
        public List<SysSample> GetList(ref GridPager pager)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<SysSample> queryData = null;
                    queryData = db.SysSample;
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    #region 排序
                    //if (pager.order == "desc")
                    //{
                    //    switch (pager.order)
                    //    {
                    //        case "Id":
                    //            queryData = queryData.OrderByDescending(c => c.Id);
                    //            break;
                    //        case "Name":
                    //            queryData = queryData.OrderByDescending(c => c.Name);
                    //            break;
                    //        default:
                    //            queryData = queryData.OrderByDescending(c => c.CreateTime);
                    //            break;
                    //    }
                    //}
                    //else
                    //{

                    //    switch (pager.order)
                    //    {
                    //        case "Id":
                    //            queryData = queryData.OrderBy(c => c.Id);
                    //            break;
                    //        case "Name":
                    //            queryData = queryData.OrderBy(c => c.Name);
                    //            break;
                    //        default:
                    //            queryData = queryData.OrderBy(c => c.CreateTime);
                    //            break;
                    //    }
                    //}

                    //if (total > 0)
                    //{
                    //    if (pager.page <= 1)
                    //    {
                    //        queryData = queryData.Take(pager.rows);
                    //    }
                    //    else
                    //    {
                    //        queryData = queryData.Skip((pager.page - 1) * pager.rows).Take(pager.rows);
                    //    }
                    //}
                    #endregion
                    return queryData.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }
        #endregion


        #region 模块菜单

        /// <summary>
        /// 根据父级菜单id获取其子菜单
        /// </summary>
        /// <param name="moduleId">父级菜单id</param>
        /// <returns></returns>
        public List<SysModule> GetMenuByModuleIdAndUserId(string moduleId, string userId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<SysModule> menus =
                    (
                        from m in db.SysModule
                        join srt in db.SysRight on m.Id equals srt.ModuleId
                        join r in
                            (
                                from r in db.SysRole
                                from u in r.SysUser
                                where u.Id == userId
                                select r
                            )
                        on srt.RoleId equals r.Id
                        where m.ParentId == moduleId
                        where srt.Rightflag == true
                        where m.Id != "0"
                        select m
                              ).Distinct().OrderBy(a => a.Sort).ToList();
                    return menus;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
            
        }

        #endregion

        #region 模块
        /// <summary>
        /// 获取所有模块信息
        /// </summary>
        /// <returns></returns>
        public List<SysModule> GetModuleList()
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    List<SysModule> list = db.SysModule.ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 根据父模块id获取子模块
        /// </summary>
        /// <param name="parentId">父模块id</param>
        /// <returns></returns>
        public List<SysModule> GetModuleByParentId(string parentId)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    List<SysModule> list = db.SysModule.Where(n=>n.ParentId==parentId).OrderBy(n=>n.Sort).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 添加新模块
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int CreateModule(SysModule entity, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    if (IsModuleExist(entity.Id))
                    {
                        throw new Exception(Suggestion.PrimaryRepeat);
                    }
                    db.SysModule.Add(entity);
                    db.Entry(entity).State = EntityState.Added;
                    if (db.SaveChanges() == 1)
                    {
                        //将新增模块分配给各个角色
                        db.P_Sys_InsertSysRight();
                        return 1;
                    }
                    else 
                    {
                        return 0;
                    }
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
        /// 根据模块id删除模块
        /// </summary>
        /// <param name="id">模块id</param>
        /// <returns></returns>
        public int DeleteModule(string id, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    //判断是否有子模块
                    if (db.SysModule.Where(n => n.SysModule2.Id == id).Count() > 0)
                    {
                        throw new Exception("删除模块失败,被删除模块下面存在子模块,请先删除子模块.");
                    }

                    SysModule entity = db.SysModule.Where(n=>n.Id==id).SingleOrDefault();
                    db.SysModule.Remove(entity);
                    int result=db.SaveChanges();
                    if(1==result)
                    {
                        //清理被删除模块残留的数据
                        db.P_Sys_ClearUnusedRightOperate();
                    }
                    return result;
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
        /// 编辑模块
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int EditModule(SysModule entity, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.SysModule.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
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
        /// 根据模块id获取模块
        /// </summary>
        /// <param name="id">模块id</param>
        /// <returns></returns>
        public SysModule GetModuleById(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysModule entity = db.SysModule.Where(n => n.Id == id).SingleOrDefault();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 判断模块是否存在
        /// </summary>
        /// <param name="id">模块id</param>
        /// <returns></returns>
        public bool IsModuleExist(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysModule entity = db.SysModule.Where(n => n.Id == id).SingleOrDefault();
                    if (entity != null)
                    {
                        return true;
                    }
                    else 
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return false;
            }
        }

        #endregion

        #region SysLog 系统日志
        /// <summary>
        /// 获取系统日志分页数据
        /// </summary>
        /// <param name="pager">分页参数</param>
        /// <param name="queryStr">搜索参数</param>
        /// <returns></returns>
        public List<SysLog> GetSysLogList(ref GridPager pager, string queryStr)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<SysLog> query = null;
                    IQueryable<SysLog> list = db.SysLog;
                    if (!string.IsNullOrWhiteSpace(queryStr))
                    {
                        list = list.Where(a => a.Message.Contains(queryStr) || a.Module.Contains(queryStr));
                        pager.totalRows = list.Count();
                    }
                    else
                    {
                        pager.totalRows = list.Count();
                    }

                    if (pager.order == "desc")
                    {
                        query = list.OrderByDescending(c => c.CreateTime).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                    }
                    else
                    {
                        query = list.OrderBy(c => c.CreateTime).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                    }


                    return query;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
            
        }
        /// <summary>
        /// 根据Id获取SysLog的信息
        /// </summary>
        /// <param name="id">系统日志id</param>
        /// <returns></returns>
        public SysLog GetSysLogById(string id)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    SysLog entity = db.SysLog.Where(n=>n.Id==id).SingleOrDefault();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public int AddSysLog(SysLog model)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    db.SysLog.Add(model);
                    db.Entry(model).State = EntityState.Added;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }

        #endregion

        #region SysException 系统异常
        /// <summary>
        /// 获取系统异常分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public List<SysException> GetSysExceptionList(ref GridPager pager, string queryStr)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    List<SysException> list =null;
                    IQueryable<SysException> query = db.SysException;
                    if(!string.IsNullOrEmpty(queryStr))
                    {
                        query = query.Where(n => n.Message.Contains(queryStr));
                    }
                    pager.totalRows = query.Count();
                    if(pager.order=="desc")
                    {
                        list = query.OrderByDescending(n => n.CreateTime).Skip((pager.page - 1) * pager.rows).Take(pager.rows).ToList();
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }
        
        /// <summary>
        /// 根据id获取系统异常数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SysException GetSysExceptionById(string id)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    SysException ex = db.SysException.Where(n=>n.Id==id).SingleOrDefault();
                    return ex;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }
        #endregion

        #region 登录
        public SysUser Login(string username, string password)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysUser user = db.SysUser.SingleOrDefault(a => a.UserName == username && a.Password == password);
                    return user;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        #endregion

        #region 权限控制
        public List<permModel> GetPermission(string userId, string controller)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    List<permModel> permList = (
                        from r in db.P_Sys_GetRightOperate(userId, controller)
                        select new permModel {
                            KeyCode=r.KeyCode,
                            IsValid=r.IsValid
                        }).ToList();
                    return permList;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        #endregion


        #region wx_service_points

        public wx_service_points get_wx_service_points_ByName(string serviceName) 
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    return db.wx_service_points.Where(s=>s.service_name==serviceName).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        #endregion

        #region 订单数据
        public List<wx_product_cart> GetWXProductCartList()
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    List<wx_product_cart> list = db.wx_product_cart.ToList();
                    Conversive.PHPSerializationLibrary.Serializer serializer = new Conversive.PHPSerializationLibrary.Serializer();
                    Hashtable data = serializer.Deserialize(list[0].info) as Hashtable;
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取指定时间段内的订单信息
        /// </summary>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="compareDay">比较日期跨度，例如1表示与昨天的数据比较</param>
        /// <returns></returns>
        public List<sp_wx_getOrderInfo_Result> GetWXProductOrderInfo(DateTime startDate,DateTime endDate,int compareDay)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getOrderInfo_Result> list = db.sp_wx_getOrderInfo(startDate, endDate, compareDay).OrderByDescending(n => n.sale).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取指定时间段内某服务点的订单信息
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public List<ProductOrder> GetWXProductOrderInfoByAddressId(DateTime startDate, DateTime endDate, string addressName) 
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<wx_product_cart> product_cart = db.wx_product_cart.Where(n => n.time >= startDate && n.time < endDate);
                    IQueryable<wx_service_points> service_points = db.wx_service_points.Where(s=>s.service_name==addressName);

                    var queryData = from l in product_cart
                                    join s in service_points on
                                    l.address_id equals s.Id
                                    select new
                                    {
                                        address_id = s.Id,
                                        address_name = s.service_name,
                                        l.time,
                                        l.price
                                    };
                    var result = from q in queryData
                           group q by q.time into g
                           select new ProductOrder
                           {
                               address_name = addressName,
                               time=g.Key,
                               count=g.Count(),
                               price=g.Sum(p=>p.price)
                           };
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }

        public List<sp_wx_getOrderInfo_Result> GetWXProductOrderTableInfo(ref GridPager pager, DateTime startDate, DateTime endDate, int compareDay)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<sp_wx_getOrderInfo_Result> queryData = null;
                    IQueryable<sp_wx_getOrderInfo_Result> totalData = null;
                    queryData = db.sp_wx_getOrderInfo(startDate, endDate, compareDay).AsQueryable();
                    totalData = db.sp_wx_getOrderInfo(startDate, endDate, compareDay).AsQueryable();
                    int total = totalData.Count();
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
        /// 获取指定时间段内的用户数据
        /// </summary>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="compareDay">比较日期跨度，例如1表示与昨天的数据比较</param>
        /// <returns></returns>
        public List<sp_wx_getCustomerInfo_Result> GetWXCustomerInfo(DateTime startDate, DateTime endDate, int compareDay)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getCustomerInfo_Result> list = db.sp_wx_getCustomerInfo(startDate, endDate, compareDay).OrderByDescending(n => n.totalPrice).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取指定时间段内二次以上购买的数据
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<PurchaseRate> GetWXCustomerRateOfPurchaseInfo(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<PurchaseRate> listPurchaseRate = new List<PurchaseRate>();
                    var query = db.wx_customer_order.Where(t=>t.time>=startDate && t.time<endDate);
                    
                    var buyTimesGroup = from q in query
                               group q by new { q.wechat_id,q.truename }
                                   into g
                                   select new
                                   {
                                       g.Key,
                                       buyTimes = g.Sum(n=>n.totalOrder)
                                   };
                    int oneBuyTime = buyTimesGroup.Count();
                    // 获取所有用户中最多的购买次数
                    int maxBuyTimes = buyTimesGroup.Max(n=>n.buyTimes);
                    for (int i = 1; i <= maxBuyTimes;i++ )
                    {
                        var buySomeTimes=buyTimesGroup.Where(n=>n.buyTimes==i).Count();
                        listPurchaseRate.Add(new PurchaseRate() { buyTimes = i, rate =Convert.ToDecimal(buySomeTimes * 1.0 / oneBuyTime) });

                    }

                    return listPurchaseRate;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取对应购买次数的用户数据
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="buyTimes">购买次数</param>
        /// <returns></returns>
        public List<CustomerOrder> GetWXCustomerInfoByBuyTimes(DateTime startDate, DateTime endDate, int buyTimes)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<PurchaseRate> listPurchaseRate = new List<PurchaseRate>();
                    var query = db.wx_customer_order.Where(t => t.time >= startDate && t.time < endDate);

                    var buyTimesGroup = from q in query
                                        group q by new { q.wechat_id,q.truename }
                                            into g
                                            where g.Sum(n=>n.totalOrder) == buyTimes
                                            select new
                                            {
                                                g.Key,
                                                buyTimes = g.Sum(n => n.totalOrder)
                                            };
                    var data = from q in query
                               join b in buyTimesGroup
                               on new { q.wechat_id,q.truename } equals new { b.Key.wechat_id ,b.Key.truename}
                               select new
                               {
                                   totalCount = q.totalCount,
                                   totalOrder = q.totalOrder,
                                   totalPrice = q.totalPrice,
                                   truename = q.truename,
                                   wechat_id = q.wechat_id
                               };
                    var result = from q in data
                                 group q by new { q.wechat_id, q.truename }
                                     into g
                                     select new CustomerOrder
                                     {
                                         wechat_id = g.Key.wechat_id,
                                         totalCount = g.Sum(n => n.totalCount),
                                         totalPrice = g.Sum(n => n.totalPrice),
                                         totalOrder = g.Sum(n => n.totalOrder),
                                         truename = g.Key.truename
                                     };
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }


        /// <summary>
        /// 获取单个用户的订单数据
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<wx_customer_order> GetWXSingleCustomerInfo(string customerName, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<wx_customer_order> list = db.wx_customer_order.Where(n => n.truename == customerName && n.time >= startDate && n.time <= endDate).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取指定时间段内的商品销售数据
        /// </summary>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="compareDay">比较日期跨度，例如1表示与昨天的数据比较</param>
        /// <returns></returns>
        public List<sp_wx_getProductInfo_Result> GetWXProductSaleInfo(DateTime startDate, DateTime endDate, int compareDay)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getProductInfo_Result> list = db.sp_wx_getProductInfo(startDate, endDate, compareDay).OrderByDescending(n => n.totalPrice).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取指定时间段内的商品销售数据
        /// </summary>
        /// <param name="pager">分页参数</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="compareDay">比较日期</param>
        /// <returns></returns>
        public List<sp_wx_getProductInfo_Result> GetWXPagingProductSaleInfo(ref GridPager pager,DateTime startDate, DateTime endDate, int compareDay)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<sp_wx_getProductInfo_Result> queryData = null;
                    IQueryable<sp_wx_getProductInfo_Result> totalData = null;
                    queryData = db.sp_wx_getProductInfo(startDate, endDate, compareDay).AsQueryable();
                    totalData = db.sp_wx_getProductInfo(startDate, endDate, compareDay).AsQueryable();
                    int total = totalData.Count();
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
        /// 获取商品分类的销售数据
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<sp_wx_getProductCategoryInfo_Result> GetWXProductCategoryInfo(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getProductCategoryInfo_Result> queryData = null;
                    queryData = db.sp_wx_getProductCategoryInfo(startDate, endDate).ToList();
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
        /// 获取某商品制定时间内的销售情况
        /// </summary>
        /// <param name="name">商品名称</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> GetWXSingleProductSaleInfoByNameAndDate(string name, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> list = db.sp_wx_getSingleProductSaleInfoByNameAndDate(startDate, endDate, name).OrderByDescending(n => n.time).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }


        /// <summary>
        /// 获取指定时间范围内的各个服务点的总订单数和总销售额
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<sp_wx_getRegionProductInfo_Result> GetWXRegionProductInfo(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getRegionProductInfo_Result> list = db.sp_wx_getRegionProductInfo(startDate, endDate).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取指定时间范围内，指定服务点的每月订单数和销售额
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="addressId">服务点id</param>
        /// <returns></returns>
        public List<sp_wx_getMonthlyProductInfoByAddressId_Result> GetWXMonthlyProductOrderInfo(DateTime startDate, DateTime endDate, int addressId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    List<sp_wx_getMonthlyProductInfoByAddressId_Result> list = db.sp_wx_getMonthlyProductInfoByAddressId(startDate, endDate, addressId).ToList();
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


        #region 模块操作

        public List<SysModuleOperate> GetModuleOperateList(ref GridPager pager, string moduleId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<SysModuleOperate> queryData = null;
                    queryData = db.SysModuleOperate;
                    if (!string.IsNullOrEmpty(moduleId))
                    {
                        queryData = queryData.Where(n => n.ModuleId == moduleId);
                    }
                    else 
                    {
                        return null;
                    }
                    int total = queryData.Count();
                    pager.totalRows = total;
                    queryData = LinqHelper.SortingAndPaging(queryData, pager.sort, pager.order, pager.page, pager.rows);
                    return queryData.ToList() ;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 新增模块操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int CreateModuleOperate(SysModuleOperate model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    if (IsModuleOperateExist(model.Id))
                    {
                        throw new Exception(Suggestion.PrimaryRepeat);
                    }
                    db.SysModuleOperate.Add(model);
                    db.Entry(model).State = EntityState.Added;
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
        /// 根据id删除模块操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteModuleOperateById(string id, ref ValidationError error)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    SysModuleOperate model = db.SysModuleOperate.Where(n => n.Id == id).SingleOrDefault();
                    db.SysModuleOperate.Remove(model);
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
        /// 根据id获取模块操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SysModuleOperate GetModuleOperateById(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysModuleOperate model = db.SysModuleOperate.Where(n => n.Id == id).SingleOrDefault();
                    return model;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// 判断模块操作是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsModuleOperateExist(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    if (db.SysModuleOperate.Where(n => n.Id == id).SingleOrDefault() != null)
                    {
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return false;
            }

        }

        #endregion

        #region 角色管理

        public List<SysRole> GetSysRoleList(ref GridPager pager, string queryStr)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<SysRole> queryData = null;
                    queryData = db.SysRole;
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        queryData = queryData.Where(n => n.Name.Contains(queryStr));
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

        public int CreateSysRole(SysRole model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysRole entity = db.SysRole.Where(n=>n.Id==model.Id).SingleOrDefault();
                    if (entity != null)
                    {
                        throw new Exception(Suggestion.PrimaryRepeat);
                    }
                    entity = new SysRole();
                    entity.Id = model.Id;
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.CreateTime = model.CreateTime;
                    entity.CreatePerson = model.CreatePerson;
                    db.SysRole.Add(entity);
                    if (db.SaveChanges() == 1)
                    {
                        //分配给角色
                        db.P_Sys_InsertSysRight();
                        //清理无用的项
                        db.P_Sys_ClearUnusedRightOperate();
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            catch (Exception ex)
            {
                error.ErrorMessage = ex.Message;
                ExceptionHelper.WriteException(ex);
                return 0;
            }
        }

        public int DeleteSysRole(string id, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysRole model = db.SysRole.Where(r=>r.Id==id).SingleOrDefault();
                    if(model!=null)
                    {
                        //删除用户角色关系
                        db.P_Sys_DeleteSysRoleSysUserByRoleId(model.Id);
                        //删除角色下面所有的权限
                        db.P_Sys_DeleteRightByRoleId(model.Id);
                    }
                    db.SysRole.Remove(model);
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

        public int EditSysRole(SysRole model, ref ValidationError error)
        {
            try
            {
                using(DBContainer db=new DBContainer())
                {
                    SysRole entity = db.SysRole.Where(r=>r.Id==model.Id).SingleOrDefault();
                    if (entity == null)
                    {
                        throw new Exception(Suggestion.Disable);
                    }
                    entity.Id = model.Id;
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.CreateTime = model.CreateTime;
                    entity.CreatePerson = model.CreatePerson;
                    db.SysRole.Attach(entity);
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

        public SysRole GetSysRoleById(string id)
        {
            try
            {
                if (IsSysRoleExist(id))
                {
                    using (DBContainer db = new DBContainer())
                    {
                        SysRole entity = db.SysRole.Where(r=>r.Id==id).SingleOrDefault() ;
                        return entity;
                    }
                
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }
            

        }

        public bool IsSysRoleExist(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysRole entity = db.SysRole.Where(r => r.Id == id).SingleOrDefault();
                    if (entity != null)
                    {
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return false;
            }

        }

        #endregion


        #region SysRightOperate

        public int UpdateRight(SysRightOperate model)
        {
            //转换
            SysRightOperate rightOperate = new SysRightOperate();
            rightOperate.Id = model.Id;
            rightOperate.RightId = model.RightId;
            rightOperate.KeyCode = model.KeyCode;
            rightOperate.IsValid = model.IsValid;
            //判断rightOperate是否存在，如果存在就更新rightOperate,否则就添加一条
            using (DBContainer db = new DBContainer())
            {
                SysRightOperate right = db.SysRightOperate.Where(a => a.Id == rightOperate.Id).FirstOrDefault();
                if (right != null)
                {
                    right.IsValid = rightOperate.IsValid;
                }
                else
                {
                    db.SysRightOperate.Add(rightOperate);
                }
                if (db.SaveChanges() > 0)
                {
                    //更新角色--模块的有效标志RightFlag
                    var sysRight = (from r in db.SysRight
                                    where r.Id == rightOperate.RightId
                                    select r).First();
                    db.P_Sys_UpdateSysRightRightFlag(sysRight.ModuleId, sysRight.RoleId);
                    return 1;
                }
            }
            return 0;

        }

        public List<P_Sys_GetRightByRoleAndModule_Result> GetRightByRoleAndModule(string roleId, string moduleId)
        {
            List<P_Sys_GetRightByRoleAndModule_Result> result = null;
            using (DBContainer db = new DBContainer())
            {
                result = db.P_Sys_GetRightByRoleAndModule(roleId, moduleId).ToList();
            }
            return result;

        }

        #endregion

        #region 用户管理

        public List<SysUser> GetSysUserList(ref GridPager pager, string queryStr)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<SysUser> queryData = null;
                    queryData = db.SysUser;
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        queryData = queryData.Where(n => n.TrueName.Contains(queryStr));
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

        public int CreateSysUser(SysUser model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysUser entity = db.SysUser.Where(n => n.Id == model.Id).SingleOrDefault();
                    if (entity != null)
                    {
                        throw new Exception(Suggestion.PrimaryRepeat);
                    }
                    db.SysUser.Add(model);
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

        public int DeleteSysUser(string id, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysUser model = db.SysUser.Where(r => r.Id == id).SingleOrDefault();
                    db.SysUser.Remove(model);
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

        public int EditSysUser(SysUser model, ref ValidationError error)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysUser entity = db.SysUser.Where(r => r.Id == model.Id).SingleOrDefault();
                    if (entity == null)
                    {
                        throw new Exception(Suggestion.Disable);
                    }
                    db.SysUser.Attach(entity);
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

        public SysUser GetSysUserById(string id)
        {
            try
            {
                if (IsSysUserExist(id))
                {
                    using (DBContainer db = new DBContainer())
                    {
                        SysUser entity = db.SysUser.Where(r => r.Id == id).SingleOrDefault();
                        return entity;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public bool IsSysUserExist(string id)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    SysUser entity = db.SysUser.Where(r => r.Id == id).SingleOrDefault();
                    if (entity != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return false;
            }

        }

        public List<P_Sys_GetRoleByUserId_Result> GetRoleByUserId(ref GridPager pager, string userId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<P_Sys_GetRoleByUserId_Result> queryData = db.P_Sys_GetRoleByUserId(userId).AsQueryable();
                    IQueryable<P_Sys_GetRoleByUserId_Result> totalData = db.P_Sys_GetRoleByUserId(userId).AsQueryable();
                    int total = totalData.Count();
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

        public int UpdateSysRoleSysUser(string userId, string[] roleIds)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.P_Sys_DeleteSysRoleSysUserByUserId(userId);
                    foreach (string roleid in roleIds)
                    {
                        if (!string.IsNullOrWhiteSpace(roleid))
                        {
                            db.P_Sys_UpdateSysRoleSysUser(roleid, userId);
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }

        public List<SysUser> GetRefSysUserByRoleId(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    using (DBContainer db = new DBContainer())
                    {
                        var data= from m in db.SysRole
                               from f in m.SysUser
                               where m.Id == id
                               select f;
                        return data.ToList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return null;
            }

        }

        public List<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(ref GridPager pager, string roleId)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<P_Sys_GetUserByRoleId_Result> queryData = db.P_Sys_GetUserByRoleId(roleId).AsQueryable();
                    IQueryable<P_Sys_GetUserByRoleId_Result> totalData = db.P_Sys_GetUserByRoleId(roleId).AsQueryable();
                    int total = totalData.Count();
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

        public int UpdateSysRoleSysUserByRoleId(string roleId, string[] userIds)
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    db.P_Sys_DeleteSysRoleSysUserByRoleId(roleId);
                    foreach (string userid in userIds)
                    {
                        if (!string.IsNullOrWhiteSpace(userid))
                        {
                            db.P_Sys_UpdateSysRoleSysUser(roleId, userid);
                        }
                    }
                    db.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                return 0;
            }

        }

        #endregion



        public string test(int num){
            string result = string.Empty;
            if(num==0)
            {
                result = "白日依山尽";
            }
            else if (1 == num)
            {
                result = "黄河入海流";
            }
            else {
                result = "jojo的奇妙冒险";
            }
            return result;
        }










    }
}
