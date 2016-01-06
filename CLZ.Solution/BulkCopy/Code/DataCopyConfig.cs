using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BulkCopy
{
    public class DataCopyConfig
    {
        public string SourceConnectionString { get; set; }
        public string TargetConnectionString { get; set; }
        public string TargetTableName { get; set; }
        public string PrepareQuery { get; set; }
        public string SelectQuery { get; set; }
        public string AfterQuery { get; set; }
        public string IsUsing { get; set; }


        /// <summary>
        /// 从xml文件中读取配置信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataCopyConfig ReadCopyConfigFromXML(string filePath)
        {
            //初始化一个xml实例
            XmlDocument xml = new XmlDocument();
            //加载xml文件
            xml.Load(filePath);
            DataCopyConfig config = new DataCopyConfig();
            //指定一个节点
            XmlNode nodeSourceConnectionString = xml.SelectSingleNode("/root/SourceConnectionString");
            XmlNode nodeTargetConnectionString = xml.SelectSingleNode("/root/TargetConnectionString");
            XmlNode nodeTargetTableName = xml.SelectSingleNode("/root/TargetTableName");
            XmlNode nodePrepareQuery = xml.SelectSingleNode("/root/BeforeQuery");
            XmlNode nodeSelectQuery = xml.SelectSingleNode("/root/SelectQuery");
            XmlNode nodeAfterQuery = xml.SelectSingleNode("/root/AfterQuery");
            XmlNode nodeIsUsing = xml.SelectSingleNode("/root/IsUsing");
            config.SourceConnectionString = nodeSourceConnectionString.InnerText;
            config.TargetConnectionString = nodeTargetConnectionString.InnerText;
            config.TargetTableName = nodeTargetTableName.InnerText;
            config.PrepareQuery = nodePrepareQuery.InnerText;
            config.SelectQuery = nodeSelectQuery.InnerText;
            config.AfterQuery = nodeAfterQuery.InnerText;
            config.IsUsing = nodeIsUsing.InnerText;
            return config;
        }
    }
}
