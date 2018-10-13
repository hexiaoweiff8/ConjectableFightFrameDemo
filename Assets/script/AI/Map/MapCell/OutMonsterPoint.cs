using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Util;

/// <summary>
/// 出兵点单位
/// </summary>
public class OutMonsterPoint : MapCellBase
{


    

    /// <summary>
    /// 刷怪列表
    /// </summary>
    private Queue<MonsterData> monsterQueue = new Queue<MonsterData>();

    /// <summary>
    /// 出兵计时器
    /// </summary>
    private Timer timer = null;

    /// <summary>
    /// 出兵时间间隔
    /// </summary>
    private float timeInterval = 1f;

    /// <summary>
    /// 创建单个完毕回调
    /// </summary>
    private Action onCreateOneDone = null;

    /// <summary>
    /// 全部创建完毕回调
    /// </summary>
    private Action onCreateAllDone = null;


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="dataId">数据ID</param>
    /// <param name="drawLayer">绘制层级</param>
    public OutMonsterPoint(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {
        // 读取数据
        // 生成创建事件链 
        // 多个事件链
        // 数据格式 : 类型Id, 数量, Id,
        MapCellType = UnitType.OutPoint;
    }

    /// <summary>
    /// 开始出兵
    /// </summary>
    public void Start()
    {
        // TODO 时间间隔创建
        // 创建单位有大量DrawCall上升问题, 待研究解决
        Stop();
        BeginCreateUnits();
    }

    /// <summary>
    /// 停止出兵
    /// </summary>
    public void Stop()
    {
        if (timer != null)
        {
            timer.Kill();
            timer = null;
        }
    }

    /// <summary>
    /// 暂停出兵
    /// </summary>
    public void Parse()
    {
        if (timer != null)
        {
            timer.Pause();
        }
    }

    public void GoOn()
    {
        if (timer != null)
        {
            timer.GoOn();
        }
    }

    /// <summary>
    /// 设置单个创建完毕事件
    /// </summary>
    /// <param name="action"></param>
    public void SetOnCreateOneDone([NotNull]Action action)
    {
        onCreateOneDone = action;
    }

    /// <summary>
    /// 设置全部创建完毕事件
    /// </summary>
    /// <param name="action"></param>
    public void SetOnCreateAllDone([NotNull]Action action)
    {
        onCreateAllDone = action;
    }


    

    /// <summary>
    /// 添加刷怪数据
    /// </summary>
    /// <param name="monsterData"></param>
    public void AddData([NotNull]MonsterData monsterData)
    {
        monsterQueue.Enqueue(monsterData);
    }

    /// <summary>
    /// 添加刷怪数据列表
    /// </summary>
    /// <param name="monsterDataList"></param>
    public void AddData([NotNull] List<MonsterData> monsterDataList)
    {
        foreach (var monsterData in monsterDataList)
        {
            monsterQueue.Enqueue(monsterData);
        }
    }


    /// <summary>
    /// 开始循环出兵
    /// </summary>
    private void BeginCreateUnits()
    {
        // 创建计时器, 并反复计时出兵
        Action action = null;
        action = () =>
        {
            // 从列表中将单位弹出
            if (monsterQueue.Count > 0)
            {
                var monsterData = monsterQueue.Dequeue();
                // 创建单位
                CreateUnit(monsterData);
                // 计时运行
                timer = new Timer(monsterData.Interval).OnCompleteCallback(action).Start();

            }
            else
            {
                // 出兵结束
                Debug.Log("出兵结束");
                Stop();
            }
        };
        action();
    }

    /// <summary>
    ///  创建单位
    /// </summary>
    private void CreateUnit([NotNull] MonsterData monsterData)
    {
        //// 获取目标位置
        //var targetMapCellList = FightManager.Single.MapBase.GetMapCellList(MapManager.MapNpcLayer,
        //    MapManager.InMonsterPointId);
        //DataItem dataItem = new DataItem();
        //if (targetMapCellList != null && targetMapCellList.Count > 0 && GameObj != null)
        //{
        //    var targetMapCell = targetMapCellList[0] as InMonsterPoint;
        //    if (targetMapCell != null)
        //    {
        //        for (var i = 0; i < monsterData.Count; i++)
        //        {
        //            dataItem.SetInt(FightUnitManager.FightItemStartX, X);
        //            dataItem.SetInt(FightUnitManager.FightItemStartY, Y);
        //            dataItem.SetInt(FightUnitManager.FightItemTargetX, targetMapCell.X);
        //            dataItem.SetInt(FightUnitManager.FightItemTargetY, targetMapCell.Y);

        //            // 创建怪单位
        //            var displayOwner = FightUnitManager.Single.LoadMember(UnitType.FightUnit, monsterData.MonsterId,
        //                dataItem);
        //            // 开始移动
        //            displayOwner.ClusterData.StartMove();
        //            var clusterData = displayOwner.ClusterData as ClusterData;
        //            if (clusterData != null)
        //            {
        //                clusterData.Complete = o =>
        //                {
        //                    targetMapCell.MonsterArrival(displayOwner);
        //                };
        //            }

        //            displayOwner.ClusterData.MapCell.GameObj.transform.position = GameObj.transform.position;
        //            dataItem.Clear();
        //        }
        //    }
        //}
        //else
        //{
        //    timer.Kill();
        //    Debug.LogError("没有目标位置");
        //}
    }
}


/// <summary>
/// 刷怪单位信息
/// </summary>
public class MonsterData
{
    // 波次
    public int TurnId = 0;
    // 类型
    public int MonsterId = 0;
    // 数量
    public int Count = 0;
    // 与下一波的时间间隔
    public float Interval = 0f;
}