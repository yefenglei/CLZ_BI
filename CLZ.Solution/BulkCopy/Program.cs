using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BulkCopy
{
    public class Program
    {
        static void Main(string[] args)
        {

            //初始化log4net配置
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            try
            {
                logger.Info(string.Format("######开始数据库数据拷贝######"));
                Console.WriteLine(string.Format("######开始数据库数据拷贝######"));

                //从mysql拷贝数据到mssqlserver
                string confDirPath = ConfigurationManager.AppSettings["MySqlPath"];
                if (!string.IsNullOrEmpty(confDirPath) && Directory.Exists(confDirPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(confDirPath);
                    foreach (FileInfo fileInfo in dirInfo.GetFiles("*.xml"))
                    {
                        DataCopyConfig config = DataCopyConfig.ReadCopyConfigFromXML(fileInfo.FullName);
                        BaseDataCopy mysqlCopy = new MySqlDataCopy();
                        mysqlCopy.Setting(config);
                        if (config.IsUsing.Trim().ToLower()=="true")
                        {
                            logger.Info(string.Format("######开始执行文件{0}######", fileInfo.Name));
                            Console.WriteLine(string.Format("######开始执行文件{0}######", fileInfo.Name));
                            //从mysql数据库拷贝数据
                            mysqlCopy.Copy();
                        }
                    }

                }
                else
                {
                    logger.Error("无效的配置文件路径" + confDirPath);
                    Console.WriteLine("无效的配置文件路径" + confDirPath);
                    return;
                }
                //从mssqlserver拷贝数据到mssqlserver
                string MSSQLServerPath = ConfigurationManager.AppSettings["MSSQLServerPath"];
                if (!string.IsNullOrEmpty(MSSQLServerPath) && Directory.Exists(MSSQLServerPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(MSSQLServerPath);
                    foreach (FileInfo fileInfo in dirInfo.GetFiles("*.xml"))
                    {
                        DataCopyConfig config = DataCopyConfig.ReadCopyConfigFromXML(fileInfo.FullName);
                        BaseDataCopy mssqlCopy = new MSSQLServerDataCopy();
                        mssqlCopy.Setting(config);
                        if (config.IsUsing.Trim().ToLower() == "true")
                        {
                            logger.Info(string.Format("######开始执行文件{0}######", fileInfo.Name));
                            Console.WriteLine(string.Format("######开始执行文件{0}######", fileInfo.Name));
                            //从mssqlserver数据库拷贝数据
                            mssqlCopy.Copy();
                        }
                    }
                }
                else
                {
                    logger.Error("无效的配置文件路径:" + MSSQLServerPath);
                    Console.WriteLine("无效的配置文件路径:" + MSSQLServerPath);
                    return;
                }

                // 计算每日均价
                string isOpenCopy = ConfigurationManager.AppSettings["specialCopy"].ToString().Trim();
                if(isOpenCopy=="true")
                {
                    ZH_DAILY_GOODS_DEAL(logger);
                }

                
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally 
            {
                logger.Info(string.Format("######数据库数据拷贝结束######"));
                Console.WriteLine(string.Format("######数据库数据拷贝结束######"));
            }
        }


        public static string StringArrayToString(string[] arr)
        {
            StringBuilder sb=new StringBuilder();
            foreach(string s in arr)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }


        public static void ZH_DAILY_GOODS_DEAL(ILog logger) 
        {
            DataCopyConfig config1 = DataCopyConfig.ReadCopyConfigFromXML(@"D:\workspace\CLZ_MIS\CLZ.Solution\BulkCopy\Config\MSSQLServer\HISTORY_SETTLE_DEAL_DETAIL_Copy.xml");
            DataCopyConfig config2 = DataCopyConfig.ReadCopyConfigFromXML(@"D:\workspace\CLZ_MIS\CLZ.Solution\BulkCopy\Config\MSSQLServer\HISTORY_SETTLE_DEAL_MAIN_Copy.xml");
            MSSQLServerDataCopy mssqlCopy1 = new MSSQLServerDataCopy();
            MSSQLServerDataCopy mssqlCopy2 = new MSSQLServerDataCopy();
            string startDateStr = ConfigurationManager.AppSettings["StartDate"];
            string endDateStr = ConfigurationManager.AppSettings["EndDate"];
            DateTime startDate = Convert.ToDateTime(startDateStr);
            DateTime endDate = Convert.ToDateTime(endDateStr);

            mssqlCopy1.Setting(config1);
            mssqlCopy2.Setting(config2);

            while(startDate>=endDate)
            {
                Console.WriteLine(string.Format("######开始拷贝 {0} 的数据######", startDate));
                logger.Info(string.Format("######开始拷贝 {0} 的数据######", startDate));
                mssqlCopy1.HISTORY_SETTLE_DEAL_DETAIL_Copy(startDate);
                mssqlCopy2.HISTORY_SETTLE_DEAL_MAIN_Copy(startDate);
                startDate=startDate.AddDays(-1);
            }


        }
    }
}
