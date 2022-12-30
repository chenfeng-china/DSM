using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    public class 形式化语料串 : IComparable<形式化语料串>
    {
        public bool 仅示例 { get; set; }

        public Guid ObjectID { get; set; }

        //public int 序号 { get; set; }

        public Guid 所属抽象串 { get; set; }

        public string 字符串 { get; set; }

        public bool 是抽象串 { get; set; }

        public int 语言 { get; set; }

        //public bool 是算子 { get; set; }

        //public int 算子Value { get; set; }

        //public System.Guid 同义词 { get; set; }

        //public System.Guid 使用规则 { get; set; }

        //public short 名词和形容词义项数目 { get; set; }

        //public short 动词义项数目 { get; set; }

        //public short 副词义项数目 { get; set; }

        //public short 其它义项数目 { get; set; }

        //public string 说明 { get; set; }

        public int 语义数 { get; set; }

        public int 匹配次数 { get; set; }

        public int 优先级 { get; set; }

        /// <summary>
        /// 是否来源于核心库
        /// </summary>
        public bool IsCore { get; set; }

        public int CompareTo(形式化语料串 other)
        {
            return  Data.Unicode比较(this.字符串, other.字符串);     
            //return this.字符串.CompareTo(other.字符串);
        }
    }
}
