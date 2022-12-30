using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    /// <summary>
    /// 基本对象表，包含对象集合，
    /// </summary>
    public abstract class BaseTable<TKey, TValue> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey> where TValue : BaseObject<TKey> 
    {
        /// <summary>
        /// 表变更事件
        /// </summary>
        public event TableChangeEventHandle<TValue> TableChangeEvent;

        /// <summary>
        /// 表中对象有序集合
        /// </summary>
        private SortedList<TKey, TValue> 所有对象集合 = new SortedList<TKey, TValue>();

        /// <summary>
        /// 有序对象集合
        /// </summary>
        public BindingList<TValue> 对象集合
        {
            get
            {
                return new BindingList<TValue>(所有对象集合.Values);
            }
        }

        /// <summary>
        /// 存储被删除的对象
        /// </summary>
        public List<TKey> 删除集合 = new List<TKey>(); 
        
        /// <summary>
        /// 加载数据到有序对象集合
        /// </summary>
        protected abstract void LoadObjects();
     
        /// <summary>
        /// 添加新对象,添加新增对象到新增对象集合
        /// </summary>
        public void 新加对象(TValue 对象, bool isNew = true)
        {
            对象.IsNew = isNew;                
            所有对象集合.Add(对象.ID, 对象);
           
            对象.PropertyChanged += 对象_PropertyChanged;

            RaiseTableChangeEvent(new TableChangeEventArgs<TValue>(对象, TableChangeOption.AddNew));
            
        }

        private void 对象_PropertyChanged(BaseObject<TKey> sender, BaseObject<TKey>.PropertyChangedEventArgs args)
        {
            TValue 对象 = sender as TValue;
            if (对象 != null)
            {
                更新对象(对象, args);
            }
        }
        
        /// <summary>
        /// 更新对象，添加更新对象到更新对象集合
        /// </summary>
        public void 更新对象(TValue 对象, object otherInfo = null)
        {
            if (对象.IsNew != true)
                对象.IsUpdated = true;

            RaiseTableChangeEvent(new TableChangeEventArgs<TValue>(对象, TableChangeOption.Updated) { OtherInfo = otherInfo });
        }

        /// <summary>
        /// 删除对象，添加删除对象到删除对象集合
        /// </summary>
        /// <param name="对象">被删除对象</param>
        public void 删除对象(TValue 对象)
        {
            对象.PropertyChanged -= 对象_PropertyChanged;
            RaiseTableChangeEvent(new TableChangeEventArgs<TValue>(对象, TableChangeOption.Deleted));

            所有对象集合.Remove(对象.ID);            

            if (对象.IsNew == false)
            {
                删除集合.Add(对象.ID);
            }            
        }
        
        /// <summary>
        /// 提交对数据做出的修改, 在UI执行保存或软件推出时进行
        /// </summary>
        public void CommitChanges()
        {
            // 1. 提交删除对象
            CommitDeletedObject();

            // 2. 提交新增对象
            CommitNewObject();

            // 3. 提交更改对象
            CommitUpdatedObject();
        }

        /// <summary>
        /// 提交删除对象
        /// </summary>
        protected abstract void CommitDeletedObject();

        /// <summary>
        /// 提交更新对象
        /// </summary>
        protected abstract void CommitUpdatedObject();

        /// <summary>
        /// 提交新增对象
        /// </summary>
        protected abstract void CommitNewObject();

        /// <summary>
        /// 提交新增对象
        /// </summary>
        public void clear()
        {
            所有对象集合.Clear();
            删除集合.Clear();
        }
        /// <summary>
        /// 根据ID从集合中查找对象
        /// </summary>
        /// <param name="id">对象Id</param>
        /// <returns>结果对象</returns>
        public TValue FindById(TKey id)
        {
            TValue result = null;
            所有对象集合.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// 判断是否存在指定ID的对象
        /// </summary>
        public bool Exist(TKey id)
        {
            return this.所有对象集合.ContainsKey(id);
        }
        
        private void RaiseTableChangeEvent(TableChangeEventArgs<TValue> args)
        {
            if (TableChangeEvent != null)
            {
                TableChangeEvent(this, args);
            }
        }


        /// <summary>
        /// 表变更事件处理委托
        /// </summary>
        public delegate void TableChangeEventHandle<T>(object sender, TableChangeEventArgs<T> args);

        /// <summary>
        /// 表变更事件参数
        /// </summary>
        public class TableChangeEventArgs<T> : EventArgs 
        {
            private T source;
            private TableChangeOption changeOption;

            public TableChangeEventArgs(T source, TableChangeOption changeOption)
            {
                this.source = source;
                this.changeOption = changeOption;
            }

            public T Source
            {
                get { return this.source; }
            }

            public TableChangeOption ChangeOption
            {
                get { return this.changeOption; }
            }

            /// <summary>
            /// 引起表变更的其他信息
            /// </summary>
            public object OtherInfo { get; set; }
        }

        /// <summary>
        /// 变变更类型, 新添加对象， 更新现有对象， 删除现有对象
        /// </summary>
        public enum TableChangeOption
        {
            AddNew,     // 添加新对象
            Updated,    // 更新对象
            Deleted     // 删除对象
        }
    }
    
    
}
