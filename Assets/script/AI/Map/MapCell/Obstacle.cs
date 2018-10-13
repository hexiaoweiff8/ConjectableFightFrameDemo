using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 障碍物
/// </summary>
public class Obstacle : FightUnitBase
{
    /// <summary>
    /// 初始化障碍物
    /// </summary>
    /// <param name="obj">游戏物体</param>
    /// <param name="dataId">数据Id</param>
    /// <param name="drawLayer">绘制层级</param>
    public Obstacle(GameObject obj, int dataId, int drawLayer)
        : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.Obstacle;
    }

    public override TheFiveProperties GetTheFiveProperties()
    {
        throw new NotImplementedException();
    }
}

