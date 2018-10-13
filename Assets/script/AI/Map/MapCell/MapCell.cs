using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 地图单元
/// </summary>
public class MapCell : MapCellBase
{
    /// <summary>
    /// 单位Obj 从该层对应的对象池获取
    /// </summary>
    public override GameObject GameObj
    {
        get
        {
            // 判断当前状态, 如果当前状态为不显示进行报错
            if (!isShow && !IsInScreen)
            {
                Debug.LogError("不在显示范围内的情况下被显示.");
            }
            return base.GameObj;
        }
        set { base.GameObj = value; }
    }

    /// <summary>
    /// 是否在屏幕中状态
    /// </summary>
    protected bool IsInScreen = false;




    /// <summary>
    /// 初始化地图单元
    /// </summary>
    /// <param name="obj">游戏物体</param>
    /// <param name="dataId">数据Id</param>
    /// <param name="drawLayer">绘制层级</param>
    public MapCell(GameObject obj, int dataId, int drawLayer)
        : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.MapCell;
    }

    /// <summary>
    /// 进入屏幕操作
    /// </summary>
    public void EnterScreen()
    {
        // 设置标记
        if (!IsInScreen)
        {
            Utils.DrawGraphics(GetGraphics(), Color.white);
            isShow = true;
            IsInScreen = true;
            if (base.GameObj == null)
            {
                // 加载对象
                //base.GameObj = UnitFictory.Single.GetMapBaseObjFromPool(DataId);
            }
        }
    }

    /// <summary>
    /// 出屏幕操作
    /// </summary>
    public void OutScreen()
    {
        // 设置标记
        if (IsInScreen)
        {
            Utils.DrawGraphics(GetGraphics(), Color.red);
            isShow = false;
            IsInScreen = false;

            // 回收对象
            if (base.GameObj != null)
            {
                //UnitFictory.Single.CycleBackMapBaseObj(base.GameObj);
                base.GameObj = null;
            }
            else
            {
                Debug.LogError("单位在回收前已被销毁");
            }
            historyUnitWidth = -1;
            historyXForDraw = -1;
            historyYForDraw = -1;
        }
    }
}