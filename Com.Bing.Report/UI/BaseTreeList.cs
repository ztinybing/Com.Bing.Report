using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Com.Bing.Business;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraBars.Docking;
using System.Drawing;
using DevExpress.Utils.Drawing;
using Com.Bing.API;

namespace Com.Bing.UI
{
    public class BaseTreeList : TreeListX, IContextMenuStrip
    {
        private bool isItemChanged = false;
        public bool IsItemChanged
        {
            get { return isItemChanged; }
        }
        private ContextMenuStrip contextMenuStrip = null;
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return contextMenuStrip; }
            set { contextMenuStrip = value; }
        }
        public virtual TreeListNode CurEditNode
        {
            get { return null; }
        }
        public BaseTreeList()
            : base()
        {
            this.OptionsBehavior.ImmediateEditor = false;
            this.KeyUp += new KeyEventHandler(BaseTreeList_KeyUp);
            this.EditorKeyDown += new KeyEventHandler(BaseTreeList_EditorKeyDown);
            this.CellValueChanging += new CellValueChangedEventHandler(BaseTreeList_CellValueChanging);
            this.CellValueChanged += new CellValueChangedEventHandler(BaseTreeList_CellValueChanged);
            this.MouseWheel += new MouseEventHandler(BaseTreeList_MouseWheel);
            this.MouseDown += DefaultMouseDown;
            this.MouseClick += DefaultMouseClick;

            AppearanceObjectHelper.LoadFont(this.Appearance.Row);
        }

        public bool ShowFocusNode
        {
            set
            {
                this.CustomDrawNodeCell -= BaseTreeList_CustomDrawNodeCell;
                if (value)
                {
                    this.CustomDrawNodeCell += BaseTreeList_CustomDrawNodeCell;
                }
            }
        }
        void BaseTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Node == this.FocusedNode)
            {
                e.Appearance.BackColor = Color.Blue;
                e.Appearance.ForeColor = Color.White;
            }
        }

        //为了准确获取hitInfo 在MouseDown时记录下此值
        protected TreeListHitInfo hitInfo = null;
        void DefaultMouseDown(object sender, MouseEventArgs e)
        {
            hitInfo = this.CalcHitInfo(new Point(e.X, e.Y));
            if (Function.DebugMode && hitInfo.Node != null)
            {
                Function.SetProfileString(Function.IniFile, "调试信息", "焦点控件文本", hitInfo.Node.GetDisplayText(hitInfo.Column));
            }
        }

        void activateParentForm()
        {
            Control parent = this;
            while (parent != null)
            {
                if (parent is Form)
                {
                    (parent as Form).Activate();
                    break;
                }
                parent = parent.Parent;
            }
        }

        public override void FilterNodes()
        {
            base.FilterNodes();
            if (Nodes.Count > 0)
            {
                TreeListNode node = FocusedNode;
                FocusedNode = Nodes[0];
                FocusedNode = node;
            }
        }
        protected virtual void DefaultMouseClick(object sender, MouseEventArgs e)
        {
            activateParentForm();
            if (hitInfo == null) return;
            //DevExpress.XtraTreeList.TreeListHitInfo hitInfo = this.CalcHitInfo(new Point(e.X, e.Y));
            if (hitInfo.Node == null) return;
            if (hitInfo.Column == null) return;

            this.Focus();
            this.FocusedNode = hitInfo.Node;
            this.FocusedColumn = hitInfo.Column;
            //鼠标键处理
            if (e.Button == MouseButtons.Right)
            {
                //如果处于选择模式，推送选择结果，解除选择模式
                if (SelectMode != SelectModeEnum.Default)
                {
                    SelectMode = SelectModeEnum.Default;
                    fireSelDone();
                }
                else if (contextMenuStrip != null) //如果自定义菜单不为空，则显示出来
                {
                    contextMenuStrip.Show(Cursor.Position.X, Cursor.Position.Y);
                }

                //右键取消选择
                if (this.Selection.Count <= 1)
                {
                    this.Selection.Clear();
                    this.Selection.Add(hitInfo.Node);
                }

            }
            else if (e.Button == MouseButtons.Left)
            {
                //左键取消选择
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control &&
                    (Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                {
                    this.Selection.Clear();
                    this.Selection.Add(hitInfo.Node);
                }
            }

        }
        protected override void OnItemChanged(int index)
        {
            if (IsUnboundLoad) return;
            isItemChanged = true;
            TreeListNode node = FindNodeByID(index);
            ViewInfo.UpdateRowViewInfo(new RowInfo(ViewInfo, node));
        }
        private bool singleItemPropChanging = true;
        public void SetSingleItemPropChanging()
        {
            singleItemPropChanging = true;
        }
        protected override void UpdateLayout()
        {
            try
            {
                if (!isItemChanged)
                {   
                    // base.UpdateLayout();                    
                    if (singleItemPropChanging)
                    {
                        UpdateLayoutButScrollBar();
                        singleItemPropChanging = false;
                    }
                    else
                    {
                        base.UpdateLayout();
                    }
                    
                }
            }
            catch
            {
                //如果TreeList页出现大X的情况，则是TreeList自身BUG
            }
            isItemChanged = false;
        }
        private void UpdateLayoutButScrollBar()
        {
            if (ViewInfo.IsValid || IsLockUpdate) return;
            FitColumns();
            ViewInfo.CalcViewInfo();
            //UpdateScrollBars();
            //navigationHelper.UpdateButtons();
            RaiseLayoutUpdated();
        }
        void BaseTreeList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }
        }

        public void SetValue(TreeListNode node, TreeListColumn column, object value)
        {
            setValue(new object[] { node, column, value });
        }
        protected void setValue(object[] args)
        {
            TreeListNode node = (args[0] as TreeListNode);
            //StorageTreeElement element = GetDataRecordByNode(node) as StorageTreeElement;
            //if (element != null)
            //{
            //    element.BeginResetItem();
            //    node.SetValue(args[1] as TreeListColumn, args[2] as string);
            //    element.EndResetItem();
            //}
        }

        private object oldCellValue = "";
        public object OldCellValue
        {
            get
            {
                return oldCellValue;
            }
        }
        private bool isFirstChange = true;
        void BaseTreeList_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (isFirstChange)
            {
                oldCellValue = e.Node.GetValue(e.Column);
                isFirstChange = false;
            }
        }

        void BaseTreeList_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            isFirstChange = false;
        }

        public new void RaiseCellValueChanged(CellValueChangedEventArgs e)
        {
            base.RaiseCellValueChanged(e);
        }
        public void RaiseFocusedNodeChanged(TreeListNode newFocusNode)
        {
            base.RaiseFocusedNodeChanged(this.FocusedNode, newFocusNode);
        }

        private bool lockDefaultFocus = false;
        public void LockDefaultFocus()
        {
            lockDefaultFocus = true;
        }
        private int defaultFocusColumn = 0;
        public int DefaultFocusColumn
        {
            set
            {
                defaultFocusColumn = value;
            }
            get
            {
                return defaultFocusColumn;
            }
        }
        public virtual bool NeedDefaultFocus
        {
            get
            {
                return false;
            }
        }
        private Keys prevKeyCode = Keys.None;
        public Keys PrevKeyCode
        {
            get { return prevKeyCode; }
        }
        void BaseTreeList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.FocusedNode != null && this.FocusedColumn != null)
            {
                if (lockDefaultFocus)
                {
                    lockDefaultFocus = false;
                    return;
                }

                e.Handled = true;
                if (this.FocusedNode.NextVisibleNode != null)
                {
                    this.FocusedNode = this.FocusedNode.NextVisibleNode;
                }
                if (NeedDefaultFocus && DefaultFocusColumn >= 0 && DefaultFocusColumn < this.Columns.Count)
                {
                    this.FocusedColumn = this.Columns[DefaultFocusColumn];
                }
                if (FocusedNode != null)
                {
                    this.Selection.Clear();
                    this.Selection.Add(FocusedNode);
                }
            }
            e.Handled = false;
        }

        void BaseTreeList_EditorKeyDown(object sender, KeyEventArgs e)
        {
            prevKeyCode = e.KeyCode;
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            try
            {
                base.OnPaint(e);
            }
            catch
            {
                //Kill one
            }
        }
    }
}
