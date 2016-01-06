/**
 *  Author:Ye Fenglei 
 *  CreateDate:2015/5/14  
 *  Description:将MySql表中的数据拷贝到MS Sqlserver中
 *  ModifyDate:2015/5/15
 *  Version:0.0.1
 * 
 * 
 **/
using MySql.Data.MySqlClient;
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
using Conversive.PHPSerializationLibrary;
using System.Collections;

namespace BulkCopy
{
    public class MySqlDataCopy : BaseDataCopy
    {
        public override void Copy() 
        {   
            //获取mysql对应表中的数据
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);
            DataTable dt = MysqlHelper.ExecuteDataTable(SourceConnection, QueryString, timeout);

            using (SqlConnection destinationConnection =
                       new SqlConnection(DestinationConnection))
            {
                destinationConnection.Open();
                //在拷贝执行之前
                if(!string.IsNullOrEmpty(BeforeQuery))
                {
                    SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, BeforeQuery);
                }
                using (SqlBulkCopy bulkCopy =
                           new SqlBulkCopy(DestinationConnection))
                {
                    bulkCopy.DestinationTableName =
                        DestinationTableName;

                    try
                    {
                        // Write from the source to the destination.
                        foreach (DataColumn colName in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(colName.ColumnName, colName.ColumnName);
                        }
                        bulkCopy.BatchSize = BatchSize;
                        bulkCopy.WriteToServer(dt);
                        //在拷贝执行之后
                        if (!string.IsNullOrEmpty(AfterQuery))
                        {
                            SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, AfterQuery);
                        }

                        if (DestinationTableName.ToLower().Equals("wx_product_cart"))
                        {
                            InsertProductsInfo();
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 将订单商品数据反序列化，并存到表中
        /// </summary>
        public void InsertProductsInfo() 
        {

            using (SqlConnection destinationConnection =
                       new SqlConnection(DestinationConnection))
            {
                destinationConnection.Open();
                //清空旧表数据
                string sql = "delete from wx_product_salegoods;";
                SqlHelper.ExecuteNonQuery(destinationConnection,CommandType.Text,sql);
                //下载新数据
                sql = "select orderid,info,time,address_id from dbo.wx_product_cart;";
                DataTable dt = SqlHelper.ExecuteDataset(destinationConnection,CommandType.Text,sql).Tables[0];
                Serializer serializer = new Serializer();
                List<SqlParameter> paramList = new List<SqlParameter>();
                foreach(DataRow dr in dt.Rows)
                {
                    Hashtable data = serializer.Deserialize(dr["info"].ToString()) as Hashtable;
                    int? address_id = (int?)dr["address_id"];
                    foreach (DictionaryEntry de in data)
                    {
                        sql = "insert into wx_product_salegoods([orderid],[sn],[price],[count],[address_id],[time]) values(@orderid,@sn,@price,@count,@address_id,@time)";
                        paramList.Clear();
                        paramList.Add(new SqlParameter("@orderid", dr["orderid"]));
                        paramList.Add(new SqlParameter("@time", dr["time"]));
                        Hashtable ht=(Hashtable)de.Value;
                        paramList.Add(new SqlParameter("@sn", ht["sn"]));
                        paramList.Add(new SqlParameter("@price", ht["price"]));
                        paramList.Add(new SqlParameter("@count", ht["count"]));
                        paramList.Add(new SqlParameter("@address_id", address_id));

                        SqlHelper.ExecuteNonQuery(destinationConnection,CommandType.Text,sql,paramList.ToArray());
                    }
                }


            }
        }
    }
}
