using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace Com.Bing.Report
{
    /// <summary>
    /// 用于存取文字属性
    /// </summary>
    public class TextAttibute
    {
        public const int PROPERTYCOUNT = 22;

        private System.Drawing.Font font = null;
        public System.Drawing.Font Font
        {
            get { return font; }
            set { font = value; }
        }

        //合并成画刷
        private System.Drawing.Brush foreBrush = System.Drawing.Brushes.Black;
        public System.Drawing.Brush ForeBrush
        {
            get { return foreBrush; }
            set { foreBrush = value; }
        }
        private System.Drawing.Brush backgroupBrush = System.Drawing.Brushes.Black;
        public System.Drawing.Brush BackgroupBrush
        {
            get { return backgroupBrush; }
            set { backgroupBrush = value; }
        }
        //合并成stringFormat属性

        private System.Drawing.StringFormat valign = new System.Drawing.StringFormat();
        public System.Drawing.StringFormat Valign
        {
            get { return valign; }
            set { valign = value; }
        }

        private BoundaryLine boundaryLine = new BoundaryLine();
        public BoundaryLine BoundaryLine
        {
            get { return boundaryLine; }
            set { boundaryLine = value; }
        }

        private bool autoWrap = false;
        public bool AutoWrap
        {
            get { return autoWrap; }
            set { autoWrap = value; }
        }
        private bool isAdjustWeith = false;
        public bool IsAdjustWeith
        {
            get { return isAdjustWeith; }
            set { isAdjustWeith = value; }
        }

        private PrintStyle printStyle = null;
        public PrintStyle PrintStyle
        {
            get { return printStyle; }
            set { printStyle = value; }
        }

        private int diagonal = -1;
        public int Diagonal
        {
            get { return diagonal; }
            set { diagonal = value; }
        }

        private int excursion = 0;
        public int Excursion
        {
            get { return excursion; }
            set { excursion = value; }
        }
        private string nameZh_cn = "";
        public string NameZh_cn
        {
            get { return nameZh_cn; }
            set { nameZh_cn = value; }
        }
        //动态列属性(只适用于列的属性)
        private bool dynamicColumn = false;
        public bool DynamicColumn
        {
            get { return dynamicColumn; }
            set { dynamicColumn = value; }
        }
        string[] textPropertyArray = null;
        public string[] TextPropertyArray
        {
            get { return textPropertyArray; }
            set { textPropertyArray = value; }
        }
        public TextAttibute()
        {
            printStyle = new PrintStyle();
            font = ConvertUtil.GetFont("", "");
        }

        public void Add(string[] value, Report report)
        {
            textPropertyArray = value;

            this.font = ConvertUtil.GetFont(value[0], value[1], value[2], value[3], value[4], value[5], report);

            //this.brush = ConvertUtil.getBrush(value[6], value[7]);
            //前景色默认为空，则取黑色 , 背景色为空，则取白色
            this.foreBrush = ConvertUtil.GetBrush(string.IsNullOrEmpty(value[6]) ? "" : value[6]);

            if (Color.White.ToArgb() == ((SolidBrush)foreBrush).Color.ToArgb())
            {
                foreBrush = Brushes.Black;
            }

            this.backgroupBrush = ConvertUtil.GetBrush(string.IsNullOrEmpty(value[7]) ? "" : value[7]);

            //字位置

            this.valign = ConvertUtil.GetStringFormat(value[8], value[9]);

            //上下左右边框
            this.boundaryLine.UpperBoundaryLine = value[10].Trim().Equals("1") || value[10].Trim().Equals(string.Empty);
            this.boundaryLine.LowerBoundaryLine = value[11].Trim().Equals("1") || value[11].Trim().Equals(string.Empty);
            this.boundaryLine.LeftBoundaryLine = value[12].Trim().Equals("1") || value[12].Trim().Equals(string.Empty);
            this.boundaryLine.RightBooundaryLine = value[13].Trim().Equals("1") || value[13].Trim().Equals(string.Empty);
            

            //折行
            this.autoWrap = (value[14].Trim() == "1");

            //打印相关设置：无数据不打印以及固定不打印
            this.printStyle.NoDataNoPrint = (value[15].Equals("1"));
            this.printStyle.NoPrint = (value[16].Equals("1"));

            //自动宽度
            this.isAdjustWeith = (value[17].Equals(string.Empty) || value[17].Equals("1"));

			//特殊线
            this.diagonal = (value[18].Equals(string.Empty) ? this.diagonal : Convert.ToInt32(value[18]));
            switch (diagonal)
            {
                case 1:
                    this.boundaryLine.IsSlash = true;
                    break;
                case 2:
                    this.boundaryLine.IsBackSlash = true;
                    break;
                case 3:
                    this.boundaryLine.IsCrossLine = true;
                    break;
                default:
                    this.boundaryLine.IsSlash = false;
                    this.boundaryLine.IsBackSlash = false;
                    this.boundaryLine.IsCrossLine = false;
                    break;
            }
            this.excursion = value[19].Trim().Equals(string.Empty) ? 0 : int.Parse(value[19]);
            this.nameZh_cn = value[20];
            this.dynamicColumn = value[21].Equals("1");
        }
        public override string ToString()
        {
            string[] textPropertyArray = new string[22];

            string[] fontInfo = ConvertUtil.FontInfoTo(this.font);
            fontInfo.CopyTo(textPropertyArray, 0);

            textPropertyArray[6] = ConvertUtil.BrushTo(this.ForeBrush);
            textPropertyArray[7] = ConvertUtil.BrushTo(this.backgroupBrush);

            //字位置
            textPropertyArray[8] = ConvertUtil.AlignTo(this.Valign.LineAlignment);
            textPropertyArray[9] = ConvertUtil.AlignTo(this.valign.Alignment);

            //上下左右边框
            textPropertyArray[10] = boundaryLine.UpperBoundaryLine ? "1" : "0";
            textPropertyArray[11] = boundaryLine.LowerBoundaryLine ? "1" : "0";
            textPropertyArray[12] = boundaryLine.LeftBoundaryLine ? "1" : "0";
            textPropertyArray[13] = boundaryLine.RightBooundaryLine ? "1" : "0";

            //折行
            textPropertyArray[14] = autoWrap ? "1" : "0";
            //打印相关设置：无数据不打印以及固定不打印
            textPropertyArray[15] = this.printStyle.NoDataNoPrint ? "1" : "0";
            textPropertyArray[16] = this.printStyle.NoPrint ? "1" : "0";
            //自动宽度
            textPropertyArray[17] = this.isAdjustWeith ? "1" : "0";
            textPropertyArray[18] = diagonal.ToString();
            textPropertyArray[19] = excursion.ToString();
            textPropertyArray[20] = NameZh_cn;
            textPropertyArray[21] = this.DynamicColumn ? "1" : "0";
            return string.Join(",", textPropertyArray);
        }

        #region ICloneable 成员
        public TextAttibute Clone()
        {
            //克隆丢失动态列属性   
            TextAttibute textAttr = new TextAttibute();
            textAttr.Font = font;
            textAttr.ForeBrush = ForeBrush;
            textAttr.BackgroupBrush = backgroupBrush;
            textAttr.Valign = valign;
            textAttr.BoundaryLine = boundaryLine;
            textAttr.AutoWrap = autoWrap;
            textAttr.PrintStyle = printStyle;
            textAttr.IsAdjustWeith = isAdjustWeith;
            textAttr.Diagonal = diagonal;
            textAttr.Excursion = Excursion;
            textAttr.NameZh_cn = NameZh_cn;
            textAttr.textPropertyArray = this.textPropertyArray;
            textAttr.DynamicColumn = dynamicColumn;
            return textAttr;
        }
        #endregion
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
    public class TextAttibuteTest
    {
        string[] property =  ",,400,0,0,0,0,,2,2,1,1,1,1,0,0,0,0,,,序号,0".Split(',');
        TextAttibute ta = new TextAttibute();
        [Test]
        public void Add()
        {
            ta.Add(property,new Report());
            //Assert.AreEqual(ta.ForeBrush, System.Drawing.Brushes.Black);
            //Assert.AreEqual(ta.BackgroupBrush, System.Drawing.Brushes.White);
            //字位置
            Assert.AreEqual( ta.Valign.Alignment ,System.Drawing.StringAlignment.Center);
            Assert.AreEqual(ta.Valign.LineAlignment, System.Drawing.StringAlignment.Center);
            //上下左右边框
            Assert.True(ta.BoundaryLine.UpperBoundaryLine);
            Assert.True(ta.BoundaryLine.LowerBoundaryLine);
            Assert.True(ta.BoundaryLine.LeftBoundaryLine);
            Assert.True(ta.BoundaryLine.RightBooundaryLine);

            //折行
            Assert.False(ta.AutoWrap);

            //打印相关设置：无数据不打印以及固定不打印
            Assert.False(ta.PrintStyle.NoDataNoPrint);
            Assert.False(ta.PrintStyle.NoPrint);

            //自动宽度
            Assert.False(ta.IsAdjustWeith);
            Assert.AreEqual(ta.NameZh_cn, "序号");
            Assert.False(ta.DynamicColumn);
        }
        [Test]
        public void String()
        {
            ta = new TextAttibute();
            ta.Add(property,new Report());
            Assert.AreEqual(ta.ToString(), string.Join(",", property));
        }
    }
}
#endif


