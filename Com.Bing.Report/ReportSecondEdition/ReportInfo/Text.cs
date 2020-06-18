using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Com.Bing.Report
{
    /// <summary>
    /// 记录每个框所需的文本属性    
    /// </summary>
    public class Text
    {
        
        //持有报表总体属性以便于访问Report的BAND
        private Report report = null;
        public Report Report
        {
            get { return report; }
            set { report = value; }
        }
        private int textNum = 0;
        public int TextNum
        {
            get { return textNum; }
            set { textNum = value; }
        }
        private int bandIndex = 0;
        public int BandIndex
        {
            get { return bandIndex; }
            set
            {
                bandIndex = value;
            }
        }
        private Location location = new Location();
        public Location Location
        {
            get
            {
                if (report == null)
                {
                    return location;
                }
                else
                {
                    return report.Bands.GetMapLacation(bandIndex, location);
                }
            }
            set
            {
                location = value;
            }
        }
        public Location OriginalLocation
        {
            get { return location; }
        }
        public int RowSpan
        {
            get { return location.Y2 - location.Y1 + 1; }
        }
        public int ColumnSpan
        {
            get { return location.X2 - location.X1 + 1; }
        }
        
        public bool IsEcho
        {
            get
            {
                return (Location != null) && (Location.Y1 != -1);
            }
        }
        private string context = "";
        public string MContext
        {
            get { return context; }
        }
        public string Context
        {
            get
            {
                if (isIncludeMacros)
                    return replaceMacrosContext;
                else
                    return context;
            }
            set
            {
                setMacros(value);
                setFormula(value);
                context = value;
            }
        }
        public string CellText
        {
            get { return context; }
        }
        private bool isFormula = false;
        public bool IsFormula
        {
            get { return isFormula; }
            set { isFormula = value; }
        }
        //一个用于保存函数据名称，一个用于保存计算的列信息
        private string[] formula = null;
        public string[] Formula
        {
            get { return formula; }
            set { formula = value; }
        }
        public string FunColName
        {
            get { return isFormula ? Formula[2] : string.Empty; }
        }
        private bool isIncludeMacros = false;
        public bool IsIncludeMacros
        {
            get { return isIncludeMacros; }
        }
        private List<string> macrosArgsName = new List<string>();
        public List<string> MacrosArgsName
        {
            get { return macrosArgsName; }
        }
        private List<string> macrosContext = new List<string>();
        public List<string> MacrosContext
        {
            get { return macrosContext; }
            set
            {
                if (macrosContext == value)
                    return;
                macrosContext = value;
                int i = 0;
                replaceMacrosContext = context;
                foreach (string s in macrosArgsName)
                {
                    replaceMacrosContext = replaceMacrosContext.Replace("[" + s + "]", value[i]);
                    i++;
                }
            }
        }
        private string replaceMacrosContext = string.Empty;
        private TextAttibute attribute = new TextAttibute();
        public TextAttibute Attribute
        {
            get { return attribute; }
            set { attribute = value; }
        }
        private void setMacros(string name)
        {
            if (Regex.IsMatch(name, @"\[\w+\]"))
            {
                isIncludeMacros = true;
                MatchCollection matches = Regex.Matches(name, @"\[\w+\]");
                foreach (Match m in matches)
                {
                    macrosArgsName.Add(Regex.Replace(m.Value, @"\[|\]", string.Empty));
                }
            }
            else
            {
                isIncludeMacros = false;
                macrosArgsName = null;
            }

        }
        private void setFormula(string value)
        {
            if (Regex.IsMatch(value, @"\$.*\$"))
            {
                IsFormula = true;
                value = Regex.Replace(value, @"\$", "");
                Formula = Regex.Split(value, ":");
                if (Formula.Length != 3)
                {
                    throw new FormulaWrongException(value);
                }
            }
        }
        private int contextWidth = 0;
        public int ContextWidth
        {
            get { return contextWidth; }
            set { contextWidth = value; }
        }
        /// <summary>
        /// 用于动态列扩展TExt的X座标用
        /// </summary>        
        public void SetLocationX(int tempX1, int tempX2)
        {
            location.SetLocationX(tempX1, tempX2);
        }
     
        internal Text CopyInNewLoction(int X1, int X2)
        {
            Text cloneText = this.Clone();
            cloneText.SetLocationX(X1, X2);
            return cloneText;
        }
        public Text Clone()
        {
            Text cloneText = new Text();
            cloneText.textNum = this.textNum;
            cloneText.bandIndex = this.bandIndex;
            cloneText.location = this.location.Clone();
            cloneText.context = this.context;
            cloneText.isFormula = this.isFormula;
            cloneText.isIncludeMacros = this.isIncludeMacros;
            if(this.macrosArgsName != null)
                cloneText.macrosArgsName.AddRange(this.macrosArgsName);
            cloneText.MacrosContext.AddRange(this.MacrosContext);
            cloneText.replaceMacrosContext = this.replaceMacrosContext;
            cloneText.attribute = this.attribute.Clone();
            cloneText.contextWidth = this.contextWidth;
            return cloneText;
        }
        public override string ToString()
        {
            StringBuilder textStr = new StringBuilder();
            textStr.Append(bandIndex);
            textStr.Append(",");
            textStr.Append(location.ToString());
            textStr.Append(",");
            textStr.Append(ConvertUtil.ReplaceRN( context));
            textStr.Append(",");
            textStr.Append(Attribute.ToString());
            return textStr.ToString();
        }

    }
}
