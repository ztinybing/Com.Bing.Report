using System;
using System.Collections.Generic;
using Com.Bing.API;
using DevExpress.XtraTreeList.Nodes;
using System.Drawing;
using System.Windows.Forms;

namespace Com.Bing.Report
{
    public partial class ModifyReportProperty : Com.Bing.Forms.BaseDialog
    {
        private RptFileCollection rptFileCollection = null;
        public ModifyReportProperty(RptFileCollection rptFileCollection)
        {
            this.rptFileCollection = rptFileCollection;
            InitializeComponent();
        }

        private void ModifyReportProperty_Shown(object sender, EventArgs e)
        {
            treeList1.DataSource = rptFileCollection.RptFileTable.DefaultView;
            treeList1.Columns["name"].Caption = "��������";
            treeList1.Columns["src"].Visible = false;
            treeList1.Columns["level"].Visible = false;

        }

        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            ChangeCheckState(e.Node, e.Node.Checked);
        }

        private void ChangeCheckState(TreeListNode node, bool checkState)
        {
            foreach (TreeListNode childNode in node.Nodes)
            {
                childNode.Checked = checkState;
                ChangeCheckState(childNode, checkState);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Function.Confirm("ȷ��Ҫ�����޸ı������ԣ�", "��ʾ"))
            {
                SetReportProperty(treeList1.Nodes);
                Function.Alert("�޸����", "��ʾ");
            }
        }
        private void SetReportProperty(TreeListNodes nodes)
        {
            foreach (TreeListNode node in nodes)
            {
                if (node.Checked && node["src"].ToString() != "")
                {
                    string rptPath = string.Empty;
                    if (!Function.DebugMode)
                    {
                        rptPath = Function.UserReportFolder + node["src"];
                        if (!System.IO.File.Exists(rptPath))
                        {
                            string sysRptPath = Function.ReportFolder + node["src"];
                            if (System.IO.File.Exists(sysRptPath))
                            {
                                System.IO.File.Copy(sysRptPath, rptPath);
                            }
                        }
                    }
                    else
                    {
                        rptPath = Function.ReportFolder + node["src"];
                    }
                    if (!System.IO.File.Exists(rptPath))
                        continue;
                    Report report = new Report();
                    report.RptFilePath = rptPath;
                    report.ReadRPT();

                    string fontName = ddlFontName.Text == "Ĭ��" ? report.Attributes.Font.Name : ddlFontName.Text;
                    float fontSize = ddlFontSize.Text == "Ĭ��" ? report.Attributes.Font.Size : Math.Abs(float.Parse(ddlFontSize.Text));
                    report.Attributes.Font = new Font(fontName, fontSize);
                    foreach (ReportColumn column in report.Columns)
                    {
                        column.Attibutes.Font = new Font(fontName, fontSize, report.Attributes.Font.Style);
                    }
                    if (ddlMutliRowHeight.Text != "Ĭ��")
                    {
                        report.Attributes.RowHeiht = float.Parse(ddlMutliRowHeight.Text.Replace("���и�", ""));
                    }
                    if (checkBrifeReport.CheckState != CheckState.Indeterminate)
                        report.Attributes.PrintStyle.IsBrifeStyle = checkBrifeReport.Checked;

                    Texts titleTexts = report.Texts.GetTextsBy(1);
                    foreach (Text text in report.Texts)
                    {
                        string textFontName = string.Empty;
                        string textFontSize = string.Empty;
                        FontStyle style = new FontStyle();
                        bool bold = false;
                        switch (text.BandIndex)
                        {
                            case 1:
                                textFontName = ddlTitleFontName.Text;
                                textFontSize = ddlTitleFontSize.Text;
                                if (checkTitleBold.CheckState == CheckState.Indeterminate)
                                    bold = text.Attribute.Font.Bold;
                                else
                                    bold = checkTitleBold.Checked;
                                //��bandIndex = 1�������У��������Ĺ�Ϊ����������С����Ĺ�Ϊ��ͷ��
                                foreach (Text titleText in titleTexts)
                                {
                                    if (text.Attribute.Font.Size < titleText.Attribute.Font.Size)
                                    {
                                        textFontName = ddlHeadFontName.Text;
                                        textFontSize = ddlHeadFontSize.Text;
                                        if (checkHeadBold.CheckState == CheckState.Indeterminate)
                                            bold = text.Attribute.Font.Bold;
                                        else
                                            bold = checkHeadBold.Checked;
                                    }
                                }
                                break;
                            case 2:
                                textFontName = ddlHeadFontName.Text;
                                textFontSize = ddlHeadFontSize.Text;
                                if (checkHeadBold.CheckState == CheckState.Indeterminate)
                                    bold = text.Attribute.Font.Bold;
                                else
                                    bold = checkHeadBold.Checked;
                                break;
                            case 3:
                                textFontName = ddlBodyFontName.Text;
                                textFontSize = ddlBodyFontSize.Text;
                                if (checkBodyBold.CheckState == CheckState.Indeterminate)
                                    bold = text.Attribute.Font.Bold;
                                else
                                    bold = checkBodyBold.Checked;
                                break;
                            case 4:
                                textFontName = ddlSummaryFontName.Text;
                                textFontSize = ddlSummaryFontSize.Text;
                                if (checkSummaryBold.CheckState == CheckState.Indeterminate)
                                    bold = text.Attribute.Font.Bold;
                                else
                                    bold = checkSummaryBold.Checked;
                                break;
                            case 5:
                                textFontName = ddlFootFontName.Text;
                                textFontSize = ddlFootFontSize.Text;
                                if (checkFootBold.CheckState == CheckState.Indeterminate)
                                    bold = text.Attribute.Font.Bold;
                                else
                                    bold = checkFootBold.Checked;
                                break;
                        }
                        if (bold)
                            style |= FontStyle.Bold;
                        if (text.Attribute.Font.Italic)
                            style |= FontStyle.Italic;
                        if (text.Attribute.Font.Underline)
                            style |= FontStyle.Underline;


                        textFontName = textFontName == "Ĭ��" ? text.Attribute.Font.Name : textFontName;
                        textFontSize = textFontSize == "Ĭ��" ? text.Attribute.Font.Size.ToString() : textFontSize;
                        try
                        {
                            text.Attribute.Font = new Font(textFontName, Math.Abs(float.Parse(textFontSize)), style);
                        }
                        catch (Exception e)
                        {
                            Function.Alert(String.Format("�����޸��������ô�����Ϣ��{0}", e), "����");
                        }
                    }
                    if (!string.IsNullOrEmpty(txtTopMargin.Text))
                        report.Attributes.Margin.Top = ConvertUtil.GetBoundary(txtTopMargin.Text);
                    if (!string.IsNullOrEmpty(txtBottomMargin.Text))
                        report.Attributes.Margin.Bottom = ConvertUtil.GetBoundary(txtBottomMargin.Text);
                    if (!string.IsNullOrEmpty(txtLeftMargin.Text))
                        report.Attributes.Margin.Left = ConvertUtil.GetBoundary(txtLeftMargin.Text);
                    if (!string.IsNullOrEmpty(txtRightMargin.Text))
                        report.Attributes.Margin.Right = ConvertUtil.GetBoundary(txtRightMargin.Text);
                    report.WriteRpt(false);
                }
                SetReportProperty(node.Nodes);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}