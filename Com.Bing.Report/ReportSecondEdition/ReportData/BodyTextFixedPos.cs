using System;
using System.Collections.Generic;
using System.Text;
using Key = System.Collections.Generic.KeyValuePair<int, int>;
namespace Com.Bing.Report
{
    public class BodyTextFixedPos
    {
        private Report report = null;

        Dictionary<Key, Text> textBuffer = new Dictionary<Key, Text>();

        //用于保存 不存在列所在Text的合并Index减少合并列数        
        private Texts bodyTexts = null;
        public Texts BodyTexts
        {
            get { return bodyTexts; }
        }

        public Text this[int rowGroup, int columnIndex]
        {
            get
            {
                Text text = null;
                Key key = new Key(rowGroup, columnIndex);
                textBuffer.TryGetValue(key, out text);
                return text;
            }
        }
        public Text this[int rowGroup, string columnName]
        {
            get
            {
                int columnIndex = report.Columns.IndexOf(columnName);
                return this[rowGroup, columnIndex + 1];
            }
        }
        public BodyTextFixedPos(Report report)
        {
            this.report = report;
            //形成可快速查找结构
            this.bodyTexts = report.Texts.GetTextsBy(Bands.BodyBandID);
            List<int> noPrintColumn = InitNoPrintColumn();
            InitSpeedStruct(noPrintColumn);
        }

        /// <summary>
        /// 调整无数不打印问题
        /// </summary>        
        public void InitNoDataPrintColumn(List<string> noDataPrintColumn)
        {
            List<int> noPrintColumn = InitNoPrintColumn();
            foreach (string columnName in noDataPrintColumn)
            {
                noPrintColumn.Add(report.Columns.IndexOf(columnName)+1);
            }

            textBuffer.Clear();            
            InitSpeedStruct(noPrintColumn);
        }
        private List<int> InitNoPrintColumn()
        {
            List<int> noPrintColumn = new List<int>();
            for (int columnIndex = 1; columnIndex <= report.Columns.Count ; columnIndex++)
            {
                if (report.Columns[columnIndex-1].Attibutes.PrintStyle.NoPrint)
                {
                    noPrintColumn.Add(columnIndex);
                }
            }
            return noPrintColumn;
        }
        private void InitSpeedStruct(List<int> noPrintColumn)
        {
            Texts tempTexts = null;
            int rowGroup = -1;
            for (int index = 1; index <= report.Bands[Bands.BodyBandID]; index++)
            {
                tempTexts = bodyTexts.GetTextsBy(Bands.BodyBandID, index);
                if (tempTexts.Count > 0)
                {
                    if (!int.TryParse(tempTexts[tempTexts.Count - 1].Context, out rowGroup))
                    {
                        throw new RowGounpInvalidException();
                    }
                    //终是默认最后一个Text 为 rowGroupText
                    for (int textIndex = 0; textIndex < tempTexts.Count - 1; textIndex++)
                    {
                        //处理合并单元素第1列被合并的情况
                        if (tempTexts[textIndex].ColumnSpan > 1 && noPrintColumn.Contains(tempTexts[textIndex].Location.X1))
                        {
                            //找到未被隐藏的列
                            int columnIndex = tempTexts[textIndex].Location.X1;
                            while (noPrintColumn.Contains(columnIndex))
                            {
                                columnIndex++;
                            }
                            if (columnIndex <= tempTexts[textIndex].Location.X2)
                            {
                                Text cloneText = tempTexts[textIndex].Clone();
                                cloneText.SetLocationX(columnIndex, cloneText.Location.X2);                                
                                textBuffer[new Key(rowGroup, columnIndex)] = cloneText;
                            }
                        }
                        else
                        {
                            textBuffer[new Key(rowGroup, tempTexts[textIndex].Location.X1)] = tempTexts[textIndex];

                        }
                        
                    }
                }
            }
        }
        public bool HasText(int rowGroup, int columnIndex)
        {
            return textBuffer.ContainsKey(new Key(rowGroup, columnIndex));
        }
        public bool HasText(int rowGroup, string columnName)
        {
            int columnIndex = report.Columns.IndexOf(columnName);
            return HasText(rowGroup, columnIndex + 1);
        }

    }
}
