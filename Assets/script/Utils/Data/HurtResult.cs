using System;
using UnityEngine;
using System.Collections;

public class HurtResult
{

    /// <summary>
    /// 是否暴击
    /// </summary>
    public static bool IsCrit { get; private set; }


    /// <summary>
    /// 普通战斗伤害结算
    /// </summary>
    /// <param name="active">伤害发起的主动方</param>
    /// <param name="target">伤害被动方</param>
    /// <returns></returns>
    public static float GetHurt(DisplayOwner active, DisplayOwner target)
    {
        var zhanqianJueduizhi = active.ClusterData.AllData.MemberData.Attack1;
        var zhandouJueduizhiAdd = 0.0f;
        var zhanqianBaifenbiAdd = 0.0f;
        var zhandouBaifenbiAdd = 0.0f;

        // 伤害能力
        var huoli = (zhanqianJueduizhi + zhandouJueduizhiAdd)*(1 + zhanqianBaifenbiAdd + zhandouBaifenbiAdd);
        // 暴击
        var baojixishu = AdjustIsBaoji(active, target);
        var hurtAdd = 0.0f;
        var antiHurtAdd = 0.0f;
        // 减伤
        var jianshanglv = AdjustJianshang(active, target);
        // 克制关系
        var kezhixishu = AdjustKezhi(active, target);

        var shanghaijiacheng = 0.0f;
        var mianyijiacheng = 0.0f;
        var jinengjiacheng = 0.0f;

        /**
         * 最终伤害=进攻方火力*（1-减伤率）*（IF（本次暴击，暴击伤害系数，1））*
         * （IF（满足克制关系，1+克制伤害加成，1））*
         * （1+Max（-40%，（进攻方伤害加成-防守方免伤加成）））+
         * 攻守双方技能绝对值加成和
         **/
        if (baojixishu > 1)
        {
            IsCrit = true;
        }
        else
        {
            IsCrit = false;
        }

        //// 计算增伤
        //shanghaijiacheng = GetDemageUpper(active, target);

        //// 计算减伤
        //mianyijiacheng = GetDemageLower(active, target);


        var hurt = huoli*(1 - jianshanglv)*baojixishu*kezhixishu*
                   (1 + Mathf.Max(-0.8f,(shanghaijiacheng - mianyijiacheng))) + jinengjiacheng;
        return hurt;
    }

    ///// <summary>
    ///// 计算技能伤害/治疗
    ///// </summary>
    ///// <param name="member"></param>
    ///// <param name="target">技能目标</param>
    ///// <param name="type">伤害或治疗</param>
    ///// <param name="unitType">伤害/治疗值类型</param>
    ///// <param name="calculationType">计算类型</param>
    ///// <param name="value">具体值, 必须大于等于0</param>
    ///// <returns>伤害/治疗具体量</returns>
    //public static float GetHurtForSkill(DisplayOwner member, DisplayOwner target, DemageOrCure type, HealthChangeType unitType, HealthChangeCalculationType calculationType, float value)
    //{
    //    if (member == null
    //        || member.ClusterData == null
    //        || target == null
    //        || target.ClusterData == null
    //        || target.ClusterData.AllData.MemberData == null)
    //    {
    //        throw new Exception("目标对象为空.");
    //    }
    //    if (value < 0)
    //    {
    //        throw new Exception("伤害/治疗值不能为负数.");
    //    }
    //    var result = 0f;
    //    // 区分伤害类型
    //    switch (calculationType)
    //    {
    //        case HealthChangeCalculationType.Fix:
    //        {
    //            var zhanqianJueduizhi = member.ClusterData.AllData.MemberData.Attack1;
    //            var zhandouJueduizhiAdd = 0.0f;
    //            var zhanqianBaifenbiAdd = 0.0f;
    //            var zhandouBaifenbiAdd = 0.0f;

    //            // 伤害能力
    //            var huoli = (zhanqianJueduizhi + zhandouJueduizhiAdd) * (1 + zhanqianBaifenbiAdd + zhandouBaifenbiAdd);
    //            // 减伤
    //            var jianshanglv = AdjustJianshang(member, target);
    //            // 克制关系
    //            var kezhixishu = AdjustKezhi(member, target);
    //            //if (unitType == HealthChangeType.Percentage)
    //            //{
    //            //    result = target.ClusterData.AllData.MemberData.TotalHp*value;
    //            //}

