using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;

namespace Com.Bing.Report
{
    public class PageInfo
    {
        //需要包含 BODY起讫行[startRowIndex,endRowIndex) 尾行Index 尾行数 空白行数
        private List<int> pageRowHeight = new List<int>();
        public List<int> PageRowHeight
        {
            get { return pageRowHeight; }
        }
        private int startRowIndex = 0;
        public int StartRowIndex
        {
            get { return startRowIndex; }
            set { startRowIndex = value; }
        }
        private int endRowIndex = 0;
        public int EndRowIndex
        {
            get { return endRowIndex; }
            set { endRowIndex = value; }
        }
        private int emptyRowCount = 0;
        public int EmptyRowCount
        {
            get { return emptyRowCount; }
            set { emptyRowCount = value; }
        }
        private int tailPrintRowCount = 0;
        public int TailPrintRowCount
        {
            get { return tailPrintRowCount; }
            set { tailPrintRowCount = value; }
        }
        //横向分页标识
        private bool isCrosswisePage = false;
        public bool IsCrosswisePage
        {
            get { return isCrosswisePage; }
            set { isCrosswisePage = value; }
        }
        //纵向分页标识
        private bool isDataPaging;

        public bool IsDataPaging
        {
            get { return isDataPaging; }
            set { isDataPaging = value; }
        }

        private List<int> containCols = new List<int>();
        public List<int> ContainCols
        {
            get { return containCols; }
            set { containCols = value; }
        }
        private ColumnInfo[] columnInfo = null;
        public ColumnInfo[] ColumnInfo
        {
            get { return columnInfo; }
            set { columnInfo = value; }
        }
        private int hpageIndex = -1;
        public int HpageIndex
        {
            get { return hpageIndex; }
            set { hpageIndex = value; }
        }
        private Dictionary<string, string> pageMacrs = new Dictionary<string, string>();
        public Dictionary<string, string> PageMacrs
        {
            get { return pageMacrs; }
            set { pageMacrs = value; }
        }
        private Dictionary<string, string> funResults = new Dictionary<string, string>();
        public Dictionary<string, string> FunResults
        {
            get { return funResults; }
        }
        internal PageInfo Clone()
        {
            PageInfo info = new PageInfo();
            info.pageRowHeight.AddRange(this.pageRowHeight);
            info.startRowIndex = this.startRowIndex;
            info.endRowIndex = this.endRowIndex;
            info.EmptyRowCount = this.emptyRowCount;
            info.TailPrintRowCount = this.tailPrintRowCount;
            info.isCrosswisePage = this.isCrosswisePage;
            info.containCols.AddRange(this.containCols);
            info.hpageIndex = this.HpageIndex;
            foreach (string key in funResults.Keys)
            {
                info.funResults[key] = funResults[key];
            }
            foreach (string key in pageMacrs.Keys)
            {
                info.PageMacrs[key] = pageMacrs[key];
            }
            return info;
        }
        public void SetPageMacrs(int page, int pageCount)
        {
            pageMacrs["pageno"] = page.ToString();
            pageMacrs["pagecount"] = pageCount.ToString();
        }
        //添加参数，去除魔鬼数据
        public void ChangePageMacrs(int pageOffset, int pageCount, bool setTotalPage)
        {
            if (setTotalPage)
            {
                PageMacrs["pagecount"] = pageCount.ToString();
            }
            PageMacrs["pageno"] = (Convert.ToInt32(PageMacrs["pageno"]) + pageOffset).ToString();
        }
    }
    public class ColumnInfo
    {
        private bool fixedColumn;

