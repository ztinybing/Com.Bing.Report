using System;
using System.Collections.Generic;
using System.Text; 

namespace Com.Bing.Report
{
    public class Location
    {
        /// <summary>     
        /// 代表起始列
        /// </summary>
        private int x1 = 0;

        public int X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        /// <summary>
        /// 代表起始行
        /// </summary>
        private int y1 = 0;

        public int Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        /// <summary>
        /// 终止列
        /// </summary>
        private int x2 = 1;

        public int X2
        {
            get { return x2; }
            set { x2 = value; }
        }
        /// <summary>
        /// 终止行
        /// </summary>
        private int y2 = 1;

        public int Y2
        {
            get { return y2; }
            set { y2 = value; }
        }

        public void SetLocation(string x1, string y1, string x2, string y2)
        {
            bool valid = int.TryParse(x1, out this.x1) &&
                int.TryParse(y1, out this.y1) &&
                int.TryParse(x2, out this.x2) &&
                int.TryParse(y2, out this.y2);
            if (!valid)
            {
                throw new TextLocationInvalidException();
            }
        }
        public void SetLocation(int x1,int y1 ,int x2 ,int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        /// <summary>
        /// 迁移X方向上的坐标
        /// </summary>        
        public void SetLocationX(int tempX1, int tempX2)
        {
            this.x1 = tempX1;
            this.x2 = tempX2;
        }
        public override string ToString()
        {
            StringBuilder locationStr = new StringBuilder();
            locationStr.Append(x1);
            locationStr.Append(",");
            locationStr.Append(y1);
            locationStr.Append(",");
            locationStr.Append(x2);
            locationStr.Append(",");
            locationStr.Append(y2);
            return locationStr.ToString();
        }
        internal Location Clone()
        {            
            Location newLocation = new Location();
            newLocation.SetLocation(this.x1, this.y1, this.x2, this.y2);
            return newLocation;
        }
    }

}