    //            var shanghaijiacheng = 0.0f;
    //            var mianyijiacheng = 0.0f;
    //            var jinengjiacheng = 0.0f;

    //            // 计算增伤
    //            shanghaijiacheng = GetDemageUpper(member, target);

    //            // 计算减伤
    //            mianyijiacheng = GetDemageLower(member, target);

    //            result = huoli * (1 - jianshanglv) * kezhixishu *
    //                       (1 + Mathf.Max(-0.8f, (shanghaijiacheng - mianyijiacheng))) + jinengjiacheng;

    //        }
    //            break;
    //        case HealthChangeCalculationType.Calculation:

    //            break;
    //    }
        
    //    return result;
    //}

    /// <summary>
    /// 判断是否命中
    /// </summary>
    /// <param name="active">攻击方</param>
    /// <param name="target">被攻击方</param>
    /// <returns>是否命中</returns>
    public static bool AdjustIsMiss(DisplayOwner active, DisplayOwner target)
    {
        /**
         * 计算攻击方【战斗属性-命中率】
            命中率=战前加成和
            战前属性的计算在文档的开始部分
            3.5.2计算进攻方【战斗属性-命中率加成】
            命中率加成=战斗中百分比加成和
            PS1:战斗中加成目前已知来源：被附加的BUFF，友方的光环
            PS2:战斗中的加成因为BUFF的属性，所以这里的加成和是计算增益减益和
            计算防守方【战斗属性-闪避率】
            闪避率=战前加成和
            计算防守方【战斗属性-闪避率加成】
            闪避率加成=战斗中百分比加成和
            计算命中率差值
            命中率差值=Max（30%，进攻方命中率*（1+命中率加成）-防守方闪避率*（1+闪避率加成））
            判定攻击是否命中
            在1-1000中取随机整数a
	        如果a<=命中率差值*1000,判定为命中
	        如果a>命中率差值*1000,判定为闪避
        **/
        if (null == target || null == target.ClusterData || target.ClusterData.AllData.MemberData.CurrentHP <= 0 ||
            null == active || null == active.ClusterData)
        {
            return true;
        }
        var gongjiMingzhong = active.ClusterData.AllData.MemberData.Hit;
        var gongjiMingzhongAdd = 0.0f;

        var fangshouShanbi = target.ClusterData.AllData.MemberData.Dodge;
        var fangshouShanbiAdd = 0.0f;

        var mingzhongchazhi = Mathf.Max(0.3f,
            gongjiMingzhong*(1 + gongjiMingzhongAdd) - fangshouShanbi*(1 + fangshouShanbiAdd));
        // TODO 包装随机
        //var ran = new QKRandom((int)DateTime.Now.Ticks);
        var value = RandomPacker.Single.GetRangeI(0, 1000);
        bool isMiss = value >= mingzhongchazhi*1000;
        return isMiss;
    }

    /// <summary>
    /// 判断是否暴击
    /// </summary>
    /// <param name="active">攻击方</param>
    /// <param name="target">被攻击方</param>
    /// <returns></returns>
    public static float AdjustIsBaoji(DisplayOwner active, DisplayOwner target)
    {
        var gongjiBaoji = active.ClusterData.AllData.MemberData.Crit;
        var gongjiBaojiAdd = 0.0f;

        var fangshouFangbao = target.ClusterData.AllData.MemberData.AntiCrit;
        var fangshouFangbaoAdd = 0.0f;

        var chazhi = Mathf.Max(0, gongjiBaoji*(1 + gongjiBaojiAdd) - fangshouFangbao*(1 + fangshouFangbaoAdd));

        //暴击伤害系数=战前百分比加成和+战斗中百分比加成和
        var beforeFight = 0f;
        // 暴击伤害值
        var inFight = RandomPacker.Single.GetRangeI(0, 1000) <= active.ClusterData.AllData.MemberData.FixCrit*1000
            ? active.ClusterData.AllData.MemberData.FixCritDemage
            : active.ClusterData.AllData.MemberData.CritDamage;


        return RandomPacker.Single.GetRangeI(0, 1000) <= chazhi * 1000 ? (beforeFight + inFight) : 1;
    }

