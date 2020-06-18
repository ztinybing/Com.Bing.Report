using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Com.Bing.API;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Com.Bing.Report
{
    public partial class ReportProperty : Com.Bing.Forms.BaseDialog
    {
        Report report = null;
        public ReportProperty(Report report)
        {
            this.report = report;
            InitializeComponent();
            //根据报表，添加选项卡
            foreach (ReportColumn column in report.Columns)
            {
                XtraTabPage tabpage = new XtraTabPage();
                tabpage.Text = column.Attibutes.NameZh_cn;
                tabpage.Tag = column.ColumnName;
                xtraTabControl1.TabPages.Add(tabpage);
            }
            BindWhileProperty();
            //debug模式下才可设置可变列，不予用户交互
            colDynamicCol.Enabled = Function.DebugMode;
        }
        private void BindWhileProperty()
        {
            fontName.DataBindings.Add(new FontNameBinding("SelectedIndex", report.Attributes, "Font"));
            fontSize.DataBindings.Add(new FontSizeBinding("SelectedIndex", report.Attributes, "Font"));
            mutliRowHeight.DataBindings.Add(new RowHeightBinding("SelectedIndex", report.Attributes, "RowHeiht"));
            hPaging.DataBindings.Add(new HPagingBinding("SelectedIndex", report.Attributes, "Pagination", fixedColNo));
            fixedColNo.DataBindings.Add(new Binding("Text", report.Attributes, "Pagination"));
            lineWidth.DataBindings.Add(new Binding("Text", report.Attributes, "LineWidth"));
            blineWidth.DataBindings.Add(new Binding("Text", report.Attributes, "BordLineWidth"));
            printLast.DataBindings.Add(new Binding("Checked", report.Attributes.PrintStyle, "IsPrintInLast"));
            briftStyle.DataBindings.Add(new Binding("Checked", report.Attributes.PrintStyle, "IsBrifeStyle"));
            orientation.DataBindings.Add(new OrientationBinding("SelectedIndex", report.Attributes, "GrainDirection"));
            upMargin.DataBindings.Add(new MarginBinding("Text", report.Attributes.Margin, "Top"));
            bottomMargin.DataBindings.Add(new MarginBinding("Text", report.Attributes.Margin, "Bottom"));
            leftMargin.DataBindings.Add(new MarginBinding("Text", report.Attributes.Margin, "Left"));
            rightMargin.DataBindings.Add(new MarginBinding("Text", report.Attributes.Margin, "Right"));


        }
        private void BindColProperty(ReportColumn col)
        {
            columnName.DataBindings.Add(new Binding("Text", col.Attibutes, "NameZh_cn"));
            colFontName.DataBindings.Add(new FontNameBinding("SelectedIndex", col.Attibutes, "Font"));
            colFontSize.DataBindings.Add(new FontSizeBinding("SelectedIndex", col.Attibutes, "Font"));
            autoWidth.DataBindings.Add(new Binding("Checked", col.Attibutes, "IsAdjustWeith"));
            autoWrap.DataBindings.Add(new Binding("Checked", col.Attibutes, "AutoWrap"));
            noPrint.DataBindings.Add(new Binding("Checked", col.Attibutes.PrintStyle, "NoPrint"));
            noDataPrint.DataBindings.Add(new Binding("Checked", col.Attibutes.PrintStyle, "NoDataNoPrint"));
            colDynamicCol.DataBindings.Add(new Binding("Checked", col.Attibutes, "DynamicColumn"));


            colForecolor.DataBindings.Add(new ColorBinding("SelectedColor", col.Attibutes, "ForeBrush"));
            colBackgroundColor.DataBindings.Add(new ColorBinding("SelectedColor", col.Attibutes, "BackgroupBrush"));

            //基于图片点击          
            colFontBold.DataBindings.Add(new FontStyleBinding("Checked", col.Attibutes, "Font", "Bold", FontStyle.Bold));
            colFontStike.DataBindings.Add(new FontStyleBinding("Checked", col.Attibutes, "Font", "Italic", FontStyle.Italic));
            colFontUnderLine.DataBindings.Add(new FontStyleBinding("Checked", col.Attibutes, "Font", "Underline", FontStyle.Underline));

            colVUp.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "Alignment", StringAlignment.Near));
            colVCenter.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "Alignment", StringAlignment.Center));
            colVBottom.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "Alignment", StringAlignment.Far));

            colHLeft.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "LineAlignment", StringAlignment.Near));
            colHCenter.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "LineAlignment", StringAlignment.Center));
            colHRight.DataBindings.Add(new VAlignBinding("Checked", col.Attibutes.Valign, "LineAlignment", StringAlignment.Far));

            colUpBorder.DataBindings.Add(new Binding("Checked", col.Attibutes.BoundaryLine, "UpperBoundaryLine"));
            colDownBorder.DataBindings.Add(new Binding("Checked", col.Attibutes.BoundaryLine, "LowerBoundaryLine"));
            colLeftBorder.DataBindings.Add(new Binding("Checked", col.Attibutes.BoundaryLine, "LeftBoundaryLine"));
            colRightBorder.DataBindings.Add(new Binding("Checked", col.Attibutes.BoundaryLine, "RightBooundaryLine"));

            strike.DataBindings.Add(new InLineBinding("Checked", col.Attibutes, "Diagonal", 4));
            stroke.DataBindings.Add(new InLineBinding("Checked", col.Attibutes, "Diagonal", 1));
            backlash.DataBindings.Add(new InLineBinding("Checked", col.Attibutes, "Diagonal", 2));
            crossLine.DataBindings.Add(new InLineBinding("Checked", col.Attibutes, "Diagonal", 3));
        }
        private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            ReportColumn column = report.Columns.Find(xtraTabControl1.SelectedTabPage.Tag.ToString());
            AddColControlToSelectPage(xtraTabControl1.SelectedTabPage);
            ClearColControlDataBinding(xtraTabControl1.SelectedTabPage);
            BindColProperty(column);
        }
        private void AddColControlToSelectPage(XtraTabPage tabPage)
        {
            if (tabPage.Controls.Count > 0)
            {
                return;
            }
            XtraTabPage templatePage = null;
            foreach (XtraTabPage page in this.xtraTabControl1.TabPages)
            {
                if (page.Controls.Count > 0)
                {
                    templatePage = page;
                    break;
                }
            }
            for (int index = templatePage.Controls.Count - 1; index >= 0; index--)
            {
                tabPage.Controls.Add(templatePage.Controls[index]);
            }
        }
        private void ClearColControlDataBinding(XtraTabPage tabPage)
        {
            foreach (Control control in tabPage.Controls)
            {
                control.DataBindings.Clear();
            }
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            report.WriteRpt();
            this.Close();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //report.Restore();
            this.Close();
        }
    }
    public abstract class ObjectBinding : Binding
    {
        public ObjectBinding(string propertyName, object dataObject, string dataMemeber)
            : base(propertyName, dataObject, dataMemeber)
        {
            this.Parse += new ConvertEventHandler(ObjectParse);
            this.Format += new ConvertEventHandler(ObjectFormat);
            this.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged; //及时变改，不经过控件验证
        }
        public abstract void ObjectFormat(object sender, ConvertEventArgs e);
        public abstract void ObjectParse(object sender, ConvertEventArgs e);
    }
    public class FontNameBinding : ObjectBinding
    {
        public FontNameBinding(string propertyName, object dataObject, string dataMemeber)
            : base(propertyName, dataObject, dataMemeber)
        {

        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            string fontName = (e.Value as Font).Name;
            ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
            if (comboBoxEdit.Properties.Items.Contains(fontName))
                e.Value = comboBoxEdit.Properties.Items.IndexOf(fontName);
            else
                e.Value = 0;
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            if ((sender as Binding).DataSource is ReportAttribute)
            {
                ReportAttribute attr = (sender as Binding).DataSource as ReportAttribute;
                ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
                e.Value = new Font(comboBoxEdit.SelectedItem.ToString(), attr.Font.Size, attr.Font.Style);

            }
            else
            {
                TextAttibute attr = (sender as Binding).DataSource as TextAttibute;
                ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
                e.Value = new Font(comboBoxEdit.SelectedItem.ToString(), attr.Font.Size, attr.Font.Style);
            }
        }
    }
    public class FontSizeBinding : ObjectBinding
    {
        public FontSizeBinding(string propertyName, object dataObject, string dataMemeber)
            : base(propertyName, dataObject, dataMemeber)
        {
        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
            Font font = e.Value as Font;
            for (int index = 0; index < comboBoxEdit.Properties.Items.Count; index++)
            {
                if (comboBoxEdit.Properties.Items[index].ToString() == "-" + font.Size)
                {
                    e.Value = index;
                    break;
                }
            }

        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
            if ((sender as Binding).DataSource is ReportAttribute)
            {
                ReportAttribute attr = (sender as Binding).DataSource as ReportAttribute;
                e.Value = new Font(attr.Font.Name, Math.Abs(float.Parse(comboBoxEdit.SelectedItem.ToString()
                )), attr.Font.Style);
            }
            else
            {
                TextAttibute attr = (sender as Binding).DataSource as TextAttibute;
                e.Value = new Font(attr.Font.Name, Math.Abs(float.Parse(comboBoxEdit.SelectedItem.ToString()
                )), attr.Font.Style);
            }
        }
    }
    public class RowHeightBinding : ObjectBinding
    {
        public RowHeightBinding(string propertyName, object dataObject, string dataMemeber)
            : base(propertyName, dataObject, dataMemeber) { }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
            string rowHeight = string.Format("{0:f2}倍行高", e.Value);
            for (int index = 0; index < comboBoxEdit.Properties.Items.Count; index++)
            {
                if (comboBoxEdit.Properties.Items[index].ToString() == rowHeight)
                {
                    e.Value = index;
                    break;
                }
            }
        }

        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            ComboBoxEdit comboBoxEdit = ((sender as Binding).BindableComponent as ComboBoxEdit);
            string rowHeightstr = comboBoxEdit.SelectedItem.ToString();
            e.Value = rowHeightstr.Replace("倍行高", "");
        }
    }
    public class HPagingBinding : ObjectBinding
    {
        Control relateControl = null;
        public HPagingBinding(string propertyName, object dataObject, string dataMemeber, Control relateControl)
            : base(propertyName, dataObject, dataMemeber)
        {
            this.relateControl = relateControl;

        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            if (0 > (int)e.Value)
            {
                e.Value = 1;
            }
            else
            {
                e.Value = 0;
            }
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            if ((int)e.Value == 0)
            {
                e.Value = 0;
                relateControl.Enabled = true;
            }
            else
            {
                e.Value = -1;
                relateControl.Enabled = false;
            }
            relateControl.Refresh();
        }
    }
    public class OrientationBinding : ObjectBinding
    {

        public OrientationBinding(string property, object dataSource, string dataMemeber)
            : base(property, dataSource, dataMemeber)
        {

        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            e.Value = (int)e.Value - 1;
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            e.Value = (int)e.Value + 1;
        }
    }
    public class MarginBinding : ObjectBinding
    {
        public MarginBinding(string propertyName, object dataObject, string dataMemeber)
            : base(propertyName, dataObject, dataMemeber) { }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            e.Value = ConvertUtil.GetBoundaryMM((int)e.Value);
        }

        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            e.Value = ConvertUtil.GetBoundary(e.Value.ToString());
        }
    }
    public class FontStyleBinding : ObjectBinding
    {
        FontStyle style = FontStyle.Regular;
        string styleMemeber = string.Empty;
        public FontStyleBinding(string peroperty, object dataSource, string dataMemeber, string styleMemeber, FontStyle style)
            : base(peroperty, dataSource, dataMemeber)
        {
            this.styleMemeber = styleMemeber;
            this.style = style;
        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            e.Value = GetSetter(e.Value, styleMemeber);
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            Font font = (DataSource as TextAttibute).Font;
            if ((bool)e.Value)
            {
                font = new Font(font, font.Style | style);
            }
            else
            {
                font = new Font(font, font.Style & (~style));
            }
            e.Value = font;

        }
        public object GetSetter(object font, string styleMemeber)
        {
            Type type = font.GetType();
            return type.InvokeMember(styleMemeber, BindingFlags.GetProperty, null, font, null);
        }
    }
    public class VAlignBinding : ObjectBinding
    {
        StringAlignment align = StringAlignment.Center;
        public VAlignBinding(string perporty, object dataSource, string dataMemeber, StringAlignment alignment)
            : base(perporty, dataSource, dataMemeber)
        {
            align = alignment;
        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            e.Value = (align == (StringAlignment)e.Value);
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            if ((bool)e.Value)
            {
                e.Value = align;
            }
        }
    }
    public class InLineBinding : ObjectBinding
    {
        private int lineIndex = -1;
        public InLineBinding(string perporty, object dataSource, string dataMember, int lineIndex)
            : base(perporty, dataSource, dataMember)
        {
            this.lineIndex = lineIndex;
        }
        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            e.Value = (lineIndex == (int)e.Value);
        }
        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            e.Value = lineIndex;
        }
    }
    public class ColorBinding : ObjectBinding
    {
        public ColorBinding(string property, object dataSource, string dataMember)
            : base(property, dataSource, dataMember)
        {

        }

        public override void ObjectFormat(object sender, ConvertEventArgs e)
        {
            SolidBrush brush = e.Value as SolidBrush;
            e.Value = brush.Color;
        }

        public override void ObjectParse(object sender, ConvertEventArgs e)
        {
            e.Value = new SolidBrush((Color)e.Value);
        }

    }
}