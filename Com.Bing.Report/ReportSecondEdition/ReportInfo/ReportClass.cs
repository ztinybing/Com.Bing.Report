using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bing.Report
{ 
    /// <summary>
    /// 报表类型枚举类
    /// 1:Grid
    /// 0:FreeForm
    /// </summary>
    public class ReportClass
    {
        int typeID = 0;
        public int TypeID
        {
            get { return typeID; }            
        }
        private ReportClass(int typeID) { this.typeID = typeID; }
        private static readonly ReportClass grid = new ReportClass(0);

        public static ReportClass Grid
        {
            get { return grid; }
        } 
        private static readonly ReportClass freeForm = new ReportClass(1);

        public static ReportClass FreeForm
        {
            get { return freeForm; }
        }
        private static readonly ReportClass bigText = new ReportClass(2);
        public static ReportClass BigText
        {
            get { return bigText; }
        }
        public static ReportClass GetClass(string classID)
        {
            int id = 0;
            if (!int.TryParse(classID, out id))
            {
                throw new ReportTypeIDInvalidException(classID);
            }
#if DEBUG
            if (id != 1 && id != 0&&id != 2)
            {
                throw new ReportTypeIDInvalidException(classID);
            }
#endif
            ReportClass reportClass = Grid;
            if (freeForm.TypeID == id)
            {
                reportClass = FreeForm;
            }   
            else if (bigText.typeID == id)
            {
                reportClass = BigText;
            }
            return reportClass;
        }
        public override string ToString()
        {
            return string.Format("d001={0}", typeID);   
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
    public class ReportClassTest
    {
        [Test]
        public void GetClass()
        {
            Assert.True(ReportClass.Grid ==ReportClass.GetClass("1"));
            Assert.True(ReportClass.FreeForm ==ReportClass.GetClass("0"));
        }
        [Test]
        [ExpectedException(typeof(ReportTypeIDInvalidException))]
        public void TestException()
        {
            ReportClass.GetClass("-1");
        }
    }
}
#endif

