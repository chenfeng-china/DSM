namespace PatternApplication.Forms
{
	partial class 对话Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(对话Form));
            this.形式化语料串BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.学习 = new System.Windows.Forms.ToolStripButton();
            this.检查新知识 = new System.Windows.Forms.ToolStripButton();
            this.翻译 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.解释和推导 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.后台 = new System.Windows.Forms.ToolStripButton();
            this.简略回答 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.comboBox解析 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.comboBox生成 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripButton动态输出 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.形式化语料串BindingSource)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // 形式化语料串BindingSource
            // 
            this.形式化语料串BindingSource.DataMember = "形式化语料串";
            this.形式化语料串BindingSource.Sort = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.学习,
            this.检查新知识,
            this.翻译,
            this.toolStripButton4,
            this.解释和推导,
            this.toolStripButton5,
            this.后台,
            this.简略回答,
            this.toolStripLabel1,
            this.comboBox解析,
            this.toolStripLabel2,
            this.comboBox生成,
            this.toolStripButton动态输出,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(835, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 学习
            // 
            this.学习.Checked = true;
            this.学习.CheckOnClick = true;
            this.学习.CheckState = System.Windows.Forms.CheckState.Checked;
            this.学习.Image = ((System.Drawing.Image)(resources.GetObject("学习.Image")));
            this.学习.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.学习.Name = "学习";
            this.学习.Size = new System.Drawing.Size(88, 22);
            this.学习.Text = "学习和对话";
            // 
            // 检查新知识
            // 
            this.检查新知识.CheckOnClick = true;
            this.检查新知识.Image = ((System.Drawing.Image)(resources.GetObject("检查新知识.Image")));
            this.检查新知识.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.检查新知识.Name = "检查新知识";
            this.检查新知识.Size = new System.Drawing.Size(88, 22);
            this.检查新知识.Text = "检查新知识";
            // 
            // 翻译
            // 
            this.翻译.CheckOnClick = true;
            this.翻译.Image = ((System.Drawing.Image)(resources.GetObject("翻译.Image")));
            this.翻译.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.翻译.Name = "翻译";
            this.翻译.Size = new System.Drawing.Size(52, 22);
            this.翻译.Text = "翻译";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.CheckOnClick = true;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton4.Text = "双语对照学习";
            // 
            // 解释和推导
            // 
            this.解释和推导.CheckOnClick = true;
            this.解释和推导.Image = ((System.Drawing.Image)(resources.GetObject("解释和推导.Image")));
            this.解释和推导.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.解释和推导.Name = "解释和推导";
            this.解释和推导.Size = new System.Drawing.Size(88, 22);
            this.解释和推导.Text = "解释和推导";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(48, 22);
            this.toolStripButton5.Text = "新会话";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // 后台
            // 
            this.后台.CheckOnClick = true;
            this.后台.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.后台.Image = ((System.Drawing.Image)(resources.GetObject("后台.Image")));
            this.后台.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.后台.Name = "后台";
            this.后台.Size = new System.Drawing.Size(36, 22);
            this.后台.Text = "后台";
            this.后台.CheckedChanged += new System.EventHandler(this.后台_CheckedChanged);
            // 
            // 简略回答
            // 
            this.简略回答.CheckOnClick = true;
            this.简略回答.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.简略回答.Image = ((System.Drawing.Image)(resources.GetObject("简略回答.Image")));
            this.简略回答.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.简略回答.Name = "简略回答";
            this.简略回答.Size = new System.Drawing.Size(60, 22);
            this.简略回答.Text = "简略回答";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel1.Text = "人";
            // 
            // comboBox解析
            // 
            this.comboBox解析.AutoSize = false;
            this.comboBox解析.BackColor = System.Drawing.Color.LightBlue;
            this.comboBox解析.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.comboBox解析.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox解析.MergeIndex = 0;
            this.comboBox解析.Name = "comboBox解析";
            this.comboBox解析.Size = new System.Drawing.Size(45, 24);
            this.comboBox解析.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboBox解析.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.comboBox解析_DropDownItemClicked);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel2.Text = "机";
            // 
            // comboBox生成
            // 
            this.comboBox生成.AutoSize = false;
            this.comboBox生成.BackColor = System.Drawing.Color.LightBlue;
            this.comboBox生成.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.comboBox生成.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox生成.MergeIndex = 0;
            this.comboBox生成.Name = "comboBox生成";
            this.comboBox生成.Size = new System.Drawing.Size(45, 24);
            this.comboBox生成.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.comboBox生成.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.comboBox生成_DropDownItemClicked);
            // 
            // toolStripButton动态输出
            // 
            this.toolStripButton动态输出.Checked = true;
            this.toolStripButton动态输出.CheckOnClick = true;
            this.toolStripButton动态输出.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton动态输出.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton动态输出.Image")));
            this.toolStripButton动态输出.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton动态输出.Name = "toolStripButton动态输出";
            this.toolStripButton动态输出.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton动态输出.Text = "动态输出";
            this.toolStripButton动态输出.Click += new System.EventHandler(this.toolStripButton动态输出_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(835, 619);
            this.splitContainer1.SplitterDistance = 341;
            this.splitContainer1.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.BackColor = System.Drawing.Color.LightBlue;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.textBox1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(341, 619);
            this.textBox1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox3);
            this.splitContainer2.Size = new System.Drawing.Size(490, 619);
            this.splitContainer2.SplitterDistance = 394;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.AcceptsReturn = true;
            this.textBox2.BackColor = System.Drawing.Color.LightBlue;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.textBox2.ForeColor = System.Drawing.Color.MidnightBlue;
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(490, 394);
            this.textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            this.textBox3.AcceptsReturn = true;
            this.textBox3.BackColor = System.Drawing.Color.LightBlue;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.textBox3.Location = new System.Drawing.Point(0, 0);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3.Size = new System.Drawing.Size(490, 220);
            this.textBox3.TabIndex = 1;
            this.textBox3.WordWrap = false;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // 对话Form
            // 
            this.Appearance.BackColor = System.Drawing.Color.LightBlue;
            this.Appearance.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 644);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "对话Form";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.对话Form_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.对话Form_FormClosing);
            this.Load += new System.EventHandler(this.对话Form_Load);
            this.Move += new System.EventHandler(this.对话Form_Move);
            ((System.ComponentModel.ISupportInitialize)(this.形式化语料串BindingSource)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.BindingSource 形式化语料串BindingSource;
		public System.Windows.Forms.ToolStrip toolStrip1;
		public System.Windows.Forms.ToolStripButton 翻译;
		public System.Windows.Forms.ToolStripButton 学习;
		public System.Windows.Forms.ToolStripButton toolStripButton4;
		public System.Windows.Forms.SplitContainer splitContainer1;
		public System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.TextBox textBox1;
		public System.Windows.Forms.TextBox textBox2;
		public System.Windows.Forms.TextBox textBox3;
		public System.Windows.Forms.ToolStripButton toolStripButton5;
		public System.Windows.Forms.ToolStripButton 解释和推导;
        private System.Windows.Forms.ToolStripButton 后台;
        private System.Windows.Forms.ToolStripButton 简略回答;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripDropDownButton comboBox解析;
		private System.Windows.Forms.ToolStripDropDownButton comboBox生成;
        private System.Windows.Forms.ToolStripButton toolStripButton动态输出;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton 检查新知识;
	}
}