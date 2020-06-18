using System;
using System.Collections.Generic;
using System.Text;


namespace Com.Bing.Report 
{
    public  class  ReportAttribute
    {
        private Report report = null;
        private string reportName = "";
        public string ReportName
        {
            get { return reportName; }
            set { reportName = value; }
        }
        System.Drawing.Printing.Margins margin = new System.Drawing.Printing.Margins();
        public System.Drawing.Printing.Margins Margin
        {
            get { return margin; }
            set { margin = value; }
        }
        //纸张方向
        private int grainDirection = 0;
        public int GrainDirection
        {
            get { return grainDirection; }
            set {
                if (grainDirection != value)
                {
                    grainDirection = value;                    
                }
            }
        }

        private float rowHeiht = 0;
        public float RowHeiht
        {
            get { return rowHeiht; }
            set { rowHeiht = value; }
        }

        private int pagination = 0;
        public int Pagination
        {
            get { return pagination; }
            set { pagination = value; }
        }

        private System.Drawing.Font font = null;
        public System.Drawing.Font Font
        {
            get { return font; }
            set { font = value; }
        }

        private PrintStyle printStyle = null;
        public PrintStyle PrintStyle
        {
            get { return printStyle; }
            set { printStyle = value; }
        }

        private float lineWidth = 0;
        public float LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }
        private float bordLineWidth = 1;
        public float BordLineWidth
        {
            get { return bordLineWidth; }
            set { bordLineWidth = value; }
        }
        public  ReportAttribute(Report report) 
        {
            this.report = report;
            printStyle = new PrintStyle();
            font = ConvertUtil.GetFont("宋体","9");
            rowHeiht = 20;
            LineWidth = 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <example>
        /// 工程量清单单价分析表,201,201,203,203,1,2.25,-1,宋体,-9,0,0,1.0,1.5
        /// </example>
        public void Add(string propertyValue)
        {   
            string[] propertyArray = propertyValue.Split(',');
            if (propertyValue.Length < 13)
            {
                throw new ReportWholePropertyNoLengthException();
            }
            this.reportName = propertyArray[0];
            this.margin.Top = ConvertUtil.GetBoundary(propertyArray[1]) ;
            this.margin.Bottom = ConvertUtil.GetBoundary(propertyArray[2]);
            this.margin.Left = ConvertUtil.GetBoundary(propertyArray[3]) ;
            this.margin.Right = ConvertUtil.GetBoundary(propertyArray[4]);
            this.grainDirection = ConvertUtil.GetGrainDirection(propertyArray[5]);
            this.rowHeiht = ConvertUtil.GetRowHeight(propertyArray[6]);
            this.Pagination = ConvertUtil.GetPagination(propertyArray[7]);
            this.font = ConvertUtil.GetFont(propertyArray[8], propertyArray[9]);
            this.printStyle = ConvertUtil.GetPrintStyle(propertyArray[10], propertyArray[11]);
            this.lineWidth = ConvertUtil.GetLineWidth(propertyArray[12]);
            if (propertyArray.Length > 13)
            {
                this.bordLineWidth = ConvertUtil.GetLineWidth(propertyArray[13]);
            }
        }
        public override string  ToString()
        {
            object[] propertyArray = new object[14];
            propertyArray[0] = ReportName; ;
            propertyArray[1] = ConvertUtil.GetBoundaryMM(this.margin.Top);
            propertyArray[2] = ConvertUtil.GetBoundaryMM(this.margin.Bottom);
            propertyArray[3] = ConvertUtil.GetBoundaryMM(this.margin.Left);
            propertyArray[4] = ConvertUtil.GetBoundaryMM(this.margin.Right);
            propertyArray[5] = this.grainDirection;
            propertyArray[6] = this.rowHeiht;
            propertyArray[7] = this.pagination;

            string[] fontInfo = ConvertUtil.FontInfoTo(this.font);
            Array.Copy(fontInfo, 0, propertyArray, 8, 2);
            propertyArray[10] = printStyle.IsPrintInLast?1:0;
            propertyArray[11] = printStyle.IsBrifeStyle?1:0;
            propertyArray[12] = lineWidth;            
            propertyArray[13] = this.bordLineWidth;
            StringBuilder builder = new StringBuilder();
            builder.Append("d001001=");
            int tempIndex = 0;
            foreach (object obj in propertyArray)
            {
                builder.Append(obj);
                if (tempIndex == 13)
                {
                    break;
                }
                builder.Append(",");
                tempIndex++;
            }
            return builder.ToString();
        }
        
    }
    //分部分项工程量清单综合单价计算表,200,200,200,200,2,1.25,-1,宋体,-9,0,1,0.1,0.1
}
