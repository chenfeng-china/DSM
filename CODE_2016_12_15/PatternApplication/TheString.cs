using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PatternApplication.DataObject;

namespace PatternApplication
{

    public static class 字符类
    {
        public static int 分句 = 1;//这个字符必须进行分句。
		public static int 空格 = 2;//不可看见的字符，都当着空格处理。
		public static int 忽略 = 4;//这个字符进行忽略，以及复制。
		public static int 可停顿标点 = 8;//这个字符可以作为中间停顿。
		public static int 可作为括号 = 16;//这个字符可以成对出现作为括号。
		public static int 标点符号 = 32;
        public static int 分隔标点 = 64;//间隔

        public static byte[] 字符类数组 = new byte[65536];
        public static void 设置一组(char 开始,char 结束,byte 取值)
        {
            for (char i = 开始; i <= 结束; i++)
                字符类数组[i] = 取值;
        }

        public static bool 初始化字符类数组()
        {
			字符类数组[' '] = (byte)(空格 | 分隔标点);
            字符类数组['　'] = (byte)(空格 | 分隔标点);
            字符类数组['\t'] = (byte)(空格 | 分隔标点);

            字符类数组[','] = (byte)(可停顿标点 | 标点符号 | 分隔标点);
            字符类数组['，'] = (byte)(可停顿标点 | 标点符号 | 分隔标点);

			字符类数组['。'] = (byte)(分句 | 分隔标点);
            字符类数组['\n'] = (byte)(分句 | 分隔标点);

			字符类数组['☆'] = (byte)(忽略 | 可作为括号);
			字符类数组['★'] = (byte)(忽略 | 可作为括号);
			字符类数组['◇'] = (byte)忽略;
			字符类数组['□'] = (byte)忽略;
            return true;
        }
        public static bool 标志 = 初始化字符类数组();

        public static bool 允许忽略(char 字符)
        {
            return (字符类数组[字符] & (空格 | 忽略)) > 0;
        }

		public static bool 符合类型(char 字符, int 类型)
		{
			return (字符类数组[字符] & (类型)) > 0;
		}

		public static bool 是标点符号(char 字符)
        {
            return (字符类数组[字符] & (标点符号)) > 0;
        }

        public static bool 是停顿标点(char 字符)
        {
            //暂时只支持逗号
			return (字符类数组[字符] & (可停顿标点)) > 0;
		}

        public static bool 是分隔标点(char 字符)
        {
            //暂时只支持逗号
            return (字符类数组[字符] & (分隔标点)) > 0;
        }
    }
    /// <summary>
    /// 用这个类来访问字符串，避免很多的字符串操作
    /// 暂时用.net来做，以后可能C++替换会更容易
    /// </summary>
    public class MyString : SubString
    {
        public char[] 源语言文字;
        public MyString(char[] 源串)
        {
            源语言文字 = 源串;
        }
        public int Length
        {
            get
            {
                return endindex - begindex;
            }
        }
        public string Substring(int startindex, int length)
        {
            return new string(源语言文字, begindex + startindex, length);
        }

        public char this[int index]
        {
            get
            {
                return 源语言文字[begindex + index];
            }
            set
            {
                源语言文字[begindex + index] = value;
            }
        }
    }

    public class SubString
    {
        //在源字符串中隔离出的位置
        //负数的值表示是开放的，比如开始值是-5，表示左边界线是5，而且可以向左边继续，扩展。而结束值是-10，表示右边界是10，而且可以向右扩展
        public int begindex;
        public int endindex;

        public int 长度
        {
            get
            {
                return endindex - begindex;
            }

        }

		public override string ToString()
		{
            string str = new string(Processor.当前处理器.源语言文字, begindex, endindex - begindex);
            return str;
		}


        public SubString()
        {

        }
        public SubString(int b = -1, int e = -1)
        {
            begindex = b;
            endindex = e;
        }

        public void 复制位置(SubString obj)
        {
            begindex = obj.begindex;
            endindex = obj.endindex;
        }
        public void 复制位置(int beg, int end)
        {
            begindex = beg;
            endindex = end;
        }        //public void SetValue(int begin, int end, int 右置)
        //{
        //    begindex = begin;
        //    endindex = end;
        //    右置空对象位置 = 右置;
        //}


