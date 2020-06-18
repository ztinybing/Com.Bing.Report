using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Com.Bing.Report
{
    public class ConvertUtil
    {
        private ConvertUtil() { }

        public static int GetBoundary(string distance)
        {
            int margin = 201;
            //暂时还没有找到转换工式,设个初始值
            if (!string.IsNullOrEmpty(distance))
            {
                if (!int.TryParse(distance, out margin))
                {
                    throw new ReportMarginInvalidException();
                }
            }
            return DrawUtil.MmTopix(margin / 10);
        }
        public static int GetBoundaryMM(int marge)
        {
            return DrawUtil.PixTOMm(marge) * 10;
        }

        public static int GetGrainDirection(string direction)
        {
            //1：表示竖向
            //2: 表示横向
            int intDirection = 0;
            if (!string.IsNullOrEmpty(direction))
            {
                if (!int.TryParse(direction, out intDirection))
                {
                    throw new ReportGrainDirectionInvalidException();
                }
            }
            return intDirection;
        }
        public static Font GetFont(string family, string size)
        {
            float fontSize = 9.0f;
            if (family.Equals(string.Empty))
                family = "宋体";
            if (!size.Equals(string.Empty))
            {
                if (!float.TryParse(size, out fontSize))
                {
                    throw new FontSizeInvalidException();
                }
            }
            return new Font(family, Math.Abs(fontSize));
        }
        public static Font GetFont(string family, string size, string bold, string italic, string underline, string strikout, Report report)
        {
            //为空则取报表整体的属性设置
            if (family.Equals(string.Empty))
                family = report.Attributes.Font.Name;
            if (size.Equals(string.Empty) || size == "-")
                size = report.Attributes.Font.Size.ToString();
            Font font = GetFont(family, size);
            //初始化列字体的字体样式 ， 如粗体、斜体、下划线、中划线
            FontStyle fontStyle = FontStyle.Regular;
            if (bold.Equals("700"))
                fontStyle |= FontStyle.Bold;
            if (italic.Equals("1"))
                fontStyle |= System.Drawing.FontStyle.Italic;
            if (underline.Equals("1"))
                fontStyle |= FontStyle.Underline;
            if (strikout.Equals("1"))
                fontStyle |= FontStyle.Strikeout;
            font = new Font(font, fontStyle);
            return font;
        }
        internal static string[] FontInfoTo(Font font)
        {
            string[] fontInfo = new string[6];
            fontInfo[0] = font.Name;
            fontInfo[1] = (-font.Size).ToString();
            fontInfo[2] = (font.Style & FontStyle.Bold) == FontStyle.Bold ? "700" : "";
            fontInfo[3] = (font.Style & FontStyle.Italic) == FontStyle.Italic ? "1" : "0";
            fontInfo[4] = (font.Style & FontStyle.Underline) == FontStyle.Underline ? "1" : "0";
            fontInfo[5] = (font.Style & FontStyle.Strikeout) == FontStyle.Strikeout ? "1" : "0";
            return fontInfo;
        }

        public static float GetRowHeight(string height)
        {
            float multipleHeight = 2.00f;
            //返回的为行高度的倍数，行高度由字体大小决定
            if (!string.IsNullOrEmpty(height))
            {
                if (!float.TryParse(height, out multipleHeight))
                {
                    throw new ReportRowMultiHeigthInvalidException();
                }
            }
            return multipleHeight;
        }
        public static int GetPagination(string pagination)
        {
            //横向分页设置
            int landscapePaging = 0;
            if (!pagination.Equals(string.Empty))
            {
                if (!int.TryParse(pagination, out landscapePaging))
                {
                    throw new ReportLandscapePagingInvalidException();
                }
            }
            return landscapePaging;
        }
        public static float GetLineWidth(string linewidth)
        {
            //表格线宽
            float fLineWidth = 0.1f;
            if (!string.IsNullOrEmpty(linewidth))
            {
                if (!float.TryParse(linewidth, out fLineWidth))
                {
                    throw new ReportLineWidthInvalidException();
                }
            }
            return fLineWidth;
        }
        public static PrintStyle GetPrintStyle(string printLast, string briefPrint)
        {
            //简洁报表样式、页脚最后一页打印            
            if (string.IsNullOrEmpty(printLast))
            {
                printLast = "0";
            }
            if (string.IsNullOrEmpty(briefPrint))
            {
                briefPrint = "0";
            }
            PrintStyle printStyle = new PrintStyle();
            printStyle.IsPrintInLast = "1" == printLast;
            printStyle.IsBrifeStyle = "1" == briefPrint;
            return printStyle;
        }
        public static Brush GetBrush(string Color)
        {
            int intColor = 16646655;
            //前景色、背景色,初始化画笔 
            if (!string.IsNullOrEmpty(Color))
            {
                if (!int.TryParse(Color, out intColor))
                {
                    throw new ReportColorValInvalidException();
                }
            }

            return new SolidBrush(getColorFromBGR(intColor));
        }
        private static Color getColorFromBGR(int bgr)
        {
            bgr = Math.Abs(bgr);
            int r = 255, g = 255, b = 255;
            if (bgr > 16646655)
                bgr = 16646655;
            if (bgr < 16646655)
            {
                b = bgr > 16581375 ? b = Convert.ToInt32(Math.Floor(bgr / (double)(255 * 255))) : 0;
                g = (bgr - b * 255 * 255) > 255 && (bgr - b * 255 * 255) < 65026 ?

                Convert.ToInt32(Math.Floor((bgr - b * 255 * 255) / (double)255)) : 0;
                r = (bgr - b * 255 * 255 - g * 255) < 256 ? (bgr - b * 255 * 255 - g * 255) : 0;
            }
            return Color.FromArgb(r, g, b);
        }
        internal static string BrushTo(Brush brush)
        {
            SolidBrush solidBrush = brush as SolidBrush;
            return GetIntFromColor(solidBrush.Color).ToString(); ;
        }
        private static int GetIntFromColor(Color color)
        {
            return color.ToArgb() & 0x00FFFFFF;
        }
        public static StringFormat GetStringFormat(string halign, string valign)
        {

            //0:居右
            //1:居左
            //2：居中
            StringFormat stringFormat = new StringFormat();
            //水平方向
            if (halign.Equals("0"))
                stringFormat.LineAlignment = StringAlignment.Near;
            else if (halign.Equals("1"))
                stringFormat.LineAlignment = StringAlignment.Far;
            else
                stringFormat.LineAlignment = StringAlignment.Center;
            //竖直方向
            if (valign.Equals("0"))
                stringFormat.Alignment = StringAlignment.Near;
            else if (valign.Equals("1"))
                stringFormat.Alignment = StringAlignment.Far;
            else
                stringFormat.Alignment = StringAlignment.Center;

            return stringFormat;
        }
        internal static string AlignTo(StringAlignment stringAlignment)
        {
            string alignInfo = string.Empty;
            switch (stringAlignment)
            {
                case StringAlignment.Near:
                    alignInfo = "0";
                    break;
                case StringAlignment.Far:
                    alignInfo = "1";
                    break;
                case StringAlignment.Center:
                    alignInfo = "2";
                    break;
            }
            return alignInfo;
        }
        //将PB中的回车换行字符（char(13)char(10)）换成C#中的回车换行符(\r\n)
        public static string ReplaceCRLF(string s)
        {
            return s.Replace("char(13)char(10)", "\r\n");
        }
        public static string ReplaceRN(string s)
        {
            return s.Replace("\r\n", "char(13)char(10)");
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
    public class ConvertUtilTest
    {
        [Test]
        public void RepalceCRLF()
        {
            Assert.AreEqual(ConvertUtil.ReplaceCRLF("mchar(13)char(10)sxx"), "m\r\nsxx");
        }
        [Test]
        public void ReplaceRN()
        {
            Assert.AreEqual("mchar(13)char(10)sxx", ConvertUtil.ReplaceRN("m\r\nsxx"));
        }
        [Test]
        public void GetStringFormat()
        {
            //0：居中
            
            
            StringFormat sf = ConvertUtil.GetStringFormat("2","2");
            Assert.AreEqual(StringAlignment.Center, sf.Alignment);
            Assert.AreEqual(StringAlignment.Center, sf.LineAlignment);

            //1:居右
            sf = ConvertUtil.GetStringFormat("0","0");
            Assert.AreEqual(StringAlignment.Near, sf.Alignment);
            Assert.AreEqual(StringAlignment.Near, sf.LineAlignment);

            //2:居左
            sf = ConvertUtil.GetStringFormat("1","1");
            Assert.AreEqual(StringAlignment.Far, sf.Alignment);
            Assert.AreEqual(StringAlignment.Far, sf.LineAlignment);
        }
        [Test]
        public void getColorFromBGR()
        {
            //测定几种特定的颜色
        }
        [Test]
        public void GetPrintStyle()
        {
            PrintStyle ps = ConvertUtil.GetPrintStyle("1", "1");
            Assert.IsTrue(ps.IsBrifeStyle);
            Assert.IsTrue(ps.IsPrintInLast);

            ps = ConvertUtil.GetPrintStyle("", "");
            Assert.False(ps.IsBrifeStyle);
            Assert.False(ps.IsPrintInLast);

            ps = ConvertUtil.GetPrintStyle("0", "0");
            Assert.False(ps.IsBrifeStyle);
            Assert.False(ps.IsPrintInLast);

        }
        [Test]        
        public void GetLineWidth()
        {
            Assert.AreEqual(ConvertUtil.GetLineWidth(""), 0.1f);
            Assert.AreEqual(ConvertUtil.GetLineWidth("0.2"), 0.2f);
            Assert.AreEqual(ConvertUtil.GetLineWidth(null), 0.1f);            
        }
        [Test]
        public void GetPagination()
        {
            Assert.AreEqual(1, ConvertUtil.GetPagination("1"));
            Assert.AreEqual(0, ConvertUtil.GetPagination("0"));
        }
        [Test]
        public void GetRowHeight()
        {
            Assert.AreEqual(2.0f, ConvertUtil.GetRowHeight(""));
            Assert.AreEqual(1.25f, ConvertUtil.GetRowHeight("1.25"));
        }
        [Test]
        public void GetGrainDirection()
        {
            //0 null：表示竖向
            //1: 表示横向
            Assert.AreEqual(0, ConvertUtil.GetGrainDirection(""));
        }
        [Test]
        public void GetBoundary()
        {
            Assert.AreEqual(201, ConvertUtil.GetBoundary(""));
        }
    }
}
#endif
