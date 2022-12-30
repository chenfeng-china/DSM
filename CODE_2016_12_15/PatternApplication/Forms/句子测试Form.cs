using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using PatternApplication.DataObject;

namespace PatternApplication.Forms
{
    public partial class 句子测试Form : DevExpress.XtraEditors.XtraForm
    {
        private string txtFileName;
        private bool txtChanged = true;
        public 对话Form 对话frm;
        public 句子测试Form()
        {
            InitializeComponent();
        }
        public void init()
        {
            句子测试Form_Load(null,null);
        }
        private void 句子测试Form_Load(object sender, EventArgs e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            if (dir.EndsWith(@"\bin\Debug\") || dir.EndsWith(@"\bin\Release\"))
            {
                dir = Directory.GetParent(dir).Parent.Parent.FullName;
            }
            this.加载句子文本(dir + @"\句子测试.txt");
            txtChanged = false;
        }

        private void txt句子_DoubleClick(object sender, EventArgs e)
        {
            if (txt句子.TextLength>0)
            {
                int linestartPos=txt句子.GetFirstCharIndexOfCurrentLine();
                int lineNum = txt句子.GetLineFromCharIndex(linestartPos);
                string lineText = txt句子.Lines[lineNum];
                if (lineText.Trim().Length > 0)
                {
                    if (对话frm != null)
                        对话frm.开始解析句子(lineText);
                    else
                        Data.模式frm.测试句子(lineText);
                }
            }
        }

        private void 加载句子文本(string fileName)
        {
            txtFileName = fileName;
            if (File.Exists(fileName))
            {
                FileStream file = File.Open(fileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file, Encoding.Default);
                this.txt句子.Text = reader.ReadToEnd();
                reader.Close();
                file.Close();
            }
        }
        public void saveText()
        {
            if (!string.IsNullOrEmpty(txtFileName) && txtChanged)
            {
                FileStream file = File.Open(txtFileName, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(file, Encoding.Default);
                for (int i = 0; i < txt句子.Lines.Length; i++)
                {
                    writer.WriteLine(txt句子.Lines[i]);
                }
                txtChanged = false;
                writer.Close();
                file.Close();
            }
        }
        private void 句子测试Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveText();
            e.Cancel = true;
            Hide();
        }

        private void txt句子_TextChanged(object sender, EventArgs e)
        {
            if (!txtChanged)
                txtChanged = true;
        }
        private void txt句子_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(txt句子,e.Location);
            }

        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveText();
        }

        private void 重新加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            init();
        }

        private void 保存至服务器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Knowledge.save句子(txt句子.Lines);
        }

        private void 从服务器重新加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text=Knowledge.load句子();
            if (text != null)
                txt句子.Text = text;
        }
    }
}