using System;
using System.Collections.Generic;
using System.Text;
using Com.Bing.API;
using System.Drawing;

namespace Com.Bing.Report
{    
    public class BigTextAnalyse
    {
        private ReportDrawStruct drawStruct = null;
        private Report report = null;
        Graphics graphics = null;
        public BigTextAnalyse(ReportDrawStruct drawStruct , Report report , Graphics graphics)
        {
            this.drawStruct = drawStruct;
            this.report = report;
            this.graphics = graphics;
        }

        public void Analyse()
        {
            //出求最大行能占的最大高度 以及行Index
            int maxHeight = 0;
            int maxRowIndex = -1;
            int totalHeightExMaxHeightRow = 0;
            for (int index = 0; index < drawStruct.ExBodyHeight.Count; index++)
            {
                if (drawStruct.ExBodyHeight[index] > maxHeight)
                {
                    maxHeight = drawStruct.ExBodyHeight[index];
                    maxRowIndex = index;
                }
                totalHeightExMaxHeightRow += drawStruct.ExBodyHeight[index];
            }
            totalHeightExMaxHeightRow -= maxHeight;
            int maxRowEnableHeight = report.AreaHeight - totalHeightExMaxHeightRow;            
            drawStruct.ExBodyHeight.ChangeHeight(maxRowIndex , maxRowEnableHeight);

            //求最大行高的宏变量 、 具体值 以及最大列宽 , 最大行高必须要宏变量扩展，否则不处理。 
            MacrsCellStruct cellStruct = drawStruct.GetExFunCellStruct(maxRowIndex);
            if (CellStruct.IsNullOrEmpty(cellStruct))
            {   
                throw new BigTextAnalyseException("大文本字段分隔失败，请手动处理！");
            }
            //最大可能行高需要加上Cell向下合并的行数
            for (int index = maxRowIndex + 1; index < cellStruct.ExcelInfo.RowMerge + maxRowIndex; index++)
            {
                maxRowEnableHeight += drawStruct.ExBodyHeight[index];
            }

            int colWidth = 0;
            int mergeColIndex = 0;
            //计算可行列宽
            foreach (int columnName in drawStruct.ColumnInfo.Keys)
            {   
                if (columnName > cellStruct.ExcelInfo.ExcelPoint.Y)
                {
                    mergeColIndex++;
                    colWidth += drawStruct.ColumnInfo[columnName].Width;
                }
                if (mergeColIndex == cellStruct.ExcelInfo.ColMerge)
                {
                    break;
                }
            }

            string macrsKey = System.Text.RegularExpressions.Regex.Match(cellStruct.Context, @"(?<=\[).*(?=\])").Value;
            string macrsVal = report.Data.MacorsVerbCollection[macrsKey,1];

            Text macrsText = null;
            foreach (Text text in report.Texts)
            {
                if (text.MContext == cellStruct.Context)
                {
                    macrsText = text;
                    break;
                }
            }

            List<string> spliteContents = GetPartition(macrsVal, macrsText.Attribute.Font, colWidth, maxRowEnableHeight);

            PageInfo pageInfo = null;
            foreach (string marcsSplitVal in spliteContents)
            {
                pageInfo = new PageInfo();
                pageInfo.StartRowIndex = 0;
                pageInfo.EndRowIndex = drawStruct.ExBodyHeight.Count;
                drawStruct.ExBodyHeight.Init(pageInfo.PageRowHeight);                
                drawStruct.PageList.Add(pageInfo);
                drawStruct.VerifyPageColumn(pageInfo);
                //pageInfo.PageMacrs = 
                Dictionary<string, string> tempMacrs = report.Data.MacorsVerbCollection.GetPageMacrs(1); 
                foreach (KeyValuePair<string, string> entry in tempMacrs)
                {
                    pageInfo.PageMacrs[entry.Key] = entry.Value;
                }
                pageInfo.PageMacrs[macrsKey] = marcsSplitVal;                
            }

        }
        /// <summary>
        /// 对大文本数据以固定的高宽进行分割
        /// </summary>        
        private List<string> GetPartition(string context, Font font, int enableWidth, int enableHeight)
        {
            List<string> spliteContexts = new List<string>();

            while (true)
            {
                int i = 0, m = 0, n;
                n = context.Length;
                SizeF textSizeF = new SizeF();

                while (i < n && i + 1 != n)
                {
                    m = (i + n) / 2;
                    textSizeF = DrawUtil.MeasureString(context.Substring(0, m), font, enableWidth, graphics,report.Attributes.RowHeiht);
                    if (textSizeF.Height > enableHeight)
                    {
                        n = m;
                    }
                    else
                    {
                        i = m;
                    }
                }
                if (textSizeF.Height > enableHeight)
                {
                    m -= 1;
                }
                if (context.Length <= m + 1)
                {
                    spliteContexts.Add(context);
                    break;
                }
                else
                {
                    spliteContexts.Add(context.Substring(0, m));
                    context = context.Substring(m);
                }
            }

            return spliteContexts;
        }
    }
}
