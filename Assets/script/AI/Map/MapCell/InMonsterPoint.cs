using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 入兵点单位
/// </summary>
public class InMonsterPoint : MapCellBase
{

    /// <summary>
    /// 怪物到达事件
    /// </summary>
    private Action onMonsterArrival = null;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="dataId">数据ID</param>
    /// <param name="drawLayer">绘制层级</param>
    public InMonsterPoint(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.InPoint;
    }

    /// <summary>
    /// 设置当怪到达事件
    /// </summary>
    /// <param name="action">事件</param>
    public void SetOnMonsterArrival([NotNull]Action action)
    {
        onMonsterArrival = action;
    }


    ///// <summary>
    ///// 回收单位
    ///// </summary>
    //public void MonsterArrival([NotNull]DisplayOwner displayOwner)
    //{
    //    // 怪物到达事件
    //    if (onMonsterArrival != null)
    //    {
    //        onMonsterArrival();
    //    }
    //    // 销毁目标
    //    FightUnitManager.Single.Destory(displayOwner);
    //}
}