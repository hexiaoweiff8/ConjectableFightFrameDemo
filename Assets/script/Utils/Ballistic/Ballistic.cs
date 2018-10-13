using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 弹道脚本
/// </summary>
public class Ballistic : MonoBehaviour , IBallistic
{
    // -------------------------公共属性--------------------------
    /// <summary>
    /// 当前方向
    /// </summary>
    public Vector3 Direction { get; set; }


    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    /// <summary>
    /// 已移动时间
    /// </summary>
    public float PassedTime { get; set; }

    /// <summary>
    /// 弹道物体半径
    /// </summary>
    public float Radius = 2;

    /// <summary>
    /// 当前速度
    /// </summary>
    public float Speed = 10;

    /// <summary>
    /// 是否有重力
    /// </summary>
    public bool HasGravity = true;

    /// <summary>
    /// 重力系数 默认9.8
    /// </summary>
    public float Gravity = 9.8f;

    ///// <summary>
    ///// 发射角度
    ///// </summary>
    //public float ShootTheta = Utils.NotCompleted - 1;

    /// <summary>
    /// 发射起始点
    /// </summary>
    public Vector3 StartPos;

    /// <summary>
    /// 当前重力方向
    /// </summary>
    public Vector3 GravityDirection = Vector3.down;
    
    /// <summary>
    /// 到达判断类
    /// </summary>
    public BallisticArriveTarget BallisticArriveTarget = null;

    /// <summary>
    /// 移动
    /// </summary>
    public Action<Ballistic, BallisticArriveTarget> Move;

    /// <summary>
    /// 击中回调
    /// </summary>
    public Action<Ballistic, BallisticArriveTarget> OnComplete;

    /// <summary>
    /// 销毁回调
    /// </summary>
    public Action<Ballistic, BallisticArriveTarget> OnKill;

    // ---------------------------私有属性------------------------------

    /// <summary>
    /// 是否结束
    /// </summary>
    private bool notComplete = true;

    /// <summary>
    /// 是否暂停
    /// </summary>
    private bool isPause = true;

    void Update()
    {
        // TODO 放入Looper中运行
        if (!isPause && BallisticArriveTarget != null && notComplete)
        {
            // 移动
            if (Move != null)
            {
                Move(this, BallisticArriveTarget);
            }
            // 到达目标
            if (BallisticArriveTarget.IsArrivedTarget(this))
            {
                notComplete = false;
                OnComplete(this, BallisticArriveTarget);
            }
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void Start()
    {
        isPause = false;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Kill()
    {
        // 回收至对象池
        if (OnKill != null)
        {
            OnKill(this, BallisticArriveTarget);
        }
    }
}



public interface IBallistic
{
    /// <summary>
    /// 暂停
    /// </summary>
    void Pause();

    /// <summary>
    /// 继续
    /// </summary>
    void Start();

    /// <summary>
    /// 销毁
    /// </summary>
    void Kill();
}