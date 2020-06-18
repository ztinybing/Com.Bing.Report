using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using Com.Bing.API;
using KeyType = System.Collections.Generic.KeyValuePair<string, int>;

namespace Com.Bing.Report
{
    /// <summary>
    /// 将加载设计信息的数据转换成能直接画的数据
    /// </summary>
    public class DataAnalyse : IDisposable
    {
        /// <summary>
        /// 在数据分析过程中,使用Report从文件中读取的属性及数据Table
        /// </summary>
        private Report report = null;
        private Report Report
        {
            get { return (report.DataValid && report.Data.DynamicReport != null) ? report.Data.DynamicReport : report; }


        }
        internal ReportDrawStruct drawStruct = null;
        private Graphics Graphics
        {
            get { return Report.GraphicsBuffer; }

        }
        public DataTable ReportBodyData
        {
            get
            {
                return Report.Data.BodyData;
            }
        }
        public DataAnalyse(Report report)
        {
            this.report = report;
            report.DataChanged += new ReportDataChangedHandler(reportChanged);
            report.PropertyChanged += new ReportProeprtyChangedHandler(reportChanged);
        }
        void reportChanged(object sender, EventArgs e)
        {
            try
            {
                if (Report.DataValid)
                {
                    drawStruct = new ReportDrawStruct();
                    Analysing();
                    drawStruct.Invalid = true;
                }
                else
                {
                    drawStruct = new ReportDrawStruct();
                    drawStruct.Invalid = false;
                }
            }
            catch (DataAnaysisException analysingException)
            {
                drawStruct.Invalid = false;
                throw analysingException;
            }
            //catch (Exception unException)
            //{
            //    drawStruct.Invalid = false;
            //    throw unException;
            //}
        }
        public void Analysing()
        {
            try
            {
                drawStruct.EmptyRowHeight = Report.RowHeight;
                //整体信息初始化
                initWholeInfo();
                //drawStruct.ColumnInfo的初步初始化
                List<ReportColumn> usedRColumns = AnalyseColumnWidth();
                //drawStruct.ColumnInfo 横向分页，未超过横向分页则报错
                Dictionary<int, List<int>> pageContainCols = HorizontalTAB();
                //处理Text的定位问题 draw.exBodyData的表结构必已初始化
                InitTextsData(pageContainCols, usedRColumns);
                //处理体表行高 ，进行BodyCellStruct初始化

                InitBodyData();
                //总体分页
                InitWholePaging(pageContainCols);

                //分页后的页宏变理设置SetPageMacrs
                InitPageMacrs();
                drawStruct.ExcelSizeF = Report.GetExcelSizeF();

                report.Dispose();
            }
            catch (DataAnaysisException e)
            {
                Report.DataValid = false;
                Function.Alert(e.Message, "提示");
            }
        }

        private void InitPageMacrs()
        {
            int pageIndex = 1;
            foreach (PageInfo info in this.drawStruct.PageList)
            {
                info.SetPageMacrs(pageIndex++, drawStruct.PageCount);
            }
        }
        internal void initWholeInfo()
        {
            drawStruct.WholeInfo.ReportName = Report.DesrcName;
            drawStruct.WholeInfo.Top = Report.Attributes.Margin.Top;
            drawStruct.WholeInfo.Buttom = Report.Attributes.Margin.Bottom;
            drawStruct.WholeInfo.Left = Report.Attributes.Margin.Left;
            drawStruct.WholeInfo.Right = Report.Attributes.Margin.Right;
            drawStruct.WholeInfo.RowHeight = Report.Attributes.RowHeiht;
            drawStruct.WholeInfo.FontSize = Report.Attributes.Font.Size;
            //边框线宽未处理
            drawStruct.WholeInfo.GrainDirection = Report.Attributes.GrainDirection;
        }
        internal void InitWholePaging(Dictionary<int, List<int>> pageContainCols)
        {
            //计算ExBody的高度            
            if (Report.RptClass.TypeID == ReportClass.Grid.TypeID)
            {

                //if (exBodyHeight > Report.AreaHeight)
                //{
                //    throw new ExBodyHeight2BigException();
                //}
                List<PageInfo> pageInfoList = CalcGridPaging();
                //修正分页宏变量不正确的问题
                int dataPagingIndex = 0;
                foreach (PageInfo info in pageInfoList)
                {
                    info.PageMacrs = new Dictionary<string, string>();
                    Dictionary<string, string> tempMacrs = Report.Data.MacorsVerbCollection.GetPageMacrs(dataPagingIndex + 1);
                    foreach (KeyValuePair<string, string> entry in tempMacrs)
                    {
                        info.PageMacrs[entry.Key] = entry.Value;
                    }

                    if (info.IsDataPaging)
                    {
                        dataPagingIndex++;
                    }
                }
                //计算分页后每页的本页小计之类的函数
                CalcTextsFun(pageInfoList);
                PageInfo newpageInfo = null;
                //纵横向分页合并
                int flagPagingCount = 1;
                foreach (PageInfo info in pageInfoList)
                {
                    drawStruct.InitInfoRowHeight(info, flagPagingCount);
                    if (info.IsDataPaging)
                    { flagPagingCount++; }
                    if (pageContainCols.Count > 1)
                    {
                        int pageIndex = 1;
                        foreach (int key in pageContainCols.Keys)
                        {
                            newpageInfo = info.Clone();
                            newpageInfo.HpageIndex = pageIndex++;
                            newpageInfo.IsCrosswisePage = true;
                            newpageInfo.ContainCols.AddRange(pageContainCols[key]);
                            drawStruct.PageList.Add(newpageInfo);
                        }
                    }
                    else
                    {
                        drawStruct.VerifyPageColumn(info);
                        drawStruct.PageList.Add(info);
                    }
                }
            }
            else
            {
                if (pageContainCols.Count > 1)
                {
                    throw new ExBodyWidth2BigException();
                }
                //自由风格与大文本
                CalcFreePaging();
            }
        }

