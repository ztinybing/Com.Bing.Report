using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Com.Bing.UI;
using Com.Bing.API;

namespace Com.Bing.Forms
{

    /// <example>
    /// string[][] connarr = new string[3][];
    /// connarr[0] = new string[]{"工程量小数位数保留设置"};
    /// connarr[1] = new string[] { "行背景色设置", "行前景色设置" };
    /// connarr[2] = new string[] { "启动设置" };
    /// List《string》 titles = new List string (new string[] { "小数位数设置" , "" , ""});
    /// CommonConfigDialog dlg = new CommonConfigDialog(  connarr , titles);
    /// </example>
    public partial class CommonConfigDialog : BaseDialog
    {
        #region  常量字串
        private static string OkText = "确定(&O)";
        private static string NextText = "下一步(&N)";
        private static string FormTitle = "参数设置";
        private static string ResetParaText = "恢复(&R)";
        #endregion

        
        public CommonConfigDialog()
        {
            InitializeComponent();
        }
        public CommonConfig GetCommonConfig()
        {
            // 
            // commonConfig1
            // 
            CommonConfig commonConfig = new Com.Bing.UI.CommonConfig();
            commonConfig.Location = new System.Drawing.Point(4, 34);
            commonConfig.Size = new System.Drawing.Size(429, 242);
            commonConfig.TabIndex = 0;
            commonConfig.Visible = false;
            Controls.Add(commonConfig);
            Controls.SetChildIndex(commonConfig, 0);
            return commonConfig;
        }

        public CommonConfigDialog(string ini, string[] nodeNames, string title)
            : this(Function.AreaSysIniFile(ini), Function.AreaIniFile(ini), nodeNames, title)
        {
        }
        public CommonConfigDialog(string sysIni, string userIni, string[] nodeNames, string title)
            : this()
        {
            CommonConfig config = GetCommonConfig();
            config.Visible = true;
            commonConfigDic.Add(0, config);
            commonConfigDic[curPageIndex].Init(sysIni, userIni, nodeNames);
            if (title == "")
            {
                if (nodeNames.Length == 1)
                {
                    titleList.Add(nodeNames[0]);
                }
                else
                {
                    titleList.Add(FormTitle);
                }
            }
            else
            {
                titleList.Add(title);
            }
            //preStep.Visible = false;
            preStep.Text = ResetParaText;
            nextStep.Text = OkText;
        }
        public CommonConfigDialog(string[] nodeNames, string title)
            : this("", nodeNames, title)
        { }
        public CommonConfigDialog(string ini, string[] nodeNames)
            : this(ini, nodeNames, "")
        {



        }
        public CommonConfigDialog(string[] nodeNames)
            : this("", nodeNames, "")
        {
        }

        private int PageCount
        {
            get { return commonConfigDic.Count; }
        }
        private int curPageIndex = 0;
        protected CommonConfig CurConfig
        {
            get { return commonConfigDic[curPageIndex]; }
        }
        private Dictionary<int, CommonConfig> commonConfigDic = new Dictionary<int, CommonConfig>();
        private List<string> titleList = new List<string>();
        public override string Text
        {
            get
            {
                if (titleList.Count > 0)
                {
                    return titleList[curPageIndex];// base.Text;
                }
                return FormTitle;
            }
            set
            {
                base.Text = value;
            }
        }

 
        private void nextStep_Click(object sender, EventArgs e)
        {
            if (PageCount - 1 == curPageIndex)
            {
                foreach (int pageIndex in commonConfigDic.Keys)
                {
                    commonConfigDic[pageIndex].SaveIni();
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                reSetCommonConfig();
            }
        }
        private void reSetCommonConfig()
        {
            CurConfig.Visible = false;
            curPageIndex += 1;
            CurConfig.Visible = true;

            if (curPageIndex == PageCount - 1)
            {
                nextStep.Text = OkText;
            }

            base.Text = this.Text;
            preStep.Enabled = true;
            this.Refresh();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void preStep_Click(object sender, EventArgs e)
        {
            if (preStep.Text == ResetParaText)
            {
                CurConfig.BindData(true);
            }
            else
            {
                CurConfig.Visible = false;
                curPageIndex -= 1;
                CurConfig.Visible = true;
                nextStep.Text = NextText;
                base.Text = this.Text;
                if (curPageIndex == 0)
                {
                    preStep.Enabled = false;
                }
                this.Refresh();
            }
        }
    }
}