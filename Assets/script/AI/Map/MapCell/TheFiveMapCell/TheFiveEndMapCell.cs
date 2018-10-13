using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 五行结束点
/// </summary>
public class TheFiveEndMapCell : TheFiveCellBase
{

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dataId"></param>
    /// <param name="drawLayer"></param>
    public TheFiveEndMapCell(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {
        this.Action = (mapCell) =>
        {
            // 获取下一节点列表

        };
    }



    /// <summary>
    /// 执行
    /// </summary>
    public void DoAction()
    {
        if (Action != null)
        {
            Action(this);
        }
    }

}