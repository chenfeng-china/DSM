using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    /// <summary>
    /// 所有数据类的基类, 该类型有唯一标识符ID, 类型为T
    /// </summary>
    /// <typeparam name="T">唯一标识属性ID类型, 该类型必须实现IComparable<T>, IEquatable<T>, IComparable接口</typeparam>
    public abstract class BaseObject<T> : IComparable<BaseObject<T>>, IEquatable<BaseObject<T>>, IComparable where T : IComparable<T>, IEquatable<T>, IComparable
    {
        private T id;

        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler<T> PropertyChanged;

        public BaseObject()
        { 
            
        }
        
        public BaseObject(T id)
        {
            this.id = id;
        }


        /// <summary>
        /// 获取或设置对象ID
        /// </summary>
        public T ID
        {
            get { return this.id; }
            set
            {
                if (!this.id.Equals(value))
                {
                    T oldValue = id;
                    id = value;
                    OnChanged("ID", oldValue, id);
                }
            }
        }

        /// <summary>
        /// 是否新添加
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 是否被更新
        /// </summary>
        public bool IsUpdated { get; set; }
        
        
        public void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName, oldValue, newValue));
            }
        }

        /// <summary>
        /// 实现两个对象的比较，主要用于排序，查找
        /// </summary>
        /// <param name="other">要与之比较的对象</param>
        public int CompareTo(BaseObject<T> other)
        {
            return this.ID.CompareTo(other.ID);
        }

        /// <summary>
        /// 主要用于泛型集合对象，如Dictionary<TKey, TValue>, List<T>和LinkedList<T>,
        /// 用于测试相等性的方法，例如： Contains, IndexOf, LastIndexOf和Remove
        /// </summary>
        public bool Equals(BaseObject<T> other)
        {
            return this.ID.Equals(other.ID);
        }

        /// <summary>
        /// 如果实现IEquatable<T>接口， 则应重写Object.Equals(Object)和GetHashCode方法， 
        /// 以使其与IEquatable<T>.Equals(T)方法行为保持一致
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as BaseObject<T>);
        }

        /// <summary>
        /// 重写Equals方法， 应重写GetHashCode方法，确保相等的对象返回相同的哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        
        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as BaseObject<T>);
        }


        /// <summary>
        /// 属性变更事件处理委托
        /// </summary>
        /// <param name="sender">属性发生变更的对象</param>
        /// <param name="args">变更参数</param>
        public delegate void PropertyChangedEventHandler<T>(BaseObject<T> sender, PropertyChangedEventArgs args) where T : IComparable, IComparable<T>, IEquatable<T>;


        /// <summary>
        /// 属性变更事件参数
        /// </summary>
        public class PropertyChangedEventArgs : EventArgs
        {
            private string propertyName;
            private object oldValue;
            private object newValue;

            public PropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
            {
                this.propertyName = propertyName;
                this.oldValue = oldValue;
                this.newValue = newValue;
            }

            /// <summary>
            /// 属性名
            /// </summary>
            public String PropertyName { get { return this.propertyName; } }
            /// <summary>
            /// 变更前属性值
            /// </summary>
            public object OldValue { get { return this.oldValue; } }
            /// <summary>
            /// 变更后属性值（当前值）
            /// </summary>
            public object NewValue { get { return this.newValue; } }
        }
    }
    
}