		//0表示不重叠。1表示重叠。2表示交叉。
		//算法要调整，一方面要计算位置，但是在位置是相邻的情况下，要看两者是否加入了相同的对象！
		public static int 计算位置重叠性(SubString obj1, SubString obj2)
		{
			//覆盖型对象的长度为空，而虚拟长度其实无限，因此可以重叠，但只能出现一次。
			//if (obj1.长度 == 0 && obj1.覆盖型对象位标识 > 0 || obj2.长度 == 0 && obj2.覆盖型对象位标识 > 0)
			//    return (obj1.覆盖型对象位标识 ^ obj2.覆盖型对象位标识) > 0 ? 0 : 1;

			SubString 左对象 = obj1;
			SubString 右对象 = obj2;
			if (obj2.begindex < obj1.begindex)
			{
				左对象 = obj2;
				右对象 = obj1;
			}

			if (obj2.begindex == obj1.begindex)//分不出左右。
			{
				if (obj2.长度 == 0 && obj1.长度 == 0)//两个都是虚拟的，暂时也认为是重叠。
					return 1;
                if (obj2.长度 > 0 && obj1.长度 > 0)
                {
                    return 1;
                }
				return 0;
			}

			if (左对象.endindex < 右对象.begindex)
				return 0;
            if (左对象.endindex > 右对象.begindex)
                return 左对象.endindex > 右对象.endindex ? 1 : 2;

			return 0;
		}


        public static bool 一个对象拆分了另一个对象(SubString 拆分者, SubString 被拆分者)
        {
            if (拆分者.begindex <= 被拆分者.begindex)
                if (拆分者.endindex > 被拆分者.begindex && 拆分者.endindex < 被拆分者.endindex)
                    return true;
            if (拆分者.endindex >= 被拆分者.endindex)
                if (拆分者.begindex < 被拆分者.endindex && 拆分者.begindex > 被拆分者.begindex)
                    return true;
            return false;
        }


		public void 增加范围(SubString 目标范围)
		{
			if (目标范围.begindex!=-1 &&  begindex > 目标范围.begindex)
				begindex = 目标范围.begindex;
			if (目标范围.endindex != -1 &&  endindex < 目标范围.endindex )
				endindex = 目标范围.endindex;
        }
        public void 增加范围(int beg,int end)
        {
            if (beg != -1 && begindex > beg)
                begindex = beg;
            if (end != -1 && endindex < end)
                endindex = end;
        }
        public SubString 减去范围(SubString 减数范围)
        {
            int b = begindex;
            int e = endindex;
            if (减数范围.endindex <=b || 减数范围.begindex >= e)//没有交叉，返回空的。
                return new SubString();
			if (减数范围.begindex <= b)
                b = 减数范围.endindex;
			if (减数范围.endindex >= e)
                e = 减数范围.begindex;
			return e > b ? new SubString(b, e) : new SubString();
        }
        public SubString 重叠范围(SubString 另一个范围)
        {
            int b = begindex;
            int e = endindex;
            if (另一个范围.endindex <= b || 另一个范围.begindex >= e)//没有交叉，返回空的。
                return new SubString();
            if (另一个范围.begindex <= b)
                e = 另一个范围.endindex;
            if (另一个范围.endindex >= e)
                b = 另一个范围.begindex;
            return e > b ? new SubString(b, e) : new SubString();
        }

    }


    public class 匹配语料对象 : SubString
    {
        public 形式化语料串 语料串;//可以是很原子的串，也可以是一个复杂的模式。
        public 匹配语料对象(int beg, int end, 形式化语料串 语料)
        {
            begindex = beg;
            endindex = end;
            语料串 = 语料;
        }
    }
	//public class TheString
	//{
	//	bool 正确 = true;
	//	//单引号和双引号是系统的。别的都没有关系。具体就是[']和["]。
	//	public string headstr;
	//	public string str1 = "";//第一个引号括起来的串。代表整个树的串。
	//	public int 串1状态;//0表示没有，1表示单引号，2表示双引号
	//	public string midstr;
	//	public string str2 = "";//第二个引号括起来的串。代表本节点的串。
	//	public int 串2状态;//0表示没有，1表示单引号，2表示双引号
	//	public string trailstr;

	//	public string GetStr()
	//	{
	//		if (str1 == "")
	//			return str2;
	//		return str1;
	//	}

	//	public TheString()
	//	{
	//	}

	//	public TheString(string str)
	//	{
	//		splitstr(str);
	//	}

	//	public void CopyStr(TheString str)
	//	{
	//		if (str.串1状态 > 0)
	//		{
	//			str1 = str.str1;
	//			串1状态 = 2;
	//		}
	//		if (str.串2状态 > 0)
	//		{
	//			str2 = str.str2;
	//			串2状态 = 2;
	//		}
	//	}

