using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{
    public class Band
    {
        private int id = -1;
        public Band(int id)
        {
            this.id = id;
        }
        private int bandCount = 0;
        public int BandCount
        {
            get { return bandCount; }
            set { bandCount = value; }
        }

        public int EchoCount
        {
            get
            {
                if (id == Bands.BodyBandID)
                {
                    return bandCount;
                }
                return bandCount - echoRowDict.Count;
            }
        }
        private Dictionary<int, bool> echoRowDict = new Dictionary<int, bool>();
        public Dictionary<int, bool> EchoRowDict
        {
            get { return echoRowDict; }
        }


    }
    public class Bands
    {
        public const int HeadBandID = 1;
        public const int TitleBandID = 2;
        public const int BodyBandID = 3;
        public const int PageBandID = 4;
        public const int RootBandID = 5;
        public Dictionary<int, Band> BandDict = new Dictionary<int, Band>();
        public int this[int bandID]
        {
            get
            {
                Band band = null;
                if (BandDict.TryGetValue(bandID, out band))
                {
                    return band.EchoCount;
                }
                throw new BandIDInvalidException();
            }
        }
        public Bands()
        {
            for (int i = HeadBandID; i <= RootBandID; i++)
            {
                BandDict.Add(i, new Band(i));
            }
        }

        public void SetBindRowNum(string bandInfo, string countInfo)
        {
            int bandIndex = 0;
            int count = 0;
            bool invalid = int.TryParse(bandInfo.Substring(4, 4), out bandIndex) && int.TryParse(countInfo, out count);
            if (!invalid)
            {
                throw new ReportBandNumInvalidException();
            }
            SetBindRowNum(bandIndex, count);
        }
        //初始化报表的band属性，包括行数，显示的行等
        public void SetBindRowNum(int bandIndex, int num)
        {
            if (bandIndex <= RootBandID && bandIndex != BodyBandID)
            {
                BandDict[bandIndex].BandCount = num;
            }
        }
        public void SetBindRowEcho(string bandRowInfo, string echoIno)
        {
            //2->所有BAND区 1所在行 （0不显示 1显示）
            //d00400020001=0
            int bandIndex = 0;
            int rowIndex = 0;
            bool infoInvalid = int.TryParse(bandRowInfo.Substring(4, 4), out bandIndex) && int.TryParse(bandRowInfo.Substring(8, 4), out rowIndex);
            if (!infoInvalid)
            {
                throw new ReportBandEchoRowInvalidException();
            }
            SetBindRowEcho(bandIndex, rowIndex, echoIno == "1");
        }

        public void SetBindRowEcho(int bandIndex, int rowNum, bool echo)
        {
            //只记录不显示的行
            if (echo)
            {
                return;
            }
            BandDict[bandIndex].EchoRowDict[rowNum] = echo;
        }
        public void ClearEchoDic()
        {
            foreach (int key in BandDict.Keys)
            {
                BandDict[key].EchoRowDict.Clear();
            }
        }
        public Location GetMapLacation(int bandIndex, Location location)
        {
            Location mapLocation = null;
            switch (bandIndex)
            {
                case 1:
                case 2:
                case 4:
                case 5:
                    mapLocation = GetMap(BandDict[bandIndex].EchoRowDict, location);
                    break;
                case 3:
                    mapLocation = location;
                    break;
                default:
                    mapLocation = new Location();
                    mapLocation.SetLocation(-1, -1, -1, -1);
                    break;
            }
            return mapLocation;
        }
        private Location GetMap(Dictionary<int, bool> BandEchoRowDict, Location location)
        {
            Location tempLocation = new Location();
            tempLocation.SetLocation(location.X1, location.Y1, location.X2, location.Y2);

            int Y1 = location.Y1;

            if (BandEchoRowDict.ContainsKey(location.Y1))
            {
                tempLocation.SetLocation(-1, -1, -1, -1);
                return tempLocation;
            }

            for (int i = 1; i <= location.Y2; i++)
            {
                if (BandEchoRowDict.ContainsKey(i))
                {
                    tempLocation.Y2--;
                    if (Y1 >= i)
                    {
                        tempLocation.Y1--;
                    }
                }
            }
            return tempLocation;
        }
        public override string ToString()
        {
            StringBuilder bandStr = new StringBuilder();
            for (int i = 1; i < 6; i++)
            {
                bandStr.AppendLine(string.Format("d004000{0}={1}", i, BandDict[i].BandCount));
            }
            echoRowInfo(bandStr, 1);
            echoRowInfo(bandStr, 2);
            echoRowInfo(bandStr, 4);
            echoRowInfo(bandStr, 5);
            return bandStr.ToString();
        }
        private void echoRowInfo(StringBuilder bandStr, int bandIndex)
        {
            foreach (int echoIndex in BandDict[bandIndex].EchoRowDict.Keys)
            {
                if (!BandDict[bandIndex].EchoRowDict[echoIndex])
                {
                    bandStr.AppendLine(string.Format("d004000{0}{1:D4}=0", bandIndex, echoIndex));
                }
            }
        }
        internal void CopyRootTo(Bands band)
        {
            //band .rootBand = rootBand;
            //band.rootEchoRowDict.Clear();
            //foreach (int rowIndex in rootEchoRowDict.Keys)
            //{
            //    band.rootEchoRowDict.Add(rowIndex, rootEchoRowDict[rowIndex]);
            //}
        }
        /// <summary>
        /// y1未去掉隐藏行的
        /// </summary>        
        internal int GetExBodyRowIndex(int BandID, int y1)
        {
            int preBindRowCount = 0;
            for (int id = 1; id < BandID; id++)
            {
                if (id == BodyBandID)
                {
                    continue;
                }



                preBindRowCount += this[id];
            }

            int tempY = y1;
            foreach (int rowIndex in BandDict[BandID].EchoRowDict.Keys)
            {
                if (!BandDict[BandID].EchoRowDict[rowIndex] && rowIndex < tempY)
                {
                    y1--;
                }
            }

            return preBindRowCount + y1;
        }

        internal int GetCount(int bandId)
        {
            return BandDict[bandId].BandCount;
        }
    }
}