using System;
using System.Collections.Generic;
using System.Text;
using Com.Bing.API;
using System.Drawing;

namespace Com.Bing.Report
{
    public class ReportCopyHelper
    {
        public static void RootCopy(Report sourceReport, Report destReport)
        {
            //生成块模型
            RptTextModule rootModule = new RptTextModule(sourceReport, new List<int>(new int[] { Bands.RootBandID }));
            //清除已以前的表脚
            destReport.Texts.ClearTexts(Bands.RootBandID);
            //同步表脚行显示及行数
            sourceReport.Bands.CopyRootTo(destReport.Bands);
            //依据新的columnsWidth生成RootTexts
            Texts texts = rootModule.MapNewText(destReport.Columns.GetColumnsWidth(null));
            foreach (Text text in texts)
            {
                destReport.Texts.Add(text);
            }
            destReport.WriteRpt();
        }
    }
    public class RptTextModule : List<SignRowTextBlock>
    {
        private Report report = null;
        private List<int> bandsID = null;
        int[] usedColumns = null;
        Texts texts = new Texts();
        public Texts Texts
        {
            get { return texts; }

        }
        public RptTextModule(Report report, List<int> bandsID)
            : this(report, bandsID, null)
        {

        }
        public RptTextModule(Report report, List<int> bandsID, int[] usedListColumn)
        {

            this.report = report;
            this.bandsID = bandsID;
            this.usedColumns = usedListColumn;
            InitModuleByReport();
        }
        private void InitModuleByReport()
        {
            texts.Clear();
            int[] reportColumnsWidth = report.Columns.GetColumnsWidth(usedColumns);
            int[] x2columns = null;
            if (usedColumns != null)
            {
                x2columns = new int[usedColumns.Length];
                for (int index = 1; index < usedColumns.Length; index++)
                {
                    x2columns[index] = x2columns[index - 1];
                    if (usedColumns[index] == 0)
                    {
                        x2columns[index] -= 1;
                    }
                }
            }

            Texts tempRowTexts = null;
            SignRowTextBlock rowRootBlock = null;
            foreach (int bandID in bandsID)
            {
                for (int index = 0; index < report.Bands.BandDict[bandID].BandCount; index++)
                {
                    tempRowTexts = report.Texts.GetTextsBy(bandID, index + 1);
                    tempRowTexts = reMapTexts(tempRowTexts, usedColumns, x2columns, x2columns);
                    texts.AddRange(tempRowTexts);
                    rowRootBlock = new SignRowTextBlock(tempRowTexts, reportColumnsWidth);
                    Add(rowRootBlock);
                }
            }
        }

        private Texts reMapTexts(Texts tempRowTexts, int[] usedColumns, int[] x1columns, int[] x2columns)
        {
            if (usedColumns == null)
            {
                return tempRowTexts;
            }

            Texts texts = new Texts();
            foreach (Text text in tempRowTexts)
            {
                try
                {
                    if (usedColumns[text.Location.X1] != 0)
                    {

                        texts.Add(text.CopyInNewLoction(text.Location.X1 + x1columns[text.Location.X1],
                            text.Location.X2 + x2columns[text.Location.X2]));
                    }
                    else if (usedColumns[text.Location.X1] == 0 && text.Location.X2 != text.Location.X1)
                    {
                        //处理表头 页汇合并性的问题
                        Text tempText = text.CopyInNewLoction(text.Location.X1 + x1columns[text.Location.X1] + 1,
                            text.Location.X2 + x2columns[text.Location.X2]);
                        if (tempText.Location.X1 <= tempText.Location.X2)
                        {
                            texts.Add(tempText);
                        }
                        
                    }
                }
                catch
                {
                    Function.Alert("文本合并越界", "提示");
                }
            }
            return texts;
        }

