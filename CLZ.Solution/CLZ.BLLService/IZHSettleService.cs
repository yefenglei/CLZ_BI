using CLZ.Common;
using CLZ.Model;
using CLZ.Model.common;
using CLZ.Model.ZHService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CLZ.BLLService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IZHSettleService”。
    [ServiceContract]
    public interface IZHSettleService
    {
        #region ZH_SETTLE_DEAL_MAIN
        [OperationContract]
        List<ZH_SETTLE_DEAL_MAIN> getAllZH_SETTLE_DEAL_MAIN();
        [OperationContract]
        int AddZH_SETTLE_DEAL_MAIN(ZH_SETTLE_DEAL_MAIN model, ref ValidationError error);
        [OperationContract]
        ZH_SETTLE_DEAL_MAIN getZH_SETTLE_DEAL_MAINById(Int64 ID);
        [OperationContract]
        int EditZH_SETTLE_DEAL_MAIN(ZH_SETTLE_DEAL_MAIN model, ref ValidationError error);
        [OperationContract]

        int DeleteZH_SETTLE_DEAL_MAIN(Int64 ID, ref ValidationError error);
        [OperationContract]
        List<ZH_SETTLE_DEAL_MAIN> GetZH_SETTLE_DEAL_MAINList(ref GridPager pager);

        [OperationContract]
        List<ZH_SETTLE_DEAL_MAIN> GetMainSettleDealInfo(DateTime startDate, DateTime endDate);

        #endregion


        #region ZH_DAILY_GOODS_DEAL
        [OperationContract]
        List<ZH_DAILY_GOODS_DEAL> getAllZH_DAILY_GOODS_DEAL();
        [OperationContract]
        int AddZH_DAILY_GOODS_DEAL(ZH_DAILY_GOODS_DEAL model, ref ValidationError error);
        [OperationContract]
        ZH_DAILY_GOODS_DEAL getZH_DAILY_GOODS_DEALById(String ID);
        [OperationContract]
        int EditZH_DAILY_GOODS_DEAL(ZH_DAILY_GOODS_DEAL model, ref ValidationError error);
        [OperationContract]

        int DeleteZH_DAILY_GOODS_DEAL(String ID, ref ValidationError error);
        [OperationContract]
        List<ZH_DAILY_GOODS_DEAL> GetZH_DAILY_GOODS_DEALList(ref GridPager pager);
        [OperationContract]
        List<sp_zhsettle_getGoodsDealInfo_Result> GetZH_DAILY_GOODS_DEALInfo(DateTime startDate, DateTime endDate, int limitRowNumber, string sort, string order,int tradeMode);
        [OperationContract]
        List<sp_zhsettle_getGoodsDealInfo_Result> GetZH_DAILY_GOODS_DEALPagingInfo(ref GridPager pager, DateTime startDate, DateTime endDate, int tradeMode);
        [OperationContract]
        List<sp_zhsettle_getGoodsDealCategoryInfo_Result> GetZH_GOODS_DEAL_CategoryInfo(DateTime startDate, DateTime endDate, int tradeMode);
        [OperationContract]
        List<ZH_DAILY_GOODS_DEAL> GetZH_Single_GOODS_DEALInfo(string name, DateTime startDate, DateTime endDate, int tradeMode);
        #endregion

        #region ZH_DIC_PRODUCT
        [OperationContract]
        List<ZH_DIC_PRODUCT> getAllZH_DIC_PRODUCT();
        [OperationContract]
        int AddZH_DIC_PRODUCT(ZH_DIC_PRODUCT model, ref ValidationError error);
        [OperationContract]
        ZH_DIC_PRODUCT getZH_DIC_PRODUCTById(String PRODUCT_SN);
        [OperationContract]
        int EditZH_DIC_PRODUCT(ZH_DIC_PRODUCT model, ref ValidationError error);
        [OperationContract]
        int DeleteZH_DIC_PRODUCT(String PRODUCT_SN, ref ValidationError error);
        [OperationContract]
        List<ZH_DIC_PRODUCT> GetZH_DIC_PRODUCTList(ref GridPager pager);
        [OperationContract]
        List<ZH_DIC_PRODUCT> GetZH_DIC_PRODUCTPagingInfo(ref GridPager pager, string searchText);
        [OperationContract]
        List<ZHProductModel> GetPRODUCTOrderByCountPagingInfo(ref GridPager pager, string searchText);
        #endregion
    }
}
