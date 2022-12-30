using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NERUtil
{
    // 数字、日期、时间检查对象
    internal class NEFilter
    {
        public NameEntityType FilterType { get; set; }
        public int Language { get; set; }
        public string Rule { get; set; }
        public string RegExpression { get; set; }
    }


    public enum NameEntityType : int
    {
        Number = 1,                // 数字
        Time = 2,                  // 时间
        Identifier=3,              //编号
        Date = 4,                  // 日期
        PersonName = 8,            // 人名
        LocationName = 16,         // 地名
        OrganizationName = 32,     // 组织机构名
        OtherName = 64             // 其他专用名词
    }

    public struct NameEntity
    {
        public NameEntity(NameEntityType type, int startIndex, int endIndex, string 串)
        {
            实体类型 = type;
            BeginIndex = startIndex;
            EndIndex = endIndex;
            this.串 = 串;
            概率 = 1;                     // 默认为1， 后期需要处理， 取值在1到9之间
        }


        public NameEntityType 实体类型;

        public int BeginIndex;

        public int EndIndex;

        public string 串;

        public int 概率;
    }

    /*
     * 通用语言处理器
     * 个性化的可以单独处理
     * 各种语言可以处理方言
     */

    public static class 串类型
    {
        public static int 任意 = 0;
        public static int 汉字 = 1;
        public static int 连续 = 100;
        public static int 英语字母 = 200;
        public static int 数字 = 300;
        public static int 计算字符类型(char c)
        {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == ' ')//字母
                return 英语字母;
            if ((c >= '0' && c <= '9'))//数字
                return 数字;
            if (c >= 0x4e00 && c <= 0x9fa5)//现在暂时假设每一个汉字是一个词
                return 汉字;
            return 0;
        }
    }

}
