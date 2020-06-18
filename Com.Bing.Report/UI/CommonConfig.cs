using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using Com.Bing.API;
using DevExpress.XtraEditors.Repository;


namespace Com.Bing.UI
{
    public partial class CommonConfig : UserControl
    {
        string strSysIniFile = Function.SysIniFile;
        string strUserIniFile = Function.IniFile;

        ConfigMsgs configMsgs = new ConfigMsgs();
        int[] arrConfigCount = new int[] { 2 };
        string[] nodeNames = null;

        public void Init(string sysIni, string userIni, string[] nodeName)
        {
            strSysIniFile = sysIni;
            strUserIniFile = userIni;
            this.nodeNames = nodeName;
            BindData(false);
        }


        public CommonConfig()
        {
            InitializeComponent();
        }

        public void SaveIni()
        {
            foreach (ConfigMsg item in configMsgs)
            {
                foreach (ConfigMsg itemChild in item.ConfigMsgs)
                {
                    if (itemChild.ConfigType == "D")
                    {
                        try
                        {
                            Function.SetProfileString(strUserIniFile, item.ConfigName, itemChild.ConfigName, DateTime.Parse(itemChild.ConfigValue.ToString()).ToShortDateString());
                        }
                        catch { }
                    }
                    //新标注
                    else if (itemChild.ConfigType == "C")
                    {
                        try
                        {
                           Function.SetProfileString(strUserIniFile, item.ConfigName, itemChild.ConfigName, ((Color)(itemChild.ConfigValue)).R + "," + ((Color)(itemChild.ConfigValue)).G + "," + ((Color)(itemChild.ConfigValue)).B);
                        }
                        catch
                        {
                            Function.SetProfileString(strUserIniFile, item.ConfigName, itemChild.ConfigName, itemChild.ConfigValue.ToString());
                        }
                    }
                    else
                    {
                        Function.SetProfileString(strUserIniFile, item.ConfigName, itemChild.ConfigName, itemChild.ConfigValue.ToString());
                    }
                }
            }
        }

        public void BindData(bool bReset)
        {
            if (Function.IsEmpty(strSysIniFile) || Function.IsEmpty(strUserIniFile))
                return;
            int itemCount;
            string strValueTemp;
            string strDefValue;
            int pos;
            string[] valueTemp;         //配置值的集合
            string[] valueList;         //最大、最小或下拉选项
            ConfigMsg configMsg;        //TreeList第一层节点
            ConfigMsg configMsgItem;    //TreeList第二层节点
            configMsgs.Clear();

            foreach (string nodeName in nodeNames)
            {
                configMsg = new ConfigMsg();
                configMsg.ConfigName = nodeName;
                configMsg.ConfigDesc = Function.ProfileString(strSysIniFile, nodeName, "desc", "");
                configMsg.ConfigValue = Function.ProfileString(strSysIniFile, nodeName, "count", "");
                configMsg.ConfigType = "G";             //把类型设置为“G”

                try
                {
                    itemCount = int.Parse(configMsg.ConfigValue.ToString());
                }
                catch { itemCount = 0; }
                configMsg.ConfigValue = itemCount.ToString() + "项";

                //添加配置项
                for (int j = 1; j <= itemCount; j++)
                {
                    strDefValue = "";
                    strValueTemp = Function.ProfileString(strSysIniFile, nodeName, j.ToString(), "");
                    valueTemp = strValueTemp.Split(new char[] { ',' });
                    if (valueTemp.Length < 2) continue;

                    configMsgItem = new ConfigMsg();
                    configMsgItem.ConfigDesc = valueTemp[0];
                    configMsgItem.ConfigName = valueTemp[0];

                    pos = valueTemp[1].IndexOf('(');
                    if (pos > 0)
                    {
                        configMsgItem.ConfigType = valueTemp[1].Substring(0, pos);
                        strDefValue = valueTemp[1].Substring(pos + 1, valueTemp[1].Length - 3);
                    }
                    else
                    {
                        configMsgItem.ConfigType = valueTemp[1];    //配置值类型
                    }

                    if (!bReset && Function.ProfileString(strUserIniFile, nodeName, configMsgItem.ConfigName, "") != "")
                    {
                        configMsgItem.ConfigValue = Function.ProfileString(strUserIniFile, nodeName, configMsgItem.ConfigName, "");
                    }
                    else
                    {
                        configMsgItem.ConfigValue = strDefValue;
                    }
                    //新标注
                    //处理Type为N（数字最大值、最小值）、L（下拉项）的项
                    if (valueTemp.Length > 2 && (configMsgItem.ConfigType == "N" || configMsgItem.ConfigType == "L" || configMsgItem.ConfigType == "C"))
                    {
                        valueList = new string[valueTemp.Length - 2];
                        for (int k = 0; k < valueList.Length; k++)
                        {
                            valueList[k] = valueTemp[2 + k];
                        }
                        configMsgItem.ConfigValueList = valueList;
                    }

                    if (configMsgItem.ConfigType == "R")
                    {
                        try
                        {
                            configMsgItem.ConfigValue = Convert.ToBoolean(configMsgItem.ConfigValue);
                        }
                        catch { configMsgItem.ConfigValue = false; }
                    }

                    if (configMsgItem.ConfigType == "C")
                    {
                        configMsgItem.ConfigValue = (configMsgItem.ConfigValue as string).Replace(";", ",");
                    }

                    configMsg.ConfigMsgs.Add(configMsgItem);
                }

                configMsgs.Add(configMsg);
            }
            this.treeList1.Nodes.Clear();
            if (configMsgs.Count == 1)
            {
                this.treeList1.DataSource = configMsgs[0].ConfigMsgs;
            }
            else
            {
                this.treeList1.DataSource = configMsgs;

            }
            this.treeList1.RefreshDataSource();
        }

