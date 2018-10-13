using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 范围攻击
/// </summary>
public class PositionScopeGeneralAttack : IGeneralAttack
{
    /// <summary>
    /// 攻击者数据
    /// </summary>
    private PositionObject attacker = null;

    /// <summary>
    /// 显示特效
    /// </summary>
    private string effectKey;

    /// <summary>
    /// 目标位置
    /// </summary>
    private Vector3 targetPos;

    /// <summary>
    /// 范围检测图形
    /// </summary>
    private ICollisionGraphics graphics;

    /// <summary>
    /// 持续时间
    /// </summary>
    private float durTime;

    /// <summary>
    /// 结束时回调
    /// </summary>
    private Action callback;

    /// <summary>
    /// 每个被击目标的回调
    /// </summary>
    private Action<GameObject> callbackForEveryOne;


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="scopeRaduis">范围半径</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    public PositionScopeGeneralAttack(PositionObject attacker, 
        string effectKey, 
        Vector3 targetPos, 
        float scopeRaduis, 
        float durTime, 
        Action callback = null,
        Action<GameObject> callbackForEveryOne = null)
    {
        if (attacker == null)
        {
            //throw new Exception("攻击者数据为null");
            return;
        }
        this.attacker = attacker;
        this.effectKey = effectKey;
        this.targetPos = targetPos;
        graphics = new CircleGraphics(new Vector2(targetPos.x, targetPos.z), scopeRaduis);
        this.durTime = durTime;
        this.callback = callback;
        this.callbackForEveryOne = callbackForEveryOne;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标点位置</param>
    /// <param name="graphics">范围检测图形</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    public PositionScopeGeneralAttack(PositionObject attacker, 
        string effectKey, 
        Vector3 targetPos, 
        ICollisionGraphics graphics, 
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        if (attacker == null)
        {
            //throw new Exception("攻击者数据为null");
            return;
        }
        this.attacker = attacker;
        this.effectKey = effectKey;
        this.targetPos = targetPos;
        this.graphics = graphics;
        this.durTime = durTime;
        this.callback = callback;
        this.callbackForEveryOne = callbackForEveryOne;
    }

    public void Begin()
    {
        // 如果攻击者已死则不进行攻击
        if (attacker == null)
        {
            return;
        }
        // 范围内选择单位
        //var memberList = ClusterManager.Single.CheckRange(graphics, attacker.AllData.MemberData.Camp, true);
        // 攻击者数据
        //var attackerDisplayOwner = FightUnitManager.Single.GetElementByPositionObject(attacker);
        // 所有单位扣除生命
        //foreach (var member in memberList)
        //{
        //    // 被攻击者数据
        //    var beAttackDisplayOwner = FightUnitManager.Single.GetElementByPositionObject(member);
        //    if (beAttackDisplayOwner == null || attackerDisplayOwner == null)
        //    {
        //        continue;
        //    }
        //    // 独计算是否命中, 是否伤害
        //    var isMiss = HurtResult.AdjustIsMiss(attackerDisplayOwner, beAttackDisplayOwner);
        //    if (!isMiss)
        //    {
        //        // 计算伤害值
        //        var hurt = HurtResult.GetHurt(attackerDisplayOwner, beAttackDisplayOwner);
        //        // 记录被击触发 记录扣血 伤害结算时结算
        //        //SkillManager.Single.SetTriggerData(new TriggerData()
        //        //{
        //        //    HealthChangeValue = hurt,
        //        //    ReceiveMember = attackerDisplayOwner,
        //        //    ReleaseMember = beAttackDisplayOwner,
        //        //    TypeLevel1 = TriggerLevel1.Fight,
        //        //    TypeLevel2 = TriggerLevel2.BeAttack,
        //        //    DemageType = DemageType.NormalAttackDemage,
        //        //    IsCrit = HurtResult.IsCrit
        //        //});
        //        // 回调每个受击单位
        //        if (callbackForEveryOne != null && member != null)
        //        {
        //            callbackForEveryOne(member.MapCellObj);
        //        }

        //        var effect = member.AllData.EffectData;

        //        var getHitEffect = effect.GetHitByBulletEffect;
        //        var getHitDurTime = 0f;
        //        // 分辨特效类型
        //        switch (effect.BulletType)
        //        {
        //            case 1:
        //                getHitEffect = effect.GetHitByBulletEffect;
        //                getHitDurTime = effect.GetHitByBulletEffectTime;
        //                break;

        //            case 2:
        //                getHitEffect = effect.GetHitByBombEffect;
        //                getHitDurTime = effect.GetHitByBombEffectTime;
        //                break;
        //        }

        //        if (getHitDurTime > 0)
        //        {
        //            // 对每个单位播受击特效
        //            // TODO 使用挂点
        //            EffectsFactory.Single.CreatePointEffect(getHitEffect,
        //                ParentManager.Single.GetParent(ParentManager.BallisticParent).transform,
        //                member.MapCellObj.transform.position,
        //                new Vector3(1, 1, 1),
        //                getHitDurTime,
        //                0,
        //                null,
        //                Utils.EffectLayer).Begin();
        //        }
        //    }
        //    else
        //    {

        //        // 闪避时事件
        //        //SkillManager.Single.SetTriggerData(new TriggerData()
        //        //{
        //        //    ReceiveMember = attackerDisplayOwner,
        //        //    ReleaseMember = beAttackDisplayOwner,
        //        //    TypeLevel1 = TriggerLevel1.Fight,
        //        //    TypeLevel2 = TriggerLevel2.Dodge
        //        //});
        //        //var beAttackVOBase = beAttackDisplayOwner.ClusterData.AllData.MemberData;
        //        //// 抛出miss事件
        //        //FightManager.Single.DoHealthChangeAction(beAttackDisplayOwner.GameObj, beAttackVOBase.TotalHp,
        //        //    beAttackVOBase.CurrentHP, 0f, FightManager.HurtType.Miss, beAttackVOBase.ObjID.ObjType);
        //    }
        //}

        // 播放特效
        EffectsFactory.Single.CreatePointEffect(effectKey,
            ParentManager.Single.GetParent(ParentManager.BallisticParent).transform, 
            targetPos, 
            new Vector3(1, 1, 1), 
            durTime, 
            0, 
            callback, 
            Utils.EffectLayer).Begin();
    }
}