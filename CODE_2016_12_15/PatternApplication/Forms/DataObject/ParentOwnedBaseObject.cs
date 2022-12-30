using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    /// <summary>
    /// 具有父子结构的数据基类
    /// </summary>
    public abstract class ParentOwnedBaseObject<T> : BaseObject<T> where T: IComparable, IComparable<T>, IEquatable<T>
    {
        private T parentID;

        public ParentOwnedBaseObject() { }

        public ParentOwnedBaseObject(T id, T parentID) : base(id)
        {
            this.parentID = parentID;
        }
        
        /// <summary>
        /// 父对象ID
        /// </summary>
        public T ParentID
        {
            get { return this.parentID; }
            set
            {
                if (!this.parentID.Equals(value))
                {
                    T oldValue = this.parentID;
                    this.parentID = value;
                    OnChanged("ParentID", oldValue, parentID);
                }
            }
        }
    }
}
