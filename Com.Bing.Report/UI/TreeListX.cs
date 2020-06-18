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
        /// Treelist��չ�ؼ�
        /// </summary>
        /// <remarks>Treelist��չ�ؼ�</remarks>
        public TreeListX()
        {
            #region �ؼ�����

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);

            #endregion
        }

        // ΪʲôҪ���Ѵ����ڴ����
        //~TreeListX()
        //{
        //    GC.Collect();
        //}

        #region ��Ա����

        ToolTip toolTip = new ToolTip(); // ��ʾ��

        DataTable table = null;//���ѡ�еĵ�Ԫ����ڰ�סCtrl��ʱ�����������

        DataTable tempTable = null;//ѡ�еĵ�Ԫ���,��������

        SelectedCell FirstCell = new SelectedCell();//��¼�µ�һ����Ԫ��

        bool isMouseDown = false;//����Ƿ���

        string toolTipText = ""; //�Զ���TIP

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

        int inf = 1;//���ֵ����ȳ��ֵ�node��id

        int enf = 0;//���ֵ��к���ֵ�node��id

        bool go = false;//�Ƿ��������

        bool isKeyCtrl = false;//�Ƿ���Ctrl��

        int index = 0; //ÿһ��ѡ�е�ʱ��ȥ�����пɼ��ڵ��id���ϵļ�

        bool AllSelect = false;//�ǲ���ȫѡ��������Ctrlʱ��Ϊȫѡ

        Dictionary<int, int> idc= new Dictionary<int,int>();//ÿ��������׼��ѡ�е�ʱ��Ҫˢ���ֵ�

        Color _selectBackColor = Color.LightSteelBlue;//ѡ�е�Ԫ��ı�����ɫ

        Color focusedbackcolor = Color.White;//���㵥Ԫ��ı�����ɫ

        #endregion

        #region �¼�֪ͨ
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
        #region ��Ա����

        //��ȡѡ�е�Ԫ���node����
        [Browsable(false)]
        public DevExpress.XtraTreeList.Nodes.TreeListNodes SelectNodes
        {
            get 
            {
                //������Ҫ�Ż�
                TreeListNodes nodes = new TreeListNodes(this);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    nodes.Add((TreeListNode)(table.Rows[i]["Node"]));
                }
                return nodes;
            }
        }

        //��ȡѡ�еĵ�Ԫ�����ĺ�
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
        //��ȡѡ�еĵ�Ԫ�����������Լ�����Ķ���
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

        //��ȡ������ѡ�е�Ԫ��ı�����ɫ
        /// <summary>
        /// ѡ�е�Ԫ��ı�����ɫ
        /// </summary>
        /// <remarks>ѡ�е�Ԫ��ı�����ɫ</remarks>
        [Browsable(true)]
        public Color SelectBackColor
        {
            get { return _selectBackColor; }
            set 
            {
                if (value == Color.Transparent)
                {
                    throw new Exception("��֧��͸����ɫ");
                }
                _selectBackColor = value; 
                this.Invalidate(true); 
            }
        }

        //��ȡ�����ý����ı�����ɫ
        /// <summary>
        /// ���㵥Ԫ��ı�����ɫ
        /// </summary>
        /// <remarks>���㵥Ԫ��ı�����ɫ</remarks>
        [Browsable(true)]
        public Color FocusedBackColor
        {
            get { return focusedbackcolor; }
            set { focusedbackcolor = value; }
        }

        /// <summary>
        /// ��ȡѡ�й����е�����
        /// </summary>
        /// <remarks>��ȡѡ�й����е�����</remarks>
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
        /// ��ȡѡ���е�����������˵ѡ�нڵ������
        /// </summary>
        /// <remarks>��ȡѡ���е�����������˵ѡ�нڵ������</remarks>
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
        /// ��ȡ������ѡ��ģʽ
        /// </summary>
        /// <remarks>��ȡ������ѡ��ģʽ</remarks>
        bool preMultiSelect = false;
        bool preEnableAppearanceFocusedRow = false;
        public SelectModeEnum SelectMode
        {
            get { return sm; }
            set
            {
                sm = value;
                if (table == null) //�״�ʹ��
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
                    //�ո����ó�Marquee��ʱ���أ���������ϴ�Marquee�Ķ���

                    #region ע��Ҫ�õ��¼�

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

                    #region ��ԭ����
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
        /// ��ȡ������ȫѡ������������
        /// </summary>
        /// <remarks>��ȡ������ȫѡ������������</remarks>
        [Browsable(true)]
        public int WarningCount
        {
            get { return _mcount; }
            set { _mcount = value; }
        }

        #endregion

        #region ��д

        //��Ϊѡ��ʽΪMarquee��ʱ�򣬲�Ҫ��ѡ����Ҫѡ����
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
        //��ΪMarquee��ʱ�򣬵��һ�²�����Ԫ�����뽹��
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

        #region ��Ա����

        #region �����¼�
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
                    #region һ��ѡ�еĵ�Ԫ�񡪡����������������ѡ��Ļ����,�����Ѿ����
                    //�ӵ�һ������ǰ����µĵ�Ԫ���λ�ã�����ѭ���ϴ�

                    tempTable.Rows.Clear();
                    #region ���ѭ�����˿���node��id˳��,���ܿ����Ż�

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

                    #region ��ѡ�еĵ�Ԫ��ľ��δ�ŵ�tempselect�У�����select����Ҫ�����˵Ļ����Ͳ��Ž�ȥ��
                    double val;
                    if (col.VisibleIndex > FirstCell.Col)
                    {
                        //����ǰ�������ĵ�Ԫ���������С�ڵ�һ����Ԫ���������ʱ
                        //����ѡ�й�����
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
                        //����ǰ�������ĵ�Ԫ������������ڵ�һ����Ԫ���������ʱ
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
                    //�������»���
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
        /// ��node��˳�򱣴浽�ֵ�
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
        /// ���ⰴ������뿪�ؼ��������������ſ�
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
            //ÿһ�ε���󣬸����ֵ�idc�����Ż�Ϊ����Դ�ı�+���������
            idc.Clear();
            getNodeID((TreeListNode)null, idc);//��ȡ��ǰtree�пɼ���node

            #region ��ʧȥ����������»�ý�����ʱ�ж��ǲ��ǰ�����Ctrl��
            //�������ʱ���ȡ�������
            if (Control.ModifierKeys == Keys.Control)
            {
                isKeyCtrl = true;
            }
            else
            {
                isKeyCtrl = false;
            }
            #endregion

            AllSelect = false;//����ȫѡ״̬

            this.Invalidate(true);//���»��Ʊ���

            isMouseDown = false;//������갴��״̬

            #region �����ǰ���������������ѡ��ģʽ������Marquee
            if (e.Button == MouseButtons.Left && SelectMode == SelectModeEnum.Marquee)
            {
                Point p = this.PointToClient(Cursor.Position);
                isMouseDown = true;
                //����갴��׼����ѡ��ʱ�����ĳ��Ԫ�������뽹�㣬���ܹ������������
                CellInfo cell = this.ViewInfo.GetHitTest(p).CellInfo;
                if (cell == null || cell.Focused)
                {
                    isMouseDown = false;
                }
                #region ��ԭδѡ��״̬��
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
                //����ѡ��Ҫ��󣬰ѵ�ǰ��Ԫ���¼Ϊ��һ����Ԫ��
                if (node != null && col != null)
                {
                    FirstCell = new SelectedCell();
                    FirstCell.Col = col.VisibleIndex;//.ColumnHandle;
                    FirstCell.ID = node.Id;
                    //���»��ƺ󣬽���һ����Ԫ����ӵ�ѡ�м���
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

        #region ����ѡ�е�Ԫ��

        //����ѡ�еĵ�Ԫ��,��ʱ�и�������ɫ��˼��˼
        private void TreeGrid_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            if (this.SelectMode == SelectModeEnum.Marquee)
            {
                if (AllSelect)//ȫѡ���Ǻ�
                {
                    e.Graphics.FillRectangle(new SolidBrush(SelectBackColor), new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - e.Bounds.Left, e.Bounds.Bottom - e.Bounds.Top));
                }
                else
                {
                    #region 
                    try
                    {
                        if (//��������������Ϊѡ��//��ѡ�еĺܶ��ʱ�򣬸о�����
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
                        if (e.Focused)//��Excelһ���������㵥Ԫ�񻭲�ͬ�ı���
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

        #region Ctrl��
        private void TreeGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    isKeyCtrl = false;
                    //����Ctrl+A=ȫѡ
                    if (this.RowCount > WarningCount&&WarningCount!=0)
                    {
                        if (DialogResult.No == MessageBox.Show("���������󣬿��ܻ���ɶ��ݿ��������Ե�Ƭ�̼��ɡ�\r\n�Ƿ������", "����", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
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
                this.OptionsBehavior.Editable = false;// ���������Ctrl���������ĵ�Ԫ��������
            }

        }

        private void TreeGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                isKeyCtrl = false;
                this.OptionsBehavior.Editable = true;// ����ſ���Ctrl�����ؼ����Ա༭
            }
        }
        #endregion

        #region ����ѡ�з���
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
                getNodeID((TreeListNode)null, idc);//��ȡ��ǰtree�пɼ���node
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
                    #region ����ǰ�������ĵ�Ԫ���������С�ڵ�һ����Ԫ���������ʱ
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
                    #region ����ǰ�������ĵ�Ԫ������������ڵ�һ����Ԫ���������ʱ

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

        //������Ϣ
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        //��ȡ������λ��
        [DllImport("user32")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);
        //���ù�����λ��
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        #endregion

        #region ���ƹ�����
        public const int EM_LINESCROLL = 0xb6;
        private void SetScrollPos(int o,int t)
        {
            int i = GetScrollPos(this.Handle, 1);
            //���¹���һ��
            SendMessage(this.Handle, EM_LINESCROLL, o, t);//0��1����ֱ���������¹���
        }

        #endregion

        #endregion
    }


    /// <summary>
    /// ѡ�еĽڵ������
    /// </summary>
    public class SelectedCell
    {
        //ѡ�е�Ԫ������ڽڵ��������
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
