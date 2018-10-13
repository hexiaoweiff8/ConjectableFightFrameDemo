using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// FightUnit
/// </summary>
public class FightUnit : FightUnitBase
{
    /// <summary>
    /// 初始化战斗单位
    /// </summary>
    /// <param name="obj">游戏物体</param>
    /// <param name="dataId">数据Id</param>
    /// <param name="drawLayer">绘制层级</param>
    public FightUnit(GameObject obj, int dataId, int drawLayer)
        : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.FightUnit;
    }


    public override void Draw(Vector3 leftdown, int unitWidth)
    {
        // 检测缩放, 不进行位置控制
        CheckScale(unitWidth);
        //base.Draw(leftdown, unitWidth);
    }

    /// <summary>
    /// 获取五行属性
    /// </summary>
    /// <returns></returns>
    public override TheFiveProperties GetTheFiveProperties()
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// 战斗单位抽象类
/// </summary>
public abstract class FightUnitBase : MapCellBase, ITheFiveProperties
{
    protected FightUnitBase(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {

    }

    /// <summary>
    /// 获取五行属性
    /// </summary>
    /// <returns></returns>
    public abstract TheFiveProperties GetTheFiveProperties();
}

/// <summary>
/// 五行属性接口
/// </summary>
public interface ITheFiveProperties
{
    /// <summary>
    /// 获取五行属性
    /// </summary>
    /// <returns></returns>
    TheFiveProperties GetTheFiveProperties();
}