        /// <summary>
        /// 脚表拷贝的Core函数,用于映射生成一组新的Text
        /// </summary>
        double[] blockPercent = null;
        double[] columnPercent = null;
        public Texts MapNewText(int[] destColumnsWidth)
        {
            Texts reMapTexts = new Texts();
            foreach (SignRowTextBlock rowRootBlock in this)
            {
                if (rowRootBlock.Count >= destColumnsWidth.Length)
                {
                    //从两头往中间映射
                    Texts beginTexts = new Texts();
                    Texts endTexts = new Texts();
                    for (int beginPtr = 0, endPtr = rowRootBlock.Count - 1, destEptr = destColumnsWidth.Length - 1;
                        beginPtr <= endPtr && beginPtr <= destEptr; beginPtr++, endPtr--, destEptr--)
                    {
                        if (!rowRootBlock[beginPtr].EmptyBlock)
                        {
                            if (beginPtr == endPtr)
                            {
                                beginTexts.Add(rowRootBlock[beginPtr].Text.CopyInNewLoction(beginPtr + 1, beginPtr + 1));
                                break;
                            }
                            beginTexts.Add(rowRootBlock[beginPtr].Text.CopyInNewLoction(beginPtr + 1, beginPtr + 1));

                        }
                        if (!rowRootBlock[endPtr].EmptyBlock)
                        {
                            endTexts.Insert(0, rowRootBlock[endPtr].Text.CopyInNewLoction(destEptr + 1, destEptr + 1));
                        }
                    }
                    reMapTexts.AddRange(beginTexts);
                    reMapTexts.AddRange(endTexts);
                }
                else if (rowRootBlock.Count == 1)
                {
                    reMapTexts.Add(rowRootBlock[0].Text.CopyInNewLoction(1, destColumnsWidth.Length));
                }
                else
                {
                    //使用插桩分组方式
                    //0.053064275 0.156950673 0.091928251 0.200298954 0.183856502 0.313901345
                    //0.086956522 0.204508857 0.09178744 0.161030596 0.086956522 0.095008052 0.120772947 0.152979066 
                    //能插入的位置为 1 ,2 , 3 ,4 ,5, 6, 7 取1  2  3  4    6 映ColumnIndex为 11 22 33 44 56 78   
                    blockPercent = rowRootBlock.BlockPercent();
                    columnPercent = GetColumnPercent(destColumnsWidth);
                    List<Point> groupLocation = GroupColumnArray();
                    for (int i = 0; i < rowRootBlock.Count; i++)
                    {
                        if (!rowRootBlock[i].EmptyBlock)
                        {
                            reMapTexts.Add(rowRootBlock[i].Text.CopyInNewLoction(groupLocation[i].X + 1, groupLocation[i].Y + 1));
                        }
                    }
                }
            }
            return reMapTexts;
        }

        //[a,b) 取a不取b
        public List<Point> GroupColumnArray()
        {
            if (blockPercent.Length == 0)
            {
                return null;
            }

            List<Point> points = new List<Point>();
            int[] bestGroup = Combine.BestGroup(blockPercent, columnPercent);
            int x1 = 0;
            for (int index = 0; index < bestGroup.Length; index++)
            {
                if (bestGroup[index] == 1)
                {
                    if (index > 0)
                    {
                        points.Add(new Point(x1, index - 1));
                    }
                    x1 = index;
                }
            }
            points.Add(new Point(x1, bestGroup.Length - 1));
            return points;
        }

