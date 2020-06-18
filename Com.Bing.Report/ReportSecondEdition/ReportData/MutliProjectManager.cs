using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;

namespace Com.Bing.Report
{
    public delegate void CheckProjectNodeHandler(List<KeyValuePair<List<DataRow>, string>> multiProjectList, int projectIndex);
    public delegate void ReCheckProjectNodeHandler(List<KeyValuePair<List<DataRow>, string>> multiProjectList);
    public delegate Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> InitMutliProjectHandler();
    public class MutliProjectManager
    {
        private object projectData = null;
        public object ProjectData
        {
            get { return projectData; }
            set { projectData = value; }
        }
        public event CheckProjectNodeHandler CheckProjectNode = null;
        public event InitMutliProjectHandler InitMutliProject = null;
        public event ReCheckProjectNodeHandler ReCheckProjectNode = null;
        public List<KeyValuePair<List<DataRow>, string>> this[int rptLevel]
        {
            get
            {
                Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> mutliProjectStruct = InitMutliProject();
                if (mutliProjectStruct.ContainsKey(rptLevel))
                {
                    return mutliProjectStruct[rptLevel];
                }
                return new List<KeyValuePair<List<DataRow>, string>>();
            }
        }
        public bool CheckValidity()
        {
            return CheckProjectNode != null && InitMutliProject != null && ReCheckProjectNode != null;
        }

        #region 相当于事务，需成对出现

        List<KeyValuePair<List<DataRow>, string>> atomicityCheck = null;

        internal bool StartCheckProject(int level, int prjIndex)
        {
            bool check = true;
            atomicityCheck = this[level];
            if (prjIndex <= atomicityCheck.Count)
            {
                CheckProjectNode(atomicityCheck, prjIndex);
            }
            else
            {
                check = false;
            }
            return check;
            //return true;
        }
        internal void EndCheckProject()
        {
            ReCheckProjectNode(atomicityCheck);
            atomicityCheck = null;
        }

        #endregion

        /// <summary>
        /// mutliProjectStruct:
        ///               Level  projectIndex subRow
        ///  项目         3        0       row1 row2 row3 row4
        ///    单项       2        0           row1 row2
        ///       单位    1        0           row1
        ///       单位    1        1           row2
        ///    单项       2        1         row1 row2
        ///       单位    1        2           row3
        ///       单位    1        3           row4
        /// 输出:   (1 3) (1 2) (2 1) (1 1) (1 0) (2 0) (3 0)
        /// </summary>        
        internal List<Point> GetByProjectOrder()
        {
            List<Point> projectIndexDict = new List<Point>();
            Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> mutliProjectStruct = InitMutliProject();
            //批量打印的报表数据 --工程顺序模式                
            Stack<Point> projectCheckStack = new Stack<Point>();
            //用于记录List<dataRow> 的levelIndex与projectIndex                
            for (int projectIndex = mutliProjectStruct[mutliProjectStruct.Count].Count - 1; projectIndex >= 0; projectIndex--)
            {   
                projectCheckStack.Push(new Point(mutliProjectStruct.Count, projectIndex));

            }

            while (projectCheckStack.Count != 0)
            {
                Point checkPoint = projectCheckStack.Pop();
                int curLeveIndex = checkPoint.X;
                int curProjectIndex = checkPoint.Y;
                projectIndexDict.Add(checkPoint);
                if (mutliProjectStruct.ContainsKey(curLeveIndex - 1))
                {
                    for (int projectIndex = mutliProjectStruct[curLeveIndex - 1].Count - 1; projectIndex >= 0; projectIndex--)
                    {
                        List<DataRow> projectCheckRows = mutliProjectStruct[checkPoint.X][checkPoint.Y].Key;
                        if (projectCheckRows.IndexOf(mutliProjectStruct[curLeveIndex - 1][projectIndex].Key[0]) < 0)
                        {
                            continue;
                        }                        
                        projectCheckStack.Push(new Point(curLeveIndex -1, projectIndex));

                    }
                }
                
            }
            return projectIndexDict;
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
    public class MutliProjectManagerTest
    {
        MutliProjectManager projectManager = new MutliProjectManager();        
        [SetUp]
        public void SetUp()
        {
            projectManager.InitMutliProject += InitMutliProjectStruct;
        }
        [Test]
        public void GetByProjectOrder()
        {
            List<Point> projectOrder = projectManager.GetByProjectOrder();

        }
        protected virtual Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> InitMutliProjectStruct()
        {
            Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> mutliProjectStruct = new Dictionary<int, List<KeyValuePair<List<DataRow>, string>>>();

            DataTable table = new DataTable();
            //项目
            //单位工程1
            DataRow row1 = table.NewRow();
            DataRow row2 = table.NewRow();
            //单位工程2
            DataRow row3 = table.NewRow();
            DataRow row4 = table.NewRow();

            List<KeyValuePair<List<DataRow>, string>> projectInfo = null;
            List<DataRow> projectDataRow = null;

            projectInfo = new List<KeyValuePair<List<DataRow>, string>>();
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row1);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单项工程1"));
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row2);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单项工程2"));
            projectDataRow = new List<DataRow>();

            projectDataRow.Add(row3);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单项工程3"));
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row4);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单项工程4"));
            mutliProjectStruct.Add(1, projectInfo);

            projectInfo = new List<KeyValuePair<List<DataRow>, string>>();
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row1);
            projectDataRow.Add(row2);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单位工程1"));
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row3);
            projectDataRow.Add(row4);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "单位工程1"));
            mutliProjectStruct.Add(2, projectInfo);

            projectInfo = new List<KeyValuePair<List<DataRow>, string>>();
            projectDataRow = new List<DataRow>();
            projectDataRow.Add(row1);
            projectDataRow.Add(row2);
            projectDataRow.Add(row3);
            projectDataRow.Add(row4);
            projectInfo.Add(new KeyValuePair<List<DataRow>, string>(projectDataRow, "项目"));
            mutliProjectStruct.Add(3, projectInfo);

            return mutliProjectStruct;
        }
    }

}
#endif


