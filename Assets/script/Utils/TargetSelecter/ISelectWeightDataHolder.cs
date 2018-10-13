/// <summary>
/// 选择目标权重抽象类
/// </summary>
public interface ISelectWeightDataHolder
{
    SelectWeightData SelectWeightData { get; set; }
}

/// <summary>
/// 目标选择权重
/// </summary>
public class SelectWeightData
{
    // Level 1, 2, 3所有值都是从-1 - 正无穷, -1为完全不理会, 0为不影响权重, 权重越大越重要
    // Level 4的值 0 - 正无穷 不会出现完全不理会的情况


    // ----------------------------权重选择 Level1-----------------------------
    /// <summary>
    /// 选择地面单位权重
    /// </summary>
    public float SurfaceWeight { get; set; }

    /// <summary>
    /// 选择天空单位权重
    /// </summary>
    public float AirWeight { get; set; }

    /// <summary>
    /// 选择建筑权重
    /// </summary>
    public float BuildWeight { get; set; }

    /// <summary>
    /// 阵营选择权重
    /// 0全选
    /// 1己方
    /// 2敌方
    /// </summary>
    public int CampWeight { get; set; }

    /// <summary>
    /// 生命值选择范围类型
    /// 0:百分比
    /// 1:绝对值
    /// </summary>
    public int HpScopeType { get; set; }

    /// <summary>
    /// 生命值选择范围最大值
    /// -1则该值无效
    /// </summary>
    public float HpScopeMaxValue { get; set; }

    /// <summary>
    /// 生命值选择范围最小值
    /// -1则该值无效
    /// </summary>
    public float HpScopeMinValue { get; set; }

    /// <summary>
    /// 身上有Debuff时
    /// 0: 不作为条件
    /// 1: 身上必须有负面buff
    /// -1:身上必须没有负面buff
    /// </summary>
    public int DeBuffWeight { get; set; }

    /// <summary>
    /// 身上有正面buff时
    /// 0: 不作为条件
    /// 1: 身上必须有正面buff
    /// -1:身上必须没有正面buff
    /// </summary>
    public int BuffWeight { get; set; }


    // ----------------------------权重选择 Level2-----------------------------

    /// <summary>
    /// 人族权重
    /// </summary>
    public float HumanWeight { get; set; }

    /// <summary>
    /// 兽族权重
    /// </summary>
    public float OrcWeight { get; set; }

    /// <summary>
    /// 智械权重
    /// </summary>
    public float OmnicWeight { get; set; }
    ///// <summary>
    ///// 选择坦克权重
    ///// </summary>
    //public float TankWeight { get; set; }

    ///// <summary>
    ///// 选择轻型载具权重
    ///// </summary>
    //public float LVWeight { get; set; }

    ///// <summary>
    ///// 选择火炮权重
    ///// </summary>
    //public float CannonWeight { get; set; }

    ///// <summary>
    ///// 选择飞行器权重
    ///// </summary>
    //public float AirCraftWeight { get; set; }

    ///// <summary>
    ///// 选择步兵权重
    ///// </summary>
    //public float SoldierWeight { get; set; }


    // ----------------------------权重选择 Level3-----------------------------
    /// <summary>
    /// 选择隐形单位权重
    /// </summary>
    public float HideWeight { get; set; }

    /// <summary>
    /// 选择嘲讽权重(这个值应该很大, 除非有反嘲讽效果的单位)
    /// </summary>
    public float TauntWeight { get; set; }


    // ----------------------------权重选择 Level4-----------------------------


    /// <summary>
    /// 低生命权重
    /// </summary>
    public float HealthMinWeight { get; set; }

    /// <summary>
    /// 高生命权重
    /// </summary>
    public float HealthMaxWeight { get; set; }


    /// <summary>
    /// 近位置权重
    /// </summary>
    public float DistanceMinWeight { get; set; }

    /// <summary>
    /// 远位置权重
    /// </summary>
    public float DistanceMaxWeight { get; set; }

    ///// <summary>
    ///// 角度权重
    ///// </summary>
    //public float AngleWeight { get; set; }

    /// <summary>
    /// 初始化
    /// </summary>
    public SelectWeightData()
    {

    }

    ///// <summary>
    ///// 初始化
    ///// </summary>
    ///// <param name="armyaim">初始化数据</param>
    //public SelectWeightData(armyaim_cInfo armyaim)
    //{
    //    SetSelectWeightData(armyaim);
    //}


    ///// <summary>
    ///// 设置数据
    ///// </summary>
    ///// <param name="armyaim"></param>
    //public void SetSelectWeightData(armyaim_cInfo armyaim)
    //{
    //    SurfaceWeight = armyaim.Surface;
    //    AirWeight = armyaim.Air;
    //    BuildWeight = armyaim.Build;

    //    HumanWeight = armyaim.Human;
    //    OrcWeight = armyaim.Orc;
    //    OmnicWeight = armyaim.Omnic;

    //    HideWeight = armyaim.Hide;
    //    TauntWeight = armyaim.Taunt;

    //    HealthMinWeight = armyaim.HealthMin;
    //    HealthMaxWeight = armyaim.HealthMax;
    //    DistanceMinWeight = armyaim.RangeMin;
    //    DistanceMaxWeight = armyaim.RangeMax;

    //    HpScopeMaxValue = -1;
    //    HpScopeMinValue = -1;
    //}
}