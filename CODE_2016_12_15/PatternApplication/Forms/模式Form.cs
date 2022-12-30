using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using PatternApplication.DataObject;
using PatternApplication.Interfaces;
using DevExpress.XtraTreeList.Columns;
using System.Configuration;
using System.Diagnostics;
using System.Timers;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace PatternApplication.Forms
{
    public partial class 模式Form : DevExpress.XtraBars.Ribbon.RibbonForm, ISaveForm
    {
        //private bool _isFirstLoaded = true;

        private Guid _markRowID;
        private string _markCol = null;
        private TreeList _markTreeList;

        private int viewstatus = 0;

        private TreeList activelist;

        private TreeListNode refnode;

        private TreeListNode dragnode;
        private TreeListColumn dragcol;

        private Guid 左对象;
        private Guid 右对象;

        private List<模式> 快速配置_关联类型 = new List<模式>();
        private List<Guid> selectrowid = new List<Guid>();
        int index = 0;

        private bool 是否精确匹配 = false;
        HashSet<Guid> ids = new HashSet<Guid>();
        句子测试Form 句子测试frm; 
        public 模式Form()
        {
            InitializeComponent();

            Load += OnLoad;
            Data.对话frm = new 对话Form();
            句子测试frm = new 句子测试Form();
            句子测试frm.init();
            for (int i = 0; i < 句子测试frm.Controls.Count; i++)
            {
                this.dockPanel1.Controls.Add(句子测试frm.Controls[i]);
            }
        }

        public DevExpress.XtraBars.BarEditItem 编辑框()
        {
            return EditString;
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            InitControls();
            Data.模式frm = this;

            Data.patternDataSet = PatternDataSet;

            Data.模式表.TableChangeEvent += 模式OnTableChangeEvent;
            Data.模式编辑表.TableChangeEvent += 模式OnTableChangeEvent;

            treeListEdit.CollapseAll();
            foreach (DataRow row in PatternDataSet.字典_语言)
            {
                repositoryItemComboBox1.Items.Add(row["名称"]);
                repositoryItemComboBox5.Items.Add(row["名称"]);
            }
            选择生成语言.EditValue = "汉";
            选择解析语言.EditValue = "汉";
            foreach (DataRow row in PatternDataSet.字典_限定目标)
            {
                repositoryItemComboBox2.Items.Add(row["名称"]);
            }
            //读取“是否调试模式”的配置标志
            string value = ConfigurationManager.AppSettings.Get("IsDebugMode");
            if (value != null && value.ToLower() == "true")
                Data.是调试模式 = true;
            else
                Data.是调试模式 = false;

            Data.重置缓存根模式集合();            

            //加载快速配置数据绑定
            快速配置_关联类型.Add(Data.FindRowByID(Data.拥有Guid));
            快速配置_关联类型.Add(Data.FindRowByID(Data.聚合Guid));
            快速配置_关联类型.Add(Data.FindRowByID(Data.概念拥有属性Guid));
            快速配置_关联类型.Add(Data.FindRowByID(Data.概念拥有角色Guid));
            this.lookUpEdit关联.Properties.DataSource = 快速配置_关联类型;
            this.lookUpEdit关联.EditValue = null;

            //添加拖拽处理事件
            添加拖拽处理事件(this.lookUpEdit关联);
            添加拖拽处理事件(this.textEditA端);
            添加拖拽处理事件(this.textEditB端);
            添加拖拽处理事件(this.textEdit父对象);
        }

        private void 模式OnTableChangeEvent(object sender, BaseTable<Guid, 模式>.TableChangeEventArgs<模式> args)
        {
            var mainFrom = this.ParentForm as MainForm;
            if (mainFrom != null)
                mainFrom.iSave.Enabled = true;
        }


        private void InitControls()
        {            
            barButtonItemRefreshChildrenSn.ItemClick += 刷新子级一级序号;

            barButtonItemPaste.ItemClick += On粘贴ItemClick;
            barButtonItemPasteChild.ItemClick += BarButtonItem粘贴行到子级OnItemClick;
            barButtonItemPasteRow.ItemClick += 粘贴行到前_ItemClick;

            把子树挂接到标记字段.ItemClick += BarButtonItemCopyRowIdOnItemClick;
            barButtonItemCopy.ItemClick += BarButtonItemCopyOnItemClick;
            barButtonItemCopyRow.ItemClick += BarButtonItemCopyRowOnItemClick;
            barButtonItemCopyTree.ItemClick += BarButtonItemCopyTreeOnItemClick;

            barButtonItemNew.ItemClick += BarButtonItemNewOnItemClick;
            barButtonItemNewChild.ItemClick += BarButtonItemNewChildOnItemClick;

            barEditItemForeColor.EditValueChanged += BarEditItemForeColorOnEditValueChanged;
            barEditItemBackColor.EditValueChanged += BarEditItemBackColorOnEditValueChanged;


            popupMenu1.BeforePopup += PopupMenu1OnBeforePopup;

            ribbon.Manager.HighlightedLinkChanged += ManagerOnHighlightedLinkChanged;

            treeListBase.FocusedColumnChanged += TreeListOnFocusedColumnChanged;
            treeListBase.FocusedNodeChanged += TreeListOnFocusedNodeChanged;
            treeListBase.MouseDown += TreeListOnMouseDown;
            //treeListBase.NodeCellStyle += TreeListOnNodeCellStyle;
            treeListBase.MouseDoubleClick += TreeListOnMouseDoubleClick;
            treeListBase.DragDrop += treeList_DragDrop;
            treeListBase.DragOver += treeList_DragOver;
            treeListBase.DragEnter += treeList_DragEnter;
            treeListBase.MouseUp += TreeListOnMouseUp;
            treeListBase.CellValueChanging += treeListCellValueChanging;
            treeListBase.CellValueChanged += treeListEdit_CellValueChanged;
            treeListBase.BeforeDragNode += treeList_BeforeDragNode;
            //treeListBase.FilterNode += treeListBase_FilterNode;
            treeListBase.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;

            treeListEdit.FocusedColumnChanged += TreeListOnFocusedColumnChanged;
            treeListEdit.FocusedNodeChanged += TreeListOnFocusedNodeChanged;
            treeListEdit.MouseDown += TreeListOnMouseDown;
            //treeListEdit.NodeCellStyle += TreeListOnNodeCellStyle;
            treeListEdit.MouseDoubleClick += TreeListOnMouseDoubleClick;
            treeListEdit.DragDrop += treeList_DragDrop;
            treeListEdit.DragOver += treeList_DragOver;
            treeListEdit.DragEnter += treeList_DragEnter;
            treeListEdit.MouseUp += TreeListOnMouseUp;
            treeListEdit.CellValueChanging += treeListCellValueChanging;
            treeListEdit.CellValueChanged += treeListEdit_CellValueChanged;
            treeListEdit.BeforeDragNode += treeList_BeforeDragNode;
            treeListEdit.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;

            treeListParam.FocusedColumnChanged += TreeListOnFocusedColumnChanged;
            treeListParam.FocusedNodeChanged += TreeListOnFocusedNodeChanged;
            treeListParam.MouseDown += TreeListOnMouseDown;
            //treeListParam.NodeCellStyle += TreeListOnNodeCellStyle;
            treeListParam.MouseDoubleClick += TreeListOnMouseDoubleClick;
            //treeListParam.DragDrop += treeList_DragDrop;
            treeListParam.DragOver += treeList_DragOver;
            treeListParam.DragEnter += treeList_DragEnter;
            treeListParam.MouseUp += TreeListOnMouseUp;
            treeListParam.BeforeDragNode += treeList_BeforeDragNode;
            treeListParam.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;

            treeliststring.FocusedNodeChanged += TreeListOnFocusedNodeChanged;
            treeliststring.MouseDown += TreeListOnMouseDown;
            treeliststring.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;
            treeListresult.MouseDown += TreeListOnMouseDown;
            treeListresult.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;

            treeList待生长对象对.CustomDrawNodeCell += TreeListOnCustomDrawNodeCell;

            CustomizeTreeListPatternAppearance();

        }

        private void CustomizeTreeListPatternAppearance()
        {
            StyleFormatCondition condition1 = new StyleFormatCondition(
              DevExpress.XtraGrid.FormatConditionEnum.Equal, treeListBase.Columns["显隐"],
              null, 字典_显隐.无效);
            condition1.ApplyToRow = true;
            condition1.Appearance.BackColor = Color.Gray;
            treeListBase.FormatConditions.Add(condition1);

            StyleFormatCondition condition2 = new StyleFormatCondition(
            DevExpress.XtraGrid.FormatConditionEnum.Equal, treeListEdit.Columns["显隐"],
                null, 字典_显隐.无效);
            condition2.ApplyToRow = true;
            condition2.Appearance.BackColor = Color.Gray;
            treeListEdit.FormatConditions.Add(condition2);

        }

        void bindingSourceBase_ListChanged(object sender, ListChangedEventArgs e)
        {
            iBaseCount.EditValue = bindingSourceBase.Count;           
        }

        void treeListEdit_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            TreeList 模式treelist = (TreeList)sender;
            模式 row = GetRowByNode(模式treelist, e.Node);
            Data.FindTableByID(row.ID).更新对象(row);

            刷新当前模式行(row.ID);
        }

        private 模式 GetRowByNode(TreeList treeList, TreeListNode node)
        {
            return treeList.GetDataRecordByNode(node) as 模式;
        }

        private DataRow GetParamRowByNode(TreeList treeList, TreeListNode node)
        {
            DataRowView dv = (DataRowView)treeList.GetDataRecordByNode(node);

            return dv.Row;
        }

        private void TreeListOnCustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Node.TreeList == treeList待生长对象对)
            {
                if (e.Column.FieldName == "左对象描述" || e.Column.FieldName == "右对象描述")
                {
                    string value=(string)e.Node.GetValue("中心对象描述");
                    if (value == "1" && e.Column.FieldName == "左对象描述")
                    {
                        Font font = new Font(e.Appearance.Font.FontFamily.Name, e.Appearance.Font.Size, FontStyle.Bold);
                        e.Appearance.Font = font;
                    }else if (value == "0" && e.Column.FieldName == "右对象描述")
                    {
                        Font font = new Font(e.Appearance.Font.FontFamily.Name, e.Appearance.Font.Size, FontStyle.Bold);
                        e.Appearance.Font = font;
                    }
                }
                if ((string)e.Node.GetValue("是否成功生长描述") == "是")
                {
                    e.Appearance.ForeColor = Color.Red;
                }else if ((string)e.Node.GetValue("是否已生长描述") == "是")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
                else if ((string)e.Node.GetValue("是否已生长描述") == "否")
                {
                    e.Appearance.ForeColor = Color.Gray;
                }
                return;
            }
            if (e.Node.TreeList == treeliststring)
            {

            }
            Guid v = (Guid)e.Node.GetValue("ID");
            if (v != Guid.Empty && v == _markRowID)
            {
                if (_markCol == null)
                    e.Appearance.BackColor = Color.Gold;
                else if (_markCol == e.Column.FieldName)
                    e.Appearance.BackColor = Color.Gold;
            }

            if (e.Node == refnode)
                e.Appearance.BackColor2 = Color.YellowGreen;

            if (e.Node.TreeList == treeListresult)
            {
                if (v.Equals(左对象) || v.Equals(右对象))
                    e.Appearance.BackColor2 = Color.YellowGreen;
            }


            if (e.Node == dragnode && e.Column == dragcol)
                e.Appearance.BackColor = Color.Green;

            if ((e.Node.TreeList == treeListBase || e.Node.TreeList == treeListEdit) && e.Column.FieldName == "形式")
            {
                Guid v3 = (Guid)e.Node.GetValue("连接");

                if (Data.拥有形式Guid.Equals(v) || Data.拥有形式Guid.Equals(v3))
                {
                    if (Data.在基本关联集合中(v, false) || Data.在基本关联集合中((Guid)e.Node.GetValue("源记录"), false))
                        e.Appearance.ForeColor = Color.Green;//原始的形式行。
                    else
                        e.Appearance.ForeColor = Color.SteelBlue;//派生的形式行，其实是完全可以省略的。
                }
                else if (Data.拥有语言角色Guid.Equals(v) || Data.拥有语言角色Guid.Equals(v3))
                {
                    e.Appearance.ForeColor = Color.DarkOrchid;//拥有语言角色。
                }
                else if (Data.属于Guid.Equals(v3))
                {
                    if (Data.是子记录(Data.FindRowByID((Guid)e.Node.GetValue("B端")), Data.形式Guid, true))
                        e.Appearance.ForeColor = Color.Green;
                }
            }

            if ((e.Node.TreeList == treeliststring) && e.Column.FieldName == "字符串" && e.Node.ParentNode == null)
                e.Appearance.ForeColor = Color.Green;

            if ((e.Node.TreeList == treeListresult) && e.Column.FieldName == "字符串")
            {
                生长对象 obj = Find节点对象(v);
                if (obj != null)
                {
                    if (obj.是无形式空对象 || obj.处理轮数==-2)
                        e.Appearance.ForeColor = Color.SteelBlue;
                    else if (obj.中心对象 == null)
                        e.Appearance.ForeColor = Color.Green;
                    if(obj.概率打分<=0)
                        e.Appearance.ForeColor = Color.DimGray;
                }
            }

            if (!(e.CellValue is Guid))
                return;

            if (Guid.Empty.Equals(e.CellValue))
            {
                e.CellText = "";
                return;
            }

            if (e.Node.TreeList == treeListParam)
            {
                DataRow r = GetParamRowByNode(e.Node.TreeList, e.Node);
                //if ("属于" == (string)r["标记"])
                //    e.Appearance.ForeColor = Color.SkyBlue;
                if ("拥有" == (string)r["标记"])
                    e.Appearance.ForeColor = Color.DimGray;
                else if ("拥有形式" == (string)r["标记"])
                    e.Appearance.ForeColor = Color.Green;
                else if ("This" == (string)r["标记"])
                    e.Appearance.ForeColor = Color.DarkRed;
            }

            模式 row;

            row = Data.FindRowByID((Guid)e.CellValue);

            if (row == null)
                return;

            模式 thisrow = GetRowByNode((TreeList)sender, e.Node);

            e.CellText = row.形式;



            if (thisrow != null)
            {
                Guid v1 = (Guid)thisrow.ParentID;
                if (!v1.Equals(Data.NullParentGuid))
                {
                    Guid g = (Guid)v1;
                    if (g.Equals(e.CellValue))
                        e.CellText = "↑" + e.CellText;//等于父节点，进行特殊标记。

                }
            }

            //          if (vv.ToString() == v1.ToString())

            if (e.Node["That根"] != null)
            {
                int k = (int)e.Node["That根"];
                if ((e.Column.FieldName == "A端" && k == 字典_目标限定.A端)
                    || (e.Column.FieldName == "B端" && k == 字典_目标限定.B端)
                    || (e.Column.FieldName == "源记录" && k == 字典_目标限定.连接))
                    e.Appearance.ForeColor = Color.Brown;
            }


           
            //row = patternDataSet.模式.FindByID((Guid)e.CellValue);
            //if (row == null)
            //{
            //	row = patternDataSet.模式编辑.FindByID((Guid)e.CellValue);
            //}

            //if (row != null)
            //{
            //	e.CellText = row["形式"].ToString();
            //}
            //else
            //{
            //	e.CellText = "(空)";
            //}
        }

        private void TreeListOnMouseDoubleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            var treeList = sender as TreeList;
            if (treeList == null)
                return;

            if (treeList == treeListParam)//这个窗口暂时不响应双击
                return;

            //对于编辑窗口，双击的时候认为是标记。
            if (treeList == treeListEdit)
                MarkRowColumn(treeList);


            列出参数();

            //MarkRowColumn(treeList);双击不要进行标记了。
        }

        private void MarkRowColumn(Guid row, string col)
        {
            _markRowID = row;
            if (Data.IsObjectField(col, true))
                _markCol = col;

            TreeListNode node=FindNodeByID(_markRowID);
            if (node!=null)
                _markTreeList = node.TreeList;

            UpdateMarkRowColumnUi();

        }

        private void MarkRowColumn(TreeList treeList)
        {
            if (treeList == null || treeList.FocusedNode == null || treeList.FocusedNode.Focused == false)
            {
                return;
            }

            模式 _markRow = GetRowByNode(treeList, treeList.FocusedNode);

            _markCol = null;

            if (_markRow != null)
                _markRowID = _markRow == null ? Guid.Empty : (Guid)_markRow.ID;

            _markTreeList = FindNodeByID(_markRowID).TreeList;

            if (treeList.FocusedColumn != null && Data.IsObjectField(treeList.FocusedColumn.FieldName, true))
                _markCol = treeList.FocusedColumn.FieldName;

            UpdateMarkRowColumnUi();

            //treeListParam.CancelUpdate();


        }

        private void 加入参数(Guid ObjectID, string thatcol)
        {
            object source = treeListParam.DataSource;

            //treeListParam.DataSource = null;
            模式 row = Data.FindRowByID(ObjectID);

            if (Data.是派生类(Data.系统对象Guid, row, 替代.正向替代))
                return;

            treeListParam.BeginUpdate();

            PatternDataSet.模式查找.Clear();


            float 阀值 = float.Parse(阀值Text.Text);

            Data.加入基类树到参数(null, row, thatcol, 包含编辑.Checked, 仅知识.Checked, 阀值, Data.当前解析语言);

            //Guid id = (Guid)row["ID"];
            //if(thatcol!="")
            //	id = (Guid)row[thatcol];

            //Guid id = (Guid)row[thatcol];
            //if (Data.ThisGuid.Equals(id))
            //    id = ObjectID;

            //row = Data.FindRowByID(id);
            if (派生.Checked)
                Data.加入派生树到参数(null, row, thatcol, 仅知识.Checked, 阀值);
            //treeListParam.Update();

            //			treeListParam.DataSource = source;

            treeListParam.ExpandAll();

            treeListParam.EndUpdate();

            treeListParam.MoveFirst();

        }

        private void UpdateMarkRowColumnUi()
        {

            barEditItemMarkRow.EditValue = barEditItemMarkCol.EditValue = null;

            if (_markRowID == Guid.Empty)
                goto End;

            模式 _markRow = Data.FindRowByID(_markRowID);

            if (_markRow == null)
                goto End;

            barEditItemMarkRow.EditValue = _markRow.形式;

            if (_markCol == null)
                goto End;
            var a = typeof(模式).GetProperty(_markCol).GetValue(_markRow);
            if (a is Guid)
            {
                模式 _markColRow = Data.FindRowByID((Guid)a);
                barEditItemMarkCol.EditValue = _markCol + "：";
                if (_markColRow != null)
                {
                    barEditItemMarkCol.EditValue += _markColRow.形式;
                }
            }

        End:

            treeListBase.Refresh();
            treeListEdit.Refresh();

        }

        private void BarButtonItemCopyTreeOnItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
            {
                return;
            }

            模式 row = Data.FindRowByID((Guid)activelist.FocusedNode["ID"]);
            if (row == null)
                return;

            List<模式> rows = Data.CopyTree(row);
            Data._clipObject = rows;

            //BindingSource rowsource = activelist.DataSource as BindingSource;

            //row = Data.CopyRow(row, rowsource.DataMember);
            //if (row == null)
            //    return;

            //var rows = new List<DataRow>();

            //rows.Add(row);

            //rows.AddRange(Data.CopyTree(activelist.FocusedNode));

            //Data._clipObject = rows;
        }





        private void BarButtonItemNewChildOnItemClick(object sender, ItemClickEventArgs e)
        {

            //var newRow = NewRow(treeListBase.FocusedNode);
            //PatternDataSet.模式.Add模式Row(newRow);

            //var node = treeListBase.FindNodeByKeyID(newRow.ID);
            //if (node != null)
            //{
            //    treeListBase.FocusedNode = node;
            //}
        }



        private void BarButtonItemNewOnItemClick(object sender, ItemClickEventArgs e)
        {
            //var newRow = NewRow();
            //PatternDataSet.模式.Add模式Row(newRow);

            //var node = treeListBase.FindNodeByKeyID(newRow.ID);
            //if (node != null)
            //{
            //    treeListBase.FocusedNode = node;
            //}
        }

        private void BarButtonItemCopyRowOnItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.Selection.Count == 0)
                return;

            List<模式> rows = new List<模式>();
            foreach (TreeListNode node in activelist.Selection)
            {
                模式 row = Data.FindRowByID((Guid)node["ID"]);

                row = Data.CopyRow(row, false);

                if (row != null)
                    rows.Add(row);
            }

            Data._clipObject = rows;
        }

        //private PatternDataSet.模式Row NewRow(TreeListNode parentNode = null)
        //{
        //	var newObj = PatternDataSet.模式.New模式Row();
        //	newObj.ID = Guid.NewGuid();
        //	if (parentNode != null)
        //	{
        //		newObj.ParentID = (Guid)parentNode["ID"];
        //	}

        //	return newObj;
        //}




        private void ManagerOnHighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
        {
            if (e.Link != null)
            {
                var superTip = e.Link.Item.SuperTip;
                if (superTip != null)
                {
                    try
                    {
                        var info = new ToolTipControlInfo(MousePosition, null) { SuperTip = superTip };
                        toolTipController1.ShowHint(info);

                        return;
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            toolTipController1.HideHint();
        }

        private void PopupMenu1OnBeforePopup(object sender, CancelEventArgs e)
        {
            barButtonItemPaste.Enabled = (Data._clipObject != null && !(Data._clipObject is List<DataRow>));
            barButtonItemPasteRow.Enabled = Data._clipObject is List<模式>;
            barButtonItemPasteChild.Enabled = (Data._clipObject as List<模式>) != null;
            barButtonItemCopyTree.Enabled = (activelist.FocusedNode != null && activelist.FocusedNode.HasChildren);

            if (Data._clipObject != null)
            {
                var superTip = new SuperToolTip();
                superTip.Items.Add(Data._clipObject.ToString());
                barButtonItemPaste.SuperTip = superTip;
            }
            else
            {
                barButtonItemPaste.SuperTip = null;
            }
        }

        //private void TreeListOnNodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        //{
        //    var style = e.Node["风格"];
        //    if (style != DBNull.Value && style != null)
        //    {
        //        var doc = XDocument.Parse(style.ToString());
        //        if (doc.Root == null)
        //        {
        //            return;
        //        }

        //        var columnElement = doc.Root.Element(e.Column.FieldName);
        //        if (columnElement == null)
        //        {
        //            return;
        //        }

        //        var foreColorElement = columnElement.Element("ForeColor");
        //        if (foreColorElement != null && !string.IsNullOrEmpty(foreColorElement.Value))
        //        {
        //            e.Appearance.ForeColor = Color.FromArgb(int.Parse(foreColorElement.Value));
        //        }

        //        var backColorElement = columnElement.Element("BackColor");
        //        if (backColorElement != null && !string.IsNullOrEmpty(backColorElement.Value))
        //        {
        //            e.Appearance.BackColor = Color.FromArgb(int.Parse(backColorElement.Value));
        //        }
        //    }

        //    if (activelist != treeListBase && activelist != treeListEdit)
        //        return;



        //    if (e.Node == activelist.FocusedNode && e.Column == activelist.FocusedColumn)
        //    {
        //        barEditItemForeColor.EditValueChanged -= BarEditItemForeColorOnEditValueChanged;
        //        barEditItemBackColor.EditValueChanged -= BarEditItemBackColorOnEditValueChanged;

        //        barEditItemForeColor.EditValue = e.Appearance.ForeColor;
        //        barEditItemBackColor.EditValue = e.Appearance.BackColor;

        //        barEditItemForeColor.EditValueChanged += BarEditItemForeColorOnEditValueChanged;
        //        barEditItemBackColor.EditValueChanged += BarEditItemBackColorOnEditValueChanged;
        //    }
        //}

        private void BarEditItemBackColorOnEditValueChanged(object sender, EventArgs eventArgs)
        {
            if (activelist != treeListBase && activelist != treeListEdit)
                return;

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            WriteCellColor();
            activelist.InvalidateCell(activelist.FocusedNode, activelist.FocusedColumn);
        }

        private void BarEditItemForeColorOnEditValueChanged(object sender, EventArgs eventArgs)
        {
            if (activelist != treeListBase && activelist != treeListEdit)
                return;

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            WriteCellColor();
            activelist.InvalidateCell(activelist.FocusedNode, activelist.FocusedColumn);
        }

        private void WriteCellColor()
        {
            if (activelist != treeListBase && activelist != treeListEdit)
                return;

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }


            XDocument doc = null;
            var style = activelist.FocusedNode["风格"];
            if (style != DBNull.Value)
            {
                doc = XDocument.Parse(style.ToString());
            }

            if (doc == null)
            {
                doc = new XDocument(new XElement("Style"));
            }

            if (doc.Root != null)
            {
                var columnElement = doc.Root.Element(activelist.FocusedColumn.FieldName);
                if (columnElement == null)
                {
                    columnElement = new XElement(activelist.FocusedColumn.FieldName);
                    doc.Root.Add(columnElement);
                }

                if (barEditItemForeColor.EditValue != null)
                {
                    columnElement.SetElementValue("ForeColor", ((Color)barEditItemForeColor.EditValue).ToArgb());
                }

                if (barEditItemBackColor.EditValue != null)
                {
                    columnElement.SetElementValue("BackColor", ((Color)barEditItemBackColor.EditValue).ToArgb());
                }
            }

            activelist.FocusedNode["风格"] = doc.ToString();
        }

        private void TreeListOnFocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            var treeList = sender as TreeList;
            if (treeList == null)
            {
                return;
            }

            ReadCellColor(treeList);
        }

        private void BarButtonItemCopyOnItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            Data._clipObject = activelist.FocusedNode[activelist.FocusedColumn];
        }

        private void On粘贴ItemClick(object sender, ItemClickEventArgs e)
        {

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            try
            {
                //activelist.FocusedNode[activelist.FocusedColumn] = Data._clipObject;
                模式 row = GetRowByNode(activelist, activelist.FocusedNode);
                typeof(模式).GetProperty(activelist.FocusedColumn.FieldName).SetValue(row, Data._clipObject);
                Data.FindTableByID(row.ID).更新对象(row);
                刷新当前模式行(row.ID);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BarButtonItemCopyRowIdOnItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            Data._clipObject = activelist.FocusedNode["ID"];
        }

        private void 刷新子级一级序号(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            ReorderSn(activelist, false, (Guid)activelist.FocusedNode["ID"]);
        }

        private 生长对象 Find节点对象(Guid id)
        {
            Processor p = Processor.当前处理器;
            foreach (生长对象 o in p.全部对象)
            {
                if (o.ID.Equals(id))
                    return o;
            }
            return null;

        }

        private void TreeListOnMouseDown(object sender, MouseEventArgs e)
        {
            var treeList = sender as TreeList;

            if (treeList == null)
            {
                return;
            }


            activelist = treeList;

            //this.Text = string.Format("子记录数：{0}",GetAllNodeCount(treeList.FocusedNode));

            TreeListHitInfo hitInfo = treeList.CalcHitInfo(e.Location);

            Guid id = Guid.Empty;

            if (e.Button == MouseButtons.Right)
            {
                //if (hitInfo.HitInfoType == HitInfoType.Cell || hitInfo.HitInfoType == HitInfoType.Row)
                {
                    treeList.Focus();

                    treeList.FocusedNode = hitInfo.Node;

                    treeList.FocusedColumn = hitInfo.Column;

                }
            }

            if (treeList == treeListBase)
            {
                if (hitInfo.Node != null)
                {
                    Guid id1 = (Guid)hitInfo.Node["ID"];

                    for (int i = 0; i < selectrowid.Count(); i++)
                        if (id1.Equals(selectrowid[i]))
                            selectrowid.RemoveAt(i--);

                    selectrowid.Add(id1);
                    index = selectrowid.Count() - 1;
                }

            }

            refnode = null;

            if (hitInfo.Node != null)
            {
                if (activelist == treeListEdit || activelist == treeListBase)
                {
                    if (hitInfo.Column != null && Data.IsObjectField(hitInfo.Column.FieldName, false))
                        id = (Guid)hitInfo.Node[hitInfo.Column];
                }
                else if (activelist == treeListParam || activelist == treeliststring || activelist == treeListresult)
                {
                    id = (Guid)hitInfo.Node["ObjectID"];
                    if (activelist == treeListresult)
                    {
                        TreeListNode n = FindNodeByID(id);
                        if (n != null)
                        {
                            n.Selected = true;
                        }

                        左对象 = Guid.Empty;
                        右对象 = Guid.Empty;
                        生长对象 obj = Find节点对象((Guid)hitInfo.Node["ID"]);
                        if (obj != null)
                        {
                            if (obj.左对象 != null)
                                左对象 = obj.左对象.ID;
                            if (obj.右对象 != null)
                                右对象 = obj.右对象.ID;
                            EditString.EditValue = obj.生成所有参数的串();
                        }
                        treeListresult.Refresh();
                    }

                }


                if (!id.Equals(Guid.Empty))
                {
                    refnode = FindNodeByID(id);
                    GuidEdit.EditValue = id;
                }
                else
                    GuidEdit.EditValue = hitInfo.Node["ID"];

            }


            treeListBase.Refresh();
            treeListEdit.Refresh();



            if (e.Button == MouseButtons.Right)
                popupMenu1.ShowPopup(MousePosition);



        }

        private void BarButtonItemRefreshRootSnOnItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ReorderSn(TreeList treeList, bool 递归, Guid parentId)
        {
            List<模式> 对象集合 = 根据表获取模式集合(treeList).ToList();

            using (new UseWaitCursor())
            {

                treeList.BeginSort();

                递归排序(对象集合, 递归, parentId);

                treeList.EndSort();
            }
        }

        private IList<模式> 根据表获取模式集合(TreeList treeList)
        {
            if (treeList == treeListBase)
                return Data.模式表.对象集合;

            if (treeList == treeListEdit)
                return Data.模式编辑表.对象集合;

            return new List<模式>();
        }


        private void 递归排序(List<模式> 对象集合, bool 递归, Guid parentId)
        {
            进行一个层级的排序(对象集合, parentId);
            if (递归 && !parentId.Equals(Data.NullParentGuid))
            {
                List<模式> rows = 对象集合.Where(r => r.ParentID == parentId).ToList();

                foreach (模式 n in rows)
                {
                    递归排序(对象集合, 递归, (Guid)n.ID);
                }
            }
        }

        private void 进行一个层级的排序(List<模式> 对象集合, Guid parentId)
        {

            IEnumerable<模式> objects;
            //if (parentId != null)
            //{

            //负数和大于1000000的不参与排序。
            objects = 对象集合.Where(r => r.ParentID == parentId && r.序号 >= 0 && r.序号 < 1000000).OrderBy(r => r.序号).ToList();

            int i = 1;
            var plusObjects = objects.Where(r => (int)r.序号 >= 0).OrderBy(r => (int)r.序号);
            var lastOriginSn = 0;
            var lastSn = 0;
            foreach (var plusObject in plusObjects)
            {
                if ((int)plusObject.序号 != 0)
                {
                    if ((int)plusObject.序号 == lastOriginSn)
                    {
                        plusObject.序号 = lastSn;
                    }
                    else
                    {
                        lastOriginSn = (int)plusObject.序号;
                        plusObject.序号 = i * 100;
                        i++;
                    }

                    lastSn = (int)plusObject.序号;
                }

                Data.FindTableByID(plusObject.ID).更新对象(plusObject);
            }

            var minusObjects = objects.Where(r => (int)r.序号 < 0).OrderBy(r => (int)r.序号);
            var count = minusObjects.Count();
            lastSn = 0;
            lastOriginSn = 0;
            foreach (var minusObject in minusObjects)
            {
                if ((int)minusObject.序号 == lastOriginSn)
                {
                    minusObject.序号 = lastSn;
                }
                else
                {
                    lastOriginSn = (int)minusObject.序号;
                    minusObject.序号 = -count * 100;
                    count--;
                }

                lastSn = (int)minusObject.序号;

                Data.FindTableByID(minusObject.ID).更新对象(minusObject);
            }

        }

        private void TreeListOnFocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {

            var treeList = sender as TreeList;
            if (treeList == null)
            {
                return;
            }

            if (treeList == treeListBase || treeList == treeListEdit)
            {
                if (e.Node != null)
                {
                    string str = (string)e.Node["形式"];
                    TheString s = new TheString(str);
                    if (s.有嵌入串)
                        str = s.嵌入串;
                    else
                    {
                        s.清理嵌入串();
                        str = s.ToString();
                        str = str.Replace("[", "");
                        str = str.Replace("]", "");
                    }
                    str = str.Replace("\"", "");
                    EditString.EditValue = str;
                }
            }


            if (treeList.Tag == null)
            {
                treeList.FocusedNode = treeList.Nodes.FirstNode;
                treeList.CollapseAll();

                treeList.Tag = true;
            }

            ReadCellColor(treeList);
        }

        private void ReadCellColor(TreeList treeList)
        {
            if (treeList.FocusedNode == null || treeList.FocusedNode.Focused == false)
            {
                return;
            }

            treeList.InvalidateCell(treeList.FocusedNode, treeList.FocusedColumn);
        }

        private PatternDataSet _patternDataSet;
        public PatternDataSet PatternDataSet
        {
            get { return _patternDataSet; }
            set
            {
                _patternDataSet = value;

                bindingSourceBase.DataSource = new BindingList<模式>(Data.模式表.对象集合.Where(r=>r.Bounding).ToList());
                bindingSourceEdit.DataSource = Data.模式编辑表.对象集合;

                bindingSourceParam.DataSource = _patternDataSet;
                bindingSourceString.DataSource = _patternDataSet;
                bindingSource模式结果.DataSource = _patternDataSet;

                bindingSource字典_语言.DataSource = _patternDataSet;
                bindingSource字典_限定目标.DataSource = _patternDataSet;
                bindingSource字典_语言角色.DataSource = _patternDataSet;
                bindingSource字典_隐藏形式.DataSource = _patternDataSet;
                bindingSource字典_的.DataSource = _patternDataSet;
            }
        }

        private void 展开全部_ItemClick(object sender, ItemClickEventArgs e)
        {

            activelist.ExpandAll();
        }

        private void 收缩全部_ItemClick(object sender, ItemClickEventArgs e)
        {
            activelist.CollapseAll();
        }

        private void barButtonItemSetNull_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }


            try
            {
                模式 row = GetRowByNode(activelist, activelist.FocusedNode);
                typeof(模式).GetProperty(activelist.FocusedColumn.FieldName).SetValue(row, Data.NullGuid);
                Data.FindTableByID(row.ID).更新对象(row);
                刷新当前模式行(row.ID);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void 模式Form_Load(object sender, EventArgs e)
        {
            // TODO: 这行代码将数据加载到表“patternDataSet.字典_语言角色”中。您可以根据需要移动或删除它。
            //this.字典_语言角色TableAdapter.Fill(this.patternDataSet.字典_语言角色);

        }

        private void 粘贴为父记录_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;

            TreeListNode node = activelist.FocusedNode;
            Guid id = (Guid)node["ID"];

            模式 row = GetRowByNode(activelist, activelist.FocusedNode);

            if (row != null && Data._clipObject is Guid)
            {
                //TreeListNode parent = node.ParentNode;
                //node["ParentID"] = Data._clipObject;
                Guid parentid = (Guid)Data._clipObject;
                模式 rowparent = Data.FindRowByID(parentid);
                if (rowparent == null)
                    return;
                row.ParentID = parentid;


                Data.FindTableByID(row.ID).更新对象(row);
                刷新当前模式行(row.ID);
            }
        }

        private void 设置父记录为空_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;

            模式 row = GetRowByNode(activelist, activelist.FocusedNode);

            row.ParentID = Data.NullParentGuid;

            Data.FindTableByID(row.ID).更新对象(row);

            刷新当前模式行(row.ID);
        }

        private void 粘贴行到后_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.Nodes.Count == 0)
            {
                粘贴所有行(false, 1, null);
                return;
            }

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;


            粘贴所有行(false, 1, activelist.FocusedNode);

        }

        private void 粘贴所有行(bool IsChild, int order, TreeListNode parentNode)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;

            模式 row = Data.FindRowByID((Guid)activelist.FocusedNode["ID"]);

            using (new UseWaitCursor())
            {
                //var stopWatch = new Stopwatch();
                //stopWatch.Start();
                //activelist.BeginUpdate();
                //activelist.BeginSort();
                //activelist.LockReloadNodes();

                List<模式> rows = Data._clipObject as List<模式>;
                row = Data.PasteRows(rows, IsChild, order, row);

                //activelist.UnlockReloadNodes();
                //activelist.EndSort();
                //activelist.EndUpdate();

                //stopWatch.Stop();
                //Console.WriteLine("粘贴到前耗时" + stopWatch.ElapsedMilliseconds);
            }

            if (row != null)
            {
                //TreeListNode node = activelist.FindNodeByKeyID(row.ID);
                //node.Expanded = false;
                //node.Selected = true;

                定位选择模式行(row.ID, true, false, false);
            }

            if (parentNode != null)
            {
                parentNode = activelist.FindNodeByKeyID(parentNode["ID"]);
                if (parentNode != null)
                    parentNode.Expanded = true;
            }
        }

        private void 粘贴行到前_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.Nodes.Count == 0)
            {
                粘贴所有行(false, -1, null);
                return;
            }

            粘贴所有行(false, -1, activelist.FocusedNode);
        }

        private void BarButtonItem粘贴行到子级OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.Nodes.Count == 0)
            {
                粘贴所有行(false, 0, null);
                return;
            }

            粘贴所有行(true, 0, activelist.FocusedNode);
        }

        //private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    treeListBase.ShowFindPanel();
        //    treeListBase.ExpandAll();
        //}

        public void EndEdit()
        {
            PatternDataSet.模式查找.Clear();
            PatternDataSet.形式化语料串.Clear();
            PatternDataSet.模式结果.Clear();

            Data.模式表.CommitChanges();
            Data.模式编辑表.CommitChanges();
        }


        private TreeListNode FindNodeByID(Guid id)
        {
            if (id.Equals(Guid.Empty))
                return null;
            TreeListNode n = treeListEdit.FindNodeByKeyID(id);
            if (n == null)
                n = treeListBase.FindNodeByKeyID(id);
            return n;
        }

        private BindingSource FindBindingSourceByID(Guid id)
        {
            if (Data.模式表.Exist(id))
                return bindingSourceBase;
            if (Data.模式编辑表.Exist(id))
                return bindingSourceEdit;

            return null;
        }

        private void 定位ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            var a = activelist.FocusedNode[activelist.FocusedColumn];
            if ((a is Guid) == false)
            {
                if (activelist == treeListEdit || activelist == treeListBase)
                    a = (Guid)activelist.FocusedNode["ID"];
                else
                    a = (Guid)activelist.FocusedNode["ObjectID"];
            }

            选择文本.EditValue = "";
            //treeListBase.FilterNodes();

            // 先从模式编辑表中找
            var id = (Guid)a;
            var n = treeListEdit.FindNodeByKeyID(id);
            if (n != null)
            {
                n.Selected = true;
                n.TreeList.MakeNodeVisible(n);
            }
            else
            {
                Data.设置模式树一般可见性(id, true, false);
                定位选择模式行(id, true, true, false);
            }
        }

        private void MoveSelect_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_markRowID != Guid.Empty)
            {
                TreeListNode n = FindNodeByID(_markRowID);
                if (n != null)
                {
                    n.Selected = true;
                }
            }
        }


        private void 定位上一条(object sender, ItemClickEventArgs e)
        {
            选择文本.EditValue = "";
            //treeListBase.FilterNodes();
            //RefreshTreeListBaseDataSource();
            if (index == 0)
                return;
            index--;
            //TreeListNode n = treeListBase.FindNodeByKeyID(selectrowid[index]);
            //if (n != null)
            //    n.Selected = true;
            定位选择模式行(selectrowid[index], true, false, false);
        }

        private void 定位下一条(object sender, ItemClickEventArgs e)
        {
            选择文本.EditValue = "";
            //treeListBase.FilterNodes();
            //RefreshTreeListBaseDataSource();
            if (index >= selectrowid.Count() - 1)
                return;
            index++;

            //TreeListNode n = treeListBase.FindNodeByKeyID(selectrowid[index]);
            //if (n != null)
            //    n.Selected = true;
            定位选择模式行(selectrowid[index], true, false, false);
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            Data.对话frm.Show();
            Data.对话frm.显示句子测试frm();
            Data.对话frm.设置焦点();   
        }


        //刷所有的根模式
        private void 刷根_ItemClick(object sender, ItemClickEventArgs e)
        {

            foreach (模式 r2 in Data.模式表.对象集合)
            {
                Data.刷一条记录的根(r2);
            }

            foreach (模式 r2 in Data.模式编辑表.对象集合)
            {
                Data.刷一条记录的根(r2);
            }

        }


        private void 刷新顶级序号_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (new UseWaitCursor())
            {
                ReorderSn(activelist, false, Data.NullParentGuid);
            }
        }

        private void 删除下级节点_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;
            Guid id = (Guid)activelist.FocusedNode["ID"];

            模式基表 目标表 = Data.FindTableByID(id);
            if (目标表 != null)
            {
                var containInvisible = false;
                递归检查是否包含未显示子节点(目标表, activelist.FocusedNode, ref containInvisible);
                if (containInvisible)
                {
                    var result = XtraMessageBox.Show(this, "该节点包含隐藏字节点，继续删除？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                activelist.BeginUpdate();
                activelist.BeginSort();
                activelist.LockReloadNodes();

                Data.递归删除记录树(目标表, id, false);
                if (目标表 == Data.模式表)
                {
                    RefreshTreeListBaseDataSource();
                }

                activelist.UnlockReloadNodes();
                activelist.EndSort();
                activelist.EndUpdate();
            }

            刷新当前模式行(id);
        }

        private void 删除节点_ItemClick(object sender, ItemClickEventArgs e)
        {
            TreeListNode activenode = activelist.FocusedNode;

            if (activenode == null)
                return;

            TreeListNode node = (activenode.PrevNode ?? activenode.ParentNode) ?? activenode.NextNode;
            Guid? id = node == null ? (Guid?)null : (Guid)node["ID"];


            var targetId = (Guid)activenode["ID"];

            模式基表 目标表 = Data.FindTableByID(targetId);
            if (目标表 != null)
            {
                if (目标表 == Data.模式表)
                {
                    var containInvisible = false;                    
                    递归检查是否包含未显示子节点(目标表, activenode, ref containInvisible);

                    if (containInvisible)
                    {
                        var result = XtraMessageBox.Show(this, "该节点包含隐藏字节点，继续删除？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }
                    }
                }
                

                activelist.BeginUpdate();
                activelist.BeginSort();
                activelist.LockReloadNodes();

                Data.递归删除记录树(目标表, targetId, true);
                if (目标表 == Data.模式表)
                {
                    RefreshTreeListBaseDataSource();
                }

                activelist.UnlockReloadNodes();
                activelist.EndSort();
                activelist.EndUpdate();
            }  

            if (id.HasValue)
                刷新当前模式行(id.Value, false);
        }

        private void 递归检查是否包含未显示子节点(模式基表 目标表, TreeListNode activenode, ref bool containInvisible)
        {
            if (containInvisible || 目标表 != Data.模式表)
            {
                return;
            }
            else
            {
                var id = (Guid)activenode["ID"];
                var subItems = 目标表.对象集合.Where(r => r.ParentID == id);
                if (subItems.Count() != activenode.Nodes.Count)
                {
                    containInvisible = true;
                }

                if (!containInvisible)
                {
                    foreach (TreeListNode item in activenode.Nodes)
                    {
                        if (containInvisible)
                            break;
                        递归检查是否包含未显示子节点(目标表, item, ref containInvisible);
                    }
                }
            }
        }

        private int GetAllNodeCount(TreeListNode node)
        {
            int c = node.Nodes.Count;
            foreach (TreeListNode childnode in node.Nodes)
            {
                c += GetAllNodeCount(childnode);
            }
            return c;
        }

        public void 新建会话()
        {
            模式 row = Data.New派生行(Data.会话组织Guid);
            row.形式 = "[会话组织]";

            row.序号 = 0;
            if (treeListEdit.Nodes.LastNode != null)
                row.序号 = (int)treeListEdit.Nodes.LastNode["序号"] + 100;

            Data.模式编辑表.新加对象(row);

            MarkRowColumn(row.ID, null);
            activelist = treeListEdit;
            treeListEdit.Focus();
            Data.当前会话 = row.ID;
        }


        public 模式 执行添加句子(string str)
        {
            模式 row = Data.New派生行(Data.属于Guid);

            //str = str.Replace("'", "[']");
            str = str.Replace("\"", "[\"]");

            row.B端 = Data.单句分析Guid;
            row.形式 = "[" + str + "]" + "\"" + str + "\"";
            int 序号 = 0;
            TreeListNode node = treeListEdit.FindNodeByKeyID(Data.当前会话);
            if (node != null)
            {
                row.ParentID = (Guid)node["ID"];
                if (node.Nodes.LastNode != null)
                    序号 = (int)node.Nodes.LastNode["序号"] + 100;
            }
            else if (treeListEdit.Nodes.LastNode != null)
                序号 = (int)treeListEdit.Nodes.LastNode["序号"] + 100;

            row.序号 = (序号 / 100) * 100;

            Data.模式编辑表.新加对象(row);

            MarkRowColumn(row.ID, null);

            activelist = treeListEdit;
            treeListEdit.FocusedNode = FindNodeByID(row.ID);
            treeListEdit.BeginUpdate();
            treeListEdit.BeginSort();
            treeListEdit.LockReloadNodes();

            全部执行得到结果(row.ID);

            treeListEdit.UnlockReloadNodes();
            treeListEdit.EndSort();
            treeListEdit.EndUpdate();
            return row;
        }

        public void 添加句子_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (EditString == null)
                return;
            string str = (string)EditString.EditValue;

            执行添加句子(str);

        }

        public void 删除下级节点(Guid id)
        {
            模式基表 目标表 = Data.FindTableByID(id);
            Data.递归删除记录树(目标表, id, false);
            if (目标表 == Data.模式表)
            {
                RefreshTreeListBaseDataSource();
            }
        }

        public void 定位选择模式行(Guid id, bool 是否选择 = true, bool 是否展开 = true, bool 是否全部展开 = true)
        {
            var bindSource = FindBindingSourceByID(id);
            if (bindSource == null)
                return;
            ResetPatternBindingSource(bindSource);

            TreeListNode node = FindNodeByID(id);
            if (node == null)
                return;
            else
                node.Expanded = false;

            if (是否选择)
                node.Selected = true;

            if (是否展开)
                node.Expanded = true;

            if (是否全部展开)
                node.ExpandAll();
            node.TreeList.FocusedNode = node;
            node.TreeList.MakeNodeVisible(node);
        }

        private void ResetPatternBindingSource(BindingSource srcBindingSource)
        {
            if (srcBindingSource == bindingSourceBase)
            {
                RefreshTreeListBaseDataSource();             
            }

            srcBindingSource.ResetBindings(false);
        }


        public void 刷新当前模式行(Guid id, bool isExpanded = true)
        {
            var bindSource = FindBindingSourceByID(id);
            if (bindSource == null)
                return;
            TreeListNode node = FindNodeByID(id);
            bindSource.ResetItem(node.Id);

            node = FindNodeByID(id);
            node.TreeList.FocusedNode = node;
            node.Selected = true;

            if (isExpanded)
                node.Expanded = true;

            node.TreeList.MakeNodeVisible(node);
        }

        public void 全部执行得到结果(Guid id)
        {
            if (解析前准备(true) == false)
                return;


            using (new UseWaitCursor())
            {
				treeListresult.BeginUpdate();
                treeListresult.LockReloadNodes();
                删除下级节点(id);
                
                Processor p = Processor.当前处理器;
                Data.timeCount = 0;
                treeList待生长对象对.DataSource = p.待生长对象对集合;
                treeList待生长对象对.BeginUpdate();
                treeList待生长对象对.LockReloadNodes();

                long t = System.DateTime.Now.Ticks;
                Data.分解串并生成串对象(id);
                Data.进行生长并得到所有中间结果();
                Data.选择最优结果生成(生成树时删除空行.Checked);
                Data.递归设置语境树知识有效性(Data.当前句子Row, -1);
                t = System.DateTime.Now.Ticks - t;

                treeListresult.UnlockReloadNodes();
                treeListresult.EndUpdate();
                treeList待生长对象对.UnlockReloadNodes();
                treeList待生长对象对.EndUpdate();

				
				double r=t/10000000.0;
                double z = Data.timeCount / 10000000.0;
				Data.显示信息(r.ToString()+"\t"+z.ToString());
            }

            tabControl1.SelectedIndex = 2;
            treeListresult.ExpandAll();
            if (展开.Checked == false)
                展开.Checked = true;
        }


        //如果是追加参数，
        //如果不是追加参数，那么就是完全自由的子树。不建立和父的联系。
        private void 递归处理一组选择参数(TreeObject 树结构, TreeListNodes nodes, 模式 ParentRow, string 挂接到父字段)
        {

            foreach (TreeListNode node in nodes)
            {
                if (node.Checked)//这个节点是被选择了的。
                {
                    Guid id = (Guid)node.GetValue("ObjectID");

                    DataRowView v = (DataRowView)node.TreeList.GetDataRecordByNode(node);
                    string str1 = v.Row["标记"] as string;

                    模式 row = null;

                    //做了特殊标记的处理。
                    //注意：对于这个，后边可能要修改，让【源记录】有更合适的记录而不是为空。
                    //if (str1 == "属于")
                    //{
                    //    //    row = Data.New属于行(ParentRow, id, table.TableName);
                    //    row = Data.New属于行(id, table);
                    //}
                    //else
                    {
                        int that = Data.FindThatValue((string)node.GetValue("根"));
                        row = Data.New派生行(id, that);
                    }

                    if (ParentRow == null)//是一棵新的树
                        树结构.现有数据创建为新树();
                    else//在已有树上增加记录
                        Data.挂接记录(ParentRow, row, null);

                    树结构.AddRow(row);

                    //row["序号"] = 全局序号;
                    //全局序号 += 100;

                    if (替代.可正向替代(Data.一级关联类型(row)) && ParentRow != null)//对于属于，那么把后边的参数提到上一级
                        //					if (Data.是分类(Data.根模式(row)) && (Data.ThisGuid.Equals(row["A端"])==false))//对于属于，那么把后边的参数提到上一级
                        递归处理一组选择参数(树结构, node.Nodes, ParentRow, null);
                    else//这里应该是【拥有】
                        递归处理一组选择参数(树结构, node.Nodes, row, null);//对自身的根肯定是追加参数
                }
                else//这个节点没有被选择，把内部参数向外连接。
                {
                    递归处理一组选择参数(树结构, node.Nodes, ParentRow, 挂接到父字段);
                }

            }
            return;

        }


        private void ProcessAllCheckNode(TreeListNode node, List<Guid> guids)
        {
            if (node.Checked)
                guids.Add((Guid)node["ObjectID"]);

            foreach (TreeListNode n in node.Nodes)
                ProcessAllCheckNode(n, guids);
        }


        private 模式 加入派生记录(TreeObject 树结构, Guid newobject, int that)
        {


            模式 row = Data.New派生行(newobject, that);

            row.形式 = "[]";
            //		int that= Data.FindThatValue((string)node.GetValue("根"));
            //         row = Data.New派生行(newobject, tablename, that);

            树结构.AddRow(row);

            //row["序号"] = 全局序号;
            //全局序号 += 100;

            //自由记录
            {

                //if (基连接 == Data.属于Guid)//属于
                //{
                //	row["连接"] = 基连接;//仍然是用属于。
                //	row["B端"] = baserowid;//基类进行置换
                //	row["形式"] = "◀" + baserow["形式"];
                //}
                //else if (基连接 == Data.拥有Guid)//拥有
                //{
                //	row["连接"] = baserowid;//用【拥有】的派生。
                //	row["B端"] = baserow["B端"];
                //	row["形式"] = "▷" + baserow["形式"];
                //}
            }

            return row;

        }

        // 新建增加子树。并挂接到父记录或者父记录的字段。不挂接的话，就是完全的自由子树，和父记录只有parent关联
        private 模式 加入子树(模式 parentrow, bool 进行挂接, string 挂接到父字段, int that)
        {
            if (activelist == null)
                return null;

            //暂时先这样。
            if (that == 0)
                that = 字典_目标限定.A端;

            TreeObject 树结构 = new TreeObject();

            //全局序号 = 100;

            if (activelist == treeListParam)
            {
                //这个的that自己进行处理

                //树结构.basethatid = (Guid)activelist.FocusedNode.GetValue("ObjectID");
                //树结构.thatcol = activelist.FocusedNode.GetValue("根") as string;

                递归处理一组选择参数(树结构, treeListParam.Nodes, null, null);
                //这里不挂接到外部，执行完以后才做。
            }
            else
            {
                if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                    return null;

                //树结构.basethatid = (Guid)activelist.FocusedNode.GetValue("ID");
                //树结构.thatcol = (string)activelist.FocusedColumn.FieldName;


                加入派生记录(树结构, (Guid)activelist.FocusedNode["ID"], that);

            }

            树结构.RecalcAllRoot();

            if (树结构.树集合 != null && 树结构.树集合.Count > 0)
            {
                foreach (TreeObject obj in 树结构.树集合)
                    加入新树到目标位置(obj, parentrow, 进行挂接, 挂接到父字段);
            }

            加入新树到目标位置(树结构, parentrow, 进行挂接, 挂接到父字段);

            return 树结构.组织Parent根;
            //处理改变(parentrow, 1);

        }


        private void 加入新树到目标位置(TreeObject 树结构, 模式 parentrow, bool 进行挂接, string 挂接到父字段)
        {
            if (进行挂接)//和原始根进行挂接
            {
                Data.挂接记录(parentrow, 树结构.语义根, 挂接到父字段);
            }
            else
            {//自由记录
                树结构.语义根.ParentID = parentrow.ID;
            }

            foreach (var data in 树结构.rows)
            {
                Data.FindTableByID(parentrow.ID).新加对象(data.Value);
            }
        }

        private void 加入自由子树_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (_markRowID == Guid.Empty)
            {
                XtraMessageBox.Show("请先标记行。");
                return;
            }
            模式 parentrow = Data.FindRowByID(_markRowID);

            if (parentrow == null)
            {
                XtraMessageBox.Show("标记行已经失效。");
                return;
            }


            模式 派生对象 = 加入子树(parentrow, false, _markCol, 0);

            //模式 派生子树 = Data.FindTableByID(parentrow.ID).对象集合.FirstOrDefault(r => r.ParentID == parentrow.ID);
            Guid id = 派生对象 == null ? parentrow.ID : 派生对象.ID;
            定位选择模式行(id);
            //ReorderSn(_markTreeList, true, (Guid)parentrow["ID"]);
        }


        private void 派生一个新对象_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            if (activelist == null || activelist.FocusedNode == null)
                return;

            模式 parentrow = GetRowByNode(activelist, activelist.FocusedNode);
            string 挂接到父字段 = null;

            if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                挂接到父字段 = activelist.FocusedColumn.FieldName;

            模式 派生对象 = 加入子树(parentrow, false, 挂接到父字段, 0);
            //ReorderSn(_markTreeList, true, (Guid)parentrow["ID"]);
            //模式 派生对象 = Data.FindTableByID(parentrow.ID).对象集合.FirstOrDefault(r => r.ParentID == parentrow.ID);
            Guid id = 派生对象 == null ? parentrow.ID : 派生对象.ID;

            定位选择模式行(id);
        }


        private void 加入扩展子树_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_markRowID == Guid.Empty)
                return;
            模式 parentrow = Data.FindRowByID(_markRowID);

            if (parentrow == null)
                return;

            加入子树(parentrow, true, _markCol, 0);


            //ReorderSn(_markTreeList, true, (Guid)parentrow["ID"]);
        }

        private void 列出参数()
        {
            if (activelist == treeListEdit || activelist == treeListBase)
            {
                if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                    return;

                Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
                string col = null;

                //if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                //	id = (Guid)activelist.FocusedNode[activelist.FocusedColumn];

                if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                    col = activelist.FocusedColumn.FieldName;//用户指定了特定端
                else
                    col = "ID";


                加入参数(id, col);
                tabControl1.SelectedIndex = 0;

                if (展开.Checked == false)
                    展开.Checked = true;

            }
        }


        private void 选择参数_ItemClick(object sender, ItemClickEventArgs e)
        {
            列出参数();
        }

        private void 标记_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == treeListEdit || activelist == treeListBase)
                MarkRowColumn(activelist);
        }



        private void 向上_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;
            int v = (int)activelist.FocusedNode["序号"];
            TreeListNode pn = activelist.FocusedNode.PrevNode;
            if (pn != null)
            {
                int v2 = (int)pn["序号"];
                if (v2 < 0 && v >= 0)
                    v = -1;
                else
                    v = v2 - 1;
            }
            else if (v >= 0)
                v = -1;//移动到0的前边。

            activelist.FocusedNode["序号"] = v;
            activelist.EndCurrentEdit();

            //if (activelist.FocusedNode.ParentNode == null)
            //	ReorderSn(activelist, false, Data.NullParentGuid);
            //else
            //	ReorderSn(activelist, false, (Guid)activelist.FocusedNode.ParentNode["ID"]);
        }

        private void 向下_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;
            int v = (int)activelist.FocusedNode["序号"];
            TreeListNode pn = activelist.FocusedNode.NextNode;
            if (pn != null)
            {
                int v2 = (int)pn["序号"];
                if (v2 > 0 && v <= 0)
                    v = 1;
                else
                    v = v2 + 1;
            }
            else if (v <= 0)
                v = 1;//移动到0的前边。

            activelist.FocusedNode["序号"] = v;
            activelist.EndCurrentEdit();

            //if (activelist.FocusedNode.ParentNode == null)
            //	ReorderSn(activelist, false, Data.NullParentGuid);
            //else
            //	ReorderSn(activelist, false, (Guid)activelist.FocusedNode.ParentNode["ID"]);
        }

        private void 自动序号_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void 递归刷新子级序号_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }

            ReorderSn(activelist, true, (Guid)activelist.FocusedNode["ID"]);
        }

        private void 把子树挂接到标记字段_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            {
                return;
            }
            if (activelist != _markTreeList)
            {
                XtraMessageBox.Show("要求挂接子树和标记字段必须在一张表。");
                return;
            }
            if (_markCol == null)
            {
                XtraMessageBox.Show("挂接子树必须挂接到指定一个字段而不是行。");
                return;
            }

            模式 row = Data.FindRowByID(_markRowID);

            设置ID引用(row, _markCol, false);

            //处理改变(row, 3);


        }

        private void 置行ID到标记字段_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_markCol == null)
            {
                XtraMessageBox.Show("没有标记字段。");
                return;
            }

            模式 row = Data.FindRowByID(_markRowID);

            设置ID引用(row, _markCol);

            //处理改变(row, 3);

        }

        private void 移动到记录(TreeListNode node, int order = 1)
        {
            TreeList treelist = node.TreeList;

            TreeListNode n = activelist.FocusedNode;
            if (node == null)
                return;

            //DataRow parentrow = (treelist.GetDataRecordByNode(node) as DataRowView).Row;
            if (n == node)
                return;

            if (order == 1 && n == node.NextNode)
                return;

            if (order == -1 && n == node.PrevNode)
                return;

            模式 nRow = Data.FindRowByID((Guid)n["ID"]);
            模式 nodeRow = Data.FindRowByID((Guid)node["ID"]);

            if (n.ParentNode != node.ParentNode)
            {
                if (node.ParentNode == null)
                    nRow.ParentID = Data.NullParentGuid;
                else
                    nRow.ParentID = nodeRow.ParentID;
            }
            nRow.序号 = nodeRow.序号 + order;
            Data.FindTableByID(nRow.ID).更新对象(nRow);

            刷新当前模式行(nRow.ID);
        }

        private void 设置ID引用(模式 Row, string fieldname, bool NoMove = true)
        {
            TreeListNode node = activelist.FocusedNode;
            if (node == null)
                return;

            Guid id = (Guid)node["ID"];

            if (activelist == treeListParam)
                id = (Guid)node["ObjectID"];


            if (id.Equals((Guid)typeof(模式).GetProperty(fieldname).GetValue(Row)) == false && id.Equals(Row.ID) == false)
            {
                typeof(模式).GetProperty(fieldname).SetValue(Row, id);
                Data.FindTableByID(Row.ID).更新对象(Row);
                刷新当前模式行(id);
            }

        }

        private void 展开节点全部_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode != null)
            {
                if (activelist == treeListBase)
                {
                    var id = (Guid)activelist.FocusedNode.GetValue("ID");
                    var 模式根 = Data.FindRowByID(id);
                    模式根.Bounding = true;
                    Data.设置模式子树全部可见性(模式根, true);

                    选择文本.EditValue = string.Empty;
                    RefreshTreeListBaseDataSource();

                    var focusedNode = treeListBase.FindNodeByKeyID(id);
                    treeListBase.FocusedNode = focusedNode;              
                }

                activelist.FocusedNode.ExpandAll();         
            }                
        }

        private void treeList_DragDrop(object sender, DragEventArgs e)
        {
            dragnode = null;
            dragcol = null;

            //取消系统的自动处理。
            DragDropEffects theeffect = e.Effect;
            e.Effect = DragDropEffects.None;

            if (theeffect == DragDropEffects.None)
                return;

            TreeList thetreelist = (TreeList)sender;
            Point p = new Point(e.X, e.Y);
            p = thetreelist.PointToClient(p);

            TreeListHitInfo hitInfo = thetreelist.CalcHitInfo(p);

            if (hitInfo.Node == null || hitInfo.Column == null)
                return;


            if (theeffect == DragDropEffects.Move)
            {
                if (hitInfo.MousePoint.Y < hitInfo.Bounds.Y + hitInfo.Bounds.Height / 2)
                    移动到记录(hitInfo.Node, -1);
                else
                    移动到记录(hitInfo.Node, 1);

                return;
            }

            模式 parentrow = Data.FindRowByID((thetreelist.GetDataRecordByNode(hitInfo.Node) as 模式).ID);

            //只设置连接引用，肯定是Guid字段
            if (theeffect == DragDropEffects.Link && Data.IsObjectField(hitInfo.Column.FieldName, true))
            {
                设置ID引用(parentrow, hitInfo.Column.FieldName);
                return;
            }


            bool 执行挂接 = (theeffect == DragDropEffects.All);
            //对于非Guid字段一种只是复制，一种是复制并和旧的对象链接

            string col = hitInfo.Column.FieldName;
            if (Data.IsObjectField(col, true) == false)
                col = null;

            加入子树(parentrow, 执行挂接, col, 0);

            //ReorderSn(thetreelist, true, (Guid)parentrow["ID"]);

            //DataRow row = null;
            //if (activelist == treeListParam)
            //{
            //	全局序号 = 1000000;

            //	row = 递归处理选择好的参数(treeListParam.Nodes, parentrow, 执行挂接);
            //}
            //else
            //{
            //	if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
            //		return;

            //	row = Data.加入派生记录(parentrow, (Guid)activelist.FocusedNode["ID"], 执行挂接);
            //}

            ////挂接到字段。
            //if (Data.IsObjectField(hitInfo.Column.FieldName, true) && row != null)
            //{
            //	parentrow[hitInfo.Column.FieldName] = row["ID"];
            //}

            //ReorderSn(thetreelist, true, (Guid)parentrow["ID"]);

            (this.ParentForm as MainForm).iSave.Enabled = true;
        }

        private void TreeListOnMouseUp(object sender, MouseEventArgs e)
        {
            if (dragnode != null)
            {
                dragnode.TreeList.RefreshNode(dragnode);
            }

            dragnode = null;
            dragcol = null;
        }


        //这个是在接收窗口和源窗口不同的时候发生。
        private void treeList_DragEnter(object sender, DragEventArgs e)
        {


            //e.Effect = DragDropEffects.None;
            //TreeList thetreelist = (TreeList)sender;


            //if (activelist == treeListParam)//从参数窗口的默认是派生复制
            //{
            //    if(thetreelist != treeListParam)
            //        e.Effect = DragDropEffects.Copy; 
            //}
            //else//从基本或者编辑窗口发起
            //{
            //    if (thetreelist == treeListParam)//参数窗口暂时不接收拖放。
            //        return;

            //    //默认是设置连接，只有按下CTRL是派生复制
            //    if((e.KeyState & 8)!=0)//CTRL
            //        e.Effect = DragDropEffects.Copy;
            //    else
            //        e.Effect = DragDropEffects.Link;
            //}
        }

        private void treeList_BeforeDragNode(object sender, BeforeDragNodeEventArgs e)
        {
            TreeList treelist = sender as TreeList;
            //if (treelist==treeListParam && e.Node.Checked == false)
            // e.CanDrag = false;
        }

        private void treeList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            TreeList thetreelist = (TreeList)sender;
            Point p = new Point(e.X, e.Y);

            p = thetreelist.PointToClient(p);

            TreeListHitInfo hitInfo = thetreelist.CalcHitInfo(p);

            if (thetreelist == treeListParam)
                e.Effect = DragDropEffects.None;//参数窗口暂时不接收拖放。
            else
            {
                if (hitInfo.Node == null || hitInfo.Column == null)
                    goto label;


                if (Data.IsObjectField(hitInfo.Column.FieldName, true))//是可编辑的GUID字段，才可能设置引用
                {
                    e.Effect = DragDropEffects.Link;//只是链接

                    //默认是设置连接，只有按下CTRL是派生复制
                    if (activelist == treeListParam)
                    {
                        if (activelist.FocusedNode.Checked)
                            e.Effect = DragDropEffects.All;//拷贝加链接
                    }
                    else if ((e.KeyState & 8) != 0)//CTRL
                        e.Effect = DragDropEffects.All;//拷贝加链接                    }

                }
                else
                {

                    if (activelist == treeListParam || (e.KeyState & 8) != 0)//CTRL
                    {
                        if (hitInfo.Column.FieldName == "形式")
                            e.Effect = DragDropEffects.All;//拷贝加链接
                        else
                            e.Effect = DragDropEffects.Copy;//只拷贝

                        if (activelist == treeListParam && activelist.FocusedNode.Checked == false)
                            e.Effect = DragDropEffects.None;
                    }
                    else if (activelist != treeListParam && activelist == thetreelist)
                    {//同一个窗口，移动到非ID字段，则进行移动
                        e.Effect = DragDropEffects.Move;//移动
                    }
                }
            }

        label:

            if (hitInfo.Node != dragnode || hitInfo.Column != dragcol)
            {
                thetreelist.RefreshNode(dragnode);
                dragnode = null;
                dragcol = null;
                if (hitInfo.Column != null)
                {
                    dragnode = hitInfo.Node;
                    dragcol = hitInfo.Column;
                }

                thetreelist.RefreshNode(dragnode);
            }



        }

        private void treeListCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            TreeList treelist = sender as TreeList;
            if (Data.IsTextField(e.Column.FieldName))
                return;

            //立即提交
            e.Node[e.Column] = e.Value;
            treelist.RefreshNode(e.Node);
            //treelist.PostEditor();
            //treelist.EndCurrentEdit();


        }

        private void 粘贴替换_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            Guid id = (Guid)activelist.FocusedNode["ID"];

            粘贴所有行(false, 1, activelist.FocusedNode);

            Guid focusedID = (Guid)activelist.FocusedNode["ID"];

            activelist.BeginUpdate();
            activelist.BeginSort();
            activelist.LockReloadNodes();


            模式基表 目标表 = Data.FindTableByID(id);
            if (目标表 != null)
                Data.递归删除记录树(目标表, id, true);

            activelist.UnlockReloadNodes();
            activelist.EndSort();
            activelist.EndUpdate();

            刷新当前模式行(focusedID);
        }

        private void 增加形式行_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            Guid id = (Guid)activelist.FocusedNode["ID"];

            var 目标表 = Data.FindTableByID(id);
            模式 row = Data.FindRowByID(id);

            if (目标表 == null)
                return;


            模式 newObj = Data.New派生行(Data.拥有形式Guid);
            newObj.ParentID = id;
            newObj.A端 = id;
            newObj.B端 = Data.ThisGuid;
            string s = row.形式;
            if (s != "")
                s = s.Replace("[", "").Replace("]", "");
            newObj.形式 = s;
            newObj.语言 = Data.当前生成语言;
            newObj.语言角色 = 字典_语言角色.全部;
            newObj.序号 = 0;
            newObj.Bounding = row.Bounding;

            目标表.新加对象(newObj);

            定位选择模式行(newObj.ID, true, false, false);
        }


        private void 移动到标记行后_ItemClick(object sender, ItemClickEventArgs e)
        {
            TreeListNode node = FindNodeByID(_markRowID);

            if (node == null)
                return;

            TreeList treelist = node.TreeList;

            if (activelist != treelist)
                return;

            移动到记录(node, 1);


        }

        private void 移动到标记前_ItemClick(object sender, ItemClickEventArgs e)
        {
            TreeListNode node = FindNodeByID(_markRowID);

            if (node == null)
                return;

            TreeList treelist = node.TreeList;

            if (activelist != treelist)
                return;

            移动到记录(node, -1);
        }



        private void 切换视图_ItemClick(object sender, ItemClickEventArgs e)
        {
            viewstatus++;
        Label:
            switch (viewstatus)
            {
                case 0:
                    treeListEdit.Columns["A端"].Visible = treeListBase.Columns["A端"].Visible = true;
                    treeListEdit.Columns["B端"].Visible = treeListBase.Columns["B端"].Visible = true;
                    treeListEdit.Columns["C端"].Visible = treeListBase.Columns["C端"].Visible = true;
                    treeListEdit.Columns["连接"].Visible = treeListBase.Columns["连接"].Visible = true;
                    treeListEdit.Columns["源记录"].Visible = treeListBase.Columns["源记录"].Visible = true;
                    treeListEdit.Columns["说明"].Visible = treeListBase.Columns["说明"].Visible = true;
                    break;
                case 1:
                    treeListEdit.Columns["A端"].Visible = treeListBase.Columns["A端"].Visible = false;
                    treeListEdit.Columns["B端"].Visible = treeListBase.Columns["B端"].Visible = false;
                    treeListEdit.Columns["C端"].Visible = treeListBase.Columns["C端"].Visible = false;
                    treeListEdit.Columns["连接"].Visible = treeListBase.Columns["连接"].Visible = false;
                    treeListEdit.Columns["源记录"].Visible = treeListBase.Columns["源记录"].Visible = false;
                    treeListEdit.Columns["说明"].Visible = treeListBase.Columns["说明"].Visible = true;
                    break;
                case 2:
                    treeListEdit.Columns["A端"].Visible = treeListBase.Columns["A端"].Visible = true;
                    treeListEdit.Columns["B端"].Visible = treeListBase.Columns["B端"].Visible = true;
                    treeListEdit.Columns["C端"].Visible = treeListBase.Columns["C端"].Visible = true;
                    treeListEdit.Columns["连接"].Visible = treeListBase.Columns["连接"].Visible = true;
                    treeListEdit.Columns["源记录"].Visible = treeListBase.Columns["源记录"].Visible = true;
                    treeListEdit.Columns["说明"].Visible = treeListBase.Columns["说明"].Visible = false;
                    break;
                case 3:
                    viewstatus = 0;
                    goto Label;
            }
        }


        //按照语义对象的关键性等信息，进行自动调序，一般是调整成【主谓宾】的次序。
        private void 展开设置序列态和调序_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            activelist.BeginUpdate();
            activelist.BeginSort();
            activelist.LockReloadNodes();


            Data.第一阶段展开设置角色及默认次序(Data.FindRowByID(id));

            //ReorderSn(activelist, false, id);
            activelist.UnlockReloadNodes();
            activelist.EndSort();
            activelist.EndUpdate();

            定位选择模式行(id);
        }

        //在已经调整好各个参数次序基础上，根据位置，不违反规则的进行序列化，分配角色。把能状语化的做成状语。能做成主谓宾的做成主谓宾。
        private void 根据语序微调角色_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            activelist.BeginUpdate();
            activelist.BeginSort();
            activelist.LockReloadNodes();

            模式 row = Data.FindRowByID(id);
            Data.第二阶段根据语序微调角色(row);

            //ReorderSn(activelist, false, id);

            activelist.UnlockReloadNodes();
            activelist.EndSort();
            activelist.EndUpdate();

            定位选择模式行(id);
        }



        private void 最后配形式和生成语言_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            模式 row = Data.FindRowByID(id);

            activelist.BeginUpdate();
            activelist.BeginSort();
            activelist.LockReloadNodes();

            Data.第三阶段构造表现形式(row, false);
            Data.第四阶段最终生成形式串(row, Data.当前生成语言);

            activelist.UnlockReloadNodes();
            activelist.EndSort();
            activelist.EndUpdate();

            定位选择模式行(id);
        }


        private void 全部执行_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;
            if (activelist != treeListEdit && activelist != treeListBase)
                return;


            activelist.BeginUpdate();
            activelist.BeginSort();
            activelist.LockReloadNodes();
            long t = System.DateTime.Now.Ticks;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
            模式 row = Data.FindRowByID(id);
            Data.第一阶段展开设置角色及默认次序(row);
            Data.第二阶段根据语序微调角色(row);
            Data.第三阶段构造表现形式(row, false);
            Data.第四阶段最终生成形式串(row, Data.当前生成语言);

            t = System.DateTime.Now.Ticks - t;
            double r = t / 10000000.0;
            Data.显示信息(r.ToString());

            activelist.UnlockReloadNodes();
            activelist.EndSort();
            activelist.EndUpdate();

            定位选择模式行(id);
        }

        private void GuidEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = GuidEdit.EditValue.ToString();
                Guid g;
                if (Guid.TryParse(s, out g))
                {
                    //TreeListNode n = FindNodeByID(g);
                    //if (n != null)
                    //    n.Selected = true;
                    选择文本.EditValue = string.Empty;
                    Data.设置模式树一般可见性(g, true, false);
                    定位选择模式行(g, true, false, false);
                }
            }
        }



        private void 清除语义信息_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            Data.递归清除语义信息(Data.FindRowByID(id));

            activelist.EndCurrentEdit();

            TreeListNode node = FindNodeByID(id);
            node.ExpandAll();
            node.Selected = true;

        }



        //进行语义信息的优化，比如去除多余的模式信息等
        private void 优化语义信息_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void 选择生成语言_EditValueChanged(object sender, EventArgs e)
        {
            string str = "名称='" + 选择生成语言.EditValue + "'";
            PatternDataSet.字典_语言Row[] dr = (PatternDataSet.字典_语言Row[])PatternDataSet.字典_语言.Select(str, "树ID", DataViewRowState.CurrentRows);
            if (dr.Count() == 0)
            {
                选择生成语言.EditValue = "汉";
                Data.当前生成语言 = 字典_语言.汉语;
                return;
            }
            Data.当前生成语言 = dr[0].树ID;
        }

        private void 选择解析语言_EditValueChanged(object sender, EventArgs e)
        {
            string str = "名称='" + 选择解析语言.EditValue + "'";
            PatternDataSet.字典_语言Row[] dr = (PatternDataSet.字典_语言Row[])PatternDataSet.字典_语言.Select(str, "树ID", DataViewRowState.CurrentRows);
            if (dr.Count() == 0)
            {
                选择解析语言.EditValue = "汉";
                Data.当前解析语言 = 字典_语言.汉语;
                return;
            }
            Data.当前解析语言 = dr[0].树ID;
        }

        private void 设置选择树为当前语言_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            if (activelist != treeListEdit && activelist != treeListBase)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            模式 row = Data.FindRowByID(id);

            Data.递归设置语言(row, Data.当前解析语言);

            activelist.EndCurrentEdit();

        }

        private void treeListBase_FilterNode(object sender, FilterNodeEventArgs e)
        {
            bool b = false;
            if (选择文本.EditValue == null)
                b = true;
            else
            {
                string s1 = 选择文本.EditValue.ToString();
                if (s1 == "")
                    b = true;
                else
                {
                    string str = e.Node["形式"].ToString();
                    if (文本.Checked == true)
                    {
                        if (str.Contains("\"" + s1 + "\"")/* || str.Contains("\'" + s1 + "\'")*/)
                            b = true;
                    }
                    else if (str.Contains(s1))
                        b = true;

                    if (知识.Checked && (int)e.Node["语境树"] != 0)
                        b = false;
                }
            }


            e.Node.Visible = b;
            e.Handled = true;
        }

        private void RefreshTreeListBaseDataSource()
        {
            var searchKeywords = 选择文本.EditValue as string;
            IList<模式> newDataSourceList = null;
            if (string.IsNullOrEmpty(searchKeywords))
            {
                foreach (var item in Data.模式表.对象集合.Where(r => r.isSearching))
                {
                    item.isSearching = false;
                }

                newDataSourceList = Data.模式表.对象集合.Where(r => r.Bounding).ToList();
            }
            else
            { 
                if (知识.Checked)
                {
                    newDataSourceList = Data.模式表.对象集合.Where(r=>r.语境树 != 0 && r.形式.Contains(searchKeywords)).ToList();
                }
                else
                {
                    newDataSourceList = Data.模式表.对象集合.Where(r=> r.形式.Contains(searchKeywords)).ToList();
                }

                foreach (var item in newDataSourceList)
                {
                    Data.设置模式树查询可见性(item.ID, true, false);
                }

                newDataSourceList = Data.模式表.对象集合.Where(r => r.isSearching).ToList();
            }

            bindingSourceBase.DataSource = new BindingList<模式>(newDataSourceList);
            bindingSourceBase.ResetBindings(false);

            if (!string.IsNullOrEmpty(searchKeywords))
            {
                treeListBase.ExpandAll();
            }
            else
            {
                treeListBase.CollapseAll();
            }
        }

        private void treeListEdit_ShownEditor(object sender, EventArgs e)
        {
            TextEdit edit = treeListEdit.ActiveEditor as TextEdit;
            if (edit == null)
                return;
            edit.KeyDown += edit_KeyDown;
        }

        void edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            TextEdit edit = sender as TextEdit;
            if (edit == null)
                return;

            int begin = edit.SelectionStart;
            int length = edit.SelectionLength;
            string s = edit.SelectedText;

            选择文本.EditValue = s;
            //treeListBase.ExpandAll();
            //treeListBase.FilterNodes();
            RefreshTreeListBaseDataSource();
        }

        private void 选择文本_HiddenEditor(object sender, ItemClickEventArgs e)
        {
            //treeListBase.ExpandAll();
            //treeListBase.FilterNodes();
            RefreshTreeListBaseDataSource();
        }

        private void 文本_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            //treeListBase.ExpandAll();
            //treeListBase.FilterNodes();
            RefreshTreeListBaseDataSource();
        }



        private void 知识_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            //treeListBase.ExpandAll();
            //treeListBase.FilterNodes();
            RefreshTreeListBaseDataSource();
        }        

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (EditString == null)
                return;
            string str = (string)EditString.EditValue;
            if (str != "")
            {
                if (Data.语料库.Count == 0)
                    执行加入形式();
                Processor.english.源语言文字 = str.ToCharArray();
                Processor.english.字符串匹配处理();
                Processor.english.Output();
            }
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (EditString == null)
                return;
            string str = (string)EditString.EditValue;
            if (str != "")
            {
                if (Data.语料库.Count == 0)
                    执行加入形式();
                Processor.chinese.源语言文字 = str.ToCharArray();
                Processor.chinese.字符串匹配处理();
                Processor.chinese.Output();

                //for (int i = 0; i < Data.有效串Count;i++ )
                //	System.Diagnostics.Debug.WriteLine(Data.语料库[i].字符串);

            }
        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (EditString == null)
                return;
            string str = (string)EditString.EditValue;
            if (str != "")
            {
                if (Data.语料库.Count == 0)
                    执行加入形式();
                //Processor.chinese.源语言文字 = str.ToCharArray();
                //Processor.chinese.理解整篇文章();
            }
        }

        private void 执行加入形式()
        {
            //treeliststring.DataSource = null;
            treeliststring.BeginUpdate();
            Data.加入所有形式();
            Data.重置缓存根模式集合();
            treeliststring.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }



        private bool 解析前准备(bool 刷新语料串)
        {
            if (activelist != treeListEdit || treeListEdit.FocusedNode == null)
            {
                XtraMessageBox.Show("没有选择模式编辑窗口中的记录。");
                return false;
            }

            TreeListNode n = treeListEdit.FocusedNode;

            while (n != null)
            {
                if (Data.单句分析Guid.Equals(n["B端"]))
                {
                    Data.当前句子Row = Data.FindRowByID((Guid)n["ID"]);
                    TheString str = new TheString(Data.当前句子Row.形式);
                    Data.当前句子串 = str.嵌入串;
                    if (刷新语料串)
                        执行加入形式();
                    return true;
                }
                n = n.ParentNode;
            }

            XtraMessageBox.Show("没有选择模式编辑窗口中的句子。");
            return false;
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (treeListEdit.Columns["说明"] == null)
                return;
            treeListEdit.Columns["说明"].FieldName = "参数集合";
            treeListBase.Columns["说明"].FieldName = "参数集合";
            treeListEdit.Columns["参数集合"].Caption = "参数集合";
            treeListBase.Columns["参数集合"].Caption = "参数集合";
            repositoryItemButtonEdit1.Buttons[0].Visible = true;
            repositoryItemButtonEdit2.Buttons[0].Visible = true;
        }



        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (treeListEdit.Columns["参数集合"] == null)
                return;
            treeListEdit.Columns["参数集合"].FieldName = "说明";
            treeListBase.Columns["参数集合"].FieldName = "说明";
            treeListEdit.Columns["说明"].Caption = "说明";
            treeListBase.Columns["说明"].Caption = "说明";
            repositoryItemButtonEdit1.Buttons[0].Visible = false;
            repositoryItemButtonEdit2.Buttons[0].Visible = false;
        }



        private void 分解串并生成一级对象_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListEdit || activelist.FocusedNode == null)
                return;

            Guid id = (Guid)treeListEdit.FocusedNode.GetValue("ID");
            if (解析前准备(true) == false)
                return;
            using (new UseWaitCursor())
            {


                treeListEdit.BeginUpdate();
                treeListEdit.BeginSort();
                treeListEdit.LockReloadNodes();
                treeListresult.BeginUpdate();

                删除下级节点(id);

                Data.分解串并生成串对象(id);

                treeListresult.EndUpdate();
                treeListEdit.UnlockReloadNodes();
                treeListEdit.EndSort();
                treeListEdit.EndUpdate();

                定位选择模式行(id);

                tabControl1.SelectedIndex = 2;
                treeListresult.ExpandAll();
                if (展开.Checked == false)
                    展开.Checked = true;
            }
        }



        private void 参数表中查找匹配关联_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == treeListEdit || activelist == treeListBase)
            {
                if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                    return;

                Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
                if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                    id = (Guid)activelist.FocusedNode[activelist.FocusedColumn];

                模式 row = Data.FindRowByID(id);

                参数树结构 objset = Data.调整行得到基类和关联树(true, row, true, 0, /*0,*/ Data.当前解析语言);
                if (objset == null)
                    return;

                foreach (PatternDataSet.模式查找Row r in PatternDataSet.模式查找)
                {
                    模式 row1 = Data.FindRowByID(r.ObjectID);
                    TreeListNode node = treeListParam.FindNodeByKeyID(r.ID);
                    Guid theid = Guid.Empty;
                    if (r.根 == "A端")
                        theid = (Guid)row1.B端;
                    else if (r.根 == "B端")
                        theid = (Guid)row1.A端;
                    if (Guid.Empty.Equals(theid) || Data.概念Guid.Equals(theid))//【概念】类排除掉，因为它肯定所有的都满足。
                        continue;
                    node.Checked = objset.递归从基类树中查找广义匹配的基类(theid, 替代.正向替代  /*| 正向扮演替代*/) != -1;
                }
                if (展开.Checked == false)
                    展开.Checked = true;
            }
        }

        private void 生成语料库_ItemClick(object sender, ItemClickEventArgs e)
        {
            执行加入形式();
            tabControl1.SelectedIndex = 1;
        }

        private void 两个概念的匹配度_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == treeListEdit || activelist == treeListBase || Guid.Empty.Equals(_markRowID))
            {
                if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                    return;

                Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

                if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                    id = (Guid)activelist.FocusedNode[activelist.FocusedColumn];

                Guid id2 = _markRowID;

                if (_markCol != null)
                {
                    模式 row = Data.FindRowByID(id2);
                    id2 = (Guid)typeof(模式).GetProperty(_markCol).GetValue(row);
                }

                float t = Data.A概念对B概念的符合度(id, id2);
                符合度.Text = t.ToString();

                if (展开.Checked == false)
                    展开.Checked = true;
            }

        }

        private void 展开_CheckedChanged(object sender, EventArgs e)
        {
            if (展开.Checked == true)
                splitContainerControl1.SplitterPosition = 450;
            else
                splitContainerControl1.SplitterPosition = 250;
        }

        //private void 同步检查状态(TreeListNodes nodes)
        //{
        //	foreach (TreeListNode n in nodes)
        //	{
        //		参数字段 打分 = new 参数字段((string)n["打分"]);
        //		n.Checked = (打分.长度分 > 0);
        //		同步检查状态(n.Nodes);
        //	}
        //}

        private void 列出两个概念相互的关联_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null || activelist.FocusedNode.Focused == false)
                return;

            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");

            if (activelist.FocusedColumn != null && Data.IsObjectField(activelist.FocusedColumn.FieldName, true))
                id = (Guid)activelist.FocusedNode[activelist.FocusedColumn];

            Guid id2 = _markRowID;

            if (_markCol != null)
            {
                模式 row = Data.FindRowByID(id2);
                id2 = (Guid)typeof(模式).GetProperty(_markCol).GetValue(row);
            }

            treeListParam.BeginUpdate();

            PatternDataSet.模式查找.Clear();
            Data.全局序号 = 0;

            Data.填充两个概念相互的关联(id, id2, 含属于.Checked);

            treeListParam.ExpandAll();

            treeListParam.EndUpdate();

            treeListParam.MoveFirst();


            if (展开.Checked == false)
                展开.Checked = true;

            tabControl1.SelectedIndex = 0;
        }

        private void 进行一轮关联生长_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (解析前准备(false) == false || Data.当前素材Row==null)
                return;
            using (new UseWaitCursor())
            {
                treeListEdit.BeginUpdate();
                treeListEdit.BeginSort();
                treeListEdit.LockReloadNodes();

                treeListresult.BeginUpdate();

                Guid id = (Guid)treeListEdit.FocusedNode.GetValue("ID");

                Data.进行一轮关联生长();

                treeListresult.EndUpdate();

                treeListEdit.UnlockReloadNodes();
                treeListEdit.EndSort();
                treeListEdit.EndUpdate();

                定位选择模式行(id);
            }

            treeListresult.ExpandAll();
        }

        private void 查看生成树_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListresult)
                return;
            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
            if (Guid.Empty.Equals(id))
                return;

            using (new UseWaitCursor())
            {
                treeListEdit.BeginUpdate();
                treeListEdit.BeginSort();
                treeListEdit.LockReloadNodes();

                Processor p = Processor.当前处理器;
                模式 row = p.查看生成树(id, 生成树时删除空行.Checked);

                treeListEdit.UnlockReloadNodes();
                treeListEdit.EndSort();
                treeListEdit.EndUpdate();

                if (row != null)
                {
                    activelist = treeListEdit;
                    定位选择模式行(row.ID, true, false, false);
                }
            }
        }
        public void 测试句子(string txt)
        {
            TreeListNode node = treeListEdit.FindNodeByFieldValue("序号", -1);
            if (node != null)
            {
                treeListEdit.FocusedNode = node;
                activelist = treeListEdit;
                treeListEdit.FocusedNode["形式"] = "\"" + txt + "\"";
                全部执行得到一个结果_ItemClick(null, null);
            }
            else
                MessageBox.Show("在模式编辑表中没有找到序号为-1的记录，请先添加！", "提示");
        }
        private void 全部执行得到一个结果_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListEdit || activelist.FocusedNode == null)
                return;
            Guid id = (Guid)treeListEdit.FocusedNode["ID"];

            treeListEdit.BeginUpdate();
            treeListEdit.BeginSort();
            treeListEdit.LockReloadNodes();
            
            全部执行得到结果(id);

            treeListEdit.UnlockReloadNodes();
            treeListEdit.EndSort();
            treeListEdit.EndUpdate();

            定位选择模式行(id, true, true, false);
        }

        private void 全部执行并清除中间结果_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListEdit || activelist.FocusedNode == null)
                return;
            Guid id = (Guid)treeListEdit.FocusedNode.GetValue("ID");

            treeListEdit.BeginUpdate();
            treeListEdit.BeginSort();
            treeListEdit.LockReloadNodes();

            全部执行得到结果(id);

            var dr = Data.模式编辑表.对象集合.Where(r => r.ParentID == id).ToList();
            foreach (模式 row in dr)
            {
                if (Data.生长素材Guid.Equals(row.B端))
                    Data.递归删除记录树(Data.模式编辑表, row.ID, true);
            }
            _patternDataSet.模式结果.Clear();

            treeListEdit.UnlockReloadNodes();
            treeListEdit.EndSort();
            treeListEdit.EndUpdate();

            定位选择模式行(id, true, true, false);
        }

        private void 匹配派生树_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist != treeListresult)
                return;
            Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
            if (Guid.Empty.Equals(id))
                return;
            using (new UseWaitCursor())
            {
                treeListresult.BeginUpdate();
                Processor p = Processor.当前处理器;
                p.进行派生匹配(id);

                treeListresult.EndCurrentEdit();
                treeListresult.EndUpdate();

                treeListresult.ExpandAll();
            }

        }

        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            treeListEdit.OptionsView.ShowColumns = !treeListEdit.OptionsView.ShowColumns;
        }

        private void barBtnItem创建等价关联对象_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;

            if (activelist == treeListEdit || activelist == treeListBase)
            {
                模式 markRow = Data.FindRowByID(_markRowID);
                if (markRow == null)
                    return;

                模式 selectedRow = GetRowByNode(activelist, activelist.FocusedNode);
                if (selectedRow == null || markRow.Equals(selectedRow))
                    return;

                模式 row = 新建并挂接等价模式行(selectedRow.ID, _markRowID);

                if (row != null)
                    定位选择模式行(row.ID, true, false, false);
            }
        }

        private 模式 新建并挂接等价模式行(Guid A端模式Id, Guid B端模式Id)
        {
            模式 A端对象 = Data.FindRowByID(A端模式Id);
            模式 B端对象 = Data.FindRowByID(B端模式Id);
            模式 等价对象 = Data.FindRowByID(Data.等价Guid);

            if (A端对象.A端 != Data.ThisGuid && !Data.属于Guid.Equals(Data.一级关联类型(A端对象)) || B端对象.A端 != Data.ThisGuid && !Data.属于Guid.Equals(Data.一级关联类型(B端对象)))
                return null;

            模式 等价模式行 = new 模式(true);
            等价模式行.A端 = A端模式Id;
            等价模式行.B端 = B端模式Id;
            等价模式行.源记录 = Data.等价Guid;
            等价模式行.连接 = Data.等价Guid;
            等价模式行.That根 = 字典_目标限定.连接;
            等价模式行.C端 = Data.NullGuid;
            等价模式行.语言角色 = 字典_语言角色.无;
            等价模式行.语言 = 字典_语言.语义;
            等价模式行.显隐 = 字典_显隐.正常;
            等价模式行.形式 = "[" + new TheString(A端对象.形式).嵌入串 + "][等价][" + new TheString(B端对象.形式).嵌入串 + "]";

            模式 根模式 = Data.FindRowByID(B端对象.ParentID);
            if (根模式 == null)
                return null;

            while (!(Data.单个结果Guid.Equals(根模式.B端) && Data.属于Guid.Equals(Data.一级关联类型(根模式))))
            {
                根模式 = Data.FindRowByID(根模式.ParentID);
            }

            等价模式行.序号 = Data.FindTableByID(根模式.ID).对象集合.Where(r => r.ParentID == 根模式.ID).Max(r => r.序号) + 1;
            等价模式行.ParentID = 根模式.ID;

            Data.FindTableByID(根模式.ID).新加对象(等价模式行);

            return 等价模式行;
        }

        private void 粘贴为父记录并设置为基类_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (activelist == null || activelist.FocusedNode == null)
                return;

            TreeListNode node = activelist.FocusedNode;
            Guid id = (Guid)node["ID"];

            模式 row = GetRowByNode(activelist, activelist.FocusedNode);

            if (row != null && Data._clipObject is Guid)
            {
                //TreeListNode parent = node.ParentNode;
                //node["ParentID"] = Data._clipObject;
                Guid parentid = (Guid)Data._clipObject;
                模式 rowparent = Data.FindRowByID(parentid);
                if (rowparent == null)
                    return;
                row.ParentID = parentid;
                row.源记录 = parentid;
                row.B端 = parentid;


                Data.FindTableByID(row.ID).更新对象(row);
                刷新当前模式行(row.ID);
                //node.Visible = false;
            }

        }

        /// <summary>
        /// 是否加载外围语料库
        /// </summary>
        private void barCheckItem2_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (this.barCheckItem2.Checked)
            {
                if (!Data.外围语料库.Any())
                {
                    try
                    {
                        Data.加载外围语料库();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show("外围库加载失败:"+(ex.InnerException==null?ex.Message:ex.InnerException.Message),
                                            "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.barCheckItem2.Checked = false;
                    }
                }
                Data.语料库.AddRange(Data.外围语料库);
                Data.语料库.Sort();
            }
            else
            {
             
                Data.语料库.RemoveAll(r => r.IsCore == false);
                Data.外围语料库.Clear();
                Data.语料库.Sort();

                treeListEdit.BeginUpdate();
                treeListEdit.BeginSort();
                treeListEdit.LockReloadNodes();

                删除下级节点(Data.公共新对象Guid); //删除已经加载的公共新对象

                treeListEdit.UnlockReloadNodes();
                treeListEdit.EndSort();
                treeListEdit.EndUpdate();

            }
        }

        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
           //if (this.句子测试frm==null)
           //    this.句子测试frm=new 句子测试Form();
           //this.句子测试frm.Show();
           dockPanel1.Visibility = DockVisibility.AutoHide;
           dockPanel1.Show();
        }

        private void barCheckItem4_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (this.barCheckItem4.Checked)
                Data.允许新词汇自动处理 = true;
            else
                Data.允许新词汇自动处理 = false;
        }

        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (new UseWaitCursor())
            {
                treeListBase.BeginUpdate();
                treeListBase.BeginSort();
                treeListBase.LockReloadNodes();
                try
                {
                    Data.reLoad();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("核心库加载失败:" +(ex.InnerException==null?ex.Message:ex.InnerException.Message), 
                                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    treeListBase.UnlockReloadNodes();
                    treeListBase.EndSort();
                    treeListBase.EndUpdate();

                    //bindingSourceBase.DataSource = Data.模式表.对象集合;
                    //bindingSourceEdit.DataSource = Data.模式编辑表.对象集合;
                    //treeListEdit.CollapseAll();
                    //treeListEdit.FocusedNode = treeListEdit.Nodes[0];

                    Data.重置缓存根模式集合();
                    Data.加入所有形式();
                }
            }
        }

        private void iResetCore_ItemClick(object sender, ItemClickEventArgs e)
        {
            Data.初始化模式可见性();
            RefreshTreeListBaseDataSource();
        }
        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            showParamsForm(sender);
        }

        private void repositoryItemButtonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            showParamsForm(sender);
        }

        private void showParamsForm(object sender)
        {
            ParamsForm form = new ParamsForm();
            form.Tag = sender;
            form.ShowDialog();

            if (activelist == null || activelist.FocusedNode == null)
            {
                return;
            }
            模式 row = Data.FindRowByID((Guid)activelist.FocusedNode["ID"]);
            if (row == null)
                return;

            activelist.BeginUpdate();
            ButtonEdit btn = (ButtonEdit)sender;
            row.参数集合 = btn.Text;
            activelist.EndUpdate();
        }
        //为LookUpEdit控件添加拖拽处理事件
        private void 添加拖拽处理事件(LookUpEdit lookUpEdit)
        {
            lookUpEdit.DragEnter += lookUpEdit_DragEnter;
            lookUpEdit.DragDrop += lookUpEdit_DragDrop;
        }
        //为TextEdit控件添加拖拽处理事件
        private void 添加拖拽处理事件(TextEdit textEdit)
        { 
            textEdit.DragEnter+=textEdit_DragEnter;
            textEdit.DragDrop += textEdit_DragDrop;
            textEdit.TextChanged += textEdit_TextChanged;
        }
        //TextEdit控件内容手动修改时，设置Tag对象为Null
        private void textEdit_TextChanged(object sender, EventArgs e)
        {
            ((TextEdit)sender).Tag =null;
        }
        //拖拽模式行至TextEdit控件时，设置拖拽处理方式
        private void textEdit_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        //拖拽模式行至TextEdit控件后，填入对象信息
        private void textEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (activelist != null && activelist.FocusedNode != null)
            {
                string fieldName = activelist.FocusedColumn.FieldName;
                Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
                if (Data.IsObjectField(fieldName, true))
                    id = (Guid)activelist.FocusedNode.GetValue(fieldName);
                if (id != null)
                {
                    模式 row = Data.FindRowByID(id);
                    if (row != null)
                    {
                        ((TextEdit)sender).Text = row.形式;
                        ((TextEdit)sender).Tag = row;
                    }
                }
            }
        }
        //拖拽模式行至LookUpEdit控件时，设置拖拽处理方式
        private void lookUpEdit_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        //拖拽模式行至LookUpEdit控件后，填入对象信息
        private void lookUpEdit_DragDrop(object sender, DragEventArgs e)
        {
            if (activelist != null && activelist.FocusedNode != null && activelist.FocusedColumn!=null)
            {
                string fieldName = activelist.FocusedColumn.FieldName;
                Guid id = (Guid)activelist.FocusedNode.GetValue("ID");
                if (Data.IsObjectField(fieldName,true))
                    id = (Guid)activelist.FocusedNode.GetValue(fieldName);
                if (id != null)
                {
                    模式 row=Data.FindRowByID(id);
                    添加并选择一个新模式选项((LookUpEdit)sender, row);
                }
            }
        }
        private void 添加并选择一个新模式选项(LookUpEdit lookUpEdit, 模式 row)
        {
            if (row != null)
            {
                List<模式> 关联类型 = (List<模式>)lookUpEdit.Properties.DataSource;
                if (关联类型 != null)
                {
                    if (关联类型.Exists(r => r.ID == row.ID) == false)
                    {
                        关联类型.Add(row);
                    }
                    (lookUpEdit).EditValue = row.ID;
                }
            }
        }
        //快速生成两个对象的关联配置
        private void btn生成_Click(object sender, EventArgs e)
        {
            if (textEditA端.Tag != null && textEditB端.Tag != null && lookUpEdit关联.EditValue != null && lookUpEdit关联.EditValue is Guid)
            {
                模式 A端 = (模式)textEditA端.Tag;
                模式 B端 = (模式)textEditB端.Tag;
                模式 关联 = Data.FindRowByID((Guid)lookUpEdit关联.EditValue);
                if (Data.是二元关联(A端, true) == false && Data.是二元关联(B端, true) == false)
                {
                    if (Data.存在于模式表中(A端.ID) && Data.存在于模式表中(B端.ID) && Data.存在于模式表中(关联.ID))
                    {
                        if (Data.是二元关联(关联, true))
                        {
                            treeListBase.BeginUpdate();
                            treeListBase.BeginSort();
                            treeListBase.LockReloadNodes();

                            模式 关联row = 根据AB端快速生成关联配置(关联, A端, B端, this.textEdit前置介词.Text, this.textEdit后置介词.Text);

                            treeListBase.UnlockReloadNodes();
                            treeListBase.EndSort();
                            treeListBase.EndUpdate();
                            定位选择模式行(关联row.ID, true, true, true);
                        }
                        else
                            MessageBox.Show("关联必须是[二元关联]对象!", "提示");
                    }
                    else
                        MessageBox.Show("A端、B端、关联必须存在于[模式表]中!", "提示");
                }
                else
                    MessageBox.Show("A端或B端不能为[二元关联]对象!", "提示");
            }
            else
            {
                MessageBox.Show("A端、B端、关联均不能为空!", "提示");
            }
        }
        private 模式 根据AB端快速生成关联配置(模式 关联, 模式 A端, 模式 B端, string 前置介词, string 后置介词)
        {
            if (关联 == null || A端 == null || B端 == null)
                return null;

            模式 关联row = Data.New派生行(关联);
            关联row.ParentID = A端.ID;
            关联row.A端 = A端.ID;
            关联row.B端 = B端.ID;
            关联row.序号 = 100;
            根据关联设置默认形式(关联row);
            关联row.说明 = "";
            Data.模式表.新加对象(关联row);

            模式 语言角色row = Data.CopyRow(Data.FindRowByID(Data.拥有语言角色Guid));
            语言角色row.ParentID = 关联row.ID;
            语言角色row.A端 = A端.ID;
            语言角色row.B端 = B端.ID;
            语言角色row.源记录 = 关联row.ID;
            语言角色row.连接 = Data.拥有语言角色Guid;
            语言角色row.序号 = 0;
            根据关联设置默认形式(语言角色row);
            语言角色row.说明 = "";
            语言角色row.语言角色 = 字典_语言角色.定语;
            语言角色row.语言 = Data.当前解析语言;
            语言角色row.That根 = 字典_目标限定.A端;
            Data.模式表.新加对象(语言角色row);

            if (string.IsNullOrWhiteSpace(前置介词) == false)
            {
                模式 前置介词row = Data.New派生行(Data.关联拥有前置介词Guid);
                前置介词row.ParentID = 关联row.ID;
                前置介词row.A端 = 关联row.ID;
                前置介词row.B端 = Data.ThisGuid;
                前置介词row.序号 = 100;
                前置介词row.说明 = "";
                前置介词row.语言角色 = 字典_语言角色.全部;
                前置介词row.语言 = Data.当前解析语言;
                前置介词row.形式 = 前置介词.Trim();
                Data.模式表.新加对象(前置介词row);
            }

            if (string.IsNullOrWhiteSpace(后置介词) == false)
            {
                模式 后置介词row = Data.New派生行(Data.关联拥有后置介词Guid);
                后置介词row.ParentID = 关联row.ID;
                后置介词row.A端 = 关联row.ID;
                后置介词row.B端 = Data.ThisGuid;
                后置介词row.序号 =200;
                后置介词row.说明 = "";
                后置介词row.语言角色 = 字典_语言角色.全部;
                后置介词row.语言 = Data.当前解析语言;
                后置介词row.形式 = 后置介词.Trim();
                Data.模式表.新加对象(后置介词row);
            }
            return 关联row;
        }
        private void 根据关联设置默认形式(模式 关联row)
        { 
            string ret="拥有";
            if (Data.是派生关联(Data.属于Guid, 关联row) > 0)
                ret = "属于";
            else if (Data.是派生关联(Data.聚合Guid, 关联row) > 0)
                ret = "聚合";

            模式 A端 = Data.FindRowByID(关联row.A端);
            模式 B端 = Data.FindRowByID(关联row.B端);

            if (A端 != null && B端 != null)
            {
                if (关联row.连接.Equals(Data.拥有语言角色Guid))
                    ret = "["+添加形式括号(A端.形式,true) + 添加形式括号(ret,true) + 添加形式括号(B端.形式,true)+"][拥有][语言角色]";
                else
                    ret = 添加形式括号(A端.形式) + 添加形式括号(ret) + 添加形式括号(B端.形式);
                关联row.形式 = ret;
            }
        }
        private string 添加形式括号(string 形式,bool 去除括号=false)
        {
            if (string.IsNullOrWhiteSpace(形式) == false)
            {
                if (去除括号)
                {
                    if (形式.Substring(0, 1) == "[")
                        return 形式.Substring(1, 形式.Length - 2);
                    else
                        return 形式;
                }
                else
                {
                    if (形式.Substring(0, 1) == "[")
                        return 形式;
                    else
                        return "[" + 形式 + "]";
                }
            }
            return "";
        }
        //快速派生语义对象
        private void btn生成语义派生_Click(object sender, EventArgs e)
        {
            if (this.textEdit父对象.Tag != null && string.IsNullOrWhiteSpace(this.textEdit形式.Text) == false)
            {
                模式 父对象 = (模式)this.textEdit父对象.Tag;
                if (Data.存在于模式表中(父对象.ID))
                {
                    treeListBase.BeginUpdate();
                    treeListBase.BeginSort();
                    treeListBase.LockReloadNodes();

                    模式 row = 派生一个子对象(父对象, this.textEdit形式.Text);

                    treeListBase.UnlockReloadNodes();
                    treeListBase.EndSort();
                    treeListBase.EndUpdate();
                    定位选择模式行(row.ID, true, true, true);
                }
                else
                    MessageBox.Show("父对象必须存在于[模式表]中!", "提示");
            }
            else
                MessageBox.Show("父对象、形式均不能为空!", "提示");
        }
        //从指定父对象派生一个新的语义对象，并添加形式
        private 模式 派生一个子对象(模式 父对象, string 形式)
        {
            if (父对象 == null || string.IsNullOrWhiteSpace(形式))
                return null;

            模式 新对象row = Data.New派生行(父对象);
            新对象row.ParentID = 父对象.ID;
            新对象row.A端 = Data.ThisGuid;
            新对象row.B端 = 父对象.ID;
            新对象row.序号 = 100;
            新对象row.形式 = 添加形式括号(形式);
            新对象row.说明 = "";
            Data.模式表.新加对象(新对象row);

            模式 形式row = Data.New派生行(Data.FindRowByID(Data.拥有形式Guid));
            形式row.ParentID = 新对象row.ID;
            形式row.A端 = 新对象row.ID;
            形式row.B端 =Data.ThisGuid;
            形式row.序号 = 0;
            形式row.形式 = 形式;
            形式row.说明 = "";
            形式row.语言角色 = 字典_语言角色.全部;
            形式row.语言 = Data.当前解析语言;
            形式row.That根 = 字典_目标限定.连接;
            Data.模式表.新加对象(形式row);

            return 新对象row;
        }

        private void 派生关联至根对象_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.activelist == treeListParam)
            {
                Guid id=(Guid)activelist.FocusedNode.GetValue("ObjectID");
                模式 关联 = Data.FindRowByID(id);
                if (关联 != null && Data.是二元关联(关联, false))
                {
                    var row  = PatternDataSet.模式查找.First() ;
                    模式 根对象=Data.FindRowByID(row.ObjectID);

                    //回填信息至快速配置界面，方便手动配置
                    this.textEditA端.Text = 根对象.形式;
                    this.textEditA端.Tag = 根对象;
                    模式 B端=Data.FindRowByID(关联.B端);
                    this.textEditB端.Text = B端.形式;
                    this.textEditB端.Tag = B端;
                    添加并选择一个新模式选项(this.lookUpEdit关联, 关联);

                    //直接将【关联】行的配置整体复制过来
                    treeListBase.BeginUpdate();
                    treeListBase.BeginSort();
                    treeListBase.LockReloadNodes();

                    模式 新关联row=Data.PasteRows(Data.CopyTree(关联), true, 0, 根对象);
             
                    Dictionary<Guid, 模式> keys = new Dictionary<Guid, 模式>();
                    keys.Add(关联.A端, 根对象);
                    递归替换ID(新关联row, keys);

                    新关联row.源记录 = 关联.ID;
                    新关联row.序号 = 100;
                    新关联row.说明 = "";
                    根据关联设置默认形式(新关联row);
                    递归设置默认形式(新关联row);

                    treeListBase.UnlockReloadNodes();
                    treeListBase.EndSort();
                    treeListBase.EndUpdate();
                    定位选择模式行(新关联row.ID, true, true, true);
                }
            }
        }
        private void 递归设置默认形式(模式 row)
        {
            if (row.连接.Equals(Data.拥有语言角色Guid) || row.连接.Equals(Data.拥有Guid)  || row.连接.Equals(Data.属于Guid)
                    ||row.连接.Equals(Data.聚合Guid)) 
            {
                根据关联设置默认形式(row);
            }
            if (row.端索引表_Parent != null)
            {
                foreach (模式 obj in row.端索引表_Parent)
                    递归设置默认形式(obj);
            }
        }
        private void 递归替换ID(模式 row, Dictionary<Guid, 模式> keys)
        {
            foreach (var r in keys)
            {
                if (keys.ContainsKey((Guid)row.ParentID))
                {
                    row.ParentID = keys[(Guid)row.ParentID].ID;
                }

                if (keys.ContainsKey((Guid)row.A端))
                {
                    row.A端 = keys[(Guid)row.A端].ID;
                }
                if (keys.ContainsKey((Guid)row.B端))
                {
                    row.B端 = keys[(Guid)row.B端].ID;
                }
                if (keys.ContainsKey((Guid)row.连接))
                {
                    row.连接 = keys[(Guid)row.连接].ID;
                }
                if (keys.ContainsKey((Guid)row.源记录))
                {
                    row.源记录 = keys[(Guid)row.源记录].ID;
                }
                if (keys.ContainsKey((Guid)row.C端))
                {
                    row.C端 = keys[(Guid)row.C端].ID;
                }
            }

            if (row.端索引表_Parent != null)
            {
                foreach (模式 obj in row.端索引表_Parent)
                    递归替换ID(obj, keys);
            }
        }
    }
}



