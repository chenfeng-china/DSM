using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PatternApplication.Forms
{
    public partial class 对话Form : DevExpress.XtraEditors.XtraForm
    {
        private 句子测试Form 句子测试frm;
        public 对话Form()
        {
            InitializeComponent();
        }

        private void InitControls()
        {
            学习.CheckedChanged += new System.EventHandler(this.toolStripButton_CheckedChanged);
            翻译.CheckedChanged += new System.EventHandler(this.toolStripButton_CheckedChanged);
            toolStripButton4.CheckedChanged += new System.EventHandler(this.toolStripButton_CheckedChanged);
            textBox2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            textBox1.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            textBox3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox3_KeyDown);

            foreach (DataRow row in Data.patternDataSet.字典_语言)
            {
                comboBox解析.DropDownItems.Add((string)row["名称"]);
				comboBox生成.DropDownItems.Add((string)row["名称"]);
            }

            comboBox解析.Text = Data.模式frm.选择解析语言.EditValue.ToString();
            comboBox生成.Text = Data.模式frm.选择生成语言.EditValue.ToString();
        }

        public void 设置焦点()
        {
            textBox3.Focus();
        }

        private void 对话Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            if (句子测试frm!=null)
                句子测试frm.Hide();
        }

        private void 对话Form_Load(object sender, EventArgs e)
        {
            // TODO: 这行代码将数据加载到表“patternDataSet.形式化语料串”中。您可以根据需要移动或删除它。
            //this.形式化语料串TableAdapter.Fill(this.patternDataSet.形式化语料串);
            // TODO: 这行代码将数据加载到表“patternDataSet.形式化语料串”中。您可以根据需要移动或删除它。
            //this.形式化语料串TableAdapter.Fill(this.patternDataSet.形式化语料串);
            // TODO: 这行代码将数据加载到表“patternDataSet.形式化语料串”中。您可以根据需要移动或删除它。
            //this.形式化语料串TableAdapter.Fill(this.patternDataSet.形式化语料串);
            //形式化语料串BindingSource.DataSource = Data.patternDataSet;
            //int i=Data.patternDataSet.形式化语料串.Rows.Count;
            形式化语料串BindingSource.DataSource = Data.patternDataSet;
            InitControls();
            //句子测试frm = new 句子测试Form();
            //句子测试frm.对话frm = this;
        }

        public void 设置添加新知识状态()
        {
            Processor p = Processor.当前处理器;
			if (p.新知识对象.Count > 0)
			{
				string s = string.Format("学习到{0}条新知识，是否添加到库？", p.新知识对象.Count);
				Data.输出处理信息("确认", s);
				Data.输出对话信息(s);
				p.等待确认加入新知识 = true;
			}
			else
				p.等待确认加入新知识 = false;
        }

		public bool 处理添加新知识(string str)
		{
			Processor p = Processor.当前处理器;
			if (p.等待确认加入新知识 == false)
			{
				p.新知识对象.Clear();
				return false;
			}
			Data.输出对话信息(str, false);
			if (str == "是" || str == "是的" || str == "好" || str == "好的")
			{
				p.保存新知识();
				Data.输出处理信息("完成", "添加新知识到库。");
			}
			else
				Data.输出处理信息("取消", "取消添加新知识。");
			p.等待确认加入新知识 = false;
			p.新知识对象.Clear();
			return true;
		}

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox3.Text == "")
                    return;
                int k = textBox3.GetLineFromCharIndex(textBox3.GetFirstCharIndexOfCurrentLine());
                string s = textBox3.Lines[k];
                if (string.IsNullOrEmpty(s) == false)
                {
                    开始解析句子(s);
                    textBox3.Clear();
                    textBox3.Focus();
                }
            }
        }
        public void 开始解析句子(string s)
        {
            Data.简略回答 = 简略回答.Checked;
            Data.解释和推导 = 解释和推导.Checked;
            using (new UseWaitCursor())
            {
                if (处理添加新知识(s) == false)
                {
                    Data.检查新知识 = 检查新知识.Checked;
                    if (Data.动态绑定至Form)
                    {
                        if (Data.FindTableByID(Data.当前会话) != Data.模式编辑表)
                            Data.新建会话();
                        Data.处理接受输入(s);
                    }
                    else
                    {
                        using (new UseWaitCursor())
                        {
                            if (Data.FindTableByID(Data.当前会话) != Data.模式会话表)
                                Data.新建会话();
                            Data.模式frm.treeList待生长对象对.DataSource = null;
                            long t = System.DateTime.Now.Ticks;
                            Data.解析句子(s);
                            t = System.DateTime.Now.Ticks - t;
                            double r = t / 10000000.0;
                            Data.显示信息(r.ToString());

                            Data.输出对话信息(s, false);
                            Processor p = Processor.当前处理器;
                            if (翻译.Checked == false)
                                p.尝试对问题进行查询对命题进行推导();
                            else
                                p.进行翻译();
                        }
                    }
                    Data.检查新知识 = false;
                    设置添加新知识状态();
                }
            }
        }
        private void toolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == 翻译 && 翻译.Checked)
            {
                学习.Checked = false;
                toolStripButton4.Checked = false;
            }
            if (sender == 学习 && 学习.Checked)
            {
                翻译.Checked = false;
                toolStripButton4.Checked = false;
            }
            if (sender == toolStripButton4 && toolStripButton4.Checked)
            {
                学习.Checked = false;
                翻译.Checked = false;
            }
        }


		private void textBox3_TextChanged(object sender, EventArgs e)
		{
			if (textBox3.Text == "\r\n")
				textBox3.Clear();
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			Data.新建会话();
			textBox3.Focus();
		}

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Select(box.Text.Length, 0);
            box.ScrollToCaret();
        }

        private void 后台_CheckedChanged(object sender, EventArgs e)
        {
            TopMost=!后台.Checked;
        }

		private void comboBox解析_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			string 解析语言 = e.ClickedItem.Text;
			comboBox解析.Text = 解析语言;
			Data.模式frm.选择解析语言.EditValue = 解析语言;
		}

		private void comboBox生成_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			string 生成语言 = e.ClickedItem.Text;
			comboBox生成.Text = 生成语言;
			Data.模式frm.选择生成语言.EditValue = 生成语言;
		}

        private void toolStripButton动态输出_Click(object sender, EventArgs e)
        {
            Data.动态绑定至Form = toolStripButton动态输出.Checked;
        }

        private void 对话Form_Move(object sender, EventArgs e)
        {
            if (句子测试frm != null)
            {
                重新定位句子测试frm();
            }
        }
        public void 显示句子测试frm()
        {
            重新定位句子测试frm();
            if (句子测试frm != null && 句子测试frm.Visible==false)
                句子测试frm.Show(this);  
        }
        private void 重新定位句子测试frm()
        {
            //句子测试frm.Left = this.Left + this.Width;
            //句子测试frm.Top = this.Top;
            //句子测试frm.Height = this.Height;
        }
        private void 对话Form_Activated(object sender, EventArgs e)
        {
            if (句子测试frm != null && 句子测试frm.Visible)
            {
                this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width - 句子测试frm.Width) / 2;
                重新定位句子测试frm();
            }
        }
    }
}