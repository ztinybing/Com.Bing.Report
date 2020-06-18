using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;


using System.Drawing.Printing;

namespace Com.Bing.Report
{
    public class DrawUtil
    {
        public static void drawText(Graphics g, RectangleF range, string context, TextAttibute style)
        {
            Font drawFont = style.Font;
            Brush drawBrush = new HatchBrush(HatchStyle.SmallGrid, Color.Black);
            Pen blackPen = new Pen(Color.Black);
            //g.DrawRectangle(blackPen, x, y, width, height);            
            StringFormat drawFormat = new StringFormat();
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.FormatFlags = StringFormatFlags.NoClip;
            g.DrawString(context, drawFont, drawBrush, range, drawFormat);
        }
        public static int MmTopix(double Mm)
        {
            Unit unit = new Unit(Mm, UnitTypes.Mm);
            return Convert.ToInt32(unit.To(UnitTypes.Px).Value);
        }
        public static int PixTOMm(double pix)
        {
            Unit unit = new Unit(pix, UnitTypes.Px);
            return Convert.ToInt32(unit.To(UnitTypes.Mm).Value);
        }
        #region 自身实现的MeasureString 供其它地方进行调用，不在实时变更
        public static SizeF MeasureString(string context, Font font, int width, Graphics g, float heightMuti)
        {
            //测量多行时不考虑行倍数问题
            SizeF sizeF;
            sizeF = g.MeasureString(context, font, width);
			//字体与表格线之间留部分空隙
            sizeF.Height = sizeF.Height + font.Height / 4;
            return sizeF;
        }
        public static SizeF MeasureString(string context, Font font, Graphics g, float heightMuti)
        {
            SizeF sizeF;
            sizeF = g.MeasureString(context, font);
            sizeF.Height = sizeF.Height + font.Height * (heightMuti - 1);
            return sizeF;
        }
        #endregion
        public static string RemoveSpecialCharater(string ori)
        {
            //去除如下字符 /\?*:
            //将字符[]替换成()
            //长度不超过31，超过自动截取
            string[] ostring = new string[] { "/", "\\", "?", "*", ":", "[", "]" };
            string[] nstring = new string[] { "", "", "", "", "", "(", ")" };
            for (int i = 0; i < ostring.Length; i++)
            {
                ori = ori.Replace(ostring[i], nstring[i]);
            }
            return ori;
        }
        public static void ExpandLenghtCala(double[] colnumsWidthd, ref int[] colnumsWidth)
        {
            double preLenghtd = 0;
            int preLenght = 0;
            for (int i = 0; i < colnumsWidthd.Length; i++)
            {
                if (i > 0 && colnumsWidthd[i] != 0 && preLenghtd != 0)
                    preLenght += colnumsWidth[i - 1];
                preLenghtd += colnumsWidthd[i];
                if (colnumsWidthd[i] != 0)
                {
                    colnumsWidth[i] = Convert.ToInt32(Math.Floor(preLenghtd - preLenght));
                }
            }
        }
    }
}


