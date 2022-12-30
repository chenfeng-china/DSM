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
    public abstract class BaseObjectTable<T> where T : BaseDataObject
    {
        /// <summary>
        /// 表中对象有序集合
        /// </summary>
        protected SortedList<Guid, T> 所有对象集合 = new SortedList<Guid, T>();

        /// <summary>
        /// 有序对象集合
        /// </summary>
        public BindingList<T> 对象集合
        {
            get
            {
                return new BindingList<T>(所有对象集合.Values);
            }
        }

        /// <summary>
        /// 存储被删除的对象
        /// </summary>
        public List<Guid> 删除集合 = new List<Guid>(); 
        
        /// <summary>
        /// 加载数据到有序对象集合
        /// </summary>
        protected abstract void LoadObjects();
     
        /// <summary>
        /// 添加新对象,添加新增对象到新增对象集合
        /// </summary>
        /// <param name="对象">新加对象</param>
        /// <param name="是否保存到数据库">是否最终保存到数据库，对于数字，日期等，不需要</param>
        public void 新加对象(T 对象, bool 是否保存到数据库 = true)
        {
            if (是否保存到数据库 == true)
            {
                对象.IsNew = true;
            }
                
            所有对象集合.Add(对象.ID, 对象);

            RaiseTableChangeEvent(new TableChangeEventArgs(TableChangeAction.Add, 对象.ID));
        }

        /// <summary>
        /// 更新对象，添加更新对象到更新对象集合
        /// </summary>
        public void 更新对象(T 对象)
        {
            if (对象.IsNew != true)
                对象.IsUpdated = true;

            RaiseTableChangeEvent(new TableChangeEventArgs(TableChangeAction.Update, 对象.ID));
        }

        /// <summary>
        /// 删除对象，添加删除对象到删除对象集合
        /// </summary>
        /// <param name="对象">被删除对象</param>
        public void 删除对象(T 对象)
        {
            所有对象集合.Remove(对象.ID);
            if (对象.IsNew == false)
                删除集合.Add(对象.ID);

            RaiseTableChangeEvent(new TableChangeEventArgs(TableChangeAction.Delete, 对象.ID));
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
        /// 根据ID从集合中查找对象
        /// </summary>
        /// <param name="id">对象Id</param>
        /// <returns>结果对象</returns>
        public T FindById(Guid id)
        {
            T result = null;
            所有对象集合.TryGetValue(id, out result);
            return result;
        }

        public bool Exist(Guid id)
        {
            return this.所有对象集合.ContainsKey(id);
        }


        #region TableChangeEvent

        public class TableChangeEventArgs : EventArgs
        {
            public TableChangeAction ChangeOption;
            public Guid ChangeObject;

            public TableChangeEventArgs(TableChangeAction action, Guid obj)
            {
                this.ChangeObject = obj;
                this.ChangeOption = action;
            }
        }

        public delegate void TableChangeEventHandle(object sender, TableChangeEventArgs args);

        public event TableChangeEventHandle TableChangeEvent;

        protected void RaiseTableChangeEvent(TableChangeEventArgs args)
        {
            if (TableChangeEvent != null)
            {
                TableChangeEvent(this, args);
            }
        }


        public enum TableChangeAction
        { 
            Add,
            Update, 
            Delete
        }


        #endregion

    }
}
