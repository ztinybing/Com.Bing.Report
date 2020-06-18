using System;
using System.Collections.Generic;
using System.Data;
using Com.Bing.API;

namespace Com.Bing.Report
{
    public class ConstantKey
    {
        //可更改
        public const string XXXFLAG = "xxx_flag";
        public const int PAGINGFLAGVAL = -1;
        public const int BOTTOMDATAROW = 1000;
        public const int TOGETHERDATAROW = 2000;
        public const int COMMONDATAROW = 0;
    }
    public class ReportData
    {
        private Report dynamicReport;
        public Report DynamicReport
        {
            get { return dynamicReport; }
        }

        private Report report;
        BodyTextFixedPos bodyTexts;
        private List<string> listNoPrintColumn = new List<string>();
        private List<string> listNoDataPrintColumn = new List<string>();
        private DynmicColumnInfo dynmicColumnInfo;
        private DataTable bodyData = new DataTable();
        public DataTable BodyData
        {
            get { return bodyData; }
        }
        private MacrosVerbCollection macorsVerbCollection = new MacrosVerbCollection();
        public MacrosVerbCollection MacorsVerbCollection
        {
            get { return macorsVerbCollection; }
        }
        public ReportData()
        {
        }
        public ReportData(Report report)
        {
            bodyTexts = new BodyTextFixedPos(report);
            this.report = report;
        }

