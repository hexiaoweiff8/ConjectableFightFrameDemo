/// <summary>
/// 选择目标权重抽象类
/// TODO 改成接口, 不适用抽象类
/// </summary>
public interface ISelectWeightData
{
    // Level 1, 2, 3所有值都是从-1 - 正无穷, -1为完全不理会, 0为不影响权重, 权重越大越重要
    // Level 4的值 0 - 正无穷 不会出现完全不理会的情况


    // ----------------------------权重选择 Level1-----------------------------
    /// <summary>
    /// 选择地面单位权重
    /// </summary>
    float SurfaceWeight { get; set; }

    /// <summary>
    /// 选择天空单位权重
    /// </summary>
    float AirWeight { get; set; }

    /// <summary>
    /// 选择建筑权重
    /// </summary>
    float BuildWeight { get; set; }

    
    // ----------------------------权重选择 Level1-----------------------------

    /// <summary>
    /// 选择坦克权重
    /// </summary>
    float TankWeight { get; set; }

    /// <summary>
    /// 选择轻型载具权重
    /// </summary>
    float LVWeight { get; set; }

    /// <summary>
    /// 选择火炮权重
    /// </summary>
    float CannonWeight { get; set; }

    /// <summary>
    /// 选择飞行器权重
    /// </summary>
    float AirCraftWeight { get; set; }

    /// <summary>
    /// 选择步兵权重
    /// </summary>
    float SoldierWeight { get; set; }


    // ----------------------------权重选择 Level3-----------------------------
    /// <summary>
    /// 选择隐形单位权重
    /// </summary>
    float HideWeight { get; set; }

    /// <summary>
    /// 选择嘲讽权重(这个值应该很大, 除非有反嘲讽效果的单位)
    /// </summary>
    float TauntWeight { get; set; }


    // ----------------------------权重选择 Level4-----------------------------

    
    /// <summary>
    /// 低生命权重
    /// </summary>
    float HealthMinWeight { get; set; }

    /// <summary>
    /// 高生命权重
    /// </summary>
    float HealthMaxWeight { get; set; }


    /// <summary>
    /// 近位置权重
    /// </summary>
    float DistanceMinWeight { get; set; }

    /// <summary>
    /// 远位置权重
    /// </summary>
    float DistanceMaxWeight { get; set; }

    /// <summary>
    /// 角度权重
    /// </summary>
    float AngleWeight { get; set; }


}