        public bool FixedColumn
        {
            get { return fixedColumn; }
            set { fixedColumn = value; }
        }
        private Dictionary<int, int> fixedColumnWidth = new Dictionary<int, int>();
        public Dictionary<int, int> FixedColumnWidth
        {
            get { return fixedColumnWidth; }
        }
        public int GetPagingWidth(int pageIndex)
        {
            if (!fixedColumn || !fixedColumnWidth.ContainsKey(pageIndex))
            {
                return width;
            }
            return fixedColumnWidth[pageIndex];
        }
        private int width = 0;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        private DrawInfo drawInfo = null;
        public DrawInfo DrawInfo
        {
            get { return drawInfo; }
            set { drawInfo = value; }
        }
        public ColumnInfo()
        { }
        public ColumnInfo(int width)
        {
            this.width = width;
        }
    }
    public class ExBodyRowHeight
    {
        public int Count
        { get { return singleRowHeights.Count; } }
        public int this[int rowIndex]
        {
            get
            {
                return singleRowHeights[rowIndex];
            }
        }
        List<int> singleRowHeights = new List<int>();
        List<Dictionary<int, int>> mutliRowHeights = new List<Dictionary<int, int>>();
        public void Add(int rowHeight)
        {
            singleRowHeights.Add(rowHeight);
            mutliRowHeights.Add(null);
        }
        public void Add(Dictionary<int, int> mutliMacrsRowHeigth)
        {
            int[] keys = new int[mutliMacrsRowHeigth.Count];
            mutliMacrsRowHeigth.Keys.CopyTo(keys, 0);
            singleRowHeights.Add(mutliMacrsRowHeigth[keys[0]]);
            mutliRowHeights.Add(mutliMacrsRowHeigth);
        }

        internal void ChangeHeight(int rowIndex, int rowHeight)
        {
            singleRowHeights[rowIndex] = rowHeight;
            mutliRowHeights[rowIndex] = null;
        }
        internal void ChangeHeight(int rowIndex, Dictionary<int, int> rowHeightDict)
        {

            int[] keys = new int[rowHeightDict.Count];
            rowHeightDict.Keys.CopyTo(keys, 0);
            singleRowHeights[rowIndex] = rowHeightDict[keys[0]];
            mutliRowHeights[rowIndex] = rowHeightDict;
        }
        internal int GetTotalHeight(int flagPagingCount)
        {
            int totalHeight = 0;
            if (flagPagingCount == 0)
            {
                foreach (int height in singleRowHeights)
                {
                    totalHeight += height;
                }
            }
            else
            {
                for (int index = 0; index < singleRowHeights.Count; index++)
                {
                    if (mutliRowHeights[index] != null && mutliRowHeights[index].ContainsKey(flagPagingCount))
                    {
                        totalHeight += mutliRowHeights[index][flagPagingCount];
                    }
                    else
                    {
                        totalHeight += singleRowHeights[index];
                    }
                }
            }
            return totalHeight;
        }
        internal void Init(List<int> list)
        {
            list.AddRange(singleRowHeights);
        }
        internal bool BigHeightRow(int TextMaxRowHeight)
        {
            bool hasBig = false;
            foreach (int height in singleRowHeights)
            {
                if (height > TextMaxRowHeight)
                {
                    hasBig = true;
                    break;
                }
            }
            return hasBig;
        }
        internal int GetHeight(int i)
        {
            return singleRowHeights[i];
        }
        internal int GetHeight(int rowIndex, int flagPagingIndex)
        {
            int height = 0;
            if (mutliRowHeights[rowIndex] != null && mutliRowHeights[rowIndex].ContainsKey(flagPagingIndex))
            {
                height = mutliRowHeights[rowIndex][flagPagingIndex];
            }
            else
            {
                height = singleRowHeights[rowIndex];
            }
            return height;
        }
    }
    public class ReportDrawStruct
    {
        private SizeF excelSizeF = new SizeF(1.0f, 1.0f);
        public SizeF ExcelSizeF
        {
            get { return excelSizeF; }
            set { excelSizeF = value; }
        }

