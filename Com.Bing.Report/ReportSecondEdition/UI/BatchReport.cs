using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.IO;
using Com.Bing.API;

namespace Com.Bing.Report
{
    public class BatchReport
    {
        ReportBatchSet batchSet = null;
        private PrintList ReportList
        {
            get
            {
                return batchSet.PrintListData;
            }
        }
        private MutliProjectManager projectManager = null;
        BackgroundWorker backgroundWorker = null;
        ReportCatlog reportCatlog = null;

        public BatchReport(DataTable rptTable, MutliProjectManager projectManager, PrintReportControl control)
        {
            batchSet = new ReportBatchSet(rptTable);
            this.projectManager = projectManager;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(control.BackgroundWorker_ProgressChanged);
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(control.BackgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            reportCatlog = new ReportCatlog();
        }
        public void StopBatch()
        {
            backgroundWorker.CancelAsync();
        }
        internal bool Print()
        {
            if (batchSet.ShowDialog() == DialogResult.OK)
            {
                if (PrintUtil.ShowPrintDlg(batchSet.PrintListData.ProjectPDF) == DialogResult.OK)
                {
                    backgroundWorker.RunWorkerAsync("");
                    return true;
                }
            }
            return false;
        }
        string xlsFilePath = "";
        internal bool SendExcel()
        {
            if (batchSet.ShowDialog() == DialogResult.Cancel)
            {
                return false;
            }
            string xlsName = string.Empty;
            Com.Bing.Business.StorageDataSet dataSet = this.projectManager.ProjectData as Com.Bing.Business.StorageDataSet;
            DataTable dtGcDescription = dataSet.Tables["GcDescription"];
            string order = dtGcDescription.Columns.Contains("gc_chandle") ? "gc_chandle" : "";
            DataRow[] row = dtGcDescription.Select("DescriptionName = '工程名称'", order);
            if (row.Length > 0)
            {
                xlsName = row[0]["DescriptionValue"].ToString();
            }
            else
                xlsName = "批量导出数据";


            if (XLSFile.ShowSaveFile(ref xlsFilePath, xlsName))
            {
                Function.TransBegin(xlsFilePath);
                backgroundWorker.RunWorkerAsync("send");
                return true;
            }
            return false;
        }
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //int totalPage = ReportList.CalcTotalPageNum ?0:-1;           
            //起始页的值是否应加在总体报表上
            //修正问题：将输入启起页码作为总页数起始值、目录页码起始值
            int pageOffset = this.ReportList.StartPageNum - 1;
            int totalPage = pageOffset;
            int catlogPageIndex = this.ReportList.StartPageNum;
            PrintProgress.SCalaTotalPage(ReportList.Count, sender as BackgroundWorker);
            foreach (PrintElement item in ReportList)
            {
                // 中止
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //读报表
                if (Path.GetExtension(item.ReportSrc).ToLower() == ".doc")
                    item.NewReport(projectManager);
                else
                    item.NewReport();


                if (!item.Pagination)
                    continue;
                if (ReportList.CalcTotalPageNum)
                {
                    //计算总页数                
                    item.Report.InitReportData(projectManager, -1);
                    totalPage += item.Report.TatalPage;
                }
            }
            PrintProgress.ECalcTotalPage();

            reportCatlog.Clear();
            if (ReportList.ProjectCatalog)
            {
                //工程顺序模式
                List<Point> projectOrder = projectManager.GetByProjectOrder();
                foreach (Point point in projectOrder)
                {
                    foreach (PrintElement item in ReportList)
                    {
                        if (item.Level == point.X)
                        {
                            PrintProgress.PrtReport(item.ReportName);
                            item.Report.InitReportData(projectManager, point.Y);
                            if (item.Pagination)
                            {
                                item.Report.WholeDrawStruct.SetPageOffset(pageOffset, totalPage, this.ReportList.CalcTotalPageNum);
                                pageOffset += item.Report.TatalPage;
                            }
                            reportCatlog.Add(item.ReportName, catlogPageIndex);
                            catlogPageIndex += item.Report.TatalPage;
                            InnerDo(e.Argument.ToString(), item);
                        }
                    }
                }
            }
            else
            {
                foreach (PrintElement item in ReportList)
                {
                    PrintProgress.PrtReport(item.ReportName);
                    item.Report.InitReportData(projectManager, -1);
                    if (item.Pagination)
                    {
                        item.Report.WholeDrawStruct.SetPageOffset(pageOffset, totalPage, ReportList.CalcTotalPageNum);
                        pageOffset += item.Report.TatalPage;
                    }
                    reportCatlog.Add(item.ReportName, catlogPageIndex);
                    catlogPageIndex += item.Report.TatalPage;
                    InnerDo(e.Argument.ToString(), item);
                }
            }
            if (ReportList.PrintCatalogue)
            {
                reportCatlog.Print(e.Argument.ToString(), xlsFilePath);
            }
            PrintProgress.PrtOver();
            e.Result = e.Argument;
        }
        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.ToString() == "send")
            {
                Function.TransEnd();
            }
        }
        private void InnerDo(string flagName, PrintElement item)
        {

            if (flagName == "send")
            {
                //item.Report.SendToExcel(this.xlsFilePath);
                item.Report.SendToExcelAsPart();
            }
            else
            {
                item.Report.Print();
            }
        }
        internal void CancelAsync()
        {
            backgroundWorker.CancelAsync();
        }
    }
}

