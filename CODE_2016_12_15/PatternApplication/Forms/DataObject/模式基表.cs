using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PatternApplication.DataObject
{
    public class 模式基表 : BaseTable<Guid, 模式>
    {
        public readonly string TableName;
        public readonly string ConnectionString;

        public 模式基表(string tableName, string connectionString = null)
        {
            this.TableName = tableName;
            if (string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;                
            }
            else
            {
                ConnectionString = connectionString;
            }
            if (!string.IsNullOrEmpty(tableName))
                LoadObjects();
        }
        public void reLoad()
        {
            this.clear();
            if (!string.IsNullOrEmpty(this.TableName))
                this.LoadObjects();
        }
        //根据表名，获取应该使用的数据库连接(用于模式表在SQL库时)
        private IDbConnection getConnectionByTableName(string tableName)
        {
            return new OleDbConnection(ConnectionString);     
        }
        /// <summary>
        /// 加载数据到有序集合
        /// </summary>
        protected override void LoadObjects()
        {
            using (
                IDbConnection conn = getConnectionByTableName(this.TableName)
                  )
            {
                try
                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT ID, ParentID, 序号, 语言, 形式, 源记录, A端, 连接, B端, that根, 的, 显隐, 语言角色, C端, 通用分类, 说明, 参数集合, " +
                        "成立度, 实例数, Aα, Aβ, Bα, Bβ, 联α, 联β, 说话时间, 说话地点, 全等引用计数, 级别, 认知年龄, 附加信息, 关系距离, " +
                        "语境树, 层级, 风格, 打分 FROM " + TableName;
                    command.CommandType = CommandType.Text;

                    conn.Open();

                    IDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int index = reader.GetOrdinal("ID");
                        Guid _ID = reader.GetGuid(index);
                        index = reader.GetOrdinal("ParentID");
                        Guid _ParentID = reader.GetGuid(index);
                        index = reader.GetOrdinal("序号");
                        int _序号 = reader.GetInt32(index);
                        index = reader.GetOrdinal("语言");
                        int _语言 = reader.GetInt32(index);
                        index = reader.GetOrdinal("形式");
                        string _形式 = reader.GetString(index);
                        index = reader.GetOrdinal("源记录");
                        Guid _源记录 = reader.GetGuid(index);
                        index = reader.GetOrdinal("A端");
                        Guid _A端 = reader.GetGuid(index);
                        index = reader.GetOrdinal("连接");
                        Guid _连接 = reader.GetGuid(index);
                        index = reader.GetOrdinal("B端");
                        Guid _B端 = reader.GetGuid(index);
                        index = reader.GetOrdinal("that根");
                        int _That根 = reader.GetInt32(index);
                        index = reader.GetOrdinal("的");
                        int _的 = reader.GetInt32(index);
                        index = reader.GetOrdinal("显隐");
                        short _显隐 = reader.GetInt16(index);
                        index = reader.GetOrdinal("语言角色");
                        int _语言角色 = reader.GetInt32(index);
                        index = reader.GetOrdinal("C端");
                        Guid _C端 = reader.GetGuid(index);
                        index = reader.GetOrdinal("通用分类");
                        short _通用分类 = reader.GetInt16(index);
                        index = reader.GetOrdinal("说明");
                        string _说明 = reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
                        index = reader.GetOrdinal("参数集合");
                        string _参数集合 = reader.GetString(index);
                        index = reader.GetOrdinal("成立度");
                        float _成立度 = reader.GetFloat(index);
                        index = reader.GetOrdinal("实例数");
                        short _实例数 = reader.GetInt16(index);
                        index = reader.GetOrdinal("Aα");
                        float _Aα = reader.GetFloat(index);
                        index = reader.GetOrdinal("Aβ");
                        float _Aβ = reader.GetFloat(index);
                        index = reader.GetOrdinal("Bα");
                        float _Bα = reader.GetFloat(index);
                        index = reader.GetOrdinal("Bβ");
                        float _Bβ = reader.GetFloat(index);
                        index = reader.GetOrdinal("联α");
                        float _联α = reader.GetFloat(index);
                        index = reader.GetOrdinal("联β");
                        float _联β = reader.GetFloat(index);
                        index = reader.GetOrdinal("说话时间");
                        Guid _说话时间 = reader.GetGuid(index);
                        index = reader.GetOrdinal("说话地点");
                        Guid _说话地点 = reader.GetGuid(index);
                        index = reader.GetOrdinal("全等引用计数");
                        int _全等引用计数 = reader.GetInt32(index);
                        index = reader.GetOrdinal("级别");
                        int _级别 = reader.GetInt32(index);
                        index = reader.GetOrdinal("认知年龄");
                        int _认知年龄 = reader.GetInt32(index);
                        index = reader.GetOrdinal("附加信息");
                        int _附加信息 = reader.GetInt32(index);
                        index = reader.GetOrdinal("关系距离");
                        int _关系距离 = reader.GetInt32(index);
                        index = reader.GetOrdinal("语境树");
                        int _语境树 = reader.GetInt32(index);
                        index = reader.GetOrdinal("层级");
                        byte _层级 = reader.GetByte(index);
                        index = reader.GetOrdinal("风格");
                        string _风格 = reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
                        index = reader.GetOrdinal("打分");
                        string _打分 = reader.IsDBNull(index) ? string.Empty : reader.GetString(index);


                        模式 item = new 模式(_ID, _ParentID, _序号, _语言, _形式, _源记录, _A端, _连接, _B端, _That根, _的, _显隐, _语言角色, _C端, _通用分类, _说明,
                            _参数集合, _成立度, _实例数, _Aα, _Aβ, _Bα, _Bβ, _联α, _联β, _说话时间, _说话地点, _全等引用计数, _级别, _认知年龄, _附加信息, _关系距离,
                            _语境树, _层级, _风格, _打分);

                        新加对象(item, false);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // TODO: 处理异常
                }
            }
        }

        /// <summary>
        /// 提交删除对象
        /// </summary>
        protected override void CommitDeletedObject()
        {
            bool isSuccess = false;
            using (
                IDbConnection conn = getConnectionByTableName(this.TableName)
                  )
            {
                IDbTransaction transaction = null;
                try
                {
                    conn.Open();

                    transaction = conn.BeginTransaction();

                    foreach (Guid id in 删除集合)
                    {
                        if (conn is SqlConnection)
                        {
                            using (SqlCommand command = (SqlCommand)conn.CreateCommand())
                            {
                                command.Transaction = (SqlTransaction)transaction;
                                command.CommandText = "Delete from " + TableName + " where ID = @p1";
                                command.Parameters.Add("@p1", SqlDbType.UniqueIdentifier).Value = id;
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (OleDbCommand command = (OleDbCommand)conn.CreateCommand())
                            {
                                command.Transaction = (OleDbTransaction)transaction;
                                command.CommandText = "Delete from " + TableName + " where ID = ?";
                                command.Parameters.Add("@p1", OleDbType.Guid).Value = id;
                                command.ExecuteNonQuery();
                            }
                        }

                    }

                    transaction.Commit();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    // TODO: 处理异常
                }
            }
            if (isSuccess)
            {
                删除集合.Clear();
            }
        }

        /// <summary>
        /// 提交更新对象
        /// </summary>
        protected override void CommitUpdatedObject()
        {
            var 更新对象集 = 对象集合.Where(r => r.IsUpdated == true).ToList();
            bool isSuccess = false;

            using (
                IDbConnection conn = getConnectionByTableName(this.TableName)
                  )
            {
                IDbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    foreach (模式 item in 更新对象集)
                    {
                        if (conn is SqlConnection)
                        {
                            using (SqlCommand command = (SqlCommand)conn.CreateCommand())
                            {
                                command.Transaction = (SqlTransaction)transaction;
                                command.CommandText = "update " + TableName + " set " +
                                                  "ParentID=@p1, " +
                                                  "序号=@p2, " +
                                                  "语言=@p3, " +
                                                  "形式=@p4, " +
                                                  "源记录=@p5, " +
                                                  "A端=@p6, " +
                                                  "连接=@p7, " +
                                                  "B端=@p8, " +
                                                  "that根=@p9, " +
                                                  "的=@p10, " +
                                                  "显隐=@p11, " +
                                                  "语言角色=@p12, " +
                                                  "C端=@p13, " +
                                                  "通用分类=@p14, " +
                                                  "说明=@p15, " +
                                                  "参数集合=@p16, " +
                                                  "成立度=@p17, " +
                                                  "实例数=@p18, " +
                                                  "Aα=@p19, " +
                                                  "Aβ=@p20, " +
                                                  "Bα=@p21, " +
                                                  "Bβ=@p22, " +
                                                  "联α=@p23, " +
                                                  "联β=@p24, " +
                                                  "说话时间=@p25, " +
                                                  "说话地点=@p26, " +
                                                  "全等引用计数=@p27, " +
                                                  "级别=@p28, " +
                                                  "认知年龄=@p29, " +
                                                  "附加信息=@p30, " +
                                                  "关系距离=@p31, " +
                                                  "语境树=@p32, " +
                                                  "层级=@p33, " +
                                                  "风格=@p34, " +
                                                  "打分=@p35 " +
                                                  "where ID = @p36";
                                command.Parameters.Add("@p1", SqlDbType.UniqueIdentifier).Value = item.ParentID;
                                command.Parameters.Add("@p2", SqlDbType.Int).Value = item.序号;
                                command.Parameters.Add("@p3", SqlDbType.Int).Value = item.语言;
                                command.Parameters.Add("@p4", SqlDbType.NVarChar).Value = item.形式;
                                command.Parameters.Add("@p5", SqlDbType.UniqueIdentifier).Value = item.源记录;
                                command.Parameters.Add("@p6", SqlDbType.UniqueIdentifier).Value = item.A端;
                                command.Parameters.Add("@p7", SqlDbType.UniqueIdentifier).Value = item.连接;
                                command.Parameters.Add("@p8", SqlDbType.UniqueIdentifier).Value = item.B端;
                                command.Parameters.Add("@p9", SqlDbType.Int).Value = item.That根;
                                command.Parameters.Add("@p10",SqlDbType.Int).Value = item.的;
                                command.Parameters.Add("@p11", SqlDbType.SmallInt).Value = item.显隐;
                                command.Parameters.Add("@p12", SqlDbType.Int).Value = item.语言角色;
                                command.Parameters.Add("@p13", SqlDbType.UniqueIdentifier).Value = item.C端;
                                command.Parameters.Add("@p14", SqlDbType.SmallInt).Value = item.通用分类;
                                command.Parameters.Add("@p15", SqlDbType.NVarChar).Value = item.说明;
                                command.Parameters.Add("@p16", SqlDbType.NVarChar).Value = item.参数集合;
                                command.Parameters.Add("@p17", SqlDbType.Real).Value = item.成立度;
                                command.Parameters.Add("@p18", SqlDbType.SmallInt).Value = item.实例数;
                                command.Parameters.Add("@p19", SqlDbType.Real).Value = item.Aα;
                                command.Parameters.Add("@p20", SqlDbType.Real).Value = item.Aβ;
                                command.Parameters.Add("@p21", SqlDbType.Real).Value = item.Bα;
                                command.Parameters.Add("@p22", SqlDbType.Real).Value = item.Bβ;
                                command.Parameters.Add("@p23", SqlDbType.Real).Value = item.联α;
                                command.Parameters.Add("@p24", SqlDbType.Real).Value = item.联β;
                                command.Parameters.Add("@p25", SqlDbType.UniqueIdentifier).Value = item.说话时间;
                                command.Parameters.Add("@p26", SqlDbType.UniqueIdentifier).Value = item.说话地点;
                                command.Parameters.Add("@p27", SqlDbType.Int).Value = item.全等引用计数;
                                command.Parameters.Add("@p28", SqlDbType.Int).Value = item.级别;
                                command.Parameters.Add("@p29", SqlDbType.Int).Value = item.认知年龄;
                                command.Parameters.Add("@p30", SqlDbType.Int).Value = item.附加信息;
                                command.Parameters.Add("@p31", SqlDbType.Int).Value = item.关系距离;
                                command.Parameters.Add("@p32", SqlDbType.Int).Value = item.语境树;
                                command.Parameters.Add("@p33", SqlDbType.TinyInt).Value = item.层级;
                                command.Parameters.Add("@p34", SqlDbType.NVarChar).Value = item.风格;
                                command.Parameters.Add("@p35", SqlDbType.NVarChar).Value = item.打分;
                                command.Parameters.Add("@p36", SqlDbType.UniqueIdentifier).Value = item.ID;

                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (OleDbCommand command = (OleDbCommand)conn.CreateCommand())
                            {
                                command.Transaction = (OleDbTransaction)transaction;
                                command.CommandText = "update " + TableName + " set " +
                                                  "ParentID=?, " +
                                                  "序号=?, " +
                                                  "语言=?, " +
                                                  "形式=?, " +
                                                  "源记录=?, " +
                                                  "A端=?, " +
                                                  "连接=?, " +
                                                  "B端=?, " +
                                                  "that根=?, " +
                                                  "的=?, " +
                                                  "显隐=?, " +
                                                  "语言角色=?, " +
                                                  "C端=?, " +
                                                  "通用分类=?, " +
                                                  "说明=?, " +
                                                  "参数集合=?, " +
                                                  "成立度=?, " +
                                                  "实例数=?, " +
                                                  "Aα=?, " +
                                                  "Aβ=?, " +
                                                  "Bα=?, " +
                                                  "Bβ=?, " +
                                                  "联α=?, " +
                                                  "联β=?, " +
                                                  "说话时间=?, " +
                                                  "说话地点=?, " +
                                                  "全等引用计数=?, " +
                                                  "级别=?, " +
                                                  "认知年龄=?, " +
                                                  "附加信息=?, " +
                                                  "关系距离=?, " +
                                                  "语境树=?, " +
                                                  "层级=?, " +
                                                  "风格=?, " +
                                                  "打分=? " +
                                                  "where ID = ?";
                                command.Parameters.Add("@p1", OleDbType.Guid).Value = item.ParentID;
                                command.Parameters.Add("@p2", OleDbType.Integer).Value = item.序号;
                                command.Parameters.Add("@p3", OleDbType.Integer).Value = item.语言;
                                command.Parameters.Add("@p4", OleDbType.VarWChar).Value = item.形式;
                                command.Parameters.Add("@p5", OleDbType.Guid).Value = item.源记录;
                                command.Parameters.Add("@p6", OleDbType.Guid).Value = item.A端;
                                command.Parameters.Add("@p7", OleDbType.Guid).Value = item.连接;
                                command.Parameters.Add("@p8", OleDbType.Guid).Value = item.B端;
                                command.Parameters.Add("@p9", OleDbType.Integer).Value = item.That根;
                                command.Parameters.Add("@p10", OleDbType.Integer).Value = item.的;
                                command.Parameters.Add("@p11", OleDbType.SmallInt).Value = item.显隐;
                                command.Parameters.Add("@p12", OleDbType.Integer).Value = item.语言角色;
                                command.Parameters.Add("@p13", OleDbType.Guid).Value = item.C端;
                                command.Parameters.Add("@p14", OleDbType.SmallInt).Value = item.通用分类;
                                command.Parameters.Add("@p15", OleDbType.VarWChar).Value = item.说明;
                                command.Parameters.Add("@p16", OleDbType.VarWChar).Value = item.参数集合;
                                command.Parameters.Add("@p17", OleDbType.Double).Value = item.成立度;
                                command.Parameters.Add("@p18", OleDbType.SmallInt).Value = item.实例数;
                                command.Parameters.Add("@p19", OleDbType.Double).Value = item.Aα;
                                command.Parameters.Add("@p20", OleDbType.Double).Value = item.Aβ;
                                command.Parameters.Add("@p21", OleDbType.Double).Value = item.Bα;
                                command.Parameters.Add("@p22", OleDbType.Double).Value = item.Bβ;
                                command.Parameters.Add("@p23", OleDbType.Double).Value = item.联α;
                                command.Parameters.Add("@p24", OleDbType.Double).Value = item.联β;
                                command.Parameters.Add("@p25", OleDbType.Guid).Value = item.说话时间;
                                command.Parameters.Add("@p26", OleDbType.Guid).Value = item.说话地点;
                                command.Parameters.Add("@p27", OleDbType.Integer).Value = item.全等引用计数;
                                command.Parameters.Add("@p28", OleDbType.Integer).Value = item.级别;
                                command.Parameters.Add("@p29", OleDbType.Integer).Value = item.认知年龄;
                                command.Parameters.Add("@p30", OleDbType.Integer).Value = item.附加信息;
                                command.Parameters.Add("@p31", OleDbType.Integer).Value = item.关系距离;
                                command.Parameters.Add("@p32", OleDbType.Integer).Value = item.语境树;
                                command.Parameters.Add("@p33", OleDbType.UnsignedTinyInt).Value = item.层级;
                                command.Parameters.Add("@p34", OleDbType.VarWChar).Value = item.风格;
                                command.Parameters.Add("@p35", OleDbType.VarWChar).Value = item.打分;
                                command.Parameters.Add("@p36", OleDbType.Guid).Value = item.ID;

                                command.ExecuteNonQuery();
                            }
                        }

                    }

                    transaction.Commit();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    // TODO: 处理异常
                }
            }
            if (isSuccess)
            {
                foreach (var 模式 in 更新对象集)
                {
                    模式.IsUpdated = false;
                }
            }
        }

        /// <summary>
        /// 提交新增对象
        /// </summary>
        protected override void CommitNewObject()
        {
            var 新增对象集 = 对象集合.Where(r => r.IsNew == true).ToList();
            bool isSuccess = false;

            using (
               IDbConnection conn = getConnectionByTableName(this.TableName)
                 )
            {
                IDbTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    foreach (模式 item in 新增对象集)
                    {
                        if (conn is SqlConnection)
                        {
                            using (SqlCommand command = (SqlCommand)conn.CreateCommand())
                            {
                                command.Transaction = (SqlTransaction)transaction;
                                command.CommandText =
                                "insert into " + TableName + "(ID, ParentID, 序号, 语言, 形式, 源记录, A端, 连接, B端, That根, 的, 显隐, 语言角色, " +
                                "C端, 通用分类, 说明, 参数集合, 成立度, 实例数, Aα, Aβ, Bα, Bβ, 联α, 联β, 说话时间, 说话地点, 全等引用计数, " +
                                "级别, 认知年龄, 附加信息, 关系距离, 语境树, 层级, 风格, 打分) " +
                                "values(@p1, @p2, @p3, @p4, @p5, @p6, @p7 ,@p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, " +
                                "@p18, @p19, @p20, @p21, @p22, @p23, @p24 ,@p25, @p26, @p27, @p28, @p29, @p30, @p31, @p32, @p33, @p34, @p35, @p36)";

                                command.Parameters.Add("@p1", SqlDbType.UniqueIdentifier).Value = item.ID;
                                command.Parameters.Add("@p2", SqlDbType.UniqueIdentifier).Value = item.ParentID;
                                command.Parameters.Add("@p3", SqlDbType.Int).Value = item.序号;
                                command.Parameters.Add("@p4", SqlDbType.Int).Value = item.语言;
                                command.Parameters.Add("@p5", SqlDbType.NVarChar).Value = item.形式 ?? string.Empty;
                                command.Parameters.Add("@p6", SqlDbType.UniqueIdentifier).Value = item.源记录;
                                command.Parameters.Add("@p7", SqlDbType.UniqueIdentifier).Value = item.A端;
                                command.Parameters.Add("@p8", SqlDbType.UniqueIdentifier).Value = item.连接;
                                command.Parameters.Add("@p9", SqlDbType.UniqueIdentifier).Value = item.B端;
                                command.Parameters.Add("@p10", SqlDbType.Int).Value = item.That根;
                                command.Parameters.Add("@p11", SqlDbType.Int).Value = item.的;
                                command.Parameters.Add("@p12", SqlDbType.SmallInt).Value = item.显隐;
                                command.Parameters.Add("@p13", SqlDbType.Int).Value = item.语言角色;
                                command.Parameters.Add("@p14", SqlDbType.UniqueIdentifier).Value = item.C端;
                                command.Parameters.Add("@p15", SqlDbType.SmallInt).Value = item.通用分类;
                                command.Parameters.Add("@p16", SqlDbType.NVarChar).Value = item.说明 ?? string.Empty;
                                command.Parameters.Add("@p17", SqlDbType.NVarChar).Value = item.参数集合 ?? string.Empty;
                                command.Parameters.Add("@p18", SqlDbType.Real).Value = item.成立度;
                                command.Parameters.Add("@p19", SqlDbType.SmallInt).Value = item.实例数;
                                command.Parameters.Add("@p20", SqlDbType.Real).Value = item.Aα;
                                command.Parameters.Add("@p21", SqlDbType.Real).Value = item.Aβ;
                                command.Parameters.Add("@p22", SqlDbType.Real).Value = item.Bα;
                                command.Parameters.Add("@p23", SqlDbType.Real).Value = item.Bβ;
                                command.Parameters.Add("@p24", SqlDbType.Real).Value = item.联α;
                                command.Parameters.Add("@p25", SqlDbType.Real).Value = item.联β;
                                command.Parameters.Add("@p26", SqlDbType.UniqueIdentifier).Value = item.说话时间;
                                command.Parameters.Add("@p27", SqlDbType.UniqueIdentifier).Value = item.说话地点;
                                command.Parameters.Add("@p28", SqlDbType.Int).Value = item.全等引用计数;
                                command.Parameters.Add("@p29", SqlDbType.Int).Value = item.级别;
                                command.Parameters.Add("@p30", SqlDbType.Int).Value = item.认知年龄;
                                command.Parameters.Add("@p31", SqlDbType.Int).Value = item.附加信息;
                                command.Parameters.Add("@p32", SqlDbType.Int).Value = item.关系距离;
                                command.Parameters.Add("@p33", SqlDbType.Int).Value = item.语境树;
                                command.Parameters.Add("@p34", SqlDbType.TinyInt).Value = item.层级;
                                command.Parameters.Add("@p35", SqlDbType.NVarChar).Value = item.风格 ?? string.Empty;
                                command.Parameters.Add("@p36", SqlDbType.NVarChar).Value = item.打分 ?? string.Empty;
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (OleDbCommand command = (OleDbCommand)conn.CreateCommand())
                            {
                                command.Transaction = (OleDbTransaction)transaction;
                                command.CommandText =
                                "insert into " + TableName + "(ID, ParentID, 序号, 语言, 形式, 源记录, A端, 连接, B端, That根, 的, 显隐, 语言角色, " +
                                "C端, 通用分类, 说明, 参数集合, 成立度, 实例数, Aα, Aβ, Bα, Bβ, 联α, 联β, 说话时间, 说话地点, 全等引用计数, " +
                                "级别, 认知年龄, 附加信息, 关系距离, 语境树, 层级, 风格, 打分) " +
                                "values(?, ?, ?, ?, ?, ?, ? ,?, ?, ?, ?, ?, ?, ?, ?, ?, ?, " +
                                "?, ?, ?, ?, ?, ?, ? ,?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                                command.Parameters.Add("@p1", OleDbType.Guid).Value = item.ID;
                                command.Parameters.Add("@p2", OleDbType.Guid).Value = item.ParentID;
                                command.Parameters.Add("@p3", OleDbType.Integer).Value = item.序号;
                                command.Parameters.Add("@p4", OleDbType.Integer).Value = item.语言;
                                command.Parameters.Add("@p5", OleDbType.VarWChar).Value = item.形式 ?? string.Empty;
                                command.Parameters.Add("@p6", OleDbType.Guid).Value = item.源记录;
                                command.Parameters.Add("@p7", OleDbType.Guid).Value = item.A端;
                                command.Parameters.Add("@p8", OleDbType.Guid).Value = item.连接;
                                command.Parameters.Add("@p9", OleDbType.Guid).Value = item.B端;
                                command.Parameters.Add("@p10", OleDbType.Integer).Value = item.That根;
                                command.Parameters.Add("@p11", OleDbType.Integer).Value = item.的;
                                command.Parameters.Add("@p12", OleDbType.SmallInt).Value = item.显隐;
                                command.Parameters.Add("@p13", OleDbType.Integer).Value = item.语言角色;
                                command.Parameters.Add("@p14", OleDbType.Guid).Value = item.C端;
                                command.Parameters.Add("@p15", OleDbType.SmallInt).Value = item.通用分类;
                                command.Parameters.Add("@p16", OleDbType.VarWChar).Value = item.说明 ?? string.Empty;
                                command.Parameters.Add("@p17", OleDbType.VarWChar).Value = item.参数集合 ?? string.Empty;
                                command.Parameters.Add("@p18", OleDbType.Double).Value = item.成立度;
                                command.Parameters.Add("@p19", OleDbType.SmallInt).Value = item.实例数;
                                command.Parameters.Add("@p20", OleDbType.Double).Value = item.Aα;
                                command.Parameters.Add("@p21", OleDbType.Double).Value = item.Aβ;
                                command.Parameters.Add("@p22", OleDbType.Double).Value = item.Bα;
                                command.Parameters.Add("@p23", OleDbType.Double).Value = item.Bβ;
                                command.Parameters.Add("@p24", OleDbType.Double).Value = item.联α;
                                command.Parameters.Add("@p25", OleDbType.Double).Value = item.联β;
                                command.Parameters.Add("@p26", OleDbType.Guid).Value = item.说话时间;
                                command.Parameters.Add("@p27", OleDbType.Guid).Value = item.说话地点;
                                command.Parameters.Add("@p28", OleDbType.Integer).Value = item.全等引用计数;
                                command.Parameters.Add("@p29", OleDbType.Integer).Value = item.级别;
                                command.Parameters.Add("@p30", OleDbType.Integer).Value = item.认知年龄;
                                command.Parameters.Add("@p31", OleDbType.Integer).Value = item.附加信息;
                                command.Parameters.Add("@p32", OleDbType.Integer).Value = item.关系距离;
                                command.Parameters.Add("@p33", OleDbType.Integer).Value = item.语境树;
                                command.Parameters.Add("@p34", OleDbType.UnsignedTinyInt).Value = item.层级;
                                command.Parameters.Add("@p35", OleDbType.VarWChar).Value = item.风格 ?? string.Empty;
                                command.Parameters.Add("@p36", OleDbType.VarWChar).Value = item.打分 ?? string.Empty;

                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }

            if (isSuccess)
            {
                foreach (var 模式 in 新增对象集)
                {
                    模式.IsNew = false;
                }
            }
        }

    }
}
