using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace PatternApplication.Forms
{
    public partial class ParamsForm : Form
    {
        private List<KeyValuePair<int, string>> 字典_方向 = new List<KeyValuePair<int, string>>()
        {
            new KeyValuePair<int,string>(1,"正向"),
            new KeyValuePair<int,string>(2,"反向"),
            new KeyValuePair<int,string>(3,"双向")
        };
        private List<KeyValuePair<int, string>> 字典_扩展位码 = new List<KeyValuePair<int, string>>()
        {
            new KeyValuePair<int,string>(1,"A端不再向后派生"),
            new KeyValuePair<int,string>(2,"B端不再向后派生"),
            new KeyValuePair<int,string>(4,"抑制基关联"),
            new KeyValuePair<int,string>(8,"A端不向前继承"),
            new KeyValuePair<int,string>(16,"B端不向前继承"),
            new KeyValuePair<int,string>(32,"A端和B端必须紧密相邻")
        };
        private List<KeyValuePair<int, string>> 字典_词性 = new List<KeyValuePair<int, string>>()
        {
            new KeyValuePair<int,string>(1,"包含宾语"),
            new KeyValuePair<int,string>(2,"离合式"),
            new KeyValuePair<int,string>(4,"小品词组"),
            new KeyValuePair<int,string>(8,"名词后缀"),
            new KeyValuePair<int,string>(16,"简称"),
        };
        private 参数字段 paramField=new 参数字段();
        public ParamsForm()
        {
            InitializeComponent();
            field方向.Properties.DataSource = 字典_方向;
            //field词性扩展.Properties.DataSource = 字典_词性;
            //field扩展位码.Properties.DataSource = 字典_扩展位码;

            foreach (var obj in 字典_词性)
            {
                field词性扩展.Properties.Items.Add(obj.Key, obj.Value, CheckState.Unchecked, true);
            }
            foreach (var obj in 字典_扩展位码)
            {
                field扩展位码.Properties.Items.Add(obj.Key, obj.Value, CheckState.Unchecked, true);
            }
        }
        private void InitParams()
        {
            if (this.Tag != null && this.Tag is ButtonEdit)
            {
                //读取并初始化显示各参数值
                ButtonEdit btn = (ButtonEdit)this.Tag;
                paramField.ReadString(btn.Text);
                foreach (var obj in 字典_方向)
                {
                    if (obj.Key == paramField.方向)
                        field方向.EditValue = obj;
                }
                
                this.fieldB对A的创建性.Text = paramField.B对A的创建性.ToString();
                this.fieldB对A的关键性.Text = paramField.B对A的关键性.ToString();
                this.field正误分.Text = paramField.正误分.ToString();
                this.field概率分.Text = paramField.概率分.ToString();
                this.field具体化程度分.Text = paramField.具体化程度分.ToString();
                this.field靠外层级分.Text = paramField.在左端时靠外层级分.ToString();
                this.fieldB端重复数.Text = paramField.B端重复数.ToString();
                this.fieldAa.Text = paramField.Aa.ToString();
                this.fieldAb.Text = paramField.Ab.ToString();
                this.fieldBa.Text = paramField.Ba.ToString();
                this.fieldBb.Text = paramField.Bb.ToString();
                this.field联a.Text = paramField.联a.ToString();
                this.field联b.Text = paramField.联b.ToString();
                this.field说话方.Text = paramField.说话方.ToString();
                this.field层级值.Text = paramField.层级值.ToString();

                setCheckedComboxValue(field扩展位码, paramField.扩展位码);
                setCheckedComboxValue(field词性扩展, paramField.词性扩展);
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.Tag != null && this.Tag is ButtonEdit)
            {
                ButtonEdit btn = (ButtonEdit)this.Tag;
                if (this.field方向.EditValue != null)
                {
                    paramField.方向 = ((KeyValuePair<int, string>)this.field方向.EditValue).Key;
                }
                paramField.B对A的创建性=int.Parse(this.fieldB对A的创建性.Text);
                paramField.B对A的关键性 = int.Parse(this.fieldB对A的关键性.Text);
                paramField.概率分=int.Parse(this.field概率分.Text);
                paramField.具体化程度分=int.Parse(this.field具体化程度分.Text);
                paramField.在左端时靠外层级分=int.Parse(this.field靠外层级分.Text);
                paramField.B端重复数 = int.Parse(this.fieldB端重复数.Text);
                paramField.Aa=int.Parse(this.fieldAa.Text);
                paramField.Ab=int.Parse(this.fieldAb.Text);
                paramField.Ba=int.Parse(this.fieldBa.Text);
                paramField.Bb=int.Parse(this.fieldBb.Text);
                paramField.联a=int.Parse(this.field联a.Text);
                paramField.联b=int.Parse(this.field联b.Text);
                paramField.说话方=int.Parse(this.field说话方.Text);
                paramField.层级值=int.Parse(this.field层级值.Text);

                paramField.扩展位码 = getCheckedComboxValue(field扩展位码);
                paramField.词性扩展 = getCheckedComboxValue(this.field词性扩展);

                btn.Text = paramField.ToString();
            }
            this.Close();
        }
        private int getCheckedComboxValue(CheckedComboBoxEdit cboEdit)
        {
            int r = 0;
            foreach (CheckedListBoxItem obj in cboEdit.Properties.Items)
            {
                if (obj.CheckState == CheckState.Checked)
                    r += (int)obj.Value;
            }
            return r;
        }
        private void setCheckedComboxValue(CheckedComboBoxEdit cboEdit, int value)
        {
            foreach (CheckedListBoxItem obj in cboEdit.Properties.Items)
            {
                if (((int)obj.Value & value)>0)
                    obj.CheckState = CheckState.Checked;
            }
        }

        private void ParamsForm_Shown(object sender, EventArgs e)
        {
            InitParams();
        }
    }
}
