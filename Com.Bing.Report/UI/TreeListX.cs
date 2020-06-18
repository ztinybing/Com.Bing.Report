using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList;
using System.Runtime.InteropServices;

namespace Com.Bing.UI
{
    public class TreeListX : DevExpress.XtraTreeList.TreeList
    {
        /// <summary>
        /// Treelist扩展控件
        /// </summary>
        /// <remarks>Treelist扩展控件</remarks>
        public TreeListX()
        {
            #region 控件设置

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);

            #endregion
        }

        // 为什么要自已处理内存回收
        //~TreeListX()
        //{
        //    GC.Collect();
        //}

        #region 成员变量

        ToolTip toolTip = new ToolTip(); // 提示框

        DataTable table = null;//最后选中的单元格表，在按住Ctrl的时候他不会清空

        DataTable tempTable = null;//选中的单元格表,高速运行

        SelectedCell FirstCell = new SelectedCell();//记录下第一个单元格

        bool isMouseDown = false;//鼠标是否按下

        string toolTipText = ""; //自定义TIP

        public string ToolTip
        {
            get 
            { 
                return toolTipText; 
            }
            set 
            { 
                toolTipText = value;
                if (value != "")
                {
                    this.MouseMove += TreeGrid_MouseMove;
                    this.MouseLeave += TreeListX_MouseLeave;
                }
                else
                {
                    this.MouseMove -= TreeGrid_MouseMove;
                    this.MouseLeave -= TreeListX_MouseLeave;
                }
            }
        }

        void TreeListX_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        int inf = 1;//在字典中先出现的node的id

        int enf = 0;//在字典中后出现的node的id

        bool go = false;//是否继续查找

        bool isKeyCtrl = false;//是否按下Ctrl键

        int index = 0; //每一次选中的时候去找所有可见节点的id集合的键

        bool AllSelect = false;//是不是全选，当按下Ctrl时，为全选

        Dictionary<int, int> idc= new Dictionary<int,int>();//每次鼠标点下准备选中的时候要刷新字典

        Color _selectBackColor = Color.LightSteelBlue;//选中单元格的背景颜色

        Color focusedbackcolor = Color.White;//焦点单元格的背景颜色

        #endregion

        #region 事件通知
        public delegate void MarqueeDoneDelegate(double value);
        public event MarqueeDoneDelegate SelDone = null;
        protected void fireSelDone()
        {
            if (SelDone != null)
            {
                SelDone(SumValue);
            }
        }
        #endregion
        #region 成员属性