    /// <summary>
    /// 判断是否穿甲
    /// </summary>
    /// <param name="active">攻击方</param>
    /// <param name="target">被攻击方</param>
    /// <returns></returns>
    public static float AdjustJianshang(DisplayOwner active, DisplayOwner target)
    {
        /**
         *  穿甲=（战前值+战斗中绝对值加成和）*（1+战前加成和+战斗中百分比加成和）
            PS1:战斗中加成目前已知来源：被附加的BUFF，友方的光环
            PS2:战斗中的加成因为BUFF的属性，所以这里的加成和是计算增益减益和
            防御=（战前值+战斗中绝对值加成和）*（1+战前加成和+战斗中百分比加成和）
            装甲=（战前值+战斗中绝对值加成和）*（1+战前加成和+战斗中百分比加成和）
         * */
        //战斗中绝对值加成和
        var chuanjiaJueduizhiAdd = 0.0f;
        var chuanjiaBaifenbiAdd = 0.0f;
        var chuanjiaZhanqianAdd = 0.0f;

        var chuanjiashuxing = active.ClusterData.AllData.MemberData.AntiArmor;
        var chuanjia = (chuanjiashuxing + chuanjiaJueduizhiAdd)*(1 + chuanjiaZhanqianAdd + chuanjiaBaifenbiAdd);

        var fangyuJueduizhiAdd = 0.0f;
        var fangyuBaifenbiAdd = 0.0f;
        var fangyuZhanqianAdd = 0.0f;

        var fangyuShuxing = target.ClusterData.AllData.MemberData.Defence;
        var fangyu = (fangyuShuxing + chuanjiaJueduizhiAdd)*(1 + fangyuZhanqianAdd + fangyuBaifenbiAdd);

        var zhuangjiaJueduizhiADD = 0.0f;
        var zhuangjiaBaifenbiiADD = 0.0f;
        var zhuangjiaZhanqianADD = 0.0f;

        var zhuangjiaShuxing = target.ClusterData.AllData.MemberData.Armor;
        var zhuangjia = (zhuangjiaShuxing + zhuangjiaJueduizhiADD)*(1 + zhuangjiaZhanqianADD + zhuangjiaBaifenbiiADD);
        //判定攻击是否无视防御或装甲,攻击方有几率无视防御或装甲的技能，在1 - 1000中取随机整数a
        var wushifangyu = 0.0f;
        var wushizhuangjia = 0.0f;

        // TODO 包装随机
        //var ran = new QKRandom((int)DateTime.Now.Ticks);
        var a = RandomPacker.Single.GetRangeI(0, 1000);
        var a1 = RandomPacker.Single.GetRangeI(0, 1000);

        fangyu = a <= wushifangyu*1000 ? 0 : fangyu;
        zhuangjia = a1 <= wushizhuangjia*1000 ? 0 : zhuangjia;

        return (fangyu + Mathf.Max(0, zhuangjia - chuanjia))/
                          (fangyu + Mathf.Max(0, zhuangjia - chuanjia) + 2000);
    }

    /// <summary>
    /// 判断攻守防是否满足克制关系
    /// </summary>
    /// <param name="active"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float AdjustKezhi(DisplayOwner active, DisplayOwner target)
    {
        // TODO 本地缓存数据
        //var config = SData_kezhi_c.Single.GetDataOfID(active.ClusterData.AllData.MemberData.ArmyType);
        //if (config.KezhiType == target.ClusterData.AllData.MemberData.ArmyType)
        //{
        //    return 1 + config.KezhiAdd;
        //}
        return 1.0f;
    }


    ///// <summary>
    ///// 获取增伤
    ///// </summary>
    ///// <param name="active"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //private static float GetDemageUpper(DisplayOwner active, DisplayOwner target)
    //{
    //    var result = 0f;
    //    // 计算伤害加成与概率
    //    if (active.ClusterData.AllData.SkillInfoList != null && active.ClusterData.AllData.SkillInfoList.Count > 0)
    //    {
    //        foreach (var skill in active.ClusterData.AllData.SkillInfoList)
    //        {
    //            if (skill.IsActive || skill.DemageChangeType != 0)
    //            {
    //                continue;
    //            }
    //            result += GetDemageChange(skill, target);
    //        }
    //    }
    //    if (active.ClusterData.AllData.BuffInfoList != null && active.ClusterData.AllData.BuffInfoList.Count > 0)
    //    {
    //        foreach (var buff in active.ClusterData.AllData.BuffInfoList)
    //        {
    //            result += GetDemageChange(buff, target);
    //        }
    //    }

