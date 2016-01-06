using CLZ.CLZService;
using CLZ.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLZ.Core
{
    public static class LogHandler
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="oper">操作人</param>
        /// <param name="mes">操作信息</param>
        /// <param name="result">结果</param>
        /// <param name="type">类型</param>
        /// <param name="module">操作模块</param>
        public static void WriteServiceLog(string oper, string mes, string result, string type, string module)
        {
            SysLog entity = new SysLog();
            entity.Id = ResultHelper.NewId;
            entity.Operator = oper;
            entity.Message = mes;
            entity.Result = result;
            entity.Type = type;
            entity.Module = module;
            entity.CreateTime = ResultHelper.NowTime;
            try
            {
                CommonServiceClient client = new CommonServiceClient();
                int resultCode = client.AddSysLog(entity);
                if(1==resultCode)
                {
                    //讲日志写入数据库成功
                }
            }
            catch (Exception ex)
            {
                
                //throw ex;
            }
        }
    }
}