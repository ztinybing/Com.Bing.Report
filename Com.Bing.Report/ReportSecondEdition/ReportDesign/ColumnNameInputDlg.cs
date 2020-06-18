using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Com.Bing.Report
{
    public partial class ColumnNameInputDlg : Com.Bing.Forms.BaseDialog
    {
        public string EColumnText
        {
            get { return textEdit1.Text; }
        }
        public string IColumnText
        {
            get { return comboBoxEdit1.Text; }
        }

        public ColumnNameInputDlg(string ExternalColumnText, string InternalColumnText, DataColumnCollection originalColumns)
        {
            InitializeComponent();
            textEdit1.Text = ExternalColumnText;
            comboBoxEdit1.Text = InternalColumnText;
            foreach (DataColumn column in originalColumns)
            {
                //�ڲ�����rowGroup�����������
                if (column.ColumnName.ToLower() != "rowgroup")
                    comboBoxEdit1.Properties.Items.Add(column.ColumnName);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (IColumnText != "")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("�ڲ����Ʋ���Ϊ��", "��ʾ");
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}