        private bool invalid = false;
        public bool Invalid
        {
            get { return invalid; }
            set { invalid = value; }
        }
        public int EmptyRowHeight = 0;
        private WholeReportInfo wholeInfo = null;
        public WholeReportInfo WholeInfo
        {
            get { return wholeInfo; }
            set { wholeInfo = value; }
        }
        //object 需要包括宽度，DrawInfo
        private Dictionary<int, ColumnInfo> columnInfo = new Dictionary<int, ColumnInfo>();
        public Dictionary<int, ColumnInfo> ColumnInfo
        {
            get { return columnInfo; }
        }
        private List<int> bodyRowHeight = new List<int>();
        public List<int> BodyRowHeightList
        {
            get { return bodyRowHeight; }
        }
        //private List<ExRowHeight> exBodyHeigth = new List<int>();
        private ExBodyRowHeight exBodyHeigth = new ExBodyRowHeight();
        public ExBodyRowHeight ExBodyHeight
        {
            get { return exBodyHeigth; }
        }
        //public int ExBodyTotalHeight
        //{
        //    get
        //    {
        //        int totalHeight = 0;
        //        foreach (int height in exBodyHeigth)
        //        {
        //            totalHeight += height;
        //        }
        //        return totalHeight;
        //    }
        //}
        private DataTable bodyData = new DataTable();
        public DataTable BodyData
        {
            get { return bodyData; }
        }

        private DataTable exBodyData = new DataTable();
        public DataTable ExBodyData
        {
            get { return exBodyData; }
        }
        /// <summary>
        /// object 需要包含 BODY起讫行 尾行Index 尾行数 空白行数
        /// </summary>
        private List<PageInfo> pageList = new List<PageInfo>();
        public List<PageInfo> PageList
        {
            get { return pageList; }
        }
        public int PageCount
        {
            get { return pageList.Count; }
        }
        public ReportDrawStruct()
        {
            wholeInfo = new WholeReportInfo();
        }

        internal void InitInfoRowHeight(PageInfo pageInfo, int flagPagingCount)
        {
            //复制表头            
            int exBodyRowIndex = 0;
            bool isAddBody = false;
            foreach (DataRow row in exBodyData.Rows)
            {
                if (Convert.ToInt32(row[ConstantKey.XXXFLAG]) > Bands.BodyBandID && !isAddBody)
                {
                    isAddBody = true;
                    for (int rowIndex = pageInfo.StartRowIndex; rowIndex < pageInfo.EndRowIndex; rowIndex++)
                    {
                        pageInfo.PageRowHeight.Add(bodyRowHeight[rowIndex]);
                    }
                    //空白行添加
                    InsertEmptyRowHeight(pageInfo);
                }
                switch (Convert.ToInt32(row[ConstantKey.XXXFLAG]))
                {
                    case Bands.HeadBandID:
                    case Bands.TitleBandID:
                        pageInfo.PageRowHeight.Add(ExBodyHeight.GetHeight(exBodyRowIndex++, flagPagingCount));
                        break;
                    case Bands.PageBandID:
                        pageInfo.PageRowHeight.Insert(
                            pageInfo.PageRowHeight.Count - pageInfo.TailPrintRowCount, ExBodyHeight.GetHeight(exBodyRowIndex++, flagPagingCount));
                        break;
                    case Bands.RootBandID:
                        pageInfo.PageRowHeight.Add(ExBodyHeight.GetHeight(exBodyRowIndex++, flagPagingCount));
                        break;

                }
            }
            if (!isAddBody)
            {
                for (int rowIndex = pageInfo.StartRowIndex; rowIndex < pageInfo.EndRowIndex; rowIndex++)
                {
                    pageInfo.PageRowHeight.Add(bodyRowHeight[rowIndex]);
                }
                //空白行添加
                InsertEmptyRowHeight(pageInfo);
            }
        }
        public PageInfo GetPageInfo(int page)
        {
            if (page > pageList.Count)
            {
                throw new OutOfPageIndexException();
            }
            ColumnInfo[] info = new ColumnInfo[pageList[page - 1].ContainCols.Count];
            int index = 0;
            foreach (int columnName in pageList[page - 1].ContainCols)
            {
                info[index++] = columnInfo[columnName];
            }
            pageList[page - 1].ColumnInfo = info;
            return pageList[page - 1];
        }
        public DataTable GetPageData(int page)
        {
            //复制表头            
            DataTable pageData = new DataTable();
            PageInfo pageInfo = GetPageInfo(page);
            //pageInfo.SetPageMacrs(page, PageCount);
            InitPageDataColumn(pageData, pageInfo);
            DataRow newRow = null;
            bool isAddBody = (bodyData.Rows.Count < pageInfo.EndRowIndex);
            foreach (DataRow row in exBodyData.Rows)
            {
                if (!isAddBody && Convert.ToInt32(row[ConstantKey.XXXFLAG]) > Bands.BodyBandID)
                {
                    isAddBody = true;
                    GetPageBodyData(page, pageData, pageInfo);
                }
                switch (Convert.ToInt32(row[ConstantKey.XXXFLAG]))
                {
                    case Bands.HeadBandID:
                        newRow = pageData.NewRow();
                        pageData.Rows.Add(newRow);
                        SynTextCellData(pageInfo, row, newRow, pageInfo.ContainCols);
                        break;
                    case Bands.TitleBandID:
                        newRow = pageData.NewRow();
                        pageData.Rows.Add(newRow);
                        SynBandCellData(pageInfo, row, newRow, pageInfo.ContainCols);
                        break;
                    case Bands.PageBandID:
                        newRow = pageData.NewRow();
                        SynBandCellData(pageInfo, row, newRow, pageInfo.ContainCols);
                        //回头进行数据确认
                        pageData.Rows.InsertAt(newRow, pageData.Rows.Count - pageInfo.TailPrintRowCount);
                        break;
                    case Bands.RootBandID:
                        newRow = pageData.NewRow();
                        pageData.Rows.Add(newRow);
                        SynTextCellData(pageInfo, row, newRow, pageInfo.ContainCols);
                        break;
                }
            }
            if (!isAddBody)
            {
                GetPageBodyData(page, pageData, pageInfo);
            }
            CalcRect(page, pageData);
            return pageData;
        }

