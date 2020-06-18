using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Com.Bing.Report;


namespace Com.Bing.Report
{
    public partial class UIPreviewReport : UserControl
    {
        private Report currReport;
        public Report CurrReport
        {
            get { return currReport; }
        }
        public UIPreviewReport()
        {
            InitializeComponent();
        }

        public ReportDrawStruct GetReportStruct(string rptFilePath, DataTable dt, Dictionary<string, int> dicMergeColInfo)
        {
            using (Report report = new Report())
            {
                report.RptFilePath = rptFilePath;
                report.ReadRPT();

                report.Attributes.GrainDirection = 0;

                ReportData reportData = new ReportData(report);
                reportData.InitReportData(dt);
                report.Data = reportData;
                reportData.MergeBlack(report, dicMergeColInfo);
                report.DataValid = true;
                report.RaiseDataChanged();
                report.RaisePropertyChanged();

                return report.WholeDrawStruct;
            }
        }

        public void ViewReport(string rptFilePath, DataTable dt, Dictionary<string, int> dicMergeColInfo)
        {
            Report report = new Report();
            report.RptFilePath = rptFilePath;
            report.ReadRPT();

            report.Attributes.GrainDirection = 0;

            ReportData reportData = new ReportData(report);
            reportData.InitReportData(dt);
            report.Data = reportData;
            reportData.MergeBlack(report, dicMergeColInfo);
            report.DataValid = true;
            report.RaiseDataChanged();
            report.RaisePropertyChanged();

            Repaint(report);

            currReport = report;
        }

        private void Repaint(Report report)
        {
            ReportMode.Image = report.SpeedPreview(1);
            if (ReportMode.Image != null)
            {
                ReportMode.Dock = DockStyle.None;
                ReportMode.Size = new System.Drawing.Size(this.Width, this.Height);
                ReportMode.Cursor = Cursors.Arrow;
                
                //ReportMode.Size = ReportMode.Image.Size;
                ReportMode.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void ReportMode_DoubleClick(object sender, EventArgs e)
        {
            if (currReport != null)
            {
                PreviewReport viewDialog = new PreviewReport(currReport);
                viewDialog.Show();
                Repaint(currReport);
            }
        }
    }
}
