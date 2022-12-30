namespace PatternApplication.Forms
{
    partial class 句子测试Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txt句子 = new System.Windows.Forms.RichTextBox();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.重新加载ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.保存至服务器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.从服务器重新加载ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txt句子);
            this.splitContainer1.Size = new System.Drawing.Size(387, 644);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 0;
            // 
            // txt句子
            // 
            this.txt句子.BackColor = System.Drawing.Color.LightBlue;
            this.txt句子.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt句子.Location = new System.Drawing.Point(0, 0);
            this.txt句子.Name = "txt句子";
            this.txt句子.Size = new System.Drawing.Size(387, 644);
            this.txt句子.TabIndex = 0;
            this.txt句子.Text = "";
            this.txt句子.WordWrap = false;
            this.txt句子.TextChanged += new System.EventHandler(this.txt句子_TextChanged);
            this.txt句子.DoubleClick += new System.EventHandler(this.txt句子_DoubleClick);
            this.txt句子.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt句子_MouseDown);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
            // 
            // 重新加载ToolStripMenuItem
            // 
            this.重新加载ToolStripMenuItem.Name = "重新加载ToolStripMenuItem";
            this.重新加载ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.重新加载ToolStripMenuItem.Text = "重新加载";
            this.重新加载ToolStripMenuItem.Click += new System.EventHandler(this.重新加载ToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存ToolStripMenuItem,
            this.重新加载ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.保存至服务器ToolStripMenuItem,
            this.从服务器重新加载ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 120);
            // 
            // 保存至服务器ToolStripMenuItem
            // 
            this.保存至服务器ToolStripMenuItem.Name = "保存至服务器ToolStripMenuItem";
            this.保存至服务器ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.保存至服务器ToolStripMenuItem.Text = "上传至服务器";
            this.保存至服务器ToolStripMenuItem.Click += new System.EventHandler(this.保存至服务器ToolStripMenuItem_Click);
            // 
            // 从服务器重新加载ToolStripMenuItem
            // 
            this.从服务器重新加载ToolStripMenuItem.Name = "从服务器重新加载ToolStripMenuItem";
            this.从服务器重新加载ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.从服务器重新加载ToolStripMenuItem.Text = "从服务器加载";
            this.从服务器重新加载ToolStripMenuItem.Click += new System.EventHandler(this.从服务器重新加载ToolStripMenuItem_Click);
            // 
            // 句子测试Form
            // 
            this.Appearance.BackColor = System.Drawing.Color.LightBlue;
            this.Appearance.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 644);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "句子测试Form";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "句子测试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.句子测试Form_FormClosing);
            this.Load += new System.EventHandler(this.句子测试Form_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox txt句子;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 重新加载ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 保存至服务器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 从服务器重新加载ToolStripMenuItem;
    }
}