        private void GetPageBodyData(int page, DataTable pageData, PageInfo pageInfo)
        {
            DataRow newRow = null;
            for (int rowIndex = pageInfo.StartRowIndex; rowIndex < pageInfo.EndRowIndex; rowIndex++)
            {
                newRow = pageData.NewRow();
                pageData.Rows.Add(newRow);
                if (rowIndex != 0 && rowIndex == pageInfo.StartRowIndex && pageList[page - 2].EndRowIndex == pageInfo.StartRowIndex)
                {
                    SynCellRowMergeUpCell(rowIndex, pageInfo.EndRowIndex, bodyData.Rows[rowIndex], newRow, pageInfo.ContainCols);
                }
                else
                {
                    bool hasExceedRowMarge = false;
                    foreach (int columnName in pageInfo.ContainCols)
                    {
                        CellStruct cellStruct = bodyData.Rows[rowIndex][columnName.ToString()] as CellStruct;
                        if (cellStruct.ExcelInfo.RowMerge > pageInfo.EndRowIndex - rowIndex)
                        {
                            hasExceedRowMarge = true;
                            break;
                        }
                    }
                    if (hasExceedRowMarge)
                    {
                        //包含向下合并的行
                        SynCellRowMergeDownCell(rowIndex, pageInfo.EndRowIndex,
                            bodyData.Rows[rowIndex], newRow, pageInfo.ContainCols);
                    }
                    else
                    {
                        SynCellData(bodyData.Rows[rowIndex], newRow, pageInfo.ContainCols);
                    }
                }
            }
            //空白行添加
            InsertEmptyRow(pageData, pageInfo);

            //表体最后一行Cell(必需)需要有下边框线
            int rowCount = pageData.Rows.Count - 1;
            if (rowCount < 0)
                return;
            foreach (DataColumn column in pageData.Columns)
            {
                CellStruct cellStruct = pageData.Rows[rowCount][column] as CellStruct;
                if (!CellStruct.IsNullOrEmpty(cellStruct))
                {
                    //有左右边框的才加入下边线
                    //if (cellStruct.DrawInfo.BoundaryLine.LeftBoundaryLine && cellStruct.DrawInfo.BoundaryLine.RightBooundaryLine)
                    //有左边框的才加入下边线
                    if (cellStruct.DrawInfo.BoundaryLine.LeftBoundaryLine)
                    {
                        cellStruct.DrawInfo.BoundaryLine.LowerBoundaryLine = true;
                    }
                }
            }
        }
        private void SynCellRowMergeDownCell(int rowIndex, int endIndex, DataRow dataRow, DataRow newRow, List<int> list)
        {
            CellStruct cellStruct = null;
            foreach (int columnName in list)
            {
                cellStruct = dataRow[columnName.ToString()] as CellStruct;
                if (cellStruct.ExcelInfo.RowMerge > endIndex - rowIndex)
                {
                    cellStruct = cellStruct.Clone();
                    cellStruct.ExcelInfo.RowMerge = endIndex - rowIndex;
                }
                newRow[columnName.ToString()] = cellStruct;
            }
        }

