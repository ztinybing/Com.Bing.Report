using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Com.Bing.Report
{
    public class DataAnaysisException : Exception
    {

    }
    public class FlagRowInvalidException : DataAnaysisException
    {

    }
    public class BigTextAnalyseException : DataAnaysisException
    {
        string err = string.Empty;
        public override string Message
        {
            get
            {
                return err; ;
            }
        }
        public BigTextAnalyseException(string err)
        {
            this.err = err;
        }
    }
    public class CellLocationInvalidExcption : DataAnaysisException
    {
        string err = string.Empty;
        public override string Message
        {
            get
            {
                return string.Format("设计器单元格填充内容出错,出错单元格：\r\n{0}", err);
            }
        }
        public CellLocationInvalidExcption(string err)
        {
            this.err = err;
        }
    }
    public class RowGounpInvalidException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "设计的RowGroup内容无效，请使用数字!";
            }
        }
    }
    public class SinglePageException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "单页报表无法容纳所有数据信息，请调整报表行高或改用更大的打印纸张!";
            }
        }
    }
    public class FunSumColNotExistsException : DataAnaysisException
    {
        private string err = "";
        public override string Message
        {
            get
            {
                return string.Format("页汇的计算公式列不存在，不存在的列为：{0}", err);
            }
        }
        public FunSumColNotExistsException(string err)
        {
            this.err = err;
        }
    }

    public class OutOfPageIndexException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "获取的报表页数超出报表总页数!";
            }
        }
    }
    public class BodyColumnInvalidException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "报表数据出现无效列!";
            }
        }
    }
    public class BandIDInvalidException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "获取报表BINDID出错!";
            }
        }
    }
    public class PaginationCountInvalidException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "横向分页固定列数超过显示列数!";
            }
        }
    }
    public class FixedColumnWidthToBigException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "横向分页固定列宽度总和过大，导致横向分页失败!";
            }
        }
    }
    public class ExBodyHeight2BigException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                //Grid
                return "非数据区所占高度过量!";
            }
        }
    }
    public class ExBodyWidth2BigException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                //FreeGrid
                return "自由风格报表不能进行横向分页";
            }
        }
    }
    public class TogetherRowHeight2BigException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "报表在一起的行过高，竖向分页失败!";
            }
        }
    }
    public class BodyRow2HighException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "报表数据行行高过高，竖向分页失败!";
            }
        }
    }
    public class ColumnNumInvalidException : DataAnaysisException
    {
        public override string Message
        {
            get
            {
                return "数据列与Cell位置不对应";
            }
        }
    }
}
