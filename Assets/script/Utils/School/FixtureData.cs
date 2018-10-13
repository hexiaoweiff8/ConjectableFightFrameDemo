using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 固定障碍物
/// </summary>
public class FixtureData : PositionObject
{
    // TODO 添加分类可被击毁

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="allData">数据类</param>
    /// <param name="mapCell">地图单元</param>
    public FixtureData(AllData allData, FightUnitBase mapCell)
        : base(allData, mapCell)
    {
        if (AllData.MemberData == null)
        {
            AllData.MemberData = new MemberData();
        }
        Quality = 10000;
        SpeedDirection = Vector3.zero;
        
    }

    /// <summary>
    /// 该物体是否可移动
    /// 默认不能移动
    /// </summary>
    public override bool CouldMove
    {
        get { return false; }
    }
}