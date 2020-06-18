using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Com.Bing.Report
{
    public class PrintProgress
    {
        private static int progressPercentage = 0;
        public static int ProgressPercentage
        {
            get { return progressPercentage; }
        }
        private static string workContent = "";
        public static string WorkContent
        {
            get { return workContent; }
        }
        private static int preCent = 1;
        private static BackgroundWorker worker = null;
        private PrintProgress() { }
        public static void SCalaTotalPage(int pageNum, BackgroundWorker worker)
        {

            progressPercentage = 0;
            PrintProgress.worker = worker;
            preCent = 1;
            preCent = (int)Math.Ceiling((preCent * 80 / (double)pageNum));
            workContent = "正在计算总页数....";
            worker.ReportProgress(progressPercentage);
        }
        public static void ECalcTotalPage()
        {
            progressPercentage = 10;
            workContent = "总页数计算完毕";
            worker.ReportProgress(progressPercentage);
        }
        public static void PrtReport(string name)
        {
            progressPercentage += preCent;
            workContent = string.Format("处理报表:{0}....", name); ;
            worker.ReportProgress(progressPercentage);
            if (progressPercentage >= 100)
            {
                progressPercentage = 0;
            }
        }
        public static void PrtOver()
        {
            progressPercentage = 100;
            worker.ReportProgress(progressPercentage);
            workContent = "工作完成";
        }
    }
}