        //记录固定不打印和无数据不打印的列
        private void InitTableStruct(Report report, DataTable dt)
        {
            List<string> columns = new List<string>();
            foreach (ReportColumn column in report.Columns)
            {
                if (column.Attibutes.PrintStyle.NoPrint)
                {
                    //固定不打印
                    if (dt.Columns.Contains(column.ColumnName))
                    {
                        //将固定不打印列的合并数据移动到下一个显示的列中
                        foreach (DataRow row in dt.Rows)
                        {
                            //空group不处理
                            if (dt.Columns.Contains("rowGroup") && !string.IsNullOrEmpty(row["rowGroup"].ToString()))
                            {
                                int rowGroup = Convert.ToInt32(row["rowGroup"]);
                                if (rowGroup > 0)
                                {
                                    Text text = bodyTexts[rowGroup, column.ColumnName];
                                    if (text != null)
                                    {
                                        for (int i = text.Location.X1 - 1; i < text.Location.X2 - 1; i++)
                                        {
                                            if (!report.Columns[i].Attibutes.PrintStyle.NoPrint)
                                            {
                                                string colName = report.Columns[i].ColumnName;
                                                row[colName] = row[column.ColumnName];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        dt.Columns.Remove(column.ColumnName);
                    }
                    listNoPrintColumn.Add(column.ColumnName);
                    continue;
                }
                if (column.Attibutes.PrintStyle.NoDataNoPrint)
                {
                    if (!dt.Columns.Contains(column.ColumnName))
                    {
                        listNoDataPrintColumn.Add(column.ColumnName);
                        continue;
                    }
                    //只确认表体中是否存在数据
                    DataRow[] notNullRows;
                    if (dt.Columns.Contains("rowGroup"))
                        notNullRows = dt.Select(String.Format("[{0}]<> '' and rowGroup > 0", column.ColumnName));
                    else
                        notNullRows = dt.Select(String.Format("[{0}]<> '' ", column.ColumnName));
                    if (notNullRows.Length == 0)
                    {
                        listNoDataPrintColumn.Add(column.ColumnName);
                        continue;
                    }
                }
                columns.Add(column.ColumnName);
            }
            bodyTexts.InitNoDataPrintColumn(listNoDataPrintColumn);
            InitTableColumn(columns.ToArray());
        }
        //初始化数据区结构
        public void InitTableColumn(string[] column)
        {
            bodyData.Columns.Clear();
            foreach (string columnName in column)
            {
                bodyData.Columns.Add(columnName, typeof(BodyDataStruct));
            }
            bodyData.Columns.Add(ConstantKey.XXXFLAG);
        }

        //无数据情况加入显示报表设计中的样式
        private void ShowDesignStyle(Report report)
        {
            //依次读取表体的行,填入到数据区中
            for (int i = 1; i <= report.Bands[3]; i++)
            {
                DataRow row = bodyData.NewRow();
                int notVisibleCount = 0;
                string xxxFlag = "0";
                foreach (Text text in bodyTexts.BodyTexts.GetTextsBy(3, i))
                {
                    if (text.Location.X2 != report.Columns.Count + 1)
                    {
                        string columnName = report.Columns[text.Location.X1 - 1].ColumnName;
                        if (listNoPrintColumn.Contains(columnName) || listNoDataPrintColumn.Contains(columnName))
                        {
                            notVisibleCount++;
                            continue;
                        }
                        BodyDataStruct dataStruct = new BodyDataStruct();
                        //合并信息
                        int noPrintCount = 0;
                        int noDataPrintCount = 0;

                        if (text.ColumnSpan > 1)
                        {
                            int columnSpan = text.ColumnSpan;
                            noPrintCount = GetNotDisplayColumnCount(report, columnName, columnSpan, listNoPrintColumn);
                            noDataPrintCount = GetNotDisplayColumnCount(report, columnName, columnSpan, listNoDataPrintColumn);
                        }
                        dataStruct.ColMergeInfo.MergeCount = text.ColumnSpan - noPrintCount - noDataPrintCount;

                        dataStruct.RowMergeInfo.RowMergeCount = text.RowSpan;

                        dataStruct.Context = text.Context;
                        //对齐方式

                        dataStruct.ColMergeInfo.LineAlignment = text.Attribute.Valign.LineAlignment;
                        dataStruct.ColMergeInfo.Alignment = text.Attribute.Valign.Alignment;

                        //边框信息
                        dataStruct.RowMergeInfo.BoundaryLine = text.Attribute.BoundaryLine.Clone();
                        //字体
                        dataStruct.ColMergeInfo.Font = text.Attribute.Font;
                        row[columnName] = dataStruct;
                    }
                    else
                    {
                        xxxFlag = text.Context;
                    }
                }
                row[ConstantKey.XXXFLAG] = xxxFlag;


                bodyData.Rows.Add(row);
            }
            DataRow newRow = bodyData.NewRow();
            newRow[ConstantKey.XXXFLAG] = ConstantKey.PAGINGFLAGVAL;
            bodyData.Rows.Add(newRow);
        }
        //填充数据区的数据及格式
        public void InitReportData(DataTable dt)
        {
            //有共享列的情况下，需要克隆个report
            dynmicColumnInfo = new DynmicColumnInfo(report, dt, this);
            Report currReport = dynmicColumnInfo.CurrReport;
            if (dynmicColumnInfo.CurrReport != report)
            {
                dynamicReport = currReport;
            }
            try
            {
                InitTableStruct(currReport, dt);
                //RemoveNoPrintColumn(currReport,listNoPrintColumn);
                //RemoveNoPrintColumn(currReport,listNoDataPrintColumn);
                if (dt.Columns.Contains(ConstantKey.XXXFLAG))
                {
                    //旧版报表数据处理,带XXX_FLAG
                    FillOldData(dt, currReport);
                }
                else
                {
                    FillNewData(dt, currReport);
                }
            }
            catch (Exception e)
            {
                throw new InitDesignColumnException(e);
            }
            if (bodyData.Rows.Count > 0)
            {
                DataRow lastRow = bodyData.Rows[bodyData.Rows.Count - 1];
                if (lastRow[ConstantKey.XXXFLAG].ToString() != ConstantKey.PAGINGFLAGVAL.ToString())
                {
                    //最后一行必须为-1
                    DataRow newRow = bodyData.NewRow();
                    newRow[ConstantKey.XXXFLAG] = ConstantKey.PAGINGFLAGVAL;
                    bodyData.Rows.Add(newRow);
                }
            }
            else
            {
                ShowDesignStyle(currReport);
            }
        }

        //合并空白行信息
        public void MergeBlack(Report report, Dictionary<string, int> dicMergeColInfo)
        {
            foreach (KeyValuePair<string, int> pair in dicMergeColInfo)
            {
                string[] point = pair.Key.Split(',');
                int rowIndex = Convert.ToInt32(point[0]);
                int colIndex = Convert.ToInt32(point[1]) - 1;
                BodyDataStruct dataStruct = report.Data.BodyData.Rows[rowIndex][colIndex] as BodyDataStruct;
                if (dataStruct != null)
                {
                    dataStruct.RowMergeInfo.RowMergeCount = pair.Value + 1;
                    for (int i = rowIndex + 1; i < rowIndex + pair.Value + 1; i++)
                    {
                        BodyDataStruct bStruct = report.Data.BodyData.Rows[i][colIndex] as BodyDataStruct;
                        bStruct.Tag = "mergeCol";
                        if (bStruct != null)
                            bStruct.RowMergeInfo.BoundaryLine.UpperBoundaryLine = false;
                    }
                }
            }

            for (int i = 0; i < report.Data.BodyData.Rows.Count; i++)
            {
                int colMerge = 1;
                for (int j = report.Data.BodyData.Columns.Count - 1; j >= 0; j--)
                {
                    BodyDataStruct dataStruct = bodyData.Rows[i][j] as BodyDataStruct;
                    if (report.Data.BodyData.Columns[j].ColumnName != "xxx_flag")
                    {
                        if (dataStruct == null)
                        {
                            colMerge++;
                        }
                        else if (dataStruct.Context.Trim() == "" && dataStruct.Tag.ToString() == "")
                        {
                            //去掉空白行的左右边框线
                            dataStruct.RowMergeInfo.BoundaryLine.LeftBoundaryLine = false;
                            if (j != bodyData.Columns.Count - 2)
                            {
                                dataStruct.RowMergeInfo.BoundaryLine.RightBooundaryLine = false;
                            }
                            colMerge++;
                        }
                        else
                        {
                            dataStruct.ColMergeInfo.MergeCount = colMerge;
                            //合并空行右边框显示问题
                            if (j + colMerge - 1 != bodyData.Columns.Count - 2)
                            {
                                dataStruct.RowMergeInfo.BoundaryLine.RightBooundaryLine = false;
                            }
                            colMerge = 1;
                        }
                    }
                }
            }

        }

        private void FillOldData(DataTable dt, Report currReport)
        {
            int pageNum = 1;
            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row[ConstantKey.XXXFLAG].ToString()))
                {
                    row[ConstantKey.XXXFLAG] = 0;
                }
                DataRow newRow = bodyData.NewRow();
                switch (row[ConstantKey.XXXFLAG].ToString())
                {
                    case "-1":
                        pageNum++;
                        newRow[ConstantKey.XXXFLAG] = -1;
                        bodyData.Rows.Add(newRow);
                        break;
                    case "-3":
                        string[] macros = System.Text.RegularExpressions.Regex.Split(row["xxx_expand1"].ToString(), @"\(@@\)");
                        string macroName = macros[0];
                        string macroValue = macros[1];
                        macorsVerbCollection.Add(pageNum, macroName, macroValue);
                        break;
                    case "0":
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (bodyData.Columns.Contains(column.ColumnName))
                            {
                                BodyDataStruct dataStruct = new BodyDataStruct();
                                dataStruct.Context = row[column].ToString();
                                //设置是否进入本页小计
                                setInPageRow(dt, row, dataStruct);

                                newRow[column.ColumnName] = dataStruct;
                            }
                        }
                        bodyData.Rows.Add(newRow);
                        break;
                    default:
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (column.ColumnName.StartsWith("xxx_expand") && !column.ColumnName.EndsWith("sx_ex"))
                            {
                                string value = row[column].ToString();
                                if (!string.IsNullOrEmpty(value))
                                {
                                    string[] propertys = System.Text.RegularExpressions.Regex.Split(value, @"\(@@\)");
                                    BodyDataStruct dataStruct = new BodyDataStruct();
                                    //设置是否进入本页小计
                                    setInPageRow(dt, row, dataStruct);

                                    int startColumnIndex = int.Parse(propertys[0]);
                                    int endColumnIndex = int.Parse(propertys[1]);
                                    //有列合并的，查看是否存在固定不打印的列
                                    int noPrintCount = 0;
                                    int noDataPrintCount = 0;
                                    int columnSpan = endColumnIndex - startColumnIndex + 1;
                                    if (columnSpan > 1)
                                    {
                                        noPrintCount = GetNotDisplayColumnCount(currReport, startColumnIndex, endColumnIndex, listNoPrintColumn);
                                        noDataPrintCount = GetNotDisplayColumnCount(currReport, startColumnIndex, endColumnIndex, listNoDataPrintColumn);
                                    }

                                    //合并信息
                                    dataStruct.ColMergeInfo.MergeCount = columnSpan - noPrintCount - noDataPrintCount;
                                    dataStruct.Context = propertys[2];
                                    if (propertys[3].ToString() == "0")
                                        dataStruct.ColMergeInfo.LineAlignment = System.Drawing.StringAlignment.Near;
                                    else if (propertys[3].ToString() == "1")
                                        dataStruct.ColMergeInfo.LineAlignment = System.Drawing.StringAlignment.Far;
                                    else
                                        dataStruct.ColMergeInfo.LineAlignment = System.Drawing.StringAlignment.Center;

                                    if (propertys[4].ToString() == "700")
                                        dataStruct.ColMergeInfo.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);

                                    string sxColumn = column.ColumnName + "_sx_ex";
                                    if (dt.Columns.Contains(sxColumn))
                                    {
                                        string sxValue = row[sxColumn].ToString();
                                        if (!string.IsNullOrEmpty(sxValue))
                                        {
                                            string[] merge = sxValue.Split(',');
                                            if (merge.Length >= 5)
                                            {
                                                dataStruct.RowMergeInfo.RowMergeCount = int.Parse(merge[0]);
                                                dataStruct.RowMergeInfo.BoundaryLine.UpperBoundaryLine = merge[1] == "1";
                                                dataStruct.RowMergeInfo.BoundaryLine.LowerBoundaryLine = merge[2] == "1";
                                                dataStruct.RowMergeInfo.BoundaryLine.LeftBoundaryLine = merge[3] == "1";
                                                dataStruct.RowMergeInfo.BoundaryLine.RightBooundaryLine = merge[4] == "1";
                                            }
                                        }
                                    }

                                    for (int i = startColumnIndex - 1; i < endColumnIndex; i++)
                                    {
                                        if (report.Columns.Count > i)
                                        {
                                            string colName = report.Columns[i].ColumnName;
                                            if (bodyData.Columns.Contains(colName))
                                            {
                                                newRow[colName] = dataStruct;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        newRow[ConstantKey.XXXFLAG] = row[ConstantKey.XXXFLAG];
                        bodyData.Rows.Add(newRow);
                        break;
                }
            }
        }
        private void FillNewData(DataTable dt, Report currReport)
        {
            int pageNum = 1;
            int titleBandCount = 0;
            //将datatable根据-1,-2进行拆分
            List<DataTable> listDt = SplitTable(dt);

            foreach (DataTable dataTable in listDt)
            {
                //整张表无数据,并且不是第一张表，忽略掉此表
                if (dataTable.Select("rowGroup>=0").Length == 0 && pageNum > 1)
                    continue;
                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow newRow = bodyData.NewRow();
                    //-1分页符 -2分隔符 -3宏变量  (-20,-5)共享列中的表头
                    int rowGroup = Convert.ToInt32(row["rowGroup"]);
                    if (rowGroup == -1)
                    {
                        pageNum++;
                        newRow[ConstantKey.XXXFLAG] = -1;
                        bodyData.Rows.Add(newRow);
                    }
                    else if (rowGroup == -2)
                    {

                    }
                    else if (rowGroup == -3)
                    {
                        macorsVerbCollection.Add(pageNum, row["macroName"].ToString(), row["macroValue"].ToString());
                    }
                    else if (rowGroup >= -20 && rowGroup <= -5)
                    {
                        int count = 0;
                        int dynmicColumnCount = 0;

                        for (int i = 0; i < bodyData.Columns.Count - 1; i++)
                        {
                            string colName = bodyData.Columns[i].ColumnName;
                            ReportColumn column = currReport.Columns.Find(colName);
                            bool isDynmicColumn = column.Attibutes.DynamicColumn;

                            string[] strs = colName.Split('_');
                            if (bodyTexts.HasText(Convert.ToInt32(row["rowGroup"]), colName) || isDynmicColumn)
                            {
                                Text text = bodyTexts[Convert.ToInt32(row["rowGroup"]), isDynmicColumn ? strs[0] : colName];
                                if (text == null)
                                {
                                    if (isDynmicColumn)
                                        count++;
                                    continue;
                                }
                                Text newText = new Text();
                                newText.Attribute = text.Attribute;
                                Location location = new Location();
                                location.Y1 = text.Location.Y1;
                                location.Y2 = text.Location.Y2;
                                /*
                                  单元格位置信息
                                  数据中共享列的位置 = rpt中的位置 + 共享列个数
                                  共享列之钱普通数据位置 = rpt中的位置
                                  共享列之后的普通数据 = rpt中的位置 + 共享列个数 - 1(去掉原始的占列位置)
                                */

                                if (isDynmicColumn)
                                {
                                    int notVisibleColCount = dynmicColumnInfo.GetNotVistbleCount(dynmicColumnInfo.DynmicColumns, strs[0], text.ColumnSpan);
                                    int beforeNotVisibleColCount = dynmicColumnInfo.GetBeforeNotVisibleCount(dynmicColumnInfo.DynmicColumns, strs[0]);
                                    int xs = count / dynmicColumnInfo.DynmicColumns.VisibleColumnCount;
                                    location.X1 = text.Location.X1 + xs * dynmicColumnInfo.DynmicColumns.VisibleColumnCount - beforeNotVisibleColCount;
                                    location.X2 = text.Location.X2 + xs * dynmicColumnInfo.DynmicColumns.VisibleColumnCount - notVisibleColCount - beforeNotVisibleColCount;
                                    count += text.ColumnSpan - notVisibleColCount;
                                    dynmicColumnCount += text.ColumnSpan - notVisibleColCount;

                                }
                                else
                                {
                                    ////共享列的个数，有共享列的情况下，需要去掉原始共享列暂用位置
                                    //int tempDynmicColCount = dynmicColumnCount;
                                    //if (dynmicColumnCount > 0)
                                    //    tempDynmicColCount = dynmicColumnCount - dynmicColumnInfo.DynmicColumns.Count;
                                    location.X1 = text.Location.X1 + dynmicColumnCount;
                                    location.X2 = text.Location.X2 + dynmicColumnCount;
                                }

                                if (text.ColumnSpan > dynmicColumnInfo.DynmicColumns.Count)
                                {
                                    int colSpan = 0;
                                    List<string> list = new List<string>();
                                    //判断合并列中是否包含了共享列，包含的话从新计算columnSpan
                                    int index = currReport.Columns.IndexOf(colName);
                                    int spanCount = GetSpanCount(report, index, text.ColumnSpan);

                                    for (int k = index; k < index + spanCount; k++)
                                    {
                                        if (currReport.Columns[k].Attibutes.DynamicColumn)
                                        {
                                            string dynmicName = currReport.Columns[k].ColumnName.Split('_')[0];
                                            if (!list.Contains(dynmicName))
                                            {
                                                list.Add(dynmicName);
                                                foreach (ReportColumn col in currReport.Columns)
                                                {
                                                    if (dynmicName == col.ColumnName.Split('_')[0])
                                                    {
                                                        colSpan += 1;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            colSpan += 1;
                                    }
                                    int oldX2 = location.X2;
                                    location.X2 = location.X1 + colSpan - 1;
                                    int noPrintColumnCount = GetNotDisplayColumnCount(report, colName, text.ColumnSpan, listNoPrintColumn);
                                    dynmicColumnCount = location.X2 - oldX2 + noPrintColumnCount;
                                }

                                newText.Location = location;
                                string context = string.Empty;
                                if (dataTable.Columns.Contains(colName))
                                    context = row[colName].ToString();
                                newText.Context = context;
                                newText.BandIndex = 2;
                                currReport.Texts.Add(newText);
                                i += newText.ColumnSpan - 1;
                                currReport.Bands.SetBindRowNum(2, 1);
                            }
                        }
                        titleBandCount++;
                        currReport.Bands.SetBindRowNum(2, titleBandCount);
                    }
                    else
                    {
                        int xxx_flag = 0;
                        bool validRowGroup = false;
                        for (int i = 0; i < bodyData.Columns.Count - 1; i++)
                        {
                            xxx_flag++;
                            string colName = bodyData.Columns[i].ColumnName;

                            bool isDynmicColumn = false;
                            if (dynamicReport != null)
                            {
                                ReportColumn column = dynamicReport.Columns.Find(colName);
                                if (column != null)
                                    isDynmicColumn = column.Attibutes.DynamicColumn;
                            }
                            string[] strs = colName.Split('_');
                            Text text = null;

                            if (isDynmicColumn)
                                text = bodyTexts[rowGroup, strs[0]];
                            else
                            {
                                if (bodyTexts.HasText(rowGroup, colName))
                                    text = bodyTexts[rowGroup, colName];
                                else
                                {
                                    text = GetPreText(rowGroup, colName);
                                }
                            }


                            if (text == null)
                                continue;


                            validRowGroup = true;
                            BodyDataStruct dataStruct = new BodyDataStruct();
                            //设置是否进入本页小计
                            setInPageRow(dataTable, row, dataStruct);

                            if (dataTable.Columns.Contains(colName))
                            {
                                dataStruct.Context = string.IsNullOrEmpty(text.CellText) ? row[colName].ToString() : text.CellText;
                            }
                            else
                            {
                                dataStruct.Context = text.CellText;
                            }

                            //有列合并的，查看是否存在固定不打印的列
                            int noPrintCount = 0;
                            int noDataPrintCount = 0;

                            if (text.ColumnSpan > 1)
                            {
                                int columnSpan = text.ColumnSpan;
                                noPrintCount = GetNotDisplayColumnCount(currReport, colName, columnSpan, listNoPrintColumn);
                                noDataPrintCount = GetNotDisplayColumnCount(currReport, colName, columnSpan, listNoDataPrintColumn);
                            }

                            //合并信息
                            dataStruct.ColMergeInfo.MergeCount = text.ColumnSpan - noPrintCount - noDataPrintCount;
                            if (text.RowSpan > 1)
                            {
                                List<string> listRowGroup = new List<string>();

                                //有行合并的，查看数据中包含多少个合并行
                                for (int j = text.Location.Y1; j < text.Location.Y1 + text.RowSpan; j++)
                                {
                                    foreach (Text bodyText in bodyTexts.BodyTexts)
                                    {
                                        if (
                                        bodyText.Location.Y1 == j
                                        && bodyText.Location.Y2 == j
                                        && bodyText.Location.X1 == report.Columns.Count + 1
                                        && bodyText.Location.X2 == report.Columns.Count + 1
                                        )
                                        {
                                            listRowGroup.Add(bodyText.Context);
                                        }
                                    }
                                }
                                DataRow[] rows = dataTable.Select(String.Format("RowGroup in ({0})", string.Join(",", listRowGroup.ToArray())));
                                dataStruct.RowMergeInfo.RowMergeCount = rows.Length;
                            }
                            else
                            {
                                dataStruct.RowMergeInfo.RowMergeCount = text.RowSpan;
                            }
                            //对齐方式

                            dataStruct.ColMergeInfo.LineAlignment = text.Attribute.Valign.LineAlignment;
                            dataStruct.ColMergeInfo.Alignment = text.Attribute.Valign.Alignment;

                            //边框信息
                            dataStruct.RowMergeInfo.BoundaryLine = text.Attribute.BoundaryLine.Clone();
                            //特殊线
                            switch (text.Attribute.Diagonal)
                            {
                                case 1:
                                    dataStruct.RowMergeInfo.BoundaryLine.IsSlash = true;
                                    break;
                                case 2:
                                    dataStruct.RowMergeInfo.BoundaryLine.IsBackSlash = true;
                                    break;
                                case 3:
                                    dataStruct.RowMergeInfo.BoundaryLine.IsCrossLine = true;
                                    break;
                                default:
                                    dataStruct.RowMergeInfo.BoundaryLine.IsSlash = false;
                                    dataStruct.RowMergeInfo.BoundaryLine.IsBackSlash = false;
                                    dataStruct.RowMergeInfo.BoundaryLine.IsCrossLine = false;
                                    break;
                            }
                            //字体
                            dataStruct.ColMergeInfo.Font = text.Attribute.Font;

                            newRow[colName] = dataStruct;
                            i += text.ColumnSpan - 1 - noPrintCount - noDataPrintCount;
                        }
                        if (validRowGroup)
                        {
                            newRow[ConstantKey.XXXFLAG] = rowGroup > 1000 ? rowGroup : xxx_flag;
                            bodyData.Rows.Add(newRow);
                        }
                    }
                }
            }

        }

        #region 支持函数
        //获取前一个不显示的单元格的信息

        private int GetSpanCount(Report report, int index, int columnSpan)
        {
            int spanCount = 0;
            for (int i = index; i < index + columnSpan; i++)
            {
                if (!report.Columns[i].Attibutes.PrintStyle.NoPrint)
                    spanCount++;
            }
            return spanCount;
        }
        private Text GetPreText(int rowGroup, string colName)
        {
            Text preText = null;
            int index = report.Columns.IndexOf(colName);
            if (report.Columns.Count >= index && index > 0)
            {
                string preColumnName = report.Columns[index - 1].ColumnName;
                if (bodyTexts.HasText(rowGroup, preColumnName))
                {
                    Text text = bodyTexts[rowGroup, preColumnName].Clone();
                    if (report.Columns[preColumnName].Attibutes.PrintStyle.NoPrint && text.ColumnSpan > 1)
                    {
                        text.Location.X1 += 1;
                        return text;
                    }
                }
                else
                {
                    preText = GetPreText(rowGroup, preColumnName);
                    if (preText != null)
                    {
                        preText.Location.X1 += 1;
                    }
                }
            }
            return preText;
        }

        //设置是否进入本页小计标识
        private void setInPageRow(DataTable dt, DataRow row, BodyDataStruct dataStruct)
        {
            if (dt.Columns.Contains("inpagerow") && row["inpagerow"].ToString().ToLower() == "no")
            {
                dataStruct.IsFunCell = false;
            }
        }

        //根据-1,-2拆分数据
        private List<DataTable> SplitTable(DataTable dtSource)
        {
            List<DataTable> list = new List<DataTable>();

            DataTable dt = dtSource.Clone();
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                if (dtSource.Rows[i]["rowGroup"].ToString() != "-1" && dtSource.Rows[i]["rowGroup"].ToString() != "-2")
                {
                    dt.ImportRow(dtSource.Rows[i]);
                }
                else
                {
                    //添加分隔符(-1为分页,-2为分隔)
                    dt.ImportRow(dtSource.Rows[i]);

                    list.Add(dt);
                    dt = dtSource.Clone();
                }
            }
            if (dt.Rows.Count > 0)
            {
                list.Add(dt);
            }
            return list;
        }
        //获取不显示的列的个数
        private int GetNotDisplayColumnCount(Report report, string columnName, int count, List<string> list)
        {
            int noPrintCount = 0;
            int index = report.Columns.IndexOf(columnName);
            if (index > -1)
            {
                for (int i = index; i < index + count; i++)
                {
                    if (list.Contains(report.Columns[i].ColumnName))
                    {
                        noPrintCount++;
                    }
                }
            }
            return noPrintCount;
        }
        private int GetNotDisplayColumnCount(Report report, int startIndex, int endIndex, List<string> list)
        {
            int noPrintCount = 0;
            for (int i = startIndex - 1; i < endIndex; i++)
            {
                if (list.Contains(report.Columns[i].ColumnName))
                {
                    noPrintCount++;
                }
            }
            return noPrintCount;
        }
        #endregion
    }

}