        private void CalcTextsFun(List<PageInfo> pageInfoList)
        {
            List<Text> listTexts = new List<Text>();
            List<Text> funTexts = Report.Texts.FunTexts();
            foreach (Text text in funTexts)
            {
                if (!ReportBodyData.Columns.Contains(text.FunColName))
                {
                    if (report.Columns[text.FunColName] != null && report.Columns[text.FunColName].Attibutes.PrintStyle.NoPrint)
                    {
                        listTexts.Add(text);
                    }
                    else
                    {
                        throw new FunSumColNotExistsException(text.FunColName);
                    }
                }
            }
            foreach (Text text in listTexts)
            {
                funTexts.Remove(text);
            }

            BodyDataStruct dataStruct = null;
            foreach (PageInfo info in pageInfoList)
            {
                foreach (Text text in funTexts)
                {
                    info.FunResults.Add(text.Context, "0");
                }
                for (int rowIndex = info.StartRowIndex; rowIndex < info.EndRowIndex; rowIndex++)
                {
                    foreach (Text text in funTexts)
                    {
                        dataStruct = ReportBodyData.Rows[rowIndex][text.FunColName] as BodyDataStruct;
                        if (dataStruct != null && dataStruct.IsFunCell)
                        {
                            info.FunResults[text.Context] = StringAdd(info.FunResults[text.Context], dataStruct.Context);
                        }
                    }
                }
            }
        }

        private string StringAdd(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return b;
            } if (string.IsNullOrEmpty(b))
            {
                return a;
            }
            return (Function.ToDouble(a) + Function.ToDouble(b)).ToString();
        }
        internal List<PageInfo> CalcGridPaging()
        {
            List<PageInfo> pageInfoList = new List<PageInfo>();
            //ConstantKey.XXXFLAG            
            int blockCount = 0; //打印在一起的行数

            int tempHeight = 0; //记录本页的行高
            int blockHeight = 0;//打印在一起的行 的行高
            int xxxFlag = 0;
            PageInfo info = new PageInfo();
            info.StartRowIndex = 0;
            int pageBottomRowCount = 0;
            //-1分页计数器
            int flagPagingCount = 1;
            int bodyHeight = report.AreaHeight - drawStruct.ExBodyHeight.GetTotalHeight(flagPagingCount);
            for (int rowIndex = 0; rowIndex < ReportBodyData.Rows.Count; rowIndex++)
            {
                //大于等于2000的粘连到一起，XXX_FLAG变小后重新计数
                if (Convert.ToInt32(ReportBodyData.Rows[rowIndex][ConstantKey.XXXFLAG]) < xxxFlag)
                {
                    blockCount = 0;
                }

                xxxFlag = Convert.ToInt32(ReportBodyData.Rows[rowIndex][ConstantKey.XXXFLAG]);
                if (info.EndRowIndex != 0 && xxxFlag < ConstantKey.TOGETHERDATAROW)
                {
                    if (tempHeight + blockHeight > bodyHeight)
                    {
                        info.TailPrintRowCount = pageBottomRowCount;
                        pageInfoList.Add(info);
                        pageBottomRowCount = 0;
                        info = new PageInfo();
                        info.StartRowIndex = pageInfoList[pageInfoList.Count - 1].EndRowIndex;
                        tempHeight = blockHeight;

                        if (tempHeight > bodyHeight)
                        {
                            throw new TogetherRowHeight2BigException();
                        }
                    }
                    else
                    {
                        info.EndRowIndex = 0;
                        tempHeight += blockHeight;
                    }
                }
                if (xxxFlag < ConstantKey.TOGETHERDATAROW)
                {
                    tempHeight += drawStruct.BodyRowHeightList[rowIndex];
                    if (tempHeight > bodyHeight)
                    {
                        //if (rowIndex < info.StartRowIndex + 2)
                        //{
                        //    throw new BodyRow2HighException();
                        //}
                        info.EndRowIndex = rowIndex;
                        info.TailPrintRowCount = pageBottomRowCount;
                        pageInfoList.Add(info);

                        info = new PageInfo();
                        pageBottomRowCount = 0;
                        info.StartRowIndex = rowIndex;
                        tempHeight = drawStruct.BodyRowHeightList[rowIndex];
                    }
                    if (ConstantKey.BOTTOMDATAROW <= xxxFlag && ConstantKey.TOGETHERDATAROW > xxxFlag)
                    {
                        //进入末尾行模式
                        pageBottomRowCount++;
                    }
                }
                else if (ConstantKey.TOGETHERDATAROW <= xxxFlag)
                {
                    if (blockHeight == 0)
                    {
                        info.EndRowIndex = rowIndex;
                    }
                    blockCount++;

                    //打印在一起的行
                    blockHeight += drawStruct.BodyRowHeightList[rowIndex];
                    //对打印在一起的行进行分页
                    if (blockHeight > bodyHeight)
                    {
                        info.EndRowIndex = rowIndex - blockCount + 1;
                        info.TailPrintRowCount = pageBottomRowCount;
                        pageInfoList.Add(info);

                        info = new PageInfo();
                        pageBottomRowCount = 0;
                        info.StartRowIndex = rowIndex - blockCount + 1;
                        tempHeight = drawStruct.BodyRowHeightList[rowIndex];

                        blockHeight = 0;
                        for (int i = rowIndex - blockCount; i < rowIndex; i++)
                        {
                            blockHeight += drawStruct.BodyRowHeightList[i];
                        }

                    }
                }
                if (xxxFlag == ConstantKey.PAGINGFLAGVAL)
                {
                    if (info.StartRowIndex == rowIndex)
                    {
                        continue;
                    }
                    //分页
                    info.EndRowIndex = rowIndex;
                    info.TailPrintRowCount = pageBottomRowCount;
                    info.EmptyRowCount = CalcEmptyRowCount(bodyHeight - tempHeight,
                        Report.Attributes.PrintStyle.IsBrifeStyle, pageBottomRowCount > 0);
                    info.IsDataPaging = true;
                    flagPagingCount++;
                    bodyHeight = report.AreaHeight - drawStruct.ExBodyHeight.GetTotalHeight(flagPagingCount);
                    pageInfoList.Add(info);
                    info = new PageInfo();
                    info.StartRowIndex = rowIndex + 1;
                    tempHeight = 0;
                    pageBottomRowCount = 0;
                }
            }
            if (pageInfoList.Count == 0)
            {
                bodyHeight = report.AreaHeight - drawStruct.ExBodyHeight.GetTotalHeight(flagPagingCount);
                info.EmptyRowCount = CalcEmptyRowCount(bodyHeight, Report.Attributes.PrintStyle.IsBrifeStyle, false);
                //处理表体数据为空的情况
                pageInfoList.Add(info);
            }
            return pageInfoList;
        }
        private int CalcEmptyRowCount(int emptyHeight, bool isBrifeStyle, bool hasBottomRow)
        {
            bool isNeedEmptyRow = (!isBrifeStyle || hasBottomRow);
            //空行的标准行高3rh/2	
            int emptyRowCount = 0;
            if (isNeedEmptyRow)
            {
                emptyRowCount = emptyHeight / Report.RowHeight;
            }
            return emptyRowCount;
        }

