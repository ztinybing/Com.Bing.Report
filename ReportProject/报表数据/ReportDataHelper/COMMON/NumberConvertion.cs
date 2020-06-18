using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.Bing
{
    public class NumberConvertion
    {
        #region DigitalToChina
        public NumberConvertion(int integer)
        {
            foreach (UnitItem item in unitList)
            {
                int mod = integer / item.Value;
                if (mod > 0)
                {
                    Left = new NumberConvertion(mod);
                    Right = new NumberConvertion(integer - mod * item.Value);
                    Top = item.Name;
                    break;
                }
            }
            if (Left == null && Right == null)
            {
                Top = getSimpleDigial(integer);
            }
        }

        public string Top;

        public NumberConvertion Left;

        public NumberConvertion Right;

        static List<UnitItem> unitList = null;

        static string[] BaseChineseDigital = null;

        static NumberConvertion()
        {
            BaseChineseDigital = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            unitList = new List<UnitItem>();
            unitList.Add(new UnitItem("亿", 100000000));
            unitList.Add(new UnitItem("万", 10000));
            unitList.Add(new UnitItem("千", 1000));
            unitList.Add(new UnitItem("百", 100));
            unitList.Add(new UnitItem("十", 10));
        }

        private string getString()
        {
            string strLeft = "", strRight = "";
            if (this.Right != null)
            {
                strRight = this.Right.getString();
            }
            if (this.Left != null)
            {
                strLeft = this.Left.getString();
            }
            return string.Format("{0}{1}{2}", strLeft, Top, strRight);
        }

        /// <summary>
        /// 将正整数转换成中文(适用与目录小写转换为大写)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>exp:123 -> 一百二十三</returns>
        public static string ToUpper(int value)
        {
            string str = value.ToString();
            if (value > 0)
                str = System.Text.RegularExpressions.Regex.Replace(new NumberConvertion(value).getString(), "^一十", "十");
            return str;
        }

        private string getSimpleDigial(int value)
        {
            for (int i = 1; i < 10; i++)
            {
                if (i == value)
                {
                    return BaseChineseDigital[i - 1];
                }
            }
            return "";
        }
        #endregion

        #region ChinaToDigital
        //todo:类似可以做
        private static int ToLower(string chinaStr)
        {


            return 1;
        }
        #endregion

        private struct UnitItem
        {
            public UnitItem(string name, int value)
            {
                this.Name = name;
                this.Value = value;
            }

            public string Name;
            public int Value;
        }

    }
}
