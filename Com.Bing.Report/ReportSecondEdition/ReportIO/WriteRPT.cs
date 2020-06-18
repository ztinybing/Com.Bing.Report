using System;
using System.Collections.Generic;
using System.Text;
using Com.Bing.API;
using System.IO;

namespace Com.Bing.Report
{
    public class WriteRpt
    {
        Report report = null;
        public WriteRpt(Report report)
        {
            this.report = report;
        }
        public void Write()
        {
            Write(GetUserRptPath());

        }
        private string GetUserRptPath()
        {
            if (!report.RptFilePath.Contains(Function.UserReportFolder) && !Function.DebugMode)
            {
                return Function.UserReportFolder + Path.GetFileName(report.RptFilePath);
            }
            else
            {
                File.Delete(report.RptFilePath);
            }
            return report.RptFilePath;
        }

        internal void Write(string newRptFile)
        {
            using (FileStream fileStream = new FileStream(newRptFile, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);
                writer.AutoFlush = true;
                writer.Write(report.ToString());
                writer.Close();
                report.RptFilePath = newRptFile;
            }
        }
    }
}
