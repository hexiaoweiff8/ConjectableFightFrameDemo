using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 单位特效数据
/// </summary>
public class EffectData
{
    /// <summary>
    /// 兵种ID
    /// </summary>
    public int ArmyID { get; set; }

    /// <summary>
    /// 枪口火焰
    /// </summary>
    public string MuzzleFlashEffect { get; set; }

    /// <summary>
    /// 子弹
    /// </summary>
    public string Bullet { get; set; }

    /// <summary>
    /// 链接特效
    /// </summary>
    public string ChainEffect { get; set; }

    /// <summary>
    /// 范围效果
    /// </summary>
    public string RangeEffect { get; set; }

    /// <summary>
    /// 轨迹特效
    /// </summary>
    public string TrajectoryEffect { get; set; }

    /// <summary>
    /// 被子弹击中特效
    /// </summary>
    public string GetHitByBulletEffect { get; set; }

    /// <summary>
    /// 收到炸弹攻击特效
    /// </summary>
    public string GetHitByBombEffect { get; set; }



    /// <summary>
    /// 枪口火焰时间
    /// </summary>
    public float MuzzleFlashEffectTime { get; set; }

    /// <summary>
    /// 链接特效时间
    /// </summary>
    public float ChainEffectTime { get; set; }

    /// <summary>
    /// 范围效果时间
    /// </summary>
    public float RangeEffectTime { get; set; }

    /// <summary>
    /// 被子弹击中特效时间
    /// </summary>
    public float GetHitByBulletEffectTime { get; set; }

    /// <summary>
    /// 收到炸弹攻击特效时间
    /// </summary>
    public float GetHitByBombEffectTime { get; set; }

    /// <summary>
    /// 收到炸弹攻击特效时间
    /// </summary>
    public int BulletType { get; set; }


    ///// <summary>
    ///// 初始化
    ///// </summary>
    ///// <param name="effect"></param>
    //public EffectData(effect_cInfo effect)
    //{
    //    SetData(effect);
    //}

    ///// <summary>
    ///// 设置数据
    ///// </summary>
    ///// <param name="effect">数据源</param>
    //public void SetData(effect_cInfo effect)
    //{
    //    ArmyID = effect.ArmyID;
    //    MuzzleFlashEffect = effect.MuzzleFlash;
    //    Bullet = effect.Bullet;
    //    ChainEffect = effect.Chain;
    //    RangeEffect = effect.Range;
    //    TrajectoryEffect = effect.Trajectory;
    //    GetHitByBulletEffect = effect.Gethit_Bullet;
    //    GetHitByBombEffect = effect.Gethit_Missile;

    //    MuzzleFlashEffectTime = effect.FlashPlayTime;
    //    ChainEffectTime = effect.ChainPlayTime;
    //    RangeEffectTime = effect.RangePlayTime;
    //    GetHitByBulletEffectTime = effect.Gethit_BulletPlayTime;
    //    GetHitByBombEffectTime = effect.Gethit_MissilePlayTime;

    //    BulletType = effect.BulletType;
    //}
}