        private void CalcFreePaging()
        {
            //自由风格报表不存在多页宏变量的情况
            int exBodyHeight = drawStruct.ExBodyHeight.GetTotalHeight(0);
            if (exBodyHeight < Report.AreaHeight)
            {
                //自动加长所有行，使其占满整个报表
                PageInfo pageInfo = new PageInfo();
                pageInfo.StartRowIndex = 0;
                pageInfo.EndRowIndex = drawStruct.ExBodyHeight.Count;
                drawStruct.ExBodyHeight.Init(pageInfo.PageRowHeight);
                //pageInfo.PageRowHeight.AddRange(drawStruct.ExBodyHeight);
                drawStruct.PageList.Add(pageInfo);
                drawStruct.VerifyPageColumn(pageInfo);
                pageInfo.PageMacrs = Report.Data.MacorsVerbCollection.GetPageMacrs(1);
            }
            else if (report.RptClass == ReportClass.BigText)
            {
                //大文本字段判定标识
                //int TextMaxRowHeight = 100;
                //bool bigTextFlag = false;
                //bigTextFlag = drawStruct.ExBodyHeight.BigHeightRow(TextMaxRowHeight);
                //if (bigTextFlag)
                //{
                BigTextAnalyse bTextAnalyse = new BigTextAnalyse(drawStruct, Report, this.Graphics);
                bTextAnalyse.Analyse();
                //}
            }
            else
            {
                //[0,2) 只算0 1两行
                PageInfo pageInfo = new PageInfo();
                pageInfo.StartRowIndex = 0;
                int tempheight = 0;
                int rowIndex = 1;
                int pageIndex = 1;

                for (int i = 0; i < drawStruct.ExBodyHeight.Count; i++)
                {
                    int rowHeight = drawStruct.ExBodyHeight.GetHeight(i);
                    tempheight += rowHeight;

                    if (tempheight > Report.AreaHeight)
                    {
                        pageInfo.EndRowIndex = rowIndex;
                        pageInfo.PageMacrs = Report.Data.MacorsVerbCollection.GetPageMacrs(pageIndex);
                        drawStruct.PageList.Add(pageInfo);
                        pageIndex++;
                        drawStruct.VerifyPageColumn(pageInfo);
                        tempheight = rowHeight;
                        pageInfo = new PageInfo();
                        pageInfo.StartRowIndex = rowIndex;
                    }
                    rowIndex++;
                    pageInfo.PageRowHeight.Add(rowHeight);

                }

                pageInfo.EndRowIndex = drawStruct.ExBodyHeight.Count;
                pageInfo.PageMacrs = Report.Data.MacorsVerbCollection.GetPageMacrs(pageIndex);
                drawStruct.PageList.Add(pageInfo);
                drawStruct.VerifyPageColumn(pageInfo);
                if (report.RptClass != ReportClass.BigText && drawStruct.PageList.Count > 1)
                {
                    throw new SinglePageException();
                }
            }
        }
        internal void InitBodyData()
        {
            for (int rowIndex = 0; rowIndex < ReportBodyData.Rows.Count; rowIndex++)
            {
                drawStruct.AddBodyDataRow(ReportBodyData.Rows[rowIndex][ConstantKey.XXXFLAG], Report.RowHeight);
                for (int columnIndex = 0; columnIndex < ReportBodyData.Columns.Count - 1; columnIndex++)
                {
                    int rowHeight = DealBodyRowHeight(rowIndex, columnIndex, ReportBodyData.Rows[rowIndex][columnIndex] as BodyDataStruct);
                    drawStruct.AddBody(
                        NewCellStruct(ReportBodyData.Rows[rowIndex][columnIndex] as BodyDataStruct,
                         drawStruct.ColumnInfo[columnIndex + 1].DrawInfo), rowIndex, columnIndex, rowHeight
                         );
                }
            }
        }

        internal int DealBodyRowHeight(int rowIndex, int columnIndex, BodyDataStruct bodyDataStruct)
        {
            int rowHeight = drawStruct.GetBodyRowHeight(rowIndex);
            if (null == bodyDataStruct || bodyDataStruct == BodyDataStruct.Empty)
            {
                return rowHeight;
            }

            int colWidth = drawStruct.GetColWidth(columnIndex, bodyDataStruct.ColMergeInfo.MergeCount);
            Font font = drawStruct.ColumnInfo[columnIndex + 1].DrawInfo.Font;
            if (bodyDataStruct.ColMergeInfo.Font != null)
            {
                font = bodyDataStruct.ColMergeInfo.Font;
            }
            if (font == null)
            { }

            SizeF sizef = Graphics.MeasureString(bodyDataStruct.Context, font, colWidth);

            int tempRowHeight = drawStruct.GetBodyRowHeight(rowIndex, bodyDataStruct.RowMergeInfo.RowMergeCount, Report.RowHeight);

            if (sizef.Height > tempRowHeight)
            {
                //cjl 20120514 考虑小数位数丢失问题，进一法
                rowHeight += Convert.ToInt32(Math.Ceiling(sizef.Height)) - tempRowHeight + 3;//出现显示不完整，多添加3个单位
            }
            return rowHeight;
        }

        internal void InitTextsData(Dictionary<int, List<int>> hPaging, List<ReportColumn> usedRColumns)
        {
            int[] bandsID = new int[] { Bands.HeadBandID, Bands.TitleBandID, Bands.PageBandID, Bands.RootBandID };
            foreach (int bandID in bandsID)
            {
                for (int rowIndex = 0; rowIndex < Report.Bands[bandID]; rowIndex++)
                {
                    drawStruct.AddExBodyDataRow(bandID, Report.RowHeight, null);
                }
            }
            reTitleAndPage(hPaging, usedRColumns);
            reHeadAndFoot(hPaging);
        }

