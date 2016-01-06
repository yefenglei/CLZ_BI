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
    public class MSSQLServerDataCopy : BaseDataCopy
    {
        public override void Copy() 
        {
            //获取mssqlserver对应表中的数据
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);

            using (SqlConnection SourceConn =new SqlConnection(SourceConnection))
            {
                SourceConn.Open();
                DataTable dt = SqlHelper.ExecuteDataset(SourceConn, CommandType.Text, QueryString,timeout).Tables[0];
                using (SqlConnection destinationConnection = new SqlConnection(DestinationConnection))
                {
                    destinationConnection.Open();
                    //在拷贝执行之前
                    if (!string.IsNullOrEmpty(BeforeQuery))
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
                            bulkCopy.BulkCopyTimeout = timeout;
                            bulkCopy.BatchSize = BatchSize;
                            bulkCopy.WriteToServer(dt);
                            //在拷贝执行之后
                            if (!string.IsNullOrEmpty(AfterQuery))
                            {
                                SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, AfterQuery);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }  
        }

        public void CustomCopy() 
        {
            //获取mssqlserver对应表中的数据
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);

            using (SqlConnection SourceConn = new SqlConnection(SourceConnection))
            {
                SourceConn.Open();
                string sql = "select PRODUCT_NAME from DIC_PRODUCT;";
                DataTable dt = SqlHelper.ExecuteDataset(SourceConn, CommandType.Text, sql, timeout).Tables[0];//获取所有商品名称列表

                using (SqlConnection destinationConnection = new SqlConnection(DestinationConnection))
                {
                    destinationConnection.Open();
                    //在拷贝执行之前
                    if (!string.IsNullOrEmpty(BeforeQuery))
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
                            foreach(DataRow dr in dt.Rows)//遍历所有商品
                            {
                                DataTable datatable = SqlHelper.ExecuteDataset(SourceConn, CommandType.Text, QueryString.Replace("@productName", "'" + dr["PRODUCT_NAME"].ToString() + "'"), timeout).Tables[0];//根据商品名称，获取对应商品的数据
                                // Write from the source to the destination.
                                bulkCopy.ColumnMappings.Clear();
                                 foreach (DataColumn colName in datatable.Columns)
                                {
                                    bulkCopy.ColumnMappings.Add(colName.ColumnName, colName.ColumnName);
                                }
                                bulkCopy.BulkCopyTimeout = timeout;
                                bulkCopy.WriteToServer(datatable);
                                //在拷贝执行之后
                                if (!string.IsNullOrEmpty(AfterQuery))
                                {
                                    SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, AfterQuery);
                                }
                            }      
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }  
        }

        /// <summary>
        /// ZH_DAILY_GOODS_DEAL
        /// </summary>
        public void HISTORY_SETTLE_DEAL_MAIN_Copy(DateTime startDate) 
        {
            //获取mssqlserver对应表中的数据
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);

            using (SqlConnection SourceConn = new SqlConnection(SourceConnection))
            {
                SourceConn.Open();
                List<SqlParameter> paramArr = new List<SqlParameter>();
                paramArr.Add(new SqlParameter("@startDate", startDate));
                paramArr.Add(new SqlParameter("@endDate", startDate.AddDays(1)));
                DataTable dt = SqlHelper.ExecuteDataset(SourceConn, CommandType.Text, QueryString, timeout, paramArr.ToArray()).Tables[0];
                using (SqlConnection destinationConnection = new SqlConnection(DestinationConnection))
                {
                    destinationConnection.Open();
                    //在拷贝执行之前
                    if (!string.IsNullOrEmpty(BeforeQuery))
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
                            bulkCopy.BulkCopyTimeout = timeout;
                            bulkCopy.BatchSize = BatchSize;
                            bulkCopy.WriteToServer(dt);
                            //在拷贝执行之后
                            if (!string.IsNullOrEmpty(AfterQuery))
                            {
                                SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, AfterQuery);
                            }

                            // 调用存储过程，计算每日均价
                            List<SqlParameter> paramList = new List<SqlParameter>();
                            paramList.Add(new SqlParameter("@startDate", startDate));
                            paramList.Add(new SqlParameter("@endDate", startDate));
                            SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.StoredProcedure, "ps_calculate_avg", paramList.ToArray());
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }      
        }


        public void HISTORY_SETTLE_DEAL_DETAIL_Copy(DateTime startDate)
        {
            //获取mssqlserver对应表中的数据
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOut"]);

            using (SqlConnection SourceConn = new SqlConnection(SourceConnection))
            {
                SourceConn.Open();
                List<SqlParameter> paramArr = new List<SqlParameter>();
                paramArr.Add(new SqlParameter("@startDate", startDate));
                paramArr.Add(new SqlParameter("@endDate", startDate.AddDays(1)));
                DataTable dt = SqlHelper.ExecuteDataset(SourceConn, CommandType.Text, QueryString, timeout, paramArr.ToArray()).Tables[0];
                using (SqlConnection destinationConnection = new SqlConnection(DestinationConnection))
                {
                    destinationConnection.Open();
                    //在拷贝执行之前
                    if (!string.IsNullOrEmpty(BeforeQuery))
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
                            bulkCopy.BulkCopyTimeout = timeout;
                            bulkCopy.BatchSize = BatchSize;
                            bulkCopy.WriteToServer(dt);
                            //在拷贝执行之后
                            if (!string.IsNullOrEmpty(AfterQuery))
                            {
                                SqlHelper.ExecuteNonQuery(destinationConnection, CommandType.Text, AfterQuery);
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                        }
                    }
                }
            }
        }
    }
}