        //获取选中单元格的node对象
        [Browsable(false)]
        public DevExpress.XtraTreeList.Nodes.TreeListNodes SelectNodes
        {
            get 
            {
                //可能需要优化
                TreeListNodes nodes = new TreeListNodes(this);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    nodes.Add((TreeListNode)(table.Rows[i]["Node"]));
                }
                return nodes;
            }
        }

        //获取选中的单元格对象的和
        public double SumValue
        {
            get
            {
                double sum = 0;
                if (this.SelectMode == SelectModeEnum.Marquee)
                {
                    if (table != null && tempTable != null)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            sum += (double)row["val"];
                        }
                        foreach (DataRow row in tempTable.Rows)
                        {
                            sum += (double)row["val"];
                        }
                    }
                }
                else
                {
                    sum = this.Selection.Count;
                }
                return sum;
            }
        }
        //获取选中的单元格对象，这个是自己定义的对象
        [Browsable(false)]
        public List<SelectedCell> SelectedCells
        {
            get
            {
                List<SelectedCell> tempnodes = new List<SelectedCell>();
                if (this.SelectMode == SelectModeEnum.Marquee)
                { 
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        TreeListNode nd = table.Rows[i]["Node"] as TreeListNode;
                        TreeListColumn cl = table.Rows[i]["Column"] as TreeListColumn;
                        SelectedCell n = new SelectedCell(Convert.ToInt32(table.Rows[i]["Nid"]), Convert.ToInt32(table.Rows[i]["Col"]),nd,cl);
                        tempnodes.Add(n);
                    }
                }
                return tempnodes;
            }
        }

        //获取或设置选中单元格的背景颜色
        /// <summary>
        /// 选中单元格的背景颜色
        /// </summary>
        /// <remarks>选中单元格的背景颜色</remarks>
        [Browsable(true)]
        public Color SelectBackColor
        {
            get { return _selectBackColor; }
            set 
            {
                if (value == Color.Transparent)
                {
                    throw new Exception("不支持透明颜色");
                }
                _selectBackColor = value; 
                this.Invalidate(true); 
            }
        }

        //获取或设置焦点框的背景颜色
        /// <summary>
        /// 焦点单元格的背景颜色
        /// </summary>
        /// <remarks>焦点单元格的背景颜色</remarks>
        [Browsable(true)]
        public Color FocusedBackColor
        {
            get { return focusedbackcolor; }
            set { focusedbackcolor = value; }
        }

        /// <summary>
        /// 获取选中过的列的数量
        /// </summary>
        /// <remarks>获取选中过的列的数量</remarks>
        [Browsable(false)]
        public int SelectColumnCount
        {
            get 
            {
                DataTable temp = table.Copy();
                temp= temp.DefaultView.ToTable(true, new string[] { "Col" });
                return temp.Rows.Count; 
            }
        }
        /// <summary>
        /// 获取选中行的数量，或者说选中节点的数量
        /// </summary>
        /// <remarks>获取选中行的数量，或者说选中节点的数量</remarks>
        [Browsable(false)]
        public int SelectNodeCount
        {
            get
            {
                DataTable temp = table.Copy();
                temp = temp.DefaultView.ToTable(true, new string[] { "Nid" });
                return temp.Rows.Count;
            }
        }

        public enum SelectModeEnum
        {
            Marquee = 0,
            FullRow = 1,
            Default = 2,
        }

        private SelectModeEnum sm = SelectModeEnum.Default;
        /// <summary>
        /// 获取或设置选中模式
        /// </summary>
        /// <remarks>获取或设置选中模式</remarks>
        bool preMultiSelect = false;
        bool preEnableAppearanceFocusedRow = false;
        public SelectModeEnum SelectMode
        {
            get { return sm; }
            set
            {
                sm = value;
                if (table == null) //首次使用
                {
                    preMultiSelect = this.OptionsSelection.MultiSelect;
                    preEnableAppearanceFocusedRow = this.OptionsSelection.EnableAppearanceFocusedRow;
                    table = new DataTable();
                    table.Columns.Add("Nid");
                    table.Columns.Add("Col");
                    table.Columns.Add("Node", typeof(TreeListNode));
                    table.Columns.Add("Column", typeof(TreeListColumn));
                    table.Columns.Add("Val", typeof(double));
                    table.PrimaryKey = new DataColumn[] { table.Columns["Nid"], table.Columns["Col"] };
                    tempTable = table.Clone();
                }
                if (value == SelectModeEnum.Marquee)
                {
                    //刚刚设置成Marquee的时候呢，清空所有上次Marquee的东西

                    #region 注册要用的事件

                    this.OptionsSelection.MultiSelect = false;
                    this.OptionsSelection.EnableAppearanceFocusedRow = false;
                    this.CustomDrawNodeCell += TreeGrid_CustomDrawNodeCell;
                    this.MouseDown += TreeGrid_MouseDown;
                    this.MouseUp += TreeGrid_MouseUp;
                    this.MouseLeave += TreeGrid_MouseLeave;
                    this.MouseMove += TreeGrid_MouseMove;
                    this.KeyDown += TreeGridView_KeyDown;
                    this.KeyUp += this.TreeGridView_KeyUp;

                    #endregion 

                    #region 还原设置
                    table.Rows.Clear();
                    tempTable.Rows.Clear();
                    idc.Clear();
                    index = 0;
                    inf = 0;
                    enf = 0;
                    go = false;
                    #endregion
                }
                else
                {
                    AllSelect = false;
                    this.OptionsSelection.MultiSelect = preMultiSelect;
                    this.OptionsSelection.EnableAppearanceFocusedRow = preEnableAppearanceFocusedRow;
                    this.CustomDrawNodeCell -= TreeGrid_CustomDrawNodeCell;
                    this.MouseDown -= TreeGrid_MouseDown;
                    this.MouseUp -= TreeGrid_MouseUp;
                    this.MouseLeave -= TreeGrid_MouseLeave;
                    this.MouseMove -= TreeGrid_MouseMove;
                    this.KeyDown -= TreeGridView_KeyDown;
                    this.KeyUp -= this.TreeGridView_KeyUp;
                }
                this.Invalidate(true); 
            }
        }

        int _mcount = 800;
        /// <summary>
        /// 获取或设置全选警告数据数量
        /// </summary>
        /// <remarks>获取或设置全选警告数据数量</remarks>
        [Browsable(true)]
        public int WarningCount
        {
            get { return _mcount; }
            set { _mcount = value; }
        }

        #endregion

        #region 重写

        //当为选择方式为Marquee的时候，不要多选，不要选择行
        protected override TreeListOptionsSelection CreateOptionsSelection()
        {
            TreeListOptionsSelection tlo = base.CreateOptionsSelection();
            if (this.SelectMode == SelectModeEnum.Marquee)
            {
                tlo.EnableAppearanceFocusedRow = false;
                tlo.MultiSelect = false;
            }
            return tlo;
        }
        //当为Marquee的时候，点击一下不给单元格输入焦点
        protected override TreeListOptionsBehavior CreateOptionsBehavior()
        {
            TreeListOptionsBehavior tob = base.CreateOptionsBehavior();
            if (this.SelectMode == SelectModeEnum.Marquee)
            {
                tob.ImmediateEditor = false;
            }
            return tob;
        }

        #endregion

        #region 成员方法

        #region 对象事件
        private static int preX = 0, preY = 0;
        private void TreeGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = this.PointToClient(Cursor.Position);
            bool bChanged = (preX != e.X || preY != e.Y);
            if (bChanged)
            {
                preX = e.X;
                preY = e.Y;
            }
            if (isMouseDown)
            {
                TreeListNode node = this.ViewInfo.GetHitTest(p).Node;
                TreeListColumn col = this.ViewInfo.GetHitTest(p).Column;

                if (node != null && col != null)
                {
                    #region 一坨选中的单元格——方案三——排序后选择的会混乱,但是已经解决
                    //从第一个到当前鼠标下的单元格的位置，估计循环较大

                    tempTable.Rows.Clear();
                    #region 这个循环用了控制node的id顺序,可能可以优化

                    for (int i = 1; i <= idc.Count; i++)
                    {
                        try
                        {
                            if (idc[i] == node.Id && !go)
                            {
                                inf = i;
                                go = true;
                                continue;
                            }
                            if (idc[i] == FirstCell.ID && !go)
                            {
                                inf = i;
                                go = true;
                                continue;
                            }
                            if (idc[i] == node.Id && go)
                            {
                                enf = i;
                                go = false;
                                break;
                            }
                            if (idc[i] == FirstCell.ID && go)
                            {
                                enf = i;
                                go = false;
                                break;
                            }
                        }
                        catch { }
                    }
                    #endregion

                    #region 将选中的单元格的矩形存放到tempselect中，但是select里面要是有了的化，就不放进去了
                    double val;
                    if (col.VisibleIndex > FirstCell.Col)
                    {
                        //当当前鼠标下面的单元格的列索引小于第一个单元格的列索引时
                        //计算选中过的列
                        for (int c = FirstCell.Col; c < col.VisibleIndex + 1; c++)
                        {
                            for (int i = inf; i < enf + 1; i++)
                            {
                                if (inf != 0)
                                {
                                    if (!table.Rows.Contains(new object[] { idc[i], c }))
                                    {
                                        TreeListNode node1 = this.GetNodeByVisibleIndex(i - 1);
                                        TreeListColumn col1 = this.GetColumnByVisibleIndex(c);
                                        val = 0;
                                        double.TryParse(node1.GetDisplayText(col1), out val);
                                        tempTable.Rows.Add(new object[] { idc[i], c, node1, col1, val });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //当当前鼠标下面的单元格的列索引大于第一个单元格的列索引时
                        for (int c = col.VisibleIndex; c < FirstCell.Col + 1; c++)
                        {
                            for (int i = inf; i < enf + 1; i++)
                            {
                                if (inf != 0)
                                {
                                    if (!table.Rows.Contains(new object[] { idc[i], c }))
                                    {
                                        TreeListNode node1 = this.GetNodeByVisibleIndex(i - 1);
                                        TreeListColumn col1 = this.GetColumnByVisibleIndex(c);
                                        val = 0;
                                        double.TryParse(node1.GetDisplayText(col), out val);
                                        tempTable.Rows.Add(new object[] { idc[i], c, node1, col1, val });
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion
                    //立即从新绘制
                    this.SetFocusedNode(node);
                    this.FocusedColumn = col;
                    if (bChanged)
                    {
                        toolTip.Show(SumValue.ToString(), this, e.X + 16, e.Y + 16, 3000);
                    }
                    this.Invalidate(true);
                }
            }
            else if (toolTipText != "")
            {
                if (bChanged)
                {
                    toolTip.Show(toolTipText, this, e.X + 16, e.Y + 16, 3000);
                }
            }
            else
            {
                toolTip.Hide(this);
            }
        }

        /// <summary>
        /// 将node的顺序保存到字典
        /// </summary>
        /// <param name="node"></param>
        /// <param name="id"></param>
        private void getNodeID(TreeListNode node, Dictionary<int, int> id)
        {
            if (node==null||node.Visible)
            {
                index++;
                if (node == null)
                {
                    for (int i = 0; i < this.Nodes.Count; i++)
                    {
                        id.Add(index, this.Nodes[i].Id);
                        getNodeID(this.Nodes[i], id);
                    }
                }
                else
                {
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        id.Add(index, node.Nodes[i].Id);
                        getNodeID(node.Nodes[i], id);
                    }
                }
            }
        }

        /// <summary>
        /// 避免按着鼠标离开控件，监听不到鼠标放开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeGrid_MouseLeave(object sender, EventArgs e)
        {

            isMouseDown = false;
            for (int i = 0; i < tempTable.Rows.Count; i++)
            {
                table.ImportRow(tempTable.Rows[i]);
            }
            tempTable.Rows.Clear();
            GC.Collect();
            
        }

        private void TreeGrid_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            for (int i = 0; i < tempTable.Rows.Count; i++)
            {
                try
                {
                    table.ImportRow(tempTable.Rows[i]);
                }
                catch { }
            }
            tempTable.Rows.Clear();
            GC.Collect();
        }

        private void TreeGrid_MouseDown(object sender, MouseEventArgs e)
        {
            //每一次点击后，更新字典idc，可优化为数据源改变+从新排序后
            idc.Clear();
            getNodeID((TreeListNode)null, idc);//获取当前tree中可见的node

            #region 当失去焦点后，又重新获得焦点点击时判断是不是按下了Ctrl键
            //当点击的时候获取这个东西
            if (Control.ModifierKeys == Keys.Control)
            {
                isKeyCtrl = true;
            }
            else
            {
                isKeyCtrl = false;
            }
            #endregion

            AllSelect = false;//丢弃全选状态

            this.Invalidate(true);//重新绘制背景

            isMouseDown = false;//丢弃鼠标按下状态

            #region 必须是按下鼠标左键，并且选中模式必须是Marquee
            if (e.Button == MouseButtons.Left && SelectMode == SelectModeEnum.Marquee)
            {
                Point p = this.PointToClient(Cursor.Position);
                isMouseDown = true;
                //当鼠标按下准备拖选的时候，如果某格单元格有输入焦点，则不能够进行这个操作
                CellInfo cell = this.ViewInfo.GetHitTest(p).CellInfo;
                if (cell == null || cell.Focused)
                {
                    isMouseDown = false;
                }
                #region 还原未选中状态等
                if (!isKeyCtrl)
                {
                    table.Rows.Clear();
                }
                go = false;
                index = 0;
                inf = 1;
                enf = 0;
                #endregion

                TreeListNode node = this.ViewInfo.GetHitTest(p).Node;
                TreeListColumn col = this.ViewInfo.GetHitTest(p).Column;
                //符合选择要求后，把当前单元格记录为第一个单元格
                if (node != null && col != null)
                {
                    FirstCell = new SelectedCell();
                    FirstCell.Col = col.VisibleIndex;//.ColumnHandle;
                    FirstCell.ID = node.Id;
                    //重新绘制后，将第一个单元格添加到选中集合
                    if (!table.Rows.Contains(new object[] { node.Id, col.VisibleIndex }))
                    {
                        double val = 0;
                        double.TryParse(node.GetDisplayText(col), out val);
                        table.Rows.Add(new object[] { FirstCell.ID, FirstCell.Col, node, col, val });
                    }
                }
                //else
                //{
                //    FirstCell = null;
                //}
                this.Invalidate(true);
            }
            #endregion
        }

        #endregion

        #region 绘制选中单元格

        //绘制选中的单元格,暂时有个背景颜色意思意思
        private void TreeGrid_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            if (this.SelectMode == SelectModeEnum.Marquee)
            {
                if (AllSelect)//全选，呵呵
                {
                    e.Graphics.FillRectangle(new SolidBrush(SelectBackColor), new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - e.Bounds.Left, e.Bounds.Bottom - e.Bounds.Top));
                }
                else
                {
                    #region 
                    try
                    {
                        if (//这两个条件都视为选中//当选中的很多的时候，感觉很慢
                            (table.Rows.Contains(new object[] { e.Node.Id, e.Column.VisibleIndex }))
                            ||
                            (tempTable.Rows.Contains(new object[] { e.Node.Id, e.Column.VisibleIndex }))
                           )
                        {
                            e.Graphics.FillRectangle(new SolidBrush(SelectBackColor), new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - e.Bounds.Left, e.Bounds.Bottom - e.Bounds.Top));
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.Transparent), new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - e.Bounds.Left, e.Bounds.Bottom - e.Bounds.Top));
                        }
                        if (e.Focused)//跟Excel一样，给焦点单元格画不同的背景
                        {
                            e.Graphics.FillRectangle(new SolidBrush(FocusedBackColor), new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - e.Bounds.Left, e.Bounds.Bottom - e.Bounds.Top));
                        }
                    }
                    catch { }
                    #endregion 
                }
            }
        }
        #endregion

        #region Ctrl键
        private void TreeGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    isKeyCtrl = false;
                    //按下Ctrl+A=全选
                    if (this.RowCount > WarningCount&&WarningCount!=0)
                    {
                        if (DialogResult.No == MessageBox.Show("数据量过大，可能会造成短暂卡机现象，稍等片刻即可。\r\n是否继续？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            return;
                        }
                    }
                    AllSelect = true;
                    this.Invalidate(true);
                    for (int c = 0; c < this.Columns.Count; c++)
                    {
                        for (int i = 0; i < this.RowCount; i++)
                        {
                            TreeListNode nd = this.GetNodeByVisibleIndex(i);
                            TreeListColumn cl = this.GetColumnByVisibleIndex(c);
                            if (!table.Rows.Contains(new object[] { nd.Id, cl.VisibleIndex }))
                            {
                                double val = 0;
                                double.TryParse(nd.GetDisplayText(cl), out val);
                                this.table.Rows.Add(new object[] { nd.Id, cl.VisibleIndex, nd, cl, val });
                            }
                        }
                    }
                }
                else
                {
                    isKeyCtrl = true;
                }
                this.OptionsBehavior.Editable = false;// 如果按下了Ctrl键，单击的单元格不能输入
            }

        }

        private void TreeGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                isKeyCtrl = false;
                this.OptionsBehavior.Editable = true;// 如果放开了Ctrl键，控件可以编辑
            }
        }
        #endregion

        #region 代码选中方法
        public void SelectCells(SelectedCell First, SelectedCell Last)
        {
            SelectCells(First,Last,false);
        }

        public void SelectCells(SelectedCell First, SelectedCell Last,bool Mul)
        {
            //FirstCell = First;
            if (!Mul)
            {
                table.Rows.Clear();
                this.Invalidate(true);
                idc.Clear();
                getNodeID((TreeListNode)null, idc);//获取当前tree中可见的node
            }
          
            #region 

            for (int i = 1; i <= idc.Count; i++)
            {
                try
                {
                    if (idc[i] ==this.GetNodeByVisibleIndex(Last.ID).Id && !go)
                    {
                        inf = i;
                        go = true;
                        continue;
                    }
                    if (idc[i] ==this.GetNodeByVisibleIndex(First.ID).Id && !go)
                    {
                        inf = i;
                        go = true;
                        continue;
                    }
                    if (idc[i] ==this.GetNodeByVisibleIndex( Last.ID).Id && go)
                    {
                        enf = i;
                        go = false;
                        break;
                    }
                    if (idc[i] == this.GetNodeByVisibleIndex(First.ID).Id && go)
                    {
                        enf = i;
                        go = false;
                        break;
                    }
                }
                catch { }
            }
            #endregion 
            try
            {
                #region 
                if (Last.Col > First.Col)
                {
                    #region 当当前鼠标下面的单元格的列索引小于第一个单元格的列索引时
                    for (int c = this.GetColumnByVisibleIndex(First.Col).VisibleIndex; c < this.GetColumnByVisibleIndex(Last.Col).VisibleIndex + 1; c++)
                    {
                        for (int i = inf; i < enf + 1; i++)
                        {
                            if (inf != 0)
                            {
                                if (!table.Rows.Contains(new object[] { idc[i], c }))
                                {
                                    TreeListNode node1 = this.GetNodeByVisibleIndex(i - 1);
                                    TreeListColumn col1 = this.GetColumnByVisibleIndex(c);
                                    double val = 0;
                                    double.TryParse(node1.GetDisplayText(col1), out val);
                                    this.table.Rows.Add(new object[] { idc[i], c, node1, col1, val });
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 当当前鼠标下面的单元格的列索引大于第一个单元格的列索引时

                    for (int c = this.GetColumnByVisibleIndex(Last.Col).VisibleIndex; c < this.GetColumnByVisibleIndex(First.Col).VisibleIndex + 1; c++)
                    {
                        for (int i = inf; i < enf + 1; i++)
                        {
                            if (inf != 0)
                            {
                                if (!table.Rows.Contains(new object[] { idc[i], c }))
                                {
                                    TreeListNode node1 = this.GetNodeByVisibleIndex(i - 1);
                                    TreeListColumn col1 = this.GetColumnByVisibleIndex(c);
                                    double val = 0;
                                    double.TryParse(node1.GetDisplayText(col1), out val);
                                    table.Rows.Add(new object[] { idc[i], c, node1, col1 });
                                }
                            }
                        }
                    }
                    #endregion 
                }
                #endregion
            }
            catch { }
            this.Invalidate(true);
        }

        #endregion

        #region AIP

        //发送消息
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        //获取滚动条位置
        [DllImport("user32")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);
        //设置滚动条位置
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        #endregion

        #region 控制滚动条
        public const int EM_LINESCROLL = 0xb6;
        private void SetScrollPos(int o,int t)
        {
            int i = GetScrollPos(this.Handle, 1);
            //向下滚动一行
            SendMessage(this.Handle, EM_LINESCROLL, o, t);//0，1代表垂直滚动条向下滚动
        }

        #endregion

        #endregion
    }


    /// <summary>
    /// 选中的节点对象类
    /// </summary>
    public class SelectedCell
    {
        //选中单元格的所在节点和所在列
        public SelectedCell() { }
        public SelectedCell(int node, int col)
        {
            _id = node;
            _col = col;
        }
        public SelectedCell(int node, int col,DevExpress.XtraTreeList.Nodes.TreeListNode n,DevExpress.XtraTreeList.Columns.TreeListColumn cols)
        {
            _id = node;
            _col = col;
            _node = n;
            _cols = cols;
        }
        int _id;
        int _col;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }
        DevExpress.XtraTreeList.Nodes.TreeListNode _node;
        public DevExpress.XtraTreeList.Nodes.TreeListNode SelectNode
        {
            get { return _node; }
            set { _node = value; }
        }
        DevExpress.XtraTreeList.Columns.TreeListColumn _cols;
        public DevExpress.XtraTreeList.Columns.TreeListColumn SelectColumn
        {
            get { return _cols;}
            set { _cols = value; }
        }

        public string Text
        {
            get { return _node.GetValue(_cols.ColumnHandle).ToString();}
            set { _node.SetValue(_cols.ColumnHandle, value); }
        }
    }

}