        private void reTitleAndPage(Dictionary<int, List<int>> hPaging, List<ReportColumn> usedRcolumns)
        {
            int[] usedColumnsMap = null;

            List<int> bands = new List<int>(new int[] { Bands.TitleBandID, Bands.PageBandID });
            usedColumnsMap = new int[Report.Columns.Count + 1];
            usedColumnsMap.Initialize();
            for (int i = 1; i < usedColumnsMap.Length; i++)
            {
                if (usedRcolumns.Contains(Report.Columns[i - 1]))
                {
                    usedColumnsMap[i] = 1;
                }
            }

            RptTextModule textModule = new RptTextModule(Report, bands, usedColumnsMap);
            textModule.MergeTextSize(Graphics, Report.Data.MacorsVerbCollection);
            int[] destColumnWidth = drawStruct.GetColumnsWidth(null);

            SetTextsToExBody(0, 0, textModule.Texts, hPaging);
        }

        private void reHeadAndFoot(Dictionary<int, List<int>> hPaging)
        {
            if (hPaging.Count > 1)
            {
                int pageColumnCount = 0;
                foreach (int pageIndex in hPaging.Keys)
                {
                    int[] destColumnWidth = drawStruct.GetColumnsWidth(hPaging[pageIndex]);
                    //drawStruct.exBodyHeigth
                    ReMapTextLocation(pageColumnCount, destColumnWidth, pageIndex);
                    pageColumnCount += hPaging[pageIndex].Count;
                }
            }
            else
            {
                int[] destColumnWidth = drawStruct.GetColumnsWidth(null);
                ReMapTextLocation(0, destColumnWidth, 0);
            }
        }
        private void ReMapTextLocation(int pageColumnCount, int[] destColumnWidth, int pageIndex)
        {
            RptTextModule textModule = new RptTextModule(Report, new List<int>(new int[] { Bands.HeadBandID, Bands.RootBandID }));
            //依据实际内容计算TextBlock所点百分比
            textModule.MergeTextSize(Graphics, Report.Data.MacorsVerbCollection);
            SetTextsToExBody(pageIndex, pageColumnCount, textModule.MapNewText(destColumnWidth), null);
        }
        private void SetTextsToExBody(int pageIndex, int pageColumnCount, Texts texts, Dictionary<int, List<int>> hPaging)
        {
            DrawInfo drawInfo = null;
            CellStruct cellStruct = null;
            Location location = null;

            foreach (Text text in texts)
            {
                if (text.IsEcho)
                {
                    //Fixed DEBUG: 再对标题、表头等区域进行块重排时，会复制text，导致Report属性丢失,
                    //继而丢失设计状态的行显隐。出现行定位混乱.
                    if (text.Report == null)
                        text.Report = this.report;
                    location = text.Location;
                    if (pageColumnCount > 0)
                    {
                        if (location.X1 > Report.Attributes.Pagination)
                        {
                            //X 平移 pageColumnCount - Pagination * (pageIndex- 1) + X
                            int offset = pageColumnCount - Report.Attributes.Pagination * (pageIndex - 1);
                            location.SetLocationX(location.X1 + offset,
                                location.X2 + offset);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    int rowIndex = Report.Bands.GetExBodyRowIndex(text.BandIndex, text.OriginalLocation.Y1);

                    drawInfo = new DrawInfo(text.Attribute.Font, text.Attribute.ForeBrush, text.Attribute.Valign);

                    if (text.IsFormula)
                    {
                        cellStruct = new FunCellStruct(text.Context, drawInfo, new ExcelInfo(location.Y2 - location.Y1 + 1
                         , location.X2 - location.X1 + 1));
                    }
                    else if (text.IsIncludeMacros)
                    {
                        cellStruct = new MacrsCellStruct(text.MContext, drawInfo, new ExcelInfo(location.Y2 - location.Y1 + 1
                                                , location.X2 - location.X1 + 1));
                    }
                    else
                    {
                        cellStruct = new CellStruct(text.Context, drawInfo, new ExcelInfo(location.Y2 - location.Y1 + 1
                            , location.X2 - location.X1 + 1));
                    }

                    try
                    {
                        cellStruct.DrawInfo.BoundaryLine = text.Attribute.BoundaryLine;
                        //设置特殊线的属性
                        switch (text.Attribute.Diagonal)
                        {
                            case 1:
                                cellStruct.DrawInfo.BoundaryLine.IsSlash = true;
                                break;
                            case 2:
                                cellStruct.DrawInfo.BoundaryLine.IsBackSlash = true;
                                break;
                            case 3:
                                cellStruct.DrawInfo.BoundaryLine.IsCrossLine = true;
                                break;
                            default:
                                drawInfo.BoundaryLine.IsSlash = false;
                                drawInfo.BoundaryLine.IsBackSlash = false;
                                drawInfo.BoundaryLine.IsCrossLine = false;
                                break;
                        }
                        if (report.Data.MacorsVerbCollection.HasMorePages(text.MacrosArgsName))
                        {
                            MacrosVerbCollection macrosVerbCollection = report.Data.MacorsVerbCollection.SubVerbCollection(text.MacrosArgsName);
                            Dictionary<int, int> textFirstRowHeightIDict = DealTextRowHeight(text, drawStruct.ColumnInfo, location, macrosVerbCollection);
                            drawStruct.AddExBody(cellStruct, rowIndex - 1, location.X1 - 1, textFirstRowHeightIDict);
                        }
                        else
                        {
                            //处理行高问题
                            int textFirstRowHeight = DealTextRowHeight(text, drawStruct.ColumnInfo, location);
                            if (report.Attributes.Pagination > 0)
                            {
                                int pagingIndex = getBetweenPagingIndex(location.X1, location.X2, hPaging);
                                if (pagingIndex > 0)
                                {
                                    cellStruct.ExcelInfo.ColMerge = pagingIndex - location.X1 + 1;
                                    CellStruct cellStuctClone = cellStruct.Clone();
                                    cellStuctClone.ExcelInfo.ColMerge = location.X2 - pagingIndex;
                                    drawStruct.AddExBody(cellStuctClone, rowIndex - 1, pagingIndex, textFirstRowHeight);
                                }
                            }

                            drawStruct.AddExBody(cellStruct, rowIndex - 1, location.X1 - 1, textFirstRowHeight);
                        }
                    }
                    catch (Exception e)
                    {
                        Function.Alert("初始化非表体区数据时出错!错误信息：" + e, "提示");
                    }
                }
            }
        }
        private int getBetweenPagingIndex(int x1, int x2, Dictionary<int, List<int>> dicPaging)
        {
            int pagingIndex = -1;
            if (dicPaging != null)
            {
                List<int> listMaxIndex = new List<int>();
                foreach (KeyValuePair<int, List<int>> pair in dicPaging)
                {
                    listMaxIndex.Add(pair.Value[pair.Value.Count - 1]);
                }
                foreach (int index in listMaxIndex)
                {
                    if (x1 <= index && x2 > index)
                        pagingIndex = index;
                }
            }
            return pagingIndex;
        }
        internal int DealTextRowHeight(Text text, Dictionary<int, ColumnInfo> dictionary, Location location)
        {
            int columnWidth = 0;
            for (int columnIndex = location.X1; columnIndex < location.X2 + 1; columnIndex++)
            {
                columnWidth += dictionary[columnIndex].Width;
            }
            SizeF sizeF = DrawUtil.MeasureString(text.Context, text.Attribute.Font, columnWidth, Graphics, Report.Attributes.RowHeiht);
            //FIXED BUG: 进行行高度扩展时，报表的Text在获取高设时需要装Location映射到整体行数上，以防出现高
            //高度重复累计的情况
            int startRowIndex = Report.Bands.GetExBodyRowIndex(text.BandIndex, location.Y1);
            int endRowIndex = Report.Bands.GetExBodyRowIndex(text.BandIndex, location.Y2);
            int tempHeight = drawStruct.GetExRowHeight(startRowIndex - 1, endRowIndex);
            int firstRowHeight = drawStruct.GetExRowHeight(startRowIndex - 1);
            //页汇$fun:sum:  $ 不参与计算高度
            if (sizeF.Height > firstRowHeight && sizeF.Height > tempHeight && !Regex.IsMatch(text.Context, @"\$.*\$"))
            {
                firstRowHeight += Convert.ToInt32(Math.Ceiling(sizeF.Height)) - tempHeight + 3;
            }
            return firstRowHeight;
        }
        internal Dictionary<int, int> DealTextRowHeight(Text text, Dictionary<int, ColumnInfo> dictionary, Location location, MacrosVerbCollection verbs)
        {
            int columnWidth = 0;
            for (int columnIndex = location.X1; columnIndex < location.X2 + 1; columnIndex++)
            {
                columnWidth += dictionary[columnIndex].Width;
            }
            Dictionary<int, int> mutliMacrasRowHeight = new Dictionary<int, int>();
            foreach (KeyType entry in verbs.Keys)
            {
                if (text.IsIncludeMacros)
                {
                    List<string> list = new List<string>();
                    foreach (string s in text.MacrosArgsName)
                    {
                        list.Add(verbs[s, entry.Value]);
                    }
                    text.MacrosContext = list;
                }
                SizeF sizeF = DrawUtil.MeasureString(text.Context, text.Attribute.Font, columnWidth, Graphics, Report.Attributes.RowHeiht);
                int tempHeight = drawStruct.GetExRowHeight(location.Y1 - 1, location.Y2);
                int rowIndex = Report.Bands.GetExBodyRowIndex(text.BandIndex, location.Y1);
                int firstRowHeight = drawStruct.GetExRowHeight(rowIndex - 1);
                if (sizeF.Height > firstRowHeight && sizeF.Height > tempHeight)
                {
                    firstRowHeight += Convert.ToInt32(Math.Ceiling(sizeF.Height)) - tempHeight + 3;
                }
                mutliMacrasRowHeight[entry.Value] = firstRowHeight;
            }
            return mutliMacrasRowHeight;
        }
        /// <summary>
        /// 利用已有的Column数据处理横向分页的问题
        /// </summary>
        internal Dictionary<int, List<int>> HorizontalTAB()
        {
            if (Report.Attributes.Pagination >= drawStruct.ColumnInfo.Count)
            {
                throw new PaginationCountInvalidException();
            }
            Dictionary<int, List<int>> pageContainCols = new Dictionary<int, List<int>>();
            if (Report.Attributes.Pagination >= 0)
            {
                List<int> fixedColumn = new List<int>();
                int fixedColumnWidth = 0;
                for (int columnIndex = 1; columnIndex <= Report.Attributes.Pagination; columnIndex++)
                {
                    fixedColumn.Add(columnIndex);
                    fixedColumnWidth += drawStruct.ColumnInfo[columnIndex].Width;
                    drawStruct.ColumnInfo[columnIndex].FixedColumn = true;
                }
                int pageTempWidth = fixedColumnWidth;
                int pageIndex = 1;
                pageContainCols.Add(pageIndex, new List<int>());
                pageContainCols[pageIndex].AddRange(fixedColumn);

                for (int columnIndex = 1; columnIndex <= drawStruct.ColumnInfo.Count; columnIndex++)
                {
                    if (fixedColumn.Contains(columnIndex))
                    {
                        continue;
                    }

                    if (pageTempWidth + drawStruct.ColumnInfo[columnIndex].Width > Report.AreaWidth)
                    {
                        pageIndex++;
                        pageContainCols.Add(pageIndex, new List<int>());
                        pageContainCols[pageIndex].AddRange(fixedColumn);
                        pageContainCols[pageIndex].Add(columnIndex);
                        pageTempWidth = fixedColumnWidth + drawStruct.ColumnInfo[columnIndex].Width;
                        if (pageTempWidth > Report.AreaWidth)
                        {
                            throw new FixedColumnWidthToBigException();
                        }
                    }
                    else
                    {
                        pageContainCols[pageIndex].Add(columnIndex);
                        pageTempWidth += drawStruct.ColumnInfo[columnIndex].Width;
                    }
                }
                int pIndex = 1;
                //每一页宽度回写
                foreach (int pageKey in pageContainCols.Keys)
                {
                    allotWidth(pIndex, drawStruct.ColumnInfo, pageContainCols[pageKey].ToArray(), pageContainCols[pageKey].ToArray());
                    pIndex++;
                }
            }
            return pageContainCols;
        }
        internal List<ReportColumn> AnalyseColumnWidth()
        {
            //判断报表的有效性
            bool reportValid = Report.DataValid && Report.PropertyValid;
            if (!reportValid)
            {
                return null;
            }
            //step 1:列宽初始化[依据BodyData初始化具体的行数]
            ReportColumn reportColumn = null;
            List<int> autoWidth = new List<int>();
            List<int> autoWrapAndWidth = new List<int>();
            int columnIndex = 1;
            List<ReportColumn> usedReportColumn = new List<ReportColumn>();
            foreach (DataColumn column in ReportBodyData.Columns)
            {
                if (column.DataType == typeof(BodyDataStruct))
                {
                    string baseColumnName = column.ColumnName;
                    //if (Regex.IsMatch(baseColumnName, @"\w+_\d+"))
                    //{
                    //    //扩展列对应处理
                    //    reportColumn = Report.Columns.Find(Regex.Match(baseColumnName,
                    //        @"\w+(?=_\d+)").Value);
                    //}
                    //else
                    //{
                    //    //找到相应的列
                    //    reportColumn = Report.Columns.Find(column.ColumnName);
                    //    usedReportColumn.Add(reportColumn);
                    //}

                    reportColumn = Report.Columns.Find(column.ColumnName);
                    usedReportColumn.Add(reportColumn);
                    if (reportColumn == null)
                    {
                        throw new BodyColumnInvalidException();
                    }
                    //临时记录列宽度是否可扩展
                    if (!reportColumn.Attibutes.AutoWrap && reportColumn.Attibutes.IsAdjustWeith)
                        autoWidth.Add(columnIndex);
                    else if (reportColumn.Attibutes.AutoWrap && reportColumn.Attibutes.IsAdjustWeith)
                        autoWrapAndWidth.Add(columnIndex);

                    ColumnInfo columnInfo = new ColumnInfo();
                    columnInfo.Width = reportColumn.ColumnWidth;

                    columnInfo.DrawInfo = new DrawInfo(reportColumn.Attibutes.Font,
                        reportColumn.Attibutes.ForeBrush, reportColumn.Attibutes.Valign);

                    columnInfo.DrawInfo.BoundaryLine = reportColumn.Attibutes.BoundaryLine;

                    drawStruct.ColumnInfo.Add(columnIndex++, columnInfo);
                }
            }

            //初始化可画表结构的Table列
            drawStruct.InitTableColumn();
            //遍历数据，初始化具体的列宽,记录数据CELL在列宽分配后还需要进行高度扩展的                                    
            foreach (int index in autoWidth)
            {
                foreach (DataRow row in ReportBodyData.Rows)
                {
                    int intMax = int.MaxValue;
                    if (row[index - 1] == null || row[index] == BodyDataStruct.Empty || row[index - 1] == System.DBNull.Value)
                    {
                        continue;
                    }
                    columnWidthAllot(index, drawStruct.ColumnInfo, row[index - 1] as BodyDataStruct, ref intMax);
                }
            }
            //计算剩于宽度,对可自宽自折的列进行宽度处理,对于标明横向分页的表
            int tempTotalWidth = 0;
            foreach (int index in drawStruct.ColumnInfo.Keys)
            {
                tempTotalWidth += drawStruct.ColumnInfo[index].Width;
            }
            tempTotalWidth = Report.AreaWidth - tempTotalWidth;

            foreach (int index in autoWrapAndWidth)
            {
                foreach (DataRow row in ReportBodyData.Rows)
                {
                    if (row[index - 1] == null || !BodyDataStruct.IsEmptyOrNull(row[index - 1]))
                    {
                        continue;
                    }
                    columnWidthAllot(index, drawStruct.ColumnInfo, row[index - 1] as BodyDataStruct, ref tempTotalWidth);
                }
            }
            if (tempTotalWidth > 0)
            {
                //还有剩于宽度
                int[] autoColumnArray = new int[autoWidth.Count + autoWrapAndWidth.Count];

                autoWidth.CopyTo(autoColumnArray);
                autoWrapAndWidth.CopyTo(0, autoColumnArray, autoWidth.Count, autoWrapAndWidth.Count);
                int[] columnArray = new int[drawStruct.ColumnInfo.Count];
                drawStruct.ColumnInfo.Keys.CopyTo(columnArray, 0);
                if (autoColumnArray.Length != 0)
                {
                    allotWidth(1, drawStruct.ColumnInfo, columnArray, autoColumnArray);
                }
                else
                {
                    allotWidth(1, drawStruct.ColumnInfo, columnArray, columnArray);
                }

            }

            return usedReportColumn;
        }
        internal void allotWidth(int pageIndex, Dictionary<int, ColumnInfo> dictionary, int[] columnArray, int[] autoColumnArray)
        {
            int totolWidth = 0;
            int autoTotalWidth = 0;
            int[] columnWidth = new int[columnArray.Length];
            int index = 0;
            foreach (int columnIndex in columnArray)
            {
                totolWidth += dictionary[columnIndex].Width;
                columnWidth[index] = dictionary[columnIndex].Width;
                index++;
                if (ArrayContains(autoColumnArray, columnIndex))
                {
                    autoTotalWidth += dictionary[columnIndex].Width;
                }
            }

            if (totolWidth < Report.AreaWidth)
            {
                double[] colnumsWidthd = new double[columnArray.Length];

                for (index = 0; index < columnArray.Length; index++)
                {
                    colnumsWidthd[index] = columnWidth[index];
                    if (ArrayContains(autoColumnArray, columnArray[index]))
                    {
                        colnumsWidthd[index] += (columnWidth[index] / (float)autoTotalWidth) * (Report.AreaWidth - totolWidth - 1);
                    }
                }
                DrawUtil.ExpandLenghtCala(colnumsWidthd, ref columnWidth);

                for (index = 0; index < columnArray.Length; index++)
                {
                    if (pageIndex > 1 && dictionary[columnArray[index]].FixedColumn)
                    {
                        dictionary[columnArray[index]].FixedColumnWidth.Add(pageIndex, columnWidth[index]);
                    }
                    else
                    {
                        dictionary[columnArray[index]].Width = columnWidth[index];
                    }
                }
            }
        }
        internal bool ArrayContains(int[] array, int key)
        {
            bool contains = false;
            for (int index = 0; index < array.Length; index++)
            {
                if (key == array[index])
                {
                    contains = true;
                    break;
                }
            }
            return contains;
        }
        internal void columnWidthAllot(int columnName, Dictionary<int, ColumnInfo> dictionary, BodyDataStruct bodyDataStruct, ref int allotWidth)
        {
            int width = 0;
            for (int columnIndex = columnName; columnIndex - columnName < bodyDataStruct.ColMergeInfo.MergeCount; columnIndex++)
            {
                width += dictionary[columnIndex].Width;
            }
            SizeF sizef = DrawUtil.MeasureString(bodyDataStruct.Context, dictionary[columnName].DrawInfo.Font, Graphics, Report.Attributes.RowHeiht);
            if (sizef.Width > width)
            {
                if (allotWidth > sizef.Width - width)
                {
                    allotWidth = allotWidth - (Convert.ToInt32(Math.Ceiling(sizef.Width)) - width);
                    //可用分配宽度还有够
                    dictionary[columnName].Width += (Convert.ToInt32(Math.Ceiling(sizef.Width)) - width);
                }
                else if (allotWidth > 0)
                {
                    //可用分配宽度不够整列进行扩展但还有一些剩
                    dictionary[columnName].Width += allotWidth;
                    allotWidth = 0;
                }
                //可用分配宽度无，则直接使用相应的设计宽度
            }
        }
        internal CellStruct NewCellStruct(BodyDataStruct bodyDataStruct, DrawInfo columnDrawInfo)
        {
            if (bodyDataStruct == BodyDataStruct.Empty || bodyDataStruct == null)
            {
                return CellStruct.Empty;
            }
            CellStruct cellStruct = new CellStruct(bodyDataStruct.Context, null, new ExcelInfo(bodyDataStruct.RowMergeInfo.RowMergeCount,
                bodyDataStruct.ColMergeInfo.MergeCount));
            cellStruct.DrawInfo = columnDrawInfo.Clone();
            cellStruct.DrawInfo.Format.Alignment = bodyDataStruct.ColMergeInfo.Alignment;
            cellStruct.DrawInfo.Format.LineAlignment = bodyDataStruct.ColMergeInfo.LineAlignment;
            cellStruct.DrawInfo.BoundaryLine = bodyDataStruct.RowMergeInfo.BoundaryLine;
            if (bodyDataStruct.ColMergeInfo.Font != null)
            {
                cellStruct.DrawInfo.Font = bodyDataStruct.ColMergeInfo.Font;
            }
            return cellStruct;
        }

        #region 分析状态保存
        ReportDrawStruct saveState = null;
        public void Save()
        {
            saveState = this.drawStruct;
        }
        public void Cover()
        {
            this.drawStruct = saveState;
        }
        #endregion

        public void Dispose()
        {
            if (report != null)
                report.Dispose();
        }
    }
}

#if UNITTEST
namespace UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using Com.Bing.Report;
    using System.IO;
    [TestFixture]
    public class DataAnalyseTest
    {
        string rptPath = string.Empty;
        Report Report = new Report();
        DataAnalyse dataAnalyse = null;
        [SetUp]
        public void SetUp()
        {
            rptPath = Environment.CurrentDirectory + @"\Data\report_分部分项工程量清单项目综合单价计算表.rpt";
            Report.ReadPRT(rptPath);
            //Report 数据数据列                        
            dataAnalyse = new DataAnalyse(Report);
        }

