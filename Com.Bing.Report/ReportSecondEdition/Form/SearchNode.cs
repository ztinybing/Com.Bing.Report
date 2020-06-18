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
    public partial class SearchNode : Com.Bing.Forms.BaseDialog
    {
        public event EventHandler<NodeEventArgs> SearchNodeByName;
        private List<int> listResult = new List<int>();
        private int index = -1;
        private DataTable dt;
        public SearchNode(DataTable dt)
        {
            this.dt = dt;
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            index = -1;
            listResult.Clear();
            string text = textEdit1.Text.Trim();
            if (text != "")
            {
                DataRow[] rows = dt.Select("name like '%" + textEdit1.Text.Trim() + "%'");
                foreach (DataRow row in rows)
                {
                    listResult.Add(Convert.ToInt32(row["ID"]));
                }
                if (listResult.Count > 0)
                {
                    index = 0;
                    executeSearch(listResult[0]);
                }
            }

        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (--index == -1)
            {
                index = listResult.Count - 1;
            }
            if (listResult.Count > index && index >= 0)
            {
                int id = listResult[index];
                executeSearch(id);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (listResult.Count <= ++index)
            {
                index = 0;
            }
            if (listResult.Count > index && index >= 0)
            {
                int id = listResult[index];
                executeSearch(id);
            }
        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void executeSearch(int id)
        {
            SearchNodeByName(this, new NodeEventArgs(id));
            labelControl1.Text = String.Format("{0}/{1}Ìõ", index + 1, listResult.Count);
        }
    }
    public class NodeEventArgs : EventArgs
    {
        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public NodeEventArgs(int id)
        {
            this.id = id;
        }
    }
}