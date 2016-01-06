using CLZ.Common;
using CLZ.Core;
using CLZ.Model;
using CLZ.Model.common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CLZ.Controllers
{
    public class OverviewController : BaseController
    {
        //
        // GET: /Overview/
        public static readonly string connStr = ConfigurationManager.AppSettings["SQLServer"];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview(int univgrid_id) 
        {
            try
            {
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<univgrid> univgrid = db.univgrid.Where(u => u.univgrid_id == univgrid_id);
                    IQueryable<univgrid_parameter> univgrid_parameterTable = db.univgrid_parameter.Where(u => u.univgrid_id == univgrid_id);
                    IQueryable<easyuigrid> easyuigrid = db.easyuigrid.Where(e => e.univgrid_id == univgrid_id);
                    IQueryable<easyuicolumn> easyuicolumnTable = from ec in db.easyuicolumn
                                                                  join eg in easyuigrid 
                                                                  on ec.easyuigrid_id equals eg.easyuigrid_id
                                                                  orderby ec.ordernum ascending
                                                                      select ec;
                    IQueryable<easyui_query_parameters> easyui_query_paramsTable = from eqp in db.easyui_query_parameters
                                                                                    join eg in easyuigrid
                                                                                    on eqp.easyuigrid_id equals eg.easyuigrid_id
                                                                                    select eqp;
                    IQueryable<highchart> highchart = db.highchart.Where(h => h.univgrid_id == univgrid_id);
                    easyuigrid easyuigridEntity=easyuigrid.SingleOrDefault();
                    highchart highchartEntity = highchart.SingleOrDefault();
                    IQueryable<highchart_column> highchart_column = db.highchart_column.Where(h => h.chartid == highchartEntity.chartid);


                    List<string> columnList=new List<string>();
                    string cols = univgrid_parameterTable.Where(u => u.param_id == 1).SingleOrDefault().value;//param 1 :获取需要在前台显示的列
                    foreach(var obj in easyuicolumnTable){
                        if (cols.Contains(obj.field))
                        {
                            columnList.Add("{ field: '" + obj.field + "', title: '" + obj.title + "', width: " + obj.width + ", sortable: " + obj.sortable.ToString().ToLower() + " }");
                        }
                    }
                    string columnStr="[["+string.Join(",",columnList.ToArray())+"]]";

                    string queryDateParams = string.Empty;
                    univgrid_parameter isShowDateSelector = univgrid_parameterTable.Where(u => u.param_id == 6).SingleOrDefault();//param 6 ：如果值为true，则在前台显示日期范围选择控件以及查询按钮
                    if (isShowDateSelector != null)
                    {
                        ViewBag.isShowDateSelector = isShowDateSelector.value;
                        if (isShowDateSelector.value=="true")
                        {
                            queryDateParams = "startDate:startDate,endDate:endDate,";
                        }
                    }
                    else 
                    {
                        ViewBag.isShowDateSelector = "false";
                    }

                    StringBuilder sbParams = new StringBuilder();
                    sbParams.Append("{");
                    sbParams.Append(string.Format("{0}:{1},", "univgrid_id", univgrid_id));
                    //foreach(var obj in easyui_query_paramsTable){
                    //    sbParams.Append(string.Format("{0}:{1},", obj.key, obj.key));
                    //}
                    sbParams.Append(queryDateParams);
                    sbParams.Remove(sbParams.Length - 1, 1);
                    sbParams.Append("}");

                    ViewBag.easyuiUrl = "GetEasyUIGridInfo";
                    ViewBag.easyuiMethord = easyuigridEntity.methord;
                    ViewBag.easyuiWidth = easyuigridEntity.width;
                    ViewBag.easyuiHeight = easyuigridEntity.height;
                    ViewBag.easyuiQueryParams = sbParams.ToString();
                    ViewBag.easyuiFitColumns = easyuigridEntity.fitColumns.ToString().ToLower();
                    ViewBag.easyuiSortName = easyuigridEntity.sortName;
                    ViewBag.easyuiSortOrder = easyuigridEntity.sortOrder;
                    ViewBag.easyuiRemoteSort = easyuigridEntity.remoteSort.ToString().ToLower();
                    ViewBag.easyuiIdField = easyuigridEntity.idField;
                    ViewBag.easyuiPageSize = easyuigridEntity.pageSize;
                    ViewBag.easyuiPageList = easyuigridEntity.pageList;
                    ViewBag.easyuiPagination = easyuigridEntity.pagination.ToString().ToLower();
                    ViewBag.easyuiShowFooter = easyuigridEntity.showFooter.ToString().ToLower();
                    ViewBag.easyuiStriped = easyuigridEntity.striped.ToString().ToLower();
                    ViewBag.easyuiSingleSelect = easyuigridEntity.singleSelect.ToString().ToLower();
                    ViewBag.easyuiRownumbers = easyuigridEntity.rownumbers.ToString().ToLower();
                    ViewBag.easyuiColumns = columnStr;

                    univgrid_parameter isHighChart = univgrid_parameterTable.Where(u => u.param_id == 21).SingleOrDefault();//param 21 :是否启用highchart图表
                    if (isHighChart != null && isHighChart.value == "true")
                    {
                        List<string> arrFieldList = new List<string>();
                        // 是否显示图
                        ViewBag.isShowHighChart = isHighChart.value;
                        string xAxisColumn = univgrid_parameterTable.Where(u => u.param_id == 22).SingleOrDefault().value;//param 22 :highchart图表 x轴的列

                        // 拼接图表的请求参数
                        string highchartQueryParams = string.Empty;
                        if (isShowDateSelector != null && isShowDateSelector.value == "true")
                        {
                            highchartQueryParams = "{ 'univgrid_id':" + univgrid_id + ",'startDate': startDate, 'endDate': endDate}";
                        }
                        else
                        {
                            highchartQueryParams = "{ 'univgrid_id':" + univgrid_id + "}";
                        }
                        ViewBag.highchartQueryParams = highchartQueryParams;

                        // 图表X轴的类型
                        string chartMode = highchartEntity.chartmode;

                        // 拼接series
                        StringBuilder sbSeries = new StringBuilder();
                        foreach (highchart_column col in highchart_column)
                        {
                            arrFieldList.Add(col.field);
                            sbSeries.Append("{name: '" + col.field_display + "',data: " + col.field  + ",yAxis: " + col.field_type + "},");
                        }
                        sbSeries.Remove(sbSeries.Length - 1, 1);
                        string series = "[" + sbSeries.ToString() + "]";

                        // 拼接x轴y轴数据
                        string arrListString = string.Empty;
                        string arrList = string.Empty;
                        string xAxis = highchartEntity.xAxis;
                        if (chartMode == "date")
                        {
                            var xAxisColumnArray = xAxisColumn + "Array";
                            arrListString = arrListString + string.Format("var {0}=[];", xAxisColumnArray);
                            foreach (string s in arrFieldList)
                            {
                                arrListString = arrListString + string.Format("var {0}=[];", s);
                            }
                            arrListString = arrListString + "var length = Number(data.total);";

                            arrList = "for (var i = 0; i < length; i++) { \r\n var tmpDateArray = data.rows[i]." + xAxisColumn + ".split(' ')[0].split('-');\r\n" + xAxisColumnArray + ".push(Date.UTC(parseInt(tmpDateArray[0]), parseInt(tmpDateArray[1]) - 1, parseInt(tmpDateArray[2]) + 1));\r\n";
                            for (int i = 0; i < arrFieldList.Count; i++)
                            {
                                arrList = arrList + "if(data.rows[i]." + arrFieldList[i] + " && data.rows[i]." + arrFieldList[i] + "!=''){" + arrFieldList[i] + ".push([" + xAxisColumnArray + "[i], data.rows[i]." + arrFieldList[i] + "]);}\r\n";
                            }
                            arrList = arrList + "}";
                        }
                        else if (chartMode == "normal")
                        {
                            arrListString = arrListString + string.Format("var {0}=[];", xAxisColumn);
                            foreach (string s in arrFieldList)
                            {
                                arrListString = arrListString + string.Format("var {0}=[];", s);
                            }
                            arrListString = arrListString + "var length = Number(data.total);";

                            arrList = "for (var i = 0; i < length; i++) { \r\n " + xAxisColumn + ".push(data.rows[i]." + xAxisColumn + ");\r\n";
                            for (int i = 0; i < arrFieldList.Count; i++)
                            {
                                arrList = arrList + arrFieldList[i] + ".push(data.rows[i]." + arrFieldList[i] + ");\r\n";
                            }
                            arrList = arrList + "}";

                            xAxis = "{categories: " + xAxisColumn + "}";
                        }


                        ViewBag.highchartArrayListDefinition = arrListString;
                        ViewBag.highchartArrayList = arrList;
                        ViewBag.highchartChart = highchartEntity.chart;
                        ViewBag.highchartTitle = highchartEntity.title;
                        ViewBag.highchartTooltip = highchartEntity.tooltip;
                        ViewBag.highchartPlotOptions = highchartEntity.plotOptions;
                        ViewBag.highchartXAxis = xAxis;
                        ViewBag.highchartYAxis = highchartEntity.yAxis;
                        ViewBag.highchartWidth = highchartEntity.width;
                        ViewBag.highchartHeight = highchartEntity.height;
                        ViewBag.highchartSeries = series;
                    }
                    else 
                    {
                        ViewBag.isShowHighChart = "false";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
            }

            return View();
        }


        public JsonResult GetEasyUIGridInfo(GridPager pager) {
            try
            {
                int univgrid_id = int.Parse(Request["univgrid_id"]);
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<univgrid> univgrid = db.univgrid.Where(u => u.univgrid_id == univgrid_id);
                    IQueryable<univgrid_parameter> univgrid_parameterTable = db.univgrid_parameter.Where(u => u.univgrid_id == univgrid_id);
                    IQueryable<easyuigrid> easyuigrid = db.easyuigrid.Where(e => e.univgrid_id == univgrid_id);
                    IQueryable<easyuicolumn> easyuicolumnTable = from ec in db.easyuicolumn
                                                                 join eg in easyuigrid
                                                                 on ec.easyuigrid_id equals eg.easyuigrid_id
                                                                 select ec;
                    IQueryable<easyui_query_parameters> easyui_query_paramsTable = from eqp in db.easyui_query_parameters
                                                                                   join eg in easyuigrid
                                                                                   on eqp.easyuigrid_id equals eg.easyuigrid_id
                                                                                   select eqp;

                    string cols = univgrid_parameterTable.Where(u=>u.param_id==1).SingleOrDefault().value;//param 1 :获取需要在前台显示的列
                    string tblName = univgrid_parameterTable.Where(u => u.param_id == 2).SingleOrDefault().value;//param 2 :获取数据源表
                    string sqlcmd = string.Empty;
                    string startDate = string.Empty;
                    string endDate=string.Empty;
                    string dateFilterColumn = string.Empty;
                    string primaryKeys = univgrid_parameterTable.Where(u => u.param_id == 5).SingleOrDefault().value;//param 5
                    univgrid_parameter isShowDateSelector = univgrid_parameterTable.Where(u => u.param_id == 6).SingleOrDefault();//param 6
                    if (isShowDateSelector != null && isShowDateSelector.value=="true")
                    {
                        dateFilterColumn = univgrid_parameterTable.Where(u => u.param_id == 7).SingleOrDefault().value;//param 7 需要进行日期范围筛选的字段
                        startDate = ConvertDate(Request["startDate"]).ToShortDateString();
                        endDate = ConvertDate(Request["endDate"], DateTime.Now.ToShortDateString()).ToShortDateString();
                    }

                    int totalRows = Convert.ToInt32(SqlHelper.ExecuteScalar(connStr, CommandType.Text, string.Format("select count(*) from {0}", tblName)));// 获取总的条数，用于计算页数
                    int startNum = (pager.page-1)*pager.rows;
                    int endNum = pager.page* pager.rows;
                    if (string.IsNullOrEmpty(dateFilterColumn))
                    {
                        sqlcmd = string.Format("select {0} from (select {0},ROW_NUMBER() over(order by {4} {5}) as rownum from {1})t where t.rownum>{2} and t.rownum<={3}", cols, tblName, startNum, endNum, pager.sort, pager.order);
                    }
                    else 
                    {
                        totalRows = Convert.ToInt32(SqlHelper.ExecuteScalar(connStr, CommandType.Text, string.Format("select count(*) from {0} where {1}>'{2}' and {1}<='{3}'", tblName, dateFilterColumn, startDate, endDate)));// 获取总的条数，用于计算页数
                        sqlcmd = string.Format("select {0} from (select t1.*,ROW_NUMBER() over(order by {4} {5}) as rownum from (select * from {1} where {6}>'{7}' and {6}<='{8}')t1)t where t.rownum>{2} and t.rownum<={3} ", cols, tblName, startNum, endNum, pager.sort, pager.order, dateFilterColumn, startDate, endDate);
                    }



                    DataTable dt = SqlHelper.ExecuteDataset(connStr, CommandType.Text, sqlcmd).Tables[0];
                    
                    var jsondata = new
                    {
                        total = totalRows,
                        rows = JsonHandler.Dtb2Json(dt)
                    };
                    return Json(jsondata, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHighChartInfo() {
            try
            {
                int univgrid_id = int.Parse(Request["univgrid_id"]);
                using (DBContainer db = new DBContainer())
                {
                    IQueryable<univgrid_parameter> univgrid_parameterTable = db.univgrid_parameter.Where(u => u.univgrid_id == univgrid_id);
                    IQueryable<easyuigrid> easyuigrid = db.easyuigrid.Where(e => e.univgrid_id == univgrid_id);
                    IQueryable<highchart> highchart = db.highchart.Where(h=>h.univgrid_id==univgrid_id);
                    highchart highchartEntity = highchart.SingleOrDefault();
                    IQueryable<highchart_column> highchart_column = db.highchart_column.Where(h => h.chartid == highchartEntity.chartid);

                    string cols = univgrid_parameterTable.Where(u => u.param_id == 1).SingleOrDefault().value;//param 1 :获取需要在前台显示的列
                    string tblName = univgrid_parameterTable.Where(u => u.param_id == 2).SingleOrDefault().value;//param 2 :获取数据源表
                    string sqlcmd = string.Empty;
                    string startDate = string.Empty;
                    string endDate = string.Empty;
                    string dateFilterColumn = string.Empty;

                    univgrid_parameter isShowDateSelector = univgrid_parameterTable.Where(u => u.param_id == 6).SingleOrDefault();//param 6
                    if (isShowDateSelector != null && isShowDateSelector.value == "true")
                    {
                        dateFilterColumn = univgrid_parameterTable.Where(u => u.param_id == 7).SingleOrDefault().value;//param 7 需要进行日期范围筛选的字段
                        startDate = ConvertDate(Request["startDate"]).ToShortDateString();
                        endDate = ConvertDate(Request["endDate"], DateTime.Now.ToShortDateString()).ToShortDateString();
                    }

                    string xAxisColumn = univgrid_parameterTable.Where(u => u.param_id == 22).SingleOrDefault().value;//param 22 :highchart图表 x轴的列
                    if (!cols.Contains(xAxisColumn))
                    {
                        cols += "," + xAxisColumn;
                    }

                    if (string.IsNullOrEmpty(dateFilterColumn))
                    {
                        sqlcmd = string.Format("select {0} from {1}", cols, tblName);
                    }
                    else
                    {
                        sqlcmd = string.Format("select {0} from {1} where {2}>'{3}' and {2}<='{4}' order by {2} asc", cols, tblName, dateFilterColumn, startDate, endDate);
                    }
                    string isHighChart = univgrid_parameterTable.Where(u => u.param_id == 21).SingleOrDefault().value;//param 21 :是否启用highchart图表
                    string xAxis = univgrid_parameterTable.Where(u => u.param_id == 22).SingleOrDefault().value;//param 22 :highchart图表 x轴的列


                    DataTable dt = SqlHelper.ExecuteDataset(connStr, CommandType.Text, sqlcmd).Tables[0];

                    var jsondata = new
                    {
                        total=dt.Rows.Count,
                        rows = JsonHandler.Dtb2Json(dt)
                    };
                    return Json(jsondata, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.WriteException(ex);
                throw ex;
            }
        }

        private DateTime ConvertDate(string date, string defaultDate = "1970/01/01")
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            if (string.IsNullOrEmpty(date))
            {
                return Convert.ToDateTime(defaultDate, format);
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(date, format);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.WriteException(new Exception("开始日期格式不对", ex));
                    return Convert.ToDateTime(defaultDate, format);
                }
            }
        }
    }
}