        private static ReportData InitData()
        {
            string[] columnList = new string[] { "no", "mark", "name", "type_1", "type_2", "type_3", "unitprice", "amount", "price", "mome" };
            ReportData reportData = new ReportData();
            reportData.InitTableColumn(columnList);
            reportData.BodyData.Rows.Add(new object[] { new BodyDataStruct("1"),
                new BodyDataStruct("第一章",1,2),
                new BodyDataStruct("xxxxxxxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"),
                new BodyDataStruct("axxxxxxxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",3,2),
                BodyDataStruct.Empty,
                BodyDataStruct.Empty,
                new BodyDataStruct("100.36"),
                new BodyDataStruct("200"),
                new BodyDataStruct("20072"),
                new BodyDataStruct("纯属假造纯属假造纯属假造纯属假造纯属假造"),
                1
                });
            reportData.BodyData.Rows.Add(new object[] { new BodyDataStruct("1.1"),
                new BodyDataStruct("第一章",1,2),
                new BodyDataStruct("xxxxxxxXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"),
                BodyDataStruct.Empty,
                BodyDataStruct.Empty,
                BodyDataStruct.Empty,
                new BodyDataStruct("x100.36"),
                new BodyDataStruct("200x"),
                new BodyDataStruct("20072xxxxx"),
                new BodyDataStruct("纯属假造纯属假造纯属假造纯属假造纯属假造"),
                2
                });
            return reportData;
        }

