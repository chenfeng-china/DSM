using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace PatternApplication.DataObject
{
    public class 形式对象
    {
        public int 语言;
        public string 参数;
        public 形式对象(int 语言类型, string 参数值)
        {
            语言 = 语言类型;
            参数 = 参数值;
        }
    }
    public class Knowledge
    {
        private static SqlConnection conn;
        private static List<OntologyClass> OntologyClasses = new List<OntologyClass>();
        public static List<形式化语料串> 外围语料串 = new List<形式化语料串>();
        public static List<模式> 对象集合 = new List<模式>();
        static Knowledge()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KnowledgeBaseConnectionString"].ConnectionString;
            conn = new SqlConnection(connectionString);

            LoadOntologyClasses();
           // LoadObjects();
        }

        public static void LoadOntologyClasses()
        {
            try
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT ID, Name, ParentID, MappingPatternClass1, MappingPatternClass2, MappingPatternClass3 FROM OntologyClass";
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            OntologyClass instance = new OntologyClass();
                            instance.ID = dataReader.GetGuid(0);
                            instance.Name = dataReader.GetString(1);

                            if (!dataReader.IsDBNull(2))
                            { 
                                instance.ParentID = dataReader.GetGuid(2);
                            }
                            if (!dataReader.IsDBNull(3))
                            { 
                                instance.MappingPatternClass1 = dataReader.GetGuid(3);
                            }
                            if (!dataReader.IsDBNull(4))
                            { 
                                instance.MappingPatternClass2 = dataReader.GetGuid(4);
                            }
                            if (!dataReader.IsDBNull(5))
                            {
                                instance.MappingPatternClass2 = dataReader.GetGuid(5);
                            }

                            OntologyClasses.Add(instance);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        public  void LoadObjects()
        { 
            try
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT o.id,s.Str,s.Local,o.Name FROM LSKStr s inner join LSKObj o on s.ObjID=o.ID " +
                                                " and s.state=0 and o.state=0 order by o.ID";
                    command.CommandTimeout = 120;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        int startIndex = 100;
                        Guid lastGuid=Guid.Empty;
                        模式 last模式 = null;
                        while (dataReader.Read())
                        {
                            if (!lastGuid.Equals(dataReader.GetGuid(0)))
                            {
                                模式 obj = Data.New派生行(Data.未分类事物Guid, 字典_目标限定.A端);
                                obj.ID = dataReader.GetGuid(0);
                                obj.形式 = "[" + dataReader.GetString(3) + "]";
                                obj.ParentID = Data.未分类事物Guid;
                                obj.序号 = startIndex;
                                startIndex++;
                                对象集合.Add(obj);

                                lastGuid = dataReader.GetGuid(0);
                                last模式 = obj;
                            }
                            模式 形式Row = 增加形式行(last模式, dataReader.GetString(1), ParseStrLanguage(dataReader.GetString(2)));
                            对象集合.Add(形式Row);

                            外围语料串.Add(new 形式化语料串() { 字符串 = 形式Row.形式, ObjectID = 形式Row.ID, IsCore = false });
                            //外围语料串.Add(new 形式化语料串() { 字符串 = dataReader.GetString(1), ObjectID = dataReader.GetGuid(0), IsCore = false });
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        private 模式 增加形式行(模式 模式Row, string word, int 语言)
        {
            模式 形式Row = Data.New派生行(Data.拥有形式Guid);
            形式Row.ParentID = 模式Row.ID;
            形式Row.A端 = 模式Row.ID;
            形式Row.B端 = Data.ThisGuid;
            形式Row.形式 = word;
            形式Row.语言 = 语言;
            形式Row.语言角色 = 字典_语言角色.全部;
            return 形式Row;
        }
        public static void LoadAllWords(List<形式化语料串> list)
        {
            List<KeyValuePair<string, Guid>> result = new List<KeyValuePair<string, Guid>>();
            try
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT Str, ObjID FROM LSKStr where state=0";
                    command.CommandTimeout = 120;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            list.Add((new 形式化语料串() { 字符串 = dataReader.GetString(0), ObjectID = dataReader.GetGuid(1), IsCore = false }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public static  List<KeyValuePair<string, Guid>> GetAllWords()
        {
            List<KeyValuePair<string, Guid>> result = new List<KeyValuePair<string, Guid>>();
            try
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT Str, ObjID FROM LSKStr";
                    command.CommandTimeout = 120;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new KeyValuePair<string, Guid>(dataReader.GetString(0), dataReader.GetGuid(1)));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public static  KeyValuePair<string, List<Guid>> GetClassesOfObj(Guid 对象ID)
        {
            KeyValuePair<string, List<Guid>> classesList = new KeyValuePair<string,List<Guid>>();

            try
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT Name, OntologyClasses, PatternClass1, PatternClass2, PatternClass3, PatternClass4, PatternClass5, PatternClass6, PatternClass7, PatternClass8 FROM LSKObj WHERE ID=@ID";
                    command.Parameters.AddWithValue("@ID", 对象ID);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            List<Guid> classes = new List<Guid>();
                            string name = dataReader.GetString(0);  // 对象名

                            if (!dataReader.IsDBNull(2))
                            {
                                Guid patternClass = dataReader.GetGuid(2);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(3))
                            {
                                Guid patternClass = dataReader.GetGuid(3);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(4))
                            {
                                Guid patternClass = dataReader.GetGuid(4);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(5))
                            {
                                Guid patternClass = dataReader.GetGuid(5);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(6))
                            {
                                Guid patternClass = dataReader.GetGuid(6);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(7))
                            {
                                Guid patternClass = dataReader.GetGuid(7);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(8))
                            {
                                Guid patternClass = dataReader.GetGuid(8);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }


                            if (!dataReader.IsDBNull(9))
                            {
                                Guid patternClass = dataReader.GetGuid(9);
                                if (!patternClass.Equals(Guid.Empty) && !classes.Contains(patternClass))
                                {
                                    classes.Add(patternClass);
                                }
                            }

                            if (!dataReader.IsDBNull(1) && classes.Count==0) //如果没有配置明细分类，则启用默认分类配置
                            {
                                ParseOntologyClassesToPatternClass(dataReader.GetString(1), classes);
                            }
                            classesList = new KeyValuePair<string,List<Guid>>(name, classes);
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return classesList;
        }

        private static void ParseOntologyClassesToPatternClass(string ontologyClasses, List<Guid> classes)
        {
            if (string.IsNullOrEmpty(ontologyClasses))
            {
                return;
            }

            List<string> ontologyClassList = ontologyClasses.Split('|').ToList();

            foreach (var item in ontologyClassList)
            {
                List<Guid> mappingPatternClasses = GetMappingPatternClasses(OntologyClasses.Single(r=>r.Name.Equals(item)));

                foreach (var patternClass in mappingPatternClasses)
                {
                    if (!classes.Contains(patternClass))
                    {
                        classes.Add(patternClass);
                    }
                }
            }
        }

        private static List<Guid> GetMappingPatternClasses(OntologyClass item)
        {
            List<Guid> patternClassList = new List<Guid>();

            if (item.MappingPatternClass1.HasValue)
            {
                patternClassList.Add(item.MappingPatternClass1.Value);
            }

            if (item.MappingPatternClass2.HasValue)
            {
                patternClassList.Add(item.MappingPatternClass2.Value);
            }

            if (item.MappingPatternClass3.HasValue)
            {
                patternClassList.Add(item.MappingPatternClass3.Value);
            }

            if (!patternClassList.Any() && item.ParentID.HasValue)
            {
                patternClassList.AddRange(GetMappingPatternClasses(OntologyClasses.Single(r=>r.ID==item.ParentID)));
            }

            return patternClassList;
        }

        public static List<KeyValuePair<string, 形式对象>> GetStrsOfObj(Guid 对象ID)
        {
            List<KeyValuePair<string, 形式对象>> result = new List<KeyValuePair<string, 形式对象>>();

            try
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT Str, Local,Param FROM LSKStr WHERE ObjID=@ObjID";
                    command.Parameters.AddWithValue("@ObjID", 对象ID);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string str = dataReader.GetString(0);
                            int language = dataReader.GetInt32(1);  //ParseStrLanguage(dataReader.GetString(1))
                            string param=dataReader.IsDBNull(2)?null:dataReader.GetString(2);
                            result.Add(new KeyValuePair<string, 形式对象>(str, new 形式对象(language,param)));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
     
        private static  int ParseStrLanguage(string local)
        {
            int language = 字典_语言.公语;
            if (local=="zh-cn")
                    language = 字典_语言.汉语;
            else if (local=="en")
                    language = 字典_语言.英语;
            else if (local==字典_语言.汉语.ToString())
                    language = 字典_语言.汉语;
            else if (local==字典_语言.英语.ToString())
                    language = 字典_语言.英语;
            return language;
        }

        public static string load句子()
        {
            string ret="";
            try
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM 句子 WHERE flag=0";
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            ret+= dataReader.GetString(0)+"\n";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            return ret;
        }
        public static bool save句子(string[] 句子)
        {
            SqlTransaction trans = null;
            try
            {
                conn.Open();
                trans = conn.BeginTransaction();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.Transaction = trans;
                    command.CommandText = "delete from 句子";
                    command.ExecuteNonQuery();
                }
                for (int i = 0; i < 句子.Length; i++)
                {
                    if (string.IsNullOrEmpty(句子[i]) == false)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = trans;
                            command.CommandText = "insert into 句子 (name,flag) values(@p1,@p2) ";
                            command.Parameters.AddWithValue("@p1", 句子[i]);
                            command.Parameters.AddWithValue("@p2", 0);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                trans.Commit();
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }
    }

    
    /// <summary>
    /// DBpeida的本体分类对象
    /// </summary>
    public class OntologyClass
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public Guid? ParentID { get; set; }

        public Guid? MappingPatternClass1 { get; set; }

        public Guid? MappingPatternClass2 { get; set; }

        public Guid? MappingPatternClass3 { get; set; }
    }
}