	//	public void 设置状态2()
	//	{
	//		if (串1状态 > 0)
	//		{
	//			串1状态 = 2;
	//		}
	//		if (串2状态 > 0)
	//		{
	//			串2状态 = 2;
	//		}
	//	}
	//	public void 清理生成串()
	//	{
	//		//if (串1状态 == 2 && 串2状态 == 1)//只允许有一个原始串，而且移到第一个位置上去。
	//		//	{
	//		//		串1状态 = 1;
	//		//		str1 = str2;
	//		//		串2状态 = 0;
	//		//		str2 = ""
	//		//	}

	//		if (串2状态 == 2)
	//		{
	//			串2状态 = 0;
	//			str2 = "";
	//		}

	//		if (串1状态 == 2)
	//		{
	//			串1状态 = 0;
	//			str1 = "";
	//		}
	//		正确 = true;
	//	}
	//	public void 清理全部串()
	//	{
	//		//if (串1状态 == 2 && 串2状态 == 1)//只允许有一个原始串，而且移到第一个位置上去。
	//		//	{
	//		//		串1状态 = 1;
	//		//		str1 = str2;
	//		//		串2状态 = 0;
	//		//		str2 = ""
	//		//	}

	//		串2状态 = 0;
	//		str2 = "";

	//		串1状态 = 0;
	//		str1 = "";

	//		正确 = true;
	//	}

	//	public void AddString(string str, int 引号状态)
	//	{
	//		if (串1状态 != 0)//移到后边
	//		{
	//			str2 = str1;
	//			串2状态 = 串1状态;
	//			if (midstr == "")
	//			{
	//				midstr = headstr;
	//				headstr = "";
	//			}
	//		}

	//		str1 = str;
	//		串1状态 = 引号状态;
	//	}


	//	//分解串。
	//	public bool splitstr(string surcestr)
	//	{
	//		正确 = false;
	//		headstr = str1 = midstr = str2 = trailstr = "";
	//		串1状态 = 串2状态 = 0;

	//		string ss = surcestr.Replace("[\']", "\x11");
	//		ss = ss.Replace("[\"]", "\x12");

	//		int k = ss.IndexOf('\'');
	//		int k1 = ss.IndexOf('\"');
	//		if (k != -1)//发现了第一个单引号。
	//		{
	//			if (k1 == -1 || k < k1)//单引号优先
	//			{
	//				if (k > 0)
	//					headstr = ss.Substring(0, k);
	//				ss = ss.Substring(k + 1, ss.Length - k - 1);
	//				k = ss.IndexOf('\'');//第二个单引号
	//				if (k == -1)
	//					return false;//没有找到配对的单引号
	//				if (k > 0)
	//					str1 = ss.Substring(0, k);
	//				ss = ss.Substring(k + 1, ss.Length - k - 1);
	//				串1状态 = 1;
	//				goto label;
	//			}
	//		}

	//		k = k1;
	//		if (k != -1)//双引号
	//		{
	//			if (k > 0)
	//				headstr = ss.Substring(0, k);
	//			ss = ss.Substring(k + 1, ss.Length - k - 1);
	//			k = ss.IndexOf('\"');//第二个双引号
	//			if (k == -1)
	//				return false;//没有找到配对的双引号
	//			if (k > 0)
	//				str1 = ss.Substring(0, k);
	//			ss = ss.Substring(k + 1, ss.Length - k - 1);
	//			串1状态 = 2;
	//			goto label;
	//		}
	//		else//引号都没有找到，只有一个串
	//		{
	//			headstr = ss;
	//			goto end;
	//		}

	//	label://第二个串的处理。
	//		k = ss.IndexOf('\'');
	//		k1 = ss.IndexOf('\"');
	//		if (k != -1)//发现了第1个单引号。
	//		{
	//			if (k1 == -1 || k < k1)//单引号优先
	//			{
	//				if (k > 0)
	//					midstr = ss.Substring(0, k);
	//				ss = ss.Substring(k + 1, ss.Length - k - 1);
	//				k = ss.IndexOf('\'');//第二个单引号
	//				if (k == -1)
	//					return false;//没有找到配对的单引号
	//				if (k > 0)
	//					str2 = ss.Substring(0, k);
	//				trailstr = ss.Substring(k + 1, ss.Length - k - 1);
	//				串2状态 = 1;
	//				goto end;
	//			}
	//		}