        private void InitReportColumn()
        {
            ReportColumn col = new ReportColumn("no");
            col.ColumnWidth = 40;
            Report.Columns.Add(col);
            col = new ReportColumn("mark");
            col.ColumnWidth = 100;
            Report.Columns.Add(col);
            col = new ReportColumn("name");
            col.Attibutes.IsAdjustWeith = true;
            col.ColumnWidth = 200;
            Report.Columns.Add(col);

            col = new ReportColumn("type");
            col.Attibutes.DynamicColumn = true;
            col.ColumnWidth = 50;
            Report.Columns.Add(col);

            col.ColumnWidth = 50;
            col = new ReportColumn("unitprice");
            Report.Columns.Add(col);

            col = new ReportColumn("amount");
            col.ColumnWidth = 50;
            Report.Columns.Add(col);

            col = new ReportColumn("price");
            col.ColumnWidth = 50;
            Report.Columns.Add(col);

            col = new ReportColumn("mome");
            col.ColumnWidth = 150;
            col.Attibutes.IsAdjustWeith = true;
            Report.Columns.Add(col);
        }
        [Test]
        public void initWholeInfo()
        {
            dataAnalyse.initWholeInfo();
        }
        [Test]
        [ExpectedException(typeof(PaginationCountInvalidException))]
        public void HorizontalTAB1()
        {
            Report.Attributes.Pagination = 1;
            dataAnalyse.drawStruct.ColumnInfo.Add(1, new ColumnInfo(10));
            dataAnalyse.HorizontalTAB();//throw PaginationCountInvalidException
        }
        [Test]
        public void HorizontalTAB2()
        {
            Report.Attributes.Pagination = 2;
            dataAnalyse.drawStruct.ColumnInfo.Add(1, new ColumnInfo(110));
            dataAnalyse.drawStruct.ColumnInfo.Add(2, new ColumnInfo(210));
            dataAnalyse.drawStruct.ColumnInfo.Add(3, new ColumnInfo(310));
            dataAnalyse.drawStruct.ColumnInfo.Add(4, new ColumnInfo(120));
            dataAnalyse.drawStruct.ColumnInfo.Add(5, new ColumnInfo(130));
            dataAnalyse.drawStruct.ColumnInfo.Add(6, new ColumnInfo(140));
            dataAnalyse.drawStruct.ColumnInfo.Add(7, new ColumnInfo(240));


            Dictionary<int, List<int>> pages = dataAnalyse.HorizontalTAB();

            Assert.AreEqual(pages.Count, 2);
        }
        [Test]
        public void HorizontalTAB3()
        {
            Report.Attributes.Pagination = 0;
            dataAnalyse.drawStruct.ColumnInfo.Add(1, new ColumnInfo(110));
            dataAnalyse.drawStruct.ColumnInfo.Add(2, new ColumnInfo(210));
            dataAnalyse.drawStruct.ColumnInfo.Add(3, new ColumnInfo(310));
            dataAnalyse.drawStruct.ColumnInfo.Add(4, new ColumnInfo(120));
            dataAnalyse.drawStruct.ColumnInfo.Add(5, new ColumnInfo(130));
            dataAnalyse.drawStruct.ColumnInfo.Add(6, new ColumnInfo(140));
            dataAnalyse.drawStruct.ColumnInfo.Add(7, new ColumnInfo(240));
            Dictionary<int, List<int>> pages = dataAnalyse.HorizontalTAB();
            Assert.AreEqual(pages.Count, 2);
        }
        [Test]
        public void allotWidth()
        {
            Dictionary<int, ColumnInfo> columnDic = new Dictionary<int, ColumnInfo>();
            columnDic.Add(1, new ColumnInfo(110));
            columnDic.Add(2, new ColumnInfo(210));
            columnDic.Add(3, new ColumnInfo(310));
            columnDic.Add(4, new ColumnInfo(120));
            columnDic.Add(5, new ColumnInfo(130));
            columnDic.Add(6, new ColumnInfo(140));
            columnDic.Add(7, new ColumnInfo(240));
            dataAnalyse.allotWidth(columnDic, new int[] { 1, 2, 3, 4, 5 }, new int[] { 1, 2 });
            Assert.AreEqual(allotWidthValite(columnDic, new int[] { 1, 2, 3, 4, 5 }), Report.AreaWidth - 1);
            dataAnalyse.allotWidth(columnDic, new int[] { 1, 2, 6, 7 }, new int[] { 6 });
            Assert.AreEqual(allotWidthValite(columnDic, new int[] { 1, 2, 6, 7 }), Report.AreaWidth - 1);
        }
        int allotWidthValite(Dictionary<int, ColumnInfo> columnDic, int[] columns)
        {
            int width = 0;
            foreach (int col in columns)
            {
                width += columnDic[col].Width;
            }
            return width;
        }
        [Test]
        public void columnWidthAllot1()
        {
            Dictionary<int, ColumnInfo> columnDic = new Dictionary<int, ColumnInfo>();

            columnDic.Add(1, new ColumnInfo(33));
            columnDic[1].DrawInfo = new DrawInfo();
            columnDic[1].DrawInfo.Font = new Font("宋体", 9.0f);
            columnDic.Add(2, new ColumnInfo(33));

            BodyDataStruct dds = new BodyDataStruct();
            dds.Context = "XXXXXXXXXXXXXXXXXXXX";//~128
            dds.ColMergeInfo.MergeCount = 2;
            
            int width = 100;
            dataAnalyse.columnWidthAllot(1, columnDic, dds, ref width);
            
            Assert.AreEqual(columnDic[1].Width, 95);
            Assert.AreEqual(width, 100 - (columnDic[1].Width - columnDic[2].Width));
        }
        [Test]
        public void columnWidthAllot2()
        {
            Dictionary<int, ColumnInfo> columnDic = new Dictionary<int, ColumnInfo>();
            columnDic.Add(1, new ColumnInfo(33));
            columnDic[1].DrawInfo = new DrawInfo();
            columnDic[1].DrawInfo.Font = new Font("宋体", 9.0f);
            columnDic.Add(2, new ColumnInfo(33));
            BodyDataStruct dds = new BodyDataStruct();
            dds.Context = "XXXXXXXXXXXXXXXXXXXX";//~128
            dds.ColMergeInfo.MergeCount = 2;
            int width = 30;
            dataAnalyse.columnWidthAllot(1, columnDic, dds, ref width);
            Assert.AreEqual(columnDic[1].Width, 96);
            Assert.AreEqual(width, 0);
        }
        [Test]
        public void NewCellStruct()
        {
            BodyDataStruct dataStruct = new BodyDataStruct();
            DrawInfo drawInfo = new DrawInfo();
            dataAnalyse.NewCellStruct(dataStruct, drawInfo);
        }
        [Test]
        public void AnalyseColumnWidth()
        {
            InitReportColumn();
            ReportData reportData = InitData();
            MutliProjectManager projectManager = new MutliProjectManager();
            projectManager.ProjectData = reportData;

            Report.InitReportData(projectManager,-1);

            dataAnalyse = new DataAnalyse(Report);            
            dataAnalyse.AnalyseColumnWidth();
            
        }
        [Test]
        public void CalcGridPaging()
        {
            ReportData reportData = new ReportData();
            reportData.InitTableColumn(new string[0]);
            //Report.AreaHeight
            //dataAnalyse.drawStruct.ColumnInfo
            //dataAnalyse.drawStruct.BodyRowHeightList            
        }
    }
}
#endif