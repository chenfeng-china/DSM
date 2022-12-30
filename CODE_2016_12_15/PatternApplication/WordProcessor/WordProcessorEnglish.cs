using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessor
{
    public class WordProcessorEnglish : WordProcessor
    {
        /// <summary>
        /// 返回动词的时态形式
        /// </summary>
        /// <param name="verb">动词</param>
        /// <param name="tenseState">时态状态</param>
        /// <param name="tenseTiming">时态时间</param>
        /// <param name="info">时态格式附加信息</param>
        /// <returns>动词的时态形式</returns>
        public static string GetVerbTenseFormat(string verb, TenseState tenseState, TenseTiming tenseTiming, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb) || info == null)
            {
                return null;
            }

            var tense = (int)tenseState + (int)tenseTiming;
            VerbTenseFormatFunc func;
            if (!VerbTenseFormatFuncDict.TryGetValue(tense, out func))
            {
                return null;
            }

            return func(verb, info);
        }

        /// <summary>
        /// 返回动词的过去式形式
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        public static string GetVerbPastFormat(string verb)
        {
            string result;
            var v = verb.ToLower();
            if (IrregularVerbPastFormatDict.TryGetValue(v, out result))
            {
                return result;
            }

            var lastChar = v.Last();
            if (lastChar == 'y')    //以辅音字母+y结尾的:变y为i+ed
            {
                var c = v[v.Length - 2];
                if (!IsVowel(c))
                {
                    result = verb.Remove(v.Length - 1, 1) + "ied";
                }
            }
            else if (v.EndsWith("ic"))  //以ic结尾的:把ic变为ick+ed
            {
                result = verb + "ked";
            }
            else if (!IsVowel(lastChar))    //以辅音字母结尾的:双写最后一个辅音字母+ed
            {
                result = verb + verb.Last() + "ed";
            }
            else if (lastChar == 'e')
            {
                result = verb + "d";
            }
            else
            {
                result = verb + "ed";
            }

            return result;
        }

        /// <summary>
        /// 返回动词的过去分词形式
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        public static string GetVerbPastParticipleFormat(string verb)
        {
            string result;
            var v = verb.ToLower();
            if (IrregularVebPastParticipleFormatDict.TryGetValue(v, out result))
            {
                return result;
            }

            var lastChar = v.Last();
            if (lastChar == 'y')    //以辅音字母+y结尾的:变y为i+ed
            {
                var c = v[v.Length - 2];
                if (!IsVowel(c))
                {
                    result = verb.Remove(v.Length - 1, 1) + "ied";
                }
            }
            else if (v.EndsWith("ic"))  //以ic结尾的:把ic变为ick+ed
            {
                result = verb + "ked";
            }
            else if (!IsVowel(lastChar))    //以辅音字母结尾的:双写最后一个辅音字母+ed
            {
                result = verb + verb.Last() + "ed";
            }
            else if (lastChar == 'e')
            {
                result = verb + "d";
            }
            else
            {
                result = verb + "ed";
            }

            return result;
        }

        /// <summary>
        /// 返回动词的现在分词
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        public static string GetVerbDoingFormat(string verb)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var v = verb.ToLower();
            var lastChar = v.Last();
            if (lastChar == 'e')
            {
                var c = v[v.Length - 2];
                if (!IsVowel(c))
                {
                    return verb.Remove(verb.Length - 1, 1) + "ing";
                }
            }
            else if (!IsVowel(lastChar) && v.Length >= 3)
            {
                var c2 = v[v.Length - 2];
                var c3 = v[v.Length - 3];
                if (IsVowel(c2) && !IsVowel(c3))
                {
                    return verb + lastChar + "ing";
                }
            }

            return verb + "ing";
        }

        /// <summary>
        /// 返回单词的复数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string GetWordComplexFormat(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }

            // 第一、第二、第三人称单数时，系动词的复数形式是其本身
            if (word == "am" || word == "are" || word == "is")  
            {
                return word;
            }


            var w = word.ToLower();

            var lastChar = w.Last();
            if (lastChar == 'o')
            {
                var c = w[w.Length - 2];
                if (!IsVowel(c))
                {
                    return word + "es";
                }
            }
            else if (lastChar == 's' || lastChar == 'x')
            {
                return word + "es";
            }
            else if (lastChar == 'y')
            {
                var c = w[w.Length - 2];
                if (!IsVowel(c))
                {
                    return word.Remove(word.Length - 1, 1) + "ies";
                }
            }
            else if (w.EndsWith("sh") || w.EndsWith("ch"))
            {
                return word + "es";
            }
            else if (w == "have")
            {
                return word.Remove(word.Length - 2, 2) + "s";
            }

            return word + "s";
        }

        /// <summary>
        /// 是否是元音字母
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsVowel(char c)
        {
            return "aeiou".Contains(c);
        }

        public static readonly string[] AuxilizryVerbWords = new[] { "am", "is", "are" };
        /// <summary>
        /// 是否是助动词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsAuxilizryVerb(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return AuxilizryVerbWords.Contains(word.ToLower());
        }

        public static readonly string[] Personal3RdSingularWords = new[] { "he", "she", "it" };
        public static readonly string[] Personal3RdComplexWords = new[] { "they" };
        /// <summary>
        /// 是否是第三人称单数
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool Is3RdPersonalSingular(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Personal3RdSingularWords.Contains(word.ToLower());
        }

        /// <summary>
        /// 是否是第三人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool Is3RdPersonal(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            var w = word.ToLower();
            return Personal3RdSingularWords.Contains(w) || Personal3RdComplexWords.Contains(w);
        }

        public static readonly string[] Personal2NdWords = new[] { "you" };
        /// <summary>
        /// 是否是第二人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool Is2NdPersonal(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Personal2NdWords.Contains(word.ToLower());
        }

        public static readonly string[] Personal1StSingualarWords = new[] { "i" };
        public static readonly string[] Personal1StComplexWords = new[] { "we" };
        /// <summary>
        /// 是否是第一人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool Is1StPersonal(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            var w = word.ToLower();
            return Personal1StSingualarWords.Contains(w) || Personal1StComplexWords.Contains(w);
        }

        /// <summary>
        /// 是否是人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsPersonal(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Is1StPersonal(word) || Is2NdPersonal(word) || Is3RdPersonal(word);
        }

        /// <summary>
        /// 是否单数人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsPersonalSingualar(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            var w = word.ToLower();
            return Personal1StSingualarWords.Contains(w) || Personal3RdSingularWords.Contains(w);
        }

        /// <summary>
        /// 是否复数人称
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsPersonalComplex(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            var w = word.ToLower();
            return Personal1StComplexWords.Contains(w) || Personal3RdComplexWords.Contains(w);
        }

        private static Dictionary<string, string> _irregularVerbPastFormatDict;
        /// <summary>
        /// 不规则动词过去式字典
        /// </summary>
        public static Dictionary<string, string> IrregularVerbPastFormatDict
        {
            get
            {
                if (_irregularVerbPastFormatDict == null)
                {
                    _irregularVerbPastFormatDict = new Dictionary<string, string>
                        {
                            {"have", "had"},
                            {"has", "had"},
                            {"am", "were"},
                            {"are", "were"},
                            {"get", "got"},
                            {"say", "said"},
                            {"feel", "felt"},
                            {"do", "did"},
                            {"is", "was"},
                            {"am", "was"},
                            {"go", "went"},
                            {"drink", "drank"},
                            {"eat", "ate"},
                            {"bring", "brought"},
                            {"think", "thought"},
                            {"buy", "bought"},
                            {"catch", "caught"},
                            {"teach", "tought"},
                            {"sit", "sat"},
                            {"wear", "wore"},
                            {"cut", "cut"},
                            {"sweep", "swept"},
                            {"sleep", "slept"},
                            {"see", "saw"},
                            {"become", "became"},
                            {"read", "read"},
                            {"lend", "lent"}
                        };
                }

                return _irregularVerbPastFormatDict;
            }
        }

        private static Dictionary<string, string> _irregularVebPastParticipleFormatDict;
        /// <summary>
        /// 不规则动词过去分词字典
        /// </summary>
        public static Dictionary<string, string> IrregularVebPastParticipleFormatDict
        {
            get 
            {
                if (_irregularVebPastParticipleFormatDict == null)
                {
                    _irregularVebPastParticipleFormatDict = new Dictionary<string, string>() 
                    { 
                        {"has", "had"},
                        {"have", "had"},
                        {"am", "been"},
                        {"is", "been"},
                        {"are", "been"},
                        {"arise", "arisen"},
                        {"bear", "born"},
                        {"beat", "beaten"},
                        {"become", "become"},
                        {"befall", "befallen"},
                        {"beget", "begotten"},
                        {"begin", "began"},
                        {"behold", "beheld"},
                        {"bend", "bent"},
                        {"bereave", "bereft"},
                        {"beset", "beset"},
                        {"bespeak", "bespeaken"},
                        {"bespread", "bespread"},
                        {"bestrew", "bestrewn"},
                        {"bestride", "bestridden"},
                        {"betake", "betaken"},
                        {"bethink", "bethought"},
                        {"bid", "bidden"},
                        {"bide", "bided"},
                        {"bind", "bound"}, 
                        {"bite", "biten"},
                        {"bleed", "bled"},
                        {"blow", "blown"},
                        {"break", "broken"},
                        {"breed", "bred"},
                        {"bring", "brought"},
                        {"browbeat", "browbeaten"},
                        {"build", "built"},
                        {"burst", "burst"},
                        {"buy", "bought"},
                        {"cast", "cast"},
                        {"catch", "caught"},
                        {"choose", "choosen"},
                        {"cleave", "cleaven"},
                        {"cling", "clung"},
                        {"come", "come"},
                        {"cost", "cost"},
                        {"creep", "crept"},
                        {"crow", "crowed"},
                        {"cut", "cut"},
                        {"dare", "dared"},
                        {"deal", "dealt"},
                        {"die", "died"},
                        {"dig", "dug"},
                        {"dispread", "dispread"},
                        {"draw", "drawn"},
                        {"drink", "drunk"},
                        {"drive", "driven"},
                        {"eat", "eaten"},
                        {"fall", "fallen"},
                        {"feed", "fed"},
                        {"feel", "felt"},
                        {"fight", "fought"},
                        {"find", "found"},
                        {"flee", "fled"},
                        {"fling", "flung"},
                        {"fly", "flown"},
                        {"forbear", "forborne"},
                        {"forbid", "forbiden"},
                        {"fordo", "fordone"},
                        {"forego", "foregone"},
                        {"foreknow", "foreknown"},
                        {"forerun", "forerun"},
                        {"foresee", "foreseen"},
                        {"foreshow", "foreshown"},
                        {"foretell", "foretold"},
                        {"forget", "forgotten"},
                        {"forgive", "forgiven"},
                        {"forsake", "forsaken"},
                        {"forswear", "forsworn"},
                        {"freeze", "frozen"},
                        {"gainsay", "gainsaid"},
                        {"get", "gotten"},
                        {"give", "given"},
                        {"go", "gone"},
                        {"grind", "ground"},
                        {"grow", "grown"},
                        {"hang", "hung"},
                        {"hear", "heard"},
                        {"hide", "hidden"},
                        {"hit", "hit"},
                        {"hold", "held"},
                        {"hurt", "hurt"},
                        {"inlay", "inlaid"},
                        {"keep", "kept"},
                        {"kneel", "knelt"},
                        {"knit", "knitted"},
                        {"know", "known"},
                        {"lay", "laid"},
                        {"lead", "lead"},
                        {"lean", "leant"}, 
                        {"leap", "leapt"},
                        {"learn", "learnt"},
                        {"leave", "left"},
                        {"lend", "lent"},
                        {"let", "let"},
                        {"lie", "lain"},
                        {"light", "lit"},
                        {"lose", "lost"},
                        {"make", "made"},
                        {"may", "might"},
                        {"mean", "meant"},
                        {"meet", "met"},
                        {"melt", "melten"},
                        {"misdeal", "misdealt"},
                        {"misgive", "misgiven"},
                        {"mislay", "mislaid"},
                        {"mislead", "misled"},
                        {"mistake", "mistaken"},
                        {"misunderstand", "misunderstood"},
                        {"mow", "mown"},
                        {"ought", "ought"},
                        {"outbid", "outbidden"},
                        {"outbreed", "outbred"},
                        {"outdo", "outdone"},
                        {"outeat", "outeaten"},
                        {"outfight", "outfought"},
                        {"outgo", "outgone"},
                        {"outgrow", "outgrown"},
                        {"outlay", "outlaid"},
                        {"outride", "outridden"},
                        {"outrun", "outrun"},
                        {"outsell", "outsold"},
                        {"outshine", "outshone"},
                        {"outshoot", "outshot"},
                        {"outsit", "outsat"},
                        {"outspend", "outspent"},
                        {"outspread", "outspread"},
                        {"outthrow", "outthrown"},
                        {"outthrust", "outthrust"},
                        {"outwear", "outworn"},
                        {"overbear", "overborne"},
                        {"overbid", "overbidden"}, 
                        {"overblow", "overblown"},
                        {"overbuild", "overbuilt"},
                        {"overbuy", "overbought"},
                        {"overcast", "overcast"},
                        {"overcome", "overcome"},
                        {"overdo", "overdone"},
                        {"overdraw", "overdrawn"},
                        {"overdrive", "overdriven"},
                        {"overeat", "overeaten"},
                        {"overfeed", "overfed"},
                        {"overfly", "overflown"},
                        {"overgrow", "overgrown"},
                        {"overhang", "overhung"},
                        {"overhear", "overheard"},
                        {"overlade","overladen"},
                        {"overlay", "overlaid"},
                        {"overlie", "overlain"},
                        {"overpay", "overpaid"},
                        {"override", "overridden"},
                        {"overrun", "overrun"},
                        {"oversee", "overseen"},
                        {"oversell", "oversold"},
                        {"overset", "overset"},
                        {"oversew", "oversewn"},
                        {"overshoot", "overshot"},
                        {"oversleep", "overslept"},
                        {"overspend", "overspent"},
                        {"overspread", "overspread"},
                        {"overtake", "overtaken"},
                        {"overthrow", "overthrown"},
                        {"overwind", "overwound"},
                        {"overwrite", "overwritten"},
                        {"partake", "partaken"},
                        {"pay", "paid"},
                        {"precast", "precast"},
                        {"prechoose", "prechosen"},
                        {"prove", "proven"},
                        {"put", "put"},
                        {"quit", "quitted"},
                        {"read", "read"},
                        {"reave", "reaved"},
                        {"rebuild", "rebuilt"},
                        {"recast", "recast"},
                        {"reeve", "reeved"},
                        {"relay", "relaid"},
                        {"rend", "rent"},
                        {"repay", "repaid"},
                        {"reset", "reset"},
                        {"retell", "retold"},
                        {"rid", "ridded"},
                        {"ring", "rung"},
                        {"rise", "risen"},
                        {"rive", "rived"},
                        {"run", "run"},
                        {"saw", "sawed"},
                        {"say", "said"},
                        {"see", "seen"},
                        {"seek", "sought"},
                        {"sell", "sold"},
                        {"send", "sent"}
                    };
                }

                return _irregularVebPastParticipleFormatDict;
            }
        }


        private static Dictionary<int, VerbTenseFormatFunc> _verbTenseFormatFuncDict;
        /// <summary>
        /// 动词时态处理函数字典
        /// </summary>
        public static Dictionary<int, VerbTenseFormatFunc> VerbTenseFormatFuncDict
        {
            get
            {
                if (_verbTenseFormatFuncDict == null)
                {
                    _verbTenseFormatFuncDict = new Dictionary<int, VerbTenseFormatFunc>
                        {
                            {(int)TenseState.Normal+(int)TenseTiming.Past, GetVerbTenseFormatNormalPast},
                            {(int)TenseState.Normal+(int)TenseTiming.Now, GetVerbTenseFormatNormalNow},
                            {(int)TenseState.Normal+(int)TenseTiming.Future, GetVerbTenseFormatNormalFuture},
                            {(int)TenseState.Normal+(int)TenseTiming.PastFuture, GetVerbTenseFormatNormalPastFuture},

                            {(int)TenseState.Doing+(int)TenseTiming.Past, GetVerbTenseFormatDoingPast},
                            {(int)TenseState.Doing+(int)TenseTiming.Now, GetVerbTenseFormatDoingNow},
                            {(int)TenseState.Doing+(int)TenseTiming.Future, GetVerbTenseFormatDoingFuture},
                            {(int)TenseState.Doing+(int)TenseTiming.PastFuture, GetVerbTenseFormatDoingPastFuture},

                            {(int)TenseState.Finish+(int)TenseTiming.Past, GetVerbTenseFormatFinishPast},
                            {(int)TenseState.Finish+(int)TenseTiming.Now, GetVerbTenseFormatFinishNow},
                            {(int)TenseState.Finish+(int)TenseTiming.Future, GetVerbTenseFormatFinishFuture},
                            {(int)TenseState.Finish+(int)TenseTiming.PastFuture, GetVerbTenseFormatFinishPastFuture},

                            {(int)TenseState.FinishDoing+(int)TenseTiming.Past, GetVerbTenseFormatFinishDoingPast},
                            {(int)TenseState.FinishDoing+(int)TenseTiming.Now, GetVerbTenseFormatFinishDoingNow},
                            {(int)TenseState.FinishDoing+(int)TenseTiming.Future, GetVerbTenseFormatFinishDoingFuture},
                            {(int)TenseState.FinishDoing+(int)TenseTiming.PastFuture, GetVerbTenseFormatFinishDoingPastFuture}
                        };
                }

                return _verbTenseFormatFuncDict;
            }
        }

        /// <summary>
        /// 动词的一般过去时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatNormalPast(string verb, VerbTenseFormatInfo info)
        {
            return GetVerbPastFormat(verb);
        }

        /// <summary>
        /// 动词的一般现在时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatNormalNow(string verb, VerbTenseFormatInfo info)
        {
            var result = verb;
            if (Is3RdPersonalSingular(info.Subject) ||
                (!info.IsComplex && !IsPersonal(info.Subject)))
            {
                result = GetWordComplexFormat(verb);
            }

            return result;
        }

        /// <summary>
        /// 动词的一般将来时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatNormalFuture(string verb, VerbTenseFormatInfo info)
        {
            if (IsAuxilizryVerb(verb))
            {
                verb = "be";
            }

            var result = "will " + verb;
            if (Is1StPersonal(info.Subject))
            {
                result = "shall " + verb;
            }

            return result;
        }

        /// <summary>
        /// 动词的一般过去将来时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatNormalPastFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var specials = new[] { "come", "go", "leave", "arrive", "start" };
            var v = verb.ToLower();

            if (specials.Contains(v))
            {
                if (Is2NdPersonal(info.Subject) || IsPersonalComplex(info.Subject) || info.IsComplex)
                {
                    return "were " + GetVerbDoingFormat(verb);
                }
                if (IsPersonal(info.Subject) || !info.IsComplex)
                {
                    return "was " + GetVerbDoingFormat(verb);
                }
            }

            return "would " + verb;
        }

        /// <summary>
        /// 动词的过去进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatDoingPast(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is2NdPersonal(info.Subject) || IsPersonalComplex(info.Subject) || info.IsComplex)
            {
                return "were " + doingVerb;
            }

            return "was " + doingVerb;
        }

        /// <summary>
        /// 动词的现在进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatDoingNow(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is2NdPersonal(info.Subject) || IsPersonalComplex(info.Subject) || info.IsComplex)
            {
                return "are " + doingVerb;
            }
            if (Is1StPersonal(info.Subject))
            {
                return "am " + doingVerb;
            }

            return "is " + doingVerb;
        }

        /// <summary>
        /// 将来进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatDoingFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is1StPersonal(info.Subject))
            {
                return "shall be " + doingVerb;
            }

            return "will be " + doingVerb;
        }

        /// <summary>
        /// 过去将来进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatDoingPastFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is1StPersonal(info.Subject))
            {
                return "should be " + doingVerb;
            }

            return "would be " + doingVerb;
        }

        /// <summary>
        /// 过去完成时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishPast(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var pastVerb = GetVerbPastParticipleFormat(verb);


            return "had " + pastVerb;
        }

        /// <summary>
        /// 现在完成时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishNow(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var pastVerb = GetVerbPastParticipleFormat(verb);
  

            if (Is1StPersonal(info.Subject) || Is2NdPersonal(info.Subject) || info.IsComplex)
            {
                return "have " + pastVerb;
            }

            return "has " + pastVerb;
        }

        /// <summary>
        /// 将来完成时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var pastVerb = GetVerbPastParticipleFormat(verb);


            if (Is1StPersonal(info.Subject))
            {
                return "shall have " + pastVerb;
            }

            return "will have " + pastVerb;
        }

        /// <summary>
        /// 过去将来完成时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishPastFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var pastVerb = GetVerbPastParticipleFormat(verb);
            

            if (Is1StPersonal(info.Subject))
            {
                return "should have " + pastVerb;
            }

            return "would have " + pastVerb;
        }

        /// <summary>
        /// 过去完成进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishDoingPast(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            return "had been " + doingVerb;
        }

        /// <summary>
        /// 现在完成进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishDoingNow(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is1StPersonal(info.Subject) || Is2NdPersonal(info.Subject) || info.IsComplex)
            {
                return "have been " + doingVerb;
            }

            return "has been " + doingVerb;
        }

        /// <summary>
        /// 将来完成进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishDoingFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is1StPersonal(info.Subject))
            {
                return "shall have " + doingVerb;
            }

            return "will have " + doingVerb;
        }

        /// <summary>
        /// 过去将来完成进行时
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetVerbTenseFormatFinishDoingPastFuture(string verb, VerbTenseFormatInfo info)
        {
            if (string.IsNullOrEmpty(verb))
            {
                return null;
            }

            var doingVerb = GetVerbDoingFormat(verb);
            if (Is1StPersonal(info.Subject))
            {
                return "should have been " + doingVerb;
            }

            return "would have been " + doingVerb;
        }
        
        private static Dictionary<string, string> _irregularNounPluralFormDict;
        /// <summary>
        /// 不规则名词复数字典
        /// </summary>
        public static Dictionary<string, string> IrregularNounPluralFormDict
        {
            get
            {
                if (_irregularNounPluralFormDict == null)
                {
                    _irregularNounPluralFormDict = new Dictionary<string, string>()
                    {
                        // 特殊形式的复数
                        {"man", "men"},
                        {"woman", "women"},

                        // 以"o"结尾， 有些加s, 有些加es, 这里将加了es的当做不规则处理
                        {"potato", "potatoes"},
                        {"tomato", "tomatoes"},
                        {"hero", "heroes"},
                        {"negro", "negroes"},

                        // 单复数形式相同
                        {"sheep", "sheep"},
                        {"deer", "deer"},
                        {"chinese", "chinese"},
                        {"japanese", "japanese"},
                        {"swiss", "swiss"},

                        // 以f/fe结尾， 但不需要变f/fe为ve, 再加es, 而是直接加s的情况， 视为不规则处理
                        {"belief", "beliefs"},
                        {"roof", "roofs"},
                        {"safe", "safes"},
                        {"gulf", "gulfs"}
                    };
                }

                return _irregularNounPluralFormDict;
            }
        }

        private static Dictionary<string, string> _irregularNounIndefiniteArticleDict;
        public static Dictionary<string, string> IrregularNounIndefiniteArticleDict
        {
            get 
            {
                if (_irregularNounIndefiniteArticleDict == null)
                    _irregularNounIndefiniteArticleDict = new Dictionary<string, string>() 
                    {
                        {"hour", "an"},
                        {"honor", "an"},
                        {"honest", "an"},
                        {"university", "a"},
                        {"unit", "a"},
                        {"uniform", "a"},
                        {"usual", "a"},
                        {"united", "a"},
                        {"European", "a"}
                    };

                return _irregularNounIndefiniteArticleDict;
            }
        }
             

        /// <summary>
        /// 获取名词的复数形式
        /// </summary>
        /// <param name="noun">名词单数</param>
        /// <returns></returns>
        public static string GetNounPluralForm(string noun)
        {
            string result;
            var n = noun.ToLower();

            // 1. 不规则名词复数处理
            if (IrregularNounPluralFormDict.TryGetValue(n, out result))
            {
                return result;
            }
            
            // 2. 以s, sh, ch, x, o结尾， 加es
            if (n.EndsWith("s") || n.EndsWith("sh") || n.EndsWith("ch") || n.EndsWith("x") || n.EndsWith("o"))
            {
                result = n + "es";
                return result;
            }

            // 3. 以f/fe结尾， 变f/fe为v， 再加es
            if (n.EndsWith("f"))
            {
                result = n.Substring(0, n.Length - 1) + "ves";
                return result;
            }
            if (n.EndsWith("fe"))
            {
                result = n.Substring(0, n.Length - 2) + "ves";
                return result;
            }

            // 4. 以辅音字母+y结尾， 变y为i, 再加es
            if (n.EndsWith("y") && (!"aeoiu".Contains(n[n.Length - 2])))
            {
                result = n.Substring(0, n.Length - 1) + "ies";
                return result;
            }

            // 5. 其他为一般情况， 在名词末尾加s即可
            result = n + "s";
            return result;
        }

        public static void 英文名词a与an处理(ref string resultString)
        {
            string result = null;  // 返回的结果不定冠词
            // TODO: 1. 特殊名词处理
            if (IrregularNounIndefiniteArticleDict.TryGetValue(resultString, out result))
                resultString = result + " " + resultString;
            else
            {
                // 2. 普通名词处理
                char[] 元音字母数组 = { 'a', 'A', 'e', 'E', 'i', 'I', 'o', 'O', 'u', 'U' };
                if (元音字母数组.Contains(resultString[0]))
                    resultString = "an " + resultString;
                else
                    resultString = "a " + resultString;
            }            
        }

        public static void 英文名词复数处理(ref string resultString)
        {
            resultString = WordProcessorEnglish.GetNounPluralForm(resultString);
        }
    }

    /// <summary>
    /// 时态状态
    /// </summary>
    public enum TenseState
    {
        /// <summary>
        /// 一般
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 进行
        /// </summary>
        Doing = 2,
        /// <summary>
        /// 完成
        /// </summary>
        Finish = 3,
        /// <summary>
        /// 完成进行
        /// </summary>
        FinishDoing = 4
    }

    /// <summary>
    /// 时态时间
    /// </summary>
    public enum TenseTiming
    {
        /// <summary>
        /// 现在
        /// </summary>
        Now = 10,
        /// <summary>
        /// 过去
        /// </summary>
        Past = 20,
        /// <summary>
        /// 将来
        /// </summary>
        Future = 30,
        /// <summary>
        /// 过去将来
        /// </summary>
        PastFuture = 40
    }

    /// <summary>
    /// 动词时态形式的附加信息
    /// </summary>
    public class VerbTenseFormatInfo
    {
        public VerbTenseFormatInfo(string subject = null, bool isComplex = false)
        {
            Subject = subject;
            IsComplex = isComplex;
        }

        /// <summary>
        /// 主语
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 是否复数
        /// </summary>
        public bool IsComplex { get; set; }
    }

    public delegate string VerbTenseFormatFunc(string verb, VerbTenseFormatInfo info);
}
