using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{

    /// <summary>
    /// 字体框里面要画的框线(分别为上、下、左、右)
    /// </summary>
    public class BoundaryLine
    {
        private bool upperBoundaryLine = true;

        public bool UpperBoundaryLine
        {
            get { return upperBoundaryLine; }
            set { upperBoundaryLine = value; }
        }
        private bool lowerBoundaryLine = true;

        public bool LowerBoundaryLine
        {
            get { return lowerBoundaryLine; }
            set { lowerBoundaryLine = value; }
        }
        private bool leftBoundaryLine = true;

        public bool LeftBoundaryLine
        {
            get { return leftBoundaryLine; }
            set { leftBoundaryLine = value; }
        }
        private bool rightBooundaryLine = true;

        public bool RightBooundaryLine
        {
            get { return rightBooundaryLine; }
            set { rightBooundaryLine = value; }
        }


        //正斜线
        private bool isSlash;
        public bool IsSlash
        {
            get { return isSlash; }
            set { isSlash = value; }
        }
        //反斜线
        private bool isBackSlash;
        public bool IsBackSlash
        {
            get { return isBackSlash; }
            set { isBackSlash = value; }
        }
        //十字线
        private bool isCrossLine;
        public bool IsCrossLine
        {
            get { return isCrossLine; }
            set { isCrossLine = value; }
        }


        internal BoundaryLine Clone()
        {
            BoundaryLine obj = new BoundaryLine();
            obj.UpperBoundaryLine = this.upperBoundaryLine;
            obj.LowerBoundaryLine = this.lowerBoundaryLine;
            obj.LeftBoundaryLine = this.leftBoundaryLine;
            obj.RightBooundaryLine = this.rightBooundaryLine;
            obj.IsSlash = this.isSlash;
            obj.IsBackSlash = this.isBackSlash;
            obj.IsCrossLine = this.isCrossLine;
            return obj;
        }
        public void SetNoBoundary()
        {
            upperBoundaryLine = false;
            lowerBoundaryLine = false;
            leftBoundaryLine = false;
            rightBooundaryLine = false;
        }
        public void SetBoundary()
        {
            upperBoundaryLine = true;
            lowerBoundaryLine = true;
            leftBoundaryLine = true;
            rightBooundaryLine = true;
        }
    }
}
