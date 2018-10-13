using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;


/// <summary>
/// 普通攻击类型管理
/// </summary>
public class GeneralAttackManager : SingleItem<GeneralAttackManager>
{


    public IGeneralAttack GetGeneralAttack()
    {
        IGeneralAttack result = null;



        return result;
    }



    /// <summary>
    /// 常规普通攻击
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="beAttackMember">被攻击者数据</param>
    /// <param name="effectKey">子弹预设key(或path)</param>
    /// <param name="releasePos">子弹飞行起点</param>
    /// <param name="targetObj">子弹目标单位</param>
    /// <param name="speed">子弹飞行速度</param>
    /// <param name="taType">子弹飞行轨迹</param>
    /// <param name="callback">攻击结束回调</param>
    /// <returns></returns>
    public IGeneralAttack GetNormalGeneralAttack(PositionObject attacker,
        PositionObject beAttackMember, 
        string effectKey, 
        Vector3 releasePos, 
        GameObject targetObj, 
        float speed, 
        TrajectoryAlgorithmType taType, 
        Action<GameObject> callback)
    {
        IGeneralAttack result = null;

        result = new NormalGeneralAttack(attacker, 
            beAttackMember, 
            effectKey, 
            releasePos, 
            targetObj, 
            speed, 
            taType, 
            callback);

        return result;
    }


    /// <summary>
    /// 点对对象范围攻击
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">子弹预设key(或path)</param>
    /// <param name="releasePos">子弹飞行起点</param>
    /// <param name="targetObj">子弹目标单位</param>
    /// <param name="scopeRaduis">范围半径</param>
    /// <param name="speed">子弹飞行速度</param>
    /// <param name="durTime">范围特效持续时间</param>
    /// <param name="taType">子弹飞行轨迹</param>
    /// <param name="callback">攻击结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPointToObjScopeGeneralAttack(PositionObject attacker,
        string[] effectKey,
        Vector3 releasePos,
        GameObject targetObj,
        float scopeRaduis,
        float speed,
        float durTime,
        TrajectoryAlgorithmType taType,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        result = new PointToObjScopeGeneralAttack(attacker, 
            effectKey,
            releasePos, 
            targetObj, 
            scopeRaduis, 
            speed, 
            durTime, 
            taType, 
            callback,
            callbackForEveryOne);

        return result;
    }


    /// <summary>
    /// 点对点范围伤害
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="releasePos">发射点</param>
    /// <param name="targetPos">目标点</param>
    /// <param name="scopeRaduis">范围伤害半径</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="taType">弹道类型</param>
    /// <param name="callback">完成回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPointToPositionScopeGeneralAttack(PositionObject attacker,
        Vector3 releasePos,
        Vector3 targetPos,
        float scopeRaduis,
        float speed,
        TrajectoryAlgorithmType taType,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {

        IGeneralAttack result = null;

        result = new PointToPositionScopeGeneralAttack(attacker,
            releasePos,
            targetPos,
            scopeRaduis,
            speed,
            taType,
            callback,
            callbackForEveryOne);

        return result;
    }


    /// <summary>
    /// 范围伤害(圆形)
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="scopeRaduis">范围半径</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPositionCircleScopeGeneralAttack(PositionObject attacker,
        string effectKey,
        Vector3 targetPos,
        float scopeRaduis,
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        result = new PositionScopeGeneralAttack(attacker, 
            effectKey, 
            targetPos, 
            scopeRaduis, 
            durTime,
            callback,
            callbackForEveryOne);

        return result;
    }


    /// <summary>
    /// 范围伤害(扇形)
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="scopeRaduis">范围半径</param>
    /// <param name="openAngle">扇形角度</param>
    /// <param name="rotate">旋转角度(0度为z轴正方向)</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPositionSectorScopeGeneralAttack(PositionObject attacker,
        string effectKey,
        Vector3 targetPos,
        float scopeRaduis,
        float openAngle,
        float rotate,
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        // 创建图形
        var graphics = new SectorGraphics(new Vector2(targetPos.x, targetPos.z), rotate, scopeRaduis, openAngle);
        result = new PositionScopeGeneralAttack(attacker,
            effectKey,
            targetPos,
            graphics,
            durTime,
            callback,
            callbackForEveryOne);

        return result;
    }

    /// <summary>
    /// 范围伤害(矩形)
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="rotate">旋转角度(0度为z轴正方向)</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPositionRectScopeGeneralAttack(PositionObject attacker,
        string effectKey,
        Vector3 targetPos,
        float width,
        float height,
        float rotate,
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        // 创建图形 加入偏移 效果为目标前方一条直线
        var graphics = new RectGraphics(new Vector2(targetPos.x, targetPos.z), width, height, rotate);
        result = new PositionScopeGeneralAttack(attacker,
            effectKey,
            targetPos,
            graphics,
            durTime,
            callback,
            callbackForEveryOne);

        return result;
    }


    /// <summary>
    /// 范围伤害(矩形带正方向偏移)
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="rotate">旋转角度(0度为z轴正方向)</param>
    /// <param name="offset">正方向偏移值</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPositionRectScopeGeneralAttack(PositionObject attacker,
        string effectKey,
        Vector3 targetPos,
        float width,
        float height,
        float rotate,
        float offset,
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        // 创建图形 加入偏移 效果为目标前方一条直线
        // 将图形像攻击者前方推移offset的距离
        var offsetV2 = new Vector2(attacker.X, attacker.Y);
        var targetPosV2 = offsetV2 + new Vector2(attacker.MapCellObj.transform.forward.x, attacker.MapCellObj.transform.forward.z) * offset;
        var graphics = new RectGraphics(targetPosV2, width, height, rotate);
        result = new PositionScopeGeneralAttack(attacker,
            effectKey,
            targetPos,
            graphics,
            durTime,
            callback,
            callbackForEveryOne);

        return result;
    }


    /// <summary>
    /// 范围伤害(任意图形)
    /// </summary>
    /// <param name="attacker">攻击者数据</param>
    /// <param name="effectKey">范围特效</param>
    /// <param name="targetPos">目标点位置</param>
    /// <param name="graphics">范围检测图形</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="callback">结束回调</param>
    /// <param name="callbackForEveryOne">每个受击单位的回调</param>
    /// <returns></returns>
    public IGeneralAttack GetPositionScopeGeneralAttack(PositionObject attacker,
        string effectKey,
        Vector3 targetPos,
        ICollisionGraphics graphics, 
        float durTime,
        Action callback,
        Action<GameObject> callbackForEveryOne = null)
    {
        IGeneralAttack result = null;

        result = new PositionScopeGeneralAttack(attacker,
            effectKey,
            targetPos,
            graphics,
            durTime,
            callback,
            callbackForEveryOne);

        return result;
    }


}


/// <summary>
/// 普通攻击接口
/// </summary>
public interface IGeneralAttack
{
    void Begin();
}