using CLZ.Common;
using CLZ.Model;
using CLZ.Model.CLZ;
using CLZ.Model.common;
using CLZ.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CLZ.BLLService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface ICommonService
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作

        #region Sample测试样例

        /// <summary>
        /// 获取所有Sample数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [ServiceKnownType(typeof(SysSample))]
        List<SysSample> getAllSamples();

        [OperationContract]
        [ServiceKnownType(typeof(SysSample))]
        int AddSample(SysSample model,ref ValidationError error);

        [OperationContract]
        SysSample getSysSampleById(int id);
        [OperationContract]
        int Edit(SysSample model,ref ValidationError error);
        [OperationContract]
        int Delete(int id, ref ValidationError error);
        [OperationContract]
        List<SysSample> GetList(ref GridPager pager);

        #endregion

        #region Module
        [OperationContract]
        [ApplyProxyDataContractResolver]
        List<SysModule> GetMenuByModuleIdAndUserId(string moduleId,string userId);

        [OperationContract]
        [ApplyProxyDataContractResolver]
        List<SysModule> GetModuleList();
        [OperationContract]
        [ApplyProxyDataContractResolver]
        List<SysModule> GetModuleByParentId(string parentId);
        [OperationContract]
        [ApplyProxyDataContractResolver]
        int CreateModule(SysModule entity, ref ValidationError error);
        [OperationContract]
        [ApplyProxyDataContractResolver]
        int DeleteModule(string id, ref ValidationError error);
        [OperationContract]
        [ApplyProxyDataContractResolver]
        int EditModule(SysModule entity, ref ValidationError error);
        [OperationContract]
        [ApplyProxyDataContractResolver]
        SysModule GetModuleById(string id);
        [OperationContract]
        [ApplyProxyDataContractResolver]
        bool IsModuleExist(string id);



        [OperationContract]
        List<SysModuleOperate> GetModuleOperateList(ref GridPager pager, string moduleId);
        [OperationContract]
        int CreateModuleOperate(SysModuleOperate model, ref ValidationError error);
        [OperationContract]
        int DeleteModuleOperateById(string id, ref ValidationError error);
        [OperationContract]
        SysModuleOperate GetModuleOperateById(string id);
        [OperationContract]
        bool IsModuleOperateExist(string id);


        #endregion

        #region 权限
        [OperationContract]
        List<permModel> GetPermission(string userId,string controller);

        #endregion

        #region SysRole
        [OperationContract]
        List<SysRole> GetSysRoleList(ref GridPager pager, string queryStr);
        [OperationContract]
        int CreateSysRole(SysRole model, ref ValidationError error);
        [OperationContract]
        int DeleteSysRole(string id, ref ValidationError error);
        [OperationContract]
        int EditSysRole(SysRole model, ref ValidationError error);
        [OperationContract]
        SysRole GetSysRoleById(string id);
        [OperationContract]
        bool IsSysRoleExist(string id);

        #endregion

        #region SysLog
        [OperationContract]
        List<SysLog> GetSysLogList(ref GridPager pager, string queryStr);
        [OperationContract]
        SysLog GetSysLogById(string id);
        [OperationContract]
        int AddSysLog(SysLog model);

        #endregion

        #region SysException
        [OperationContract]
        List<SysException> GetSysExceptionList(ref GridPager pager, string queryStr);
        [OperationContract]
        SysException GetSysExceptionById(string id);

        #endregion

        #region SysRight

        //更新
        [OperationContract]
        int UpdateRight(SysRightOperate model);
        //按选择的角色及模块加载模块的权限项
        [OperationContract]
        List<P_Sys_GetRightByRoleAndModule_Result> GetRightByRoleAndModule(string roleId, string moduleId);

        #endregion

        #region SysUser

        [OperationContract]
        List<SysUser> GetSysUserList(ref GridPager pager, string queryStr);
        [OperationContract]
        int CreateSysUser(SysUser model, ref ValidationError error);
        [OperationContract]
        int DeleteSysUser(string id, ref ValidationError error);
        [OperationContract]
        int EditSysUser(SysUser model, ref ValidationError error);
        [OperationContract]
        SysUser GetSysUserById(string id);
        [OperationContract]
        bool IsSysUserExist(string id);
        [OperationContract]
        List<P_Sys_GetRoleByUserId_Result> GetRoleByUserId(ref GridPager pager, string userId);
        [OperationContract]
        int UpdateSysRoleSysUser(string userId, string[] roleIds);
        [OperationContract]
        List<SysUser> GetRefSysUserByRoleId(string id);
        [OperationContract]
        List<P_Sys_GetUserByRoleId_Result> GetUserByRoleId(ref GridPager pager, string roleId);
        [OperationContract]
        int UpdateSysRoleSysUserByRoleId(string roleId, string[] userIds);
        #endregion

        #region 登录

        [OperationContract]
        SysUser Login(string username,string password);
        #endregion

        #region wx_service_points

        [OperationContract]
        wx_service_points get_wx_service_points_ByName(string serviceName);

        #endregion

        #region 订单数据
        [OperationContract]
        List<wx_product_cart> GetWXProductCartList();

        [OperationContract]
        List<sp_wx_getOrderInfo_Result> GetWXProductOrderInfo(DateTime startDate, DateTime endDate, int compareDay);

        [OperationContract]
        List<ProductOrder> GetWXProductOrderInfoByAddressId(DateTime startDate, DateTime endDate, string addressName);

        [OperationContract]
        List<sp_wx_getOrderInfo_Result> GetWXProductOrderTableInfo(ref GridPager pager,DateTime startDate, DateTime endDate, int compareDay);

        [OperationContract]
        List<sp_wx_getCustomerInfo_Result> GetWXCustomerInfo(DateTime startDate, DateTime endDate, int compareDay);
        [OperationContract]
        List<wx_customer_order> GetWXSingleCustomerInfo(string customerName,DateTime startDate, DateTime endDate);
        [OperationContract]
        List<PurchaseRate> GetWXCustomerRateOfPurchaseInfo(DateTime startDate, DateTime endDate);
        [OperationContract]
        List<CustomerOrder> GetWXCustomerInfoByBuyTimes(DateTime startDate, DateTime endDate, int buyTimes);

        [OperationContract]
        List<sp_wx_getProductInfo_Result> GetWXProductSaleInfo(DateTime startDate, DateTime endDate, int compareDay);
        [OperationContract]
        List<sp_wx_getProductCategoryInfo_Result> GetWXProductCategoryInfo(DateTime startDate, DateTime endDate);


        [OperationContract]
        [ApplyProxyDataContractResolver]
        List<sp_wx_getProductInfo_Result> GetWXPagingProductSaleInfo(ref GridPager pager, DateTime startDate, DateTime endDate, int compareDay);

        [OperationContract]
        List<sp_wx_getSingleProductSaleInfoByNameAndDate_Result> GetWXSingleProductSaleInfoByNameAndDate(string name, DateTime startDate, DateTime endDate);
        #endregion



        [OperationContract]
        List<sp_wx_getRegionProductInfo_Result> GetWXRegionProductInfo(DateTime startDate, DateTime endDate);
        [OperationContract]
        List<sp_wx_getMonthlyProductInfoByAddressId_Result> GetWXMonthlyProductOrderInfo(DateTime startDate, DateTime endDate,int addressId);


        [OperationContract]
        string test(int num);

    }


    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