	//		k = k1;
	//		if (k != -1)//双引号
	//		{
	//			if (k > 0)
	//				midstr = ss.Substring(0, k);
	//			ss = ss.Substring(k + 1, ss.Length - k - 1);
	//			k = ss.IndexOf('\"');//第二个双引号
	//			if (k == -1)
	//				return false;//没有找到配对的双引号
	//			if (k > 0)
	//				str2 = ss.Substring(0, k);
	//			trailstr = ss.Substring(k + 1, ss.Length - k - 1);
	//			串2状态 = 2;
	//			goto end;
	//		}
	//		else//引号都没有找到，只有一个串
	//			midstr = ss;

	//	end:

	//		headstr = headstr.Replace("\x11", "[\']");
	//		headstr = headstr.Replace("\x12", "[\"]");
	//		str1 = str1.Replace("\x11", "[\']");
	//		str1 = str1.Replace("\x12", "[\"]");
	//		midstr = midstr.Replace("\x11", "[\']");
	//		midstr = midstr.Replace("\x12", "[\"]");
	//		str2 = str2.Replace("\x11", "[\']");
	//		str2 = str2.Replace("\x12", "[\"]");
	//		trailstr = trailstr.Replace("\x11", "[\']");
	//		trailstr = trailstr.Replace("\x12", "[\"]");
	//		正确 = true;
	//		return true;
	//	}


	//	public string 生成()
	//	{
	//		if (正确 == false)
	//			return "错误";
	//		string s = headstr;
	//		if (串1状态 == 1)
	//			s = s + "\'" + str1 + "\'";
	//		else if (串1状态 == 2)
	//			s = s + "\"" + str1 + "\"";

	//		s = s + midstr;
	//		if (串2状态 == 1)
	//			s = s + "\'" + str2 + "\'" + trailstr;
	//		else if (串2状态 == 2)
	//			s = s + "\"" + str2 + "\"" + trailstr;

	//		return s;
	//	}
	//}

    public class TheString
    {
        bool 正确=true;
        //单引号和双引号是系统的。别的都没有关系。具体就是[']和["]。
        public string 前置语义串="";
        public string 嵌入串="";

		public bool 有嵌入串
		{
			get
			{
				return 嵌入串 != "";
			}
		}

		public TheString(string str)
		{
			if (!string.IsNullOrEmpty(str))
			    splitstr(str);
		}

        public void 设置嵌入串(TheString str)
        {
                嵌入串 = str.嵌入串;
        }

		public void 设置嵌入串(string str)
		{
			嵌入串 = str;
		}

        public void 清理嵌入串()
        {
                嵌入串 = "";
                正确 = true;
        }

        //分解串。
        public bool splitstr(string surcestr)
        {
            正确 = false;
            前置语义串=嵌入串="";

			string ss = surcestr.Replace("[\"]", "\x12");
			//string ss = ss.Replace("[\"]", "\x12");

			//int k=ss.IndexOf('\'');
			//int k1 = ss.IndexOf('\"');
			//if (k != -1 )//发现了第一个单引号。
			//{
			//	if(k1==-1 || k < k1)//单引号优先
			//		{
			//			if (k > 0)
			//				前置语义串 = ss.Substring(0, k);
			//			ss = ss.Substring(k+1, ss.Length - k-1);
			//			k = ss.IndexOf('\'');//第二个单引号
			//			if (k == -1)
			//				return false;//没有找到配对的单引号
			//			if (k > 0)
			//				嵌入串 = ss.Substring(0,k);
			//			goto end;
			//		}
			//}

			int k = ss.IndexOf('\"');
			if (k != -1)//双引号
            {
                if (k > 0)
                    前置语义串 = ss.Substring(0, k);
                ss = ss.Substring(k + 1, ss.Length - k-1);
                k = ss.IndexOf('\"');//第二个双引号
                if (k == -1)
                    return false;//没有找到配对的双引号
                if (k > 0)
                    嵌入串 = ss.Substring(0, k);
            }
            else//引号都没有找到，只有一个串
                前置语义串 = ss;
end:

			//前置语义串 = 前置语义串.Replace("\x11", "[\']");
            前置语义串 = 前置语义串.Replace("\x12", "[\"]");
			//嵌入串 = 嵌入串.Replace("\x11", "[\']");
            嵌入串 = 嵌入串.Replace("\x12", "[\"]");
            正确 = true;
            return true;
        }

        public override string ToString()
        {
            if (正确 == false)
                return "错误";
            string s = 前置语义串;
            if (有嵌入串)
                s = s + "\"" + 嵌入串 + "\"";
            return s;
        }
    }

}
