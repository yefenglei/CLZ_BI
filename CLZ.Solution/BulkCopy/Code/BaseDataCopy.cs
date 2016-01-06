/**
 *  Author:Ye Fenglei 
 *  CreateDate:2015/5/14  
 *  Description:将MySql表中的数据拷贝到MS Sqlserver中
 *  ModifyDate:2015/5/15
 *  Version:0.0.1
 * 
 * 
 **/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using System.Collections;

namespace BulkCopy
{
    public class BaseDataCopy
    {
        public ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string DestinationTableName { get; set; }
        public string DestinationConnection { get; set; }
        public string SourceConnection { get; set; }
        public string QueryString { get; set; }
        public string BeforeQuery { get; set; }
        public string AfterQuery { get; set; }
        public string IsUsing { get; set; }
        public virtual void Copy()
        {
        
        }

        public int BatchSize
        {
            get
            {
                int size = 800;
                try
                {
                    size=Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                return size;
            }
        } 
        /// <summary>
        /// 配置基本参数
        /// </summary>
        /// <param name="config"></param>
        public void Setting(DataCopyConfig config)
        {
            this.DestinationConnection = config.TargetConnectionString;
            this.DestinationTableName = config.TargetTableName;
            this.SourceConnection = config.SourceConnectionString;
            this.QueryString = config.SelectQuery;
            this.BeforeQuery = config.PrepareQuery;
            this.AfterQuery = config.AfterQuery;
            this.IsUsing = config.IsUsing;
        }

    }
}