        private void SynCellRowMergeUpCell(int rowIndex, int endIndex, DataRow dataRow, DataRow newRow, List<int> list)
        {
            CellStruct cellStruct = null;
            foreach (int colName in list)
            {
                cellStruct = dataRow[colName.ToString()] as CellStruct;
                if (CellStruct.IsNullOrEmpty(cellStruct))
                {
                    CellStruct tempCell = null;
                    //向上找            
                    for (int i = rowIndex - 1; i >= 0; i--)
                    {
                        tempCell = bodyData.Rows[i][colName.ToString()] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(tempCell))
                        {
                            if (tempCell.ExcelInfo.RowMerge > rowIndex - i)
                            {
                                int tempMerge = tempCell.ExcelInfo.RowMerge - (rowIndex - i);
                                if (tempMerge > endIndex - rowIndex)
                                    tempMerge = endIndex - rowIndex;
                                cellStruct = tempCell.Clone();
                                cellStruct.ExcelInfo.RowMerge = tempMerge;
                                break;
                            }
                        }
                    }
                }
                newRow[colName.ToString()] = cellStruct;
            }
        }
        public void VerifyPageColumn(PageInfo pageInfo)
        {
            if (pageInfo.ContainCols.Count == 0)
            {
                foreach (int column in columnInfo.Keys)
                {
                    pageInfo.ContainCols.Add(column);
                }
            }
        }
        private void SynTextCellData(PageInfo info, DataRow row, DataRow newRow, List<int> column)
        {
            CellStruct cellStruct = null;
            int index = 0;
            foreach (int columnName in column)
            {
                cellStruct = row[columnName.ToString()] as CellStruct;

                if (!CellStruct.IsNullOrEmpty(cellStruct) && cellStruct.ExcelInfo.ColMerge > column.Count - index)
                {
                    cellStruct = cellStruct.Clone();
                    cellStruct.ExcelInfo.ColMerge = column.Count - index;
                }
                if (cellStruct is MacrsCellStruct)
                {
                    cellStruct = (cellStruct as MacrsCellStruct).ReplaceMacrs(info.PageMacrs);
                }
                newRow[columnName.ToString()] = cellStruct;
                index++;
            }
        }
        private void SynCellData(DataRow row, DataRow newRow, List<int> column)
        {
            foreach (int columnName in column)
            {
                newRow[columnName.ToString()] = row[columnName.ToString()];
            }
        }
        private void SynBandCellData(PageInfo info, DataRow row, DataRow newRow, List<int> column)
        {
            CellStruct cellStruct = null;
            CellStruct tempStruct = null;
            int index = 0;
            foreach (int columnName in column)
            {
                cellStruct = row[columnName.ToString()] as CellStruct;
                if (cellStruct is MacrsCellStruct)
                {
                    cellStruct = (cellStruct as MacrsCellStruct).ReplaceMacrs(info.PageMacrs);
                }
                if (!CellStruct.IsNullOrEmpty(cellStruct))
                {
                    if (cellStruct.ExcelInfo.ColMerge > column.Count - index)
                    {
                        cellStruct = cellStruct.Clone();
                        cellStruct.ExcelInfo.ColMerge = column.Count - index;
                    }
                }
                if (index == 0 && CellStruct.IsNullOrEmpty(cellStruct))
                {
                    //查询该CellStruct之前是否有列合并Cell
                    for (int i = columnName - 1; i > 0; i--)
                    {
                        tempStruct = row[i.ToString()] as CellStruct;
                        if (!CellStruct.IsNullOrEmpty(tempStruct))
                        {
                            if (tempStruct.ExcelInfo.ColMerge > columnName - i)
                            {
                                cellStruct = tempStruct.Clone();
                                cellStruct.ExcelInfo.ColMerge -= columnName - i;
                                if (cellStruct.ExcelInfo.ColMerge > column.Count)
                                {
                                    cellStruct.ExcelInfo.ColMerge = column.Count;
                                }
                            }
                            break;
                        }
                    }
                }



                if (cellStruct is FunCellStruct)
                {
                    string funResult = info.FunResults[cellStruct.Context];
                    cellStruct = cellStruct.Clone();
                    cellStruct.Context = funResult;
                }
                newRow[columnName.ToString()] = cellStruct;
                index++;
            }
        }
        private void InitPageDataColumn(DataTable pageData, PageInfo pageInfo)
        {
            if (pageInfo.ContainCols.Count != 0)
            {
                foreach (int columnIndex in pageInfo.ContainCols)
                {
                    pageData.Columns.Add(columnIndex.ToString(), typeof(CellStruct));
                }
            }
            else
            {
                foreach (int columnIndex in columnInfo.Keys)
                {
                    pageData.Columns.Add(columnIndex.ToString(), typeof(CellStruct));
                }
            }
        }
        private void InsertEmptyRowHeight(PageInfo pageInfo)
        {
            //这个函数需要进一步确认
            int insertIndex = pageInfo.PageRowHeight.Count - pageInfo.TailPrintRowCount;
            for (int emptyIndex = 0; emptyIndex < pageInfo.EmptyRowCount; emptyIndex++)
            {
                pageInfo.PageRowHeight.Insert(insertIndex, EmptyRowHeight);
            }
        }
        private void InsertEmptyRow(DataTable pageData, PageInfo pageInfo)
        {
            //这个函数需要进一步确认
            int insertIndex = pageData.Rows.Count - pageInfo.TailPrintRowCount;
            DataRow newRow = null;
            for (int emptyIndex = 0; emptyIndex < pageInfo.EmptyRowCount; emptyIndex++)
            {
                newRow = pageData.NewRow();
                int columnCount = 1;
                foreach (int key in pageInfo.ContainCols)
                {
                    newRow[key.ToString()] = new CellStruct(
                             ""
                        , columnInfo[key].DrawInfo.Clone(),
                        new ExcelInfo(1, 1));
                    columnCount++;
                }
                pageData.Rows.InsertAt(newRow, insertIndex);
                // pageInfo.PageRowHeight.Insert(insertIndex, EmptyRowHeight);
            }
        }
        public void InitTableColumn()
        {
            foreach (int columnIndex in columnInfo.Keys)
            {
                bodyData.Columns.Add(columnIndex.ToString(), typeof(CellStruct));
                exBodyData.Columns.Add(columnIndex.ToString(), typeof(CellStruct));
            }
            //添加分页列标识及页尾打印标识
            bodyData.Columns.Add(ConstantKey.XXXFLAG, typeof(Int32));
            //添加Band列
            exBodyData.Columns.Add(ConstantKey.XXXFLAG, typeof(Int32));

        }
        internal void AddExBodyDataRow(int bandID, int intRowHeight, Dictionary<int, int> rowHeightDict)
        {
            DataRow newExBodyRow = exBodyData.NewRow();
            exBodyData.Rows.Add(newExBodyRow);
            newExBodyRow[ConstantKey.XXXFLAG] = bandID;
            if (rowHeightDict != null)
            {
                this.exBodyHeigth.Add(rowHeightDict);
            }
            else
            {
                //初始化行高
                exBodyHeigth.Add(intRowHeight);
            }
        }
        internal void AddExBody(CellStruct cellStruct, int rowIndex, int columnIndex, int rowHeight)
        {
            exBodyData.Rows[rowIndex][columnIndex] = cellStruct;
            exBodyHeigth.ChangeHeight(rowIndex, rowHeight);
        }
        internal void AddExBody(CellStruct cellStruct, int rowIndex, int columnIndex, Dictionary<int, int> rowHeightDict)
        {
            exBodyData.Rows[rowIndex][columnIndex] = cellStruct;
            exBodyHeigth.ChangeHeight(rowIndex, rowHeightDict);
        }
        internal int GetExRowHeight(int Y1, int Y2)
        {
            int tempHeight = 0;
            for (int rowIndex = Y1; rowIndex < Y2 && rowIndex < exBodyHeigth.Count; rowIndex++)
            {
                tempHeight += exBodyHeigth[rowIndex];
            }
            return tempHeight;
        }
        internal int GetExRowHeight(int rowIndex)
        {
            return exBodyHeigth[rowIndex];
        }
        internal void AddBodyDataRow(object flagID, int height)
        {
            DataRow newBodyRow = bodyData.NewRow();
            bodyData.Rows.Add(newBodyRow);
            newBodyRow[ConstantKey.XXXFLAG] = flagID;
            int intflagID = -1;
            if (int.TryParse(flagID.ToString(), out intflagID))
            {
                //初始化行高
                if (intflagID == ConstantKey.PAGINGFLAGVAL)
                    bodyRowHeight.Add(0);
                else
                    bodyRowHeight.Add(height);
            }
            else
            {
                throw new FlagRowInvalidException();
            }
        }
        internal void AddBody(CellStruct cellStruct, int rowIndex, int columnIndex, int rowHeight)
        {
            bodyData.Rows[rowIndex][columnIndex] = cellStruct;
            bodyRowHeight[rowIndex] = rowHeight;
        }
        internal int GetBodyRowHeight(int rowIndex)
        {
            return bodyRowHeight[rowIndex];
        }
        internal int GetBodyRowHeight(int rowIndex, int mergeCount, int emptyRowHeight)
        {
            return bodyRowHeight[rowIndex] + (mergeCount - 1) * emptyRowHeight;
        }
        internal int GetColWidth(int columnIndex, int mergeCount)
        {
            //横向分页未考虑
            int colWidth = 0;
            try
            {
                for (int mergeIndex = 0; mergeIndex < mergeCount; mergeIndex++)
                {
                    colWidth += columnInfo[columnIndex + mergeIndex + 1].Width;
                }
            }
            catch
            {
                throw new ColumnNumInvalidException();
            }
            return colWidth;
        }
        private void CalcRect(int pageIndex, DataTable pagetable)
        {
            //依据rowHeight columnInfo 计算point size excelInfo等信息
            PageInfo info = pageList[pageIndex - 1];
            //info.PageRowHeight
            List<int> rowPointList = new List<int>();
            rowPointList.Add(0);
            for (int rowIndex = 0; rowIndex < info.PageRowHeight.Count; rowIndex++)
            {
                rowPointList.Add(info.PageRowHeight[rowIndex] + rowPointList[rowIndex]);
            }

            int[] colArray = null;
            if (info.ContainCols.Count != 0)
            {
                colArray = new int[info.ContainCols.Count];
                info.ContainCols.CopyTo(colArray);
            }
            else
            {
                colArray = new int[columnInfo.Count];
                columnInfo.Keys.CopyTo(colArray, 0);
            }
            List<int> colPointList = new List<int>();
            colPointList.Add(0);
            for (int colIndex = 0; colIndex < colArray.Length; colIndex++)
            {
                colPointList.Add(ColumnInfo[colArray[colIndex]].GetPagingWidth(info.HpageIndex)
                    + colPointList[colIndex]);
            }

            CellStruct tempCell = null;
            for (int rowIndex = 0; rowIndex < pagetable.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < pagetable.Columns.Count; colIndex++)
                {
                    tempCell = pagetable.Rows[rowIndex][colIndex] as CellStruct;
                    if (tempCell == null || tempCell == CellStruct.Empty)
                    {
                        continue;
                    }
                    tempCell.DrawInfo.Point = new Point(colPointList[colIndex], rowPointList[rowIndex]);
                    tempCell.SetExcelPoint(rowIndex, colIndex);

                    //如何:处理被分页拆分的合并行列问题
                    //横向分页折分
                    //纵向分页折分[可暂不处理]

                    tempCell.DrawInfo.Size = new Size(GetPageColWidth(info.HpageIndex, colIndex, tempCell.ExcelInfo.ColMerge, colArray),
                        GetPageRowHeight(rowIndex, tempCell.ExcelInfo.RowMerge, info.PageRowHeight));

                }
            }
        }
        private int GetPageRowHeight(int rowIndex, int mergeCount, List<int> rowHeightlist)
        {
            int rowHeight = 0;
            for (int index = rowIndex; index < mergeCount + rowIndex; index++)
            {
                if (index >= rowHeightlist.Count)
                {
                    //确认为合并行Cell被分页拆分
                    break;
                }
                rowHeight += rowHeightlist[index];
            }
            return rowHeight;
        }
        private int GetPageColWidth(int hpageIndex, int colIndex, int colMerge, int[] colArray)
        {
            int colWidth = 0;
            for (int index = colIndex; index < colIndex + colMerge; index++)
            {
                if (index >= colArray.Length)
                {
                    //确认为列合并CELL横向拆分
                    break;
                }
                colWidth += columnInfo[colArray[index]].GetPagingWidth(hpageIndex);
            }
            return colWidth;
        }
        internal int[] GetColumnsWidth(List<int> containsCol)
        {
            int[] columns = null;
            if (containsCol != null)
            {
                columns = new int[containsCol.Count];
                int index = 0;
                foreach (int columnName in containsCol)
                {
                    columns[index] = ColumnInfo[columnName].Width;
                    index++;
                }
            }
            else
            {
                columns = new int[columnInfo.Count];
                int index = 0;
                foreach (int columnName in ColumnInfo.Keys)
                {
                    columns[index] = ColumnInfo[columnName].Width;
                    index++;
                }
            }
            return columns;
        }
        /// <summary>
        /// 更改页数宏变量 
        /// </summary>     
        /// <param name="pageOffset">
        /// 当前报表对于批量打印报表的页数总体偏移量
        /// </param>
        /// <param name="pageCount">
        /// 批量打印时的总体页数, -1表示不更改批量打印时的总页数
        /// </param>
        /// <param name="setTotalPage">
        /// 是否设置总页数
        /// </param>
        internal void SetPageOffset(int pageOffset, int pageCount, bool setTotalPage)
        {
            foreach (PageInfo info in PageList)
            {
                info.ChangePageMacrs(pageOffset, pageCount, setTotalPage);
            }
        }

        public MacrsCellStruct GetExFunCellStruct(int maxRowIndex)
        {
            MacrsCellStruct cellStruct = null;
            foreach (int columnName in this.ColumnInfo.Keys)
            {
                cellStruct = exBodyData.Rows[maxRowIndex][columnName.ToString()] as MacrsCellStruct;
                if (cellStruct != null)
                {

                    cellStruct.ExcelInfo.SetCurrentPageLocation(maxRowIndex, columnName - 1);
                    break;
                }
            }
            return cellStruct;
        }
    }
}