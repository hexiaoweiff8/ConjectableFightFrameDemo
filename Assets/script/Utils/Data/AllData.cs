using System.Collections.Generic;

/// <summary>
/// 数据持有类 隶属于ClusterData, 所有数据放入该类
/// </summary>
public class AllData : ISelectWeightDataHolder
{
    /// <summary>
    /// 目标筛选数据
    /// </summary>
    public MemberData MemberData { get; set; }

    /// <summary>
    /// 目标选择权重数据
    /// </summary>
    public SelectWeightData SelectWeightData { get; set; }

    /// <summary>
    /// 单位特效数据
    /// </summary>
    public EffectData EffectData { get; set; }

    ///// <summary>
    ///// 技能列表
    ///// </summary>
    //public IList<SkillInfo> SkillInfoList { get; set; }

    ///// <summary>
    ///// buff列表
    ///// </summary>
    //public IList<BuffInfo> BuffInfoList = new List<BuffInfo>();

    ///// <summary>
    ///// 光环列表
    ///// </summary>
    //public IList<RemainInfo> RemainInfoList = new List<RemainInfo>();

    /// <summary>
    /// 图形类型
    /// </summary>
    public GraphicType GraphicType { get; set; }


}