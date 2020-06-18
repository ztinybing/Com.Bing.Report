using System;
using System.Collections.Generic;
using System.Text;
using Com.Bing.Report;
using Com.Bing.UI;
using System.Windows.Forms;
using Com.Bing.Forms;
using Com.Bing.API;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace Com.Bing.UI
{
    public class CostUIReport : PrintReportControl
    {
        public CostUIReport()
            : base()
        {
            string[] reportGroup = new string[] { "报表示例A", "报表示例B" };

            SetReportGroup(reportGroup);
            InitMutliProjectStruct();
        }
        public override string EchoNameReplace(string xmlName)
        {
            return base.EchoNameReplace(xmlName);
        }
        protected override void ReCheckProjectNode(List<KeyValuePair<List<DataRow>, string>> mutliProjectList)
        {
            for (int index = 0; index < mutliProjectList.Count; index++)
            {
                foreach (DataRow row in mutliProjectList[index].Key)
                {
                    row["keyval"] = "true";
                }
            }
        }
        protected override void CheckProjectNode(List<KeyValuePair<List<DataRow>, string>> mutliProjectList, int projectIndex)
        {
            StringBuilder realCheckedBD = new StringBuilder();
            for (int index = 0; index < mutliProjectList.Count; index++)
            {
                foreach (DataRow row in mutliProjectList[index].Key)
                {
                    realCheckedBD.Append("," + row["gc_chandle"].ToString());
                    if (index == projectIndex)
                    {
                        row["keyval"] = "true";
                    }
                    else
                    {
                        row["keyval"] = "false";
                    }
                }
            }
        }
        protected override Dictionary<int, List<KeyValuePair<List<DataRow>, string>>> InitMutliProjectStruct()
        {
             return base.InitMutliProjectStruct();
        }

        private void AddRowInParent(KeyValuePair<List<DataRow>, object> projectEntry, List<KeyValuePair<List<DataRow>, object>> tempList, object phandle)
        {
            int findIndex = -1;
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].Value.ToString() == phandle.ToString())
                {
                    findIndex = i;
                    break;
                }
            }
            if (findIndex == -1)
            {
                tempList.Add(new KeyValuePair<List<DataRow>, object>(new List<DataRow>(), phandle));
                findIndex = tempList.Count - 1;
            }
            foreach (DataRow row in projectEntry.Key)
            {
                tempList[findIndex].Key.Add(row);
            }
        }
        public void ReportSetting(object sender, EventArgs e)
        {
        }
    }
}
