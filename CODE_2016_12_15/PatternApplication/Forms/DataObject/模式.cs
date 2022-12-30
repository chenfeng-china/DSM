using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternApplication.DataObject
{
    /// <summary>
    /// 模式类
    /// </summary>
    public class 模式 : ParentOwnedBaseObject<Guid>
    {
        private int _序号;
        private int _语言;
        private string _形式;
        private Guid _源记录;
        private Guid _A端;
        private Guid _连接;
        private Guid _B端;
        private int _That根;
        private int _的;
        private short _显隐;
        private int _语言角色;
        private Guid _C端;
        private short _通用分类;
        private string _说明;
        private string _参数集合;
        private float _成立度;
        private short _实例数;
        private float _Aα;
        private float _Aβ;
        private float _Bα;
        private float _Bβ;
        private float _联α;
        private float _联β;
        private Guid _说话时间;
        private Guid _说话地点;
        private int _全等引用计数;
        private int _级别;
        private int _认知年龄;
        private int _附加信息;
        private int _关系距离;
        private int _语境树;
        private byte _层级;
        private string _风格;
        private string _打分;
        public Dictionary<Guid, bool> 派生类判断结果缓存表 = new Dictionary<Guid, bool>();
        public bool 被替换标记 = false;
        //public List<派生类判断结果> 派生类判断结果缓存表 = new List<派生类判断结果>();
        public 模式() { }

        public 模式(Guid id, Guid parentID, int _序号, int _语言, string _形式, Guid _源记录, Guid _A端, Guid _连接, Guid _B端, int _That根,
            int _的, short _显隐, int _语言角色, Guid _C端, short _通用分类, string _说明, string _参数集合, float _成立度, short _实例数,
            float _Aα, float _Aβ, float _Bα, float _Bβ, float _联α, float _联β, Guid _说话时间, Guid _说话地点, int _全等引用计数,
            int _级别, int _认知年龄, int _附加信息, int _关系距离, int _语境树, byte _层级, string _风格, string _打分)
            : base(id, parentID)
        {
            this._序号 = _序号;
            this._语言 = _语言;
            this._形式 = _形式;
            this._源记录 = _源记录;
            this._A端 = _A端;
            this._连接 = _连接;
            this._B端 = _B端;
            this._That根 = _That根;
            this._的 = _的;
            this._显隐 = _显隐;
            this._语言角色 = _语言角色;
            this._C端 = _C端;
            this._通用分类 = _通用分类;
            this._说明 = _说明;
            this._参数集合 = _参数集合;
            this._成立度 = _成立度;
            this._实例数 = _实例数;
            this._Aα = _Aα;
            this._Aβ = _Aβ;
            this._Bα = _Bα;
            this._Bβ = _Bβ;
            this._联α = _联α;
            this._联β = _联β;
            this._说话时间 = _说话时间;
            this._全等引用计数 = _全等引用计数;
            this._级别 = _级别;
            this._认知年龄 = _认知年龄;
            this._附加信息 = _附加信息;
            this._关系距离 = _关系距离;
            this._语境树 = _语境树;
            this._层级 = _层级;
            this._风格 = _风格;
            this._打分 = _打分;
        }


        public 模式(模式 item)
            : this(item.ID, item.ParentID, item.序号, item.语言, item.形式, item.源记录, item.A端, item.连接, item.B端,
                item.That根, item.的, item.显隐, item.语言角色, item.C端, item.通用分类, item.说明, item.参数集合, item.成立度, item.实例数, item.Aα,
                item.Aβ, item.Bα, item.Bβ, item.联α, item.联β, item.说话时间, item.说话地点, item.全等引用计数, item.级别, item.认知年龄, item.附加信息,
                item.关系距离, item.语境树, item.层级, item.风格, item.打分) { }

        public 模式(bool isNew)
        {
            this.IsNew = isNew;
        }


        /// <summary>
        /// 序号
        /// </summary>
        public int 序号
        {
            get { return this._序号; }
            set
            {
                if (this._序号 != value)
                {
                    int oldValue = this._序号;
                    this._序号 = value;
                    OnChanged("序号", oldValue, _序号);
                }
            }
        }

        /// <summary>
        /// 语言
        /// </summary>
        public int 语言
        {
            get { return _语言; }
            set
            {
                if (this._语言 != value)
                {
                    int oldValue = _语言;
                    _语言 = value;
                    OnChanged("语言", oldValue, _语言);
                }
            }
        }

        /// <summary>
        /// 形式
        /// </summary>
        public string 形式
        {
            get { return _形式; }
            set
            {
                if (this._形式 != value)
                {
                    var oldValue = this._形式;
                    this._形式 = value;
                    OnChanged("形式", oldValue, _形式);
                }
            }
        }

        /// <summary>
        /// 源记录Id
        /// </summary>
        public Guid 源记录
        {
            get { return _源记录; }
            set
            {
                if (this._源记录 != value)
                {
                    var oldValue = this._源记录;
                    this._源记录 = value;
                    OnChanged("源记录", oldValue, _源记录);
                }
            }
        }

        /// <summary>
        /// A端Id
        /// </summary>
        public Guid A端
        {
            get { return _A端; }
            set
            {
                if (this._A端 != value)
                {
                    var oldValue = this._A端;
                    this._A端 = value;
                    OnChanged("A端", oldValue, _A端);
                }
            }
        }

        /// <summary>
        /// 连接Id
        /// </summary>
        public Guid 连接
        {
            get { return _连接; }
            set
            {
                if (this._连接 != value)
                {
                    var oldValue = _连接;
                    _连接 = value;
                    OnChanged("连接", oldValue, _连接);
                }
            }

        }

        /// <summary>
        /// B端Id
        /// </summary>
        public Guid B端
        {
            get { return _B端; }
            set
            {
                if (_B端 != value)
                {
                    var oldValue = _B端;
                    _B端 = value;
                    OnChanged("B端", oldValue, _B端);
                }
            }
        }

		/// <summary> 
		/// B端Id
		/// </summary>
        private 参数字段 _参数;
		public 参数字段 参数
		{
			get {
                if (_参数==null)
                    _参数 = new 参数字段(参数集合);
                return _参数;
                    //return new 参数字段(参数集合);
			}
			set
			{
				string s = value.ToString();
				if (s != 参数集合)
					参数集合 = s;
			}
		}

        /// <summary>
        /// That根
        /// </summary>
        public int That根
        {
            get { return _That根; }
            set
            {
                if (this._That根 != value)
                {
                    var oldValue = _That根;
                    _That根 = value;
                    OnChanged("That根", oldValue, _That根);
                }
            }
        }

        /// <summary>
        /// 的
        /// </summary>
        public int 的
        {
            get { return _的; }
            set
            {
                if (this._的 != value)
                {
                    var oldValue = _的;
                    _的 = value;
                    OnChanged("的", oldValue, _的);
                }
            }
        }

        /// <summary>
        /// 显隐
        /// </summary>
        public short 显隐
        {
            get { return _显隐; }
            set
            {
                if (this._显隐 != value)
                {
                    var oldValue = _显隐;
                    _显隐 = value;
                    OnChanged("显隐", oldValue, _显隐);
                }
            }
        }

        /// <summary>
        /// 语言角色
        /// </summary>
        public int 语言角色
        {
            get { return _语言角色; }
            set
            {
                if (this._语言角色 != value)
                {
                    var oldValue = this._语言角色;
                    this._语言角色 = value;
                    OnChanged("语言角色", oldValue, _语言角色);
                }
            }
        }

        /// <summary>
        /// C端
        /// </summary>
        public Guid C端
        {
            get { return _C端; }
            set
            {
                if (this._C端 != value)
                {
                    var oldValue = _C端;
                    _C端 = value;
                    OnChanged("C端", oldValue, _C端);
                }
            }
        }

        /// <summary>
        /// 通用分类
        /// </summary>
        public short 通用分类
        {
            get { return _通用分类; }
            set
            {
                if (this._通用分类 != value)
                {
                    var oldValue = this._通用分类;
                    _通用分类 = value;
                    OnChanged("通用分类", oldValue, _通用分类);
                }
            }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string 说明
        {
            get { return _说明; }
            set
            {
                if (this._说明 != value)
                {
                    var oldValue = this._说明;
                    this._说明 = value;
                    OnChanged("说明", oldValue, _说明);
                }
            }
        }

        /// <summary>
        /// 参数集合
        /// </summary>
        public string 参数集合
        {
            get { return _参数集合; }
            set
            {
                if (_参数集合 != value)
                {
                    var oldValue = _参数集合;
                    _参数集合 = value;
                    _参数 = new 参数字段(value);
                    OnChanged("参数集合", oldValue, _参数集合);
                }
            }
        }

        /// <summary>
        /// 成立度
        /// </summary>
        public float 成立度
        {
            get { return _成立度; }
            set
            {
                if (_成立度 != value)
                {
                    var oldValue = _成立度;
                    _成立度 = value;
                    OnChanged("成立度", oldValue, _成立度);
                }
            }
        }

        /// <summary>
        /// 实例数
        /// </summary>
        public short 实例数
        {
            get { return _实例数; }
            set
            {
                if (_实例数 != value)
                {
                    var oldValue = _实例数;
                    _实例数 = value;
                    OnChanged("实例数", oldValue, _实例数);
                }
            }
        }

        /// <summary>
        /// Aα
        /// </summary>
        public float Aα
        {
            get { return _Aα; }
            set
            {
                if (_Aα != value)
                {
                    var oldValue = _Aα;
                    _Aα = value;
                    OnChanged("Aα", oldValue, _Aα);
                }
            }
        }

        /// <summary>
        /// Aβ
        /// </summary>
        public float Aβ
        {
            get { return _Aβ; }
            set
            {
                if (_Aβ != value)
                {
                    var oldValue = _Aβ;
                    _Aβ = value;
                    OnChanged("Aβ", oldValue, _Aβ);
                }
            }
        }

        /// <summary>
        /// Bα
        /// </summary>
        public float Bα
        {
            get { return _Bα; }
            set
            {
                if (_Bα != value)
                {
                    var oldValue = _Bα;
                    _Bα = value;
                    OnChanged("Bα", oldValue, _Bα);
                }
            }
        }

        /// <summary>
        /// Bβ
        /// </summary>
        public float Bβ
        {
            get { return _Bβ; }
            set
            {
                if (_Bβ != value)
                {
                    var oldValue = _Bα;
                    _Bα = value;
                    OnChanged("Bβ", oldValue, _Bβ);
                }
            }
        }


        /// <summary>
        /// 联α
        /// </summary>
        public float 联α
        {
            get { return _联α; }
            set
            {
                if (_联α != value)
                {
                    var oldValue = _联α;
                    _联α = value;
                    OnChanged("联α", oldValue, _联α);
                }
            }
        }

        /// <summary>
        /// 联β
        /// </summary>
        public float 联β
        {
            get { return _联β; }
            set
            {
                if (_联β != value)
                {
                    var oldValue = _联β;
                    _联β = value;
                    OnChanged("联β", oldValue, _联β);
                }
            }
        }

        /// <summary>
        /// 说话时间
        /// </summary>
        public Guid 说话时间
        {
            get { return _说话时间; }
            set
            {
                if (_说话时间 != value)
                {
                    var oldValue = _说话时间;
                    _说话时间 = value;
                    OnChanged("说话时间", oldValue, _说话时间);
                }
            }
        }

        /// <summary>
        /// 说话地点
        /// </summary>
        public Guid 说话地点
        {
            get { return _说话地点; }
            set
            {
                if (_说话地点 != value)
                {
                    var oldValue = _说话地点;
                    _说话地点 = value;
                    OnChanged("说话地点", oldValue, _说话地点);
                }
            }
        }

        /// <summary>
        /// 全等引用计数
        /// </summary>
        public int 全等引用计数
        {
            get { return _全等引用计数; }
            set
            {
                if (_全等引用计数 != value)
                {
                    var oldValue = this._全等引用计数;
                    _全等引用计数 = value;
                    OnChanged("全等引用计数", oldValue, _全等引用计数);
                }
            }
        }

        /// <summary>
        /// 级别
        /// </summary>
        public int 级别
        {
            get { return _级别; }
            set
            {
                if (_级别 != value)
                {
                    var oldValue = _级别;
                    _级别 = value;
                    OnChanged("级别", oldValue, _级别);
                }
            }
        }

        /// <summary>
        /// 认知年龄
        /// </summary>
        public int 认知年龄
        {
            get { return _认知年龄; }
            set
            {
                if (_认知年龄 != value)
                {
                    var oldValue = _认知年龄;
                    _认知年龄 = value;
                    OnChanged("认知年龄", oldValue, _认知年龄);
                }
            }
        }

        /// <summary>
        /// 附加信息
        /// </summary>
        public int 附加信息
        {
            get { return _附加信息; }
            set
            {
                if (_附加信息 != value)
                {
                    var oldValue = _附加信息;
                    _附加信息 = value;
                    OnChanged("附加信息", oldValue, _附加信息);
                }
            }
        }

        /// <summary>
        /// 关系距离
        /// </summary>
        public int 关系距离
        {
            get { return _关系距离; }
            set
            {
                if (_关系距离 != value)
                {
                    var oldValue = _关系距离;
                    _关系距离 = value;
                    OnChanged("关系距离", oldValue, _关系距离);
                }
            }
        }

        /// <summary>
        /// 语境树
        /// </summary>
        public int 语境树
        {
            get { return _语境树; }
            set
            {
                if (_语境树 != value)
                {
                    var oldValue = _语境树;
                    _语境树 = value;
                    OnChanged("语境树", oldValue, _语境树);
                }
            }
        }

        /// <summary>
        /// 层级
        /// </summary>
        public byte 层级
        {
            get { return _层级; }
            set
            {
                if (_层级 != value)
                {
                    var oldValue = _层级;
                    _层级 = value;
                    OnChanged("层级", oldValue, _层级);
                }
            }
        }

        /// <summary>
        /// 风格
        /// </summary>
        public string 风格
        {
            get { return _风格; }
            set
            {
                if (_风格 != value)
                {
                    var oldValue = _风格;
                    _风格 = value;
                    OnChanged("风格", oldValue, _风格);
                }
            }
        }

        /// <summary>
        /// 打分
        /// </summary>
        public string 打分
        {
            get { return _打分; }
            set
            {
                if (_打分 != value)
                {
                    var oldValue = _打分;
                    _打分 = value;
                    OnChanged("打分", oldValue, _打分);
                }
            }
        } 

        private bool _bounding = true;
        /// <summary>
        ///  是否绑定
        /// </summary>
        public bool Bounding
        {
            get { return _bounding; }
            set { _bounding = value; }
        }

        /// <summary>
        /// 是否处于查询可见状态
        /// </summary>
        public bool isSearching { get; set; }


        public override string ToString()
        {
            return 形式;
        }

        /// <summary>
        /// 算子集合
        /// </summary>
        public readonly List<Guid> 算子集合List = new List<Guid>();
        
        //private List<端索引> _端索引表;
        //public List<端索引> 端索引表
        //{
        //    get
        //    {
        //        if (_端索引表 == null)
        //        {
        //            _端索引表 = new List<端索引>();
        //        }

        //        return _端索引表;
        //    }
        //}
        private List<模式> _端索引表_A端;
        public List<模式> 端索引表_A端
        {
            get
            {
                if (_端索引表_A端 == null)
                {
                    _端索引表_A端 = new List<模式>();
                }

                return _端索引表_A端;
            }
        }
        private List<模式> _端索引表_B端;
        public List<模式> 端索引表_B端
        {
            get
            {
                if (_端索引表_B端 == null)
                {
                    _端索引表_B端 = new List<模式>();
                }

                return _端索引表_B端;
            }
        }
        private List<模式> _端索引表_源记录;
        public List<模式> 端索引表_源记录
        {
            get
            {
                if (_端索引表_源记录 == null)
                {
                    _端索引表_源记录 = new List<模式>();
                }

                return _端索引表_源记录;
            }
        }
        private List<模式> _端索引表_Parent;
        public List<模式> 端索引表_Parent
        {
            get
            {
                if (_端索引表_Parent == null)
                {
                    _端索引表_Parent = new List<模式>();
                }

                return _端索引表_Parent;
            }
        }
    }
	public class 端索引 : IEquatable<端索引>
	{
		public int that端;
		public 模式 模式Row;

		public 端索引(int _that端, 模式 _模式Row)
		{
			this.that端 = _that端;
			this.模式Row = _模式Row;
		}

		public bool Equals(端索引 other)
		{
			return this.that端 == other.that端 && this.模式Row.ID.Equals(other.模式Row.ID);
		}

        override public string ToString()
        {
            return that端.ToString() + "：" + 模式Row.ToString();
        }

	}
}
