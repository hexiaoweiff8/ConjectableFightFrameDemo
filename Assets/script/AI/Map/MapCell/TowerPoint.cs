using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 防御塔
/// </summary>
public class TowerPoint : MapCellBase
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dataId"></param>
    /// <param name="drawLayer"></param>
    public TowerPoint(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.TowerPoint;
    }

}