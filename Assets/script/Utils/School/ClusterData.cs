using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 集群行为
/// </summary>
public class ClusterData: PositionObject
{

    // ---------------------------设置属性-----------------------------
    /// <summary>
    /// 外部注入VO
    /// </summary>
    public void SetDataValue(MemberData data)
    {
        AllData.MemberData = data;
    }

    /// <summary>
    /// 旋转速度
    /// </summary>
    public float RotateSpeed {
        get { return rotateSpeed; }
        set { rotateSpeed = value < 0 ? 1 : value; }
    }

    /// <summary>
    /// 转向权重
    /// 值越大, 转向越快
    /// </summary>
    public float RotateWeight {
        get { return rotateWeight; }
        set { rotateWeight = value < 0 ? 1 : value; }
    }

    /// <summary>
    /// 目标位置
    /// </summary>
    public Vector3 TargetPos {
        get { return targetPos; }
        set {
            // TODO 切换当前状态
            targetPos = value;
        }
    }

    /// <summary>
    /// 单位当前状态
    /// </summary>
    public SchoolItemState State = SchoolItemState.Unstart;

    /// <summary>
    /// 开始移动时调用
    /// </summary>
    public Action<GameObject> Moveing { get; set; }

    /// <summary>
    /// 拥挤等待时调用
    /// </summary>
    public Action<GameObject> Wait { get; set; }

    /// <summary>
    /// 移动到终点后调用
    /// 不是所有对象都会掉用该回调, 只有到达终点的会调用
    /// </summary>
    public Action<GameObject> Complete { get; set; }


    // -------------------------私有属性-------------------------


    /// <summary>
    /// 当前单位的目标点
    /// </summary>
    private Vector3 targetPos;

    /// <summary>
    /// 组编号
    /// </summary>
    private int groupId;

    /// <summary>
    /// 旋转速度
    /// </summary>
    private float rotateSpeed = 1;

    /// <summary>
    /// 转向权重
    /// 值越大, 转向越快
    /// </summary>
    private float rotateWeight = 1;

    /// <summary>
    /// 是否可被阻挡
    /// 如果为true则遇到其他单位则将其挤开
    /// 如果为false则减速等待
    /// </summary>
    private bool couldObstruct = true;

    /// <summary>
    /// 目标路径栈
    /// </summary>
    private Stack<Vector3> targetQueue = new Stack<Vector3>();



    // ------------------------------公共方法---------------------------------

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="allData">初始化数据</param>
    /// <param name="mapCell">地图单元</param>
    public ClusterData([NotNull]AllData allData, FightUnitBase mapCell)
        : base(allData, mapCell)
    {
        AllData = allData;
    }

    /// <summary>
    /// 压入新位置, 并将该位置设置为当前目标点
    /// </summary>
    /// <param name="target">新目标点</param>
    public void PushTarget(Vector3 target)
    {
        targetPos = target;
        targetQueue.Push(target);
        // 设置新目标点后重置状态
        State = SchoolItemState.Unstart;
    }

    /// <summary>
    /// 压入新位置, 以列表的形式
    /// </summary>
    /// <param name="targetList">目标列表</param>
    public void PushTargetList(List<Vector3> targetList)
    {
        if (targetList != null)
        {
            foreach (var target in targetList)
            {
                PushTarget(target);
            }
        }
    }

    /// <summary>
    /// 弹出栈顶目标位置
    /// </summary>
    /// <returns>下一目标点</returns>
    public bool PopTarget()
    {
        var result = false;
        if (targetQueue.Count > 0)
        {
            targetPos = targetQueue.Pop();
            result = true;
        }
        plusQuality += quality * 0.5f;
        if (!result)
        {
            plusQuality = 0;
        }
        return result;
    }

    /// <summary>
    /// 清空目标点栈
    /// </summary>
    public void ClearTarget()
    {
        targetQueue.Clear();
        plusQuality = 0;
    }

#if UNITY_EDITOR
    public void Do()
    {
        // 显示所有路径点
        foreach (var point in targetQueue)
        {
            Utils.DrawRect(point, ClusterManager.Single.UnitWidth, ClusterManager.Single.UnitWidth, 0, Color.magenta);
        }
    }
#endif

    public void Destory() {
        // 销毁时从列表中消除当前队员
        //Group.MemberList.Remove(this);
    }
}


/// <summary>
/// 集群单位状态
/// </summary>
public enum SchoolItemState
{
    // 未开始状态
    Unstart,
    // 移动中状态
    Moving,
    // 等待中状态
    Waiting,
    // 结束状态
    Complete,
}