        private void treeList1_GetCustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            if (e.Column.FieldName != "ConfigValue") return;
            ConfigMsg configItem = treeList1.GetDataRecordByNode(e.Node) as ConfigMsg;
            if (configItem == null) return;

            switch (configItem.ConfigType)
            {
                case "R":
                    e.RepositoryItem = this.repositoryItemCheckEdit1;
                    break;
                case "G":
                    e.RepositoryItem = this.repositoryItemTextEdit1;
                    break;
                case "C":
                    e.RepositoryItem = this.repositoryItemColorEdit1;
                    break;
                case "D":
                    e.RepositoryItem = this.repositoryItemDateEdit1;
                    break;
                case "L":
                    this.repositoryItemComboBox1 = new RepositoryItemComboBox();
                    this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    this.repositoryItemComboBox1.Items.AddRange(configItem.ConfigValueList);
                    e.RepositoryItem = this.repositoryItemComboBox1;
                    break;
                case "N":
                    this.repositoryItemSpinEdit1 = new RepositoryItemSpinEdit();
                    this.repositoryItemSpinEdit1.IsFloatValue = false;
                    if (configItem.ConfigValueList.Length >= 1) //长度为1有最小值项
                    {
                        if (!Function.IsEmpty(configItem.ConfigValueList[0]) && Function.IsNumber(configItem.ConfigValueList[0]))
                            this.repositoryItemSpinEdit1.MinValue = decimal.Parse(configItem.ConfigValueList[0]);
                        else
                            this.repositoryItemSpinEdit1.MinValue = decimal.MinValue;
                    }
                    if (configItem.ConfigValueList.Length == 2)
                    {
                        if (!Function.IsEmpty(configItem.ConfigValueList[1]) && Function.IsNumber(configItem.ConfigValueList[1]))
                            this.repositoryItemSpinEdit1.MaxValue = decimal.Parse(configItem.ConfigValueList[1]);
                        else
                            this.repositoryItemSpinEdit1.MaxValue = decimal.MaxValue;
                    }
                    e.RepositoryItem = this.repositoryItemSpinEdit1;
                    break;
            }
        }

        private void treeList1_NodesReloaded(object sender, EventArgs e)
        {
            treeList1.ExpandAll();
        }

    }

    public class ConfigMsg
    {
        string strConfigDesc;
        string strConfigName;   //节点名
        object objConfigValue;  //节点值
        string strConfigType;   //节点类型
        string[] arrConfigValueList;    //可选值（最大、最小或下拉选项）
        ConfigMsgs configMsgs;

        ConfigMsgs owner;

        public ConfigMsg()
        {
            this.strConfigDesc = "";
            this.strConfigName = "";
            this.objConfigValue = "";
            this.strConfigName = "S";
            this.arrConfigValueList = new string[] { };
            this.configMsgs = new ConfigMsgs();
        }
        public ConfigMsg(string configName, string configDesc, object configValue, string configType, string[] configValueList)
        {
            this.strConfigDesc = configDesc;
            this.strConfigName = configName;
            this.objConfigValue = configValue;
            this.strConfigType = configType;
            this.arrConfigValueList = configValueList;
            this.configMsgs = new ConfigMsgs();
        }
        public ConfigMsg(ConfigMsgs obj, string configName, string configDesc, object configValue, string configType, string[] configValueList)
            : this(configName , configDesc , configValue , configType, configValueList)
        {
            this.configMsgs = obj;
        }


        [Browsable(false)]
        public ConfigMsgs Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public string ConfigType
        {
            get { return strConfigType; }
            set { strConfigType = value; }
        }
        public string[] ConfigValueList
        {
            get { return arrConfigValueList; }
            set { arrConfigValueList = value; }
        }
        public string ConfigName
        {
            get { return strConfigName; }
            set
            {
                if (ConfigName == value) return;
                strConfigName = value;
                OnChanged();
            }
        }
        public object ConfigValue
        {
            get { return objConfigValue; }
            set
            {
                if (ConfigValue == value) return;
                objConfigValue = value;
                OnChanged();
            }
        }
        public string ConfigDesc
        {
            get { return strConfigDesc; }
            set { strConfigDesc = value; }
        }

        [Browsable(false)]
        public ConfigMsgs ConfigMsgs
        {
            get { return configMsgs; }
        }

        void OnChanged()
        {
            if (owner == null) return;
            int index = owner.IndexOf(this);
            owner.ResetItem(index);
        }
    }

    public class ConfigMsgs : BindingList<ConfigMsg>, TreeList.IVirtualTreeListData
    {
        void TreeList.IVirtualTreeListData.VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            ConfigMsg obj = info.Node as ConfigMsg;
            info.Children = obj.ConfigMsgs;
        }

        void TreeList.IVirtualTreeListData.VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        {
            ConfigMsg obj = info.Node as ConfigMsg;
            switch (info.Column.FieldName)
            {
                case "ConfigDesc":
                    info.CellData = obj.ConfigDesc;
                    break;
                case "ConfigValue":
                    info.CellData = obj.ConfigValue;
                    break;
            }
        }

        void TreeList.IVirtualTreeListData.VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        {
            ConfigMsg obj = info.Node as ConfigMsg;
            switch (info.Column.FieldName)
            {
                case "ConfigValue":
                    obj.ConfigValue = info.NewCellData;
                    break;
            }
        }

        protected override void InsertItem(int index, ConfigMsg item)
        {
            item.Owner = this;
            base.InsertItem(index, item);
        }
    }
}