        private double CalcAbs(double[] blockPercent, double[] columnPercent, int[] selectArray)
        {
            double result = 0.0;
            double tempblockPercent = 0.0;

            int blockIndex = blockPercent.Length - 1;
            for (int i = columnPercent.Length - 1; i >= 0; i--)
            {
                if (selectArray[i] == 1)
                {
                    tempblockPercent += columnPercent[i];
                    result += Math.Abs(tempblockPercent - blockPercent[blockIndex]);
                    tempblockPercent = 0.0;
                    blockIndex--;
                }
                else
                {
                    tempblockPercent += columnPercent[i];
                }
            }
            return result;
        }
        private void RePreInit(List<int> selectArray, int changeLoction)
        {
            int zeroIndex = -1;
            for (int index = 0; index < changeLoction; index++)
            {
                if (selectArray[index] == 0)
                {
                    zeroIndex = index;
                }
                else
                {
                    break;
                }
            }
            if (zeroIndex != -1)
            {
                selectArray.RemoveRange(0, zeroIndex + 1);
                int firstZeroIndex = -1;
                for (int index = 0; index < selectArray.Count; index++)
                {
                    if (selectArray[index] == 0)
                    {
                        firstZeroIndex = index;
                        break;
                    }
                }
                for (int index = 0; index < zeroIndex + 1; index++)
                {
                    selectArray.Insert(firstZeroIndex, 0);
                }

            }
        }
        private static bool Verify(List<int> selectArray)
        {
            bool verifyFlag = true;
            for (int index = 0; index < selectArray.Count - 1; index++)
            {
                if (selectArray[index] == 1 && selectArray[index + 1] == 0)
                {
                    verifyFlag = false;
                    break;
                }
            }
            return verifyFlag;
        }
        private double[] GetColumnPercent(int[] destColumnsWidth)
        {
            int totalWidth = 0;
            foreach (int width in destColumnsWidth)
            {
                totalWidth += width;
            }
            double[] columnPercent = new double[destColumnsWidth.Length];
            for (int i = 0; i < destColumnsWidth.Length; i++)
            {
                columnPercent[i] = destColumnsWidth[i] / (double)totalWidth;
            }
            return columnPercent;
        }
        internal void MergeTextSize(System.Drawing.Graphics g, MacrosVerbCollection macrosVerbCollection)
        {
            foreach (SignRowTextBlock textBlock in this)
            {
                foreach (RowBlock block in textBlock)
                {
                    block.MergeTextSize(g, macrosVerbCollection);
                }
                textBlock.ReInitTextInfo();
            }
        }
    }
    public class SignRowTextBlock
        : List<RowBlock>
    {

        public SignRowTextBlock() { }
        public SignRowTextBlock(Texts texts, int[] columnsWidth)
        {
            InitRowRootBlock(texts, columnsWidth);
        }
        public void InitRowRootBlock(Texts texts, int[] columnsWidth)
        {
            int totalWidth = 0;
            foreach (int width in columnsWidth)
            {
                totalWidth += width;
            }

            Add(new RowBlock(null, 0, 0, 0));
            int recordLastBlocPostion = 0;
            foreach (Text text in texts)
            {

                if (!string.IsNullOrEmpty(text.Context) || text.IsIncludeMacros)
                {
                    if (text.Location.X1 > recordLastBlocPostion + 1)
                    {
                        //加入空白块
                        Add(new RowBlock(null, this[Count - 1].EndPostion,
                            GetWidth(recordLastBlocPostion + 1, text.Location.X1 - 1, columnsWidth), totalWidth));
                    }

                    //加入有字块                
                    Add(new RowBlock(text, this[Count - 1].EndPostion,
                        GetWidth(text.Location.X1, text.Location.X2, columnsWidth), totalWidth));
                    recordLastBlocPostion = text.Location.X2;
                }

            }
            RemoveAt(0);


        }
        private int GetWidth(int x1, int x2, int[] columnWidth)
        {
            int width = 0;
            for (int columnIndex = x1 - 1; columnIndex < x2; columnIndex++)
            {
                if (columnIndex > columnWidth.Length - 1)
                {
                    break;
                }
                width += columnWidth[columnIndex];
            }
            return width;
        }
        public double[] BlockPercent()
        {
            double[] blockPercent = new double[Count];
            int blockIndex = 0;
            foreach (RowBlock block in this)
            {
                blockPercent[blockIndex] = block.WidthPrecent;
                blockIndex++;
            }
            return blockPercent;
        }
        internal void ReInitTextInfo()
        {
            int totalWidth = 0;
            Insert(0, (new RowBlock(null, 0, 0, 0)));
            for (int blockIndex = 1; blockIndex < this.Count; blockIndex++)
            {
                this[blockIndex].StartPostion = this[blockIndex - 1].EndPostion;
                totalWidth += this[blockIndex].Width;
            }
            RemoveAt(0);
            foreach (RowBlock rowBlock in this)
            {
                rowBlock.TotalWidth = totalWidth;
            }
        }
    }
    public class RowBlock
    {
        private Text text = null;

        public Text Text
        {
            get { return text; }
        }
        private int startPostion = 0;
        public bool EmptyBlock
        {
            get { return text == null; }
        }

        public int StartPostion
        {
            get { return startPostion; }
            set { startPostion = value; }
        }
        private int width = 0;
        public int Width
        {
            get { return width; }
        }
        public int EndPostion
        {
            get { return startPostion + width; }
        }
        private int totalWidth = 0;
        public int TotalWidth
        {
            set { totalWidth = value; }
        }
        public double WidthPrecent
        {
            get
            {
                if (totalWidth == 0)
                {
                    return 0.0;
                }
                return width / (double)totalWidth;
            }
        }
        public RowBlock(Text text, int startPostion, int width, int totalWidth)
        {
            this.text = text;
            this.startPostion = startPostion;
            this.width = width;
            this.totalWidth = totalWidth;
        }

        internal void MergeTextSize(System.Drawing.Graphics g, MacrosVerbCollection macrosVerbCollection)
        {
            if (EmptyBlock)
            {
                return;
            }
            if (text.IsIncludeMacros)
            {
                List<string> list = new List<string>();
                foreach (string s in text.MacrosArgsName)
                {
                    list.Add(macrosVerbCollection[s, 1]);
                }
                text.MacrosContext = list;
            }
            if (!string.IsNullOrEmpty(text.Context))
            {
                SizeF sizef = g.MeasureString(text.Context, text.Attribute.Font);
                if (sizef.Width > (double)width)
                {
                    width = Convert.ToInt32(sizef.Width);
                }
            }
        }
    }

}
