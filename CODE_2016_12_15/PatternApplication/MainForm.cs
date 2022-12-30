using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Helpers;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraNavBar;
using DevExpress.XtraTabbedMdi;
using PatternApplication.Forms;
using System.Linq;
using PatternApplication.Interfaces;


namespace PatternApplication
{
    public partial class MainForm : XtraForm 
    {
		public 模式Form frm;
        public MainForm()
        {
            InitializeComponent();
            InitSkinGallery();
            Load += OnLoad;
            Closing += OnClosing;

            iSave.ItemClick += ISaveOnItemClick;
            iExit.ItemClick += IExitOnItemClick;
        }
        public void Init()
        {
            OnLoad(null);

        }
        private void IExitOnItemClick(object sender, ItemClickEventArgs itemClickEventArgs)
        {
            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (iSave.Enabled)
            {
                var result = XtraMessageBox.Show("是否保存修改后的数据？", "提示", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == DialogResult.Yes)
                {
                    Save();
                }
            }
        }

        private void Save()
        {
            using (new UseWaitCursor())
            {
                foreach (var child in this.MdiChildren)
                {
                    var saveForm = child as ISaveForm;
                    if (saveForm != null)
                    {
                        saveForm.EndEdit();
                    }
				}

                tableAdapterManager1.UpdateAll(patternDataSet1);
            }
        }

        private void ISaveOnItemClick(object sender, ItemClickEventArgs e)
        {
            Save();
            iSave.Enabled = false;
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            //模式TableAdapter1.Fill(patternDataSet1.模式);
            //模式编辑TableAdapter1.Fill(patternDataSet1.模式编辑);
            模式查找TableAdapter1.Fill(patternDataSet1.模式查找);
            //形式化语料串TableAdapter1.Fill(patternDataSet1.形式化语料串);
            字典_语言TableAdapter1.Fill(patternDataSet1.字典_语言);
            字典_限定目标TableAdapter1.Fill(patternDataSet1.字典_限定目标);
            字典_语言角色TableAdapter1.Fill(patternDataSet1.字典_语言角色);
            字典_隐藏形式TableAdapter1.Fill(patternDataSet1.字典_隐藏形式);
            字典_的TableAdapter1.Fill(patternDataSet1.字典_的);

            foreach (DataTable table in patternDataSet1.Tables){
                if (table != patternDataSet1.Tables["模式查找"] && table != patternDataSet1.Tables["形式化语料串"] && table != patternDataSet1.Tables["模式结果"])
				{
                table.RowChanged += TableOnRowChanged;
                table.RowDeleted += TableOnRowDeleted;
                table.TableNewRow += TableOnTableNewRow;
				}

            }
            try
            {
                frm = new 模式Form { MdiParent = this, WindowState = FormWindowState.Maximized };
                frm.PatternDataSet = patternDataSet1;
                Data.mainfrm = this;
                frm.Show();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("核心库加载失败:" +(ex.InnerException==null?ex.Message:ex.InnerException.Message),
                                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       
            }
        }

        private void TableOnTableNewRow(object sender, DataTableNewRowEventArgs dataTableNewRowEventArgs)
        {
            //if (dataTableNewRowEventArgs.Row.Table == patternDataSet1.模式)
            //    Data.模式表Select缓存.Clear();
            iSave.Enabled = true;
        }

        private void TableOnRowDeleted(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            //if (dataRowChangeEventArgs.Row.Table == patternDataSet1.模式)
            //    Data.模式表Select缓存.Clear();
			iSave.Enabled = true;
        }

        private void TableOnRowChanged(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
        {
            //if (dataRowChangeEventArgs.Row.Table == patternDataSet1.模式)
            //    Data.模式表Select缓存.Clear();iSave.Enabled = true;
        }

        void InitSkinGallery()
        {
            //SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        //private void 字典_语言角色BindingNavigatorSaveItem_Click(object sender, EventArgs e)
        //{
        //    this.Validate();
        //    this.字典_语言角色BindingSource.EndEdit();
        //    this.tableAdapterManager1.UpdateAll(this.patternDataSet1);

        //}
    }
}