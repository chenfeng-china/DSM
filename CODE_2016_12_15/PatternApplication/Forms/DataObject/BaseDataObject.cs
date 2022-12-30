using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    /// <summary>
    /// 公共基础类，包含Guid标识属性，并实现IComparable接口
    /// </summary>
    public class BaseDataObject : IComparable<BaseDataObject>, IEquatable<BaseDataObject>
    {
        /// <summary>
        /// 构造新对象
        /// </summary>
        /// <param name="isNew">是否全新构造</param>
        public BaseDataObject(bool isNew)
        {
            if (isNew == true)
                ID = Guid.NewGuid();
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public BaseDataObject()
        {}

        /// <summary>
        /// 对象Id
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 父对象Id
        /// </summary>
        public Guid ParentID { get; set; }
                
        /// <summary>
        /// 实现两个对象的比较，主要用于排序，查找
        /// </summary>
        /// <param name="other">要与之比较的对象</param>
        public int CompareTo(BaseDataObject other)
        {
            return this.ID.CompareTo(other.ID);
        }

        
        public bool Equals(BaseDataObject other)
        {
            if (other == null)
                return false;
            return this.ID.Equals(other.ID);
        }


        /// <summary>
        /// 作为Dictionary的key，必须重写GetHashCode方法
        /// </summary>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj is BaseDataObject)
                return this.Equals((BaseDataObject) obj);
            else
                return false;
        }

        /// <summary>
        /// 是否新添加
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否被更新
        /// </summary>
        public bool IsUpdated { get; set; }
    }
}
