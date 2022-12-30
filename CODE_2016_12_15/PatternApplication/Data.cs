using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using PatternApplication.DataObject;
using PatternApplication.Interfaces;
using PatternApplication;
using PatternApplication.Forms;
using WordProcessor;

namespace PatternApplication
{

	public class 关联替代
	{
		public Guid 关联ID;

		public int A对B的替换性;//这个关联的A端对B端的替代能力。
		public int B对A的替换性;//这个关联的B端对A端的替代能力。
		public int A端可被替代性;
		public int B端可被替代性;

		public 关联替代(Guid 关联ID, int A对B的替换性, int B对A的替换性, int A端可被替代性, int B端可被替代性)
		{
			this.关联ID = 关联ID;
			this.A对B的替换性 = A对B的替换性;
			this.B对A的替换性 = B对A的替换性;
			this.A端可被替代性 = A端可被替代性;
			this.B端可被替代性 = B端可被替代性;
		}
	}

	public struct 整数对
	{
		public int beginindex;
		public int endindex;
		public 整数对(int begin = 0, int end = 0)
		{
			beginindex = begin;
			endindex = end;
		}
	}

	public class 介动词后生存记录
	{
		public string 生存串;
		public Guid 对应生存阶段;
		public 介动词后生存记录(string 串, Guid 生存阶段)
		{
			生存串 = 串;
			对应生存阶段 = 生存阶段;
		}
		public static 介动词后生存记录[] 介动词后生存集合 = new 介动词后生存记录[]
        {
            new 介动词后生存记录("了",Data.完成Guid),
            new 介动词后生存记录("着",Data.进行Guid),
            new 介动词后生存记录("过",Data.过去完成Guid)
        };
	}

	public class 基准类型
	{
		public Guid id;
		public int 层级;

		public 基准类型(Guid id, int 层级)
		{
			this.id = id;
			this.层级 = 层级;
		}

		//必须根据派生关系倒排。才好计算。
		static public 基准类型[] 基准类型结构 = new 基准类型[]
		{
			new 基准类型(Data.事物概念Guid,5),
			new 基准类型(Data.时间点Guid,4),
			new 基准类型(Data.时间量Guid,4),
			new 基准类型(Data.代词Guid,3),
			new 基准类型(Data.路径区间Guid,4),
			new 基准类型(Data.表达式Guid,4),
			new 基准类型(Data.并列集合Guid,4),
			new 基准类型(Data.形式集合Guid,3),
			new 基准类型(Data.抽象形式集合Guid,2),
			new 基准类型(Data.推导即命题间关系Guid,2),
			new 基准类型(Data.实体概念Guid,2),
			new 基准类型(Data.度量Guid,2),
			new 基准类型(Data.值概念Guid,1),
			new 基准类型(Data.概念Guid,0)
		};

		static public 基准类型 找到匹配的基准类型(Guid id)
		{
			foreach (基准类型 o in 基准类型结构)
				if (id.Equals(o.id))
					return o;
			return null;
		}
	}

	public class 推理角色
	{
		public 模式 目标;
		public 推导 推导;
		public bool 是基本推理角色;
		public bool 作为中心;//这个推理角色可作为中心，比如【所以】【因此】【那么】
		public bool 放在左端;//这个推理角色一般放在左端，比如【之所以】。
		public 推理角色(模式 源记录)
		{
			目标 = 源记录;
			是基本推理角色 = Data.是子记录(源记录, Data.推理角色Guid, false);
		}
	}

	public class 推导
	{
		public 模式 目标;
		public bool 是基本推导;
		public 推导(模式 源记录)
		{
			目标 = 源记录;
			是基本推导 = Data.是子记录(源记录, Data.基本推导组织Guid, false);
		}

	}

	public struct 替代
	{
		static public int 无 = 0;
		static public int 正向替代 = 1;//【苹果对水果的替代】
		static public int 反向替代 = 2;//【水果对苹果的替代】
		static public int 等价替代 = 4;//【双向】这个一般总是允许的。

		static public int 聚合替代 = 8;//【借出者被人】的替代。也就是反向扮演

		//扮演一般是单向的。人可以扮演【借入者】，而【借入者】就不会替代人。

		static public 关联替代[] 替代结构 = new 关联替代[]
        {
			new 关联替代(Data.引用Guid,   无,  无,   无,   无 ),

            new 关联替代(Data.属于Guid,   正向替代,  反向替代,   正向替代 | 等价替代,                     反向替代 | 等价替代 ),
            new 关联替代(Data.包括Guid,   反向替代,  正向替代,   反向替代 | 等价替代,                     正向替代 | 等价替代 ),
            new 关联替代(Data.等价Guid,   正向替代|反向替代|等价替代,  正向替代|反向替代|等价替代,等价替代,等价替代 ),

            //new 关联替代(Data.扮演Guid,   反向聚合替代,  无,             正向替代 | 等价替代,                    正向替代 | 等价替代  ),
            new 关联替代(Data.聚合Guid,   聚合替代,  聚合替代,             正向替代 | 等价替代,                    正向替代 | 等价替代  ),
            

			new 关联替代(Data.属拥Guid,   无,            无,             正向替代 | 等价替代 | 聚合替代,    正向替代 | 等价替代 | 聚合替代  ),
			new 关联替代(Data.拥有Guid,   无,            无,             正向替代 | 等价替代  | 聚合替代,    正向替代 | 等价替代  | 聚合替代  ),
            new 关联替代(Data.拥有形式Guid,   无,            无,             无,         无  ),//概念拥有形式的两边都不能被替代。
	
			new 关联替代(Data.并列关联Guid,   无,            无,             正向替代 | 等价替代 ,    正向替代 | 等价替代),

			new 关联替代(Data.松散并列Guid,   无,            无,             正向替代 | 等价替代 ,    正向替代 | 等价替代),

            new 关联替代(Data.基本关联Guid,   无,            无,             等价替代,                                    等价替代  ),//参数太少，所以别的处理都不允许。

            new 关联替代(Data.推导即命题间关系Guid,   无,            无,             无,         无  ),
            new 关联替代(Data.二元比较关系Guid,   无,            无,             无,         无  ),

            new 关联替代(Data.NullGuid,   无,            无,             无,         无  )

        };

		static public bool 是本质分类(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.包括Guid.Equals(一级基本关联id) || Data.等价Guid.Equals(一级基本关联id))
				return true;
			return false;
		}

		static public bool 是本质正向分类(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.等价Guid.Equals(一级基本关联id))
				return true;
			return false;
		}

		static public bool 是属于等价或聚合(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.包括Guid.Equals(一级基本关联id) || Data.等价Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 是属于或聚合(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 是分类或聚合或属拥(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.包括Guid.Equals(一级基本关联id) || Data.等价Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id) || Data.属拥Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 是等价或聚合(Guid 一级基本关联id)
		{
			if (Data.等价Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id))
				return true;
			return false;
		}

		static public bool 是等价或聚合或属拥(Guid 一级基本关联id)
		{
			if (Data.等价Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id) || Data.属拥Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 是聚合或者属拥(Guid 一级基本关联id)
		{
			if (Data.属拥Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 是属拥(Guid 一级基本关联id)
		{
			if (Data.属拥Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}
		static public bool 可继承参数(Guid 一级基本关联id)
		{
			if (Data.属于Guid.Equals(一级基本关联id) || Data.等价Guid.Equals(一级基本关联id) || Data.属拥Guid.Equals(一级基本关联id) || Data.聚合Guid.Equals(一级基本关联id)/*|| Data.扮演Guid.Equals(一级基本关联id)*/)
				return true;
			return false;
		}

		static public bool 可正向替代(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
				if (一级基本关联id.Equals(o.关联ID) && (o.A对B的替换性 & (替代.正向替代 | 替代.等价替代 | 替代.聚合替代)) > 0)
					return true;
			return false;
		}
		static public bool 可反向替代(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
			{
				if (一级基本关联id.Equals(o.关联ID) && (o.A对B的替换性 & (替代.反向替代 | 替代.等价替代)) > 0)
					return true;
			}
			return false;
		}

		/*
		关联类型           A端被替代   	    B端被替代	        B替代A	        A替代B
		[A]属于[B]	    >正向替代	        <反向替代	        <反向替代	    >正向替代
		[A]扮演[B]	     >正向替代	    >正向替代	        不能	            >正向替代
		[A]等价[B]	    等价替代	        等价替代	        等价替代	    等价替代
		[A]拥有[B]	    >正向替代       	>正向替代       	不能	        不能
		[A]属拥[B]	    >正向替代	        >正向替代	        不能	        不能
		正向指用派生类替代基类，反向指用基类替代派生类。其中等价替代总是可用的，就不单独说明！！				
		具体查找的时候，还是按正向方向处理，也就是从派生类向基类方向找寻。但实现有明确的目的。				
		 */

		//这个关联的另一端（一般就是B端）设定的概念允许被其派生概念替换。
		//1、纯的【属于】（包括【扮演】）不允许。比如【苹果属于水果】里边的【水果】不能被替换为【香蕉】。
		//2、拥有和属拥（本质也是一种拥有）都允许被替换。比如【苹果拥有颜色】的【颜色】可以替换为【红色】。【借属拥生存】的【生存】可以替换为【发生】。
		//public int 获得关联的B端允许的被替代类型()
		//{
		//    Guid 一级关联 = Data.一级关联类型(目标);
		//    if (替代.可正向替代(一级关联) == false)//不是【分类】关联（通常就是【拥有关联】），总是可以被派生和扮演替换的。
		//        return 替代.本质正向替代 | 替代.正向聚合替代;
		//    DataRow row = Data.FindRowByID((Guid)目标["源记录"]);
		//    if (Data.是派生关联(Data.属拥Guid, row) > 0)//【属拥】可以被
		//        return 替代.本质正向替代;
		//    if (Data.是派生关联(Data.扮演Guid, row) > 0)//【扮演】也可以被派生替换。
		//        return 替代.本质正向替代;
		//    return 0;//纯的【属于】的B端必须是显式的本身概念，不能被派生概念替换。
		//}

		static public int A端可被替换性(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
			{
				if (一级基本关联id.Equals(o.关联ID))
					return o.A端可被替代性;
			}
			Data.Assert(false);
			return 0;
		}
		static public int B端可被替换性(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
			{
				if (一级基本关联id.Equals(o.关联ID))
					return o.B端可被替代性;
			}
			Data.Assert(false);
			return 0;
		}
		static public int A端对B端替换性(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
			{
				if (一级基本关联id.Equals(o.关联ID))
					return o.A对B的替换性;
			}
			Data.Assert(false);
			return 0;
		}
		static public int B端对A端替换性(Guid 一级基本关联id)
		{
			foreach (关联替代 o in 替代.替代结构)
			{
				if (一级基本关联id.Equals(o.关联ID))
					return o.B对A的替换性;
			}
			Data.Assert(false);
			return 0;
		}
	}



	public static class 符合度
	{
		//大致是从0到10。
		public const float 最大符合度 = 10;
		public const float 最小符合度 = 0;
		public static float 符合度乘积(float 符合度1, float 符合度2)
		{
			if (符合度1 == 最小符合度 || 符合度2 == 最小符合度)
				return 最小符合度;
			if (符合度1 == 最大符合度)
				return 符合度2;
			if (符合度2 == 最大符合度)
				return 符合度1;
			//这里计算不是很对，因为不是真正的概率，但暂时先这样吧。
			float v = (int)((符合度1 * 符合度2) / 10);
			return v;
		}
	}


	public class 参数树结构遍历位置
	{
		public 参数树结构 根对象;
		public 参数树结构 当前对象;
		//public int 当前子节点;
		public 参数树结构遍历位置(参数树结构 根)
		{
			根对象 = 根;
			当前对象 = 根;
			//当前子节点 = 0;
		}
		public 参数树结构 下一个兄弟()
		{
			if (当前对象 == 根对象)
				当前对象 = null;

			return 当前对象;
		}
	}

	public class 记录树
	{
		public 模式 目标行;
		public List<记录树> 子节点;
		public static 记录树 形成一个记录树(模式 根记录)
		{
			记录树 tree = new 记录树();
			tree.目标行 = 根记录;
			//var dr = Data.FindTableByID(根记录.ID).对象集合.Where(r => r.ParentID == 根记录.ID).ToList();

			if (根记录.端索引表_Parent.Count > 0)
				tree.子节点 = new List<记录树>();
			foreach (模式 row in 根记录.端索引表_Parent)
			{
				tree.子节点.Add(形成一个记录树(row));
			}
			return tree;
		}

		//这个算法写得还是很正确的，只是因为算法变了，直接已经调整好，所以，就不用这个了。
		public void 调整所有聚合属拥参数到顶层根(记录树 顶层根, bool 执行调整)
		{
			Data.Assert(Data.ThisGuid.Equals(目标行.A端));
			if (子节点 == null)
				return;
			foreach (记录树 关联节点 in 子节点)
			{
				Data.Assert(Data.ThisGuid.Equals(关联节点.目标行.A端) == false);
				Guid 关联类型 = Data.一级关联类型(关联节点.目标行);
				if (执行调整 && Data.拥有形式Guid.Equals(关联类型) == false)
					关联节点.目标行.ParentID = 顶层根.目标行.ID;
				if (关联节点.子节点 == null)
					continue;
				foreach (记录树 参数概念 in 关联节点.子节点)
				{
					if (替代.是聚合或者属拥(关联类型))
						参数概念.调整所有聚合属拥参数到顶层根(顶层根, true);
					else if (Data.拥有形式Guid.Equals(关联类型) == false)
						参数概念.调整所有聚合属拥参数到顶层根(参数概念, false);
				}
			}

		}
	}

	//考虑用来处理附加关联。
	public class 模式行复制关系
	{
		public Guid 源模式行;
		public Guid 派生模式行;
		public 模式行复制关系(Guid 源模式行, Guid 派生模式行)
		{
			this.源模式行 = 源模式行;
			this.派生模式行 = 派生模式行;
		}
		public static List<模式行复制关系> 模式行复制关系集合 = new List<模式行复制关系>();
		public static void 加入一个模式行复制关系(Guid 源模式行, Guid 派生模式行)
		{
			模式行复制关系集合.Add(new 模式行复制关系(源模式行, 派生模式行));
		}
	}

	public class 模式树结构
	{
		public 模式 目标;
		public 模式 源目标;//模式树在复制等过程中，不改变这个原始目标。

		public 模式 匹配目标;
		public bool 匹配成功;
		public 模式 替换目标;
		public int 替换方式;

		public 参数树结构 派生树;

		public 模式树结构 父节点;

		public List<模式树结构> 语法树子节点;
		public List<模式树结构> 关联路径子节点;

		public List<推导结构> 前置推导;
		public List<推导结构> 后置推导;
		public 模式 附加关联;

		public 模式树结构(模式 模式, 模式树结构 父节点)
		{
			this.父节点 = 父节点;
			this.目标 = 模式;
			this.源目标 = this.目标;
		}

		//public List<模式> 建立对其他模式的附加关联(List<模式> 源附加关联,模式 目标模式,模式 parent)
		//{
		//	List<模式> s = new List<模式>();

		//}

		public 模式树结构 递归复制一个模式树结构()
		{
			模式树结构 obj = new 模式树结构(this.目标, this.父节点);
			obj.源目标 = this.源目标;
			obj.匹配目标 = this.匹配目标;
			obj.替换目标 = this.替换目标;
			obj.替换方式 = this.替换方式;
			obj.匹配成功 = this.匹配成功;
			if (this.语法树子节点 != null)
			{
				obj.语法树子节点 = new List<模式树结构>();
				foreach (模式树结构 模式树结构2 in this.语法树子节点)
					obj.语法树子节点.Add(模式树结构2.递归复制一个模式树结构());
			}
			return obj;
		}

		public 模式树结构 复制一个模式树结构()
		{

			模式树结构 obj = 递归复制一个模式树结构();

			List<模式树结构> 所有子节点 = new List<模式树结构>();

			obj.递归得到所有(所有子节点);

			obj.递归构建关联路径子节点(所有子节点);

			return obj;
		}

		public void 初始化匹配信息()
		{
			匹配目标 = null;
			替换目标 = null;
			替换方式 = 0;
			匹配成功 = false;
			if (语法树子节点 != null)
				foreach (模式树结构 o in 语法树子节点)
					o.初始化匹配信息();
		}

		public void 对答案进行最后处理()
		{
			if (语法树子节点 == null)
				return;
			foreach (模式树结构 obj in 语法树子节点)
			{
				if (Data.是派生关联(Data.概念属拥句子Guid, obj.目标) > 0)
				{
					foreach (模式树结构 o in obj.语法树子节点)
					{
						o.替换目标 = Data.FindRowByID(Data.陈述句Guid);
						o.替换方式 = 9;
					}
				}
			}
		}

		//这样的算法效率显然不高，以后，要考虑把一些模式归纳在一起，甚至是专门的结构化表的方法来存储。采取专门的等价的算法。
		public void 递归直接对问题查询出答案(模式树结构 发起节点, 参数树结构 派生树, List<模式树结构> 结果, bool 执行)
		{
			if (执行 && (this.匹配成功 = this.递归执行开放式模式匹配((模式树结构)null, 发起节点, 派生树.目标, new List<模式>(), false, true) > 0))
			{
				模式树结构 模式树结构 = this.复制一个模式树结构();
				模式树结构.对答案进行最后处理();
				结果.Add(模式树结构);
			}

			this.初始化匹配信息();

			if (派生树.子节点 == null)
				return;
			foreach (参数树结构 obj in 派生树.子节点)
				this.递归直接对问题查询出答案(发起节点, obj, 结果, true);
		}


		public void 递归生成各节点的派生树()
		{
			模式 row = this.目标;

			if (row.A端.Equals(Data.ThisGuid) == false)
				goto 计算节点;

			if (Data.是派生类(Data.疑问算子Guid, row, 替代.正向替代) || Data.是疑问替代对象(row.ID))
				goto 计算节点;


			//if (Data.是二元关联(row.ID,true))
			//	goto 计算节点;

			if (Data.是派生类(Data.事物概念Guid, row, 替代.正向替代))
				派生树 = Data.利用缓存得到派生树(Data.FindRowByID(目标.源记录), false, false);

		计算节点:

			if (this.语法树子节点 != null)
				foreach (模式树结构 o in this.语法树子节点)
					o.递归生成各节点的派生树();
		}

		public 模式树结构 递归选择最优的发起节点()
		{
			if (派生树 != null)
				return this;

			if (this.语法树子节点 != null)
				foreach (模式树结构 o in this.语法树子节点)
				{
					模式树结构 r = o.递归选择最优的发起节点();
					if (r != null)
						return r;
				}

			return null;
		}



		public void 递归准备()
		{
			if (Data.属于Guid.Equals(this.目标.源记录) && this.附加关联 == null && (this.找到一个端对象(true) != null && this.找到一个端对象(false) != null))
				this.附加关联 = Data.创建附加关联(Data.等价Guid, Data.FindRowByID(Data.NullGuid), Data.FindRowByID(Data.NullGuid));
			if (this.语法树子节点 == null)
				return;
			foreach (模式树结构 模式树结构 in this.语法树子节点)
				模式树结构.递归准备();
		}

		//不进行推导，根据问题本身进行查询匹配得出答案。

		public void 直接对问题查询出答案(List<模式树结构> 结果)
		{
			this.递归生成各节点的派生树();
			this.递归准备();

			模式树结构 发起节点 = this.递归选择最优的发起节点();
			if (发起节点 == null)
				return;
			this.递归直接对问题查询出答案(发起节点, 发起节点.派生树, 结果, false);//先不让本节点进行
		}


		public bool 匹配出推导模式的一端(推导结构 推导结构, bool 要匹配的是发起端, bool 执行参数替换)
		{

			if (要匹配的是发起端)
				return 推导结构.发起端模式树结构.匹配成功 = 推导结构.发起端模式树结构.执行模式匹配(this, false, 执行参数替换);

			return 推导结构.结果端模式树结构.匹配成功 = 推导结构.结果端模式树结构.执行模式匹配(this, false, 执行参数替换);

			//目标命题 = 根据模式树生成生长对象(0, row1, 0);

			//模式 目标命题row = Data.加入到素材(Data.New派生行(推导结构.推导模式行.B端, 字典_目标限定.空, true));
			//目标命题row.序号 = -1;//不是从当前串中取数据。
			//生长对象 目标命题 = new 生长对象(目标命题row, 2);

			//以下根据原始命题的显式参数按照等价转换的原则来创建目标命题的参数。

			//var 附加等价参数Rows = Data.模式表.对象集合.Where(r => r.C端 == 推导结构.推导模式行.ID);

			//foreach (模式 附加关联/*比如借拥有的借出等价于还拥有的还入*/ in 附加等价参数Rows)
			//{
			//	//
			//	if (Data.等价Guid.Equals(附加关联.连接) == false)
			//		continue;

			//	模式 推导的A端源对象/*比如借*/ = Data.FindRowByID((Guid)附加关联.A端);
			//	List<模式> A端对象引用路径 = 取得引用路径(推导的A端源对象);

			//	参数 等价参数A端 = 遵循路径查找参数对象(推导结构.推导源模式, A端对象引用路径, 1);

			//	if (等价参数A端 == null)
			//		continue;
			//	//找到了。
			//	模式 推导的B端源对象/*比如借*/ = Data.FindRowByID((Guid)附加关联.B端);
			//	List<模式> B端对象引用路径 = 取得引用路径(推导的B端源对象);

			//	生长对象 参数对象 = 等价参数A端.对端派生对象.B端对象;
			//	if (参数对象.是隐藏对象())
			//		参数对象 = 等价参数A端.对端派生对象.A端对象;

			//	目标命题 = 遵循路径增加参数对象(目标命题, B端对象引用路径, 1, 参数对象);

			//下边建立A端和B端参数实际的等价性。
			//}
		}


		public void 根据问题本身及前置推导匹配出答案(List<模式> 结果, 模式 parentrow)
		{

			if (前置推导 == null)
				return;

			List<模式树结构> 问题答案集合 = new List<模式树结构>();

			直接对问题查询出答案(问题答案集合);

			foreach (模式树结构 问题答案 in 问题答案集合)
			{
				结果.Add(问题答案.从模式树结构创建模式树(parentrow, false,true));
			}

			return;

			//下边对问题反向推导后匹配的代码有了问题，因为正向匹配代码里边分析对象的【关联路径节点】是要依赖知识本身的模式行的。
			//而下属根据推导模板进行模式的复制和转换代码，没有把模式行完全进行复制处理，导致匹配不能进行，所以需要重新设计后再放开。
			foreach (推导结构 推导结构 in 前置推导)
			{
				//1、对每个前置推导，根据问题本身，对推导的末端进行匹配，要求推导末端的参数在问题中都有满足。
				if (匹配出推导模式的一端(推导结构, false, false) == false)
					continue;

				//2、如果匹配成功，让推导结构导出推导的另一端，即等价的问题模式
				模式树结构 等价问题 = 推导结构.根据附加关联将参数从一棵树传递给另一棵树(推导结构.结果端模式树结构, 推导结构.发起端模式树结构, false);

				//3、让等价问题模式实体化，才方便在知识库中进行查询。
				模式 等价问题模式 = 等价问题.从模式树结构创建模式树(Data.当前素材Row, true, true);
				Data.递归设置语境树知识有效性(等价问题模式, -1);
				//模式树结构 整理后的等价问题模式树 = 模式树结构.从一个根模式生成模式树结构(等价问题模式);

				//4、让实体化的等价问题模式在知识库中进行查询匹配，得到等价问题答案
				List<模式树结构> 等价问题答案集合 = new List<模式树结构>();
				等价问题.直接对问题查询出答案(等价问题答案集合);

				foreach (模式树结构 等价问题答案 in 等价问题答案集合)
				{
					//推导结构.发起端模式树结构.初始化匹配信息();

					推导结构.结果端模式树结构.初始化匹配信息();

					模式树结构 a1 = 推导结构.根据附加关联将参数从一棵树传递给另一棵树(等价问题答案, 推导结构.结果端模式树结构, true);

					模式 a = a1.从模式树结构创建模式树(Data.当前素材Row, true, true);
					Data.递归设置语境树知识有效性(a, -1);
					//a1 = 模式树结构.从一个根模式生成模式树结构(a);

					//8、让原始问题对推导后的答案，再次进行匹配，以满足原始问题的语言组织形式。
					if (this.匹配成功 = this.执行模式匹配(a1, false, true))
					{
						结果.Add(从模式树结构创建模式树(parentrow, false, true));
					}
				}
			}
		}

		public void 匹配并执行后置推导(推导结构 上一个推导结构)
		{

			后置推导 = new List<推导结构>();
			var 推导行 = Data.模式表.对象集合.Where(r => r.连接.Equals(Data.推导即命题间关系Guid));

			foreach (模式 模式 in 推导行)
			{
				if (模式.A端.Equals(Data.NullGuid) || 模式.B端.Equals(Data.NullGuid))
					continue;
				if (Data.是派生类(模式.A端, 目标, 替代.正向替代))
				{
					bool t = 上一个推导结构 != null && 上一个推导结构.源推导模式行.A端.Equals(模式.B端) && 上一个推导结构.源推导模式行.B端.Equals(模式.A端);
					推导结构 推导 = new 推导结构(模式, true);
					if (匹配出推导模式的一端(推导, true, false))
					{
						if (推导.推导出推导模式的一端(false,t) != null)
							后置推导.Add(推导);
					}
				}
			}
		}

		public void 准备前置推导模式()
		{

			前置推导 = new List<推导结构>();

			var 推导行 = Data.模式表.对象集合.Where(r => r.连接.Equals(Data.推导即命题间关系Guid));

			foreach (模式 模式 in 推导行)
			{
				if (Data.是派生类(模式.B端, 目标, 替代.正向替代))
					前置推导.Add(new 推导结构(模式, true));
			}

		}



		public override string ToString()
		{
			string s = 目标.形式;
			return s + "--" + Data.FindRowByID(目标.源记录).形式;
		}

		//public bool 匹配成功()
		//{
		//	if (匹配目标 == null)
		//		return false;

		//	foreach (模式树结构 obj in 子节点)
		//	{
		//		if (obj.匹配成功() == false)
		//			return false;
		//	}

		//	return true;
		//}

		public 模式树结构 递归找到附加关联对应模式行(Guid 附加关联ID)
		{
			//如果本模式树是复制的，那么就采用源模式树的！
			//if (源目标 != null && 源目标.目标.ID.Equals(附加关联ID))
			//	return this;

			if (源目标.ID.Equals(附加关联ID))
				return this;


			if (语法树子节点 != null)
			{
				foreach (模式树结构 obj in 语法树子节点)
				{
					模式树结构 r = obj.递归找到附加关联对应模式行(附加关联ID);
					if (r != null)
						return r;
				}
			}
			return null;
		}

		static public int 直接判断模式行(模式树结构 模式树结构对象, bool 匹配拥有形式)
		{

			模式 模板对象 = 模式树结构对象.目标;

			if (匹配拥有形式 == false && Data.拥有形式Guid.Equals(Data.一级关联类型(模板对象)))
				return 2;

			bool b1 = Data.ThisGuid.Equals(模板对象.A端);
			if (b1)
			{
				//模板对象是疑问标点【？】
				if (Data.是派生类(Data.疑问句Guid, 模板对象, 替代.正向替代))
					return 2;
			}
			else
			{
				if (Data.是派生关联(Data.事件属拥被动Guid, 模板对象) > 0)
					return 12;
				if (Data.是派生关联(Data.概念属拥句子Guid, 模板对象) > 0)//所有【属拥句子，都不用匹配】。
					return 12;
				if (Data.是派生关联(Data.事物拥有什么疑问Guid, 模板对象) > 0)
				{
					模式树结构对象.替换方式 = 2;
					return 12;
				}
			}

			return 0;
		}

		public 模式树结构 找到一个端对象结构(bool 找A端)
		{
			if (this.关联路径子节点 == null)
				return (模式树结构)null;
			foreach (模式树结构 模式树结构 in this.关联路径子节点)
			{
				if (找A端 && Data.是派生关联(Data.关联拥有A端Guid, 模式树结构.目标) > 0 || !找A端 && Data.是派生关联(Data.关联拥有B端Guid, 模式树结构.目标) > 0)
					return 模式树结构;
			}
			return (模式树结构)null;
		}


		public 模式 找到一个端对象(bool 找A端)
		{
			模式树结构 模式树结构 = this.找到一个端对象结构(找A端);
			if (模式树结构 == null)
				return (模式)null;
			else
				return Data.FindRowByID(模式树结构.目标.B端);
		}

		public static int 计算类型满足(模式 模板模式, 模式 知识模式)
		{
			int 结果;
			//模板对象是系统改设置的绝对变量。
			if (Data.是派生类(Data.变量Value, 模板模式, 替代.正向替代))
				return 2;
			if ((结果 = Data.是疑问对象(模板模式)) > 0)
				return 结果;
			//if (Data.是派生类(Data.疑问算子Guid, 模板模式, 替代.正向替代) || Data.是疑问替代对象(模板模式.源记录))
			//	return 2;
			//if (Data.是派生类(模板模式.源记录, 知识模式, 替代.正向替代))
			//	return Data.获取子节点中指定类型的节点(模板模式, Data.事物拥有什么疑问Guid, false) != null ? 11 : 1;
			//else
			return Data.满足聚合派生(模板模式.源记录, 知识模式) ? 1 : 0;
		}

		public static int 匹配两个模式行(模式树结构 上一级模板结构, 模式树结构 模板对象结构, ref 模式 知识模式, bool 是否匹配拥有形式)
		{
			int 结果 = 模式树结构.直接判断模式行(模板对象结构, 是否匹配拥有形式);

			if (结果 > 0)
				return 结果;

			bool b1 = Data.ThisGuid.Equals(模板对象结构.目标.A端);
			bool b2 = Data.ThisGuid.Equals(知识模式.A端);

			if (b1 != b2)
			{
				//判断一个展开的二元关联的根和知识里边一个压缩的二元关联的匹配。

				if (Data.是二元关联(模板对象结构.目标, true) == false)
					return 0;

				if (Data.属于Guid.Equals(模板对象结构.目标.源记录))
					return 1;

				if (Data.是派生关联(模板对象结构.目标.源记录, 知识模式) == 0)
					return 0;

				模式 A端对象 = 模板对象结构.找到一个端对象(true);
				if (A端对象 != null && Data.满足聚合派生(A端对象.源记录, Data.FindRowByID(知识模式.A端)) == false)
					return 0;

				模式 B端对象 = 模板对象结构.找到一个端对象(false);
				if (B端对象 != null && Data.满足聚合派生(B端对象.源记录, Data.FindRowByID(知识模式.B端)) == false)
					return 0;
				return 1;
			}
			else
			{
				if (b1 == false && b2 == true)
					return 0;
				if (b1 == true && b2 == true)
				{
					if (上一级模板结构 != null && (Data.是派生关联(Data.关联拥有A端Guid, 上一级模板结构.目标) > 0 || Data.是派生关联(Data.关联拥有B端Guid, 上一级模板结构.目标) > 0))
					{
						模式树结构 二元关联模式 = 上一级模板结构.父节点;
						if (Data.属于Guid.Equals(二元关联模式.目标.源记录))//是【显式的属于关联】
						{
							if (Data.是派生关联(Data.关联拥有A端Guid, 上一级模板结构.目标) > 0)
							{
								模式树结构 B端模式结构 = 二元关联模式.找到一个端对象结构(false);
								if (B端模式结构 != null && B端模式结构.匹配目标 != null)
									return 模式树结构.判断完成后的展开属于模式(二元关联模式, 模板对象结构, B端模式结构.关联路径子节点[0], ref 知识模式, true);
							}
							else if (Data.是派生关联(Data.关联拥有B端Guid, 上一级模板结构.目标) > 0)
							{
								模式树结构 A端模式结构 = 二元关联模式.找到一个端对象结构(true);
								if (A端模式结构 != null && A端模式结构.匹配目标 != null)
									return 模式树结构.判断完成后的展开属于模式(二元关联模式, A端模式结构.关联路径子节点[0], 模板对象结构, ref 知识模式, false);
							}
						}

						else if (Data.是派生关联(Data.拥有Guid, 二元关联模式.目标) > 0)//是【显式的拥有关联】
						{
							int 计算拥有关联 = 1;
							if (Data.是派生关联(Data.关联拥有A端Guid, 上一级模板结构.目标) > 0)
							{
								模式树结构 B端模式结构 = 二元关联模式.找到一个端对象结构(false);
								if (B端模式结构 != null && B端模式结构.匹配目标 != null)
									计算拥有关联 = 模式树结构.判断完成后的展开拥有模式(二元关联模式, 模板对象结构, B端模式结构.关联路径子节点[0], ref 知识模式, true);
							}
							else if (Data.是派生关联(Data.关联拥有B端Guid, 上一级模板结构.目标) > 0)
							{
								模式树结构 A端模式结构 = 二元关联模式.找到一个端对象结构(true);
								if (A端模式结构 != null && A端模式结构.匹配目标 != null)
									计算拥有关联 = 模式树结构.判断完成后的展开拥有模式(二元关联模式, A端模式结构.关联路径子节点[0], 模板对象结构, ref 知识模式, false);
							}
							if (计算拥有关联 == 0)
								return 0;
						}
					}
					结果 = 模式树结构.计算类型满足(模板对象结构.目标, 知识模式);
				}
				else if (!b1 && !b2)
				{
					if (Data.是派生关联(Data.关联拥有A端Guid, 模板对象结构.目标) > 0 || Data.是派生关联(Data.关联拥有B端Guid, 模板对象结构.目标) > 0)
					{
						if (Data.属于Guid.Equals(上一级模板结构.目标.源记录))
							return 100;
						Guid id = Data.是派生关联(Data.关联拥有A端Guid, 模板对象结构.目标) > 0 ? 知识模式.A端 : 知识模式.B端;
						if ((Data.是派生关联(上一级模板结构.目标.源记录, 知识模式) > 0 || Data.是广义等价(上一级模板结构.目标, 知识模式)) && Data.满足聚合派生(Data.FindRowByID(模板对象结构.目标.B端).源记录, Data.FindRowByID(id)))
							return 100;
					}
					else if (Data.是派生关联(模板对象结构.目标.源记录, 知识模式) > 0)
						return 1;
				}
				return 结果;
			}
		}

		private static bool 可以建立附加关联(模式 新对象, List<模式> 聚合体)
		{
			foreach (模式 row in 聚合体)
			{
				if (Data.是派生类(新对象.源记录, row, 替代.正向替代))
					return true;
			}
			return false;
		}


		public 聚合体对象及关联模式 根据对象查询关联模式行及聚合体对象()
		{
			聚合体对象及关联模式 聚合体对象及关联模式 = new 聚合体对象及关联模式();
			this.递归加入聚合对象(聚合体对象及关联模式);
			return 聚合体对象及关联模式;
		}

		public void 递归加入聚合对象(聚合体对象及关联模式 obj)
		{
			if (this.匹配目标 == null)
				return;
			if (Data.ThisGuid.Equals(this.匹配目标.A端))
			{
				obj.聚合体.Add(this.匹配目标);
			}
			else
			{
				if (!替代.是属于等价或聚合(Data.一级关联类型(this.匹配目标)))
					return;
				obj.所有关联.Add(this.匹配目标);
			}
			if (this.语法树子节点 == null)
				return;
			foreach (模式树结构 模式树结构 in this.语法树子节点)
				模式树结构.递归加入聚合对象(obj);
		}
		public static int 判断完成后的展开属于模式(模式树结构 二元关联模式, 模式树结构 A端结构, 模式树结构 B端结构, ref 模式 知识模式, bool A端是最后匹配)
		{
			bool 是附加二元关联 = 二元关联模式.匹配目标 == 二元关联模式.附加关联;
			int 结果;

			模式 A端 = A端是最后匹配 ? Data.FindRowByID(A端结构.目标.B端) : A端结构.匹配目标;
			模式 B端 = A端是最后匹配 ? B端结构.匹配目标 : Data.FindRowByID(B端结构.目标.B端);

			模式树结构 已有结构 = A端是最后匹配 ? B端结构 : A端结构;
			聚合体对象及关联模式 完整聚合体 = 已有结构.根据对象查询关联模式行及聚合体对象();
			聚合体对象及关联模式 知识完整聚合体 = Data.知识完整聚合体(已有结构.匹配目标, false, (List<模式>)null);
			模式 主体 = 知识完整聚合体.得到聚合体的主体();
			模式 剩余体 = 知识完整聚合体.得到聚合体的剩余体(完整聚合体.聚合体);
			if (剩余体 == null)
				剩余体 = 主体;
			if (A端是最后匹配)
			{
				if (是附加二元关联)
					二元关联模式.附加关联.B端 = B端.ID;
				if ((结果 = Data.是疑问对象(A端结构.目标)) > 0)
				{
					A端结构.匹配目标 = 剩余体;
					A端结构.替换目标 = 剩余体;
					二元关联模式.附加关联.A端 = A端结构.匹配目标.ID;
					知识模式 = A端结构.匹配目标;
					return 结果;
				}
				else if (模式树结构.可以建立附加关联(A端, 知识完整聚合体.聚合体))
					A端结构.匹配目标 = 主体;
				if (A端结构.匹配目标 != null)
				{
					二元关联模式.附加关联.A端 = A端结构.匹配目标.ID;
					知识模式 = A端结构.匹配目标;
					return 1;
				}
			}
			else
			{
				if (是附加二元关联)
					二元关联模式.附加关联.A端 = A端.ID;
				if ((结果 = Data.是疑问对象(B端结构.目标)) > 0)
				{
					B端结构.匹配目标 = 剩余体;
					B端结构.替换目标 = 剩余体;
					二元关联模式.附加关联.B端 = B端结构.匹配目标.ID;
					知识模式 = B端结构.匹配目标;
					return 结果;
				}
				else if (模式树结构.可以建立附加关联(B端, 知识完整聚合体.聚合体))
					B端结构.匹配目标 = 主体;
				if (B端结构.匹配目标 != null)
				{
					二元关联模式.附加关联.B端 = B端结构.匹配目标.ID;
					知识模式 = B端结构.匹配目标;
					return 1;
				}
			}
			return 0;
		}

		public static int 判断完成后的展开拥有模式(模式树结构 二元关联模式, 模式树结构 A端结构, 模式树结构 B端结构, ref 模式 知识模式, bool A端是最后匹配)
		{

			if (Data.是派生关联(Data.概念拥有角色Guid, 二元关联模式.匹配目标) > 0)
				return 0;

			return 1;
			//int 结果=0;

			////模式 A端 = A端是最后匹配 ? Data.FindRowByID(A端结构.目标.B端) : A端结构.匹配目标;
			////模式 B端 = A端是最后匹配 ? B端结构.匹配目标 : Data.FindRowByID(B端结构.目标.B端);

			//if (Data.是派生关联(Data.拥有Guid, 知识模式) == 0)
			//	return 0;

			//return 结果;
		}

		public int 递归执行开放式模式匹配(模式树结构 上一级对象, 模式树结构 初始发起节点, 模式 知识模式行, List<模式> 已匹配关联, bool 是否匹配拥有形式, bool 是否执行参数替换)
		{

			int 匹配结果 = 0;

			模式树结构 已处理的子对象 = (模式树结构)null;

			//初始发起节点还没有得到匹配，先去匹配到初始发起节点。
			if (初始发起节点 != this)
			{

				if (this.关联路径子节点 != null)
					foreach (模式树结构 子对象 in this.关联路径子节点)
					{
						//目标对象不变，找到初始发起节点后进行匹配。
						if (子对象.递归执行开放式模式匹配(this, 初始发起节点, 知识模式行, 已匹配关联, 是否匹配拥有形式, 是否执行参数替换) != 0)
						{

							已处理的子对象 = 子对象;


							//下边这一部分，是找把已经匹配的初始发起对象和this根对象联系起来的那一个关联，然后处理this根对象本身。
							List<模式> 知识关联模式行集合 = new List<模式>();

							if (Data.是派生关联(Data.关联拥有A端Guid, 子对象.目标) > 0 || Data.是派生关联(Data.关联拥有B端Guid, 子对象.目标) > 0)
							{
								if (Data.属于Guid.Equals(this.目标.源记录) && this.附加关联 != null)
									知识关联模式行集合.Add(this.附加关联);
								else if (Data.是二元关联(子对象.匹配目标, false))
									知识关联模式行集合.Add(子对象.匹配目标);
							}
							else if (上一级对象 != null && 上一级对象.附加关联 != null && Data.属于Guid.Equals(上一级对象.目标.源记录) && (Data.是派生关联(Data.关联拥有A端Guid, this.目标) > 0 || Data.是派生关联(Data.关联拥有B端Guid, this.目标) > 0))
								知识关联模式行集合.Add(上一级对象.附加关联);
							else
								知识关联模式行集合 = Data.知识完整聚合体(子对象.匹配目标, false, 已匹配关联).所有关联;
							foreach (模式 知识模式 in 知识关联模式行集合)
							{
								模式 a = 知识模式;
								匹配结果 = 模式树结构.匹配两个模式行(上一级对象 != null ? 上一级对象 : (模式树结构)null, this, ref a, 是否匹配拥有形式);
								if (匹配结果 > 0)
								{
									知识模式行 = 知识模式;
									goto 下一步;
								}
							}
						}
					}
			}
			else
				匹配结果 = 模式树结构.匹配两个模式行(上一级对象 != null ? 上一级对象 : (模式树结构)null, this, ref 知识模式行, 是否匹配拥有形式);

		下一步:
			if (匹配结果 == 0)
				return 0;

			this.匹配目标 = 知识模式行;
			Data.加入模式到链表去除重复(已匹配关联, 知识模式行);

			if (匹配结果 != 100)
			{
				if (匹配结果 == 11 && 是否执行参数替换)
				{
					替换目标 = 知识模式行;
					//return 11;
				}
				else if (匹配结果 == 12 || 匹配结果 == 15)
				{
					替换目标 = 知识模式行;
					return 匹配结果;
				}
			}
			if (this.关联路径子节点 != null)
			{
				foreach (模式树结构 子对象 in 关联路径子节点)
				{

					if (已处理的子对象 == 子对象)
						continue;

					if (模式树结构.直接判断模式行(子对象, 是否匹配拥有形式) > 0)
						continue;

					List<模式> 知识关联模式行集合 = new List<模式>();
					if (Data.是派生关联(Data.关联拥有A端Guid, 子对象.目标) > 0 || Data.是派生关联(Data.关联拥有B端Guid, 子对象.目标) > 0)
					{
						if (Data.属于Guid.Equals(this.目标.源记录) && this.附加关联 != null)
							知识关联模式行集合.Add(this.附加关联);
						else if (Data.是二元关联(知识模式行, false))
							知识关联模式行集合.Add(知识模式行);
					}
					else if (Data.是派生关联(Data.关联拥有A端Guid, this.目标) > 0)
						知识关联模式行集合.Add(Data.FindRowByID(知识模式行.A端));
					else if (Data.是派生关联(Data.关联拥有B端Guid, this.目标) > 0)
						知识关联模式行集合.Add(Data.FindRowByID(知识模式行.B端));
					else
						知识关联模式行集合 = Data.知识完整聚合体(知识模式行, false, 已匹配关联).所有关联;
					foreach (模式 知识模式 in 知识关联模式行集合)
					{
						if (子对象.递归执行开放式模式匹配(this, 子对象, 知识模式, 已匹配关联, 是否匹配拥有形式, 是否执行参数替换) > 0)
							goto next;
					}
					已匹配关联.Remove(知识模式行);
					return 0;
				//这个判断似乎有点奇怪，不应该放在这里吧？有刚放的？
				//if (匹配成功==false && Data.是疑问替代对象(this.目标.源记录) == false) //当前节点如果是“什么、谁”等，下级节点允许匹配不成功

				next: ;
				}
			}

			if (是否执行参数替换 && 匹配结果 == 2)
				替换目标 = 知识模式行;

			return 匹配结果;
		}

		//以自身作为匹配模板，在知识模式树中进行匹配
		public bool 执行模式匹配(模式树结构 知识模式树, bool 是否匹配拥有形式, bool 执行参数替换)
		{
			模式 row = this.目标;

			Guid 关联类型 = Data.一级关联类型(row);

			if (是否匹配拥有形式 == false && Data.拥有形式Guid.Equals(关联类型))
				return true;

			int 参数匹配 = 模式树结构.匹配两个模式行(this, this, ref 知识模式树.目标, 是否匹配拥有形式);
			//1、先匹配对象节点的根
			if (参数匹配 == 0)
			{
				if (是否可忽略匹配的模式行(row) == true)
					return true;
				else
					return false;
			}

			if (参数匹配 == 11 && 执行参数替换) //当前节点是“什么书、什么人”一类的节点，后期直接用匹配目标替换当前节点
			{
				匹配目标 = 知识模式树.目标;
				替换目标 = 知识模式树.目标;
				return true;
			}
			else if (参数匹配 == 12) //当前节点是“事件拥有什么疑问”的关联时，不需再检查子节点 
				return true;
			//2、然后再匹配子节点
			if (语法树子节点 != null && 知识模式树.语法树子节点 != null)
			{
				foreach (模式树结构 模板关联 in 语法树子节点)
				{
					bool 成功 = false;
					foreach (模式树结构 知识关联 in 知识模式树.语法树子节点)
					{
						if (模板关联.执行模式匹配(知识关联, 是否匹配拥有形式, 执行参数替换))
						{
							成功 = true;
							break;
						}
					}

					if (成功 == false && Data.是疑问替代对象(row.源记录) == false) //当前节点如果是“什么、谁”等，下级节点允许匹配不成功
						return false;
				}
			}
			this.匹配目标 = 知识模式树.目标;

			if (执行参数替换 && 参数匹配 == 2)
				this.替换目标 = 知识模式树.目标;


			return true;

		}


		public bool 是否可忽略匹配的模式行(模式 语义row)
		{
			if (语义row != null)
			{
				if (Data.是派生关联(Data.事件属拥被动Guid, 语义row) > 0)
					return true;
				else if (Data.是派生关联(Data.事物拥有什么疑问Guid, 语义row) > 0)
					return true;
			}
			return false;
		}
		public void 递归构建模式树结构()
		{

			List<模式> 端索引表 = 目标.端索引表_Parent;

			if (端索引表 != null && 端索引表.Count > 0)
				语法树子节点 = new List<模式树结构>();

			foreach (模式 row in 端索引表)
			{
				模式树结构 obj = new 模式树结构(row, this);
				语法树子节点.Add(obj);
				obj.递归构建模式树结构();
			}
		}

		public void 递归得到所有(List<模式树结构> 所有节点)
		{
			所有节点.Add(this);
			if (this.语法树子节点 != null)
				foreach (模式树结构 o in this.语法树子节点)
					o.递归得到所有(所有节点);
		}


		public void 递归构建关联路径子节点(List<模式树结构> 所有节点)
		{
			if (所有节点.Count() <= 1)
				return;

			关联路径子节点 = new List<模式树结构>();

			for (int i = 0; i < 所有节点.Count(); i++)
			{
				模式树结构 obj = 所有节点[i];

				if (obj == this)
				{
					所有节点.RemoveAt(i);
					i--;
					continue;
				}

				if (this.目标.A端.Equals(obj.目标.ID) || this.目标.B端.Equals(obj.目标.ID) || this.目标.ID.Equals(obj.目标.A端) || this.目标.ID.Equals(obj.目标.B端))
				{
					关联路径子节点.Add(obj);
					所有节点.RemoveAt(i);
					i--;
					continue;
				}
			}

			for (int i = 0; i < 关联路径子节点.Count(); i++)
				关联路径子节点[i].递归构建关联路径子节点(所有节点);

		}
		public static 模式树结构 从一个根模式生成模式树结构(模式 根模式)
		{
			模式树结构 obj = new 模式树结构(根模式, null);

			obj.递归构建模式树结构();

			List<模式树结构> 所有子节点 = new List<模式树结构>();

			obj.递归得到所有(所有子节点);

			obj.递归构建关联路径子节点(所有子节点);

			return obj;
		}

		public bool 根据整个句子判断答案对象的附加信息是否有效(模式树结构 根结构, 模式树结构 答案对象结构)
		{

			if (Data.是二元关联(答案对象结构.目标, false))
			{
				if (答案对象结构.语法树子节点 != null)
					foreach (模式树结构 obj in 答案对象结构.语法树子节点)
					{
						if (obj.目标.显隐 == 字典_显隐.隐藏)
							return false;
					}
			}

			//首先根据问题附属的信息进行判断。
			if (根据疑问对象判断答案对象的附加信息是否有效(答案对象结构) == false)
				return false;

			return true;
		}
		public bool 根据疑问对象判断答案对象的附加信息是否有效(模式树结构 答案对象结构)
		{

			if (Data.是二元关联(答案对象结构.目标, false))
			{
				if (语法树子节点 != null)
					foreach (模式树结构 obj in 语法树子节点)
					{
						if (Data.是二元关联(obj.目标, false))
						{
							if (答案对象结构.目标.源记录 == obj.目标.源记录)
								return false;
						}
					}
			}

			return true;
		}

		public void 递归增加答案对象的附加信息(模式树结构 根结构, 模式树结构 答案对象结构, 模式 parent, 模式基表 目标表, Dictionary<Guid, 模式> keys)
		{
			if (答案对象结构.语法树子节点 != null)
				foreach (模式树结构 obj in 答案对象结构.语法树子节点)
				{
					if (根据整个句子判断答案对象的附加信息是否有效(根结构, obj) == false)
						continue;
					模式 子参数 = obj.递归构建模式树(根结构, 目标表, keys, false, false);
					if (子参数 == null)
						continue;
					子参数.ParentID = parent.ID;
					if (obj.目标.That根 == 字典_目标限定.A端)
						子参数.A端 = parent.ID;
					else
						子参数.B端 = parent.ID;
				}
		}

		public 模式树结构 发现指定单位的变量(Guid 单位)
		{
			if (语法树子节点 == null)
				return null;
			foreach (模式树结构 单位和数的关联 in 语法树子节点)
				if (单位和数的关联.语法树子节点 != null)
					foreach (模式树结构 数 in 单位和数的关联.语法树子节点)
					{
						if (目标.源记录.Equals(单位) && Data.是派生类(Data.数Guid, 数.目标, 替代.正向替代))
							return 数;
						模式树结构 下一级 = 数.发现指定单位的变量(单位);
						if (下一级 != null)
							return 下一级;
					}
			return null;
		}

		public 模式树结构 发现指定的参数(Guid 参数关联)
		{
			if (语法树子节点 == null)
				return null;
			foreach (模式树结构 关联 in 语法树子节点)
				if (关联.语法树子节点 != null && 关联.目标.源记录.Equals(参数关联))
					foreach (模式树结构 数量 in 关联.语法树子节点)
					{
						if (关联.目标.B端.Equals(数量.目标.ID))
							return 数量;
					}
			return null;
		}


		public 模式 执行单位转换(模式树结构 根结构, 模式基表 目标表, Dictionary<Guid, 模式> keys)
		{

			模式树结构 原始值 = 从一个根模式生成模式树结构(this.替换目标);

			if (目标.源记录 == Data.星期Guid)
			{
				模式树结构 星期数 = 发现指定单位的变量(Data.星期Guid);
				if (星期数 != null && Data.是派生类(Data.疑问算子Guid, 星期数.目标, 替代.正向替代))
				{
					模式树结构 年数 = 原始值.发现指定单位的变量(Data.年Guid);
					模式树结构 月数 = 原始值.发现指定单位的变量(Data.月Guid);
					模式树结构 日数 = 原始值.发现指定单位的变量(Data.日Guid);
					if (年数 != null && 月数 != null && 日数 != null)
					{
						int y = Convert.ToInt32(年数.目标.形式);
						int m = Convert.ToInt32(月数.目标.形式);
						int d = Convert.ToInt32(日数.目标.形式);
						//int w= (d+2*m+3*(m+1)/5+y+y/4-y/100+y/400) % 7;
						DateTime a = new DateTime(y, m, d);
						int w = Convert.ToInt32(a.DayOfWeek);
						//这行代码可能不很严谨，暂时先这样。
						模式 row = Data.创建临时数字变量(w.ToString());
						this.替换目标 = null;
						星期数.替换目标 = row;
						星期数.替换方式 = 9;

						return 递归构建模式树(根结构, 目标表, keys, false, false);
					}
				}
			}

			return null;

		}

		public 模式树结构 递归发现一个可计算表达式()
		{
			if (Data.是派生类(Data.表达式Guid, 目标, 替代.正向替代))
				return this;

			if (语法树子节点 != null)
				foreach (模式树结构 obj in 语法树子节点)
				{
					模式树结构 r = obj.递归发现一个可计算表达式();
					if (r != null)
						return r;
				}
			return null;
		}

		public 模式 发现数字()
		{
			if (Data.是派生类(Data.数Guid, 目标, 替代.正向替代))
				return 目标;
			else if (匹配目标 != null && Data.是派生类(Data.数Guid, 匹配目标, 替代.正向替代))
				return 匹配目标;
				

			if (语法树子节点 != null)
				foreach (模式树结构 obj in 语法树子节点)
				{
					模式 row = obj.发现数字();
					if (row != null)
						return row;
				}
			return null;

		}
        public string 获得数字对象的公共数字形式(模式 数字对象)
        {
            if (数字对象 == null)
                return null;
            double number;
            if (double.TryParse(数字对象.形式, out number))
                return 数字对象.形式;
            模式 源对象 = Data.FindRowByID(数字对象.源记录);
            if (源对象 != null)
            {
                foreach (模式 row in 源对象.端索引表_Parent)
                {
                    if (Data.是拥有形式(row))
                    {
                        if (double.TryParse(row.形式, out number))
                            return row.形式;
                    }
                }
            }
            return null;
        }
		public 模式 尝试计算表达的值()
		{
			if (目标.源记录 == Data.除法表达式Guid)
			{
				模式树结构 被除数 = 发现指定的参数(Data.除法表达式拥有被除Guid);
				模式树结构 除数 = 发现指定的参数(Data.除法表达式拥有除Guid);
				if (被除数 != null && 除数 != null && 被除数.匹配目标 != null && 除数.匹配目标 != null)
				{
					被除数 = 从一个根模式生成模式树结构(被除数.匹配目标);
					除数 = 从一个根模式生成模式树结构(除数.匹配目标);

					模式 被除数字 = 被除数.发现数字();
					模式 除数字 = 除数.发现数字();
					if (被除数字 != null && 除数字 != null)
					{
                        string 被除值 = 获得数字对象的公共数字形式(被除数字);
                        string 除值 = 获得数字对象的公共数字形式(除数字);
                        if (string.IsNullOrEmpty(被除值) == false && string.IsNullOrEmpty(除值) == false)
                        {
                            double y = Convert.ToDouble(被除值);
                            double m = Convert.ToDouble(除值);
                            double w = y / m;
                            //这行代码可能不很严谨，暂时先这样。
                            模式 row = Data.创建临时数字变量(w.ToString());
                            
                            return row;
                        }
					}

				}
			}
            else if (目标.源记录 == Data.减法表达式Guid)
            {
                模式树结构 被减数 = 发现指定的参数(Data.减法表达式拥有被减Guid);
                模式树结构 减数 = 发现指定的参数(Data.减法表达式拥有减Guid);
                if (被减数 != null && 减数 != null && 被减数.匹配目标 != null && 减数.匹配目标 != null)
                {
                    被减数 = 从一个根模式生成模式树结构(被减数.匹配目标);
                    减数 = 从一个根模式生成模式树结构(减数.匹配目标);

                    模式 被减数字 = 被减数.发现数字();
                    模式 减数字 = 减数.发现数字();

                    if (被减数字 != null && 减数字 != null)
                    {
                        string 被减值 = 获得数字对象的公共数字形式(被减数字);
                        string 减值 = 获得数字对象的公共数字形式(减数字);
                        if (string.IsNullOrEmpty(被减值) == false && string.IsNullOrEmpty(减值) == false)
                        {
                            double y = Convert.ToDouble(被减值);
                            double m = Convert.ToDouble(减值);
                            double w = y - m;
                            //这行代码可能不很严谨，暂时先这样。
                            模式 row = Data.创建临时数字变量(w.ToString());
                            row.显隐 = 字典_显隐.正常;
                            return row;
                        }
                    }

                }
            }
			return null;
		}

		public void 递归转换匹配目标为替换目标()
		{
			if (Data.是派生类(Data.变量Value, 目标, 替代.正向替代))
			{
				替换目标 = 匹配目标;
				替换方式 = 4;
			}
			if (语法树子节点 != null)
				foreach (模式树结构 obj in 语法树子节点)
					obj.递归转换匹配目标为替换目标();

		}

		public 模式树结构 尝试进行表达式计算()
		{
			模式树结构 表达式=递归发现一个可计算表达式();
			if (表达式 == null)
				return null;
			模式 表达式的值 = 表达式.尝试计算表达的值();
			if (表达式的值 != null)
			{
				递归转换匹配目标为替换目标();
				表达式.替换目标 = 表达式的值;
				表达式.替换方式 = 9;
				return this;
			}

			return null;
		}

		public 模式 对一个答案对象进行具体化处理(模式树结构 根结构, 模式基表 目标表, Dictionary<Guid, 模式> keys)
		{
			//这里需要再细化处理。

			模式 row = null;

			if (Data.是疑问对象(目标) == 15)
			{
				//进行单位转换的处理。
				row = 执行单位转换(根结构, 目标表, keys);
				if (row != null)
					return row;
			}


			return row;

			//对一个答案对象模式树进行整理(根结构);
			//row = 替换对象结构.递归构建模式树(根结构, 目标表, keys, 重新加载模式行);
			//row.语言角色 = 目标.语言角色;
			//keys.Remove(替换对象结构.目标.ID);
			//keys.Add(目标.ID, row);
			//row.源记录 = 替换目标.源记录;
			//row.B端 = 替换目标.源记录;

			//如果是有匹配结果的替换节点，直接将匹配到的目标节点复制过来，触发节点必须是只含有形式行的语义对象
			//if (Guid.Empty.Equals(obj.替换目标) == false && obj.匹配目标 != null && Data.ThisGuid.Equals(obj.目标.A端))
			//{
			//	//List<模式> rows = Data.CopyTree(obj.匹配目标);
			//	//Dictionary<Guid, 模式> 粘贴keys = new Dictionary<Guid, 模式>();
			//	//模式 语义row = Data.PasteRows(rows, true, obj.目标.序号, row, true, 粘贴keys);
			//	////用粘贴后的新模式行，替换B端
			//	//模式 B端 = 递归获取替换后的新模式行(row.B端, 粘贴keys);
			//	//if (B端 != null)
			//	//	row.B端 = B端.ID;
			//	//else
			//	//	row.B端 = 语义row.ID;
			//}

		}

		public 模式 递归构建模式树(模式树结构 根结构, 模式基表 目标表, Dictionary<Guid, 模式> keys, bool 重新加载模式行, bool 执行具体化处理)
		{
			if (Data.是拥有形式(目标))
				return null;
			if ((替换方式 & 2) > 0)
				return null;

			if (keys.ContainsKey(目标.ID))
				return null;

			模式 row = null;

			if (替换目标 != null && 执行具体化处理)
			{
				row = 对一个答案对象进行具体化处理(根结构, 目标表, keys);
				//if (row != null)
				//	return row;
			}
            if (row == null)
			{

				if (替换目标 != null)
				{
					if ((替换方式 & 1) > 0)
					{
						row = Data.CopyRow(目标);
						keys.Add(目标.ID, row);
						//模式行复制关系.加入一个模式行复制关系(目标.ID, row.ID);
						目标表.新加对象(row);
						Data.刷一条记录的根(row);
						row.源记录 = 替换目标.ID;
						row.B端 = 替换目标.ID;
						row.被替换标记 = true;
					}
					else
					{
						row = Data.CopyRow(替换目标);
						keys.Add(目标.ID, row);
						//模式行复制关系.加入一个模式行复制关系(目标.ID, row.ID);
						目标表.新加对象(row);
						Data.刷一条记录的根(row);
						row.被替换标记 = true;
						if (执行具体化处理 && 替换方式!=4)//执行具体化处理后，就不能做这个处理了。
						{
							//为目标答案增加各种参数方式
							模式树结构 替换对象结构 = 从一个根模式生成模式树结构(this.替换目标);
							
							//这个代码容易产生重复的对象添加，要注意。
							递归增加答案对象的附加信息(根结构, 替换对象结构, row, 目标表, keys);
						}
					}
				}
				else
				{
					//row = 匹配目标 == null ? Data.CopyRow(目标) : Data.CopyRow(匹配目标);
						row = Data.CopyRow(目标);
						keys.Add(目标.ID, row);
						//模式行复制关系.加入一个模式行复制关系(目标.ID, row.ID);
						目标表.新加对象(row);
						Data.刷一条记录的根(row);
				}
			}


			if (语法树子节点 != null)
			{
				foreach (模式树结构 obj in 语法树子节点)
				{
					//if (keys.ContainsKey(obj.目标.ID))
					//{
					//	Data.Assert();
					//	continue;
					//}
					//把下边的子节点都不生成了，但聚合角色需要保留。
					if ((替换方式 & 8) > 0 && (父节点 == null || obj.语法树子节点==null || 父节点.目标.B端!=obj.语法树子节点[0].目标.ID))
							continue;

					模式 o = obj.递归构建模式树(根结构, 目标表, keys, 重新加载模式行, 执行具体化处理);
					if (o != null)
						o.ParentID = row.ID;
				}
			}

			//替换了目标！
			//if (重新加载模式行)
			//{
			//	目标 = row;
			//	匹配目标 = null;
			//	替换目标 = Guid.Empty;
			//	匹配成功 = false;
			//}

			return row;
		}
		public 模式 递归获取替换后的新模式行(Guid 模式行ID, Dictionary<Guid, 模式> keys)
		{
			if (this.匹配目标 != null && this.目标.ID.Equals(模式行ID))
			{
				if (keys.ContainsKey(this.匹配目标.ID))
					return keys[this.匹配目标.ID];
			}
			if (语法树子节点 != null)
			{
				foreach (模式树结构 obj in 语法树子节点)
				{
					模式 r = obj.递归获取替换后的新模式行(模式行ID, keys);
					if (r != null)
						return r;
				}
			}
			return null;
		}



		public void 递归刷新ID(模式基表 目标表, Dictionary<Guid, 模式> keys)
		{

			foreach (var r in keys)
			{
				模式 row = r.Value;

				if (keys.ContainsKey((Guid)row.ParentID))
				{
					row.ParentID = keys[(Guid)row.ParentID].ID;
				}

				if (keys.ContainsKey((Guid)row.A端))
				{
					row.A端 = keys[(Guid)row.A端].ID;
				}
				if (keys.ContainsKey((Guid)row.B端))
				{
					row.B端 = keys[(Guid)row.B端].ID;
				}
				if (keys.ContainsKey((Guid)row.连接))
				{
					row.连接 = keys[(Guid)row.连接].ID;
				}
				if (keys.ContainsKey((Guid)row.源记录))
				{
					row.源记录 = keys[(Guid)row.源记录].ID;
				}
				if (keys.ContainsKey((Guid)row.C端))
				{
					row.C端 = keys[(Guid)row.C端].ID;
				}

			}

			if (语法树子节点 != null)
			{
				foreach (模式树结构 obj in 语法树子节点)
					obj.递归刷新ID(目标表, keys);
			}
		}


		public 模式 从模式树结构创建模式树(模式 parentrow, bool 重新加载模式行, bool 执行具体化处理)
		{
			模式基表 目标表 = Data.FindTableByID(parentrow.ID);

			Guid parentId = parentrow.ID;

			Dictionary<Guid, 模式> keys = new Dictionary<Guid, 模式>();
			int i = 0;

			模式 rootrow = 递归构建模式树(this, 目标表, keys, 重新加载模式行, 执行具体化处理);

			递归刷新ID(目标表, keys);


			rootrow.ParentID = parentrow.ID;

			return rootrow;

		}
	}


	public class 参数树结构
	{
		//public DataRow 目标;//目标行
		public 模式 目标;
		//这个主要在生成的时候用一下，表示根的方向，到每个终点路径。处理【等价】等的双向等效问题。
		public int 生成树时的路径起始端;

		//public int 假设中心that = 字典_目标限定.A端;//临时变量，不被缓存，也不能保证平行计算的并发处理。

		//public bool 是This属于;//如果是属于的话，那么区分这两种情况。
		public List<参数树结构> 子节点;
		public 参数树结构 父对象;
		public bool 是关联;
		//public int 层级;
		float 打分;

		private int 当前遍历子节点;
		//private bool 已经进入过;

		public Guid 目标ID
		{
			get
			{
				return 目标.ID;
			}
		}

		public int Count()
		{
			int c = 1;
			if (子节点 == null)
				return 1;
			foreach (参数树结构 obj in 子节点)
				c += obj.Count();
			return c;
		}



		//调用者是根，要寻找的是当前对象的兄弟对象。如果成功，返回的是兄弟或者是回溯的的兄弟，如果失败表示整个树的遍历已经结束。
		public 参数树结构 得到当前级的下一对象()
		{
			if (当前遍历子节点 < 0 || 子节点 == null)
			{
				当前遍历子节点 = -2;
				return null;
			}
			参数树结构 o = 子节点[当前遍历子节点].得到当前级的下一对象();
			if (o != null)
				return o;
			if (++当前遍历子节点 < 子节点.Count)
				return 子节点[当前遍历子节点];

			当前遍历子节点 = -2;
			return null;
		}

		public void 去除给定记录的子孙记录(模式 祖先)
		{
			Data.Assert(Data.是子记录(目标, (Guid)祖先.ID, true) == false);//这个方法不处理自身是的，只处理节点。
			if (子节点 == null)
				return;
			for (int i = 0; i < 子节点.Count; i++)
			{
				if (Data.是子记录(子节点[i].目标, (Guid)祖先.ID, true) == false)
					子节点[i].去除给定记录的子孙记录(祖先);
				else
					子节点.RemoveAt(i--);
			}
			if (子节点.Count == 0)
				子节点 = null;
		}

		public void 递归初始化遍历标记()
		{
			//已经进入过 = false;
			当前遍历子节点 = -1;
			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
					obj.递归初始化遍历标记();
		}

		public 参数树结构 先尝试子对象失败再尝试同级下个对象()
		{
			参数树结构 o = 进入更深一级子对象();
			if (o != null)
				return o;
			return 得到当前级的下一对象();
		}

		//调用者是根，要寻找的是当前对象的子级对象。如果失败返回null。
		public 参数树结构 进入更深一级子对象()
		{
			//if (已经进入过)//避免无限递归。
			//    return null;

			//已经进入过 = true;

			if (当前遍历子节点 == -2 || 子节点 == null)
			{
				当前遍历子节点 = -2;
				return null;
			}
			//if (子节点 == null || 子节点.Count()==0)
			//    return null;
			if (当前遍历子节点 == -1)
			{
				当前遍历子节点 = 0;
				return 子节点[当前遍历子节点];
			}

			return 子节点[当前遍历子节点].进入更深一级子对象();

		}

		public 参数树结构 当前遍历对象()
		{
			if (当前遍历子节点 == -2)//已经结束。
				return null;
			if (当前遍历子节点 == -1)
				return this;
			return 子节点[当前遍历子节点].当前遍历对象();
		}

		//public 参数树结构(DataRow obj, Guid 发起id)
		//{
		//    //			if (子节点 == null)
		//    //				子节点 = new List<objecttree>();
		//    //			objecttree o= new objecttree();
		//    //if (根对象.Contains((Guid)obj["ID"]))
		//    //	return;

		//    目标 = obj;
		//    //目标类型 = Data.根模式(obj);
		//    目标ID = (Guid)obj["ID"];
		//    //			子节点.Add(o);
		//    //			return o;
		//    if (发起id.Equals(目标["A端"]) || Guid.Empty.Equals(发起id))
		//        发起端 = 字典_目标限定.A端;
		//    else if (发起id.Equals(目标["B端"]))
		//        发起端 = 字典_目标限定.B端;
		//    else
		//        发起端 = 字典_目标限定.连接;
		//}

		public override string ToString()
		{
			return 目标.形式;
		}

		public 参数树结构(模式 row, int 路径起始端, bool 查找关联/*, int 当前层级*/)
		{
			目标 = row;
			//是This属于 = Data.是分类(目标类型) && Data.ID相等(Data.ThisGuid,obj["A端"]);
			生成树时的路径起始端 = 路径起始端;
			是关联 = 查找关联;
			//层级 = 当前层级;
			当前遍历子节点 = -1;
		}


		public 参数树结构 复制一个结构和一级子节点()
		{
			参数树结构 o = new 参数树结构(目标, 生成树时的路径起始端, 是关联/*, 层级*/);
			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
					o.Add子对象(obj);//这些节点没有复制，直接加入，唯一的问题就是子节点的父对象不同了。不过现在父对象其实并不怎么用。
			return o;
		}

		public 参数树结构 Add子对象(参数树结构 o)
		{
			if (子节点 == null)
				子节点 = new List<参数树结构>();
			o.父对象 = this;
			//if (this.是This属于 == false)//这是有传递性的。父对象的不满足，那么子对象就不能满足。
			//	o.是This属于 = false;
			子节点.Add(o);
			return o;
		}



		//给一个对象ID，从本参数结构（基类树）中匹配出和这个对象广义匹配的记录。从而可以说这个对象是这棵树的根对象的派生。
		//利用的方法是【本质属于】和【扮演】等来表达。
		//返回的对象应该是本对象的真正基类，也就是本对象可以完全替代该基类。
		//【拥有】和【属拥】本质都是拥有，不能传递替换。
		//允许抽象扩展的意思是，比如【人扮演人角色】，而【借出角色派生于人角色】，就能推导出【人是借出角色的派生类】。
		public int 递归从基类树中查找广义匹配的基类(Guid 假设基类对象ID, int 允许的替代类型/*派生肯定是可以的，就看可否扮演*/, int 深度 = 0)
		{
            //陈峰，看起来，聚合替代是不起作用的！
            允许的替代类型 &= ~替代.聚合替代;

            if (Data.ThisGuid.Equals(目标ID))//this不是任何类的派生类。
				return -1;

			if (Data.概念Guid.Equals(假设基类对象ID) || Data.概念Guid.Equals(目标ID))//)//概念是所有的基类，但太泛了，所以这里先不允许。。
				return -1;

			Guid 类型 = 生成树时的路径起始端 == 字典_目标限定.连接 ? Data.属于Guid : Data.一级关联类型(目标);

			if ((允许的替代类型 & 替代.A端对B端替换性(类型)) == 0)
				return -1;

			//两者相等了，返回正确，自己等价自己总是允许的。
			if (假设基类对象ID.Equals(目标ID))
				return 深度;

			//这种情况一般就是【A属于B】，B当然不能被B的派生类替换，比如【苹果属于水果】不能变成【苹果属于香蕉】。
			//但其实不是不能替换，理论上，可以换成B的基类。只是这个不需要，因为基类将会自己匹配上，不需要依托这个了。
			//下边代码放开，就是允许一个对一个基类的扮演就可以拖延出所有的派生扮演
			//但这也有问题，比如有的代码不是用角色名来多继承，而是介词，就得不到合适的结果了
			//暂时只允许【扮演】这样做，比如需要匹配【【推导概念】拥有【条件】】。而【【关系概念】扮演【推理角色】】且【条件属于推理角色】
			//【关系】扮演【推理角色】使【关系】可以满足推理角色的派生类条件。
			//这个时候，可以说，【关系概念扮演推理角色】等价于【关系概念【属于】角色的所有派生类】，一般情况下，基类不能替换为派生类的。

			//按【桥梁方式来串接两个模式体系】！以前说的反向扩展。
			//if (Data.ThisGuid.Equals(目标["A端"]) == false)//不是this的情况才有桥接效果。this自身体系是【本质】！！！
			//{
			//	//暂时只让【扮演】的关联可以进行。【等价】不能允许，否则有问题，而且等价在别处做了处理。剩下就是【属于】...。出现问题再看。
			//	if ((替代.正向聚合替代 & 替代.A端对B端替换性(Data.一级关联类型(目标))) > 0)
			//	{
			//		Guid B端ID = (Guid)目标["B端"];
			//		if (假设基类对象ID.Equals(B端ID)) //因为现在[得到基类和拥有关联记录树]是从源记录开始，不包含自身，所以先直接判断。
			//			return this;
			//		DataRow 假设基类源模式行 = Data.FindRowByID((Guid)Data.FindRowByID(假设基类对象ID)["源记录"]);
			//		参数树结构 tree = Data.利用缓存得到基类和关联记录树(假设基类源模式行, false, false);
			//		参数树结构 r = tree.递归从纯的基类树中查找广义匹配的基类(B端ID, 替代.本质正向替代);
			//		//如果数据有问题，这里有递归死循环的风险，需要的话，在这里对【递归从纯的基类树中查找广义匹配的基类】进行递归深度检查（加一个深度值），调试期发现数据错。
			//		if (r != null)
			//			return this;
			//	}
			//}


			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
				{
					int r = obj.递归从基类树中查找广义匹配的基类(假设基类对象ID, 允许的替代类型, 深度 + 1);
					if (r != -1)
						return r;
				}

			return -1;
		}

		//形式层级为0，表示不取，其它的就是递归到的层级，一般为1。取最大就100可以。
		public 参数树结构 递归得到所有基类和关联树(bool 包含拥有, /*int 形式层级,*/ bool 仅知识, float 阀值, int 语言, Dictionary<模式, 参数树结构> 缓存 = null)
		{
			if (this.目标.形式 == "[实体概念]")
				仅知识 = 仅知识;
			模式 row = 目标;
			Guid objectid = row.ID;

			if (Data.DataIDS.Contains(objectid))
				return null;
			Data.DataIDS.Add(objectid);

			if (Data.ThisGuid.Equals(objectid))
				return null;

			if (Data.是介词或者串(row, true, true, true))
				return this;

			//对于模式表中的可以使用缓存，现在改为都可以使用。
			if (缓存 != null /*&& 目标.Table == Data.patternDataSet.模式*/)
			{
				if (缓存.ContainsKey(目标))
					return 缓存[目标];
			}

			if (是关联)//如果是一个关联，就先顺着把【连接】字段全部找寻完。
			{
				参数树结构 obj2 = this;
				while (true)
				{
					List<参数树结构> 一批结果 = Data.得到一批同一层级的基类和关联(row, 包含拥有, true,/*obj2.层级 <= 形式层级,*/ 仅知识, 阀值/*, obj2.层级 + 1*/, 语言);
					if (一批结果 != null)
						foreach (参数树结构 r1 in 一批结果)
						{
							参数树结构 obj = r1.递归得到所有基类和关联树(包含拥有, /*形式层级,*/ 仅知识, 阀值, 语言);
							if (obj != null)
								obj2.Add子对象(obj);
						}

					if (Data.基本关联Guid.Equals(objectid))
						break;

					objectid = (Guid)row.源记录;
					row = Data.FindRowByID(objectid);
					参数树结构 obj1 = new 参数树结构(row, 字典_目标限定.连接, 是关联/*, obj2.层级 + 1*/);
					obj2.Add子对象(obj1);
					obj2 = obj1;
				}
			}
			else if (替代.是本质正向分类(Data.一级关联类型(row)))	//可以进行递归的【本质正向】属于记录。
			{
				List<参数树结构> 一批结果 = null;
				参数树结构 obj2 = this;

				if (Data.ThisGuid.Equals(row.A端))//this本身记录
					一批结果 = Data.得到一批同一层级的基类和关联(row, 包含拥有, true,/*obj2.层级 <= 形式层级,*/ 仅知识, 阀值,/* obj2.层级 + 1, */语言);
				else//附加属于
				{
					//如果是B端的情况，就是【等价】的类型反转的。
					模式 r5 = 生成树时的路径起始端 == 字典_目标限定.B端 ? Data.FindRowByID((Guid)row.A端) : Data.FindRowByID((Guid)row.B端);
					参数树结构 obj1 = new 参数树结构(r5, 字典_目标限定.A端, Data.是二元关联但排除掉等价型(r5)/*, obj2.层级 + 1*/);
					obj2.Add子对象(obj1);
					obj2 = obj1;
					一批结果 = Data.得到一批同一层级的基类和关联(r5, 包含拥有, false, 仅知识, 阀值, /*obj2.层级 + 1, */语言);//附加属于的形式是不加入的。
				}

				if (一批结果 != null)
					foreach (参数树结构 r1 in 一批结果)
					{
						//概念和概念自身是递归的，去除掉这条递归记录。
						if (Data.概念Guid.Equals(目标ID) && Data.概念Guid.Equals(r1.目标ID))
							continue;
						参数树结构 obj = r1.递归得到所有基类和关联树(包含拥有, /*形式层级,*/ 仅知识, 阀值, 语言);
						if (obj != null)
							obj2.Add子对象(obj);
					}
			}

			if (缓存 != null /*&& 目标.Table == Data.patternDataSet.模式*/)
				缓存.Add(目标, this);

			return this;
		}

		public 参数树结构 递归得到所有派生类(bool 仅知识, float 阀值, bool 包括扩展派生)
		{
			模式 row = 目标;
			Guid objectid = row.ID;

			if (Data.是介词或者串(row, true, true, true))//串都不考虑派生。
				return this;

			if (Data.ThisGuid.Equals(objectid))
				return null;//【this】节点不展开

			if (Data.概念Guid.Equals(objectid))
				return null;//概念不展开

			if (Data.拥有形式Guid.Equals(row.ID))
				return this; //拥有形式不展开

			List<参数树结构> Rows = Data.得到一批同一层级的派生类(row, 仅知识, 阀值, /*层级,*/ 是关联, 包括扩展派生);


			foreach (参数树结构 r1 in Rows)
			{
				参数树结构 obj = r1.递归得到所有派生类(仅知识, 阀值, 包括扩展派生);
				if (obj != null)
					this.Add子对象(obj);
			}

			return this;

		}


		public void 插入满足的关联结果(List<参数树结构> 满足的结果)
		{
			for (int i = 0; i < 满足的结果.Count(); i++)
			{
				参数树结构 o = 满足的结果[i];
				if (o.目标ID == 目标ID)//完全相同，去除掉。
					return;
				if (Data.是派生关联(o.目标ID, 目标) > 0)//保证派生的关联放在前边。
				{
					满足的结果.Insert(i, this);
					return;
				}
			}
			满足的结果.Add(this);
		}



		public void 插入满足的结果(List<三级关联> 满足的结果, int 假设发起端)
		{
			bool b = 三级关联.遍历关联.加入一级(this);

			Data.Assert(三级关联.遍历关联.级数 <= 3);

			三级关联 obj = 三级关联.遍历关联.整理出一个结果(假设发起端);

			if (b)
				三级关联.遍历关联.回退一级();

			//注意，下边的这些代码要加入和处理。
			int i = 0;
			for (i = 0; i < 满足的结果.Count(); i++)
			{
				三级关联 o = 满足的结果[i];
				if (obj.等价(o))//完全相同，去除掉。
					return;
				if (obj.是派生关联路径(o))//保证派生的关联放在前边。
					break;
			}
			满足的结果.Insert(i, obj);
		}

		public void 插入满足的并列关系结果(List<三级关联> 满足的结果)
		{
			//int i = 0;
			//for (i = 0; i < 满足的结果.Count(); i++)
			//{
			//    参数树结构 o = 满足的结果[i].并列关联的基类;
			//    if (o == null)
			//        continue;
			//    if (Data.是派生类(this.目标ID, o.目标, 替代.正向替代))//已经有了派生类的共同基类，去除抽象类。
			//        return;
			//    //if (obj.是派生关联路径(o))//保证派生的关联放在前边。
			//    //    break;
			//}
			//三级关联 obj = new 三级关联();
			//obj.并列关联的基类 = this;
			//满足的结果.Insert(i, obj);
		}


		public void 插入满足的派生结果(ref List<参数树结构> 满足的结果)
		{
			for (int i = 0; i < 满足的结果.Count(); i++)
			{
				参数树结构 o = 满足的结果[i];
				if (o.目标ID == 目标ID)//完全相同，去除掉。
					return;
				if (Data.是派生关联(o.目标ID, 目标) > 0)//只保留派生的。基的不需要了。
				{
					满足的结果[i] = this;
					return;
				}
			}
			满足的结果.Add(this);
		}



		//public void 递归处理派生语义树节点(生长对象 对象, int that端, List<参数树结构> 满足的结果)
		//{
		//    if (目标ID.Equals(对象.中心第一根类.模式行ID) == false)//排除掉自己。
		//    {
		//        this.假设中心that = that端;
		//        插入满足的派生结果(ref 满足的结果);//是一个有效的关联，加入到结果中去。
		//    }

		//    if (子节点 != null)
		//        foreach (参数树结构 obj in 子节点)
		//            obj.递归处理派生语义树节点(对象, that端, 满足的结果);
		//}

		//public void 递归判断被关联满足的各语义树节点(Guid 关联ID, int that端, List<参数树结构> 满足的结果)
		//{
		//	if (Data.是派生关联(关联ID, 目标) > 0)
		//	{
		//		插入满足的关联结果(满足的结果);//是一个有效的关联，加入到结果中去。
		//		打分 = 1;
		//	}

		//	if (子节点 != null)
		//		foreach (参数树结构 obj in 子节点)
		//			obj.递归判断被关联满足的各语义树节点(关联ID, that端, 满足的结果);
		//}
		//从树中过滤出被目标满足的
		//this是用一个中心对象（比如借出）查询出来的关联参数树（比如【借出拥有借出者】【借出拥有借入者】）
		//里边有很多和别的对象的关联（比如借入者，借出者）
		//本方法是返回带入的目标对象（比如人）是否满足其中的那些（人会满足满足借出者，借入者）。
		public void 递归计算三级关联(参数树结构 对端树, int 假设发起端, List<三级关联> 满足的结果, Guid 本级关联类型, int 可替代性, List<模式> 已遍历过关联)
		{
			//调试断点代码。把要关注的对象ID填写到这里来设置断点。
			if (目标ID.Equals(new Guid("067f6278-8920-4495-a801-5d364f0ef9a5")))
				可替代性 = 可替代性;

			if (Data.概念Guid.Equals(目标ID))
				return;
			参数树结构 关联obj = null;//中间关联obj，如果不等价于根的话，那么就是隐含的。
			//如果包含【属于关联】那么能满足的比较多，比如【苹果和香蕉】，从水果开始两者有多个满足，但这不要紧！
			//1、用具体的去屏蔽基本的。
			//2、有的探讨的是否有意义！【苹果属于苹果】这是没有意义的！不是【满足】的问题，满足是100%的！这里恰恰相反，是要探讨有无研究意义的问题

			if (三级关联.遍历关联.级数 == 0 || 替代.是本质分类(本级关联类型) || 三级关联.遍历关联.可以进入下级(this))
			{
				if (Data.是拥有形式(目标))//如果是拥有形式，那么要求形式串完全相同。
				{
					//if (Data.是派生关联(Data.拥有语言部件Guid, 目标) == 0)
					//	return;
					//纯粹的拥有形式不允许，拥有部件则可以。
					string 源串 = 目标.形式/* Data.取得嵌入串(目标)*/;
					if (源串 == "" || (源串 != 对端树.目标.形式/*Data.取得嵌入串(基类树.目标)*/))
						return;
					关联obj = 对端树;
				}
				else
				{
					打分 = 0;

					//if (Data.ThisGuid.Equals(目标["A端"])==false && this.生成树时的发起端 != 字典_目标限定.连接)
					//    id = (Guid)目标["B端"];

					if (可替代性 == -1)
						可替代性 = 替代.A端可被替换性(本级关联类型);
					//可替代性 = 替代.B端可被替换性(本级关联类型);

					//int 允许的替代类型 = 上一关联 == null ? 替代.正向替代 | 替代.等价替代 : 替代.B端可被替换性(Data.一级关联类型(上一关联));
					if (替代.是本质正向分类(本级关联类型)/*Data.在基本关联集合中(目标ID, false)==false*/)//【属于】
					{
						//if (基类树 == null)//如果没有传入的情况下，那么都满足。
						//	关联obj = this;
						//if (上一关联 == null || Data.属于Guid.Equals(Data.一级关联类型(上一关联)) == false)
						//if (可替代性 & 替代.等价替代 | 替代.等价替代)
                        if (三级关联.遍历关联.级数 == 0)
                            关联obj = 对端树.目标ID.Equals(目标ID) ? this : null;
                        else
                        {

//                           可替代性 &= ~替代.聚合替代;//其实，聚合替代是不起作用的！
//                            关联obj = 对端树.递归从基类树中查找广义匹配的基类(目标ID, 可替代性) != -1 ? this : null;
                            关联obj = 对端树.递归从基类树中查找广义匹配的基类(目标ID, 可替代性) != -1 ? this : null;
                            //if (关联obj!=null && Data.是派生类(目标ID, 对端树.目标, 替代.正向替代) == false)
                            //{
                            //    Data.Assert(false);
                            //}
                        }
					}
					else//是关联，如【拥有】等
					{
						//if (Data.是介词或者串(对象.目标, true, true, true) == false)
						//查出的关联是二元关联里的【关联拥有A端】和【关联拥有B端】。除了【字符串】和【值概念】，别的都暂时都允许
						if (Data.关联拥有A端Guid.Equals(目标ID) || Data.关联拥有B端Guid.Equals(目标ID))
						{
							//if (基类树 == null)
							//	关联obj = this;
							if (Data.可作为中心对象(对端树.目标, true))//【值概念】不允许，比如【。】等。
								关联obj = 对端树;
						}
					}
				}
				//已经满足，加入。
				if (关联obj != null)
				{
					插入满足的结果(满足的结果, 假设发起端);//是一个有效的关联，加入到结果中去。
					打分 = 1;
				}
				else if (替代.是本质分类(本级关联类型) == false)//不满足，而且不是本质属于，增加路径。
				{
					if (三级关联.遍历关联.可以进入下级(this))
					{
						三级关联.遍历关联.加入一级(this);
						模式 下端对象 = Data.FindRowByID(生成树时的路径起始端 == 字典_目标限定.A端 ? 目标.B端 : 目标.A端);
						参数树结构 下一级结构 = Data.利用缓存得到基类和关联记录树(下端对象, false);
						下一级结构.递归计算三级关联(对端树, 假设发起端, 满足的结果, Data.一级关联类型(下端对象), 替代.B端可被替换性(Data.一级关联类型(目标)), 已遍历过关联);
						三级关联.遍历关联.回退一级();
					}
				}
			}


			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
				{
					//如果上次是属于，那么根据【源记录】等于子节点的ID判断还是传递属于，这样主要正确解决【二元关联】的问题。
					Guid 下级类型 = 替代.是本质分类(本级关联类型) && obj.目标ID.Equals(目标.源记录) ? Data.属于Guid : Data.一级关联类型(obj.目标);

					if (下级类型.Equals(Data.属于Guid) == false && 三级关联.遍历关联.在当前级中找到已经遍历过的派生关联且抑制基关联(obj.目标))
						continue;
					三级关联.遍历关联.在当前级中加入一个遍历过的派生关联(obj.目标);

					//if (下级类型.Equals(Data.属于Guid))
					int 下级可替代性 = 替代.B端可被替换性(下级类型);
					//else
					//	下级可替代性 = 替代.B端可被替换性(下级类型);
					obj.递归计算三级关联(对端树, 假设发起端, 满足的结果, 下级类型, 下级可替代性, 已遍历过关联);
				}
		}

		public int 递归计算基准对象(ref 基准类型 基准对象, int 层级)
		{
			基准对象 = 基准类型.找到匹配的基准类型(目标ID);
			if (基准对象 != null)
				return 层级;
			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
				{
					基准对象 = 基准类型.找到匹配的基准类型(obj.目标ID);
					if (基准对象 != null)
						return 层级 + 1;
				}
			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
				{
					int r = obj.递归计算基准对象(ref 基准对象, 层级 + 1);
					if (r != -1)
						return r;
				}

			return -1;
		}

		public int 查找本质基类返回深度(模式 基类)
		{
			if (Data.是二元关联(目标, true))
				return Data.是派生关联(基类.ID, 目标) > 0 ? 0 : -1;

			if (Data.是二元关联(基类.ID, true))
				return -1;

			return 递归从基类树中查找广义匹配的基类(基类.ID, 替代.正向替代);
		}

		public void 递归计算相似性关联(参数树结构 基类树, int 深度, ref List<并列关联> 结果)
		{
			if (替代.是本质正向分类(Data.一级关联类型(目标)))
			{
				int 另一个深度 = 基类树.查找本质基类返回深度(目标);
				if (另一个深度 != -1)
				{
					并列关联 关联 = new 并列关联(目标, 深度, 另一个深度);
					结果.Add(关联);
					//插入满足的并列关系结果(满足的结果);
					return;//共同基类只计算一个！并不需要多个。
				}
			}

			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
					obj.递归计算相似性关联(基类树, 深度 + 1, ref 结果);

		}

		public 模式 递归计算共同基类(参数树结构 基类树)
		{
			if (替代.是本质正向分类(Data.一级关联类型(目标)))
				if (Data.是派生类(this.目标ID, 基类树.目标, 替代.正向替代))
				{
					//插入满足的并列关系结果(满足的结果);
					return this.目标;//共同基类只计算一个！并不需要多个。但也可以考虑多个？
				}

			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
				{
					模式 r = obj.递归计算共同基类(基类树);
					if (r != null)
						return r;
				}

			return null;
		}
		//public bool 递归判断属于和聚合(参数树结构 基类树, int 允许的替代类型)
		//{
		//    if (Data.概念Guid.Equals(目标ID))
		//        return false;

		//    if (基类树.递归从基类树中查找广义匹配的基类(目标ID, 允许的替代类型) != null)
		//        return true;


		//    if (子节点 != null)
		//        foreach (参数树结构 obj in 子节点)
		//        {
		//            Guid 类型 = obj.生成树时的路径起始端 == 字典_目标限定.连接 ? Data.属于Guid : Data.一级关联类型(目标);
		//            if ((允许的替代类型 & 替代.B端对A端替换性(类型)) == 0)
		//                continue;
		//            Guid 对端 = obj.生成树时的路径起始端 == 字典_目标限定.A端 ? obj.目标.B端 : obj.目标.B端;
		//            if (obj.递归判断属于和聚合(基类树, 允许的替代类型) == true)
		//                return true;
		//        }
		//    return false;
		//}
		public void 递归加入参数树到界面(PatternDataSet.模式查找Row ObjectRow)
		{

			string that = Data.GetThatString(生成树时的路径起始端);

			ObjectRow = Data.加入一条参数记录(ObjectRow, 目标, that);

			//参数字段 打分对象 = new 参数字段();

			//ObjectRow.打分 = 目标.

			Guid 一级关联类型 = Data.一级关联类型(目标);
			if (替代.是分类或聚合或属拥(一级关联类型) == false && 生成树时的路径起始端 != 字典_目标限定.连接)
			{
				if (Data.拥有形式Guid.Equals(一级关联类型))
					ObjectRow.标记 = "拥有形式";
				else
					ObjectRow.标记 = "拥有";
			}
			else if (Data.ID相等(Data.ThisGuid, 目标.A端) && 生成树时的路径起始端 != 字典_目标限定.连接)
				ObjectRow.标记 = "This";



			if (子节点 != null)
				foreach (参数树结构 obj in 子节点)
					obj.递归加入参数树到界面(ObjectRow);			//foreach (DataRow row in objlist)
		}

		//对本基类树里边的每一个节点提取出来，看是否是另一棵树里边的节点。
		public void 递归得到共同基类(参数树结构 另一棵基类树, ref List<参数树结构> 结果)
		{
			if (另一棵基类树.递归从基类树中查找广义匹配的基类(this.目标ID, 替代.正向替代) != -1)
				结果.Add(this);
			if (this.子节点 == null)
				return;
			foreach (参数树结构 obj in this.子节点)
			{
				obj.递归得到共同基类(另一棵基类树, ref 结果);
			}
		}


		public 参数树结构 递归查找优先的共同基类(参数树结构 另一棵基类树, bool 允许目标派生)
		{
			if (允许目标派生 == false)
			{
				if (另一棵基类树.目标ID.Equals(this.目标ID))//对另一棵树只允许根等于，不允许基类树计算。
					return this;
			}
			else
			{
				if (另一棵基类树.递归从基类树中查找广义匹配的基类(this.目标ID, 替代.正向替代 | 替代.聚合替代) != -1)
					return this;
			}
			if (this.子节点 == null)
				return null;
			foreach (参数树结构 obj in this.子节点)
			{
				参数树结构 o = obj.递归查找优先的共同基类(另一棵基类树, 允许目标派生);
				if (o != null)
					return o;
			}
			return null;
		}

		public void 递归根据形式增加参数(生长对象 所属对象, List<参数> 结果, int 语言, 生长对象 前置介词, 生长对象 后置介词, 生长对象 的或地)
		{
			//必须有介词，否则不做调整。
			if (前置介词 == null && 后置介词 == null)
				return;

			if (Data.概念Guid.Equals(目标ID))
				return;

			int 当前语言 = (int)(目标.语言);
			if (字典_语言.满足语义或者指定语言(语言, 当前语言))
			{
				foreach (参数 o in 结果)
					if (Data.是派生关联(目标ID, o.源关联记录) > 0)
						goto 结束;

				Guid 基本类型 = Data.一级关联类型(目标);

				if (Data.拥有Guid.Equals(基本类型) == false)//只考虑拥有的情况。
					goto 结束;

				List<参数> 关联参数集合 = new List<参数>();
				参数树结构 tree = Data.利用缓存得到基类和关联记录树(目标, true);
				tree.递归取出关联的形式参数(null, ref 关联参数集合, Data.当前解析语言, 0, 0);
				int k = 0;
				foreach (参数 形式 in 关联参数集合)
				{
					if (前置介词 != null && Data.是派生关联(Data.关联拥有前置介词Guid, 形式.源关联记录) > 0 && 前置介词.取子串 == 形式.源关联记录.形式)
						k++;
					if (后置介词 != null && Data.是派生关联(Data.关联拥有后置介词Guid, 形式.源关联记录) > 0 && 后置介词.取子串 == 形式.源关联记录.形式)
						k++;
				}

				if (k > 0)
					结果.Add(new 参数(所属对象, 目标, 字典_语言角色.中心, 生成树时的路径起始端));
			}

		结束:

			if (this.子节点 == null)
				return;
			foreach (参数树结构 obj in this.子节点)
				obj.递归根据形式增加参数(所属对象, 结果, 语言, 前置介词, 后置介词, 的或地);
		}


		public void 递归取出形式和关键参数(生长对象 所属对象, ref List<参数> 结果, int 语言, int 层级, int 阀值)
		{
			if (Data.概念Guid.Equals(目标ID))
				return;

			//if (生成树时的路径起始端 == 字典_目标限定.B端)//一般是【等价】记录。//只能是A端或者连接端（在二元关联时，从而也转换为A端）。不能是B端。
			//	return;

			int 当前语言 = (int)(目标.语言);
			if (字典_语言.满足语义或者指定语言(语言, 当前语言))
			{
				foreach (参数 o in 结果)
					if (Data.是派生关联(目标ID, o.源关联记录) > 0)//已经有派生的了。
						goto 结束;

				if (Data.是拥有形式(目标))//暂时让语义拥有的形式都算是必须具备的关键参数。最后也可以都归结到参数集合上来做。
				{
					//【的】和【地】只能是隐含关联带出的，显示的【关联】不具有【的】和【地】形式，所以，应该在去除关联的形式参数里边才能查到，这里不能查到。
					if (层级 == 1 && Data.关联拥有的Guid.Equals(目标.ID) == false && Data.关联拥有地Guid.Equals(目标.ID) == false)
						结果.Add(new 参数(所属对象, 目标, 字典_语言角色.中心, 字典_目标限定.A端));
				}
				else
				{
					if (生成树时的路径起始端 == 字典_目标限定.A端)//根据【A关键性】来判断是否关键。
					{
						//参数字段 参数 =new 参数字段((string)目标.参数集合);
						if (目标.参数.B对A的关键性 >= 阀值)
							结果.Add(new 参数(所属对象, 目标, 字典_语言角色.中心, 字典_目标限定.A端));
					}
					else
					{
						//参数字段 参数 = new 参数字段((string)目标.参数集合);
						if (目标.参数.B对A的创建性 >= 阀值)
							结果.Add(new 参数(所属对象, 目标, 字典_语言角色.中心, 字典_目标限定.B端));
					}
				}
			}

		结束:

			if (this.子节点 == null)
				return;
			foreach (参数树结构 obj in this.子节点)
				obj.递归取出形式和关键参数(所属对象, ref 结果, 语言, 层级 + 1, 阀值);
		}

		public void 递归取出关联的形式参数(生长对象 所属对象, ref List<参数> 结果, int 语言, int 层级, int 阀值)
		{
			if (Data.概念Guid.Equals(目标ID))
				return;

			int 当前语言 = (int)目标.语言;
			if (字典_语言.满足语义或者指定语言(语言, 当前语言))
			{
				//对于关联，暂时只列出形式。因为关键参数其实就是A端和B端，而这是体现在代码中的。一次完成。
				if (Data.是拥有形式(目标))//暂时让语义拥有的形式都算是必须具备的关键参数。最后也可以都归结到参数集合上来做。
				{
					if (层级 == 1)
					{
						bool 被重载 = false;
						foreach (参数 o in 结果)
							if (Data.是派生关联(目标ID, o.源关联记录) > 0)
							{
								被重载 = true;
								break;
							}
						if (被重载 == false)
							结果.Add(new 参数(所属对象, 目标, 目标.语言角色, 字典_目标限定.A端));
					}
				}
				//else//根绝【A关键性】来判断是否关键。
				//{
				//    参数集合字段 参数 = new 参数集合字段((string)目标["参数集合"]);
				//    if (发起端 == 字典_目标限定.A端 && 参数.A端关键性 > 阀值 && 字典_语言.满足语义或者指定语言(语言, 当前语言))
				//        结果.Add(new 参数(目标, 参数.A端关键性));
				//    else if (发起端 == 字典_目标限定.B端 && 参数.B端关键性 > 阀值 && 字典_语言.满足语义或者指定语言(语言, 当前语言))
				//        结果.Add(new 参数(目标, 参数.B端关键性));
				//}
			}
			if (this.子节点 == null)
				return;
			foreach (参数树结构 obj in this.子节点)
				obj.递归取出关联的形式参数(所属对象, ref 结果, 语言, 层级 + 1, 阀值);
		}

		public 模式 得到拥有特定形式(string 形式)
		{
			if (this.子节点 == null)
				return null;

			foreach (参数树结构 obj in this.子节点)
			{
				模式 o = obj.目标;
				if (Data.是拥有形式(o))
				{
					if (Data.取得嵌入串(o) == 形式)
						return o;
					//{
					//    if (o.参数.概率分 == 0)//已经被屏蔽。
					//        return null;
					//    return o;
					//}
					//while (Data.拥有形式集合.Contains(o.源记录) == false && Data.拥有形式集合.Contains(o.源记录) == false)
					//{
					//    o = Data.FindRowByID(o.源记录);//被重载。
					//    if (Data.取得嵌入串(o.形式) == 形式)
					//        return null;
					//}
				}
			}
			//foreach (参数树结构 obj in this.子节点)
			//{
			//    模式 r = obj.递归得到拥有特定形式(形式);
			//    if (r != null)
			//        return r;
			//}

			return null;
		}

	}

	public class 并列关联
	{
		public 模式 基类;
		public int 基准层级;
		基准类型 基准对象;
		public int 距离1;
		public int 距离2;
		public int 总距离
		{
			get
			{
				if (基准层级 >= 4)
					return 9;
				return 基准层级 + 2 - (距离1 + 距离2);
			}
		}
		public 并列关联(模式 基类, int 距离1, int 距离2)
		{
			this.基类 = 基类;
			this.距离1 = 距离1;
			this.距离2 = 距离2;
			基准类型 基准对象 = null;
			基准层级 = Data.计算基准对象(基类, ref 基准对象);
		}
	}


	public class 三级关联
	{
		public bool 有效 = true;
		public int 级数;
		public 参数树结构 开始端聚合关联;
		public 参数树结构 中心主关联;
		public 参数树结构 结束端聚合关联;        //注意：用这个方法，是要检查使用这个的地方是否有错需要调整。因为把以前【关联】的转换为【路径】后没有仔细检查。
		public int that端;//只有一个
		public bool 反向 = false;

		static public 三级关联 遍历关联 = new 三级关联();
		static public List<模式>[] 已遍历过模式 = { new List<模式>(), new List<模式>(), new List<模式>() };

		public bool 在当前级中找到已经遍历过的派生关联且抑制基关联(模式 row)
		{
			if (级数 == 3)
				return false;

			foreach (模式 obj in 已遍历过模式[级数])
				if (obj.源记录.Equals(row.ID))
					if ((obj.参数.扩展位码 & 参数字段.抑制基关联) > 0)
						return true;

			return false;
		}

		public static void 初始化()
		{
			已遍历过模式[0] = new List<模式>();
			遍历关联.级数 = 0;
		}

		public void 在当前级中加入一个遍历过的派生关联(模式 row)
		{
			if (级数 < 3)
				已遍历过模式[级数].Add(row);
		}
		public int 语义打分
		{
			get
			{
				//if (并列关联的基类 != null)
				//{
				//    打分 = 9;//其实这里应该根据这个类型的具体化程度进行打分。
				//}
				//else
				//{
				int 打分 = 中心主关联.目标.参数.概率分;
				if (开始端聚合关联 != null)
					打分 = Data.合并概率打分(打分, 开始端聚合关联.目标.参数.概率分);
				if (结束端聚合关联 != null)
					打分 = Data.合并概率打分(打分, 结束端聚合关联.目标.参数.概率分);
				//}
				return 打分;

			}
		}

		override public string ToString()
		{
			string str = 有效.ToString();
			//if (并列关联的基类 != null)
			//    str += 并列关联的基类.ToString();
			//else
			//{
			int i = 1;
			if (开始端聚合关联 != null)
				str += i++.ToString() + "、" + 开始端聚合关联.ToString();
			if (中心主关联 != null)
				str += i++.ToString() + "、" + 中心主关联.ToString();
			if (结束端聚合关联 != null)
				str += i++.ToString() + "、" + 结束端聚合关联.ToString();
			//}
			return str;
		}


		public bool 调整that(bool 前边有的关联)
		{
			return true;
			//int 要求的that = 中心主关联.假设中心that;
			//if (级数 == 1)
			//{
			//    int 路径关联起始端 = 中心主关联.生成树时的路径起始端;
			//    if ((要求的that & 路径关联起始端) == 0)
			//        return false;
			//    return true;
			//}
			//if (开始端聚合关联 != null)
			//{
			//    int 路径关联起始端 = 开始端聚合关联.生成树时的路径起始端;
			//    if ((路径关联起始端 & Data.正向关联) == 0)//不允许正向关联的调整为反向。
			//        开始端聚合关联.假设中心that = 字典_目标限定.B端;
			//    if ((路径关联起始端 & Data.反向关联) == 0)//不允许反向关联的调整为正向。
			//        开始端聚合关联.假设中心that = 字典_目标限定.A端;
			//}
			//if (结束端聚合关联 != null)
			//{
			//    int 路径关联起始端 = 结束端聚合关联.生成树时的路径起始端;
			//    if ((路径关联起始端 & Data.正向关联) == 0)//不允许正向关联的调整为反向。
			//        结束端聚合关联.假设中心that = 字典_目标限定.B端;
			//    if ((路径关联起始端 & Data.反向关联) == 0)//不允许反向关联的调整为正向。
			//        结束端聚合关联.假设中心that = 字典_目标限定.A端;
			//}
			//return true;
		}

		public 三级关联(模式 中心关联, 模式 开始端关联 = null, 模式 结束端关联 = null)
		{
			中心主关联 = new 参数树结构(中心关联, 字典_目标限定.A端, false);
			that端 = 字典_目标限定.A端;
			级数 = 1;
			if (开始端关联 != null)
			{
				开始端聚合关联 = new 参数树结构(开始端关联, 字典_目标限定.A端, false);
				级数++;
			}
			if (结束端关联 != null)
			{
				结束端聚合关联 = new 参数树结构(结束端关联, 字典_目标限定.A端, false);
				级数++;
			}
		}
		public 三级关联()
		{
		}
		public 三级关联(参数树结构 参数树)
		{
			//暂时用第一个，后边要修改。
			中心主关联 = 参数树;
			级数 = 1;
			that端 = 字典_目标限定.A端;
		}

		public bool 等价(三级关联 o)
		{
			if (级数 != o.级数)
				return false;
			return 开始端聚合关联 == o.开始端聚合关联 && 中心主关联 == o.中心主关联 && 结束端聚合关联 == o.结束端聚合关联;
		}

		public bool 是派生关联路径(三级关联 基关联路径)
		{
			Data.Assert(级数 > 0);
			if (级数 > 基关联路径.级数)
				return false;
			if (开始端聚合关联 != null || 基关联路径.开始端聚合关联 != null)
				if ((开始端聚合关联 == null || 基关联路径.开始端聚合关联 == null) || Data.是派生关联(基关联路径.开始端聚合关联.目标ID, 开始端聚合关联.目标) == 0)
					return false;
			if (中心主关联 != null)
				if (Data.是派生关联(基关联路径.中心主关联.目标ID, 中心主关联.目标) == 0)
					return false;
			if (结束端聚合关联 != null || 基关联路径.结束端聚合关联 != null)
				if ((结束端聚合关联 == null || 基关联路径.结束端聚合关联 == null) || Data.是派生关联(基关联路径.结束端聚合关联.目标ID, 结束端聚合关联.目标) == 0)
					return false;
			return true;
		}
		public bool 加入一级(参数树结构 参数树)
		{
			//只有一级的情况下才允许加入属于。
			if (级数 > 0 && 替代.是本质分类(Data.一级关联类型(参数树.目标)))
				return false;
			if (级数 == 0)
				开始端聚合关联 = 参数树;
			else if (级数 == 1)
				中心主关联 = 参数树;
			else if (级数 == 2)
				结束端聚合关联 = 参数树;
			else
				return false;

			if (++级数 < 3)
				已遍历过模式[级数] = new List<模式>();

			return true;
		}
		public void 回退一级()
		{
			Data.Assert(级数 > 0);
			级数--;
		}
		public bool 可以进入下级(参数树结构 参数树)
		{
			Guid 下一关联类型 = Data.一级关联类型(参数树.目标);

			//只允许【聚合】【拥有】两种加入。【推导】【本质属于】等都不行。
			//本质分类如果满足，前边已经就加入到结果了，不会再进行后续的处理。
			if (替代.是聚合或者属拥(下一关联类型) == false && Data.拥有Guid.Equals(下一关联类型) == false && Data.并列关联Guid.Equals(下一关联类型) == false)
				return false;

			if (级数 == 0)
				return true;
			if (级数 == 1)
			{
				Guid 上一关联类型 = Data.一级关联类型(开始端聚合关联.目标);

				if (Data.能进行一级传递(开始端聚合关联.目标) == false)
					return false;

				if (Data.能进行二级传递(参数树.目标) == false)
					return false;

				if (替代.是聚合或者属拥(上一关联类型) && Data.是派生关联(Data.并列聚合Guid, 开始端聚合关联.目标) == 0)//并列聚合不可以。
				{

					//【事物属拥存在阶段】的【存在阶段】必须显式化，所以，不能作为两级关联计算，只是一级可以计算。但【关系属拥成立阶段】的【成立阶段】则可以隐含，所以是允许的。
					if (Data.是派生关联(Data.事物属拥存在阶段Guid, 开始端聚合关联.目标) > 0 && Data.是派生关联(Data.关系属拥成立阶段Guid, 开始端聚合关联.目标) == 0)
						return false;

					//【事物属拥存在类型】和【关系属拥成立类型】都不创建，因为要求【存在】【成立】【发生】必须是显式出现。
					if (Data.是派生关联(Data.事物属拥存在类型Guid, 开始端聚合关联.目标) > 0)
						return false;
					if (Data.拥有Guid.Equals(下一关联类型))
						return true;
					if (Data.聚合Guid.Equals(上一关联类型) && Data.属拥Guid.Equals(下一关联类型))
						return true;
				}
				if ((Data.拥有Guid.Equals(上一关联类型) || Data.属拥Guid.Equals(上一关联类型)) && Data.聚合Guid.Equals(下一关联类型))
					return true;
			}
			if (级数 == 2)
			{
				Guid 上一关联类型 = Data.一级关联类型(中心主关联.目标);
				//第二级必须是【拥有】或者【属拥】
				if (Data.拥有Guid.Equals(上一关联类型) == false && Data.属拥Guid.Equals(上一关联类型) == false)
					return false;
				//第三级必然是聚合，不能是属拥或者拥有。
				if (Data.聚合Guid.Equals(下一关联类型) && Data.是派生关联(Data.并列聚合Guid, 开始端聚合关联.目标) == 0)
					return true;
			}

			return false;
		}

		public void 调整三级关联次序()
		{
			if (中心主关联.生成树时的路径起始端 == 字典_目标限定.B端)
			{
				if (级数 > 1)
				{
					参数树结构 o = 开始端聚合关联;
					开始端聚合关联 = 结束端聚合关联;
					结束端聚合关联 = o;
				}
				that端 = 字典_目标限定.另一端(that端);
				反向 = true;
			}
			//注意：这样进行调整后，就成为了A->B完全正向的关联形式。【生成树时的路径起始端】就都看着是A端了。


			//需要计算的对象对已经假设了中心和参数，这里是因为关联存在方向的问题，调整that值，来保证对象对认为的中心和参数是正确的。
			//1、调用计算三级关联的时候，是用【左对象】向【右对象】的方向传递的参数也就是A->B的方向，这时如果【假设中心在右】，那么中心就是B端。
			//2、在此之前，本三级关联的that端其实记录了三级关联是正向和反向的。
			//因此，这两个参数共同影响最后设定that值才和假设的中心是一致的。
			//if (假设中心在右)
			//	that端 = 字典_目标限定.另一端(that端);
		}

		public 三级关联 整理出一个结果(int 假设发起端)
		{
			三级关联 结果 = new 三级关联();

			if (级数 == 1)
				结果.中心主关联 = 开始端聚合关联;
			else if (级数 == 2)
			{
				//【聚合+拥有】
				if (Data.是派生关联(Data.聚合Guid, 开始端聚合关联.目标) > 0 || Data.是派生关联(Data.属拥Guid, 开始端聚合关联.目标) > 0)
				{
					结果.开始端聚合关联 = 开始端聚合关联;
					结果.中心主关联 = 中心主关联;
				}
				else//【拥有+聚合】
				{
					结果.中心主关联 = 开始端聚合关联;
					结果.结束端聚合关联 = 中心主关联;
				}
			}
			else//三级
			{
				结果.开始端聚合关联 = 开始端聚合关联;
				结果.中心主关联 = 中心主关联;
				结果.结束端聚合关联 = 结束端聚合关联;
			}

			结果.that端 = 假设发起端;
			结果.级数 = 级数;

			return 结果;
		}

		public 参数树结构 左端关联
		{
			get
			{
				if (开始端聚合关联 != null)
					return 开始端聚合关联;
				return 中心主关联;
			}
		}
		public 参数树结构 右端关联
		{
			get
			{
				if (结束端聚合关联 != null)
					return 结束端聚合关联;
				return 中心主关联;
			}
		}
	}


	public class EnglishProcessor : Processor
	{
		public override int This语言
		{
			get
			{
				return 字典_语言.英语;
			}
		}
	}

	public class ChineseProcessor : Processor
	{
		public override int This语言
		{
			get
			{
				return 字典_语言.汉语;
			}
		}
		public override char[] Get分句子字符()
		{
			return 汉语分句字符;
		}

	}

	//【关联】总是配合【的】使用，没有【的】就不会用关联。两者都和【序列状态】是紧密关联。
	//主谓宾也可以处理到这个里边？
	//关联式和【的】配合。而【主谓宾】正好是和【地】进行配合定义的。
	/*以下主要对参数进行控制，也包括参数自己的类型*/
	static public class 字典_参数
	{
		public const int 无 = 0;//全部隐藏，连参数都没有。这时，字典_角色也都是无效的。这条记录只是内部分析，表面上部显示。
		public const int 参 = 64;//仅显示参数
		public const int 的 = 72;//64+8 参数+的
		public const int 地 = 80;//64+16 参数+地
		public const int 参类 = 96;//64+32 参数+类型
		public const int 类的 = 104;//64+32+8参数+类型+的
		public const int 类地 = 112;//64+32+16参数+类型+地
		//后边位数还对于【的】和【地】的形态进行扩展，比如英语里边的of的方式还是's的方式等。

		//public const int 类 = 64;//类型名
		//public const int 的 = 144;//16+128参数和的
		//public const int 地 = 160;//32+128参数和地

	}

	public class 参数字段
	{
		//都是对A端（或者二元关联的连接端）来说的（B端是被动的）。
		//从A端的模式对象来看，所有的B端的拥有都是参数，【拥有形式】和【拥有参数】都是一样的！！并列在一起，共同定义【创建关键性】和【完成关键性】打分。

		public int 方向;
		public int B对A的创建性;//有了B以后，对于A端对象进行创建的必要性。是以多个记录总分来看待的。每条记录最多可以打分为9，多条记录加起来的总分等于或大于9，那么就可以触发A端对象的创建。
		public int B对A的关键性;//B对于A端对象的关键性。是各条记录单独看待的，最大是9，分值越大，就越要求这个参数要有明确的取值，而不能模糊。（语言上的省略如果能推断出值，也是可以的）

		public int 正误分;//这个出现的正确性打分。
		public int 概率分;//这个结合的出现概率。比如【一个红色红果】。【苹果和一个】结合优先远大于【红色和一个】的结合。
		public int 具体化程度分;//这个关联的具体化程度。
		public int 在左端时靠外层级分;//一般是动词们进行比较，打分越高，这个对象可能越容易靠在句子树的外层--也就是更接近根。是否要扩展为【在左边时的靠外层级】和【在右边时的靠外层级】？
		//-9到9。0是默认值，表示最极端情况下是允许的，但是正常的时候不会得到阀值。
		//负值表示反对，负值越大越反对。

		public int Aa;//指数而不是比例数。将[A端]和[其它参数]看着两个命题它们的统计概率。比如[[苹果]拥有颜色]，那么这个量可以置为9，因为[100%的苹果][拥有颜色]。如果信息加上了【环境】【相对标准】的信息后，就更完整了。这个量还可以当着概率的概念,对于[如果...那么...]的命题的两端,同样适用！
		public int Ab;//同Aα反向。对于[[苹果]拥有颜色]，这个值为1。因为[拥有颜色]的对象只有极少数[是苹果]。但不能为0，0就表示不可能是了。具体地，0表示[不可能是],9表示[肯定]，其它值表示[可能]。
		public int Ba;//同Aα类似，只是将[B端]和[其它参数]看着两个命题。比如[苹果拥有[颜色]]，那么这个量可以置为1，因为只有[很少量的颜色][被苹果拥有]。
		public int Bb;	//同Aβ类似。对于[苹果拥有[颜色]]，这个值为1。[苹果拥有]的东西有一定可能[是颜色]。
		public int 联a;//将[连接端]和[其它参数]看着两个命题，比如[苹果[拥有]颜色]，那么这个量可以置为1，因为只有[几乎可以忽略的拥有]属于[苹果和颜色之间的关联]。
		public int 联b;//同联α反向。对于[苹果[拥有]颜色]，这个值为5。[苹果和颜色之间的关联]有极大可能[是拥有]。
		public int B端重复数;//A端一般都不允许重复（因为是树）。这个值不继承，每个关联自己的单独有效！
		public int 说话方;//先这样简便处理，其实应该直接放两个ID字段，来表现说话方和听话方。现在0，表示计算机说的话。1表示人给计算机说的话。
		public int 扩展位码;
		public int 词性扩展;
		public int 层级值; //用于层级概念的级别
		public static 参数字段 缺省参数集合 = new 参数字段();


		public static int A端不再向后派生 = 1;
		public static int B端不再向后派生 = 2;
		public static int 抑制基关联 = 4;
		public static int A端不向前继承 = 8;
		public static int B端不向前继承 = 16;
		public static int A端和B端必须紧密相邻 = 32;

		public static int 词性_包含宾语 = 1;
		public static int 词性_离合式 = 2;
		public static int 词性_小品词组 = 4;
		public static int 词性_名词后缀 = 8;
		public static int 词性_简称 = 16;
		public 参数字段()
		{
			方向 = Data.正向关联;
			B对A的创建性 = 5;
			B对A的关键性 = 0;

			正误分 = 9;
			概率分 = 9;
			具体化程度分 = 0;
			在左端时靠外层级分 = 0;

			Aa = 0;
			Ab = 0;
			Ba = 0;
			Bb = 0;
			联a = 0;
			联b = 0;
			B端重复数 = 0;
			说话方 = 0;
			扩展位码 = 0;
			词性扩展 = 0;
			层级值 = 0;
		}

		public 参数字段(string str)
		{
			ReadString(str);
		}



		override public string ToString()
		{
			string str = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", 方向, B对A的创建性, B对A的关键性, 正误分, 概率分, 具体化程度分, 在左端时靠外层级分, Aa, Ab, Ba, Bb, 联a, 联b, B端重复数, 说话方, 扩展位码, 词性扩展, 层级值);
			return str;
		}

		public void ReadString(string str)
		{
			//long t = System.DateTime.Now.Ticks;
			if (str == null)
				return;
			String[] strings = str.Split(',');
			int i = 0;
			if (strings.Length > i && strings[i] != "")
				方向 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				B对A的创建性 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				B对A的关键性 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				正误分 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				概率分 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				具体化程度分 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				在左端时靠外层级分 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				Aa = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				Ab = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				Ba = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				Bb = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				联a = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				联b = (int)float.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				B端重复数 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				说话方 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				扩展位码 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				词性扩展 = int.Parse(strings[i++]);
			if (strings.Length > i && strings[i] != "")
				层级值 = int.Parse(strings[i++]);
			//Data.timeCount += System.DateTime.Now.Ticks - t;
		}
	}

	static public class 字典_显隐
	{
		public const int 正常 = 0;
		public const int 隐藏 = 2;//不显示
		public const int 附加 = 4;//不显示
		public const int 无效 = 8;//不显示
		//		public const int 离合 = 4;//不显示
	}

	static public class 字典_目标限定
	{
		public const int 空 = 0;
		public const int A端 = 4;
		public const int B端 = 5;
		public const int 连接 = 6;
		public static int 另一端(int 本端)
		{
			if (本端 == A端)
				return B端;
			if (本端 == B端)
				return A端;
			Data.Assert(false);
			return 0;
		}
	}
	public class 语言对照类
	{
		public int 语言ID;
		public Guid 语言模式Guid;
		public static 语言对照类[] 语言对象表 = new 语言对照类[]
        {
            new 语言对照类(字典_语言.汉语,Data.汉语Guid),
            new 语言对照类(字典_语言.英语,Data.英语Guid)
        };
		public static int 获取语言ID(Guid 模式ID)
		{
			foreach (语言对照类 obj in 语言对象表)
			{
				if (obj.语言模式Guid.Equals(模式ID))
					return obj.语言ID;
			}
			return Data.当前解析语言;
		}
		public 语言对照类(int 语言, Guid 模式ID)
		{
			语言ID = 语言;
			语言模式Guid = 模式ID;
		}
	}
	static public class 字典_语言
	{
		public const int 公语 = 0x7fffffff;
		public const int 语义 = 0;
		public const int 汉语 = 0x1000000;
		public const int 英语 = 0x100;

		public static bool 满足语义或者指定语言(int 要求语言, int 实际语言)
		{
			if (实际语言 == 语义)
				return true;
			return (要求语言 & 实际语言) > 0;
		}
		public static bool 满足指定语言(int 要求语言, int 实际语言)
		{
			return (要求语言 & 实际语言) > 0;
		}
	}

	static public class 字典_语言角色
	{
		//以后考虑前后位置要分别占一个位置吧。
		//public const int 前位=
		//public const int 后位=

		//public const int 根中心 = 1 << 29;
		//public const int 根谓语 = 1 << 30;

		//作为根的取值是原始状态的【中心语】

		//public const int 中心 = 1 << 1;//普通事物概念的根
		//public const int 谓语 = 1 << 2;//序列化了的根

		public const int 无 = 0;//

		public const int 中心 = 1;//【借】等常说的谓语动词也归于中心语。
		public const int 名谓 = 1 << 1;//中心语稍微的变种，主要就是用在【苹果红了】这种情况下，一个名词用作谓语的形式时候的差别。
		//真正谓语隐藏后，以前的宾语形式上好像谓语一样的表现。原始谓语一般是【是】。

		//public const int 前边 = 1 << 2;//前边，在中心语前边，
		//public const int 后边 = 1 << 3;//后边，在中心语后边
		public const int 前独 = 1 << 2;//前独，一般在主语位置

		public const int 句首 = 1 << 3;//最前边，在主语前边，特殊的一些符号等。
		public const int 前定 = 1 << 4;//中心语前的定语，但是在主语（如果有的话）之后。
		public const int 后定 = 1 << 5;//中心语后的定语
		public const int 定语 = 前定 | 后定;
		public const int 从定 = 1 << 6;//从句定语。这个比较特殊。 //更名为前从定
		public const int 后从定 = 1 << 7; //后从定，由于英语动词时态问题需要增加

        public const int 前或从定 = 前定 | 从定;

		public const int 前状 = 1 << 8;//谓语前状语，但是在主语之后。
		public const int 后状 = 1 << 9;//谓语后状语
		public const int 得状 = 1 << 10;//【得】后状语，【得】肯定在谓语以后。
		//public const int 状被 = 1 << 11;//被前状语，比如【他昨天被】。
		public const int 状语 = 前状 | 后状;

		public const int 主语 = 1 << 12;//主语，在谓语前
		public const int 集主 = 1 << 13;//集合主语，依附于主语，是对主语的增加信息。

		public const int 前句 = 1 << 14;  // 前句，放在句首
		public const int 后句 = 1 << 15;  // 后句，放在句末
		public const int 主被 = 1 << 21;//宾语，位置移到谓语前，一般也在主语前
		public const int 把宾 = 1 << 22;//宾语，位置移到谓语前，在主语后宾语

		public const int 兼宾 = 1 << 17;//兼语式的第二宾语
		public const int 一宾 = 1 << 18;//谓语后第一宾语
		public const int 二宾 = 1 << 19;//谓语后第二宾语
		public const int 谓宾 = 1 << 20;//【他借书给了她】的情况。【给了她】整体就是谓语状语，特点就是这个角色的介词后边带着【了】。
		public const int 宾语 = 一宾 | 二宾;
		public const int 兼一宾 = 兼宾 | 一宾;
		public const int 兼二宾 = 兼宾 | 二宾;

		public const int 句尾 = 1 << 27;//
		public const int 尾标 = 1 << 28;//结尾的标点符号，一般是句号。

		public const int 前同 = 1 << 29;
		public const int 后同 = 1 << 30;
		public const int 同位 = 前同 | 后同;

		public const int 非主语 = 0x7FFFFFFF ^ 主语;
		public const int 非一宾 = 0x7FFFFFFF ^ 一宾;
		public const int 非二宾 = 0x7FFFFFFF ^ 二宾;
		public const int 非主宾 = 0x7FFFFFFF ^ (主语 | 一宾 | 二宾);

		public const int 全部 = 0x7FFFFFFF;
		//public const int 中置介 = 1 << 24;//放在句子中间的介词，比如【他如果昨天游泳...】里边的【如果】。
		//现在去掉，是因为拥有位置标示就够了。

		//public const int 全部中 = 0x3FFFFFFF;

		//public const int 全部谓 = 0x5FFFFFFF;

		//public const int 全部全 = 0x7FFFFFFF;

		//public const int 非谓全 = 全部全 - 谓语;

		//public const int 叶子序列态 = 64;
		//public const int 叶子原始态 = 128;
		//public const int 根序列态 = 16384;
		//public const int 根原始态 = 0;

		////作为根的取值是原始状态的【中心语】
		//public const int 中心 = 0;
		//public const int 谓语 = 64;//序列化
		//public const int 定语 = 128;//叶子语的派生
		//public const int 状语 = 192;//定语加序列化
		//public const int 主语 = 208;//状语的派生192+16
		//public const int 宾语 = 224;//状语的派生192+32
		//public const int 一宾 = 228;//宾语的派生224+8
		//public const int 二宾 = 232;//宾语的派生224+4
		//public const int 被主 = 464;//主语的被形态208+256
		//public const int 主被 = 480;//宾语的被形态224+256
		//public const int 把宾 = 736;//宾语的把形态224+512

		//作为根的取值是序列化的【谓语】
		//根的角色取值只有两种，因为肯定是中心。所以区别在于原始态还是序列态而已。原始态的取值是0，和上边一样。只有序列态取值标记为1。占位取值到16384
		//public const int 中心谓 = 16384;//
		//public const int 谓语谓 = 16448;//
		//public const int 定语谓 = 16512;//
		//public const int 状语谓 = 16576;//
		//public const int 主语谓 = 16592;//
		//public const int 宾语谓 = 16608;//
		//public const int 一宾谓 = 16612;//
		//public const int 二宾谓 = 16616;//
		//public const int 被主谓 = 16848;//
		//public const int 主被谓 = 16864;//
		//public const int 把宾谓 = 17120;//

		//这里要求只能是具体角色，不能是多个角色的混合体。
		static public int 计算一级序号(int 语言角色)
		{
			//语言角色 &= 字典_语言角色.全部叶;
			int v = 0;
			switch (语言角色)
			{
				case 字典_语言角色.无:
					v = 0;
					break;
				case 字典_语言角色.前句:
					v = -1000000;
					break;


				case 字典_语言角色.句首:
					v = -500000;
					break;

				case 字典_语言角色.主被:
					v = -400000;
					break;

				//case 字典_语言角色.状被://被前状语。
				//    v = -300000;
				//    break;
				case 字典_语言角色.前独:
					v = -300000;
					break;

				case 字典_语言角色.主语:
					v = -200000;
					break;

				case 字典_语言角色.集主:
					v = -200000;
					break;

				case 字典_语言角色.把宾:
					v = -100000;
					break;

				case 字典_语言角色.前状:
					v = -100000;
					break;

				case 字典_语言角色.前定:
					v = -100000;
					break;

				case 字典_语言角色.从定:
					v = -100000;
					break;

				case 字典_语言角色.前同:
					v = -10000;
					break;

				case 字典_语言角色.中心:
					v = 0;
					break;

				case 字典_语言角色.后同:
					v = 10000;
					break;

				case 字典_语言角色.后定:
					v = 100000;
					break;

				case 字典_语言角色.后状:
					v = 100000;
					break;

				//case 字典_语言角色.兼宾:
				//    v = 200000;
				//    break;

				case 字典_语言角色.一宾:
					v = 200000;
					break;

				case 字典_语言角色.二宾:
					v = 300000;
					break;

				case 字典_语言角色.谓宾://接近在二宾的位置上。
					v = 300000;
					break;

				case 字典_语言角色.得状://得状的位置比所有宾语都靠后。比如【他借书给她得很高兴】。基本可以说都是在句尾。
					v = 400000;
					break;

				case 字典_语言角色.句尾:
					v = 500000;
					break;

				case 字典_语言角色.尾标:
					v = 900000;
					break;
				case 字典_语言角色.后句:
					v = 1000000;
					break;


				default:
					Data.Assert(false);//只能是具体角色，不能是多个角色的混合体。
					break;
			}
			return v;
		}
	}

	public class 附加关联结构
	{
		public 模式 附加关联模式行;
		public 模式树结构 A端模式树结构;
		public 模式树结构 B端模式树结构;

		public 附加关联结构(模式 源附加关联模式行, 模式树结构 A端源模式树结构, 模式树结构 B端源模式树结构)
		{
			this.附加关联模式行 = 源附加关联模式行;
			this.A端模式树结构 = A端源模式树结构;
			this.B端模式树结构 = B端源模式树结构;
		}
	}

	public class 推导结构
	{
		public 推导结构 复制基模板;//以后，把基模板预先加载好，用的时候直接复制一个，效率更高。
		public 推导结构 基推导;//推导也可以派生，但是一般是单继承吧。


		public 模式 源推导模式行;
		public bool 正向推导;

		public 模式树结构 A端模式树结构;
		public 模式树结构 B端模式树结构;
		public int 推导状态;
		public List<附加关联结构> 附加关联结构集合;

		public override string ToString()
		{
			return 源推导模式行.形式;
		}

		public void 建立基本附加关联集合()
		{

			附加关联结构集合 = new List<附加关联结构>();

			var 附加等价参数Rows = Data.模式表.对象集合.Where(r => r.C端 == 源推导模式行.ID);

			foreach (模式 附加关联 in 附加等价参数Rows)
			{
				if (Data.等价Guid.Equals(附加关联.连接) == false || 附加关联.That根 != 字典_目标限定.连接)
					continue;

				模式树结构 A端 = A端模式树结构.递归找到附加关联对应模式行(附加关联.A端);
				if (A端 != null)
				{
					模式树结构 B端 = B端模式树结构.递归找到附加关联对应模式行(附加关联.B端);
					if (B端 != null)
					{
						附加关联结构集合.Add(new 附加关联结构(附加关联, A端, B端));
					}
				}

			}

		}

		//public List<附加关联结构> 为推导出的两个模式树实例建立附加关联(模式树结构 A模式树结构, 模式树结构 B模式树结构)
		//{
		//	List<附加关联结构>  附加关联集合 = new List<附加关联结构>();

		//	foreach (附加关联结构 源附加关联 in 源附加关联集合)
		//	{
		//		if (源附加关联.A端源模式树结构.匹配目标 != null && 源附加关联.B端源模式树结构.匹配目标 != null)
		//			附加关联集合.Add(源附加关联.A端源模式树结构.匹配目标, 源附加关联.B端源模式树结构.匹配目标);
		//	}

		//	return 附加关联集合;
		//}

		public 模式树结构 根据附加关联将参数从一棵树传递给另一棵树(模式树结构 源树, 模式树结构 目标树, bool 正向)
		{

			foreach (附加关联结构 附加关联结构 in 附加关联结构集合)
			{
				模式 附加关联 = 附加关联结构.附加关联模式行;
				if (Data.等价Guid.Equals(附加关联.连接) == false || 附加关联.That根 != 字典_目标限定.连接)
					continue;

				Guid 目标变量 = 正向 == this.正向推导 ? 附加关联.B端 : 附加关联.A端;
				Guid 源变量 = 正向 == this.正向推导 ? 附加关联.A端 : 附加关联.B端;

				模式树结构 目标端 = 目标树.递归找到附加关联对应模式行(目标变量);
				if (目标端 != null)
				{
					模式树结构 源端 = 源树.递归找到附加关联对应模式行(源变量);
					if (源端 != null && 源端.匹配目标 != null)
					{
						//陈峰，这里要修改，有问题。
						//Data.Assert(目标端.目标.A端.Equals(Data.ThisGuid));
						//目标端.目标.源记录 = 目标端.目标.B端 = 源端.匹配目标.源记录;
						目标端.替换目标 = 源端.匹配目标;
					}
				}

			}

			return 目标树;

		}

		public 模式树结构 发起端模式树结构
		{
			get
			{
				return 正向推导 ? A端模式树结构 : B端模式树结构;
			}
			set
			{
				if (正向推导)
					A端模式树结构 = value;
				else
					B端模式树结构 = value;
			}
		}

		public 模式树结构 结果端模式树结构
		{
			get
			{
				return 正向推导 ? B端模式树结构 : A端模式树结构;
			}
			set
			{
				if (正向推导)
					B端模式树结构 = value;
				else
					A端模式树结构 = value;
			}
		}

		//根据推导的一端推导出另一端，并执行等价变量的转换等。
		public 模式树结构 推导出推导模式的一端(bool 目标是发起端,bool 只能计算)
		{
			//PatternDataSet.模式结果Row Row = Data.patternDataSet.模式结果.New模式结果Row();
			//Row.ID = Guid.NewGuid();
			//Row.字符串 = "推导结果" + 序号;
			//Row.ObjectID = Guid.Empty;
			//Row.序号 = 1000 + 序号;
			//Row.ParentID = Guid.Empty;
			//Data.patternDataSet.模式结果.Add模式结果Row(Row);

			//比如【借推导还】，这里的目标命题就是【还】。

			//生长对象 目标命题 = null;

			if (目标是发起端)
			{
				模式树结构 r = 结果端模式树结构.尝试进行表达式计算();
				if (r != null)
				{
					发起端模式树结构 = r;
					推导状态 = -1;
					return r;
				}
				if (只能计算 == false)
				{
					推导状态 = 1;
					return 根据附加关联将参数从一棵树传递给另一棵树(结果端模式树结构, 发起端模式树结构, false);
				}

			}
			else
			{
				模式树结构 r = 发起端模式树结构.尝试进行表达式计算();
				if (r != null)
				{
					结果端模式树结构 = r;
					推导状态 = -1;
					return r;
				}
				if (只能计算 == false)
				{
					推导状态 = 1;
					return 根据附加关联将参数从一棵树传递给另一棵树(发起端模式树结构, 结果端模式树结构, true);
				}
			}

			return null;
			//然后需要建立对其他模式的附加关联(对端模式树,parentrow);



			//模式树结构 目标结构 = 模式树结构.复制一个模式树结构(目标端是发起端 ? 发起端模式树结构 : 结果端模式树结构);

			//目标命题 = 根据模式树生成生长对象(0, row1, 0);

			//模式 目标命题row = Data.加入到素材(Data.New派生行(推导结构.推导模式行.B端, 字典_目标限定.空, true));
			//目标命题row.序号 = -1;//不是从当前串中取数据。
			//生长对象 目标命题 = new 生长对象(目标命题row, 2);

			//以下根据原始命题的显式参数按照等价转换的原则来创建目标命题的参数。

			//var 附加等价参数Rows = Data.模式表.对象集合.Where(r => r.C端 == 推导结构.推导模式行.ID);

			//foreach (模式 附加关联/*比如借拥有的借出等价于还拥有的还入*/ in 附加等价参数Rows)
			//{
			//	//
			//	if (Data.等价Guid.Equals(附加关联.连接) == false)
			//		continue;

			//	模式 推导的A端源对象/*比如借*/ = Data.FindRowByID((Guid)附加关联.A端);
			//	List<模式> A端对象引用路径 = 取得引用路径(推导的A端源对象);

			//	参数 等价参数A端 = 遵循路径查找参数对象(推导结构.推导源模式, A端对象引用路径, 1);

			//	if (等价参数A端 == null)
			//		continue;
			//	//找到了。
			//	模式 推导的B端源对象/*比如借*/ = Data.FindRowByID((Guid)附加关联.B端);
			//	List<模式> B端对象引用路径 = 取得引用路径(推导的B端源对象);

			//	生长对象 参数对象 = 等价参数A端.对端派生对象.B端对象;
			//	if (参数对象.是隐藏对象())
			//		参数对象 = 等价参数A端.对端派生对象.A端对象;

			//	目标命题 = 遵循路径增加参数对象(目标命题, B端对象引用路径, 1, 参数对象);

			//下边建立A端和B端参数实际的等价性。
			//}



			//推导结构.推导目标对象 = 目标命题;
		}


		//推导结构是可以反复重用的，所以推导结构一创建，直接就建立了原始模式树，以便重用。
		public 推导结构(模式 推导模式行, bool 正向推导)
		{
			this.源推导模式行 = 推导模式行;
			this.正向推导 = 正向推导;

			List<模式> 推导的所有参数模式 = 推导模式行.端索引表_A端;

			//暂时只支持一个A端，一个B端。
			foreach (模式 目标模式根 in 推导的所有参数模式)
			{
				if (Data.关联拥有A端Guid.Equals(目标模式根.源记录))
					A端模式树结构 = 模式树结构.从一个根模式生成模式树结构(Data.FindRowByID(目标模式根.B端));
				else if (Data.关联拥有B端Guid.Equals(目标模式根.源记录))
					B端模式树结构 = 模式树结构.从一个根模式生成模式树结构(Data.FindRowByID(目标模式根.B端));
			}

			建立基本附加关联集合();

		}

	}

	public class TreeObject
	{
		public Dictionary<Guid, 模式> rows = new Dictionary<Guid, 模式>();//加入的数据对象

		public string 语义根列名;	//语义上的根的列名。
		public 模式 组织Parent根;//组织上的根。
		public 模式 语义根;//语义上的根的行。

		public List<TreeObject> 树集合;

		public void 现有数据创建为新树()
		{
			if (rows.Count == 0)
				return;
			if (树集合 == null)
				树集合 = new List<TreeObject>();
			TreeObject newobj = new TreeObject();
			while (rows.Count > 0)
			{
				var v = rows.First();
				newobj.rows.Add(v.Key, v.Value);
				rows.Remove(v.Key);
			}
			树集合.Add(newobj);
		}

		public void RecalcAllRoot()
		{
			组织Parent根 = null;
			语义根 = null;
			foreach (var obj in rows)
			{
				//查找形式组织的根。只应该有一个。
				Guid pid = (Guid)obj.Value.ParentID;
				if (rows.ContainsKey(pid) == false)//这个指向的parent不在树内部而是在外部，因此，这个记录是组织上的根。
					组织Parent根 = obj.Value;

				//寻找语义上的根。
				int k = (int)obj.Value.That根;
				string str = Data.FindThatName(k);

				pid = (Guid)typeof(模式).GetProperty(str).GetValue(obj.Value);
				if (!rows.ContainsKey(pid))//这个that指向的记录不在树内部而是在外部，因此，这个记录是语义上的根。
				{
					语义根 = obj.Value;
					语义根列名 = str;
				}
			}

			if (树集合 != null)
				foreach (TreeObject obj in 树集合)
					obj.RecalcAllRoot();

		}

		public TreeObject()
		{
		}

		public TreeObject(模式 row)
		{
			AddRow(row);
		}

		public void AddRow(模式 row)
		{
			rows.Add(row.ID, row);
		}
	}


	public class 中心对象版本 : SubString
	{
		public 生长对象 所属树;//以这个树来生成的版本。
		public List<参数> 参数集合;
	}

	public class 完成对象
	{
		public static Dictionary<生长对象, List<中心对象版本>> 对象集合 = new Dictionary<生长对象, List<中心对象版本>>();

		//从一棵树上，找出一个根对应的完成对象。
		public static 中心对象版本 查找一个中心对象版本(生长对象 中心根, 生长对象 树根)
		{
			List<中心对象版本> obj = 对象集合[中心根];
			if (obj == null)
				return null;

			foreach (中心对象版本 对象 in obj)
				if (对象.所属树 == 树根)
					return 对象;
			return null;
		}

		//以自身为树的根，自己的主对象为中心来找出完成对象。
		public static 中心对象版本 查找一个自身为根的中心对象版本(生长对象 对象)
		{
			return 查找一个中心对象版本(对象.中心第一根类, 对象);
		}
	}

	public class 关联对象对
	{
		public 生长对象 中心对象;
		public 生长对象 参数对象;
		public 生长对象 对象对;
		public int 处理类型;
		public bool 已处理;
		public int 生长次序 = 0;
		public 封闭范围 所属范围;
		public bool 成功生长;
		public int 实际生长顺序;
		public bool 被推后生长;
		public 关联对象对(生长对象 中心对象, 生长对象 参数对象, 生长对象 对象对)
		{
			this.中心对象 = 中心对象;
			this.参数对象 = 参数对象;
			this.对象对 = 对象对;
			处理类型 = 0;
		}

		public string 左对象描述 { get { return 中心对象.begindex < 参数对象.begindex ? 中心对象.取子串 : 参数对象.取子串; } }
		public string 右对象描述 { get { return 中心对象.begindex < 参数对象.begindex ? 参数对象.取子串 : 中心对象.取子串; } }
		public string 中心对象描述 { get { return 中心对象.begindex < 参数对象.begindex ? "1" : "0"; } }
		public string 生长次序描述 { get { return 生长次序.ToString(); } }
		public string 是否已生长描述 { get { return 已处理 ? "是" : "否"; } }
		public string 是否成功生长描述 { get { return 成功生长 ? "是" : "否"; } }
		public int 实际生长顺序描述 { get { return 实际生长顺序; } }
		public string 生长类型描述
		{
			get
			{
				if (处理类型 == Processor.生长_集合处理)
					return "集合";
				else if (处理类型 == Processor.生长_名词谓语处理)
					return "名词谓语";
				else if (处理类型 == Processor.生长_正常处理)
					return "正常";
				else if (处理类型 == Processor.生长_全部)
					return "全部";
				else if (处理类型 == Processor.生长_连动处理)
					return "连动";
				else if (处理类型 == Processor.生长_两组动词并列处理)
					return "并列动词";
				else if (处理类型 == 0)
					return "全部";
				return 处理类型.ToString();
			}
		}
	}


	public class 对象对
	{
		public 生长对象 原对象;
		public 生长对象 派生对象;
		public 对象对(生长对象 原对象, 生长对象 派生对象)
		{
			Data.Assert(原对象 != null && 派生对象 != null);
			this.原对象 = 原对象;
			this.派生对象 = 派生对象;
		}
		public static 对象对 查找原对象(List<对象对> 对象对集合, 生长对象 原对象)
		{
			foreach (对象对 objs in 对象对集合)
				if (原对象 == objs.原对象)
					return objs;

			return null;
		}
	}
	public class 语言参数
	{
		public 生长对象 原始语义对象;//指向本来的语义对象，肯定是关联对象，比如【生存拥有时间】，【借拥有借出方】等。
		public 生长对象 调整后的语言方面的信息;//调整到新的parent后的语言角色、关联形式介词等方面的信息。比如【借拥有时间】【借拥有人】等。
		public 语言参数(生长对象 原始语义对象, 生长对象 调整后的语言方面的信息)
		{
			this.原始语义对象 = 原始语义对象;
			this.调整后的语言方面的信息 = 调整后的语言方面的信息;
		}
	}
	public class 待创建对象结构
	{
		public 模式 知识记录;
		public 生长对象 发起参数;//第一个线索参数，一般都是字符串对象，即依据【拥有形式】发起。
		public 生长对象 已创建对象;

		public 待创建对象结构(模式 源对象, 生长对象 发起参数)
		{
			this.知识记录 = 源对象;
			this.发起参数 = 发起参数;
		}

	}

	public class 聚合体对象及关联模式
	{
		public List<模式> 聚合体;
		public List<模式> 所有关联;

		public 聚合体对象及关联模式()
		{
			this.聚合体 = new List<模式>();
			this.所有关联 = new List<模式>();
		}

		public 模式 得到聚合体的剩余体(List<模式> 已有聚合体)
		{
			模式 结果 = null;
			foreach (模式 current in 聚合体)
			{
				foreach (模式 模式 in 已有聚合体)
				{
					if (current == 模式)
						goto next;
				}
				if (结果 == null || current.显隐 == 字典_显隐.隐藏)
				结果 = current;
			next:
				;
			}
			return 结果;
		}


		public 模式 得到聚合体的主体()
		{
			if (this.聚合体.Count() == 0)
				return null;

			模式 模式 = this.聚合体[0];

			模式 本模式;

			do
			{
				本模式 = 模式;

				foreach (模式 row in this.所有关联)
				{
					if (row.B端.Equals(本模式.ID) && 替代.是属于或聚合(Data.一级关联类型(row)))
					{
						模式 = Data.FindRowByID(row.A端);
						break;
					}
				}
			}
			while (模式 != 本模式);
			return 本模式;
		}
	}

	public class 参数
	{
		public 生长对象 所属根;

		//根关联对象的“源记录”记录的是原始的关联。
		public 模式 源关联记录;

		//A端和B端就是真实的派生对象。
		public 生长对象 A端对象;
		public 生长对象 B端对象;
		public int 语言角色;
		//对象本身是关联的哪一端。
		public int that = 字典_目标限定.A端;
		//public DataRow 源关联记录
		//{
		//	get
		//	{
		//		return 根关联对象.源模式行;
		//	}
		//}
		public 生长对象 对端派生对象
		{
			get
			{
				return that == 字典_目标限定.A端 ? B端对象 : A端对象;
			}
			set
			{
				if (that == 字典_目标限定.A端)
					B端对象 = value;
				else
					A端对象 = value;
			}
		}

		public bool 已经派生()//这个关联参数已经被满足了。
		{
			return 对端派生对象 != null;
		}

		public 参数(生长对象 所属对象, 模式 源关联行, int 语言角色, int that端)
		{
			所属根 = 所属对象;
			源关联记录 = 源关联行;
			that = that端;
			this.语言角色 = 语言角色;
		}

		public 参数(生长对象 所属对象, 生长对象 实际对象, int 语言角色, bool 关联用本身)
		{
			//创建新知识对象的参数
			所属根 = 所属对象;
			that = 字典_目标限定.A端;
			源关联记录 = 关联用本身 == true ? 实际对象.模式行 : 实际对象.源模式行;
			对端派生对象 = 实际对象;
			this.语言角色 = 语言角色;
		}

		public 参数(参数 对象)
		{
			所属根 = 对象.所属根;
			源关联记录 = 对象.源关联记录;
			A端对象 = 对象.A端对象;
			B端对象 = 对象.B端对象;
			that = 对象.that;
			语言角色 = 对象.语言角色;
		}
		public override string ToString()
		{
			string str = "根对象:" + (所属根 == null ? "" : 所属根.显式串) + " 原记录:" + 源关联记录.形式;
			if (对端派生对象 != null)
			{
				if (对端派生对象.B端对象 != null)
					str += "对象:" + 对端派生对象.B端对象.中心第一根类.ToString();
				else
					str += "对象:" + 对端派生对象.中心第一根类.ToString();

			}
			return str;
		}
	}

	//public class 根关联对象
	//{
	//    public 生长对象 A端;
	//    public 生长对象 B端;
	//    public Guid 源关联;
	//    public int that = 字典_目标限定.A端;
	//    //public Guid 唯一性标识;
	//    public 根关联对象(生长对象 A端obj, 生长对象 B端obj, Guid 源关联obj, int that端)
	//    {
	//        A端 = A端obj;
	//        B端 = B端obj;
	//        源关联 = 源关联obj;
	//        that = that端;
	//        //唯一性标识 = Guid.NewGuid();
	//    }
	//}

	public class 封闭范围 : SubString
	{
		public List<封闭范围> 子范围;
		public 封闭范围 父范围;
		public 生长对象 左括号对象;
		public 生长对象 右边界对象;
		public int 封闭值;
		//public int 处理阶段;
		public bool 已经完成所有可能生长 = false;
		public int 打分起始分;
		public bool 是括号型区间
		{
			get
			{
				return 左括号对象 != null;
			}
		}

		public 封闭范围(int begindex, int endindex, int 封闭值)
			: base(begindex, endindex)
		{
			//this.封闭值 = 封闭值;
			this.封闭值 = 封闭值;
		}

		public 封闭范围(生长对象 左边对象/*, int 封闭值*/)
			: base(左边对象.begindex, -1)
		{
			左括号对象 = 左边对象;
			封闭值 = 10;//括号型的封闭型很高，绝不允许跨越。
			//不过，对于【空格】这样的也可以设定封闭范围，但是封闭值可能低一些。
		}

		public static int 计算间隔型的封闭值(模式 间隔)
		{
			//分段的强制性比句号更高？

			if (Data.是派生类(Data.句子语用基类Guid, 间隔, 替代.正向替代))//句子
			{
				if (Data.下文引导冒号Guid.Equals(间隔.ID))//冒号比句号的要弱。
					return 5;
				return 9;
			}

			if (Data.是派生类(Data.逗号停顿Guid, 间隔, 替代.正向替代))//逗号
				return 2;


			return 1;
		}

		public void 加入子范围(封闭范围 obj)
		{
			if (子范围 == null)
				子范围 = new List<封闭范围>();
			子范围.Add(obj);
		}

		//public 封闭范围(int 左边范围)
		//    : base(左边范围, -1)
		//{
		//}
		public int 内在begindex//排除掉左边的封闭符号以后的内在
		{
			get
			{
				if (左括号对象 != null)
					return 左括号对象.endindex;
				return begindex;
			}
		}
		public int 内在endindex//排除掉右边的封闭符号以后的内在
		{
			get
			{
				if (右边界对象 != null)
					return 右边界对象.begindex;
				return endindex;
			}
		}


		public int 递归计算串跨越范围的冲突值(int 串beg, int 串end, int 封闭性打分)
		{
			if (是括号型区间)//括号型，两边都不能穿越
			{
				if (封闭性打分 < 封闭值 && (串beg < begindex && 串end > begindex && 串end < endindex || 串end > endindex && 串beg < endindex && 串beg > begindex))
					封闭性打分 = 封闭值;
			}
			else//间隔型，只考虑右边边界
			{
				if (封闭性打分 < 封闭值 && 串end > endindex && 串beg < endindex && 串beg > begindex)
					封闭性打分 = 封闭值;
			}
			if (子范围 != null)
				foreach (封闭范围 obj in 子范围)
					封闭性打分 = obj.递归计算串跨越范围的冲突值(串beg, 串end, 封闭性打分);
			return 封闭性打分;
		}

		public bool 递归判断一个对象被包含在封闭范围内(int 外部范围beg, int 外部范围end, int 对象beg, int 对象end)
		{
			if (对象beg <= endindex || 对象end <= begindex)
			{
				//要求目标对象处于封闭范围中间，而封闭范围又包含在外部范围中间。
				if (对象beg >= begindex && 对象end <= endindex && begindex >= 外部范围beg && endindex <= 外部范围end)
					return true;
				if (子范围 != null)
					foreach (封闭范围 obj in 子范围)
						if (obj.递归判断一个对象被包含在封闭范围内(外部范围beg, 外部范围end, 对象beg, 对象end))
							return true;
			}
			return false;
		}

		public bool 递归判断占满了封闭范围(int 对象beg, int 对象end, bool 要求是括号型区间)
		{
			if (对象beg <= endindex || 对象end <= begindex)
			{
				if ((是括号型区间 || 要求是括号型区间 == false) && (对象beg == 内在begindex || 对象end == 内在endindex))
					return true;
				if (子范围 != null)
					foreach (封闭范围 obj in 子范围)
						if (obj.递归判断占满了封闭范围(对象beg, 对象end, 要求是括号型区间))
							return true;
			}
			return false;
		}
		public 封闭范围 递归返回左边紧挨的封闭范围(int 位置)
		{
			if (内在begindex == 位置)
				return this;
			if (子范围 != null)
				foreach (封闭范围 obj in 子范围)
				{
					封闭范围 r = obj.递归返回左边紧挨的封闭范围(位置);
					if (r != null)
						return r;
				}
			return null;
		}
		public 封闭范围 递归返回右边紧挨的封闭范围(int 位置)
		{
			if (内在endindex == 位置)
				return this;
			if (子范围 != null)
				foreach (封闭范围 obj in 子范围)
				{
					封闭范围 r = obj.递归返回右边紧挨的封闭范围(位置);
					if (r != null)
						return r;
				}
			return null;
		}
		public bool 递归计算是封闭范围的边界(生长对象 对象, bool 计算左边)
		{
			if (左括号对象 == 对象 && 计算左边)
				return true;
			if (右边界对象 == 对象 && 计算左边 == false)
				return true;
			if (子范围 != null)
				foreach (封闭范围 obj in 子范围)
				{
					if (obj.递归计算是封闭范围的边界(对象, 计算左边))
						return true;
				}
			return false;
		}

		public void 删除没有完成的括号范围()
		{
			if (子范围 == null)
				return;
			int i = 0;
			for (i = 0; i < 子范围.Count; i++)
				if (子范围[i].endindex == -1)
				{
					子范围.RemoveAt(i);
					i--;
				}
				else
					子范围[i].删除没有完成的括号范围();
		}
		public 封闭范围 递归返回指定位置所属子范围(int 位置)
		{
			if (子范围 != null)
				foreach (封闭范围 obj in 子范围)
				{
					封闭范围 r = obj.递归返回指定位置所属子范围(位置);
					if (r != null)
						return r;
				}
			if (位置 >= begindex && 位置 <= endindex)
				return this;
			return null;
		}
	}

	// 把复杂的有语义的和原始字符串匹配的都给合并在一起了，不再设计很多的派生类，就用这一个类来进行处理，有字段浪费，但不要紧。
	// 语义和字符串的对照中假设一个比较重要的原则是假设语义模式对照的字符串应该是连续的。中间不能有空洞。
	public class 生长对象 : SubString
	{
		//public List<生长对象> 被上级模式嵌套引用列表;本模式被上级模式嵌套引用(也就是被聚集或者说嵌套)的列表记录，如果为0就表示是自由模式。
		//public int 语言算子掩码;
		//public object[] 模式参数赋值对象;//对这个模式参数的赋值，这个没有用范式化，是因为在分析模式里边，这个类型要带上质量方面的信息。

		public Guid ID = Guid.NewGuid();
		//public int 覆盖型对象位标识;
		public bool 是无形式空对象 = false;
		public bool 是介词形式创建的对象;
		public bool 是省略对象 = false;
		public bool 是压缩形式对象;//应该在模式里边加一个属性，然后生长对象对等起来。
		public bool 是定语形式对象;//应该在模式里边加一个属性，然后生长对象对等起来。
		public PatternDataSet.模式结果Row 结果Row;//只是为了进行显示的行，实际计算不需要。
		//public Guid 唯一性标识 = Guid.Empty;
		public int 处理轮数;//是第几层嵌套的，语言级最底层为1层。
		public int 有效对象序数;
		public int 关联总数 = 1;
		public int 生长次数 = 0;
		public bool 介动词等情况延后一阶段生长;
		public int 序号;
		public int 处理阶段 = 0;
		public bool 已进行所有可能的生长 = false;
		public int 生长次序打分 = 9;//打分越高越应该先生长
		public 匹配语料对象 对应匹配语料对象;
		public 模式 匹配的形式行;
		//public 生长对象 空对象挂接根;

		#region 以下是[关联]本身的信息
		//每一次都是合并两个模式形成本模式，其合并的基本依据就是模式行。
		//理论上，就是把两个节点对象代表的链条串联成一个更大的串，因为串是一种组合，而没有排列次序，所以，谁先谁后并不重要。
		public 模式 模式行;
		public 模式 源模式行;
		public 模式 模式匹配对应的匹配模式行;
		public List<参数> 关联参数集合;//这个节点本身关联的参数集合。主要就是介词等。
		//public List<语言参数> 语言参数表;//记录另一种形式的语言参数，看来现在不需要了。
		public bool 生成时派生模式行 = false;
		public bool 中心在右;//标记哪个是中心对象
		public int that = 字典_目标限定.A端;
		public int 语言角色 = 字典_语言角色.中心;
		public int 传递语言角色;

		public byte 起始_附加串;
		public 生长对象 前置介词;
		public byte 前置介词_附加串;
		public 生长对象 前置介词逗号;
		public byte 前置介词逗号_附加串;
		public 生长对象 介动词后生存;
		public byte 介动词后生存_附加串;
		public 生长对象 介动词后生存逗号;
		public byte 介动词后生存逗号_附加串;

		//这是对象本身所在的位置。

		public 生长对象 后置介词;
		public byte 后置介词_附加串;
		public 生长对象 后置介词逗号;
		public byte 后置介词逗号_附加串;
		public 生长对象 中间的和地;//这个应该在后置介词后。//暂时不允许出现后置的定语和状语，所以，的和地只会出现在左边的参数和右边的中心词之间。
		public byte 中间的和地_附加串;
		public 生长对象 中间的和地逗号;
		public byte 结束_附加串;

		//如果是真的右边没有东西的空的【的】，那么就会是产生一个空的对象来进行处理。
		//也就是暂时不允许英语里边的：【他跑飞快地】的这种。
		public 模式 前置介词关联;
		public 模式 介动词后生存关联;
		public 模式 后置介词关联;
		public 模式 中间的和地关联;
		#endregion

		#region 以下是两端对象的信息
		public 生长对象 左对象;
		public 生长对象 中间未解决串对象;//间隔在中间的串，等待后边再解决生长。
		public 生长对象 右对象;
		public 生长对象 A端实际对象;//比如【借属拥完成拥有时间】，【借】是中心对象，而【完成】是A端实际对象。
		public 生长对象 B端实际对象;//比如【借拥有借出者他】，【他】是参数对象，而【借出者】是B端实际对象。
		//这个是1级的【this属于】的记录，也就是到此为止的根对象，一级级回溯本来是可以的，但是效率太低。所以放这个字段做快速查找。
		//第一根类其实也是一个参数，和别的参数有共同的地方，也可以考虑直接合并在参数表中，而去除这个特殊定义，则更灵活。
		public 生长对象 中心第一根类;
		public 生长对象 参数第一根类;
		//一个关联关联了A端和B端两个对象，而这两个对象都受这个关联的影响，从而使其参数变化，他们各自都把关联和对端看着自己的参数。
		//这两个参数表理论上是冗余的，可以通过基类树计算。但基类树只单向挂接在A端上，避免计算冗余。而这里以对象为中心的参数表是冗余计算，以对象自己为中心来看待。对象分别记录自己的参数的双向和反向。
		public List<参数> 概念对象参数表;//每个基对象的双向参数表，参数表不考虑句子的that中心，比如【红色的苹果】和【苹果的红色】中，都是描述【苹果】的概念，因为【苹果拥有红色】
		public 生长对象 Parent;
		public 参数树结构 中心对象基类和拥有树;
		public 参数树结构 中心对象基类树;
		//也许要考虑为参数对象也增加基类树。
		#endregion

		public List<模式> 附加关联;//记录了所有的附加关联。

		//记录这个对象的基对象。
		public 生长对象 基对象;
		public 模式 集合对象的基对象;

		public 参数字段 参数 = new 参数字段();
		public int 长度分 = 0;
		public int 类型分 = 0;
		public int 完成分 = 9;
		public bool 取消抑制性 = false;
		public bool 释放标志 = false;
		//记录这个对象被其它对象拆解的次数
		public int 左拆解 = 0;
		public int 右拆解 = 0;

		public int 概率打分
		{
			get
			{
				return 参数.概率分/* + 完成分*/;
				//return 参数.概率分 ;
				//int v = /*长度 * 100 +*/ 参数集合.概率分 + 生长次数;
				//if (中心第一根类 == null || 是介词或者串(true, true, true))
				//	v -= 4;
				//return v;
			}
		}
		public int 概率分
		{
			get
			{
				return 参数.概率分;
			}
		}
		public int 创建者;//是谁把这个模式实例创建出来的，根据这个值，可以删除这个拥有者创建的模式。

		public int 正确性;//这个表示当前模式正确性的指标，比如已经明确了肯定能生存下来。

		public int 同一局面计数 = 0;

		public 生长对象遍历节点 匹配遍历对象;
		public int 关联个数;

		public bool 是疑问变量;

		//public List<参数> 中心派生集合;//用来进行回溯。
		//public int 当前中心派生对象Index;
		//public List<参数> 参数派生集合;
		//public int 当前参数派生对象Index;
		//public 生长对象 遍历终点对象;


		//public static 生长对象 构造This对象()
		//{
		//	PatternDataSet.模式编辑Row row = (PatternDataSet.模式编辑Row)Data.New派生行(Data.ThisGuid, Data.patternDataSet.模式编辑);
		//	return new 生长对象(row, 0);
		//}


		public int 总分
		{
			get
			{
				return 长度分 * 2 + 类型分;

			}
		}

		public void 清空所有介词和关联信息()
		{
			前置介词 = null;
			介动词后生存 = null;
			后置介词 = null;
			中间的和地 = null;
			前置介词逗号 = null;
			介动词后生存逗号 = null;
			后置介词逗号 = null;
			中间的和地逗号 = null;
			前置介词关联 = null;
			介动词后生存关联 = null;
			后置介词关联 = null;
			中间的和地关联 = null;
			起始_附加串 = 0;
			前置介词_附加串 = 0;
			前置介词逗号_附加串 = 0;
			介动词后生存_附加串 = 0;
			介动词后生存逗号_附加串 = 0;
			后置介词_附加串 = 0;
			后置介词逗号_附加串 = 0;
			中间的和地_附加串 = 0;
			结束_附加串 = 0;
		}


		public List<参数> 得到指定根对象的参数表(生长对象 根对象 = null, bool 反转 = false)
		{
			if (根对象 == null)
				根对象 = 中心第一根类;
			List<参数> r = new List<参数>();
			if (概念对象参数表 == null)
				return r;
			foreach (参数 obj in 概念对象参数表)
				if (根对象 == obj.所属根)
					r.Add(obj);
			if (反转)
				r.Reverse();
			return r;
		}

		public 参数树结构 利用缓存得到基类和关联记录树()
		{
			//if (包含拥有)
			//{
			if (中心对象基类和拥有树 == null)
				中心对象基类和拥有树 = 生成基类和关联记录树();
			return 中心对象基类和拥有树;
			//}

			//if (中心对象基类树 == null)
			//    中心对象基类树 = 生成基类和关联记录树(包含拥有);
			//return 中心对象基类树;
		}

		private 参数树结构 生成基类和关联记录树()
		{
			//return Data.利用缓存得到基类和关联记录树(中心第一根类.源模式行, 包含拥有, false);
			//这里反复了几次，就是在一次解析中计算新的对象具有的参数是就完全依赖知识库里边的记录，还是要把新的信息加入的问题。
			//其中关键在于聚合，一旦聚合，多个对象就看成是了一个对象，参数也就可以合并。现在看来，就如下，还是用知识库里边的足够，理由如下：
			//1、假设【A聚合B】已经有效，而且A作为了中心对象也就是根对象。
			//2、当遇到了【A】自己的参数的时候，显然没有问题，【A】可以吸收这个参数。
			//3、当遇到了【B】的参数比如C的时候，现在使用了三级关联而且是双向做了两次计算，所以即使聚合是反向的也能计算出【A->B->C】的三级关联路径，也是没有问题的！
			//4、而生长的时候，处理中间对象【B】重复创建的问题在别处本身要解决，所以，这里没有任何问题了！
			//因此，现在就用这样的方式！

			if (是介词或者串(true, true, true))
				return new 参数树结构(模式行, 字典_目标限定.A端, false/*, 0*/);
			return Data.利用缓存得到基类和关联记录树(中心第一根类.源模式行, false);


			//如果用以下的代码。那么就把新的关联加入。尤其是【聚合】
			if (Data.是介词或者串(模式行, true, true, true))
				return new 参数树结构(模式行, 字典_目标限定.A端, false/*, 0*/);
			if (中心第一根类 == this)
				return Data.利用缓存得到基类和关联记录树(模式行, false);

			//根据已经生长的关联，实时扩展基类和拥有树。
			//下边的代码会造成新增的关联又作为依据，这个要处理。一次运算的时候，不会重复作为依据。
			参数树结构 上一个结构 = A端对象.生成基类和关联记录树();
			参数树结构 结构 = 上一个结构.复制一个结构和一级子节点();
			//if (替代.是分类或聚合(Data.一级关联类型(模式行)))
			//{
			//Data.Assert(Data.等价Guid.Equals(Data.一级关联类型(模式行)));//其实都已经升级为了等价。
			//    结构.Add子对象(new 参数树结构(模式行, 字典_目标限定.A端, false, 0));
			//}
			//else if (包含拥有)
			结构.Add子对象(new 参数树结构(模式行, 字典_目标限定.A端, false/*, 0*/));
			return 结构;

		}

		public bool 扩展计算对象能序列化()
		{
			if (Data.能够序列化(中心第一根类.源模式行))
				return true;
			if (查找已结合的推理角色(true) != null)
				return true;

			return false;
		}

		public void 设置模式行(模式 关联row)
		{
			模式行 = 关联row;
			if (模式行 != null)
				设置源模式行(Data.FindRowByID((Guid)模式行.源记录));
		}

		public void 设置源模式行(模式 row)
		{
			if (源模式行 == null && row != null)
				参数.概率分 = Data.合并概率打分(参数.概率分, row.参数.概率分);//new 参数字段(row.参数集合).概率分);
			源模式行 = row;

		}


		public Guid 源模式行ID
		{
			get
			{
				return (Guid)源模式行.ID;
			}
		}

		public Guid 模式行ID
		{
			get
			{
				return (Guid)模式行.ID;
			}
		}


		public string 生成所有参数的串()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			string s = "";
			foreach (参数 o in 概念参数表)
			{
				s = s + "【" + o.ToString() + "】";
			}
			return s;
		}



		public 生长对象 克隆简单信息()
		{
			生长对象 o = new 生长对象();
			o.that = that;
			o.设置模式行(模式行);
			o.设置源模式行(o.源模式行);
			o.左对象 = 左对象;
			o.右对象 = 右对象;
			o.中心在右 = 中心在右;
			o.语言角色 = 语言角色;
			o.传递语言角色 = 传递语言角色;
			o.前置介词 = 前置介词;
			o.介动词后生存 = 介动词后生存;
			o.后置介词 = 后置介词;
			o.中间的和地 = 中间的和地;
			o.前置介词关联 = 前置介词关联;
			o.介动词后生存关联 = 介动词后生存关联;
			o.后置介词关联 = 后置介词关联;
			o.中间的和地关联 = 中间的和地关联;
			o.前置介词逗号 = 前置介词逗号;
			o.介动词后生存逗号 = 介动词后生存逗号;
			o.后置介词逗号 = 后置介词逗号;
			o.中间的和地逗号 = 中间的和地逗号;

			o.起始_附加串 = 起始_附加串;
			o.前置介词_附加串 = 前置介词_附加串;
			o.前置介词逗号_附加串 = 前置介词逗号_附加串;
			o.介动词后生存_附加串 = 介动词后生存_附加串;
			o.介动词后生存逗号_附加串 = 介动词后生存逗号_附加串;
			o.后置介词_附加串 = 后置介词_附加串;
			o.后置介词逗号_附加串 = 后置介词逗号_附加串;
			o.中间的和地_附加串 = 中间的和地_附加串;
			o.结束_附加串 = 结束_附加串;


			o.参数 = new 参数字段(参数.ToString());
			//o.处理阶段 = 处理阶段;
			//o.当前根 = 当前根;
			o.处理轮数 = 处理轮数;
			o.是介词形式创建的对象 = 是介词形式创建的对象;
			o.是压缩形式对象 = 是压缩形式对象;
			o.是定语形式对象 = 是定语形式对象;
			o.介动词等情况延后一阶段生长 = 介动词等情况延后一阶段生长;
			o.完成分 = 完成分;

			o.集合对象的基对象 = 集合对象的基对象;
			return o;
		}


		public bool 质量好于(生长对象 o)
		{
			return 正确性 > o.正确性;
		}

		public 生长对象 A端对象
		{
			get
			{
				return that == 字典_目标限定.A端 ? 中心对象 : 参数对象;
			}
			set
			{
				if (that == 字典_目标限定.A端)
					中心对象 = value;
				else
					参数对象 = value;
			}
		}

		public void 递归调整序号()
		{
			//Guid 类型 = Data.一级关联类型(模式行);
			if (中心第一根类 == this)//原始概念。其实用字符串的最好，但字符串现在没有加入。
			{
				生长对象 p = Parent;
				while (p != null)
				{
					if (p.序号 > 序号)
						p.序号 = 序号;
					p = p.Parent;
				}
			}
			if (中心对象 != null)
				中心对象.递归调整序号();
			if (参数对象 != null)
				参数对象.递归调整序号();
		}

		public void 清空序号和Parent()
		{
			//Guid 类型 = Data.一级关联类型(模式行);
			//if (Data.拥有形式Guid.Equals(类型) == false)
			Parent = null;
			//语言角色 = 字典_语言角色.无;
			if (中心第一根类 == this)//非原始概念。其实用字符串的最好，但字符串现在没有加入。
				序号 = begindex;
			else
				序号 = 100000;
			if (中心对象 != null)
				中心对象.清空序号和Parent();
			if (参数对象 != null)
				参数对象.清空序号和Parent();
		}

		public 生长对象 B端对象
		{
			get
			{
				return that == 字典_目标限定.A端 ? 参数对象 : 中心对象;
			}
			set
			{
				if (that == 字典_目标限定.A端)
					参数对象 = value;
				else
					中心对象 = value;
			}
		}


		public string 源语言串
		{
			get { return Data.当前句子串; }
		}

		public string 根Name
		{
			get
			{
				if (中心第一根类 == null)
					return "[null]";
				if (是NullThis空对象())
					return "[nullthis]";
				return 中心第一根类.显式串;
			}
		}

		public string 显式串
		{
			get
			{
				if (源模式行 == null)
					return 取子串;
				return 取子串 + "@" + 源模式行.形式;
			}
		}

		public string 取子串
		{
			get
			{
				if (begindex < 0)
					return 模式行.形式;
				return Data.当前句子串.Substring(begindex, endindex - begindex);
			}
		}

		public bool 是的或者地(bool 包含的, bool 包含地)
		{
			string s = 取子串;
			if (包含的 && 取子串 == "的")
				return true;
			if (包含地 && 取子串 == "地")
				return true;
			return false;
		}

		public 生长对象 中心对象
		{
			get
			{
				if (中心在右)
					return 右对象;
				else
					return 左对象;
			}
			set
			{
				if (中心在右)
					右对象 = value;
				else
					左对象 = value;
			}
		}

		public 生长对象 参数对象
		{
			get
			{
				if (中心在右)
					return 左对象;
				else
					return 右对象;
			}
			set
			{
				if (中心在右)
					左对象 = value;
				else
					右对象 = value;
			}
		}

		public int 参数左边界()
		{
			if (前置介词 != null)
				return 前置介词.begindex;
			return 参数对象.begindex;

		}

		public int 内部对象计算左边界()
		{
			int v = 左对象.begindex;
			if (前置介词 != null && 前置介词.begindex < v)
				v = 前置介词.begindex;
			if (右对象.begindex < v)
				v = 右对象.begindex;
			return v;
			//if (中心在右 && 前置介词 != null)
			//    return 前置介词.begindex;
			//return 左对象.begindex;
		}
		public int 内部对象计算右边界()
		{
			int v = 右对象.endindex;
			if (中间的和地逗号 != null && 中间的和地逗号.endindex > v)
				v = 中间的和地逗号.endindex;
			if (中间的和地 != null && 中间的和地.endindex > v)
				v = 中间的和地.endindex;
			if (后置介词逗号 != null && 后置介词逗号.endindex > v)
				v = 中间的和地逗号.endindex;
			if (后置介词 != null && 后置介词.endindex > v)
				v = 后置介词.endindex;
			if (左对象 != null && 左对象.endindex > v)
				v = 左对象.endindex;
			return v;
			//if (中心在右)
			//    return 右对象.endindex;
			//if (的后逗号 != null)
			//    return 的后逗号.endindex;
			//if (中间的和地 != null)
			//    return 中间的和地.endindex;
			//if (后介逗号 != null)
			//    return 后介逗号.endindex;
			//if (后置介词 != null)
			//    return 后置介词.endindex;
			//return 右对象.endindex;
		}

		public bool 集合已有第一元素()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
				if (Data.是派生关联(Data.抽象形式集合拥有第一元素Guid, obj.源关联记录) > 0)
					return true;
			return false;
		}

		public bool 集合已经封闭()//还没有完成的概念参数
		{
			bool 第一元素 = false;
			bool 第二元素 = false;
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
			{
				if (Data.是派生关联(Data.抽象形式集合拥有第一元素Guid, obj.源关联记录) > 0)
					第一元素 = true;
				if (Data.是派生关联(Data.抽象形式集合拥有后续元素Guid, obj.源关联记录) > 0)
					第二元素 = true;
			}
			return 第一元素 && 第二元素;
		}

		public 生长对象 查找第一个已经满足的集合元素()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
				if (Data.是派生关联(Data.抽象形式集合拥有元素Guid, obj.源关联记录) > 0)
					return obj.对端派生对象;
			return null;
		}



		public 生长对象 找到第一个原生对象分支()
		{
			if (中心对象 == null || 参数对象 == null)
				return null;
			生长对象 obj = this;
			while (obj.中心对象 != null && obj.参数对象 != null)
			{
				if (obj.中心对象.长度 > 0)
				{
					if (obj.参数对象.长度 > 0)
						return obj;
					obj = obj.中心对象;
				}
				else if (obj.参数对象.长度 > 0)
					obj = obj.参数对象;
				else
					return null;
			}
			return null;
		}

		//原生对象，指不是隐藏的语义角色，也没有和隐藏的语义角色结合的对象
		//有的隐藏对象，根据【省略创建】的，也属于原生对象。
		public 生长对象 找到第一个原生对象()
		{
			生长对象 obj = this;
			while (obj != null)
			{
				if (obj.处理轮数 == -2)
					obj = obj.中心对象;
				return obj;
			}
			Data.Assert(false);
			return null;
		}

		public bool 已经拥有了非集合成员的关联()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
				if (Data.是拥有形式(obj.源关联记录) == false && Data.是派生关联(Data.抽象形式集合拥有元素Guid, obj.源关联记录) == 0)
					return true;
			return false;
		}

		public 参数 查找第一个已经满足的参数()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
				if (obj.对端派生对象 != null)
					return obj;
			return null;
		}

		public bool 集合中的所有元素都兼容这个概念(模式 目标)
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
			{
				生长对象 集合参数 = obj.对端派生对象;
				if (集合参数 == null)
					continue;
				if (Data.是拥有形式(集合参数.源模式行))
					continue;
				if (Data.是派生类((Guid)目标.ID, 集合参数.参数对象.源模式行, 替代.正向替代 | 替代.聚合替代) == false)
					return false;
				//参数树结构 tree = obj.对端派生对象.参数对象.利用缓存得到基类和关联记录树(false);
				//参数树结构 r = tree.递归从纯的基类树中查找广义匹配的基类((Guid)目标["ID"], 替代.正向替代 | 替代.正向聚合替代);
				//if (r == null)
				//	return false;
			}
			return true;
		}

		//关键是要区分是否显式的！！！！如果没有出现，那么就可以抽象扮演！！！！如果已经显式出现了，显然就需要用显式出现的明确的这个的派生了！！！
		//这里就是要找必须显式出现的关联证据，具体说，就是对【推理角色】的【如果】【那么】，现在要求显式的。
		public bool 证明满足关联的显式参数证据(Guid 目标ID)
		{
			if (Data.ThisGuid.Equals(目标ID))
				return true;

			//目前只对推理角色这样要求，也就是推理角色要求显式的【如果】【那么】等，其他的都不要求。
			模式 目标Row = Data.FindRowByID((Guid)Data.FindRowByID(目标ID).源记录);
			if (Data.是派生类(Data.推理角色Guid, 目标Row, 替代.正向替代) == false)
				return true;

			//集合类暂时不处理
			//if (Data.是派生类(Data.并列集合Guid, this.当前根.模式行, 替代.本质正向替代))
			//	return true;

			if (Data.是派生类(目标ID, 中心第一根类.模式行, 替代.正向替代))
				return true;

			//用当前实际多继承的参数来判断。
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
			{
				if (obj.已经派生() == false)
					continue;
				if (替代.可正向替代(Data.一级关联类型(obj.源关联记录)) == false)
					continue;
				return obj.对端派生对象.参数对象.证明满足关联的显式参数证据(目标ID);

			}
			return false;
		}


		//复制一个对象并把其中的基概念替换为派生概念，并派生并替换该概念向两端分别连接的关联。
		//替换产生的所有对象设置为和原来对象先相同层级，因此，返回后，所有的对象都是旧的层级，所以，不会参与下轮的运算。
		//最终的返回对象如果要参与下轮运算，要在返回后让对象的层级等于当前轮数。
		public 生长对象 替换基概念为派生概念重建对象(List<对象对> 替换对象集合)
		{
			//递归终止点：对于最小的叶子对象，如果等于基对象，那么返回派生对象，如果不等于，就返回自身。
			//if (中心第一根类 == this)
			//{
			对象对 obj = 对象对.查找原对象(替换对象集合, this);//中心第一根类);
			//return obj == null ? this : obj.派生对象;
			if (obj != null)
				return obj.派生对象;
			else if (中心第一根类 == this)
				return this;
			//}

			//1、分别对中心对象和参数对象进行递归替换。
			生长对象 替换后的中心对象 = 中心对象.替换基概念为派生概念重建对象(替换对象集合);
			if (替换后的中心对象 == null)
				return null;
			生长对象 替换后的参数对象 = 参数对象.替换基概念为派生概念重建对象(替换对象集合);
			if (替换后的参数对象 == null)
				return null;
			//没有发生改变，就返回自身。
			if (替换后的中心对象 == 中心对象 && 替换后的参数对象 == 参数对象)
				return this;

			//2、中心对象和参数对象替换了，现在再重新进行一次生长，完成对象自己的创建。
			//暂时这样，后边要替换为要派生的关联。
			模式 派生关联 = 源模式行;
			生长对象 生长对象 = 克隆简单信息();
			生长对象.模式行 = null;
			生长对象.设置源模式行(派生关联);
			生长对象.中心对象 = 替换后的中心对象;
			生长对象.参数对象 = 替换后的参数对象;
			生长对象.that = that;
			//关联参数集合需重建，因为会影响“基本设置语言角色并结合语言语义两者打分判断”
			生长对象.中间的和地关联 = null;
			生长对象.构建并匹配关联本身的所有形式参数_和语言角色无关(Data.当前解析语言);

			对象对 obj1 = 对象对.查找原对象(替换对象集合, A端实际对象);
			生长对象 替换上端实际对象 = obj1 == null ? null : obj1.派生对象;
			obj1 = 对象对.查找原对象(替换对象集合, B端实际对象);
			生长对象 替换下端实际对象 = obj1 == null ? null : obj1.派生对象;

			if (Data.是派生关联(Data.聚合Guid, 生长对象.参数对象.源模式行) > 0)
			{
				if (生长对象.参数对象.A端对象.是隐藏对象())
					替换下端实际对象 = 生长对象.参数对象.A端实际对象;
				else
					替换下端实际对象 = 生长对象.参数对象.B端实际对象;
			}
			//2016年7月18日做了如下修改，有刚前边改错了？总之，这样改了就合适了。
			//道理是，既然这个对象的中心或者参数已经变了，那么这个模式行是必须重新创建的，所以不把原来的模式行代进去。
			//有刚不知道是处理别处的什么功能需要这个？如果实在需要，那么，这个【替换基概念为派生概念重建对象】的方法另外代入这个参数好了。
			//生长对象 = Processor.当前处理器.直接一级关联生长(生长对象, -2, false, 替换上端实际对象, 替换下端实际对象, 模式行);//层级传入-2，中间结果，不会参与计算，也不会被选择为结果。
			生长对象 = Processor.当前处理器.直接一级关联生长(生长对象, -2, false, 替换上端实际对象, 替换下端实际对象);//层级传入-2，中间结果，不会参与计算，也不会被选择为结果。
			//if (替换下端实际对象 != null && 生长对象 != null)
			//    生长对象.模式行.B端 = 替换下端实际对象.模式行.ID;
			//这里要重新调整语言角色，如果失败返回null。
			if (生长对象 != null && Processor.当前处理器.设置语言角色并结合语言语义两者打分判断(生长对象) == false)
				return null;
			return 生长对象;
		}

		public 生长对象 查找已结合的某个类型对象(Guid 类型ID)
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表(null, true);
			foreach (参数 obj in 概念参数表)
			{
				if (obj.已经派生() == false)
					continue;
				if (Data.是派生关联(类型ID, obj.源关联记录) > 0)
					return obj.对端派生对象.B端对象;
			}
			return null;
		}
		//算法的思路是，由新向老遍历这一级的参数，因为肯定是先聚合【推理角色】，然后进行【推导】，所以：
		//如果先发现有推理角色，那么说明是孤立的，返回true，如果先发现【推导关系】，那么返回false。
		public 生长对象 查找已结合的推理角色(bool 是孤立的)
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表(null, true);
			foreach (参数 obj in 概念参数表)
			{
				if (obj.已经派生() == false)
					continue;
				if (是孤立的 && (Data.是派生关联(Data.推导即命题间关系Guid, obj.源关联记录) > 0 || Data.是派生关联(Data.松散并列Guid, obj.源关联记录) > 0))
					return null;
				if (Data.是派生关联(Data.事件反聚推理角色Guid, obj.源关联记录) > 0)
					return obj.对端派生对象.B端对象;
			}
			return null;
		}

		public void 一级对象构建形式和关键参数(生长对象 串对象, 模式 已有记录 = null, string 形式串 = null)
		{
			//介词和串本身肯定不再分解。
			if (是介词或者串(true, true, true))
				return;
			if (概念对象参数表 == null)
				概念对象参数表 = new List<参数>();
			参数树结构 tree = 利用缓存得到基类和关联记录树();
			tree.递归取出形式和关键参数(this.中心第一根类, ref 概念对象参数表, Data.当前解析语言, 0, 1);

			//一级语义对象.加入各概念参数行到界面(一级语义对象.模式行, false, false);//形式行加入到界面。
			//下边对形式串进行满足性挂接。
			//本对象是一级对象，已经设置了【模式行】并且有了字符串，现在查找出来要求的形式里边的哪个串和自己匹配，表示对应上了，进行满足。
			if (串对象 == null && 形式串 == null)//空对象可能没有串对象，也就没有满足依据。
			{
				return;
			}
			foreach (参数 o in 概念对象参数表)
			{
				模式 原始记录 = o.源关联记录;
				if (已有记录 == null)
					已有记录 = Data.加入参数(模式行, o.源关联记录, begindex);
				else
					Data.加入参数(模式行, o.源关联记录, begindex);
				if (形式串 == null)
				{
					if (Data.是拥有形式(原始记录) && 串对象.取子串 == Data.取得嵌入串(原始记录))
					{
						o.对端派生对象 = 串对象;
						已有记录.B端 = o.对端派生对象.模式行ID;
					}
				}
				else
				{
					if (Data.是拥有形式(原始记录) && 形式串 == Data.取得嵌入串(原始记录))
					{
						o.对端派生对象 = 串对象;
						已有记录.B端 = o.对端派生对象.模式行ID;
					}
				}
			}

		}

		public void 一级对象扩展形式参数(参数 要增加的参数)
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 o in 概念参数表)
			{
				if (要增加的参数.源关联记录 == o.源关联记录 && o.对端派生对象 == null)
					o.对端派生对象 = 要增加的参数.对端派生对象;
			}

		}

		//只查找直接一级的。
		public bool 查找包含的一级参数语言角色(int 角色)
		{
			//生长对象 the对象 = this;

			//while (the对象 != null && the对象 != 中心第一根类)
			//{
			//	if ((the对象.语言角色 & 角色) > 0)
			//		return true;

			//	the对象 = the对象.中心对象;
			//}

			List<参数> 概念参数表 = 得到指定根对象的参数表();
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() == false)
					continue;
				if ((o.语言角色 & 角色) > 0)
					return true;
			}

			return false;
		}
		public bool 是省略对象的结合对象()
		{
			if (this.取子串 == "一个唱歌")
				this.是省略对象 = this.是省略对象;
			if (是省略对象)
				return true;
			if (this.参数对象 != null)
				if (this.参数对象.是省略对象的结合对象())
					return true;
			if (this.中心对象 != null)
				if (this.中心对象.是省略对象的结合对象())
					return true;
			return false;
		}
		public 生长对象 递归查找参数对象(生长对象 参数)
		{
			if (this == 参数)
				return this;

			生长对象 r = null;

			if (参数对象 != null)
			{
				r = 参数对象.递归查找参数对象(参数);
				if (r != null)
					return r;
			}

			if (中心对象 != null)
			{
				r = 中心对象.递归查找参数对象(参数);
				if (r != null)
					return r;
			}

			return null;
		}
		public 生长对象 递归查找模式行(模式 row)
		{
			if (Data.是拥有形式(row) || 是介词或者串(true, true, true))
				return null;

			if (模式行 == row)
				return this;

			生长对象 r = null;

			if (参数对象 != null)
			{
				r = 参数对象.递归查找模式行(row);
				if (r != null)
					return r;
			}

			if (中心对象 != null)
			{
				r = 中心对象.递归查找模式行(row);
				if (r != null)
					return r;
			}

			return null;
		}

		public int 递归判断冲突(生长对象 对象, SubString 重叠范围)
		{
			if (对象 == null)
				return 0;

			int 重叠方式 = 计算位置重叠性(重叠范围, 对象);

			if (重叠方式 == 2)//是交叉的了，肯定是冲突。
				return 2;

			if (重叠方式 == 0)//不是同一位置。
				return 1;

			if (重叠范围.begindex == 对象.begindex && 重叠范围.endindex == 对象.endindex)
			{
				if (this.递归查找参数对象(对象) != null)//找到了该对象的全部。
					return 0;
			}

			//模式 row = 对象.模式行;

			//if (Data.是拥有形式(row) || 对象.是介词或者串(true, true, true))//拥有形式和形式本身都认为是相同的东西。
			//    return false;

			//if (递归查找模式行(row) == null)//对方的一个关联模式行在我们这个对象里边不存在。
			//{
			//    if (计算位置重叠性(this, 对象) > 0)//然后判断是不是在本对象这个范围内。
			//        return true;//发现了冲突，本对象抑制了对方这个对象。
			//}
			int k = 递归判断冲突(对象.参数对象, 重叠范围);
			if (k == 0 || k == 2)
				return k;

			k = 递归判断冲突(对象.中心对象, 重叠范围);
			if (k == 0 || k == 2)
				return k;

			Data.Assert(false);//不应该会走到这步。
			return 1;
		}

		public void 构建并匹配关联本身的所有形式参数_和语言角色无关(int 语言)
		{
			//暂时假定拥有形式本身不会再有参数，以后可能不同，也就是一个字符串还可以拥有子串。
			if (Data.是拥有形式(源模式行))
			{
				前置介词 = null;
				前置介词逗号 = null;
				介动词后生存 = null;
				介动词后生存逗号 = null;
				后置介词 = null;
				后置介词逗号 = null;
				中间的和地 = null;
				中间的和地逗号 = null;

				return;
			}

			//一、查找出所有的关联表
			if (关联参数集合 == null)
				关联参数集合 = new List<参数>();
			参数树结构 tree = Data.利用缓存得到基类和关联记录树(源模式行, true);
			tree.递归取出关联的形式参数(this, ref 关联参数集合, 语言, 0, 0);

			//代码留着，如果出现了显示的【关联名】对象，就需要这部分内容。但现在，如果【关联名】出现，我们将把它为概念展开，所以，应该下边代码是不用了。
			//foreach (参数 o in 关联参数集合)
			//{
			//	PatternDataSet.模式Row 原始记录 = (PatternDataSet.模式Row)o.源关联记录;
			//	if (Data.概念拥有形式Guid.Equals(Data.根模式(原始记录)) && (this.ToString() == Data.取得嵌入串((string)原始记录.形式)))
			//	{
			//		生长对象 实际对象 = new 生长对象(模式行, 0);
			//		o.实际对象 = 实际对象;
			//	}
			//}
			bool 是从定 = that == 字典_目标限定.B端;
			if (前置介词 != null && 前置介词.是介词形式创建的对象) //短句停顿符介词，加入拥有前置介词形式
			{
				参数树结构 t = Data.利用缓存得到基类和关联记录树(Data.FindRowByID(Data.短句停顿符处理为介词Guid), true);
				t.递归取出关联的形式参数(this, ref 关联参数集合, 语言, 0, 0);
				是从定 = false;
			}
			//二、从关联参数集合中提取出特定的几个并定义特定的变量，比如前置介词等，方便后边的处理。
			//查找并完成介词的匹配，不管什么角色，先把这个关联对象满足的字符串对应的介词匹配出来。
			foreach (参数 o in 关联参数集合)
			{
				if (Data.是拥有形式(o.源关联记录) == false)
					continue;

				if (是从定 && o.源关联记录.That根 != 字典_语言角色.从定)//对于从定，必须是一条单独的记录的设定。
					continue;

				if (是从定 == false && o.源关联记录.That根 == 字典_语言角色.从定)
					continue;

				//DataRow 原始记录 = Data.二级关联类型(o);
				Guid 关联类型 = Data.二级关联类型(o.源关联记录);

				//看介词针对的语言角色要求的次序是否和当前实际次序相符。修改掉，这里不需要了。
				//int 序号 = 字典_语言角色.计算一级序号((int)row["语言角色"]);
				//if ((序号 != 0) && ((序号 < 0 && obj.中心在右 == false) || (序号 > 0 && obj.中心在右 == true)))
				//    continue;
				string str = Data.取得嵌入串(o.源关联记录);
				//前置介词
				if (前置介词 != null && 前置介词关联 == null && Data.关联拥有前置介词Guid.Equals(关联类型) && 前置介词.取子串 == str)
					前置介词关联 = o.源关联记录;
				//介动词后生存
				if (介动词后生存 != null && 介动词后生存关联 == null && Data.关联拥有介动词后生存Guid.Equals(关联类型) && Data.是介动词后生存串(介动词后生存.取子串))
					介动词后生存关联 = o.源关联记录;
				//后置介词
				else if (后置介词 != null && 后置介词关联 == null && Data.关联拥有后置介词Guid.Equals(关联类型) && 后置介词.取子串 == str)
					后置介词关联 = o.源关联记录;
				//的
				else if (中间的和地 != null && 中间的和地关联 == null && Data.关联拥有的Guid.Equals(关联类型) && 中间的和地.取子串 == str)
					中间的和地关联 = o.源关联记录;
				//地
				else if (中间的和地 != null && 中间的和地关联 == null && Data.关联拥有地Guid.Equals(关联类型) && 中间的和地.取子串 == str)
					中间的和地关联 = o.源关联记录;
			}


			if (前置介词关联 == null)
			{
				前置介词 = null;
				前置介词逗号 = null;
			}
			if (介动词后生存关联 == null)
			{
				介动词后生存 = null;
				介动词后生存逗号 = null;
			}
			if (后置介词关联 == null)
			{
				后置介词 = null;
				后置介词逗号 = null;
			}
			if (中间的和地关联 == null)
			{
				if (中间的和地 != null && 中间的和地.取子串 == "的")//参数表里边没有，但是实际上确实出现了"的"。也就是任何情况下，"的"总是允许的。
				{
					参数 的 = new 参数(this, Data.FindRowByID(Data.关联拥有的Guid), 字典_语言角色.从定, 字典_目标限定.A端);
					关联参数集合.Add(的);
					中间的和地关联 = 的.源关联记录;
				}
				else
				{
					中间的和地 = null;
					中间的和地逗号 = null;
				}
			}

			//三、和真实对象进行匹配。
			foreach (参数 o in 关联参数集合)
			{
				模式 原始记录 = o.源关联记录;
				if (o.对端派生对象 == null)
				{
					if (原始记录 == 前置介词关联)
						o.对端派生对象 = 前置介词;
					else if (原始记录 == 介动词后生存关联)
						o.对端派生对象 = 介动词后生存;
					else if (原始记录 == 后置介词关联)
						o.对端派生对象 = 后置介词;
					else if (原始记录 == 中间的和地关联)
						o.对端派生对象 = 中间的和地;
				}
			}
		}
		public string 显示打分串()
		{
			string s = "[" + 概率分.ToString() + "," + 完成分.ToString() + "]";
			s += "[" + 处理阶段.ToString() + "," + 有效对象序数.ToString() + "]";
			//if (中心对象 != null)
			//	s += "," + 中心对象.生长优先分.ToString();
			//if (参数对象 != null)
			//	s += "," + 参数对象.生长优先分.ToString();
			return s;
		}

		public 模式 CopyAddRow(ref Dictionary<Guid, 模式> 字典)
		{
			模式 oldRow = this.模式行;
			if (字典.ContainsKey((Guid)oldRow.ID))
			{
				//也许这里执行下数据的更新
				return 字典[(Guid)oldRow.ID];
			}
			模式 newObj;

			//注意，下边这条记录放开的话，匹配出的问题就是从原始知识派生的，而不是另起炉灶
			//当然，更准确的，应该匹配的问题要和原始知识记录等价或者引用才对，这里暂时先就如此了。
			if (生成时派生模式行)
				newObj = Data.New派生行(oldRow);
			else
				newObj = Data.CopyRow(oldRow);

			newObj.语言角色 = 语言角色;
			newObj.打分 = 显示打分串();
			newObj.ParentID = Data.NullParentGuid;
			newObj.That根 = that;
			newObj.序号 = 序号;

			字典.Add((Guid)oldRow.ID, newObj);
			if (Data.是拥有形式(newObj))//对于拥有形式，B端使用原始的。 
			{
				模式 r = Data.FindRowByID((Guid)oldRow.源记录);
				newObj.B端 = (Guid)r.B端;
			}

			Data.get模式编辑表().新加对象(newObj);

			//以下把【形式】行都填写出来。
			if (关联参数集合 != null)
				foreach (参数 参数 in 关联参数集合)
				{
					if (参数.已经派生() == false || 字典.ContainsKey(参数.对端派生对象.ID))
						continue;
					if (Data.是拥有形式(参数.源关联记录) == false)
						continue;
					模式 形式行 = Data.加入参数(newObj, 参数.源关联记录, 参数.对端派生对象.begindex);

					if (Data.是派生关联(Data.关联拥有介动词后生存Guid, 参数.源关联记录) > 0 && 介动词后生存关联 != null)//关联拥有介动词后生存这个关联的【形式】特殊，是变量，所以，这里要进行一个设置。
					{
						TheString str = new TheString(形式行.形式);
						str.嵌入串 = Data.取得嵌入串(介动词后生存.模式行);
						形式行.形式 = str.ToString();
					}

					字典.Add(参数.对端派生对象.ID, 形式行);
				}

			if (this == 中心第一根类)//只执行一次
				foreach (参数 参数 in 得到指定根对象的参数表())
				{
					if (参数.已经派生() == false || 字典.ContainsKey(参数.对端派生对象.ID))
						continue;
					if (Data.是拥有形式(参数.源关联记录) == false)
						continue;
					//PatternDataSet.模式编辑Row 形式行 = (PatternDataSet.模式编辑Row)table.NewRow();
					//oldRow = 参数.实际对象.模式行;

					//foreach (DataColumn column in table.Columns)
					//    形式行.SetField(column, oldRow[column.ColumnName]);

					//形式行.ID = Guid.NewGuid();
					//形式行.ParentID = newObj.ID;
					//形式行.that根 = (int)字典_目标限定.A端;

					//table.Add模式编辑Row(形式行);

					模式 形式行 = Data.加入参数(newObj, 参数.源关联记录, 参数.对端派生对象.begindex);
					字典.Add(参数.对端派生对象.ID, 形式行);
				}

			return newObj;

		}

		//public void 用关联对象递归设置Parent和语言角色(生长对象 整体树)
		//{
		//	if (参数对象 == null)
		//		return;
		//	if (this == 中心第一根类)
		//		return;
		//	Guid 关联类型 = Data.一级关联类型(源模式行);

		//	if (替代.是聚合或者属拥(关联类型))
		//	{
		//		this.Parent = 中心对象.中心第一根类;
		//		参数对象.中心第一根类.Parent = this;
		//	}
		//	else
		//	{
		//		生长对象 Parent对象 = 整体树.在一棵树上查找指定根所在聚合对象的最终根(整体树, 中心对象.中心第一根类);
		//		this.Parent = Parent对象;
		//		生长对象 下端对象 = 整体树.在一棵树上查找指定根所在聚合对象的最终根(整体树, 参数对象.中心第一根类);
		//		下端对象.Parent = this;
		//		下端对象.语言角色 = this.语言角色;
		//	}

		//	中心对象.用关联对象递归设置Parent和语言角色(整体树);
		//	参数对象.用关联对象递归设置Parent和语言角色(整体树);
		//}
		public void 用关联对象递归设置Parent和语言角色(生长对象 整体树)
		{
			if (参数对象 == null)//最末端的叶子，下边没有子对象可以设置。
				return;
			if (this == 中心第一根类)//最高层根，没有parent，这个只有一个。
				return;
			Guid 关联类型 = Data.一级关联类型(源模式行);


			this.Parent = 中心对象.中心第一根类;
			参数对象.中心第一根类.Parent = this;
			//生长对象 下端对象 = 整体树.在一棵树上查找指定根所在聚合对象的最终根(整体树, 参数对象.中心第一根类);
			//下端对象.Parent = this;
			参数对象.中心第一根类.语言角色 = this.语言角色;

			中心对象.用关联对象递归设置Parent和语言角色(整体树);
			参数对象.用关联对象递归设置Parent和语言角色(整体树);
		}
		public void 递归调整生成树(ref Dictionary<Guid, 模式> 字典)
		{

			模式 关联行 = CopyAddRow(ref 字典);

			//if (参数对象 != null)
			//    关联行.序号 = 参数对象.begindex;
			//else if (中心对象 != null)
			//    关联行.序号 = 中心对象.begindex;
			//else
			//    关联行.序号 = 中心第一根类.begindex;//字符串

			//if (this.层级 <= 1 || 参数对象 == null)
			//    return;


			//处理附加关联
			foreach (模式 附加关联 in Processor.当前处理器.附加关联集合)
			{
				if (附加关联.B端.Equals(模式行.ID) == false)
					continue;
				模式 newObj = Data.CopyRow(附加关联);
				字典.Add((Guid)附加关联.ID, newObj);
				newObj.显隐 = 字典_显隐.附加;
				newObj.ParentID = 关联行.ID;
				Data.get模式编辑表().新加对象(newObj);
			}


			if (Parent == null || 参数对象 == null || 中心对象 == null)
				return;

			模式 Parent概念行 = Parent.CopyAddRow(ref 字典);

			关联行.ParentID = Parent概念行.ID;
			bool 参数是字符串 = Data.字符串Guid.Equals(参数对象.中心第一根类.源模式行ID);
			if (参数是字符串 == false)
			{
				模式 参数概念行 = 参数对象.中心第一根类.CopyAddRow(ref 字典);
				参数概念行.ParentID = 关联行.ID;
			}

			中心对象.递归调整生成树(ref 字典);

			if (参数是字符串 == false)
				参数对象.递归调整生成树(ref 字典);


		}



		public override string ToString()
		{

			string str = "[整体:\"" + 显式串 + "\"根：" + 根Name + "]";
			if (中心对象 == null)
				str += "      [中心:null]";
			else
				str += "      [中心:\"" + 中心对象.显式串 + "\"根：" + 中心对象.根Name + "]";
			if (参数对象 == null)
				str += "      [参数:null]";
			else
				str += "      [参数:\"" + 参数对象.显式串 + "\"根：" + 参数对象.根Name + "]";

			return str;
		}


		public void 递归找出所有的无形式空对象(List<生长对象> 结果)
		{
			if (是无形式空对象)
				结果.Add(this);
			if (左对象 != null)
				左对象.递归找出所有的无形式空对象(结果);
			if (右对象 != null)
				右对象.递归找出所有的无形式空对象(结果);
		}

		public bool 包含有结果中的无形式空对象(List<生长对象> 结果)
		{
			if (是无形式空对象)
			{
				foreach (生长对象 obj in 结果)
					if (this == obj)
						return true;
			}
			if (左对象 != null && 左对象.包含有结果中的无形式空对象(结果))
				return true;
			if (右对象 != null && 右对象.包含有结果中的无形式空对象(结果))
				return true;

			return false;
		}


		public void 重计算打分()
		{
			长度分 = endindex - begindex;
			if (中心对象 == null)
				类型分 = -1;//字符串
			else if (中心对象.是NullThis空对象())
				类型分 = -1;
			else
			{
				if (Data.能够序列化(中心对象.中心第一根类.模式行))
					类型分 = 1;
				else
					类型分 = 0;
				//if (中心对象.覆盖型对象位标识 > 0)//对于【推导】等覆盖型的，加分。
				//    打分.类型分 += 1;

			}
		}

		//表示这个对象是任意的，就好比【概念】一样
		//【这】【那】单独出现的时候就是如此，可以代表任何东西。
		public bool 是任意概念()
		{
			//if(是NullThis空对象())//空对象应该说也是的。
			//	return true;
			if (Data.是派生类(Data.外延指定Guid, 模式行, 替代.正向替代))//【这】【那】单独用可以指代任何概念。
				return true;
			if (Data.是派生类(Data.什么Guid, 模式行, 替代.正向替代))//疑问【什么】单独用可以指代任何概念。
				return true;
			if (Data.是派生类(Data.什么事物Guid, 模式行, 替代.正向替代))//疑问【什么】单独用可以指代任何概念。
				return true;
			return false;
		}

		public bool 是要求紧密相邻的参数()
		{
			if (Data.什么Guid.Equals(源模式行ID) || Data.什么Guid.Equals(模式行ID))
				return true;
			if (Data.什么量Guid.Equals(源模式行ID) || Data.什么量Guid.Equals(模式行ID))
				return true;
			if (Data.是派生类(Data.符合程度Guid, 源模式行, 替代.正向替代))
				return true;
			return false;
		}

		public bool 是NullThis空对象()
		{
			bool b = Data.ThisGuid.Equals(中心第一根类.源模式行ID);
			Data.Assert(b == false || 字典_显隐.隐藏 == (short)中心第一根类.模式行.显隐);
			return b;
		}

		public bool 是隐藏对象()
		{
			return 中心第一根类.长度 == 0;//字符串是空的。
		}



		public void 根据内部对象完成范围设置()
		{
			SubString 左位置对象 = 左对象;
			SubString 右位置对象 = 右对象;
			if (左对象.长度 == 0/* && 左对象.覆盖型对象位标识 > 0*/)
				左位置对象 = 右对象;
			if (右对象.长度 == 0/* && 右对象.覆盖型对象位标识 > 0*/)
				右位置对象 = 左对象;

			SubString 新增范围 = new SubString(内部对象计算左边界(), 内部对象计算右边界());//取出包括外围介词等的最终边界。

			中心第一根类 = 中心对象.中心第一根类;//根也要调整。

			begindex = 左位置对象.begindex;
			endindex = 右位置对象.endindex;

			增加范围(新增范围);

			//覆盖型对象位标识 = 左对象.覆盖型对象位标识 + 右对象.覆盖型对象位标识;
		}


		public void 完成两端的对象参数表()
		{
			//参数等于空的情况下，就是对象自身没有发生变化，这里简单的把自己的参数再重新设置一遍（其实没有变化），只是最后对新增关联进行【概念参数】的扩展。


			//注意：概念参数挂在真正的拥有参数的A端中心，而不考虑形式上关注的that中心。

			//生长对象 最新上一版本A端对象 = 选择未完成树对的一枝查找指定根的最新版本对象(A端对象.中心第一根类, 新增关联.that);
			//复制概念参数(最新上一版本A端对象);

			复制概念参数(A端对象);
			复制概念参数(B端对象);

			处理对象的概念参数表(this, false);

			//if (Data.是分类(Data.根模式(新增关联.模式行)))//是属于和拥有关联，立即进行参数的复制。
			//{
			//	//需要把关联的A端和B端替换了再增加。
			//	//最新的上一版本对象 = 选择未完成树对的一枝查找指定根的最新版本对象(B端参数.当前根,!新增关联.反向关联);
			//	//复制概念参数(最新的上一版本对象);//概念参数总是挂在真正的拥有参数的A端，正向的一边，而不考虑形式上关注的中心。
			//}

		}
		public void 复制概念参数(生长对象 基对象)
		{
			if (概念对象参数表 == null)
				概念对象参数表 = new List<参数>();
			if (基对象.概念对象参数表 == null)
				return;
			foreach (参数 参数 in 基对象.概念对象参数表)
				概念对象参数表.Add(new 参数(参数));
		}

		public void 复制关联参数(生长对象 基对象)
		{
			if (基对象.关联参数集合 == null)
				return;
			关联参数集合 = new List<参数>();
			foreach (参数 参数 in 基对象.关联参数集合)
			{
				参数 obj = new 参数(参数);
				关联参数集合.Add(obj);
			}
		}
		public bool 允许该关联重复(模式 关联)
		{
			//句子和短句是不会有重复出现的，但是如果有封闭范围的间隔就可以出现
			//同一封闭范围的间隔前边已经处理了，这里出现重复的情况肯定是在不同封闭范围内的，因此也可以看着允许重复的。比如【他说'我吃了。'。】里边的两个
			//句号是允许的。
			if (Data.是派生关联(Data.概念属拥句子Guid, 关联) > 0)
				return true;
			if (Data.是派生关联(Data.概念属拥短句停顿Guid, 关联) > 0)
				return true;
			//参数字段 参数 = new 参数字段((string)关联.参数集合);
			if (关联.参数.B端重复数 > 0)
				return true;
			return false;
		}

		public bool 是不能再属拥的终结事物对象()
		{
			if (Data.是派生类(Data.什么事物Guid, 中心第一根类.模式行, 替代.正向替代))
				return true;
			return false;
		}

		//附属对象一般是一种角色，首先要被主对象【属于】结合进去，所以，开始不会被拥有。
		//这里的判断是用【概念拥有形式】产生的对象才不是附属对象，方法可能不严谨，以后再改。
		//而且附属对象最后不能单独存在，可能要被拥有。
		public bool 是附属对象()
		{
			//if (Data.是派生类(Data.推理角色Guid, 模式行, 替代.正向替代))//【如果】【那么】等符号本身不能直接被拥有，需要被结合起来以后。
			//    return true;
			if (Data.什么Guid.Equals(源模式行ID) || Data.什么Guid.Equals(模式行ID))
				return true;
			return false;
			//if (Data.是介词或者串(模式行, true, true, true))//形式对象自己本身不是附属对象。
			//	return false;
			//if (是虚拟对象())
			//	return false;
			//if (概念参数集合 != null)
			//	foreach (参数 参数 in 概念参数集合)
			//		if (Data.是真正的形式(参数.源关联记录))
			//			return false;
			//if (关联参数集合 != null)
			//	foreach (参数 参数 in 关联参数集合)
			//		if (Data.是真正的形式(参数.源关联记录))
			//			return false;
			//return true;
		}

		public bool 初步判断允许介词()
		{
			if (Data.是派生类(Data.语用基类Guid, 中心第一根类.源模式行, 替代.正向替代))
				return false;
			return true;
		}
		public 生长对象 获取已结合的最右端的参数对象()
		{
			List<参数> 概念参数表 = 得到指定根对象的参数表(this.中心第一根类);
			foreach (参数 参数 in 概念参数表)
			{
				if (参数.源关联记录 != null && 参数.已经派生())
				{
					if (参数.对端派生对象.endindex == this.endindex)
						return 参数.对端派生对象.参数对象;
				}
			}
			return null;
		}

		//this对象是整体的根，然后在指定的对象所属根中寻找
		public 参数 查找已经实现的参数(模式 参数关联, 生长对象 所属根 = null, int 实现方式 = Data.派生实现, 模式 B端 = null)
		{
			if (所属根 == null)
				所属根 = this.中心第一根类;
			List<参数> 概念参数表 = 得到指定根对象的参数表(所属根);
			foreach (参数 参数 in 概念参数表)
			{
				if (参数.源关联记录 != null && 参数.已经派生())
				{
					//if (包括的从句关联 == false && 参数.实际对象.是显式的字句())//的从句可能可以重复，所以不参与计算是否重复
					//	continue;//或者，以后也处理为不允许重复，外部的等价对象的关系额外处理。
					//if (id.Equals(参数.源关联记录["ID"]))//完全相等
					//{
					//    生长对象 a=this.参数对象.当前根;
					//    生长对象 b = 参数.实际对象.当前根;
					//}
					//if (id.Equals(参数.源记录["ID"]))//主要就是对关联进行处理，因此就是用【源记录】。
					//if (Data.是派生关联(基关联ID, 参数.源关联记录) > 0 )//是派生关联，就屏蔽了基关联。
					//注意，以前是单向计算，现在是双向计算了关联。
					if ((实现方式 & Data.派生实现) > 0 && Data.是派生关联(参数关联.ID, 参数.源关联记录) > 0)
					{
						if (B端 == null || Data.是派生类((Guid)B端.ID, 参数.对端派生对象.B端对象.模式行, 替代.正向替代))
							return 参数;
					}
					if ((实现方式 & Data.基类实现) > 0 && Data.是派生关联(参数.源关联记录.ID, 参数关联) > 0)
					{
						if (B端 == null || Data.是派生类(参数.对端派生对象.B端对象.模式行.ID, B端, 替代.正向替代))
							return 参数;
					}
				}
			}
			return null;
		}
		//返回当前对象已经结合的冒号或破折号
		public 生长对象 查找已结合的冒号或破折号()
		{
			参数 参数 = 查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid));
			if (参数 != null)
			{
				if (Data.是派生类(Data.下文引导冒号Guid, 参数.对端派生对象.参数对象.源模式行, 替代.正向替代)
					|| Data.是派生类(Data.破折停顿符Guid, 参数.对端派生对象.参数对象.源模式行, 替代.正向替代))
					return 参数.对端派生对象.参数对象;
			}
			return null;
		}
		//两个实体参数是相同的，就找出这两个实体参数各种诞生的形式，看形式是否是不同部件，如果是不同部件，那么就返回合适的。
		//返回0，表示没有满足。
		//返回1，表示语义上满足，但是形式上是不同部件。
		//返回2，表示语义和形式上都重复。
		public int 这个参数重复满足(生长对象 所属根, 模式 参数关联, 模式 B端 = null, bool 允许重复 = true)
		{
			if (允许重复 && 允许该关联重复(参数关联))
				return 0;
			//if (Data.是派生关联(id, Data.FindRowByID(模式行.源记录)) > 0)//先处理本身，概念参数集合中不包括本身。不过看来不用了。
			//	return 2;

			参数 已有参数 = 查找已经实现的参数(参数关联, 所属根, Data.派生实现, B端);
			if (已有参数 != null)
				return 2;
			return 0;
		}

		//增加了这个关联记录对象后，本对象的参数满足表发生变化。
		//为了方便，还是让参数表都存在中心对象，而不是A端对象中。那么，一个对象的参数就还需要在其他地方寻找。
		public void 处理对象的概念参数表(生长对象 关联记录对象, bool 关联记录对象是新知识)
		{
			if (概念对象参数表 == null)
				概念对象参数表 = new List<参数>();

			生长对象 实际对象 = A端实际对象 != null ? A端实际对象.中心第一根类 : A端对象.中心第一根类;

			List<参数> 概念参数表 = 得到指定根对象的参数表(实际对象);

			foreach (参数 参数 in 概念参数表)
			{
				if (参数.源关联记录 != null && (关联记录对象.源模式行ID).Equals(参数.源关联记录.ID))//主要就是对关联进行处理，因此就是用【源记录】。
				{
					if (参数.已经派生())//已经填写了，产生了一个参数填写两次的问题，应该只能填写一个，或者进行一个重新的调整。
					{
						//考虑尝试把所有参数进行一个调整。
					}
					else
					{
						参数.对端派生对象 = 关联记录对象;
						参数.语言角色 = 关联记录对象.语言角色;
					}
					return;
				}
			}

			参数 obj = new 参数(实际对象, 关联记录对象, 语言角色, 关联记录对象是新知识);
			//obj.反向关联 = 反向关联;
			概念对象参数表.Add(obj);
		}

		public 模式 从关联参数表中查找形式(Guid 目标形式分类, int 语言, int 语言角色值, string 明确的串)
		{
			if (关联参数集合 == null)
				return null;
			foreach (参数 obj in this.关联参数集合)
			{
				Guid 关联类型 = Data.二级关联类型(obj.源关联记录);

				if (目标形式分类.Equals(关联类型) == false)
					continue;
				if (字典_语言.满足语义或者指定语言(语言, (int)obj.源关联记录.语言) == false)
					continue;
				//这个代码有点奇怪，不检查的话肯定不行，就出现了【主被】的角色却要求“把”
				//如果检查，好像又有地方会出现错误。
				if ((语言角色值 & (int)obj.源关联记录.语言角色) == 0)
					continue;
				if (明确的串 == "" || 明确的串 == Data.取得嵌入串(obj.源关联记录))
					return obj.源关联记录;
			}
			return null;
		}

		public bool 解释介动词后生存的合法性(模式 语言部件行, Guid 目标形式分类, int 语言)
		{
			if (语言部件行 != null)
			{
				if ((this.语言角色 & (int)语言部件行.语言角色) != 0)
					return true;//暂时返回真，表示多了介词总是可以的。只要语言角色等都满足。
				else
					return false;
			}
			return true;
		}

		public bool 从句必须带显式的()
		{
			if (Data.能够序列化(中心对象.中心第一根类.源模式行))
				return true;
			return false;
		}

		//用于剥离隐藏对象
		public 生长对象 剥离隐藏对象()
		{
			if (中心对象 != null && 中心对象.是隐藏对象())
				return 参数对象;
			if (参数对象 != null && 参数对象.是隐藏对象())
				return 中心对象;
			return this;
		}

		public bool 允许从句不带显式的()
		{
			if (中心对象.剥离隐藏对象().是压缩形式对象)//压缩形式对象都可以不带，比如【者】【时】等
				return true;

			if (Data.能够序列化(参数对象.中心第一根类.源模式行) && Data.能够序列化(中心对象.中心第一根类.源模式行) == false)//序列化对象的从句，
			{
				Guid id = 中心对象.剥离隐藏对象().中心第一根类.源模式行ID;
				if (id.Equals(Data.时间点Guid) || id.Equals(Data.基准相对时间Guid))//【时】【时间】等可以在动词后边不带【的】作为从定。不过，此时可能要求两者是紧密相邻。
					return true;
				return false;
			}
			//显示的推理角色为中心时，允许不带的
			if (Data.是派生类(Data.推理角色Guid, 中心对象.中心第一根类.源模式行, 替代.正向替代) && 中心对象.中心第一根类.是介词形式创建的对象 == false)
				return true;
			return false;
		}

		//一个对象已经完成，可以作为另一个对象的参数。
		public bool 完成性检查()
		{

			if (参数对象.是压缩形式对象 == true)//压缩对象，比如【以来】【者】【时】【率】【上】【下】等不行！
			{
				//“以前的苹果”是可以的，所以[基准相对时间]，如果有介词“的”是允许的
				if (Data.是派生类(Data.基准相对时间Guid, 参数对象.源模式行, 替代.正向替代) && 中间的和地 != null)
					return true;
				else
					return false;
			}

			return true;
		}

		public int 解释语言角色语序和语言形式部件的合法性打分(模式 语言部件行, Guid 目标形式分类, int 语言)
		{
			bool 计算的关联 = (that == 字典_目标限定.B端) && Data.关联拥有的Guid.Equals(目标形式分类);

			if (语言部件行 == null)//实际上没有介词，给出没有的理由
			{
				//这部分的强制设置优先于后边的关联参数表设置！
				if (计算的关联)//计算从句【的】关联的有效性。
				{
					if (从句必须带显式的())//必须带显式的.
						return 0;

					if (允许从句不带显式的())//对于从句，允许不带显式的，那么返回9.
						return 9;
				}

				模式 row = 从关联参数表中查找形式(目标形式分类, 语言, this.语言角色, "");
				if (row == null)//知识没有要求有，很合适。
				{
					if (计算的关联 && 允许从句不带显式的() == false)//对于从句，如果参数方是【可序列化的】，那么必须有【的】，否则会有歧义，比如【借的她】。当然，还有一种情况，就是后边的中心是【者】等这种【压缩形式】，是例外，相当于已经有了【的】
						return 0;
					return 9;
				}
				return 9 - row.参数.B对A的关键性;//要求有而实际上没有，返回一个计算的阀值。
			}
			else//实际上有，给出有的理由。
			{
				模式 row = 从关联参数表中查找形式(Data.二级关联类型(语言部件行), 语言, this.语言角色, Data.取得嵌入串(语言部件行));
				if (row == null)
					return this.语言角色 == 字典_语言角色.从定 ? 9 : 0;//从定在没有规定的情况下，是允许【的】。
				return row.参数.概率分;//知识没有要求有，这种情况不允许。
				//if ((this.语言角色 & (int)语言部件行.语言角色) != 0)
				//	return 9;//暂时返回真，表示多了介词总是可以的。只要语言角色等都满足。
				//else
				//	return 0;
			}
		}

		public 生长对象(生长对象 A端obj, 生长对象 B端obj, 模式 源关联, int that端)
		{
			A端对象 = A端obj;
			B端对象 = B端obj;
			设置源模式行(源关联);
			that = that端;
		}

		public 生长对象()
		{
		}


		public 生长对象(模式 row, int 处理层级, bool 是概念行 = true, bool 是模板对象 = false)
		{
			if (是概念行)
			{
				endindex = begindex = (int)row.序号;
				if (是模板对象 == false || Data.是拥有形式(row))
				{
					string str = row.显隐 == 字典_显隐.隐藏 ? "" : row.形式;
					endindex = begindex + str.Length;
				}
			}
			设置模式行(row);
			中心第一根类 = this;
			处理轮数 = 处理层级;
			if (Data.是派生类(Data.形式集合Guid, row, 替代.正向替代))
				集合对象的基对象 = Data.FindRowByID(Data.ThisGuid);
			//打分 = new 打分字段(关联row.打分);
		}

		//为匹配派生的时候使用.
		public 生长对象(生长对象 原始对象, 模式 关联row)
		{
			设置模式行(关联row);
			是疑问变量 = 原始对象.是疑问变量;
			生成时派生模式行 = true;
			处理轮数 = 原始对象.处理轮数;
			是介词形式创建的对象 = 原始对象.是介词形式创建的对象;
			完成分 = 原始对象.完成分;
			是压缩形式对象 = 原始对象.是压缩形式对象;
			是定语形式对象 = 原始对象.是定语形式对象;
			介动词等情况延后一阶段生长 = 原始对象.介动词等情况延后一阶段生长;
			中心第一根类 = this;
			begindex = 原始对象.begindex;
			endindex = 原始对象.endindex;
			中心在右 = 原始对象.中心在右;
			that = 原始对象.that;
			语言角色 = 原始对象.语言角色;
			集合对象的基对象 = 原始对象.集合对象的基对象;
			//覆盖型对象位标识 = 原始对象.覆盖型对象位标识;
		}

		public 生长对象(生长对象 中心, 生长对象 参数, 模式 关联row)
		{
			处理轮数 = 中心.处理轮数 + 1;
			设置模式行(关联row);
			中心对象 = 中心;
			参数对象 = 参数;
			中心第一根类 = 中心.中心第一根类;
			//覆盖型对象位标识 = 中心.覆盖型对象位标识 + 参数.覆盖型对象位标识;

			关联row.A端 = 中心.中心第一根类.模式行ID;
			关联row.B端 = 参数.中心第一根类.模式行ID;

			//生长对象 最新的上一版本对象 = 选择未完成树对的一枝查找指定根的最新版本对象(A端对象.中心第一根类, 字典_目标限定.A端);
			//复制概念参数(最新的上一版本对象);
			完成两端的对象参数表();
			//复制概念参数(中心对象);
			//复制概念参数(参数对象);

			//处理对象的关键完成参数表(this, false);
		}




		public void 设置疑问状态()
		{
			//if (Data.是派生类(Data.什么Guid, 源模式行, 替代.正向替代))
			//     是疑问变量 = true;
			// else if (Data.是派生关联(Data.概念属拥疑问性Guid, 源模式行)>0)
			//     是疑问变量 = true;
			是疑问变量 = false;
		}

		//现在考虑是要相邻的，以后其实可以不要求一定要相邻，可以远程的发现两个概念可以先结合，然后中间生长，那样有更高的效率和可能性。
		public List<生长对象> 在树上查找准备作为中心来生长的边界对齐对象(int 边界, bool 查找右边界)
		{
			List<生长对象> result = new List<生长对象>();

			//递归在树上查找准备作为中心来生长的边界对齐对象(边界, 查找右边界, ref result);

			//如果把上边屏蔽而用下边，就是必须按部就班的从叶子到根生长，不允许先长树枝，但实际效率并不能提高，反而失去了明确的优先的框架生长优势。
			if (是介词或者串(true, true, true))
				return result;

			if ((查找右边界 == true && endindex == 边界) || (查找右边界 == false && begindex == 边界))
				result.Add(this);

			return result;
		}

		public bool 是介词或者串(bool 包含前置介词, bool 包含后置介词, bool 包含串)
		{
			return Data.是介词或者串(中心第一根类.模式行, 包含前置介词, 包含后置介词, 包含串);
		}


		private 生长对象 列表中有相同根的对象(ref List<生长对象> result)
		{
			foreach (生长对象 obj in result)
				if (中心第一根类 == obj.中心第一根类)
					return obj;
			return null;
		}

		public 生长对象 选择未完成树对的一枝查找指定根的最新版本对象(生长对象 根, int that端)
		{
			生长对象 o = that端 == 字典_目标限定.B端 ? 参数对象 : 中心对象;

			o = o.在一棵树上查找指定根的最新版本对象(根);

			Data.Assert(o != null);

			return o;
		}

		public 生长对象 在一棵树上查找指定根的最新版本对象(生长对象 根)
		{
			if (that == 字典_目标限定.A端 && 中心第一根类 == 根)
				return this;

			if (A端对象 == null)
				return null;

			if (A端对象.中心第一根类 == 根)
				return this;

			if (参数对象 != null)
			{
				生长对象 o = 参数对象.在一棵树上查找指定根的最新版本对象(根);
				if (o != null)
					return o;
			}
			if (中心对象 != null)
			{
				生长对象 o = 中心对象.在一棵树上查找指定根的最新版本对象(根);
				if (o != null)
					return o;
			}

			return null;
		}

		//这个方法的原理，其实就是排除掉所有的，只进行两个处理：
		//1、如果目标对象在【聚合或者属拥】作为参数，那么就转换给中心对象，并从整体树开始重头来过。
		//2、最终会找到一个对象，从整体树开始没有发现在【聚合或者属拥】中作为参数，那么最后返回这个对象的原始对象。
		//比如【推理角色】【借】【完成】的三个对象的聚合里边，对任意一个的查询最终都返回【借】。对【借出方】【人】的两个的聚合的任意一个访问，也都返回【人】。
		public 生长对象 在一棵树上查找指定根所在聚合对象的最终根(生长对象 整体树, 生长对象 根)
		{
			//找到了原始对象，直接返回。
			if (this == 根)
				return this;

			//if (A端对象 == null)
			//    return null;

			Guid 关联类型 = Data.一级关联类型(源模式行);

			//交换对象，重头来过。
			if (替代.是聚合或者属拥(关联类型))
			{
				if (参数对象.中心第一根类 == 根)
					return 整体树.在一棵树上查找指定根所在聚合对象的最终根(整体树, 中心对象.中心第一根类);
			}

			if (参数对象 != null)
			{
				生长对象 o = 参数对象.在一棵树上查找指定根所在聚合对象的最终根(整体树, 根);
				if (o != null)
					return o;
			}
			if (中心对象 != null)
			{
				生长对象 o = 中心对象.在一棵树上查找指定根所在聚合对象的最终根(整体树, 根);
				if (o != null)
					return o;
			}

			return null;
		}


		private void 递归在树上查找准备作为中心来生长的边界对齐对象(int 边界, bool 查找右边界, ref List<生长对象> result)
		{
			if (begindex > 边界 || endindex < 边界)
				return;

			if (是介词或者串(true, true, true))//串不会作为主动发起的生长，所以不会返回。
				return;

			//if (新版本对象列表 != null)
			//{
			//    foreach (生长对象 o in 新版本对象列表)
			//    {
			//        if ((查找右边界 == true && o.方面主对象.endindex == 边界) || (查找右边界 == false && o.方面主对象.begindex == 边界) && result.IndexOf(o) == -1)
			//        {
			//            if (o.列表中有相同根的对象(ref result) == null)//相同根的对象，只取当前最新的（参数更多）。屏蔽旧的（参数较少）。
			//                result.Add(o);
			//        }
			//    }
			//}

			if ((查找右边界 == true && endindex == 边界) || (查找右边界 == false && begindex == 边界) && result.IndexOf(this) == -1)
			{
				if (列表中有相同根的对象(ref result) == null)//相同根的对象，只取当前最新的（参数更多）。屏蔽旧的（参数较少）。
					result.Add(this);
			}

			if (左对象 != null)
				左对象.递归在树上查找准备作为中心来生长的边界对齐对象(边界, 查找右边界, ref result);
			if (右对象 != null)
				右对象.递归在树上查找准备作为中心来生长的边界对齐对象(边界, 查找右边界, ref result);
		}


		public void 合并字符串位置()
		{
			//直接合并A端和B端两个行的字符位置。如果有别的介词等那么应该是合并到A端或B端中的。如果【关联】是显示的，那么就展开把【关联本身】作为一端和
			//两端建立两个连接了。
		}

		//static public bool 递归增加范围(生长对象 每个受影响对象, 生长对象 实际合并中心对象, SubString 新增范围)
		//{
		//	if (每个受影响对象 == null)
		//		return false;

		//	if (每个受影响对象 == 实际合并中心对象)
		//	{
		//		每个受影响对象.增加范围(新增范围);
		//		return true;
		//	}

		//	if (每个受影响对象.中心对象 != null && 递归增加范围(每个受影响对象.中心对象, 实际合并中心对象, 新增范围))
		//	{
		//		每个受影响对象.增加范围(新增范围);
		//		return true;
		//	}

		//	if (每个受影响对象.参数对象 != null && 递归增加范围(每个受影响对象.参数对象, 实际合并中心对象, 新增范围))
		//	{
		//		每个受影响对象.增加范围(新增范围);
		//		return true;
		//	}

		//	return false;
		//}

		//最新的对象被调用处理这个来把实际合并的对象到自己上一级的对象中所有受叶子补生长变化的对象加入到新生长结果中，但自己却不用，因为自己当前只有一个版本。
		//public bool 生成受叶子补生长影响的所有对象新版本(生长对象 目标, 生长对象 实际合并中心对象, 生长对象 实际合并参数对象, SubString 新增范围, bool 反向关联)//参数是最新的对象的上一个中心对象。
		//{
		//    if (目标 == null)
		//        return false;

		//    if (目标 == 实际合并中心对象)//递归到最深找到了实际生长的参数对象，深度递归截止，处理后返回。这个只可能执行一次。
		//    {
		//        加入受影响的子对象的新版本(目标, 实际合并参数对象, 新增范围, 反向关联);
		//        return true;
		//    }

		//    //处理中心对象，不过以后想想似乎可以去掉。只是参数部分有这个需要？中心部分不需要。
		//    if (目标.中心对象 != null && 生成受叶子补生长影响的所有对象新版本(目标.中心对象, 实际合并中心对象, 实际合并参数对象, 新增范围, 反向关联))
		//    {
		//        加入受影响的子对象的新版本(目标, 实际合并参数对象, 新增范围, 反向关联);
		//        return true;
		//    }

		//    //处理参数对象
		//    if (目标.参数对象 != null && 生成受叶子补生长影响的所有对象新版本(目标.参数对象, 实际合并中心对象, 实际合并参数对象, 新增范围, 反向关联))
		//    {
		//        加入受影响的子对象的新版本(目标, 实际合并参数对象, 新增范围, 反向关联);
		//        return true;
		//    }

		//    return false;
		//}

		//public 生长对象 加入受影响的子对象的新版本(生长对象 目标, 生长对象 新增的参数对象, SubString 新增范围, bool 反向关联)
		//{

		//    Guid 唯一标识 = Data.合并Guid(this.模式行.ID, 目标.唯一性标识, 新增的参数对象.唯一性标识);
		//    生长对象 同一对象 = Processor.当前处理器.查找同一局面对象(唯一标识);
		//    if (Processor.当前处理器.查找同一局面对象(唯一标识) != null)
		//        return 同一对象;

		//    生长对象 o = new 生长对象(this);
		//    o.设置中心和参数(目标, 新增的参数对象, 新增范围, this);
		//    o.唯一性标识 = 唯一标识;

		//    if (新版本对象列表 == null)
		//        新版本对象列表 = new List<生长对象>();
		//    新版本对象列表.Insert(0, o);//插入到最前边，保证最新的最先查找到。

		//    //注意，把这个中间生成的东西也加到结果表，看看这部分是否可以优化去掉!!!!
		//    o.同步到结果表();
		//    return o;
		//}

		//public 生长对象 加入新版本对象(生长对象 旧对象, bool 反向关联)
		//{

		//	Guid 唯一标识 = Data.合并Guid(this.模式行.ID, 旧对象.唯一性标识, Guid.Empty);

		//	生长对象 同一对象 = Processor.当前处理器.查找同一局面对象(唯一标识);
		//	if (Processor.当前处理器.查找同一局面对象(唯一标识) != null)
		//		return 同一对象;

		//	生长对象 o = new 生长对象(旧对象);
		//	旧对象.模式行 = this.模式行;
		//	o.设置中心和参数(旧对象, null, null, this, 反向关联);
		//	o.唯一性标识 = 唯一标识;

		//	if (新版本对象列表 == null)
		//		新版本对象列表 = new List<生长对象>();
		//	新版本对象列表.Insert(0, o);//插入到最前边，保证最新的最先查找到。

		//	//注意，把这个中间生成的东西也加到结果表，看看这部分是否可以优化去掉!!!!
		//	o.同步到结果表();
		//	return o;
		//}



		//先中心，后参数，所以，这是返回最终端的那个参数就是最终节点。
		public 生长对象 遍历得到终点对象()
		{
			if (参数对象 == null)
				return this;
			return 参数对象.遍历得到终点对象();

		}

		//
		public void 为构建查询模板递归得到查找所有根概念节点(List<生长对象> 概念节点)
		{
			if (Data.是派生关联(Data.概念属拥疑问性Guid, 模式行) > 0)
				是疑问变量 = true;

			if (this == 中心第一根类)//是一个根节点。
			{
				foreach (疑问概念结构 o in Data.疑问变量结构)
					if (o.疑问概念.Equals(源模式行ID) && o.疑问概念.Equals(Data.疑问句Guid) == false)
						是疑问变量 = true;

				//要去掉【句子】【逗号】等一些信息，这些信息不作为问题的参数，忽略掉。
				Data.Assert(Data.ThisGuid.Equals(模式行.A端) && 替代.可正向替代(Data.一级关联类型(模式行)));
				//if (Data.什么Guid.Equals(模式行["源记录"]))//疑问【什么】，这个记录不处理。而且，应该要做一些主对象的标记。
				//    return;
				//if (Data.是派生类(Data.句子语用基类Guid, 源模式行, 替代.正向替代))//属拥句子这个记录不处理。自身这个时候一般是疑问句。
				//    return;
				if (Data.是介词或者串(模式行, true, true, true) == false)
					概念节点.Add(this);
				return;
			}
			if (中心对象 != null)
			{
				中心对象.中心第一根类.关联个数++;
				中心对象.为构建查询模板递归得到查找所有根概念节点(概念节点);
			}
			if (参数对象 != null)
			{
				参数对象.中心第一根类.关联个数++;
				参数对象.为构建查询模板递归得到查找所有根概念节点(概念节点);
			}
		}

		public void 递归得到查找所有关联节点(生长对象遍历节点 X节点, List<生长对象遍历节点> 遍历结构, int X节点索引)
		{

			if (中心第一根类 == this)//概念节点。
				return;
			if (中心对象 != null)
				中心对象.递归得到查找所有关联节点(X节点, 遍历结构, X节点索引);
			if (参数对象 != null)
				参数对象.递归得到查找所有关联节点(X节点, 遍历结构, X节点索引);

			for (int Y节点索引 = 0; Y节点索引 < X节点索引; Y节点索引++)
			{
				生长对象遍历节点 Y节点 = 遍历结构[Y节点索引];
				//用上下端对象来计算，以得到实际语言的组合，而不是【parent】的组合。
				if (A端实际对象.中心第一根类 == X节点.模板对象.中心第一根类 && B端实际对象.中心第一根类 == Y节点.模板对象.中心第一根类)
				{
					X节点.加入关联(new 生长对象遍历节点(this, Y节点索引, 字典_目标限定.A端));
					return;
				}
				if (B端实际对象.中心第一根类 == X节点.模板对象.中心第一根类 && A端实际对象.中心第一根类 == Y节点.模板对象.中心第一根类)
				{
					X节点.加入关联(new 生长对象遍历节点(this, Y节点索引, 字典_目标限定.B端));
					return;
				}
			}
		}

		public void 加入疑问变量的匹配结果(List<生长对象> 结果)
		{
			if (是疑问变量)
				结果.Add(this);
			else
			{
				if (中心对象 != null && 中心对象 != this)
					中心对象.加入疑问变量的匹配结果(结果);
				if (参数对象 != null && 参数对象 != this)
					参数对象.加入疑问变量的匹配结果(结果);
			}
		}

		//根据【全部遍历结构】当前已经完成的匹配状态，结合源结构obj和遍历结构中的实际【对象】和【关联】来生成。
		public 生长对象 用当前匹配结果构造一个匹配对象()
		{
			//if(匹配遍历对象==null)//这条记录没有进行遍历，一般不会发生。
			//    return null;
			if (Data.是派生关联(Data.概念属拥疑问性Guid, 模式行) > 0)
			{
				生长对象 o = 中心对象.用当前匹配结果构造一个匹配对象();
				o.是疑问变量 = true;
				return o;
			}
			//DataRow 源对象 = Data.FindRowByID((Guid)(Data.FindRowByID((Guid)模式行["B端"])["源记录"]));
			//if (Data.是派生关联(Data.属拥句子Guid, 模式行) > 0 && Data.是派生类(Data.疑问句Guid, 源对象, 替代.正向替代))
			//{
			//    return 中心对象.用当前匹配结果构造一个匹配对象();
			//}

			//if (Data.什么Guid.Equals(模式行["B端"]))
			//    return 中心对象.用当前匹配结果构造一个匹配对象(parentid);
			//if (匹配遍历对象.匹配的派生模式行 == null)//这条记录无需匹配即成功，比如【疑问】。
			//{//一般是参数对象是疑问。
			//    return 中心对象.用当前匹配结果构造一个匹配对象(parentid);
			//}
			if (this == 中心第一根类)//是一个根节点。
			{
				Data.Assert(Data.ThisGuid.Equals(模式行.A端) && 替代.可正向替代(Data.一级关联类型(模式行)));
				Data.Assert(参数对象 == null);

				return new 生长对象(this, 匹配遍历对象.匹配出的答案对象);
			}
			else
			{
				生长对象 newobj = new 生长对象(this, 匹配遍历对象.匹配出的答案对象);

				newobj.中心对象 = 中心对象.用当前匹配结果构造一个匹配对象();
				newobj.参数对象 = 参数对象.用当前匹配结果构造一个匹配对象();

				newobj.中心第一根类 = newobj.中心对象.中心第一根类;

				return newobj;
			}
		}

		public void 同步解析结果到结果表()
		{
			if (结果Row == null)
			{
				结果Row = Data.patternDataSet.模式结果.New模式结果Row();
				结果Row.ID = this.ID;
				Data.patternDataSet.模式结果.Add模式结果Row(结果Row);
			}

			结果Row.字符串 = 是无形式空对象 ? (string)模式行.形式 : 源语言串.Substring(begindex, endindex - begindex);
			结果Row.ObjectID = 模式行ID;
			结果Row.序号 = begindex;
			结果Row.打分 = 显示打分串();
			if (中心对象 != null)
			{
				if (中心对象.结果Row == null)
					中心对象.同步解析结果到结果表();
				结果Row.ParentID = 中心对象.结果Row.ID;
			}

			if (模式行 == null)
				return;

			//if (this.语言角色 != 字典_语言角色.无)//把计算好的语言角色对记录行进行设置。
			模式行.语言角色 = this.语言角色;
		}

		public void 加入各关联参数行到界面(模式 parentrow)
		{
			if (关联参数集合 == null)
				return;
			foreach (参数 参数 in 关联参数集合)
				Data.加入参数(parentrow, 参数.源关联记录, begindex);
		}


	}

	/// <summary>
	/// 公共语言结构
	/// 这个结构里边存储的就是理解以后的语言信息
	/// </summary>
	public class CommonLanageStruct
	{
		public static CommonLanageStruct cs;
	}

	public struct 生长对象引用
	{
		public int 排序号;
		public 生长对象 对象;
		public 生长对象引用(生长对象 obj, int 排序号参数)
		{
			排序号 = 排序号参数;
			对象 = obj;
		}
	}

	public class 选择结果
	{
		public List<生长对象引用> 选入的结果成员;//这里边是假定是正确的生长对象，也就是下步工作的假设前提，链表次序就是生成的次序，可以回溯。这些对象在分析任务中形成根对象。
		public int 包含字符串个数;
		public byte[] 源串位码;//用这个来计算哪些字符已经被长成，哪些是空着的。
		public int 未完成字符数;
		public int 长度分;
		public int 类型分;

		public 选择结果()
		{
			源串位码 = new byte[Data.当前句子串.Length];
			未完成字符数 = Data.当前句子串.Length;
		}

		public bool 可以加入(生长对象 obj)
		{
			//层级是-2的，一般是中间虚拟的局部对象，不能加入结果，而是它们的外围对象才能加入结果。
			if (obj.处理轮数 == -2)
				return false;
			if (obj.endindex <= obj.begindex)//空对象不考虑加入
				return false;
			for (int i = obj.begindex; i < obj.endindex; i++)
			{
				if (源串位码[i] != 0)
					return false;
			}
			return true;
		}

		public int 下一结果以及回溯()
		{
			生长对象引用 obj = 选入的结果成员.Last();
			for (int i = obj.对象.begindex; i < obj.对象.endindex; i++)
				源串位码[i] = 0;
			未完成字符数 += obj.对象.endindex - obj.对象.begindex;
			if (Data.字符串Guid.Equals(选入的结果成员.Last().对象.模式行.B端))
				包含字符串个数--;
			选入的结果成员.RemoveAt(选入的结果成员.Count() - 1);
			return obj.排序号 + 1;
		}

		public void 加入一个匹配对象(生长对象 obj, int 排序号)
		{
			for (int i = obj.begindex; i < obj.endindex; i++)
				源串位码[i] = 1;
			未完成字符数 -= obj.endindex - obj.begindex;

			生长对象引用 newobj = new 生长对象引用(obj, 排序号);
			选入的结果成员.Add(newobj);
			if (Data.字符串Guid.Equals(obj.模式行.B端))
				包含字符串个数++;
		}
		public void 完成并选入未匹配字符串()
		{
			if (未完成字符数 == 0)
				return;
			int 未匹配串开始位置 = -1;
			for (int i = 0; i <= Data.当前句子串.Length; i++)
			{
				if (i == Data.当前句子串.Length || 源串位码[i] == 1)
				{
					//空串结束。
					if (未匹配串开始位置 != -1)
					{
						SubString str = new SubString(未匹配串开始位置, i);
						模式 字串row = Data.增加字符串生长素材(str);
						生长对象 串对象 = new 生长对象(字串row, 0);
						Processor.当前处理器.加入一个对象到池(串对象);
						加入一个匹配对象(串对象, 选入的结果成员.Count);
					}
					未匹配串开始位置 = -1;
					continue;
				}
				if (未匹配串开始位置 == -1)
					未匹配串开始位置 = i;
			}
		}

		public int 熵值()
		{
			return 选入的结果成员.Count + 包含字符串个数;
		}

		public void 合并选入成员打分()
		{
			foreach (生长对象引用 obj in 选入的结果成员)
				合并(obj.对象);
		}

		public void 合并(生长对象 v)
		{
			长度分 += v.长度分;
			类型分 += v.类型分;
		}
	}
	public class 模式匹配结果节点
	{
		public 模式 匹配到的模式;
		public List<生长对象遍历节点> 匹配对象集合;

		public 模式匹配结果节点(模式 匹配模式)
		{
			匹配到的模式 = 匹配模式;
			匹配对象集合 = new List<生长对象遍历节点>();
		}
	}
	//这个类有两种作用，首先是作为【this属于**】的概念对象，然后这样的概念对象里边的【前边已完成关联对象】里边存储了该概念对象对应的关联。
	//这些关联是和前边已经出现过的概念连接的，总是后边的概念连接前边的。
	public class 生长对象遍历节点
	{
		public 生长对象 模板对象;
		public 模式 匹配出的答案对象;
		public Guid 关联ID;
		public 参数树结构 派生树;//用来进行结果匹配的回溯。
		public List<生长对象遍历节点> 前边已完成关联对象;
		public int 另一端的索引 = -1;
		public int that;
		//构造一个概念对象。
		public 生长对象遍历节点(生长对象 概念对象)
		{
			模板对象 = 概念对象;
			模板对象.匹配遍历对象 = this;
		}
		public 生长对象遍历节点(生长对象 概念对象, 模式 答案对象, Guid 关联)
		{
			模板对象 = 概念对象;
			模板对象.匹配遍历对象 = this;
			匹配出的答案对象 = 答案对象;
			关联ID = 关联;
		}
		//构造一个关联对象。
		public 生长对象遍历节点(生长对象 关联对象, int 索引, int that)
		{
			模板对象 = 关联对象;
			另一端的索引 = 索引;
			this.that = that;
			模板对象.匹配遍历对象 = this;
		}
		public override string ToString()
		{

			return 模板对象.ToString() + "派生概念：" + (派生树 == null ? 0 : 派生树.Count()) + "关联个数：" + (前边已完成关联对象 == null ? 0 : 前边已完成关联对象.Count());
		}
		public void 加入关联(生长对象遍历节点 obj)
		{
			if (前边已完成关联对象 == null)
				前边已完成关联对象 = new List<生长对象遍历节点>();
			前边已完成关联对象.Add(obj);
		}
	}

	/// <summary>
	///这个类就是一个分析任务，加入到这个任务中的对象是假设肯定成立的，因此，相互之间没有冲突
	///并且，已经加入任务的对象作为假设前提，对接下来要加入的对象要进行过滤甄别。
	///如果有多种解释的话，就会有多个任务会完成，最后进行比较。
	///现在看来没有必要，直接合并到模式中去好了。
	/// </summary>
	//public class 结果完成树
	//{
	//    //public 结果完成树 父任务;//任务如果有分支就这样做，这样的话可以复制一个任务并创建一个新的分支，来得到不同的结果。

	//    //public TargetStruct 源数据;

	//    public string 源语言串;

	//    public List<生长对象引用> 选择结果;//这里边是假定是正确的生长对象，也就是下步工作的假设前提，链表次序就是生成的次序，可以回溯。这些对象在分析任务中形成根对象。

	//    public char[] 源串位码;//用这个来计算哪些字符已经被长成，哪些是空着的。

	//    public int 未完成字符数;

	//    //public int[] 回溯栈;似乎可以不用这个，已经选择结果就是。

	//    public 结果完成树(string 源串)
	//    {
	//        源语言串 = 源串;
	//        //源数据 = p;
	//        源串位码 = new char[源串.Length];
	//        未完成字符数 = 源串.Length;
	//        选择结果 = new List<生长对象引用>();
	//    }

	//    public bool 可以加入(生长对象 obj)
	//    {
	//        for (int i = obj.begindex; i < obj.endindex; i++)
	//        {
	//            if (源串位码[i] != '\0')
	//                return false;
	//        }
	//        return true;
	//    }

	//    public int 回退一级()
	//    {
	//        生长对象引用 obj = 选择结果.Last();
	//        for (int i = obj.对象.begindex; i < obj.对象.endindex; i++)
	//            源串位码[i] = '\0';
	//        未完成字符数 += obj.对象.endindex - obj.对象.begindex;
	//        选择结果.RemoveAt(选择结果.Count() - 1);
	//        return obj.排序号 + 1;
	//    }

	//    public void 加入一个匹配对象(生长对象 obj,int 排序号)
	//    {
	//        for (int i = obj.begindex; i < obj.endindex; i++)
	//            源串位码[i] = '1';
	//        未完成字符数 -= obj.endindex - obj.begindex;

	//        生长对象引用 newobj = new 生长对象引用(obj,排序号);
	//        选择结果.Add(newobj);
	//    }

	//    public void 加入到结果表()
	//    {
	//        PatternDataSet.模式结果DataTable table = Data.patternDataSet.模式结果;
	//        if (选择结果.Count() == 0)
	//            return;
	//        PatternDataSet.模式结果Row parentrow = table.New模式结果Row();
	//        parentrow.ID = Guid.NewGuid();
	//        parentrow.字符串 = 选择结果[0].对象.模式行.形式;
	//        table.Add模式结果Row(parentrow);
	//        //int i = 1;
	//        foreach (生长对象引用 obj in 选择结果)
	//        {
	//            PatternDataSet.模式结果Row row = table.New模式结果Row();
	//            row.ID = Guid.NewGuid();
	//            row.字符串 = obj.对象.模式行.形式;
	//            row.ParentID = parentrow.ID;
	//            row.序号 = obj.排序号;
	//            //row.序号 = i++;
	//            table.Add模式结果Row(row);
	//        }
	//    }

	//    //public List<生长对象> 挂起的未完成对象;//。
	//}

	/// <summary>
	/// 这个是生长对象库，他在运行中动态变化，增加和删除里边的内容。
	/// 生长对象库基本上不会随机的删除里边的内容，事实上，里边的内容应该是用栈一样的方法进行成批删除。
	/// 这样，生长对象也是一个栈，
	/// </summary>
	public class 生长对象库
	{
		public List<生长对象> instances = new List<生长对象>();
		public void Add(生长对象 对象)
		{
			instances.Add(对象);
		}
		public void Remove(生长对象 对象)
		{
			instances.Remove(对象);
		}
		/// <summary>
		/// 根据创建者来删除其中的模式实例
		/// 创建者一般是一个方法
		/// </summary>
		/// <param name="创建者"></param>
		public void Popup(int 创建者)
		{
			foreach (生长对象 a in instances)
			{
				if (a.创建者 == 创建者)
					instances.Remove(a);
			}
		}
	}
	/*
	 * 语境
	 * 这个类比较重要，把一种整体的全局的参数都压在里边，并且用栈来做
	 */
	public struct ContextEntironment
	{
		public Stack<int> sourcelanager;//源语言
		public Stack<int> targetlanager;//目标语言
		public Stack<int> target;//目标
		public Stack<int> addressor;//说话者
		public Stack<int> listener;//听者
	}

	/// <summary>
	/// 这个其实就是语言模式的库
	/// </summary>
	//public class 模板_语料串 : IComparable
	//{
	//    public Guid ID;
	//    public string 字符串;
	//    public bool 是抽象串;
	//    public 模板_语料串 所属抽象串;
	//    public Guid 所属抽象串id;
	//    public int 语言;
	//    public int 语义数目;//这个字符串对应有多少个语义，这个值是一个为了优化的值，因为是语言去记录对应的语义，所以，这个值可以根据语义进行统计过来，但这里使用频繁，所以这个值进行计算并保存在这里，这样就比较方便了。
	//    public Guid 同义词;
	//}

	public class 疑问概念结构
	{
		public Guid 疑问概念;
		public Guid 类型概念;
		public Guid 单位概念;
		public 疑问概念结构(Guid 疑问, Guid 类型, Guid 单位)
		{
			疑问概念 = 疑问;
			类型概念 = 类型;
			单位概念 = 单位;
		}
	}

	public class 代词类型结构
	{
		public Guid 代词;
		public Guid 代表类型;
		public 代词类型结构(Guid 代词, Guid 代表类型)
		{
			this.代词 = 代词;
			this.代表类型 = 代表类型;
		}
	}

	public class 派生类判断结果
	{
		public Guid 模式行ID;
		public Guid 派生类ID;
		public bool 结果;
		public 派生类判断结果(Guid 基类ID, Guid 模式ID, bool 是否派生)
		{
			派生类ID = 基类ID;
			模式行ID = 模式ID;
			结果 = 是否派生;
		}
	}
	static public class Data
	{
		static public Guid 系统对象Guid = new Guid("986c8942-7f1c-4a6e-952d-5b0b4695aa3d");
		static public Guid 形式Guid = new Guid("ef19ae22-8d31-449b-a6e3-abd4e6d76288");
		static public Guid 概念Guid = new Guid("b8511915-a385-4cb8-8e59-96809967207f");
		static public Guid 变量Value = new Guid("a87c576d-d634-4a5b-b6a7-1ed7c679feba");
		static public Guid 代词Guid = new Guid("66de4757-a495-438f-b85c-5adb176d0dc9");
		static public Guid 事物概念Guid = new Guid("e72e2050-89fe-4efc-bcff-d5c39ea8819e");
		static public Guid 实体概念Guid = new Guid("ae58bbd9-c30f-49ea-b47b-2c0ec7d41cf6");
		static public Guid 值概念Guid = new Guid("da51c50d-4e1b-4421-a818-96090e068918");
		static public Guid 时间段Guid = new Guid("a61c428c-092b-4b9e-836c-dc9ef723e7e8");
		static public Guid 相对空间Guid = new Guid("e09c332f-d6b9-42fa-b340-978580c33005");
		static public Guid 封闭范围Guid = new Guid("b21b52e4-9e9b-4a06-aaff-f9a8e41456c4");
		static public Guid 度量Guid = new Guid("59d26397-1605-4d1c-9a21-6824c032954a");
		static public Guid 量Guid = new Guid("57b37e62-0467-4998-bf31-ddf887db936a");
		static public Guid 量化概念Guid = new Guid("59e10005-304a-48f9-9bb2-d45e68b274c4");
		static public Guid 基本量Guid = new Guid("52911bbd-621f-415a-8cc7-66aab945299b");
		static public Guid 次数量Guid = new Guid("80b3e853-8666-4c6d-84b1-eb5ebeb320f7");
		static public Guid 外延指定Guid = new Guid("374fd617-5353-42b5-bf78-68ddaf432a20");
		static public Guid 第3指定Guid = new Guid("c4aa362e-26d8-4948-97b9-1e49d261a0ce");
		static public Guid 每Guid = new Guid("bc0a3962-2346-490d-af2e-f2be4a2e15ed");
		static public Guid 第Guid = new Guid("0be49c71-a9e2-43cd-9df7-28c6431d886b");
		static public Guid 抽象形式集合Guid = new Guid("fb55078b-5eda-4298-b224-e4d2878649b8");
		static public Guid 抽象形式集合拥有元素Guid = new Guid("ba88d650-d5f0-461b-bf41-41bc425f11eb");
		static public Guid 抽象形式集合拥有第一元素Guid = new Guid("b0e7dc69-cdd9-4a06-8f7c-3df77f14993c");
		static public Guid 抽象形式集合拥有后续元素Guid = new Guid("5cc76691-974f-4b35-9821-ca7dfd4131e5");
		static public Guid 形式集合Guid = new Guid("abae7980-a2df-414b-86ed-a2d2264fa380");
		static public Guid 并列集合Guid = new Guid("bd06eb06-a206-4183-a7b3-47f5267ec747");
		static public Guid 并列集合拥有后续元素Guid = new Guid("8a3a58e6-3e1a-4f95-918b-7c39c96d21eb");
		static public Guid 表达式Guid = new Guid("e4e3529d-7224-485a-9ba9-fae051dfbe24");
		static public Guid 除法表达式Guid = new Guid("d9b7a105-c717-4bb5-9419-563c5dbcc62a");
		static public Guid 除法表达式拥有被除Guid = new Guid("72d5d761-cbcd-4817-b83f-f8e781653e2c");
		static public Guid 除法表达式拥有除Guid = new Guid("712ce28e-6139-42cc-8beb-e804f086100f");
        static public Guid 减法表达式Guid = new Guid("45e91697-472b-430c-8a54-88bf8b3fa1cc");
        static public Guid 减法表达式拥有被减Guid = new Guid("378e2460-bc71-41a4-b61d-6281d6170e10");
        static public Guid 减法表达式拥有减Guid = new Guid("1729581b-8c60-4b7f-ac19-46fef84da3a7");
		static public Guid 方程式Guid = new Guid("60408a18-137f-4076-b514-25928b286ca6");
		static public Guid 路径区间Guid = new Guid("8b210b15-a6a0-44d7-bb3c-1cca2123d862");
		static public Guid 语篇句Guid = new Guid("5946a885-bc92-40df-92ba-87ca5b4a1256");
		static public Guid 模板Guid = new Guid("e5104675-9592-4caf-b7b6-e3270658d656");
		static public Guid 属于Guid = new Guid("10B394F0-AF79-46F4-94E9-7C11D824B3F4");
		static public Guid 包括Guid = new Guid("95e7211e-c9ea-4db2-aa85-8a4b0e9d1744");
		static public Guid 拥有Guid = new Guid("582FA45E-AF28-4F45-81FC-9E55AEDF7659");
		static public Guid 概念拥有角色Guid = new Guid("565dc81f-4978-416a-aac0-bb72b8b34d3d");
		static public Guid 并列关联Guid = new Guid("cf3e09b3-c074-4c19-a5d1-451ae0bfc16a");
		static public Guid 松散并列Guid = new Guid("2e751327-822c-4e57-bf85-5fe97a600286");
		static public Guid 代理拥有Guid = new Guid("f45f5709-4529-44a7-b8c0-29ea467c9417");
		static public Guid 概念拥有属性Guid = new Guid("a72916f8-6916-4eb7-925b-76a083e740f9");
		static public Guid 概念拥有构成Guid = new Guid("f4ac0571-4031-4d84-8b33-c9508799a30f");
		static public Guid 概念拥有对象Guid = new Guid("dfe94d95-23ee-4740-898d-48d8ea53d8dc");
		static public Guid 概念拥有子名称Guid = new Guid("b47c236c-74f5-464c-968e-8c43d0cb01f7");
		static public Guid 概念拥有补语解释Guid = new Guid("AFE0FB71-E7B3-458B-A176-BAA52B46EE5C");
		static public Guid 属拥Guid = new Guid("23507985-a7d1-4c08-a205-b634321493d3");
		static public Guid 聚合Guid = new Guid("39c74178-1141-4a84-a27b-ca7ca455ab2e");
		static public Guid 反向聚合Guid = new Guid("86409280-cc49-49ec-bac5-64cbeb07130b");
		static public Guid 扮演Guid = new Guid("515cc3ed-4dfc-4323-802c-2809a18e2552");
		static public Guid 并列聚合Guid = new Guid("b946daee-e5f6-4722-8358-4d713c90e5fc");
		static public Guid ThisGuid = new Guid("5AD247E2-52C5-4911-B84B-29ED3BB5BA10");
		static public Guid Null事物 = new Guid("f6351da5-467b-4289-9b18-e22b3fb4d60f");
		static public Guid 模式Guid = new Guid("DA51C50D-4E1B-4421-A818-96090E068918");
		static public Guid 基本关联Guid = new Guid("2ED04C1B-8ED6-4BFD-B649-8CF507C8C7CE");
		static public Guid 独立关联Guid = new Guid("4b55c3ff-1971-43a1-9c8a-b4bfe1c061e3");
		static public Guid 关联A端Guid = new Guid("58e0c570-c684-45d4-a47f-68be7ae81d6e");
		static public Guid 关联B端Guid = new Guid("6d4a1c69-3e93-4c75-ae9f-7bf59f6d91f5");
		static public Guid 等价Guid = new Guid("eec51554-e3f5-4bfd-bcd7-b8afe3cb39a4");
		static public Guid 同一Guid = new Guid("b63830a5-bda2-4327-9c48-497292cd459b");
		static public Guid 会话组织Guid = new Guid("27893256-3a3a-403d-b6e2-f9aadef7b0c0");
		static public Guid 单句分析Guid = new Guid("8B295A9E-B15E-4E2C-94AC-3B0FFF6756E5");
		static public Guid 生长素材Guid = new Guid("6b47bd9a-87ff-42f4-b6ab-56b614e8ac3b");
		static public Guid 新对象Guid = new Guid("bd25ad8b-7ab3-4eac-ab38-71144fe68be9");
		static public Guid 事件Guid = new Guid("d1a6db72-daf0-457a-9369-bc149b4b6c7f");
		static public Guid 动作Guid = new Guid("90b1f374-c757-48ce-9bae-a9525243b220");
		static public Guid 人对事动作Guid = new Guid("d68b485e-1055-49e3-bbc5-56e43969444b");
		static public Guid 针对Guid = new Guid("70f0c2fd-2057-48a2-abb4-934710b09c6b");
		static public Guid 表达Guid = new Guid("fa34fd2a-eeab-4919-be43-9091a9632ed0");
		static public Guid 拥有形式Guid = new Guid("17F7463C-739D-4532-B655-306BAE32D535");
		static public Guid 拥有语言算子Guid = new Guid("77d610aa-c4bb-41e9-9a6f-5e1a76c91374");
		static public Guid 拥有语言角色Guid = new Guid("711498c1-e186-4aca-8a97-11dc4872d726");
		static public Guid 个人拥有名字Guid = new Guid("4a033bc2-3c55-41ea-94da-15310f4b27c9");
		static public Guid 概念拥有介词形式Guid = new Guid("677d14ed-a73b-43b5-bc0f-216c5d46f2be");
		static public Guid 概念拥有压缩形式Guid = new Guid("7468f252-3d5b-4906-9f23-a54e9ee7b63e");
		static public Guid 概念拥有定语形式Guid = new Guid("e671a08d-d751-4831-91e4-8a915e96f266");
		static public Guid 关联拥有前置关联Guid = new Guid("6aedf2e7-473c-4787-8857-b01fa4bd8389");
		static public Guid 关联拥有后置关联Guid = new Guid("ea5a9d0f-ad89-4ef2-81dc-46e6340c6f4d");
		static public Guid 关联拥有前置介词Guid = new Guid("54c0cd1e-1e10-4ca8-a346-3153bc8d1414");
		static public Guid 关联拥有介动词后生存Guid = new Guid("7bda9d58-ebce-4ca2-ba64-48c840addde5");
		static public Guid 关联拥有后置介词Guid = new Guid("76eb7888-54ef-44fc-bdbd-f6ed11ee1416");
		static public Guid 拥有后置附件Guid = new Guid("38d1e31a-5fc6-4017-aaa9-5f83a2d1c002");
		static public Guid 关联拥有的Guid = new Guid("6a48c8e1-264f-4f73-9af7-2dde956ee583");
		static public Guid 关联拥有地Guid = new Guid("d2b4183c-befa-4d09-b3f5-36c677cd831d");
		static public Guid 关联拥有A端Guid = new Guid("371fb02b-bdb4-4ced-a95f-7de3985dad78");
		static public Guid 关联拥有B端Guid = new Guid("900d0e08-317a-4f78-b510-611602e15171");
		static public Guid 关联拥有离合谓语Guid = new Guid("4245626a-5c4c-4d27-ab18-8082b3ac1dcf");
		static public Guid 关联拥有离合宾语Guid = new Guid("477da9ad-4b52-445d-8d4c-a9c09696f02f");
		static public Guid 动作拥有离合宾语动作Guid = new Guid("1344821c-bafa-48ba-a5aa-f8316ad5f799");
		static public Guid 拥有语言部件Guid = new Guid("10b81f15-d3dd-4bcf-b88e-9a1d07337a11");
		static public Guid 字符串Guid = new Guid("3313da19-d769-4c20-82c5-8d54e046d374");
		static public Guid 相对空间拥有原始空间Guid = new Guid("00a27ed5-447b-433f-828b-a1e1972d2511");
		static public Guid 事物属拥空间Guid = new Guid("5ac16ca5-77de-4afd-8ad5-b93eff5fa723");
		static public Guid 生存类型Guid = new Guid("110cc3b0-a0fb-463c-a084-0c252bc0be83");
		//static public Guid 通用介词Guid = new Guid("4760bd6d-c6bf-40d2-a2b2-a98df0452269");
		//static public Guid 专用介词Guid = new Guid("9a7d39a7-d212-416a-9930-71c68cbaad6d");
		static public Guid 介词Guid = new Guid("c59bf6a1-d7eb-4fc5-83d6-3d97fcbce2d0");
		static public Guid 前置介词Guid = new Guid("47e9d95c-83af-43d3-b271-e62038151aa3");
		static public Guid 后置介词Guid = new Guid("0f4627f8-c62e-4431-8571-7835072404a7");
		//static public Guid 集合介词Guid = new Guid("8bb968af-2ae5-46b7-8c9b-1412e0892ddd");
		//static public Guid 表达式介词Guid = new Guid("bef3428c-0490-43f2-b6e7-3064d4ae4b67");
		//static public Guid 推导介词Guid = new Guid("b9cc7084-0248-4e06-879d-2067dd0260e8");
		static public Guid NullGuid = new Guid("6B90684D-90C7-47BB-B3FB-B1B128D224A3");
		static public Guid NullParentGuid = new Guid("00000000-0000-0000-0000-000000000000");
		static public Guid 推导即命题间关系Guid = new Guid("26407db5-8549-44f8-accb-1739ce75a25d");//也就是命题之间的关系。
		static public Guid 基本推导组织Guid = new Guid("5ea47260-b938-4fb5-9d79-a29f575e5a8c");
		static public Guid 二元比较关系Guid = new Guid("c7a52ae6-9224-4dd6-87dd-20d70f4a12b7");
		static public Guid 推理角色Guid = new Guid("dd4e098a-cdd8-49f8-85af-d7ad5c06d02c"); //已更名为事件角色
		static public Guid 命题关系拥有前件Guid = new Guid("9d76be14-6e5b-49fb-afc9-c0947761fd34");
		static public Guid 命题关系拥有后件Guid = new Guid("12f234e3-bb6f-4664-8584-8c4fcc2cf91a");
		static public Guid 短句停顿Guid = new Guid("cc957d96-62c8-4e52-af86-21012255c8a5");
		static public Guid 逗号停顿Guid = new Guid("c73a6f2b-3247-4b48-a9f8-6781d539fad4");
		static public Guid 概念属拥短句停顿Guid = new Guid("c7872409-ba0d-45da-b050-affa069991f9");
		static public Guid 概念属拥句子Guid = new Guid("56ec7f34-57f5-43df-8855-4d30aec9943b");
		static public Guid 概念属拥括号Guid = new Guid("5d09d1a2-729d-4f43-8b70-5a27cd41c17a");
		static public Guid 语用基类Guid = new Guid("14e62f29-d95a-42e0-9d26-9ff788fabe72");
		static public Guid 句子语用基类Guid = new Guid("a673a7f8-04a9-466c-bb2c-4d00e0898102");
		static public Guid 下文引导冒号Guid = new Guid("10aa49e2-bb69-429e-9779-dc9b73e472ff");
		static public Guid 破折停顿符Guid = new Guid("9686BD4D-86EA-4C90-AB72-2C5286E0A281");
		static public Guid 拥有连动Guid = new Guid("e5d838e1-ca15-4480-bc9c-9e12c5239017");
		static public Guid 动词拥有紧密并列动词Guid = new Guid("7ff2d16e-bd1c-4e8f-b0e8-3d2fef8b1f24");
		static public Guid 拥有小品动词Guid = new Guid("5d4cde92-40f9-4e17-a6ad-175879a3aa51");
		static public Guid 拥有补语动词Guid = new Guid("36a91c2b-ec70-4df2-a9d1-d64475259482");
		static public Guid 什么Guid = new Guid("144dfec4-b765-4cef-a78e-0da26b000e06");
		static public Guid 什么事物Guid = new Guid("98618409-f07e-48c1-a95d-3e90f0685d85");
		static public Guid 什么量Guid = new Guid("2634db0b-5198-44e5-92af-d01891bad977");
		static public Guid 是否Guid = new Guid("0b70cefa-ee93-40d7-8e59-4e48cd5bbb0e");
		static public Guid 关系拥有成立疑问Guid = new Guid("6fabd400-3474-45df-a883-4ef11aca8b53");
		static public Guid 符合程度Guid = new Guid("5c2e454b-4892-4efe-9c29-a43889cdc60d");
		static public Guid 概念属拥疑问性Guid = new Guid("961da15b-93ff-485a-b5d5-69da0bf59ed9");
		static public Guid 路径引用Guid = new Guid("300c65e3-0e0c-49c4-bcf8-812d8aef1ef0");
		static public Guid 引用Guid = new Guid("15627065-4789-47e8-a571-9b78f39616f6");
		static public Guid 模板引用Guid = new Guid("3040ab52-436a-42d3-b950-fc844ae8d22a");
		static public Guid 疑问句Guid = new Guid("bffa62d5-bbd2-4b6c-ae30-b01af46337c3");
		public static Guid 一般疑问句Guid = new Guid("06d55dc8-b1ee-4eba-84ed-d757c01eb3e1");
		static public Guid 陈述句Guid = new Guid("144ea7e9-2e83-4d0f-bcb0-c0b039877d9b");
		static public Guid 定性Guid = new Guid("80f763e7-0292-4e5c-8194-bb1bc5e52064");
		static public Guid 定性量Guid = new Guid("0d590318-1f98-4e93-8376-3c908abc0729");
		static public Guid 生存阶段Guid = new Guid("84c39c1f-dc1c-4022-9acf-120e97fb64e0");
		static public Guid 事物属拥存在阶段Guid = new Guid("87f5d17e-3c46-4cae-bd3c-90a04123143f");
		static public Guid 关系属拥成立阶段Guid = new Guid("13c0a3ee-a3ca-4f43-8304-bdfd30d59a08");
		static public Guid 事物属拥存在类型Guid = new Guid("1e8046ce-d917-4028-9d57-e634d295b55b");
		static public Guid 关系属拥成立类型Guid = new Guid("e6d33f5d-1788-4fd7-87f1-328ae8c294cb");
		static public Guid 事件拥有主体属主Guid = new Guid("fec4691a-9535-448a-81b1-220598d34115");//实际二级主语
		static public Guid 事件属拥被动Guid = new Guid("a1f70061-0332-4bda-9652-30f00be2c09f");
		static public Guid 表达拥有内容Guid = new Guid("95eeb777-47a2-444c-b842-b09a7569d682");
		static public Guid 单个结果Guid = new Guid("d1c946e0-5723-4ea8-b047-54c6955beea4");
		static public Guid 量拥有倍数Guid = new Guid("301e5137-1403-427f-987a-2aedc90f0315");
		static public Guid 量化概念聚合量Guid = new Guid("6ce6ce08-8ce3-4399-b7ff-ae9a119709d8");
		static public Guid 情绪Guid = new Guid("9768e031-3bc7-4291-ab13-779acf5790ad");
		static public Guid 事件反聚推理角色Guid = new Guid("86e0da46-487f-4066-91a1-611f211ef1d7");
		static public Guid 推导拥有推理角色Guid = new Guid("79134c26-a6c4-475c-8f39-199fdb1de1e3");
		static public Guid 物动作Guid = new Guid("40aeadb9-2a0a-4692-bd4b-ded14e123a17");
		static public Guid 人单动作Guid = new Guid("a4ab8d3c-dc59-44d1-b8de-a32d07c73b06");
		static public Guid 信息媒介Guid = new Guid("0443c541-1f51-4e7f-8c75-f4bd6831f9d9");
		//static public Guid 主被介词Guid = new Guid("0bbcf7e6-af53-4a96-8d97-48a1b2d150aa");
		//static public Guid 被主介词Guid = new Guid("2ba09d9f-1c11-4249-811a-30db062edb26");
		//static public Guid 把宾介词Guid = new Guid("4b150ff4-f478-4715-a399-56ab2d9a60f0");

		static public Guid 完成Guid = new Guid("0dcafc80-6b8a-4e79-bad3-77b5757d3cab");
		static public Guid 进行Guid = new Guid("c3223ea7-5282-4288-910c-9c37c46a42ed");
		static public Guid 过去完成Guid = new Guid("6e8230c9-3bf9-4a4d-accb-b39db4de0ff3");


		static public Guid 语言算子Guid = new Guid("b6f682ef-93ae-4d4b-b5e4-45604c4987c9");
		static public Guid 人称算子Guid = new Guid("372C2899-E64F-44D2-BC4C-04C06D65F33A");
		static public Guid 算子第一人称Guid = new Guid("4df633cb-a19f-4f03-b2de-9ebc0a4c073f");
		static public Guid 算子第二人称Guid = new Guid("ddaba87d-d8e1-45d4-9121-d293688f9be5");
		static public Guid 算子第三人称Guid = new Guid("f3193f9b-82e5-4192-802b-e46ccaf8dc89");
		static public Guid 算子人称单数Guid = new Guid("b167ab92-8d63-4a6d-9b1c-b53f78f65433");
		static public Guid 算子人称复数Guid = new Guid("f849764f-785e-464f-99cf-776d4b0fa78d");
		static public Guid 算子不定冠词Guid = new Guid("a42ff147-2245-479c-adf0-8fa385bba418");
		static public Guid 单复算子Guid = new Guid("BDE7E791-0313-4DAD-B238-8E4A459A270F");
		static public Guid 算子单数Guid = new Guid("0a9a5cbb-f033-4c60-a57f-dee52e183b0e");
		static public Guid 算子复数Guid = new Guid("63ffcb67-caa3-4840-bfa3-784aae52eed8");
		static public Guid 时态算子Guid = new Guid("7E268F9E-E5C3-4817-BCE9-193F4B56D455");
		static public Guid 算子时态状态一般Guid = new Guid("ca12686e-3f2a-46c7-8356-0c49872fb3e9");
		static public Guid 算子时态状态进行Guid = new Guid("4ea979ca-85ce-4f66-95aa-44ca5697337b");
		static public Guid 算子时态状态完成Guid = new Guid("17bad338-31c6-4fee-b09d-ff602daf4f09");
		static public Guid 算子时态状态完成进行Guid = new Guid("43a16c51-990d-4036-b267-fcd545563f55");
		static public Guid 算子时态时间现在Guid = new Guid("24b31557-3792-4109-8150-d705214ca617");
		static public Guid 算子时态时间过去Guid = new Guid("6ea936a0-b735-41fc-9417-402f00e59750");
		static public Guid 算子时态时间将来Guid = new Guid("d76c9394-f056-4f2a-acb0-81be789c6339");
		static public Guid 算子时态时间过去将来Guid = new Guid("2c286101-d5e5-4b80-b1a4-e97cd2cf4ee7");
		static public Guid[] 时态和人称算子集合 = new Guid[] {算子第一人称Guid, 算子第二人称Guid, 算子第三人称Guid, 算子人称单数Guid, 算子人称复数Guid, 
                                                 算子时态时间过去Guid, 算子时态时间过去将来Guid, 算子时态时间将来Guid, 算子时态时间现在Guid,
                                                 算子时态状态进行Guid, 算子时态状态完成Guid, 算子时态状态完成进行Guid, 算子时态状态一般Guid};

		static public Guid 比较级算子Guid = new Guid("8fbc6f0a-e7fe-47ed-b3ba-3a41cf4f3497");
		static public Guid 最高级算子Guid = new Guid("c3ac2498-9fb1-45f8-b773-691b7eb8fee9");

		static public Guid 谁Guid = new Guid("c087890c-2956-4913-8b00-45cf7e2dc073");
		static public Guid 人Guid = new Guid("9949f24f-2bed-4a33-8b90-d45a90b86b19");
		static public Guid 多少时间Guid = new Guid("1fa42c1b-4bab-4ee3-b46a-93e7011f784d");
		static public Guid 什么时间Guid = new Guid("f640e444-53bd-48fc-b5e5-41aab37d6ad5");
		static public Guid 星期几Guid = new Guid("e6e7d380-5dc8-4c8e-a655-eb83d6830085");



		static public Guid 他Guid = new Guid("57564b36-a6a9-4128-894c-5d8a5cf525d2");
		static public Guid 她Guid = new Guid("1701ff78-80b2-4531-9749-e52da44b08c1");

		static public Guid 时间量Guid = new Guid("400bdce5-e80e-4a05-9c0c-bc597a331ac9");
		static public Guid 星期Guid = new Guid("77716183-0b78-4154-b9fd-4a741fbb05a5");
		static public Guid 年Guid = new Guid("5456c5f0-e6d9-4fb5-ba88-53750fe75af2");
		static public Guid 月Guid = new Guid("4d0ae55f-1ce7-4cf5-8323-c50a0a939a41");
		static public Guid 日Guid = new Guid("13f38377-9dba-4cdd-bc3f-d07b26590d5e");

		static public Guid 数Guid = new Guid("9003aeb9-5bb0-4858-bd44-d25cd596cfbf");
		static public Guid 日期Guid = new Guid("bcddd604-84b4-46bb-bce6-1b04e5c10a4e");
		static public Guid 时间点Guid = new Guid("303d2784-477e-49f1-851e-1c1ec442f2ef");   // TODO： 此处使用时间点， 还是时间段更合适？
		static public Guid 基准相对时间Guid = new Guid("22856867-97C4-4DE0-A3E0-179379E54CA2");
		static public Guid 组织Guid = new Guid("67488e76-abcc-4104-bde7-39d87494d290");
		static public Guid 地点Guid = new Guid("cef055d4-a40d-47b4-ac2e-aa320042a03e");
		static public Guid 项目编号Guid = new Guid("34734FD2-3B3C-450C-8F2B-66660810FD99");

		public static Guid 动作副词Guid = new Guid("1a29f443-e12f-4ce3-89e6-b3d3b30d1b8c");
		public static Guid 事物关系Guid = new Guid("d5b67e2a-8e2e-451b-8fe1-6f230e5b183a");
		public static Guid 定性概念量Guid = new Guid("e4ef9ab9-e50c-4a19-8523-283f58d23274");
		public static Guid 分类形容词Guid = new Guid("4107162a-66ad-4177-974b-e91fedaf44fc");
		public static Guid 量词个Guid = new Guid("b78b7005-000f-4ae7-ae28-73b696f0c3f6");
		public static Guid 未分类时间Guid = new Guid("5385b76d-72ed-46e1-9496-f9f9faf13721");
		public static Guid 未分类事物Guid = new Guid("a746f917-e583-4af4-9e3e-0a3304db034c");
		public static Guid 人对物动作Guid = new Guid("63679133-80b2-4c8a-a9e1-3100467783af");
		public static Guid 未分类单字动词Guid = new Guid("13dc0c7a-70ce-4891-8512-36efd5b0436c");
		public static Guid 未分类双字动词Guid = new Guid("bbc4bd76-dd5a-49f1-b2e7-4263d27aac57");
		public static Guid 公共新对象Guid = new Guid("B0486222-820D-4600-8BDA-09E9BF85D0F9");

		public static Guid 实体反聚项目编号Guid = new Guid("A6A28BAD-EE11-4679-84D3-F1084BBCE4F1");
		public static Guid 短句停顿符处理为介词Guid = new Guid("6ADEB4DB-8674-41E4-A297-AEB4D3C39CFC");
		public static Guid 到达Guid = new Guid("1DAFDBB9-229B-4FC9-8D59-AE189C97BB20");
		public static Guid 到达拥有终点Guid = new Guid("C65E42B5-765E-43A4-A03A-914139B1D79E");
		public static Guid 到达拥有起点Guid = new Guid("16400364-E943-464D-ABC6-5012C2AAFBB0");
		public static Guid 空间路径Guid = new Guid("C41B804B-2FEC-4143-88E1-FD60C6408F26");
		public static Guid 空间路径拥有终点Guid = new Guid("D8FB62CD-3085-4B7F-A18A-4348F525D6C9");
		public static Guid 空间路径拥有起点Guid = new Guid("4545E5E0-434C-40D6-BD78-03A6774F0ED8");
		public static Guid 情态动词Guid = new Guid("25E1077E-F095-4EED-9A89-CC471DBCFB42");

		public static Guid 人反聚人角色Guid = new Guid("ECB55D68-DAD4-4D76-9D5D-B03852107AD8");
		public static Guid 实体反拥习惯数词Guid = new Guid("DD318388-6933-4F5F-B242-2A07F9436076");
		public static Guid 人角色Guid = new Guid("FC379A01-97A8-48CD-B30D-C00BF1194855");
		public static Guid 集合省略Guid = new Guid("4C33E2D5-D110-4905-B578-3BBA835C2F0F");
		public static Guid 比较结果Guid = new Guid("474C4F6F-9AFF-46D9-90AD-6A798EA7E3C1");
		public static Guid 比较结果拥有比较标准Guid = new Guid("9E518EF0-D5EC-4679-9D29-F21C5868F943");
		public static Guid 比较结果拥有B方Guid = new Guid("205AA0A9-10B8-49E1-9403-106197C0DEED");
		public static Guid 比较标准Guid = new Guid("F688303E-31ED-401E-9320-596633BE7C16");
		public static Guid 人对人动作Guid = new Guid("CF5FEF79-507E-4F97-9D57-BB254D4D1EE7");
		public static Guid 抽象移动Guid = new Guid("67ADB660-744E-4DEE-A51A-3D73D933D2F6");
		public static Guid 事件拥有时间量Guid = new Guid("2D38F4D0-BDC5-45C5-AD5A-D2DC675FF972");
		public static Guid 事物拥有什么疑问Guid = new Guid("447387FF-D8A6-4C83-8F49-2E0D3E11B489");
		public static Guid 什么疑问算子Guid = new Guid("144DFEC4-B765-4CEF-A78E-0DA26B000E06");
		public static Guid 物品拥有主人Guid = new Guid("EE89C97A-ACFC-4A0C-A6E4-32E5AF1DDA59");
		public static Guid 抽象给Guid = new Guid("409E1629-7565-44F0-8329-78E46CDA724D");
		public static Guid 定性时间度Guid = new Guid("974D1B4C-A2E7-4510-AAF9-98AC1A06C134");
		public static Guid 事件拥有所在位置Guid = new Guid("067F6278-8920-4495-A801-5D364F0EF9A5");
		public static Guid 事物反聚空间位置Guid = new Guid("79717AF8-F946-4F8F-93AF-5C86A981E219");
		public static Guid 概念反拥符合程度Guid = new Guid("5E8B5A53-A872-454E-A83C-E64CC4CF8AF1");
		public static Guid 定性大小量Guid = new Guid("D8127843-483D-42DB-9785-C7A7A3638F9F");
		public static Guid 位置关系Guid = new Guid("92351388-BA0A-4417-AACE-BC7DB8D8637E");
		public static Guid 级别Guid = new Guid("7E9965EC-0790-40A5-B1DD-3DE69A75BFAE");
		public static Guid 方向Guid = new Guid("FFCBC1C5-4EAE-4109-8CB9-C559DAB7704C");
		public static Guid 疑问算子Guid = new Guid("b7bf2519-064e-4dea-a13a-573465d0a227");
		public static Guid 性别Guid = new Guid("53156892-a9a5-47cc-8fee-2bb44c7e2f38");
		public static Guid 范围集合拥有后续元素Guid = new Guid("086D7627-0F5E-4F75-BE5D-DDDB54DDA931");
		public static Guid 名称Guid = new Guid("CB4B74FD-D258-4AD6-8BC0-928126BDDDC5");
		public static Guid 名称拥有语言Guid = new Guid("DBCE4BF9-7BA8-4404-AD42-D22A44850688");
		public static Guid 汉语Guid = new Guid("1d7254a9-68f6-441c-83c7-4c712a1aad08");
		public static Guid 英语Guid = new Guid("e494b3ba-26ef-4d8d-821b-d15b116f8efb");
		public static Guid 层级概念Guid = new Guid("B4CC08B0-278D-4E42-B35A-4EDCE771971F");
		public static Guid 名词谓语形式是Guid = new Guid("39383961-e042-4306-a844-d5453b5ce0fb");
		public static Guid 名词谓语形式IsGuid = new Guid("b2dd7240-c110-4548-bfb1-8db4ad7381fa");
		public static Guid 角色拥有相对角色Guid = new Guid("f3431ce7-7e69-4323-9fcf-2993652597cc");
		public static Guid 特定时间Guid = new Guid("5385b76d-72ed-46e1-9496-f9f9faf13721");
		public static Guid 姓氏Guid = new Guid("02b18c70-49c5-4649-afc0-29fae131eff6");
		public static Guid 表达式拥有第一元素Guid = new Guid("04964911-0a33-4c45-8da1-1d6b8b399c0d");
		public static Guid 表达式拥有后续元素Guid = new Guid("c157affc-3211-4722-8281-bd764414453d");
		public static bool 允许新词汇自动处理 = false;
		public static bool 是调试模式 = true;
		static public 疑问概念结构[] 疑问变量结构 = new 疑问概念结构[]
		{
		    new 疑问概念结构(什么事物Guid,   事物概念Guid,   事物概念Guid ),
		    new 疑问概念结构(谁Guid,   人Guid,   人Guid ),
		    new 疑问概念结构(什么量Guid,   量Guid ,   量Guid),
		    new 疑问概念结构(多少时间Guid,   时间量Guid,   时间量Guid ),
		    new 疑问概念结构(什么时间Guid,   时间量Guid,   时间量Guid ),
		    new 疑问概念结构(星期几Guid,   时间量Guid,   星期Guid ),
		    new 疑问概念结构(疑问句Guid,   陈述句Guid,   陈述句Guid ),
            new 疑问概念结构(事物拥有什么疑问Guid,   基本关联Guid,   基本关联Guid ),
            new 疑问概念结构(什么疑问算子Guid,   实体概念Guid,   实体概念Guid )
		};

		static public 代词类型结构[] 代词类型结构 = new 代词类型结构[]
		{
		    new 代词类型结构(他Guid,   人Guid ),
		    new 代词类型结构(她Guid,   人Guid ),
		};

		public static MainForm mainfrm;
		public static 对话Form 对话frm;
		public static 模式Form 模式frm;

		public static int 当前生成语言 = 字典_语言.汉语;
		public static int 当前解析语言 = 字典_语言.汉语;

		public const int 无效序号 = -99999999;

		public const int 正向关联 = 1;
		public const int 反向关联 = 2;

		public const int 基类实现 = 1;
		public const int 派生实现 = 2;

		static public PatternDataSet patternDataSet;

		static public List<Guid> DataIDS = new List<Guid>();

		static public HashSet<Guid> 一级基本关联集合 = new HashSet<Guid>();
		static public HashSet<Guid> 所有基本关联集合 = new HashSet<Guid>();
		static public HashSet<Guid> 拥有形式集合 = new HashSet<Guid>();
		//static public HashSet<string> 通用介词集合 = new HashSet<string>();
		//static public HashSet<string> 专用前置介词集合 = new HashSet<string>();
		//static public HashSet<string> 专用后置介词集合 = new HashSet<string>();
		//static public HashSet<string> 集合介词集合 = new HashSet<string>();
		//static public HashSet<string> 表达式介词集合 = new HashSet<string>();
		//static public HashSet<string> 推导介词集合 = new HashSet<string>();
		static public HashSet<string> 前置介词集合 = new HashSet<string>();
		static public HashSet<string> 后置介词集合 = new HashSet<string>();
		static public List<推导> 推导集合 = new List<推导>();
		static public List<推理角色> 推理角色集合 = new List<推理角色>();

		//static private Dictionary<模式, 参数树结构> 属于基类树缓存 = new Dictionary<模式, 参数树结构>();
		static private Dictionary<模式, 参数树结构> 属于和拥有树缓存 = new Dictionary<模式, 参数树结构>();
		//static private Dictionary<模式, 参数树结构> 属于基类树关联缓存 = new Dictionary<模式, 参数树结构>();
		static private Dictionary<模式, 参数树结构> 属于和拥有树关联缓存 = new Dictionary<模式, 参数树结构>();
		//static public Dictionary<string, DataRow[]> 模式表Select缓存 = new Dictionary<string, DataRow[]>();

		public static List<模式> 语言模板集合 = new List<模式>();

		static public Guid 当前会话;
		static public 模式 当前句子Row;
		static public 模式 当前素材Row;
		static public 模式 当前新对象Row;
		static public string 当前句子串;
		static public int 计数器;
		static public bool 动态绑定至Form = true;
		static public bool 检查新知识 = false;
		//public static int 实体语料串数;

		public static List<命名实体> 当前命名实体集合 = new List<命名实体>();

		public static int 全局序号;

		public static Dictionary<Guid, Object> 对象缓存 = new Dictionary<Guid, Object>();

		public static object _clipObject;

		public static bool 简略回答; public static bool 解释和推导;

		public static readonly 模式基表 模式表 = null;
		public static readonly 模式基表 模式编辑表 = null;
		public static readonly 模式基表 模式会话表 = null;
		public static int 模式会话总数 = 0;
		public static List<形式化语料串> 语料库 = new List<形式化语料串>();
		public static List<形式化语料串> 外围语料库 = new List<形式化语料串>();
		public static Knowledge 外围知识库 = null;
		public static long timeCount = 0;
		static Data()
		{
			模式表 = new 模式基表("模式");
			模式编辑表 = new 模式基表("模式编辑");
			模式会话表 = new 模式基表(null);

			foreach (模式 item in 模式表.对象集合.Union(模式编辑表.对象集合))
			{
				添加端索引项(item);
			}

			模式表.TableChangeEvent += 模式基表_TableChangeEvent;
			模式编辑表.TableChangeEvent += 模式基表_TableChangeEvent;
			模式会话表.TableChangeEvent += 模式基表_TableChangeEvent;
			初始化模式可见性();
			语言模板集合.AddRange(模式表.对象集合.Where(r => r.连接 == Data.模板引用Guid && r.语言 != 字典_语言.语义));
		}

		public static void 初始化模式可见性()
		{
			// 初始化不可见模式根集合
			var invisiblePatternIds = new Guid[] 
            { 
                Data.未分类事物Guid,
                Data.物动作Guid,
                Data.人单动作Guid,
                Data.人对人动作Guid,
                Data.人对物动作Guid,
                Data.人对事动作Guid,
                Data.定性概念量Guid,
                Data.分类形容词Guid,
                Data.动作副词Guid,
                Data.事物关系Guid,
                //Data.拟声Guid,
                //Data.叹词Guid
            };

			foreach (var item in invisiblePatternIds)
			{
				设置模式树一般可见性(item, false, true);
			}
		}

		public static void 设置模式子树全部可见性(模式 根Pattern, bool isVisible)
		{
			foreach (var item in 根Pattern.端索引表_Parent)
			{
				设置模式子树全部可见性(item, isVisible);
				item.Bounding = isVisible;
			}
		}

		/// <summary>
		/// 设置模式树可见性
		/// </summary>
		/// <param name="根id">模式ID</param>
		/// <param name="isVisible">是否UI可见</param>
		/// <param name="isSubRecursive">是否递归子节点</param>
		public static void 设置模式树一般可见性(Guid 根id, bool isVisible, bool isSubRecursive)
		{
			模式 根Pattern = Data.FindRowByID(根id);
			if (根Pattern == null)
				return;
			根Pattern.Bounding = isVisible;

			var children = 根Pattern.端索引表_Parent.OrderBy(r => r.序号);
			int i = 0;
			foreach (var item in children)
			{
				item.Bounding = isVisible;
				i++;
				if (isVisible)
				{
					if (i > 50)
					{
						break;
					}

					if (isSubRecursive)
					{
						设置模式树一般可见性(item.ID, true, true);
					}
				}
				else
				{
					设置模式树一般可见性(item.ID, false, true);
				}
			}

			if (isVisible)
			{
				var parentID = 根Pattern.ParentID;
				while (true)
				{
					if (parentID == Guid.Empty)
					{
						break;
					}
					var parentPattern = Data.FindRowByID(parentID);
					parentPattern.Bounding = true;
					parentID = parentPattern.ParentID;
				}
			}
		}

		public static void 设置模式树查询可见性(Guid 根id, bool isVisible, bool isSubRecursive)
		{
			var 根Pattern = Data.FindRowByID(根id);
			根Pattern.isSearching = isVisible;

			var children = 根Pattern.端索引表_Parent.OrderBy(r => r.序号);
			int i = 0;
			foreach (var item in children)
			{
				item.isSearching = isVisible;
				i++;

				if (isVisible)
				{
					if (i > 50)
					{
						break;
					}

					if (isSubRecursive)
					{
						设置模式树查询可见性(item.ID, true, true);
					}
				}
				else
				{
					设置模式树查询可见性(item.ID, false, true);
				}

				item.isSearching = isVisible;
			}

			if (isVisible)
			{
				var parentID = 根Pattern.ParentID;
				while (true)
				{
					if (parentID == Guid.Empty)
					{
						break;
					}
					var parentPattern = Data.FindRowByID(parentID);
					parentPattern.isSearching = true;
					parentID = parentPattern.ParentID;
				}
			}
		}

		public static 聚合体对象及关联模式 根据对象查询关联模式行及聚合体对象(模式 发起对象, bool 加入形式, List<模式> 去除对象 = null)
		{
			聚合体对象及关联模式 聚合体对象及关联模式 = new 聚合体对象及关联模式();
			Data.递归加入关联模式行(聚合体对象及关联模式, 发起对象, 加入形式);
			if (去除对象 != null)
			{
				for (int index = 0; index < 聚合体对象及关联模式.所有关联.Count(); index++)
				{
					foreach (模式 模式 in 去除对象)
					{
						if (聚合体对象及关联模式.所有关联[index] == 模式)
						{
							聚合体对象及关联模式.所有关联.RemoveAt(index--);
							break;
						}
					}
				}
			}
			return 聚合体对象及关联模式;
		}

		static public 模式 创建附加关联(Guid 基关联, 模式 A端, 模式 B端)
		{
			模式基表 table = Data.FindTableByID(Data.当前素材Row.ID);
			模式 模式 = Data.CopyRow(Data.FindRowByID(基关联), true);
			模式.A端 = A端.ID;
			模式.B端 = B端.ID;
			模式.ParentID = Data.当前素材Row.ID;
			模式.连接 = 基关联;
			模式.形式 = "附加关联" + A端.形式 + "<=>" + B端.形式;
			table.新加对象(模式, true);
			Data.刷一条记录的根(模式);
			Data.递归设置语境树知识有效性(模式, 0);
			return 模式;
		}

		static public 模式 创建临时数字变量(string 数字)
		{
			模式 语义Row = null, 形式Row = null;

			Data.构建命名实体语义和形式模式行(Data.数Guid, 数字, 9, 字典_语言.公语, out 语义Row, out 形式Row);

			return 语义Row;
		}

		public static void reLoad()
		{
			//1.模式编辑表不需要重新加载，但必须清空模式编辑表中所有对象的端索引
			foreach (模式 item in 模式编辑表.对象集合)
			{
				item.端索引表_A端.Clear();
				item.端索引表_B端.Clear();
				item.端索引表_Parent.Clear();
				item.端索引表_源记录.Clear();
			}
			//2.重新加载模式表
			模式表.reLoad();
			//3.清空会话表
			模式会话表.reLoad();
			//4.重新生成端索引
			foreach (模式 item in 模式表.对象集合.Union(模式编辑表.对象集合))
			{
				添加端索引项(item);
			}
			初始化模式可见性();
			语言模板集合.Clear();
			语言模板集合.AddRange(模式表.对象集合.Where(r => r.连接 == Data.模板引用Guid && r.语言 != 字典_语言.语义));
		}
		public static void 加载外围语料库()
		{
			Knowledge.LoadAllWords(Data.外围语料库);
		}

		public static 模式 查找拥有形式模式行(形式化语料串 语料row, int 当前序号, ref int 外围对象总数)
		{
			模式 拥有形式模式Row = Data.FindRowByID(语料row.ObjectID);
			if (拥有形式模式Row == null)
			{
				if (!语料row.IsCore && Data.外围语料库.Any()) //外围库和语料如果和核心库的语料有重复时，此时有可能需重新建造其对应的模式对象
				{
					拥有形式模式Row = Data.查找和创建外围对象模式及其拥有形式模式行(语料row.ObjectID, 语料row.字符串, 当前序号, ref 外围对象总数);
					//语料row.ObjectID = 拥有形式模式Row.ID;
					//语料row = new 形式化语料串() { ObjectID = 拥有形式模式Row.ID, 字符串 = 拥有形式模式Row.形式, IsCore = false };
				}
			}

			return 拥有形式模式Row;
		}
		public static 模式 查找和创建外围对象模式及其拥有形式模式行(Guid 对象ID, string 形式串)
		{
			int i = 0;
			return 查找和创建外围对象模式及其拥有形式模式行(对象ID, 形式串, 0, ref i);
		}
		public static 模式 查找和创建外围对象模式及其拥有形式模式行(Guid 对象ID, string 形式串, int 当前序号, ref int 外围对象总数)
		{
			KeyValuePair<string, List<Guid>> 对象及其类型 = Knowledge.GetClassesOfObj(对象ID);

			// 构造根对象模式 
			string 对象名 = 对象及其类型.Key;
			List<Guid> 对象类型 = 对象及其类型.Value;
			if (对象类型 == null)
				return null;
			if (!对象类型.Any())
			{
				对象类型.Add(Data.事物概念Guid);  // 对未分配类型的对象，默认给定类型为事物
			}
			外围对象总数 = 对象类型.Count;
			//先在模式编辑表中的“当前新对象”节点下查找是否已经有相同的行式行
			模式 row = 从模式编辑表递归查找语义和形式模式行(对象类型[当前序号], 形式串, 字典_语言.公语);
			if (row != null)
				return row;
			//////////// data.字典
			//模式 根类对象 = Data.New派生行(对象类型[0], 字典_目标限定.A端);
			//根类对象.ID = Guid.NewGuid();//对象ID; 
			//根类对象.形式 = "[" + 对象名 + "]";
			//根类对象.ParentID = Data.当前新对象Row.ID;//Data.公共新对象Guid
			//Data.get模式编辑表().新加对象(根类对象);

			// 挂接拥有形式行
			List<KeyValuePair<string, 形式对象>> 串对象及其语言 = Knowledge.GetStrsOfObj(对象ID);
			模式 特定形式模式Row = null;
			// 挂接对象类型
			for (int i = 0; i < 对象类型.Count; i++)
			{
				模式 其他类对象 = Data.New派生行(对象类型[i], 字典_目标限定.A端);
				其他类对象.形式 = "[" + 对象名 + "]";
				其他类对象.ID = Guid.NewGuid();//对象ID; 
				其他类对象.ParentID = Data.公共新对象Guid; //Data.当前新对象Row.ID;//
				Data.get模式编辑表().新加对象(其他类对象);

				foreach (var item in 串对象及其语言)
				{
					string 串 = item.Key;
					int 语言 = item.Value.语言;

					模式 形式Row = Data.New派生行(Data.拥有形式Guid);
					形式Row.ParentID = 其他类对象.ID;
					形式Row.A端 = 其他类对象.ID;
					形式Row.B端 = Data.ThisGuid;
					形式Row.形式 = 串;
					形式Row.语言 = 语言;
					形式Row.语言角色 = 字典_语言角色.全部;
					if (string.IsNullOrEmpty(item.Value.参数) == false)
						形式Row.参数集合 = item.Value.参数;
					if (形式Row.形式.Equals(形式串) && 特定形式模式Row == null)
					{
						特定形式模式Row = 形式Row;
					}

					Data.get模式编辑表().新加对象(形式Row);
				}
			}
			return 特定形式模式Row;
		}

		//用于查询时，一些疑问模式需要进行替代后再进行模式匹配
		public static Guid 疑问模式行进行替代(Guid 模式行ID)
		{
			foreach (疑问概念结构 o in Data.疑问变量结构)
			{
				if (o.疑问概念.Equals(模式行ID))
				{
					return o.类型概念;
				}
			}
			return 模式行ID;
		}

		public static Guid 获取疑问变量的单位类型(Guid 模式行ID)
		{
			foreach (疑问概念结构 o in Data.疑问变量结构)
			{
				if (o.疑问概念.Equals(模式行ID))
				{
					return o.单位概念;
				}
			}
			return 模式行ID;
		}
		//判断指定模式Guid,是否为疑问替代对象
		public static bool 是疑问替代对象(Guid 模式行ID)
		{
			foreach (疑问概念结构 o in Data.疑问变量结构)
			{
				if (o.疑问概念.Equals(模式行ID))
				{
					return true;
				}
			}
			return false;
		}

		public static Guid 取得代词代表类型(Guid 代词)
		{
			foreach (代词类型结构 o in Data.代词类型结构)
			{
				if (o.代词.Equals(代词))
					return o.代表类型;
			}
			//Data.Assert(false);
			return Guid.Empty;
		}

		private static void 模式基表_TableChangeEvent(object sender, BaseTable<Guid, 模式>.TableChangeEventArgs<模式> args)
		{
			模式 item = args.Source;
			switch (args.ChangeOption)
			{
				case BaseTable<Guid, 模式>.TableChangeOption.AddNew:
					if (item.IsNew == true)
					{
						添加端索引项(item);
					}
					break;
				case BaseTable<Guid, 模式>.TableChangeOption.Updated:
					var otherInfo = args.OtherInfo as BaseObject<Guid>.PropertyChangedEventArgs;
					if (otherInfo != null)
					{
						更改端索引项(item, otherInfo);
					}
					break;
				case BaseTable<Guid, 模式>.TableChangeOption.Deleted:
					删除端索引项(item);
					break;
			}
		}

		private static void 更改端索引项(模式 item, BaseObject<Guid>.PropertyChangedEventArgs otherInfo)
		{
			switch (otherInfo.PropertyName)
			{
				case "A端":
					List<模式> 更改前A端模式端索引表 = Data.FindRowByID((Guid)otherInfo.OldValue).端索引表_A端;
					更改前A端模式端索引表.Remove(item);
					if (Data.IsThisOrNull((Guid)otherInfo.NewValue) == false)
					{
						List<模式> 更改后A端模式端索引表 = Data.FindRowByID((Guid)otherInfo.NewValue).端索引表_A端;
						更改后A端模式端索引表.Add(item);
					}
					item.派生类判断结果缓存表.Clear();
					break;
				case "B端":
					List<模式> 更改前B端模式端索引表 = Data.FindRowByID((Guid)otherInfo.OldValue).端索引表_B端;
					更改前B端模式端索引表.Remove(item);
					if (Data.IsThisOrNull((Guid)otherInfo.NewValue) == false)
					{
						List<模式> 更改后B端模式端索引表 = Data.FindRowByID((Guid)otherInfo.NewValue).端索引表_B端;
						更改后B端模式端索引表.Add(item);
					}
					item.派生类判断结果缓存表.Clear();
					break;
				case "源记录":
					List<模式> 更改前源记录端模式端索引表 = Data.FindRowByID((Guid)otherInfo.OldValue).端索引表_源记录;
					更改前源记录端模式端索引表.Remove(item);
					if (Data.IsThisOrNull((Guid)otherInfo.NewValue) == false)
					{
						List<模式> 更改后源记录端模式端索引表 = Data.FindRowByID((Guid)otherInfo.NewValue).端索引表_源记录;
						更改后源记录端模式端索引表.Add(item);
					}
					item.派生类判断结果缓存表.Clear();
					break;
				case "ParentID":
					if (Guid.Empty.Equals(otherInfo.OldValue) == false)
					{
						List<模式> 更改前Parent端模式端索引表 = Data.FindRowByID((Guid)otherInfo.OldValue).端索引表_Parent;
						更改前Parent端模式端索引表.Remove(item);
					}
					if (Data.IsThisOrNull((Guid)otherInfo.NewValue) == false)
					{
						List<模式> 更改后Parent端模式端索引表 = Data.FindRowByID((Guid)otherInfo.NewValue).端索引表_Parent;
						更改后Parent端模式端索引表.Add(item);
					}
					break;
			}
		}

		public static int 计算基准对象(模式 对象, ref 基准类型 基准对象)
		{
			参数树结构 结构 = Data.利用缓存得到基类和关联记录树(对象, false);
			return 结构.递归计算基准对象(ref 基准对象, 0);
		}

		private static void 删除端索引项(模式 item)
		{
			模式 A端对象 = Data.FindRowByID(item.A端);
			if (A端对象 != null)
			{
				List<模式> A端模式端索引表 = A端对象.端索引表_A端;
				A端模式端索引表.Remove(item);
			}

			模式 B端对象 = Data.FindRowByID(item.B端);
			if (B端对象 != null)
			{
				List<模式> B端模式端索引表 = B端对象.端索引表_B端;
				B端模式端索引表.Remove(item);
			}

			模式 源记录对象 = Data.FindRowByID(item.源记录);
			if (源记录对象 != null)
			{
				List<模式> 源记录端模式端索引表 = 源记录对象.端索引表_源记录;
				源记录端模式端索引表.Remove(item);
			}

			模式 Parent对象 = Data.FindRowByID(item.ParentID);
			if (Parent对象 != null)
			{
				List<模式> Parent端模式端索引表 = Parent对象.端索引表_Parent;
				Parent端模式端索引表.Remove(item);
			}
			while (item.端索引表_A端.Count > 0)
			{
				模式 obj = item.端索引表_A端[0];
				obj.A端 = Data.NullGuid;
			}
			while (item.端索引表_B端.Count > 0)
			{
				模式 obj = item.端索引表_B端[0];
				obj.B端 = Data.NullGuid;
			}
			while (item.端索引表_源记录.Count > 0)
			{
				模式 obj = item.端索引表_源记录[0];
				obj.源记录 = Data.NullGuid;
			}
			while (item.端索引表_Parent.Count > 0)
			{
				模式 obj = item.端索引表_Parent[0];
				obj.ParentID = Data.NullGuid;
			}
			//while (item.端索引表.Count > 0)
			//{
			//    端索引 obj = item.端索引表[0];

			//    switch (obj.that端)
			//    {
			//        case 字典_目标限定.A端:
			//            obj.模式Row.A端 = Data.NullGuid;
			//            break;
			//        case 字典_目标限定.B端:
			//            obj.模式Row.B端 = Data.NullGuid;
			//            break;
			//        case 字典_目标限定.连接:
			//            obj.模式Row.源记录 = Data.NullGuid;
			//            break;
			//    }
			//}
		}

		private static void 添加端索引项(模式 item)
		{
			if (Data.IsThisOrNull(item.ID))
				return;

			if (Data.IsThisOrNull(item.A端) == false)
			{
				//Assert(Data.Exist(item.A端));
				模式 对端 = Data.FindRowByID(item.A端);
				if (对端 != null)
				{
					List<模式> A端模式端索引表 = 对端.端索引表_A端;
					A端模式端索引表.Add(item);
				}
			}

			if (Data.IsThisOrNull(item.B端) == false)
			{
				//Assert(Data.Exist(item.B端));
				模式 对端 = Data.FindRowByID(item.B端);
				if (对端 != null)
				{
					List<模式> B端模式端索引表 = 对端.端索引表_B端;
					B端模式端索引表.Add(item);
				}
			}

			if (Data.IsThisOrNull(item.源记录) == false)
			{
				//Assert(Data.Exist(item.源记录));
				模式 对端 = Data.FindRowByID(item.源记录);
				if (对端 != null)
				{
					List<模式> 源记录端模式端索引表 = 对端.端索引表_源记录;
					源记录端模式端索引表.Add(item);
				}
			}

			if (Data.IsThisOrNull(item.ParentID) == false && Guid.Empty.Equals(item.ParentID) == false)
			{
				//Assert(Data.Exist(item.ParentID));
				模式 对端 = Data.FindRowByID(item.ParentID);
				if (对端 != null)
				{
					List<模式> Parent端模式端索引表 = 对端.端索引表_Parent;
					Parent端模式端索引表.Add(item);
				}
			}
		}



		static public Guid 合并三个Guid(Guid c, Guid a, Guid b)
		{
			byte[] vc = c.ToByteArray();
			if (a.Equals(Guid.Empty) == false)
			{
				byte[] va = a.ToByteArray();
				for (int i = 0; i < 16; i++)
					vc[i] ^= va[i];
			}
			if (b.Equals(Guid.Empty) == false)
			{
				byte[] vb = b.ToByteArray();
				for (int i = 0; i < 16; i++)
					vc[i] ^= vb[i];
			}
			return new Guid(vc);
		}

		static public string 语言角色名称(int 语言角色)
		{
			PatternDataSet.字典_语言角色Row r = patternDataSet.字典_语言角色.FindByID(语言角色);
			return r.名称;
		}

		//static public DataRow[] Select并满足基本关联(DataTable table, string select串, string 排序, Guid 基本关联, bool 可以是派生 = true)
		//{
		//	Assert(在二元关联集合中(基本关联, false));

		//	if (Guid.Empty.Equals(基本关联) == false)
		//	{
		//		string s;
		//		if (可以是派生)
		//		{
		//			if (串缓存.ContainsKey(基本关联))
		//				s = 串缓存[基本关联];
		//			else
		//			{
		//				s = 连接过滤串(基本关联);
		//				串缓存.Add(基本关联, s);
		//			}
		//		}
		//		else
		//			s = " 连接='" + 基本关联 + "' ";

		//		select串 = "(" + select串 + ") AND ( " + s + ")";
		//	}
		//	DataRow[] r1 = table.Select(select串, 排序, DataViewRowState.CurrentRows);
		//	return r1;
		//}

		static Dictionary<Guid, string> 串缓存 = new Dictionary<Guid, string>();
		static private string 连接过滤串(Guid 基本关联)
		{
			string s = " 连接='" + 基本关联 + "' ";
			//List<模式> dr = 模式表.对象集合.Where(r => r.ParentID == 基本关联).ToList();
			foreach (模式 r in Data.FindRowByID(基本关联).端索引表_Parent) s = s + " OR " + 连接过滤串((Guid)r.ID);
			return s;
		}

		static public void 新建会话()
		{
			if (Data.动态绑定至Form)
				模式frm.新建会话();
			else
			{
				模式 row = Data.New派生行(Data.会话组织Guid);
				row.形式 = "[会话组织]";
				Data.模式会话总数++;
				row.序号 = Data.模式会话总数;
				Data.模式会话表.新加对象(row);
				Data.当前会话 = row.ID;
			}
		}

		static public int 合并概率打分(int 打分1, int 打分2)
		{
			return 打分1 + 打分2 - 9;
		}

		//static public DataRow[] Select(DataTable table, string selectstr, string sortstr)
		//{
		//    if (table == patternDataSet.模式)
		//    {
		//        if (模式表Select缓存.ContainsKey(selectstr))
		//            return 模式表Select缓存[selectstr];
		//    }
		//    DataRow[] r = table.Select(selectstr, sortstr, DataViewRowState.CurrentRows);
		//    if (table == patternDataSet.模式)
		//        模式表Select缓存.Add(selectstr, r);
		//    return r;

		//}

		static public bool ID相等(object id1, object obj2)
		{
			Guid id = (Guid)id1;
			return id.Equals(obj2);
		}


		static public bool 是二元关联但排除掉等价型(模式 row)
		{
			//假定关联型对象不会有等价。
			//if (等价Guid.Equals(row["连接"]))
			//{
			//    Assert(等价Guid.Equals(row["源记录"]));
			//    return false;
			//}
			return 是二元关联(row, true);
		}


		static public bool 是二元关联(模式 row, bool 包含推导)
		{
			return 是二元关联(row.ID, 包含推导);
		}

		static public bool 是二元关联(Guid id, bool 包含推导和比较)
		{
			while (概念Guid.Equals(id) == false && NullGuid.Equals(id) == false)
			{
				if (Data.在基本关联集合中(id, false))
				{
					if (包含推导和比较 == false && (Data.推导即命题间关系Guid.Equals(id) || Data.二元比较关系Guid.Equals(id)))
						return false;
					return true;
				}
				模式 r = Data.FindRowByID(id);
				if (r == null)
					break;
				Assert(id.Equals(r.源记录) == false);
				id = (Guid)r.源记录;
			}
			return false;

		}


		static public Guid 一级关联类型(模式 row)
		{
			Assert(在基本关联集合中((Guid)row.连接));
			if (Data.在基本关联集合中(row.ID))
				return row.ID;
			return (Guid)row.连接;
		}

		static public Guid 二级关联类型(模式 row)
		{
			if (Data.在基本关联集合中(row.ID, false))
				return row.ID;
			return 返回基本关联(row, false);
		}

		static public Guid 返回基本关联(Guid id, bool 限于一级 = true)
		{
			模式 row = Data.FindRowByID(id);
			//以后考虑根据比例值Aa和Ab等值进行修正，把【属于】转换为【等价】？                        
			return 返回基本关联(row, 限于一级);
		}
		static public Guid 返回基本关联(模式 row, bool 限于一级 = true)
		{
			Guid id = calcbaseclass(row, 限于一级);
			//以后考虑根据比例值Aa和Ab等值进行修正，把【属于】转换为【等价】？                        
			return id;
		}

		static private Guid calcbaseclass(模式 row, bool 限于一级)
		{
			Guid id = row.ID;
			int count = 0;
			while (Data.NullGuid.Equals(id) == false)
			{
				if (限于一级)
				{
					if (Data.在基本关联集合中(id))
						return id;
				}
				else
				{
					if (Data.在基本关联集合中(id, false))
						return id;
				}

				模式 r = count == 0 ? row : Data.FindRowByID(id);

				count++;

				if (r == null)
					break;
				Assert(id.Equals(r.源记录) == false);
				id = (Guid)r.源记录;
			}
			return Data.NullGuid;

		}
		//一级二元关联是指最基本的几个二元关联，而不考虑其派生的。
		static public bool 在基本关联集合中(Guid obj, bool 限于一级 = true)
		{
			if (限于一级)
				return 一级基本关联集合.Contains(obj);
			return 所有基本关联集合.Contains(obj);

		}

		static public bool 是派生类(Guid 基类Guid, 模式 row, int 允许的替代类型)
		{
            //陈峰，看起来，聚合替代是不起作用的！
            允许的替代类型 &= ~替代.聚合替代;


			if (row.派生类判断结果缓存表.ContainsKey(基类Guid))
			{
				return row.派生类判断结果缓存表[基类Guid];
			}
			//foreach (派生类判断结果 obj in row.派生类判断结果缓存表)
			//{
			//    if (obj.派生类ID == 基类Guid)
			//        return obj.结果;
			//}
			bool 是否派生 = false;
			if (是二元关联(row, true))
				是否派生 = 是派生关联(基类Guid, row) > 0;

			else if (是二元关联(基类Guid, true))
				是否派生 = false;
			else
			{
				参数树结构 基类树 = 利用缓存得到基类和关联记录树(row, false);

				if (基类树.递归从基类树中查找广义匹配的基类(基类Guid, 允许的替代类型) != -1)
					是否派生 = true;
				else if ((允许的替代类型 & 替代.聚合替代) == 0)
					是否派生 = false;
				else
				{
					//如果允许聚合方式的，那么就要这个扩展的方法。
					//这个方法效率不是很高，也许要做一个更简单方便的方法。
					参数树结构 参数树 = 利用缓存得到基类和关联记录树(FindRowByID(基类Guid), false);
					List<三级关联> 结果 = new List<三级关联>();
					参数树.递归计算三级关联(基类树, 字典_目标限定.A端, 结果, Data.属于Guid, -1, new List<模式>());
					foreach (三级关联 obj in 结果)
					{
						if (替代.是属于等价或聚合(一级关联类型(obj.中心主关联.目标)))
						{
							是否派生 = true;
							break;
						}
					}
				}
			}
			//将结果加入缓存表
			row.派生类判断结果缓存表.Add(基类Guid, 是否派生);
			//row.派生类判断结果缓存表.Add(new 派生类判断结果(基类Guid,row.ID,是否派生));
			return 是否派生;
		}

		//一些单主语动词，以及可以做名词谓语的词就都可以作为小品词。
		//本质上是同一主语来关联，但是没有主语的时候也明显能看出来。【走累】【长熟】【杀死】等。
		static public int 能够做动词小品词(模式 row)
		{
			////假定属于关系的类和形式集合（和、加等）是可以序列化的。
			if (是派生类(人单动作Guid, row, 替代.正向替代))
				return 2;

			//if (能够作为名词谓语(row))
			//	return 1;

			return 0;
		}

		//【苹果红了】这样的【红】就是名词谓语。
		static public bool 能够作为名词谓语(模式 源模式行)
		{
			if (是派生类(定性Guid, 源模式行, 替代.正向替代 | 替代.等价替代 | 替代.聚合替代))
			{
				if (是派生类(符合程度Guid, 源模式行, 替代.正向替代))
					return false;
				if (是派生类(相对空间Guid, 源模式行, 替代.正向替代))//里边，外，中等一般都不作为名词谓语
					return false;
				if (是派生类(定性时间度Guid, 源模式行, 替代.正向替代))//先、后，不作为名词谓语
					return false;
				return true;
			}
			return false;
		}


		static public bool 拥有动词宾语(模式 row)
		{
			//暂时这样处理，如果通过计算来得到，效率比较低，因为中间要通过角色来算
			//以后可以考虑放置一个缓存信息来记录
			if (是派生类(人对事动作Guid, row, 替代.正向替代))
				return true;
			return false;
		}

		static public bool 正常处理情况下要求右边先生长完全(模式 row)
		{
			//暂时这样处理，如果通过计算来得到，效率比较低，因为中间要通过角色来算
			//以后可以考虑放置一个缓存信息来记录

			if (拥有动词宾语(row))
				return true;

			return false;
		}

		static public bool 拥有动词主语(模式 row)
		{
			//暂时这样处理，如果通过计算来得到，效率比较低，因为中间要通过角色来算
			//以后可以考虑放置一个缓存信息来记录
			if (是二元关联(row, true))
				return true;
			return false;
		}

		static public bool 能够序列化(模式 row)
		{
			////假定属于关系的类和形式集合（和、加等）是可以序列化的。
			if (是二元关联(row, true))
				return true;

			//暂时让形式集合看着非序列化的，实际上，应该根据元素来决定。集合自己并不决定。
			if (是派生类(抽象形式集合Guid, row, 替代.正向替代))
				return false;

			if (是派生类(事件Guid, row, 替代.正向替代))
				return true;

			if (是派生类(生存类型Guid, row, 替代.正向替代))
				return true;

			return false;
		}

		//[是][+]等一些对象，只能作为序列化，很少会被用做【的】字句研究的目标，这样进行规定，就排除了他们和前边的【的】进行结合的可能。前边的【的】就必须进行自行封闭。
		static public bool 能够做从句的中心(模式 row)
		{
			//暂时先这样。
			if (能够序列化(row))
				return false;
			if (是派生类(语用基类Guid, row, 替代.正向替代))
				return false;
			return true;
		}


		public static bool 是拥有形式(模式 item)
		{
			return 拥有形式Guid.Equals(一级关联类型(item));
		}


		static public bool 是形式(模式 row)
		{
			return 属于Guid.Equals(一级关联类型(row)) && 是派生类(形式Guid, row, 替代.正向替代);
		}
		//源记录处于基本集合中。
		static public bool 是原始拥有形式(模式 row)
		{
			if (拥有形式集合.Contains((Guid)row.ID) || 拥有形式集合.Contains(row.源记录))
				return true;
			if (是拥有形式(row) == false)
				return false;
			模式 上一个 = FindRowByID(row.源记录);
			if (取得嵌入串(上一个) == 取得嵌入串(row))//和上一个的串相同，也就是从基类继承的形式，也就不算是原始形式。
				return false;
			return true;
		}

		//概念型形式而不是介词，的，地等形式。
		static public bool 是实体参数的形式(Guid id)
		{
			return id.Equals(概念拥有介词形式Guid) || id.Equals(拥有形式Guid) || id.Equals(个人拥有名字Guid);
		}

		//static public bool 是真正的形式(模式 row)
		//{
		//	if (是实体参数的形式(二级关联类型(row)))
		//		return true;
		//	if (表达式介词集合.Contains(取得嵌入串(row)))//表达式介词产生的表达式也是真正的形式。"+"，"-"等形式。
		//		return true;
		//	return false;
		//}

		static public void 处理接受输入(string str)
		{
			Processor p = Processor.当前处理器;
			模式frm.编辑框().EditValue = str;
			模式 新加模式 = 模式frm.执行添加句子(str);
			输出对话信息(str, false);

			模式frm.treeListresult.BeginUpdate();
			模式frm.treeListresult.LockReloadNodes();
			if (对话frm.翻译.Checked == false)
				p.尝试对问题进行查询对命题进行推导();
			else
				p.进行翻译();

			模式frm.treeListresult.UnlockReloadNodes();
			// 模式frm.treeListresult.EndCurrentEdit();
			模式frm.treeListresult.EndUpdate();
			Data.模式frm.定位选择模式行(新加模式.ID, true, true, false);
		}
		static public void 解析句子(string str)
		{
			模式 row = Data.New派生行(Data.属于Guid);
			//str = str.Replace("'", "[']");
			//str = str.Replace("\"", "[\"]");
			row.B端 = Data.单句分析Guid;
			row.形式 = "\"" + str + "\"";

			模式 会话模式 = Data.FindRowByID(Data.当前会话);
			row.ParentID = 会话模式.ID;
			row.序号 = 会话模式.端索引表_Parent.Count + 1;
			Data.模式会话表.新加对象(row);

			//开始解析
			Data.当前句子Row = row;
			Data.当前句子串 = str;
			//if (Data.语料库.Count < 1)
			//{
			Data.加入所有形式();
			Data.重置缓存根模式集合();
			//}
			Data.分解串并生成串对象(row.ID);
			Data.进行生长并得到所有中间结果();
			Data.选择最优结果生成(true);
			Data.递归设置语境树知识有效性(row, -1);
		}
		static public void 输出处理信息(string 信息分类, string 输出信息)
		{
			输出信息 = 对话frm.textBox1.Text + "【" + 信息分类 + "】" + 输出信息 + "\r\n";
			对话frm.textBox1.Text = 输出信息;
		}

		static public void 输出对话信息(string 输出信息, bool 机器说话 = true)
		{
			if (机器说话)
				输出信息 = "【机】：" + 输出信息;
			else
				输出信息 = "【人】：" + 输出信息;
			输出信息 = 对话frm.textBox2.Text + 输出信息 + "\r\n";
			对话frm.textBox2.Text = 输出信息;

		}

		static public void 刷一条记录的根(模式 r1)
		{
			参数字段 c = new 参数字段(r1.参数集合);

			r1.参数集合 = c.ToString();

			//if (拥有形式Guid.Equals(r1.连接))
			//{
			//    if (是子记录(r1, 拥有形式Guid, true) == false)
			//    {
			//        string s = 取得嵌入串(r1.形式);
			//        r1.形式 = s;
			//        FindTableByID(r1.ID).更新对象(r1);
			//    }
			//}
			//bool b = Data.是子记录(r1, Data.语篇句Guid, true);
			//if (b && (int)r1.语境树 == 0)
			//{
			//    r1.语境树 = 1;
			//    FindTableByID(r1.ID).更新对象(r1);
			//}
			//else if (b == false && (int)r1.语境树 != 0)
			//{
			//    r1.语境树 = 0;
			//    FindTableByID(r1.ID).更新对象(r1);
			//}

			//if (是拥有形式(r1))
			//if (r1.参数.B对A的创建性 == 0)
			//{
			//    参数字段 a = r1.参数;
			//    a.B对A的创建性 = 5;
			//    r1.参数 = a;
			//    FindTableByID(r1.ID).更新对象(r1);
			//}



			//参数字段 c = new 参数字段(r1.参数集合);
			//int k = c.方向;
			//int r = 允许的生长方向(r1);
			//if (r != k)
			//{
			//	c.方向 = r;
			//	r1.参数集合 = c.ToString();
			//	FindTableByID(r1.ID).更新对象(r1);
			//}

			//Guid id = 一级关联类型(r1);
			//if (id.Equals(拥有形式Guid) == false && id.Equals(拥有语言角色Guid) == false && (int)r1["语言角色"] != 0)
			//	r1["语言角色"] = 0;


			//if (路径引用Guid.Equals(r1获取连接类型))//如果是【路径引用】，那么就不重设置了。
			//    return;

			//Guid v = 返回二元关联的基本分类((Guid)r1["源记录"], false);
			//if (v.Equals(r1获取连接类型) == false)
			//    r1获取连接类型 = v;


			//if (属于Guid.Equals(v))
			//{
			//    参数集合字段 参数 = new 参数集合字段((string)r1["参数集合"]);
			//    if (参数.Aa == 0)
			//        参数.Aa = 9;
			//    if (参数.Ab == 0)
			//        参数.Ab = 1;
			//    if(参数.ToString()!=r1["参数集合"])
			//        r1["参数集合"] = 参数.ToString();
			//}
		}

		static public void 输出串(string 串)
		{
			System.Diagnostics.Debug.WriteLine(串);
		}

		static public void Assert(bool b)
		{
			if (是调试模式)
				System.Diagnostics.Debug.Assert(b);
		}

		static public void 显示信息(string 串)
		{
			mainfrm.Text = "DSM-System:" + 串;
		}


		public static void 清除缓存()
		{
			patternDataSet.模式结果.Clear();
			属于和拥有树缓存.Clear();
			//属于基类树缓存.Clear();
			属于和拥有树关联缓存.Clear();
			//属于基类树关联缓存.Clear();
		}

		//static public bool 是逗号(DataRow row)
		//{
		//    Guid B端 = (Guid)row["B端"];
		//    if (Data.短句停顿Guid.Equals(B端))
		//    {
		//        Data.Assert(Data.属于Guid.Equals(Data.一级关联类型(row)));
		//        return true;
		//    }
		//    return false;
		//}

		static public 模式 CopyRow(模式 row, bool newid = true)
		{
			if (row == null)
				return null;

			模式 newObj = new 模式(row);

			//从素材中复制的记录，语境树可能是-1。所以设置为0。
			newObj.语境树 = 0;

			if (newid)
				newObj.ID = Guid.NewGuid();

			return newObj;
		}

		static private void 递归CopyTree(List<模式> 模式集合, 模式 tree, List<模式> rows)
		{
			模式 row = CopyRow(tree, false);
			rows.Add(row);

			//var dr = 模式集合.Where(r => r.ParentID == tree.ID).ToList();

			foreach (模式 r in tree.端索引表_Parent)
				递归CopyTree(模式集合, r, rows);
		}

		static public List<模式> CopyTree(模式 tree)
		{
			var rows = new List<模式>();
			//List<模式> 模式集合 = Data.FindTableByID(tree.ID).对象集合.ToList();
			递归CopyTree(null, tree, rows);
			return rows;
		}

		public static 模式 PasteRows(List<模式> rows, bool IsChild, int order, 模式 parentrow, bool 重新排列序号 = false, Dictionary<Guid, 模式> 传出keys = null)
		{
			模式基表 目标表 = Data.FindTableByID(parentrow.ID);

			if (rows == null)
				return null;

			Guid parentId = IsChild ? parentrow.ID : (Guid)parentrow.ParentID;

			Dictionary<Guid, 模式> keys = new Dictionary<Guid, 模式>();
			int i = 0;
			int 起始序号 = -1;
			foreach (模式 oldRow in rows)
			{
				模式 row = Data.CopyRow(oldRow);

				keys.Add(oldRow.ID, row);
				if (传出keys != null)
					传出keys.Add(oldRow.ID, row);
				if (起始序号 == -1)
					起始序号 = row.序号;

				if (重新排列序号)
					row.序号 = row.序号 - 起始序号 + order;
				else
				{
					if (i == 0 && order != 0)
						row.序号 = (int)parentrow.序号 + order;
				}
				i++;
				目标表.新加对象(row);
				刷一条记录的根(row);
			}

			模式 returnrow = null;

			foreach (var r in keys)
			{
				模式 row = r.Value;

				//先替换所有父ID。
				if (!Data.NullParentGuid.Equals(row.ParentID))
				{
					if (keys.ContainsKey((Guid)row.ParentID))
					{
						row.ParentID = keys[(Guid)row.ParentID].ID;
					}
					else
						row.ParentID = parentId;
				}
				else
					row.ParentID = parentId;

				if (keys.ContainsKey((Guid)row.A端))
				{
					row.A端 = keys[(Guid)row.A端].ID;
				}
				if (keys.ContainsKey((Guid)row.B端))
				{
					row.B端 = keys[(Guid)row.B端].ID;
				}
				if (keys.ContainsKey((Guid)row.连接))
				{
					row.连接 = keys[(Guid)row.连接].ID;
				}
				if (keys.ContainsKey((Guid)row.源记录))
				{
					row.源记录 = keys[(Guid)row.源记录].ID;
				}
				if (keys.ContainsKey((Guid)row.C端))
				{
					row.C端 = keys[(Guid)row.C端].ID;
				}

				if (parentId.Equals(row.ParentID))
					returnrow = row;
			}

			return returnrow;
		}

		static public bool 是介动词后生存串(string str)
		{
			foreach (介动词后生存记录 o in 介动词后生存记录.介动词后生存集合)
				if (str == o.生存串)
					return true;
			return false;
		}


		//有的介词可以作为前置介词，也可以作为后置介词，比如【和】等
		//所以，这里的处理有些不严谨，先这样。
		static public bool 是介词或者串(模式 row, bool 包含前置介词, bool 包含后置介词, bool 包含串)
		{
			string str = row.形式;

			int 次数 = 0;

		再做一次:

			Guid ID = row.ID;

			if (介词Guid.Equals(ID))
			{
				Data.Assert(Data.属于Guid.Equals(Data.一级关联类型(row)));
				if (包含前置介词 && 前置介词集合.Contains(str))
					return true;
				if (包含后置介词 && 后置介词集合.Contains(str))
					return true;
			}

			if (包含串 && Data.字符串Guid.Equals(ID))
			{
				Data.Assert(Data.属于Guid.Equals(Data.一级关联类型(row)));
				return true;
			}

			if (次数 == 0)
			{
				次数++;
				row = Data.FindRowByID((Guid)row.源记录);
				goto 再做一次;
			}

			return false;
		}

		static public bool 可作为中心对象(模式 row, bool 允许外延指定)
		{
			//if (ThisGuid.Equals(row.ID))//【this】自己是一个事物也就是【**的】后边空的。
			//    return true;
			if (允许外延指定 == false && 是派生类(外延指定Guid, row, 替代.正向替代))
				return false;
			if (是介词或者串(row, true, true, true))
				return false;
			if (Data.是派生类(Data.值概念Guid, row, 替代.正向替代))//值概念一般不作为关注对象。
			{
				if (Data.是派生类(Data.疑问算子Guid, row, 替代.正向替代) == false) //疑问算子允许
					return false;
			}
			if (是派生类(基本量Guid, row, 替代.正向替代))
				return false;
			return true;
		}

		//一些关联，比如【属拥】【扮演】我们只允许正向关联，而不允许反向关联，也就是【如果借】，那么是【借】为中心，除非显式的【的从句】约束。
		//而另一些，则只允许反向关联不允许正向关联。因为聚合其实是双向的，为了优化，就只保留一个方向。普通聚合都只保留正向关联。
		//而【推理角色聚合关系】则保留反向关联，这是为了让【关系】成为中心，而肯定不会把【推理角色】作为中心。
		static public int 允许的生长方向(模式 row)
		{
			//参数字段 o = new 参数字段(row.参数集合);
			return row.参数.方向;//o.方向;
			//Guid 一级类型 = 一级关联类型(row);
			//Guid 二级类型 = 二级关联类型(row);
			//if (属拥Guid.Equals(一级类型) || 拥有Guid.Equals(一级类型))
			//{
			//    return 正向关联;
			//}
			//else if (属于Guid.Equals(一级类型) || 包括Guid.Equals(一级类型))
			//    return 正向关联;
			//else if (等价Guid.Equals(一级类型))
			//    return 正向关联 | 反向关联;
			//else if (聚合Guid.Equals(一级类型))
			//{
			//    if (反向聚合Guid.Equals(二级类型))
			//        return 反向关联;
			//    //if (Data.扮演Guid.Equals(二级类型))//扮演也是正向关联吧？
			//    //	return 反向关联;
			//    return 正向关联;
			//}

			////if (是派生关联(相对空间拥有原始空间Guid, 派生关联row)>0)//【桌子上面的茶杯】，就只能是【聚合了上面的茶杯】，禁止掉这条反向的【被上面拥有的茶杯】，当然后者来说其实也不算错。
			////	return 正向关联;
			//return 正向关联 | 反向关联;

		}


		static public bool 是根据派生关联计算的派生类(Guid 基关联ID, 模式 派生关联row)
		{
			if (替代.是分类或聚合或属拥(Data.一级关联类型(派生关联row)) == false)
				return false;
			模式 源关联 = Data.FindRowByID((Guid)(Data.FindRowByID((Guid)派生关联row.B端).源记录));
			if (Data.是派生类(基关联ID, 源关联, 替代.正向替代))
				return true;
			return false;
		}

		static public bool 能进行一级传递(模式 关联)
		{
			//if (Data.是派生关联(Data.量化概念聚合量Guid, 关联) > 0)
			//	return true;
			return true;
		}

		static public bool 能进行二级传递(模式 关联)
		{
			//【量拥有倍数】不能被隐含传递，必须是直接的【量】。比如【100速度】就不行。【100【公里速度】】也就不行，必须是【【100公里】速度】。
			if (Data.是派生关联(Data.量拥有倍数Guid, 关联) > 0)
				return false;
			return true;
		}

		static public bool 是广义等价(模式 模板, 模式 知识)
		{
			if (Data.属于Guid.Equals(模板.源记录) == false)
				return false;
			return 替代.是属于等价或聚合(一级关联类型(知识));
		}

		//根据【连接】字段进行查找，假定是单继承的。
		//如果等于，返回2，派生返回1，不是派生的返回0.
		static public int 是派生关联(Guid 基关联ID, 模式 派生关联row)
		{
			if (基关联ID.Equals(Data.语用基类Guid))
				基关联ID = 基关联ID;

			if (基关联ID.Equals(派生关联row.ID))
				return 2;

			//if (是分类(Data.根模式(派生关联row)))//[A端苹果：连接水果：B端水果]的连接等于一个【属于】连接。
			//{
			//    DataRow 基关联 = Data.FindRowByID(基关联ID);
			//    if (Data.是分类(Data.根模式(基关联)) && 基关联获取连接类型 == 基关联["B端"])
			//    {
			//        Data.Assert(Data.ThisGuid.Equals(基关联["A端"]));
			//        基关联ID = Data.属于Guid;//
			//    }
			//}

			模式 派生关联 = 派生关联row;

			if (基本关联Guid.Equals(基关联ID))//所有关联都是【二元关联】派生的。
				return 1;
			while (true)
			{
				if (基关联ID.Equals(派生关联.ID))
					return 1;

				派生关联 = FindRowByID((Guid)派生关联.源记录);

				if (NullGuid.Equals(派生关联.ID) || 概念Guid.Equals(派生关联.ID) || 基本关联Guid.Equals(派生关联.ID))//【概念属于概念】和【二元关联】排除掉，不是任何关联的派生。
					break;
			}

			//if (替代.可正向替代(Data.一级关联类型(派生关联row)))//如果是【属于】，把B端做一次。
			//{
			//	派生关联 = 派生关联row;
			//	while (true)
			//	{
			//		if (基关联ID.Equals(派生关联.ID))
			//			return 1;
			//		派生关联 = FindRowByID((Guid)派生关联.B端);

			//		if (NullGuid.Equals(派生关联.ID) || 概念Guid.Equals(派生关联.ID) || 基本关联Guid.Equals(派生关联.ID))//【概念属于概念】和【二元关联】排除掉，不是任何关联的派生。
			//			break;
			//	}
			//}
			//else
			//{
			foreach (模式 row in 派生关联row.端索引表_A端)
			{
				if (row.语境树 != 0 || 替代.是本质正向分类(Data.一级关联类型(row)) == false)
					continue;
				int k = 是派生关联(基关联ID, FindRowByID(row.B端));
				if (k > 0)
					return k;
			}

			return 0;
		}
		static public 模式 创建新的语义对象到公共新对象(SubString obj, 模式 baserow)
		{
			模式 语义概念行 = Data.New派生行(baserow, 字典_目标限定.空, true);
			语义概念行.ParentID = Data.公共新对象Guid;
			Data.get模式编辑表().新加对象(语义概念行);

			语义概念行.序号 = obj.begindex;
			语义概念行.形式 = "[" + obj.ToString() + "]";
			语义概念行.语言角色 = 字典_语言角色.全部;

			模式 新对象拥有形式行 = Data.New派生行(Data.拥有形式Guid, 字典_目标限定.空);
			新对象拥有形式行.B端 = Data.ThisGuid;
			新对象拥有形式行.A端 = 语义概念行.ID;
			新对象拥有形式行.ParentID = 语义概念行.ID;
			新对象拥有形式行.形式 = obj.ToString();
			新对象拥有形式行.序号 = obj.begindex;
			新对象拥有形式行.语言 = Data.当前解析语言;
			新对象拥有形式行.语言角色 = 字典_语言角色.全部;
			Data.get模式编辑表().新加对象(新对象拥有形式行);

			return 新对象拥有形式行;
		}
		static public int Unicode比较(string t, string s)
		{
			///这个地方吃了大亏
			///如果用字符串的compareto方法比较，好像字符串变成了GBK类似的一种编码来比较
			///和我现在写的单个字符自己比较的方法完全不同
			///我这里用的char的方法，都是unicode单码的标准方式
			///单码的方式里边，字符串的排列次序和数据库可能不一样的！数据库往往也是GBK的方式
			///GBK的方式下，汉字主要是按拼音排的，而unicode不是。

			//if (o.是抽象串 && !t.是抽象串)
			//	return -1;

			//if (!o.是抽象串 && t.是抽象串)
			//	return 1;

			int i = 0;
			while (true)
			{
				if (t.Length == i)
				{
					if (s.Length == i)
						return 0;
					else
						return -(i + 1);
				}
				else
				{
					if (s.Length == i)
						return i + 1;
				}

				char ci = t[i];
				char cj = s[i];
				i++;
				if (ci > cj)
					return i;
				if (ci < cj)
					return -i;
			}
		}
		//static public void 快速排序(int left, int right)
		//{
		//    int i = left;
		//    int j = right;
		//    PatternDataSet.形式化语料串Row ti, tj;
		//    PatternDataSet.形式化语料串Row middle;
		//    middle = 语料库[(right + left) / 2];
		//    while (Unicode比较(语料库[i], middle) < 0 && (i < right))
		//    {
		//        i++;
		//    }
		//    while (Unicode比较(语料库[j], middle) > 0 && (j > left))
		//    {
		//        j--;
		//    }
		//    if (i <= j)
		//    {
		//        if (i < j)
		//        {
		//            ti = 语料库[i];
		//            tj = 语料库[j];
		//            语料库.Rows.RemoveAt(j);
		//            语料库.Rows.RemoveAt(i);
		//            语料库.Rows.InsertAt(tj, i);
		//            语料库.Rows.InsertAt(ti, j);
		//        }
		//        i++;
		//        j--;
		//    }
		//    if (left < j)
		//    {
		//        快速排序(left, j);
		//    }
		//    if (right > i)
		//    {
		//        快速排序(i, right);
		//    }
		//}

		static public string 取得嵌入串(模式 item)
		{
			if (拥有形式Guid.Equals(item.连接))
			{
				//if (是子记录(item, 拥有形式Guid, true) == false)
				{
					return item.形式;
				}
			}

			TheString thestr = new TheString(item.形式);
			return thestr.嵌入串;
		}

		static public String GetShowString(Guid ID)
		{
			string str = "";
			if (ID == Guid.Empty)
				return str;
			PatternDataSet ds = Data.patternDataSet;
			模式 row = Data.FindRowByID(ID);
			if (row != null)
				str = row.形式;
			return str;
		}

		//public static 形式化语料串 加入形式串(Guid 目标, string 串)
		//{
		//    if (String.IsNullOrEmpty(串))
		//        return null;

		//    形式化语料串 item = new 形式化语料串() { 字符串 = 串, ObjectID = 目标 };


		//    return item;
		//}

		static public void 加入所有形式()
		{
			语料库.RemoveAll(r => r.IsCore == true);
			foreach (模式 row in 模式表.对象集合.Where(r => r.语境树 == 0)) //只取知识
			{
				if (是子记录(row, 语篇句Guid, true))
					continue;
				if (拥有形式Guid.Equals(row.连接))
				{
					if (是子记录(row, 拥有形式Guid, true) == false && !string.IsNullOrEmpty(row.形式))
					{
						Data.语料库.Add(new 形式化语料串() { 字符串 = row.形式, ObjectID = row.ID, IsCore = true });
						continue;
					}
				}

				TheString str = new TheString((string)row.形式);
				if (str.有嵌入串)
				{
					Data.语料库.Add(new 形式化语料串() { 字符串 = str.嵌入串, ObjectID = row.ID, IsCore = true });
				}
			}
			语料库.Sort();
		}

		static public string 合并串(string 前串, string 后串, int 语言, int 选项 = 0)
		{
			if (后串 == null || 后串 == "")
				return 前串;

			if (前串 == null || 前串 == "")
				return 后串;

			if (语言 == 字典_语言.英语)
			{
				if (第一个字符为标点符号(后串))
					return 前串 + 后串;
				else
					return 前串 + " " + 后串;
			}

			return 前串 + 后串; ;
		}

		public static bool 第一个字符为标点符号(string 字符串, int 语言 = 字典_语言.英语)
		{
			if (语言 != 字典_语言.英语 || string.IsNullOrEmpty(字符串))
				return false;

			return 字符类.是标点符号(字符串[0]);

		}



		public static void 预处理()
		{
			// 快速排序(0, 语料库.Count-1);
			//语料库.Sort();
			////这个排序后，抽象串将全部排到最后边。

			//实体语料串数 = 0;
			//for (int i = 语料库.Count - 1; i >= 0; i--)
			//{
			//    if ((语料库[i] as 模板_语料串).是抽象串 == false)
			//    {
			//        实体语料串数 = i + 1;
			//        break;
			//    }
			//}

			//for (int i = 0; i < 实体语料串数; i++)
			//{
			//    模板_语料串 o = 语料库[i] as 模板_语料串;
			//    if (o.所属抽象串id != null)
			//    {
			//        for (int j = 实体语料串数; j < 语料库.Count; j++)
			//        {
			//            if ((语料库[j] as 模板_语料串).ID == o.所属抽象串id)
			//            {
			//                o.所属抽象串 = 语料库[j] as 模板_语料串;
			//                break;
			//            }
			//        }
			//    }
			//}

		}

		//public static string 返回语料字符串(Guid id)
		//{
		//    形式化语料串 row = 语料库.FirstOrDefault(r => r.ID == id);
		//    if (row != null)
		//        return row.字符串;
		//    return id.ToString();
		//}
		///没有找到的话，返回－1
		public static int GetFisrtChar(int f, int e, char c, int index)
		{
			///这里开始计算时，保证语料库已经是排序的，并且
			///在f和e之间的所有语料的前边index-1都已经匹配上前边的字符串
			///所以，index位置的字符肯定是存在的。
			while (f <= e && 语料库[f].字符串.Length <= index)
				f++;
			if (语料库[f].字符串.Length <= index)
				return -1;
			int m;
			while (f <= e)
			{
				char sf = 语料库[f].字符串[index];
				if (c == sf)
					return f;
				else if (c < sf)
					return -1;

				Assert(语料库[e].字符串.Length > index);//前边【f】计算了，这里不应该会长度不够。

				char se = 语料库[e].字符串[index]; ;
				if (c > se)
					return -1;
				if (c == se && e - f == 1)
					return e;

				m = (e + f) / 2;
				if (m == e || m == f)
					return -1;//没有找到了

				char sm = 语料库[m].字符串[index]; ;
				if (c <= sm)
					e = m;
				else
					f = m;
			}
			return -1;
		}

		///没有找到的话，返回－1
		public static int GetLastChar(int f, int e, char c, int index)
		{
			///这里开始计算时，保证语料库已经是排序的，并且
			///在f和e之间的所有语料的前边index-1都已经匹配上前边的字符串
			///所以，index位置的字符肯定是存在的。

			int m;
			while (f <= e)
			{
				char se = 语料库[e].字符串[index]; ;
				if (c == se)
					return e;
				else if (c > se)
					return -1;

				char sf = 语料库[f].字符串[index]; ;
				if (c < sf)
					return -1;
				if (c == sf && e - f == 1)
					return f;

				m = (e + f) / 2;
				if (m == e || m == f)
					return -1;//没有找到了

				char sm = 语料库[m].字符串[index]; ;
				if (c >= sm)
					f = m;
				else
					e = m;
			}
			return -1;
		}

		static public int 取得语言序号(int 序号)
		{
			if (序号 >= 1000000)
				return 序号 - 1000000;
			if (序号 < 0)
				return 序号;
			return 0;
		}

		static public void 递归加入二元关联(Guid parentid)
		{
			//var dr = 模式表.对象集合.Where(r => r.ParentID == parentid).ToList();

			foreach (模式 row in Data.FindRowByID(parentid).端索引表_Parent)
			{
				所有基本关联集合.Add(row.ID);
				递归加入二元关联(row.ID);
			}
		}

		static public void 重置缓存根模式集合()
		{
			//var dr = 模式表.对象集合.Where(r => r.ParentID == 基本关联Guid);

			//把所有【关联】为parent下边的记录作为一级记录。
			一级基本关联集合.Clear();
			一级基本关联集合.Add(基本关联Guid);
			一级基本关联集合.Add(Data.NullGuid); //null模式行不在端索引表中，所以需手动插入
			foreach (模式 row in Data.FindRowByID(基本关联Guid).端索引表_Parent)
			{
				一级基本关联集合.Add(row.ID);
			}
			所有基本关联集合.Clear();
			所有基本关联集合.Add(基本关联Guid);
			递归加入二元关联(基本关联Guid);

			//缓存根模式集合.Add(概念Guid);//错误情况下，会是这个。这时需要检查。

			拥有形式集合.Clear();
			递归加入形式集合(拥有形式Guid);

			//通用介词集合.Clear();
			//集合介词集合.Clear();
			//表达式介词集合.Clear();
			//推导介词集合.Clear();
			//专用前置介词集合.Clear();
			//专用后置介词集合.Clear();
			前置介词集合.Clear();
			后置介词集合.Clear();
			推导集合.Clear();
			推理角色集合.Clear();
			//dr = 模式表.对象集合.Where(r => r.ParentID == 通用介词Guid);
			//foreach (模式 row in dr)
			//{
			//	string s = 取得嵌入串(row);
			//	if (s == string.Empty)
			//		continue;
			//	通用介词集合.Add(s);
			//	if (集合介词Guid.Equals(row.B端))
			//		集合介词集合.Add(s);
			//	if (表达式介词Guid.Equals(row.B端))
			//		表达式介词集合.Add(s);
			//	if (推导介词Guid.Equals(row.B端))
			//		推导介词集合.Add(s);
			//}

			var dr = 模式表.对象集合.Where(r => r.连接 == 拥有形式Guid);
			foreach (模式 row in dr)
			{
				string s = 取得嵌入串(row);
				if (s == string.Empty)
					continue;
				if (是派生关联(关联拥有后置介词Guid, row) == 1 && 后置介词集合.Contains(s) == false)
					后置介词集合.Add(s);
				if (是派生关联(关联拥有前置介词Guid, row) == 1 && 前置介词集合.Contains(s) == false)
					前置介词集合.Add(s);
			}
			后置介词集合.Add("的");
			后置介词集合.Add("地");

			参数树结构 推导派生树 = 利用缓存得到派生树(FindRowByID(推导即命题间关系Guid), true, false);
			递归加入推导(推导派生树);
			推导集合.Reverse();

			参数树结构 推理角色派生树 = 利用缓存得到派生树(FindRowByID(推理角色Guid), true, false);
			递归加入推理角色(推理角色派生树);
			推理角色集合.Reverse();

			组织推导和推理角色的关系();
			//foreach (模式 row in Data.模式表.对象集合)
			//{
			//    if (Data.是拥有形式(row) == false)
			//    {
			//        利用缓存得到基类和关联记录树(row, true); 
			//    }
			//}
		}

		static public void 组织推导和推理角色的关系()
		{
			foreach (推理角色 推理角色 in 推理角色集合)
			{
				if (推理角色.是基本推理角色 == false)
					continue;
				foreach (推导 推导 in 推导集合)
				{
					if (推导.是基本推导 == false)
						continue;
					foreach (模式 row in 推导.目标.端索引表_A端)
					{
						if (row.语境树 != 0)
							continue;
						if (是派生类(row.B端, 推理角色.目标, 替代.正向替代))
						{
							推理角色.推导 = 推导;
							if (是派生关联(命题关系拥有前件Guid, row) > 0)
							{
							}
							else if (是派生关联(命题关系拥有后件Guid, row) > 0)
							{
								推理角色.作为中心 = true;
							}
							else
								Data.Assert(false);

						}
					}

				}
			}
		}

		static public void 递归加入推导(参数树结构 推导派生树)
		{
			推导集合.Add(new 推导(推导派生树.目标));

			if (推导派生树.子节点 != null)
				foreach (参数树结构 obj in 推导派生树.子节点)
					递归加入推导(obj);
		}

		static public void 递归加入推理角色(参数树结构 推理角色派生树)
		{
			推理角色集合.Add(new 推理角色(推理角色派生树.目标));

			if (推理角色派生树.子节点 != null)
				foreach (参数树结构 obj in 推理角色派生树.子节点)
					递归加入推理角色(obj);
		}

		//被动生长的形式对象，比如介词就是，一般不会主动触发。
		static public void 递归加入形式集合(Guid rootid)
		{
			拥有形式集合.Add(rootid);

			//var dr = 模式表.对象集合.Where(r => r.ParentID == rootid).ToList();

			foreach (模式 row in Data.FindRowByID(rootid).端索引表_Parent)
				递归加入形式集合(row.ID);
		}

		//这个方法是排除掉一些肯定不会主动生长，而是必须等待别人生长的时候来合并的对象
		//比如【拥有介词】【拥有的】【拥有被】等这些。这些自己创建没有意义。
		static public bool 是附属形式(Guid id, bool 可以触发创建)
		{
			//此时，已经保证了id的一级是【拥有形式】。
			id = 返回基本关联(id, false);
			if (id.Equals(关联拥有前置介词Guid) || id.Equals(关联拥有介动词后生存Guid) || id.Equals(关联拥有后置介词Guid)
				|| id.Equals(关联拥有的Guid) || id.Equals(关联拥有地Guid) || id.Equals(拥有后置附件Guid))
			{
				if (可以触发创建 == false || id.Equals(关联拥有前置介词Guid) || id.Equals(关联拥有后置介词Guid))
					return true;
			}
			return false;
		}


		static public bool 是子记录(模式 childrow, Guid parent, bool 包含自身)
		{
			if (childrow == null)
				return false;
			if (包含自身 && parent.Equals(childrow.ID))
				return true;
			while (childrow != null)
			{
				if (parent.Equals(childrow.ParentID))
					return true;
				if (NullGuid.Equals(childrow.ParentID) || NullParentGuid.Equals(childrow.ParentID))
					return false;
				childrow = FindRowByID((Guid)childrow.ParentID);
			}
			return false;
		}

		static public void 如果不同则赋新值(ref object obj, object value)
		{
			if (value is Guid)
			{
				if (((Guid)value).Equals(obj) == false)
					obj = value;
				return;
			}
			if (obj != value)
				obj = value;
		}

		static public 模式 New派生行(模式 baserow, int that = 字典_目标限定.空, bool 二元关联处理成This = false)
		{
			Guid baserowid = baserow.ID;

			if (that == 字典_目标限定.空)
				that = (int)baserow.That根;

			//DataRow row = newtable.NewRow();
			//row["ID"] = Guid.NewGuid();
			模式 row = CopyRow(baserow);
			row.ParentID = NullParentGuid;
			//row["Aα"] = 0;
			//row["Aβ"] = 0;
			//row["Bα"] = 0;
			//row["Bβ"] = 0;
			//row["联α"] = 0;
			//row["联β"] = 0;
			//row["that根"] = 字典_目标限定.A端;
			//row["序号"] = 0;
			//row["C端"] = NullGuid;
			//row["的"] = 字典_参数.无;
			//row["显隐"] = 0;
			//row["打分"] = "";
			//row["通用分类"] = 0;
			//row["成立度"] = 100;
			//row["实例数"] = 0;
			//row["全等引用计数"] = 0;
			//row["级别"] = 1;
			//row["认知年龄"] = 20;
			//row["附加信息"] = 0;
			//row["关系距离"] = 0;
			//row["语境树"] = 0;
			//row["层级"] = 1;
			//row["说话时间"] = NullGuid;
			//row["说话地点"] = NullGuid;
			//row["语言"] = baserow["语言"];
			//row["A端"] = baserow["A端"];
			//row["B端"] = baserow["B端"];
			//row["序号"] = baserow["序号"];
			//row["参数集合"] = baserow["参数集合"];
			//row["语言角色"] = baserow["语言角色"];

			row.源记录 = baserowid;
			Guid 基关联 = Data.一级关联类型(baserow);
			row.连接 = 基关联;


			//遇到展开的二元关联的时候，把二元关联本身处理成一个【this】【属于】【二元关联】的记录。
			if (二元关联处理成This == true && 是二元关联(baserow, true))
			{
				that = 字典_目标限定.A端;
				row.A端 = ThisGuid;
				row.B端 = baserowid;
				row.连接 = 属于Guid;
			}

			string str = baserow.形式;
			if (str != null)
			{
				TheString thestr = new TheString(baserow.形式);
				str = thestr.ToString();
			}

			row.形式 = str;

			if (that == 字典_目标限定.A端)
			{
				if (替代.可正向替代(基关联))//属于
				{
					if (ThisGuid.Equals(baserow.A端))//新建的对象，把基类向后移动，产生一个新的对象。
						row.B端 = baserowid;
					row.形式 = /*"?" + */str;
				}
				else if (ID相等(拥有Guid, 基关联))//拥有
					row.形式 = /*"?" + */str;
			}

			row.That根 = that;

			return row;
		}

		static public 模式 New派生行(Guid baserowid, int that = 字典_目标限定.空, bool 二元关联处理成This = false)
		{

			模式 baserow = Data.FindRowByID(baserowid);
			return New派生行(baserow, that, 二元关联处理成This);
		}


		static public string FindThatName(int that值)
		{
			PatternDataSet.字典_限定目标Row row = patternDataSet.字典_限定目标.FindByID(that值);
			if (row == null)
				return null;
			return row.名称;
		}

		static public int FindThatValue(string that名称)
		{
			foreach (DataRow row in patternDataSet.字典_限定目标)
				if (that名称 == (string)row["名称"])
					return (int)row["ID"];
			return 字典_目标限定.空;
		}

		static public string GetThatString(int that)
		{
			foreach (DataRow row in patternDataSet.字典_限定目标)
				if (that == (int)row["ID"])
					return (string)row["名称"];
			return "";
		}

		//在句子内找到目前实际的语言的根。
		static DataRow Find句内语言根(Dictionary<Guid, DataRow> 句内IDs, DataRow obj)
		{
			while (true)
			{
				Guid id = (Guid)obj["ParentID"];
				if (句内IDs.ContainsKey(id))//还在句内。
				{
					obj = 句内IDs[id];
					continue;
				}
				return obj;
			}
		}

		static public void 执行挂接记录(模式 parentrow, TreeObject childtree, string parentfield = null)
		{
			//一、语义形式进行调整
			if (parentfield != null)
				//父行的一个字段来挂接子树。
				typeof(模式).GetProperty(parentfield).SetValue(parentrow, childtree.语义根.ID);
			else
			{
				//让子树的[that]字段去挂接父行。是一种优化，视为从父行增加了一个虚拟的字段来挂接子树。
				typeof(模式).GetProperty(childtree.语义根列名).SetValue(childtree.语义根, parentrow.ID);

				#region 添加拥有形式行的算子形式的Guid到中心行的算子集合中
				//如果挂接的子树中， 每个模式编辑行的链接为拥有形式，且该形式为语言算子，则添加该语言算子到关联中心所指的模式编辑行的算子集合中
				//foreach (模式 row in childtree.rows.Values)
				//{
				//    Guid 一级关联 = Data.一级关联类型(row);
				//    Guid 二级关联 = Data.二级关联类型(row);

				//    if (!Data.拥有形式Guid.Equals(一级关联) || !Data.拥有语言算子Guid.Equals(二级关联))
				//        continue;

				//    模式 parentparentRow = FindRowByID((Guid)parentrow.ParentID);

				//    // 寻找中心
				//    模式 中心Row = null;
				//    switch (parentparentRow.That根)
				//    {
				//        case 字典_目标限定.A端:
				//            if (ThisGuid != parentparentRow.A端)
				//                中心Row = FindRowByID(parentparentRow.A端);
				//            else
				//                中心Row = parentparentRow;
				//            break;
				//        case 字典_目标限定.B端:
				//            if (ThisGuid != parentparentRow.B端)
				//                中心Row = FindRowByID(parentparentRow.B端);
				//            else
				//                中心Row = parentparentRow;
				//            break;
				//    }

				//    if (中心Row != null && !中心Row.算子集合List.Contains(row.B端))
				//        中心Row.算子集合List.Add(row.B端);
				//}
				#endregion
			}

			//二、形式组织进行调整。
			childtree.组织Parent根.ParentID = parentrow.ID;
		}

		// 挂接记录，两条记录已经创建并已经加入句子中(句内IDs)，在同一个表，都属于一个句子（对句子外的引用不用这个方法），新句子的that已经设置。
		// 两种挂接方法，一种是parentfield给出了的话，就是用父记录的parentfield引用子记录， 另一种是childrow指向父记录。

		static public void 挂接记录(模式 parentrow, 模式 childrow, string parentfield)
		{
			TreeObject childobj = new TreeObject(childrow);

			childobj.RecalcAllRoot();

			执行挂接记录(parentrow, childobj, parentfield);

		}



		static public 模式 FindRowByID(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			模式 item = Data.模式表.FindById(id);

			if (item == null)
				item = Data.模式编辑表.FindById(id);

			if (item == null)
			{
				item = Data.模式会话表.FindById(id);
			}
			return item;
		}

		public static bool Exist(Guid id)
		{
			if (id == Guid.Empty)
			{
				return false;
			}
			else
			{
				return Data.模式表.Exist(id) || Data.模式编辑表.Exist(id) || Data.模式会话表.Exist(id);
			}
		}

		public static bool 存在于模式表中(Guid id)
		{
			if (id == Guid.Empty)
				return false;
			else
				return Data.模式表.Exist(id);
		}

		public static bool IsThisOrNull(Guid id)
		{
			return Data.ThisGuid.Equals(id) || Data.NullGuid.Equals(id) || Data.NullParentGuid.Equals(id);
		}
		//static public PatternDataSet.模式编辑Row FindRowEditByID(Guid id)
		//{
		//    if (id == Guid.Empty)
		//        return null;

		//    return patternDataSet.模式编辑.FindByID(id);
		//}

		//static public PatternDataSet.模式Row FindRowByID(Guid id)
		//{
		//	if (id == Guid.Empty)
		//		return null;

		//	return patternDataSet.模式.FindByID(id);
		//}

		static public 模式基表 FindTableByID(Guid id)
		{
			if (id == Guid.Empty)
				return null;

			模式 row = 模式表.FindById(id);

			if (row != null)
				return 模式表;

			row = 模式编辑表.FindById(id);
			if (row != null)
				return 模式编辑表;

			row = 模式会话表.FindById(id);
			if (row != null)
				return 模式会话表;
			return null;
		}


		public static bool IsTextField(string fieldname)
		{
			if (fieldname == "形式" ||
				fieldname == "说明" ||
				fieldname == "序号"
				)
				return true;
			return false;
		}

		public static bool IsObjectField(string fieldname, bool 只返回可编辑的)
		{
			if (fieldname == "A端" ||
				fieldname == "B端" ||
				fieldname == "C端" ||
				fieldname == "连接" ||
				fieldname == "源记录" ||
				fieldname == "ParentID"
				)
				return true;

			return false;
		}

		public static void 递归AddIds(IList<模式> 所有表对象集合, Guid root)
		{
			DataIDS.Add(root);

			//var dr = 所有表对象集合.Where(r => r.ParentID == root).ToList();
			foreach (模式 row in Data.FindRowByID(root).端索引表_Parent)
				递归AddIds(所有表对象集合, row.ID);
		}

		//从父来调用各个子的。

		public static void 格式处理(模式 row, TheString strobj, int 语言)
		{
			//主要参数已经放置在strobj中，要把【的】等都加上。
			//strobj应该只有一个串。
			//   int 的 = (int)row["的"];
			//  if (的 == 字典_参数.的)
			// {
			//     if (语言 == 字典_语言.汉语)
			//         strobj.str1 = strobj.str1 + "的";
			//  }
		}


		public static void 递归删除记录树(模式基表 目标表, Guid id, bool 包含自己)
		{
			模式 row = 目标表.FindById(id);

			//var dr = 目标表.对象集合.Where(r => r.ParentID == id).ToList();
			while (row.端索引表_Parent.Count > 0)
			{
				模式 r = row.端索引表_Parent[0];
				递归删除记录树(目标表, (Guid)r.ID, true);
			}
			//foreach (模式 r in Data.FindRowByID(id).端索引表_Parent)
			//    递归删除记录树(目标表, (Guid)r.ID, true);

			if (包含自己)
				目标表.删除对象(row);
		}

		public static Guid 找到继承的关联(模式 row, Guid 关联ID)
		{
			while (true)
			{
				Guid id = (Guid)row.ID;
				if (基本关联Guid.Equals(id))
					return 关联ID;

				//var dr = 模式编辑表.对象集合.Where(r => r.A端 == id).ToList();
				foreach (模式 r in Data.FindRowByID(id).端索引表_A端)
				{
					//只应该有一个。
					if (是派生关联(关联ID, r) > 0)
						return (Guid)r.ID;
				}

				//dr = 模式会话表.对象集合.Where(r => r.A端 == id).ToList();
				//foreach (模式 r in dr)
				//{
				//    //只应该有一个。
				//    if (是派生关联(关联ID, r) > 0)
				//        return (Guid)r.ID;
				//}

				//dr = 模式表.对象集合.Where(r => r.A端 == id).ToList();
				//foreach (模式 r in dr)
				//{
				//    //只应该有一个。
				//    if (是派生关联(关联ID, r) > 0)
				//        return (Guid)r.ID;
				//}

				//找基类的。
				row = FindRowByID((Guid)row.源记录);
			}
		}

		static public Guid 取得端的值(模式 row, int that端)
		{
			switch (that端)
			{
				case 字典_目标限定.A端:
					return row.A端;
				case 字典_目标限定.B端:
					return row.B端;
				case 字典_目标限定.连接:
					return row.连接;
			}
			Data.Assert(false);
			return Guid.Empty;
		}

		public static void 替换一个端(模式 rootrow, int AB端)
		{
			模式基表 目标表 = FindTableByID(rootrow.ID);
			Guid id = rootrow.ID;
			Guid 拥有AB关联 = (AB端 == 字典_目标限定.A端) ? 找到继承的关联(rootrow, 关联拥有A端Guid) : 找到继承的关联(rootrow, 关联拥有B端Guid);

			模式 row1 = New派生行(拥有AB关联, 字典_目标限定.连接);

			row1.B端 = 取得端的值(rootrow, AB端);
			row1.A端 = id;
			row1.ParentID = id;
			row1.That根 = 字典_目标限定.A端;

			目标表.新加对象(row1);
			模式 row2 = 目标表.FindById(取得端的值(rootrow, AB端));
			while (row2 != null)
			{
				if (id.Equals(row2.ParentID))
				{
					row2.ParentID = row1.ID;
					break;
				}
				row2 = FindRowByID(row2.ParentID);
			}
		}

		//把收敛态的二元关联展开为展开态
		public static void 递归展开(IList<模式> 对象集合, 模式 rootrow)
		{
			Guid id = rootrow.ID;

			//var dr = 对象集合.Where(r => r.ParentID == id).ToList();

			foreach (模式 row in rootrow.端索引表_Parent)
				递归展开(对象集合, row);

			if ((int)rootrow.That根 != 字典_目标限定.连接)
				return;
			if ((int)rootrow.显隐 == 字典_显隐.附加)
				return;
			Guid 关联类型 = Data.一级关联类型(rootrow);
			if (关联类型.Equals(基本关联Guid) || 关联类型.Equals(拥有Guid) || 替代.可正向替代(关联类型))
			{
				替换一个端(rootrow, 字典_目标限定.A端);
				替换一个端(rootrow, 字典_目标限定.B端);

				rootrow.B端 = rootrow.源记录;
				rootrow.A端 = ThisGuid;
				rootrow.连接 = 属于Guid;
				rootrow.源记录 = rootrow.B端;
				rootrow.That根 = 字典_目标限定.A端;
			}
		}
		public static void 第一阶段展开设置角色及默认次序(模式 row)
		{

			递归清除非原始形式(row/*, true*/);

			IList<模式> 所有对象集合 = Data.FindTableByID(row.ID).对象集合;

			递归展开(所有对象集合, row);

			DataIDS.Clear();
			递归AddIds(所有对象集合, row.ID);

			递归自动设置语言角色和调序(row, 字典_语言角色.中心, Data.当前生成语言);

		}


		public static void 递归清除语义信息(模式 rootrow)
		{
			Guid id = (Guid)rootrow.ID;
			if (单句分析Guid.Equals(rootrow.B端) == false)
				rootrow.形式 = string.Empty;

			//var dr =
			//    Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == rootrow.ID).OrderBy(r => r.序号).ToList();

			foreach (模式 row in rootrow.端索引表_Parent)
				递归清除语义信息(row);
		}

		//假设离合谓语肯定是子节点的位置上，两者紧密在一起。
		public static 模式 查找离合谓语(模式 row)
		{
			Guid id = row.ID;

			//var dr = Data.FindTableByID(row.ID).对象集合.Where(r => r.ParentID == id);
			foreach (模式 r in row.端索引表_Parent)
			{
				if (id.Equals(r.A端) == false)
					continue;
				//值应该有一个。
				if (是派生关联(关联拥有离合谓语Guid, r) > 0)
					return r;
			}
			return null;
		}

		//public static void 递归自动设置语言角色和调序(DataRow rootrow, int 传递角色)
		//{
		//	Guid 根id = (Guid)rootrow["ID"];

		//	string s = "ParentID='" + 根id + "' ";
		//	DataRow[] dr = rootrow.Table.Select(s, "序号", DataViewRowState.CurrentRows);

		//	bool 序列化 = 能够序列化(rootrow);
		//	int 角色, 序号, 本记录角色;
		//	int c = dr.Count();
		//	//除了中心语和谓语，不允许别的参数的序号为0。

		//	//设置自己作为根的角色。

		//	//本记录角色 = 字典_语言角色.定语;
		//	//if (ThisGuid.Equals(rootrow["A端"]) && 是分类(Data.根模式(rootrow)))//只把[this属于**]的作为中心语。
		//	本记录角色 = 序列化 ? 字典_语言角色.谓语 : 字典_语言角色.中心;

		//	if (传递角色 == 0)//原始的根记录
		//	{
		//		rootrow["语言角色"] = 本记录角色;
		//		//严格说，应该以下边的正确，也就是作为根的谓语。不过那样处理太复杂，所以，把【根】和【叶子】都处理成同样的形式！
		//		//rootrow["语言角色"] = 序列化 ? 字典_语言角色.根谓语 : 字典_语言角色.根中心;
		//	}
		//	else
		//	{
		//		if (传递角色 > 0)//进行了传递。
		//			rootrow["语言角色"] = 传递角色;

		//		rootrow["语言角色"] = (int)rootrow["语言角色"] & 字典_语言角色.全部叶;

		//		if (dr.Count() > 0)//进行了传递。并且有子节点。
		//			rootrow["语言角色"] = (int)rootrow["语言角色"] | 本记录角色;
		//	}

		//	foreach (DataRow row in dr)
		//	{
		//		//取基类的角色。
		//		DataRow r1 = FindRowByID((Guid)row["源记录"]);
		//		角色 = (int)r1["语言角色"] & 字典_语言角色.全部叶;//先假定是叶子的，后边二次递归处理再计算是【中心根】还是【谓语根】。
		//		序号 = 取得语言序号((int)r1["序号"]);
		//		Guid 子id = (Guid)row["ID"];

		//		//【哑记录】就是自己占一个记录，但是参数都是指向内部的别的模式，所以自己不做形式的处理而是转给别人处理。而指向外部的，相当于本记录要处理，就不传递了。
		//		//目前判断依据是看父记录的A端和B端是否指向直接子记录，这样就是自己不处理，而已隐含是【等价关系】。可能不严谨。暂时先这样。
		//		if (子id.Equals(rootrow["A端"]) || 子id.Equals(rootrow["B端"]))//反过来看如果父记录的A端和B端指向子记录，那么父记录是一个【哑记录】，执行角色传递。
		//		{
		//			if (传递角色 != -1)
		//				角色 = 传递角色;
		//			else
		//				角色 = (int)rootrow["语言角色"];

		//			// rootrow["语言角色"] = 字典_语言角色.无;
		//		}
		//		else
		//		{
		//			传递角色 = -1;

		//			//如果不是哑记录传递的，那么该记录就要自己计算语言角色。
		//			if (序列化)//根是序列化的
		//			{
		//				//对汉语里边形容词等做谓语的处理，其实是省略了【是】【成为】等。比如【苹果红了】【苹果很甜】等。这时把宾语处理为【谓宾】，暂时这样处理。
		//				if ((short)rootrow["显隐"] == 字典_显隐.隐藏)//普通的隐藏只是隐藏本记录，不影响别的记录，而在根式序列化情况下，这个谓语的责任需要下级对象承担起来。
		//				{//这里判断依据是根对象是序列化的而且是隐藏的就是这种情况，因为【序列化的根对象】基本是不会隐藏的，也许要再加上对【连接】是否【是】【成为】的判断。
		//					if ((角色 & 字典_语言角色.一宾) != 0)//一般是原来的第一个宾语，因为这个连接一般是【属于】，比如【苹果【是】红色】，现在就把【红色】做谓语。
		//					{
		//						角色 = (int)rootrow["语言角色"];//进行角色传递。
		//						传递角色 = 角色;
		//						goto end;
		//						//   rootrow["语言角色"] = 字典_语言角色.无;
		//					}
		//				}

		//				if ((角色 & 字典_语言角色.中心) != 0)//中心0或者中心1。表示这个角色没有明确，不是主语、宾语等。所以要在这里动态分配角色。
		//					角色 = 序号 < 0 ? 字典_语言角色.前状 : 字典_语言角色.后状;
		//				//这后边应该对状语进行位置排列。
		//			}
		//			else//根不是序列化的
		//			{
		//				//这里暂时这样，可能不严谨。以后可能需要更严谨的分析。
		//				角色 = 字典_语言角色.中心;//先假设子记录自己是独立成为对象的（一般会是[this属于**]），也就是中心语。
		//				if (根id.Equals(row["A端"]) || 根id.Equals(row["B端"]))
		//					角色 = 字典_语言角色.前定;//如果子记录的A端和B端指向父，就假设子记录是附加修饰父的，就是定语。
		//				//if(ThisGuid.Equals(row["A端"]) && 是分类(Data.根模式(row)))//只把[this属于**]的作为中心语。
		//				//	角色 = 字典_语言角色.中心;
		//				//else
		//				//	角色 = 字典_语言角色.前定;//中文一般是前定。
		//			}
		//		}

		//	end:
		//		//中心概念的序号设置为0。
		//		if ((角色 & 字典_语言角色.中心) != 0)
		//			序号 = 0;

		//		row["语言角色"] = 角色;
		//		row["序号"] = 计算一级序号(角色) + 序号;

		//		递归自动设置语言角色和调序(row, 传递角色);
		//	}
		//	//序号 = 取得语言序号((int)rootrow["序号"]);
		//	//if ((角色 & 字典_语言角色.中心) != 0)
		//	//    序号 = 0;
		//	//rootrow["序号"] = 计算一级序号(角色) + 序号; 
		//}

		public static int 计算一个明确的默认的语言角色(模式 关联row, int 语言)
		{
			//// 这里临时对【松散并列】关联返回 【前句】语言角色，后期需要补充解析时对【前句】和【后句】语言角色的处理。
			//if (Data.是派生关联(Data.松散并列Guid, 关联row) > 0)
			//{
			//    return 字典_语言角色.前句;
			//}
			//其实可能多条记录，我们先就取第一条。
			int 语言角色 = 查找关联行的语言角色(FindRowByID((Guid)关联row.源记录), FindRowByID((Guid)关联row.A端), FindRowByID((Guid)关联row.B端), (int)关联row.That根, 语言);

			//语言角色 |= ((语言角色 & 字典_语言角色.主语) > 0 || (语言角色 & 字典_语言角色.宾语) > 0) ? 字典_语言角色.状语 : 字典_语言角色.定语;
			//if ((语言角色 & 字典_语言角色.主语) > 0 || (语言角色 & 字典_语言角色.宾语) > 0)
			//    语言角色 |= 字典_语言角色.状语;

			if ((语言角色 & 字典_语言角色.主语) > 0 || (语言角色 & 字典_语言角色.宾语) > 0)
				语言角色 |= 字典_语言角色.状语;


			List<int> 参数语言角色集合 = new List<int>();

			//这里取子记录，以后如果调整了角色为中心，那么可能还要修改。
			模式 参数对象 = null;
			foreach (var item in 关联row.端索引表_Parent)
			{
				if (是拥有形式(item))
					continue;
				参数对象 = item;
				break;
			}

			if (参数对象 == null)
				return 字典_语言角色.无;

			if (参数对象.显隐 == 字典_显隐.隐藏)
				参数语言角色集合.Add(字典_语言角色.全部);
			else
			{
				bool 使用公共语言 = false;

			做两次://公共语言也是可以的，但优先级低
				foreach (模式 row in Data.FindRowByID(参数对象.源记录).端索引表_A端)
				{
					if (Data.是拥有形式(row) == false || Data.是拥有语言算子形式行(row))
						continue;

					if (字典_语言.满足指定语言(语言, (int)row.语言) == false)
						continue;

					if (((int)row.语言 == 字典_语言.公语 && 使用公共语言 == false)
						|| ((int)row.语言 != 字典_语言.公语 && 使用公共语言 == true))//第一遍不考虑公共语言。
						continue;

					if (row.参数.概率分 <= 0)
						continue;

					参数语言角色集合.Add(row.语言角色);

				}
				if (使用公共语言 == false && !参数语言角色集合.Any())
				{
					使用公共语言 = true;
					goto 做两次;
				}
			}
			if (拥有形式集合.Count == 0)
				return 字典_语言角色.无;

			参数语言角色集合.Sort();
			foreach (int v in 参数语言角色集合)
			{
				if ((语言角色 & 字典_语言角色.前句) > 0 && (v & 字典_语言角色.前句) > 0)
					return 字典_语言角色.前句;
				if ((语言角色 & 字典_语言角色.前独) > 0 && (v & 字典_语言角色.前独) > 0)
					return 字典_语言角色.前独;
				if ((语言角色 & 字典_语言角色.主语) > 0 && (v & 字典_语言角色.主语) > 0)
					return 字典_语言角色.主语;
				//if ((语言角色 & 字典_语言角色.兼宾) > 0)
				//    return 字典_语言角色.兼宾;
				if ((语言角色 & 字典_语言角色.一宾) > 0 && (v & 字典_语言角色.一宾) > 0)
					return 字典_语言角色.一宾;
				if ((语言角色 & 字典_语言角色.二宾) > 0 && (v & 字典_语言角色.二宾) > 0)
					return 字典_语言角色.二宾;
				if ((语言角色 & 字典_语言角色.主被) > 0 && (v & 字典_语言角色.主被) > 0)
					return 字典_语言角色.主被;
				if ((语言角色 & 字典_语言角色.把宾) > 0 && (v & 字典_语言角色.把宾) > 0)
					return 字典_语言角色.把宾;
				if ((语言角色 & 字典_语言角色.谓宾) > 0 && (v & 字典_语言角色.谓宾) > 0)
					return 字典_语言角色.谓宾;
				if ((语言角色 & 字典_语言角色.前状) > 0 && (v & 字典_语言角色.前状) > 0)
					return 字典_语言角色.前状;
				if ((语言角色 & 字典_语言角色.后状) > 0 && (v & 字典_语言角色.后状) > 0)
					return 字典_语言角色.后状;
				if ((语言角色 & 字典_语言角色.前同) > 0 && (v & 字典_语言角色.前同) > 0)
					return 字典_语言角色.前同;
				if ((语言角色 & 字典_语言角色.后同) > 0 && (v & 字典_语言角色.后同) > 0)
					return 字典_语言角色.后同;
				if ((语言角色 & 字典_语言角色.得状) > 0 && (v & 字典_语言角色.得状) > 0)
					return 字典_语言角色.得状;
				if ((语言角色 & 字典_语言角色.中心) > 0 && (v & 字典_语言角色.中心) > 0)
					return 字典_语言角色.中心;
				if ((语言角色 & 字典_语言角色.句首) > 0 && (v & 字典_语言角色.句首) > 0)
					return 字典_语言角色.句首;
				if ((语言角色 & 字典_语言角色.句尾) > 0 && (v & 字典_语言角色.句尾) > 0)
					return 字典_语言角色.句尾;
				if ((语言角色 & 字典_语言角色.尾标) > 0 && (v & 字典_语言角色.尾标) > 0)
					return 字典_语言角色.尾标;
				if ((语言角色 & 字典_语言角色.前定) > 0 && (v & 字典_语言角色.前定) > 0)
					return 字典_语言角色.前定;
				if ((语言角色 & 字典_语言角色.后定) > 0 && (v & 字典_语言角色.后定) > 0)
					return 字典_语言角色.后定;
				if ((语言角色 & 字典_语言角色.从定) > 0 && (v & 字典_语言角色.从定) > 0)
					return 字典_语言角色.从定;
				if ((语言角色 & 字典_语言角色.后句) > 0 && (v & 字典_语言角色.后句) > 0)
					return 字典_语言角色.后句;
			}
			return 字典_语言角色.无;
		}

		public static void 递归自动设置语言角色和调序(模式 rootrow, int 传递角色, int 语言)
		{
			if (rootrow.显隐 == 字典_显隐.附加)
				return;

			Data.Assert(Data.ThisGuid.Equals(rootrow.A端));

			Guid 根id = (Guid)rootrow.ID;
			bool 序列化 = 能够序列化(Data.FindRowByID((Guid)rootrow.源记录));
			//除了中心语和谓语，不允许别的参数的序号为0。

			rootrow.语言角色 = 传递角色;
            rootrow.序号 = 字典_语言角色.计算一级序号(传递角色);
			//List<模式> dr = Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == 根id).OrderBy(r => r.序号).ToList();
			//if (dr.Any())//没有作传递。并且有子节点。
			//{
			//    //rootrow["语言角色"] = (int)rootrow["语言角色"] | (序列化 ? 字典_语言角色.根谓语 : 字典_语言角色.根中心);
			//}

			foreach (模式 关联row in rootrow.端索引表_Parent)
			{
				//Assert(Data.ThisGuid.Equals(关联row.A端) == false);


				//	if (序列化)//根是序列化的
				//	{
				//		//对汉语里边形容词等做谓语的处理，其实是省略了【是】【成为】等。比如【苹果红了】【苹果很甜】等。这时把宾语处理为【谓宾】，暂时这样处理。
				//		if ((short)rootrow["显隐"] == 字典_显隐.隐藏)//普通的隐藏只是隐藏本记录，不影响别的记录，而在根式序列化情况下，这个谓语的责任需要下级对象承担起来。
				//		{//这里判断依据是根对象是序列化的而且是隐藏的就是这种情况，因为【序列化的根对象】基本是不会隐藏的，也许要再加上对【连接】是否【是】【成为】的判断。
				//			if ((角色 & 字典_语言角色.一宾) != 0)//一般是原来的第一个宾语，因为这个连接一般是【属于】，比如【苹果【是】红色】，现在就把【红色】做谓语。
				//			{
				//				角色 = (int)rootrow["语言角色"];//进行角色传递。
				//				传递角色 = 角色;
				//				goto end;
				//			}
				//		}
				//		if ((角色 & 字典_语言角色.中心) != 0)//中心0或者中心1。表示这个角色没有明确，不是主语、宾语等。所以要在这里动态分配角色。
				//			角色 = 序号 < 0 ? 字典_语言角色.前状 : 字典_语言角色.后状;
				//		//这后边应该对状语进行位置排列。
				//	}
				//	else//根不是序列化的
				//	{
				//		//这里暂时这样，可能不严谨。以后可能需要更严谨的分析。
				//		角色 = 字典_语言角色.中心;//如果没有指向，假设子记录自己是独立成为对象的（一般会是[this属于**]），也就是中心语。
				//		if (根id.Equals(row["A端"]) || 根id.Equals(row["B端"]))
				//			角色 = 字典_语言角色.前定;//先看子记录的A端和B端是否指向父，如果指向，就假设子记录是附加修饰父的，就是定语
				//		//if(ThisGuid.Equals(row["A端"]) && 是分类(Data.根模式(row)))//只把[this属于**]的作为中心语。
				//		//	角色 = 字典_语言角色.中心;
				//		//else
				//		//	角色 = 字典_语言角色.前定;//中文一般是前定。
				//	}

				//}

				//end:

				//中心概念的序号设置为0。
				//if ((角色 & 字典_语言角色.中心) != 0)
				//	序号 = 0;

				//int 计算序号角色 = 原始角色 != 0 ? 原始角色 : 角色;
				if (关联row.显隐 == 字典_显隐.附加)
					continue;
				int 语言角色 = 计算一个明确的默认的语言角色(关联row, 语言);

				if (语言角色 == 字典_语言角色.无)
					continue;

				//if ((语言角色 & 字典_语言角色.同位) != 0 && (short)rootrow["显隐"] == 字典_显隐.隐藏)
				//    语言角色 = 传递角色;

				关联row.语言角色 = 语言角色;

				关联row.序号 = 字典_语言角色.计算一级序号(语言角色); //+ 序号;//计算序号不用传递的语言角色。

                //// 计算序号时考虑关联row的拥有语言角色行
                //var 关联row源记录行 = Data.FindRowByID(关联row.源记录);
                //模式 关联拥有角色row = null;
                //foreach (var item in 关联row源记录行.端索引表_源记录)
                //{
                //    if (item.连接 == 拥有语言角色Guid)
                //    {
                //        if (字典_语言.满足指定语言(语言, (int)item.语言) == false)
                //            continue;
                //        if ((int)item.语言 == 字典_语言.公语)
                //            continue;
                //        关联拥有角色row = item;
                //        break;
                //    }
                //}

                //if (关联拥有角色row == null)
                //{
                //    foreach (var item in 关联row源记录行.端索引表_源记录)
                //    {
                //        if (item.连接 == 拥有语言角色Guid)
                //        {
                //            if (字典_语言.满足指定语言(语言, (int)item.语言) == false)
                //                continue;
                //            if ((int)item.语言 == 字典_语言.公语)                               
                //                关联拥有角色row = item;
                //            break;
                //        }
                //    }
                //}

                //if (关联拥有角色row != null && (关联拥有角色row.语言角色 & 关联row.语言角色) > 0)
                //{
                //    关联row.序号 += 关联拥有角色row.序号;
                //}


				//List<模式> dr参数 = Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == 关联row.ID).ToList();
				Assert(关联row.端索引表_Parent.Count == 1);

				foreach (模式 参数row in 关联row.端索引表_Parent)
					递归自动设置语言角色和调序(参数row, 语言角色, 语言);
			}
		}

		public static void 递归调序(模式 rootrow)
		{
			if (是拥有形式(rootrow))
				return;

			//Guid 根id = rootrow.ID;

			//List<模式> dr = Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == 根id).OrderBy(r => r.序号).ToList();


			foreach (模式 关联row in rootrow.端索引表_Parent)
			{
				if (是拥有形式(关联row))
					continue;
				关联row.序号 = 字典_语言角色.计算一级序号(关联row.语言角色); //+ 序号;//计算序号不用传递的语言角色。

				//List<模式> dr参数 = Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == 关联row.ID).ToList();

				//foreach (模式 参数row in 关联row.端索引表_Parent)
				递归调序(关联row);
			}
		}
		public static void 第二阶段根据语序微调角色(模式 row)
		{
			递归清除非原始形式(row/*, true*/);

			DataIDS.Clear();

			IList<模式> 所有对象集合 = Data.FindTableByID(row.ID).对象集合;

			递归AddIds(所有对象集合, row.ID);

			递归_根据位置微调语言角色(row, row.语言角色);


			//重计算打分(row);

			return;
		}


		public static void 第三阶段构造表现形式(模式 row, bool 是引用记录)
		{
			递归清除非原始形式(row/*, true*/);

			DataIDS.Clear();

			IList<模式> 所有对象集合 = Data.FindTableByID(row.ID).对象集合;

			递归AddIds(所有对象集合, row.ID);

			递归_构造语言表现形式(row, Data.当前生成语言, 是引用记录);

			//重计算打分(row);

			return;
		}


		public static string 递归合成语言串(模式 root, int 语言)
		{
			string 前串 = null;
			string 后串 = null;
			var dr = root.端索引表_Parent.OrderBy(r => r.序号);

			#region 添加算子到语义中心
			root.算子集合List.Clear();
			foreach (模式 subRow in dr)
			{
				if (是拥有语言算子形式行(subRow))
				{
					var 算子 = Data.FindRowByID(subRow.源记录).B端;
					if (!root.算子集合List.Contains(算子))
					{
						root.算子集合List.Add(算子);
					}
				}
			}
			#endregion

			#region 计算前串和后串
			foreach (模式 row in dr)
			{
				if (root.算子集合List.Any() && Data.是拥有形式(row))
				{
					continue;
				}

				string ss = 递归合成语言串(row, 语言);

				if (string.IsNullOrEmpty(ss))
					continue;

				if ((int)row.序号 < 0)
					前串 = 合并串(前串, ss, 语言);
				else
					后串 = 合并串(后串, ss, 语言);
			}
			#endregion

			string ThisString = 为一条记录计算形式串(root, 语言);

			//合并子串为整体串。
			string parentstring = string.Empty;
			string 原字串 = string.Empty;
			string 结果串 = string.Empty;

			if (!root.算子集合List.Any())
			{
				原字串 = (结果串 = ThisString);
			}
			else
			{
				#region 获取语义中心行拥有的字符串形式
				foreach (模式 subRow in dr)
				{
					if (Data.是拥有形式(subRow) && !Data.是拥有语言算子形式行(subRow))
					{
						原字串 = subRow.形式;
						break;
					}
				}
				#endregion

				if (!string.IsNullOrEmpty(原字串))
				{
					if (root.算子集合List.Count == 1 && root.算子集合List.Contains(算子不定冠词Guid))
					{
						结果串 = 合并串(合并串(前串, 原字串, 语言), 后串, 语言);
						WordProcessorEnglish.英文名词a与an处理(ref 结果串);
					}
					else
					{
						结果串 = 根据中心行拥有的形式字符串和算子Guid集合获取中心行形式(原字串, root.算子集合List, 语言);
					}
				}

				#region 英语从句处理
				//if (root.语言角色 == 字典_语言角色.从定 && 语言 == 字典_语言.英语)
				//{					
				//    string 从句引导词 = "that";
				//    前串 = string.IsNullOrEmpty(前串) ? 从句引导词 : string.Format("{0} {1}", 从句引导词, 前串);
				//}
				#endregion
			}

			if (root.语言角色 != 字典_语言角色.中心)
			{
				#region 不定冠词处理(算子集合仅包含不定冠词算子)
				if (root.算子集合List.Count == 1 && root.算子集合List.Contains(算子不定冠词Guid))
					parentstring = 结果串;
				#endregion
				else
					parentstring = 合并串(合并串(前串, 结果串, 语言), 后串, 语言);
			}
			else
			{
				#region 句式处理
				模式 句式Row = null;
				Guid 关系扮演句子Guid = new Guid("56ec7f34-57f5-43df-8855-4d30aec9943b");
				foreach (var row in dr)
				{
					if (关系扮演句子Guid.Equals((Guid)row.源记录))
					{
						句式Row = FindRowByID((Guid)row.B端);
						break;
					}
				}

				if (句式Row == null || !疑问句Guid.Equals(句式Row.源记录))
					parentstring = 合并串(合并串(前串, 结果串, 语言), 后串, 语言);
				else
				{
					#region 英语疑问句处理
					if (语言 != 字典_语言.英语)
						parentstring = 合并串(合并串(前串, 结果串, 语言), 后串, 语言);
					else
					{
						var 中心结果串前部分 = 结果串.Split(' ')[0];  // 为中心字符串的第一个单词
						var 中心结果串后部分 = 结果串.Length > 中心结果串前部分.Length ? 结果串.Substring(中心结果串前部分.Length + 1) : string.Empty;
						//英语环境下一般疑问句处理
						//1. 如果中心字符串为系动词am is are was were, 则直接将中心词提前
						var 系动词数组 = new string[] { "am", "is", "are", "was", "were" };
						if (系动词数组.Contains(中心结果串前部分))
							parentstring = 合并串(合并串(合并串(中心结果串前部分, 前串, 语言), 中心结果串后部分, 语言), 后串, 语言);
						//2. 如果中心字符串包含情态动词， 则将情态动词提前
						var 情态动词数组 = new string[] { "can", "could", "will", "would", "may", "might", "must", "shall", "should" };
						if (情态动词数组.Contains(中心结果串前部分))
							parentstring = 合并串(合并串(合并串(中心结果串前部分, 前串, 语言), 中心结果串后部分, 语言), 后串, 语言);
						//3. 完成时处理
						var 完成时数组 = new string[] { "has", "have", "had" };
						if (完成时数组.Contains(中心结果串前部分) && !string.IsNullOrEmpty(中心结果串后部分))  // has, have, had有可能是代表实意动词
							parentstring = 合并串(合并串(合并串(中心结果串前部分, 前串, 语言), 中心结果串后部分, 语言), 后串, 语言);
						//4. 当动词为实意词， 时态为一般现在时，或者一般过去时， 句首加助动词do，或did, 动词使用原型
						if (string.IsNullOrEmpty(parentstring) && string.IsNullOrEmpty(中心结果串后部分) && !string.IsNullOrEmpty(结果串))
						{
							TenseState? 时态状态 = TenseState.Normal;
							TenseTiming? 时态时间 = TenseTiming.Now;
							Data.尝试从算子集集合中获取时态(root.算子集合List, ref 时态状态, ref 时态时间, true);

							if (时态时间 == TenseTiming.Past)  // 一般过去时
								parentstring = 合并串(合并串(合并串("did", 前串, 语言), 原字串, 语言), 后串, 语言);
							else  // 一般现在时
							{
								if (root.算子集合List.Contains(算子第三人称Guid) && root.算子集合List.Contains(算子人称单数Guid))
									parentstring = 合并串(合并串(合并串("does", 前串, 语言), 原字串, 语言), 后串, 语言);
								else
									parentstring = 合并串(合并串(合并串("do", 前串, 语言), 原字串, 语言), 后串, 语言);
							}
						}
					}
					#endregion
				}

				#endregion
			}


			ThisString = parentstring;//加入第二个整体的串。

			if (单句分析Guid.Equals(root.B端))
				goto end;

			root.形式 = ThisString;

		end:
			if (Data.是拥有语言算子形式行(root))
				return string.Empty;
			else
				return ThisString;
		}


		private static bool 是拥有语言算子形式行(模式 row)
		{
			var 一级关联类型 = Data.一级关联类型(row);
			var 二级关联类型 = Data.二级关联类型(row);
			if (一级关联类型 == Data.拥有形式Guid && 二级关联类型 == Data.拥有语言算子Guid)
			{
				return true;
			}
			return false;
		}

		private static void 递归查找和为中心行添加时态和人称算子(模式 row, 模式 中心Row)
		{
			if (row.语言角色 != 字典_语言角色.主语 && row.语言角色 != 字典_语言角色.后状 && row.语言角色 != 字典_语言角色.前状 && row.语言角色 != 字典_语言角色.状语)
				return;

			foreach (Guid 算子Guid in row.算子集合List)
			{
				if (!时态和人称算子集合.Contains(算子Guid))
					continue;

				if (!中心Row.算子集合List.Contains(算子Guid))
					中心Row.算子集合List.Add(算子Guid);
			}

			//找出所有子记录。
			// List<模式> dr = Data.get模式编辑表().对象集合.Where(r => r.ParentID == row.ID).OrderBy(r => r.序号).ToList();
			foreach (模式 subRow in row.端索引表_Parent.OrderBy(r => r.序号))
				递归查找和为中心行添加时态和人称算子(subRow, 中心Row);
		}

		private static string 根据中心行拥有的形式字符串和算子Guid集合获取中心行形式(string 中心字符串, List<Guid> 算子形式Guid集合, int 语言)
		{
			//string resultString = 中心字符串.Trim(new[] { '\'', ' ' }).Trim(new[] { '\'', ' ' }); // 去除前后的空格或引号
			string resultString = 中心字符串; // 去除前后的空格或引号
			bool 是否已处理 = false;


			// TODO: 第一步处理通过查询表，该表记录中心行拥有的形式字符串在不同的语言算子下表现形式， 如果查询到，设置是否已处理为true

			if (是否已处理)
			{
				return resultString;
			}

			if (语言 == 字典_语言.英语)  // 先处理英语的情况
			{
				if (算子形式Guid集合.Contains(算子人称单数Guid) || 算子形式Guid集合.Contains(算子人称复数Guid)
					|| 算子形式Guid集合.Contains(算子第一人称Guid) || 算子形式Guid集合.Contains(算子第二人称Guid) || 算子形式Guid集合.Contains(算子第三人称Guid))
				{
					TenseState? 时态状态 = null;  // 时态状态默认为一般
					TenseTiming? 时态时间 = null;   // 时态时间默认为现在

					尝试从算子集集合中获取时态(算子形式Guid集合, ref 时态状态, ref 时态时间, true);
					if (时态状态.HasValue || 时态时间.HasValue)
						resultString = 计算包含人称及其单复数的动词在不同时态下形式(resultString, 算子形式Guid集合, 时态状态 ?? TenseState.Normal, 时态时间 ?? TenseTiming.Now);
				}
				else
				{
					TenseState? 时态状态 = null;  // 时态状态默认为一般
					TenseTiming? 时态时间 = null;   // 时态时间默认为现在

					尝试从算子集集合中获取时态(算子形式Guid集合, ref 时态状态, ref 时态时间, false);
					if (时态状态.HasValue || 时态时间.HasValue)
					{
						resultString = 计算包含人称及其单复数的动词在不同时态下形式(resultString, 算子形式Guid集合, 时态状态 ?? TenseState.Normal, 时态时间 ?? TenseTiming.Now);
					}
					else   // 不含人称和时态算子的情况
					{
						if (算子形式Guid集合.Count() == 1)
						{
							if (算子形式Guid集合.Contains(比较级算子Guid))    // 处理形容词和副词比较级
							{

							}
							else if (算子形式Guid集合.Contains(最高级算子Guid))    // 处理形容词或副词最高级
							{

							}
						}
					}
				}
			}

			return resultString;
		}

		private static void 尝试从算子集集合中获取时态(List<Guid> 算子形式Guid集合, ref TenseState? 时态状态, ref TenseTiming? 时态时间, bool 设置默认值)
		{
			const TenseState 时态状态默认值 = TenseState.Normal;
			const TenseTiming 时态时间默认值 = TenseTiming.Now;

			if (算子形式Guid集合.Contains(算子时态状态进行Guid))
				时态状态 = TenseState.Doing;
			else if (算子形式Guid集合.Contains(算子时态状态完成Guid))
				时态状态 = TenseState.Finish;
			else if (算子形式Guid集合.Contains(算子时态状态完成进行Guid))
				时态状态 = TenseState.FinishDoing;
			else if (算子形式Guid集合.Contains(算子时态状态一般Guid))
				时态状态 = TenseState.Normal;

			if (算子形式Guid集合.Contains(算子时态时间过去Guid))
				时态时间 = TenseTiming.Past;
			else if (算子形式Guid集合.Contains(算子时态时间将来Guid))
				时态时间 = TenseTiming.Future;
			else if (算子形式Guid集合.Contains(算子时态时间过去将来Guid))
				时态时间 = TenseTiming.PastFuture;
			else if (算子形式Guid集合.Contains(算子时态时间现在Guid))
				时态时间 = TenseTiming.Now;

			if (设置默认值 == true)
			{
				if (!时态状态.HasValue)
					时态状态 = 时态状态默认值;
				if (!时态时间.HasValue)
					时态时间 = 时态时间默认值;
			}
		}


		private static string 计算包含人称及其单复数的动词在不同时态下形式(string resultString, List<Guid> 算子形式Guid集合, TenseState 时态状态, TenseTiming 时态时间)
		{
			// 人称代词即单复数
			if (算子形式Guid集合.Contains(算子第一人称Guid))
			{
				if (算子形式Guid集合.Contains(算子人称单数Guid))
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "I", IsComplex = false });
				else // 不含单数算子时以复数处理
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "we", IsComplex = true });

			}
			else if (算子形式Guid集合.Contains(算子第二人称Guid))
			{
				if (算子形式Guid集合.Contains(算子人称单数Guid))
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "you", IsComplex = false });
				else
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "you", IsComplex = true });
			}
			else if (算子形式Guid集合.Contains(算子第三人称Guid))
			{
				if (算子形式Guid集合.Contains(算子人称单数Guid))
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "it", IsComplex = false });
				// 这里subject可以是 he, she, it, 为方便起见，统一记为it
				else
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "they", IsComplex = true });
			}
			else if (!算子形式Guid集合.Contains(算子第一人称Guid) && !算子形式Guid集合.Contains(算子第二人称Guid) &&
					 !算子形式Guid集合.Contains(算子第三人称Guid)) // 不含人称时，假定为第三人称
			{
				if (算子形式Guid集合.Contains(算子人称单数Guid))
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "it", IsComplex = false });
				else
					resultString = WordProcessorEnglish.GetVerbTenseFormat(resultString, 时态状态, 时态时间, new VerbTenseFormatInfo() { Subject = "they", IsComplex = true });
			}
			return resultString;
		}

		public static string 为一条记录计算形式串(模式 row, int 语言)
		{
			if (字典_语言.满足指定语言(语言, (int)row.语言) && (是拥有形式(row) || 是介词或者串(row, true, true, true)))
			{
				return row.形式;
			}
			else
			{
				return string.Empty;
			}
		}

		public static TheString 旧版本为一条记录计算形式串(模式 row, bool 直接返回 = false)
		{
			int 语言 = 当前解析语言;

			//一、首先从本身记录【形式】字段里边查找。
			TheString strobj = new TheString((string)row.形式);

			if (strobj.有嵌入串)//找到了数据。
				return strobj;

			if (直接返回)
				return strobj;

			if (是拥有形式(row) && ThisGuid.Equals(row.B端) == false)
			{
				//本记录本身就是拥有形式的表现，比如拥有介词。
				模式 row4 = Data.FindRowByID((Guid)row.B端);
				if (row4 != null && 字典_语言.满足指定语言(语言, (int)row4.语言))
				{
					TheString strobj4 = 旧版本为一条记录计算形式串(row4, true);
					if (strobj4.有嵌入串)
					{
						strobj.设置嵌入串(strobj4);
						格式处理(row, strobj, 语言);
						return strobj;
					}
				}
			}

			//二、查找本记录所具有的【拥有形式】的记录。

			int that = (int)row.That根;
			Guid id = Guid.Empty;
			switch (that)
			{
				case 字典_目标限定.A端:
					id = (Guid)row.B端;
					break;
				case 字典_目标限定.B端:
					id = (Guid)row.A端;
					break;
				case 字典_目标限定.连接:
					id = (Guid)row.源记录;
					break;
			}

			//如果这个子对象在本句子内部，那么直接返回，让该对象自己去处理。
			if (DataIDS.Contains(id))
				return strobj;

			//List<模式> drs = 模式表.对象集合.Where(r => r.A端 == id && r.语言 == 语言 && r.连接 == 拥有形式Guid).ToList();

			foreach (模式 row1 in Data.FindRowByID(id).端索引表_A端)
			{
				if (row1.语言 == 语言 && row1.连接 == 拥有形式Guid)
				{
					TheString strobj1 = 旧版本为一条记录计算形式串(row1, true);

					//只要有数据，就可以。不要求原始串
					if (strobj1.有嵌入串)
					{
						strobj.设置嵌入串(strobj1);
						格式处理(row, strobj, 语言);
						return strobj;
					}

					if (ThisGuid.Equals(row1.B端) == false)//一条【拥有形式】记录的B字段不是[this]，应该是指向一个引用串。
					{
						模式 row2 = FindRowByID((Guid)row1.B端);
						if (row2 == null || 字典_语言.满足指定语言(语言, (int)row2.语言) == false)
							continue;

						TheString strobj2 = 旧版本为一条记录计算形式串(row2, true);
						if (strobj2.有嵌入串)
						{
							strobj.设置嵌入串(strobj2);
							格式处理(row, strobj, 语言);
							return strobj;
						}
					}
				}
			}

			//三、仍然没有找到，那么从本身记录向自己的基类去遍历找。
			模式 r3 = FindRowByID(id);

			TheString strobj3 = 旧版本为一条记录计算形式串(r3);
			strobj.设置嵌入串(strobj3);
			格式处理(row, strobj, 语言);
			return strobj;
		}

		public static void 递归清除非原始形式(模式 root)
		{
			List<模式> 删除对象集合 = new List<模式>();
			递归清除非原始形式(root, ref 删除对象集合);
			while (删除对象集合.Count > 0)
			{
				Data.get模式编辑表().删除对象(删除对象集合[0]);
				删除对象集合.RemoveAt(0);
			}
		}
		public static void 递归清除非原始形式(模式 root, ref List<模式> 删除对象集合)//*, bool 清除语言部件*/)
		{
			Guid id = root.ID;

			//开始的时候，大家都是中心语！也就是相互之间是独立的！！然后需要的合并成为谓语。           
			//找出所有子记录。
			//List<模式> dr = Data.FindTableByID(root.ID).对象集合.Where(r => r.ParentID == id).ToList();

			foreach (模式 row in root.端索引表_Parent)
				递归清除非原始形式(row, ref 删除对象集合);//*, 清除语言部件*/);

			if (root.端索引表_Parent.Count == 0 && 是拥有形式(root))
			{
				//注意，也许以后要考虑一些接近语义对象的后添加语言部件有可能不删除。
				{
					//Data.FindTableByID(root.ID).删除对象(root);
					删除对象集合.Add(root);
					return;
				}
			}

			if (单句分析Guid.Equals(root.B端))
				return;

			TheString strobj = new TheString(root.形式);
			strobj.清理嵌入串();
			root.形式 = strobj.ToString();
		}

		public static void 递归_根据位置微调语言角色(模式 rootrow, int 语言角色)
		{
			//if (是拥有形式(rootrow))
			//    return;
			//rootrow = FindRowByID((Guid)rootrow["源记录"]);
			//bool 允许兼宾 = (语言角色 & 字典_语言角色.兼宾) > 0;

			if (Data.ThisGuid.Equals(rootrow.A端) == false)
				return;
			//Data.Assert(Data.ThisGuid.Equals(rootrow.A端));

			rootrow.语言角色 = 语言角色;

			IList<模式> 所有对象集合 = Data.FindTableByID(rootrow.ID).对象集合;

			//找出所有子记录。
			//List<模式> dr = 所有对象集合.Where(r => r.ParentID == rootrow.ID).OrderBy(r => r.序号).ToList();

			//if (单句分析Guid.Equals(rootrow["B端"]))
			//	goto end;

			//if (dr.Count() == 1)//在只有一条子记录，并且本记录的A端或者B端等于子记录的，就认为本记录是哑记录，进行传递。
			//{//这个判断哑记录的方法不一定完美，暂时可以用。
			//    Guid id = (Guid)dr[0]["ID"];
			//    if (id.Equals(root["A端"]) || id.Equals(root["B端"]))
			//    {
			//        if ((字典_目标限定)root["that根"] == 字典_目标限定.B端)
			//        {
			//            DataRow 原始中心 = FindRowByID((Guid)root["A端"]);
			//            if (能够序列化(原始中心))
			//                root["语言角色"] = 字典_语言角色.从定;
			//        }
			//        dr[0]["语言角色"] = root["语言角色"];
			//        goto end;
			//    }
			//}

			//if (替代.是分类或聚合或属拥(一级关联类型(rootrow)))
			//{
			//	rootrow["语言角色"] = 字典_语言角色.同位;
			//	goto end;
			//}
			var dr = rootrow.端索引表_Parent.OrderBy(r => r.序号);
			foreach (模式 关联row in dr)
			{
				Data.Assert(Data.ThisGuid.Equals(关联row.A端) == false);

				if (关联row.That根 == 字典_目标限定.B端)
				//if (Data.反向聚合Guid.Equals(二级关联类型(关联row)) == false)
				{
					关联row.语言角色 = 字典_语言角色.从定;
					goto end;
				}

				if (((int)关联row.语言角色 & 字典_语言角色.同位) != 0 && (short)rootrow.显隐 == 字典_显隐.隐藏)
				//if ((short)rootrow["显隐"] == 字典_显隐.隐藏)
				{
					关联row.语言角色 = rootrow.语言角色;
					goto end;
				}

				//Guid 子id = (Guid)关联row["ID"];
				//if (子id.Equals(rootrow["A端"]) || 子id.Equals(rootrow["B端"]))//如果父记录的A端和B端指向子记录，那么父记录是一个【哑记录】，执行角色传递。
				//{
				//	Assert(dr.Count() == 1);
				//	关联row["语言角色"] = rootrow["语言角色"];
				//	goto end;
				//}
			}
			int 谓语index = 0;
			int 主语序号 = 无效序号;
			int 一宾序号 = 无效序号;
			int 二宾序号 = 无效序号;
			int 得状序号 = 无效序号;
			int 主语 = 字典_语言角色.前状;//设置基本取值。
			int 一宾 = 字典_语言角色.后状;
			int 二宾 = 字典_语言角色.后状;
			int 得状 = 字典_语言角色.得状;
			//递归查找各个参数可能的语言成分。

			//除了谓语，不允许别的参数的序号为0。
			foreach (模式 关联row in dr)
			{
				//Guid 连接ID = (Guid)row[获取连接类型];
				//DataRow r1 = Data.FindRowByID(连接ID);
				// if (row["语言角色"] != r1["语言角色"])
				//     row["语言角色"] = r1["语言角色"];
				if ((int)关联row.序号 < 0)
					谓语index++;

				//把根的部分去除后再计算。
				//int v = (int)row["语言角色"] & 字典_语言角色.全部叶;
				int v = (int)关联row.语言角色;

				int 序号 = (int)关联row.序号;
				//主语和宾语都假设只有一个。所以只找第一个。
				if (v == 字典_语言角色.主语 && 主语序号 == 无效序号)
					主语序号 = 序号;
				//if ((允许兼宾 ? v == 字典_语言角色.兼宾 : v == 字典_语言角色.一宾) && 一宾序号 == 无效序号)
				//    一宾序号 = 序号;
				if (v == 字典_语言角色.一宾 && 一宾序号 == 无效序号)
					一宾序号 = 序号;
				if (v == 字典_语言角色.二宾 && 二宾序号 == 无效序号)
					二宾序号 = 序号;
				if (v == 字典_语言角色.得状 && 得状序号 == 无效序号)
					得状序号 = 序号;
			}

			if (主语序号 != 无效序号 && 主语序号 > 0)
				主语 = 字典_语言角色.后状;
			if (一宾序号 != 无效序号 && 一宾序号 < 0)
				一宾 = 字典_语言角色.前状;
			if (二宾序号 != 无效序号 && 二宾序号 < 0)
				二宾 = 字典_语言角色.前状;

			//算法是逐步提升的过程。开始是状语，然后满足一定条件才升级。
			//够到更高级的省略才用。

			//处理主语
			if (主语序号 != 无效序号)//有主语
			{
				if (主语序号 < 0)//主语在谓语前
					主语 = 字典_语言角色.主语;//。
				else
					主语 = 字典_语言角色.后状;//。
				//在谓语前就是主语，【把】【被】都是由宾语去处理。
				//在谓语后就是状语形式，不用【被】，只用【从】等，【被】是语言上的一种混用而已！英语里边就是[by]等。
			}

			//处理一宾
			if (一宾序号 != 无效序号)//有一宾
			{
				if (一宾序号 > 0)//一宾在谓语后
				{
					if (二宾序号 == 无效序号 || 二宾序号 < 0 || 二宾序号 > 一宾序号)//一宾和谓语之间没有二宾
					{
						if (主语序号 == 无效序号 || 主语序号 < 0 || 主语序号 > 一宾序号)//一宾和谓语之间也没有主语
							//一宾 = 允许兼宾 ? 字典_语言角色.兼宾 : 字典_语言角色.一宾;//一宾紧挨在谓语后边，使用最高级的一宾。【谓】【一宾】
							一宾 = 字典_语言角色.一宾;//一宾紧挨在谓语后边，使用最高级的一宾。【谓】【一宾】
						else//主语在谓语和一宾之间。
							一宾 = 字典_语言角色.把宾;//一宾在主语后边，使用中间级的把宾【谓】被【主】把【一宾】
					}
					else//二宾在一宾和谓语之间
					{
						if (主语序号 != 无效序号 && 主语序号 > 二宾序号 && 主语序号 < 一宾序号)//主语在在二宾和一宾之间
							一宾 = 字典_语言角色.把宾;//一宾在主语后边，使用中间级的把宾【谓】【二宾】被【主】把【一宾】
						else//主语不在中间。
							一宾 = 字典_语言角色.后状;//一宾在二宾后边，使用原始级【谓】【二宾】【一宾】
					}
				}
				else//一宾在谓语前
				{
					if (二宾序号 == 无效序号 || 二宾序号 > 一宾序号)//一宾前没有二宾
					{
						if (主语序号 == 无效序号 || 主语序号 > 一宾序号)//一宾前也没有主语
							一宾 = 字典_语言角色.主被;//一宾在最前，使用中间级的被宾。【一宾】被
						else//主语在一宾之前。
							一宾 = 字典_语言角色.把宾;//一宾在主语后边，使用中间级的把宾【主】把【一宾】
					}
					else//一宾前有二宾
					{
						if (主语序号 != 无效序号 && 主语序号 > 二宾序号 && 主语序号 < 一宾序号)//主语在在二宾和一宾之间
							一宾 = 字典_语言角色.把宾;//一宾在主语后边，使用中间级的把宾【二宾】被【主】把【一宾】
						else//主语不在中间。
							一宾 = 字典_语言角色.前状;//一宾在二宾后边，使用原始级【二宾】【一宾】
					}
				}
			}

			//处理二宾
			if (二宾序号 != 无效序号)//有二宾
			{
				if (二宾序号 > 0)//一宾在谓语后
				{
					if (一宾序号 == 无效序号 || 一宾序号 < 0 || 一宾序号 > 二宾序号)//二宾和谓语之间没有一宾
					{
						if (主语序号 == 无效序号 || 主语序号 < 0 || 主语序号 > 二宾序号)//二宾和谓语之间也没有主语
							二宾 = 字典_语言角色.后状;//二宾紧挨在谓语后边，使用原始状态。
						else//主语在谓语和二宾之间。
							二宾 = 字典_语言角色.后状;//二宾在主语后边，使用原始状态。
					}
					else//一宾在二宾和谓语之间
					{
						if (主语序号 != 无效序号 && 主语序号 > 一宾序号 && 主语序号 < 二宾序号)//主语在在二宾和一宾之间
							二宾 = 字典_语言角色.后状;//二宾在主语后边，使用原始状态
						else//主语不在中间。
							二宾 = 字典_语言角色.二宾;//二宾在一宾后边，使用最高级的
					}
				}
				else//二宾在谓语前
				{
					if (一宾序号 == 无效序号 || 一宾序号 > 二宾序号)//二宾前没有一宾
					{
						if (主语序号 == 无效序号 || 主语序号 > 二宾序号)//二宾前也没有主语
							二宾 = 字典_语言角色.主被;//二宾在最前，使用中间级的被宾。【二宾】被
						else//主语在二宾之前。
							二宾 = 字典_语言角色.把宾;//二宾在主语后边，使用中间级的把宾【主】把【二宾】
					}
					else//二宾前有一宾
					{
						if (主语序号 != 无效序号 && 主语序号 > 一宾序号 && 主语序号 < 二宾序号)//主语在在二宾和一宾之间
							二宾 = 字典_语言角色.把宾;//二宾在主语后边，使用中间级的把宾【一宾】被【主】把【二宾】
						else//主语不在中间。
							二宾 = 字典_语言角色.前状;//二宾在一宾后边，使用原始级【一宾】【二宾】
					}
				}
			}

			if (得状序号 != 无效序号)
			{
				if (得状序号 < 0)
					得状 = 字典_语言角色.前状;
				else if ((一宾序号 != 无效序号 && 一宾序号 > 得状序号) || (二宾序号 != 无效序号 && 二宾序号 > 得状序号))//得状必须在所有宾语之后，如果在一个宾语前，就变成普通后状。
					得状 = 字典_语言角色.后状;
			}

			foreach (模式 关联row in dr)
			{
				//int 根部分 = (int)row["语言角色"] - ((int)row["语言角色"] & 字典_语言角色.全部叶);
				if (主语序号 != 无效序号 && 主语序号 == (int)关联row.序号)
					关联row.语言角色 = 主语;
				if (一宾序号 != 无效序号 && 一宾序号 == (int)关联row.序号)
					关联row.语言角色 = 一宾;
				if (二宾序号 != 无效序号 && 二宾序号 == (int)关联row.序号)
					关联row.语言角色 = 二宾;
				if (得状序号 != 无效序号 && 得状序号 == (int)关联row.序号)
					关联row.语言角色 = 得状;
				if ((int)关联row.语言角色 == 字典_语言角色.定语)
					关联row.语言角色 = (int)关联row.序号 < 0 ? 字典_语言角色.前定 : 字典_语言角色.后定;
				if (关联row.语言角色 == 字典_语言角色.状语)
					关联row.语言角色 = (int)关联row.序号 < 0 ? 字典_语言角色.前状 : 字典_语言角色.后状;
			}

		end:
			foreach (模式 关联row in dr)
			{
				if (关联row.显隐 == 字典_显隐.附加)
					continue;

				//List<模式> dr参数 = 所有对象集合.Where(r => r.ParentID == 关联row.ID).ToList();
				//Assert(关联row.端索引表_Parent.Count == 1);

				foreach (模式 参数row in 关联row.端索引表_Parent)
					递归_根据位置微调语言角色(参数row, (int)关联row.语言角色);
			}

			return;
		}

		public static void 递归_传递语言角色(模式 rootrow)
		{
			if (是拥有形式(rootrow))
				return;

			Data.Assert(Data.ThisGuid.Equals(rootrow.A端));

			IList<模式> 所有对象集合 = Data.FindTableByID(rootrow.ID).对象集合;

			List<模式> dr = 所有对象集合.Where(r => r.ParentID == rootrow.ID).OrderBy(r => r.序号).ToList();

			foreach (模式 关联row in rootrow.端索引表_Parent.OrderBy(r => r.序号))
			{
				Data.Assert(Data.ThisGuid.Equals(关联row.A端) == false);

				bool 传递角色 = ((int)关联row.语言角色 & 字典_语言角色.同位) != 0 && (short)rootrow.显隐 == 字典_显隐.隐藏;

				if (传递角色)
					关联row.语言角色 = rootrow.语言角色;

				//List<模式> dr参数 = 所有对象集合.Where(r => r.ParentID == 关联row.ID).ToList();

				foreach (模式 参数row in 关联row.端索引表_Parent)
				{
					if (传递角色)
						参数row.语言角色 = rootrow.语言角色;

					递归_传递语言角色(参数row);
				}

			}

		}
		//递归第三阶段最终生成形式串(row);
		public static string 第四阶段最终生成形式串(模式 row, int 语言)
		{
			//方便计算是否在这个句子范围内，但现在暂时没有使用。
			DataIDS.Clear();

			IList<模式> 所有对象集合 = Data.FindTableByID(row.ID).对象集合;

			递归AddIds(所有对象集合, row.ID);

			递归合成语言串(row, 语言);
			return null;
		}

		public static void 递归_构造语言表现形式(模式 row, int 语言, bool 是引用记录)
		{
			//现在进行修改了，语义行本身嵌入的串不再使用！！
			//TheString strobj = new TheString((string)row["形式"]);
			//strobj.清理嵌入串();
			//if (strobj.有嵌入串)//有原始串。
			//	return;

			//开始的时候，大家都是中心语！也就是相互之间是独立的！！然后需要的合并成为谓语。
			//找出所有子记录。
			模式 依据row = 是引用记录 ? FindRowByID(row.源记录) : row;

            // 进行模板匹配，若匹配成功，从模板中取形式
            foreach (var 模板 in Data.语言模板集合.Where(r => r.语言 == 语言))
            {
                var 模板拥有形式行 = Data.模式表.对象集合.Where(r => r.A端 == 模板.ID && 是拥有形式(r));
                if (模板拥有形式行.Any())
                {
                    if (Data.进行模板匹配(模板, row))
                    {
                        var 模板源记录Guid = Data.FindRowByID(模板.源记录).源记录;
                        if (模板源记录Guid == Data.NullGuid)
                        {
                            // 开放模板时，关联一端为n.,形式行挂接到关联上
                            var 关联row = row.端索引表_Parent[0];
                            var row1 = 增加一体化形式(关联row, 模板.ID, 语言);
                            if (row1 != null)
                            {
                                row1.序号 = 关联row.序号;
                            }
                        }
                        else
                        {
                            var row2 = 增加一体化形式(row, 模板.ID, 语言);
                            if (row2 != null)
                            {
                                row2.序号 = row.序号;
                            }
                            return;
                        }
                    }
                }
            }			

			//List<模式> dr = Data.FindTableByID(row.ID).对象集合.Where(r => r.ParentID == row.ID).OrderBy(r => r.序号).ToList();
			var dr = row.端索引表_Parent.OrderBy(r => r.序号);
			//一、如果找到了一条原始形式（源记录等于原始拥有形式的），那么就等于这个对象以及下边参数的【完整串】，就不用再计算了。
			foreach (模式 childrow in dr)
			{//找到满足要求的串，则返回。
				if (是原始拥有形式(childrow))
				{
					Assert(ID相等(childrow.A端, row.ID));
					return;
				}
			}

			foreach (模式 childrow in dr)
			{
				if (是派生关联(动作拥有离合宾语动作Guid, childrow) > 0)
				{
					var 离合宾语 = FindRowByID(childrow.B端);
					离合宾语.显隐 = 字典_显隐.隐藏;
				}
			}

			//二、没有找到【完整串】现在就要进行重新生成。
			//重生成包含各参数对象和本记录本身代表的中心对象的。
			//各参数对象的各自处理。是依据parent的组织来做的。
			foreach (模式 childrow in dr)
				递归_构造语言表现形式(childrow, 语言, 是引用记录);

			//本条记录自身的处理。这就不依赖parent，而是根据各记录的源记录去找寻。

			if (单句分析Guid.Equals(row.B端))
				return;

			if ((short)row.显隐 == 字典_显隐.隐藏)
			{
				if (是二元关联(row, false))
				{
					var 关联拥有B端Row = row.端索引表_Parent.FirstOrDefault(r => r.源记录 == Data.关联拥有B端Guid);
					if (关联拥有B端Row != null)
					{
						var B端源模式行 = Data.FindRowByID(Data.FindRowByID(关联拥有B端Row.B端).源记录);
						if (能够作为名词谓语(B端源模式行))
						{
							模式 名词谓语形式Row = null;
							switch (语言)
							{
								case 字典_语言.汉语:
									//名词谓语形式Row = Data.FindRowByID(Data.名词谓语形式是Guid); 
									关联拥有B端Row.序号 = 0;
									break;
								case 字典_语言.英语:
									名词谓语形式Row = Data.FindRowByID(Data.名词谓语形式IsGuid);
									break;
							}

							if (名词谓语形式Row != null)
							{
								var row2 = Data.New派生行(名词谓语形式Row, 字典_目标限定.A端);
								row2.序号 = row.序号;
								row2.语言角色 = row.语言角色;
								挂接记录(row, row2, null);
								Data.FindTableByID(row.ID).新加对象(row2);
							}
						}
					}
				}
				return;
			}


			if (查找离合谓语(依据row) != null)//如果有离合谓语，那么原始谓语就不展现了。离合谓语替代原始谓语的展现。
				return;

			模式 的 = null;

			int that = row.That根;

			//这样的介词处理，就是把介词看着角色本身的，只是优化而依附于【关联】。
			//if (parentrow != null && ((Guid)parentrow["B端"]).Equals(row["ID"]))
			//    增加介词形式(row, parentrow, 语言);

			//if (增加一体化形式(row, (Guid)row["源记录"], 语言) != null)//如果挂接【形式】成功，就不做别的了。【形式】表示是已经处理好的东西。
			//	return;

			//用【关联】【角色】【介词】【参数】【的】几个部分来生成形式。

			if (that == 字典_目标限定.A端)//正常的语序都是这个状态。
			{
				增加参数形式(row, (Guid)row.B端, (Guid)依据row.B端, 语言, 0);
				增加参数形式(row, (Guid)row.A端, (Guid)依据row.A端, 语言, 0);
				//介词的层级和对象本身是平级的，这样，在对象和介词直接可以插入别的参数，比如【书(昨天)被借...】，【他借书给(了)她...】
				//重点注意！！！！考虑到上述的，【介词】中间可能插入别的状语，所以，介词要提出到上一级处理，也就是如下代码
				//增加介词形式(parentrow, row, 语言);//这种处理方法，介词就是【关联】自己直接处理和挂接。
				//但别处代码也要变，因为介词和直接参数不连续了，我暂时固不上，所以先保留如下代码，回头再处理。
				增加介词形式(row, 依据row, 语言);//这种处理方法，介词就是【关联】自己直接处理和挂接。
				增加的或地形式(row, 依据row, 语言, 10000);
			}
			else if (that == 字典_目标限定.B端)//这种情况一般就是从句的形式，这个时候介词是不使用的了。但【的】一般不会省略。
			{
				增加参数形式(row, (Guid)row.A端, (Guid)依据row.A端, 语言, 0);
				增加参数形式(row, (Guid)row.B端, (Guid)依据row.B端, 语言, 0);
				if (增加的或地形式(row, 依据row, 语言, 10000) == false && 字典_语言角色.从定 == (int)row.语言角色)//【的】从句的形式。
					增加从句的形式(row, 语言, 10000);
			}
			else if (that == 字典_目标限定.连接)//要表现中间的连接，这种情况后边应该去掉，都采用展开方式
			{
				if (row.显隐 != 字典_显隐.附加)
				{
					//现在，分析生成的结构里边不会出现这种情况，可能只在压缩的知识库里边的记录直接生成文字才会遇到。
					增加参数形式(row, (Guid)row.源记录, (Guid)依据row.源记录, 语言, 0);
					//增加参数形式(row, (Guid)row.A端, (Guid)依据row.A端, 语言, 0);
					//增加参数形式(row, (Guid)row.B端, (Guid)依据row.B端, 语言, 0);
					//增加的或地形式(row, 依据row, 语言, 10000);
				}
			}
		}

		/// <summary>
		/// 模板匹配
		/// </summary>
		/// <param name="模板"></param>
		/// <param name="待匹配模式"></param>
		/// <returns></returns>
		private static bool 进行模板匹配(模式 模板模式, 模式 待匹配模式)
		{
			var result = false;

			var 待匹配模式树结构 = 模式树结构.从一个根模式生成模式树结构(待匹配模式);
			var 模板源模式 = Data.FindRowByID(模板模式.源记录);
			var 模板模式树结构 = 模式树结构.从一个根模式生成模式树结构(模板源模式);

			result = 递归匹配两个模式树结构(待匹配模式树结构, 模板模式树结构);

			return result;
		}

		private static bool 递归匹配两个模式树结构(模式树结构 待匹配模式树结构, 模式树结构 模板模式树结构)
		{
			bool matched = false;
			var 待匹配源模式行 = 待匹配模式树结构.目标;
			var 匹配模板模式行 = 模板模式树结构.目标;

			if (匹配两个模式行(待匹配源模式行, 匹配模板模式行))
			{
				if (模板模式树结构.语法树子节点 == null)
				{
					matched = true;
				}
				else
				{
                    if (待匹配模式树结构.语法树子节点 != null && 待匹配模式树结构.语法树子节点.Count == 模板模式树结构.语法树子节点.Count)
					{
                        for (int i = 0; i < 模板模式树结构.语法树子节点.Count; i++)
                        {
                            matched = 递归匹配两个模式树结构(待匹配模式树结构.语法树子节点[i], 模板模式树结构.语法树子节点[i]);
                            if (matched == false)
                            {
                                break;
                            }
                        }
					}
				}
			}

			return matched;
		}

		private static bool 匹配两个模式行(模式 待匹配源模式行, 模式 匹配模板模式行)
		{
			var result = false;

			var 待匹配源模式行源记录Guid = 待匹配源模式行.源记录;
			var 匹配模板模式行源记录Guid = 匹配模板模式行.源记录;

			while (true)
			{
				if (NullGuid == 待匹配源模式行源记录Guid || NullParentGuid == 待匹配源模式行源记录Guid || ThisGuid == 待匹配源模式行源记录Guid)
				{
					break;
				}

                // 开放模板匹配，这种情况下，模板的A端或者B端源记录行为n.
                if (匹配模板模式行源记录Guid == Data.NullGuid)
                {
                    result = true;
                    break;
                }

				if (待匹配源模式行源记录Guid == 匹配模板模式行源记录Guid)
				{
					result = true;
					break;
				}
				else
				{
					var new源记录Guid = Data.FindRowByID(待匹配源模式行源记录Guid).源记录;
					if (new源记录Guid == 待匹配源模式行源记录Guid)
					{
						break;
					}
					else
					{
						待匹配源模式行源记录Guid = new源记录Guid;
					}
				}
			}

			return result;
		}

		public static 模式 增加参数形式(模式 parentrow, Guid id, Guid 依据id, int 语言, int 序号)
		{
			if (DataIDS.Contains(id))//如果该参数在本句子内部，这里不处理，等待该对象自己处理。
				return null;
			if (ThisGuid.Equals(id))//对于this，也不处理。
				return null;
			模式 row = 增加一体化形式(parentrow, 依据id, 语言);
			if (row != null)
				row.序号 = 序号;
			return row;
		}

		public static bool 增加从句的形式(模式 挂接位置row, int 语言, int 序号)
		{
			模式 row = FindRowByID(关联拥有的Guid);
			return 执行加入的或地(挂接位置row, row, (int)挂接位置row.语言角色, 序号);
		}

		//这个是由关联决定的。
		public static bool 增加的或地形式(模式 挂接位置row, 模式 关联行, int 语言, int 序号)
		{
			int v = (int)挂接位置row.语言角色;

			//            if ((v & 字典_语言角色.谓宾) != 0)//对于谓宾，不增加【的和地】。
			//                return null;

			模式 row = null;

			//if ((v & 字典_语言角色.状语) != 0)//是状语，先按更专用的状语尝试，定语更通用，所以放后边。
			//	row = 递归查找成分((Guid)parentrow["源记录"], 关联拥有地Guid, 语言, v, "");
			//if (row == null && (v & 字典_语言角色.定语) != 0)//按定语尝试下。
			//	row = 递归查找成分((Guid)parentrow["源记录"], 关联拥有的Guid, 语言, v, "");
			row = 递归查找成分((Guid)关联行.源记录, 关联拥有地Guid, 语言, v, "");
			if (row == null)//按定语尝试下。
				row = 递归查找成分((Guid)关联行.源记录, 关联拥有的Guid, 语言, v, "");

			if (row == null)
				return false;

			return 执行加入的或地(挂接位置row, row, v, 序号);
		}

		public static void 增加介词形式(模式 挂接位置row, 模式 关联行, int 语言)
		{
			int v = (int)挂接位置row.语言角色;

			//前置介词的默认位置是【句首】，但是如果放在中间，比如【他昨天如果...】里边的如果，那么可能不是这么简单的计算。
			//位置应该根据当前实际的主语【他】来进行计算，而不是默认的【句首】
			//【拥有介词】本身的【序号】信息是否大于0比较重要，如果大于0，就表示可以放在主语的后边。
			//也许这个位置要要记录下来，【拥有介词】的记录不会删除，而不是总是删除后自动生成，那样就丢失了信息了。

			模式 row = 递归查找成分((Guid)关联行.源记录, 关联拥有前置介词Guid, 语言, v, "");
            if (row != null)
            {
                var 序号 = row.序号 - Math.Abs(挂接位置row.序号);
                执行加入介词(挂接位置row, row, v, /*(int)关联行["序号"] +如果改成挂接到上一级，就要这样序号叠加*/ 序号);
            }
				

			row = 递归查找成分((Guid)关联行.源记录, 关联拥有介动词后生存Guid, 语言, v, "");
			//暂时不生成介动后生存，英语没有这个，把这个转换成普通的生成阶段。
			//在解析时则可以。
			//if (row != null)
			//	执行加入介词(挂接位置row, row, v, /*(int)关联行["序号"] +如果改成挂接到上一级，就要这样序号叠加*/ (int)row.序号);

			row = 递归查找成分((Guid)关联行.源记录, 关联拥有后置介词Guid, 语言, v, "");
            if (row != null)
            {
                var 序号 = row.序号 + Math.Abs(挂接位置row.序号);
                执行加入介词(挂接位置row, row, v, /*(int)关联行["序号"] +如果改成挂接到上一级，就要这样序号叠加*/ 序号);
            }				
		}

		public static 模式 执行加入介词(模式 parentrow, 模式 BaseRow, int 语言角色, int 序号 = 无效序号)
		{
			模式 row = Data.New派生行(BaseRow, 字典_目标限定.A端);

			row.语言角色 = 语言角色;
			if (序号 != 无效序号)
				row.序号 = 序号;
			挂接记录(parentrow, row, null);
			Data.FindTableByID(parentrow.ID).新加对象(row);

			return row;
		}

		public static bool 已经有了相同形式(模式 parentrow, 模式 BaseRow)
		{
			foreach (模式 o in parentrow.端索引表_A端)
			{
				if (o == BaseRow)
					return true;
			}
			return false;
		}


		public static bool 执行加入的或地(模式 parentrow, 模式 BaseRow, int 语言角色, int 序号 = 无效序号)
		{
			if (已经有了相同形式(parentrow, BaseRow))
				return true;
			模式 row = Data.New派生行(BaseRow, 字典_目标限定.A端);

			row.语言角色 = 语言角色;
			if (序号 != 无效序号)
				row.序号 = 序号;

			挂接记录(parentrow, row, null);
			Data.FindTableByID(parentrow.ID).新加对象(row);

			return true;
		}
		public static 模式 加入参数(模式 parentrow, 模式 BaseRow, int 序号 = 无效序号)
		{
			模式 row = Data.New派生行(BaseRow, 字典_目标限定.A端);

			if (序号 != 无效序号)
				row.序号 = 序号;

			挂接记录(parentrow, row, null);
			Data.FindTableByID(parentrow.ID).新加对象(row);
			return row;
		}

		public static 模式 递归查找成分(Guid id, Guid 目标形式分类, int 语言, int 语言角色值, string 明确的串)
		{
			while (true)
			{
				//var dr = 模式表.对象集合.Where(r => r.A端 == id).ToList();

				foreach (模式 row1 in Data.FindRowByID(id).端索引表_A端)
				{
					if (语言 != (int)row1.语言)
						continue;

					if ((语言角色值 & (int)row1.语言角色) == 0)//语言角色不满足
						continue;

					if (是派生关联(目标形式分类, row1) == 0)
						continue;

					if (明确的串 == "" || 明确的串 == Data.取得嵌入串(row1))
						return row1;
				}

				模式 row = Data.FindRowByID(id);
				id = (Guid)row.源记录;
				if (id.Equals(Data.NullGuid))
					return null;
				if (基本关联Guid.Equals(id) || 替代.可正向替代(id))//找不到了。
					return null;
			}
		}

		//已有一个语义对象id，给它添加形式化记录并挂接到下边。
		public static 模式 增加一体化形式(模式 parentrow, Guid 语义对象id, int 语言)
		{
			//首先增加语言部件。
			int 语言角色 = (int)parentrow.语言角色;

			//首先处理语言部件，语言部件是不互斥的，有多少个就要使用多少个。
			模式 parentparentrow = FindRowByID((Guid)parentrow.ParentID);

			//List<模式> dr = Data.FindTableByID(语义对象id).对象集合.Where(r => r.A端 == 语义对象id).OrderBy(r => r.序号).ToList();
			var dr = Data.FindRowByID(语义对象id).端索引表_A端.OrderBy(r => r.序号);
			if (parentparentrow != null)
			{
				foreach (模式 row in dr)
				{
					if (字典_语言.满足指定语言(语言, (int)row.语言) == false)
						continue;
					if (是派生关联(拥有语言部件Guid, row) == 0)
						continue;
                    if ((row.语言角色 & 语言角色) == 0)
                        continue;
					模式 row2 = Data.New派生行(row, 字典_目标限定.A端);


					//row2["语言角色"] = 计算一个明确的默认的语言角色(row2, 语言);
					row2.语言角色 = parentrow.语言角色;
					row2.序号 = 字典_语言角色.计算一级序号((int)row2.语言角色) + (int)row2.序号;

					// 语言算子往其语义中心上挂
					模式 挂接Row = null;
					var 一级关联类型 = Data.一级关联类型(row2);
					var 二级关联类型 = Data.二级关联类型(row2);
					if (一级关联类型 == Data.拥有形式Guid && 二级关联类型 == Data.拥有语言算子Guid)
					{
						if (parentparentrow.That根 == 字典_目标限定.A端 && parentparentrow.A端 != Data.ThisGuid)
						{
							挂接Row = FindRowByID(parentparentrow.A端);
						}

						if (parentparentrow.That根 == 字典_目标限定.B端 && parentparentrow.B端 != Data.ThisGuid)
						{
							挂接Row = FindRowByID(parentparentrow.B端);
						}
					}

					if (挂接Row != null)
					{
						挂接记录(挂接Row, row2, null);
						Data.FindTableByID(parentrow.ID).新加对象(row2);
					}
				}
			}

			//对于语言形式只使用概率最高的一个。选择一个。
			//按照序号排序，如果有不同角色对应的形式，那么把更具体的形式放到前边，这样保证更优先获取到。

			if (!dr.Any())
				return null;

			bool 使用公共语言 = false;

			//对【数字】暂时只考虑使用公共语言。因为生成的汉字分析时为数字的问题暂时没有解决。
            //if (Data.是派生类(Data.数Guid, parentrow, 替代.正向替代))
            //    使用公共语言 = true;


		做两次://公共语言也是可以的，但优先级低
			foreach (模式 row in dr)
			{
				if (拥有形式Guid.Equals(row.连接) == false)
					continue;

				if (字典_语言.满足指定语言(语言, (int)row.语言) == false)
					continue;


				if ((int)row.语言 == 字典_语言.公语 && 使用公共语言 == false)//第一遍不考虑公共语言。
					continue;

				if ((语言角色 & (int)row.语言角色) == 0)//语言角色不满足
					continue;

				if (是实体参数的形式(二级关联类型(row)) == false)//只对概念具有的形式处理，关联具有的【介词】等不在这里处理
					continue;

				模式 row2 = Data.New派生行(row, 字典_目标限定.A端);

				row2.语言角色 = parentrow.语言角色;
				//row2["序号"] = 0;//形式的序号列为0。介词和的等的就在前后。

				挂接记录(parentrow, row2, null);
				Data.FindTableByID(parentrow.ID).新加对象(row2);

				//加上附加记录，附加记录完全是平等的并列。如果是角色，那么一般是查到介词形式，一般情况下形式和介词形式只会有一个。
				//var dr3 = Data.FindTableByID(语义对象id).对象集合.Where(r => r.A端 == row.ID).ToList();
				foreach (模式 row3 in row.端索引表_A端)
				{
					if (拥有形式Guid.Equals(row3.连接) == false)
						continue;

					if (字典_语言.满足指定语言(语言, (int)row3.语言) == false)
						continue;

					if (是实体参数的形式(二级关联类型(row3)) == false)
						continue;

					模式 row4 = Data.New派生行(row3, 字典_目标限定.A端);

					row4.语言角色 = parentrow.语言角色;
					//row4["序号"] = 0;//形式的序号列为0。介词和的等的就在前后。

					挂接记录(parentrow, row4, null);
					Data.FindTableByID(parentrow.ID).新加对象(row4);
				}
				return row2;
			}

			if (使用公共语言 == false)
			{
				使用公共语言 = true;
				goto 做两次;
			}

			//注意：这里加【数字】等不能穷举的对象的形式生成。

			return null;
		}

		public static int 返回对应角色的概率打分(int 语言角色, 模式 关联行, int 语言)
		{
			//如果关联是属于和聚合，那么都是同位语？
			//if (替代.是分类或聚合或属拥(Data.一级关联类型(关联行)))
			//	return 字典_语言角色.同位;

			//看是否可以继承语言角色，现在考虑不继承了。
			//for (int 层级 = 0; 层级 < 1; 层级++)
			//{
			//List<模式> dr =模式表.对象集合.Where(r => r.源记录 == 关联行.ID && r.连接 == 拥有语言角色Guid && r.That根 == that).ToList();

			//序号越高的越优先，也就是派生的设置放在下边。
			//List<模式> dr = 模式表.对象集合.Where(r => r.源记录 == 关联行.ID && r.连接 == 拥有语言角色Guid).OrderBy(r => -r.序号).ToList();
			//List<端索引> dr=Data.FindRowByID(关联行.ID).端索引表.Where(r => r.that端 == 字典_目标限定.连接 && r.模式Row.连接 == 拥有语言角色Guid).OrderBy(r => -r.模式Row.序号).ToList();
			bool 使用公共语言 = false;
			int 概率分 = 9;
			int 序号 = -1;
		做两次://公共语言也是可以的，但优先级低
			foreach (模式 row in Data.FindRowByID(关联行.ID).端索引表_源记录)
			{
				if (row.连接 == 拥有语言角色Guid)
				{
					if (字典_语言.满足指定语言(语言, (int)row.语言) == false)
						continue;
					if ((int)row.语言 == 字典_语言.公语 && 使用公共语言 == false)//第一遍不考虑公共语言。
						continue;

					if ((语言角色 & (int)row.语言角色) > 0)
					{
						if (row.序号 >= 序号) //相当于按序号排序，只取序号最大的那条记录
						{
							序号 = row.序号;
							//参数字段 参数 = new 参数字段((string)row.参数集合);
							//if (参数.概率分 > 0)
							//{
							概率分 = row.参数.概率分;
							//return 索引.模式Row.参数.概率分;
							//}
						}
					}

				}
			}
			if (使用公共语言 == false && 序号 == -1) //第一次没有获取到概率分，需要再用“公共语言”获取一次
			{
				使用公共语言 = true;
				goto 做两次;
			}
			return 概率分;
		}


		//根据关联行计算语言角色，后边要调整，把A端和B端都考虑上。
		public static int 查找关联行的语言角色(模式 关联行, 模式 A端, 模式 B端, int that, int 语言, bool 中心对象是介词形式 = false)
		{
			Data.Assert(that == 字典_目标限定.A端 || that == 字典_目标限定.B端);

			int 语言角色 = that == 字典_目标限定.B端 ? 字典_语言角色.从定 : 字典_语言角色.无;//从定先默认允许，除非后边被屏蔽掉
			bool b = false;
			//如果关联是属于和聚合，那么都是同位语？
			//if (替代.是分类或聚合或属拥(Data.一级关联类型(关联行)))
			//	return 字典_语言角色.同位;

			//看是否可以继承语言角色，现在考虑不继承了。
			//for (int 层级 = 0; 层级 < 1; 层级++)
			//{
			//List<模式> dr =模式表.对象集合.Where(r => r.源记录 == 关联行.ID && r.连接 == 拥有语言角色Guid && r.That根 == that).ToList();

			//List<端索引> dr = Data.FindRowByID(关联行.ID).端索引表.Where(r => r.that端 == 字典_目标限定.连接 && r.模式Row.连接 == 拥有语言角色Guid).ToList();
			//List<模式> dr = 模式表.对象集合.Where(r => r.源记录 == 关联行.ID && r.连接 == 拥有语言角色Guid).ToList();
			bool 使用公共语言 = false;

		做两次://公共语言也是可以的，但优先级低
			foreach (模式 row in Data.FindRowByID(关联行.ID).端索引表_源记录)
			{
				if (row.连接 == 拥有语言角色Guid)
				{
					if (字典_语言.满足指定语言(语言, (int)row.语言) == false)
						continue;
					if ((int)row.语言 == 字典_语言.公语 && 使用公共语言 == false)//第一遍不考虑公共语言。
						continue;
					b = true;

					//参数字段 参数 = new 参数字段((string)row.参数集合);
					if (row.参数.概率分 > 0)
						语言角色 |= (int)row.语言角色;
					else
						语言角色 &= ~((int)row.语言角色);
				}

			}

			// 如果当前语言未找到，则使用公共语言
			if (使用公共语言 == false && 语言角色 == 字典_语言角色.无)
			{
				使用公共语言 = true;
				goto 做两次;
			}
			//关联行 = Data.FindRowByID((Guid)关联行.源记录);
			//}
			Guid 基本类型 = 一级关联类型(关联行);
			if (that == 字典_目标限定.B端)
			{
				//1.B端为中心时，必须是从定,但如果是显示的推理角色则例外
				if (替代.是属于等价或聚合(基本类型) && Data.是派生类(Data.推理角色Guid, B端, 替代.正向替代) && 中心对象是介词形式 == false)
					return 字典_语言角色.同位;
				return (语言角色 & 字典_语言角色.从定) > 0 ? 字典_语言角色.从定 : 字典_语言角色.无;
			}

			if (b)
				return 语言角色;

			//没有找到，设置一些默认的角色。
			//聚合等都是同位语。
			//Guid 基本类型 = 一级关联类型(关联行);

			if (替代.是属于等价或聚合(基本类型))
				return 字典_语言角色.同位;

			if (替代.是属拥(基本类型))
				return 字典_语言角色.状语;



			//if (拥有Guid.Equals(基本类型))
			//{
			//    if(that == 字典_目标限定.A端)
			//        return 能够序列化(A端) ? 字典_语言角色.状语 : 字典_语言角色.定语;
			//    return 字典_语言角色.从定;
			//}

			return 字典_语言角色.无;
		}


		//从父来调用各个子的。
		//以该行为根，遍历到叶子后重新计算整棵树的打分。



		public static void 递归设置语言(模式 rootrow, int 语言)
		{
			rootrow.语言 = 语言;

			//var dr = Data.FindTableByID(rootrow.ID).对象集合.Where(r => r.ParentID == rootrow.ID).ToList();

			foreach (模式 row in rootrow.端索引表_Parent)
				递归设置语言(row, 语言);

		}

		public static 模式 加入到素材(模式 row)
		{
			row.ParentID = Data.当前素材Row.ID;
			get模式编辑表().新加对象(row);
			return row;
		}

		public static 模式 增加字符串生长素材(SubString obj)
		{
			Guid 基类串 = 字符串Guid;

			//下边可以让原始的【属于】记录就有区分，但也可以不做，到分析的时候再处理。
			string 串 = obj.ToString();

			if (前置介词集合.Contains(串))
				基类串 = 介词Guid;
			else if (后置介词集合.Contains(串))
				基类串 = 介词Guid;

			模式 row = 加入到素材(New派生行(基类串, 字典_目标限定.A端));

			row.形式 = obj.ToString();
			row.序号 = obj.begindex;
			row.语言 = Data.当前解析语言;
			//参数字段 打分 = new 参数字段();//这里应该是要找到目标串来得到概率分，现在暂时先这样。
			row.打分 = row.参数.概率分.ToString();


			return row;
		}
		public static 模式基表 get模式编辑表()
		{
			if (Data.动态绑定至Form)
				return 模式编辑表;
			else
				return 模式会话表;
		}
		public static void 分解串并生成串对象(Guid id)
		{
			模式 row = FindRowByID(id);
			模式 r = New派生行(生长素材Guid, 字典_目标限定.A端);
			r.序号 = 10000;
			r.ParentID = row.ID;

			get模式编辑表().新加对象(r);
			当前素材Row = r;

			模式 新对象Row = New派生行(新对象Guid, 字典_目标限定.A端);
			新对象Row.序号 = 20000;
			新对象Row.ParentID = row.ID;
			get模式编辑表().新加对象(新对象Row);
			当前新对象Row = 新对象Row;


			TheString thestr = new TheString((string)row.形式);
			string str = thestr.嵌入串;
			if (str == "")
				return;

			Processor.当前处理器.执行分解串并生成串对象(str);
		}
		public static void 选择最优结果生成(bool 删除空行)
		{
			Processor.当前处理器.选择最优结果生成(删除空行);
		}

		static public void 递归设置语境树知识有效性(模式 素材, int 取值)
		{
			if (素材 == null)
				return;
			素材.语境树 = 取值;

			//List<模式> dr = Data.get模式编辑表().对象集合.Where(r => r.ParentID == 素材.ID).ToList();
			foreach (模式 r in 素材.端索引表_Parent)
				递归设置语境树知识有效性(r, 取值);
		}


		public static void 进行一轮关联生长()
		{
			Processor.当前处理器.进行一轮生长();
		}

		public static void 进行生长并得到所有中间结果()
		{
			Processor.当前处理器.第一阶段生长();
			Processor.当前处理器.第四阶段尝试构建属于模式的新知识();
			//Processor.当前处理器.填补未匹配字符串();
		}

		//对于形式，只取本级的，也就是不考虑继承基类的！这样更简单。
		static public 参数树结构 利用缓存得到基类和关联记录树(模式 row, bool 查询连接)
		{
			if (是介词或者串(row, true, true, true))
				return new 参数树结构(row, 字典_目标限定.A端, false/*, 0*/);
			//进行一个转换处理。
			if (替代.可正向替代(一级关联类型(row)) && 是二元关联((Guid)row.B端, true))
				查询连接 = true;

			Dictionary<模式, 参数树结构> 对应缓存 = null;
			//if (包含拥有)
			//{
			if (查询连接)
				对应缓存 = 属于和拥有树关联缓存;
			else
				对应缓存 = 属于和拥有树缓存;
			//}
			//else
			//{
			//    if (查询连接)
			//        对应缓存 = 属于基类树关联缓存;
			//    else
			//        对应缓存 = 属于基类树缓存; ;
			//}

			//参数树结构 tree = Data.得到基类和拥有树(true, 模式行, !包含拥有和基概念, 包含拥有和基概念, false, true, 0, 查询连接, 0, Data.当前语言, 对应缓存);//属于，拥有和形式。【被拥有】暂不体现。
			//注意，【形式】层级定为1，只列出第一级的形式，基类继承的形式都不列出来！！因为很基类的形式不能使用。

			参数树结构 tree = 得到基类和关联树(true, row, true, 0, /*查询连接, 1, */Data.当前解析语言, 对应缓存);//属于，拥有和形式。【被拥有】暂不体现。
			return tree;
		}

		//在进行疑问匹配的时候用这个，比如【谁】【属于】【人】，我们要去除疑问部分，也就是转换成为【人】来进行。
		//这样，就能查询出【毛泽东、王菲】等都可以【属于】【谁】。
		static public 参数树结构 去除疑问部分利用缓存得到派生树(模式 模式行, bool 包括扩展派生)
		{
			foreach (疑问概念结构 o in 疑问变量结构)
				if (o.疑问概念.Equals(模式行.ID))
				{
					模式行 = FindRowByID(o.类型概念);
					break;
				}
			return 利用缓存得到派生树(模式行, false, 包括扩展派生);
		}

		public static bool 是疑问数量(模式 模板模式)
		{
			if (是派生类(度量Guid, 模板模式, 替代.正向替代))
				foreach (模式 row in 模板模式.端索引表_Parent)
					foreach (模式 row1 in row.端索引表_Parent)
					{
						if (Data.是派生类(Data.疑问算子Guid, row1, 替代.正向替代))
							return true;
					}
			return false;
		}


		public static int 是疑问对象(模式 模板模式)
		{
			if (Data.是派生类(Data.疑问算子Guid, 模板模式, 替代.正向替代))
				return 12;
			if (Data.是疑问替代对象(模板模式.源记录))
				return 12;
			if (是疑问数量(模板模式))
				return 15;
			if (获取子节点中指定类型的节点(模板模式, Data.事物拥有什么疑问Guid, false) != null)
				return 11;

			return 0;
		}

		static public 模式 获取子节点中指定类型的节点(模式 模式行, Guid 类型Guid, bool 是否递归 = false)
		{
			if (模式行 != null && 模式行.端索引表_Parent != null)
			{
				bool 是二元关联 = false;
				if (Data.是二元关联(类型Guid, false))
					是二元关联 = true;
				foreach (模式 row in 模式行.端索引表_Parent)
				{
					if (是二元关联)
					{
						if (Data.是派生关联(类型Guid, row) > 0)
							return row;
					}
					else
					{
						if (Data.是派生类(类型Guid, row, 替代.正向替代))
							return row;
					}
					if (是否递归)
					{
						模式 r = 获取子节点中指定类型的节点(row, 类型Guid, 是否递归);
						if (r != null)
							return r;
					}
				}
			}
			return null;
		}

		static public 参数树结构 利用缓存得到派生树(模式 模式行, bool 仅知识, bool 包括扩展派生)
		{
			if (Data.是介词或者串(模式行, true, true, true) == true)
				return null;

			//Dictionary<DataRow, 参数树结构> 对应缓存 = 属于基类树缓存;
			//if (包含拥有和基概念 == true)
			//{
			//    if (查询连接 == true)
			//        对应缓存 = 属于和拥有被拥有树关联缓存;
			//    else
			//        对应缓存 = 属于和拥有被拥有树缓存;
			//}
			//else if (查询连接 == true)
			//    对应缓存 = 属于基类树关联缓存;

			//if(对应缓存.ContainsKey(模式行))
			//    return 对应缓存[模式行];

			参数树结构 tree = 得到派生树(true, 模式行, 仅知识, 0, 包括扩展派生);//属于，拥有和形式。【被拥有】暂不体现。

			//对应缓存.Add(模式行, tree参数);

			return tree;
		}

		static public 参数树结构 调整行得到基类和关联树(bool 是否清除唯一性缓存, 模式 row, bool 仅知识, float 阀值, /*int 形式层级,*/ int 语言, Dictionary<模式, 参数树结构> 缓存 = null)
		{
			return 得到基类和关联树(是否清除唯一性缓存, row, 仅知识, 阀值, /*形式层级,*/ 语言, 缓存);
		}

		static public 参数树结构 得到基类和关联树(bool 是否清除唯一性缓存, 模式 row, bool 仅知识, float 阀值, /*int 形式层级,*/ int 语言, Dictionary<模式, 参数树结构> 缓存 = null)
		{
			if (是否清除唯一性缓存)
				Data.DataIDS.Clear();

			参数树结构 objset = null;

			bool 查询连接 = 是二元关联但排除掉等价型(row);

			if (查询连接)
				objset = new 参数树结构(row, 字典_目标限定.连接, true/*, 0*/);
			else
				objset = new 参数树结构(row, 字典_目标限定.A端, false/*, 0*/);
			//long t = System.DateTime.Now.Ticks;
			参数树结构 returnset = objset.递归得到所有基类和关联树(true, /*形式层级,*/ 仅知识, 阀值, 语言, 缓存);
			//timeCount += System.DateTime.Now.Ticks - t;
			if (returnset != null)
				return returnset;

			return objset;
		}

		//在查找二元关联的时候，A端和B端如果有值，才会查询，否则，派生的对象太多了。
		static public 参数树结构 得到派生树(bool 是否清除唯一性缓存, 模式 row, bool 仅知识, float 阀值, bool 包括扩展派生)
		{
			if (是否清除唯一性缓存)
				Data.DataIDS.Clear();

			参数树结构 objset = null;

			bool 查询连接 = 是二元关联但排除掉等价型(row);

			if (查询连接)
				objset = new 参数树结构(row, 字典_目标限定.连接, true/*, 层级*/);
			else
				objset = new 参数树结构(row, 字典_目标限定.A端, false/*, 层级*/);

			Data.DataIDS.Add(row.ID);
			参数树结构 returnset = objset.递归得到所有派生类(仅知识, 阀值, 包括扩展派生);

			if (returnset != null)
				return returnset;

			return objset;
		}
		public static PatternDataSet.模式查找Row 加入一条参数记录(PatternDataSet.模式查找Row root, 模式 objectrow, string 列名)
		{
			PatternDataSet.模式查找Row newRow = patternDataSet.模式查找.New模式查找Row();
			newRow.标记 = "";
			newRow.ID = Guid.NewGuid();
			newRow.ObjectID = objectrow.ID;
			newRow.参数 = objectrow.参数集合;
			if (root != null)
				newRow.ParentID = root.ID;
			else
				newRow.ParentID = Data.NullParentGuid;

			newRow.根 = 列名;
			newRow.序号 = 全局序号;
			全局序号 += 1;
			//暂时还没有考虑好序号如何排列，应该体现一种优先级别。
			patternDataSet.模式查找.Add模式查找Row(newRow);

			return newRow;
		}

		public static void 加入基对象和关联对象(PatternDataSet.模式查找Row ObjectRow, Guid objectid, bool 去除概念, bool 包含编辑,/* int 形式层级, */bool 仅知识, float 阀值, /*bool 查询连接,*/ int 语言)
		{
			//基类的基本节点

			模式 therow = Data.FindRowByID(objectid);

			参数树结构 obj = 调整行得到基类和关联树(false, therow, 仅知识, 阀值, /*形式层级,*/ 语言);//这里不清除缓存。

			if (obj != null)
				obj.递归加入参数树到界面(ObjectRow);
		}

		public static void 加入派生对象(PatternDataSet.模式查找Row ObjectRow, Guid objectid, bool 仅知识, float 阀值)
		{
			模式 therow = Data.FindRowByID(objectid);

			//参数树结构 obj = 调整行得到派生树(false, therow,仅知识, 阀值, 是查找连接);//这里不清除缓存。
			参数树结构 obj = 得到派生树(false, therow, 仅知识, 阀值, true);//这里不清除缓存。

			if (obj != null)
				obj.递归加入参数树到界面(ObjectRow);
		}

		public static void 加入基类树到参数(PatternDataSet.模式查找Row root, 模式 therow, string 列名, bool 包含编辑, bool 仅知识, float 阀值, int 语言)
		{
			全局序号 = 0;
			DataIDS.Clear();

			Guid objectid = therow.ID;
			Guid k = (Guid)(typeof(模式).GetProperty(列名).GetValue(therow));


			PatternDataSet.模式查找Row newRow = null;
			if (objectid.Equals(k) == false && ThisGuid.Equals(k) == false)
				newRow = 加入一条参数记录(root, therow, 列名);

			if (Data.ThisGuid.Equals(k) == false)
			{
				objectid = k;
				模式 r1 = FindRowByID(k);
			}

			加入基对象和关联对象(newRow, objectid, false, 包含编辑, 仅知识, 阀值, 语言);

		}

		public static void 加入派生树到参数(PatternDataSet.模式查找Row root, 模式 therow, string 列名, bool 仅知识, float 阀值)
		{
			全局序号 = 0;
			DataIDS.Clear();

			Guid objectid = (Guid)(typeof(模式).GetProperty(列名).GetValue(therow));

			//PatternDataSet.模式查找Row newRow = null;
			//if (objectid.Equals(k) == false && Data.ThisGuid.Equals(k) == false)
			//    newRow = Data.加入一条参数记录(root, therow, 列名);

			加入派生对象(root, objectid, 仅知识, 阀值);


			//if (列名 != 连接改为源记录 && Data.ThisGuid.Equals(k) == false)
			//{
			//    objectid = k;
			//    DataRow r1 = Data.FindRowByID(k);
			//    if (r1 != null && Data.属于Guid.Equals(Data.返回二元关联(r1)) == false && Data.ThisGuid.Equals(r1["A端"]) == false)
			//        列名 = 连接改为源记录;//点击的不是【连接】，但指向的目标不是【this】【属于】的模式，就假定是【关联】模式，可能有错误。
			//}

			//if (列名 == 连接改为源记录)
			//    Data.加入对象的派生对象(newRow, objectid, 仅知识, 阀值, true);
			//else
			//    Data.加入对象的派生对象(newRow, objectid, 仅知识, 阀值, false);
		}

		public static void 加入模式到链表去除重复(List<模式> objlist, 模式 obj)
		{
			if (objlist.Contains(obj))
				return;
			objlist.Add(obj);
		}

		public static void 递归加入关联模式行(聚合体对象及关联模式 聚合体对象及关联模式, 模式 therow, bool 加入形式)
		{

			if (聚合体对象及关联模式.聚合体.Contains(therow))
				return;

			聚合体对象及关联模式.聚合体.Add(therow);

			if (Data.ThisGuid.Equals(therow.A端) == false)
			{
				加入模式到链表去除重复(聚合体对象及关联模式.所有关联, FindRowByID(therow.A端));
				加入模式到链表去除重复(聚合体对象及关联模式.所有关联, FindRowByID(therow.B端));
			}

			int flag = 0;
			do
			{
				List<模式> 端索引表;
				if (flag == 0)
					端索引表 = therow.端索引表_A端;
				else if (flag == 1)
					端索引表 = therow.端索引表_B端;
				else
					break;
				flag++;
				foreach (模式 row in 端索引表)
				{
					if (row.语境树 < 0)
						continue;

					Guid 一级分类 = 一级关联类型(row);

					//推导不列到这里了。
					if (一级分类.Equals(推导即命题间关系Guid))
						continue;

					//一、拥有，拥有形式，基本关联，推导等的处理
					if (拥有形式Guid.Equals(一级分类))
					{
						if (加入形式)
							加入模式到链表去除重复(聚合体对象及关联模式.所有关联, row);
					}
					else
					{
						//派生类不算是关联。
						if (替代.是本质正向分类(一级分类) && row.源记录.Equals(therow.ID))
							continue;
						加入模式到链表去除重复(聚合体对象及关联模式.所有关联, row);
						if (替代.是等价或聚合(一级分类))
						{
							模式 另一个对象 = FindRowByID(flag == 1 ? row.B端 : row.A端);
							递归加入关联模式行(聚合体对象及关联模式, 另一个对象, 加入形式);
						}
					}
				}
			}
			while (true);
		}



		//查询出一个对象及其聚合对象，以及整个具体体内部和对外的关联模式行，排除掉派生的对象
		public static 聚合体对象及关联模式 知识完整聚合体(模式 发起对象, bool 加入形式, List<模式> 去除对象 = null)
		{
			聚合体对象及关联模式 聚合体对象及关联模式 = new 聚合体对象及关联模式();
			Data.递归加入关联模式行(聚合体对象及关联模式, 发起对象, 加入形式);
			if (去除对象 != null)
			{
				for (int i = 0; i < 聚合体对象及关联模式.所有关联.Count(); i++)
				{
					foreach (模式 模式 in 去除对象)
					{
						if (聚合体对象及关联模式.所有关联[i] == 模式)
						{
							聚合体对象及关联模式.所有关联.RemoveAt(i--);
							break;
						}
					}
				}
			}
			return 聚合体对象及关联模式;
		}

		public static bool 满足聚合派生(Guid 基类ID, 模式 派生对象)
		{
			if (Data.是派生类(Data.疑问算子Guid, Data.FindRowByID(基类ID), 替代.正向替代) || Data.是疑问替代对象(基类ID))
				return true;

			foreach (模式 row in Data.知识完整聚合体(派生对象, false, null).聚合体)
			{
				if (Data.是派生类(基类ID, row, 替代.正向替代))
					return true;
			}
			return false;
		}

		public static List<参数树结构> 得到一批同一层级的基类和关联(模式 therow, bool 包含拥有, bool 加入形式, bool 仅知识, float 阀值, /*int 层级,*/ int 语言)
		{
			Guid objectid = therow.ID;
			List<参数树结构> objlist = new List<参数树结构>();
			List<参数树结构> 属于list = new List<参数树结构>();

			//this自身记录。应该是属于或者等价。
			if (Data.ThisGuid.Equals(therow.A端))
			{
				Guid 一级分类 = 一级关联类型(therow);
				Data.Assert(替代.是本质正向分类(一级分类));
				if (包含拥有 ? 替代.是分类或聚合或属拥(一级分类) : 替代.是本质分类(一级分类))
				{
					模式 row = Data.FindRowByID((Guid)therow.B端);
					属于list.Add(new 参数树结构(row, 字典_目标限定.A端, Data.是二元关联但排除掉等价型(row)/*, 层级*/));
				}
				//这里根据【this】【属于】【基类】来加入。
				//如果是【this 属于 基类】，把这个记录单独增加，好单独列出其参数，增加的时候则要合并。
			}
			int flag = 0;
			do
			{
				List<模式> 端索引表;
				if (flag == 0)
					端索引表 = therow.端索引表_A端;
				else if (flag == 1)
					端索引表 = therow.端索引表_B端;
				//else if (flag == 2)
				//    端索引表 = therow.端索引表_源记录;
				else
					break;
				flag++;
				foreach (模式 row in 端索引表)
				{
					//只取知识表中的，不考虑当前编辑表。
					//modi by wuyougang
					if (Data.存在于模式表中(row.ID) == false && (Data.公共新对象Guid != null && !Data.是子记录(row, Data.公共新对象Guid, true)))
						continue;

					if (row.语境树 < 0 || (仅知识 && row.语境树 != 0))
						continue;

					if (DataIDS.Contains(row.ID))
						continue;

					Guid 一级分类 = 一级关联类型(row);

					//推导不列到这里了。
					if (一级分类.Equals(推导即命题间关系Guid))
						continue;

					if (objectid.Equals(row.A端) && (允许的生长方向(row) & 正向关联) > 0)
					{
						//一、拥有，拥有形式，基本关联，推导等的处理
						if (拥有形式Guid.Equals(一级分类))
						{
							if (包含拥有 && 加入形式 && 字典_语言.满足指定语言(Data.当前解析语言, (int)row.语言))
								objlist.Add(new 参数树结构(row, 字典_目标限定.A端, false/*, 层级*/));
						}
						else if (拥有Guid.Equals(一级分类) || 基本关联Guid.Equals(一级分类) || 并列关联Guid.Equals(一级分类) || 推导即命题间关系Guid.Equals(一级分类))
						{
							if (包含拥有)
								objlist.Add(new 参数树结构(row, 字典_目标限定.A端, false/*, 层级*/));
						}
						else//二、属于，聚合方面的处理
						{
							if (包含拥有 ? 替代.是分类或聚合或属拥(一级分类) : 替代.是属于等价或聚合(一级分类))
								属于list.Add(new 参数树结构(row, 字典_目标限定.A端, false/*, 层级*/));
						}
					}

					else if (objectid.Equals(row.B端) && (允许的生长方向(row) & 反向关联) > 0)
					{
						//一、拥有，拥有形式，基本关联，推导等的处理。拥有甚至拥有形式也都可能从B端（被拥有的一端）触发！
						if (拥有形式Guid.Equals(一级分类))
						{
							if (包含拥有 && 加入形式 && 字典_语言.满足指定语言(Data.当前解析语言, (int)row.语言))
								objlist.Add(new 参数树结构(row, 字典_目标限定.B端, false/*, 层级*/));
						}
						else if (拥有Guid.Equals(一级分类) || 基本关联Guid.Equals(一级分类) || 并列关联Guid.Equals(一级分类) || 推导即命题间关系Guid.Equals(一级分类))
						{
							if (包含拥有)
								objlist.Add(new 参数树结构(row, 字典_目标限定.B端, false/*, 层级*/));
						}
						else//二、属于，聚合方面的处理
						{
							//派生是单向的，这里是B端等于主对象，要排除掉【属于】的部分--也就是其实是派生类，而不是基类。
							if (包含拥有 ? 替代.是等价或聚合或属拥(一级分类) : 替代.是等价或聚合(一级分类))
								属于list.Add(new 参数树结构(row, 字典_目标限定.B端, false/*, 层级*/));
						}
					}

				}
			} while (true);
			objlist.AddRange(属于list);

			return objlist;
		}



		public static List<参数树结构> 得到一批同一层级的派生类(模式 therow, bool 仅知识, float 阀值,/* int 层级,*/ bool 是关联, bool 包括扩展派生)
		{
			Guid objectid = therow.ID;
			List<参数树结构> objlist = new List<参数树结构>();

			//自身记录。而且是[this]等价型的需要这里单独处理。
			if (Data.ThisGuid.Equals(therow.A端) && Data.等价Guid.Equals(therow.连接))
			{
				模式 row = Data.FindRowByID((Guid)therow.B端);
				if (Data.DataIDS.Contains(row.ID) == false)
				{
					objlist.Add(new 参数树结构(row, 字典_目标限定.A端, Data.是二元关联但排除掉等价型(row)/*, 层级*/));
					Data.DataIDS.Add(row.ID);
				}
			}
			int flag = 0;
			if (therow.形式 == "丈夫")
				flag = 0;
			do
			{
				List<模式> 端索引表 = null;
				if (flag == 0 && 是关联 == false)
					端索引表 = therow.端索引表_A端;
				else if (flag == 1 && 是关联 == false)
					端索引表 = therow.端索引表_B端;
				else if (flag == 2 && 是关联 == true)
					端索引表 = therow.端索引表_源记录;
				else if (flag >= 3)
					break;
				flag++;
				if (端索引表 == null)
					continue;
				foreach (模式 row in 端索引表)
				{
					if (row.语境树 < 0 || (仅知识 && row.语境树 != 0))
						continue;

					if (仅知识 && Data.模式表.Exist(row.ID) == false)
						continue;

					if (Data.DataIDS.Contains(row.ID))
						continue;

					if (是关联)
					{
						if (objectid.Equals(row.源记录))
						{
							objlist.Add(new 参数树结构(row, 字典_目标限定.A端, 是关联/*, 层级*/));
							Data.DataIDS.Add(row.ID);
						}
					}
					else
					{
						Guid 一级基本关联 = 一级关联类型(row);
						if (objectid.Equals(row.B端) && (包括扩展派生 ? 替代.可正向替代(一级基本关联) : 替代.是本质正向分类(一级基本关联)))
						{
							模式 r3 = ThisGuid.Equals(row.A端) ? row : FindRowByID((Guid)row.A端);//如果这条记录不是this，那么引用真正的this。
							Assert(ThisGuid.Equals(r3.A端) || 基本关联Guid.Equals(r3.ID));
							objlist.Add(new 参数树结构(r3, 字典_目标限定.A端, 是关联/*, 层级*/));
							Data.DataIDS.Add(row.ID);
						}
						else if (objectid.Equals(row.A端) && 包括扩展派生 && 替代.可反向替代(一级基本关联))
						{
							模式 r3 = FindRowByID((Guid)row.B端);//如果这条记录不是this，那么引用真正的this。
							objlist.Add(new 参数树结构(r3, 字典_目标限定.B端, 是关联/*, 层级*/));
							Data.DataIDS.Add(row.ID);
						}
					}
				}
			} while (true);
			return objlist;
		}

		//给定了具体两个点概念（不再考虑其派生概念），研究它们之间关联，它们之间可能有多个关联，但这些关联必然是并列的而不会有继承关系。
		//也就是，具体两个对象之间的一个关联不会再进行派生细化，除非是这两个对象再派生的对象之间的新关联才可以。
		//现在给定两概念，去判断中间的关联。 是否成立即可。
		//A端和B端是明确的，直接等于，中间的关联可能不用等于，而是可以派生，但只可能有一个。得到一个，就可以返回了。

		static public 模式 查找给定两个对象之间的派生关联(模式 A端, 模式 B端, 模式 关联)
		{
			//var 查询结果集 = Data.get模式编辑表().对象集合.Where(r => r.A端 == A端.ID);

			foreach (模式 row in A端.端索引表_A端)
			{
				if (B端.ID.Equals(row.B端) == false)
					continue;
				if (是派生关联((Guid)关联.ID, row) > 0)
					return row;
			}

			//查询结果集 = 模式表.对象集合.Where(r => r.A端 == A端.ID);

			//foreach (模式 row in 查询结果集)
			//{
			//    if (B端.ID.Equals(row.B端) == false)
			//        continue;
			//    if (是派生关联((Guid)关联.ID, row) > 0)
			//        return row;
			//}

			return null;
		}
		public static float 求打分(DataRow 匹配对象)
		{
			float 打分 = 0;
			return 打分;
		}

		public static float 求打分(DataRow A端, DataRow 关联, DataRow B端)
		{
			float 打分 = 0;
			return 打分;
		}
		//返回结果可能是集合。
		public static object 匹配目标(DataRow 匹配对象, int 未知目标, float 打分阀值)
		{
			return null;
		}
		//返回结果可能是集合。
		public static object 匹配目标(DataRow A端, DataRow 关联, DataRow B端, int 未知目标, float 打分阀值)
		{
			return null;
		}

		public static float A概念对B概念的符合度(Guid A概念, Guid B概念)
		{
			if (A概念.Equals(B概念))//自己肯定符合自己。
				return 符合度.最大符合度;
			if (概念Guid.Equals(B概念))//任何概念都100%满足【基本概念】。
				return 符合度.最大符合度;
			if (概念Guid.Equals(A概念))//【基本概念】0%满足任意概念。
				return 符合度.最小符合度;

			模式 rowA = FindRowByID(A概念);
			参数树结构 treeA = 调整行得到基类和关联树(true, rowA, true, 0, /*0,*/ 当前解析语言);
			if (treeA == null)
				return 0;
			if (treeA.递归从基类树中查找广义匹配的基类(B概念, 替代.正向替代) != -1)//A是B的派生类
				return 符合度.最大符合度;//暂时先这样，但实际上，派生类和基类是等价的，只是比例的不同，实际还是应该提取出比例来进行计算的。

			模式 rowB = FindRowByID(B概念);
			参数树结构 treeB = 调整行得到基类和关联树(true, rowB, true, 0, /*0,*/ 当前解析语言);
			if (treeB == null)
				return 0;

			List<参数树结构> 结果 = new List<参数树结构>();
			treeB.递归得到共同基类(treeA, ref 结果);

			float t = 符合度.最小符合度;
			//其实，共同基类好比中间一个桥，然后从A到B一个链条单向计算，目前暂时假设A到共同基类因为是100%的继承所以值为最大值。
			//因此现在暂时只计算从共同基类到B的符合度。
			foreach (参数树结构 obj in 结果)
			{
				if (概念Guid.Equals(obj.目标ID))//概念肯定是大家都有的共同基类，但这里不发挥作用。
					continue;
				int c = 0;
				float t1 = 0;
				参数树结构 o = obj.父对象;
				while (o != null)//注意：现在计算一个最大的，这样并不合适，应该是把多个路径都计算出来一个总和才对。
				{
					//if (o.目标 != null)//第一个节点可能是空的。
					{
						//参数字段 参数 = new 参数字段(o.目标.参数集合);
						if (c == 0)
							t1 = o.目标.参数.Ab;
						else
							t1 = 符合度.符合度乘积(t1, o.目标.参数.Ab);
						c++;
					}
					o = o.父对象;
				}
				if (c > 0 && t1 > t)
					t = t1;
			}

			return t;
		}
		public static void 填充两个概念相互的关联(Guid A概念, Guid B概念, bool 含属于)
		{
			//DataRow rowB = FindRowByID(B概念);
			//参数树结构 treeB = 调整行得到基类和关联树(true, rowB, false, true, 0, 0, 当前语言);
			//if (treeB == null)
			//    return;

			//DataRow rowA = FindRowByID(A概念);
			//参数树结构 treeA = 调整行得到基类和关联树(true, rowA, false, true, 0, 0, 当前语言);
			//if (treeA == null)
			//    return;

			////相互交叉计算一个对象满足对方的关联的情况。
			//List<参数树结构> 结果1 = new List<参数树结构>();
			//treeB.递归判断被对象满足的各语义树节点(treeA, 字典_目标限定.A端, 结果1);

			//List<参数树结构> 结果2 = new List<参数树结构>();
			//treeA.递归判断被对象满足的各语义树节点(treeB, 字典_目标限定.B端, 结果2);

			//treeB.递归加入参数树到界面(null);

			//treeA.递归加入参数树到界面(null);

			//List<参数树结构> 结果 = new List<参数树结构>();
			//treeB.递归得到共同基类(treeA, ref 结果);

			//float t = 符合度.最小符合度;
			////其实，共同基类好比中间一个桥，然后从A到B一个链条单向计算，目前暂时假设A到共同基类因为是100%的继承所以值为最大值。
			////因此现在暂时只计算从共同基类到B的符合度。
			//foreach (参数树结构 obj in 结果)
			//{
			//    if (概念Guid.Equals(obj.目标ID))//概念肯定是大家都有的共同基类，但这里不发挥作用。
			//        continue;
			//    int c = 0;
			//    float t1 = 0;
			//    参数树结构 o = obj.父对象;
			//    while (o != null)//注意：现在计算一个最大的，这样并不合适，应该是把多个路径都计算出来一个总和才对。
			//    {
			//        if (o.目标 != null)//第一个节点可能是空的。
			//        {
			//            参数集合字段 参数 = new 参数集合字段((string)o.目标["参数集合"]);
			//            if (c == 0)
			//                t1 = 参数.Ab;
			//            else
			//                t1 = 符合度.符合度乘积(t1, 参数.Ab);
			//            c++;
			//        }
			//        o = o.父对象;
			//    }
			//    if (c > 0 && t1 > t)
			//        t = t1;
			//}
		}

		public static void 提取命名实体()
		{
			Data.当前命名实体集合.Clear();

			using (NERUtil.NameEntityRecognizer recongnizer = new NERUtil.NameEntityRecognizer())
			{
				IList<NERUtil.NameEntity> 命名实体集合 = recongnizer.GetNameEntities(Data.当前句子串, Data.当前解析语言);
				foreach (var item in 命名实体集合)
				{
					命名实体 obj = new 命名实体(item.BeginIndex, item.EndIndex, Data.当前解析语言, item.概率);
					switch (item.实体类型)
					{
						case NERUtil.NameEntityType.Number:
							obj.基类型ID = Data.数Guid;
							break;
						case NERUtil.NameEntityType.Identifier:
							obj.基类型ID = Data.项目编号Guid;
							break;
						case NERUtil.NameEntityType.Date:
							obj.基类型ID = Data.日期Guid;
							break;
						case NERUtil.NameEntityType.Time:
							obj.基类型ID = Data.时间点Guid;
							break;
						case NERUtil.NameEntityType.PersonName:
							obj.基类型ID = Data.人Guid;
							break;
						case NERUtil.NameEntityType.OrganizationName:
							obj.基类型ID = Data.组织Guid;
							break;
						case NERUtil.NameEntityType.LocationName:
							obj.基类型ID = Data.地点Guid;
							break;
						case NERUtil.NameEntityType.OtherName:
							// TODO: 其他类型命名实体处理
							break;
					}

					if (obj.基类型ID.Equals(Guid.Empty) == false)
						Data.当前命名实体集合.Add(obj);
				}
			}
		}

		public static bool 构建命名实体语义和形式模式行(Guid 基类ID, string 串形式, int 概率, int 语言, out 模式 语义Row, out 模式 形式Row)
		{
			// 首先判断模式表是否已存在该命名实体
			形式Row = 从模式编辑表递归查找语义和形式模式行(基类ID, 串形式, 语言);
			if (形式Row != null)
			{
				语义Row = Data.FindRowByID(形式Row.ParentID);// 模式表.对象集合.Union(模式编辑表.对象集合).Single(r => r.ID == 语义RowID);
				return true;
			}

			// 构建语义模式行
			语义Row = Data.New派生行(基类ID, 字典_目标限定.A端, true);
			//语义Row.ParentID = 上级语义Guid;
			语义Row.形式 = "[" + 串形式 + "]";

			// 构建形式模式行, 代码来源：增加形式行_ItemClick方法
			形式Row = Data.New派生行(Data.拥有形式Guid);
			形式Row.ParentID = 语义Row.ID;
			形式Row.A端 = 语义Row.ID;
			形式Row.B端 = Data.ThisGuid;
			形式Row.形式 = 串形式;
			形式Row.语言 = 语言;
			形式Row.语言角色 = 字典_语言角色.全部;

			语义Row.ParentID = Data.公共新对象Guid;// Data.当前新对象Row.ID;
			Data.get模式编辑表().新加对象(语义Row);
			Data.get模式编辑表().新加对象(形式Row);
			return false;
		}

		/// <summary>
		/// 根据基类ID, 串形式和语言，从模式表中查找对应的语义模式行和拥有形式模式行
		/// </summary>
		/// <returns>拥有形式模式行, 模式行的ParentID为语义行Guid</returns>
		public static 模式 从模式编辑表递归查找语义和形式模式行(Guid 基类ID, string 串形式, int 语言)
		{
			模式 形式行 = null;
			//List<模式> 基类一级子语义模式集合 = Data.get模式编辑表().对象集合.Where(r => r.ParentID == Data.当前新对象Row.ID && r.A端 == ThisGuid && r.源记录 == 基类ID && r.B端 == 基类ID && r.连接 == Data.属于Guid && r.That根 == 字典_目标限定.A端).ToList();
			//模式表.对象集合.Union(模式编辑表.对象集合.Where(x => x.ParentID == Data.当前新对象Row.ID)).Where(r => r.A端 == ThisGuid && r.源记录 == 基类ID && r.B端 == 基类ID && r.连接 == Data.属于Guid && r.That根 == 字典_目标限定.A端).ToList();
			foreach (模式 item in Data.FindRowByID(Data.公共新对象Guid).端索引表_Parent)//Data.当前新对象Row.端索引表_Parent)
			{
				if (item.A端 == ThisGuid && item.源记录 == 基类ID && item.B端 == 基类ID && item.连接 == Data.属于Guid && item.That根 == 字典_目标限定.A端)
				{
					//形式行 = Data.get模式编辑表().对象集合.Where(x => x.ParentID == item.ID && x.A端 == item.ID && x.B端 == ThisGuid && (x.语言 & 语言) > 0 && Data.是拥有形式(x) && Data.取得嵌入串(x).Equals(串形式)).FirstOrDefault();
					//模式表.对象集合.Union(模式编辑表.对象集合).Where(x => x.A端 == item.ID && x.B端 == ThisGuid && (x.语言 & 语言) > 0 && Data.是拥有形式(x) && Data.取得嵌入串(x).Equals(串形式)).FirstOrDefault();
					foreach (模式 row in item.端索引表_Parent)
					{
						if (row.A端 == item.ID && row.B端 == ThisGuid && (row.语言 & 语言) > 0 && Data.是拥有形式(row) && Data.取得嵌入串(row).Equals(串形式))
							形式行 = row;
					}
					if (形式行 == null)
						形式行 = 从模式编辑表递归查找语义和形式模式行(item.ID, 串形式, 语言);
					else
						break;
				}
			}
			return 形式行;
		}



		//public static 匹配语料对象 构建命名实体匹配语料对象(命名实体 实体对象, 模式 形式Row)
		//{
		//    形式化语料串 语料串 = Data.语料库.FirstOrDefault(r => r.字符串.Equals(实体对象.ToString()));
		//    if (语料串 == null)
		//        语料串 = 加入形式串(形式Row.ID, 实体对象.ToString());

		//    return new 匹配语料对象(实体对象.begindex, 实体对象.endindex, 语料串);
		//}
	}

	public class 命名实体 : SubString
	{
		public 命名实体(int begindex, int endindex, int 语言, int 概率)
		{
			this.begindex = begindex;
			this.endindex = endindex;
			this.语言 = 语言;
			this.概率 = 概率;
		}

		public Guid 基类型ID { get; set; }
		public int 语言 { get; set; }
		public int 概率 { get; set; }
		public bool 是否已存在 { get; set; }
	}

	public class 间隙 : IComparable
	{
		public int 融合度;
		public int 位置索引;
		public 间隙(int 融合度, int 位置索引)
		{
			this.融合度 = 融合度;
			this.位置索引 = 位置索引;
		}

		public int CompareTo(object other)
		{
			间隙 v2 = other as 间隙;
			if (this.融合度 > v2.融合度)
				return 1;
			if (this.融合度 < v2.融合度)
				return -1;
			return 0;
		}

		public override string ToString()
		{
			return "位置：" + 位置索引.ToString() + "融合度：" + 融合度.ToString();
		}

	}
	public class 新词汇
	{
		public 生长对象 类型对象;
		public 生长对象 外围中心对象;
		public 参数 关联参数;
		public string 形式串 = "";
		public int begindex = 0;
		public int 忽略前begindex = -1;
		public int 偏移量 = 0;
		public bool 已增加 = false;
		public Guid 类型ID;
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
		public static int 英语单词 = 200;
		public static int 数字 = 300;
		public static int 计算字符类型(char c)
		{
			if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))//字母
				return 英语单词;
			if ((c >= '0' && c <= '9'))//数字
				return 数字;
			if (c >= 0x4e00 && c <= 0x9fa5)//现在暂时假设每一个汉字是一个词
				return 汉字;
			return 0;
		}
	}


	public class 一次说话
	{
		public int 说话方;//计算机也当作一个人
		public int 听话方;//这个不一定能明确
		public int 时间;//
		public string 说话内容;//
	}

	public class 一组对话
	{
		public List<一次说话> 对话链;
	}

	//各个实体对象将在不同的容器中存放，多个容器中存放的，可能存在不同的对应关系。

	//这个对应着在上帝眼里看到的，也就是客观的东西
	public class 客观容器
	{

	}

	//这个对应着相应的一些内容在一个个体大脑中的存储和映射
	public class 个人思维容器
	{
	}

	public class 任务对象
	{
		public 模式树结构 当前模式;

		public 模式 任务目标;//比如【张三要买一张飞机票】。

		public 模式 本句话最优模式;

		public 生长对象 本句话最优对象;

		public List<模式树结构> 模式对象池 = new List<模式树结构>();

		public List<推导结构> 推导结构池 = new List<推导结构>();


	}


	public class Processor
	{
		public static EnglishProcessor english = new EnglishProcessor();
		public static ChineseProcessor chinese = new ChineseProcessor();
		//记录上一个语言处理器，这样本处理器如果在不懂的情况下，可能交给上一个处理的。
		public Processor 上一个语言处理器;

		public static Processor 当前处理器
		{
			get
			{
				if (字典_语言.满足指定语言(字典_语言.英语, Data.当前解析语言))
					return Processor.english;
				if (字典_语言.满足指定语言(字典_语言.汉语, Data.当前解析语言))
					return Processor.chinese;
				return Processor.english;
			}
		}

		static public int 处理独立语 = 1;
		static public int 第一轮处理 = 2;
		static public int 生长_正常处理 = 1;
		static public int 生长_名词谓语处理 = 2;
		static public int 生长_集合处理 = 4;
		static public int 生长_两组动词并列处理 = 8;
		static public int 生长_连动处理 = 16;
		static public int 生长_全部 = -1;
		public int 处理选项 = 0;
		//public int 处理阶段 = 0;
		public int 句子合并和深度再处理阶段 = 0;
		/// <summary>
		/// 用字符数组处理效率高。
		/// 以后可以改为C++。
		/// </summary>
		public char[] 源语言文字;
		public List<间隙> 间隙数组;//记录字符中间的间隙的值。

		//按照语言要求筛选出来的所有可能的串
		public List<string> allresults = new List<string>();

		//所有和语料库里边匹配上的串
		public List<匹配语料对象> matchresults = new List<匹配语料对象>();

		//对一个句子构建的分析的对象池
		//所有可能的对象都会放在里边，包括冲突的
		//放置所有的已经长成的对象。
		public 生长对象 起始哨兵;
		public 生长对象 结束哨兵;
		public 任务对象 任务对象;
		public List<生长对象> 全部对象 = new List<生长对象>();
		public List<生长对象> 左边界排序对象 = new List<生长对象>();
		public List<生长对象> 右边界排序对象 = new List<生长对象>();
		public List<生长对象> 本轮结果集 = new List<生长对象>();
		public List<生长对象> 待选用抽象对象集 = new List<生长对象>();
		//public int 连续生长开始轮数;
		public List<生长对象> 被抑制对象 = new List<生长对象>();
		//public List<生长对象> 待创建对象 = new List<参数树结构>();
		public List<生长对象> 新知识对象 = new List<生长对象>();
		public List<生长对象> 一级原始对象 = new List<生长对象>();
		public List<关联对象对> 待生长对象对集合 = new List<关联对象对>();
		public HashSet<生长对象> 已进行关联创建对象集合 = new HashSet<生长对象>();
		//记录等待创建的一级派生对象。和抑制无关，这里边的对象相当于不够创建阀值还没有真正创建的对象。
		public List<待创建对象结构> 同名待创建对象 = new List<待创建对象结构>();
		public List<生长对象> 被处理阶段抑制对象 = new List<生长对象>();
		public bool 等待确认加入新知识;
		//public int 覆盖型对象标识生成器;
		public List<模式匹配结果节点> 模式匹配结果集合 = new List<模式匹配结果节点>();
		public List<生长对象> 全部匹配结果 = new List<生长对象>();
		public List<模式> 分析结果树 = new List<模式>();
		public List<生长对象遍历节点> 全部遍历结构 = new List<生长对象遍历节点>();
		public List<生长对象> 递归生长的对象链 = new List<生长对象>();
		public List<生长对象> 强势完成对象集合 = new List<生长对象>();
		public List<新词汇> 新词汇集合 = new List<新词汇>();
		public List<模式> 附加关联集合 = new List<模式>();

		public 封闭范围 内部范围, 当前内部范围;
		public int 当前遍历位置;
		public 模式 名词谓语关联;

		public string 分析结果串;

		public int 处理轮数;//每一轮只让上一轮新产生的对象可以发起请求。
		public int 有效对象计数;
		//public int 推导个数;
		public int 生长阶段;
		public int 意义优先性阀值 = 0;
		/// <summary>
		/// 这里是为了方便和语料库里边的句子进行匹配处理，
		/// 在预处理把下列符号进行替换处理，替换的情况可以记录下来，最后结果需要的话可以再替换回来。
		/// 第一个字符是将被替换成的目标字符，后边的字符是将被替换的
		/// 也就意味着这些字符是允许出现在汉语语料库中的，如果不允许出现，那么，就没有必要在这里处理了。
		/// </summary>
		public string[] 等价字符 = new string[]{
       "'‘’",//全角单引号
       "\"“”",//全角双引号
       "!！",//全角惊叹号
       "-－—",//全角减号和破折号
       "=＝",//全角等于号
       "++",//全角加号
       "(（",//全角括号
       ")）",//全角括号
       "[【",//全角中括号
       "]】",//全角中括号
       "{｛",//全角大括号
       "}｝",//全角大括号
       "<《",//全角尖括号
       ">》",//全角尖括号
       ":：",//全角冒号
       "?？",//全角问号
        ";；",//全角分号
       ",，",//全角逗号
       " 　"//全角空格
       };

		//分段肯定就会分行。
		public char[] 分段字符 = new char[] { '\n' };

		//这些是比较固定的字符，但是分句子很可能在碰到其他一些不易处理的字符时候也会进行分句处理。
		public char[] 普通分句字符 = new char[] { '.' };
		public char[] 汉语分句字符 = new char[] { '。' };

		//定义这个方法来进行统一的创建对象。其中有两种模式。
		//一种是：创建起始对象。基对象为空，也就是完全根据【参数】来创建，一般都是【拥有参数】，比如根据字符串参数“借”创建【借】，根据【如果】创建【推导】。
		//二种是：创建派生对象。已经有了基对象，然后增加了参数。比如【借】的基础上有了【借出方主语】以后，就可以创建【借出】。
		public List<生长对象> 最新_统一创建对象(生长对象 参数, 生长对象 基对象 = null)
		{
			return null;
		}

		public virtual char[] Get分段字符()
		{
			return 分段字符;
		}


		public virtual char[] Get分句子字符()
		{
			return 普通分句字符;
		}

		public void 保存新知识()
		{
			模式 returnRow = null;
			foreach (生长对象 obj in 新知识对象)
			{
				if (Data.是介词或者串(obj.源模式行, false, false, true)) //是添加形式串的新知识
				{
					模式 parent = Data.FindRowByID(obj.模式行.A端);
					模式 row = Data.New派生行(Data.拥有形式Guid, 字典_目标限定.连接, false);
					row.形式 = obj.模式行.形式;
					row.语言 = obj.模式行.语言;
					row.语言角色 = 字典_语言角色.全部;
					row.B端 = Data.ThisGuid;
					row.A端 = parent.ID;
					row.ParentID = parent.ID;

					模式基表 目标表 = Data.FindTableByID(parent.ID);
					目标表.新加对象(row);
					Data.刷一条记录的根(row);

					returnRow = row;
				}
				else //是添加语义对象的新知识
				{
					模式 row = obj.模式行;
					模式 root = Data.FindRowByID((Guid)row.B端);
					if (root == null)
						continue;
					var 数据 = Data.CopyTree(row);
					returnRow = Data.PasteRows(数据, true, 0, root);
				}
			}

			if (returnRow != null && Data.动态绑定至Form)
				Data.模式frm.定位选择模式行(returnRow.ID, false, false, false);
		}

		public void 加入对象到当前对象集(生长对象 对象)
		{
			对象.有效对象序数 = 有效对象计数++;
			本轮结果集.Add(对象);
		}

		public void 去除最近的未完成生长对象(int 起始序数)
		{
			for (int i = 0; i < 本轮结果集.Count; i++)
			{
				if (本轮结果集[i].有效对象序数 >= 起始序数)
					本轮结果集.RemoveAt(i--);
			}
		}

		public void 进行翻译()
		{
			生长对象 最优对象 = 任务对象.本句话最优对象;
			if (最优对象 == null)
			{
				Data.输出对话信息("!解析未成功，不能完成翻译!", true);
				return;
			}

			if (分析结果树.Count() == 0)
				return;

			模式 第一个分析结果树 = 分析结果树[0];
			List<模式> rows = Data.CopyTree(第一个分析结果树);
			模式 row = Data.PasteRows(rows, false, 第一个分析结果树.序号 + 100, 第一个分析结果树);
			//int 语言 = Data.当前生成语言;
			//Data.当前生成语言 = 字典_语言.英语;
			//Data.递归调序(row);
            Data.第一阶段展开设置角色及默认次序(row);
            Data.第二阶段根据语序微调角色(row);
			//Data.第二阶段根据语序微调角色(row);
			Data.第三阶段构造表现形式(row, false);
			Data.第四阶段最终生成形式串(row, Data.当前生成语言);
			Data.递归设置语境树知识有效性(row, 0);
			//Data.当前生成语言 = 语言;
			//string str = Data.取得嵌入串(row);
			string str = row.形式;
			//Data.输出处理信息("翻译", "英语");
			Data.输出处理信息("翻译", Data.模式frm.选择生成语言.EditValue.ToString());
			Data.输出对话信息(str, true);
		}

		public void 进行查询处理()
		{

			List<模式> 全部查询结果 = 考虑各种推导对问题查询出答案(任务对象.本句话最优模式, Data.当前句子Row);

			if (全部查询结果 == null || 全部查询结果.Count() == 0)
			{
				Data.输出对话信息("没有找到匹配的答案。", true);
				return;
			}

			Data.输出处理信息("查询", "匹配结果个数：" + 全部查询结果.Count.ToString());
			string str = "";// "推导结果=" + 全部推导结果.Count.ToString() + "个：";
			int i = 0;

			foreach (模式 row in 全部查询结果)
			{
				//1.创建模式树并替换匹配节点
				//2.重新构建形式
				Data.第一阶段展开设置角色及默认次序(row);
				Data.第二阶段根据语序微调角色(row);
				Data.第三阶段构造表现形式(row, false);
				Data.第四阶段最终生成形式串(row, Data.当前生成语言);
				row.序号 = (i + 1) * 100;
				Data.递归设置语境树知识有效性(row, -1);//查询的结果不能再作为知识。
				string 答案 = "";
				if (Data.简略回答)
					答案 = 从答案模式树递归获取简略回答结果(row);
				else
					答案 = (string.IsNullOrEmpty(Data.取得嵌入串(row)) ? row.形式 : Data.取得嵌入串(row));
				if (string.IsNullOrEmpty(答案) == false)
					str += "\r\n　　 " + (++i).ToString() + "：" + 答案;
			}
			if (str != "")
				Data.输出对话信息(str, true);
            Data.递归设置语境树知识有效性(Data.当前句子Row, -1);
		}
		public string 从答案模式树递归获取简略回答结果(模式 答案树)
		{
			string ret = "";
			if (答案树.被替换标记)
			{
				if (Data.是派生类(Data.概念属拥句子Guid, 答案树, 替代.正向替代) == false && Data.是派生类(Data.句子语用基类Guid, 答案树, 替代.正向替代) == false)
					return string.IsNullOrEmpty(Data.取得嵌入串(答案树)) ? 答案树.形式 : Data.取得嵌入串(答案树);
			}
			foreach (模式 r in 答案树.端索引表_Parent.OrderBy(r => r.序号))
			{
				string s = 从答案模式树递归获取简略回答结果(r);
				if (string.IsNullOrEmpty(s) == false)
				{
					if (string.IsNullOrEmpty(ret))
						ret = s;
					else
						return ret + " " + s;
				}
			}
			return ret;
		}
		public List<模式> 将目标句子进行拆解(模式 目标句子)
		{
			List<模式> 短句集合 = new List<模式>();
            递归拆解动词为中心的短句(目标句子, 短句集合);
            短句集合.Add(目标句子);
			return 短句集合;
        }
        public bool 递归拆解动词为中心的短句(模式 目标句子, List<模式> 短句集合)
        {
            bool ret = false;
			foreach (模式 row in 目标句子.端索引表_Parent)
			{
                //只能是[松散并列]、[连动]关联的动词短句
                if (Data.是派生关联(Data.拥有连动Guid, row) > 0 || Data.是派生关联(Data.松散并列Guid, row) > 0
                     || Data.是派生关联(Data.动词拥有紧密并列动词Guid, row) > 0)
				{
                    foreach (模式 r in row.端索引表_Parent)
                    {
                        ret = 递归拆解动词为中心的短句(r, 短句集合);
                        //结合了主语的动词加入短句集合(如果当前短句，内部嵌套了动词短句，则当前短句不管有没有结合主语都需要加入集合)
                        if (Data.能够序列化(r) && (ret==true || 判断目标句子是否已结合指定的语言角色(r,字典_语言角色.主语)))
                        {
                            短句集合.Add(r);
                            ret = true;
                }
                    }
                }
			}
            return ret;
		}
        public bool 判断目标句子是否已结合指定的语言角色(模式 目标句子, int 语言角色)
		{
			foreach (模式 row in 目标句子.端索引表_Parent)
			{
                if ((row.语言角色 & 语言角色) > 0)
                    return true;
			}
            return false;
		}

		public void 递归处理推导结果(模式 模式row, 推导结构 上一个推导结构,int 层数, ref string str)
		{
            //将目标句子进行拆解后分别推导
			if (层数 > 3)
                return;

			模式树结构 tree = 模式树结构.从一个根模式生成模式树结构(模式row);

			tree.匹配并执行后置推导(上一个推导结构);

			//string str = "推导结果个数：" + 全部推导结果.Count.ToString();
			for (int i = 0; i < tree.后置推导.Count(); i++)
			{

				//模式 row = 生成一棵树(全部推导结果[i].推导目标对象, false, Data.当前句子Row, 2, Data.当前生成语言);
				模式 row = tree.后置推导[i].结果端模式树结构.从模式树结构创建模式树(Data.当前句子Row, false,true);
				row.序号 = 层数 * 100 + (i + 1) ;

				参数字段 参数 = tree.后置推导[i].源推导模式行.参数;//new 参数字段((string)推导记录[i].目标.参数集合);

				Data.递归设置语境树知识有效性(row, 0);

				if (Data.解释和推导)
				{
					Data.递归调序(row);
					Data.第三阶段构造表现形式(row, false);
					Data.第四阶段最终生成形式串(row, Data.当前生成语言);
					str += "\r\n    " + (i + 1).ToString() + "、" + (string.IsNullOrEmpty(Data.取得嵌入串(row)) ? row.形式 : Data.取得嵌入串(row));
					str += "【概率=" + ((参数.Aa + 1) * 10).ToString() + "%】";
					//if (深度 < 1 && Data.是派生类(Data.方程式Guid, 原对象.中心第一根类.源模式行, 替代.正向替代) == false)//增加一级推导。排除掉算式方程式，因为会死循环，以后要加另一种完备的检测方法。
					//	进行推导处理(全部推导结果[i],深度+1);
				}

				if (tree.后置推导[i].推导状态 != 1)
					continue;

				递归处理推导结果(row,tree.后置推导[i], 层数 + 1, ref str);
			}
		}

		public void 进行推导处理()
		{
			//推导是将匹配出的模式看着一个端点，然后推演出包含这个模式的【推导】模式和另一个端点的模式。也就是进行扩展。
			string str = "结果：";

			List<模式> 分解短句 = 将目标句子进行拆解(任务对象.本句话最优模式);

			foreach (模式 o in 分解短句)
			{
				递归处理推导结果(o, null, 0, ref str);
			}

			if (Data.解释和推导)
				Data.输出处理信息("推导", str);
		}

		public void 尝试对问题进行查询对命题进行推导()
		{

			本轮结果集.Clear();

			模式行复制关系.模式行复制关系集合.Clear();

			生长对象 最优对象 = 任务对象.本句话最优对象;

			if (最优对象 == null)
				return;

			全部匹配结果.Clear();
			全部遍历结构.Clear();

			//寻找后边完成了，前边没有完成的。
			//完成了句子的完整匹配后才做这些。

			if (最优对象.这个参数重复满足(最优对象.中心第一根类, Data.FindRowByID(Data.概念属拥句子Guid), Data.FindRowByID(Data.疑问句Guid), false) > 0)
			{
				//查询是对匹配的模式本身进行完善（填写出疑问对应的参数）进行输出。
				//{
				//对查询匹配派生答案(最优对象);

				//	Data.输出处理信息("查询", "匹配结果个数：" + 全部匹配结果.Count.ToString());
				//	string str = "";// "推导结果=" + 全部推导结果.Count.ToString() + "个：";
				//	int i = 1;

				//	foreach (生长对象 o in 全部匹配结果)
				//	{
				//		模式 row = 生成一棵树(o, false, Data.当前句子Row, 0, Data.当前生成语言);
				//		row.序号 = i * 100;
				//		Data.递归设置语境树知识有效性(row, -1);//查询的结果不能再作为知识。
				//		str += "\r\n　　 " + i++.ToString() + "：" + (string.IsNullOrEmpty(Data.取得嵌入串(row)) ? row.形式 : Data.取得嵌入串(row));
				//	}

				//	if (str != "")
				//		Data.输出对话信息(str, true);
				//}


				进行查询处理();


			}
			else if (最优对象.这个参数重复满足(最优对象.中心第一根类, Data.FindRowByID(Data.概念属拥句子Guid), Data.FindRowByID(Data.陈述句Guid), false) > 0)
			{
				foreach (模式 r in 分析结果树)
					Data.递归设置语境树知识有效性(r, 0);//成立的情况下设置为知识。

				进行推导处理();

				Data.输出对话信息("明白。", true);
			}
		}
		public bool 判断指定位置是否为某个类型的对象(int beg, int end, Guid 类型Guid)
		{
			for (int i = 在左边界排序对象中定位(beg); i < 左边界排序对象.Count; i++)
			{
				生长对象 obj = 左边界排序对象[i];
				if (obj.endindex > end)
					break;
				if (obj.begindex == beg && obj.endindex == end)
				{
					if (Data.是派生类(类型Guid, obj.源模式行, 替代.正向替代))
						return true;
				}
			}
			return false;
		}
		//用于在对话中，检查是否有类似于“苹果的英文名是apple”的新增形式串的对象
		public void 尝试构建添加形式的新知识()
		{
			生长对象 合适的新增对象模板 = null;
			foreach (生长对象 o in 全部对象)
			{
				if (o.begindex == 0 && o.endindex == Data.当前句子串.Length)//已经有全部完成的
					return;
				//寻找前边完成了，后边没有完成的。
				if (o.endindex == Data.当前句子串.Length)
					continue;
				if (o.begindex != 0)
					continue;
				if (Data.属于Guid.Equals(o.中心第一根类.源模式行ID) == false)//不是【是】则不处理
					continue;
				if (o.查找已经实现的参数(Data.FindRowByID(Data.关联拥有B端Guid)) != null)
					continue;
				if (o.查找已经实现的参数(Data.FindRowByID(Data.关联拥有A端Guid)) == null)
					continue;
				if (o.中心第一根类.是隐藏对象())//必须是显式的【属于】才考虑。
					continue;
				合适的新增对象模板 = o;
				break;
			}
			if (合适的新增对象模板 == null)
				return;
			int end = Data.当前句子串.Length;
			//去掉右边的句号
			if (判断指定位置是否为某个类型的对象(Data.当前句子串.Length - 1, Data.当前句子串.Length, Data.句子语用基类Guid))
				end = Data.当前句子串.Length - 1;
			SubString str = new SubString(合适的新增对象模板.endindex, end);
			List<参数> 概念参数表 = 合适的新增对象模板.得到指定根对象的参数表(合适的新增对象模板.中心第一根类);
			生长对象 A端 = null;
			foreach (参数 obj in /*属于对象.概念参数表*/概念参数表)
			{
				if (obj.已经派生() == false)
					continue;
				if (Data.是派生关联(Data.关联拥有B端Guid, obj.源关联记录) > 0)
					return;//拥有A端已经满足了。
				else if (Data.是派生关联(Data.关联拥有A端Guid, obj.源关联记录) > 0)
				{
					A端 = obj.对端派生对象.B端对象;
					break;
				}
			}
			if (A端 != null && Data.是派生类(Data.名称Guid, A端.中心第一根类.源模式行, 替代.正向替代))
			{
				生长对象 语义对象 = null;
				生长对象 语言对象 = null;
				参数 参数 = A端.查找已经实现的参数(Data.FindRowByID(Data.概念拥有子名称Guid));
				if (参数 != null)
					语义对象 = 参数.对端派生对象.B端对象.中心第一根类;
				else
				{
					if (Data.是派生关联(Data.概念拥有子名称Guid, A端.源模式行) > 0)
						语义对象 = A端.参数对象;
				}
				参数 = A端.查找已经实现的参数(Data.FindRowByID(Data.名称拥有语言Guid));
				if (参数 != null)
					语言对象 = 参数.对端派生对象.B端对象.中心第一根类;
				if (语义对象 != null)
				{
					//生成串对象
					模式 字串row = Data.增加字符串生长素材(str);
					字串row.A端 = 语义对象.源模式行ID;
					if (语言对象 != null)
						字串row.语言 = 语言对照类.获取语言ID(语言对象.源模式行.ID);
					else
						字串row.语言 = Data.当前解析语言;
					字串row.序号 = str.begindex;
					新知识对象.Add(new 生长对象(字串row, 0));

					//创建语义对象及形式行
					模式 形式row = Data.创建新的语义对象到公共新对象(str, 语义对象.源模式行);
					//根据新创建的形式行，创建一级语义生长对象
					模式 素材row = Data.增加字符串生长素材(str);
					生长对象 串对象 = new 生长对象(素材row, 0);
					串对象.begindex = str.begindex;
					串对象.endindex = str.endindex;
					加入一个对象到池(串对象);

					生长对象 新对象 = 构造一级语义对象(串对象, 形式row, 0);
					新对象.中心对象 = 串对象;
					新对象.一级对象构建形式和关键参数(串对象);
					加入一个对象到池(新对象);

					//尝试生长
					生长对象 计算对象对 = 未知关联构造待分析对象对(合适的新增对象模板, 新对象);

					if (计算对象对 == null)
						return;

					计算对象对.设置源模式行(Data.FindRowByID(Data.关联拥有B端Guid));
					直接一级关联生长(计算对象对, 处理轮数, true);

					foreach (生长对象 o in 本轮结果集)
						加入一个对象到池(o);

					进行一轮生长();
				}
			}
		}
		public void 第四阶段尝试构建属于模式的新知识()
		{
			if (Data.检查新知识 == false)
				return;
			本轮结果集.Clear();

			生长对象 合适的新增对象模板 = null;
			foreach (生长对象 o in 全部对象)
			{
				if (o.begindex == 0 && o.endindex == Data.当前句子串.Length)//已经有全部完成的
					return;
				//寻找后边完成了，前边没有完成的。
				if (o.begindex == 0)
					continue;
				if (判断相邻对象是否为指定类型(o, Data.句子语用基类Guid, true))
				{
					if (o.endindex != Data.当前句子串.Length - 1)
						continue;
				}
				else
				{
					if (o.endindex != Data.当前句子串.Length)
						continue;
				}
				if (Data.属于Guid.Equals(o.中心第一根类.源模式行ID) == false)//不是【是】则不处理
					continue;
				if (o.查找已经实现的参数(Data.FindRowByID(Data.关联拥有A端Guid)) != null)
					continue;
				if (o.查找已经实现的参数(Data.FindRowByID(Data.关联拥有B端Guid)) == null)
					continue;
				if (o.中心第一根类.是隐藏对象())//必须是显式的【属于】才考虑。
					continue;
				合适的新增对象模板 = o;
				break;
			}

			if (合适的新增对象模板 == null)
			{
				尝试构建添加形式的新知识();
				return;
			}
			SubString str = new SubString(0, 合适的新增对象模板.begindex);

			if (在语料库中查找串(str.ToString()) != null)//已经存在的串不考虑。
				return;

			生长对象 属于对象 = 合适的新增对象模板.在一棵树上查找指定根的最新版本对象(合适的新增对象模板.中心第一根类);

			生长对象 B端 = null;

			List<参数> 概念参数表 = 合适的新增对象模板.得到指定根对象的参数表(合适的新增对象模板.中心第一根类);

			foreach (参数 obj in /*属于对象.概念参数表*/概念参数表)
			{
				if (obj.已经派生() == false)
					continue;
				if (Data.是派生关联(Data.关联拥有A端Guid, obj.源关联记录) > 0)
					return;//拥有A端已经满足了。
				else if (Data.是派生关联(Data.关联拥有B端Guid, obj.源关联记录) > 0)
				{
					B端 = obj.对端派生对象.B端对象.中心第一根类;
					break;
				}
			}

			if (B端 == null)
				return;
			//创建语义对象及形式行
			模式 形式row = Data.创建新的语义对象到公共新对象(str, B端.源模式行);
			新知识对象.Add(new 生长对象(Data.FindRowByID(形式row.A端), 0));
			//根据新创建的形式行，创建一级语义生长对象
			模式 字串row = Data.增加字符串生长素材(str);
			生长对象 串对象 = new 生长对象(字串row, 0);
			串对象.begindex = str.begindex;
			串对象.endindex = str.endindex;
			加入一个对象到池(串对象);

			生长对象 新对象 = 构造一级语义对象(串对象, 形式row, 0);
			新对象.中心对象 = 串对象;
			新对象.一级对象构建形式和关键参数(串对象);
			加入一个对象到池(新对象);

			//尝试生长
			生长对象 计算对象对 = 未知关联构造待分析对象对(属于对象, 新对象);

			if (计算对象对 == null)
				return;

			计算对象对.设置源模式行(Data.FindRowByID(Data.关联拥有A端Guid));
			直接一级关联生长(计算对象对, 处理轮数, true);

			foreach (生长对象 o in 本轮结果集)
				加入一个对象到池(o);

			进行一轮生长();

		}

		public void 把对象放入被抑制集合(生长对象 obj)
		{
			if (obj.概率打分 <= 0)
				return;
			int i;
			//按照概率打分排序进行插入
			for (i = 0; i < 被抑制对象.Count; i++)
			{
				生长对象 o = 被抑制对象[i];
				if (obj.长度 > o.长度)
					break;
				if (obj.长度 == o.长度)
					if (obj.概率打分 > o.概率打分)//长度相同，概率优先。
						break;
			}
			被抑制对象.Insert(i, obj);
		}

		public 生长对象 取出一个被抑制对象()
		{
			if (被抑制对象.Count() == 0)
				return null;
			生长对象 obj = 被抑制对象[0];
			被抑制对象.RemoveAt(0);
			return obj;
		}

		public void 加入一个同名待创建对象(模式 派生对象, 生长对象 发起参数)
		{
			同名待创建对象.Add(new 待创建对象结构(派生对象, 发起参数));
		}

		public void 标记一个同名待创建对象已经完成(待创建对象结构 待创建对象结构, 生长对象 已完成创建对象)
		{
			待创建对象结构.已创建对象 = 已完成创建对象;
		}

		public 生长对象 查找串对象(int begindex, int endindex)
		{
			if (endindex == begindex)
				return null;
			foreach (生长对象 obj in 全部对象)
				if (obj.begindex == begindex && obj.endindex == endindex)
					if (obj.中心对象 == null)
						return obj;
			return null;
		}
		public int 对象比较(生长对象 o1, 生长对象 o2)
		{
			if (o1 == o2)
				return 0;
			if (o1.长度 > o2.长度)
				return 1;
			if (o1.长度 < o2.长度)
				return -1;
			bool 是串1 = o1.中心对象 == null;
			bool 是串2 = o2.中心对象 == null;
			if (是串1 && 是串2 == false)
				return -1;
			if (是串1 == false && 是串2)
				return 1;
			if (o1.概率打分 > o2.概率打分)
				return 1;
			if (o1.概率打分 < o2.概率打分)
				return -1;
			return 0;

		}

		public void 设置集合基类型(生长对象 对象对)
		{
			if (Data.是派生类(Data.抽象形式集合Guid, 对象对.中心对象.中心第一根类.源模式行, 替代.正向替代))
			//&& Data.是派生关联(Data.抽象形式集合拥有元素Guid, 对象对.源模式行) > 0)
			{
				if (对象对.中心对象.集合对象的基对象 == null || Data.ThisGuid.Equals(对象对.中心对象.集合对象的基对象.ID))//空集合
					对象对.集合对象的基对象 = 对象对.参数对象.中心第一根类.源模式行;
				else
				{
					并列关联 关联 = 计算两元素的共同基类(对象对.左对象, 对象对.右对象);
					if (关联 == null)
						对象对.集合对象的基对象 = 对象对.中心对象.集合对象的基对象;
					else
						对象对.集合对象的基对象 = 关联.基类;//记下集合的基模式。
				}
			}

		}

		public void 加入一个对象到池(生长对象 obj)
		{

			if (obj.有效对象序数 == 0)
				obj.有效对象序数 = 有效对象计数++;

			obj.重计算打分();

			if (Data.动态绑定至Form)
				obj.同步解析结果到结果表();

			int i;

			//按照打分排序进行插入
			for (i = 0; i < 全部对象.Count; i++)
			{
				生长对象 o = 全部对象[i];
				if (对象比较(obj, o) >= 0)
					break;
			}
			全部对象.Insert(i, obj);

			for (i = 0; i < 左边界排序对象.Count; i++)
			{
				生长对象 o = 左边界排序对象[i];
				if (obj.begindex > o.begindex)
					continue;
				if (obj.begindex < o.begindex)
					break;
				if (对象比较(obj, o) >= 0)
					break;
			}
			左边界排序对象.Insert(i, obj);


			for (i = 0; i < 右边界排序对象.Count; i++)
			{
				生长对象 o = 右边界排序对象[i];
				if (obj.endindex < o.endindex)
					continue;
				if (obj.endindex > o.endindex)
					break;
				if (对象比较(obj, o) >= 0)
					break;
			}
			右边界排序对象.Insert(i, obj);
		}

		public 生长对象 查找已有的相同局面对象(生长对象 半完成对象, bool 考虑位置)
		{
			//foreach (生长对象 obj in 一级原始对象)
			//	if (半完成对象.关联总数 == obj.关联总数 && 在已有对象中递归查找相同对象(半完成对象, obj))
			//		return obj;
			foreach (生长对象 obj in 本轮结果集)
				if (半完成对象.关联总数 == obj.关联总数 && 在已有对象中递归查找相同对象(半完成对象, obj, 考虑位置))
					return obj;
			//foreach (生长对象 obj in 被抑制对象)
			//    if (半完成对象.关联总数 == obj.关联总数 && 在已有对象中递归查找相同对象(半完成对象, obj, 考虑位置))
			//        return obj;
			foreach (生长对象 obj in 全部对象)
				if (半完成对象.关联总数 == obj.关联总数 && 在已有对象中递归查找相同对象(半完成对象, obj, 考虑位置))
					return obj;

			return null;
		}

		public bool 已有位置重叠对象(生长对象 左对象, 生长对象 右对象)
		{
			foreach (生长对象 obj in 本轮结果集)
				if (obj.左对象 != null && obj.右对象 != null && 左对象.begindex == obj.左对象.begindex && 左对象.endindex == obj.左对象.endindex && 右对象.begindex == obj.右对象.begindex && 右对象.endindex == obj.右对象.endindex)
					return true;
			foreach (生长对象 obj in 被抑制对象)
				if (obj.左对象 != null && obj.右对象 != null && 左对象.begindex == obj.左对象.begindex && 左对象.endindex == obj.左对象.endindex && 右对象.begindex == obj.右对象.begindex && 右对象.endindex == obj.右对象.endindex)
					return true;
			foreach (生长对象 obj in 全部对象)
				if (obj.左对象 != null && obj.右对象 != null && 左对象.begindex == obj.左对象.begindex && 左对象.endindex == obj.左对象.endindex && 右对象.begindex == obj.右对象.begindex && 右对象.endindex == obj.右对象.endindex)
					return true;
			return false;
		}

		private bool 在已有对象中递归查找相同对象(生长对象 新对象, 生长对象 已有对象, bool 考虑位置)
		{
			if (新对象 == null)
				return true;
			if (在对象中查找相同记录(新对象, 新对象.源模式行ID, 已有对象, 考虑位置) == false)
				return false;
			if (在已有对象中递归查找相同对象(新对象.A端对象, 已有对象, 考虑位置) == false)
				return false;
			if (在已有对象中递归查找相同对象(新对象.B端对象, 已有对象, 考虑位置) == false)
				return false;
			return true;
		}

		public bool 已进行过计算(生长对象 中心对象, 生长对象 参数对象, int 处理方式)
		{
			if (处理方式 == 生长_名词谓语处理)
			{
				foreach (关联对象对 obj in 待生长对象对集合)
					if (obj.中心对象 == 中心对象 && obj.参数对象 == 参数对象 && (obj.处理类型 & 生长_名词谓语处理) > 0)
						return true;
			}
			else if (处理方式 == 生长_正常处理)
			{
				foreach (关联对象对 obj in 待生长对象对集合)
					if (obj.中心对象 == 中心对象 && obj.参数对象 == 参数对象 && (obj.处理类型 & 生长_正常处理) > 0)
						return true;
			}
			else if (处理方式 == 生长_集合处理)
			{
				foreach (关联对象对 obj in 待生长对象对集合)
					if (obj.中心对象 == 中心对象 && obj.参数对象 == 参数对象 && (obj.处理类型 & 生长_集合处理) > 0)
						return true;
			}
			else
			{
				foreach (关联对象对 obj in 待生长对象对集合)
					if (obj.中心对象 == 中心对象 && obj.参数对象 == 参数对象 && (obj.处理类型 & 生长_两组动词并列处理) > 0)
						return true;
			}
			return false;
		}

		private bool 在对象中查找相同记录(生长对象 新对象, Guid 源记录, 生长对象 已有对象, bool 考虑位置)
		{
			if (已有对象 == null)
				return false;
			if (源记录.Equals(已有对象.源模式行ID) && 新对象.that == 已有对象.that && (考虑位置 == false || 新对象.begindex == 已有对象.begindex && 新对象.endindex == 已有对象.endindex))
			{
				if ((新对象.A端对象 == 已有对象.A端对象 || (新对象.A端对象 != null && 已有对象.A端对象 != null && 新对象.A端对象.中心第一根类 == 已有对象.A端对象.中心第一根类))
					&& (新对象.B端对象 == 已有对象.B端对象 || (新对象.B端对象 != null && 已有对象.B端对象 != null && 新对象.B端对象.中心第一根类 == 已有对象.B端对象.中心第一根类)))
					return true;
			}
			if (在对象中查找相同记录(新对象, 源记录, 已有对象.A端对象, 考虑位置) == true)
				return true;
			if (在对象中查找相同记录(新对象, 源记录, 已有对象.B端对象, 考虑位置) == true)
				return true;
			return false;
		}


		public int 查找下一个右边界对象(int 字符右边界, int 当前位置 = -1)
		{
			当前位置++;

			while (当前位置 < 右边界排序对象.Count)
			{
				int 边界 = 右边界排序对象[当前位置].endindex;
				if (边界 < 字符右边界)
					return -1;
				if (边界 == 字符右边界)
					return 当前位置;
				当前位置++;
			}
			return -1;
		}

		public void 生成一个分析结果(模式 row, string 前导空格)
		{
			if (Data.拥有形式Guid.Equals(Data.一级关联类型(row)))
				return;
			string s1;
            if (Data.事件属拥被动Guid.Equals(row.源记录))
            {
                s1 = "(" + Data.语言角色名称(row.语言角色) + ")" + "[事件属拥被动]";
                分析结果串 = 分析结果串 + "\r\n" + 前导空格 + s1;
                return;
            }
			if (Data.ThisGuid.Equals(row.A端))
			{
                模式 r = Data.FindRowByID(row.B端);
                s1 = r.形式;
                r = Data.是二元关联(r, true) ? Data.FindRowByID(r.源记录) : Data.FindRowByID(r.B端);
                s1 = s1 + ":" + r.形式;
                分析结果串 = 分析结果串 + s1;
			}
			else
			{
				模式 r = Data.FindRowByID(row.源记录);
				r = Data.FindRowByID(r.B端);
				s1 = "(" + Data.语言角色名称(row.语言角色) + ")" + r.形式;
				分析结果串 = 分析结果串 + "\r\n" + 前导空格 + s1 + ":";
			}

			//List<模式> dr = Data.get模式编辑表().对象集合.Where(r => r.ParentID == row.ID).OrderBy(r => r.序号).ToList();
            
			foreach (模式 r1 in row.端索引表_Parent.OrderBy(r => r.序号))
				生成一个分析结果(r1, 前导空格 + "　　");

		}

		public void 填补未匹配字符串()
		{
			int[] 字符 = new int[Data.当前句子串.Length];
			foreach (生长对象 obj in 全部对象)
			{
				for (int i = obj.begindex; i < obj.endindex; i++)
					字符[i] = 1;
			}
			int 未匹配串开始位置 = -1;
			for (int i = 0; i <= Data.当前句子串.Length; i++)
			{
				if (i == Data.当前句子串.Length || 字符[i] == 1)
				{
					//空串结束。
					if (未匹配串开始位置 != -1)
					{
						SubString str = new SubString(未匹配串开始位置, i);
						模式 字串row = Data.增加字符串生长素材(str);
						生长对象 串对象 = new 生长对象(字串row, 0);
						加入一个对象到池(串对象);
					}
					未匹配串开始位置 = -1;
					continue;
				}
				if (未匹配串开始位置 == -1)
					未匹配串开始位置 = i;
			}
		}

		public List<生长对象> 取出一个范围内对象并按长度排序(封闭范围 范围)
		{
			List<生长对象> 结果 = new List<生长对象>();

			int i;

			for (int k = 在左边界排序对象中定位(范围.begindex); k < 左边界排序对象.Count; k++)
			{
				生长对象 obj = 左边界排序对象[k];
				if (obj.概率打分 <= 0)
					continue;
				if (obj.begindex >= 范围.endindex)
					break;
				if (obj.endindex > 范围.endindex)
					continue;
				for (i = 0; i < 结果.Count; i++)
					if (obj.总分 > 结果[i].总分)
						break;
				结果.Insert(i, obj);
			}
			return 结果;
		}

		//本身不执行生长，而是在已经生长的模式池（这里边的模式相互可能有冲突）里边，选择满足要求并且不冲突的一组对象作为结果。
		//但这样的结果可能有多组，优选满足阀值的多组出来，并加入到选择结果集中去。
		//假设前边该生长的可能性都完成了，现在就是挑选出成绩最好的多组结果，每一组结果都是完整的、不冲突的。
		//这个后边需要的是要阀值，不需要得到所有的结果了。
		//还需要考虑最多个数！这样，如果个数不够，就降低阀值再来，而个数太多，则剔除掉多余的。
		public void 选择最优结果生成(bool 删除空行)
		{
			分析结果树.Clear();

			任务对象.本句话最优模式 = null;

			任务对象.本句话最优对象 = 得到第一个完成对象();

			List<选择结果> 结果 = 在一个内部范围内选择结果(内部范围);

			for (int i = 0; i < 结果.Count; i++)
			{
				选择结果 语篇 = 结果[i];
				模式 newObj = Data.New派生行(Data.单个结果Guid);

				newObj.ParentID = Data.当前句子Row.ID;

				Data.get模式编辑表().新加对象(newObj);

				int 序号 = 1000000;

				foreach (生长对象引用 句子 in 语篇.选入的结果成员)
				{

					模式 r = 生成一棵树(句子.对象, 删除空行, newObj, 0, Data.当前解析语言);

					if (句子.对象 == 任务对象.本句话最优对象)
						任务对象.本句话最优模式 = r;

					分析结果树.Add(r);
					if (r.序号 < 序号)
						序号 = r.序号;
				}
				newObj.序号 = 序号;
				string str = Data.递归合成语言串(newObj, Data.当前解析语言);
				//Data.重计算打分(newObj);
				newObj.序号 = i;

				if (i == 0)
				{
					//if (完整匹配)
					//{
					分析结果串 = "";
					生成一个分析结果(分析结果树[0], "　　　　");
					str = str + "\r\n　　　　" + 分析结果串;
					Data.输出处理信息("分析", "整句匹配" + 结果.Count.ToString() + "个：" + str);
					//}
					//else
					//    Data.输出处理信息("分析", "不完整匹配" + 排序结果.Count.ToString() + "个：" + str);
				}
			}

			return;
		}

		public List<选择结果> 在一个内部范围内选择结果(封闭范围 范围)
		{
			List<选择结果> 结果 = new List<选择结果>();

			List<生长对象> 按总分排序对象集合 = 取出一个范围内对象并按长度排序(范围);

			//现在用最少个数来处理。以后考虑打分。
			//int 最少个数 = 1;

			int 最小熵值 = 10000;

			选择结果 result = new 选择结果();
			result.选入的结果成员 = new List<生长对象引用>();
			while (true)
			{
				//if (回溯得到一个结果(result, false) == false)
				//{
				//    if (结果.Count > 0)//
				//        break;
				//    回溯得到一个结果(result, true);//一个完全匹配的结果都没有，所以，填补好字符串，完成一个匹配。相邻的字符串要进行合并。
				//    完整匹配 = false;
				//}

				if (回溯得到一个结果(按总分排序对象集合, result, true) == false)
					if (result.未完成字符数 > 0 && 结果.Count > 0)
						break;

				选择结果 newobj = new 选择结果();
				newobj.选入的结果成员 = new List<生长对象引用>(result.选入的结果成员);
				newobj.未完成字符数 = result.未完成字符数;
				newobj.源串位码 = result.源串位码;

				newobj.包含字符串个数 = result.包含字符串个数;
				newobj.合并选入成员打分();
				newobj.完成并选入未匹配字符串();
				结果.Add(newobj);

				if (newobj.熵值() < 最小熵值)
					最小熵值 = newobj.熵值();
			}

			for (int i = 0; i < 结果.Count; i++)//取最大熵值的几个。
			{
				if (结果[i].熵值() > 最小熵值)
				{
					结果.RemoveAt(i);
					i--;
				}
			}

			//List<生长对象> 排序结果 = new List<生长对象>();
			//while (true)
			//{
			//	int k = -1;
			//	float v = -1000;
			//	for (int i = 0; i < 结果.Count; i++)
			//	{
			//		生长对象 o = 结果[i];
			//		if (o == null)
			//			continue;
			//		if (o.总分 > v)
			//		{
			//			v = o.总分;
			//			k = i;
			//		}
			//	}
			//	if (k == -1)
			//		break;
			//	排序结果.Add(结果[k]);
			//	结果[k] = null;
			//}

			return 结果;
		}


		public void 调整序号和参数角色(模式 根, ref Dictionary<Guid, 模式> 字典)
		{
			foreach (var objrow in 字典)
			{
				模式 row = objrow.Value;
				if (row.ParentID.Equals(根.ID))
				{
					调整序号和参数角色(row, ref 字典);
					row.序号 -= 根.序号;
				}
			}
		}

		//返回0示参数不够，还不能推导。
		//返回-1表示推导出不满足。
		//返回1表示满足
		public int 进行模式推导(生长对象 待推导对象)
		{
			return 1;
		}

		//返回1示参数不够，还不能推导。
		//返回0表示推导出不满足。
		//更大值表示满足程度。
		public int 计算二元关联的满足性(生长对象 A端, 生长对象 B端, 模式 基关联)
		{
			bool A端序列化 = Data.能够序列化(A端.中心第一根类.模式行);
			bool B端序列化 = Data.能够序列化(B端.中心第一根类.模式行);


			if (Data.是派生关联(Data.属于Guid, 基关联) > 0)
			{
				//对于【是】来说，A端是动词，B端不能是普通名词
				//暂时先这样，后边要更优化，【真的】【事实】等名词是可以的！
				if (A端序列化)
				{
					if (B端序列化 == false)
						return 0;
				}
				else
				{
					if (B端序列化)
						return 0;
				}

			}


			模式 关联 = 根据两个参数计算二元关联(A端, B端, 基关联, false);

			if (关联 != null)
				return 9;

			if (判断是否绝对不允许的二元关联(A端, B端))
				return 0;
			else
				return 9;
			//return 0;
		}
		//返回0示参数不够，还不能推导。
		//返回-1表示推导出不满足。
		//返回1表示满足

		public int 计算加入新关联后的模式成立度(生长对象 对象对, 模式 新关联, int that端/*, 生长对象 根*/)
		{
			生长对象 根对象 = that端 == 字典_目标限定.B端 ? 对象对.参数对象 : 对象对.中心对象;

			if (Data.是二元关联(根对象.中心第一根类.模式行, false))
			{
				生长对象 A端 = null;
				生长对象 B端 = null;
				if (Data.是派生关联(Data.关联拥有A端Guid, 新关联) > 0)
					A端 = 对象对.B端对象;
				else if (Data.是派生关联(Data.关联拥有B端Guid, 新关联) > 0)
					B端 = 对象对.B端对象;
				else
					return 9;//不是【拥有A】也不是【拥有B】,那么直接返回9。

				//生长对象 最新的上一版本对象 = 对象对.选择未完成树对的一枝查找指定根的最新版本对象(根, that端);
				List<参数> 概念参数表 = 根对象.得到指定根对象的参数表(根对象.中心第一根类);

				foreach (参数 obj in 概念参数表)
				{
					if (obj.已经派生() == false)
						continue;
					if (A端 == null && Data.是派生关联(Data.关联拥有A端Guid, obj.源关联记录) > 0)
					{
						A端 = obj.对端派生对象.B端对象;
						break;
					}
					else if (B端 == null && Data.是派生关联(Data.关联拥有B端Guid, obj.源关联记录) > 0)
					{
						B端 = obj.对端派生对象.B端对象;
						break;
					}
				}

				if (A端 != null && B端 != null)//两个参数都找到了，进行计算。
					return 计算二元关联的满足性(A端, B端, 根对象.中心第一根类.源模式行);
			}

			return 9;
		}
		//根据A端和B端判断是否不允许进行二元关联生长
		public bool 判断是否绝对不允许的二元关联(生长对象 A端, 生长对象 B端)
		{
			模式 参数 = A端.中心第一根类.模式行;
			模式 参数A = A端.中心第一根类.模式行;
			模式 参数B = B端.中心第一根类.模式行;
			for (int i = 0; i < 2; i++)
			{
				if (Data.是派生类(Data.量Guid, 参数, 替代.正向替代))
				{
					if (Data.是派生类(Data.次数量Guid, 参数, 替代.正向替代))
						return true;
					else if (Data.是派生类(Data.时间量Guid, 参数, 替代.正向替代))
						return true;
				}
				else if (Data.是派生类(Data.量词个Guid, 参数, 替代.正向替代))
					return true;
				else if (Data.是派生类(Data.符合程度Guid, 参数, 替代.正向替代))
					return true;
				else if (Data.是派生类(Data.动作副词Guid, 参数, 替代.正向替代))
					return true;
				if (Data.是派生类(Data.组织Guid, 参数A, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 参数B, 替代.正向替代))
					return true;
				参数 = B端.中心第一根类.模式行;
				参数A = B端.中心第一根类.模式行;
				参数B = A端.中心第一根类.模式行;
			}
			return false;
		}
		public 模式 尝试进行模式匹配(生长对象 目标节点, bool 正向派生 = true)
		{
			模式匹配结果集合.Clear();
			if (目标节点.中心第一根类 != 目标节点)
			{
				//1.得到中心根类的派生树
				参数树结构 派生树 = Data.利用缓存得到派生树(目标节点.中心第一根类.源模式行, false, false);
				//if (派生树 != null && 派生树.子节点.Count> 0)
				//{
				//    2.遍历派生树中的对象,将其与目标节点进行参数匹配(参数与中心的关联类型)
				//    foreach (参数树结构 obj in 派生树.子节点)
				//    {
				//        if (匹配模式的各参数关联(目标节点, obj.目标,正向派生))
				//            return obj.目标;
				//    }
				//}

				return 递归寻找派生对象进行模式匹配(派生树, 目标节点, 正向派生);
			}
			return null;
		}


		//这样的算法效率显然不高，以后，要考虑把一些模式归纳在一起，甚至是专门的结构化表的方法来存储。采取专门的等价的算法。
		public 模式 递归寻找派生对象进行模式匹配(参数树结构 派生树, 生长对象 目标节点, bool 正向派生 = true)
		{
			模式 匹配对象 = null;
			if (派生树 != null && 派生树.子节点 != null && 派生树.子节点.Count > 0)
			{
				//遍历派生树中的对象,将其与目标节点进行参数匹配(参数与中心的关联类型)
				foreach (参数树结构 obj in 派生树.子节点)
				{
					全部遍历结构.Clear();
					if (匹配模式的各参数关联(目标节点, obj.目标, 正向派生) && Data.是子记录(obj.目标, Data.当前句子Row.ID, true) == false)
					{
						//全部遍历结构.Add(new 生长对象遍历节点(目标节点, obj.目标, obj.目标.ID));
						模式匹配结果节点 匹配结果 = new 模式匹配结果节点(obj.目标);
						匹配结果.匹配对象集合.AddRange(全部遍历结构);
						模式匹配结果集合.Add(匹配结果);
						匹配对象 = obj.目标;
					}
					else if (obj.子节点 != null && obj.子节点.Count > 0)
					{
						匹配对象 = 递归寻找派生对象进行模式匹配(obj, 目标节点, 正向派生);
						if (匹配对象 != null)
						{
							全部遍历结构.Add(new 生长对象遍历节点(目标节点, obj.目标, obj.目标.ID));
						}
					}
				}
			}
			return 匹配对象;
		}
		public bool 匹配模式的各参数(生长对象 参数节点, 模式 目标模式, out 模式 匹配到的参数, bool 正向派生 = true)
		{
			Guid 基关联ID;
			Guid 派生关联ID;
			bool 参数匹配成功 = false;
			匹配到的参数 = null;
			do
			{
				//1.遍历目标模式的子模式：即已结合的所有参数模式对象
				foreach (模式 参数 in 目标模式.端索引表_Parent.OrderBy(r => -r.序号))
				{
					if (Data.是介词或者串(Data.FindRowByID(参数.源记录), true, true, true))
						continue;
					else if (Data.是拥有形式(Data.FindRowByID(参数.源记录)))
						continue;
					if (正向派生)
					{ 基关联ID = 参数节点.源模式行.ID; 派生关联ID = 参数.源记录; }
					else
					{ 派生关联ID = 参数节点.源模式行.ID; 基关联ID = 参数.源记录; }

					if (参数节点.中心第一根类 == 参数节点) //参数节点本身已经是一级对象
					{
						//只需检查参数是否与模式对象的中心有派生关系
						if (Data.是派生类(Data.疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID), 替代.正向替代))
						{
							if (参数.形式 == null || 参数.形式.Length <= 0) //隐藏对象不做为匹配结果
								continue;
							匹配到的参数 = 参数;
							//全部遍历结构.Add(new 生长对象遍历节点(参数节点, 参数, 参数节点.源模式行.ID));
							return true;
						}
						else
							break;
					}
					else if (参数节点.参数对象.是隐藏对象()) //参数节点是聚合等关联
					{
						return 匹配模式的各参数(参数节点.中心对象, 目标模式, out 匹配到的参数, 正向派生);
						//return 匹配模式的各参数关联(参数节点, 参数);
					}
					else
					{
						if (正向派生)
						{ 基关联ID = 参数节点.中心第一根类.源模式行.ID; 派生关联ID = 参数.源记录; }
						else
						{ 派生关联ID = 参数节点.中心第一根类.源模式行.ID; 基关联ID = 参数.源记录; }
						if (Data.是派生类(Data.疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID), 替代.正向替代))
						{
							return 匹配模式的各参数关联(参数节点, 参数, 正向派生, true);
						}
						else
							return false;
					}
					//2.如果是派生关联，则继续匹配参数内部
					//else if (Data.是派生关联(基关联ID, Data.FindRowByID(派生关联ID)) > 0)
					//{
					//    //全部遍历结构.Add(new 生长对象遍历节点(参数节点, 参数));
					//    if (匹配模式的各参数(参数节点.参数对象, 参数))
					//        参数匹配成功 = true;
					//    break;
					//}
				}
			} while (参数匹配成功 == true);

			return false;
		}
		//将目标模式与目标生长对象，按已结合的参数进行逐个匹配
		//正向派生:查询匹配时一般为正向，模板匹配时一般为反向
		public bool 匹配模式的各参数关联(生长对象 目标节点, 模式 目标模式, bool 正向派生 = true, bool 允许忽略节点关联 = false)
		{
			//全部遍历结构.Clear();
			Guid 基关联ID;
			Guid 派生关联ID;
			bool 参数匹配成功 = false;
			do
			{
				参数匹配成功 = false;
				//1.遍历目标模式的子模式：即已结合的所有参数模式对象
				foreach (模式 参数 in 目标模式.端索引表_Parent.OrderBy(r => -r.序号))
				{
					if (Data.是介词或者串(Data.FindRowByID(参数.源记录), true, true, true))
						continue;
					if (正向派生)
					{ 基关联ID = 目标节点.源模式行.ID; 派生关联ID = 参数.源记录; }
					else
					{ 派生关联ID = 目标节点.源模式行.ID; 基关联ID = 参数.源记录; }
					if (目标节点.中心第一根类 == 目标节点) //目标节点本身已经是一级对象
					{
						//只需检查目标节点是否与模式对象的中心有派生关系
						if (Data.是派生类(Data.疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID), 替代.正向替代))
						{

							全部遍历结构.Add(new 生长对象遍历节点(目标节点, 参数, 目标节点.中心对象.源模式行.ID));
							参数匹配成功 = true;
							break;
						}
						else
							break;
					}
					else if (目标节点.参数对象.是隐藏对象()) //参数节点是聚合等关联
					{
						if (匹配模式的各参数关联(目标节点.中心对象, 目标模式, 正向派生))
						{
							目标节点 = 目标节点.中心对象;
							全部遍历结构.Add(new 生长对象遍历节点(目标节点.中心对象, 目标模式, 目标节点.源模式行.ID));

							参数匹配成功 = true;
							break;
						}
					}
					//2.如果是派生关联，则继续匹配参数内部
					if (Data.是派生关联(Data.疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID)) > 0)
					{
						模式 匹配到的参数 = null;
						if (匹配模式的各参数(目标节点.参数对象, 参数, out 匹配到的参数, 正向派生))
						{
							if (匹配到的参数 != null)
								全部遍历结构.Add(new 生长对象遍历节点(目标节点.参数对象, 匹配到的参数, Data.FindRowByID(匹配到的参数.ParentID).源记录));
							else
								全部遍历结构.Add(new 生长对象遍历节点(目标节点.参数对象, 参数, 目标节点.源模式行.ID));
							参数匹配成功 = true;
							break;
						}
					}
				}
				if (正向派生 == false && 允许忽略节点关联)
					参数匹配成功 = true;
				//匹配下一个关联参数
				目标节点 = 目标节点.中心对象;
				if (目标节点.中心第一根类 == 目标节点)
					break;
			} while (参数匹配成功 == true);
			//1.遍历目标模式的子模式：即已结合的所有参数模式对象
			//foreach (模式 参数 in 目标模式.端索引表_Parent)
			//{
			//    if (Data.是介词或者串(Data.FindRowByID(参数.源记录), true, true, true))
			//        continue;
			//    if (正向派生)
			//    { 基关联ID = 目标节点.源模式行.ID; 派生关联ID = 参数.源记录; }
			//    else
			//    { 派生关联ID = 目标节点.源模式行.ID; 基关联ID = 参数.源记录; }
			//2.判断当前参数的关联类型是否与目标节点存在派生关系
			//但，如果目标节点是一级对象时，就应该直接检查对象的派生关系，而不是检查关联类型的派生关系
			//if (目标节点.中心第一根类 != 目标节点 && 参数.A端.Equals(Data.ThisGuid)==false)
			//{
			//if (Data.是派生关联(基关联ID, Data.FindRowByID(派生关联ID)) > 0)
			//{
			//    全部遍历结构.Add(new 生长对象遍历节点(目标节点, 参数));
			//    //生长对象遍历节点 结果=递归匹配模式的各参数关联(目标节点.参数对象, 参数, 正向派生);
			//    匹配模式的各参数关联(目标节点.中心对象, 目标模式, 正向派生);
			//}
			//}
			//else
			//{
			//    if (目标节点.中心第一根类 == 目标节点)
			//    {
			//        if (正向派生)
			//        { 基关联ID = 目标节点.中心第一根类.源模式行.ID; 派生关联ID = 参数.源记录; }
			//        else
			//        { 派生关联ID = 目标节点.中心第一根类.源模式行.ID; 基关联ID = 参数.源记录; }
			//        if (Data.是派生类(疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID), 替代.正向替代))
			//            return new 生长对象遍历节点(目标节点, 参数);
			//        else
			//            return 递归匹配模式的各参数关联(目标节点, 参数, 正向派生);
			//    }
			//    else if (参数.A端.Equals(Data.ThisGuid))
			//    {
			//        if (正向派生)
			//        { 基关联ID = 目标节点.中心第一根类.源模式行.ID; 派生关联ID = 参数.源记录; }
			//        else
			//        { 派生关联ID = 目标节点.中心第一根类.源模式行.ID; 基关联ID = 参数.源记录; }
			//        if (Data.是派生类(疑问模式行进行替代(基关联ID), Data.FindRowByID(派生关联ID), 替代.正向替代))
			//            return new 生长对象遍历节点(目标节点, 参数);
			//    }else
			//        return 递归匹配模式的各参数关联(目标节点, 参数, 正向派生);

			//}
			return 参数匹配成功;
		}

		public void 对查询匹配派生答案(生长对象 问题obj)
		{
			模式 匹配模式 = 尝试进行模式匹配(问题obj);
			if (模式匹配结果集合.Count <= 0)
				return;
			string 旧形式 = Data.当前句子串;
			foreach (模式匹配结果节点 模式匹配结果 in 模式匹配结果集合)
			{
				string 答案 = "";
				//用匹配结果替换问题节点
				if (Data.当前句子串 != 旧形式)
				{
					Data.当前句子串 = 旧形式;
					源语言文字 = Data.当前句子串.ToCharArray();
				}
				生长对象 问题结果 = 问题obj;
				List<生长对象> 疑问替换集合 = new List<生长对象>();
				foreach (生长对象遍历节点 节点 in 模式匹配结果.匹配对象集合.OrderBy(r => -r.模板对象.begindex))
				{
					if (Data.是派生类(Data.疑问算子Guid, 节点.模板对象.中心第一根类.源模式行, 替代.正向替代) == true
						|| Data.是疑问替代对象(节点.模板对象.中心第一根类.源模式行.ID))
					//|| 节点.模板对象.查找已结合的某个类型对象(Data.概念属拥疑问性Guid) != null
					//|| 节点.模板对象.查找已结合的某个类型对象(Data.疑问算子Guid)!=null        
					{
						if (节点.匹配出的答案对象 != null && 节点.匹配出的答案对象.形式 != null)
						{
							答案 = 节点.匹配出的答案对象.形式;
							//替换当前问题串
							string s_left = "";
							string s_right = "";
							if (节点.模板对象.begindex > 0)
								s_left = Data.当前句子串.Substring(0, 节点.模板对象.begindex);
							if (节点.模板对象.endindex < Data.当前句子串.Length)
								s_right = Data.当前句子串.Substring(节点.模板对象.endindex);
							Data.当前句子串 = s_left + 答案 + s_right;
							源语言文字 = Data.当前句子串.ToCharArray();

							模式 答案对象 = 节点.匹配出的答案对象;
							//由于聚合的影响，对匹配出的答案对象进行选定
							//模式 答案对象 = 节点.匹配出的答案对象.端索引表_Parent[0];
							//if ((Data.是介词或者串(答案对象, true, true, false) || Data.是派生类(Data.关联拥有前置介词Guid, 答案对象, 替代.正向替代))
							//    && 节点.匹配出的答案对象.端索引表_Parent.Count > 1)
							//    答案对象 = 节点.匹配出的答案对象.端索引表_Parent[1];

							//将匹配的答案对象(模式)，生成为生长对象
							生长对象 o = 根据模式树生成生长对象(节点.模板对象.begindex, 答案对象, 处理轮数, false);
							List<对象对> 替换对象集合 = new List<对象对>();
							替换对象集合.Add(new 对象对(节点.模板对象, o));

							//对问题节点，进行替换生长
							if (Data.是二元关联(节点.模板对象.Parent.源模式行, false))
								节点.模板对象.Parent.源模式行 = Data.FindRowByID(节点.关联ID);
							问题结果 = 问题结果.替换基概念为派生概念重建对象(替换对象集合);
							疑问替换集合.Insert(0, o);
						}
					}
				}
				if (疑问替换集合.Count > 0)
				{
					if (疑问替换集合.Count > 2) //多个疑问词无法简略回答
						全部匹配结果.Add(问题结果);
					else
					{
						if (Data.简略回答)
						{
							全部匹配结果.Add(疑问替换集合[0]);
						}
						else
							全部匹配结果.Add(问题结果);
					}
				}
			}
			全部遍历结构.Clear();
			return;

			//以下是旧的查询算法，虽未使用仍保留代码
			//这个算法需要改进，目前方法并不对。目前是：
			//1、【概念节点】处理，先把问题拆解成鼓励的【概念对象】，然后分别查询出所有这些概念对象的【派生对象】。
			//2、【关联】处理，办这些概念对象在用符合问题要求的关联连接起来。

			List<生长对象> 问题的概念节点 = new List<生长对象>();

			//一、这里得到【问题对象】本身的各个概念参数。
			问题obj.为构建查询模板递归得到查找所有根概念节点(问题的概念节点);

			//二、对问题里边的每个概念参数，先从答案库中查询出所有的派生对象。数量可能比较多，不够优化，以后再优化。
			//这些派生对象相互是孤立的，并没有联系起来。
			foreach (生长对象 o in 问题的概念节点)
			{
				生长对象遍历节点 节点 = new 生长对象遍历节点(o);
				节点.派生树 = Data.去除疑问部分利用缓存得到派生树(o.源模式行, false);//这里的对象只需要本质属于的派生。
				节点.派生树.去除给定记录的子孙记录(Data.当前句子Row);//把问题本身的记录删除。
				全部遍历结构.Add(节点);
			}

			if (全部遍历结构.Count() == 0)
				return;

			//排序，进行一个优化，把和其它对象关联数最多的放前边
			//然后把数量最少的节点放最前边
			全部遍历结构.Sort((生长对象遍历节点 v1, 生长对象遍历节点 v2) =>
			{
				if (v1.模板对象.关联个数 == v2.模板对象.关联个数)
				{
					if (v1.派生树.Count() == v2.派生树.Count())
						return 0;
					return v1.派生树.Count() < v2.派生树.Count() ? -1 : 1;
				}
				return v1.模板对象.关联个数 > v2.模板对象.关联个数 ? -1 : 1;
			});

			//三、在概念上挂上这个概念直接的关联，完成查询模板。
			//一个关联涉及两个端，现在的算法是把这个关联挂在前边排序后排到后边的对象上，而不是前边的。原因也想不起来了。
			for (int i = 1; i < 全部遍历结构.Count; i++)
			{
				生长对象遍历节点 o = 全部遍历结构[i];
				问题obj.递归得到查找所有关联节点(o, 全部遍历结构, i);
			}

			当前遍历位置 = 0;
			//四、用查询模板在库中查找各关联，看这些派生对象是否是关联起来的（属于同一个模式）。
			bool 下一个结果 = false;
			while (回溯得到一个匹配派生结果(下一个结果))
			{
				生长对象 o = 问题obj.用当前匹配结果构造一个匹配对象();
				if (Data.简略回答)
					o.加入疑问变量的匹配结果(全部匹配结果);
				else
					全部匹配结果.Add(o);
				下一个结果 = true;
			}
		}


		public List<模式> 考虑各种推导对问题查询出答案(模式 问题模式树, 模式 parentrow)
		{

			List<模式> 结果 = new List<模式>();

			模式树结构 tree = 模式树结构.从一个根模式生成模式树结构(问题模式树);

			tree.准备前置推导模式();

			tree.根据问题本身及前置推导匹配出答案(结果, parentrow);

			return 结果;

		}



		public void 递归清除匹配模式标记(生长对象 匹配模式)
		{
			匹配模式.模式匹配对应的匹配模式行 = null;
			if (匹配模式.中心对象 != null)
				递归清除匹配模式标记(匹配模式.中心对象);
			if (匹配模式.参数对象 != null)
				递归清除匹配模式标记(匹配模式.参数对象);
		}

		public void 递归匹配模式(生长对象 匹配模式, 模式 匹配知识)
		{
			匹配模式.模式匹配对应的匹配模式行 = null;
			if (匹配模式.中心对象 != null)
				递归清除匹配模式标记(匹配模式.中心对象);
			if (匹配模式.参数对象 != null)
				递归清除匹配模式标记(匹配模式.参数对象);
		}

		//注意，还要要求匹配的模式的条件都要满足。
		public void 匹配一个模式(生长对象 匹配模式, 模式 匹配知识)
		{
			递归清除匹配模式标记(匹配模式);

		}


		//递归进行处理，按照参数行回溯到根，记录到路径里边
		public 参数 遵循路径查找参数对象(生长对象 原对象, List<模式> 路径, int 起始路径位置)
		{
			生长对象 当前路径节点 = 原对象;
			for (; 起始路径位置 < 路径.Count(); 起始路径位置++)
			{
				Data.Assert(当前路径节点 != null);
				模式 当前引用 = 路径[起始路径位置];
				List<参数> 概念参数表 = 原对象.得到指定根对象的参数表(当前路径节点.中心第一根类);
				foreach (参数 参数对象 in 概念参数表)
				{
					if (参数对象.已经派生() == false)
						continue;
					if (Data.是派生关联((Guid)当前引用.源记录, 参数对象.源关联记录) > 0)
					{
						if (起始路径位置 == 路径.Count() - 1)//已经完成。
							return 参数对象;

						当前路径节点 = 字典_目标限定.A端 == (int)当前引用.That根 ? 参数对象.对端派生对象.A端对象 : 参数对象.对端派生对象.B端对象;
						if (当前路径节点.中心对象 != null)//不是根对象。
							当前路径节点 = 字典_目标限定.A端 == (int)当前路径节点.that ? 当前路径节点.中心对象 : 当前路径节点.参数对象;
						//当前路径节点 = 参数对象.对端派生对象.B端对象;
						break;
					}
				}
			}
			return null;
		}


		public 生长对象 遵循路径增加参数对象(生长对象 原对象, List<模式> 路径, int 起始路径位置, 生长对象 新建参数)
		{
			模式 当前引用 = 路径[起始路径位置];

			List<参数> 概念参数表 = 原对象.得到指定根对象的参数表(原对象.中心第一根类);
			foreach (参数 参数对象 in 概念参数表)
			{
				if (参数对象.已经派生() == false)
					continue;
				//已经存在，不用新创建。
				if (Data.是派生关联((Guid)当前引用.源记录, 参数对象.源关联记录) > 0)
				{
					if (起始路径位置 < 路径.Count() - 1)
					{
						if (字典_目标限定.A端 == (int)当前引用.That根)
						{
							参数对象.对端派生对象.A端对象 = 遵循路径增加参数对象(参数对象.对端派生对象.A端对象, 路径, 起始路径位置 + 1, 新建参数);
							//参数对象.对端派生对象.复制概念参数(参数对象.对端派生对象.A端对象);
							//参数对象.对端派生对象.处理对象的关键完成参数表(参数对象.对端派生对象, false);
							参数对象.对端派生对象.完成两端的对象参数表();
						}
						else
							参数对象.对端派生对象.B端对象 = 遵循路径增加参数对象(参数对象.对端派生对象.B端对象, 路径, 起始路径位置 + 1, 新建参数);
					}
					return 原对象;
				}
			}

			//还不存在，因此创建对象。
			//先创建关联。
			模式 源关联 = Data.New派生行((Guid)当前引用.源记录, 字典_目标限定.空);
			if (起始路径位置 < 路径.Count() - 1)//应目标对象的要求创建路径中间的对象。
			{
				Guid id = 字典_目标限定.A端 == (int)当前引用.That根 ? (Guid)源关联.A端 : (Guid)源关联.B端;
				模式 参数row = Data.加入到素材(Data.New派生行(id, 字典_目标限定.空, true));
				参数row.序号 = -1;
				生长对象 参数obj = new 生长对象(参数row, 2);
				参数row.显隐 = 当前引用.显隐;
				新建参数 = 遵循路径增加参数对象(参数obj, 路径, 起始路径位置 + 1, 新建参数);
			}
			return new 生长对象(原对象, 新建参数, 源关联);//创建路径最末端的目标对象。
		}
		//计算出从【源对象】（普通的记录，也就是参照的起始点）到【目标对象】（是引用型记录，是最终要指示的目标）的全部引用记录。
		public List<模式> 取得引用路径(模式 目标对象)
		{
			List<模式> 路径 = new List<模式>();
			Data.Assert(Data.引用Guid.Equals(Data.一级关联类型(目标对象)));//第一个肯定是的。
			while (Data.引用Guid.Equals(Data.一级关联类型(目标对象)))
			{
				路径.Insert(0, 目标对象);
				int that = (int)目标对象.That根;
				Data.Assert(字典_目标限定.A端 == that || 字典_目标限定.B端 == that);
				//如果当前的端是A端，那么源记录就是B端来指示的。
				目标对象 = Data.FindRowByID(Data.取得端的值(目标对象, 字典_目标限定.A端 == that ? 字典_目标限定.B端 : 字典_目标限定.B端));
			}
			return 路径;
		}

		//public 生长对象 生成一个匹配出的派生结果(生长对象 obj, int 序号)
		//{
		//    PatternDataSet.模式结果Row Row = Data.patternDataSet.模式结果.New模式结果Row();
		//    Row.ID = Guid.NewGuid();
		//    Row.字符串 = "匹配结果" + 序号;
		//    Row.ObjectID = Guid.Empty;
		//    Row.序号 = 1000 + 序号;
		//    Row.ParentID = Guid.Empty;
		//    Data.patternDataSet.模式结果.Add模式结果Row(Row);

		//    return obj.用当前匹配结果构造一个匹配对象();
		//}


		public bool 回溯得到一个匹配派生结果(bool 下一个结果)
		{
			//如果上一次成功了，那么，不再尝试进入更具体的对象，也就是当前行的信息就好。
			//注意，现在是从最后一个节点开始再尝试下一个，其实...，是否从第一个开始？整个的都不再尝试
			//也就是应该回溯到第一个对象然后从下一个取值开始试？
			参数树结构 当前对象 = null;
			if (下一个结果)//上一次是匹配好的了，再次匹配一个结果。
				当前对象 = 全部遍历结构[当前遍历位置].派生树.得到当前级的下一对象();
			else
				当前对象 = 全部遍历结构[当前遍历位置].派生树.当前遍历对象();//第一次开始，遍历位置应该等于0；

			while (true)
			{
				while (当前对象 == null)//得到对象失败，回退
				{
					当前遍历位置 = 当前遍历位置 - 1;
					if (当前遍历位置 < 0)//回退到最后了，结束。
						return false;
					当前对象 = 全部遍历结构[当前遍历位置].派生树.先尝试子对象失败再尝试同级下个对象();
				}
				if (计算当前选定对象的满足性(当前对象.目标, 当前遍历位置))//当前对象计算成功
				{
					if (当前遍历位置 == 全部遍历结构.Count() - 1)//完成
						return true;
					else//因为当前处理成功而整体前进一级，这个时候要把进入标记清空。
					{
						全部遍历结构[++当前遍历位置].派生树.递归初始化遍历标记();
						当前对象 = 全部遍历结构[当前遍历位置].派生树.当前遍历对象();
					}
				}
				else//当前对象计算失败。进入更具体的派生再试。
					当前对象 = 全部遍历结构[当前遍历位置].派生树.先尝试子对象失败再尝试同级下个对象();
			}

		}

		private bool 计算当前选定对象的满足性(模式 派生对象, int 当前节点索引)
		{
			生长对象遍历节点 X端节点 = 全部遍历结构[当前节点索引];

			X端节点.匹配出的答案对象 = 派生对象;

			if (X端节点.前边已完成关联对象 == null)
				return true;

			foreach (生长对象遍历节点 o in 全部遍历结构[当前节点索引].前边已完成关联对象)
			{
				Data.Assert(o.另一端的索引 < 当前节点索引);

				生长对象遍历节点 Y端节点 = 全部遍历结构[o.另一端的索引];

				模式 关联 = o.模板对象.源模式行;

				//Data.Assert(X端节点.匹配的派生对象 != Y端节点.匹配的派生对象);
				模式 A端 = o.that == 字典_目标限定.A端 ? X端节点.匹配出的答案对象 : Y端节点.匹配出的答案对象;
				模式 B端 = o.that == 字典_目标限定.A端 ? Y端节点.匹配出的答案对象 : X端节点.匹配出的答案对象;

				Data.Assert(Data.什么Guid.Equals(A端.ID) == false);//"什么"不会作为A端。
				if (Data.什么Guid.Equals(B端.ID))//"什么"疑问的关联不用计算。
				{
					if ((Data.是派生关联(Data.概念属拥疑问性Guid, 关联) > 0))
					{
						o.匹配出的答案对象 = 关联;
						continue;
					}
					else
						return false;//纯的【什么】自身不行，【什么事物】则可以。
				}
				//if (Data.是派生关联(Data.属拥句子Guid, 关联) > 0 && Data.是派生类(Data.疑问句Guid, Data.FindRowByID((Guid)B端["ID"]), 替代.正向替代))//疑问句不用计算。
				//{
				//    //这条代码可能有错，后边生成树都会出现问题。
				//    o.匹配出的答案对象 = 关联;
				//    continue;
				//}

				o.匹配出的答案对象 = Data.查找给定两个对象之间的派生关联(A端, B端, 关联);
				if (o.匹配出的答案对象 == null)//没有匹配成功。
					return false;
			}
			return true;
		}



		public 模式 生成一棵树(生长对象 obj, bool 删除空行, 模式 parentrow, int 重新生成串, int 语言)
		{
			//对于【属拥】的处理：
			//1、一个方面类（比如完成、疑问句）自己生长时是根据【拥有形式】等产生的，这条形式记录记录在这个方面类下边。
			//2、当主方面（比如借）和方面类属拥后，那么，以后再遇到方面类的参数，就直接转移给主方面了。
			//3、这样的情况就是，方面类如果有多个形式参数，比如疑问有【吗】和【？】。括号有【（】和【）】。这些参数有一些挂在方面类下边，有一些直接合并给主方面了。
			//4、这没有什么问题，对于分析，这样的分别挂着也没有任何问题（挂在方面类下的参数和【属拥】记录其实就是是一体的）。生成的时候，则就直接生成在主方面下就好。

			Dictionary<Guid, 模式> 字典 = new Dictionary<Guid, 模式>();

			obj.清空序号和Parent();
			obj.用关联对象递归设置Parent和语言角色(obj);
			obj.递归调整序号();
			obj.递归调整生成树(ref 字典);

			foreach (var objrow in 字典)
			{
				模式 row = objrow.Value;
				if (字典.ContainsKey((Guid)row.A端) && ((Guid)row.ID).Equals(字典[(Guid)row.A端].ID) == false)
					row.A端 = 字典[(Guid)row.A端].ID;
				if (字典.ContainsKey((Guid)row.B端) && ((Guid)row.ID).Equals(字典[(Guid)row.B端].ID) == false)
					row.B端 = 字典[(Guid)row.B端].ID;
				//parent为空，就是最顶级的根，设置为外部给的parent。只应该有一个。
				if (Data.NullParentGuid.Equals(row.ParentID))
					row.ParentID = parentrow.ID;
			}

			//if (删除空行)
			//{
			//	//删除所有的空this行，不过可能也可以不做这个。
			//	while (true)
			//	{
			//		Guid 空行ID = Guid.Empty;
			//		Guid ParentID = Guid.Empty;
			//		模式 row1 = null;
			//		foreach (var objrow in 字典)
			//		{
			//			row1 = objrow.Value;
			//			if (Data.Null事物.Equals(row1.源记录))//是一个空行。
			//			{
			//				空行ID = row1.ID;
			//				ParentID = row1.ParentID;
			//				字典.Remove(objrow.Key);
			//				break;
			//			}
			//		}
			//		if (空行ID.Equals(Guid.Empty))
			//			break;
			//		Guid 替换行ID = Guid.Empty;
			//		foreach (var objrow in 字典)
			//		{
			//			模式 row = objrow.Value;
			//			//if ((row.A端.Equals(空行ID) && row.that根 == (int)字典_目标限定.A端) || (row.B端.Equals(空行ID) && row.that根 == (int)字典_目标限定.B端))
			//			if (row.ParentID.Equals(空行ID))
			//			{//应该只有一个，而且应该是其子对象。
			//				row.ParentID = ParentID;
			//				替换行ID = row.ID;
			//				break;
			//			}
			//		}
			//		if (Guid.Empty.Equals(替换行ID))
			//			break;

			//		foreach (var objrow in 字典)
			//		{
			//			模式 row = objrow.Value;
			//			if (row.A端.Equals(空行ID))
			//				row.A端 = row.ID.Equals(替换行ID) ? Data.ThisGuid : 替换行ID;
			//			if (row.B端.Equals(空行ID))
			//				row.B端 = row.ID.Equals(替换行ID) ? Data.ThisGuid : 替换行ID;
			//		}

			//		Data.FindTableByID(row1.ID).删除对象(row1);
			//	}
			//}

			foreach (模式 row in 字典.Values)
			{
				//查找出最后的根记录, 根记录有且只有一个。因为生成算法是乱序的，所以并没有直接返回这个根，要用查找方法来找出来。
				if (字典.Values.Any(r => r.ID.Equals(row.ParentID)))
					continue;

				//因为调整生成树变了，所以不需要这个方法了。
				//记录树 tree = 记录树.形成一个记录树(row);
				//tree.调整所有聚合属拥参数到顶层根(tree, false);
				if (重新生成串 == 2)
				{
					Data.第一阶段展开设置角色及默认次序(row);
					Data.第二阶段根据语序微调角色(row);
					Data.第三阶段构造表现形式(row, false);
					Data.第四阶段最终生成形式串(row, 语言);
				}
				else if (重新生成串 == 3)
				{
					//调整序号和参数角色(row, ref 字典);
					Data.递归调序(row);
					Data.递归_传递语言角色(row);
					Data.第三阶段构造表现形式(row, true);
					//暂时先这样，如果查询结果记录和原始知识记录是派生关系而不是另起炉灶，那么就要传递true，保证配置形式串可以成功，否则，中间间接了一层，不能成功。
					//但这种方法很不严谨，以后再进行调整吧。需要设计查询结果和查询知识记录是【等价】甚至【引用】时，查找形式等的传递处理。
					//Data.第三阶段构造表现形式(row, false);
					Data.第四阶段最终生成形式串(row, 语言);
				}
				else if (重新生成串 == 1)
				{
					//调整序号和参数角色(row, ref 字典);
					Data.递归_传递语言角色(row);
					Data.第三阶段构造表现形式(row, false);
					Data.第四阶段最终生成形式串(row, 语言);
				}
				else if (重新生成串 == 0)
				{
					//调整序号和参数角色(row, ref 字典);
					Data.递归_传递语言角色(row);
					Data.第四阶段最终生成形式串(row, 语言);
				}

				return row;
			}
			return null;
		}

		public 模式 查看生成树(Guid id, bool 删除空行)
		{
			foreach (生长对象 obj in 全部对象)
				if (obj.结果Row.ID == id)
					return 生成一棵树(obj, 删除空行, Data.当前句子Row, 0, Data.当前解析语言);
			foreach (生长对象 obj in 全部匹配结果)
				if (obj.结果Row.ID == id)
					return 生成一棵树(obj, 删除空行, Data.当前句子Row, 1, Data.当前解析语言);
			return null;
		}

		public void 进行派生匹配(Guid id)
		{
			foreach (生长对象 obj in 全部对象)
				if (obj.结果Row.ID == id)
				{
					对查询匹配派生答案(obj);
					return;
				}
		}

		/// <summary>
		/// 预处理
		/// 预处理以后，将符合以下规则：
		/// 对于英文
		/// 1、所有的空格、标点符号等都应是正规的小写字符
		/// 2、所有的字符都基本是基本ascii的字符
		/// 3、连续的空格字符合并为一个空格
		/// 4、tab和enter将保留，他们都将起到一定的内容分段作用
		/// 对于汉字
		/// 1、各种标点符号都保留，半角和全角都可以用的全部转换为全角，只能是半角的，比如"."，还是保留半角。
		/// 2、去除所有空格字符，但保留tab和enter字符。
		/// </summary>
		/// <param name="sourcestring"></param>
		/// <returns></returns>
		public bool PreProcess()
		{
			return true;
		}

		public void Output()
		{
			Data.输出串("全部串=" + allresults.Count);
			for (int i = 0; i < allresults.Count; i++)
				Data.输出串(allresults[i]);
			Data.输出串("匹配串=" + matchresults.Count);
			for (int i = 0; i < matchresults.Count; i++)
				Data.输出串(matchresults[i].ToString());
		}

		// 进行字符串基本全匹配处理，匹配后的结果在TargetStruct的matchresults中。
		public void 字符串匹配处理()
		{
			allresults.Clear();
			matchresults.Clear();

			MyString str = new MyString(源语言文字);
			str.endindex = 源语言文字.Length;
			///设置要处理的字符为全部。
			//string ss="一苹";
			//byte[] tmp = System.Text.UnicodeEncoding.Default.GetBytes(ss);
			//char a = ss[0];
			//char b = ss[1];

			if (str.Length == 0)
				return;

			//Data.语料库.Sort();

			SubString sp = new SubString();
			while (sp.begindex < str.Length)
			{
				MoveToNextWordHead(str, ref sp);

				FindStringFrom语料库(str, ref sp);
			}
		}


		///对于取得的可能语义进行排序
		///这个排序主要考虑各个语义解释本身的优先度和当前语境，以及相互之间的制约关系
		public void SortMeaning()
		{
		}

		/// <summary>
		/// 生成语言
		/// 在这个时候，完成模式已经是被完成了，没有冲突和互斥的模式。
		/// </summary>
		public string 生成()
		{
			return "";
		}

		public void 从串中匹配出多个模式()
		{
		}

		public void 特殊特征模式查找处理()
		{
		}

		public virtual int This语言
		{
			get
			{
				return 字典_语言.公语;
			}
		}

		public 形式化语料串 在语料库中查找串(string str)
		{
			if (str == "")
				return null;
			foreach (形式化语料串 row in Data.语料库)
				if (str == row.字符串)
					return row;
			return null;
		}


		//已经排了序，用二分查找算法。
		public void FindStringFrom语料库(MyString s, ref SubString pos)
		{
			int f = 0;
			Data.Assert(Data.语料库[0].字符串.Length > 0); //语料库不能有空字符串，否则，第一个就会出错。

			int e = Data.语料库.Count - 1;
			int index = 0;//依次匹配

			while (f <= e)
			{
				if (index >= s.Length - pos.begindex)
					break;

				char sd = s[pos.begindex + index];
				//找到字符开始语料
				f = Data.GetFisrtChar(f, e, sd, index);
				if (f == -1)
					break;
				//找到字符的结束语料。
				e = Data.GetLastChar(f, e, sd, index);
				if (e == -1)
					break;

				if (Data.语料库[f].字符串.Length == index + 1 && index + 1 >= pos.长度/*要大于整体的最小长度*/)//长度也刚好，找到一个。
				{
					matchresults.Add(new 匹配语料对象(pos.begindex + s.begindex, pos.begindex + s.begindex + index + 1, Data.语料库[f]));
					f++;//可以移到下一个语料了
				}
				index++;//下一字符
			}
		}


		public void FindStringFrom语料库(MyString s, ref SubString pos, List<形式化语料串> 字符语料库)
		{
			string 需匹配串 = pos.ToString();
			List<形式化语料串> 所有可能匹配语料集合 = 字符语料库.Where(r => r.字符串.StartsWith(需匹配串)).ToList();

			foreach (形式化语料串 item in 所有可能匹配语料集合)
			{
				if (item.字符串.Length + pos.begindex > s.endindex)
					continue;

				if (s.Substring(pos.begindex, item.字符串.Length) == item.字符串)
					matchresults.Add(new 匹配语料对象(pos.begindex, pos.begindex + item.字符串.Length, item));
			}
		}

		// 这个方法的作用是，让后边的单词移动一个最小的单词(英语就是单词，汉语就是一个字)的位置。

		//public void MoveNextWord(MyString str, ref SubString pos)
		//{
		//    int i;
		//    for (i = pos.endindex; i < str.Length; i++)
		//    {
		//        int n = 串类型.计算字符类型(str[i]);
		//        //if (n==串类型.英语单词)
		//        //	continue;
		//        if (n == 串类型.汉字)//现在暂时假设每一个汉字是一个词
		//        {
		//            i++;
		//            break;
		//        }
		//        else
		//        {
		//            if (i == pos.endindex)//如果第一个就不是字母，那么也向前走一步
		//            {
		//                i++;
		//                break;
		//            }
		//        }
		//    }
		//    pos.endindex = i;
		//}

		//这个方法主要为英语，一次移动一个单词，
		//但现在算法还不严谨，需要把数字等都考虑进去，要一个更好的算法。

		public virtual void MoveToNextWordHead(MyString str, ref SubString pos)
		{
			int 类型 = 0;
			int i = pos.begindex = pos.endindex;
			while (i < str.Length)
			{
				int n = 串类型.计算字符类型(str[i]);
				if (i == pos.endindex)//第一个字符，记下类型。
					类型 = n;
				i++;
				if (n != 类型 || n < 串类型.连续)//不是连续的单词或者数字
				{
					if (i - pos.endindex >= 2)
						i--;
					break;
				}
			}
			pos.endindex = i;
		}

		public int 按语言串和基本语义初步划出一个句子(ref SubString ss)
		{
			//现在暂时不划分，就是所有的内容都当成一个句子。
			ss.begindex = 0;
			ss.endindex = 源语言文字.Length;
			return ss.endindex;
		}

		/// <summary>
		/// 理解语言
		/// 理解过程结束后，commonlanagestruct中存放理解分析后的结构化语句
		/// infomationstruct中间存放产生和推衍的中间信息
		/// outstring中存放对sourcestring进行纠错后的串。
		/// 另外处理过程中的主要参数是ContextEntironment。
		/// </summary>
		/// <param name="sourcestring"></param>
		public void 理解整篇文章(模式 row)
		{

			//对文章进行预先处理。
			//对文章进行字符串匹配，匹配出所有的语言串模式，加入到语言串的库里边去。这些匹配是可以重叠的。

			字符串匹配处理();//这个处理完成后，在TaggetStruct中存放了所有匹配上的字符串或者说是字符串基本模式。

			//这些地方可能要对整篇文章里边一些出现的公共的字符串进行高级的处理。

			SubString 字符串范围限定 = new SubString(0, 0);

			while (true)
			{

				//根据字符模式的特点以及标点符号等划分出一个句子的字符串范围限定。
				int len = 按语言串和基本语义初步划出一个句子(ref 字符串范围限定);
				if (len == 0)
					break;

				//为了效率，基本字符串是对整篇文章进行一起处理。而从原始语义模式开始，就是对每一个句子为单位进行处理。
				//执行分解串并生成一级对象(row);

				object 目标模式限定 = new object();

				///这里开始进行模式竞争生长，也就是在分析对象池之外启动各个分析任务，进行较复杂的模式生长处理
				模式自下而上迭代生长(ref 字符串范围限定, ref 目标模式限定);

				生长对象 结果 = 优选完成一个句子(ref 字符串范围限定, ref 目标模式限定);
				//调用方法来完成这个句子。

				break;
			}
		}

		void 模式自下而上迭代生长(ref SubString 字符串范围限定, ref object 目标模式限定)
		{
			//初级形式语义处理
			//在分析对象池中进行一些比较确信的语义进行初步处理，这些处理直接保留在分析对象池中，在进入分析任务之前完成
			//包括：多参数决定的模式(是红色的＝"是"红色"的")、派生模式(如满足模式和等价模式)等情况的处理
			//这些处理形成一些二级的对象模式加入在分析对象池中，供后边分析任务使用，它们会优先使用这里长成的大粒度对象
			//这些处理是大致可以确定的，但也不保证最后一定就是正确的，后续分析还是可能废弃并回退回原始对象。

			//开始做这里，根据TargetStruct中的匹配上的字符串进行能对应的语义片段的筛选。

			//也许不需要了？
			//生长对象 任务 = new 生长对象("");

			//自底向上生长
			//主要就是相邻的、吸附生长

			for (int 层级 = 0; 层级 < 10; 层级++)//进行很多次循环迭代，每一次循环，选择上一次的结果的对象进行一级的生长。
			{

				List<生长对象> 结果集 = new List<生长对象>();
				foreach (生长对象 obj in 左边界排序对象)//从左边开始逐个进行生长
				{
					//下边生长一级模式
					//以目标模式为根，向两边相邻进行最简单的生长，不嵌套，就是找跟前相邻的参数（可以是原子的，也可以是聚合的，但必须是已经长成的）如果生长失败，就返回，那么，就需要尝试别的生长！	
					//有多个参数，可以进行模式合并生长
					//object[] 参数对象 = new object[语言参数个数];//创建参数数组，用于存储参数
				}
			}
		}

		//这个是自顶向下生长方法了
		public bool 生成一个单变量方程式(ref object 被限定目标, SubString 字符串范围限定, object 目标模式限定)
		{
			return true;
		}

		public void 打分排序()
		{
			//模式直接匹配的比模式的所属类等间接特征匹配（比如属于的类、词类相同）的优先。
			//左边的（比右边的）优先，这样，可以保证左边的能被完成。
			//组合模式比起构成它们的单个模式优先，这就是说已经合并的有效。
		}

		public 生长对象 优选完成一个句子(ref SubString 字符串范围限定, ref object 目标模式限定)
		{
			///这个方法完成一个句子，

			//根据本句子的范围进行局部的模式准备，包括提取出来本句子范围的所有模式（可能有重叠）
			//查找出完全冲突的歧义的模式进行列表，比如“他把网球拍卖了”里边的歧义就要列出两个列表，这两个内容是绝对互斥的。
			//以及其他一些需要考虑的多种可能性的列表

			List<生长对象> results = new List<生长对象>();
			//根据上述列表进行循环
			{
				//按照当前的选择项对已有实例库进行重新整理，以体现出突出当前假设的模式和隐藏暂时隐藏模式的目的。
				生长对象 结果 = 生长一个合法的句子模式(ref 字符串范围限定, ref 目标模式限定/*, 已有对象库*/);
				results.Add(结果);

			}

			{
				//对这多个结果进行比较，优选最好的模式。
			}

			return results[0];

		}

		public 生长对象 生长一个合法的句子模式(ref SubString 字符串范围限定, ref object 目标模式限定)
		{
			//这个方法基本都会返回一个结果，只在于这个结果的好和坏而已，基本也保证一定是一个包装了字符串范围尤其是左边范围的一个模式。这总是能做到一个结果，结果质量不好也只是说中间的松散并列模式较多而已。
			//完整模式的要求：1、本身是完整的；2、是连续的；3、最左边的部分将被包容进来，目标模式将是最左边的一个模式！。
			//这个生长中，将从右边的模式特征中进行选取，然后让选择的模式进行重组。在这个时候右边的模式的生长不讲究左和右的概念。
			//当前处理的时候，可以使用的素材就是已有实例库，对于已有实例库按照字符串范围限定和目标模式限定两者进行限定可以筛选出真正可以用的实例内容

			//模式实例 结果=已有实例库最左边的一个;
			//初始化，如果别的都失败了，就可以直接返回这一个。
			//接下来的工作其实是找一个比这个的覆盖范围更大的模式来取代这个默认模式而已。

			//将这些模式片段作为模式参数，查询出包含他们作为关键参数的多个模式;
			//做高级的处理，即从已有实例库提取一些高级特征来查询出更多模式;
			//将上述找出的模式合并并加入已有实例库;
			//对定语、状语这样的附属模式连同配套模式等进行一个初始处理，包括进行无关联并列模式的处理，这相当于在开始的时候进行的自底向上的一个处理，合并出多个一级模式供右边的东西来完成。

			//对模式库中满足目标限定和字符串范围限定的模式进行排序，然后选择出一个有限的候选线索集M。
			//M的数目不一定要很多，只包括最有可能生长的东西（不生长的东西比如一个固定名词就不包含在内了），而且递归的浅层级别时候选多点，到深度时就几乎都只有一个可能了。

			生长对象 max = null;

			while (true)
			{
				生长对象 m = new 生长对象();///应该进行一个初始化。

				//这里需要把m这个模式实例进行一个初始化，尽量准备好

				bool b = 完成模式(m, 字符串范围限定, 目标模式限定/*, 已有对象库*/);

				if (b && (max == null || m.质量好于(max)))//记录成绩最好的一个。
					max = m;
				//if(max!=null && (max.质量>=终极阀值 || m等于最后一个))
				//{
				//	//以下处理是模式完成并且评估质量很好，终极阀值是比较高的阀值，就是说满足这个值，就可以不用考虑别的可能性了。
				//	if(模式已经最大化，即包括了尤其是左边部分在内的全部的字符串范围)
				//	{
				//		return max;
				//	}
				//	else//模式还没有最大化，把它加入成果库，然后再递归尝试更高的一个。
				//	{
				//		已有对象库.Add(max);
				//		模式实例 next=生长一个合法的句子模式(字符串范围限定,目标模式限定,已有对象库);;//递归调用，完成更大的模式。

				//		if(next != null)
				//			return next;//下一级生长成功，返回
				//		已有对象库.Remove(max);//下一级生长失败，本级需要进行回溯
				//	}				
				//}

				//if(m等于最后一个)
				//	return null;//最后一个了，所以失败

				//m=下一优先级的模式;//再次尝试
			}

		}

		public bool 完成模式(object 目标模式, SubString 字符串范围限定, object 目标模式限定)
		{
			/// 这个方法以目标模式为根(不允许被替换)，向两边进行尽可能的扩展，最大可能地生长出自己的子模式，子模式也可能扩展来包容更多的内容！
			///这个方法的主要任务就是完成本身这个目标模式，合成自己的参数，并不考虑完成的模式是不是包含了最左边的。
			///因此，本方法本身是递归的。
			///如果本模式和其他已经完成的模式冲突，尤其是和已经存在的左边的模式冲突，那么本模式必须对其进行重组以全部包容它们，如果无法做到那么就失败返回，
			///也就是说，本模式不能完全去容纳别的模式，那么，就失败了回退，等别的模式来包容自己吧。
			///这里最难的就是多参数处理，不过没有关系，可以在生长一个参数的时候就协调了其他的模式
			///模式完善自己的附属模式和变形形式（比如前后的修饰，以及否定等形式）。
			///让模式进行生长和完善，模式查找自己的多个参数（每个参数也要寻找自己的附属模式）。
			///一个模式无法完成，是表示这个模式的各个参数不能被满足，即没有任何的可能性。
			///有一个问题是，这个模式是尽量生长更大但使自己的质量变差，还是保证一个适当的大小，但质量很好呢
			return true;
		}

		public void Clear生长对象()
		{
			本轮结果集.Clear();
			一级原始对象.Clear();
			任务对象 = new 任务对象();
			全部对象.Clear();
			附加关联集合.Clear();
			待选用抽象对象集.Clear();
			//连续生长开始轮数 = -1;
			同名待创建对象.Clear();
			左边界排序对象.Clear();
			右边界排序对象.Clear();
			待生长对象对集合.Clear();
			已进行关联创建对象集合.Clear();
			完成对象.对象集合.Clear();
			被处理阶段抑制对象.Clear();
			Data.清除缓存();
			处理轮数 = 0;
			有效对象计数 = 0;
			Data.计数器 = 0;
			处理选项 = 0;
			//处理阶段 = 0;
			句子合并和深度再处理阶段 = 0;
			当前内部范围 = 内部范围 = new 封闭范围(0, Data.当前句子串.Length, 9);
			强势完成对象集合.Clear();
			新词汇集合.Clear();
			//推导个数 = 0;
			//覆盖型对象标识生成器 = 0;
		}




		//分解串，并把所有可能的串都加入到【全部对象池】中，但是根据概率计算，概率更大的串将放置在前边。
		public void 执行分解串并生成串对象(string str)
		{
			Clear生长对象();

			Processor processor = Processor.当前处理器;

			processor.源语言文字 = str.ToCharArray();
			processor.字符串匹配处理();

			//排序，把子串排后边。
			processor.matchresults.Sort((匹配语料对象 v1, 匹配语料对象 v2) =>
			{
				if (v1.begindex >= v2.begindex && v1.endindex <= v2.endindex)
					return 1;
				if (v2.begindex >= v1.begindex && v2.endindex <= v1.endindex)
					return -1;
				return 0;
			});

			foreach (匹配语料对象 obj in processor.matchresults)
			{
				//add by wuyougang 外围库创建模式对象
				if (!obj.语料串.IsCore && Data.外围语料库.Any())
				{
					模式 形式row = Data.FindRowByID(obj.语料串.ObjectID);
					if (形式row == null)
					{
						形式row = Data.查找和创建外围对象模式及其拥有形式模式行(obj.语料串.ObjectID, obj.语料串.字符串);
						//obj.语料串.ObjectID = 形式row.ID;
						//obj.语料串 = new 形式化语料串() { ObjectID = 形式row.ID, 字符串 = 形式row.形式, IsCore = false };
					}
				}
				//////////////////
				模式 字串row = Data.增加字符串生长素材(obj);

				生长对象 串对象 = new 生长对象(字串row, 0);
				串对象.begindex = obj.begindex;
				串对象.endindex = obj.endindex;
				串对象.对应匹配语料对象 = obj;
				加入一个对象到池(串对象);
			}

			// TODO: 核心算法调整结束之后，取消下行注释
			Data.提取命名实体();
			foreach (命名实体 实体对象 in Data.当前命名实体集合)
			{
				foreach (生长对象 串对象 in 全部对象)//本轮结果集)
				{
					if (串对象.begindex == 实体对象.begindex && 串对象.endindex == 实体对象.endindex)
					{
						实体对象.是否已存在 = true;
						break;
					}
				}
				// 加入新对象Row
				if (实体对象.是否已存在 == false)
				{
					模式 语义Row = null, 形式Row = null;
					Data.构建命名实体语义和形式模式行(实体对象.基类型ID, 实体对象.ToString(), 实体对象.概率, 实体对象.语言, out 语义Row, out 形式Row);

					模式 字串row = Data.增加字符串生长素材(实体对象);
					生长对象 串对象 = new 生长对象(字串row, 0);
					串对象.begindex = 实体对象.begindex;
					串对象.endindex = 实体对象.endindex;
					串对象.对应匹配语料对象 = new 匹配语料对象(实体对象.begindex, 实体对象.endindex, new 形式化语料串() { ObjectID = 形式Row.ID, 字符串 = 形式Row.形式, IsCore = false });
					加入一个对象到池(串对象);
				}
			}
		}


		//下边算法回溯是几何级数增加，在节点数超过50个以上后，将出现很长时间不返回的情况。如果有字符串完全没有匹配，将容易死机。
		//前边用了【填补未匹配字符串】这个方法后，使得现在所有字符都填满不会出现空缺，因而这个问题就不存在了。
		public bool 回溯得到一个结果(List<生长对象> 对象集合, 选择结果 result, bool 只找一个最好的不回溯)
		{
			int 对象序号 = 0;
			if (result.选入的结果成员.Count > 0)//表示是再次计算了。
				对象序号 = result.下一结果以及回溯();//尝试下一个结果。

			while (true)
			{
				if (对象序号 >= 对象集合.Count)//这一级已经全部找完了，回溯
				{
					if (只找一个最好的不回溯 == true)
						return false;//调用者知道可以用这个不完整的结果。
					if (result.选入的结果成员.Count == 0)//回溯到最后一级了，全部结束。
						return false;
					对象序号 = result.下一结果以及回溯();//回退一级。
				}
				else
				{
					生长对象 obj = 对象集合[对象序号];
					if (result.可以加入(obj))
					{
						result.加入一个匹配对象(obj, 对象序号);
						if (result.未完成字符数 == 0)//所有字符都找到了。
							return true;
					}
					对象序号++;
				}
			}
		}

		//这里要求派生的【拥有形式】是从基类的【拥有形式】继承来的。
		public 模式 获得具有同名的派生对象(模式 派生类, string 形式)
		{
			参数树结构 参数树 = Data.利用缓存得到基类和关联记录树(派生类, false);
			return 参数树.得到拥有特定形式(形式);
		}


		//obj是已经匹配好的对象。parentrow是要挂接到的位置。
		//根据匹配上的所有字符串取得可能的语义
		//这个获取目前是返回所有的。
		//今后可能会加入一个阀值，这样低于该阀值的就不计入，但是如果整体匹配失败，有一种机制可能回头来降低阀值
		//以加入以前忽略的语义，然后再次尝试
		//这个获取只考虑匹配上的各单个字符串本身，并没有考虑相互的制约关系
		public void 执行创建上级对象或派生对象(生长对象 触发创建的参数对象)
		{
			if (触发创建的参数对象.是介词或者串(true, true, true))
			{
				//根据一个唯一字符串，从【语料库】中查找有哪些【拥有形式】拥有这个语料串。
				//查找到的每条记录应该都是一种【拥有形式】的记录。
				//匹配语料对象 obj = 触发创建的参数对象.对应匹配语料对象;
				//List<形式化语料串> dr = Data.语料库.Where(r => r.ParentID == obj.语料串.ID).ToList();
				整数对 结果 = 查找所有拥有形式行(触发创建的参数对象.模式行.形式);
				//add by wuyougang
				if (结果.beginindex < 0)
				{
					结果.beginindex = -1;
					结果.endindex = -1;

				}
				////////////////////////
				int 当前外围对象序号 = 0;
				int 外围对象总数 = 0;
				for (int i = 结果.beginindex; i <= 结果.endindex; i++)
				{
					形式化语料串 语料row;
					//modi by wuyougang
					if (i >= 0 && 外围对象总数 == 0)
					{
						语料row = Data.语料库[i]; //修改前只有这一行
					}
					else
					{
						if (触发创建的参数对象.对应匹配语料对象 != null && 触发创建的参数对象.对应匹配语料对象.语料串 != null)
							语料row = 触发创建的参数对象.对应匹配语料对象.语料串;
						else
							break;
					}
					模式 拥有形式的模式行 = Data.查找拥有形式模式行(语料row, 当前外围对象序号, ref 外围对象总数);
					if (外围对象总数 > 1)
					{
						当前外围对象序号++;
						if (当前外围对象序号 < 外围对象总数)
							i--; //外围对象有多个时，特殊循环，语料库不后移
						else
							外围对象总数 = 0; //外围对象处理完成
					}
					else
						外围对象总数 = 0;

					if (拥有形式的模式行 == null)
					{
						Data.Assert(false); //外围数据库中对应的语料此时正好被删除，则有可能会取不到对象
						continue;
					}
					/////////////////
					//模式 拥有形式的模式行 = Data.FindRowByID(语料row.ObjectID);

					if (Data.是拥有形式(拥有形式的模式行))//[拥有形式][拥有介词][拥有的地]都被选取出来。只是是过滤了【属于字符串】的引用串。
					{
						模式 模板模式 = Data.FindRowByID(拥有形式的模式行.A端);
						//一、模板处理
						if (Data.一级关联类型(模板模式).Equals(Data.模板引用Guid))
						{
							//模板引用，进行模板引用的处理。
							模板模式 = Data.FindRowByID(模板模式.B端);
							生长对象 o = 根据模式树生成生长对象(触发创建的参数对象.begindex, 模板模式, 处理轮数);
							//o.中心对象 = 触发创建的参数对象;//为了挂接根
							加入结果集排除掉相同的(o);
						}
						//二、是【拥有附属形式】的创建，比如介词。根据一些【介词】的形式反过来创建真正的对象。比如，根据【和】创建集合。
						else if (Data.是附属形式(拥有形式的模式行.ID, false)) //语料row.ObjectID
						{
							continue;
							string str = 触发创建的参数对象.取子串;
							//if (str == "的")//加入一个空对象占位，可以很大的简化处理工作。最终只会在【的】的后边生效，如果有真正的对象，那么这个会是互斥的。
							//	加入结果集排除掉相同的(构造隐藏对象(Data.ThisGuid, "[nullthis]", obj.endindex, 触发创建的参数对象, 处理轮数)).打分.概率分 = Data.的后NullThis概率分;///创建在【的】的后边。
							//else if (str != "地")
							if (str != "的" && str != "地" && str != "得" && Data.是附属形式(拥有形式的模式行.ID, true))//排除一些明显不会的，提高效率
							{
								参数字段 打分 = 拥有形式的模式行.参数;// new 参数字段(拥有形式的模式行.参数集合);
								if (打分.B对A的创建性 > 0)
								{
									Guid id = Data.返回基本关联(拥有形式的模式行.ID, false);
									Data.Assert(id.Equals(Data.关联拥有前置介词Guid) || id.Equals(Data.关联拥有后置介词Guid));
									int 位置 = id.Equals(Data.关联拥有前置介词Guid) ? 触发创建的参数对象.begindex : 触发创建的参数对象.endindex;
									//对于前置介词，中心对象的位置在左，对于后置介词，中心对象的位置在右，这样，中心对象正好和这个介词以及参数进行生长。
									//对于实体中心介词，不应该在这里进行处理！

									拥有形式的模式行 = Data.FindRowByID(拥有形式的模式行.A端);//应该是关联。
									if (Data.是二元关联(拥有形式的模式行, false))
									{
										拥有形式的模式行 = Data.FindRowByID(拥有形式的模式行.A端);//真正的隐藏上级对象。
										基准类型 基准对象 = null;
										int k = Data.计算基准对象(拥有形式的模式行, ref 基准对象);
										拥有形式的模式行 = Data.FindRowByID(基准对象.id);//基准对象

										生长对象 o = 加入结果集排除掉相同的(创建或返回隐藏对象(拥有形式的模式行.ID, 拥有形式的模式行.形式, 位置, 位置, 触发创建的参数对象, 处理轮数));//创建在【和】的前边。比如【他和她】，那么，这个空的谓语看着在【他】的后边。
										o.参数.概率分 = 打分.B对A的创建性;//先这样，让对象的概率分触发对象的【隐藏创建分】。
										//if (推导个数 == 0 && Data.推导介词集合.Contains(str))//加入推导对象。现在假设一个句子只出现一个整体的推导对象。
										//{
										//    //其实，不应该在这里创建推导的！要在最后的句子的角度才可以。
										//    空对象 = 构造无形式空对象(Data.推导即命题间关系Guid, "[=>]", obj.begindex, 串对象, 1);//创建在【如果】的前边。
										//    推导个数++;
										//}
										//注意，现在把表达式还是作为二元的处理，这样，加号等就成为形式主体而不是介词了。把下边语句屏蔽掉。
										//else if (Data.表达式介词集合.Contains(str))//表达式集合，加入表达式对象。
										//{
										//    Thisrow = 增加占位空语义行(Data.表达式Guid, "[Exp]", obj.begindex);//
										//}
									}
								}
							}
						}
						//三、普通的拥有形式的处理。
						else
						{
							生长对象 一级语义对象 = null;
							参数字段 打分 = 拥有形式的模式行.参数; //new 参数字段(拥有形式的模式行.参数集合);
							if (打分.B对A的创建性 > 0)
							{

								while (true)
								{
									生长对象 o = 查找相同的一级对象(拥有形式的模式行.A端, Data.ThisGuid, 拥有形式的模式行.A端, 字典_目标限定.A端, 触发创建的参数对象.begindex, 触发创建的参数对象.endindex);
									if (o == null)
									{
										o = 构造一级语义对象(触发创建的参数对象, 拥有形式的模式行, 处理轮数);//根据[拥有形式]创建的显式对象。
										o.中心对象 = 触发创建的参数对象;//为了挂接根
										o.匹配的形式行 = 拥有形式的模式行;
										o.一级对象构建形式和关键参数(触发创建的参数对象);
										加入结果集排除掉相同的(o);
									}
									if (一级语义对象 != null)
										一级语义对象.基对象 = o;
									一级语义对象 = o;

									if (Data.拥有形式集合.Contains(拥有形式的模式行.源记录))
										break;
									拥有形式的模式行 = Data.FindRowByID(拥有形式的模式行.源记录);
									Data.Assert(Data.是拥有形式(拥有形式的模式行));
									Data.Assert(Data.取得嵌入串(拥有形式的模式行) == 触发创建的参数对象.模式行.形式);
								}
							}
							else //抽象对象先加入"待选用抽象对象集"
							{
								while (true)
								{
									生长对象 o = 查找相同的一级对象(拥有形式的模式行.A端, Data.ThisGuid, 拥有形式的模式行.A端, 字典_目标限定.A端, 触发创建的参数对象.begindex, 触发创建的参数对象.endindex);
									if (o == null)
									{
										o = 构造一级语义对象(触发创建的参数对象, 拥有形式的模式行, 处理轮数, false);//根据[拥有形式]创建的显式对象。
										o.中心对象 = 触发创建的参数对象;//为了挂接根
										o.匹配的形式行 = 拥有形式的模式行;
										//o.一级对象构建形式和关键参数(触发创建的参数对象);
										待选用抽象对象集.Add(o);
									}
									if (一级语义对象 != null)
										一级语义对象.基对象 = o;
									一级语义对象 = o;

									if (Data.拥有形式集合.Contains(拥有形式的模式行.源记录))
										break;
									拥有形式的模式行 = Data.FindRowByID(拥有形式的模式行.源记录);
									Data.Assert(Data.是拥有形式(拥有形式的模式行));
									Data.Assert(Data.取得嵌入串(拥有形式的模式行) == 触发创建的参数对象.模式行.形式);
								}
							}
						}
					}
				}
			}
			//四、根据语义对象创建语义对象
			else
			{
				return;//以前考虑是【如果】等创建【推导】，但实际上，【如果】是中心介词，创建的推导的位置并不能保证合适，是要在后边另外处理，所以这里就先返回不做了。

				//不过未来，可能对于更深层次的，比如【语义】创建【语用】对象，似乎要做！
				//而且创建位置应该还是长度为0，不需要考虑覆盖对象了！如果是语义创建，那么在创建的一刻就和触发创建的参数糅合在一起，就可以回避长度为0的问题。
				//List<模式> dr = Data.模式表.对象集合.Where<模式>(r => r.B端 == 触发创建的参数对象.源模式行ID && r.语境树 == 0).ToList();
				//List<端索引> dr=Data.FindRowByID(触发创建的参数对象.源模式行ID).端索引表.Where(r=> r.that端==字典_目标限定.B端 && r.模式Row.语境树==0).ToList();
				foreach (模式 row in Data.FindRowByID(触发创建的参数对象.源模式行ID).端索引表_B端)
				{
					if (row.语境树 == 0)
					{
						参数字段 打分 = row.参数;//new 参数字段(row.参数集合);
						if (打分.B对A的创建性 > 0 && Data.是二元关联(row, false))
						{
							模式 上级对象 = Data.FindRowByID(row.A端);//隐藏上级对象。
							基准类型 基准对象 = null;
							int k = Data.计算基准对象(上级对象, ref 基准对象);
							上级对象 = Data.FindRowByID(基准对象.id);//基准对象

							生长对象 o = 加入结果集排除掉相同的(创建或返回隐藏对象(上级对象.ID, 上级对象.形式, 触发创建的参数对象.begindex, 触发创建的参数对象.begindex, 触发创建的参数对象, 处理轮数));
							//生长对象 o = 加入结果集排除掉相同的(创建或返回隐藏对象(上级对象.ID, 上级对象.形式, 触发创建的参数对象.begindex, 触发创建的参数对象.endindex, 触发创建的参数对象, 处理轮数));
							o.参数.概率分 = 打分.B对A的创建性;//先这样，让对象的概率分触发对象的【隐藏创建分】。
						}
					}
				}
			}
		}

		private 整数对 查找所有拥有形式行(string 串)
		{

			int index = Data.语料库.BinarySearch(new 形式化语料串() { 字符串 = 串 });
			//int index = 0;
			//while (index < Data.语料库.Count)
			//{
			//    if (Data.语料库[index].字符串 == 串)
			//        break;
			//    index++;
			//}

			//remove next line by wuyougang
			//Data.Assert(index >= 0);

			整数对 结果 = new 整数对(index, index);
			//add by wuyougang
			if (index < 0)
				return 结果;
			///////////////////
			while (结果.beginindex > 0)
			{
				if (Data.语料库[结果.beginindex - 1].字符串 != 串)
					break;
				结果.beginindex--;
			}
			while (结果.endindex < Data.语料库.Count - 1)
			{
				if (Data.语料库[结果.endindex + 1].字符串 != 串)
					break;
				结果.endindex++;
			}

			return 结果;
		}


		//public void 递归创建同形式派生对象(匹配语料对象 obj, 参数树结构 派生树, 生长对象 基对象, 生长对象 串对象)
		//{
		//    if (派生树.子节点 == null)
		//        return;
		//    foreach (参数树结构 o in 派生树.子节点)//暂时只取第一级派生对象，但也许需要递归地取所有的派生树，因为参数可以跨级重载。不过派生对象也许不是很多。
		//    {
		//        模式 拥有形式的模式行 = 获得具有同名的派生对象(o.目标, obj.ToString());
		//        if (拥有形式的模式行 != null)
		//        {
		//            生长对象 一级派生语义对象 = 构造一级语义对象(obj, 拥有形式的模式行, 处理轮数);//根据[拥有形式]创建的显式对象。
		//            if (一级派生语义对象 != null)
		//            {
		//                一级派生语义对象.中心对象 = 基对象;//为了挂接根
		//                一级派生语义对象.基对象 = 基对象;
		//                一级派生语义对象.一级对象构建形式参数(串对象);
		//                加入结果集排除掉相同的(一级派生语义对象);
		//                递归创建同形式派生对象(obj, o, 一级派生语义对象, 串对象);
		//            }
		//        }
		//        else
		//            递归创建同形式派生对象(obj, o, 基对象, 串对象);
		//    }

		//}

		public 生长对象 为未匹配串添加新的语义对象(模式 字串Row, SubString obj, 模式 baserow)
		{
			生长对象 串对象 = new 生长对象(字串Row, 0);
			加入一个对象到池(串对象);

			//增加一个串对象，但实际新知识不用，这个只是为了维持【结果】treelist中间统一的一种视图表现。让新对象在这个界面中有地方可以挂。

			模式 语义概念行 = Data.加入到素材(Data.New派生行(baserow, 字典_目标限定.空, true));
			语义概念行.序号 = obj.begindex;
			语义概念行.形式 = "[" + obj.ToString() + "]";
			语义概念行.语言角色 = 字典_语言角色.主语;

			生长对象 一级语义对象 = new 生长对象(语义概念行, 2);
			一级语义对象.endindex = obj.endindex;
			一级语义对象.中心对象 = 串对象;//为了在结果表中显示

			模式 新对象拥有形式行 = Data.New派生行(Data.拥有形式Guid, 字典_目标限定.空);
			新对象拥有形式行.B端 = Data.ThisGuid;
			新对象拥有形式行.A端 = 语义概念行.ID;
			新对象拥有形式行.ParentID = 语义概念行.ID;
			新对象拥有形式行.形式 = obj.ToString();
			新对象拥有形式行.序号 = obj.begindex;
			新对象拥有形式行.语言 = Data.当前解析语言;
			新对象拥有形式行.语言角色 = 字典_语言角色.全部;
			Data.get模式编辑表().新加对象(新对象拥有形式行);
			生长对象 拥有形式对象 = new 生长对象(新对象拥有形式行, 0);

			一级语义对象.处理对象的概念参数表(拥有形式对象, true);

			加入一个对象到池(一级语义对象);

			return 一级语义对象;
		}

		//这个处理没有考虑字符串的对应，所以，【红】和【红色】都看着一个东西，需要区分的话要额外处理。
		public 生长对象 查找相同的一级对象(Guid 源关联ID, Guid A端ID, Guid B端ID, int that, int begin, int end)
		{
			foreach (生长对象 已有对象 in 一级原始对象)
				if (源关联ID.Equals(已有对象.源模式行ID) && A端ID.Equals(已有对象.模式行.A端) && B端ID.Equals(已有对象.模式行.B端)
					//&& that == 已有对象.that && (begin == -1 || (begin >= 已有对象.begindex && end <= 已有对象.endindex)))//串位置比较不是要相等，而是父串创建的对象将禁止子串创建相同的对象。比如“游泳”创建了【游泳】，“游”就禁止了。
					&& that == 已有对象.that && ((begin == -1 || begin == 已有对象.begindex) && (end == -1 || end == 已有对象.endindex)))//串位置比较不是要相等，而是父串创建的对象将禁止子串创建相同的对象。比如“游泳”创建了【游泳】，“游”就禁止了。
					return 已有对象;
			return null;
		}
		public 生长对象 创建一个介词对象(模式 row, int begindex, int 处理轮数)
		{
			if (Data.是拥有形式(row))
			{
				row.序号 = begindex;
				生长对象 一级中心根 = new 生长对象(row, 处理轮数, true, true);

				一级原始对象.Add(一级中心根);
				加入结果集排除掉相同的(一级中心根);

				SubString 范围 = new SubString(begindex, begindex);

				row.序号 = begindex;
				int beg = row.序号;
				string s = Data.取得嵌入串(row);
				int end = beg + s.Length;

				//构造语料串
				SubString 语料串 = new SubString();
				语料串.begindex = beg;
				语料串.endindex = end;

				模式 字串row = Data.增加字符串生长素材(语料串);
				字串row.形式 = s;
				字串row.语言 = row.语言;

				生长对象 串对象 = new 生长对象(字串row, 0);
				串对象.begindex = 语料串.begindex;
				串对象.endindex = 语料串.endindex;
				加入一个对象到池(串对象);

				一级中心根.模式行.形式 = s;
				一级中心根.中心对象 = 串对象;
				一级中心根.一级对象构建形式和关键参数(串对象, null, s);
				if (一级中心根.长度 > 0)
					一级中心根.增加范围(beg, end);
				else
					一级中心根.复制位置(beg, end);
				范围.增加范围(beg, end);

				一级中心根.增加范围(范围);
				return 一级中心根;
			}
			return null;
		}
		public 生长对象 递归根据模式树生成生长对象(模式 row, int begindex, int 处理轮数, bool 模板方式 = true)
		{
			Data.Assert(row.A端.Equals(Data.ThisGuid) && Data.一级关联类型(row).Equals(Data.属于Guid));
			row.序号 = begindex;
			生长对象 一级中心根 = new 生长对象(row, 处理轮数, true, true);

			一级原始对象.Add(一级中心根);
			加入结果集排除掉相同的(一级中心根);
			生长对象 中心对象 = 一级中心根;

			SubString 范围 = new SubString(begindex, begindex);

			//var dr = Data.get模式编辑表().对象集合.Where(r => r.ParentID == row.ID).OrderBy(r => r.序号).ToList();
			int 起始序号 = -1;
			foreach (模式 关联row in row.端索引表_Parent.OrderBy(r => r.序号))
			{
				if (起始序号 < 0)
					起始序号 = 关联row.序号;
				//if (模板方式)
				//{
				//    关联row.序号+= begindex;
				//}
				//else
				关联row.序号 = 关联row.序号 - 起始序号 + begindex;
				int beg = 关联row.序号;
				if (Data.是拥有形式(关联row))
				{

					string s = Data.取得嵌入串(关联row);
					int end = beg + s.Length;
					if (模板方式)
					{
						生长对象 串对象 = 查找串对象(beg, end);
						一级中心根.一级对象构建形式和关键参数(串对象, 关联row);
						一级中心根.中心对象 = 串对象;
						if (一级中心根.长度 > 0)
							一级中心根.增加范围(beg, end);
						else
							一级中心根.复制位置(beg, end);
						范围.增加范围(beg, end);
						//一级中心根.模式行.形式 += "\"" + s + "\"";
					}
					else
					{
						//构造语料串
						SubString 语料串 = new SubString();
						语料串.begindex = beg;
						语料串.endindex = end;

						模式 字串row = Data.增加字符串生长素材(语料串);
						字串row.形式 = s;
						字串row.语言 = 关联row.语言;

						生长对象 串对象 = new 生长对象(字串row, 0);
						串对象.begindex = 语料串.begindex;
						串对象.endindex = 语料串.endindex;
						加入一个对象到池(串对象);

						一级中心根.模式行.形式 = s;
						一级中心根.中心对象 = 串对象;
						一级中心根.一级对象构建形式和关键参数(串对象, null, s);
						if (一级中心根.长度 > 0)
							一级中心根.增加范围(beg, end);
						else
							一级中心根.复制位置(beg, end);
						范围.增加范围(beg, end);
					}
				}
				else
				{
					Guid 对端ID = 关联row.That根 == 字典_目标限定.A端 ? 关联row.B端 : 关联row.A端;
					模式 参数row = Data.FindRowByID(对端ID);
					if (参数row.ParentID.Equals(关联row.ID) == false && 关联row.端索引表_Parent.Count > 0)
						参数row = 关联row.端索引表_Parent[0];
					生长对象 参数对象 = 递归根据模式树生成生长对象(参数row, beg, 处理轮数, 模板方式);
					生长对象 生长对象 = new 生长对象(中心对象, 参数对象, 关联row);
					foreach (模式 子对象 in 关联row.端索引表_Parent)
					{
						if (Data.关联拥有的Guid.Equals(子对象.源记录) || Data.关联拥有地Guid.Equals(子对象.源记录))
							生长对象.中间的和地 = 创建一个介词对象(子对象, 参数对象.endindex, 处理轮数);
					}

					//聚合关系时，替换生长时，需要重新替换B端实际对象
					生长对象 替换下端实际对象 = null;
					if (Data.是派生关联(Data.聚合Guid, 参数对象.源模式行) > 0)
					{
						if (参数对象.A端对象.是隐藏对象())
							替换下端实际对象 = 参数对象.A端实际对象;
						else
							替换下端实际对象 = 参数对象.B端实际对象;
					}
					中心对象 = 直接一级关联生长(生长对象, 处理轮数, false, null, 替换下端实际对象, 关联row);
					范围.增加范围(中心对象);
					加入结果集排除掉相同的(中心对象);
				}
			}
			//string str = Data.取得嵌入串((string)中心对象.模式行.形式);
			//中心对象.endindex = 中心对象.begindex + str.Length;
			中心对象.增加范围(范围);
			return 中心对象;
		}

		public 生长对象 根据模式树生成生长对象(int begindex, 模式 模板模式, int 处理轮数, bool 模板方式 = true)
		{
			//模式 拥有形式的语义row =Data.加入到素材( Data.CopyRow(模板模式));
			//拥有形式的语义row.序号 = 匹配串对象.begindex;

			List<模式> rows = Data.CopyTree(模板模式);
			模式 语义row = Data.PasteRows(rows, true, begindex, Data.当前素材Row, 模板方式 ? false : true);

			生长对象 r = 递归根据模式树生成生长对象(语义row, begindex, 处理轮数, 模板方式);

			return r;

		}

		public 生长对象 构造一级语义对象(生长对象 匹配串对象, 模式 拥有形式模式行, int 处理层级, bool 是否加入到素材 = true)
		{
			//6月14日
			//其实匹配的是【拥有形式】的记录，然后才反查出语义的。
			//从【拥有形式】反查出的语义对象。
			//PatternDataSet.模式编辑Row 拥有形式的语义row = (PatternDataSet.模式编辑Row)New属于行(匹配上的对象.A端, "模式编辑");
			模式 拥有形式的语义row = Data.New派生行(拥有形式模式行.A端, 字典_目标限定.空, true);
			if (是否加入到素材)
				Data.加入到素材(拥有形式的语义row);
			拥有形式的语义row.序号 = 匹配串对象.begindex;
			拥有形式的语义row.形式 = 匹配串对象.模式行.形式;
			拥有形式的语义row.语言角色 = 拥有形式模式行.语言角色;//注意：暂时让语义的语言角色等于形式需要的语言角色。也许需要调整。

			生长对象 r = new 生长对象(拥有形式的语义row, 处理层级);
			if (Data.概念拥有介词形式Guid.Equals(Data.二级关联类型(拥有形式模式行)))
				r.是介词形式创建的对象 = true;//应该在模式里边加一个属性，然后生长对象对等起来。
			if (Data.概念拥有压缩形式Guid.Equals(Data.二级关联类型(拥有形式模式行)))
			{
				r.是压缩形式对象 = true;
				r.完成分 -= 7;
			}
			else if (Data.概念拥有定语形式Guid.Equals(Data.二级关联类型(拥有形式模式行)))
			{
				r.是定语形式对象 = true;
			}

			一级原始对象.Add(r);
			return r;

			//打分字段 打分 = new 打分字段(0, 0, 1, 1);
			//拥有形式的语义row.打分 = 打分.ToString();

			//注意，暂时屏蔽，也可以放开一下代码，就把当前匹配的串指示出来。
			//创建了【拥有形式】的记录。
			//PatternDataSet.模式编辑Row 形式row = (PatternDataSet.模式编辑Row)New派生行(匹配上的对象.ID, patternDataSet.模式编辑, 字典_目标限定.连接);
			//形式row.A端 = 拥有形式的语义row.ID;
			//形式row.ParentID = 拥有形式的语义row.ID;
			//形式row.B端 = 字串Row.ID;
			//形式row.序号 = 拥有形式的语义row.序号;
			//形式row.that根 = (int)字典_目标限定.连接;
			//形式row.Table.Rows.Add(形式row);
			//打分 = new 打分字段(0, 0, 0, 0);
			//形式row.打分 = 打分.ToString();
		}


		//分两种空对象
		//【的】后边的是一个嵌入型的，位置明确。
		//【推导】这样的对象本身没有文字，但是其位置实际是覆盖整体对象。
		public 生长对象 创建或返回隐藏对象(Guid 基行ID, string 形式, int 位置, int 结束位置, 生长对象 挂接根, int 处理层级, bool 要求新建 = false)
		{
			Data.Assert(位置 == 结束位置);//现在发现，隐藏对象都要求长度为0，不需要覆盖型对象！因为隐藏对象本身为0，然后可以和参数直接结合后扩展长度而已！
			生长对象 o = 查找相同的一级对象(基行ID, Data.ThisGuid, 基行ID, 字典_目标限定.A端, 位置, 结束位置);
			if (o != null)
				return 要求新建 ? null : o;

			模式 This空语义row = Data.加入到素材(Data.New派生行(基行ID, 字典_目标限定.空, true));

			This空语义row.序号 = 位置;
			This空语义row.形式 = 形式;
			This空语义row.语言角色 = 字典_语言角色.中心;
			This空语义row.显隐 = 字典_显隐.隐藏;

			//打分字段 打分 = new 打分字段(0, 0, 1, 1);
			//This空语义row.打分 = 打分.ToString();

			生长对象 This一级语义对象 = new 生长对象(This空语义row, 处理层级);
			This一级语义对象.是无形式空对象 = true;
			This一级语义对象.endindex = 结束位置;
			This一级语义对象.中心对象 = 挂接根;//本来应该是没有中心对象的，只是为了在【结果表】中有个【原因根】方便查看而设置，这个中心对象一定应该是字符串
			This一级语义对象.一级对象构建形式和关键参数(null);
			//This一级语义对象.处理阶段 = Processor.当前处理器.处理阶段;

			一级原始对象.Add(This一级语义对象);

			return This一级语义对象;
		}

		public 模式 创建或返回一级关联素材记录(生长对象 对象对, 生长对象 A端实际对象 = null, 生长对象 B端实际对象 = null)
		{
			//增加这个校验，还是让所有的依据都是系统知识，本次生长的对象不做为依据。以后再修改扩充。
			if (A端实际对象 == null)
				A端实际对象 = 对象对.A端对象;
			if (B端实际对象 == null)
				B端实际对象 = 对象对.B端对象;
			//Guid A端 = 对象对.that == 字典_目标限定.A端 ? 上端实际对象.中心第一根类.模式行ID : 下端实际对象.中心第一根类.模式行ID;
			//Guid B端 = 对象对.that == 字典_目标限定.A端 ? 下端实际对象.中心第一根类.模式行ID : 上端实际对象.中心第一根类.模式行ID;
			Guid A端 = A端实际对象.中心第一根类.模式行ID;
			Guid B端 = B端实际对象.中心第一根类.模式行ID;

			//如果这个模式行以前已经有过，直接使用，而不重新创建。

			生长对象 o = 查找相同的一级对象(对象对.源模式行ID, A端, B端, 对象对.that, -1, -1);
			if (o != null)
				return o.模式行;

			模式 row = Data.New派生行(对象对.源模式行ID, 对象对.that);

			row.A端 = A端;
			row.B端 = B端;

			生长对象 一级关联对象 = new 生长对象(row, 2, false);
			一级原始对象.Add(一级关联对象);
			Data.加入到素材(row);
			对象对.加入各关联参数行到界面(row);
			return row;
		}

		//让该模式发起一次相邻的生长。返回null表示没有得到生长。
		//返回的对象是这次生长后的根对象，可能等于这个发起者本身，也可能是别的对象。也就是说发起者不一定就是根。比如【红色的苹果】，发起者是【红色的】，根则是【苹果】。
		public 生长对象 发起一次相邻生长(生长对象 发起者)
		{
			bool 能够序列化 = Data.能够序列化(发起者.中心第一根类.模式行);

			if (能够序列化)
			{
				//优先尝试序列化的生长。
			}
			else
			{
				//非序列化的生长，就是由【的】和【地】来支配的。
			}

			return null;
		}

		//判断该对象有紧密联系的【的】或者【地】。返回1
		//英语有前置的【of】，那个再单独考虑，应该是返回-1。
		//是【的】返回1.
		//是【地】返回2.

		public int 对象中间由的或地关联(生长对象 对象)
		{
			if (对象.中间的和地 == null)
				return 0;
			if (对象.中间的和地.是的或者地(true, false))//【的】
			{
				if (对象.中心在右 == false)//暂时不允许参数在右边带【的】，比如【苹果红色的】。
					return 0;
				return 1;
			}
			return 2;//【地】

			//int index = 对象.左对象.endindex;
			//if (index >= Data.当前句子串.Count())
			//    return 0;
			//if (对象.右对象.begindex - 对象.左对象.endindex != 1)//中文要求中间就是一个【的】或者【地】。
			//    return 0;
			//string 中间串 = Data.当前句子串.Substring(index, 1);
			//if (中间串 == "的")
			//    return 1;
			//if (中间串 == "地")
			//    return 2;
			//return 0;
		}

		public void 完成一个阶段生长(int 阶段)
		{
			生长阶段 = 阶段;
			bool 继续 = false;
			do
			{
				继续 = 进行一轮生长();
			} while (继续);

		}

		public void 第一阶段生长()
		{
			//完成一个阶段生长(1);
			进行一轮生长();
		}

		public bool 两个位置是相邻的(int begin, int end/*, bool 允许多余标点 = false*/)//中间间隔空格。中文英文都可以。
		{
			if (begin == end)
				return true;
			for (int i = begin; i < end; i++)
			{
				char c = Data.当前句子串[i];
				if (字符类.允许忽略(c) || 字符类.是停顿标点(c)/* && 允许多余标点 */)
					continue;
				return false;
			}
			return true;
		}


		public 生长对象 得到第一个完成对象()
		{
			生长对象 完成对象 = null;
			for (int i = 0; i < 全部对象.Count; i++)
				if (全部对象[i].begindex == 0 && 全部对象[i].endindex == Data.当前句子串.Length)
				{
					if (完成对象 == null || 全部对象[i].总分 > 完成对象.总分)
						完成对象 = 全部对象[i];
				}
			return 完成对象;
		}
		public 生长对象 计算抑制(生长对象 目标对象, 生长对象 已有对象, int 打分, int 阀值)
		{
			int k = 0;
			if (目标对象 != 已有对象 && (k = 生长对象.计算位置重叠性(目标对象, 已有对象)) > 0)
			{
				if (重叠部分有冲突(目标对象, 已有对象) == false)
				{
					if (k == 1)//位置交叉，新对象有新内容
						return null;
					//以下是重叠。
					if (目标对象.长度 > 已有对象.长度)//对象是已有对象的超集：【他借书给她】和【他借书】
						return null;
					return 已有对象;//对象是已有对象的子集：【他借书】和【他借书给她】
				}
				else if (打分 + 阀值 < 已有对象.概率打分)//对象和已有对象是语义交叉抑制关系。
					return 已有对象;
			}
			return null;
		}


		public bool 计算单对象被抑制性(生长对象 目标对象, 生长对象 已有对象, bool 向右)
		{
			//if (处理阶段 < 4)//前几个处理阶段是产生线索，尽可能多，所以不考虑抑制，后边阶段开始寻求组合，才考虑抑制。
			//{
			//    //只考虑绝对抑制，不考虑相对抑制
			//    return false;
			//}
			//else
			//{
			//    //同时考虑绝对抑制和相对抑制。
			//}
			if (目标对象.取子串 == "伊朗" && 已有对象.取子串 == "伊朗战舰逼近")
				向右 = 向右;

			if (目标对象 == 已有对象)
				return false;

			if (目标对象.生长次数 > 0 && 目标对象.有效对象序数 > 已有对象.有效对象序数)
				return false;

			if (已有对象.中心对象 == null)//串对象不能抑制别的对象。
				return false;

			if (已有对象.概率打分 <= 0)//打分小于0的不能抑制别人。
				return false;

			if (已有对象.取消抑制性 == true)//已经取消抑制性的不能抑制别人。
				return false;

			//if (目标对象.概率分加完成分 > 9)//大于9分的不被抑制。但注意，绝对抑制不受这个控制！
			//	return false;

			if (目标对象.长度 > 已有对象.长度)//长对象不被短对象抑制。
				return false;
			//生长方向出头的不被抑制。
			if (向右)
			{
				if (目标对象.begindex < 已有对象.begindex)
					return false;
			}
			else
			{
				if (目标对象.endindex > 已有对象.endindex)
					return false;
			}

			if (目标对象.begindex == 已有对象.begindex && 目标对象.endindex == 已有对象.endindex)
			{
				if (目标对象.概率打分 >= 已有对象.概率打分)
					return false;
				if (已有对象.介动词等情况延后一阶段生长)
					return false;
			}

			//int k = 生长对象.计算位置重叠性(目标对象, 已有对象);

			//if (k == 0)
			//    return false;
			//if (k == 2)
			//    return false;
			if (目标对象.begindex < 已有对象.begindex || 目标对象.endindex > 已有对象.endindex)
				return false;
			return true;
			//return 计算多意性抑制(目标对象, 已有对象);
			//暂时先这样，有重叠就算抑制
			//具体中，还是要分析下对目标对象的拆解。
			//if (计算拆解抑制(目标对象, 已有对象))
			//    return true;

			//if (已有对象.递归查找参数对象(目标对象) == null)
			//    return false;
			//if (k == 2) //交叉时判断，已有对象是否“拥有类”
			//{
			//    if (Data.是派生类(Data.拥有Guid, 已有对象.中心第一根类.源模式行, 替代.正向替代))
			//    {
			//        return true;
			//    }
			//    if (目标对象.取子串 == "海峡" && 已有对象.取子串 == "通过海峡")
			//        k = k;
			//    //if (判断指定对象是否已经被生长为已有对象的参数(目标对象,已有对象))
			//    //    return true;
			//}
			//return 计算多意性抑制(目标对象, 已有对象);
			//return true;
			//if (重叠部分有冲突(目标对象, 已有对象))
			//    return true;

			//return false;

		}
		public bool 判断指定对象是否已经被生长为已有对象的参数(生长对象 对象, 生长对象 已有对象)
		{
			List<参数> 概念参数表 = 已有对象.得到指定根对象的参数表(已有对象.中心第一根类);
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() && o.对端派生对象.参数对象 != null && o.对端派生对象.参数对象.中心第一根类 == 对象
					&& o.对端派生对象.参数对象.中心第一根类.begindex == 对象.begindex
					&& o.对端派生对象.参数对象.中心第一根类.endindex == 对象.endindex)
					return true;
			}
			return false;
		}
		public bool 计算拆解抑制(生长对象 目标对象, 生长对象 已有对象)
		{
			if (目标对象 == 已有对象.中心对象)
				return true;
			if (目标对象 == 已有对象.参数对象)
				return true;
			return false;
		}

		public bool 计算多重解释抑制(生长对象 目标对象, 生长对象 已有对象)
		{
			if (目标对象 == 已有对象.中心对象)
				return true;
			if (目标对象 == 已有对象.参数对象)
				return true;
			return false;
		}
		public bool 计算多意性抑制(生长对象 目标对象, 生长对象 已有对象)
		{
			if (已有对象.中心对象 != null && 目标对象 != 已有对象 && 目标对象.begindex == 已有对象.中心对象.begindex && 目标对象.endindex == 已有对象.中心对象.endindex
				&& 目标对象 != (已有对象.中心对象.中心第一根类.长度 == 已有对象.中心对象.长度 ? 已有对象.中心对象.中心第一根类 : 已有对象.中心对象))
				return false;
			if (已有对象.参数对象 != null && 目标对象 != 已有对象 && 目标对象.begindex == 已有对象.参数对象.begindex && 目标对象.endindex == 已有对象.参数对象.endindex
				&& 目标对象 != (已有对象.参数对象.中心第一根类.长度 == 已有对象.参数对象.长度 ? 已有对象.参数对象.中心第一根类 : 已有对象.参数对象))
				return false;

			//生长对象 参数=已有对象.递归查找参数对象(目标对象);
			//if (参数 == null)
			//    return false;
			return true;
		}

		public bool 重叠部分有冲突(生长对象 目标对象, 生长对象 已有对象)
		{
			Data.Assert(目标对象 != 已有对象);

			SubString 重叠范围 = 目标对象.重叠范围(已有对象);

			if (重叠范围.长度 == 0)
				return false;

			return 目标对象.递归判断冲突(已有对象, 重叠范围) == 2;
		}

		public bool 所在位置对连续串进行了拆解(封闭范围 范围, int 附加限定边界, int 位置, bool 边界在左)
		{
			if (边界在左)
			{
				if (位置 == 范围.内在begindex || 位置 == 附加限定边界)
					return false;
				char c = Data.当前句子串[位置 - 1];
				return 字符类.是分隔标点(c) == false;

			}
			else
			{
				if (位置 == 范围.内在endindex || 位置 == 附加限定边界)
					return false;
				char c = Data.当前句子串[位置];
				return 字符类.是分隔标点(c) == false;
			}
		}

		public bool 禁止从合成对象中拆解(生长对象 目标对象, 生长对象 已有对象, bool 要求另一部分没有别的生长)
		{
			//如果新的部件（比如【天】）是一个拆解目标（比如【昨天】）的中心，

			if (目标对象 == null || 目标对象 == 已有对象)
				return false;

			if (已有对象.是介词或者串(true, true, true))
				return false;

			SubString 另一部分 = 已有对象.减去范围(目标对象);
			if (另一部分.长度 == 0)
				return false;

			if (要求另一部分没有别的生长 == false)
				return false;

			//if (Data.是派生类(部件.源模式行ID, 拆解目标.中心第一根类.源模式行, 替代.正向替代) == false)//如果拆解目标的中心类不是部件的类，那么就不处理
			//	return false;
			if (要求另一部分没有别的生长)
				foreach (生长对象 o in 全部对象)
				{
					if (o.是介词或者串(true, true, true))
						continue;
					if (已有对象 != o && 生长对象.计算位置重叠性(o, 另一部分) == 2)//另一部分有了交叉生长，那么就可以被拆解了。
						return false;
				}

			return true;
		}

		//这里增加对【明确完成对象】的判断，【明确完成对象】就是有很高的概率，各参数都完成了，也就是应该是明确的！
		public int 是明确完成对象(生长对象 obj)
		{
			return obj.完成分;
		}

		public bool 是后置宾语(生长对象 假设宾语对象, 生长对象 整体模式)
		{
			foreach (参数 o in 整体模式.得到指定根对象的参数表())
			{
				if (o.B端对象 != null && o.B端对象.B端对象 != null && 假设宾语对象.begindex == o.B端对象.B端对象.begindex && 假设宾语对象.endindex == o.B端对象.B端对象.endindex)  //假设宾语对象 == o.B端对象.B端实际对象)
					return (o.语言角色 & (字典_语言角色.一宾 | 字典_语言角色.二宾)) > 0;
			}

			return false;
		}

		public bool 是前置主语(生长对象 假设主语对象, 生长对象 整体模式)
		{
			foreach (参数 o in 整体模式.得到指定根对象的参数表())
			{
				if (o.B端对象 != null && 假设主语对象 == o.B端对象.B端实际对象)
					return (o.语言角色 & (字典_语言角色.主语 | 字典_语言角色.前独)) > 0;
			}

			return false;
		}
		public bool 计算单对象被抑制性(生长对象 对象)
		{
			//1.先检查是事和强势对象冲突
			if (和强势完成对象冲突(对象))
				return true;
			//2.再检查是否和已有对象冲突
			foreach (生长对象 已有对象 in 全部对象)
			{
				if (计算单对象被抑制性(对象, 已有对象, true))
					return true;
			}
			return false;
		}
		public bool 计算对象对被抑制性(生长对象 对象对)
		{
			//1.先检查是否被强势对象包含抑制
			if (和强势完成对象冲突(对象对.左对象) || 和强势完成对象冲突(对象对.右对象))
				return true;
			//2.再检查是否左、右对象都被抑制
			if (计算单对象被抑制性(对象对.左对象) && 计算单对象被抑制性(对象对.右对象))
			{
				foreach (生长对象 已有对象 in 全部对象)
				{
					//如果对象对，左、右两边 都被同一个对象抑制，数量和量词的结合应该不被抑制。
					if (计算单对象被抑制性(对象对.左对象, 已有对象, true) && 计算单对象被抑制性(对象对.右对象, 已有对象, true))
					{
						if (对象对.中心在右 && Data.是派生类(Data.数Guid, 对象对.左对象.模式行, 替代.正向替代) && Data.是派生类(Data.量词个Guid, 对象对.右对象.模式行, 替代.正向替代))
							return false;
					}
				}
				return true;
			}
			else
				return false;
			/*
			if (计算单对象被抑制性(对象对.左对象) == false || 计算单对象被抑制性(对象对.右对象) == false)
				return false;
			else
				return true;
			*/
		}
		//先考虑左边对象是否被抑制，这个好像不算很好
		//起码，应该两个方向去考虑？
		public bool 计算对象被抑制性(生长对象 对象对)
		{
			//if (obj.生长次数 < 1)//用对象是一级对象不被抑制？
			//	return false;
			//if (obj.处理阶段 < 3)
			//	return false;
			//bool 向右 = !对象对.中心在右;//调整成这样如何？以前考虑了向右不是中心对象的关系，而是生长方向的关系。
			foreach (生长对象 已有对象 in 本轮结果集)
			{
				if (计算单对象被抑制性(对象对.左对象, 已有对象, true) && 计算单对象被抑制性(对象对.右对象, 已有对象, true))
				{
					return true;
				}
			}
			foreach (生长对象 已有对象 in 全部对象)
			{
				//if (对象对.中心在右)
				//{
				//    if (已有对象.begindex <= 对象对.参数对象.begindex && 已有对象.endindex >= 对象对.中心对象.endindex
				//        && 已有对象.长度 > 对象对.中心对象.endindex - 对象对.参数对象.begindex)
				//        return true;

				//}
				//else
				//{
				//    if (已有对象.begindex <= 对象对.中心对象.begindex && 已有对象.endindex >= 对象对.参数对象.endindex
				//        && 已有对象.长度 > 对象对.参数对象.endindex - 对象对.中心对象.begindex)
				//        return true;
				//}
				if (计算单对象被抑制性(对象对.左对象, 已有对象, true) && 计算单对象被抑制性(对象对.右对象, 已有对象, true))
				{
					return true;
					//bool 忽略抑制性 = 对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && 是后置宾语(对象对.参数对象, 已有对象);
					//if (忽略抑制性 == false)
					//    return true;
					//goto 另一个方向;
				}
			}
			return false;


			//2、非本轮对象要看是否在连续生长循环内，如果在连续生长循环内就相当于是广义的本轮对象，就抑制。
			//是强势完成对象的话，不在连续生长循环内也抑制。
			foreach (生长对象 已有对象 in 本轮结果集)
			{
				if (已有对象.begindex < 对象对.左对象.endindex)
					continue;
				if (计算单对象被抑制性(对象对.右对象, 已有对象, true))
				{
					bool 忽略抑制性 = 对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && 是后置宾语(对象对.参数对象, 已有对象);
					if (忽略抑制性 == false)
						return true;
				}
			}
			foreach (生长对象 已有对象 in 全部对象)
			{
				//if (已有对象.处理轮数 < 连续生长开始轮数 && 是明确完成对象(已有对象) <= 0)//不在本个连续生长循环以内，也就是以前的对象。并且不是明确完成对象，那么可以不抑制。
				//    continue;
				//if (obj.begindex < 已有对象.begindex || obj.endindex > 已有对象.endindex)//交叉的（也就包括长度更大的）不被抑制。
				//    continue;
				if (已有对象.begindex < 对象对.左对象.endindex)
					continue;

				if (计算单对象被抑制性(对象对.右对象, 已有对象, true))
				{
					//【前宾】处理对于参数是前边动词的宾语，后边中心是一个动词的情况下，不要抑制，允许尝试生长。
					bool 忽略抑制性 = 对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && 是后置宾语(对象对.参数对象, 已有对象);

					if (忽略抑制性 == false)
						return true;
				}
			}
			return false;

		另一个方向:

			foreach (生长对象 已有对象 in 本轮结果集)
			{
				if (计算单对象被抑制性(对象对.右对象, 已有对象, false))
					return true;
			}
			foreach (生长对象 已有对象 in 全部对象)
			{
				if (计算单对象被抑制性(对象对.右对象, 已有对象, false))
					return true;
			}

			foreach (生长对象 已有对象 in 本轮结果集)
			{
				if (已有对象.endindex > 对象对.右对象.begindex)
					continue;

				if (计算单对象被抑制性(对象对.左对象, 已有对象, false))
				{
					//【前宾】处理对于参数是前边动词的宾语，后边中心是一个动词的情况下，不要抑制，允许尝试生长。
					bool 忽略抑制性 = 对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && 是后置宾语(对象对.参数对象, 已有对象);

					if (忽略抑制性 == false || Data.是派生类(Data.拥有Guid, 已有对象.中心第一根类.源模式行, 替代.正向替代))
						return true;
				}
			}
			foreach (生长对象 已有对象 in 全部对象)
			{
				if (已有对象.endindex > 对象对.右对象.begindex)
					continue;

				if (计算单对象被抑制性(对象对.左对象, 已有对象, false))
				{
					//【前宾】处理对于参数是前边动词的宾语，后边中心是一个动词的情况下，不要抑制，允许尝试生长。
					bool 忽略抑制性 = 对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && 是后置宾语(对象对.参数对象, 已有对象);

					if (忽略抑制性 == false || Data.是派生类(Data.拥有Guid, 已有对象.中心第一根类.源模式行, 替代.正向替代))
						return true;
				}
			}


			return false;


			//生长对象 和中心对象最大冲突的对象 = 查找最大冲突的对象(obj.中心对象);
			//生长对象 和参数对象最大冲突的对象 = 查找最大冲突的对象(obj.参数对象);

			//int 冲突打分 = 0;
			//if(和中心对象最大冲突的对象 != null)
			//{
			//    冲突打分 = 和中心对象最大冲突的对象.生长优先分;
			//    if(和参数对象最大冲突的对象 != null && 和中心对象最大冲突的对象 !=和参数对象最大冲突的对象)
			//        冲突打分 = Data.合并概率打分(冲突打分, 和参数对象最大冲突的对象.生长优先分);
			//}
			//else if(和参数对象最大冲突的对象 != null)
			//    冲突打分 = 和参数对象最大冲突的对象.生长优先分;

			//if (obj.生长优先分 + 阀值差 >= 冲突打分)
			//    return false;
		}



		//这个处理的意思是，对于一个以前已经长成的可信度很高的对象，只应该把它长做一个整体进行处理，而不会再和它的一个部件进行处理了。
		//这里具体计算是传入的对象对以前的一个对象进行部分A拆解，而且该对象的另一部分B只包含在该对象中（假设这了生长了A，那么以后B也是无处可去）。

		public 生长对象 判断对已有对象进行了部分拆解(生长对象 obj, bool 要求另一部分没有别的生长 = true)
		{
			return null;

			foreach (生长对象 已有对象 in 本轮结果集)
				//if (obj.生长优先分 <= 已有对象.生长优先分)
				if (禁止从合成对象中拆解(obj, 已有对象, 要求另一部分没有别的生长))
					return 已有对象;
			foreach (生长对象 已有对象 in 全部对象)
				//if (obj.生长优先分 <= 已有对象.生长优先分)
				if (禁止从合成对象中拆解(obj, 已有对象, 要求另一部分没有别的生长))
					return 已有对象;

			return null;
		}

		public int 计算本轮生长最大打分()
		{
			int 本轮生长最大打分 = 0;

			foreach (生长对象 o in 本轮结果集)
				if (本轮生长最大打分 < o.概率分)
					本轮生长最大打分 = o.概率分;
			return 本轮生长最大打分;
		}

		//这次生长可以占据整个范围区域等。这样，就不用等待下次的【处理阶段】了！
		public bool 确定这次生长可以完全完成(生长对象 对象对)
		{
			if (内部范围.递归判断占满了封闭范围(对象对.begindex, 对象对.endindex, true))
				return true;
			return false;
		}

		public bool 和处理后延对象冲突(生长对象 obj, List<生长对象> 处理阶段后延对象)
		{
			foreach (生长对象 o in 处理阶段后延对象)
				if (生长对象.计算位置重叠性(obj, o) > 0)
					//这里没有比较优先级，也就是只满足一个生长，应该调整比较优先级的情况再决定。
					return true;
			return false;
		}


		public void 远程线索调整概率()
		{
		}

		public void 根据对象的关键参数字符串线索调整概率并设置介动词的优先性(生长对象 obj)
		{
			//比如obj=【借出】，调用的时候发现有【给】这个介词，那么就把【给】这个动词先延后，而是优先作为介词处理。
			int k = 0;
			if (obj.取子串 == "制造")
				k = k;
			if (Data.前置介词集合.Contains(obj.取子串))
				obj.介动词等情况延后一阶段生长 = true;
			foreach (参数 o in obj.得到指定根对象的参数表())
			{
				if (Data.是拥有形式(o.源关联记录))
					continue;
				int 语义记录的创建性 = o.源关联记录.参数.B对A的创建性;
				//一、查找出所有的关联表
				List<参数> 关联参数集合 = new List<参数>();
				参数树结构 tree = Data.利用缓存得到基类和关联记录树(o.源关联记录, true);
				tree.递归取出关联的形式参数(obj, ref 关联参数集合, Data.当前解析语言, 0, 0);
				foreach (参数 形式 in 关联参数集合)
				{
					if (Data.是派生关联(Data.关联拥有前置介词Guid, 形式.源关联记录) > 0 || Data.是派生关联(Data.关联拥有后置介词Guid, 形式.源关联记录) > 0)
					{
						string s = 形式.源关联记录.形式;
						if (s.Length == 0)
							continue;
						if (在当前范围中查找介词以及同名动词(s))
							if (语义记录的创建性 > 5 && 形式.源关联记录.参数.B对A的创建性 > 5)
								k += 2;
					}
				}
			}
			obj.参数.概率分 += k;
		}


		public bool 在当前范围中查找介词以及同名动词(string 串)
		{
			bool b = false;
			//这里其实要根据当前范围，而不是全部对象，不过可以暂时这样做！
			//实际上应该是，在当前范围，和有单括号等括起来的一级子范围里边是允许的，而不可能在外部去。
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.begindex > 内部范围.endindex)//已经到结束了。
					break;
				if (obj.begindex < 内部范围.begindex)
					continue;
				if (obj.取子串 == 串)
				{
					b = true;
					if (obj.是介词或者串(true, true, true) == false && Data.能够序列化(obj.中心第一根类.源模式行))
						obj.介动词等情况延后一阶段生长 = true;
				}
			}

			foreach (生长对象 obj in 本轮结果集)
			{
				if (obj.begindex > 内部范围.endindex)//已经到结束了。
					break;
				if (obj.begindex < 内部范围.begindex)
					continue;
				if (obj.取子串 == 串)
				{
					b = true;
					if (obj.是介词或者串(true, true, true) == false && Data.能够序列化(obj.中心第一根类.源模式行))
						obj.介动词等情况延后一阶段生长 = true;
				}
			}
			return b;
		}


		//从左相邻排序对象中进行查找，这个方法将处理空格和不认识的字符等，直到找到下一个对象。
		//如果失败，将返回-1。
		//并且这个对象不能超越当前范围。
		public int 向右取得下一个对象(int index)
		{
			return 左边界排序对象.Count; ;
		}
		public int 向左取得下一个对象(int index)
		{
			return 右边界排序对象.Count; ;
		}
		public int 从一个位置向右取得下一个对象(int 位置)
		{
			return 左边界排序对象.Count;
		}
		public int 从一个位置向左取得下一个对象(int 位置)
		{
			return 右边界排序对象.Count; ;
		}

		//两个对象中间有间隔（一般是逗号），而两个对象本身又不是【完整对象】-也就是起码对一个串进行了拆解。
		//【昨天，他吃饭了】这里的【昨天，他】就是有逗号间隔，而【他】是对【他吃饭了】这个连续的串进行了拆解。
		public bool 中间有间隔并且对连续串有拆解(封闭范围 范围, 生长对象 对象对, int 左边界, int 右边界)
		{
			if (对象对中间的间隔值(对象对) == 0)
				return false;

			//这里是否检查下，打断了一个匹配出的对象？如果匹配出就肯定是拆解？
			//这样，即使中间有分隔符号，但是分隔符号属于一个完整的匹配对象的一个部分，那么就不视为分隔符号？

			if (所在位置对连续串进行了拆解(范围, 左边界, 对象对.内部对象计算左边界(), true)
				|| 所在位置对连续串进行了拆解(范围, 右边界, 对象对.内部对象计算右边界(), false))
				return true;

			return false;
		}


		//返回1，表示允许
		//返回0，表示这个阶段还不允许，需要后延
		//返回-1，表示肯定不允许。
		public int 允许这种类型的生长或创建(生长对象 对象对, bool 创建, 封闭范围 范围, int 处理模式, int 左边界, int 右边界, int 规则)
		{
			//不满足要求的规则的，不允许。
			if ((处理模式 & 规则) == 0)
				return 0;
			//冒号做为介词时，需判断右对象是否完整短句，以句号结尾
			if (对象对.前置介词 != null && 是否短句停顿符介词(对象对.前置介词) && 对象对.前置介词.begindex >= 对象对.左对象.endindex)
			{
				if (是否有指定类型解释(对象对.前置介词, Data.下文引导冒号Guid))
				{
					if (判断对象的右边是否已生长完成(对象对.右对象, 右边界, true) == false)
						return -1;
				}
			}
			if (创建 == true)
			{
			}
			else
			{
				//if (处理阶段 < 1 && (对象对.中心对象.是隐藏对象() || 对象对.参数对象.是隐藏对象()))//一级隐藏对象不能在一开始就进行，先让不隐藏的对象先生长。
				//	return false;

				//同一对象，进行重复和集合处理。尤其是离合谓语和宾语，两者是同一的。
				if (((Guid)对象对.左对象.中心第一根类.模式行.B端).Equals(对象对.右对象.中心第一根类.模式行.B端))
					return 1;

				//if (处理阶段 == 0)//只进行最优先的几个常用参数的生长，首先是【符合程度】和右边对象的结合，这个最优先。
				//{
				//	//可能还需要考虑【数】作为中心，左边有另外一个参数的情况，比如【正2】【多于5个】。但也可能用代码来处理。
				//	if (Data.是派生类(Data.符合程度Guid, 对象对.参数对象.源模式行, 替代.正向替代))
				//		return true;//只要参数是符合程度，就允许。
				//	return false;
				//}
				//if (处理阶段 == 1)//【量】或者【量化概念】作为中心，和左边的【数】等的生长。【一本】等。
				//{
				//	if (对象对.中心在右 && Data.是派生类(Data.量Guid, 对象对.中心对象.源模式行, 替代.正向替代))
				//		//&& Data.是派生类(Data.量化概念Guid, 对象对.中心对象.模式行, 替代.正向替代) == false) 量化概念的左边肯定是【符合程度】，前边已经做了，这里不需要再做。
				//		return true;//只要中心是【量】或者【量化概念】符合程度，就允许。
				//	return false;
				//}
				//if (处理阶段 == 2)//【指定代词：这、那、每、第】,和右边的【数量】【量化概念】等结合生长。【这一个】【每个】等。
				//{
				//	//后边考虑【什么】等也要放置到这里，但不能只依据参数判断，【什么】作为附件可以：【什么东西】可以，而【什么是苹果】则不行。
				//	if (对象对.中心在右 &&
				//		(Data.是派生类(Data.第3指定Guid, 对象对.参数对象.源模式行, 替代.正向替代)
				//		|| Data.是派生类(Data.每Guid, 对象对.参数对象.源模式行, 替代.正向替代)
				//		|| Data.是派生类(Data.第Guid, 对象对.参数对象.源模式行, 替代.正向替代))
				//		)
				//		return true;
				//	return false;
				//}
				//小于3时可以完成事物和事物的定中式的生长。可能也要考虑动词名词化的从定（比如名词谓语）
				//}

				//bool 中心有孤立推理角色 = 有孤立的推理角色(对象对.中心对象);
				//bool 参数有孤立推理角色 = 有孤立的推理角色(对象对.参数对象);

				if (进行一些基本检查(对象对) == false)
					return -1;

				bool 中心对象序列化 = Data.能够序列化(对象对.中心对象.中心第一根类.源模式行);
				bool 参数对象序列化 = Data.能够序列化(对象对.参数对象.中心第一根类.源模式行);

				if (处理模式 == 生长_集合处理)
				{
					if (中心对象序列化 && 参数对象序列化)
						if (对象对.左对象.生长次数 == 0 && 对象对.右对象.生长次数 == 0)//这里暂时这样，只允许原始一级的动词可以，但实际上，【是不是】等其实也是可以的，后边再完善，有一些状语应该都可以，当然，可能要求有【并】等连词
							if (计算两个对象能并列(对象对.左对象, 对象对.右对象))
								return 1;//对于紧密的动词集合总是允许，由外边去具体控制
				}

				if (处理模式 == 生长_连动处理)
					if (中心对象序列化 == false || 参数对象序列化 == false)
						return -1;

				if (处理模式 == 生长_两组动词并列处理)
					if (中心对象序列化 == false && 对象对.中心对象.查找已结合的推理角色(true) == null
						|| 参数对象序列化 == false && 对象对.参数对象.查找已结合的推理角色(true) == null)
						return -1;

				return 1;
				//if (参数对象序列化 && 中心有孤立推理角色)
				//{
				//    if (参数有孤立推理角色 == false)
				//        return false;
				//    //if (处理模式 != 名词谓语处理)
				//    //	return false;
				//}

				//if (范围.处理阶段 == 0)
				//{
				//if (处理模式 == 生长_名词谓语处理)//名词谓语在第一阶段不生长，避免【他高兴地游泳】生长出了【他is高兴】。
				//{
				//    //注意：可以再优化，如果右边的名词谓语确定肯定是一个结束的动词，那么也可以允许！比如【惨重】【红】等。
				//    return 0;
				//}

				//if (中心对象序列化)//中心对象是动词
				//{
				//    if (参数端是很明显的状语(对象对) == false)//动词和非常明确的状语参数可以先生长，这些状语是构成动词的基本部分！而主语宾语则要放置在后边。
				//        return 0;
				//}
				//    else if (参数对象序列化)//参数对象是动词，中心对象不是动词，假设是从定。
				//    {
				//        //如果是定语形式对象，也就是这个对象虽然是动词，但是一般当成【从定】用！
				//        //比如【相关方面】的【相关】
				//        if (对象对.参数对象.是定语形式对象 == false || 对象对.中心在右 == false)
				//            return 0;
				//    }
				////}

				//if (范围.处理阶段 <= 1)
				//{
				//    if (对象对.中心对象.介动词等情况延后一阶段生长 || 对象对.参数对象.介动词等情况延后一阶段生长)
				//        return 0;
				//}

				//if (范围.处理阶段 <= 2)//等于3以后才可以进行动作和动作的嵌套生长，
				//{
				//推理角色和【动词与动词】处理的阶段一起做，因为本质上，【推理角色】就是【动词和动词】相互结合时候的介词
				//而且，这样保证【因为卖书的人生病了】这个句子中，【卖书的人】会直接生长，而不会出现【因为卖书】先生长的问题！
				//if (Data.是派生类(Data.推理角色Guid, 对象对.中心对象.源模式行, 替代.正向替代)
				//	|| Data.是派生类(Data.推理角色Guid, 对象对.参数对象.源模式行, 替代.正向替代))
				//	return false;
				//不好，要保证在【推导】进行之前，各个推理角色应该完全和动词结合了！否则会很混乱，所以，把这个要求屏蔽了。
				//而对于【因为卖书的人生病】的这个问题，处理的方法是【相对优先】，也就是【因为】和【卖书】结合前，会让【卖书】先向右边做一轮生长。

				if (中心对象序列化)
				{
					if (参数对象序列化)//不能是两个动作。
					{
						if (可以作为符合程度的二元关联(对象对.参数对象) == false)
							return 0;
						//本来考虑让一些紧密的动词可以在一开始进行生长，后来发现有好多问题，比如【针对流传的视频】
						//这里【针对】和【流传】紧密在一起，就先进行了生长，结果屏蔽了【流传的视频】的生长。
						//所以，后边如果还是要进行这样的优化的话，要更加严谨地进行分析好然后更细致地处理。
						//注意：考虑，如果是非常明确的状语型的就可以！
						//如果是两个紧挨在一起的动词，中间没有间隔宾语或者主语，那么可能作为连动（集合）生长。
						//bool 左动词有宾语 = 已经有了某个语言角色(对象对.左对象, 字典_语言角色.宾语);
						//if (左动词有宾语)
						//	return 0;
						//bool 右动词有主语 = 已经有了某个语言角色(对象对.右对象, 字典_语言角色.主语 | 字典_语言角色.前独 | 字典_语言角色.集主);
						//if (右动词有主语)
						//	return 0;
					}
					//if (处理阶段 <= 4 && 对象对.参数对象.begindex < 对象对.中心对象.endindex)//向右边的先生长。对动词【主谓宾】等先长右边部分可以起到优化吗？因为右边的宾语要更优先吧？
					//	return false;
				}
				//else if (处理阶段 <= 4 && Data.能够序列化(对象对.参数对象.中心第一根类.模式行))//这个阶段不允许【动词对事物】的从定，需要把动词别的参数先生长完整后。
				//	return false;

				//下边考虑优先结合动词，延后结合名词的情况。
				if (对象对.中心在右 && 中心对象序列化 == false && Data.是派生类(Data.推理角色Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代))
					return 0;
				else if (对象对.中心在右 == false && 中心对象序列化 && 参数对象序列化 == false && Data.拥有动词宾语(对象对.中心对象.中心第一根类.源模式行))
					return 0;
				//}

				//if (范围.处理阶段 <= 3)//等于4后可以进行短句停顿的生长，以及中间有逗号间隔的生长。
				//{
				if (Data.是派生类(Data.短句停顿Guid, 对象对.参数对象.模式行, 替代.正向替代))
					return 0;
				if (中间有间隔并且对连续串有拆解(范围, 对象对, 左边界, 右边界))
					return 0;
				//}

				//if (范围.处理阶段 <= 4)//等于5后可以进行句子结束的生长。
				//{
				if (Data.是派生类(Data.句子语用基类Guid, 对象对.参数对象.模式行, 替代.正向替代))
					return 0;
				//}

				//if (句子合并和深度再处理阶段 < 1)//进行句子结束的生长，但不进行多句子的合并生长。
				//{
				if (已经是完成句子(对象对.参数对象))
					return 0;
				if (已经是完成句子(对象对.中心对象) && Data.是派生类(Data.封闭范围Guid, 对象对.参数对象.源模式行, 替代.正向替代) == false)//如果参数是括号则允许。
					return 0;
				//}
			}

			return 1;
		}

		public bool 已经是完成句子(生长对象 对象)
		{
			if (已经有非封闭区间内的句号(对象))
				return true;
			return false;
		}


		public int 从一个位置向右完成生长(封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			int 本次起始序数 = 有效对象计数;

			for (int i = 在左边界排序对象中定位(左边界); i < 左边界排序对象.Count; i++)
			{
				生长对象 对象 = 左边界排序对象[i];  //发起对象发起一个生长，实际生长可能是发起对象内部的一个树枝节点，需要的话已进行所有可能的生长，对象一还可以递归进行一次深度的生长

				if (对象.begindex > 右边界)
					break;

				if (对象.endindex > 右边界)
					continue;

				int 起始序数 = 有效对象计数;
				//if (一个对象向一个方向完成生长(对象, false, false, 范围, 左边界, 右边界) > 0)
				//如果生长出了对象，并且新对象的中心转移（一般也就是右端的对象作为了中心），那么从头再来。比如【一个红色的苹果】，如果【红色的苹果】完成了，那么，中止，从头来过。
				//目的是让新产生的对象向左回溯进行生长，只是不太优化，后边再进行优化写成指定这些对象向左进行生长！
				if (从一个对象向右发起连续生长(null, 对象, /*null,*/ 范围, 左边界, 右边界, 规则) > 0)
				{
					生长对象 最左端的新对象 = 取新对象里的最左端对象(起始序数);
					if (最左端的新对象 != null)
					{
						i = 回退到左边最远的相邻对象(最左端的新对象, 左边界) - 1;
						continue;
					}
					//for (int j = 0; j < 左边界排序对象.Count; j++)
					//{
					//	生长对象 obj = 左边界排序对象[j];
					//	if (obj.处理轮数 == -2/* || obj.中心第一根类 == 对象.中心第一根类*/)
					//		continue;
					//	if (obj.有效对象序数 >= 起始序数)
					//	{
					//		i = j - 1;
					//		break;
					//	}
					//}
				}
			}

			if (最近生长对象个数(本次起始序数) == 0)
				return 0;

			return 1;
		}

		//从一个对象发起，向一个方向进行发散性的。
		//如果需要，这个对象可能递归的向右继续生长
		//没有执行生长，那么返回0
		//进行了正常的生长，那么返回1
		public int 从一个对象向右发起连续生长(关联对象对 递归生长的上级关联对象, 生长对象 发起对象, /*生长对象 固定中心, */封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			//if (发起对象.中心对象 == null)//字符串
			//	return 0;
			if (发起对象.处理轮数 == -2 || 发起对象.概率打分 <= 0)
				return 0;

			int k = 0;

			if (递归生长的上级关联对象 != null)
				递归生长的对象链.Add(递归生长的上级关联对象.对象对);

			生长对象 左边对象, 右边对象;

			左边对象 = 发起对象;

			int 位置 = 左边对象.endindex;

			for (int index = 在左边界排序对象中定位(位置); index < 左边界排序对象.Count; index++)
			{
				右边对象 = 左边界排序对象[index];
				//if (右边对象.中心对象 == null)//字符串
				//	continue;

				if (右边对象 == 左边对象)
					continue;

				if (右边对象.begindex > 右边界)
					break;

				if (右边对象.endindex > 右边界)
					continue;

				if (右边对象.处理轮数 == -2 || 右边对象.概率打分 <= 0)
					continue;

				关联对象对 中心在左对象对 = 创建或者返回可以生长的一个对象对(左边对象, 右边对象);
				关联对象对 中心在右对象对 = 创建或者返回可以生长的一个对象对(右边对象, 左边对象);
				if (中心在左对象对.对象对 == null && 中心在左对象对.处理类型 == 0 && 中心在右对象对.对象对 == null && 中心在右对象对.处理类型 == 0)//这句是判断两个对象已经相距很远，完全不相邻
					return 0;

				//预先判断，如果两个组合都会被抑制，就没有必要进行了。
				if ((中心在左对象对.对象对 == null || 计算对象被抑制性(中心在左对象对.对象对)) && (中心在右对象对.对象对 == null || 计算对象被抑制性(中心在右对象对.对象对)))
					continue;
				//{//跳到远距离的下一个，进行优化，不过看来不合适。
				//    index = 在左边界排序对象中定位(右边对象.begindex + 1);
				//    if(index==左边界排序对象.Count)
				//        return 0;
				//    index--;
				//    continue;
				//}
				k = 进行生长(递归生长的上级关联对象, 左边对象, 右边对象, 中心在左对象对, 中心在右对象对, /*固定中心*/null, 范围, 左边界, 右边界, 规则);

				if (k == 2)//生长出了新对象，但是不是给出的对象对，而是右端对象向右生长的。
					index = 在左边界排序对象中定位(位置) - 1;//根据全新的对象来重新进行。

				if (k == 1)
					goto end;
			}

		end:

			if (递归生长的上级关联对象 != null)
				递归生长的对象链.RemoveAt(递归生长的对象链.Count - 1);

			return k;

			//else
			//{
			//	右边对象 = 发起对象;

			//	int 位置 = 右边对象.begindex;

			//	for (int index = 在右边界排序对象中定位(位置); index < 右边界排序对象.Count; index++)
			//	{
			//		左边对象 = 右边界排序对象[index];
			//		//if (左边对象.中心对象 == null)
			//		//	continue;

			//		if (右边对象 == 左边对象)
			//			continue;

			//		if (左边对象.endindex < 左边界)
			//			break;

			//		if (左边对象.begindex < 左边界)
			//			continue;

			//		if (左边对象.处理轮数 == -2 || 左边对象.概率分加完成分 <= 0)
			//			continue;

			//		关联对象对 对象对1 = 创建或者返回可以生长的一个对象对(左边对象, 右边对象);
			//		关联对象对 对象对2 = 创建或者返回可以生长的一个对象对(右边对象, 左边对象);
			//		if (对象对1.对象对 == null && 对象对1.已经进行过计算 == false && 对象对2.对象对 == null && 对象对2.已经进行过计算 == false)//这句是判断两个对象已经相距很远，完全不相邻
			//			return 0;

			//		//预先判断，如果两个组合都会被抑制，就没有必要进行了。
			//		if ((对象对1.对象对 == null || 计算对象被抑制性(对象对1.对象对)) && (对象对2.对象对 == null || 计算对象被抑制性(对象对2.对象对)))
			//			continue;
			//		//{//跳到远距离的下一个，进行优化，不过看来不合适。
			//		//    index = 在左边界排序对象中定位(右边对象.begindex + 1);
			//		//    if(index==左边界排序对象.Count)
			//		//        return 0;
			//		//    index--;
			//		//    continue;
			//		//}

			//		int k1 = 进行生长(左边对象, 右边对象, 向右, 固定中心, 范围, 左边界, 右边界);

			//		if (k1 > 0)
			//			return k1;
			//		//注意！！！这里我们一旦有计算成功，就返回，不进行多个生长。
			//		//在【进行生长】中产生的对象立即加入到了全部对象里边去，序号已经乱了，如果要进行多个生长，那么要考虑这一点进行重新调整！
			//	}
			//}

			return 0;
		}

		public void 从一个对象向左发起一次生长(生长对象 发起对象, 封闭范围 范围, int 左边界, int 右边界)
		{
			//if (发起对象.处理轮数 == -2 || 发起对象.概率分加完成分 <= 0)
			//	return 0;

			//int k = 0;


			//生长对象 左边对象, 右边对象;

			//右边对象 = 发起对象;

			//int 位置 = 右边对象.begindex;

			//for (int index = 在右边界排序对象中定位(位置); index < 右边界排序对象.Count; index++)
			//{
			//	左边对象 = 右边界排序对象[index];
			//	//if (左边对象.中心对象 == null)
			//	//	continue;

			//	if (右边对象 == 左边对象)
			//		continue;

			//	if (左边对象.endindex < 左边界)
			//		break;

			//	if (左边对象.begindex < 左边界)
			//		continue;

			//	if (左边对象.处理轮数 == -2 || 左边对象.概率分加完成分 <= 0)
			//		continue;


			//	关联对象对 中心在左对象对 = 创建或者返回可以生长的一个对象对(左边对象, 右边对象);
			//	关联对象对 中心在右对象对 = 创建或者返回可以生长的一个对象对(右边对象, 左边对象);
			//	if (中心在左对象对.对象对 == null && 中心在左对象对.已经处理 == 0 && 中心在右对象对.对象对 == null && 中心在右对象对.已经处理 == 0)//这句是判断两个对象已经相距很远，完全不相邻
			//		return 0;

			//	//预先判断，如果两个组合都会被抑制，就没有必要进行了。
			//	if ((中心在左对象对.对象对 == null || 计算对象被抑制性(中心在左对象对.对象对)) && (中心在右对象对.对象对 == null || 计算对象被抑制性(中心在右对象对.对象对)))
			//		continue;
			//	//{//跳到远距离的下一个，进行优化，不过看来不合适。
			//	//    index = 在左边界排序对象中定位(右边对象.begindex + 1);
			//	//    if(index==左边界排序对象.Count)
			//	//        return 0;
			//	//    index--;
			//	//    continue;
			//	//}
			//	k = 进行生长(左边对象, 右边对象, 中心在左对象对, 中心在右对象对, 右边对象, 范围, 左边界, 右边界);


			//	if (k > 1)
			//		return k;
			//}


			//return k;
		}


		private int 进行生长(关联对象对 递归生长的上级关联对象, 生长对象 左边对象, 生长对象 右边对象, 关联对象对 中心在左对象对, 关联对象对 中心在右对象对, 生长对象 固定中心, 封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			int 本次起始序数 = 有效对象计数;

			//下边根据前边的链，计算是否不向右优先，而是回退去立即进行左边的生长。
			//这里让右边对象和左边的所有待生长去匹配，看右边对象是否更适合和左边不相邻的一个对象匹配，而不是和目前相邻的左对象
			//如果发现有，就立即返回让当前左对象先和左边去生长后再来
			if (递归生长的上级关联对象 != null)
			{
				if (中心在左对象对 != null && 中心在左对象对.对象对 != null && 中心在左对象对.对象对.生长次序打分 < 递归生长的上级关联对象.对象对.生长次序打分)
					中心在左对象对 = null;
				if (中心在右对象对 != null && 中心在右对象对.对象对 != null && 中心在右对象对.对象对.生长次序打分 < 递归生长的上级关联对象.对象对.生长次序打分)
					中心在右对象对 = null;
				if (中心在左对象对 == null && 中心在右对象对 == null)
					return 0;
			}

			int k = 进行生长1(左边对象, 右边对象, 中心在左对象对, 中心在右对象对, 固定中心, 范围, 左边界, 右边界, 规则);

			整理本轮结果并加入到完成集合();

			return k;
		}

		private bool 根据形式判断直接就是连动句(生长对象 对象对)
		{
			Data.Assert(对象对.中心在右 == true);

			生长对象 介词 = 对象对.后置介词;

			if (介词 != null && 是横向关联介词(介词))
				return true;

			return false;

		}

		private bool 初步判断是动词从定(生长对象 对象对)
		{
			if (对象对 == null || 对象对.中心在右 == false)
				return false;

			if (Data.能够序列化(对象对.左对象.中心第一根类.源模式行) == false)
				return false;

			if (Data.能够序列化(对象对.右对象.中心第一根类.源模式行) == true)
				return false;

			if (对象对.中间的和地 != null && 对象对.中间的和地.是的或者地(true, false) == true)
				return true;

			return false;
		}

		private bool 动词并列处理情况下要求右边先生长完全(生长对象 对象对)
		{
			bool 计算出的中心在右 = false;
			模式 推导关联 = 计算两个动词的推导关联(对象对.左对象, 对象对.右对象, ref 计算出的中心在右);
			if (推导关联 != null && Data.松散并列Guid.Equals(推导关联.ID) == false && 计算出的中心在右 == false)
				return true;
			return false;

		}

		//两个对象应该是相邻的
		//没有执行生长，那么返回0
		//进行了正常的生长，那么返回1
		private int 进行生长1(生长对象 左边对象, 生长对象 右边对象, 关联对象对 中心在左对象对, 关联对象对 中心在右对象对, 生长对象 固定中心, 封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			Data.输出串(Data.计数器++.ToString() + "[" + 左边对象.取子串 + "]--[" + 右边对象.取子串 + "]");

			if (左边对象.中心对象 != null && 左边对象.取子串 == "因遭逼近"
				&& 右边对象.中心对象 != null && 右边对象.取子串 == "所以示警")
				左边界 = 左边界;

			int k = 0;
			bool 左边优先作为中心 = false;
			bool 左对象序列化 = Data.能够序列化(左边对象.中心第一根类.模式行);
			bool 右对象序列化 = Data.能够序列化(右边对象.中心第一根类.模式行);


			//这点是让右边的对象向右边进行深度生长，尽量完成的情况。
			//需要右边参数是一个动词，比如【说】【表示】等，这个时候，让右边的内容先进行生长，以及【推理角色】和动词的结合，其实都是如此！
			//右边如果是名词，那么在前边的处理阶段应该已经完成了，所以，这种要求右边先生长的情况是左边是一个动词，右边也是动词，但是左边的动词右边的动词先再向右扩展生长。
			//注意，还应该考虑如果左边对象的参数已经完成！那么就不需要右边先生长吧。
			if (右对象序列化)
			{
				if (中心在右对象对.对象对 != null && (参数端是很明显的状语(中心在右对象对.对象对)//主要考虑右端有紧密的动词集合。比如【从未进口并使用】，让【进口并使用】先生长。
					|| 可以作为符合程度的二元关联(中心在右对象对.对象对.参数对象)))//【是他进口并使用】
				{
					if (允许这种类型的生长或创建(中心在右对象对.对象对, false, 范围, 生长_正常处理, 左边界, 右边界, 规则) == 1)
						if (从一个对象向右发起连续生长(中心在右对象对, 右边对象, /*null,*/ 范围, 左边对象.endindex, 右边界, 生长_集合处理) > 0)
							//if (从一个位置向右完成生长(范围, 左边对象.endindex, 右边界, 生长_集合处理) > 0)
							return 2;
				}
				else if (中心在右对象对.对象对 != null && Data.是派生类(Data.推理角色Guid, 左边对象.源模式行, 替代.正向替代))
				{
					//推理角色和动词结合前，要让动词先尽可能完成生长
					//，可能只剩下【他因为喜欢音乐】这种情况下，【推理角色】结合后，还可能在前边结合【前状】和【主语】等。
					if (允许这种类型的生长或创建(中心在右对象对.对象对, false, 范围, 生长_正常处理, 左边界, 右边界, 规则) == 1)
						if (从一个对象向右发起连续生长(中心在右对象对, 右边对象, /*null,*/范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							//if (从一个位置向右完成生长(范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							return 2;
				}
				else if (中心在左对象对.对象对 != null && 左对象序列化 && Data.正常处理情况下要求右边先生长完全(左边对象.中心第一根类.源模式行))
				{
					if (允许这种类型的生长或创建(中心在左对象对.对象对, false, 范围, 生长_正常处理, 左边界, 右边界, 规则) == 1)
						if (从一个对象向右发起连续生长(中心在左对象对, 右边对象, /*null,*/范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							//if (从一个位置向右完成生长(范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							return 2;
				}
				else if (中心在左对象对.对象对 != null && 左对象序列化 && 动词并列处理情况下要求右边先生长完全(中心在左对象对.对象对))
				{
					if (允许这种类型的生长或创建(中心在左对象对.对象对, false, 范围, 生长_两组动词并列处理, 左边界, 右边界, 规则) == 1)
						if (从一个对象向右发起连续生长(中心在左对象对, 右边对象,/*null,*/范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							//if (从一个位置向右完成生长(范围, 左边对象.endindex, 右边界, 生长_全部) > 0)
							return 2;
				}
			}

			//这里处理一些特殊的优先生长。
			if (左对象序列化)//左对象是可序列化的。
			{
				左边优先作为中心 = true;//先假设优先尝试以左端为中心生长。

				if (初步判断是动词从定(中心在右对象对.对象对))
				{
					左边优先作为中心 = false;
					//if (允许这种类型的生长或创建(中心在右对象对.对象对, false, 范围, 正常处理, 左边界, 右边界) == 1)
					//	if (一个动词不改变中心向左完成连续生长(左边对象,范围, 左边界, 右边界) > 0)
					//		return 2;
				}
				else if (右对象序列化)//两个动词遇到一起的处理。
				{
					//if (Data.拥有动词主语(右边对象.中心第一根类.源模式行))
					if (中心在右对象对.对象对 != null && 根据形式判断直接就是连动句(中心在右对象对.对象对))
						左边优先作为中心 = false;
				}
			}

			//下边进行普通正常的生长
			if (左边优先作为中心)
			{
				k = 组织对象对进行生长(中心在左对象对, true, 范围, 左边界, 右边界, 规则);
				if (k > 0 && 左对象序列化 == false && 右对象序列化 == true)//左边是动词右边是名词时，也就是从定则不返回，要执行下边的从定运算。
					return 1;//有结果就返回？不，要判断结果的打分概率

				if (固定中心 == null)
				{
					//优先尝试右边名词词组将进行拆解的进行拆解性的生长。
					if (左对象序列化 == true || 右对象序列化 == false)
						if (进行左动右名的正常语序和从定的多义性处理(左边对象, 右边对象, 范围, 左边界, 右边界, 规则) > 0)
							return 1;

					//如果是动词-名词组合而上边拆解性处理不合适那么就把后边当成一个整体进行生长。
					//或者是动词-动词组合，那么也直接在这里进行生长。
					if (组织对象对进行生长(中心在右对象对, true, 范围, 左边界, 右边界, 规则) > 0)
						return 1;
				}

			}
			else
			{
				if (固定中心 == null)
				{
					if ((k = 组织对象对进行生长(中心在右对象对, true, 范围, 左边界, 右边界, 规则)) > 0)
						return 1;//有结果就返回？不，要判断结果的打分概率
				}

				if (组织对象对进行生长(中心在左对象对, true, 范围, 左边界, 右边界, 规则) > 0)
					return 1;
			}

			return k;
		}


		public 生长对象 已知关联构造待分析对象对(生长对象 上端对象, 生长对象 下端对象, 参数树结构 关联, bool 反向, int that)
		{
			生长对象 o = (反向 ^ 关联.生成树时的路径起始端 == 字典_目标限定.A端) ? 未知关联构造待分析对象对(上端对象, 下端对象) : 未知关联构造待分析对象对(下端对象, 上端对象);
			if (o == null)
				return null;
			o.设置源模式行(关联.目标);
			o.that = that;
			return o;
		}
		//有形式的对象肯定已经通过字符位置判断了，所以只需要计算无形式空对象。
		//无形式空对象是不占据位置的，所以要进行计算。
		public bool 有重复的无形式空对象(生长对象 obj1, 生长对象 obj2)
		{
			List<生长对象> 无形式对象集合 = new List<生长对象>();
			if (obj1.关联总数 > obj2.关联总数)//优化，让小的对象放前边。
			{
				生长对象 o = obj1;
				obj1 = obj2;
				obj2 = o;
			}
			obj1.递归找出所有的无形式空对象(无形式对象集合);
			if (无形式对象集合.Count() == 0)
				return false;
			return obj2.包含有结果中的无形式空对象(无形式对象集合);
		}

		public void 登记执行计算(关联对象对 关联对象对, int 处理模式)
		{
			关联对象对.处理类型 |= 处理模式;
			//if (处理模式==名词谓语处理)
			//{
			//	关联对象对.已经处理 |= 名词谓语处理;
			//	关联对象对.中心对象.介动词等情况延后一阶段生长 = false;
			//}
			//else if(处理模式 == 正常处理)
			//{
			//	关联对象对.已经处理 |= 正常处理;
			//	关联对象对.中心对象.介动词等情况延后一阶段生长 = false;
			//}
			//else
			//{
			//	关联对象对.已经处理 |= 动词集合处理;
			//	关联对象对.中心对象.介动词等情况延后一阶段生长 = false;
			//}

			//if (关联对象对.处理类型 == (生长_名词谓语处理 | 生长_正常处理 | 生长_集合处理 | 生长_两组动词并列处理 | 生长_连动处理))
			//    关联对象对.对象对 = null;//释放掉
		}

		public void 计算生长次序打分(生长对象 对象对)
		{
			//这里暂时取左边对象的【生长靠外层级】，因为一般都是动词在左边才考虑，而名词的这个值通常设置为0。
			//当然，可能重新设置下，让这个对象靠左和靠右的值可以不同。
			对象对.生长次序打分 -= 对象对.左对象.中心第一根类.参数.在左端时靠外层级分;
			对象对.生长次序打分 -= 对象对中间的间隔值(对象对);//增加一个间隔，次序打分减1。

			//【因遭遇逼近而示警】，这里，在【逼近和而示警】相遇的时候，因为【而示警】和【因遭遇】的【而】和【因】是匹配的，所以就返回，让【因遭遇逼近】先生长
			//【而】和【因】其实相当于一对括号！
			//不过，看起来可以简化了，其实，就是后边遇到推理角色就可以返回！所以改成目前代码
			//foreach (生长对象 前边的待生长对象 in 递归生长的对象链)
			//{
			//}
			生长对象 右推理角色 = 对象对.右对象.查找已结合的推理角色(true);
			生长对象 左推理角色 = 对象对.左对象.查找已结合的推理角色(true);

			if (右推理角色 != null)
			{
				if (左推理角色 != null)//这里可能要检查下两个推理角色的配对性！
					对象对.生长次序打分 += 5;
				else
					对象对.生长次序打分 -= 2;
			}

		}

		//相邻，并且没有生长过。
		public 关联对象对 创建或者返回可以生长的一个对象对(生长对象 中心对象, 生长对象 参数对象)
		{

			foreach (关联对象对 obj in 待生长对象对集合)
				if (obj.中心对象 == 中心对象 && obj.参数对象 == 参数对象)
					return obj;

			生长对象 对象对 = 未知关联构造待分析对象对(中心对象, 参数对象);

			if (对象对 != null)
				只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(对象对);

			关联对象对 关联对 = new 关联对象对(中心对象, 参数对象, 对象对 != null && 加上所有部件后对象对是相邻的(对象对) ? 对象对 : null);
			//remove by wuyougang
			//if (关联对.对象对 != null)
			//    计算生长次序打分(关联对.对象对);
			//待生长对象对集合.Add(关联对);
			//remove end

			return 关联对;
		}

		public 生长对象 未知关联构造待分析对象对(生长对象 中心, 生长对象 参数/*, bool 执行位置重叠检查 = true*/)
		{

			int b = 中心.begindex < 参数.begindex ? 中心.begindex : 参数.begindex;
			int e = 中心.endindex > 参数.endindex ? 中心.endindex : 参数.endindex;
			int v = 计算跨越范围的冲突值(b, e);
			if (v >= 9)
				return null;

			//Data.Assert(生长对象.计算位置重叠性(中心, 参数) <= 0);

			生长对象 o = new 生长对象();

			if (中心.begindex == 参数.begindex)
				o.中心在右 = 中心.endindex > 参数.endindex;
			else
				o.中心在右 = 中心.begindex > 参数.begindex;

			if (o.中心在右 == true)
			{
				o.左对象 = 参数;
				o.右对象 = 中心;
			}
			else
			{
				o.左对象 = 中心;
				o.右对象 = 参数;
			}

			if (有重复的无形式空对象(中心, 参数))
				return null;

			if (是否完整的短句(中心, 内部范围) || 是否完整的短句(参数, 内部范围))
			{
				o.参数.概率分 = 中心.参数.概率分 > 参数.概率打分 ? 参数.概率打分 : 中心.参数.概率分;
			}
			else
				o.参数.概率分 = Data.合并概率打分(中心.参数.概率分, 参数.概率打分);

			//if (o.参数对象.是介词或者串(true, true, true))
			//    o.参数.概率分 -= 3;

			if (o.参数.概率分 <= 0)
				return null;
			//o.处理阶段 = Processor.当前处理器.处理阶段;

			o.根据内部对象完成范围设置();

			return o;
		}
		public bool 是哨兵对象(生长对象 对象, bool 计算左边)
		{
			if (对象 == 起始哨兵 && 计算左边)
				return true;
			if (对象 == 结束哨兵 && 计算左边 == false)
				return true;
			return false;
		}

		public bool 是哨兵对象或者封闭区域边界(生长对象 对象, bool 计算左边)
		{
			if (对象 == 起始哨兵 && 计算左边)
				return true;
			if (对象 == 结束哨兵 && 计算左边 == false)
				return true;
			return 内部范围.递归计算是封闭范围的边界(对象, 计算左边);
		}

		public bool 是哨兵对象或者封闭区域边界或者逗号(生长对象 对象, bool 计算左边)
		{
			if (是哨兵对象或者封闭区域边界(对象, 计算左边))
				return true;

			if (Data.是派生类(Data.短句停顿Guid, 对象.源模式行, 替代.正向替代))
				return true;

			return false;
		}

		public int 对象对中间的间隔值(生长对象 对象对)
		{
			int k = 0;
			int i = 在左边界排序对象中定位(对象对.左对象.endindex);

			生长对象 obj;

			while ((obj = 左边界排序对象[i]).begindex < 对象对.右对象.begindex)
			{
				if (obj.endindex <= 对象对.右对象.begindex)
				{
					if (Data.是派生类(Data.短句停顿Guid, obj.源模式行, 替代.正向替代))
						k++;
				}
				i++;
			}
			return k;
		}

		public bool 处理逗号等封闭区间右端优先生长(生长对象 对象对, 封闭范围 范围, int 左边界, int 右边界)
		{
			if (对象对中间的间隔值(对象对) == 0)
				return false;

			int k = 在左边界排序对象中定位(对象对.右对象.endindex);
			if (是哨兵对象或者封闭区域边界或者逗号(左边界排序对象[k], false))
				return false;

			int r = 从一个位置向右完成生长(范围, 对象对.右对象.begindex, 右边界, 生长_全部);//这个右边界其实应该限定到下一个逗号的位置。
			if (r > 0)
				return true;
			return false;
		}

		//对于【红】等，其实可以在右边有【得】等【后定】。
		public bool 能够有后定(生长对象 对象)
		{
			if (Data.能够作为名词谓语(对象.中心第一根类.模式行))
			{
				return true;
			}
			return false;

		}

		public bool 要求右端优先生长(生长对象 对象对)
		{
			//对于【他借她】，在进行【他借】之前，让【借她】先进行生长。

			//暂时先屏蔽了。因为对于【从定】来说，很讨厌，比如【他借给她的书】，如果【他借】不生长而先生长了【借给她的书】
			//这个时候，中心已经是【书】，而无法把【借】生长进去了，要在从定前先长【他】，处理起来特别复杂，所以，就把这屏蔽了。
			//对于小品词来说，其实也不需要，我们就把后边的宾语挂在主动词上好了！比如【骂死了她】【骂死的她】其实都没有问题。

			return false;

			if (对象对.中心在右 && 是哨兵对象或者封闭区域边界(对象对.左对象, true) == false)
			{

				if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行))
				{
					return true;
				}
				if (能够有后定(对象对.中心对象))
				{
					return true;
				}
			}
			else
			{
				//能作为小品词的动词，自身肯定不能再作为嵌套动词。
				//对于【打死她】，在进行【打死】之前，让【死她】先进行生长。
				if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) &&
						Data.能够做动词小品词(对象对.参数对象.中心第一根类.源模式行) > 0)
					return true;
			}


			return false;
		}

		//根据当前对象对，判断是否要让右端对象先进行更右端的参数的生长
		//如果没有进行，那么返回false
		//如果进行了，那么返回true，实际完成一些向更右端生成的对象。
		public bool 不改变中心向右端优先生长(生长对象 对象对, 封闭范围 范围, int 左边界, int 右边界, bool 向右)
		{
			////if (处理阶段 <= 3)
			////	return false;
			//int 本次起始序数 = 有效对象计数;

			//一个对象不改变中心向右完成连续生长(对象对.右对象, 对象对.中心对象, 范围, 左边界, 右边界, true);

			//if (最近生长对象个数(本次起始序数) > 0)
			//{
			//	List<生长对象> 新对象集合 = 最近生长对象(本次起始序数);
			//	foreach (生长对象 新对象 in 新对象集合)
			//	{
			//		if (新对象.中心第一根类 == 对象对.中心对象.中心第一根类)
			//			//if (计算对象被抑制性(对象对.参数对象, 新对象, true, true) == false)
			//			组织对象对进行生长(新对象, 对象对.参数对象, false, 范围, 左边界, 右边界);
			//	}
			//	//if (被已有对象抑制(对象对.右对象, 0) == false)
			//	//	完成对象对所有可能的生长(对象对, 左边界, 右边界);
			//	return true;// 最近生长对象个数(e) > 0;
			//}
			//if (向右 == false)
			//{

			//	if (初步判断是动词从定(对象对) == false)
			//		return false;

			//	一个对象不改变中心向一个方向完成连续生长(对象对.左对象, 向右, 对象对.左对象, 范围, 左边界, 右边界);

			//	if (最近生长对象个数(本次起始序数) > 0)
			//	{
			//		List<生长对象> 新对象集合 = 最近生长对象(本次起始序数);
			//		foreach (生长对象 新对象 in 新对象集合)
			//		{
			//			if (新对象.中心第一根类 == 对象对.参数对象.中心第一根类)
			//				//if (计算对象被抑制性(对象对.参数对象, 新对象, true, true) == false)
			//				组织对象对进行生长(对象对.中心对象, 新对象, false, 范围, 左边界, 右边界);
			//		}
			//		//if (被已有对象抑制(对象对.右对象, 0) == false)
			//		//	完成对象对所有可能的生长(对象对, 左边界, 右边界);
			//		return true;// 最近生长对象个数(e) > 0;
			//	}
			//}

			//else if (对象对.中心在右 == false && Data.能够序列化(对象对.左对象.中心第一根类.模式行))
			//{
			//}

			return false;
		}

		public void 一个对象不改变中心向右完成连续生长(生长对象 对象, 生长对象 固定中心, 封闭范围 范围, int 左边界, int 右边界, bool 进入了右端优先)
		{

			//Data.Assert(固定中心 != null);

			//int 本次起始序数 = 有效对象计数;

			//if (从一个对象向右发起一次生长(对象, 固定中心, 范围, 左边界, 右边界) > 0)
			//{
			//	List<生长对象> 新对象集合 = 最近生长对象(本次起始序数);
			//	foreach (生长对象 新对象 in 新对象集合)
			//	{
			//		if (新对象.处理轮数 != -2 && 新对象.中心第一根类 == 固定中心.中心第一根类)
			//			一个对象不改变中心向右完成连续生长(新对象, 固定中心, 范围, 左边界, 右边界, 进入了右端优先);
			//	}
			//}
		}

		public void 一个动词不改变中心向左完成连续生长(生长对象 对象, 封闭范围 范围, int 左边界, int 右边界)
		{
			//int 本次起始序数 = 有效对象计数;
			//int k = 0;

			//if (从一个对象向左发起一次生长(对象,  范围, 左边界, 右边界) > 0)
			//{
			//	k = 1;
			//	List<生长对象> 新对象集合 = 最近生长对象(本次起始序数);
			//	foreach (生长对象 新对象 in 新对象集合)
			//	{
			//		if (新对象.处理轮数 != -2 && 新对象.中心第一根类 == 对象.中心第一根类)
			//			一个动词不改变中心向左完成连续生长(新对象, 范围, 左边界, 右边界);
			//	}
			//}
			//return k;
		}

		public bool 进行一些基本检查(生长对象 对象对)
		{
			//想优化下，在这里判断从句的【的】，但不行，因为这个位置还不知道是否就是从定！

			//if (对象对.中心在右 && (对象对.中间的和地 == null || 对象对.中间的和地.是的或者地(true, false) == false))
			//{
			//    if (对象对.从句必须带显式的())
			//        return false;

			//    //这个不能决定就是失败，所以去除
			//    //if (对象对.允许从句不带显式的() == false)
			//    //    return false;
			//}

			if (对象对.完成性检查() == false)
				return false;

			return true;
		}

		//没有执行生长，那么返回0
		//进行了正常的生长，那么返回1
		//如果不到处理阶段，是否返回别的？
		public int 组织对象对进行生长(关联对象对 关联对象, bool 允许右端优先生长, 封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			生长对象 对象对 = 关联对象.对象对;

			int k = 0;

			if (对象对 == null)
				return 0;

			Data.Assert(对象对.参数对象 != 对象对.中心对象);

			if (对象对.中心对象.概率分 <= 0 || 对象对.参数对象.概率打分 <= 0)
				return 0;

			//连续生长这个问题已经不重要了，现在是递归生长。
			//if (连续生长开始轮数 < 处理轮数)//在一个连续生长循环中，两个对象至少要有一个是在本轮循环中的
			//	if (中心对象.处理轮数 < 连续生长开始轮数 && 参数对象.处理轮数 < 连续生长开始轮数)
			//		return 0;

			//介词和串不可能发起生长。介词和串会进行一对一二元关联生长，而是等待别的二元关联生长的时候作为附属成分加入
			//是介词形式创建的对象相当于是封闭的【值对象】，不会作为中心进行处理。
			if (Data.是介词或者串(对象对.中心对象.中心第一根类.模式行, true, true, true) || 对象对.中心对象.是介词形式创建的对象)
			{
				if (是哨兵对象或者封闭区域边界(对象对.中心对象, false) == false)//对于右边的【中心区域，是可以的】
					return 0;
			}

			//if ((指定中心 == null && 假设中心.层级 == -2) || (指定中心 != null && 指定中心 != 假设中心))
			//	continue;
			if (对象对.参数对象.是介词或者串(true, true, false) && 对象对.参数对象.取子串 != "被")
			{
				return 0;
				//if (obj.是介词或者串(true, false, false))
				//{
				//    if (前置介词 != null)
				//    {
				//        //重复的前置介词。
				//    }
				//    前置介词 = obj;
				//    位置 = 前置介词.endindex;
				//    index = 在左边界排序对象中定位(位置 )-1;//下一个位置。因为下次要加1，所以这里先回退一个。
				//    continue;
				//}
				//else//后置介词，在此不处理。
				//    continue;
			}
			//至少有一个是上一轮生成的对象才会发起生长。如果是指定位置生长（不是-1），因为是复活以前所有的对象，所以就不考虑这个了。
			//if (/*指定位置 == -1 && */假设中心.层级 < 处理轮数 - 1 && 假设参数.层级 < 处理轮数 - 1)
			//    continue;
			//测试点
			//if (假设中心.取子串 == "]" && 假设参数.取子串 == "他")
			//    本次起始序数 = 本次起始序数;


			if (已进行过计算(对象对.中心对象, 对象对.参数对象, 生长_集合处理) == false)
				if ((k = 组织对象对进行生长1(关联对象, 允许右端优先生长, 范围, 左边界, 右边界, 生长_集合处理, 规则)) > 0)
					return k;

			if (已进行过计算(对象对.中心对象, 对象对.参数对象, 生长_正常处理) == false)
				if ((k = 组织对象对进行生长1(关联对象, 允许右端优先生长, 范围, 左边界, 右边界, 生长_正常处理, 规则)) > 0)
					return k;

			if (已进行过计算(对象对.中心对象, 对象对.参数对象, 生长_名词谓语处理) == false)
				if ((k = 组织对象对进行生长1(关联对象, 允许右端优先生长, 范围, 左边界, 右边界, 生长_名词谓语处理, 规则)) > 0)
					return k;

			if (已进行过计算(对象对.中心对象, 对象对.参数对象, 生长_连动处理) == false)
				if ((k = 组织对象对进行生长1(关联对象, 允许右端优先生长, 范围, 左边界, 右边界, 生长_连动处理, 规则)) > 0)
					return k;

			if (已进行过计算(对象对.中心对象, 对象对.参数对象, 生长_两组动词并列处理) == false)
				if ((k = 组织对象对进行生长1(关联对象, 允许右端优先生长, 范围, 左边界, 右边界, 生长_两组动词并列处理, 规则)) > 0)
					return k;

			return 0;
		}

		public bool 判断对象完成性(生长对象 对象对, int 处理模式)
		{
			if (对象对.参数对象.完成分 < 5)
				return false;

			//没有终结的对象不能作为参数！不过，到了最后阶段则可以强制进行。
			if (处理模式 != 生长_两组动词并列处理)
			{
				if (对象对.参数对象.查找已结合的推理角色(true) != null)
					return false;

				//这个情况其实可以不考虑了！【动词】即使没有完成，也可以和【推理角色】进行生长，因为没有改变这个【动词】的中心词的地位！
				//if (Data.是派生类(Data.推理角色Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代) && 对象已经完成可以作为参数(对象对.中心对象) == false)
				//	return false;
			}

			return true;
		}

		public int 组织对象对进行生长1(关联对象对 关联对象对, bool 允许右端优先生长, 封闭范围 范围, int 左边界, int 右边界, int 处理模式, int 规则, bool 进入了右端优先 = false)
		{

			生长对象 对象对 = 关联对象对.对象对;

			//if (范围.处理阶段 <= 4 && 判断对象完成性(对象对, 处理模式) == false)
			//    return 0;

			if (关联对象对.中心对象.概率分 <= 0 || 关联对象对.参数对象.概率打分 <= 0)
				return 0;

			int 允许 = 允许这种类型的生长或创建(对象对, false, 范围, 处理模式, 左边界, 右边界, 规则);
			if (允许 <= 0)
			{
				if (允许 == -1)
					登记执行计算(关联对象对, 处理模式);
				return 0;
			}

			//if (计算对象被抑制性(对象对))
			//    return 0;

			//采取处理阶段判断的方式进行了处理，所以，以下代码暂时不用了。
			//if (处理逗号等封闭区间右端优先生长(对象对, 范围, 左边界, 右边界))
			//	return 1;

			//注意：可能【跳跃】地向远处尝试，解决中间间隔的新知识的问题！
			//比如【一本书】，如果有了【一本】，就不会有【本书】。
			//if (判断对已有对象进行了部分拆解(对象对.中心对象) != null 
			//    || 判断对已有对象进行了部分拆解(对象对.参数对象) != null)//和以前已经生长的对象冲突，那么就抑制。
			//    return true;

			int 本次起始序数 = 有效对象计数;


			//if (进入了右端优先 == true && 初步判断是动词从定(对象对))
			//    return 0;

			//if (处理模式 == 正常处理 && 允许右端优先生长 && 要求右端优先生长(对象对))
			//{
			//	//这句现在实际不会执行，因为【要求右端优先生长】方法不会满足。
			//	if (不改变中心向右端优先生长(对象对, 范围, 左边界, 右边界, true))
			//		return 1;
			//}

			if (处理模式 == 生长_集合处理)
				尝试进行两对象横向关联生长(对象对, 生长_集合处理, 范围);
			else if (处理模式 == 生长_正常处理)
				完成对象对所有可能的生长(对象对, 范围, 左边界, 右边界);
			else if (处理模式 == 生长_名词谓语处理)//进行名词谓语生长
			{
				//if (名词谓语关联 != null)
				//{
				//	//【苹果红】。
				//	//应该先判断下名词谓语关联是否合适。
				//	尝试进行名词谓语生长(对象对, 对象对.中心对象, 对象对.参数对象, 名词谓语关联, null, 对象对.中心在右);
				//}
				//else
				//{
				//	//【红了】【红着】【正在红】【苹果昨天红】【苹果迅速地红】
				//	尝试进行名词谓语生长(对象对, 对象对.中心对象, null, null, 对象对.参数对象, 对象对.中心在右);
				//}

				//注意，上边注释的代码是别处拷贝来的，实现如果能计算出两个对象的【名词谓语关联】，那么就能给出更具体的中间关联，而现在，就是使用【基本关联】。
				//应该把一些分析代码，都放置到【尝试进行谓语生长】里边去！
				尝试进行名词谓语生长(对象对, 对象对.中心在右, 右边界);

			}
			else if (处理模式 == 生长_连动处理)//进行横向生长
			{
				尝试进行两对象横向关联生长(对象对, 生长_连动处理, 范围);
			}

			else if (处理模式 == 生长_两组动词并列处理)//进行横向生长
			{
				尝试进行两对象横向关联生长(对象对, 生长_两组动词并列处理, 范围);
			}

			if (最近生长对象个数(本次起始序数) == 0)
			{
				//以下是对封闭范围里边的【省略一半】的对象进行强制处理的方法
				//比如【他（和她游泳）】

				生长对象 介词 = 对象对.中心在右 ? 对象对.后置介词 : 对象对.前置介词;
				if (介词 == null)
					介词 = 对象对.中间的和地;
				if (介词 == null)
					return 0;
				if (内部范围.递归计算是封闭范围的边界(对象对.左对象, true))
				{
					生长对象 this对象 = 加入结果集排除掉相同的(创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", 对象对.左对象.endindex, 对象对.左对象.endindex, null, 0));
					this对象.参数.概率分 = 9;

					关联对象对 关联对象 = 创建或者返回可以生长的一个对象对(对象对.中心在右 ? 对象对.中心对象 : this对象, 对象对.中心在右 ? this对象 : 对象对.参数对象);

					组织对象对进行生长(关联对象, false, 范围, 左边界, 右边界, 规则);
				}
				else if (内部范围.递归计算是封闭范围的边界(对象对.右对象, false))
				{
					生长对象 this对象 = 加入结果集排除掉相同的(创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", 对象对.右对象.begindex, 对象对.右对象.begindex, null, 0));
					this对象.参数.概率分 = 9;

					关联对象对 关联对象 = 创建或者返回可以生长的一个对象对(对象对.中心在右 ? this对象 : 对象对.中心对象, 对象对.中心在右 ? 对象对.参数对象 : this对象);

					组织对象对进行生长(关联对象, false, 范围, 左边界, 右边界, 规则);
				}
			}

			if (最近生长对象个数(本次起始序数) > 0)
				return 1;

			return 0;
		}


		public void 完成对象对所有可能的生长(生长对象 对象对, 封闭范围 范围, int 左边界, int 右边界)
		{
			//string str = "          [" + 对象对.中心对象.取子串 + "]--[" + 对象对.参数对象.取子串 + "]";
			//Data.输出串(str);

			if (对象对.左对象.取子串 == "吃了" && 对象对.右对象.取子串 == "那么多" /*&& 对象对.中心在右==true*/)
				有效对象计数 = 有效对象计数;

			if (有效对象计数 == 8)
				有效对象计数 = 有效对象计数;

			Data.Assert(对象对.中心对象.概率分 > 0 || 对象对.参数对象.概率打分 > 0);

			int 本次起始序数 = 有效对象计数;

			if (是哨兵对象(对象对.左对象, true) || 是哨兵对象(对象对.右对象, false))
			{
				生长对象 介词 = 对象对.中心在右 ? 对象对.后置介词 : 对象对.前置介词;
				if (介词 == null)
					介词 = 对象对.中间的和地;
				if (介词 == null)
					return;
			}
			int 要忽略的对象数量 = 0;
			//参数对象右边只有“的”，比如“美丽的、对的”，而且紧挨短句停顿，需进行特殊处理让“美丽的”先生长完成
			if (!对象对.中心在右 && Data.能够序列化(对象对.左对象.中心第一根类.源模式行) && 判断对象的右边是否紧挨的和短句间隔(对象对.右对象, 右边界))
			{
				生长对象 空对象 = 创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", 对象对.右对象.endindex + 1, 对象对.右对象.endindex + 1, null, 0);
				生长对象 临时对象对 = 已知关联构造待分析对象对(空对象, 对象对.右对象, new 参数树结构(Data.FindRowByID(Data.基本关联Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
				只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(临时对象对);
				if (加上所有部件后对象对是相邻的(临时对象对))
				{
					临时对象对 = 直接一级关联生长(临时对象对, -2, true);
					if (临时对象对 != null)
					{
						要忽略的对象数量++;
						对象对.右对象 = 临时对象对;
						对象对.根据内部对象完成范围设置();
					}
				}
			}
			//进行参数预处理(对象对, 范围);
			完成对象对所有纵向关联生长(对象对/*, 语义阀值*/);

			//考虑省略的生长。
			if (最近生长对象个数(本次起始序数) - 要忽略的对象数量 == 0)
			{
				//如果处理阶段大于3，并且正常生长没有成功，那么考虑上省略对象再次进行一次生长
				//【省略】【无语义关联由语言角色和介词决定】【代表】和【新对象】都在这里处理。尤其是【无语义关联由语言角色和介词决定】也是在这里！
				尝试进行补生长或者省略对象及强制关联生长(对象对, 左边界, 右边界/*, 语义阀值*/);
			}
		//尝试进行再处理(对象对, 范围);
		//正常关联计算不够强势，那么就同时也进行下隐藏中心的横向关联生长。
		//对于推导也是如此，如果两个动作靠拢很近，在前边直接进行了动作的组合，那么就轮不到进行推导了。

			//注意，对这里的处理也要增加尝试可能有省略的情况！

		End:
			//整理本轮结果并加入到完成集合();
			;

		}
		public void 进行参数预处理(生长对象 对象对, 封闭范围 范围)
		{
			//动词“到”的处理，变相为路径和时间段
			if (Data.是派生类(Data.到达Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代))
			{
				int 空事物对象位置 = 对象对.参数对象.中心第一根类.begindex;
				模式 需要的对象类型;
				模式 参数关联;
				生长对象 需要的对象;
				生长对象 临时对象对;
				需要的对象类型 = Data.FindRowByID(Data.空间路径Guid);

				需要的对象 = 创建或返回隐藏对象(需要的对象类型.ID, 需要的对象类型.形式, 空事物对象位置, 空事物对象位置, null, -2, false);
				if (需要的对象 == null)
					return;
				需要的对象.集合对象的基对象 = 需要的对象类型;
				List<参数> 参数列表 = 对象对.参数对象.得到指定根对象的参数表();
				foreach (参数 o in 参数列表)
				{
					if (o.已经派生())
					{
						if (o.源关联记录.ID.Equals(Data.到达拥有终点Guid))
						{
							临时对象对 = 已知关联构造待分析对象对(需要的对象, o.对端派生对象.参数对象.中心对象, new 参数树结构(Data.FindRowByID(Data.空间路径拥有终点Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
							只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(临时对象对);
							if (加上所有部件后对象对是相邻的(临时对象对))
								需要的对象 = 直接一级关联生长(临时对象对, -2, false);
						}
						else if (o.源关联记录.ID.Equals(Data.到达拥有起点Guid))
						{
							临时对象对 = 已知关联构造待分析对象对(需要的对象, o.对端派生对象.参数对象.中心对象, new 参数树结构(Data.FindRowByID(Data.空间路径拥有起点Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
							只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(临时对象对);
							if (加上所有部件后对象对是相邻的(临时对象对))
								需要的对象 = 直接一级关联生长(临时对象对, -2, false);
						}
					}
				}
				if (需要的对象.生长次数 >= 对象对.参数对象.生长次数)
				{
					对象对.参数对象 = 需要的对象;
					对象对.根据内部对象完成范围设置();
					加入结果集排除掉相同的(需要的对象).参数.概率分 = 9;//这里先这么设定，实际上，这是需要进行语义计算来得到的。
				}
			}

		}
		public void 整理本轮结果并加入到完成集合()
		{
			//结果集是都计算了，但是可以对结果集进行排序处理！把一些竞争不合适的去除！

			对本轮结果集进行打分抑制处理();
			foreach (生长对象 o in 本轮结果集)
			{
				if (o.处理轮数 != -2)
					Data.输出串("                                     生成：【" + o.取子串 + "】");

				//o.处理阶段 = 阶段;
				加入一个对象到池(o);
				//if (是一个强势完成对象(o))
				//    强势完成对象集合.Add(o);
			}

			本轮结果集.Clear();

		}

		static public bool 是横向关联介词(生长对象 介词)
		{
			if (介词 == null)
				return false;
			string str = 介词.取子串;
			if (str == "和" || str == "同" || str == "与" || str == "并且" || str == "并" || str == "且" || str == "与" || str == "及"
				|| str == "、" || str == "或" || str == "或者")
				return true;
			return false;
		}

		public bool 语义分析可以进行隐式集合生长(模式 基类, 生长对象 对象对)
		{
			if (基类.ID.Equals(对象对.左对象.中心第一根类.源模式行ID) || 基类.ID.Equals(对象对.右对象.中心第一根类.源模式行ID))
				return false;//两个对象中的一个等于基类，也就是说其中一个是另一个的明显的派生类，而不是并列的。
			return true;
		}

		public 推理角色 查找推理角色(模式 源模式)
		{
			foreach (推理角色 推理角色 in Data.推理角色集合)
				if (推理角色.目标 == 源模式)
					return 推理角色;
			return null;
		}

		public 模式 计算两个动词的推导关联(生长对象 左对象, 生长对象 右对象, ref bool 中心在右)
		{
			中心在右 = true;
			推理角色 左边计算推理角色 = null;
			推理角色 右边计算推理角色 = null;
			推导 左边计算推导 = null;
			推导 右边计算推导 = null;
			生长对象 左边 = 左对象.查找已结合的推理角色(true);
			生长对象 右边 = 右对象.查找已结合的推理角色(true);
			if (左边 != null)
			{
				左边计算推理角色 = 查找推理角色(左边.中心第一根类.源模式行);
				左边计算推导 = 左边计算推理角色.推导;
			}
			else if (右边 != null)
			{
				右边计算推理角色 = 查找推理角色(右边.中心第一根类.源模式行);
				右边计算推导 = 右边计算推理角色.推导;
			}

			if (左边计算推导 != null && 右边计算推导 != null)
			{
				if (左边计算推导 != 右边计算推导)//两边计算的推导如果不匹配，那么返回失败。
					return null;
				if (左边计算推理角色.作为中心 == 右边计算推理角色.作为中心)//两个推理角色应该是一个作为中心，一个作为参数，否则，也就失败。
					return null;
			}

			if (左边计算推导 != null)
			{
				if (左边计算推理角色.作为中心)
					中心在右 = false;
				return 左边计算推导.目标;
			}
			else if (右边计算推导 != null)
			{
				if (右边计算推理角色.作为中心 == false)
					中心在右 = false;
				return 右边计算推导.目标;
			}

			return Data.FindRowByID(Data.松散并列Guid);
		}

		public void 尝试进行两对象横向关联生长(生长对象 对象对, int 处理模式, 封闭范围 范围)
		{
			if (对象对.参数对象.是介词或者串(true, true, true))//暂时不允许串进行横向关联生长，以后要考虑，主要是新对象？
				return;

			//如果一端是空对象，那么就必须要有显式的横向关联介词才能触发。

			生长对象 介词 = 对象对.中心在右 ? 对象对.后置介词 : 对象对.前置介词;
			if (介词 == null)
			{
				if (是哨兵对象或者封闭区域边界(对象对.左对象, true) || 是哨兵对象或者封闭区域边界(对象对.右对象, false))
					return;
			}
			else if (计算是否可以做为集合处理(对象对) == false && !是否短句停顿符介词(介词)) //是横向关联介词(介词) == false
				return;

			//int 形式集合生长的可能性 = 0;
			//进行叶子层面的横向关联创建生长。也有一些集合可能在正常的予以关联中进行，比如动词的聚合等。

			//现在，暂时去掉这种推导型的处理，一般并不以推导为中心。
			//一、首先要尝试推导型的生长，【如果****，那么****】，生长的时候，【如果】和【那么】已经被事件聚合了，所以没有中间参数。
			//if (尝试进行两对象推导关联生长(对象对))//如果推导型关联生长成功，那么就不再继续了。
			//	return;
			//中心和参数都已结合项目编号时，直接进行集合生长
			if (对象对.左对象.查找已结合的某个类型对象(Data.实体反聚项目编号Guid) != null &&
				对象对.右对象.查找已结合的某个类型对象(Data.实体反聚项目编号Guid) != null &&
				对象对.中心在右 == false)
			{
				生长对象 集合对象 = 未知关联构造待分析对象对(对象对.左对象, 对象对.右对象);
				if (集合对象 != null)
				{
					三级关联 右关联 = new 三级关联(Data.FindRowByID(Data.并列集合拥有后续元素Guid));
					只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(集合对象);
					集合对象 = 直接三级关联生长(集合对象, 处理轮数, 右关联, false);
				}
				return;
			}
			bool 左对象序列化 = 对象对.左对象.扩展计算对象能序列化();
			bool 右对象序列化 = 对象对.右对象.扩展计算对象能序列化();

			if (处理模式 == 生长_两组动词并列处理)//两组动词推导处理。
			{
				bool 计算出的中心在右 = false;
				模式 推导关联 = 计算两个动词的推导关联(对象对.左对象, 对象对.右对象, ref 计算出的中心在右);

				if (推导关联 != null && 计算出的中心在右 == 对象对.中心在右)
				{
					if (判断对象的左边是否已生长完成(对象对.左对象, 范围.begindex) && 判断对象的右边是否已生长完成(对象对.右对象, 范围.endindex))
						对象对.设置源模式行(推导关联);
					else if (Data.是派生类(Data.松散并列Guid, 推导关联, 替代.正向替代) && 介词 == null
						&& (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false
							|| 对象对.左对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false))
						return;
					对象对.设置源模式行(推导关联);
					对象对 = 直接一级关联生长(对象对, 处理轮数, true);
				}
				return;
			}

			if (左对象序列化 != 右对象序列化)//要么是名词集合，要么是动词集合。
				return;

			if (处理模式 == 生长_连动处理)//连动处理。
			{
				if (对象对.中心在右 == false)
					return;

				生长对象 左边推理角色 = 对象对.左对象.查找已结合的推理角色(false);
				生长对象 右边推理角色 = 对象对.右对象.查找已结合的推理角色(false);
				//如果已经结合了推理角色，就不能生长连动了！
				if (左边推理角色 != null || 右边推理角色 != null)
					return;

				if (可以作为连动(对象对))
				{
					对象对.设置源模式行(Data.FindRowByID(Data.拥有连动Guid));
					对象对 = 直接一级关联生长(对象对, 处理轮数, true);
				}
				return;
			}

			if (左对象序列化 && 右对象序列化)//动词和动词的集合处理。【紧密集合】或者连动。
			{
				//动词和动词的处理。
				//对于动词和动词，不采用集合，两边都可能作为中心，要根据具体情况而定。
				//连动本质上其实也是一种集合，但是这里要把一个动词作为中心，前边的动词作为状语。
				if (对象对.中心在右 == false)
					return;


				生长对象 左边推理角色 = 对象对.左对象.查找已结合的推理角色(false);
				生长对象 右边推理角色 = 对象对.右对象.查找已结合的推理角色(false);
				//如果已经结合了推理角色，就不能生长动词集合了！
				if (左边推理角色 != null || 右边推理角色 != null)
					return;

				if (计算两个对象能并列(对象对.左对象, 对象对.右对象))
				{
					if (介词 == null && (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false
									 || 对象对.左对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false))
						return;
					对象对.设置源模式行(Data.FindRowByID(Data.动词拥有紧密并列动词Guid));
					对象对 = 直接一级关联生长(对象对, 处理轮数, true);
				}

				return;
			}

			//名词和名词的处理。
			//对于名词和名词要求生成集合，集合的中心对象左边。
			if (对象对.中心在右)
				return;
			//二、然后进行普通集合的生长
			//if (对象对.中心在右==false)
			//{
			//if (Data.是派生类(Data.动作Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代))

			if (对象对.左对象.集合对象的基对象 == null && 对象对.右对象.集合对象的基对象 != null)//集合从左向右增加元素。但要注意，以后要改进，如果是表达式而右边有括号的时候，那么就不一样！比如3+（2+5）。右边先是集合了！
				return;

			并列关联 关联 = 计算两元素的共同基类(对象对.左对象, 对象对.右对象);

			//if (关联 != null && 关联.总距离 > 0 || (是横向关联介词(介词)))
			//{
			//    if (介词 == null)
			//    {
			//        if (对象对中间的间隔值(对象对) > 0)//没有集合介词，而中间有逗号等间隔时，不进行集合生长！
			//            return;
			//        if (语义分析可以进行隐式集合生长(关联.基类, 对象对) == false)//没有介词，但也没有间隔的时候，分析两个对象是否很明显的可以进行集合生长。
			//            return;
			//    }
			//    else if (是横向关联介词(介词) == false && !是否短句停顿符介词(介词))
			//        return;
			if (计算是否可以做为集合处理(对象对))
			{
				//{
				//	if (语义分析可以进行隐式集合生长(关联.基类, 对象对) == false)
				//		return;
				//}
				三级关联 左参数关联 = new 三级关联(Data.FindRowByID(Data.抽象形式集合拥有第一元素Guid));
				三级关联 右参数关联 = new 三级关联(Data.FindRowByID(Data.并列集合拥有后续元素Guid));
				if (Data.是派生类(Data.时间量Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
				   || Data.是派生类(Data.数Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代))
					右参数关联 = new 三级关联(Data.FindRowByID(Data.范围集合拥有后续元素Guid));

				//如果介词是数学表达式需要的介词，则做为表达式集合进行处理
				if (递归判断是对象要求的介词形式(Data.FindRowByID(Data.表达式拥有后续元素Guid), 介词))
				{
					//根据介词确定右关联、左关联
					模式 右关联 = 根据介词形式获取指定关联所派生的具体关联(Data.FindRowByID(Data.表达式拥有后续元素Guid), 介词);
					if (右关联 != null)
					{
						右参数关联 = new 三级关联(右关联);
						模式 左关联 = 根据A端获取指定关联所派生的具体关联(右关联.A端, Data.FindRowByID(Data.表达式拥有第一元素Guid));
						if (左关联 != null)
							左参数关联 = new 三级关联(左关联);
						else
							左参数关联 = new 三级关联(Data.FindRowByID(Data.表达式拥有第一元素Guid));

						尝试进行两元素抽象集合生长(对象对, 右关联.A端, Data.FindRowByID(右关联.A端).形式, 对象对.左对象.endindex, 左参数关联, 右参数关联);
						return;
					}
					else
						return;
				}
				//modi by wuyougang
				//并列集合直接以参数的形式挂接在第一个对象上
				//尝试进行两元素抽象集合生长(对象对, Data.并列集合Guid, "[并列集合]", 对象对.左对象.endindex, 左参数关联, 右参数关联);

				生长对象 集合对象 = 未知关联构造待分析对象对(对象对.左对象, 对象对.右对象);
				if (集合对象 != null)
				{
					只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(集合对象);
					集合对象 = 直接三级关联生长(集合对象, 处理轮数, 右参数关联, true);
				}
				//modi end
			}
		}
		public int 获取层级概念的层级值(生长对象 层级对象)
		{
			模式 row = 层级对象.源模式行;
			if (row.参数.层级值 > 0)
				return row.参数.层级值;
			else
			{
				while (row != null)
				{
					row = Data.FindRowByID(row.源记录);
					if (Data.NullGuid.Equals(row.ID) || Data.概念Guid.Equals(row.ID) || Data.基本关联Guid.Equals(row.ID))
						break;
					if (row != null && row.参数.层级值 > 0)
						return row.参数.层级值;
				}
			}
			return 0;
		}
		public bool 计算是否可以做为集合处理(生长对象 对象对, bool 内部检查概念属拥短句 = true)
		{
			if (对象对 == null)
				return false;
			生长对象 介词 = 对象对.中心在右 ? 对象对.后置介词 : 对象对.前置介词;
			if (介词 == null)
				介词 = 对象对.中间的和地;
			//集合与[等]
			if (对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.并列集合拥有后续元素Guid)) != null && Data.是派生类(Data.集合省略Guid, 对象对.右对象.源模式行, 替代.正向替代))
				return true;
			并列关联 关联 = 计算两元素的共同基类(对象对.左对象, 对象对.右对象);

			if (关联 != null && 关联.总距离 > 0 || (是横向关联介词(介词)))//&& 判断对象右边是否紧挨短句停顿符(对象对.右对象,当前内部范围,true)))
			{
				if (介词 == null)
				{
					if (Data.是派生类(Data.层级概念Guid, 关联.基类, 替代.正向替代)) //是层级概念
					{
						if (Math.Abs(获取层级概念的层级值(对象对.左对象) - 获取层级概念的层级值(对象对.右对象)) < 10)
							return true;
					}
					//无介词时，左边不是书名，右边是书名，肯定不是集合 
					if (根据本身是书名的阈值(对象对.左对象) <= 0 && 根据本身是书名的阈值(对象对.右对象) > 0)
						return false;
					//无介词时，左边是代词，右边不是词时，肯定不是集合
					if (Data.是派生类(Data.代词Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && !Data.是派生类(Data.代词Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					//无介词时，左右两边都是地点，肯定不是集合
					if (Data.是派生类(Data.地点Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.地点Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					//无介词时，左右两边都是时间量，肯定不是集合
					if (Data.是派生类(Data.时间量Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.时间量Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					//无介词时，左右两边都是数，肯定不是集合
					if (Data.是派生类(Data.数Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					//无介词时，右边是人角色，左边不是人角色，肯定不是集合
					if (!Data.是派生类(Data.人角色Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.人角色Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					//无介词时，左右都是方向，肯定不是集合
					if (Data.是派生类(Data.方向Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.方向Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
						return false;
					if (对象对中间的间隔值(对象对) > 0 || (内部检查概念属拥短句 && 对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid)) != null))//没有集合介词，而中间有逗号等间隔时，不进行集合生长！
						return false;
					if (语义分析可以进行隐式集合生长(关联.基类, 对象对) == false)//没有介词，但也没有间隔的时候，分析两个对象是否很明显的可以进行集合生长。
						return false;
				}
				else if (是横向关联介词(介词) == false)
				{
					if (Data.是派生类(Data.地点Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
								&& Data.是派生类(Data.地点Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代) && 是否短句停顿符介词(介词))
						return true;
					else if (Data.是派生类(Data.度量Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
						&& Data.是派生类(Data.度量Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
					{
						if (递归判断是对象要求的介词形式(Data.FindRowByID(Data.表达式拥有后续元素Guid), 介词))
							return true;
						else
						{
							if (Data.是派生类(Data.数Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
										&& Data.是派生类(Data.数Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
							{
								if (是否短句停顿符介词(介词) || 是对象要求的介词形式(Data.FindRowByID(Data.范围集合拥有后续元素Guid), 介词))
									return true;
								else
									return false;
							}
							else if (Data.是派生类(Data.时间量Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
								&& Data.是派生类(Data.时间量Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
							{
								if (是对象要求的介词形式(Data.FindRowByID(Data.范围集合拥有后续元素Guid), 介词))
									return true;
								else
									return false;
							}
						}
					}
					else
						return false;
				}
				return true;
			}
			else if (关联 != null)
			{
				if (Data.是派生类(Data.层级概念Guid, 关联.基类, 替代.正向替代)) //是层级概念
				{
					if (Math.Abs(获取层级概念的层级值(对象对.左对象) - 获取层级概念的层级值(对象对.右对象)) < 10)
						return true;
				}
				if (Data.是派生类(Data.度量Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代)
						&& Data.是派生类(Data.度量Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
				{
					if (递归判断是对象要求的介词形式(Data.FindRowByID(Data.表达式拥有后续元素Guid), 介词))
						return true;
				}
			}
			return false;
		}

		public Guid 计算具体的推导类型(模式 推理关联1, 模式 推理关联2)
		{
			//实际可能要根据两个对象的不同【推理角色】来进行判断是哪种具体的推导类型。
			return Data.推导即命题间关系Guid;
		}

		public bool 尝试进行两对象推导关联生长(生长对象 对象对)
		{
			if (对象对.中心在右)//隐藏的推导对象的位置我们要求总是创建在左边，这样统一。
				return false;
			if (Data.是派生类(Data.事件Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) == false)
				return false;
			if (Data.是派生类(Data.事件Guid, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代) == false)
				return false;
			参数 左对象推理 = 对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.事件反聚推理角色Guid));
			参数 右对象推理 = 对象对.右对象.查找已经实现的参数(Data.FindRowByID(Data.事件反聚推理角色Guid));
			if (左对象推理 == null && 右对象推理 == null)
				return false;

			Guid 具体推导类型 = 计算具体的推导类型(左对象推理 == null ? null : 左对象推理.源关联记录, 右对象推理 == null ? null : 右对象推理.源关联记录);
			三级关联 左参数关联 = new 三级关联(Data.FindRowByID(Data.推导拥有推理角色Guid), null, Data.FindRowByID(Data.事件反聚推理角色Guid));
			三级关联 右参数关联 = new 三级关联(Data.FindRowByID(Data.推导拥有推理角色Guid), null, Data.FindRowByID(Data.事件反聚推理角色Guid));

			尝试进行两元素抽象集合生长(对象对, 具体推导类型, "[推导]", 对象对.begindex, 左参数关联, 右参数关联);

			return false;
		}


		public bool 已经被作为参数生长(生长对象 obj, bool 作为中心 = true, bool 作为参数 = true)
		{
			foreach (生长对象 已有对象 in 本轮结果集)
				if (obj != 已有对象 && 已有对象.递归查找参数对象(obj) != null)
					return true;
			foreach (生长对象 已有对象 in 全部对象)
				if (obj != 已有对象 && 已有对象.递归查找参数对象(obj) != null)
					return true;
			return false;
		}

		public 生长对象 取新对象里的最左端对象(int 起始序数)
		{
			for (int j = 0; j < 左边界排序对象.Count; j++)
			{
				生长对象 obj = 左边界排序对象[j];
				if (obj.处理轮数 == -2 || obj.概率打分 <= 0)
					continue;
				if (obj.有效对象序数 >= 起始序数)
					return obj;
			}
			return null;
		}

		public int 回退到左边最远的相邻对象(生长对象 最左端的新对象, int 左边界)
		{
			Data.Assert(最左端的新对象.begindex >= 左边界);
			return 在左边界排序对象中定位(左边界);

			//for (int i = 在右边界排序对象中定位(最左端的新对象.begindex); i < 右边界排序对象.Count; i++)
			//{
			//    生长对象 左边对象 = 右边界排序对象[i];
			//    关联对象对 对象对1 = 创建或者返回可以生长的一个对象对(左边对象, 最左端的新对象);
			//    关联对象对 对象对2 = 创建或者返回可以生长的一个对象对(最左端的新对象, 左边对象);
			//    if (对象对1.对象对 == null && 对象对1.已经进行过计算 == false && 对象对2.对象对 == null && 对象对2.已经进行过计算 == false)//这句是判断两个对象已经相距很远，完全不相邻
			//        return 在左边界排序对象中定位(左边对象.begindex);
			//}
			return 0;
		}

		public void 循环处理所有对象(封闭范围 范围, int 左边界, int 右边界/*int 语义阀值, 生长对象 指定中心 = null, 生长对象 指定参数 = null, int 指定位置 = -1*/)
		{
			远程线索调整概率();//这个是跨越封闭范围的。

			if (处理轮数 <= 1)
				return;

			int 本次起始序数 = 有效对象计数;

			if (范围.子范围 != null)
			{
				foreach (封闭范围 子范围 in 范围.子范围)
					if (子范围.已经完成所有可能生长 == false)
						循环处理所有对象(子范围, 子范围.begindex, 子范围.endindex);
				if (最近生长对象个数(本次起始序数) > 0)//内部子范围生成了新对象，也就是没有结束，那么外部范围就不生长。
					return;
			}

		完成阶段处理:

			//动作的正常语序结束后，在动作和事物的从定生长前，先判断名词谓语的情况如果还有满足名词谓语的没有被别的地方吸收，那么就创建。
			//if (处理阶段 == 6)
			//	foreach (生长对象 名词谓语 in 左边界排序对象/*全部对象*/)
			//	{
			//		if (名词谓语.是介词或者串(true, true, true))
			//			continue;
			//		if (已经被作为参数生长(名词谓语))
			//			continue;
			//		//if (判断对已有对象进行了部分拆解(名词谓语) != null)
			//		//    continue;
			//		生长对象 隐含关联对象 = 尝试进行名词谓语生长(名词谓语, null, null);//启动名词谓语生长。
			//		//这里可以改进下，尝试和左边的对象能直接进行具体的隐含关联计算而不是泛泛的【基本关联】。
			//	}

			本次起始序数 = 有效对象计数;


			从一个位置向右完成生长(范围, 左边界, 右边界, 生长_全部);


			if (最近生长对象个数(本次起始序数) == 0)
			{
				//暂时还是不能如此，这样，会把所有被抑制的对象都放出来
				//还是应该选择性地计算后进行释放才好！
				//生长对象 obj=null;
				//while ((obj = 取出一个被抑制对象()) != null)
				//{
				//    组织对象进行生长(obj);
				//    if (最近生长对象的打分(本次起始序数) > 0)
				//        break;
				//}
			}

			if (最近生长对象个数(本次起始序数) == 0)//一个阶段处理没有结果，就自动进入下一个阶段。
			{
				//范围.处理阶段++;
				//if (范围.处理阶段 < 6)//这个是为了调试方便，一个阶段处理没有结果，就自动进入下一个阶段。
				//    goto 完成阶段处理;
			}

			//if (范围.处理阶段 >= 6)//后续的处理阶段再增加其实已经没有影响了，这是为了显示上好看到是哪个阶段。
			//    范围.处理阶段++;
		}


		public 封闭范围 匹配左边封闭括号(string 右边字串, bool 深度匹配)
		{
			封闭范围 范围 = 当前内部范围;
			while (范围 != null)
			{
				生长对象 obj = 范围.左括号对象;
				if (obj != null)
				{
					List<参数> 参数表 = obj.得到指定根对象的参数表();
					foreach (参数 o in 参数表)
						if (右边字串 == o.源关联记录.形式 && Data.二级关联类型(o.源关联记录).Equals(Data.拥有后置附件Guid))
							return 范围;
				}
				if (深度匹配 == false)
					return null;
				范围 = 范围.父范围;
			}
			return null;
		}

		public void 完成一个封闭范围(封闭范围 范围对象, 生长对象 右边界对象)
		{
			范围对象.endindex = 右边界对象.endindex;
			int k = 在左边界排序对象中定位(右边界对象.begindex);
			while (true)
			{
				生长对象 o = 左边界排序对象[k];
				if (o.是介词或者串(true, true, true) && o.取子串 == 右边界对象.取子串)
					break;
				k++;
			}
			范围对象.右边界对象 = 左边界排序对象[k];
			当前内部范围 = 范围对象.父范围;
		}

		public void 压栈一个封闭范围(封闭范围 范围)
		{
			if (当前内部范围.子范围 == null)
				当前内部范围.子范围 = new List<封闭范围>();
			范围.父范围 = 当前内部范围;
			当前内部范围.子范围.Add(范围);
			当前内部范围 = 范围;
		}
		public bool 是否已被其它对象吸收(生长对象 源对象)
		{
			int i = 0;//在右边界排序对象中定位(源对象.endindex);
			for (int j = i; j < 右边界排序对象.Count; j++)
			{
				生长对象 obj = 右边界排序对象[j];
				if (obj == 源对象)
					continue;
				if (obj.begindex == 源对象.begindex && obj.endindex == 源对象.endindex)
					continue;
				//if (obj.endindex < 源对象.begindex)
				//    break;
				if (obj.begindex <= 源对象.begindex && obj.endindex >= 源对象.endindex)
					return true;
			}
			return false;
		}
		//不能只用字符串，而是要结合一级对象来处理，因为一些封闭字符可能要被一级语义对象吸收掉！
		//括号型考虑最大深度吧？比如4级嵌套？如果超过，就比较强行的进行合并。
		public void 组织封闭范围()
		{
			//对括号型封闭范围进行处理
			for (int i = 0; i < 左边界排序对象.Count; i++)
			{
				生长对象 obj = 左边界排序对象[i];
				if (obj.长度 == 0)
					continue;
				if (obj.是介词或者串(true, true, true) == false)//是语义对象
				{
					if (Data.是派生类(Data.封闭范围Guid, obj.源模式行, 替代.正向替代) && 是否已被其它对象吸收(obj) == false)
					{
						封闭范围 左边对象 = 匹配左边封闭括号(obj.取子串, false);//这种情况下只匹配最近一个，比如英语的双引号，单引号等相同的符号。
						if (左边对象 != null)
							完成一个封闭范围(左边对象, obj);
						else
							压栈一个封闭范围(new 封闭范围(obj));
						i = 在左边界排序对象中定位(obj.endindex) - 1;

					}
				}
				else//是字符串
				{
					封闭范围 左边对象 = 匹配左边封闭括号(obj.取子串, true);//英语的双引号，单引号等相同的符号在上边处理了，这里遇到的肯定是右括号等单向的，所以一定要封闭。
					if (左边对象 != null)
					{
						完成一个封闭范围(左边对象, obj);
						i = 在左边界排序对象中定位(obj.endindex) - 1;
					}
				}
			}

			内部范围.删除没有完成的括号范围();

			//对分隔型符号比如【，】【。】进行封闭范围处理。
			在一个括号封闭范围中增加间隔型范围(内部范围, 内部范围.内在begindex);

			调整所有范围(内部范围);
		}

		public void 调整所有范围(封闭范围 范围)
		{
			List<封闭范围> 范围列表 = 范围.子范围;
			if (范围列表 == null)
				return;
		再来:
			for (int i = 0; i < 范围列表.Count; i++)
				for (int j = 0; j < 范围列表.Count; j++)
				{
					if (j == i)
						continue;
					if (范围列表[i].begindex >= 范围列表[j].begindex && 范围列表[i].endindex <= 范围列表[j].endindex)
					{
						范围列表[j].加入子范围(范围列表[i]);
						范围列表.RemoveAt(i);
						goto 再来;
					}
					else if (范围列表[j].begindex >= 范围列表[i].begindex && 范围列表[j].endindex <= 范围列表[i].endindex)
					{
						范围列表[i].加入子范围(范围列表[j]);
						范围列表.RemoveAt(j);
						goto 再来;
					}
				}

			for (int i = 0; i < 范围列表.Count; i++)
				调整所有范围(范围列表[i]);
		}


		public 封闭范围 进入了一个子范围(封闭范围 范围, 生长对象 左端对象)
		{
			if (范围.子范围 == null)
				return null;
			foreach (封闭范围 obj in 范围.子范围)
				if (左端对象 == obj.左括号对象)
					return obj;
			return null;
		}



		public void 在一个括号封闭范围中增加间隔型范围(封闭范围 范围, int 起始位置)
		{

			List<封闭范围> 句号子范围 = new List<封闭范围>();
			int k = 在左边界排序对象中定位(起始位置);
			while (k < 左边界排序对象.Count)
			{
				生长对象 obj = 左边界排序对象[k];
				if (obj.begindex > 范围.endindex)
					goto 结束;
				if (obj.endindex > 范围.endindex)
				{
					k++;
					continue;
				}

				封闭范围 下级范围 = 进入了一个子范围(范围, obj);
				if (下级范围 != null)
				{
					在一个括号封闭范围中增加间隔型范围(下级范围, 下级范围.内在begindex);
					//范围.子范围.Remove(下级范围);
					//新的子范围.Add(下级范围);
					k = 在左边界排序对象中定位(下级范围.endindex);
					continue;
				}
				else
				{
					if (Data.是派生类(Data.句子语用基类Guid, obj.源模式行, 替代.正向替代) && 是否已被其它对象吸收(obj) == false)//句号
					{
						句号子范围.Add(new 封闭范围(起始位置, obj.endindex, 封闭范围.计算间隔型的封闭值(obj.源模式行)));
						起始位置 = obj.endindex;
						k = 在左边界排序对象中定位(obj.endindex);
						continue;
					}
				}
				k++;
			}

		结束:
			if (句号子范围.Count > 0)
			{
				if (起始位置 != 范围.内在endindex)
					句号子范围.Add(new 封闭范围(起始位置, 范围.内在endindex, 9));

				if (范围.子范围 == null)
					范围.子范围 = new List<封闭范围>();
				foreach (封闭范围 obj in 句号子范围)
					范围.子范围.Add(obj);
			}

		}

		public int 计算跨越范围的冲突值(int begindex, int endindex)
		{
			return 内部范围.递归计算串跨越范围的冲突值(begindex, endindex, 0);
		}

		//触发对象创建可能也可以考虑根据【处理阶段】来判断。
		public void 触发对象创建()
		{
			for (int i = 0; i < 全部对象.Count; i++)
			{
				生长对象 触发创建的参数对象 = 全部对象[i];
				if (已进行关联创建对象集合.Contains(触发创建的参数对象))//已经对这个对象进行创建。
					continue;
				//if (本阶段允许这种类型的生长或创建(触发创建的参数对象, true) == false)
				//	continue;
				已进行关联创建对象集合.Add(触发创建的参数对象);
				执行创建上级对象或派生对象(触发创建的参数对象);
			}

			foreach (生长对象 o in 本轮结果集)
				根据对象的关键参数字符串线索调整概率并设置介动词的优先性(o);

			整理本轮结果并加入到完成集合();
		}

		//这里的判断不是很正确。不能用这么简单的判断。
		public bool 可以结束所有单句生长()
		{
			if (处理轮数 > 0 && 得到第一个完成对象() != null)//已经完成了一个对象且不是首轮一次匹配成功的字符串。
				return true;
			return false;
		}

		//如果有一个成功，那么就返回真。
		public bool 进行后阶段再处理()
		{
			//在前边已经对但句子生长完成的基础上进行后阶段再处理。
			//包括和【隐藏对象】的再合并。比如【‘路径’到】，【‘推导’他如果去北京】等这些情况的处理。
			//被省略等间隔开的两个句子的合并等。
			return false;
		}

		public bool 包含了哨兵对象(生长对象 对象对)
		{
			if (对象对.中心对象 == 起始哨兵 || 对象对.中心对象 == 结束哨兵)
				return true;
			if (对象对.参数对象 == 起始哨兵 || 对象对.中心对象 == 结束哨兵)
				return true;
			return false;
		}

		public void 创建哨兵对象()
		{
			起始哨兵 = 创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", 0, 0, null, 0);
			加入一个对象到池(起始哨兵);

			结束哨兵 = 创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", Data.当前句子串.Length, Data.当前句子串.Length, null, 0);
			加入一个对象到池(结束哨兵);
		}

		public void 推迟一些被介词或者名词抑制的动词的激活时间()
		{
			//【得到】这个动词用【得】表示的首先就要推迟。
			在当前范围中查找介词以及同名动词("得");

			foreach (生长对象 obj in 全部对象)
			{
				if (obj.是隐藏对象())
					continue;

				if (obj.是介词或者串(true, true, true) || Data.能够序列化(obj.中心第一根类.源模式行))
					continue;

				//注意，这个方法可能不严谨，因为没有什么类型，可能会抑制掉很多可以优先生长的动词。
				//如果需要改进的话，最好的办法是直接给【动词】设置一个【参数】，表示是否应该推迟！
				在当前范围中查找介词以及同名动词(obj.取子串);

			}
		}
		//判断指定对象是否完整的短句
		public bool 是否完整的短句(生长对象 对象, 封闭范围 范围)
		{
			if (对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid)) != null)
			{
				return true;
			}
			if (判断对象的左边是否已生长完成(对象, 范围.begindex) && 判断对象的右边是否已生长完成(对象, 范围.endindex))
				return true;

			return false;
		}
		//判断指定对象是否已经被生长为一个完整的短句
		public bool 是否被完整的短句抑制(生长对象 原对象)
		{
			if (原对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid)) != null)
				return false;
			foreach (生长对象 已有对象 in 全部对象)
			{
				if (已有对象 != 原对象)
				{
					if (已有对象.begindex <= 原对象.begindex && 已有对象.endindex >= 原对象.endindex)
					{
						if (已有对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid)) != null && 已有对象.取消抑制性 == false)
						{
							//List<参数> 概念参数表 = 已有对象.得到指定根对象的参数表();
							//foreach (参数 o in 概念参数表)
							//{
							//    if (o.已经派生() == false)
							//        continue;
							//    //相邻的参数如果是[事物或事件]则认为不允许抢用，认为抑制
							//    if (o.对端派生对象==已有对象)
							//    {
							//if (Data.是派生类(Data.事物概念Guid, 原对象.中心第一根类.源模式行, 替代.正向替代) == true
							//            || Data.是派生类(Data.事件Guid, 原对象.中心第一根类.源模式行, 替代.正向替代) == true)
							return true;
							//    }
							//}
						}
					}
				}
			}
			return false;
		}
		//public 参数 获取已吸收的最右端的参数(生长对象 原对象)
		//{
		//    List<参数> 参数表 = 原对象.得到指定根对象的参数表(原对象.中心第一根类);
		//    foreach (参数 param in 参数表)
		//    {
		//        if (param.已经派生() && param.对端派生对象.endindex == 原对象.endindex)
		//        {
		//            if (param.源关联记录.ID.Equals(Data.拥有连动Guid))
		//                return param;
		//            else if (param.对端派生对象.中心对象.endindex == 原对象.endindex)
		//                return param.对端派生对象.中心对象;
		//            else if (param.对端派生对象.参数对象 != null && Data.能够序列化(param.对端派生对象.参数对象.中心第一根类.模式行))
		//                return 获取已吸收的最右端的参数(param.对端派生对象.参数对象);
		//            else if (param.对端派生对象.参数对象 != null && param.对端派生对象.参数对象.endindex == 原对象.endindex)
		//                return param.对端派生对象.参数对象;
		//        }
		//    }
		//    return null;
		//}
		public 生长对象 获取已吸收的最右端的参数对象(生长对象 原对象)
		{
			List<参数> 参数表 = 原对象.得到指定根对象的参数表(原对象.中心第一根类);
			foreach (参数 param in 参数表)
			{
				if (param.已经派生() && param.对端派生对象.endindex == 原对象.endindex)
				{
					if (param.源关联记录.ID.Equals(Data.拥有连动Guid))
						return 获取已吸收的最右端的参数对象(param.对端派生对象);
					else if (param.对端派生对象.中心对象 != null && param.对端派生对象.中心对象.endindex == 原对象.endindex)
						return param.对端派生对象.中心对象;
					else if (param.对端派生对象.参数对象 != null && param.对端派生对象.参数对象.endindex == 原对象.endindex)
						return param.对端派生对象.参数对象;
					else
						return 原对象.中心第一根类;
				}
			}
			return null;
		}
		//主要用于忽略一些带[的]的形容词，如：美丽的……
		private 生长对象 向右忽略一些外围对象(int begindex)
		{
			生长对象 边界对象 = null; ;
			int i = 在左边界排序对象中定位(begindex);
			do
			{
				if (i >= 左边界排序对象.Count)
					break;
				生长对象 相邻对象 = 左边界排序对象[i];
				if (相邻对象.中心第一根类 == 相邻对象) //只看一级对象
				{
					if (相邻对象.是的或者地(true, false))
						边界对象 = 相邻对象;
					else if (Data.是派生类(Data.定性Guid, 相邻对象.中心第一根类.模式行, 替代.正向替代))
					{
						i = 在左边界排序对象中定位(相邻对象.endindex);
						continue;
					}
					else
						break;
				}
				i++;
			} while (true);
			return 边界对象;
		}
		private 新词汇 根据右端对象生成新词汇对象(生长对象 原对象, 生长对象 右端对象, 封闭范围 范围)
		{
			生长对象 介词 = null;
			string 新词汇_串 = "";
			int 新词汇_begindex = 0;
			int 偏移量 = 0;
			新词汇 词汇对象 = new 新词汇();
			//先检查右边紧邻对象是否介词
			if (右端对象 != null)
			{
				int i = 在左边界排序对象中定位(右端对象.begindex);
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					//必须是介词(一级对象)
					if (相邻对象.中心第一根类 == 相邻对象 && Data.是介词或者串(相邻对象.中心第一根类.模式行, true, true, false))
					{
						if (相邻对象.begindex == 右端对象.begindex && 相邻对象.endindex == 右端对象.endindex)
						{
							介词 = 相邻对象;
							break;
						}
						if (相邻对象.begindex < 右端对象.begindex || 相邻对象.endindex > 右端对象.endindex) //介词必须不被右端对象所包含
						{
							if (介词 == null || 介词.长度 < 相邻对象.长度)
								介词 = 相邻对象;
						}
					}
					if (相邻对象.endindex >= 范围.内在endindex) //必须是当前范围内
						break;
					if (相邻对象.begindex >= 右端对象.endindex) //必须是相邻的
						break;
				}
			}
			生长对象 忽略后边界对象 = 右端对象 == null ? null : 向右忽略一些外围对象(右端对象.endindex);
			if (右端对象 == null)
			{
				新词汇_串 = 源语言文字[范围.内在begindex].ToString();
				新词汇_begindex = 范围.内在begindex;
				偏移量 = 新词汇_串.Length;
			}
			else if (忽略后边界对象 != null)
			{
				新词汇_串 = 获取右边相邻的对象串(忽略后边界对象, 范围, 0);
				新词汇_begindex = 忽略后边界对象.endindex;
				偏移量 = 忽略后边界对象.endindex - 原对象.endindex + 新词汇_串.Length;
				词汇对象.忽略前begindex = 右端对象.begindex + 1;
			}
			else if (介词 != null)
			{
				新词汇_串 = 获取右边相邻的对象串(介词, 范围, 0);
				新词汇_begindex = 介词.endindex;
				偏移量 = 介词.endindex - 原对象.endindex + 新词汇_串.Length;
			}
			//右端对象如果是动词，需判断其是否要包含在新词汇中
			else if (Data.能够序列化(右端对象.中心第一根类.模式行))
			{
				if (原对象.中心第一根类 == 右端对象.中心第一根类)
				{ //如果右端对象，是原对象的中心对象，则认为不应该被新词汇包含
					if (原对象.是隐藏对象() == false)
					{
						新词汇_串 = 获取右边相邻的对象串(右端对象, 范围, 0);
						新词汇_begindex = 右端对象.endindex;
						偏移量 = 新词汇_串.Length;
					}
					else
					{
						新词汇_串 = 右端对象.中心对象.取子串;
						新词汇_begindex = 右端对象.中心对象.begindex;
						偏移量 = 0;
					}
				}
				else
				{
					if (右端对象.中心第一根类.是隐藏对象() == false)
					{
						新词汇_串 = 右端对象.中心第一根类.取子串;
						新词汇_begindex = 右端对象.中心第一根类.begindex;
						偏移量 = 0;
					}
					else
					{//该部分是否需要外移，放在生成右端对象时
						新词汇_串 = 右端对象.中心对象.取子串;
						新词汇_begindex = 右端对象.中心对象.begindex;
						偏移量 = 0;
					}
				}
			}
			else
			{
				if (Data.能够序列化(原对象.中心第一根类.模式行))
				{
					if (Data.是派生类(Data.生存阶段Guid, 右端对象.中心第一根类.模式行, 替代.正向替代)
						|| Data.是派生类(Data.代词Guid, 右端对象.中心第一根类.模式行, 替代.正向替代)
						|| 右端对象.总分 > 10)
					{
						新词汇_串 = 获取右边相邻的对象串(右端对象, 范围, 0);
						新词汇_begindex = 右端对象.endindex;
						偏移量 = 新词汇_串.Length;
					}
					else
					{
						新词汇_串 = 右端对象.取子串;
						新词汇_begindex = 右端对象.begindex;
						偏移量 = 0;
					}
				}
				else
				{
					if (Data.是派生类(Data.人Guid, 右端对象.中心第一根类.模式行, 替代.正向替代) ||
						Data.是派生类(Data.人角色Guid, 右端对象.中心第一根类.模式行, 替代.正向替代))
					{
						新词汇_串 = 获取右边相邻的对象串(右端对象, 范围, 0);
						新词汇_begindex = 右端对象.endindex;
						偏移量 = 新词汇_串.Length;
					}
					else
					{
						新词汇_串 = 右端对象.取子串;
						新词汇_begindex = 右端对象.begindex;
						偏移量 = 0;
					}
				}
			}
			词汇对象.begindex = 新词汇_begindex;
			词汇对象.形式串 = 新词汇_串;
			词汇对象.外围中心对象 = 原对象;
			词汇对象.偏移量 = 偏移量;
			//界定右边界
			将词汇对象向右进行拼接(词汇对象, 范围);
			//{
			判定并设置新词汇对象的类型(词汇对象);
			return 词汇对象;
			//}
			return null;
		}
		private bool 将词汇对象向右进行拼接(新词汇 词汇对象, 封闭范围 范围)
		{
			生长对象 原对象 = 词汇对象.外围中心对象;
			string 新词汇_串 = 词汇对象.形式串;
			int 偏移量 = 词汇对象.偏移量;
			if (新词汇_串.Length > 0)
			{
				bool 继续 = true;
				do
				{
					生长对象 右对象 = 获取右边相邻的生长对象(原对象, 范围, 偏移量);
					if (右对象 != null && (右对象.中心第一根类 == 右对象)) //右对象是一级对象，而且是动词|| 右对象.长度 <= 2
					{
						if (Data.能够序列化(右对象.中心第一根类.模式行)) //右边是动词
						{
							if (获取右边相邻的对象串(右对象, 范围, 0).Length == 0 && 右对象.长度 == 1) //如果右边是动词，且该动词后边没有其它对象,则拼接该动词
							{
								新词汇_串 += 右对象.取子串;
								偏移量 += 右对象.长度;
							}
							else if (新词汇_串.Length >= 3) //如果右边是动词，而当前词汇长度已超2位,则先尝试该词汇
								继续 = false;
							else
							{
								新词汇_串 += 右对象.取子串;
								偏移量 += 右对象.长度;
							}
						}
						else if (Data.是派生类(Data.生存阶段Guid, 右对象.中心第一根类.模式行, 替代.正向替代) && 新词汇_串.Length >= 2) //如果右边是生存阶段(了等)，而当前词汇长度已超2位,则先尝试该词汇
							继续 = false;
						else
						{
							新词汇_串 += 右对象.取子串;
							偏移量 += 右对象.长度;
						}
					}
					else if (右对象 != null && 右对象.中心第一根类 != 右对象) //右对象是已经进行过生长的对象
					{
						if (新词汇_串.Length < 2 || 右对象.概率分 < 8) //如果新词汇长度比较短，需要提出其中的一级对象进行拼接
						{
							右对象 = 获取右边相邻的生长对象(原对象, 范围, 偏移量, true);
							新词汇_串 += 右对象.取子串;
							偏移量 += 右对象.长度;
						}
						else
							继续 = false;
					}
					else if (右对象 == null)
					{
						string s = 获取右边相邻的对象串(原对象, 范围, 偏移量);
						if (s.Length < 1)
							继续 = false;
						else
						{
							新词汇_串 += s;
							偏移量 += s.Length;
						}
					}
					else
						继续 = false;
				} while (继续);
			}
			if (新词汇_串 == 词汇对象.形式串)
				return false;
			else
			{
				词汇对象.形式串 = 新词汇_串;
				词汇对象.偏移量 = 偏移量;
				return true;
			}
		}
		private void 判定并设置新词汇对象的类型(新词汇 新词汇对象)
		{
			if (新词汇对象 != null && 新词汇对象.形式串.Length > 0)
			{
				生长对象 左介词对象 = 获取指定位置左边的相邻一级介词对象(新词汇对象.忽略前begindex > 0 ? 新词汇对象.忽略前begindex : 新词汇对象.begindex);
				if (左介词对象 != null) //左边相邻介词
				{
					if (左介词对象.取子串 == "去" || 左介词对象.取子串 == "在" || 左介词对象.取子串 == "到")
						新词汇对象.类型ID = Data.地点Guid;
					else
						新词汇对象.类型ID = Data.人Guid;
				}
				//新词汇的左边是动词
				else if (新词汇对象.外围中心对象 != null && Data.能够序列化(新词汇对象.外围中心对象.中心第一根类.模式行))
				{
					//如果外围动词是[抽象移动]，则默认类型为地点
					if (Data.是派生类(Data.抽象移动Guid, 新词汇对象.外围中心对象.中心第一根类.模式行, 替代.正向替代))
						新词汇对象.类型ID = Data.地点Guid;
					else
						新词汇对象.类型ID = Data.人Guid;
					List<参数> 参数表 = 新词汇对象.外围中心对象.得到指定根对象的参数表();
					foreach (参数 param in 参数表)
					{
						if (param.已经派生() == false && ((param.语言角色 & 字典_语言角色.后状) > 0 || (param.语言角色 & 字典_语言角色.宾语) > 0))
						{
							if (param.源关联记录.B端.Equals(Data.人角色Guid) || param.源关联记录.B端.Equals(Data.人Guid))
								新词汇对象.类型ID = Data.人Guid;
							else
								新词汇对象.类型ID = Data.地点Guid;
						}
						//如果是已经吸收的参数，直接取相应的类型
						else if (param.已经派生() == true && param.对端派生对象.endindex > 新词汇对象.begindex)
						{
							if (param.源关联记录.B端.Equals(Data.人角色Guid) || param.源关联记录.B端.Equals(Data.人Guid))
								新词汇对象.类型ID = Data.人Guid;
							else
								新词汇对象.类型ID = Data.地点Guid;
						}
					}
				}
				else
				{
					新词汇对象.类型ID = Data.人Guid;
				}
			}
			if (新词汇对象.类型ID == null || 新词汇对象.类型ID.Equals(Guid.Empty))
				新词汇对象.类型ID = Data.地点Guid;
		}

		public bool 启动新词汇处理(封闭范围 内部范围, int 起始排序分)
		{
			bool 生成新词汇 = false;
			生长对象 左对象;
			新词汇 词汇对象 = null;
			//1.先检查一下当前范围，是否已经成功完成生长
			int i = 在左边界排序对象中定位(内部范围.内在begindex);
			左对象 = 左边界排序对象[i];
			if (左对象.begindex <= 内部范围.内在begindex && 左对象.endindex >= 内部范围.内在endindex)
				return false;
			//2.如果“新词汇集合”为空，则认为是第一次发现并生成新词汇对象，需要从左向右遍历已生长对象：
			if (新词汇集合.Count < 1)
			{
				//2.1.当前范围内，从左向右遍历“最长的对象”，如果当前对象，长度超出范围，则认为生长成功，没有断开点，否则认为结尾处是断开点
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					左对象 = 左边界排序对象[j];
					if (左对象.begindex <= 内部范围.内在begindex && 左对象.endindex >= 内部范围.内在endindex)
						break;
					if (j == i && 左对象.begindex > 内部范围.内在begindex) //第一个长对象，如果起始位置就是断的，要先处理左边的词汇
					{
						词汇对象 = 根据右端对象生成新词汇对象(null, null, 内部范围);
						新词汇集合.Add(词汇对象);
						break;
					}
					//如果中心对象是动词
					生长对象 右端对象 = 获取已吸收的最右端的参数对象(左对象);
					if (右端对象 != null)
					{
						词汇对象 = 根据右端对象生成新词汇对象(左对象, 右端对象, 内部范围);
						新词汇集合.Add(词汇对象);
						break;
					}

				}
			}
			else
			{
				词汇对象 = 新词汇集合[0];
			}
		开始处理词汇:
			if (词汇对象 != null)
			{
				左对象 = 词汇对象.外围中心对象;
				string 形式串 = 词汇对象.形式串;
				//与右边对象拼接字符串
				模式 语义Row = null, 形式Row = null;
				string 相邻串 = "";
				if (词汇对象.已增加 == true)
				{
					//相邻串 = 获取右边相邻的对象串(左对象, 内部范围, 词汇对象.偏移量);
					if (将词汇对象向右进行拼接(词汇对象, 内部范围)) //词汇对象向右进行拼接
						词汇对象.已增加 = false;
				}
				if (词汇对象.已增加 == false)
				{
					形式串 = 形式串 + 相邻串;
					if (Data.构建命名实体语义和形式模式行(词汇对象.类型ID, 形式串, 9, Data.当前解析语言, out 语义Row, out 形式Row))
					{
						新词汇集合.Remove(词汇对象);
						return false;
					}
					//构造语料串
					SubString 语料串 = new SubString();
					语料串.begindex = 词汇对象.begindex;
					语料串.endindex = 语料串.begindex + 形式串.Length;

					模式 字串row = Data.增加字符串生长素材(语料串);

					生长对象 串对象 = new 生长对象(字串row, 0);
					串对象.begindex = 语料串.begindex;
					串对象.endindex = 语料串.endindex;
					//串对象.对应匹配语料对象 = 语料串;
					加入一个对象到池(串对象);

					生长对象 新对象 = 构造一级语义对象(串对象, 形式Row, 0);
					新对象.中心对象 = 串对象;
					新对象.一级对象构建形式和关键参数(串对象);

					加入一个对象到池(新对象);
					Data.输出串("      增加新词汇:【" + 新对象.取子串 + "】");
					根据新对象取消交叉对象的抑制性(新对象, true);
					双向组合出待生长对象对并插入待生长集合(新对象, 内部范围, 起始排序分, false, false);

					//加入新词汇集合
					词汇对象.外围中心对象 = 左对象;
					词汇对象.偏移量 += 相邻串.Length;
					词汇对象.形式串 = 形式串;
					词汇对象.已增加 = true;
					生成新词汇 = true;
				}
				else
				{
					if (新词汇集合.Count > 0 && 新词汇集合[0] == 词汇对象)
					{
						if (词汇对象.外围中心对象.中心第一根类.begindex == 词汇对象.begindex)
						{
							词汇对象.begindex += 词汇对象.外围中心对象.中心第一根类.长度;
							词汇对象.形式串 = "";
							词汇对象.偏移量 = 0;
							goto 开始处理词汇;
						}
						else
						{
							新词汇集合.Remove(词汇对象);
							return 启动新词汇处理(内部范围, 起始排序分);
						}
					}
				}
			}
			return 生成新词汇;
		}
		public 生长对象 获取指定位置左边的相邻一级介词对象(int endindex)
		{
			int i = 在右边界排序对象中定位(endindex);
			for (int j = i; j < 右边界排序对象.Count; j++)
			{
				生长对象 左对象 = 右边界排序对象[j];
				if (左对象.endindex < endindex)
					break;
				if (左对象.中心第一根类 == 左对象)
				{
					if (Data.是介词或者串(左对象.模式行, true, true, false))
						return 左对象;
					continue;
				}
				if (Data.是派生类(Data.封闭范围Guid, 左对象.中心第一根类.模式行, 替代.正向替代))
					endindex--;
				if (Data.是派生类(Data.短句停顿Guid, 左对象.中心第一根类.模式行, 替代.正向替代))
					endindex--;
			}
			return null;
		}
		public 生长对象 获取右边相邻的生长对象(生长对象 原对象, 封闭范围 范围, int 偏移量, bool 只取一级对象 = false)
		{
			int endindex = 范围.内在begindex;
			if (原对象 != null)
				endindex = 原对象.endindex;
			int i = 在左边界排序对象中定位(endindex + 偏移量);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 右对象 = 左边界排序对象[j];
				if (右对象.endindex > 范围.内在endindex)
					break;
				if (右对象.begindex > endindex + 偏移量)
					break;
				if (Data.是派生类(Data.短句停顿Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return null;
				if (Data.是派生类(Data.句子语用基类Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return null;
				if (Data.是派生类(Data.封闭范围Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return null;
				if (只取一级对象 && 右对象.中心第一根类 == 右对象)
					return 右对象;
				if (只取一级对象 == false)
					return 右对象;
			}
			return null;
		}
		public string 获取右边相邻的对象串(生长对象 原对象, 封闭范围 范围, int 偏移量)
		{
			int endindex = 范围.内在begindex;
			if (原对象 != null)
				endindex = 原对象.endindex;
			int i = 在左边界排序对象中定位(endindex + 偏移量);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 右对象 = 左边界排序对象[j];
				if (右对象.endindex > 范围.内在endindex)
					break;
				if (右对象.begindex > endindex + 偏移量)
					break;
				if (Data.是派生类(Data.短句停顿Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return "";
				if (Data.是派生类(Data.句子语用基类Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return "";
				if (Data.是派生类(Data.封闭范围Guid, 右对象.中心第一根类.模式行, 替代.正向替代))
					return "";
				return 右对象.取子串;
			}
			//如果对象列表中找不到相邻对象，则可能是字符串，需直接获取
			if ((endindex + 偏移量) < 范围.内在endindex)
			{
				if (源语言文字.Length > endindex + 偏移量)
				{
					for (int j = endindex + 偏移量; j < 源语言文字.Length; j++)
					{
						return 源语言文字[j].ToString();
					}
				}
			}
			return "";
		}
		public bool 进行一轮生长()
		{
			//不再以发现了一个比较完整的对象作为结束条件，而是以不能生长了才作为结束条件。
			//if (false/*可以结束所有单句生长()*/)
			//{
			//    if (进行后阶段再处理() == false)
			//        return false;
			//}

			//if (处理轮数 == 1)
			//    组织封闭范围();

			处理轮数++;

			//if (处理轮数 == 1)
			//创建哨兵对象();

			//根据单对象进行上级对象或者派生对象的创建。
			触发对象创建();
			组织封闭范围();

			//处理交叉抑制打分();
			//if (处理轮数 == 1)
			推迟一些被介词或者名词抑制的动词的激活时间();
			处理拆解打分();
			处理抽象动词();

			int oldcount = 全部对象.Count();
			//再次尝试进行集合对象生长，尤其是没有显式的【和】等的情况下。
			//尝试进行集合生长();

			//循环处理所有对象(内部范围, 内部范围.begindex, 内部范围.endindex);

			int 起始分 = 0;
			生成待生长对象对并排序(内部范围, ref 起始分);

			//处理右边空了的【的】情况。假设对象1是一个中心语，左边相邻的是定语的生长。
			//最基本的就是定语的生长，如果右边的是中心语没有错的话，左边相邻的一定是其定语。这个是一个很好限制的条件。
			//{
			//	int 右边界位置 = 对象1.begindex;
			//	int index = 查找下一个右边界对象(右边界位置, -1);
			//	while (index != -1)
			//	{
			//		生长对象 对象A = 右边界排序对象[index];
			//		Data.输出串(对象A.取子串 + "--" + 对象1.取子串);
			//		index = 查找下一个右边界对象(右边界位置, index);
			//	}
			//}

			//int 本轮生长最大打分 = 计算本轮生长最大打分();
			//如果生长阀值小于一个临界值，也包括完全没有生长出对象（相当于为0），那么就触发其它方式的生长，其它方式相当于也有生长阀值。
			//if (本轮生长最大打分 <= 意义优先性阀值)//后边设置一个另外的阀值。
			//	启动特殊生长解决问题(本轮生长最大打分);

			if (全部对象.Count - oldcount == 0) //后边设置一个另外的阀值。
			{
				//启动特殊生长解决问题(1, 范围);
			}

			int c = 全部对象.Count - oldcount;

			//bool 本轮是开启新的连续生长 = 连续生长开始轮数 == 处理轮数;

			//if (c == 0)//没有计算出结果，进入下一个连续生长循环。
			//    连续生长开始轮数 = 处理轮数 + 1;

			return c > 0;//|| 内部范围.处理阶段 < 5/* || 本轮是开启新的连续生长 == false*/;
		}
		//将'待选用抽象对象集'中的抽象动词遍历匹配参数后，将成功匹配到时参数的抽象动词插入到全部对象中
		public void 处理抽象动词()
		{
			foreach (生长对象 抽象对象 in 待选用抽象对象集)
			{
				if (!全部对象.Contains(抽象对象))
				{
					封闭范围 范围 = 内部范围.递归返回指定位置所属子范围(抽象对象.begindex);
					参数树结构 参数展开树 = 抽象对象.利用缓存得到基类和关联记录树();
					生长对象 参数对象 = 递归查找关联参数(参数展开树, 范围, 字典_语言角色.宾语, 抽象对象);
					if (参数对象 != null)
					{
						抽象对象.参数.概率分++;
						//如果参数对象紧挨抽象对象，则概率增加
						if (参数对象.begindex == 抽象对象.endindex || 参数对象.endindex == 抽象对象.begindex)
							抽象对象.参数.概率分++;
						//抽象对象加入到当前素材
						if (抽象对象.模式行.ParentID != Data.当前素材Row.ID)
							Data.加入到素材(抽象对象.模式行);
						抽象对象.一级对象构建形式和关键参数(抽象对象.中心对象);
						加入一个对象到池(抽象对象);
					}
					//待选用抽象对象集.Remove(抽象对象);
				}
			}
		}
		public void 处理拆解打分()
		{
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.是隐藏对象())
					continue;
				if (obj.是介词或者串(false, false, true))
					continue;
				if (obj.长度 > 1)
				{
					for (int j = 0; j < 左边界排序对象.Count(); j++)
					{
						生长对象 o = 左边界排序对象[j];
						if (o == obj)
							continue;
						if (o.是隐藏对象())
							continue;
						if (o.是介词或者串(false, false, true))
							continue;
						if (o.endindex < obj.begindex)
							continue;
						if (o.begindex >= obj.endindex)
							break;
						if (o.长度 > 1)
						{
							int k = 生长对象.计算位置重叠性(obj, o);
							if (k > 1) //只是交叉有效
							{
								if (o.endindex < obj.endindex) //这里待考虑长度问题？……&& o.长度 >= obj.长度
									obj.左拆解++;
								else if (o.endindex > obj.endindex)
									obj.右拆解++;
							}
						}
					}
					if (是一个强势完成对象(obj))
						强势完成对象集合.Add(obj);
				}
			}
		}
		public void 处理交叉抑制打分()
		{
			int i = 0;
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.是隐藏对象())
					continue;
				if (obj.是介词或者串(true, true, true))
					continue;
				if (obj.长度 > 1)
				{
					for (int j = 0; j < 左边界排序对象.Count(); j++)
					{
						生长对象 o = 左边界排序对象[j];
						if (o == obj)
							continue;
						if (o.是隐藏对象())
							continue;
						if (o.是介词或者串(true, true, true))
							continue;
						if (o.begindex > obj.endindex)
							break;
						int k = 生长对象.计算位置重叠性(obj, o);
						if (k > 0)
							o.参数.概率分--;
					}
				}
				i++;
			}
		}
		//根据参数树，在全部对象中查找是否有‘可以做为其某个语言角色参数的对象’
		public 生长对象 递归查找关联参数(参数树结构 参数树, 封闭范围 范围, int 语言角色, 生长对象 触发对象)
		{
			生长对象 参数对象 = null;
			if (参数树.目标.连接 == Data.拥有Guid && 参数树.目标.参数.B对A的关键性 > 0)
			{
				模式 r = 参数树.目标;
				//var dr = Data.模式表.对象集合.Where(r => r.ParentID == 参数树.目标ID).ToList();
				//foreach (模式 r in dr)
				//{
				//    if (r.连接 != Data.拥有语言角色Guid)
				//        continue;
				//    if (r.语言角色 != 字典_语言角色.无 && r.语言角色 != 字典_语言角色.全部 && (r.语言角色 & 语言角色) > 0)
				//    {
				//1.先向右找
				int i = 在左边界排序对象中定位(触发对象.endindex);
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					if (相邻对象.begindex > 范围.endindex)
						break;
					if (和强势完成对象冲突(相邻对象))
						continue;
					if (是封闭范围或短句停顿符(相邻对象))
						break;
					if (Data.是派生类(r.B端, 相邻对象.中心第一根类.源模式行, 替代.正向替代 | 替代.聚合替代))
					{
						参数对象 = 相邻对象;
						return 参数对象;
					}
				}
				//2.再向左找
				i = 在左边界排序对象中定位(触发对象.begindex);
				for (int j = i; j >= 0; j--)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					if (相邻对象.begindex < 范围.begindex)
						break;
					if (和强势完成对象冲突(相邻对象))
						continue;
					if (是封闭范围或短句停顿符(相邻对象))
						break;
					if (Data.是派生类(r.B端, 相邻对象.中心第一根类.源模式行, 替代.正向替代 | 替代.聚合替代))
					{
						参数对象 = 相邻对象;
						return 参数对象;
					}
				}
				//}
				//}
			}
			if (参数树.子节点 != null)
			{
				foreach (参数树结构 o in 参数树.子节点)
				{
					if (!递归判断参数树是否被重载(o.父对象, o.目标ID))
					{
						参数对象 = 递归查找关联参数(o, 范围, 语言角色, 触发对象);
						if (参数对象 != null)
							break;
					}
				}
			}
			return 参数对象;
		}
		//判断参数树中的某个参数，是否已经被当前派生对象重载
		public bool 递归判断参数树是否被重载(参数树结构 参数树, Guid 目标ID)
		{
			bool 被重载 = false;
			if (参数树.父对象 != null)
			{
				foreach (参数树结构 o in 参数树.父对象.子节点)
				{
					if (o.目标.源记录 == 目标ID)
						return true;
				}
				被重载 = 递归判断参数树是否被重载(参数树.父对象, 目标ID);
			}
			return 被重载;
		}
		//找到可以释放的对象，取消其抵制性
		public bool 释放一个对象使其它对象可以继续生长(封闭范围 范围)
		{
			生长对象 待释放对象 = null;
			int i = 在左边界排序对象中定位(范围.内在begindex);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 左对象 = 左边界排序对象[j];
				if (对象封闭前进行完成性打分(左对象) <= 0 && 左对象.释放标志 == false && 左对象.取消抑制性 == false) //检查对象的关键参数是否完成，没有则释放
				{
					待释放对象 = 左对象;
					break;
				}
				if (左对象.begindex <= 范围.内在begindex && 左对象.endindex >= 范围.内在endindex)
					break;
				if (左对象.endindex > 范围.内在endindex)
					break;
				if (左对象.释放标志 == false && 左对象.取消抑制性 == false && 左对象.中心第一根类 != 左对象
					&& 是否被完整的短句抑制(左对象) == false)
				{
					if (待释放对象 == null || 左对象.长度 > 待释放对象.长度)
					{
						待释放对象 = 左对象;
					}
				}
			}
			if (待释放对象 != null)
			{
				待释放对象.取消抑制性 = true;
				待释放对象.释放标志 = true;
				Data.输出串("      释放对象:【" + 待释放对象.取子串 + "】");
				return true;
			}
			return false;
		}
		//遍历封闭范围，并按从左向右的顺序组合出待生长对象对，并从内向外开始生长
		public void 生成待生长对象对并排序(封闭范围 范围, ref int 起始排序分, bool 是否尝试生长 = true)
		{
			if (范围.子范围 != null)
			{
				foreach (封闭范围 子范围 in 范围.子范围)
					生成待生长对象对并排序(子范围, ref 起始排序分);
			}
			范围.打分起始分 = 起始排序分;
			int 本次起始序数 = 有效对象计数;
			for (int i = 在左边界排序对象中定位(范围.begindex); i < 左边界排序对象.Count; i++)
			{
				生长对象 左对象 = 左边界排序对象[i];
				if (Data.是介词或者串(左对象.源模式行, true, true, true))
					continue;
				//if (Data.是派生类(Data.封闭范围Guid, 左对象.源模式行, 替代.正向替代))
				//    continue;
				if (左对象.begindex > 范围.endindex)
					break;
				if (左对象.endindex > 范围.endindex)
					continue;
				if (左对象.长度 == 0)
					continue;
				if (左对象.取子串 == "1996年，王菲嫁给摇滚歌手窦唯，成为当年娱乐圈最轰动的事件。")
					i = i;
				双向组合出待生长对象对并插入待生长集合(左对象, 范围, 起始排序分, true);
			}
			if (是否尝试生长)
			{
				bool 是否继续生长 = false;
				do
				{
					是否继续生长 = 进行最优先关联对象对的生长();
					//如果没有可生长对象，检查当前范围是否生长成功，否则取消长对象的抑制性，继续尝试生长
					if (是否继续生长 == false)
					{
						if (释放一个对象使其它对象可以继续生长(范围))
							是否继续生长 = true;
						else
							break;
					}
				} while (是否继续生长);
				//当前子范围内，检查并处理新词汇
				if (Data.允许新词汇自动处理)
				{
					bool 有新词汇 = false;
					do
					{
						有新词汇 = 启动新词汇处理(范围, 起始排序分);
						if (有新词汇)
						{
							//发现新词汇，需继续完成生长
							do
							{
								是否继续生长 = 进行最优先关联对象对的生长();
							} while (是否继续生长);
						}
					} while (有新词汇);
					新词汇集合.Clear();
				}
			}
			//最内层范围起始排序分最小，越往外起始排序分越大(内层优先生长)
			起始排序分 += 30000;
		}
		public bool 是否被介词包含抑制(生长对象 目标对象)
		{
			for (int i = 0; i < 左边界排序对象.Count; i++)
			{
				生长对象 左对象 = 左边界排序对象[i];
				if (左对象.begindex > 目标对象.begindex)
					break;
				if (左对象.endindex < 目标对象.endindex)
					continue;
				if (左对象 != 目标对象 && 左对象.begindex <= 目标对象.begindex && 左对象.endindex >= 目标对象.endindex
						&& 左对象.中心第一根类 == 左对象 && 左对象.长度 > 目标对象.长度)
				{
					if (Data.是介词或者串(左对象.模式行, true, true, false))
						return true;
				}
			}
			return false;
		}
		//以某个对象为中心，先向右边尝试组合出待生长对象对，再向左边尝试组合出待生长对象对
		public bool 双向组合出待生长对象对并插入待生长集合(生长对象 触发对象, 封闭范围 范围, int 起始排序分, bool 只向右 = false, bool 被推后生长 = false)
		{
			bool ret = false;
			if (触发对象.取子串 == "1997年1月，王菲在北京协和医院产下女儿，为她取名为窦靖童。")
				ret = ret;
			if (被推后生长)
				ret = ret;
			int i = 在左边界排序对象中定位(触发对象.endindex);
			if (触发对象.begindex > 范围.endindex)
				return false;
			if (触发对象.endindex > 范围.endindex)
				return false;
			if (触发对象.长度 == 0)
				return false;
			if (是封闭范围或短句停顿符(触发对象))
				return false;
			bool 强制中止 = false;
			bool 必须右端生长完成 = false;
			bool 必须左端生长完成 = false;
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				if (强制中止)
					break;
				生长对象 右对象 = 左边界排序对象[j];
				if (Data.是介词或者串(右对象.源模式行, true, true, true) && 右对象.取子串 != "被")
					continue;
				//if (Data.是派生类(Data.封闭范围Guid, 右对象.中心第一根类.源模式行, 替代.正向替代))
				//    continue;
				if (计算单对象被抑制性(右对象, 触发对象, true))
					continue;
				if (是否被完整的短句抑制(右对象))
					break;
				if (右对象.begindex == 触发对象.begindex && 右对象.endindex == 触发对象.endindex)
					continue;
				if (右对象.概率打分 <= 0)
					continue;
				//if (右对象.长度 == 0)
				//    continue;
				if (右对象.begindex > 范围.endindex || 右对象.endindex > 范围.endindex)
					break;
				if (是封闭范围或短句停顿符(右对象)) //短句分隔符
				{
					if (Data.是介词或者串(右对象.中心对象.模式行, true, true, false)) //如果是冒号、破折号需要当介词处理，而且必须先让右端完成生长
					{
						if (判断对象的左边是否已生长完成(触发对象, 范围.begindex) == false)
							break;//冒号、破折号时，左左右两边必须都已生长完成
						必须右端生长完成 = true;
						continue;
					}
					else
					{
						if (判断对象的左边是否已生长完成(触发对象, 范围.内在begindex)) //短句已生长完成才能与分隔符生长
							强制中止 = true;
						else if (!Data.是派生类(Data.数Guid, 触发对象.模式行, 替代.正向替代))
							break;
					}
				}
				if (必须右端生长完成 && !判断对象的右边是否已生长完成(右对象, 范围.内在endindex))
					continue;
				{
					//左、右对象生成对象对，并进行优先级打分后插入待生长链表
					关联对象对 中心在左对象对 = 创建或者返回可以生长的一个对象对(触发对象, 右对象);
					关联对象对 中心在右对象对 = 创建或者返回可以生长的一个对象对(右对象, 触发对象);
					if (中心在左对象对.生长次序 > 0 || 中心在右对象对.生长次序 > 0) //已经创建过该对象对
						break;
					if (中心在左对象对.对象对 == null && 中心在左对象对.处理类型 == 0 && 中心在右对象对.对象对 == null && 中心在右对象对.处理类型 == 0)//这句是判断两个对象已经相距很远，完全不相邻
					{
						if (是否被介词包含抑制(右对象))
							continue;
						else
							break;
					}
					//预先判断，如果两个组合都会被抑制，就没有必要进行了。
					//if ((中心在左对象对.对象对 == null || 计算对象被抑制性(中心在左对象对.对象对)) && (中心在右对象对.对象对 == null || 计算对象被抑制性(中心在右对象对.对象对)))
					//    continue;
					//获取生长顺序打分
					中心在左对象对.所属范围 = 范围;
					中心在右对象对.所属范围 = 范围;
					中心在左对象对.被推后生长 = 被推后生长;
					中心在右对象对.被推后生长 = 被推后生长;
					if (中心在右对象对.对象对 != null)// && 中心在右对象对.生长次序 <= 中心在左对象对.生长次序)
					{
						中心在右对象对.生长次序 = 根据对象类型获取优先生长打分(中心在右对象对) + 起始排序分;
						if (中心在右对象对.对象对 != null)
						{
							插入一个待生长的对象对(中心在右对象对);
							ret = true;
						}
					}
					if (中心在左对象对.对象对 != null)// && 中心在左对象对.生长次序 <= 中心在右对象对.生长次序)
					{
						中心在左对象对.生长次序 = 根据对象类型获取优先生长打分(中心在左对象对) + 起始排序分;
						if (中心在左对象对.对象对 != null)
						{
							插入一个待生长的对象对(中心在左对象对);
							ret = true;
						}
					}

				}
			}
			if (!只向右)
			{
				强制中止 = false;
				必须右端生长完成 = false;
				i = 在右边界排序对象中定位(触发对象.begindex);
				for (int j = i; j < 右边界排序对象.Count; j++)
				{
					if (强制中止)
						break;
					生长对象 左对象 = 右边界排序对象[j];
					if (Data.是介词或者串(左对象.源模式行, true, true, true) && 左对象.取子串 != "被")
						continue;
					//if (Data.是派生类(Data.短句停顿Guid, 右对象.中心第一根类.源模式行, 替代.正向替代))
					//    break;
					if (计算单对象被抑制性(左对象, 触发对象, true))
						continue;
					if (是否被完整的短句抑制(左对象))
						break;
					if (左对象.begindex == 触发对象.begindex && 左对象.endindex == 触发对象.endindex)
						continue;
					if (左对象.概率打分 <= 0)
						continue;
					//if (右对象.长度 == 0)
					//    continue;
					if (左对象.endindex <= 范围.begindex)
						break;
					if (是封闭范围或短句停顿符(左对象))
					{
						if (!判断对象的右边是否已生长完成(触发对象, 范围.内在endindex))
							break;
						else if (Data.是介词或者串(左对象.中心对象.模式行, true, true, false)) //如果是冒号、破折号需要当介词处理
						{ 必须左端生长完成 = true; continue; }
					}
					if (必须左端生长完成 == true && !判断对象的左边是否已生长完成(左对象, 范围.内在begindex))
						continue;
					{
						//左、右对象生成对象对，并进行优先级打分后插入待生长链表
						关联对象对 中心在左对象对 = 创建或者返回可以生长的一个对象对(左对象, 触发对象);
						关联对象对 中心在右对象对 = 创建或者返回可以生长的一个对象对(触发对象, 左对象);
						if (中心在左对象对.生长次序 > 0 || 中心在右对象对.生长次序 > 0)
							break;
						if (中心在左对象对.对象对 == null && 中心在左对象对.处理类型 == 0 && 中心在右对象对.对象对 == null && 中心在右对象对.处理类型 == 0)//这句是判断两个对象已经相距很远，完全不相邻
						{
							if (是否被介词包含抑制(左对象))
								continue;
							else
								break;
						}
						//预先判断，如果两个组合都会被抑制，就没有必要进行了。
						//if ((中心在左对象对.对象对 == null || 计算对象被抑制性(中心在左对象对.对象对)) && (中心在右对象对.对象对 == null || 计算对象被抑制性(中心在右对象对.对象对)))
						//    continue;
						中心在左对象对.所属范围 = 范围;
						中心在右对象对.所属范围 = 范围;
						中心在左对象对.被推后生长 = 被推后生长;
						中心在右对象对.被推后生长 = 被推后生长;
						if (中心在右对象对.对象对 != null)// && 中心在右对象对.生长次序 <= 中心在左对象对.生长次序)
						{
							中心在右对象对.生长次序 = 根据对象类型获取优先生长打分(中心在右对象对) + 起始排序分;
							if (中心在右对象对.对象对 != null)
							{
								插入一个待生长的对象对(中心在右对象对);
								ret = true;
							}
						}
						if (中心在左对象对.对象对 != null) //&& 中心在左对象对.生长次序 <= 中心在右对象对.生长次序)
						{
							中心在左对象对.生长次序 = 根据对象类型获取优先生长打分(中心在左对象对) + 起始排序分;
							if (中心在左对象对.对象对 != null)
							{
								插入一个待生长的对象对(中心在左对象对);
								ret = true;
							}

						}

					}

				}
			}
			return ret;
		}
		//判断某个对象是否为封闭范围或短句停顿对象
		public bool 是封闭范围或短句停顿符(生长对象 原对象, bool 必须是句号 = false)
		{
			if (必须是句号)
			{
				if (Data.是派生类(Data.句子语用基类Guid, 原对象.中心第一根类.源模式行, 替代.正向替代))
					return true;
				else
					return false;
			}
			else
			{
				if (Data.是派生类(Data.短句停顿Guid, 原对象.中心第一根类.源模式行, 替代.正向替代) ||
					Data.是派生类(Data.句子语用基类Guid, 原对象.中心第一根类.源模式行, 替代.正向替代))
					return true;
				else
					return false;
			}
		}
		public 生长对象 获取相邻的指定类型节点对象(生长对象 原对象, Guid 目标对象Guid, bool 右边, bool 左边 = false)
		{
			int i = 0;
			if (右边)
			{
				i = 在左边界排序对象中定位(原对象.endindex);
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					if (Data.是派生类(目标对象Guid, 相邻对象.中心第一根类.源模式行, 替代.正向替代))
						return 相邻对象;
				}
			}
			if (左边)
			{
				i = 在右边界排序对象中定位(原对象.begindex);
				for (int j = i; j < 右边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 右边界排序对象[j];
					if (Data.是派生类(目标对象Guid, 相邻对象.中心第一根类.源模式行, 替代.正向替代))
						return 相邻对象;
				}
			}
			return null;
		}
		//判断指定对象的相邻对象是否为某个类型的派生类
		public bool 判断相邻对象是否为指定类型(生长对象 原对象, Guid 目标对象Guid, bool 判断右边, bool 判断左边 = false)
		{
			int i = 0;
			if (判断右边)
			{
				i = 在左边界排序对象中定位(原对象.endindex);
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					if (相邻对象.begindex > 原对象.endindex)
						break;
					if (Data.是派生类(目标对象Guid, 相邻对象.中心第一根类.源模式行, 替代.正向替代))
						return true;
				}
			}
			if (判断左边)
			{
				i = 在右边界排序对象中定位(原对象.begindex);
				for (int j = i; j < 右边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 右边界排序对象[j];
					if (相邻对象.endindex < 原对象.begindex)
						break;
					if (Data.是派生类(目标对象Guid, 相邻对象.中心第一根类.源模式行, 替代.正向替代))
						return true;
				}
			}
			return false;
		}
		//判断指定对象的相邻对象是否为短句停顿对象
		public bool 判断相邻对象是否为短句分隔(生长对象 原对象, bool 判断右边, bool 判断左边 = false, bool 必须是句号 = false)
		{
			int i = 0;
			if (判断右边)
			{
				i = 在左边界排序对象中定位(原对象.endindex);
				for (int j = i; j < 左边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 左边界排序对象[j];
					if (相邻对象.begindex > 原对象.endindex)
						break;
					if (是封闭范围或短句停顿符(相邻对象, 必须是句号))
						return true;
				}
			}
			if (判断左边)
			{
				i = 在右边界排序对象中定位(原对象.begindex);
				for (int j = i; j < 右边界排序对象.Count; j++)
				{
					生长对象 相邻对象 = 右边界排序对象[j];
					if (相邻对象.endindex < 原对象.begindex)
						break;
					if (是封闭范围或短句停顿符(相邻对象, 必须是句号))
						return true;
				}
			}
			return false;
		}
		//判断指定对象的右边是否为“的”或者“地”
		public bool 判断对象的右边是否紧挨的或者地(生长对象 对象, bool 包含的 = true, bool 包含地 = true)
		{
			int i = 在左边界排序对象中定位(对象.endindex);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 相邻对象 = 左边界排序对象[j];
				if (相邻对象.begindex > 对象.endindex)
					break;
				if (相邻对象.是的或者地(包含的, 包含地))
				{
					return true;
				}
			}
			return false;
		}
		//判断指定对象的右边是否为“的”，并且短句结束
		public bool 判断对象的右边是否紧挨的和短句间隔(生长对象 对象, int 右边界)
		{
			int i = 在左边界排序对象中定位(对象.endindex);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 相邻对象 = 左边界排序对象[j];
				if (相邻对象.begindex > 对象.endindex)
					break;
				if (相邻对象.是的或者地(true, false))
				{
					if (相邻对象.endindex == 右边界 || 判断相邻对象是否为短句分隔(相邻对象, true))
					{
						return true;
					}
				}
			}
			return false;
		}
		//判断指定对象是否处于短句结尾
		public bool 判断对象的右边是否已生长完成(生长对象 对象, int 右边界, bool 必须是句号 = false)
		{
			if (对象.endindex == 右边界 || 判断相邻对象是否为短句分隔(对象, true, false, 必须是句号))
				return true;
			return false;
		}
		//判断指定对象是否处于短句开始
		public bool 判断对象的左边是否已生长完成(生长对象 对象, int 左边界, bool 必须是句号 = false)
		{
			if (对象.begindex == 左边界 || 判断相邻对象是否为短句分隔(对象, false, true, 必须是句号))
				return true;
			return false;
		}
		//当前对象是否存在多义性对象，如果存在则返回当前对象应延后处理的分值
		public int 多义性对象获取延后分值(生长对象 对象, bool 人对事动词延后 = true, bool 只检查是否存在 = false)
		{
			bool 存在人对事语义 = false;
			for (int i = 在左边界排序对象中定位(对象.begindex); i < 左边界排序对象.Count; i++)
			{
				生长对象 参照对象 = 左边界排序对象[i];
				if (参照对象.begindex > 对象.begindex)
					break;
				if (参照对象.是介词或者串(false, false, true))
					continue;
				if (参照对象.begindex == 对象.begindex && 参照对象.endindex == 对象.endindex && 对象 != 参照对象)
				{
					if (只检查是否存在)
						return 1;
					if (Data.是派生类(Data.人对事动作Guid, 参照对象.中心第一根类.源模式行, 替代.正向替代))
						存在人对事语义 = true;
				}
			}
			if (存在人对事语义)
			{
				if (Data.是派生类(Data.人对事动作Guid, 对象.中心第一根类.源模式行, 替代.正向替代) == 人对事动词延后)
					return 300;
			}
			return 0;
		}
		//根据对象对的中心对象和参数对象类型，获取生长次序打分
		public int 根据对象类型获取优先生长打分(关联对象对 待生长对象对)
		{
			生长对象 参数根对象 = 待生长对象对.参数对象.中心第一根类;
			生长对象 中心根对象 = 待生长对象对.中心对象.中心第一根类;
			生长对象 对象对 = 待生长对象对.对象对;
			待生长对象对.处理类型 = 生长_正常处理;

			//设置默认打分在名词之后，动词之前(动词的打分从2500开始)
			int 打分 = 2400;
			if (Data.是派生类(Data.推理角色Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 = 2300;
			if (中心根对象.是隐藏对象() || 参数根对象.是隐藏对象())
				打分 = 4000;
			if (待生长对象对.对象对.左对象.取子串 == "习总书记的讲话既实在又有新意，读了再想读，"
				&& 待生长对象对.对象对.右对象.取子串 == "很吸引人")
				打分 = 打分;

			bool 中心对象可序列化 = Data.能够序列化(中心根对象.模式行);
			bool 参数对象可序列化 = Data.能够序列化(参数根对象.模式行);
			bool 中心对象是动词 = Data.是派生类(Data.事件Guid, 中心根对象.源模式行, 替代.正向替代);
			bool 参数对象是动词 = Data.是派生类(Data.事件Guid, 参数根对象.源模式行, 替代.正向替代);

			生长对象 中心推理角色 = 待生长对象对.中心对象.查找已结合的推理角色(true);
			生长对象 参数推理角色 = 待生长对象对.参数对象.查找已结合的推理角色(true);
			生长对象 中心项目编号 = 待生长对象对.中心对象.查找已结合的某个类型对象(Data.实体反聚项目编号Guid);
			生长对象 参数项目编号 = 待生长对象对.中心对象.查找已结合的某个类型对象(Data.实体反聚项目编号Guid);

			#region  名词和名词
			//数词和时间点：12时，星期一
			if (Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.时间点Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 180;
			//数词和量词
			else if (Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.量Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 180;
			//数和级别
			else if (Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.级别Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 180;
			//级别和级别
			else if (Data.是派生类(Data.级别Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.级别Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 180;
			//每和量
			else if (Data.是派生类(Data.每Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.量Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 185;
			//级别和事物
			else if (Data.是派生类(Data.级别Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 185;
			//定性大小量和人角色
			else if (Data.是派生类(Data.定性大小量Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.人角色Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 190;
			//姓氏和名词
			else if (Data.是派生类(Data.姓氏Guid, 参数根对象.源模式行, 替代.正向替代) && 中心对象可序列化 == false)
				打分 = 900;
			//符合程度和符合程度
			else if (Data.是派生类(Data.符合程度Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.符合程度Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 300;
			//符合程度和定性
			else if (Data.是派生类(Data.符合程度Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.定性Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 310;
			//符合程序和情态动词
			else if (Data.是派生类(Data.符合程度Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.情态动词Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 350;
			else if (初步判断是动词从定(对象对))
			{
				打分 = 2600 + 待生长对象对.中心对象.begindex;
			}
			//时间量和实体优先结合
			else if (Data.是派生类(Data.时间量Guid, 参数根对象.源模式行, 替代.正向替代) && 对象对.中心在右)
			{
				打分 = 860;
			}
			//定性和定性
			else if (Data.是派生类(Data.定性Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.定性Guid, 中心根对象.源模式行, 替代.正向替代))
			{
				打分 = 860;//340; //调整到名词与形容词之后，比如：贫穷的年轻混混
				if (计算是否可以做为集合处理(对象对, false))
					待生长对象对.处理类型 = 生长_集合处理;
			}
			//中心对象是名词
			else if (Data.是派生类(Data.事物概念Guid, 中心根对象.源模式行, 替代.正向替代) || Data.是派生类(Data.量化概念Guid, 中心根对象.源模式行, 替代.正向替代)
				)//||中心根对象.源模式行.ID.Equals(Data.事件Guid)) //事件元类也是名词
			{
				//代词和名词紧密结合
				if (Data.是派生类(Data.代词Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.外延指定Guid, 参数根对象.源模式行, 替代.正向替代) == false
					&& Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代) == false
					&& 对象对 != null && 计算是否可以做为集合处理(对象对, false) == false)
					打分 = 240;
				//集合 
				else if (对象对 != null && 计算是否可以做为集合处理(对象对, false) && !是哨兵对象(对象对.参数对象, false))
				{
					打分 = 250 + 待生长对象对.中心对象.begindex;
					待生长对象对.处理类型 = 生长_集合处理;
				}
				//符合程度和事物
				else if (Data.是派生类(Data.符合程度Guid, 参数根对象.源模式行, 替代.正向替代))
					打分 = 400;
				//定性量和事物
				else if (Data.是派生类(Data.定性量Guid, 参数根对象.源模式行, 替代.正向替代))
					打分 = 500;
				//性别和事物
				else if (Data.是派生类(Data.性别Guid, 参数根对象.源模式行, 替代.正向替代))
					打分 = 240;
				//量词和事物
				else if (Data.是派生类(Data.量词个Guid, 参数根对象.源模式行, 替代.正向替代) &&
						 Data.是派生类(Data.事物概念Guid, 中心根对象.源模式行, 替代.正向替代))
				{
					打分 = 600;
					if (对象对 != null && 对象对.中心在右 == false)
						打分 = 2600;
					if (对象封闭前进行完成性打分(待生长对象对.参数对象) <= 0) //未完成'B对A的关键性'对象不允许生长
						打分 = 9000;
				}
				//度量和事物
				else if (Data.是派生类(Data.度量Guid, 参数根对象.源模式行, 替代.正向替代) &&
						 Data.是派生类(Data.事物概念Guid, 中心根对象.源模式行, 替代.正向替代))
				{
					打分 = 700;
				}
				//外延指定和事物
				else if (Data.是派生类(Data.外延指定Guid, 参数根对象.源模式行, 替代.正向替代))
				{
					if (判断相邻对象是否为指定类型(参数根对象, Data.数Guid, false, true))
						待生长对象对.对象对 = null;
					打分 = 1000;
				}                //形容词和事物
				else if (Data.是派生类(Data.分类形容词Guid, 参数根对象.源模式行, 替代.正向替代))
				{
					打分 = 850;
				}

				//事物和事物
				else if (Data.是派生类(Data.事物概念Guid, 参数根对象.源模式行, 替代.正向替代) && !Data.是派生类(Data.量化概念Guid, 中心根对象.源模式行, 替代.正向替代))
				{
					打分 = 900;
					if (中心推理角色 != null)
						待生长对象对.处理类型 = 0;
					//水果刀、故事书，类似的名词与名词紧密结合时，需特别提高优先级
					if (对象对 != null && 对象对.中心在右 && 对象对.右对象.中心第一根类 == 对象对.右对象 && 对象对.左对象.中心第一根类 == 对象对.左对象)
					{
						打分 = 200;
						if (对象对.右对象.begindex > 对象对.左对象.endindex) //左右对象不相邻时，需要比相邻的稍微靠后
							打分 = 245;
					}
				}
				//事物型名词谓语
				if (Data.能够作为名词谓语(中心根对象.源模式行) && 中心根对象.begindex >= 参数根对象.endindex &&
						Data.是派生类(Data.事物概念Guid, 参数根对象.源模式行, 替代.正向替代))
				{
					打分 = 1000;
					待生长对象对.处理类型 = 0;
				}
				//动词从定
				else if (初步判断是动词从定(对象对))
				{
					//打分 = 1100;
					打分 = 2600 + 待生长对象对.中心对象.begindex;
				}
				else if (参数对象可序列化)
				{
					if (参数对象是动词 == false)
						打分 = 2100;
					else
						打分 = 2600 + 待生长对象对.中心对象.begindex;
					//待生长对象对.处理类型 = 0;
				}
				//相对空间和事物
				else if ((Data.是派生类(Data.相对空间Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 中心根对象.源模式行, 替代.正向替代)))
					打分 = 1100;
			}
			//度量和数,用于序数，如：第一
			else if (Data.是派生类(Data.度量Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 = 150;
			else if (Data.是派生类(Data.量词个Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.量Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 = 200;
			//相对空间和事物
			else if ((Data.是派生类(Data.相对空间Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 参数根对象.源模式行, 替代.正向替代)))
				打分 = 1100;

			#endregion
			//事物型名词谓语
			if (Data.能够作为名词谓语(中心根对象.源模式行) && (中心根对象.begindex >= 参数根对象.endindex &&
				参数对象可序列化 == false) && 打分 >= 1000)
			{
				打分 = 1000;
				待生长对象对.处理类型 = 0;
			}
			else if (Data.能够作为名词谓语(中心根对象.源模式行) && 打分 >= 1000)
				打分 = 2600;
			//名词后缀为中心时，需格外提前生长，如：哥哥家
			if (对象对 != null && 对象对.中心对象 == 对象对.中心对象.中心第一根类 &&
				(对象对.中心对象.源模式行.参数.词性扩展 & 参数字段.词性_名词后缀) > 0)
				打分 = 240;
			#region 动词为中心
			if (中心对象可序列化) //&& 中心根对象.源模式行.ID.Equals(Data.事件Guid)==false)
			{
				待生长对象对.处理类型 = 生长_正常处理;
				//动词集合
				if (计算两个对象能并列(中心根对象, 参数根对象) && 多义性对象获取延后分值(参数根对象, false) == 0
					&& 多义性对象获取延后分值(中心根对象, false) == 0 && 对象对 != null && !Data.是派生类(Data.人对事动作Guid, 对象对.左对象.源模式行, 替代.正向替代))
				{
					生长对象 介词 = 对象对.中心在右 ? 对象对.后置介词 : 对象对.前置介词;
					if (介词 == null && (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false
														 || 对象对.左对象.查找包含的一级参数语言角色(字典_语言角色.宾语) == false))
					{
						打分 = 打分 = 3500 + 中心根对象.begindex;
						待生长对象对.处理类型 = 0;
					}
					else
					{
						打分 = 2100 + 待生长对象对.中心对象.begindex;
						if (是横向关联介词(介词))
							待生长对象对.处理类型 = 生长_集合处理;
						//else
						//    待生长对象对.处理类型 = 0;
					}
				}
				//中心对象是“事件”基类
				else if (中心根对象.源模式行.ID.Equals(Data.事件Guid))
				{
					if (待生长对象对.中心对象.中心第一根类 == 待生长对象对.中心对象)
					{
						if (参数对象可序列化)
							打分 = 2500 + 待生长对象对.参数对象.begindex - 1;
						else
							打分 = 2500 + 待生长对象对.中心对象.begindex;
					}
				}

				//由于定性量化概念可以有符合程度，也可以同时继承动词
				else if (Data.是派生类(Data.量化概念Guid, 中心根对象.源模式行, 替代.正向替代) &&
					Data.是派生类(Data.符合程度Guid, 参数根对象.源模式行, 替代.正向替代))
					打分 = 400;
				//动词和状语
				else if (对象对 != null && 参数端是很明显的状语(对象对) && 中心根对象.介动词等情况延后一阶段生长 == false)
					打分 = 2500;
				//动词从定
				else if (初步判断是动词从定(对象对))
				{
					打分 = 2500 + 待生长对象对.中心对象.begindex;
				}
				//动词和动词，（参数对象不应该是事件基类，因为事件基类应该当名词处理）
				else if (Data.是派生类(Data.事件Guid, 参数根对象.源模式行, 替代.正向替代) && 参数根对象.源模式行.ID.Equals(Data.事件Guid) == false)
				{
					//人对事动作，且参数是动词
					if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代))
					{
						打分 = 3500 + 中心根对象.begindex;// -待生长对象对.中心对象.长度 - 待生长对象对.参数对象.长度;
						//待生长对象对.处理类型 = 0;
					}
					else if (Data.是派生类(Data.位置关系Guid, 参数根对象.源模式行, 替代.正向替代))
					{
						打分 = 3500 + 中心根对象.begindex;
					}
					else if (Data.是派生类(Data.人对事动作Guid, 参数根对象.源模式行, 替代.正向替代))
					{
						if (待生长对象对.参数对象 != 参数根对象)
							打分 = 3500 + 中心根对象.begindex;// - 待生长对象对.中心对象.长度 - 待生长对象对.参数对象.长度;
						else
							打分 = 3600 + 中心根对象.begindex;
					}
					//else if (对象对 != null && !对象对.中心在右) //左边动词做中心时优先
					//    打分 = 3200;
					else
						打分 = 3600 + 中心根对象.begindex;

					if (对象对 != null && Data.是派生类(Data.人对事动作Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) == false
						&& 多义性对象获取延后分值(对象对.左对象.中心第一根类, false) == 0)
						待生长对象对.处理类型 = 0;

					if (对象对 != null && 对象封闭前进行完成性打分(对象对.参数对象) <= 0) //检查动词的必要参数，如果参数未实现，则不允许生长
						待生长对象对.对象对 = null;
				}
				//参数是基本关联
				else if (Data.是派生类(Data.基本关联Guid, 参数根对象.源模式行, 替代.正向替代) && 中心对象是动词)
				{
					if (可以作为符合程度的二元关联(待生长对象对.参数对象))
					{
						打分 = 2600 + 中心根对象.begindex; ;
					}
					else
					{
						打分 = 3600 + 中心根对象.begindex; ;
						待生长对象对.处理类型 = 0;
					}
				}
				//参数和动词都是基本关联
				else if (Data.是派生类(Data.基本关联Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.基本关联Guid, 中心根对象.源模式行, 替代.正向替代))
				{
					打分 = 3300;
					待生长对象对.处理类型 = 生长_两组动词并列处理;
				}
				//动词和参数对象(在打分的基础上减去参数对象的长度)
				else
				{
					//人对事动词，比一般动词延后生长（动词可以充当介词时也延后至人对事同级）;
					if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代))
						打分 = 3000 + 待生长对象对.中心对象.中心第一根类.begindex; //人对事动词生长次序
					else
						打分 = 2500 + 待生长对象对.中心对象.中心第一根类.begindex; //一般动词生长次序
					//如果中心根对象是介动词，并且右边紧邻对象不是“的”，说明存在做为介词的可能：
					//1.如果右边紧邻动词，则延后至所有人对事动词之后生长
					//2.否则延后至一般动词之后生长
					if (中心根对象.介动词等情况延后一阶段生长 == true && 判断对象的右边是否紧挨的或者地(中心根对象, true, false) == false)
					{
						// if (判断相邻对象是否为指定类型(中心根对象,Data.事件Guid,true,false))
						打分 = 3200 + 待生长对象对.中心对象.中心第一根类.begindex;
						// else
						//     打分 = 2600 + 待生长对象对.中心对象.中心第一根类.begindex;
					}
					//位置关系“在”的生长顺序比其它动词适当延后
					if (Data.是派生类(Data.位置关系Guid, 中心根对象.源模式行, 替代.正向替代))
						打分 += 500;
					//动词的左边参数优先生长
					if (对象对 != null && 对象对.中心在右)
					{
						打分 -= 1;
					}
					if (对象封闭前进行完成性打分(待生长对象对.参数对象) <= 0) //未完成'B对A的关键性'对象不允许生长
						待生长对象对.对象对 = null;
					//人对事动词为中心时，如果结合的参数不是动词需要延后处理
					if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代) && 参数对象可序列化 == false && 对象对 != null && 对象对.中心在右 == false)
						if (判断对象的右边是否已生长完成(对象对.右对象, 待生长对象对.所属范围.endindex) == false)
							打分 += 3000;

				}
				//人对事动词为中心时，如果参数是动词，但参数还没有结合宾语时需延后处理
				if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代) && 参数对象可序列化 == true && 对象对 != null && 对象对.中心在右 == false)
				{
					if (已经有了某个语言角色(对象对.右对象, 字典_语言角色.宾语) == false && 判断对象的右边是否已生长完成(对象对.右对象, 待生长对象对.所属范围.endindex) == false)
						打分 += 3000;
				}

				//推理角色需调整处理类型
				if (中心推理角色 != null && 参数推理角色 != null)
				{
					打分 = 1000;
					待生长对象对.处理类型 = 0;
				}
				else if (参数推理角色 != null && 中心对象是动词)
				{
					待生长对象对.处理类型 = 0;
				}
				else if (中心推理角色 != null || 参数推理角色 != null)
				{
					待生长对象对.处理类型 = 0;
					打分 += 2000;
				}
				//中心根对象概率分高的先生长（用于解决中心根对象有多义时的生长优先问题，如“打”）
				打分 -= 中心根对象.概率分;
				//中心和参数都是人对事动词需适当延后
				if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代)
					&& Data.是派生类(Data.人对事动作Guid, 参数根对象.源模式行, 替代.正向替代))
					打分 += 100;
				//左对象如果有多义性，而且有"人对事"的语义对象，则不允许其它类型生长
				//if (对象对!=null && 中心对象可序列化 && 参数对象可序列化 &&对象对.左对象.中心第一根类.endindex==对象对.右对象.中心第一根类.begindex
				//    && 多义性对象获取延后分值(对象对.左对象.中心第一根类) > 0)
				//    待生长对象对.对象对 = null;
				//多义性适当延后
				打分 += 多义性对象获取延后分值(参数根对象);
				打分 += 多义性对象获取延后分值(中心根对象);

				//打分 += 计算获得动词对象的靠外围打分(中心根对象,待生长对象对.所属范围);
				//对象对有可能做为比较结果生长时（拥有相关前置介词），先推后主语和动词的生长
				if (对象对 != null && 是对象要求的介词形式(Data.FindRowByID(Data.比较结果拥有B方Guid), 对象对.前置介词))
					打分 += 100;
				//参数对象存在介词解释时，稍微延后,比如：飞到、捕捉到
				if (对象对 != null && 对象对.参数对象.介动词等情况延后一阶段生长)
					打分 += 100;
			}
			else if (Data.什么Guid.Equals(参数根对象.源模式行.ID) && 中心根对象.begindex > 参数根对象.begindex)
				打分 = 2400;
			else if (Data.是派生类(Data.时间量Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.量词个Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 = 200;
			//以时间点为中心，需要在动词生长之后，如：我昨天吃饭以后、我昨天吃饭时
			else if (Data.是派生类(Data.时间点Guid, 中心根对象.源模式行, 替代.正向替代) && 打分 > 300)
				打分 = 3600 + 中心根对象.begindex;
			else if (Data.是派生类(Data.时间量Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代) == false && 打分 > 300)
				打分 = 3200;
			else if (Data.是派生类(Data.值概念Guid, 参数根对象.源模式行, 替代.正向替代))
				待生长对象对.处理类型 = 0;
			//外延指定和时间量
			if (Data.是派生类(Data.外延指定Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.时间量Guid, 中心根对象.源模式行, 替代.正向替代))
				打分 = 850;
			//时间量和时间量
			else if (Data.是派生类(Data.时间量Guid, 参数根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.时间量Guid, 中心根对象.源模式行, 替代.正向替代))
			{
				打分 = 855;
				//时间范围集合 
				if (对象对 != null && 计算是否可以做为集合处理(对象对, false) && !是哨兵对象(对象对.参数对象, false))
				{
					待生长对象对.处理类型 = 生长_集合处理;
					打分 += 待生长对象对.中心对象.begindex;
				}
			}
			//其它需特殊处理的类型
			//数字范围集合 
			if (Data.是派生类(Data.数Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代))
			{
				if (对象对 != null && 计算是否可以做为集合处理(对象对, false) && !是哨兵对象(对象对.参数对象, false))
				{
					待生长对象对.处理类型 = 生长_集合处理;
					打分 += 待生长对象对.中心对象.begindex;
				}
			}
			//冒号、破折号间隔的数字、路径用集合方式生长
			if (对象对 != null && 对象对.前置介词 != null && 是否短句停顿符介词(对象对.前置介词))
			{
				if (Data.是派生类(Data.数Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 参数根对象.源模式行, 替代.正向替代))
				{
					打分 = 250 + 待生长对象对.中心对象.begindex;
					待生长对象对.处理类型 = 生长_集合处理;
				}
				else if (Data.是派生类(Data.地点Guid, 中心根对象.源模式行, 替代.正向替代) && Data.是派生类(Data.地点Guid, 参数根对象.源模式行, 替代.正向替代))
				{
					打分 = 250 + 待生长对象对.中心对象.begindex;
					待生长对象对.处理类型 = 生长_集合处理;
				}
			}
			////这部分是冒号或破折号当停句停顿符时的处理办法:
			//if (待生长对象对.中心对象.查找已结合的冒号或破折号() != null || 待生长对象对.参数对象.查找已结合的冒号或破折号() != null)
			//{
			//    if (Data.是派生类(Data.地点Guid, 中心根对象.模式行, 替代.正向替代) && Data.是派生类(Data.地点Guid, 参数根对象.模式行, 替代.正向替代))
			//        待生长对象对.处理类型 = 生长_集合处理; //地址类对象按集合方式处理，如“兰州－北京”
			//    else if (Data.是派生类(Data.数Guid, 中心根对象.模式行, 替代.正向替代) && Data.是派生类(Data.数Guid, 参数根对象.模式行, 替代.正向替代))
			//        待生长对象对.处理类型 = 生长_集合处理; //数字比例类对象按集合方式处理，如“3:2”
			//    else
			//        待生长对象对.处理类型 = 生长_正常处理;
			//}
			if (Data.是派生类(Data.表达Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 += 4000;
			//动词可以允当介词时需适当延后
			//if (中心根对象.介动词等情况延后一阶段生长)// || 参数根对象.介动词等情况延后一阶段生长)
			//    打分 += 100;
			//参数是项目编号
			if (Data.是派生类(Data.项目编号Guid, 参数根对象.源模式行, 替代.正向替代))
				打分 = 9000 + 参数根对象.begindex;
			//中心和参数都已结合项目编号时，需做为集合处理
			else if (中心项目编号 != null && 参数项目编号 != null && 对象对 != null && 对象对.中心对象.begindex < 对象对.参数对象.begindex)
			{
				打分 = 100 + 对象对.中心对象.begindex;
				待生长对象对.处理类型 = 生长_集合处理;
			}
			//关联关系需适当延后
			//人对事动词需适当延后
			//if (Data.是派生类(Data.人对事动作Guid, 中心根对象.源模式行, 替代.正向替代)
			//    || Data.是派生类(Data.人对事动作Guid, 参数根对象.源模式行, 替代.正向替代))
			//{
			//    打分 += 5000; //5000 - 待生长对象对.中心对象.begindex;
			//}
			//推理角色适当延后,显示的推理角色需要做名词来看
			if (Data.是派生类(Data.推理角色Guid, 中心根对象.源模式行, 替代.正向替代))
			{
				打分 += 2000; //5000 - 待生长对象对.中心对象.begindex;
				if (参数对象可序列化 == false)//推理角色与名词的结合需更靠后
					打分 += 2000;
			}
			if (Data.是派生类(Data.推理角色Guid, 参数根对象.源模式行, 替代.正向替代))
			{
				打分 += 2000; //5000 - 待生长对象对.中心对象.长度;
				if (中心对象可序列化 == false) //推理角色与名词的结合需更靠后
					打分 += 2000;
			}
			//显示的推理角色做中心时，生长顺序调整为和动词相同
			if (Data.是派生类(Data.推理角色Guid, 中心根对象.源模式行, 替代.正向替代) && 中心根对象.是介词形式创建的对象 == false
					&& 对象对 != null && 对象对.中心在右)
			{
				打分 = 2500 + 待生长对象对.中心对象.中心第一根类.begindex;
			}

			//靠外层对象延后处理
			if (中心根对象.源模式行.参数.在左端时靠外层级分 > 0)
			{
				if (对象对 != null)// && !对象对.中心在右)
				{
					//if (!判断对象完成性(对象对, 待生长对象对.处理类型) || !参数对象可序列化)
					if (Data.是派生类(Data.推理角色Guid, 参数根对象.源模式行, 替代.正向替代) && 参数根对象.是介词形式创建的对象 == false)
						打分 += 0;
					else
						打分 += 中心根对象.源模式行.参数.在左端时靠外层级分 * 1000 - 待生长对象对.参数对象.长度;
				}
				//else if (中心对象可序列化)
				//{
				//    if (多义性对象获取延后分值(中心根对象, false, true) > 0)
				//        打分 += 1000;
				//}
			}
			if (参数根对象.源模式行.参数.在左端时靠外层级分 > 0 && 中心根对象.源模式行.ID.Equals(Data.事件Guid) == false)
			{
				if (对象对 != null)
				{
					if (已经有了某个语言角色(对象对.参数对象, 字典_语言角色.宾语) == false)
						打分 += 参数根对象.源模式行.参数.在左端时靠外层级分 * 1000 - 待生长对象对.参数对象.长度;
				}
			}
			//短句之间的结合打分(10000以上)
			if (对象对 != null && 是否完整的短句(对象对.左对象, 待生长对象对.所属范围) && 是否完整的短句(对象对.右对象, 待生长对象对.所属范围))
				打分 += 0;
			else
			{
				//中心在右时,参数跨分隔标点符，适当延后
				if (对象对 != null && 对象对.中心在右 && 判断对象左边是否紧挨短句停顿符(待生长对象对.中心对象, 待生长对象对.所属范围)
					&& !判断对象左边是否紧挨短句停顿符(待生长对象对.参数对象, 待生长对象对.所属范围))
					打分 += 10000;
				if (是否已结合短句停顿符(待生长对象对.中心对象) || 是否已结合短句停顿符(待生长对象对.参数对象))
					打分 += 10000;
				//if (对象对 != null && !对象对.中心在右 && 是否已结合短句停顿符(待生长对象对.中心对象))
				//    打分 += 10000;
				//if (对象对 != null && 对象对.中心在右 && 是否已结合短句停顿符(待生长对象对.参数对象))
				//    打分 +=10000;

				//参数是短句停顿，且中心在左边　
				if (对象对 != null && !对象对.中心在右 && 判断对象左边是否紧挨短句停顿符(待生长对象对.中心对象, 待生长对象对.所属范围)
							&& Data.是派生类(Data.短句停顿Guid, 对象对.参数对象.源模式行, 替代.正向替代))
				{
					打分 = 100;
				}
			}
			//完整的句子和短句不允许生长
			if (对象对 != null && ((对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) != null
										&& 对象对.右对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) == null)
								|| (对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) == null
										&& 对象对.右对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) != null)
				))
			{
				待生长对象对.对象对 = null;
			}
			//两个完整的句子，生长类型为全部,主要用于短句之间的并列生长
			else if (对象对 != null && 对象对.左对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) != null
			   && 对象对.右对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid)) != null)
			{
				待生长对象对.处理类型 = 0;
			}
			else if (对象对 != null && 对象对.左对象.begindex == 待生长对象对.所属范围.内在begindex
						&& (对象对.右对象.endindex == 待生长对象对.所属范围.内在endindex
							|| 判断对象的右边是否已生长完成(对象对.右对象, 待生长对象对.所属范围.内在endindex)))
				待生长对象对.处理类型 = 0;
			//中心是哨兵对象
			if (是哨兵对象(中心根对象, false))
				打分 = 1500;
			#endregion
			//打分 -= 待生长对象对.参数对象.长度;
			return 打分;
		}
		public int 计算获得动词对象的靠外围打分(生长对象 动词对象, 封闭范围 范围)
		{
			//从当前动词对象开始，向右寻找动词对象，如果有同等级动词，则需先长右边，所以当前动词对象需延后生长
			for (int i = 在左边界排序对象中定位(动词对象.endindex); i < 左边界排序对象.Count; i++)
			{
				生长对象 右对象 = 左边界排序对象[i];
				if (右对象.中心第一根类 != 右对象) //只寻找一级动词对象
					continue;
				//if (右对象.begindex == 动词对象.endindex)//右边动词紧密结合??
				//    break;
				if (Data.是派生类(Data.短句停顿Guid, 右对象.源模式行, 替代.正向替代))
					break;
				else if (Data.是派生类(Data.句子语用基类Guid, 右对象.源模式行, 替代.正向替代))
					break;
				else if (Data.能够序列化(右对象.源模式行))
				{
					//1.右对象也是基本关联时，不用延后，先生长左边动词
					if (Data.是派生类(Data.基本关联Guid, 动词对象.源模式行, 替代.正向替代)
						&& Data.是派生类(Data.基本关联Guid, 右对象.源模式行, 替代.正向替代))
						return 0;

					//2.右对象也是人对事动作时，先生长右边动词
					else if (Data.是派生类(Data.人对事动作Guid, 动词对象.源模式行, 替代.正向替代) == true
						&& Data.是派生类(Data.人对事动作Guid, 右对象.源模式行, 替代.正向替代) == true)
						return 右对象.begindex - 动词对象.begindex + 3;

					//3.右对象与左边动词都不是人对事动作时，先生长右边动词
					else if (Data.是派生类(Data.人对事动作Guid, 动词对象.源模式行, 替代.正向替代) == false
						&& Data.是派生类(Data.人对事动作Guid, 右对象.源模式行, 替代.正向替代) == false)
					{
						//如果右对象是基本关联，不用延后，先生长左边动词
						if (Data.是派生类(Data.基本关联Guid, 右对象.源模式行, 替代.正向替代))
							return 0;
						else
							return 右对象.begindex - 动词对象.begindex + 3;
					}
					else
						return 右对象.begindex - 动词对象.begindex + 3;
				}

			}
			return 0;
		}
		public bool 是否已结合短句停顿符(生长对象 目标对象)
		{
			参数 参数 = 目标对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid));
			if (参数 != null)
			{
				//if (参数.对端派生对象.源模式行ID.Equals(Data.下文引导冒号Guid))
				//    return false;
				//else
				return true;
			}
			return false;
		}
		public bool 判断对象左边是否紧挨短句停顿符(生长对象 右对象, 封闭范围 范围)
		{
			int i = 在右边界排序对象中定位(右对象.begindex);
			if (右对象.begindex == 范围.begindex)
				return true;
			for (int j = i; j < 右边界排序对象.Count; j++)
			{
				生长对象 左对象 = 右边界排序对象[j];
				if (左对象.endindex < 右对象.begindex)
					break;
				if (左对象.长度 == 1 && 左对象.endindex == 右对象.begindex)
				{
					if (Data.是派生类(Data.短句停顿Guid, 左对象.中心第一根类.模式行, 替代.正向替代)
						|| Data.是派生类(Data.句子语用基类Guid, 左对象.中心第一根类.模式行, 替代.正向替代))
						return true;
				}
			}
			return false;
		}
		public bool 判断对象右边是否紧挨短句停顿符(生长对象 左对象, 封闭范围 范围, bool 包含横向关联介词 = false)
		{
			int i = 在左边界排序对象中定位(左对象.endindex);
			if (左对象.endindex >= 范围.endindex)
				return true;
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 相邻对象 = 左边界排序对象[j];
				if (相邻对象.begindex > 左对象.endindex)
					break;
				if (相邻对象.长度 == 1 && 左对象.endindex == 相邻对象.begindex)
				{
					if (Data.是派生类(Data.短句停顿Guid, 相邻对象.中心第一根类.源模式行, 替代.正向替代)
						|| Data.是派生类(Data.句子语用基类Guid, 相邻对象.中心第一根类.模式行, 替代.正向替代))
						return true;
					if (包含横向关联介词 && 是横向关联介词(相邻对象.中心第一根类))
						return true;
				}
			}
			return false;
		}
		//将待生长对象对，按'生长次序打分'从小到大的顺序插入集合
		public void 插入一个待生长的对象对(关联对象对 对象对)
		{
			//按照生长次序排序进行插入
			int i = 0;
			for (i = 0; i < 待生长对象对集合.Count; i++)
			{
				关联对象对 o = 待生长对象对集合[i];
				if (o.生长次序 > 对象对.生长次序)
					break;
				else if (o.生长次序 == 对象对.生长次序) //生长次序打分相同时，按中心根对象的begindex排序
				{
					if (o.中心对象.中心第一根类.begindex == 对象对.中心对象.中心第一根类.begindex + 1)
						break;
					else if (o.对象对.中心在右 && 对象对.对象对.中心在右 && o.中心对象.中心第一根类.begindex > 对象对.中心对象.中心第一根类.begindex)
						break;
				}
			}
			待生长对象对集合.Insert(i, 对象对);
		}
		//从待生长对象对集合中，取出一个生长次序打分最小的对象对，进行生长。
		//生长成功后，再用生成的新对象，与全部对象，向右、向左的组合出新的待生长对象对，插入待生长对象对集合中
		public bool 进行最优先关联对象对的生长()
		{
			bool 是否可继续生长 = false;
			if (待生长对象对集合.Count > 0)
			{
				关联对象对 对象对 = 获取最优先的待生长对象();
				是否可继续生长 = 尝试生长(对象对);
				if (对象对 != null)
				{
					//当前对象对，如果中心对象存在多义性，则同时完成生长
					List<关联对象对> 同时生长对象对 = new List<关联对象对>();
					foreach (关联对象对 待生长对象对 in 待生长对象对集合)
					{
						//中心对象有多个解释，并且中心对象的类型相同，并且参数对象相同
						if (待生长对象对.中心对象.begindex == 对象对.中心对象.begindex &&
							待生长对象对.中心对象.endindex == 对象对.中心对象.endindex &&
							待生长对象对.中心对象.中心第一根类.源模式行 != 对象对.中心对象.中心第一根类.源模式行 &&
							(Data.是派生类(Data.事件Guid, 对象对.中心对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.事件Guid, 待生长对象对.中心对象.中心第一根类.源模式行, 替代.正向替代) ||
							 Data.是派生类(Data.事物概念Guid, 对象对.中心对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 待生长对象对.中心对象.中心第一根类.源模式行, 替代.正向替代)) &&
							待生长对象对.中心对象.概率分 >= 对象对.中心对象.概率分 &&
							待生长对象对.参数对象 == 对象对.参数对象 &&
							/*待生长对象对.参数对象.begindex == 对象对.参数对象.begindex &&
							待生长对象对.参数对象.endindex == 对象对.参数对象.endindex &&*/
							待生长对象对.已处理 == false)
						{
							同时生长对象对.Add(待生长对象对);
						}
						//中心对象相同，参数对象有多个解释，并且参数对象的类型相同
						else if (待生长对象对.中心对象 == 对象对.中心对象 &&
							(Data.是派生类(Data.事件Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.事件Guid, 待生长对象对.参数对象.中心第一根类.源模式行, 替代.正向替代) ||
							 Data.是派生类(Data.事物概念Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代) && Data.是派生类(Data.事物概念Guid, 待生长对象对.参数对象.中心第一根类.源模式行, 替代.正向替代)) &&
							待生长对象对.参数对象.概率分 >= 对象对.参数对象.概率分 &&
							待生长对象对.参数对象.begindex == 对象对.参数对象.begindex &&
							待生长对象对.参数对象.endindex == 对象对.参数对象.endindex &&
							待生长对象对.已处理 == false)
						{
							同时生长对象对.Add(待生长对象对);
						}

					}
					foreach (关联对象对 待生长对象对 in 同时生长对象对)
					{
						是否可继续生长 = 尝试生长(待生长对象对) || 是否可继续生长;
					}
				}
			}
			return 是否可继续生长;
		}
		public bool 尝试生长(关联对象对 对象对)
		{
			if (对象对 != null && 对象对.已处理 == false)
			{
				Data.输出串("尝试生长:【" + 对象对.中心对象.取子串 + "】(概率分:" + 对象对.中心对象.概率打分 + ")--【" + 对象对.参数对象.取子串 + "】(概率分:" + 对象对.参数对象.概率打分 + "):" + 对象对.生长次序);
				if (对象对.中心对象.取子串 == "上" && 对象对.参数对象.取子串 == "昨天上午")
					处理轮数 = 处理轮数;
				对象对.实际生长顺序 = 处理轮数++;
				int i;
				if (对象对.处理类型 == 0)
					i = 组织对象对进行生长(对象对, true, 对象对.所属范围, 对象对.所属范围.begindex, 对象对.所属范围.endindex, -1);
				else
					i = 组织对象对进行生长1(对象对, true, 对象对.所属范围, 对象对.所属范围.begindex, 对象对.所属范围.endindex, 对象对.处理类型, -1);
				if (i > 0)
				{
					bool 左对象序列化 = Data.能够序列化(对象对.对象对.左对象.中心第一根类.模式行);
					bool 右对象序列化 = Data.能够序列化(对象对.对象对.右对象.中心第一根类.模式行);
					//优先尝试右边名词词组将进行拆解的进行拆解性的生长。
					if (左对象序列化 == true && 右对象序列化 == false)
						进行左动右名的正常语序和从定的多义性处理(对象对.对象对.左对象, 对象对.对象对.右对象, 对象对.所属范围, 对象对.所属范围.begindex, 对象对.所属范围.endindex, -1);
					对象对.成功生长 = true;
					对本轮结果集进行打分抑制处理();
					bool ret = false;
					foreach (生长对象 o in 本轮结果集)
					{
						Data.输出串("                                     生成：【" + o.取子串 + "】(概率分:" + o.概率打分 + ")");
						if (o.取子串 == "由于华为、中兴的快速成长")
							ret = ret;
						if (o.长度 >= (对象对.中心对象.长度 + 对象对.参数对象.长度) && o.概率打分 > 0)
						{
							进行附加关联处理(o);
							加入一个对象到池(o);
							根据新对象取消交叉对象的抑制性(o);
							//if (o.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid))!=null)
							//    o.参数.概率分=9;
							//if (o.是省略对象的结合对象()) //省略对象不抑制其它对象生长
							//    o.取消抑制性 = true;
							ret = 双向组合出待生长对象对并插入待生长集合(o, 对象对.所属范围, 对象对.所属范围.打分起始分, false, false) || ret;
						}
					}

				}
				else
				{
					Data.输出串("                                     生成不成功!");
					//取消失败对象的抑制性(对象对);
				}
				本轮结果集.Clear();
				对象对.已处理 = true;
				return true;
			}
			return false;
		}

		//从待生长对象对集合中，取出一个生长次序打分最小，并且不被抑制的对象对
		public 关联对象对 获取最优先的待生长对象()
		{
			关联对象对 被推后对象对 = null;
			foreach (关联对象对 对象对 in 待生长对象对集合)
			{
				if (对象对.参数对象.取子串 == "普京在会晤希腊总理齐普拉斯" && 对象对.中心对象.取子串 == "后" &&
					Data.是派生类(Data.基准相对时间Guid, 对象对.中心对象.源模式行, 替代.正向替代))
					对象对.处理类型 = 对象对.处理类型;
				if ((!对象对.已处理 && 计算对象对被抑制性(对象对.对象对) == false && 计算对象在待生长集合中的被抑制性(对象对) == false))
				//||(!对象对.已处理 && 对象对.被推后生长) )
				//&& !是否被完整的短句抑制(对象对.中心对象) && !是否被完整的短句抑制(对象对.参数对象))
				{
					if (对象对.参数对象.取消抑制性 == false && 对象对.中心对象.取消抑制性 == false)
						return 对象对;
					else
					{
						Data.输出串("被推后生长:【" + 对象对.中心对象.取子串 + "】--【" + 对象对.参数对象.取子串 + "】:" + 对象对.生长次序);
						对象对.被推后生长 = true;
						if (被推后对象对 == null)
							被推后对象对 = 对象对;
					}
				}
			}
			return 被推后对象对;
		}
		public bool 是一级对象的拆解(生长对象 原对象, 生长对象 拆解对象)
		{
			if (原对象 != 拆解对象 && 原对象.中心第一根类 == 原对象 && 拆解对象.中心第一根类 == 拆解对象)
			{
				if (拆解对象.begindex >= 原对象.begindex && 拆解对象.endindex <= 原对象.endindex && 拆解对象.长度 < 原对象.长度)
					return true;
			}
			return false;
		}
		//计算对象对在待生长对象对集合中的被抑制性
		public bool 计算对象在待生长集合中的被抑制性(关联对象对 目标对象)
		{
			foreach (关联对象对 已有对象 in 待生长对象对集合)
			{
				if (目标对象 != 已有对象 && 已有对象.已处理 == false)
				{
					if (目标对象.对象对.中心对象 == 已有对象.对象对.中心对象 && 是一级对象的拆解(已有对象.对象对.参数对象, 目标对象.对象对.参数对象) && 已有对象.对象对.参数对象.取消抑制性 == false)
						return true;
					if (目标对象.对象对.参数对象 == 已有对象.对象对.参数对象 && 是一级对象的拆解(已有对象.对象对.中心对象, 目标对象.对象对.中心对象) && 已有对象.对象对.中心对象.取消抑制性 == false)
						return true;

					bool 中心抑制性 = 目标对象.中心对象 == 已有对象.中心对象 ? false : 计算单对象被抑制性(目标对象.中心对象, 已有对象.中心对象, true);
					bool 参数抑制性 = 目标对象.参数对象 == 已有对象.参数对象 ? false : 计算单对象被抑制性(目标对象.参数对象, 已有对象.参数对象, true);
					if (中心抑制性 && 参数抑制性)
					{
						return true;
					}
					////如果左对象相同，右对象被包含抑制，则认为应该被抑制生长
					if (目标对象.对象对.左对象 == 已有对象.对象对.左对象
						&& 计算单对象被抑制性(目标对象.对象对.右对象, 已有对象.对象对.右对象, true))
					{
						//if (已有对象.对象对.右对象.概率分 >=目标对象.对象对.右对象.概率分)
						//{
						if (目标对象.对象对.中心在右 && Data.能够序列化(目标对象.对象对.中心对象.中心第一根类.模式行) && 是后置宾语(目标对象.对象对.参数对象, 已有对象.对象对.右对象))
							continue;
						if (已有对象.对象对.右对象.是省略对象的结合对象() == false)
							return true;
						//}

					}
					//如果右对象相同，左对象被包含抑制，则认为应该被抑制生长
					if (目标对象.对象对.右对象 == 已有对象.对象对.右对象
						&& 计算单对象被抑制性(目标对象.对象对.左对象, 已有对象.对象对.左对象, true))
					{
						//if (已有对象.对象对.左对象.概率分 >= 目标对象.对象对.左对象.概率分)
						//{
						if (目标对象.对象对.中心在右 && Data.能够序列化(目标对象.对象对.中心对象.中心第一根类.模式行) && 是后置宾语(目标对象.对象对.参数对象, 已有对象.对象对.左对象))
							continue;
						if (已有对象.对象对.左对象.是省略对象的结合对象() == false)
							return true;
						//}

					}
					//如果左对象串相同，右对象被包含抵制，则认为应该被抑制生长
					if (目标对象.对象对.左对象.begindex == 已有对象.对象对.左对象.begindex && 目标对象.对象对.左对象.endindex == 已有对象.对象对.左对象.endindex
						&& 计算单对象被抑制性(目标对象.对象对.右对象, 已有对象.对象对.右对象, true))
					{
						if (目标对象.对象对.中心在右 && Data.能够序列化(目标对象.对象对.中心对象.中心第一根类.模式行) && 是后置宾语(目标对象.对象对.参数对象, 已有对象.对象对.右对象))
							continue;
						if (已有对象.对象对.右对象.是省略对象的结合对象() == false)
							return true;
					}
					//如果右对象串相同，左对象被包含抵制，则认为应该被抑制生长
					if (目标对象.对象对.右对象.begindex == 已有对象.对象对.右对象.begindex && 目标对象.对象对.右对象.endindex == 已有对象.对象对.右对象.endindex
						&& 计算单对象被抑制性(目标对象.对象对.左对象, 已有对象.对象对.左对象, true))
					{
						if (目标对象.对象对.中心在右 && Data.能够序列化(目标对象.对象对.中心对象.中心第一根类.模式行) && 是后置宾语(目标对象.对象对.参数对象, 已有对象.对象对.左对象))
							continue;
						if (已有对象.对象对.左对象.是省略对象的结合对象() == false)
							return true;
					}
					//参数对象相同时，概率分高的先生长
					if (已有对象.参数对象.begindex == 目标对象.参数对象.begindex && 已有对象.参数对象.endindex == 目标对象.参数对象.endindex)
					{
						if (已有对象.参数对象.概率分 > 目标对象.参数对象.概率分 && 已有对象.中心对象.begindex == 目标对象.中心对象.begindex && 已有对象.中心对象.endindex == 目标对象.中心对象.endindex)
							return true;
						else if (已有对象.中心对象.概率分 > 目标对象.中心对象.概率分 && 目标对象.中心对象.是省略对象的结合对象())
							return true; //省略对象推迟生长
						//else if (已有对象.参数对象.概率分 == 目标对象.参数对象.概率分 && 
						//            已有对象.中心对象.概率分-目标对象.中心对象.概率分>=2)
						//    return true;
					}
					//中心对象相同时，概率分高的先生长
					if (已有对象.中心对象.begindex == 目标对象.中心对象.begindex && 已有对象.中心对象.endindex == 目标对象.中心对象.endindex)
					{
						if (已有对象.中心对象.概率分 > 目标对象.中心对象.概率分 && 已有对象.参数对象.begindex == 目标对象.参数对象.begindex && 已有对象.参数对象.endindex == 目标对象.参数对象.endindex)
							return true;
						else if (已有对象.参数对象.概率分 > 目标对象.参数对象.概率分 && 目标对象.参数对象.是省略对象的结合对象() && 目标对象.对象对.前置介词 != null)
							return true; //省略对象推迟生长
						//else if (已有对象.中心对象.概率分 ==目标对象.中心对象.概率分 &&
						//            已有对象.参数对象.概率分-目标对象.参数对象.概率分>=2)
						//    return true;
					}


				}
			}
			return false;
		}

		public bool 计算可生长抑制性(生长对象 目标对象, 生长对象 已有对象)
		{
			if (已有对象.begindex == 目标对象.begindex && 已有对象.endindex == 目标对象.endindex)
			{
				if (已有对象 == 目标对象)
					return true;
				else
					return false;
			}
			else if (已有对象.begindex <= 目标对象.begindex && 已有对象.endindex >= 目标对象.endindex)
			{
				return 计算多意性抑制(目标对象, 已有对象);
			}
			return false;
		}
		public void 对本轮结果集进行打分抑制处理()
		{
			本轮结果集.Sort((生长对象 v1, 生长对象 v2) =>
			{
				int k1 = v1.概率打分;
				int k2 = v2.概率打分;
				if (k1 == k2)
					return 0;
				return k1 > k2 ? -1 : 1;
			});

			//对于相同中心的结果，我们只保留一个，别的打分都归为0，放置着。
			for (int i = 0; i < 本轮结果集.Count - 1; i++)
			{
				生长对象 obj = 本轮结果集[i];

				if (obj.处理轮数 == -2 || obj.概率打分 <= 0)
					continue;

				for (int j = i + 1; j < 本轮结果集.Count; j++)
				{
					生长对象 obj1 = 本轮结果集[j];

					if (obj1.处理轮数 == -2 || obj1.概率打分 <= 0)
						continue;

					if (obj1.begindex != obj.begindex || obj1.endindex != obj.endindex)
						continue;

					if (obj1.中心第一根类 == obj.中心第一根类)
					{
						if (obj1.概率打分 <= 0)
							break;
						obj1.参数.概率分 = -9;
					}

				}

				//根据已生长新对象反向去除抑制对象的抑制性(obj);
			}
		}

		public void 根据已生长新对象反向去除抑制对象的抑制性(生长对象 新对象)
		{
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.endindex <= 新对象.begindex)
					continue;
				if (obj.begindex >= 新对象.endindex)
					break;
				if (obj.中心对象 == null)
					continue;
				//int k = 生长对象.计算位置重叠性(新对象, obj);
				//if (k > 0)
				obj.取消抑制性 = true;
			}

		}
		//在待生长对象对集合中，检查指定对象对中的对象，是否已经都被生长尝试生长完成，如果完成则取消其抑制性
		public void 取消失败对象的抑制性(关联对象对 关联对象对)
		{
			//1.先检查左对象
			生长对象 检查对象 = 关联对象对.对象对.左对象;
			bool 生长完成 = true;
			if (检查对象.中心第一根类 != 检查对象)
			{
				foreach (关联对象对 待生长对象对 in 待生长对象对集合)
				{
					if (待生长对象对 == 关联对象对)
						continue;
					if ((待生长对象对.参数对象 == 检查对象 || 待生长对象对.中心对象 == 检查对象) && 待生长对象对.已处理 == false)
					{
						生长完成 = false;
						break;
					}
				}
				if (生长完成)
				{
					检查对象.取消抑制性 = true;
					检查对象.已进行所有可能的生长 = true;
					Data.输出串("      生长失败时，取消了【" + 检查对象.取子串 + "】的抑制性.");
				}
			}
			//2.再检查右对象
			检查对象 = 关联对象对.对象对.右对象;
			if (检查对象.中心第一根类 != 检查对象)
			{
				生长完成 = true;
				foreach (关联对象对 待生长对象对 in 待生长对象对集合)
				{
					if (待生长对象对 == 关联对象对)
						continue;
					if ((待生长对象对.参数对象 == 检查对象 || 待生长对象对.中心对象 == 检查对象) && 待生长对象对.已处理 == false)
					{
						生长完成 = false;
						break;
					}
				}
				if (生长完成)
				{
					检查对象.取消抑制性 = true;
					检查对象.已进行所有可能的生长 = true;
					Data.输出串("      生长失败时，取消了【" + 检查对象.取子串 + "】的抑制性.");
				}
			}
		}
		public void 根据新对象取消交叉对象的抑制性(生长对象 新对象, bool 是新词汇 = false)
		{
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.endindex <= 新对象.begindex)
					continue;
				if (obj.begindex >= 新对象.endindex)
					break;
				if (obj.中心对象 == null)
					continue;
				if (新对象 == obj)
					continue;
				if (是新词汇)
				{
					if (obj.begindex < 新对象.endindex && obj.endindex > 新对象.endindex
					|| obj.begindex > 新对象.begindex && obj.endindex > 新对象.begindex) //是交叉对象
					{
						obj.取消抑制性 = true;
						Data.输出串("      新词汇:【" + 新对象.取子串 + "】检测到交叉，取消了【" + obj.取子串 + "】的抑制性.");
					}
					else if (obj.begindex >= 新对象.begindex && obj.endindex <= 新对象.endindex && obj.已进行所有可能的生长 == false) //被新对象包含
						obj.取消抑制性 = false;
				}
				else
				{
					if (obj.begindex < 新对象.begindex && obj.endindex < 新对象.endindex) //是交叉对象
					{
						obj.取消抑制性 = true;
						Data.输出串("      新对象:【" + 新对象.取子串 + "】检测到交叉，取消了【" + obj.取子串 + "】的抑制性.");
					}
					else if (obj.begindex >= 新对象.begindex && obj.endindex <= 新对象.endindex && obj.已进行所有可能的生长 == false) //被新对象包含
						obj.取消抑制性 = false;
				}
			}
		}
		public void 启动特殊生长解决问题(int 语义阀值, 封闭范围 范围)
		{
			寻找间隙();//查找不能生长下去的关键点。可能有多个，要进行排序。

			//下边对间隙进行处理
			//【隐含对象】：其实都是进行隐含对象的处理。
			对间隙进行处理(语义阀值, 范围);//比如【3对2说】。

			新对象生长();//新对象往往是字符串，因此，这时至少存在两个间隙，两个阀值大的对象中间夹的字符串可能就是一个新对象。

			忽略并跳跃生长();//跳过并忽略中间不合适的对象进行尝试。这个可能要把所有的对象都对比一下，看有两个中间间隔少，同时两者语义分析很密切的来进行。
		}

		public void 寻找间隙()
		{
			间隙数组 = new List<间隙>();
			for (int i = 0; i < Data.当前句子串.Length + 1; i++)
				间隙数组.Add(new 间隙(0, i));

			//foreach (生长对象 obj in 全部对象)
			//{
			//	for (int i = obj.begindex + 1; i < obj.endindex; i++)
			//		间隙数组[i] += obj.生长优先分;//对于每个对象，内部的间隙【融合值】是正的，也就是间隙被【焊接】。
			//	间隙数组[obj.begindex] = -obj.生长优先分;//每个对象两端对外的【融合值】是负的，也就是这个间隙是【隔离】的。
			//	间隙数组[obj.endindex] = -obj.生长优先分;
			//}
			foreach (生长对象 obj in 全部对象)
			{
				for (int i = obj.begindex + 1; i < obj.endindex; i++)
					间隙数组[i].融合度 += 1;//对于每个对象，内部的间隙【融合值】是正的，也就是间隙被【焊接】。
			}
			间隙数组.RemoveAt(0);
			间隙数组.RemoveAt(间隙数组.Count - 1);
			间隙数组.Sort();
		}

		public void 新对象生长()
		{
		}

		public int 对间隙进行处理(int 语义阀值, 封闭范围 范围)
		{
			int 结果打分 = 0;
			int k = 本轮结果集.Count;
			int index;

			for (int i = 0; i < 间隙数组.Count; i++)//遍历每一个间隙，每个间隙如果有【的】都进行补齐。
			{
				if (间隙数组[i].融合度 > 0)
					continue;
				int 间隙位置 = 间隙数组[i].位置索引;


				//二、然后进行【的】后的空对象的处理
				//这个间隙在【的】的前边位置触发，比如【红|的是美丽|的】，这样的话肯定和其它处理不冲突，末尾的【的】也不会错过。
				//index = 在左边界排序对象中定位(间隙位置);
				//if (index < 0 || index >= 左边界排序对象.Count)
				//    continue;
				//生长对象 的 = 左边界排序对象[index];
				//if (的.中心对象 == null && 的.是的或者地(true, false))
				//{
				//    index = 在右边界排序对象中定位(间隙位置);
				//    生长对象 左边参数对象 = 右边界排序对象[index];
				//    //if (左边参数对象.是NullThis空对象())
				//    //    continue;
				//    if (左边参数对象.是介词或者串(true, true, true))
				//        continue;

				//    生长对象 未知对象 = 推断未知对象并进行生长(左边参数对象, new SubString(的.endindex, 的.endindex), false, true);//启动名词谓语生长。
				//    if (未知对象 != null)//对一个间隙处理已经得到了结果，那么就不继续了。
				//        break;

				//}
			}

			结果打分 = 计算本轮生长最大打分();
			if (结果打分 > 0)
				return 结果打分;


			//   //这是降低阀值进行处理不过暂时屏蔽。
			// for (int i = 0; i < 间隙数组.Count; i++)//遍历每一个间隙。
			//{
			//    //循环处理所有对象(语义阀值, 间隙位置);
			//}

			return 计算本轮生长最大打分();
			//暂时不处理独立语，如果要处理，要放到最后，接近于选择结果的时候处理
			//1、最后处理，这时【右边的模式】应该是完成的。而【左边】留下独立语和更左边的【状语】。并且是【并列】关系。
			//2、这样的处理就要一次完成，要计算关联，并把【右边模式】和左边的【独立语】，更左边的【状语】一次全部生成完成！而不在是尝试性的生长。

			//独立语生长：是最后处理。
			foreach (生长对象 右边谓语模式 in 全部对象)
			{
				//独立语必须是，一边是一个【完整模式】，而另一边是一个【概念】
				//语义上【概念】可以结合到【完整模式】内部的一个参数上，但无法直接结合起来。
				//实际上，就是两个句子，相互是间接引用。
				//【他们，损失惨重】
				if (右边谓语模式.处理轮数 == -2 || Data.能够序列化(右边谓语模式.中心第一根类.模式行) == false)//右边模式一定是可以序列化的。而且最好应该进行计算。
					continue;
				int r = 在右边界排序对象中定位(右边谓语模式.begindex);
				while (r >= 0 && r < 右边界排序对象.Count)
				{
					生长对象 独立主语 = 右边界排序对象[r];
					if (独立主语.处理轮数 != -2 && 独立主语.begindex == 0)//暂时要求是最左边的，但也可能还有别的状语。
					{
						处理选项 |= 处理独立语;
						int oldcount = 本轮结果集.Count;
						循环处理所有对象(范围, 范围.begindex, 范围.endindex);//不需要用指定中心和指定参数的方法来生长了，因为可以用轮数来控制。
						处理选项 ^= 处理独立语;
						if (本轮结果集.Count - oldcount > 0)
							return 本轮结果集.Count - oldcount;
					}
					r++;
				}
			}

			return 计算本轮生长最大打分();
		}

		public void 按照形式和语言角色对三级关联进行过滤和排序(List<三级关联> 结果, SubString 参数对象串, bool 中心在右, int 允许的语言角色 = 字典_语言角色.全部)
		{

		}

		public 生长对象 推断未知对象并进行生长(生长对象 已知对象, SubString 未知对象串, bool 已知对象在右, bool 假设中心在右)
		{
			//List<三级关联> 结果 = new List<三级关联>();
			//if (已知对象在右)
			//	计算三级关联(null, 已知对象, 假设中心在右, 结果);
			//else
			//	计算三级关联(已知对象, null, 假设中心在右, 结果);

			////如果已知对象是B端，待求对象是A端，那么得到的对象就是直接的对象了，比如【红色的】计算出【物质拥有颜色】从而得到【颜色】。
			////如果已知对象是A端，待求对象是B端，那么得到的对象可能是【角色】，然后再寻找一次【聚合】。比如【借出】求出了【借出拥有借出方】，然后求出【人角色反聚人】从而得到【人】。

			//按照形式和语言角色对三级关联进行过滤和排序(结果, 未知对象串, 假设中心在右);

			string 形式 = 未知对象串.长度 == 0 ? "[nullthis]" : 未知对象串.ToString();
			生长对象 空事物对象 = 创建或返回隐藏对象(Data.ThisGuid, 形式, 未知对象串.begindex, 未知对象串.begindex, 已知对象.中心第一根类.中心对象, -2, true);
			if (空事物对象 == null)
				return null;
			加入结果集排除掉相同的(空事物对象).参数.概率分 = 9;//这里先这么设定，实际上，这是需要进行语义计算来得到的。

			生长对象 对象对 = 未知关联构造待分析对象对(空事物对象, 已知对象);

			只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(对象对);

			Guid 关联 = Data.基本关联Guid;
			if (Data.是二元关联(已知对象.源模式行, true))//如果前边对象是【属于】【拥有】等，那么一般就是【拥有B端】，而且语言角色就是从句，比如【拥有的**】。
			{
				if (已知对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有B端Guid)) != null)
				{
					if (已知对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有A端Guid)) == null)
						关联 = Data.关联拥有A端Guid;
				}
				else
					关联 = Data.关联拥有B端Guid;
			}
			对象对.设置源模式行(Data.FindRowByID(关联));
			if (Data.能够序列化(已知对象.源模式行))
				对象对.that = 字典_目标限定.B端;
			return 直接一级关联生长(对象对, 处理轮数, false);
		}

		public int 忽略并跳跃生长()
		{
			int 结果打分 = 0;
			return 结果打分;
		}

		//【他有钱花】【有钱买衣服】等
		//一般左边都是【有】，而右边是一个动词，动词可以吸纳左边的宾语。
		public void 尝试进行补语生长(生长对象 对象对)
		{
			if (对象对.中心在右 == true || Data.是派生关联(Data.拥有Guid, 对象对.中心对象.中心第一根类.源模式行) == 0)
				return;

			List<三级关联> 结果 = new List<三级关联>();

			//if (对象中间由的或地关联(对象对) == 1 && 对象对.中心在右)
			//{
			//	//从句，【损失惨重的他】
			//	if (Data.能够序列化(对象对.参数对象.中心第一根类.模式行) == false)
			//		return;
			//	if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行))
			//		return;
			//	计算独立语关联(对象对.中心对象, 对象对.参数对象, 结果);
			//}
			//else
			//{
			//【他损失惨重】

			计算补语关联(对象对.中心对象, 对象对.参数对象, 结果);
			//}

			执行对象对生长(对象对, 结果);
		}

		//处理【属主独立语】
		//【衬衣他买了】这种不算这里的独立语
		public void 尝试进行独立语生长(生长对象 对象对/*, int 语义阀值*/)
		{

			List<三级关联> 结果 = new List<三级关联>();

			if (对象中间由的或地关联(对象对) == 1 && 对象对.中心在右)
			{
				//从句，【损失惨重的他】
				if (Data.能够序列化(对象对.参数对象.中心第一根类.模式行) == false)
					return;
				if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行))
					return;
				计算独立语关联(对象对.中心对象, 对象对.参数对象, 结果);
			}
			else
			{
				//【他损失惨重】
				if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) == false)
					return;
				if (Data.能够序列化(对象对.参数对象.中心第一根类.模式行))
					return;
				计算独立语关联(对象对.参数对象, 对象对.中心对象, 结果);
			}

			执行对象对生长(对象对, 结果);
		}

		public bool 已经有非封闭区间内的句号(生长对象 对象)
		{
			参数 o = 对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥句子Guid));
			if (o == null)
				return false;
			生长对象 句号 = o.对端派生对象.B端对象;
			if (内部范围.递归判断一个对象被包含在封闭范围内(对象.begindex, 对象.endindex, 句号.begindex, 句号.endindex))
				return false;
			return true;
		}

		public void 尝试进行名词谓语生长(生长对象 发起对象对, bool 中心在右, int 右边界/*, int 语义阀值*/)
		{
			if (发起对象对.中心对象.是隐藏对象() || 发起对象对.参数对象.是隐藏对象())
				return;

			生长对象 名词谓语 = 发起对象对.中心对象;
			生长对象 A端对象 = null;
			模式 关联 = null;
			生长对象 状语对象 = null;
			模式 状语关联 = null;


			if (已经有非封闭区间内的句号(名词谓语))
				return;
			if (判断对象的右边是否紧挨的或者地(名词谓语) && 判断对象的右边是否紧挨的和短句间隔(名词谓语, 右边界) == false)
				return;
			//检查是否已经结合符合程度
			if (名词谓语.查找已经实现的参数(Data.FindRowByID(Data.概念反拥符合程度Guid)) == null)
			{
				//检查右边是否相邻生存阶段“了”，如“漂亮了”
				if (判断相邻对象是否为指定类型(名词谓语, Data.生存阶段Guid, true, true) == false)
					return;
			}
			//1、先确定名词谓语是否可以。
			生长对象 计算对象对 = null;
			bool 已经有的 = false;
			if (名词谓语.中心第一根类.是NullThis空对象())
			{
				//【苹果红的了】。
				已经有的 = true;
				if (名词谓语.参数对象 != null && Data.能够作为名词谓语(名词谓语.参数对象.中心第一根类.模式行))
					计算对象对 = 名词谓语.参数对象;
			}
			else if (Data.能够作为名词谓语(名词谓语.中心第一根类.模式行))
				计算对象对 = 名词谓语.中心第一根类;

			if (计算对象对 == null)
				return;
			//名词谓语，参数不能为动词
			//if (Data.能够序列化(发起对象对.参数对象.中心第一根类.模式行))
			//    return;

			关联 = 根据两个参数计算二元关联(发起对象对.参数对象, 名词谓语, null, true);
			if (关联 != null)//2、看是否合适的名词谓语的主语，比如【桔子很红】。
				A端对象 = 发起对象对.参数对象;
			else
			{
				//3、看是否合适的状语对象。比如【桔子红了】。
				//if (状语对象 != null && Data.是派生类(Data.生存阶段Guid, 发起对象对.参数对象.源模式行, 替代.正向替代) == false)
				//{
				if (Data.是派生类(Data.事物概念Guid, 发起对象对.参数对象.中心第一根类.模式行, 替代.正向替代) == false)
				{
					状语关联 = 参数端是很明显的动词状语且不是主语宾语(发起对象对);
					if (状语关联 == null)
						return;
				}
				状语对象 = 发起对象对.参数对象;
				//}
			}


			//if (已经有的)
			计算对象对 = 名词谓语;
			//else
			//{
			//    生长对象 的对象 = 构造隐藏对象(Data.ThisGuid, "[nullthis]", 名词谓语.endindex, 名词谓语.中心第一根类.中心对象/*应该是串对象*/, -2);
			//    加入结果集排除掉相同的(的对象);
			//    计算对象对 = 生长对象.未知关联构造待分析对象对(的对象, 名词谓语);
			//    //计算对象对.反向关联 = o.Value.反向关联;
			//    计算对象对.设置源模式行(Data.FindRowByID(Data.基本关联Guid));
			//    进行直接的一级关联生长(计算对象对, -2, true);
			//}

			//这里的【隐含关联】就是为后边这个名词谓语服务，所以层级设置为一，只允许在这里进行一次生长，而不会和别的生长。
			生长对象 实际谓语 = 创建或返回隐藏对象(关联 == null ? Data.基本关联Guid : 关联.ID, 关联 == null ? "[基本关联]" : 关联.形式, 名词谓语.begindex, 名词谓语.begindex, 名词谓语.中心第一根类.中心对象/*应该是串对象*/, -2, false);
			if (实际谓语 == null)
				return;
			生长对象 对象对 = 未知关联构造待分析对象对(实际谓语, 计算对象对);
			if (对象对 == null)
				return;
			加入结果集排除掉相同的(实际谓语);
			对象对.设置源模式行(Data.FindRowByID(Data.关联拥有B端Guid));
			对象对 = 直接一级关联生长(对象对, 处理轮数, true);
			if (对象对 == null)
			{
				本轮结果集.Clear();
				return;
			}
			if (A端对象 != null)
			{
				对象对.处理轮数 = -2;
				对象对 = 未知关联构造待分析对象对(对象对, A端对象);
				对象对.设置源模式行(Data.FindRowByID(Data.关联拥有A端Guid));
				对象对 = 直接一级关联生长(对象对, 处理轮数, true);
				if (对象对 == null)
				{
					本轮结果集.Clear();
					return;
				}
			}
			else if (状语对象 != null)
			{
				对象对.处理轮数 = -2;
				对象对 = 未知关联构造待分析对象对(对象对, 状语对象);
				if (状语关联 == null)
				{
					//只通过字符形式为参数对象查找相邻介词和逗号以及可舍弃字符(对象对);//中心对象总是【干净】的，两边没有【的】【地】【介词】，这些介词附着在参数对象上。
					//if (加上所有部件后对象对是相邻的(对象对) == false)//不相邻。就不生长，等待别人生长。
					//	return;
					尝试进行正常语义关联生长(对象对/*, 语义阀值*/);
				}
				else
				{
					对象对.设置源模式行(状语关联);
					对象对 = 直接一级关联生长(对象对, 处理轮数, true);
				}
				if (对象对 == null)
				{
					本轮结果集.Clear();
					return;
				}
			}
		}



		//public 生长对象 让一个对象至少进行了一次生长(生长对象 发起对象,bool 方向向右)
		//{
		//	生长对象 r = null;
		//	while (true)
		//	{

		//		循环处理所有对象(2, 发起对象);

		//	}
		//	if (方向向右)
		//	{
		//		int 位置 = 发起对象.endindex;
		//		Data.Assert(位置<Data.当前句子串.Length);
		//	}
		//	else
		//	{
		//		int 位置 = 发起对象.begindex;
		//		Data.Assert(位置 > 0);
		//		循环处理所有对象(2, 发起对象);
		//	}
		//	return r;
		//}

		//public 生长对象 指定一个对象进行最大可能的生长(生长对象 发起对象)
		//{
		//	return null;
		//}

		public 生长对象 查找介动词后生存(生长对象 对象, ref int 字符位置, int 左边界)
		{
			if (字符位置 <= 左边界)
				return null;

			生长对象 逗号 = 查找逗号(对象, ref 字符位置, 左边界, true);
			//if (逗号 != null)
			//{
			//    字符位置 = 逗号.begindex;
			//    if (字符位置 <= 左边界)
			//        return null;
			//}
			foreach (生长对象 obj in 右边界排序对象)
			{
				if (obj.endindex < 左边界)
					return null;
				if (obj.endindex > 字符位置)
					continue;
				if (obj.begindex < 左边界)
					continue;
				if (obj.是介词或者串(false, false, true))
					continue;
				SubString str = new SubString(obj.begindex, obj.endindex);
				if (Data.是介动词后生存串(str.ToString()))//因为是字符串的生成，这里依赖的是模式行，而不是源模式行。
				{
					if (两个位置是相邻的(obj.endindex, 字符位置))
					{
						对象.介动词后生存 = obj;
						字符位置 = obj.begindex;
						if (逗号 != null)
							对象.介动词后生存逗号 = 逗号;
						return obj;
					}
				}
			}
			return null;
		}

		public bool 和强势完成对象冲突(生长对象 对象)
		{
			foreach (生长对象 obj in 强势完成对象集合)
			{
				if (SubString.一个对象拆分了另一个对象(对象, obj))
					return true;
			}
			return false;
		}

		//介词和对象之间可以有空格，但不能有逗号。
		//其他任何对象之间都可能有空格也可能有逗号。

		public 生长对象 查找前置介词(生长对象 对象, ref int 字符位置, int 左边界)
		{
			if (字符位置 <= 左边界)
				return null;

			生长对象 逗号 = 查找逗号(对象, ref 字符位置, 左边界, true);
			//if (o != null)
			//{
			//    字符位置 = o.begindex;
			//    if (字符位置 <= 左边界)
			//        return null;
			//}
			foreach (生长对象 obj in 右边界排序对象)
			{
				if (obj.endindex < 左边界)
					return null;
				if (obj.endindex > 字符位置)
					continue;
				if (obj.begindex < 左边界)
					continue;
				if (和强势完成对象冲突(obj))
					continue;
				if (Data.是介词或者串(obj.中心第一根类.模式行, true, true, false))//因为是字符串的生成，这里依赖的是模式行，而不是源模式行。
				{
					if (obj.是的或者地(true, true))
						return null;
					if (obj.是介词或者串(true, false, false) == false)
						return null;
					if (两个位置是相邻的(obj.endindex, 字符位置))
					{
						对象.前置介词 = obj;
						字符位置 = obj.begindex;
						if (逗号 != null)
							对象.前置介词逗号 = 逗号;
						return obj;
					}
				}
			}
			return null;
		}

		public int 在左边界排序对象中定位(int 位置)
		{
			for (int i = 0; i < 左边界排序对象.Count; i++)
			{
				生长对象 obj = 左边界排序对象[i];
				if (obj.begindex < 位置)
					continue;
				return i;
				//if (位置 < obj.begindex)
				//    break;
				//if (位置 == obj.begindex)
				//    return i;
			}
			return 左边界排序对象.Count;
		}


		public int 在右边界排序对象中定位(int 位置)
		{
			for (int i = 0; i < 右边界排序对象.Count; i++)
			{
				生长对象 obj = 右边界排序对象[i];
				if (obj.endindex > 位置)
					continue;
				return i;
				//if (位置 > obj.endindex)
				//    break;
				//if (位置 == obj.endindex)
				//    return i;
			}
			return 左边界排序对象.Count;
		}


		public 生长对象 查找后置介词(生长对象 对象, ref int 字符位置, int 边界)
		{
			if (字符位置 >= 边界)
				return null;
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.begindex > 边界)
					break;
				if (obj.begindex < 字符位置)
					continue;
				if (obj.endindex > 边界)
					continue;
				if (和强势完成对象冲突(obj))
					continue;
				if (Data.是介词或者串(obj.中心第一根类.模式行, true, true, false))
				{
					if (obj.是的或者地(true, true))
						return null;
					if (obj.是介词或者串(false, true, false) == false)
						return null;
					if (两个位置是相邻的(字符位置, obj.begindex))
					{
						对象.后置介词 = obj;
						字符位置 = obj.endindex;
						if (obj.endindex < 边界)
							对象.后置介词逗号 = 查找逗号(对象, ref 字符位置, 边界, false);
						return obj;
					}
				}
			}
			return null;
		}

		public 生长对象 查找的和地(生长对象 对象, ref int 字符位置, int 边界)
		{
			if (字符位置 >= 边界)
				return null;
			foreach (生长对象 obj in 左边界排序对象)
			{
				if (obj.begindex > 边界)
					break;
				if (obj.begindex < 字符位置)
					continue;
				if (obj.endindex > 边界)
					continue;
				if (obj.是的或者地(对象.中心在右, true))////中心在左边时，暂不允许【的】作为后置介词。如果是【红色的】这种空的，右边会看着有空记录进行处理。
				{
					if (两个位置是相邻的(字符位置, obj.begindex))
					{
						对象.中间的和地 = obj;
						字符位置 += obj.长度;
						if (obj.endindex < 边界)
							对象.中间的和地逗号 = 查找逗号(对象, ref 字符位置, 边界, false);
						return obj;
					}
				}
			}
			return null;
		}

		public 生长对象 查找逗号(生长对象 对象, ref int 字符位置, int 边界, bool 向左找)
		{
			if (向左找)
			{
				foreach (生长对象 obj in 右边界排序对象)
				{
					if (obj.endindex < 边界)
						break;
					if (obj.endindex >= 字符位置)
						continue;
					if (obj.begindex < 边界)
						continue;
					if (Data.是派生类(Data.短句停顿Guid, obj.源模式行, 替代.正向替代))
					{
						if (两个位置是相邻的(obj.endindex, 字符位置))
						{
							字符位置 = obj.begindex;
							return obj;
						}
					}
				}
			}
			else
			{
				foreach (生长对象 obj in 左边界排序对象)
				{
					if (obj.begindex > 边界)
						break;
					if (obj.begindex <= 字符位置)
						continue;
					if (obj.endindex > 边界)
						continue;
					if (Data.是派生类(Data.短句停顿Guid, obj.源模式行, 替代.正向替代))
					{
						if (两个位置是相邻的(字符位置, obj.begindex))
						{
							字符位置 = obj.endindex;
							return obj;
						}
					}
				}
			}
			return null;
		}
		public bool 加上所有部件后对象对是相邻的(生长对象 对象对)
		{
			int pos;

			if (对象对.中间的和地 != null && 对象对.后置介词 != null)
				return false;
			if (对象对.中心在右)
			{
				if (对象对.中间的和地逗号 != null)
					pos = 对象对.中间的和地逗号.endindex;
				else if (对象对.中间的和地 != null)
					pos = 对象对.中间的和地.endindex;
				else if (对象对.后置介词逗号 != null)
					pos = 对象对.后置介词逗号.endindex;
				else if (对象对.后置介词 != null)
					pos = 对象对.后置介词.endindex;
				else pos = 对象对.参数对象.endindex;

				if (两个位置是相邻的(pos, 对象对.中心对象.begindex))
					return true;
			}
			else
			{
				if (对象对.前置介词 != null)
					pos = 对象对.前置介词.begindex;
				else if (对象对.前置介词逗号 != null)
					pos = 对象对.前置介词逗号.begindex;
				else if (对象对.介动词后生存 != null)
					pos = 对象对.介动词后生存.begindex;
				else if (对象对.介动词后生存逗号 != null)
					pos = 对象对.介动词后生存逗号.begindex;
				else
					pos = 对象对.参数对象.begindex;

				if (两个位置是相邻的(对象对.中心对象.endindex, pos))
					return true;
			}

			return false;
			//int pos = 对象对.参数对象.begindex;
			//if (对象对.前介逗号 != null)
			//{
			//    if (两个位置是相邻的(对象对.前介逗号.endindex, pos) == false)
			//        return false;
			//    pos = 对象对.前介逗号.begindex;
			//}
			//if (对象对.前置介词 != null)
			//{
			//    if (两个位置是相邻的(对象对.前置介词.endindex, pos) == false)
			//        return false;
			//    pos = 对象对.前置介词.begindex;
			//}

			//if (对象对.中心在右 == false && 两个位置是相邻的(对象对.中心对象.endindex, pos) == false)
			//    return false;

			//pos = 对象对.参数对象.endindex;
			//if (对象对.后置介词 != null)
			//{
			//    if (两个位置是相邻的(pos, 对象对.后置介词.begindex) == false)
			//        return false;
			//    pos = 对象对.后置介词.endindex;
			//}
			//if (对象对.后介逗号 != null)
			//{
			//    if (两个位置是相邻的(pos, 对象对.后介逗号.begindex) == false)
			//        return false;
			//    pos = 对象对.后介逗号.endindex;
			//}
			//if (对象对.中间的和地 != null)
			//{
			//    if (两个位置是相邻的(pos, 对象对.中间的和地.begindex) == false)
			//        return false;
			//    pos = 对象对.中间的和地.endindex;
			//}
			//if (对象对.的后逗号 != null)
			//{
			//    if (两个位置是相邻的(pos, 对象对.的后逗号.begindex) == false)
			//        return false;
			//    pos = 对象对.的后逗号.endindex;
			//}

			//if (对象对.中心在右 == true && 两个位置是相邻的(pos, 对象对.中心对象.begindex) == false)
			//    return false;

			//return true;

			//覆盖处理:
			//    //如果一个是覆盖对象，那么就要求覆盖，而不能间隔开。
			//    if ((对象对.左对象.覆盖型对象位标识 ^ 对象对.右对象.覆盖型对象位标识) > 0)
			//    {
			//        if (对象对.左对象.长度 == 0 && 对象对.左对象.覆盖型对象位标识 > 0)
			//        {
			//            if (对象对.左对象.begindex <= 对象对.右对象.endindex && 对象对.左对象.endindex >= 对象对.右对象.begindex)
			//                return true;
			//        }
			//        if (对象对.右对象.长度 == 0 && 对象对.右对象.覆盖型对象位标识 > 0)
			//        {
			//            if (对象对.右对象.begindex <= 对象对.左对象.endindex && 对象对.右对象.endindex >= 对象对.左对象.begindex)
			//                return true;
			//        }
			//    }

			//    return false;
		}


		//增加可舍弃字符的处理，这样，一些可以认为是多余的对象也就可以被舍弃掉。
		public void 只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(生长对象 对象对)
		{
			//包含了【的和地】就绝对不会有后置介词了。
			//if (对象由的或者地紧密关联(参数对象, 中心对象) == 1)
			对象对.清空所有介词和关联信息();

			if (对象对.参数对象.初步判断允许介词())
			{
				int 左边界 = 对象对.中心在右 ? 0 : 对象对.中心对象.endindex;
				//左边界 = 根据强势对象限定边界(左边界, false);

				int 位置 = 对象对.参数对象.begindex;
				查找介动词后生存(对象对, ref 位置, 左边界);

				if (查找前置介词(对象对, ref 位置, 左边界) == null)//如果前置介词不存在，那么介动词后生存就没有意义，取消掉。
				{
					对象对.介动词后生存 = null;
					对象对.介动词后生存逗号 = null;
				}

				int 右边界 = 对象对.中心在右 ? 对象对.中心对象.begindex : Data.当前句子串.Length;
				//右边界 = 根据强势对象限定边界(右边界, true);

				位置 = 对象对.参数对象.endindex; //对象对.中心在右 ?对象对.参数对象.endindex:对象对.中心对象.endindex;
				查找后置介词(对象对, ref 位置, 右边界);

				查找的和地(对象对, ref 位置, 右边界);
			}

			处理间隔附加串(对象对);
		}

		public bool 是一个强势完成对象(生长对象 对象)
		{
			//暂时只让第只有第一级的推理角色是如此。比如【因为】【因而】等。
			if (对象.处理轮数 != -2 && 对象.是隐藏对象() == false && 对象.生长次数 == 0 && Data.是派生类(Data.推理角色Guid, 对象.中心第一根类.源模式行, 替代.正向替代) && 对象.中心第一根类.左拆解 == 0)
				return true;
			else if (对象.中心第一根类 == 对象 && 对象.长度 >= 4)
				return true;
			else if (Data.是子记录(对象.中心第一根类.源模式行, Data.公共新对象Guid, true)) //外围库对象
			{
				if (对象.左拆解 == 0 && 对象.右拆解 == 0 && 对象.长度 > 1)
					return true; //外围库对象，左、右两边都不被拆解，则作为强势对象
			}
			else if (对象.模式行.参数.概率分 >= 9 && 对象.左拆解 == 0 && 对象.右拆解 == 0 && 对象.长度 > 1)
				return true; //核心库对象，左、右两边都不被拆解，就作为强势对象 //大风致16人消失 网络中流传
			return false;
		}


		public void 处理间隔附加串(生长对象 对象对)
		{
			int 左边界 = 对象对.中心在右 ? 0 : 对象对.中心对象.endindex;
			int 位置 = 对象对.参数对象.begindex;
			if (对象对.介动词后生存逗号 != null)
			{
				对象对.介动词后生存逗号_附加串 = (byte)(位置 - 对象对.介动词后生存逗号.endindex);
				位置 = 对象对.介动词后生存逗号.begindex;
			}
			if (对象对.介动词后生存 != null)
			{
				对象对.介动词后生存_附加串 = (byte)(位置 - 对象对.介动词后生存.endindex);
				位置 = 对象对.介动词后生存.begindex;
			}
			if (对象对.前置介词逗号 != null)
			{
				对象对.前置介词逗号_附加串 = (byte)(位置 - 对象对.前置介词逗号.endindex);
				位置 = 对象对.前置介词逗号.begindex;
			}
			if (对象对.前置介词 != null)
			{
				对象对.前置介词_附加串 = (byte)(位置 - 对象对.前置介词.endindex);
				位置 = 对象对.前置介词.begindex;
			}
			if (对象对.中心在右 == false && 两个位置是相邻的(对象对.中心对象.endindex, 位置))
				对象对.起始_附加串 = (byte)(位置 - 对象对.中心对象.endindex);


			int 右边界 = 对象对.中心在右 ? 对象对.中心对象.begindex : Data.当前句子串.Length;
			位置 = 对象对.参数对象.endindex;
			if (对象对.后置介词 != null)
			{
				对象对.后置介词_附加串 = (byte)(对象对.后置介词.begindex - 位置);
				位置 = 对象对.后置介词.endindex;
			}
			if (对象对.后置介词逗号 != null)
			{
				对象对.后置介词逗号_附加串 = (byte)(对象对.后置介词逗号.begindex - 位置);
				位置 = 对象对.后置介词逗号.endindex;
			}
			if (对象对.中间的和地 != null)
			{
				对象对.中间的和地_附加串 = (byte)(对象对.中间的和地.begindex - 位置);
				位置 = 对象对.中间的和地.endindex;
			}
			if (对象对.中间的和地逗号 != null)
				位置 = 对象对.中间的和地逗号.endindex;
			if (对象对.中心在右 && 两个位置是相邻的(位置, 对象对.中心对象.begindex))
				对象对.结束_附加串 = (byte)(对象对.中心对象.begindex - 位置);
		}


		//if (对象对.中心在右)
		//{
		//	int 位置 = 对象对.左对象.endindex;
		//	if (位置 < 对象对.右对象.begindex)
		//	{
		//		生长对象 介词1 = 查找后置介词(ref 位置, 对象对.右对象.begindex - 位置);
		//		if (介词1 != null)
		//		{
		//			if (介词1.是的或者地(true, true))
		//				对象对.中间的和地 = 介词1;
		//			else
		//			{
		//				对象对.后置介词 = 介词1;
		//				位置 = 介词1.endindex;
		//				if (位置 < 对象对.右对象.begindex)
		//				{
		//					生长对象 介词2 = 查找后置介词(ref 位置, 对象对.右对象.begindex - 位置);
		//					if (介词2 != null && 介词2.是的或者地(true, true))
		//						对象对.中间的和地 = 介词2;
		//				}
		//			}
		//		}
		//	}
		//	位置 = 对象对.左对象.begindex;
		//	if (位置 > 0)
		//		对象对.前置介词 = 查找前置介词(ref 位置, 位置 - 0);
		//}
		//else//中心在左。
		//{
		//	int 位置 = 对象对.右对象.begindex;
		//	if (位置 > 对象对.左对象.endindex)
		//		对象对.前置介词 = 查找前置介词(ref 位置, 位置 - 对象对.左对象.endindex);

		//	位置 = 对象对.右对象.endindex;
		//	生长对象 介词1 = 查找后置介词(ref 位置, Data.当前句子串.Length - 位置);
		//	if (介词1 != null)
		//	{
		//		if (介词1.是的或者地(true, true))
		//		{
		//			//对象对.的和地 = 介词1;//暂时不允许【的】和【地】作为后置介词。只能做中间介词，如果是【红色的】这种空的，右边会看着有空记录进行处理。
		//		}
		//		else
		//		{
		//			对象对.后置介词 = 介词1;
		//			位置 = 介词1.endindex;
		//			//生长对象 介词2 = 查找后置介词(位置, Data.当前句子串.Length - 位置);
		//			//if (介词2 != null && 介词2.是的或者地(true, true))
		//			//    对象对.的和地 = 介词2;
		//		}
		//	}
		//}

		//对于显式的属于比如【红色的水果是苹果】，专门处理这种基类属于派生类的情况，。
		//这个时候这样才能通过。两者任意一个是派生类都可以。
		//以后更严谨的要进行考虑。
		//返回1示参数不够，还不能推导。
		//返回0表示推导出不满足。
		//更大值表示满足程度。
		public 模式 根据两个参数计算二元关联(生长对象 A端对象, 生长对象 B端对象, 模式 关联, bool 名词谓语)
		{

			//参数树结构 tree中心 = A端对象.利用缓存得到基类和关联记录树();
			//参数树结构 tree参数 = B端对象.利用缓存得到基类和关联记录树();
			//三级关联.遍历关联.级数 = 0;
			//tree中心.递归计算三级关联(tree参数, 字典_目标限定.A端, 结果);
			//if (Data.基本关联Guid.Equals(关联.源记录))//如果是最基本的二元关联，那么，看着一个占位符号，任何一个关联都可以算是满足的。
			//{
			//    if (结果.Count > 0)
			//        return true;
			//    //对于基本二元关联，反向关联也是满足的。
			//    tree中心 = B端对象.利用缓存得到基类和关联记录树();
			//    tree参数 = A端对象.利用缓存得到基类和关联记录树();
			//    三级关联.遍历关联.级数 = 0;
			//    tree中心.递归计算三级关联(tree参数, 字典_目标限定.A端, 结果);
			//    if (结果.Count > 0)
			//        return true;
			//    return false;
			//}

			//这里的处理方法，应该不要去解决【可以的】，而是排除肯定不可以的！

			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(A端对象, B端对象, false, 结果);

			//名词谓语的情况下，必须要在知识库中找到具体的关联！
			if (名词谓语)
			{
				foreach (三级关联 o in 结果)
				{
					Guid 一级关联类型 = Data.一级关联类型(o.中心主关联.目标);

					if (名词谓语 && o.that端 == 字典_目标限定.A端 && Data.拥有Guid.Equals(一级关联类型))
					{
						//if (Data.是派生关联(Data.概念拥有属性Guid, o.中心主关联.目标) > 0)//可能还有一些可以满足
						return o.中心主关联.目标;
					}
				}
				return null;
			}


			foreach (三级关联 o in 结果)
			{
				if (Data.是派生关联(Data.代理拥有Guid, o.中心主关联.目标) > 0)//代理拥有不算真正的二元关联，是形式组织的代理。
					continue;

				if (Data.是派生关联(关联.ID, o.中心主关联.目标) > 0)
					return o.中心主关联.目标;

				Guid 一级关联类型 = Data.一级关联类型(o.中心主关联.目标);

				if (名词谓语 && o.that端 == 字典_目标限定.A端 && Data.拥有Guid.Equals(一级关联类型))
					return o.中心主关联.目标;

				if (Data.属于Guid.Equals(关联.ID))
				{
					//派生关系。
					//if (Data.是派生类(A端对象.中心第一根类.源模式行ID, B端对象.中心第一根类.模式行, 替代.正向替代 | 替代.聚合替代))
					//    return 9;
					//if (Data.是派生类(B端对象.中心第一根类.源模式行ID, A端对象.中心第一根类.模式行, 替代.正向替代 | 替代.聚合替代))
					//    return 9;

					if (替代.是属于等价或聚合(一级关联类型))
						return o.中心主关联.目标;
				}
			}

			if (A端对象.是任意概念() || B端对象.是任意概念())//任意对象如单独的【这】【那】满足任何东西，因为相当于新建。
				return 关联;

			bool A是NullThis对象 = A端对象.是NullThis空对象();

			bool B是NullThis对象 = B端对象.是NullThis空对象();

			if (A是NullThis对象 || B是NullThis对象)
			{
				return 关联;//暂时先都认为满足。

				//if (A是NullThis对象 && B是NullThis对象)
				//	return 关联;//两边都是空对象，没法判断，暂时就认为是满足的了。
				//if (Data.是派生关联(Data.属于Guid, 关联) == 0)//在有空对象的情况下，又不是属于关联，没有办法传递替换，就直接返回正确。
				//	return 关联;
				////有对象为空对象，然而关联是【属于】，所以可以进行消除属于后再计算。
				//参数 o = A是NullThis对象 ? A端对象.查找第一个已经满足的参数() : B端对象.查找第一个已经满足的参数();//任意找一个参数，一般来说，第一个就是合适的。
				////也有可能把所有参数都要计算一遍更好。
				//if (o == null)
				//	return null;
				//关联 = o.源关联记录;
				//if (A是NullThis对象)//[红色的是苹果]
				//{//实际对象是【this属于红色】
				//	Data.Assert(o.对端派生对象.that == 字典_目标限定.A端);
				//	if (o.对端派生对象.that == 字典_目标限定.A端)
				//	{
				//		A端对象 = B端对象;//苹果
				//		B端对象 = o.对端派生对象.B端对象;//红色。
				//	}
				//}
				//else//B端是空，[苹果是红色的]
				//{
				//	if (o.对端派生对象.that == 字典_目标限定.A端)
				//	{
				//		B端对象 = o.对端派生对象.B端对象;
				//	}
				//}
			}


			return null;
		}


		public List<参数> 根据参数对象和形式线索计算关联和中心对象(生长对象 已知对象, bool 中心在右, string 推断对象串, 生长对象 前置介词, 生长对象 后置介词, 生长对象 的或地)
		{
			List<参数> 结果 = new List<参数>();
			List<参数> 参数表 = 已知对象.得到指定根对象的参数表();
			foreach (参数 o in 参数表)
			{
				if (o.已经派生() || Data.是拥有形式(o.源关联记录))
					continue;
				if (o.源关联记录.参数.B对A的创建性 > 0)
					结果.Add(o);
			}

			结果.Sort((参数 v1, 参数 v2) =>
			{
				int k1 = v1.源关联记录.参数.B对A的创建性;
				int k2 = v2.源关联记录.参数.B对A的创建性;
				if (k1 == k2)
					return 0;
				return k1 > k2 ? -1 : 1;
			});

			return 结果;
		}
		public List<参数> 根据中心对象和形式线索计算关联和参数对象(生长对象 已知对象, bool 中心在右, string 推断对象串, 生长对象 前置介词, 生长对象 后置介词, 生长对象 的或地)
		{
			List<参数> 临时结果 = 已知对象.得到指定根对象的参数表();

			参数树结构 tree = 已知对象.利用缓存得到基类和关联记录树();
			tree.递归根据形式增加参数(已知对象.中心第一根类, 临时结果, Data.当前解析语言, 前置介词, 后置介词, 的或地);

			//一级语义对象.加入各概念参数行到界面(一级语义对象.模式行, false, false);//形式行加入到界面。
			//下边对形式串进行满足性挂接。
			//本对象是一级对象，已经设置了【模式行】并且有了字符串，现在查找出来要求的形式里边的哪个串和自己匹配，表示对应上了，进行满足。
			List<参数> 结果 = new List<参数>();

			foreach (参数 o in 临时结果)
			{
				if (o.已经派生() || Data.是拥有形式(o.源关联记录))
					continue;
				结果.Add(o);
			}

			结果.Sort((参数 v1, 参数 v2) =>
			{
				int k1 = v1.源关联记录.参数.B对A的关键性;
				int k2 = v2.源关联记录.参数.B对A的关键性;
				if (k1 == k2)
					return 0;
				return k1 > k2 ? -1 : 1;
			});

			return 结果;
		}



		public bool 是直观的拥有关系(模式 关联)
		{
			if (Data.是派生关联(Data.概念拥有属性Guid, 关联) > 0)
				return true;
			if (Data.是派生关联(Data.概念拥有构成Guid, 关联) > 0)
				return true;
			if (Data.是派生关联(Data.概念拥有对象Guid, 关联) > 0)
				return true;
			return false;
		}
		//比如【损失，他】【脸，他】等。来自于【他损失惨重】【他脸红了】等。
		//目前的方法是看有没有明显的拥有关系。还不严谨，要调整，其实关键是看两个对象是否有非常紧密的关系。
		//尤其对于目标对象来说，要求独立语对象是它需要的【关键参数】。比如【损失需要失主】，【脸需要拥有者】。
		public bool 计算明显的独立语语义关联(生长对象 目标对象, 生长对象 独立语对象)
		{
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(独立语对象, 目标对象, true, 结果);
			foreach (三级关联 关联 in 结果)
			{
				Guid 一级关联类型 = Data.一级关联类型(关联.中心主关联.目标);
				if (关联.that端 == 字典_目标限定.A端 && Data.拥有Guid.Equals(一级关联类型) == false)
					continue;
				if (是直观的拥有关系(关联.中心主关联.目标))
					return true;
			}
			return false;
		}

		//补语对象，一般是【工具】【使用】等，比如【他有钱可以买】。
		public bool 计算明显的补语语义关联(生长对象 目标对象, 生长对象 补语对象)
		{
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(补语对象, 目标对象, true, 结果);
			foreach (三级关联 关联 in 结果)
			{
				Guid 一级关联类型 = Data.一级关联类型(关联.中心主关联.目标);
				if (Data.拥有Guid.Equals(一级关联类型))
					return true;
			}
			return false;
		}

		//这里要进行真正的关联运算，主要是对象对的【参数】对象在【中心对象】的各个部件可能存在的关键参数关系。
		public void 计算独立语关联(生长对象 独立语对象, 生长对象 模式对象, List<三级关联> 三级关联结果集)
		{
			//return;
			//三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.独立关联Guid), 字典_目标限定.A端, false/*, 0*/)));
			if (Data.能够序列化(模式对象.中心第一根类.模式行) == false)//模式必须是一个序列化的对象。
				return;

			List<参数> 概念参数表 = 模式对象.得到指定根对象的参数表(模式对象.中心第一根类);
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() == false)
					continue;
				模式 原始记录 = o.源关联记录;
				if ((o.语言角色 & (字典_语言角色.主语 | 字典_语言角色.主被)) == 0)//只有主语或者主被可以，但实际可能宾语也可以，比如【他红了脸】。但【把宾】一般是不行。
					continue;

				//这个计算方法不严谨，如果有聚合，就不对，要调整下。应该是一个显式对象才对。
				if (o.对端派生对象.B端实际对象.处理轮数 != -2 && 计算明显的独立语语义关联(o.对端派生对象.B端实际对象, 独立语对象))
				{
					三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.事件拥有主体属主Guid), 字典_目标限定.A端, false/*, 0*/)));
					return;
				}
			}
		}

		public void 计算补语关联(生长对象 前边模式, 生长对象 补语对象, List<三级关联> 三级关联结果集)
		{
			if (Data.能够序列化(补语对象.中心第一根类.模式行) == false)//补语对象必须是一个序列化的对象。
				return;

			List<参数> 概念参数表 = 前边模式.得到指定根对象的参数表(前边模式.中心第一根类);

			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() == false)
					continue;
				模式 原始记录 = o.源关联记录;
				if ((o.语言角色 & (字典_语言角色.宾语)) == 0)//前边的宾语。
					continue;

				if (o.对端派生对象.B端实际对象.处理轮数 != -2 && 计算明显的补语语义关联(o.对端派生对象.B端实际对象, 补语对象))
				{
					三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.拥有补语动词Guid), 字典_目标限定.A端, false/*, 0*/)));
					return;
				}
			}
		}


		public bool 支持离合动作(生长对象 假设离合宾语对象)
		{
			if (Data.是派生类(Data.动作Guid, 假设离合宾语对象.源模式行, 替代.正向替代) == false)
				return false;
			List<参数> 概念参数表 = 假设离合宾语对象.得到指定根对象的参数表();
			foreach (参数 obj in 概念参数表)
				if (Data.是派生关联(Data.关联拥有离合宾语Guid, obj.源关联记录) > 0)
					return true;

			return false;
		}

		public bool 可以做同一重复合并(生长对象 左对象, 生长对象 右对象, bool 假设中心在右)
		{
			if (Data.能够序列化(右对象.中心第一根类.源模式行) && 已经有了某个语言角色(右对象, 字典_语言角色.主语))
				return false;
			return true;
		}

		public int 根据本身是书名的阈值(生长对象 对象)
		{
			//实际还是需要判断是那种引号。
			if (对象.中心对象 != null && 对象.中心对象.参数对象 != null
				&& Data.是派生类(Data.封闭范围Guid, 对象.中心对象.参数对象.中心第一根类.源模式行, 替代.正向替代))
				return 9;
			return 0;
		}

		public int 计算名称关联(生长对象 左对象, 生长对象 右对象, bool 假设中心在右, List<三级关联> 结果)
		{
			生长对象 类型对象 = 假设中心在右 ? 右对象 : 左对象;
			生长对象 名称对象 = 假设中心在右 ? 左对象 : 右对象;

			if (Data.是派生类(Data.信息媒介Guid, 类型对象.中心第一根类.源模式行, 替代.正向替代))
			{
				if (根据本身是书名的阈值(名称对象) > 0)
				{
					结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.概念拥有子名称Guid), 字典_目标限定.A端, false/*, 0*/)));
					return 9;
				}
			}

			return 0;
		}

		//两个参数都是【this属于型】，以后的计算可以优化，可以从一个二元【关联路径缓存表】来查询。
		public void 计算三级关联(生长对象 左对象, 生长对象 右对象, bool 假设中心在右, List<三级关联> 结果)
		{
			Data.Assert((左对象 == null || Data.ThisGuid.Equals(左对象.中心第一根类.模式行.A端)) && (右对象 == null || Data.ThisGuid.Equals(右对象.中心第一根类.模式行.A端)));

			生长对象 中心对象 = 假设中心在右 ? 右对象 : 左对象;
			生长对象 参数对象 = 假设中心在右 ? 左对象 : 右对象;

			//完全相同，此时不用分析别的关联了，两个东西是一样的，所以，直接返回一个结果。
			if (左对象 != null && 右对象 != null && ((Guid)左对象.中心第一根类.模式行.B端).Equals(右对象.中心第一根类.模式行.B端))
			{
				if (假设中心在右 ? 支持离合动作(左对象) : 支持离合动作(右对象))
				{
					结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.动作拥有离合宾语动作Guid), 字典_目标限定.A端, false/*, 0*/)));
					return;
				}
				if (可以做同一重复合并(左对象, 右对象, 假设中心在右))
				{
					结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.同一Guid), 字典_目标限定.A端, false/*, 0*/)));
					return;
				}
			}

			if (左对象 != null && 左对象.是介词或者串(true, true, true) && 左对象.取子串 == "被")
			{
				if (右对象 != null && Data.能够序列化(右对象.中心第一根类.模式行) && 已经有了某个语言角色(右对象, 字典_语言角色.主语 | 字典_语言角色.主被) == false)
					结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.事件属拥被动Guid), 字典_目标限定.A端, false/*, 0*/)));
				return;
			}

			//可能还有一些特殊的关联计算也放置在前边这个位置。
			int 名称关联打分 = 计算名称关联(左对象, 右对象, 假设中心在右, 结果);
			if (名称关联打分 >= 9)//名称关联很清晰，就不用计算别的了。
				return;

			//if ((左对象!=null && 左对象.是NullThis空对象() && 左对象.源模式行ID.Equals(Data.基本关联Guid) == false /*&& Data.是介词或者串(B端对象.源模式行, true, true, true) == false*/)
			//	 || (右对象!=null && 右对象.是NullThis空对象() && 右对象.源模式行ID.Equals(Data.基本关联Guid) == false /*&& Data.是介词或者串(A端对象.源模式行, true, true, true) == false*/))
			//{
			//	//空对象无法进行计算，所以都算满足。就加入一条基本的【二元关联】记录。暂时先这样，后边可能再斟酌。
			//	//同时排除掉上一次已经加了【基本关联】的情况。
			//	结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.基本关联Guid), 字典_目标限定.A端, false/*, 0*/)));
			//	return;
			//}
			参数树结构 参数展开树 = 参数对象.集合对象的基对象 != null ? Data.利用缓存得到基类和关联记录树(参数对象.集合对象的基对象, false) : 参数对象.利用缓存得到基类和关联记录树();//中心对象包含这些拥有参数。

			参数树结构 中心展开树 = 中心对象.集合对象的基对象 != null ? Data.利用缓存得到基类和关联记录树(中心对象.集合对象的基对象, false) : 中心对象.利用缓存得到基类和关联记录树();//如果是nullthis空对象，那么tree参数传入null。


			三级关联.初始化();
			中心展开树.递归计算三级关联(参数展开树, 字典_目标限定.A端, 结果, Data.属于Guid, -1, new List<模式>());

			//以前觉得显式的字句才可能出现反向关联，但后来发现，隐含的【属于】也需要反向关联，比如【苹果红了】等，所以，让所有的都把反向关联考虑起来。
			//bool 含反向关联 = Data.是派生类(Data.推理角色Guid, B端对象.中心第一根类.模式行, 替代.正向替代) || Data.能够做从句的中心(A端对象.中心第一根类.模式行);
			//if (含反向关联)
			//{
			三级关联.初始化();
			参数展开树.递归计算三级关联(中心展开树, 字典_目标限定.B端, 结果, Data.属于Guid, -1, new List<模式>());
			//}


			if (假设中心在右 == false && 左对象 != null && 左对象.集合对象的基对象 != null)
			{
				if (右对象.集合对象的基对象 == null /*|| Data.ThisGuid.Equals(右对象.集合对象的基对象.ID)*/)//暂时不允许右对象是集合。
				{
					并列关联 关联 = 计算两元素的共同基类(左对象, 右对象);
					if (关联 != null && 关联.总距离 > 0)
					{
						参数展开树 = 左对象.利用缓存得到基类和关联记录树();//集合自己的处理。
						参数展开树.递归计算三级关联(中心展开树, 字典_目标限定.A端, 结果, Data.属于Guid, -1, new List<模式>());
					}
				}
			}
			else if (假设中心在右 && 右对象 != null && 右对象.集合对象的基对象 != null && Data.ThisGuid.Equals(右对象.集合对象的基对象.ID))//右边是一个空集合的情况是可以的，【他和】。
			{
				并列关联 关联 = 计算两元素的共同基类(左对象, 右对象);
				if (关联 != null && 关联.总距离 > 0)
				{
					中心展开树 = 右对象.利用缓存得到基类和关联记录树();//集合自己的处理。
					中心展开树.递归计算三级关联(参数展开树, 字典_目标限定.B端, 结果, Data.属于Guid, -1, new List<模式>());
				}
			}

			//因为最终两个对象中心对象和参数对象的原因，调整三级关联的次序。
			foreach (三级关联 obj in 结果)
				obj.调整三级关联次序();

			删除错误的关联(左对象, 右对象, 假设中心在右, 结果);

			删除多余的间接关联以及所有的基关联(结果);

			//概率高的放前边。
			结果.Sort((三级关联 v1, 三级关联 v2) =>
			{
				int k1 = v1.中心主关联.目标.参数.概率分;
				int k2 = v2.中心主关联.目标.参数.概率分;
				if (k1 == k2)
				{
					//概率分相同时级数小的优先，级数相同时按序号小的优先
					if (v1.级数 == v2.级数)
					{
						if (v1.中心主关联.目标.序号 < v2.中心主关联.目标.序号)
							return -1;
						else if (v1.中心主关联.目标.序号 > v2.中心主关联.目标.序号)
							return 1;
						else
							return 0;
					}
					else
						return v1.级数 < v2.级数 ? -1 : 1;
				}
				else if (k1 > k2)
					return -1;
				return 1;
			});

			//foreach (三级关联 o in 结果)
			//	Data.输出串(o.ToString());
		}





		public bool 已经有了某个语言角色(生长对象 obj, int 语言角色)
		{
			foreach (参数 o in obj.得到指定根对象的参数表())
				if ((语言角色 & o.语言角色) > 0)
					return true;
			return false;
		}
		public bool 设置并判断假设语言角色的语序和形式的正确性(生长对象 obj, int 语言角色)
		{
			obj.语言角色 = 语言角色;

			//前独前不能再有宾语和主语。
			if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.前独))
			{
				if (obj.语言角色 != 字典_语言角色.前状 && obj.语言角色 != 字典_语言角色.前独 && obj.语言角色 != 字典_语言角色.句尾 && obj.语言角色 != 字典_语言角色.尾标)
					return false;
			}

			if (obj.语言角色 == 字典_语言角色.一宾)
			{
				if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.宾语))
					return false;
			}
			else if (obj.语言角色 == 字典_语言角色.兼一宾)//
			{
				if (已经有了某个语言角色(obj.参数对象, 字典_语言角色.主语))//兼语动词自己已经有了主语，不允许。
					return false;
				obj.语言角色 = 字典_语言角色.一宾;
			}
			else if (obj.语言角色 == 字典_语言角色.兼二宾)//
			{
				//if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.一宾))//已经有一宾就可以
				//	goto L1;
				if (已经有了某个语言角色(obj.参数对象, 字典_语言角色.主语))//兼语动词自己已经有了主语，不允许。
					return false;
				//L1:
				obj.语言角色 = 字典_语言角色.二宾;
			}
			else if (obj.语言角色 == 字典_语言角色.主语)//
			{
				if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语 | 字典_语言角色.主被))
					return false;
				if (obj.中心对象.查找已经实现的参数(Data.FindRowByID(Data.事件属拥被动Guid)) != null)
					return false;
			}
			else if (obj.语言角色 == 字典_语言角色.主被)//
			{
				if (已经有了某个语言角色(obj.中心对象, /*字典_语言角色.主语 |*/ 字典_语言角色.主被))
					return false;

			}
			else if (obj.语言角色 == 字典_语言角色.把宾)//
			{
				if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语 | 字典_语言角色.主被))//把宾必须在主语的后边。【把衬衣他买了】不允许。
					return false;
				if (obj.中心对象.查找已经实现的参数(Data.FindRowByID(Data.事件属拥被动Guid)) != null)
					return false;
			}


			//else if (obj.语言角色 == 字典_语言角色.独立)//独立语一般应该是最前边的一个，而且最后最后结合，最起码不可能再有【二宾】【主语】【把宾】【主被】等。
			//{
			//	if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.独立 | 字典_语言角色.二宾 | 字典_语言角色.主语 | 字典_语言角色.把宾 | 字典_语言角色.主被))
			//		return false;
			//}


			int 序号 = 字典_语言角色.计算一级序号(obj.语言角色);

			//判断语言角色要求的前后位置
			if ((序号 != 0) && ((序号 < 0 && obj.中心在右 == false) || (序号 > 0 && obj.中心在右 == true)))
				return false;

			if (obj.语言角色 == 字典_语言角色.从定)
			{
				if (对象中间由的或地关联(obj) != 1)
					名词谓语关联 = obj.源模式行;
				//if (Data.能够序列化(obj.参数对象.中心第一根类.源模式行))
				//	return false;
				//if (Data.是派生类(Data.情绪Guid, obj.中心对象.中心第一根类.源模式行, 替代.正向替代) )
				//	return false;
			}

			//判断各个参数之间的相互位置关系是否合理
			//if (obj.中心对象.概念对象参数表 != null)
			//    foreach (参数 o in obj.中心对象.概念对象参数表)
			//    {
			//        if (Data.是拥有形式(o.源关联记录))
			//            continue;
			//        int 序号2 = 字典_语言角色.计算一级序号(o.语言角色);
			//        if (序号 < 序号2 && obj.参数对象.begindex > o.对端派生对象.begindex)
			//            return false;
			//        if (序号 > 序号2 && obj.参数对象.begindex < o.对端派生对象.begindex)
			//            return false;
			//    }

			//上述算法测试了大体没有问题，但是依据不对，比如前状，可能位置在主语前也是可以的！
			//所以，暂时只处理【兼宾】和【二宾】的这种冲突情况。
			//if (obj.语言角色 == 字典_语言角色.兼宾 && obj.中心对象.概念对象参数表 != null)
			//    foreach (参数 o in obj.中心对象.概念对象参数表)
			//        if (o.语言角色 == 字典_语言角色.二宾)
			//            return false;


			//先去除实体角色的判断，也就是不管有没有实体角色名出线，都要求用语言角色和介词来判断必须满足。
			//if (用实体角色判断关联的满足性(obj))
			//    return true;

			//中心对象本身是基本二元关联（不包含推导）的话，暂时认为都不需要检查。
			//if (Data.是二元关联(obj.中心对象.中心第一根类.模式行, false))
			//	return true;
			//是模式行的两端，暂时不检查。
			//if (Data.是派生关联(Data.关联拥有A端Guid,obj.源模式行) > 0 || Data.是派生关联(Data.关联拥有B端Guid, obj.源模式行) > 0)
			//	return true;


			//暂时假定主语和一宾没有介词都可以。
			//if (obj.语言角色 == 字典_语言角色.主语 || obj.语言角色 == 字典_语言角色.一宾)
			//    return true;

			int r = Data.返回对应角色的概率打分(语言角色, obj.源模式行, Data.当前解析语言);
			obj.参数.概率分 = Data.合并概率打分(obj.参数.概率分, r);
			if (obj.参数.概率分 <= 0)
				return false;

			//判断要求的介词后生存是否满足
			if (obj.解释介动词后生存的合法性(obj.介动词后生存关联, Data.关联拥有介动词后生存Guid, Data.当前解析语言) == false)
				return false;

			//判断要求的前介词是否满足
			if ((r = obj.解释语言角色语序和语言形式部件的合法性打分(obj.前置介词关联, Data.关联拥有前置介词Guid, Data.当前解析语言)) == 0)
				return false;
			obj.参数.概率分 = Data.合并概率打分(obj.参数.概率分, r);

			//判断要求的前介词是否满足
			if ((r = obj.解释语言角色语序和语言形式部件的合法性打分(obj.后置介词关联, Data.关联拥有后置介词Guid, Data.当前解析语言)) == 0)
				return false;
			obj.参数.概率分 = Data.合并概率打分(obj.参数.概率分, r);

			//判断要求的【的】是否满足
			if ((r = obj.解释语言角色语序和语言形式部件的合法性打分(obj.中间的和地关联, Data.关联拥有的Guid, Data.当前解析语言)) == 0)
				return false;
			obj.参数.概率分 = Data.合并概率打分(obj.参数.概率分, r);

			//判断要求的【地】是否满足
			if ((r = obj.解释语言角色语序和语言形式部件的合法性打分(obj.中间的和地关联, Data.关联拥有地Guid, Data.当前解析语言)) == 0)
				return false;
			obj.参数.概率分 = Data.合并概率打分(obj.参数.概率分, r);

			return true;
		}



		//	//有资格当主语和宾语，但也可能当宾语或者状语，先按理想化的主宾进行尝试。
		//	if (obj.语言角色 == 字典_语言角色.主语 || obj.语言角色 == 字典_语言角色.一宾 || obj.语言角色 == 字典_语言角色.二宾)//
		//	{
		//		//Data.输出串((string)基关联["形式"]+"主宾语");
		//		if (obj.语言角色 == 字典_语言角色.一宾 || obj.语言角色 == 字典_语言角色.二宾)
		//		{
		//			if (obj.前置介词 != null && (obj.前置介词.取子串 == "把" || obj.前置介词.取子串 == "将"))//【把宾】处理。
		//			{
		//				obj.前置介词关联 = Data.递归查找成分(ID, Data.关联拥有前置介词Guid, Data.当前语言, 字典_语言角色.全部全, "把");
		//				if (obj.前置介词关联 != null)//没有找到设置了【把】，这个关联不允许用【把】。
		//				{
		//					obj.语言角色 = (int)obj.前置介词关联["语言角色"]; //这个值应该是【把宾】
		//					goto 后置的地处理;
		//				}
		//			}
		//			else if (obj.后置介词 != null && obj.后置介词.取子串 == "被")
		//			{
		//				obj.后置介词关联 = Data.递归查找成分(ID, Data.关联拥有后置介词Guid, Data.当前语言, 字典_语言角色.全部全, "被");//【主被】处理
		//				if (obj.后置介词关联 != null)//没有找到设置了【被】，这个关联不允许用【被】。
		//				{
		//					obj.语言角色 = (int)obj.前置介词关联["语言角色"]; //这个值应该是【主被】。
		//					goto 后置的地处理;
		//				}
		//			}
		//		}

		//		//按正常的主宾语位置处理
		//		//如果前边有前置介词，尝试看是否能吸收这个介词，吸收了的话就变成别的语言处理，不行的话还是按正常处理。
		//		if (obj.前置介词 != null)
		//		{
		//			obj.前置介词关联 = Data.递归查找成分(ID, Data.关联拥有前置介词Guid, Data.当前语言, 字典_语言角色.全部全, obj.前置介词.取子串);
		//			if (obj.前置介词关联 != null)//没有找到这个前置介词。
		//			{
		//				obj.语言角色 = (int)obj.前置介词关联["语言角色"];
		//				goto 后置的地处理;
		//			}
		//		}
		//	}

		//	//这里按状语等处理。

		//	//先找【得字句】处理
		//	if (obj.前置介词 != null && (obj.前置介词.取子串 == "得"))
		//	{
		//		obj.前置介词关联 = Data.递归查找成分(ID, Data.关联拥有前置介词Guid, Data.当前语言, 字典_语言角色.全部全, "得");
		//		if (obj.前置介词关联 != null)//没有找到这个前置介词。
		//		{
		//			obj.语言角色 = (int)obj.前置介词关联["语言角色"];
		//			goto 后置的地处理;
		//		}
		//		goto 结束处理;
		//	}
		//	//普通定语和状语处理。

		//	if (obj.前置介词 != null)
		//	{
		//		obj.前置介词关联 = Data.递归查找成分(ID, Data.关联拥有前置介词Guid, Data.当前语言, 字典_语言角色.全部全, obj.前置介词.取子串);
		//		if (obj.前置介词关联 != null)//没有找到这个前置介词。
		//		{
		//			obj.语言角色 = (int)obj.前置介词关联["语言角色"];
		//			goto 后置的地处理;
		//		}
		//		goto 结束处理;
		//	}

		//后置的地处理:
		//	//后置【的】处理。
		//	if (obj.的和地 != null)
		//	{
		//		if (obj.的和地.取子串 == "的")
		//		{
		//			obj.的和地关联 = Data.递归查找成分(ID, Data.关联拥有的Guid, Data.当前语言, 字典_语言角色.全部全, "的");
		//			if (obj.的和地关联 != null)
		//				obj.语言角色 = 字典_语言角色.前定;
		//			goto 结束处理;
		//		}
		//		else
		//		{
		//			obj.的和地关联 = Data.递归查找成分(ID, Data.关联拥有地Guid, Data.当前语言, 字典_语言角色.全部全, "地");
		//			if (obj.的和地关联 != null)
		//				obj.语言角色 = obj.中心在右 ? 字典_语言角色.前状 : 字典_语言角色.后状;
		//			goto 结束处理;
		//		}
		//	}

		//结束处理:
		//	int 序号 = Data.计算一级序号(obj.语言角色);
		//	if ((序号 < 0 && obj.中心在右 == false) || (序号 > 0 && obj.中心在右 == true))//次序不正确
		//		return false;

		//	if (obj.的和地关联 == null)
		//		obj.的和地 = null;
		//	if (obj.前置介词关联 == null)
		//		obj.前置介词 = null;
		//	if (obj.后置介词关联 == null)
		//		obj.后置介词 = null;

		//似乎还要检查必须满足的形式和介词等。

		//return true;
		//其实总是两个部分【介词、参数、的】和【中心对象】，这两部分的次序可以颠倒，但这两个部分的划分是不错的。
		//【介词、参数、的】这三个子部分应该是连续的。
		//所以就是两种情况，一种是参数前置中心语后置，另一种是中心语前置参数后置。
		//正常的定语以及【主语-谓语】是参数前置。英语用【of】来进行定语后置其实就和【谓语-宾语】的关系一样，是把参数后置。
		//两棵树，分别每棵作为根，尝试和对方的一个节点进行连接，这样，每棵树都不破坏完整性，如果成功，两棵树完整的结合成一棵。
		//不过这里似乎是已经可以明确其中一个是中心？而不是这样盲目的尝试吧
		//public void 尝试合并两棵树(生长对象 树对, int 处理轮数)
		//{
		//    //假设以左边为中心对象进行尝试
		//    //树对.左对象是真正的树，树对.右对象是单独的对象。
		//    List<生长对象> 左中心对象集合 = 树对.左对象.在树上查找准备作为中心来生长的边界对齐对象(树对.左对象.endindex, true);
		//    foreach (生长对象 左中心对象 in 左中心对象集合)
		//    {
		//        int old边界 = 左中心对象.方面主对象.endindex;
		//        左中心对象.方面主对象.endindex = 树对.左对象.方面主对象.endindex;
		//        生长对象 计算对象对 = 生长对象.构造待分析对象对(左中心对象, 树对.右对象);//树对象的右对象是一个整体假设为一个参数，试图挂接到左相邻对象上的一个节点（也就是作为中心）上
		//        尝试调整中心对象并完成对象对(计算对象对, 处理轮数, 树对);
		//        左中心对象.方面主对象.endindex = old边界;
		//    }

		//    //假设以右边为中心对象进行尝试
		//    //树对.右对象是真正的树，树对.左对象是单独的对象。
		//    List<生长对象> 右中心对象集合 = 树对.右对象.在树上查找准备作为中心来生长的边界对齐对象(树对.右对象.begindex, false);
		//    foreach (生长对象 右中心对象 in 右中心对象集合)
		//    {
		//        int old边界 = 右中心对象.方面主对象.begindex;
		//        右中心对象.方面主对象.begindex = 树对.右对象.方面主对象.begindex;
		//        生长对象 计算对象对 = 生长对象.构造待分析对象对(右中心对象, 树对.左对象);//树对象的左对象是一个整体假设为一个参数，试图挂接到右相邻对象上的一个节点（也就是作为中心）上
		//        尝试调整中心对象并完成对象对(计算对象对, 处理轮数, 树对);//
		//        右中心对象.方面主对象.begindex = old边界;
		//    }
		//}
		//其实总是两个部分【介词、参数、的】和【中心对象】，这两部分的次序可以颠倒，但这两个部分的划分是不错的。
		//【介词、参数、的】这三个子部分应该是连续的。
		//所以就是两种情况，一种是参数前置中心语后置，另一种是中心语前置参数后置。
		//正常的定语以及【主语-谓语】是参数前置。英语用【of】来进行定语后置其实就和【谓语-宾语】的关系一样，是把参数后置。

		//两个对象，分别每棵作为中心，尝试和对方作为参数进行连接。
		//public void 尝试完成对象对生长(生长对象 树对, int 处理轮数, int 语义阀值)
		//{
		//	//假设以左边为中心对象进行尝试
		//	//树对.左对象是真正的树，树对.右对象是单独的对象。
		//	List<生长对象> 左中心对象集合 = 树对.左对象.在树上查找准备作为中心来生长的边界对齐对象(树对.左对象.endindex, true);
		//	foreach (生长对象 左中心对象 in 左中心对象集合)
		//	{
		//		int old边界 = 左中心对象.endindex;
		//		左中心对象.endindex = 树对.左对象.endindex;
		//		生长对象 计算对象对 = 生长对象.未知关联构造待分析对象对(左中心对象, 树对.右对象);//树对象的右对象是一个整体假设为一个参数，试图挂接到左相邻对象上的一个节点（也就是作为中心）上
		//		尝试调整中心对象并完成对象对(计算对象对, 处理轮数, 语义阀值);
		//		左中心对象.endindex = old边界;
		//	}

		//	//假设以右边为中心对象进行尝试
		//	//树对.右对象是真正的树，树对.左对象是单独的对象。
		//	List<生长对象> 右中心对象集合 = 树对.右对象.在树上查找准备作为中心来生长的边界对齐对象(树对.右对象.begindex, false);
		//	foreach (生长对象 右中心对象 in 右中心对象集合)
		//	{
		//		int old边界 = 右中心对象.begindex;
		//		右中心对象.begindex = 树对.右对象.begindex;
		//		生长对象 计算对象对 = 生长对象.未知关联构造待分析对象对(右中心对象, 树对.左对象);//树对象的左对象是一个整体假设为一个参数，试图挂接到右相邻对象上的一个节点（也就是作为中心）上
		//		尝试调整中心对象并完成对象对(计算对象对, 处理轮数, 语义阀值);//
		//		右中心对象.begindex = old边界;
		//	}
		//}

		public void 删除错误的关联(生长对象 左对象, 生长对象 右对象, bool 假设中心在右, List<三级关联> 结果)
		{
			//注意，这段代码对两对对象的端的正反向处理可能还有混乱
			//发现问题后可能要调整。

			Guid 左ID = 左对象.中心第一根类.源模式行ID;
			Guid 右ID = 右对象.中心第一根类.源模式行ID;

			for (int i = 0; i < 结果.Count(); i++)
			{
				三级关联 o = 结果[i];

				int 参数位码 = o.中心主关联.目标.参数.扩展位码;
				if ((参数位码 & 参数字段.A端不再向后派生) > 0)
				{
					Guid A端对象ID = o.左端关联.目标.A端;

					Guid 对象ID = o.that端 == 字典_目标限定.A端 ? 右ID : 左ID;

					if (A端对象ID.Equals(对象ID) == false)
						结果.RemoveAt(i--);
				}
				else if ((参数位码 & 参数字段.B端不再向后派生) > 0)
				{
					Guid B端对象ID = o.右端关联.目标.B端;
					Guid 对象ID = o.that端 == 字典_目标限定.A端 ? 左ID : 右ID;

					if (B端对象ID.Equals(对象ID) == false)
						结果.RemoveAt(i--);
				}

				if (i >= 结果.Count())
					break;
			}
		}

		public void 删除多余的间接关联以及所有的基关联(List<三级关联> 三级关联集合)
		{
			for (int i = 0; i < 三级关联集合.Count(); i++)
			{
				参数树结构 o = 三级关联集合[i].中心主关联;

				bool 是并列关联 = Data.并列关联Guid.Equals(Data.一级关联类型(o.目标));

				for (int j = 0; j < 三级关联集合.Count(); j++)
				{
					if (i == j)
						continue;

					三级关联 o1 = 三级关联集合[j];

					if (是并列关联 && Data.并列关联Guid.Equals(Data.一级关联类型(o1.中心主关联.目标)))
					{
						三级关联集合.RemoveAt(j--);
						goto end;
					}

					//注意，这个可以去掉吗？
					if (三级关联集合[i].that端 != o1.that端)
						continue;

					//去除派生关联。
					if (Data.是派生关联(o1.中心主关联.目标ID, o.目标) > 0 || Data.是根据派生关联计算的派生类(o1.中心主关联.目标ID, o.目标))
					{
						三级关联集合.RemoveAt(j--);
					}
				//有了一级关联，就不需要等价的多级关联了。
				//else if (三级关联集合[i].级数 == 1 && (o.目标ID.Equals(o1.中心主关联.目标ID) || o1.开始端聚合关联 != null && o.目标ID.Equals(o1.开始端聚合关联.目标ID) || o1.结束端聚合关联 != null && o.目标ID.Equals(o1.结束端聚合关联.目标ID)))
				//{
				//	三级关联集合.RemoveAt(j--);
				//}


				end:
					if (i >= 三级关联集合.Count())
						break;
					//删除基关联，考虑只要有派生关联，就屏蔽压制所有基关联。
					//判断关联路径是派生的方法，是把其中的每个关联都对齐进行。
					//bool 是派生关联路径 = 满足的结果[i].级数 == 派生关联.级数;
					//if (是派生关联路径)
					//    for (int j = 0; j < 满足的结果[i].级数; j++)
					//    {
					//        参数树结构 o = 满足的结果[i].关联集合[j];
					//        if (Data.是派生关联(o.目标ID, 派生关联.关联集合[j].目标) == 0 && Data.是根据派生关联计算的派生类(o.目标ID, 派生关联.关联集合[j].目标) == false)
					//        {
					//            是派生关联路径 = false;
					//            break;
					//        }
					//    }
				}
			}
		}

		public void 排除不满足语义阀值的关联(List<三级关联> 三级关联集合/*, int 语义阀值*/)
		{
			//处理前边已经满足过的关联使不要冗余。并且计算加入本参数后整体模式是否还能满足。
			foreach (三级关联 o in 三级关联集合.Where(r => r.有效 == true))
				if (o.语义打分 < 3)
					o.有效 = false;
		}


		public bool 可以作为连动(生长对象 对象对)
		{

			if (Data.是派生类(Data.动作Guid, 对象对.中心对象.中心第一根类.源模式行, 替代.正向替代) == false
				|| Data.是派生类(Data.动作Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代) == false)
				return false;
			//作为连动句的辅助动词（处于前边），只可能拥有后置的宾语，不能拥有前置的主语，被宾和状语等，那些成分让主对象去拥有。

			//if (对象对.参数对象.查找包含的参数角色(字典_语言角色.全部 ^ 字典_语言角色.宾语) == true)
			//    return true;
			if (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.主语) == true)
				return false;
			if (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.把宾) == true)
				return false;
			if (对象对.左对象.中心第一根类.endindex < 对象对.右对象.begindex)
			{
				if (对象对.左对象.查找包含的一级参数语言角色(字典_语言角色.主语) == false)
					return false;
			}

			//if ((对象对.语言角色 & (字典_语言角色.全部 ^ 字典_语言角色.宾语)) > 0)
			//{
			//    //如果当前参数不是作为宾语(也包括自己又是一个连动)，那么查找中心对象是否已经作为了连动，如果是，就不允许。
			//    生长对象 obj = 对象对.递归查找参数对象(对象对.中心对象);
			//    if (obj != null)
			//    {
			//        if (Data.是派生关联(Data.拥有连动Guid, obj.源模式行) > 0)
			//            return true;
			//    }
			//}
			return true;
		}

		public 生长对象 找到等价的聚合对象(生长对象 obj)
		{
			foreach (生长对象 聚合对象 in 全部对象)
			{
				//必须是根对象才可能是聚合对象
				if (聚合对象.中心第一根类 != 聚合对象)
					continue;
				//if (聚合对象.begindex != obj.begindex || 聚合对象.endindex != obj.endindex)
				//	continue;
				if (Data.是介词或者串(聚合对象.模式行, true, true, true))
					continue;
				if (对象具有相同的关联等价(obj, 聚合对象.源模式行))
					return 聚合对象;
			}
			return null;
		}

		//两个对象具有相同的【源记录】。没有计算that和位置等其它信息。
		//注意，目前的算法比较简单，可能有好多问题。这个其实和查询匹配很像，应该做一个同一的。
		public bool 对象具有相同的关联等价(生长对象 obj, 模式 关联行)
		{
			Guid 目标中心对象id1 = obj.中心对象.源模式行ID;
			Guid 目标中心对象id2 = (Guid)关联行.源记录;
			if (目标中心对象id1.Equals(目标中心对象id2) == false)
				return false;
			if (obj.中心对象.中心第一根类 != obj.中心对象)
			{
				if (对象具有相同的关联等价(obj.中心对象, Data.FindRowByID(目标中心对象id2)) == false)
					return false;
			}

			模式 源关联row = null;

			Guid 源关联ID = obj.源模式行ID;

			foreach (模式 r1 in 关联行.端索引表_A端)//Data.模式表.对象集合.Where(r => r.A端 == 关联行.ID)
			{
				//按说应该只有一条。不会多。
				if (源关联ID.Equals(r1.源记录))
				{
					源关联row = r1;
					break;
				}
			}
			if (源关联row == null)
				return false;

			Guid 目标参数对象id1 = obj.参数对象.源模式行ID;
			Guid 目标参数对象id2 = (Guid)源关联row.B端;
			if (目标参数对象id1.Equals(目标参数对象id2) == false)
				return false;

			if (obj.参数对象.中心第一根类 != obj.参数对象)
			{
				if (对象具有相同的关联等价(obj.参数对象, Data.FindRowByID(目标参数对象id2)) == false)
					return false;
			}

			return true;
		}

		//本对象obj是一个关联，连接了中心和参数两个对象。
		//现在obj已经设定了语言角色，今后生成形式的时候，语言角色会传递给参数对象。
		//现在就可以检查，参数对象【拥有形式】的语言角色是否和传递给的语言角色相容。
		//比如”苹果了“，【事物】【属拥】【完成】语义上成立，但是语言角色要求是同位语，而【了】的形式是后状，所以就可以被排除。
		private bool 判断参数方形式和语言角色的符合(生长对象 obj)
		{
			//拥有形式本身不用判断。
			if (Data.是拥有形式(obj.源模式行))
				return true;

			//生长对象 参数方 = obj.选择未完成树对的一枝查找指定根的最新版本对象(obj.参数对象.中心第一根类, 字典_目标限定.B端);
			List<参数> 概念参数表 = obj.参数对象.得到指定根对象的参数表(obj.参数对象.中心第一根类);
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() == false)
					continue;
				模式 原始记录 = o.源关联记录;
				if (Data.是拥有形式(原始记录))
					if ((obj.语言角色 & 原始记录.语言角色) == 0)
						return false;
			}

			return true;
		}

		public bool 有后置间隔(生长对象 obj)
		{
			if (obj.结束_附加串 > 0)
				return true;
			if (obj.中间的和地逗号 != null)
				return true;
			if (obj.中间的和地_附加串 > 0)
				return true;
			if (obj.中间的和地逗号 != null)
				return true;
			if (obj.中间的和地 != null)
				return false;
			if (obj.后置介词逗号_附加串 > 0)
				return true;
			if (obj.后置介词逗号 != null)
				return true;
			if (obj.后置介词_附加串 > 0)
				return true;

			return false;
		}

		public bool 设置语言角色并结合语言语义两者打分判断(生长对象 obj)
		{
			if (基本设置语言角色并结合语言语义两者打分判断(obj) == false)
				return false;

			if (Data.是派生关联(Data.表达拥有内容Guid, obj.源模式行) > 0 && obj.中心在右)//对表达内容要特殊处理。
			{
				if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语) == false)//表达内容在左边的话，在之前必须先有主语
					return false;
				if (有后置间隔(obj))
					return true;
				参数 o = obj.参数对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥括号Guid));
				if (o != null)//有括号或者引号等。
					return true;
				return false;
			}
			//聚合关联时，如果右对象是显示的推理角色，则必须是右端对象为中心
			if (Data.是派生关联(Data.聚合Guid, obj.源模式行) > 0 && Data.是派生类(Data.推理角色Guid, obj.右对象.中心第一根类.源模式行, 替代.正向替代)
				&& obj.右对象.中心第一根类.是介词形式创建的对象 == false && obj.中心在右 == false)
				return false;
			//聚合关联时，介词形式的推理角色，不允许和实体聚合，只允许和动词聚合
			if (Data.是派生关联(Data.聚合Guid, obj.源模式行) > 0 && Data.是派生类(Data.推理角色Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代)
				&& obj.参数对象.中心第一根类.是介词形式创建的对象 == true && Data.能够序列化(obj.中心对象.中心第一根类.源模式行) == false)
				return false;
			//聚合关联时，介词形式的推理角色，不允许做为中心对象
			if (Data.是派生关联(Data.聚合Guid, obj.源模式行) > 0 && Data.是派生类(Data.推理角色Guid, obj.中心对象.中心第一根类.源模式行, 替代.正向替代)
				&& obj.中心对象.中心第一根类.是介词形式创建的对象 == true)
				return false;
			//事件拥有生存阶段时，如果生存阶段在动词的左边，生存阶段必须在主语的右边，先只限于“在”“着”这样的短生存阶段。
			if (Data.是派生类(Data.生存阶段Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代) && obj.参数对象.begindex < obj.中心对象.begindex
				&& obj.参数对象.中心第一根类.长度 == 1)
			{
				if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语))
					return false;
			}
			//时间量不允许做为关联拥有A端
			if (Data.是派生关联(Data.关联拥有A端Guid, obj.源模式行) > 0 && Data.是派生类(Data.时间量Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代))
				return false;
			//人反聚人角色时，左对象为代词时，不符合语义
			//if (Data.是派生类(Data.人反聚人角色Guid, obj.源模式行, 替代.正向替代) && obj.参数对象.是隐藏对象() == false)
			//{
			//    if (Data.是派生类(Data.代词Guid, obj.左对象.中心第一根类.模式行, 替代.正向替代))
			//        return false;
			//    else if (Data.是派生类(Data.人Guid, obj.左对象.中心第一根类.模式行, 替代.正向替代) && obj.中间的和地 != null && obj.中间的和地.是的或者地(true, false))
			//        return false;

			//}
			//事物、人、人角色与[人角色]生长时，需要判断是否允许[聚合关系]或[拥有关系]的生长
			if ((Data.是派生类(Data.人角色Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代) ||
					Data.是派生类(Data.人角色Guid, obj.中心对象.中心第一根类.源模式行, 替代.正向替代))
				&& obj.参数对象.是隐藏对象() == false)
			{
				if (Data.是派生关联(Data.聚合Guid, obj.源模式行) > 0)
				{
					if (obj.中间的和地 != null) //聚合关系时,一定不允许中间有"的"
					{
						return false;
					}
					else
					{
						if (Data.是派生类(Data.代词Guid, obj.左对象.中心第一根类.模式行, 替代.正向替代)) //他爸爸、我妈妈这样的不允许聚合生长
							return false;
						else
							return true;
					}
				}
				else if (Data.是派生关联(Data.角色拥有相对角色Guid, obj.源模式行) > 0)
				{
					//角色拥有角色关系时，一般中间都有“的”(但是，他爸爸、我妈妈这样的例外)
					if (obj.中间的和地 == null && Data.是派生类(Data.代词Guid, obj.左对象.中心第一根类.模式行, 替代.正向替代) == false)
						return false;
				}
			}
			//实体反拥习惯数词时，左对象必须紧挨右对象，如：一拖把，一吃
			if (Data.是派生类(Data.实体反拥习惯数词Guid, obj.源模式行, 替代.正向替代) && obj.中心在右)
			{
				if (obj.左对象.中心第一根类.endindex != obj.右对象.中心第一根类.begindex)
					return false;
			}
			//实体反拥虚拟量词时的特殊处理
			if (Data.是派生类(Data.量词个Guid, obj.参数对象.中心第一根类.模式行, 替代.正向替代))
			{
				//1.中心在左时，右边的量词必须结合数量，如：苹果一个
				if (obj.中心在右 == false)
				{
					if (obj.参数对象 == obj.参数对象.中心第一根类)
						return false;
				}
				else
				{
					if (obj.参数对象 == obj.参数对象.中心第一根类)
					{
						//1.如果没有结合数量，需检查是否左边相邻“这”、“那”等
						if (判断相邻对象是否为指定类型(obj.参数对象, Data.外延指定Guid, false, true) == false)
						{
							//2.再检查是否是准确量词(实体和量词有明确的配置，并且打分大于8分)
							if (obj.源模式行.B端.Equals(obj.参数对象.源模式行ID) == false || obj.源模式行.参数.概率分 < 9)
							{
								//3.再检查“量词”是否有多义性，如果有则不允许生长
								if (多义性对象获取延后分值(obj.参数对象, false, true) > 0)
									return false;
							}
						}

					}
				}
			}
			//事件拥有时间量时，[时间量]必须与中心动词紧密相邻，不允许间隙宾语，如：睡了10年；睡会儿.
			if (Data.是派生类(Data.事件拥有时间量Guid, obj.源模式行, 替代.正向替代) && !obj.中心在右)
			{
				if (Data.是派生类(Data.时间点Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代))
					return false;
				else if (Data.是派生类(Data.特定时间Guid, obj.参数对象.中心第一根类.源模式行, 替代.正向替代))
					return false;
				else if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.宾语))
					return false;
			}
			//物品拥有主人时，左边相邻动词不应该是[抽象给]
			if (Data.是派生类(Data.物品拥有主人Guid, obj.源模式行, 替代.正向替代) && obj.中心在右)
			{
				生长对象 左边动词 = 获取相邻的指定类型节点对象(obj.左对象, Data.事件Guid, false, true);
				if (左边动词 != null && Data.是派生类(Data.抽象给Guid, 左边动词.中心第一根类.模式行, 替代.正向替代))
					return false;
			}
			//关联的具体化程度参数，如果大于0，则要求参数必须是已经生长过的对象
			if (obj.源模式行.参数.具体化程度分 > 0)
			{
				if (obj.参数对象.长度 < 2) //这里为了避免“上”“下”这样的单字直接与事物生长
					return false;
			}
			//关联的[A端和B端必须紧密相邻]参数，如果有效则要求中心根对象和参数必须紧密相邻
			if ((obj.源模式行.参数.扩展位码 & 参数字段.A端和B端必须紧密相邻) > 0)
			{
                if (obj.中心对象.中心第一根类.begindex != obj.参数对象.endindex &&
                    obj.中心对象.中心第一根类.endindex != obj.参数对象.begindex)
                {
                    SubString str;
                    if (obj.中心在右)
                        str = new SubString(obj.参数对象.endindex,obj.中心对象.中心第一根类.begindex);
                    else
                        str = new SubString(obj.中心对象.中心第一根类.endindex, obj.参数对象.begindex);
                    if (string.IsNullOrWhiteSpace(str.ToString())==false)
                        return false;
                }
			}
			//如果中心对象是简称对象，如：中国的简称“中”，则要求参数对象也是简称才可以
            if (obj.中心对象.匹配的形式行 != null && (obj.中心对象.匹配的形式行.参数.词性扩展 & 参数字段.词性_简称) > 0)
			{
                if (obj.参数对象.中心第一根类.匹配的形式行 != null && (obj.参数对象.中心第一根类.匹配的形式行.参数.词性扩展 & 参数字段.词性_简称) <= 0)
					return false;
			}
            //如果参数对象是简称对象，如：中国的简称“中”，则要求中心对象也是简称才可以
            else if (obj.参数对象.匹配的形式行 != null && (obj.参数对象.匹配的形式行.参数.词性扩展 & 参数字段.词性_简称) > 0)
            {
                if (obj.中心对象.中心第一根类.匹配的形式行 != null && (obj.中心对象.中心第一根类.匹配的形式行.参数.词性扩展 & 参数字段.词性_简称) <= 0)
                    return false;
            }
			//
			////事件拥有所在位置时，如果参数是“事物反聚空间位置”时，需要前置介词必须出现
			//if (Data.是派生类(Data.事件拥有所在位置Guid, obj.源模式行, 替代.正向替代))
			//{
			//    if (obj.参数对象.源模式行.ID.Equals(Data.事物反聚空间位置Guid))
			//    {
			//        if (obj.前置介词 == null || 是对象要求的介词形式(obj.源模式行, obj.前置介词) == false)
			//            return false;
			//    }
			//}
			return true;
		}

		public bool 基本设置语言角色并结合语言语义两者打分判断(生长对象 obj)
		{
			//拥有形式本身不用判断。
			if (Data.是拥有形式(obj.源模式行))
				return true;

			//独立语的处理，也就是语义满足，但语言角色不满足的情况，设置为独立语。独立语一般是在主语之前，并且原始角色一般是把宾或者主被来修改的（没有介词而已）。
			//不是把宾和主被而要处理成独立语的，比如时间等，那就是状语而已。
			//处理独立语就是单独进行处理。
			//if ((处理选项 & 处理独立语) > 0)
			//{
			//	if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前独))
			//		return true;
			//	return false;
			//}

			int 可能的角色 = Data.查找关联行的语言角色(obj.源模式行, obj.A端对象.中心第一根类.源模式行, obj.B端对象.中心第一根类.源模式行, obj.that, Data.当前解析语言, obj.中心对象.中心第一根类.是介词形式创建的对象);

			if (obj.that == 字典_目标限定.B端 && (可能的角色 & 字典_语言角色.从定) != 0)//从句定语，B端为正向的反向关联就必然是从句定语。不过这也可以通过设置来实现。
				//if (Data.反向聚合Guid.Equals(Data.二级关联类型(obj.源模式行)) == false)
				return 设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.从定);

			//if (对象中间由的或地关联(obj) == 1)//用了【的】，就是定语
			//{
			//	可能的角色 = 字典_语言角色.定语;
			//	goto 定语;
			//}

			//下边要从位码设置的多个可以承担的语言角色中尝试确定明确的一个。
			//每个语义对象总是能承当定语的。而要承担主语，宾语等就需要知识库中【拥有语言角色】的支持。
			if (obj.传递语言角色 > 0)
				可能的角色 = obj.传递语言角色;
			//可能的角色 |= ((可能的角色 & 字典_语言角色.主语) > 0 || (可能的角色 & 字典_语言角色.宾语) > 0) ? 字典_语言角色.状语 : 字典_语言角色.定语;
			//if ((可能的角色 & 字典_语言角色.主语) > 0 || (可能的角色 & 字典_语言角色.宾语) > 0)
			//    可能的角色 |= 字典_语言角色.状语;

			if (可能的角色 == 字典_语言角色.无)
				return false;

			//这些语言角色只可能承担一个，不会和别的语言角色混合。
			//if (obj.语言角色 == 字典_语言角色.句首 || obj.语言角色 == 字典_语言角色.句尾 || obj.语言角色 == 字典_语言角色.尾标)
			//{
			//	Data.Assert(obj.that == 字典_目标限定.A端);
			//	return 设置并判断假设语言角色的语序和形式的正确性(obj, obj.语言角色);
			//}

			if ((可能的角色 & 字典_语言角色.句首) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.句首))
					return true;

			if ((可能的角色 & 字典_语言角色.句尾) != 0 && obj.中心在右 == false)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.句尾))
					return true;

			if ((可能的角色 & 字典_语言角色.尾标) != 0 && obj.中心在右 == false)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.尾标))
					return true;

			if ((可能的角色 & 字典_语言角色.前同) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前同))
					return true;

			if ((可能的角色 & 字典_语言角色.后同) != 0 && obj.中心在右 == false)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.后同))
					return true;

			if ((可能的角色 & 字典_语言角色.主语) != 0)
			{
				//if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.主语) == false)
				//{//主宾可以转换成状语的形式。当然，也能是定语，但是这种定语一定带【的】，在前边已经处理了
				//	obj.语言角色 = 字典_语言角色.状语;
				//	goto 状语;
				//}
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.主语))
					return true;
			}

			if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语))//在主语前后，【把】和【被】有不同的次序。
			{
				if ((可能的角色 & 字典_语言角色.主被) != 0 /*&& obj.后置介词 != null && (obj.后置介词.取子串 == "被")*/)//【主被】处理也许可以可以不要求介词"被"。
					if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.主被))
						return true;

				if ((可能的角色 & 字典_语言角色.把宾) != 0 /*&& obj.前置介词 != null && (obj.前置介词.取子串 == "把" || obj.前置介词.取子串 == "将")*/)
					if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.把宾))
						return true;
			}
			else
			{
				if ((可能的角色 & 字典_语言角色.主被) != 0 /*&& obj.后置介词 != null && (obj.后置介词.取子串 == "被")*/)//【主被】处理也许可以可以不要求介词"被"。
					if (obj.中心对象.查找已经实现的参数(Data.FindRowByID(Data.事件属拥被动Guid)) != null)//先尝试主被，要求已经是被动！
						if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.主被))
							return true;

				if ((可能的角色 & 字典_语言角色.把宾) != 0 /*&& obj.前置介词 != null && (obj.前置介词.取子串 == "把" || obj.前置介词.取子串 == "将")*/)
					if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.把宾))
						return true;
			}



			//if ((可能的角色 & 字典_语言角色.兼宾) != 0)
			//{
			//    if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.兼宾))
			//        return true;
			//}

			if ((可能的角色 & 字典_语言角色.一宾) != 0)
			{
				//if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.一宾) == false)
				//{//主宾可以转换成状语的形式。当然，也能是定语，但是这种定语一定带【的】，在前边已经处理了
				//	obj.语言角色 = 字典_语言角色.状语;
				//	goto 状语;
				//}
				if ((可能的角色 & 字典_语言角色.兼宾) != 0)//如果允许兼宾，就用兼宾判断
				{
					if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.兼一宾))
						return true;
				}
				else if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.一宾))
					return true;
			}

			if ((可能的角色 & 字典_语言角色.二宾) != 0)
			{
				//if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.二宾) == false)
				//{//主宾可以转换成状语的形式。当然，也能是定语，但是这种定语一定带【的】，在前边已经处理了
				//	obj.语言角色 = 字典_语言角色.状语;
				//	goto 状语;
				//}
				if ((可能的角色 & 字典_语言角色.兼宾) != 0)//兼宾肯定是二宾，如果允许兼宾，就用兼宾判断，不再考虑二宾。
				{
					if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.兼二宾))
						return true;
				}
				else if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.二宾))
					return true;
			}

			if ((可能的角色 & 字典_语言角色.谓宾) != 0)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.谓宾))
					return true;

		状语:
			if ((可能的角色 & 字典_语言角色.得状) != 0 && obj.前置介词 != null && obj.前置介词.取子串 == "得")
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.得状))
					return true;

			if ((可能的角色 & 字典_语言角色.前状) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前状))
					return true;

			if ((可能的角色 & 字典_语言角色.后状) != 0 && obj.中心在右 == false)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.后状))
					return true;

		定语:
			if ((可能的角色 & 字典_语言角色.前定) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前定))
					return true;

			if ((可能的角色 & 字典_语言角色.后定) != 0 && obj.中心在右 == false)
			{
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.后定))
					return true;
			}

			if ((可能的角色 & 字典_语言角色.前独) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前独))
					return true;
		前后句:
			if ((可能的角色 & 字典_语言角色.前句) != 0 && obj.中心在右 == true)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前句))
					return true;
			if ((可能的角色 & 字典_语言角色.后句) != 0 && obj.中心在右 == false)
				if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.后句))
					return true;
			//if ((可能的角色 & 字典_语言角色.宾语) != 0 && obj.中心在右 == true)
			//    //if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.主语))
			//    if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前独))
			//        return true;
			//if ((可能的角色 & 字典_语言角色.主语) != 0 && obj.中心在右 == true)
			//    if (已经有了某个语言角色(obj.中心对象, 字典_语言角色.前独))
			//        if (设置并判断假设语言角色的语序和形式的正确性(obj, 字典_语言角色.前独))
			//            return true;

			return false;
		}

		public bool 参数端可以代表省略对象(生长对象 参数对象)
		{
			if (Data.是派生类(Data.什么Guid, 参数对象.中心第一根类.源模式行, 替代.正向替代))
				return true;
			if (Data.是派生类(Data.量Guid, 参数对象.中心第一根类.源模式行, 替代.正向替代))
			{
				if (参数对象.中心第一根类 == 参数对象)//没有生长过的一级对象不允许，比如【把椅子】【斤苹果】。而【一把椅子】【每斤苹果】等就可以。
					return false;
				return true;
			}
			if (Data.是派生类(Data.定性量Guid, 参数对象.中心第一根类.源模式行, 替代.正向替代))
				return true;
			return false;
		}


		public bool 参数端是很明显的状语(生长对象 对象对)
		{
			模式 参数 = 对象对.参数对象.中心第一根类.源模式行;

			//有显式【地】
			if (对象对.中间的和地 != null && 对象对.中间的和地.是的或者地(false, true) == true)
			{//应该加一些更多的检查才对。
				return true;
			}

			if (Data.是派生类(Data.量Guid, 参数, 替代.正向替代))
			{
				if (Data.是派生类(Data.次数量Guid, 参数, 替代.正向替代))
					return true;
				else if (Data.是派生类(Data.时间量Guid, 参数, 替代.正向替代))
					return true;
			}
			else if (Data.是派生类(Data.符合程度Guid, 参数, 替代.正向替代))
				return true;
			else if (Data.是派生类(Data.动作副词Guid, 参数, 替代.正向替代))
				return true;

			return false;
		}


		public 模式 参数端是很明显的动词状语且不是主语宾语(生长对象 对象对)
		{
			//还可能考虑加入【介词】的判断，比如【通过】【地】等。
			//这个对象对的参数端有【地】，或者是【时间】【地点】【一直】等常规状语。

			模式 row = 对象对.参数对象.中心第一根类.源模式行;
			if (Data.是派生类(Data.量Guid, row, 替代.正向替代))
			{
				if (Data.是派生类(Data.次数量Guid, row, 替代.正向替代) == false)
					return null;
			}

			生长对象 关联对象 = new 生长对象(Data.New派生行(Data.FindRowByID(Data.基本关联Guid), 字典_目标限定.A端, true), 0);
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(关联对象, 对象对.参数对象, false, 结果);

			foreach (三级关联 关联 in 结果)
			{
				int 语言角色 = Data.查找关联行的语言角色(关联.中心主关联.目标, 关联对象.模式行, 对象对.参数对象.模式行, 字典_目标限定.A端, Data.当前解析语言);
				if ((语言角色 & 字典_语言角色.主语) == 0 && ((语言角色 & 字典_语言角色.前状) > 0 && 对象对.中心在右) || (语言角色 & 字典_语言角色.后状) > 0 && 对象对.中心在右 == false)
					//if ( (语言角色 & 字典_语言角色.前状) > 0 && 状语在前)
					return 关联.中心主关联.目标;
			}
			return null;
		}

		public int 最近生长对象个数(int 起始序数)
		{
			int k = 0;
			foreach (生长对象 o in 本轮结果集)
				if (o.有效对象序数 >= 起始序数)
					k++;
			foreach (生长对象 o in 全部对象)
				if (o.有效对象序数 >= 起始序数)
					k++;
			return k;
		}

		public void 从本轮结果集中删除(int 起始序数)
		{
			for (int i = 0; i < 本轮结果集.Count; i++)
			{
				生长对象 o = 本轮结果集[i];
				if (o.有效对象序数 >= 起始序数)
					本轮结果集.RemoveAt(i--);
			}
		}

		public List<生长对象> 最近生长对象(int 起始序数)
		{
			List<生长对象> r = new List<生长对象>();
			foreach (生长对象 o in 本轮结果集)
			{
				if (o.处理轮数 != -2 && o.有效对象序数 >= 起始序数)
					r.Add(o);
			}
			foreach (生长对象 o in 全部对象)
			{
				if (o.处理轮数 != -2 && o.有效对象序数 >= 起始序数)
					r.Add(o);
			}
			return r;
		}

		public bool 用实体角色判断关联的满足性(生长对象 obj)
		{
			//不用介词，也不考虑主语，宾语等语言角色位置标识，要求这个对象自己聚合多继承（通过角色名等）满足【实际扮演】的证据。比如【如果】【那么】等。
			//	还有【借出者他借入者她的借】。

			Guid id = obj.that == 字典_目标限定.B端 ? (Guid)obj.源模式行.A端 : (Guid)obj.源模式行.B端;

			return obj.参数对象.证明满足关联的显式参数证据(id);
		}


		public bool 拆解成两个对象和前边动词结合成动_宾_从结构(生长对象 对象对, ref 生长对象 左对象, ref 生长对象 右对象)
		{
			生长对象 分支 = 对象对.找到第一个原生对象分支();
			if (分支 == null)
				return false;
			右对象 = 分支.右对象.找到第一个原生对象();
			左对象 = 分支.左对象.找到第一个原生对象();
			return true;
		}

		//这种情况非常普遍，特点是，左边是动词A，而右边是一个或者多个名词B，这个时候起码有两种基本解释：
		//一种是正常语序，名词B是动词A的宾语，另一种是从定语序，名词B是动词A的从定中心。
		//其中后者情况下，如果是多个名词，那么还可能是一部分是宾语，最后一部分是从定中心。
		//这种情况仅靠这些信息不能决断，原则上讲是两种可能都生长，保留。
		//但如果有一种生长出来后打分很低，可能就被抑制。
		//对于【【借书】给【她的他】】的处理，进行拆解来进行。生长成为【【借书给她】的他】。
		public int 进行左动右名的正常语序和从定的多义性处理(生长对象 左边对象, 生长对象 右边对象, 封闭范围 范围, int 左边界, int 右边界, int 规则)
		{
			//要求左边是动词，而右边是一个组合名词。

			生长对象 左对象 = null;
			生长对象 右对象 = null;
			if (左边对象.查找已经实现的参数(Data.FindRowByID(Data.概念属拥短句停顿Guid)) != null)
				return 0;
			//只有一个对象，或者拆解不合适，都返回0。
			if (拆解成两个对象和前边动词结合成动_宾_从结构(右边对象, ref  左对象, ref  右对象) == false)
				return 0;
			int 起始序数1 = 有效对象计数;
			List<生长对象> 新对象集合 = null;
			bool 有的或者地 = false;
			关联对象对 关联对象对 = null;

			生长对象 临时左对象 = null;
			生长对象 临时右对象 = null;
			if (拆解成两个对象和前边动词结合成动_宾_从结构(左对象, ref  临时左对象, ref  临时右对象)) //左对象如果还可以拆，需递归拆解，但必须是有介词"的"
			{
				关联对象对 = 创建或者返回可以生长的一个对象对(临时右对象, 临时左对象);
				if (关联对象对.对象对 != null && 关联对象对.对象对.中间的和地 != null)
				{
					if (进行左动右名的正常语序和从定的多义性处理(左边对象, 左对象, 范围, 左边界, 右边界, 规则) > 0)
					{
						新对象集合 = 最近生长对象(起始序数1);
						goto 拆解生长;
					}
				}
			}

			//这里不判断抑制，强制进行生长。
			关联对象对 = 创建或者返回可以生长的一个对象对(左边对象, 左对象);
			生长对象 对象对 = 关联对象对.对象对;
			if (对象对 == null)
			{
				关联对象对 = 创建或者返回可以生长的一个对象对(左对象, 左边对象);
				对象对 = 关联对象对.对象对;
				if (对象对 == null || 对象对.中间的和地 == null) //右边拆解部分为中心时，必须有中间介词“的”
					return 0;
			}
			if (对象对.中间的和地 != null)
				有的或者地 = true;
			if (允许这种类型的生长或创建(对象对, false, 范围, 生长_正常处理, 左边界, 右边界, 规则) == 1)
				尝试进行正常语义关联生长(对象对);
		拆解生长:
			int 起始序数2 = 有效对象计数;

			新对象集合 = 最近生长对象(起始序数1);
			foreach (生长对象 新对象 in 新对象集合)
			{
				if (新对象.处理轮数 != -2 && 新对象.概率打分 > 0 && (新对象.中心第一根类 == 左边对象.中心第一根类
					|| 新对象.中心第一根类 == 左对象.中心第一根类))
				{
					if (拆解成两个对象和前边动词结合成动_宾_从结构(右对象, ref  临时左对象, ref  临时右对象)) //右对象如果还可以拆，再拆一级，但必须是有介词"的"
					{
						关联对象对 = 创建或者返回可以生长的一个对象对(临时右对象, 临时左对象);
						if (关联对象对.对象对 != null && 关联对象对.对象对.中间的和地 != null)
						{
							左对象 = 临时左对象;
							右对象 = 临时右对象;
							关联对象对 = 创建或者返回可以生长的一个对象对(左对象, 新对象);
							if (关联对象对.对象对 == null)
								continue;
							if (有的或者地 == false && 关联对象对.对象对.中间的和地 == null) //必须至少出现一次中间介词"的"
								continue;
							起始序数1 = 有效对象计数;
							尝试进行正常语义关联生长(关联对象对.对象对);
							List<生长对象> 新对象集合2 = 最近生长对象(起始序数1);
							if (新对象集合2.Count > 0)
								goto 拆解生长;
						}
					}
					关联对象对 关联对象对1 = 创建或者返回可以生长的一个对象对(右对象, 新对象);
					生长对象 对象对1 = 关联对象对1.对象对;
					if (对象对1 == null)
						continue;
					if (有的或者地 == false && 对象对1.中间的和地 == null) //必须至少出现一次中间介词"的"
						continue;
					尝试进行正常语义关联生长(对象对1);
				}
			}

			if (最近生长对象个数(起始序数2) > 0)
			{

				//降低从定的生长结果打分
				List<生长对象> 新对象集合2 = 最近生长对象(起始序数2);
				foreach (生长对象 新对象 in 新对象集合2)
				{
					if (新对象.begindex <= 左边对象.begindex && 新对象.endindex >= 右边对象.endindex)
					{
						if (Data.能够序列化(新对象.中心第一根类.模式行) == false && 新对象.源模式行.参数.概率分 > 9)
							新对象.参数.概率分 -= 2;
					}
				}

				return 1;
			}

			从本轮结果集中删除(起始序数1);

			return 0;
		}

		//已经明确了两个节点，假设了一个中心点，如果中心点不对，那么就返回失败。调用者会调换然后重试。
		//明确了两个根对象，实际只会满足一个关联，这里根据语义先查询出多个可能的关联，然后进行过滤后，剩下的可能性大的作为分支分别生长。
		public void 完成对象对所有纵向关联生长(生长对象 对象对/*, int 语义阀值*/)
		{
			int 本次起始序数 = 有效对象计数;
			//只通过字符形式为参数对象查找相邻介词和逗号以及可舍弃字符(对象对);//中心对象总是【干净】的，两边没有【的】【地】【介词】，这些介词附着在参数对象上。
			//if (加上所有部件后对象对是相邻的(对象对) == false)//不相邻。就不生长，等待别人生长。
			//	return;

			//第1步：进行正常语义关联生长。这种情况下，计算出正常的语义关联。
			//这里初始创建集合的生长判断比较严格，如果集合生长成功了，后边的生长一般都不需要进行了。
			//if (最近生长对象个数(本次起始序数) > 0)
			//	return;
			尝试进行比较结果生长(对象对);

			尝试进行正常语义关联生长(对象对/*, 语义阀值*/);

			//第2步：进行名词谓语生长。
			if (最近生长对象个数(本次起始序数) > 0)
				return;
			//先进行名词谓语处理再进行省略对象处理，因为如果出现了孤立的名词谓语，那么，前边即使有【的】，补的空对象也不可能和后边的名词谓语直接结合
			//也就是中间隐含的【基本关联】必然会创建，而另一方面，【基本关联】和【的】一起分析的时候，其实要根据【基本关联】作为中心对象
			//来发起和【的】后【空对象】的整体分析才是最合适的。
			//if (处理阶段 >= 3)
			{
				//if (对象对.中心在右 && 对以前的对象进行了部分拆解(对象对.右对象, 0) == null)//名词谓语总是在右边。
				//{
				//	//注意，这里应该判断这个对象还没有被正常的关系对象所联系，也就是比较游离的状态。
				//	//比如【苹果很红】【苹果昨天红了】里边的【红】，是被【苹果】拥有或者完全游离
				//	生长对象 隐含关联对象 = 尝试进行名词谓语生长(对象对.右对象, null, null);//启动名词谓语生长。
				//}
				//if (本轮结果集.Count - old == 0 && 对象对.中心在右 == false && 对以前的对象进行了部分拆解(对象对.左对象, 0) == null)//【红|了】等这样的情况。
				//{
				//	if (对象对.左对象.begindex == 0 && Data.是派生类(Data.生存阶段Guid, 对象对.右对象.源模式行, 替代.正向替代))//这种情况下名词谓语的左边应该是最左端，因为如果左端不是空的，在上边就应该触发了。
				//		生长对象 隐含关联对象 = 尝试进行名词谓语生长(对象对.左对象, null, null);//启动名词谓语生长。
				//}
				//if (对象对.中心在右 && 对以前的对象进行了部分拆解(对象对.右对象, 0) == null)//名词谓语总是在右边。
				//{
				//	//注意，这里应该判断这个对象还没有被正常的关系对象所联系，也就是比较游离的状态。
				//	//比如【苹果很红】【苹果昨天红了】里边的【红】，是被【苹果】拥有或者完全游离
				//	生长对象 隐含关联对象 = 尝试进行名词谓语生长(对象对.右对象, null, null);//启动名词谓语生长。
				//}
				//if (本轮结果集.Count - old == 0 && 对象对.中心在右 == false && 对以前的对象进行了部分拆解(对象对.左对象, 0) == null)//【红|了】等这样的情况。
				//{
				//	if (对象对.左对象.begindex == 0 && Data.是派生类(Data.生存阶段Guid, 对象对.右对象.源模式行, 替代.正向替代))//这种情况下名词谓语的左边应该是最左端，因为如果左端不是空的，在上边就应该触发了。
				//		生长对象 隐含关联对象 = 尝试进行名词谓语生长(对象对.左对象, null, null);//启动名词谓语生长。
				//}
				//if (名词谓语关联 != null)
				//{
				//	//【苹果红】。
				//	//应该先判断下名词谓语关联是否合适。
				//	尝试进行名词谓语生长(对象对, 对象对.中心对象, 对象对.参数对象, 名词谓语关联, null, 对象对.中心在右/*, 语义阀值*/);
				//}
				//else
				//{
				//	//【红了】【红着】【正在红】【苹果昨天红】【苹果迅速地红】
				//	尝试进行名词谓语生长(对象对, 对象对.中心对象, null, null, 对象对.参数对象, 对象对.中心在右/*, 语义阀值*/);
				//}

				//第3步：进行独立语生长。
				//名词谓语和独立语是互斥的，如果名词谓语成功了，独立语肯定就不用再尝试。
				if (最近生长对象个数(本次起始序数) > 0)
					return;
				//【强调独立语】应该在前边已经进行过了，这里应该只是【他损失惨重】等这种【属主独立语】。

				if (尝试进行作为符合程度的二元关联生长(对象对))
					return;

				//if (处理阶段 >= 4)
				尝试进行独立语生长(对象对/*, 语义阀值*/);

				//第4步：进行补语生长。
				if (最近生长对象个数(本次起始序数) > 0)
					return;
				//对象已结合冒号、破折号时，不进行补语生长
				if (对象对.中心对象.查找已结合的冒号或破折号() != null || 对象对.参数对象.查找已结合的冒号或破折号() != null)
					return;
				尝试进行补语生长(对象对/*, 语义阀值*/);

			}
		}
		public bool 递归判断是对象要求的介词形式(模式 原对象, 生长对象 前置介词)
		{
			if (原对象 == null || 前置介词 == null)
				return false;
			foreach (模式 派生行 in 原对象.端索引表_源记录)
			{
				foreach (模式 row in 派生行.端索引表_A端)
				{
					if (Data.是派生关联(Data.关联拥有前置介词Guid, row) > 0 && 前置介词.取子串 == row.形式)
						return true;
				}
				if (递归判断是对象要求的介词形式(派生行, 前置介词))
					return true;
			}
			return false;
		}
		public 模式 根据A端获取指定关联所派生的具体关联(Guid A端对象Guid, 模式 关联对象)
		{
			if (关联对象 == null || A端对象Guid == null)
				return null;
			foreach (模式 派生行 in 关联对象.端索引表_源记录)
			{
				if (派生行.A端.Equals(A端对象Guid))
					return 派生行;
				模式 关联 = 根据A端获取指定关联所派生的具体关联(A端对象Guid, 派生行);
				if (关联 != null)
					return 关联;
			}
			return null;
		}
		public 模式 根据介词形式获取指定关联所派生的具体关联(模式 关联对象, 生长对象 前置介词)
		{
			if (关联对象 == null || 前置介词 == null)
				return null;
			foreach (模式 派生行 in 关联对象.端索引表_源记录)
			{
				foreach (模式 row in 派生行.端索引表_A端)
				{
					if (Data.是派生关联(Data.关联拥有前置介词Guid, row) > 0 && 前置介词.取子串 == row.形式)
						return Data.FindRowByID(row.A端);
				}
				模式 关联 = 根据介词形式获取指定关联所派生的具体关联(派生行, 前置介词);
				if (关联 != null)
					return 关联;
			}
			return null;
		}
		public bool 是对象要求的介词形式(模式 原对象, 生长对象 前置介词)
		{
			if (原对象 == null || 前置介词 == null)
				return false;
			List<参数> 关联参数集合 = new List<参数>();
			参数树结构 t = Data.利用缓存得到基类和关联记录树(原对象, true);
			t.递归取出关联的形式参数(null, ref 关联参数集合, Data.当前解析语言, 0, 0);
			foreach (参数 形式 in 关联参数集合)
			{
				if (Data.是派生关联(Data.关联拥有前置介词Guid, 形式.源关联记录) > 0 && 前置介词.取子串 == 形式.源关联记录.形式)
					return true;
			}

			return false;
		}
		public 生长对象 获取对象左边相邻的介词(生长对象 原对象)
		{
			int 左边界 = 原对象.begindex;
			int i = 在右边界排序对象中定位(原对象.begindex);
			for (int j = i; j < 右边界排序对象.Count; j++)
			{
				生长对象 相邻对象 = 右边界排序对象[j];
				if (相邻对象.endindex < 左边界)
				{
					break;
				}
				if (Data.是介词或者串(相邻对象.中心第一根类.模式行, true, true, false))
					return 相邻对象;
				if (Data.是派生类(Data.短句停顿Guid, 相邻对象.中心第一根类.模式行, 替代.正向替代))
					左边界 = 原对象.begindex - 1;
			}
			return null;
		}
		public bool 尝试进行比较结果生长(生长对象 对象对)
		{
			if (Data.是派生类(Data.比较标准Guid, 对象对.右对象.中心第一根类.模式行, 替代.聚合替代)
				&& 是对象要求的介词形式(Data.FindRowByID(Data.比较结果拥有B方Guid), 获取对象左边相邻的介词(对象对.左对象)))
			{
				生长对象 比较结果对象 = 创建或返回隐藏对象(Data.比较结果Guid, "", 对象对.右对象.begindex, 对象对.右对象.begindex, null, -2, false);
				if (比较结果对象 == null)
					return false;
				生长对象 临时对象对 = 已知关联构造待分析对象对(比较结果对象, 对象对.右对象, new 参数树结构(Data.FindRowByID(Data.比较结果拥有比较标准Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
				if (临时对象对 != null)
				{
					临时对象对 = 直接一级关联生长(临时对象对, 处理轮数, false);
					if (临时对象对 != null)
					{
						临时对象对 = 未知关联构造待分析对象对(临时对象对, 对象对.左对象);
						只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(临时对象对);
						if (加上所有部件后对象对是相邻的(临时对象对) == false)
							return false;
						if (临时对象对.前置介词 != null && 临时对象对.前置介词.endindex < 临时对象对.左对象.begindex) //中间间隔了逗号的情况
							临时对象对.前置介词 = null;
						完成对象对所有纵向关联生长(临时对象对);
						return true;
					}
				}
			}
			return false;
		}

		public void 尝试进行两元素抽象集合生长(生长对象 对象对, Guid 中心对象ID, string 中心对象名字, int 中心对象位置, 三级关联 左对象关联, 三级关联 右对象关联)
		{
			//生长对象 集合对象 = 查找相同的一级对象(集合基对象.ID, Data.ThisGuid, 集合基对象.ID, 字典_目标限定.A端, 对象对.begindex, 对象对.begindex);
			//if (集合对象 == null)
			//{
			生长对象 集合对象 = 创建或返回隐藏对象(中心对象ID, 中心对象名字, 中心对象位置, 中心对象位置, 对象对.左对象.中心第一根类.中心对象, -2, false);
			if (集合对象 == null)
				return;
			加入结果集排除掉相同的(集合对象).参数.概率分 = 9;//这里先这么设定，实际上，这是需要进行语义计算来得到的。
			//}

			集合对象 = 未知关联构造待分析对象对(集合对象, 对象对.左对象);
			if (集合对象 == null)
				return;
			只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(集合对象);
			集合对象 = 直接三级关联生长(集合对象, -2, 左对象关联, false);
			//obj = 生长对象.已知关联构造待分析对象对(obj, 对象对.右对象, new 参数树结构(Data.FindRowByID(Data.集合拥有后续元素Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
			if (集合对象 == null)
				return;

			集合对象 = 未知关联构造待分析对象对(集合对象, 对象对.右对象);
			if (集合对象 == null)
				return;
			只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(集合对象);
			集合对象 = 直接三级关联生长(集合对象, 处理轮数, 右对象关联, false);
			if (集合对象 == null)
				return;

			对集合对象进行更多元素的生长(集合对象);
		}

		public void 对集合对象进行更多元素的生长(生长对象 集合对象)
		{
		}

		public 封闭范围 递归查找封闭范围(封闭范围 范围, int begindex)
		{
			if (范围.左括号对象 != null && 范围.begindex == begindex)
				return 范围;
			if (范围.子范围 != null)
				foreach (封闭范围 obj in 范围.子范围)
				{
					封闭范围 o = 递归查找封闭范围(obj, begindex);
					if (o != null)
						return o;
				}
			return null;
		}

		public void 执行封闭范围生长(生长对象 对象对, /*int 语义阀值, */封闭范围 范围)
		{
			List<三级关联> 三级关联结果集 = new List<三级关联>();

			参数树结构 关联 = null;

			计算三级关联(范围.左括号对象, 范围.右边界对象, false, 三级关联结果集);//对右端括号的关联。

			foreach (三级关联 o in 三级关联结果集)
				if (Data.拥有后置附件Guid.Equals(Data.返回基本关联(o.中心主关联.目标, false)))
					关联 = o.中心主关联;

			if (关联 == null)//没有找到合适的封闭范围右端关联。
				return;

			对象对.设置源模式行(Data.FindRowByID(Data.概念属拥括号Guid));//先用这个抽象关联，实际上可能派生更具体的关联，比如【说话对双引号】的关联更精确。
			对象对 = 直接一级关联生长(对象对, -2, false);

			if (对象对 == null)
				return;
			对象对 = 已知关联构造待分析对象对(对象对, 范围.右边界对象, 关联, false, 字典_目标限定.A端);//把结尾这个括号挂接到自己身上，这样简化。
			if (对象对 == null)
				return;
			对象对 = 直接一级关联生长(对象对, 处理轮数, true, 范围.左括号对象);

			//if (对象对 == null)
			//	return;
			//加入结果集排除掉相同的(实际谓语);
			//对象对.设置源模式行(Data.FindRowByID(Data.关联拥有B端Guid));
			//对象对 = 进行直接的一级关联生长(对象对, 处理轮数, true);
			//if (对象对 == null)
			//	本轮结果集.Clear();
			//if (A端对象 != null)
			//{
			//	对象对.层级 = -2;
			//	对象对 = 生长对象.未知关联构造待分析对象对(对象对, A端对象);
			//	对象对.设置源模式行(Data.FindRowByID(Data.关联拥有A端Guid));
			//	对象对 = 进行直接的一级关联生长(对象对, 处理轮数, true);
			//	if (对象对 == null)
			//		本轮结果集.Clear();
			//	三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.概念属拥括号Guid), 字典_目标限定.A端, false/*, 0*/)));
			//}
		}

		public bool 左边有非间隔对象(int 位置, bool 允许逗号等)
		{
			//	char c = Data.当前句子串[位置 - 1];
			//	if (字符类.符合类型(c, 字符类.空格))
			//		return false;
			//	return true;
			int k = 在右边界排序对象中定位(位置);
			while (k < 右边界排序对象.Count)
			{
				生长对象 obj = 右边界排序对象[k];
				if (obj.endindex < 位置)
					return false;

				if (允许逗号等 && Data.是派生类(Data.短句停顿Guid, obj.源模式行, 替代.正向替代))
					return false;

				if (Data.是派生类(Data.句子语用基类Guid, obj.源模式行, 替代.正向替代))//句子
					return false;

				//if (Data.是派生类(Data.封闭范围Guid, obj.源模式行, 替代.正向替代))//封闭范围
				//    return false;

				if (Data.是介词或者串(obj.源模式行, true, true, true) == false)//是语义对象，就不可以！
					return true;

				k++;
			}
			return false;
		}

		public bool 对象的左端是封闭的(生长对象 对象, bool 允许逗号等)
		{
			int 位置 = 对象.begindex;
			if (位置 == 0)//最左端开始显然是可以的。
				return true;
			封闭范围 范围 = 内部范围.递归返回左边紧挨的封闭范围(位置);
			if (范围 != null)//左边是一个内部范围的左边界，可以！
				return true;
			if (左边有非间隔对象(位置, 允许逗号等))
				return false;
			return true;
		}

		public void 尝试进行正常语义关联生长(生长对象 对象对/*, int 语义阀值*/)
		{
			//List<参数树结构> 关联结果集 = new List<参数树结构>();
			List<三级关联> 三级关联结果集 = new List<三级关联>();

			if (对象对.参数对象.取子串 == "借给王菲的毛泽东")
				对象对.中心在右 = 对象对.中心在右;

			//括号等特殊处理。
			if (Data.是派生类(Data.封闭范围Guid, 对象对.左对象.源模式行, 替代.正向替代))
			{
				if (对象对.中心在右 == false)
					return;
				if (Data.是派生类(Data.封闭范围Guid, 对象对.右对象.源模式行, 替代.正向替代))
					return;//这里需要判断，如果右边对象是一个空的封闭括号，那么还是可以的。但是对空的封闭括号我们可以创造一个空对象吧？
				封闭范围 范围 = 递归查找封闭范围(内部范围, 对象对.左对象.begindex);
				if (范围 == null)//没有找到封闭范围，应该把这个不封闭的左括号符号作为忽略处理？
					return;
				if (范围.endindex != 对象对.endindex + 1)//对象的右端没有靠近封闭范围，所以还不能进行封闭。
					return;
				执行封闭范围生长(对象对/*, 语义阀值*/, 范围);
				return;
			}

			//if (Data.是介词或者串(对象对.中心对象.当前根.模式行, false, false, true))
			//    return;
			//if (Data.是介词或者串(对象对.参数对象.当前根.模式行, false, false, true))//是串，有可能是拥有语言部件
			//{
			//    直接二元关联集合 = 计算对象两个参数之间的语义关联(对象对, false,true);
			//    goto 执行处理;
			//}

			//参数方已经是句子【有了句号】了，就不能合并了。
			//注意，中心对象是句子的时候，还不能决定，现在也先让不允许再合并。现在暂时允许，因为比如疑问句中，有了【吗】就是句子了，而后边却还有【？】。
			if (已经有非封闭区间内的句号(对象对.参数对象) || 已经有非封闭区间内的句号(对象对.中心对象))
				return;

			//中心不能是语用基类。
			if (Data.是派生类(Data.语用基类Guid, 对象对.中心对象.源模式行, 替代.正向替代))
				return;

			//【生存阶段】如【了，着】等被【事件】【属拥】的概念，不能直接作为中心，而是先被【事件】【属拥聚合】后使用。
			//但是注意，以后，显式的【完成】【开始】可以作为中心！【生存类型】的【发生】也可以作为中心。
			if (Data.是派生类(Data.生存阶段Guid, 对象对.中心对象.源模式行, 替代.正向替代))
				return;

			//必须紧密相邻的对象（比如【什么】和【多少】这样的疑问）和谁都能结合，但肯定要求紧密相邻。
			//if (对象对.左对象.是要求紧密相邻的参数() && 对象对.右对象.中心第一根类.begindex - 对象对.左对象.endindex > 1)
			//	return;
			//if (对象对.右对象.是要求紧密相邻的参数() && 对象对.右对象.begindex - 对象对.左对象.中心第一根类.endindex > 1)
			//	return;

			//这里可能用另一种方法判断相邻性，也就是两者紧密相邻，而在这里的方法，其实是要求在【一级】对象的时候就要完成生长，好像也不太严谨。
			//if (对象对.参数对象.是要求紧密相邻的参数() && 对象对.中心对象 != 对象对.中心对象.中心第一根类)
			//	return;


			//参数是短句停顿。
			if (Data.是派生类(Data.短句停顿Guid, 对象对.参数对象.源模式行, 替代.正向替代))
			{
				//逗号只是附着在这个对象上，并不一定是真正属于这个对象的，最终对逗号的分析，是针对前后字符串来的。
				if (对象对.中心在右)//逗号肯定在右边。
					return;
				//if (对象的左端是封闭的(对象对.中心对象, true) == false)//要求对象的左边是封闭的。
				//    return;
				//这里还可以优化下，直接只创建参数树结构，直接进行二元关联的生长。
				三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.概念属拥短句停顿Guid), 字典_目标限定.A端, false/*, 0*/)));
				goto 执行处理;
			}

			//参数是句子标点。
			if (Data.是派生类(Data.句子语用基类Guid, 对象对.参数对象.源模式行, 替代.正向替代))
			{
				//逗号只是附着在这个对象上，并不一定是真正属于这个对象的，最终对逗号的分析，是针对前后字符串来的。
				if (对象对.中心在右)//逗号肯定在右边。
					return;
				if (对象的左端是封闭的(对象对.中心对象, false) == false)//要求对象的左边是封闭的。
					return;
				//这里还可以优化下，直接只创建参数树结构，直接进行二元关联的生长。
				三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.概念属拥句子Guid), 字典_目标限定.A端, false/*, 0*/)));
				goto 执行处理;
			}

			if (Data.是派生类(Data.推导即命题间关系Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代))//推导关系只能做中心，不能做参数。除非中心本身又是一个推导。暂时不允许那么复杂。
				return;

			//推导只能和推理角色结合
			if (Data.是派生类(Data.推导即命题间关系Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代)
				&& Data.是派生类(Data.推理角色Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代) == false)
				return;
			if (Data.是派生类(Data.推导即命题间关系Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代)
				&& Data.是派生类(Data.推理角色Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代) == false)
				return;

			bool 左对象能序列化 = Data.能够序列化(对象对.左对象.中心第一根类.模式行);
			bool 右对象能序列化 = Data.能够序列化(对象对.右对象.中心第一根类.模式行);
			//反向关联的时候，必须用【的】型句子，也就是定语。
			//正常的关联才可以定义主语，宾语等序列化角色。
			//这个时候，对象对里边的两个对象的位置左右已经排好，两个对象肯定都不是介词。现在就看两者中间的是否是介词。

			//if (对象中间由的或地关联(对象对) != 0)//两个对象的中间由【的】或者【地】关联，肯定是把右对象作为中心语，即使左边是序列化右边非序列化。
			//{
			//    if (对象中间由的或地关联(对象对) == 1)//用了显式【的】，右边肯定是中心对象。
			//    {
			//        //是的从句 = true;//如果中间是【的】，那么就是从句，右边是最终的中心，但左边是生长的基本中心，要合并的是左边里边的一个节点。
			//        //如果放开下边代码，就排除了【他的借】这种左端是【**的】，右端是序列化的中心词的情况。【红色的是苹果】，【的】和【是】中间一定要插入【nullthis】
			//        //关键要点是，一个可以序列化的词，比如【借】和【是】，一般是序列态的，中心也是它，但是要强制作为收缩态的中心（一般是强调），比如【他对她的借】【苹果对水果的属于】，那么就整体是收缩态。
			//        //也就是必须位于最后边（汉语来来说），而不能一部分参数序列化一部分参数不序列。
			//        //语言形式就要成为【定语】！！！
			//        //if (Data.能够做从句的中心(对象对.中心对象.中心第一根类.模式行) == false)
			//        //	return;
			//    }
			//    else//【地】
			//    {
			//    }
			//}
			//else//没有用【的或者地】
			//{
			//    if (左对象能序列化 == true && 右对象能序列化 == false)//左边能序列化，右边不行，那么左边优先作为中心处理
			//        //if (对象对.中心在右 /*&& Data.是派生类(Data.推理角色Guid, 对象对.右对象.当前根.模式行, 替代.正向替代) == false*/)
			//        if (对象对.中心在右 && Data.是派生类(Data.推理角色Guid, 对象对.右对象.中心第一根类.模式行, 替代.正向替代) == false)
			//            return;
			//    if (左对象能序列化 == false && 右对象能序列化 == true)//右边能序列化，左边不行，那么右边优先作为中心处理
			//        //if (对象对.中心在右 == false /*&& Data.是派生类(Data.推理角色Guid, 对象对.左对象.当前根.模式行, 替代.正向替代) == false*/)
			//        if (对象对.中心在右 == false && Data.是派生类(Data.推理角色Guid, 对象对.左对象.中心第一根类.模式行, 替代.正向替代) == false)
			//            return;
			//}
			//if (左对象能序列化 == false && 右对象能序列化 == false)//都不能序列化，那么一般只能是右边是中心语。除非是左边的是非序列化的名词当做序列化来用。
			//{
			//    if (对象对.中心在右 == false)//不满足。
			//        return;
			//}
			if (左对象能序列化 == true && 右对象能序列化 == true)//两边都能序列化。
			{
			}

			//bool 中心是广义集合 = Data.是派生类(Data.形式集合Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代);
			//bool 参数是广义集合 = Data.是派生类(Data.形式集合Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代);
			//bool 中心是表达式 = Data.是派生类(Data.表达式Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代);
			//bool 参数是表达式 = Data.是派生类(Data.表达式Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代);

			//Data.输出串((对象对.中心在右 ? "" : "[中心]") + 对象对.左对象.取子串 + "--" + 对象对.右对象.取子串 + (对象对.中心在右 ? "[中心]" : "")/* + "(序列化:" + 左对象能序列化 + "-" + 右对象能序列化 + ")"*/);

			//if (中心是广义集合 == false && 参数是广义集合 == false)//两边都不是集合的正常情况。
			//	计算三级关联(对象对.左对象, 对象对.右对象, 对象对.中心在右, 三级关联结果集);
			//else
			//{
			//	//对于集合的处理，有两个原则：
			//	//1、【集合增加元素】和【集合整体和外部关联】分开，先把元素增加完全，一旦和外部进行了关联，不是【纯集合】了，就不能再生长元素。
			//	//2、和外部关联的时候，集合应该是完整的，也就是两端都是对象，集合的连接符号不是暴露在外边的。

			//	//if (中心是集合 == true && 参数是集合 == false && (对象对.中心在右 == false || 对象对.中心对象.已满足参数为空()))
			//	if ((中心是广义集合 && 参数是广义集合 == false || (中心是表达式 && 参数是表达式)) && (对象对.中心在右 == false || 对象对.中心对象.集合已有第一元素() == false))
			//	{
			//		//为集合增加元素：
			//		//要么还没有第一元素（最左边），要么集合必须在左边元素在右边，保证集合从左向右生长来减少重复，否则比如【他和她和它】。在两个【和】的前边分别创建了两个集合，可能生长出两个集合就是重复的，现在处理了只有前边一个会长全。暂时没有处理集合的嵌套合并（集合套集合）。
			//		//以后可以改进为，另一个集合可能根本不生长或创建，被第一个集合抑制尝试作为其成员，除非第一个集合死亡。
			//		//同时要求这个集合是【纯】集合，还没有和别的对象产生【集合包含元素】以外的其它关联，才可以，这是因为集合就是一个整体。
			//		if (对象对.中心对象.已经拥有了非集合成员的关联() == false)
			//			计算三级关联(对象对.左对象, 对象对.右对象, 对象对.中心在右, 三级关联结果集);
			//	}
			//	if ((中心是广义集合 == false || 对象对.中心对象.集合已经封闭()) && (参数是广义集合 == false || 对象对.参数对象.集合已经封闭()))
			//	{
			//		//对集合对象整体和另一个对象进行实际关联（而不是为集合增加元素）：
			//		//集合必须是已经完成的，也就是必须包含【第一元素】再加上至少一个第二元素。比如【他和她】，而【他和】以及【和她和它】的情况都不能允许。
			//		//计算关联取出一个元素代表类型来计算，以后应该改进，对集合有一个方法【计算所有元素的共同基类】，这样计算出一个基概念来作为集合的类型进行计算。
			//		//【递归得到共同基类】这个方法其实是已经有了的，利用起来而已。
			//		生长对象 中心对象 = 对象对.中心对象;
			//		生长对象 参数对象 = 对象对.参数对象;

			//		//以下是考虑真正的集合，排除掉【表达式】。因为对于表达式，整体取值就是表达式自己，而不是里边的元素。

			//		if (中心是广义集合 && 中心是表达式 == false)
			//			中心对象 = 对象对.中心对象.查找第一个已经满足的集合元素().参数对象;
			//		if (参数是广义集合 && 参数是表达式 == false)
			//			参数对象 = 对象对.参数对象.查找第一个已经满足的集合元素().参数对象;
			//		List<三级关联> 元素关联路径集合 = new List<三级关联>();
			//		计算三级关联(对象对.中心在右 ? 参数对象 : 中心对象, 对象对.中心在右 ? 中心对象 : 参数对象, 对象对.中心在右, 元素关联路径集合);//还是按照左对象，右对象的次序调用。

			//		if (中心是广义集合 && 中心是表达式 == false || 参数是广义集合 && 参数是表达式 == false)
			//			for (int i = 0; i < 元素关联路径集合.Count(); i++)
			//			{
			//				三级关联 o = 元素关联路径集合[i];
			//				模式 中心端 = o.that端 == 字典_目标限定.B端 ? Data.FindRowByID((Guid)o.右端关联.目标.B端) : Data.FindRowByID((Guid)o.左端关联.目标.A端); ;
			//				模式 参数端 = o.that端 == 字典_目标限定.B端 ? Data.FindRowByID((Guid)o.左端关联.目标.A端) : Data.FindRowByID((Guid)o.右端关联.目标.B端); ;
			//				if ((中心是广义集合 && 中心是表达式 == false && 对象对.中心对象.集合中的所有元素都兼容这个概念(中心端) == false)
			//					|| (参数是广义集合 && 参数是表达式 == false && 对象对.参数对象.集合中的所有元素都兼容这个概念(参数端) == false))
			//				{
			//					元素关联路径集合.RemoveAt(i);
			//					i--;
			//					continue;
			//				}
			//			}

			//		foreach (三级关联 obj in 元素关联路径集合)//合并两个关联集合。
			//			三级关联结果集.Add(obj);
			//	}
			//}

			//Data.输出串((对象对.中心在右 ? "" : "[中心]") + 对象对.左对象.取子串 + "--" + 对象对.右对象.取子串 + (对象对.中心在右 ? "[中心]" : "")/* + "(序列化:" + 左对象能序列化 + "-" + 右对象能序列化 + ")"*/);

			//对动词小品词进行处理
			if (对象对.中心在右 == false && 左对象能序列化)
			{
				if (Data.能够做动词小品词(对象对.右对象.中心第一根类.源模式行) > 0 && Data.是派生类(Data.人对事动作Guid, 对象对.左对象.中心第一根类.源模式行, 替代.正向替代) == false
					&& 多义性对象获取延后分值(对象对.左对象.中心第一根类, false) == 0)
				{
					if (对象对.右对象.查找包含的一级参数语言角色(字典_语言角色.主语) == false)
					{
						三级关联结果集.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.拥有小品动词Guid), 字典_目标限定.A端, false/*, 0*/)));
						goto 执行处理;
					}
				}
			}

			计算三级关联(对象对.左对象, 对象对.右对象, 对象对.中心在右, 三级关联结果集);

			//以下代码用于测试“所有同串对象的三级关联”
		//int a = 在左边界排序对象中定位(对象对.左对象.begindex);
		//for (int j=a; j < 左边界排序对象.Count; j++)
		//{
		//    生长对象 左对象 = 左边界排序对象[j];
		//    if (左对象.endindex > 对象对.左对象.endindex)
		//        break;
		//    if (Data.是介词或者串(左对象.中心第一根类.源模式行,true,true,true))
		//        continue;
		//    if (左对象.begindex == 对象对.左对象.begindex && 左对象.endindex == 对象对.左对象.endindex)
		//    {
		//        int b = 在左边界排序对象中定位(对象对.右对象.begindex);
		//        for (int k = b; k < 左边界排序对象.Count; k++)
		//        {
		//            生长对象 右对象 = 左边界排序对象[k];
		//            if (右对象.endindex > 对象对.右对象.endindex)
		//                break;
		//            if (Data.是介词或者串(右对象.中心第一根类.源模式行,true,true,true))
		//                continue;
		//            if (右对象.begindex == 对象对.右对象.begindex && 右对象.endindex == 对象对.右对象.endindex)
		//            {
		//                //三级关联结果集.Clear();
		//                计算三级关联(左对象, 右对象, 对象对.中心在右, 三级关联结果集);
		//            }
		//        }
		//    }

			//}


		执行处理:

			排除不满足语义阀值的关联(三级关联结果集/*, 语义阀值*/);

			//一些不能做中心的情况。
			foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true))
			{
				if (Data.并列关联Guid.Equals(Data.一级关联类型(o.中心主关联.目标)))
				{
					o.有效 = false;
					continue;
				}

				bool 含反向关联 = Data.是派生类(Data.推理角色Guid, 对象对.B端对象.中心第一根类.模式行, 替代.正向替代) || Data.能够做从句的中心(对象对.A端对象.中心第一根类.模式行);
				//注意：还有问题，也许还需要改进。现在就是即使是强制的【从句】也不允许，因为有时判断不了【的】是否从句。
				//调整that值。从句的情况下就是强制的that，就不调整了。
				//根据关联类型，强行调整that值，去掉不允许的情况。对于拥有，双向都允许。
				//一些关联，比如【属拥】【扮演】我们只允许正向关联，而不允许反向关联，也就是【如果借】，那么是【借】为中心。而另一些，则只允许反向关联不允许正向关联。
				if (o.调整that(对象中间由的或地关联(对象对) == 1) == false)
					o.有效 = false;


			}


			名词谓语关联 = null;

			执行对象对生长(对象对, 三级关联结果集);
		}

		//以右边为参考依据！
		public 生长对象 允许调整重组成另一种中心对象(生长对象 对象)
		{
			return null;

			if (Data.能够序列化(对象.中心第一根类.模式行) == false)
				return null;

			if (Data.是派生关联(Data.属于Guid, 对象.中心第一根类.模式行) > 0)
				return null;

			生长对象 obj = 对象;
			while (obj != null)
			{
				生长对象 右对象 = 对象.右对象;
				if (右对象 == null)
					break;
				if (右对象.中心第一根类 != 对象.中心第一根类)
				{
					if (是后置宾语(右对象, 对象))
					{
						return 右对象;
					}
					break;
				}
				obj = 右对象;
			}

			return null;
		}

		public 生长对象 调整重组成另一种中心对象(生长对象 对象, 生长对象 调整的新中心部件)
		{
			return null;
		}


		//把【是】【有】等二元关联蜕变为【符合程度】
		public bool 可以作为符合程度的二元关联(生长对象 对象)
		{
			if (Data.是派生类(Data.属于Guid, 对象.中心第一根类.源模式行, 替代.正向替代) == false
				&& Data.是派生类(Data.拥有Guid, 对象.中心第一根类.源模式行, 替代.正向替代) == false)
				return false;

			//要求是没有【A端】，而有【B端】
			if (对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有A端Guid)) != null
				 && 对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有B端Guid)) != null)
				return false;
			if (对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有A端Guid)) == null)
			{
				if (对象.语言角色 != 字典_语言角色.一宾 || Data.是派生关联(Data.关联拥有B端Guid, 对象.源模式行) == 0)
					return false;
			}
			else if (对象.查找已经实现的参数(Data.FindRowByID(Data.关联拥有B端Guid)) == null)
			{
				if (Data.是派生关联(Data.关联拥有A端Guid, 对象.源模式行) == 0)
					return false;
			}
			return true;

		}

		public bool 尝试进行作为符合程度的二元关联生长(生长对象 对象对)
		{
			if (Data.能够序列化(对象对.中心对象.中心第一根类.源模式行) == false)
				return false;

			if (可以作为符合程度的二元关联(对象对.参数对象) == false)
				return false;

			//下边正式进入生长
			int 起始序数1 = 有效对象计数;
			生长对象 转换生长对象对 = 已知关联构造待分析对象对(对象对.参数对象.参数对象, 对象对.参数对象.中心对象, new 参数树结构(对象对.参数对象.源模式行, 字典_目标限定.A端, false), false, 字典_目标限定.B端);
			if (转换生长对象对 == null)
				return true;
			转换生长对象对 = 直接一级关联生长(转换生长对象对, -2, false);
			if (转换生长对象对 == null)
				return true;

			int 起始序数2 = 有效对象计数;

			转换生长对象对 = 未知关联构造待分析对象对(对象对.中心对象, 转换生长对象对);

			尝试进行正常语义关联生长(转换生长对象对);

			if (最近生长对象个数(起始序数2) == 0)
				从本轮结果集中删除(起始序数1);

			return true;

		}
		public void 尝试进行补生长或者省略对象及强制关联生长(生长对象 对象对, int 左边界, int 右边界/*, int 语义阀值*/)
		{
			if (对象对.参数对象.是介词或者串(true, true, true))
				return;

			if (判断对已有对象进行了部分拆解(对象对.参数对象) != null)
				return;
			if (尝试进行补生长(对象对))
				return;
			bool 进行两级推断 = Data.能够序列化(对象对.中心对象.中心第一根类.源模式行);
			////参数对象右边只有“的”，比如“美丽的、对的”，而且紧挨短句停顿，需进行特殊处理让“美丽的”先生长后再重新生长
			//if (判断对象的右边是否紧挨的和短句间隔(对象对.右对象, 右边界))
			//{
			//    生长对象 空对象 = 创建或返回隐藏对象(Data.ThisGuid, "[nullthis]", 对象对.右对象.endindex + 1, 对象对.右对象.endindex + 1, null, 0);
			//    生长对象 临时对象对 = 已知关联构造待分析对象对(空对象, 对象对.右对象, new 参数树结构(Data.FindRowByID(Data.基本关联Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
			//    只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(临时对象对);
			//    if (加上所有部件后对象对是相邻的(临时对象对))
			//    {
			//        临时对象对 = 直接一级关联生长(临时对象对, 处理轮数, true);
			//        if (临时对象对 != null)
			//        {
			//            对象对.右对象 = 临时对象对;
			//            if (对象对.中心在右)
			//                对象对 = 未知关联构造待分析对象对(临时对象对, 对象对.左对象);
			//            else
			//                对象对 = 未知关联构造待分析对象对(对象对.左对象, 临时对象对);
			//            完成对象对所有纵向关联生长(对象对);
			//            return;
			//        }
			//    }
			//}
			//中心对象是名词的情况。
			if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) == false)
			{
				if (对象对.中间的和地 != null && 对象对.中间的和地.是的或者地(true, false) == true)//有显式【的】，但其实也可以不要求
				{
					if (Data.是派生类(Data.量Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代))
						return; //如果参数是量，不允许直接生长
					对象对 = 已知关联构造待分析对象对(对象对.中心对象, 对象对.参数对象, new 参数树结构(Data.FindRowByID(Data.基本关联Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
					只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(对象对);
					if (加上所有部件后对象对是相邻的(对象对) == false)
						return;
					对象对 = 直接一级关联生长(对象对, 处理轮数, true);
					if (对象对 != null)
						对象对.参数.概率分 -= 1;
				}
				return;
			}
			//以下，保证了中心对象是动词。

			模式 推断的对象类型 = null;
			模式 参数关联 = null;
			生长对象 推断的对象 = null;

			//以下，中心对象是动词，参数对象是名词。
			//以下是进行省略对象生长。
			if (判断相邻对象是否为指定类型(对象对.参数对象, Data.生存阶段Guid, true))
				return;
			if (参数端可以代表省略对象(对象对.参数对象) || (对象对.中间的和地 != null && 对象对.中间的和地.是的或者地(true, false) == true) ////参数对象是【一个】【什么】等这些，可以是省略对象。或者有显式【的】，可以是省略对象
				|| (对象对.参数对象.中间的和地 != null && 对象对.参数对象.中间的和地.是的或者地(true, false) == true) && 对象对.参数对象.中间的和地.endindex == 对象对.参数对象.endindex) //或者参数对象以【的】结尾，如：红色的
			{
				//这里是根据参数对象推断一个省略的对象，【一个】【红色的】【什么】等，省略的对象肯定是在右边。
				//先根据参数对象计算，比如【红色的】【一个】【什么】等，首先要看这些对象并没有和以前的结合。
				List<参数> 可能的参数关联 = 根据参数对象和形式线索计算关联和中心对象(对象对.参数对象, true, null, null, null, 对象对.中间的和地);
				//如果不能推断省略的关联，那么默认作为【事物】概念。
				可能的参数关联.RemoveAll(r => r.源关联记录.参数.概率分 < 5);
				if (可能的参数关联.Count == 0 || Data.是派生类(Data.什么Guid, 对象对.参数对象.中心第一根类.源模式行, 替代.正向替代))
				{
					推断的对象类型 = Data.FindRowByID(Data.事物概念Guid);
					参数关联 = Data.FindRowByID(Data.基本关联Guid);
				}
				else
				{
					推断的对象类型 = Data.FindRowByID(可能的参数关联[0].that == 字典_目标限定.A端 ? 可能的参数关联[0].源关联记录.B端 : 可能的参数关联[0].源关联记录.A端);
					参数关联 = 可能的参数关联[0].源关联记录;
				}
				//位置暂时先这样，可能还不很正确，因为还有别的介词以及逗号等的问题。
				int 空事物对象位置 = 对象对.中间的和地 == null ? 对象对.参数对象.endindex : 对象对.中间的和地.endindex;
				推断的对象 = 创建或返回隐藏对象(推断的对象类型.ID, 推断的对象类型.形式, 空事物对象位置, 空事物对象位置, null, -2, false);
				if (推断的对象 == null)
					return;
				加入结果集排除掉相同的(推断的对象).参数.概率分 = 9;//这里先这么设定，实际上，这是需要进行语义计算来得到的。
				推断的对象 = 未知关联构造待分析对象对(推断的对象, 对象对.参数对象);
				if (推断的对象 == null)
					return;
				只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(推断的对象);
				推断的对象.设置源模式行(参数关联);
				推断的对象 = 直接一级关联生长(推断的对象, -2, true);//这个对象只为这里的生长服务。

			}
			else
			{
				生长对象 调整的新中心部件 = 允许调整重组成另一种中心对象(对象对.参数对象);
				if (调整的新中心部件 != null)
					推断的对象 = 调整重组成另一种中心对象(对象对.参数对象, 调整的新中心部件);
			}

			if (推断的对象 == null)
				return;
			else
			{
				推断的对象.参数.概率分 -= 2; //省略对象概率分降低
				推断的对象.是省略对象 = true;
			}
			if (是哨兵对象或者封闭区域边界(对象对.中心对象, !对象对.中心在右))
			{
				推断的对象.处理轮数 = 处理轮数;
				return;
			}
			生长对象 新对象对 = 未知关联构造待分析对象对(对象对.中心对象, 推断的对象);
			if (新对象对 == null)//构造失败，一般是因为两个对象是有重叠冲突。
				return;
			只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(新对象对);
			if (加上所有部件后对象对是相邻的(新对象对) == false)
				return;

			//这里可能要先做【老的小的】这种情况的集合处理。
			//剩下的情况，基本可以保证中心对象是动词！

			完成对象对所有纵向关联生长(新对象对/*, 语义阀值*/);

			////这里是根据参数对象推断一个省略的对象，【一个】【红色的】【什么】等，省略的对象肯定是在右边。
			////先根据参数对象计算，比如【红色的】【一个】【什么】等，首先要看这些对象并没有和以前的结合。
			//List<参数> 可能的参数关联 = 根据参数对象和形式线索计算关联和中心对象(对象对.参数对象, true, null, null, null, 对象对.中间的和地);
			//模式 对象1 = Data.FindRowByID(可能的参数关联.Count == 0 ? Data.概念Guid : 可能的参数关联[0].that == 字典_目标限定.A端 ? 可能的参数关联[0].源关联记录.B端 : 可能的参数关联[0].源关联记录.A端);
			//模式 参数关联 = 对象1.ID.Equals(Data.概念Guid) ? Data.FindRowByID(Data.基本关联Guid) : 可能的参数关联[0].源关联记录;

			////然后根据另一方进行计算。比如【他说的】【红色的是漂亮的】【老的借了书给小的】。
			//List<参数> 可能的中心关联 = 根据中心对象和形式线索计算关联和参数对象(对象对.中心对象, 对象对.中心在右, null, 对象对.前置介词, 对象对.后置介词, null);
			//foreach (参数 p in 可能的中心关联)
			//{
			//	模式 对象2 = Data.FindRowByID(p.that == 字典_目标限定.A端 ? p.源关联记录.B端 : p.源关联记录.A端);
			//	三级关联 中心关联 = null;

			//	//如果第一级计算出的对象和前边给出的不兼容，那么就再计算一级，这一级要求是聚合。
			//	//不过可能要调整这个规则，第一级给出的对象如果是【角色概念】等情况，我们才要求再计算一级吧。
			//	if (Data.是派生类(对象2.ID, 对象1, 替代.正向替代))
			//	{
			//		对象2 = 对象1;
			//		中心关联 = new 三级关联(new 参数树结构(p.源关联记录, 对象对.that, false));
			//	}
			//	else if (Data.是派生类(对象1.ID, 对象2, 替代.正向替代))
			//	{
			//		中心关联 = new 三级关联(new 参数树结构(p.源关联记录, 对象对.that, false));
			//	}
			//	else
			//	{
			//		List<参数> 二级参数 = new List<参数>();
			//		参数树结构 tree = Data.利用缓存得到基类和关联记录树(对象2, false);
			//		tree.递归取出形式和关键参数(null, ref 二级参数, Data.当前解析语言, 0, 0);
			//		foreach (参数 聚合参数 in 二级参数)
			//		{
			//			if (Data.聚合Guid.Equals(Data.一级关联类型(聚合参数.源关联记录)) == false)
			//				continue;
			//			对象2 = Data.FindRowByID(聚合参数.that == 字典_目标限定.A端 ? 聚合参数.源关联记录.B端 : 聚合参数.源关联记录.A端);
			//			if (Data.是派生类(对象2.ID, 对象1, 替代.正向替代) || Data.是派生类(对象1.ID, 对象2, 替代.正向替代))
			//			{
			//				if (Data.是派生类(对象2.ID, 对象1, 替代.正向替代))
			//					对象2 = 对象1;
			//				中心关联 = new 三级关联(new 参数树结构(p.源关联记录, 对象对.that, false));
			//				中心关联.结束端聚合关联 = new 参数树结构(聚合参数.源关联记录, 聚合参数.that, false);
			//				中心关联.级数 = 2;
			//				break;
			//			}
			//		}
			//	}
			//	if (中心关联 == null)
			//		continue;

			//	//位置暂时先这样，可能还不很正确，因为还有别的介词以及逗号等的问题。
			//	int 空事物对象位置 = 对象对.中间的和地 == null ? 对象对.参数对象.endindex : 对象对.中间的和地.endindex;

			//	生长对象 空事物对象 = 创建或返回隐藏对象(对象2.ID, 对象2.形式, 空事物对象位置, 对象对.参数对象.中心第一根类.中心对象, -2, false);
			//	if (空事物对象 == null)
			//		continue;
			//	加入结果集排除掉相同的(空事物对象).参数集合.概率分 = 9;//这里先这么设定，实际上，这是需要进行语义计算来得到的。
			//	生长对象 中间对象 = 生长对象.未知关联构造待分析对象对(空事物对象, 对象对.参数对象);
			//	if (中间对象 == null)
			//		continue;
			//	只通过字符形式为参数对象查找相邻介词和逗号以及可舍弃字符(中间对象);
			//	中间对象.设置源模式行(参数关联);
			//	中间对象 = 进行直接的一级关联生长(中间对象, 处理轮数, true);//这个对象后边需要还可以进行别的生长。
			//	if (中间对象 == null)
			//		continue;
			//	生长对象 结果对象 = 生长对象.未知关联构造待分析对象对(对象对.中心对象, 中间对象);
			//	只通过字符形式为参数对象查找相邻介词和逗号以及可舍弃字符(结果对象);
			//	结果对象 = 进行直接的三级关联生长(结果对象, 处理轮数, 中心关联);
			//}

		}
		//用于解决动词已经生长完成，再补入相关语义部分,如“一个歌手，王菲喜欢吃桔子”
		public bool 尝试进行补生长(生长对象 对象对)
		{
			if (对象对.中心对象.取子串 == "艺术，在我看来分为两部分" && 对象对.参数对象.取子串 == "人文艺术")
				对象对.处理轮数 = 对象对.处理轮数;
			//中心在右时，左边是名词，右边是动词，且动词已经进行过生长
			if (Data.是派生类(Data.模板Guid, 对象对.中心对象.源模式行, 替代.正向替代))
				return 进行补生长(对象对, 字典_语言角色.定语);
			if (对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) && !Data.能够序列化(对象对.参数对象.中心第一根类.模式行)
				&& 对象对.中心对象 != 对象对.中心对象.中心第一根类)
			{
				return 进行补生长(对象对, 字典_语言角色.主语);
			}
			//中心在左时，左边是动词，右边是名词，且动词已经进行过生长
			else if (!对象对.中心在右 && Data.能够序列化(对象对.中心对象.中心第一根类.模式行) //&& !Data.能够序列化(对象对.参数对象.中心第一根类.模式行)
				&& 对象对.中心对象 != 对象对.中心对象.中心第一根类
				&& 是否已结合短句停顿符(对象对.中心对象) == true)
			{
				return 进行补生长(对象对, 字典_语言角色.宾语);
			}
			//冒号、破折号介词需要补生长
			else if (对象对.前置介词 != null && 是否短句停顿符介词(对象对.前置介词) && 对象对.前置介词.begindex >= 对象对.左对象.endindex)
			{
				生长对象 替换对象 = null;
				if (对象对.中心对象.中心第一根类 == 对象对.中心对象)
					替换对象 = 对象对.中心对象.中心第一根类;
				else
					替换对象 = 对象对.中心对象.获取已结合的最右端的参数对象();
				return 计算关联并进行替换生长(对象对.参数对象, 替换对象, 对象对.中心对象, true);
			}
			return false;
		}
		public bool 进行补生长(生长对象 对象对, int 语言角色)
		{
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(对象对.参数对象, 对象对.中心对象, true, 结果);
			生长对象 冒号对象 = 对象对.中心对象.查找已结合的冒号或破折号();
			bool 冒号等必须处理 = false;
			if (冒号对象 != null && 冒号对象.endindex == 对象对.中心对象.endindex
				|| (对象对.前置介词 != null && 是否短句停顿符介词(对象对.前置介词) && 对象对.前置介词.begindex >= 对象对.左对象.endindex)) //冒号必须在短句结尾
				冒号等必须处理 = true;
			//对于参数是动词，而且已经结合过其它参数，非冒号等情况不作补生长
			if (对象对.参数对象.中心第一根类 != 对象对.参数对象 && Data.能够序列化(对象对.参数对象.中心第一根类.模式行) && 冒号等必须处理 == false)
				return false;
			//得到中心对象的参数表
			List<参数> 概念参数表 = 对象对.中心对象.得到指定根对象的参数表(对象对.中心对象.中心第一根类);
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() == false)
					continue;
				//主语部分重新合并生长
				if ((o.语言角色 & 语言角色) > 0)
				{
					if (冒号等必须处理) //冒号、破折号等短语一般计算不出关联，必须强制进行尝试生长
					{
						生长对象 已生长对象 = o.对端派生对象.参数对象;
						if (计算关联并进行替换生长(对象对.参数对象, 已生长对象, 对象对.中心对象, true))
							return true;
					}
					else
					{
						foreach (三级关联 关联 in 结果)
						{
							Guid 一级关联类型 = Data.一级关联类型(关联.中心主关联.目标);
							if (关联.that端 == 字典_目标限定.A端 && Data.拥有Guid.Equals(一级关联类型) == false)
								continue;
							if (关联.中心主关联.目标ID.Equals(o.源关联记录.ID)) //补对象与已生长的某个对象可进行合并
							{
								生长对象 已生长对象 = o.对端派生对象.参数对象;

								if (计算关联并进行替换生长(对象对.参数对象, 已生长对象, 对象对.中心对象))
									return true;
							}
						}
					}
				}
			}
			return false;
		}

		public bool 计算关联并进行替换生长(生长对象 参数对象, 生长对象 要替换的对象, 生长对象 原完整对象, bool 是短句分隔介词 = false)
		{
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(参数对象, 要替换的对象, true, 结果);
			生长对象 冒号对象 = 原完整对象.查找已结合的冒号或破折号();
			生长对象 新对象对;
			参数树结构 关联参数;
			生长对象 派生对象;
			//如果是冒号、破折号短句，在计算不出关联时，需要用“概念拥有补语解释Guid”来进行强制生长
			if (冒号对象 != null && 冒号对象.endindex == 原完整对象.endindex || 是短句分隔介词) //冒号必须在短句结尾
			{
				结果.Add(new 三级关联(new 参数树结构(Data.FindRowByID(Data.概念拥有补语解释Guid), 字典_目标限定.A端, false/*, 0*/)));
			}
			foreach (三级关联 关联 in 结果) //有关联时先进行替换生长
			{
				if (关联.语义打分 < 3)
					continue;
				//如果中心对象已经结合冒号，需要先将冒号内移，将其与内部宾语结合
				if (冒号对象 != null && 冒号对象.endindex == 原完整对象.endindex) //冒号必须在短句结尾
				{
					关联参数 = new 参数树结构(Data.FindRowByID(Data.概念属拥短句停顿Guid), 字典_目标限定.A端, false);
					新对象对 = 已知关联构造待分析对象对(要替换的对象, 冒号对象, 关联参数, false, 字典_目标限定.A端);
					生长对象 新参数 = 直接一级关联生长(新对象对, 处理轮数, true);
					if (新参数 != null)
					{
						新对象对 = 已知关联构造待分析对象对(新参数, 参数对象, 关联.中心主关联, false, 字典_目标限定.A端);
						只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(新对象对);
						if (加上所有部件后对象对是相邻的(新对象对) == false)
							continue;
						派生对象 = 执行替换生长(新对象对, 要替换的对象, 原完整对象);
						if (派生对象 != null)
						{
							加入结果集排除掉相同的(派生对象);
							return true;
						}
					}
				}
				else
				{
					//宾语补生长
					if (要替换的对象.中心对象.长度 > 0)
						if (关联.that端 == 字典_目标限定.A端 && 关联.反向 == false)
							新对象对 = 已知关联构造待分析对象对(要替换的对象.参数对象 != null && 要替换的对象.参数对象.是隐藏对象() ? 要替换的对象.中心对象 : 要替换的对象, 参数对象, 关联.中心主关联, false, 字典_目标限定.A端);
						else
							新对象对 = 已知关联构造待分析对象对(参数对象, 要替换的对象.参数对象 != null && 要替换的对象.参数对象.是隐藏对象() ? 要替换的对象.中心对象 : 要替换的对象, 关联.中心主关联, false, 字典_目标限定.A端); // 要替换的对象 == 要替换的对象.中心第一根类?要替换的对象:要替换的对象.中心对象
					else
						新对象对 = 已知关联构造待分析对象对(参数对象, 要替换的对象.参数对象, 关联.中心主关联, true, 字典_目标限定.A端);
					只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(新对象对);
					if (加上所有部件后对象对是相邻的(新对象对) == false)
						continue;
					派生对象 = 执行替换生长(新对象对, 要替换的对象, 原完整对象);
					if (派生对象 != null)
					{
						加入结果集排除掉相同的(派生对象);
						return true;
					}
				}
			}

			//关联替换生长不成功时，再尝试集合生长
			关联参数 = new 参数树结构(Data.FindRowByID(Data.并列集合拥有后续元素Guid), 字典_目标限定.A端, false);
			新对象对 = 已知关联构造待分析对象对(参数对象, 要替换的对象 == 要替换的对象.中心第一根类 ? 要替换的对象 : 要替换的对象.中心对象, 关联参数, false, 字典_目标限定.A端);
			if (新对象对 != null && 计算是否可以做为集合处理(新对象对, false) && 参数对象.查找已结合的推理角色(true) == null)
			{
				只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(新对象对);
				if (加上所有部件后对象对是相邻的(新对象对) == false)
					return false;
				派生对象 = 执行替换生长(新对象对, 要替换的对象, 原完整对象);
				if (派生对象 != null)
				{
					加入结果集排除掉相同的(派生对象);
					return true;
				}
			}
			return false;
		}
		//新对象对:该对象对生长结果，将做为“要替换为的对象”
		//要替换的对象:要进行替换的对象
		//原完整对象：要进行替换的原完整对象
		public 生长对象 执行替换生长(生长对象 新对象对, 生长对象 要替换的对象, 生长对象 原完整对象)
		{
			if (新对象对 == null)
				return null;
			只通过字符形式查找相邻介词_逗号_间隔字符_无效字符(新对象对);
			if (加上所有部件后对象对是相邻的(新对象对))
			{
				生长对象 新参数;
				if (新对象对.前置介词 != null && 新对象对.前置介词.begindex >= 新对象对.左对象.endindex && 是否短句停顿符介词(新对象对.前置介词))
				{
					新对象对.前置介词.是介词形式创建的对象 = true;
					新参数 = 直接一级关联生长(新对象对, 处理轮数, false);
				}
				else
					新参数 = 直接一级关联生长(新对象对, 处理轮数, true);
				if (新参数 != null && 新参数.begindex <= 要替换的对象.begindex && 新参数.endindex >= 要替换的对象.endindex) //不能有损失部分对象
				{
					List<对象对> 替换对象集合 = new List<对象对>();
					替换对象集合.Add(new 对象对(要替换的对象, 新参数));
					生长对象 派生对象 = 原完整对象.替换基概念为派生概念重建对象(替换对象集合);
					if (派生对象 != null)
					{
						派生对象.处理轮数 = 处理轮数;//让这个对象可以参与下轮运算。
						//派生对象.层级 = -2;//还是让这个对象不参与下轮运算？
						//加入结果集排除掉相同的(派生对象);
						return 派生对象;
					}
				}
			}
			return null;
		}
		public void 执行对象对生长(生长对象 对象对, List<三级关联> 三级关联结果集)
		{

			if (三级关联结果集.Count == 0)
				return;

			//bool 中心是广义集合 = Data.是派生类(Data.形式集合Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代);
			//bool 参数是广义集合 = Data.是派生类(Data.抽象形式集合Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代);


			//两个完全相同的对象。
			if (三级关联结果集.Count == 1 /*&& 三级关联结果集[0].并列关联的基类==null*/ && Data.同一Guid.Equals(三级关联结果集[0].中心主关联.目标.ID))
			{
				//两个对象的根完全是等价的关系，这个情况下，有两种可能处理，留到后边具体做的时候处理
				//1、这两个对象各自的参数是独立完整的，那么，就是两个对象，中间用【等价】等关系，成为同位语
				//2、两个对象的参数各自不完整，而是合起来完整，这两个对象可能要合并成一个对象，比如【拥有语言部件】，【离合词】等。
				//中心对象和参数对象完全同一，这种情况下，基本应该是两个对象进行合并成完全同一个对象（或者是集合）。很少情况下可能还是两个对象，就正常处理成为同位语（极少，那种应该是等价，并不是绝对的同一，绝对的同一的重复两次肯定没有意义）。
				//以后扩展可以是多级参数也可以合并，但这里，先就考虑是因为拥有不同的语言部件而让一个对象产生了不同的影子就足够合适了。
				if (对象对.中心在右)//进行一下简化，这种合并总是以左边的为中心进行。
					return;
				//if (对象对.中心对象.是Null类型对象() || 对象对.参数对象.是Null类型对象())
				//	return;

				//是一个对象的不同形式部分，合并成同一个对象。
				//if (对象对.参数对象.处理轮数 == 2)
				{
					//主语：这部分的处理，可能可以合并到【具体化】处理里边，也就是多个【关键参数】决定一个实际对象的创建。
					//参数 o = 对象对.参数对象.中心第一根类.查找第一个已经满足的参数();//应该是【拥有形式】【拥有形式附件】等情况。
					//if (o != null)
					//{
					//Data.Assert(Data.是拥有形式(o.源关联记录));
					//Data.Assert(o.对端派生对象.begindex == 对象对.参数对象.begindex && o.对端派生对象.endindex == 对象对.参数对象.endindex);
					//对象对.参数对象 = o.对端派生对象;
					//以后也应该进行优化，直接进行二元关联处理。
					//三级关联结果集[0] = new 三级关联(new 参数树结构(o.源关联记录, 三级关联结果集[0].中心主关联.生成树时的路径起始端, false/*, 层级*/));
					//}
				}
			}
			else
			{
				foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true /*&& r.并列关联的基类==null*/))
				{

					if (对象中间由的或地关联(对象对) != 1)//不是从句的情况。
					{
						if (o.that端 == 字典_目标限定.B端 && ((Data.可作为中心对象(对象对.中心对象.中心第一根类.模式行, false) == false
							|| Data.是派生类(Data.生存阶段Guid, 对象对.B端对象.中心第一根类.模式行, 替代.正向替代))))//【生存阶段拥有时间】，肯定是生存为中心。
						{
							o.有效 = false;
							continue;
						}
					}

					//if (参数是广义集合 == false)
					//{
					//对于【附属对象】（例如【如果】等），只能是让主对象先进行【属于】（属于，扮演，属拥）结合成一个整体，而不能立即开始被【拥有】等使用。
					//比如【如果他给她借书】，首先创建了【推导】，但【推导】不能直接【拥有】【如果】，而是要让【借】先去【扮演】【如果】，最后，【推导】再【拥有】【借如果】。
					//其实对于【角色】，都应该如此，也就是要等待叶子先结合好。
					//if (替代.可继承参数(Data.一级关联类型(o.中心主关联.目标)) == false && 对象对.参数对象.中心第一根类.是附属对象())
					//{
					//    三级关联结果集.RemoveAt(i);
					//    i--;
					//    continue;
					//}
					//对于一些已经是【封闭】的【完整对象】就不能再【属于】，只能被直接拥有。比如【什么（事物）】，已经结合了【事物】的什么，就不能再结合。
					if (替代.可继承参数(Data.一级关联类型(o.中心主关联.目标)) && 对象对.参数对象.是不能再属拥的终结事物对象())
					{
						o.有效 = false;
						continue;
					}
					//【推导命题】只能去【拥有】参数，而不能【属于】，也就是暂时让【推导命题】只有一级，不能嵌套，让推导成为另一个推导。
					//这样避免现在因为【如果】而创建出的【推导】反过来又和这个【如果】进行属于聚合，这个时候是只能是拥有的。
					//这是临时的，以后要改变，推导本质上也是可以嵌套的。
					if (Data.一级关联类型(o.中心主关联.目标).Equals(Data.拥有Guid) == false && Data.是派生类(Data.推导即命题间关系Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代) && Data.是派生类(Data.推理角色Guid, 对象对.参数对象.中心第一根类.模式行, 替代.正向替代))
					{
						o.有效 = false;
						continue;
					}
					//}
				}
			}

			foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true /*&& r.并列关联的基类 == null*/))
			{
				参数字段 p = o.中心主关联.目标.参数; //new 参数字段((string)o.中心主关联.目标.参数集合);
				////去掉不满足本轮的意义优先性阀值的。
				//if (p.概率分 < 意义优先性阀值)
				//{
				//	被抑制对象.Add(new 生长对象(对象对.A端对象, 对象对.B端对象, Data.FindRowByID(o.中心主关联.目标ID), 对象对.that));
				//	i -= 删除这条关联(ref 三级关联结果集, o);
				//	continue;
				//}

				//去掉不是【拥有语言部件】的字符串。
				//if (Data.是介词或者串(对象对.参数对象.模式行, true, true, true) && Data.是派生关联(Data.拥有语言部件Guid, o.中心主关联.目标) == 0)
				//{
				//	i -= 删除后边的所有基关联或者基类(ref 三级关联结果集, o);
				//	continue;
				//}

				//对于【推导模式】和【二元比较】，不能直接使用【拥有A端】和【拥有B端】这两个原始关联，
				//必须是更具体的【派生关联】来保证【拥有推理角色】而不是泛泛的【拥有A端】和【拥有B端】。
				if (Data.关联拥有A端Guid.Equals(o.中心主关联.目标ID) || Data.关联拥有B端Guid.Equals(o.中心主关联.目标ID))
				{
					if (Data.是派生类(Data.推导即命题间关系Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代) || Data.是派生类(Data.二元比较关系Guid, 对象对.中心对象.中心第一根类.模式行, 替代.正向替代))
						o.有效 = false;
					if (对象对.参数对象.是介词形式创建的对象)//介词形式创建的对象不允许。
						o.有效 = false;
				}
			}

			//计算加入本参数后整体模式是否还能满足，尤其二元关联的。
			foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true /*&& r.并列关联的基类 == null*/))
			{
				int 模式成立类型 = 计算加入新关联后的模式成立度(对象对, o.中心主关联.目标, o.that端/*, 根对象.中心第一根类*/);
				if (模式成立类型 <= 0)
					o.有效 = false;
			}

			//对于名词，如果是【属于】【同一】等参数，那么要求必须是右边为中心，作为简化。
			//当然，也可以规定必须是【派生类】作为中心，这个后边再研究。
			foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true /*&& r.并列关联的基类 == null*/))
			{
				if (替代.是本质正向分类(Data.一级关联类型(o.中心主关联.目标)) && o.that端 == 字典_目标限定.A端)
				{
					//如果左对象是基类，则必须是中心在右
					if (Data.是派生类(对象对.左对象.中心第一根类.源模式行.ID, 对象对.右对象.中心第一根类.源模式行, 替代.正向替代))
					{
						if (对象对.中心在右 == false)
							if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) == false)//动词则无此要求
								o.有效 = false;
					}
					//如果右对象是基类，则必须是中心在左
					else
					{
						if (对象对.中心在右 == true)
							if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) == false)//动词则无此要求
								o.有效 = false;
					}
				}
				//if (对象对.中心在右 == false && 替代.是本质正向分类(Data.一级关联类型(o.中心主关联.目标)) && o.that端 == 字典_目标限定.A端)
				//    if (Data.能够序列化(对象对.中心对象.中心第一根类.模式行) == false)//动词则无此要求
				//        o.有效 = false;
			}

			//对于聚合关联，如果不存在语言角色的配置信息，就不允许做为节点的生长关联
			//foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true /*&& r.并列关联的基类 == null*/))
			//{
			//    if (Data.是派生关联(Data.聚合Guid, o.中心主关联.目标)>0)
			//    { 
			//        if (o.中心主关联.目标.端索引表_Parent.Count<1)
			//            o.有效=false;
			//        else 
			//        {
			//            bool 有语言角色配置 = false;
			//            foreach (模式 row in o.中心主关联.目标.端索引表_Parent)
			//            {
			//                if (row.连接 == Data.拥有语言角色Guid)
			//                { 有语言角色配置 = true; break; }
			//            }
			//            if (有语言角色配置 == false)
			//                o.有效 = false;
			//        }
			//    }
			//}
			//下边循环正式开始处理
			foreach (三级关联 o in 三级关联结果集.Where(r => r.有效 == true))
			{
				生长对象 对象 = 对象对.克隆简单信息();
				对象 = 直接三级关联生长(对象, 处理轮数, o, true);
				//也许需要连续向后生长，但不要在这个地方。
				//if (允许该对象进行连续深度生长(对象))
				//	尝试向右完成所有可能的生长(对象,false);
				//正常生长成功了，也还是可能允许进行名词谓语处理的。后边应该设置概率打分来判断，不过现在暂时先这样。
				//以下代码先屏蔽掉的原因是处理轮数不好控制，比如【他买的桔子红了】，在第一轮就产生了【桔子红了】。而【他买的桔子】后边无法结合进来。
				//if ((r == null && 对象对.中心在右 && 对象对.中间的和地 == null && o.级数 == 1)
				//{
				//    尝试进行名词谓语生长(对象对.中心对象, 对象对.参数对象, o.中心主关联.目标);
				//}
			}
		}

		public bool 允许该对象进行连续深度生长(生长对象 对象)
		{
			return false;
		}
		//从两个模式对象中，比较获得更具体的派生对象，如果不存在派生关系，默认返回模式B
		public 模式 比较获得更具体的派生对象(模式 模式A, 模式 模式B)
		{
			if (模式A != null && 模式B != null)
			{
				if (Data.是派生类(模式B.ID, 模式A, 替代.正向替代))
					return 模式A;
				else
					return 模式B;
			}
			return 模式B == null ? 模式A : 模式B;
		}
		//参数对象是聚合创建的隐藏实体，而中心对象是派生的具体对象时，直接进行中心替换生长
		public 生长对象 尝试进行隐藏实体的替换生长(生长对象 对象对)
		{
			生长对象 替换后对象 = null;
			if (对象对.参数对象.中心第一根类.是隐藏对象())
			{
				if (Data.是派生类(对象对.参数对象.中心第一根类.源模式行.ID, 对象对.中心对象.中心第一根类.源模式行, 替代.正向替代))
				{
					List<对象对> 替换对象集合 = new List<对象对>();
					替换对象集合.Add(new 对象对(对象对.参数对象.中心第一根类, 对象对.中心对象));
					替换后对象 = 对象对.参数对象.替换基概念为派生概念重建对象(替换对象集合);
				}
			}
			return 替换后对象;
		}
		public 生长对象 直接三级关联生长(生长对象 对象对, int 处理轮数, 三级关联 o, bool 执行语言方面检查)
		{
			//if(对象对.参数对象.取子串=="通过海峡时")
			//	o=o;


			//【属于】不会用"的",不会说【苹果的水果】。除非是显式的【属于】,比如【属于水果的苹果】。
			//if (对象中间由的或地关联(对象对) == 1 && 替代.可正向替代(Data.一级关联类型(o.中心主关联.目标)))
			//	continue;

			//DataRow 已实现的关联 = 对象对.参数已经满足(o.目标, o.发起端, false);//这条关联对应的参数已经满足，暂时的处理是不加入了。
			//假设一个关联只能填写一个对象（【A和B】这样的集合对象显然看着一个整体参数。）因为基类结构树是派生类为根，所以，先匹配生了派生类的关联的话，后边的基关联就被屏蔽了。
			//if (已实现的关联 != null/*|| 对象对.参数对象.参数已经满足(o.目标)*/)
			//{
			//	if (是的从句 == false)
			//		continue;
			//	else//是的从句，尝试进行增加一个【等价对象】。或者，保留着到最后统一进行增加处理。
			//	{
			//	}
			//}
			//if (Data.是派生关联(Data.属拥句子Guid, o.目标) > 0)//句子是顶层的，必须是顶层的中心对象来生长。
			//{
			//    if (对象对.中心对象 != 树对.中心对象)
			//        continue;
			//}
			//对于聚合创建的隐藏实体对象,需先尝试进行隐藏实体的替换生长
			生长对象 新对象 = 尝试进行隐藏实体的替换生长(对象对);
			if (新对象 != null)
			{
				新对象.处理轮数 = 处理轮数;
				return 新对象;
			}
			生长对象 根对象 = o.that端 == 字典_目标限定.B端 ? 对象对.参数对象 : 对象对.中心对象;
			//生长对象 最新的上一版本对象 = 对象对.选择未完成树对的一枝查找指定根的最新版本对象(根对象.中心第一根类, o.中心主关联.that);
			int 满足类型 = 根对象.这个参数重复满足(根对象.中心第一根类, o.中心主关联.目标);
			if (满足类型 > 0)
			{
				if (Data.是派生关联(Data.事件反聚推理角色Guid, o.中心主关联.目标) > 0)//【事件反聚推理角色只能有一个】
					return null;

				//【拥有类型】或者是【基本关联】
				Guid 一级关联类型 = Data.一级关联类型(o.中心主关联.目标);
				if (Data.拥有Guid.Equals(一级关联类型) || Data.基本关联Guid.Equals(一级关联类型))
				{
					//这条关联对应的参数已经满足，暂时的处理是不加入了。假设一个关联只能填写一个对象（【A和B】这样的集合对象显然看着一个整体参数。）
					//因为基类结构树是派生类为根，所以，先匹配生了派生类的关联的话，后边的基关联就被屏蔽了。
					if (对象中间由的或地关联(对象对) == 1)
					{
						//是的从句，尝试进行增加一个【等价对象】。或者，保留着到最后统一进行增加处理。比如【他借书给她的他】这两个【他】是重合的。允许。
						//如果是重合，那么，要计算重合的参数是否同一的！等于是一个对象出现在两处。
						//DataRow 等价关联 = Data.FindRowByID(Data.等价Guid);
						//参数树结构 obj1 = new 参数树结构(等价关联, 字典_目标限定.A端, false, 0);
						//直接二元关联集合.Add(obj1);//加入的这个到后边，可能还会因为已经满足而被删除掉。
						return null;
						//暂时这样，这种情况下，【他借书给她的他】中的【他】会无效，后边再调整修改。发现是同一的就允许。
					}
					else//不是的从句的情况下，不允许各参数重复。
						return null;
				}
				//不是拥有类型和拥有形式，那么就看是否需要派生。
				else if (o.开始端聚合关联 == null && o.结束端聚合关联 == null && Data.是拥有形式(o.中心主关联.目标) == false)
				{
					参数 已有参数 = 根对象.查找已经实现的参数(o.中心主关联.目标, null, Data.基类实现);
					List<对象对> 替换对象集合 = new List<对象对>();
					替换对象集合.Add(new PatternApplication.对象对(已有参数.对端派生对象.B端对象.中心第一根类, 对象对.参数对象));
					生长对象 派生对象 = 对象对.中心对象.替换基概念为派生概念重建对象(替换对象集合);
					if (派生对象 == null)
						return null;
					派生对象.处理轮数 = 处理轮数;//让这个对象可以参与下轮运算。
					//派生对象.层级 = -2;//还是让这个对象不参与下轮运算？
					加入结果集排除掉相同的(派生对象);
					return null;
				}
			}

			生长对象 生长对象 = 对象对.克隆简单信息();
			生长对象.that = o.that端;
			模式 关联 = o.中心主关联.目标;

			//处理B端角色参数，也就是【借拥有借出者】【借出者聚合他】的情况。
			//为了减少派生重构，也就是更符合【叶子优先】生长，所以先进行B端的处理。
			//但这里的A端和B端应该是原始的划分，也就是【A端拥有B端】。而不是从句的中心。
			生长对象 实际上端语义对象 = null;
			生长对象 实际下端语义对象 = null;

			int 本次起始序数 = 有效对象计数;

			if (o.结束端聚合关联 != null)
			{
				//首先判断是否已经聚合了角色。如果没有聚合，那么就首先利用角色名和角色所拥有的介词查找。
				//查找不到，就创建一个（依据关联拥有的介词和语言角色创建，只一个，不要冗余）。
				//把这个角色实体和原始的B参数进行聚合，并把介词等都吸收掉。
				//然后把角色实体替换原始的B参数，再继续。
				//基本就是1、先聚合；2、再拥有
				//2016年7月18日做了如下修改，增加了后端的参数，这部分的原因是:
				//在修改以前，【借书给王菲的毛泽东吃桔子】这个句子，因为【借书给王菲的毛泽东】开始已经根据【借】而有了关联【聚合人角色】【借出方】，旧代码根据这个聚合，直接就
				//认为【吃】需要的【聚合人角色】已经存在，就不允许了，造成了最后【毛泽东】缺少了【聚合人角色】【吃的人主体】。
				//修改的原理就是，判断角色是否已经聚合的时候，不只是根据【聚合关联】，还要判断聚合的实际角色，看是否是相容的，如果不相容，那么就要聚合两个角色。
				//参数 角色参数 = 生长对象.B端对象.查找已经实现的参数(o.结束端聚合关联.目标);
				参数 角色参数 = 生长对象.B端对象.查找已经实现的参数(o.结束端聚合关联.目标, null, Data.派生实现, Data.FindRowByID(o.中心主关联.目标.B端));
				if (角色参数 == null)//角色概念不存在
				{
					//预先利用【借】【她】这两个对象和【借拥有借出者关联】来进行计算。如果不满足，就可以不尝试了，是一种优化。

					生长对象 临时对象 = 给定对象和关联计算额外的语言参数(生长对象, o.中心主关联.目标);
					if (临时对象 == null)//预先检查语言角色等关联形式不满足。
						goto 失败;
					//下端创建的虚拟角色对象后边肯定不会有具体化的来派生替换了，因为如果有具体化的，肯定是相邻的，前边应该已经被吸纳了。
					模式 下端虚拟角色对象row = 比较获得更具体的派生对象(Data.FindRowByID(o.结束端聚合关联.目标.B端), Data.FindRowByID((Guid)关联.B端));
					//创建虚拟的【借出者】。
					生长对象 下端虚拟角色对象 = 加入结果集排除掉相同的(创建或返回隐藏对象((Guid)下端虚拟角色对象row.ID, (string)下端虚拟角色对象row.形式, 生长对象.B端对象.中心第一根类.begindex, 生长对象.B端对象.中心第一根类.begindex, null, -2));
					生长对象 下端聚合对象 = 已知关联构造待分析对象对(下端虚拟角色对象, 生长对象.B端对象, o.结束端聚合关联, o.反向, 字典_目标限定.A端);
					if (下端聚合对象 == null)
						goto 失败;
					int 已经扣过一次的分 = 9 - 生长对象.B端对象.参数.概率分;
					生长对象.B端对象 = 直接一级关联生长(下端聚合对象, -2, false);//例如【借出者】和【人】的聚合。【借出者】这里是局部的。
					if (生长对象.B端对象 == null)
						goto 失败;
					生长对象.参数.概率分 = Data.合并概率打分(生长对象.参数.概率分, 生长对象.B端对象.参数.概率分 + 已经扣过一次的分);
					if (生长对象.参数.概率分 <= 0)
						goto 失败;
					实际下端语义对象 = 下端虚拟角色对象;
				}
				else//角色概念已经存在，进行【拥有】计算，比如【借】拥有【人】
				{
					if (角色参数.对端派生对象.A端对象.是隐藏对象())
						实际下端语义对象 = 角色参数.对端派生对象.A端对象;
					else
						实际下端语义对象 = 角色参数.对端派生对象.B端对象;
				}
				//生长对象.B端对象 = 角色参数.对端派生对象.B端对象;
			}
			//处理A端角色参数，也就是【借聚合生存】【生存拥有时间】的情况。
			if (o.开始端聚合关联 != null)
			{
				//首先判断是否已经属拥了需要的另一个主体（一般就是【生存】）。如果没有，那么就创建一个虚拟的先生长。
				//2016年7月18日做了修改，目的和上述保持一致
				//参数 属拥参数 = 生长对象.A端对象.查找已经实现的参数(o.开始端聚合关联.目标);
				参数 属拥参数 = 生长对象.A端对象.查找已经实现的参数(o.开始端聚合关联.目标, null, Data.派生实现, Data.FindRowByID(o.中心主关联.目标.A端));
				if (属拥参数 == null)
				{
					//上端创建的虚拟属拥参数后边可能会被派生替换。
					模式 上端虚拟属拥参数row = 比较获得更具体的派生对象(Data.FindRowByID(o.开始端聚合关联.目标.A端), Data.FindRowByID((Guid)关联.A端));
					//创建虚拟的【生存】。
					生长对象 上端虚拟属拥参数 = 加入结果集排除掉相同的(创建或返回隐藏对象((Guid)上端虚拟属拥参数row.ID, (string)上端虚拟属拥参数row.形式, 生长对象.A端对象.中心第一根类.begindex, 生长对象.A端对象.中心第一根类.begindex, null, -2));
					生长对象 上端聚合对象 = 已知关联构造待分析对象对(生长对象.A端对象, 上端虚拟属拥参数, o.开始端聚合关联, o.反向, 字典_目标限定.A端);
					if (上端聚合对象 == null)
						goto 失败;
					上端聚合对象 = 直接一级关联生长(上端聚合对象, -2, false);
					if (上端聚合对象 == null)
						goto 失败;
					加入结果集排除掉相同的(上端聚合对象);
					int 已经扣过一次的分 = 9 - 生长对象.A端对象.参数.概率分;
					生长对象.A端对象 = 上端聚合对象;
					生长对象.参数.概率分 = Data.合并概率打分(生长对象.参数.概率分, 生长对象.A端对象.参数.概率分 + 已经扣过一次的分);
					if (生长对象.参数.概率分 <= 0)
						goto 失败;
					实际上端语义对象 = 上端虚拟属拥参数;
				}
				else
					实际上端语义对象 = 属拥参数.对端派生对象.B端对象;
			}

			生长对象.设置源模式行(关联);

			生长对象 = 直接一级关联生长(生长对象, 处理轮数, 执行语言方面检查, 实际上端语义对象, 实际下端语义对象);

			if (生长对象 != null)
				return 生长对象;

		失败:
			去除最近的未完成生长对象(本次起始序数);
			return null;
		}


		//一级关联生长，就是两个对象直接的生长，可以不相邻。
		//如果不相邻，那么范围怎么办？先不要合并，范围先用原来的某一个？然后后边要把内部间隔的解决掉才能最后合并范围。
		public 生长对象 直接一级关联生长(生长对象 对象对, int 处理轮数, bool 执行语言语义结合检查, 生长对象 A端实际对象 = null, 生长对象 B端实际对象 = null, 模式 模板行 = null, bool 是特殊处理 = false)
		{
			if (对象对.中心对象.取子串 == "中伤" && 对象对.参数对象.取子串 == "康师傅")
				处理轮数 = 处理轮数;
			Data.Assert(对象对.中心对象.概率分 > 0 && 对象对.参数对象.概率打分 > 0);
			//如果有相同局面对象，说明肯定是有效的，直接返回，不用检查。相同局面对象就是不考虑次序，但加入的所有【关联记录】是一样的。
			对象对.关联总数 += 对象对.A端对象.关联总数 + 对象对.B端对象.关联总数;
			对象对.生长次数++;
			//对象对.根据内部对象完成范围设置();
			生长对象 o = 查找已有的相同局面对象(对象对, false);
			//比如【一个】是系统库中预设的【聚合对象】，那么【一】和【个】再聚合成为【一个】就没有必要了。
			//但这里是否可以和【派生重建】合并呢？其实还真的就是如此吧，就是一回事情啊！
			if (o == null)
				o = 找到等价的聚合对象(对象对);
			if (o != null)
			{
				//o.同一局面计数++;
				//o.同步解析结果到结果表();
				o.有效对象序数 = 有效对象计数++;
				return o;
			}
			//对于短句间隔型的介词需要单独处理，如：冒号和破折号
			//if (对象对.前置介词 != null && 对象对.前置介词.begindex >= 对象对.左对象.endindex && 是否短句停顿符介词(对象对.前置介词))
			//{
			//    执行语言语义结合检查 = false;
			//    对象对.前置介词.是介词形式创建的对象 = true;
			//}
			//一、进行关联参数方面的检查。完成语言方面的生长。
			if (构建关联的形式参数及判断角色满足性(对象对, 执行语言语义结合检查) == false)
				return null;
			//这里检查下，这里的【根据内部对象完成范围设置】实际上是要做其它一些事情，而实际范围应该不会变化，否则，前边调整范围后【查找已有的相同局面对象】的算法就有问题。
			对象对.根据内部对象完成范围设置();
			int 序号 = 对象对.参数左边界();

			if (模板行 != null)
				对象对.设置模式行(模板行);
			else
			{
				//二、执行语义对象的生长。
				模式 row = 创建或返回一级关联素材记录(对象对, A端实际对象, B端实际对象);

				//把所有能聚合的关联都升级为等价。成为实际的事实！并且可以进行双向计算。但现在，因为计算三级关联是双向分别作，所以，似乎不需要这个了。不过，为了加深这个聚合成为事实的效果，
				//尤其是显式的聚合成为了【等价】的情况，还是保留这个。
				//但是，现在在计算派生类的时候，会因为这个原因，把【聚合】的算入，不太好，所以，还是先去掉了。
				//if (替代.是分类或聚合(Data.一级关联类型(row)))
				//	row.连接 = Data.等价Guid;
				对象对.设置模式行(row);
			}

			//这里对对象完成前，对参数进行封闭，如果参数的完成性太差，那么就要扣很多分。
			//但目前暂时又点问题，先屏蔽了，后边再进行调整。
			//对象对.参数.概率分 = Data.合并概率打分(对象对.参数.概率分, 对象封闭前进行完整性打分(对象对.参数对象));

			if (对象对.参数.概率分 <= 0)
				return null;

			对象对.模式行.序号 = 序号;
			对象对.A端实际对象 = A端实际对象 == null ? 对象对.A端对象 : A端实际对象;
			对象对.B端实际对象 = B端实际对象 == null ? 对象对.B端对象 : B端实际对象;
			if (B端实际对象 != null)
				对象对.模式行.B端 = B端实际对象.模式行.ID;
			对象对.完成两端的对象参数表();
			//if (对象对.长度 > 对象对.中心对象.长度 && 对象对.长度 > 对象对.参数对象.长度)//正常的生长以后，对象的打分增加1分。
			//	对象对.参数.概率分 += 1;

			对象对.处理轮数 = 处理轮数;
			设置集合基类型(对象对);
			加入对象到当前对象集(对象对);

			对象对 = 生长后的特殊处理(对象对);

			if (是特殊处理 == null)
				对象对.完成分 = 对象封闭前进行完成性打分(对象对);

			return 对象对;
		}
		public bool 是否短句停顿符介词(生长对象 介词)
		{
			int i = 在左边界排序对象中定位(介词.begindex);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 右对象 = 左边界排序对象[j];
				if (右对象.begindex > 介词.begindex)
					break;
				if (右对象 != 介词 && 右对象.长度 == 介词.长度)
				{
					if (是封闭范围或短句停顿符(右对象))
						return true;
				}
			}
			return false;
		}
		public bool 是否有指定类型解释(生长对象 介词, Guid 对象类型Guid)
		{
			int i = 在左边界排序对象中定位(介词.begindex);
			for (int j = i; j < 左边界排序对象.Count; j++)
			{
				生长对象 右对象 = 左边界排序对象[j];
				if (右对象.begindex > 介词.begindex)
					break;
				if (右对象 != 介词 && 右对象.长度 == 介词.长度)
				{
					if (Data.是派生类(对象类型Guid, 右对象.中心第一根类.源模式行, 替代.正向替代))
						return true;
				}
			}
			return false;
		}
		public int 对象封闭前进行完成性打分(生长对象 对象)
		{
			List<参数> 概念参数表 = 对象.得到指定根对象的参数表(对象.中心第一根类);
			int 扣分 = 0;
			foreach (参数 o in 概念参数表)
			{
				if (o.已经派生() || Data.是拥有形式(o.源关联记录))//拥有形式暂时不参与这个打分。
					continue;

				if (o.that == 字典_目标限定.A端)
					扣分 += o.源关联记录.参数.B对A的关键性;
			}
			return 9 - 扣分;
		}


		public 生长对象 生长后的特殊处理(生长对象 对象对)
		{
			if (对象对.前置介词 != null && 对象对.前置介词.取子串 == "被")//被动处理
			{
				对象对.处理轮数 = -2;
				生长对象 虚拟被 = 加入结果集排除掉相同的(创建或返回隐藏对象(Data.ThisGuid, "[被]", 对象对.begindex, 对象对.begindex, null, 0));
				对象对 = 已知关联构造待分析对象对(对象对, 虚拟被, new 参数树结构(Data.FindRowByID(Data.事件属拥被动Guid), 字典_目标限定.A端, false), false, 字典_目标限定.A端);
				对象对 = 直接一级关联生长(对象对, 处理轮数, false, null, null, null, true);
				对象对.参数.概率分++;
			}
			return 对象对;
		}

		public bool 构建关联的形式参数及判断角色满足性(生长对象 对象对, bool 执行检查)
		{
			对象对.构建并匹配关联本身的所有形式参数_和语言角色无关(Data.当前解析语言);
			//一、进行关联参数方面的检查。
			//注意，下边这些部分考虑要移到外边去。直接进行聚合角色的处理。
			//然后下边对于语言角色以及介词的处理，就完全针对【关联所拥有的】来做了！
			//语义角色有三种方法决定：1、语言角色（包括介词）；2、语义角色名称；3、本质决定（省略角色名称）
			if (执行检查)
			{
				if (加上所有部件后对象对是相邻的(对象对) == false)
				{
					return false;
				}
				if (设置语言角色并结合语言语义两者打分判断(对象对) == false)
				{
					return false;
				}
				if (判断参数方形式和语言角色的符合(对象对) == false)
				{
					return false;
				}
			}
			else
			{
				设置语言角色并结合语言语义两者打分判断(对象对);//设置语言角色但不检查。
			}
			return true;
		}

		//这两个对象是调整后的，比如【借出】和【她】，【借出】和【时间】。
		public 生长对象 给定对象和关联计算额外的语言参数(生长对象 对象, 模式 关联)
		{
			生长对象 对象对 = 对象.克隆简单信息();
			对象对.设置源模式行(关联);

			if (构建关联的形式参数及判断角色满足性(对象对, true) == true)
				return 对象对;
			return null;
		}

		public bool 是左边边界空对象(生长对象 对象)
		{
			if (对象 == 起始哨兵)
				return true;

			if (对象.是NullThis空对象() && 内部范围.递归返回左边紧挨的封闭范围(对象.begindex) != null)
				return true;

			return false;
		}

		public bool 是右边边界空对象(生长对象 对象)
		{
			if (对象 == 结束哨兵)
				return true;

			if (对象.是NullThis空对象() && 内部范围.递归返回右边紧挨的封闭范围(对象.endindex) != null)
				return true;

			return false;
		}

		public bool 计算两个对象能并列(生长对象 左对象, 生长对象 右对象)
		{
			List<三级关联> 结果 = new List<三级关联>();
			计算三级关联(左对象, 右对象, false, 结果);
			foreach (三级关联 obj in 结果)
			{
				if (Data.并列关联Guid.Equals(Data.一级关联类型(obj.中心主关联.目标)))
				{
					return true;
				}
			}
			return false;
		}

		public 并列关联 计算两元素的共同基类(生长对象 左对象, 生长对象 右对象)
		{
			//以下计算并列的关联，也就是共同基类的。这个只需要一个方向计算。
			//if (对象对.左对象 == null || 对象对.右对象 == null)
			//	return null;


			List<并列关联> 结果 = new List<并列关联>();

			if (是左边边界空对象(左对象))
				return new 并列关联(右对象.源模式行, 0, 0);

			if (是右边边界空对象(右对象))
				return new 并列关联(左对象.源模式行, 0, 0);

			if (左对象.集合对象的基对象 != null && Data.ThisGuid.Equals(左对象.集合对象的基对象.ID))
				return new 并列关联(右对象.源模式行, 0, 0);

			if (右对象.集合对象的基对象 != null && Data.ThisGuid.Equals(右对象.集合对象的基对象.ID))
				return new 并列关联(左对象.源模式行, 0, 0);

			参数树结构 起始展开树 = 左对象.集合对象的基对象 != null ? Data.利用缓存得到基类和关联记录树(左对象.集合对象的基对象, Data.是二元关联(左对象.集合对象的基对象, false)) : 左对象.利用缓存得到基类和关联记录树();
			参数树结构 终止基类树 = 右对象.集合对象的基对象 != null ? Data.利用缓存得到基类和关联记录树(右对象.集合对象的基对象, Data.是二元关联(右对象.集合对象的基对象, false)) : 右对象.利用缓存得到基类和关联记录树();

			起始展开树.递归计算相似性关联(终止基类树, 0, ref 结果);

			if (结果.Count == 0)
				return null;
			并列关联 r = 结果[0];
			for (int i = 1; i < 结果.Count; i++)
			{
				if (结果[i].总距离 < r.总距离)
					r = 结果[i];
			}
			return r;

			//基准类型 基准对象 = null;
			//int k = 计算基准对象(共同基类对象, ref 基准对象);
			//return k + 基准对象.层级;
		}

		public 生长对象 加入结果集排除掉相同的(生长对象 对象)
		{
			生长对象 o = 查找已有的相同局面对象(对象, true);
			if (o != null)
				return o;
			加入对象到当前对象集(对象);
			return 对象;
		}

		public 生长对象 加入对象池排除掉相同的(生长对象 对象)
		{
			生长对象 o = 查找已有的相同局面对象(对象, true);
			if (o != null)
				return o;
			加入一个对象到池(对象);
			return 对象;
		}
		//参数端的一级或者二级关联生长，结果可以不相邻。这里的三级关联的第一级应该是null，第二级是拥有，第三级是聚合或者为null。
		//比如【借拥有借出者】【借出者聚合他】。
		public 生长对象 进行拥有加聚合的一级或者二级关联生长(生长对象 对象对, 三级关联 关联, int 处理轮数, bool 是附加形式参数)
		{
			生长对象 B端参数 = 对象对.参数对象;
			//如果右边参数端有聚合关联，那么先解决聚合。参数端的聚合肯定是相邻的。
			if (关联.右端关联 != null)
			{
				Guid 角色ID = (Guid)关联.中心主关联.目标.B端;
				//查询B端参数是否已经聚合了角色。
				//如果已经聚合（一般是显式的角色名创建的角色，比如“从借出者他借书给她”），那么就试图用该角色来吸收介词和语言角色。【角色名】【语言角色】【介词】几个信息可以冗余共同创建一个角色。
				//因为已经聚合了角色，所以如果没有介词也语言角色也是有效的。
				//如果没有聚合，是否可以调用相邻查找角色名？当然，不调也可以，因为聚合本身可以自动在上一轮完成。
				//然后就试图用【语言角色】和【介词】来创建角色并完成聚合。如果找不到，就要看这个角色对【介词】等的要求的权重是多少，如果要求很必要，那么就认为是失败。
			}
			//进行【拥有】的生长。
			对象对.设置源模式行(关联.中心主关联.目标);
			return 直接一级关联生长(对象对, 处理轮数, true);
		}

		//参数端的二级或者三级关联生长，这里的三级关联的第一级应该是聚合，第二级是拥有，第三级是聚合或者为null。
		//比如【借聚合发生】【发生拥有时间】。
		public 生长对象 进行聚合加拥有的二级或者三级关联生长(生长对象 对象对, 三级关联 关联, int 处理轮数, bool 加入结果集)
		{
			//然后试图把中间的参数都吸收进来。如果有参数不能吸收，那么就终止，等待中间的参数生长。
			//成功以后一次完成所有的生长。具体地：首先让着两个主体先聚合？然后再去【拥有】各自的参数？这样好像不是叶子的感觉，但似乎不是最重要的。
			return null;
		}

		//保留着暂时不做的另一个版本。这个是一次完成多个生长。最终结果必须相邻。中间环节可能不相邻。
		//用字符位码来计算已经完成了所有的参数的匹配。
		public 生长对象 进行聚合加拥有的二级或者三级关联生长2(生长对象 对象对, 三级关联 关联, List<生长对象> A端聚合的所有对象, int 处理轮数, bool 加入结果集)
		{
			//A端对象是可以不相邻的，彼此要进行聚合的所有对象。
			//最后完成的时候A端对象们相互之间间隔的对象，都要被吸收。
			//如果中间发现了更多需要聚合的参数，那么可以继续加入到【A端聚合的所有对象】里边去。

			//查找不相邻的聚合对象时，怎么保证中间的参数能满足呢？试图去解决这些参数，原则肯定是先找已经长成的【大的】。
			//放置一个数组，挂接所有的参数如何？好！因为最多两级，所以容易解决，这个参数数组记录自己准备要的主对象。
			//这个数组，应该是一个【A端对象】【三级关联】【B端对象】的列表吧？对，确实如此！这样就够了。
			//因此，前边首先是一个准备过程！这个过程只是准备，而不真正执行！

			//这个方法，由对象对和关联发起，有一个远程不相邻的聚合，而中间间隔了多个不确定的其它参数，所以最后可能返回多个结果。
			//这些结果直接加入到结果集就好，其实并不需要返回。

			return null;
		}

		//比如查找或者创建【借出者角色】或者【生存】，查找肯定优先。
		public 生长对象 查找或者创建需要的一个对象(DataRow 对象类型, bool 要求相邻)
		{
			//对于参数端的角色处理，就要求相邻。
			return null;
		}

		//1、没有找到。2、已经存在，且是等价或者派生的。3、已经存在，但是基类或者占位。
		public 生长对象 查找已经聚合或者属于的参数(生长对象 对象, DataRow 参数类型)
		{
			return null;
		}


		public void 进行附加关联处理(生长对象 对象对)
		{
			if (Data.是派生关联(Data.松散并列Guid, 对象对.源模式行) > 0)
				递归为前后两组对象计算附加关联(对象对.左对象, 对象对.右对象, 对象对.右对象, 附加关联集合);
		}



		public bool 是可以附加关联的对象(生长对象 对象)
		{
			//疑问算子就不进行附加关联了。
			if (Data.是疑问对象(对象.模式行) > 0)
				return false;

			//暂时只允许代词进行计算。
			if (Data.是派生类(Data.代词Guid, 对象.中心第一根类.源模式行, 替代.正向替代))
				return true;


			return false;
		}



		public int 计算两个对象进行附加关联的匹配度(生长对象 左对象树, 生长对象 左对象, 生长对象 右对象树, 生长对象 目标对象)
		{
			if (Data.是二元关联(左对象.模式行, true))
				return 0;
			if (Data.是派生类(Data.代词Guid, 目标对象.源模式行, 替代.正向替代))
			{
				Guid 代词类型 = Data.取得代词代表类型(目标对象.源模式行ID);
				if (代词类型.Equals(Guid.Empty) == false)
					if (Data.是派生类(代词类型, 左对象.源模式行, 替代.正向替代))
						return 10;
			}
			return 0;
		}


		//根据目标对象，在左对象树上找匹配的附加对象.
		public void 递归为一个对象在一组对象中寻找一个最合适的对应对象(生长对象 左对象树, 生长对象 左边主对象, 生长对象 右对象树, 生长对象 右边附着对象, ref int 匹配度, ref 生长对象 结果)
		{
			if (左边主对象 == 右边附着对象)//目标对象和自己相遇了，就结束，因为目标对象在右，匹配对象在左。
				return;

			if (左边主对象.左对象 != null)
				递归为一个对象在一组对象中寻找一个最合适的对应对象(左对象树, 左边主对象.左对象, 右对象树, 右边附着对象, ref  匹配度, ref 结果);


			int v = 计算两个对象进行附加关联的匹配度(左对象树, 左边主对象, 右对象树, 右边附着对象);
			if (v > 匹配度)
			{
				匹配度 = v;
				结果 = 左边主对象;
			}

			if (左边主对象.右对象 != null)
				递归为一个对象在一组对象中寻找一个最合适的对应对象(左对象树, 左边主对象.右对象, 右对象树, 右边附着对象, ref  匹配度, ref 结果);
		}



		public void 递归为前后两组对象计算附加关联(生长对象 左对象树, 生长对象 右对象树, 生长对象 附着对象, List<模式> 附加关联)
		{
			//先算左边，再中间，然后是右边。
			if (附着对象.左对象 != null)
				递归为前后两组对象计算附加关联(左对象树, 右对象树, 附着对象.左对象, 附加关联);

			if (是可以附加关联的对象(附着对象))
			{
				int 匹配度 = 0;

				生长对象 主对象 = null;
				递归为一个对象在一组对象中寻找一个最合适的对应对象(左对象树, 左对象树, 右对象树, 附着对象, ref 匹配度, ref 主对象);
				if (主对象 != null)
					附加关联.Add(Data.创建附加关联(Data.等价Guid, 主对象.模式行, 附着对象.模式行));
			}

			if (附着对象.右对象 != null)
				递归为前后两组对象计算附加关联(左对象树, 右对象树, 附着对象.右对象, 附加关联);
		}

	}

}


