using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 单点普通攻击
/// </summary>
public class NormalGeneralAttack : IGeneralAttack
{
    /// <summary>
    /// 攻击特效
    /// </summary>
    private EffectBehaviorAbstract effect;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="beAttackMember">被攻击者数据</param>
    /// <param name="effectKey">子弹预设key(或path)</param>
    /// <param name="releasePos">子弹飞行起点</param>
    /// <param name="targetObj">子弹目标单位</param>
    /// <param name="speed">子弹飞行速度</param>
    /// <param name="taType">子弹飞行轨迹</param>
    /// <param name="callback">攻击结束回调</param>
    public NormalGeneralAttack(PositionObject attacker, 
        PositionObject beAttackMember, 
        string effectKey, 
        Vector3 releasePos, 
        GameObject targetObj, 
        float speed, 
        TrajectoryAlgorithmType taType, 
        Action<GameObject> callback)
    {
        if (attacker == null || beAttackMember == null)
        {
            //throw new Exception("被攻击者或攻击者数据为空");
            return;
        }
        // 特效数据
        var effectData = beAttackMember.AllData.EffectData;
        Action demage = () =>
        {

            //var attackerDisplayOwner = FightUnitManager.Single.GetElementByPositionObject(attacker);
            //var beAttackerDisplayOwner = FightUnitManager.Single.GetElementByPositionObject(beAttackMember);

            //if (beAttackerDisplayOwner == null
            //    || attackerDisplayOwner == null
            //    || null == beAttackerDisplayOwner.ClusterData)
            //{
            //    return;
            //}
            // 判断是否命中
            //var isMiss = HurtResult.AdjustIsMiss(attackerDisplayOwner, beAttackerDisplayOwner);
            //if (!isMiss)
            //{
            //    // 计算伤害
            //    // TODO 伤害计算加入Buff与技能的计算
            //    //var hurt = HurtResult.GetHurt(attackerDisplayOwner, beAttackerDisplayOwner);
            //    //// 记录被击触发 记录扣血 伤害结算时结算
            //    //SkillManager.Single.SetTriggerData(new TriggerData()
            //    //{
            //    //    HealthChangeValue = hurt,
            //    //    ReceiveMember = attackerDisplayOwner,
            //    //    ReleaseMember = beAttackerDisplayOwner,
            //    //    TypeLevel1 = TriggerLevel1.Fight,
            //    //    TypeLevel2 = TriggerLevel2.BeAttack,
            //    //    DemageType = DemageType.NormalAttackDemage,
            //    //    IsCrit = HurtResult.IsCrit
            //    //});
            //    //// 命中时检测技能
            //    //SkillManager.Single.SetTriggerData(new TriggerData()
            //    //{
            //    //    // 将造成的伤害带回
            //    //    HealthChangeValue = hurt,
            //    //    ReceiveMember = beAttackerDisplayOwner,
            //    //    ReleaseMember = attackerDisplayOwner,
            //    //    TypeLevel1 = TriggerLevel1.Fight,
            //    //    TypeLevel2 = TriggerLevel2.Hit,
            //    //    DemageType = DemageType.NormalAttackDemage
            //    //});

            //    var getHitEffect = effectData.GetHitByBulletEffect;
            //    var getHitDurTime = 0f;
            //    // 分辨特效类型
            //    switch (effectData.BulletType)
            //    {
            //        case 1:
            //            getHitEffect = effectData.GetHitByBulletEffect;
            //            getHitDurTime = effectData.GetHitByBulletEffectTime;
            //            break;

            //        case 2:
            //            getHitEffect = effectData.GetHitByBombEffect;
            //            getHitDurTime = effectData.GetHitByBombEffectTime;
            //            break;
            //    }
            //    if (getHitDurTime > 0)
            //    {
            //        // 对每个单位播受击特效
            //        // 计算旋转角度
            //        var beAttackAngle = Utils.GetAngleWithZ(attacker.MapCellObj.gameObject.transform.forward) + 180;
            //        // TODO 使用挂点
            //        EffectsFactory.Single.CreatePointEffect(getHitEffect,
            //            ParentManager.Single.GetParent(ParentManager.BallisticParent).transform,
            //            beAttackMember.MapCellObj.transform.position,
            //            new Vector3(1, 1, 1),
            //            getHitDurTime,
            //            0,
            //            null,
            //            Utils.EffectLayer,
            //            new Vector2(0, beAttackAngle)).Begin();
            //    }
            //}
            //else
            //{
            //    //// 闪避时事件
            //    //SkillManager.Single.SetTriggerData(new TriggerData()
            //    //{
            //    //    ReceiveMember = attackerDisplayOwner,
            //    //    ReleaseMember = beAttackerDisplayOwner,
            //    //    TypeLevel1 = TriggerLevel1.Fight,
            //    //    TypeLevel2 = TriggerLevel2.Dodge
            //    //});
            //    //var beAttackVOBase = beAttackerDisplayOwner.ClusterData.AllData.MemberData;
            //    // 抛出miss事件
            //    //FightManager.Single.DoHealthChangeAction(beAttackerDisplayOwner.GameObj, beAttackVOBase.TotalHp,
            //    //    beAttackVOBase.CurrentHP, 0f, FightManager.HurtType.Miss, beAttackVOBase.ObjID.ObjType);

            //}
        };

        // 枪口火焰
        var muzzleEffect = effectData.MuzzleFlashEffect;
        var muzzleDurTime = effectData.MuzzleFlashEffectTime;
        if (muzzleDurTime > 0)
        {
            // 对每个单位播枪口火焰特效
            // 计算角度
            var muzzleAngle = Utils.GetAngleWithZ(attacker.MapCellObj.transform.forward);
            // TODO 使用挂点
            EffectsFactory.Single.CreatePointEffect(muzzleEffect,
                ParentManager.Single.GetParent(ParentManager.BallisticParent).transform,
                attacker.MapCellObj.transform.position,
                new Vector3(1, 1, 1),
                muzzleDurTime,
                0,
                null,
                Utils.EffectLayer,
                new Vector2(0, muzzleAngle)).Begin();
        }


        Action action = () =>
        {
            if (callback != null && beAttackMember!= null)
            {
                callback(beAttackMember.MapCellObj);
            }
        };

        
        effect = EffectsFactory.Single.CreatePointToObjEffect(effectKey,
            ParentManager.Single.GetParent(ParentManager.BallisticParent).transform,
            releasePos,
            targetObj,
            new Vector3(1, 1, 1),
            speed,
            taType,
            demage + action,
            Utils.EffectLayer);

    }

    public void Begin()
    {
        effect.Begin();
    }
}