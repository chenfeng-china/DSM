DSM深度语义模型又可以称为DCSN（动态认知语义网络），是DCN动态认知网络(http://arxiv.org/abs/2010.04551)针对自然语言和语义表达和处理的具体实现。
本项目是一个较早期的版本，其中包含了DSM的基本模型定义、基本知识库以及描述、理解、生成、查询、转换、推理、学习等算法的原理演示。
DSM的主要特点如下：
DSM整体模型由语义和语言两个模型构成，语义和语言彻底分离。
基本结构结构类似三元组，但更为强大。用这个统一结构表达各种语义和语言知识。
语义模型构建采取自顶向下方法，形成金字塔形的知识和信息结构。
语言模型采用树形网结构。类似AMR抽象语义表示，但更为完整和具体。
语义模型和语言模型进行映射转换。
模型中各种知识全面地采用概率表达。
围绕基本模型实现包括理解、生成、查询、推理、计算、学习在内的算法体系闭环。
各种计算结合概率，选取综合打分最优的结果。