    //    return result;
    //}

    ///// <summary>
    ///// 获取减伤
    ///// </summary>
    ///// <param name="active"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //private static float GetDemageLower(DisplayOwner active, DisplayOwner target)
    //{
    //    var result = 0f;
    //    // 计算减免加成与概率
    //    if (target.ClusterData.AllData.SkillInfoList != null && target.ClusterData.AllData.SkillInfoList.Count > 0)
    //    {
    //        foreach (var skill in target.ClusterData.AllData.SkillInfoList)
    //        {
    //            if (skill.IsActive)
    //            {
    //                continue;
    //            }
    //            result += GetDemageChange(skill, target);
    //        }
    //    }
    //    if (target.ClusterData.AllData.BuffInfoList != null && target.ClusterData.AllData.BuffInfoList.Count > 0)
    //    {
    //        foreach (var buff in target.ClusterData.AllData.BuffInfoList)
    //        {
    //            result += GetDemageChange(buff, target);
    //        }
    //    }
    //    return result;
    //}


    ///// <summary>
    ///// 伤害增强/减免计算
    ///// </summary>
    ///// <param name="skill">被计算技能</param>
    ///// <param name="target">被计算目标</param>
    ///// <returns></returns>
    //private static float GetDemageChange(SkillBase skill, DisplayOwner target)
    //{
    //    var result = 0f;
    //    if (skill.DemageChangeProbability > 0 && skill.DemageChange > 0)
    //    {
    //        if (skill.DemageChangeProbability >= 1 ||
    //            RandomPacker.Single.GetRangeI(0, 100) <= skill.DemageChangeProbability * 100)
    //        {
    //            if (Check(skill, target.ClusterData.AllData.MemberData))
    //            {
    //                result += skill.DemageChange;
    //            }
    //        }
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// 检查是否符合技能的伤害附加/减免条件
    ///// </summary>
    ///// <param name="skill">被检测技能</param>
    ///// <param name="data">被检测单位</param>
    ///// <returns>是否符合</returns>
    //private static bool Check(SkillBase skill, MemberData data)
    //{
    //    var result = false;
    //    // 计算伤害附加
    //    switch (skill.DemageChangeTargetType)
    //    {
    //        case DemageAdditionOrReductionTargetType.All:
    //            result = true;
    //            break;
    //        case DemageAdditionOrReductionTargetType.Air:
    //            if (data.GeneralType == Utils.GeneralTypeAir)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.Surface:
    //            if (data.GeneralType == Utils.GeneralTypeSurface)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.Building:
    //            if (data.GeneralType == Utils.GeneralTypeBuilding)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.Hide:
    //            if (data.IsHide)
    //            {
    //                result = true;
    //            }
    //            break;
    //        //case DemageAdditionOrReductionTargetType.RaceHuman:
    //        //    if (data.ArmyType == Utils.HumanArmyType)
    //        //    {
    //        //        result = true;
    //        //    }
    //        //    break;
    //        //case DemageAdditionOrReductionTargetType.RaceOrc:
    //        //    if (data.ArmyType == Utils.OrcArmyType)
    //        //    {
    //        //        result = true;
    //        //    }
    //        //    break;
    //        //case DemageAdditionOrReductionTargetType.RaceMechanics:
    //        //    if (data.ArmyType == Utils.MechanicArmyType)
    //        //    {
    //        //        result = true;
    //        //    }
    //            //break;
    //        case DemageAdditionOrReductionTargetType.Mechanics:
    //            if (data.IsMechanic)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.Melee:
    //            if (data.IsMelee)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.NotMechanics:
    //            if (!data.IsMechanic)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.NotMelee:
    //            if (!data.IsMelee)
    //            {
    //                result = true;
    //            }
    //            break;
    //        case DemageAdditionOrReductionTargetType.Summoned:
    //            if (data.IsSummon)
    //            {
    //                result = true;
    //            }
    //            break;
    //    }
    //    return result;
    //}
}
