using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class Texts : List<Text>
    {
        Report report = null;
        public Texts() { }
        public Texts(Report report) { this.report = report; }
        public Texts GetTextsBy(int bandIndex, int rowIndex)
        {
            //行Text应该以从左到右的顺序排列
            Texts searchTexts = new Texts();
            foreach (Text text in this)
            {
                if (text.BandIndex == bandIndex && text.Location.Y1 == rowIndex)
                {
                    searchTexts.Add(text);
                }
            }
            return searchTexts;
        }
        public void ClearTexts(int BandID)
        {
            for (int index = Count - 1; index >= 0; index--)
            {
                if (this[index].BandIndex == BandID)
                {
                    this.RemoveAt(index);
                }
            }
        }
        public new void Add(Text t)
        {
            if (this.report != null)
                t.Report = this.report;
            base.Add(t);
        }
        public List<Text> FunTexts()
        {
            List<Text> funTexts = new List<Text>();
            foreach (Text text in this)
            {
                if (text.IsFormula)
                {
                    funTexts.Add(text);
                }
            }
            return funTexts;
        }
        /// <summary>
        /// 利用报表中的TEXT属性添加Text
        /// </summary>
        ///<example>
        /// 文本1 所在Band 索引,X1,Y1,X2,Y2,文本内容,属性（属性取值为FF 字体、FN 大
        ///小、B 加粗、I 斜体、U 下划、S 中划、FC 前景色、BC 背景色、HA 横对齐、VA 纵对齐、
        ///上边框线、下边框线、左边框线、右边框线、自动换行、无数据不打印、固定不打印、自动
        ///调整宽度、对角线、X 偏移、列中文含义）
        /// 1,1,1,6,2,工程量清单单价分析表,,-18,700,,,,,16777215,2,2,0,0,0,0,0,0,0,1,0,0
        ///</example>
        public void Add(string textInfo)
        {
            Text text = new Text();
            string[] temparray = textInfo.Split(',');
            if (temparray.Length < 6)
            {
                throw new TextInfoLostException();
            }
            int bandIndex = 0;
            if (!int.TryParse(temparray[0], out bandIndex))
            {
                throw new TextBandIndexInvalidException();
            }
            text.BandIndex = bandIndex;
            text.Location.SetLocation(temparray[1], temparray[2], temparray[3], temparray[4]);
            text.Context = ConvertUtil.ReplaceCRLF(temparray[5]);

            int lenght = temparray.Length - 6;
            string[] initArray = new string[TextAttibute.PROPERTYCOUNT];
            for (int i = 0; i < initArray.Length; i++)
            {
                if (lenght > i)
                {
                    //+6是去前面Band+Location+Context
                    initArray[i] = temparray[i + 6];
                }
                else
                {
                    initArray[i] = "";
                }
            }
            text.Attribute.Add(initArray, report);
            this.Add(text);
            if (text.BandIndex == Bands.BodyBandID && text.Location.Y2 > report.Bands.GetCount(Bands.BodyBandID))
            {
                report.Bands.BandDict[Bands.BodyBandID].BandCount = text.Location.Y2;
            }
        }
        public Texts GetTextsBy(int bandIndex)
        {
            Texts bandTexts = new Texts();
            foreach (Text text in this)
            {
                if (text.BandIndex == bandIndex)
                {
                    bandTexts.Add(text);
                }
            }
            return bandTexts;
        }
    }
}


