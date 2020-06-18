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
    public partial class FormShowOriginalData : DevExpress.XtraEditors.XtraForm
    {
        public FormShowOriginalData(DataTable originalData)
        {
            InitializeComponent();
            gridControl1.DataSource = originalData;
        }
    }
}