using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace NERUtil
{
    public class NameEntityRecognizer : IDisposable    
    {  
        static List<NEFilter> CheckFilters = new List<NEFilter>();
        private char[] 数字种子_阿拉伯 = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '百', '千', '万', '亿' };
        private char[] 数字种子_中文小写 = new char[] { '一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '百', '千', '万', '亿' };
        private char[] 数字种子_中文大写 = new char[] { '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖', '拾', '百', '千', '万', '亿' };
        private string 数字种子_英文 = "one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seveteen eighteen nineteen "+
                        "twenty thirty forty fifty sixty seventy eighty ninety hundred thousand million billion";
        private string[] 英文数字种子;
        const int 数字类型_阿拉伯 = 1;
        const int 数字类型_中文小写 = 2;
        const int 数字类型_中文大写 = 3;
        const int 数字类型_英文 = 4;
        static NameEntityRecognizer()
        {
            // TODO: 将这些过滤条件将来通过存放数据库中来的读取
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)1, Language = 0x7fffffff, Rule = "整数、小数、百分数（数字为0-9）", RegExpression = @"(\d+\.\d+)%?|(\d+)%?" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)1, Language = 0x1000000, Rule = "零、一到九、十、百、千、万、亿、点", RegExpression = @"([零一二三四五六七八九十百千万亿]+点[零一二三四五六七八九十百千万亿]+)%?|([零一二三四五六七八九十百千万亿]+)%?" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)1, Language = 0x1000000, Rule = "百分之", RegExpression = @"百分之[零一二三四五六七八九十百千万亿]+（点[零一二三四五六七八九十百千万亿]+）?" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x7fffffff, Rule = "年/月/日、年-月-日（年月日为数字0-9)", RegExpression = @"(\d+[\./-]){1,2}\d+" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x1000000, Rule = "2014年12月20日(号)", RegExpression = @"\d+年\d+月\d+[号日]?|\d+年\d+月|\d+年|\d+月\d+[号日]?|\d+[号日]" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x1000000, Rule = "二零零零年十月十日", RegExpression = @"[零一二三四五六七八九十百千万亿]+年[零一二三四五六七八九十百千万亿]+月[零一二三四五六七八九十百千万亿]+[号日]?|[零一二三四五六七八九十百千万亿]+年[零一二三四五六七八九十百千万亿]+月|[零一二三四五六七八九十百千万亿]+年|[零一二三四五六七八九十百千万亿]+月[零一二三四五六七八九十百千万亿]+[号日]?|[零一二三四五六七八九十百千万亿]+[号日]" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x1000000, Rule = "公元1740年5月12日", RegExpression = @"公元\d+年\d+月\d+[日号]|公元\d+年\d+月|公元\d+年" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x1000000, Rule = "公元一七四零年五月十二日", RegExpression = @"公元[零一二三四五六七八九十百千万亿]+年[零一二三四五六七八九十百千万亿]+月[零一二三四五六七八九十百千万亿]+[日号]|公元[零一二三四五六七八九十百千万亿]+年[零一二三四五六七八九十百千万亿]+月|公元[零一二三四五六七八九十百千万亿]+年" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)4, Language = 0x1000000, Rule = "20(二十)世纪(初、中、末)", RegExpression = @"\d+世纪[初中末]?|[零一二三四五六七八九十百千万亿]世纪[初中末]?" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)2, Language = 0x7fffffff, Rule = "12:30:36(时分秒)", RegExpression = @"(\d+:){1,2}\d+" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)2, Language = 0x1000000, Rule = "12(小)时14分（钟）30秒", RegExpression = @"\d+小?时\d+分钟?\d+秒?|\d+小?时\d+(分钟|分)?|\d+小?时\d+秒?|\d+分钟?\d+秒?|\d+秒" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)2, Language = 0x1000000, Rule = "十二(小)时十分（钟）三十秒", RegExpression = @"[零一二三四五六七八九十百千万亿]小?时[零一二三四五六七八九十百千万亿]+分钟?[零一二三四五六七八九十百千万亿]+秒?|[零一二三四五六七八九十百千万亿]+小?时[零一二三四五六七八九十百千万亿]+(分钟|分)?|[零一二三四五六七八九十百千万亿]+小?时[零一二三四五六七八九十百千万亿]+秒?|[零一二三四五六七八九十百千万亿]+分钟?[零一二三四五六七八九十百千万亿]+秒?|[零一二三四五六七八九十百千万亿]+秒" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)2, Language = 0x1000000, Rule = "子时三刻", RegExpression = @"[子丑寅卯辰巳午未申酉戌亥]时([一二三四五六七八]刻)?" });
            CheckFilters.Add(new NEFilter() { FilterType = (NameEntityType)2, Language = 0x1000000, Rule = "三更(天)", RegExpression = @"[一二三四五]更天?" });
        }


        public NameEntityRecognizer()
        {
            //if (!NLPIRWrapper.NLPIR_Init(System.AppDomain.CurrentDomain.BaseDirectory, 0, ""))
            //   System.Diagnostics.Debug.Assert(false);
        }

        public void Dispose()
        {
            //NLPIRWrapper.NLPIR_Exit();
        }  

        public string ParagraphSplit(string paragraph)
        {
            IntPtr intPtr = NLPIRWrapper.NLPIR_ParagraphProcess(paragraph);//切分结果保存为IntPtr类型
            return Marshal.PtrToStringAnsi(intPtr);//将切分结果转换为string
        }
        //检查剩余部分是否为数字并且没有"."符号
        private bool haveNumberAndNoPoint(char[] strings, int start)
        {
            for (int i = start; i < strings.Count(); i++)
            {
                if (!数字种子_阿拉伯.Contains(strings[i]))
                {
                    if (strings[i] == '.' && i<strings.Count()-1 && 数字种子_阿拉伯.Contains(strings[i+1]))
                        return false;
                    else
                    {
                        break;
                    }   
                }
            }
            for (int j = start - 2; j >= 0; j--) //再检查一下前方是否是纯数字串 避免214.22.3这样的串
            {
                if (!数字种子_阿拉伯.Contains(strings[j]))
                {
                    if (strings[j] == '.')
                        return false;
                    else
                        return true;
                }
            }
            return true;
        }

        //获取英文数字对象
        private void GetEnglishNumberEntities(char[] strings, IList<NameEntity> result)
        {
            int 类型 = 0;
            int k = 0;
            int istart = 0;
            string text = "";
            while (k < strings.Length)
            {
                int n = 串类型.计算字符类型(strings[k]);
                if (k == istart)//第一个字符，记下类型。
                    类型 = n;
                if (n != 类型)
                {
                    if (类型 == 串类型.英语字母)
                        GetEnglishNumbers(text, result, istart);
                    类型 = n;
                    text = "";
                    istart = k;
                }
                else if (n == 类型 && k == strings.Length - 1)
                {
                    text += strings[k];
                    GetEnglishNumbers(text, result, istart);
                }
                text += strings[k];
                k++;
            }
        }
        private void GetEnglishNumbers(string paragraph,IList<NameEntity> result,int istartpos=0)
        {
            if (英文数字种子==null)
                英文数字种子 = 数字种子_英文.Split(' ');
            string[] 源串 = paragraph.Split(' ');
            if (源串.Count() > 0)
            {
                int lstType = 0;
                int type = 0;
                int startIndex = 0;
                string nameStr = "";
                for (int i = 0; i < 源串.Count(); i++)
                {
                    type = 0;
                    if (isEnglishNumber(源串[i]))
                    {
                        type = 数字类型_英文;
                    }
                    if (type != lstType)
                    {
                        if (lstType != 0)
                        {
                            result.Add(new NameEntity(NameEntityType.Number,istartpos+ startIndex, istartpos+startIndex + nameStr.Length , nameStr));
                        }
                        if  (i>0)
                            startIndex +=nameStr.Length+1;                       
                        lstType = type;
                        nameStr = "";
                    }
                    nameStr +=(nameStr==""?"":" ")+源串[i];
                   
                }
                if (lstType != 0 && lstType == type)
                {
                    result.Add(new NameEntity(NameEntityType.Number, istartpos + startIndex, istartpos+startIndex + nameStr.Length, nameStr));
                }
            }
        }
        private bool isEnglishNumber(string paragraph)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(paragraph.Trim()))
            {
                if (paragraph.IndexOf('-') >= 0) //英文中允许twenty-six类似的数字
                {
                    string[] strings = paragraph.Split('-');
                    if (strings.Length == 2 && 英文数字种子.Contains(strings[0].ToLower()) && 英文数字种子.Contains(strings[1].ToLower()))
                    {
                        return true;
                    }
                }
                else if (英文数字种子.Contains(paragraph.ToLower()))
                    return true;
            }
            return ret;
        }
        //区分编号和数字,生成命名实体对象
        private NameEntity createNumberEntity(int startIndex, int endIndex, string text)
        {
            if(text[text.Length-1]=='.')
                return new NameEntity(NameEntityType.Identifier, startIndex, endIndex, text);
            else
                return new NameEntity(NameEntityType.Number, startIndex, endIndex, text);
        }
        //获取数字对象
        private void GetNumberEntities(char[] strings, IList<NameEntity> result)
        {
            if (strings.Count() > 0)
            {
                int lstType = 0;
                int type = 0;
                int startIndex = 0;
                string nameStr = "";
                for (int i = 0; i < strings.Count(); i++)
                {
                    type = 0;
                    if (数字种子_阿拉伯.Contains(strings[i]))
                    {
                        type = 数字类型_阿拉伯;
                    }
                    else if (数字种子_中文小写.Contains(strings[i]))
                    {
                        type = 数字类型_中文小写;
                    }
                    else if (数字种子_中文大写.Contains(strings[i]))
                    {
                        type = 数字类型_中文大写;
                    }
                    if (type != lstType)
                    {
                        if (type == 0 && strings[i] == '.' && i < strings.Count() - 1
                            && haveNumberAndNoPoint(strings, i + 1)) //阿拉伯数字中，允许一次小数点
                        {
                            goto next;
                        }
                        //,分隔的数字，在中文中有可能是并列的数，暂时先当成单独的数字段
                        if (lstType != 0)
                        {
                            result.Add(createNumberEntity(startIndex, i, nameStr));
                        }
                        lstType = type;
                        startIndex = i;

                        nameStr = "";
                    }
                next:
                    nameStr += strings[i];
                }
                if (lstType != 0 && lstType == type)
                {
                    result.Add(createNumberEntity(startIndex, strings.Count(), nameStr));
                }
            }
        }
        public IList<NameEntity> GetNameEntities(string paragraph, int 语言)
        {
            List<NameEntity> result = new List<NameEntity>();
            char[] strings = paragraph.ToArray();
            //#region 利用郑则表达式获取数字、时间、日期

            //List<NEFilter> toCheckFilters = NameEntityRecognizer.CheckFilters.Where(r => (r.Language & 语言) > 0).ToList();

            //foreach (var filter in toCheckFilters)
            //{
            //    Regex regExpression = new Regex(filter.RegExpression);
            //    var matchCollection = regExpression.Matches(paragraph);

            //    foreach (Match item in matchCollection)
            //    {
            //        result.Add(new NameEntity(filter.FilterType, item.Index, item.Index + item.Length, item.Value));
            //    }
            //}
            //#endregion
            GetNumberEntities(strings, result);
            GetEnglishNumberEntities(strings, result);
            #region 获取人名、地名、组织机构名等命名实体

            //if (语言 == 0x1000000)
            //{
            //    #region 汉语

            //    string 分词结果 = ParagraphSplit(paragraph);
            //    // 由于分词过程中，单个空格会变成2个空格，首先将其变回来
            //    分词结果 = 分词结果.Replace("  ", " ");

            //    List<string> 切分结果 = 分词结果.Split(' ').Select(r => string.IsNullOrEmpty(r) ? " " : r).ToList();

            //    int startIndex = 0;
            //    int endIndex = 0;
            //    foreach (string item in 切分结果)
            //    {
            //        if (item.Contains('/'))
            //        {
            //            string nameStr = item.Split('/').First();
            //            endIndex = startIndex + nameStr.Length;

            //            if (item.Contains("/nr"))
            //            {
            //                result.Add(new NameEntity(NameEntityType.PersonName, startIndex, endIndex, nameStr));
            //            }
            //            else if (item.Contains("/nt"))
            //            {
            //                result.Add(new NameEntity(NameEntityType.OrganizationName, startIndex, endIndex, nameStr));
            //            }
            //            else if (item.Contains("/ns"))
            //            {
            //                result.Add(new NameEntity(NameEntityType.LocationName, startIndex, endIndex, nameStr));
            //            }
            //            else if (item.Contains("/nz"))
            //            {
            //                result.Add(new NameEntity(NameEntityType.OtherName, startIndex, endIndex, nameStr));
            //            }
            //        }
            //        else
            //        {
            //            endIndex = startIndex + item.Length;
            //        }

            //        startIndex = endIndex;
            //    }

            //    #endregion
            //}            

            #endregion

            return result;
        }
    }
}
