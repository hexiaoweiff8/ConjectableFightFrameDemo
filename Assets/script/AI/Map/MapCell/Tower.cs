using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 塔基类
/// </summary>
public class Tower : FightUnitBase
{


    /// <summary>
    /// 塔分布数据数组
    /// </summary>
    private int[,] towerCellDataArray = null;

    /// <summary>
    /// 节点传导方向
    /// </summary>
    private MultiLinkNode<TheFiveCellBase>[,] transDirArray = null;

    /// <summary>
    /// 塔单元数组
    /// </summary>
    private TheFiveCellBase[,] towerCellArray = null;

    /// <summary>
    /// 范围碰撞图形
    /// </summary>
    private ICollisionGraphics graphics = null;

    /// <summary>
    /// 是否未设置碰撞图形
    /// </summary>
    private bool isNotSetGraphics = true;

    /// <summary>
    /// 塔cell高度
    /// </summary>
    private int height = 0;

    /// <summary>
    /// 塔cell宽度
    /// </summary>
    private int wight = 0;


    /// <summary>
    /// 塔的相对位置左下角
    /// </summary>
    private Vector2 towerLeftDown {
        get
        {
            var halfTowerUnitWidth = towerUnitWidth*0.5f;
            // 获取相对左下角位置
            var relativePos = new Vector2(-wight * halfTowerUnitWidth + halfTowerUnitWidth, -height * halfTowerUnitWidth + halfTowerUnitWidth);
            // 合算两个位置
            return GameObj == null ? Vector2.zero : new Vector2(GameObj.transform.position.x, GameObj.transform.position.y) + relativePos;
        }
    }

    /// <summary>
    /// 塔的单位宽度
    /// </summary>
    private int towerUnitWidth
    {
        get
        {
            return Math.Min(MapDrawer.Single.UnitWidth / height, MapDrawer.Single.UnitWidth / wight);
        }
    }





    /// <summary>
    /// 初始化
    /// 该cell
    /// </summary>
    /// <param name="obj">游戏对象</param>
    /// <param name="dataId">cell的DataId</param>
    /// <param name="drawLayer">所在层</param>
    public Tower(GameObject obj, int dataId, int drawLayer) : base(obj, dataId, drawLayer)
    {
        MapCellType = UnitType.Tower;
        // 触发事件
        Action = @base =>
        {
            // 获取当前单位的所有StartCell, 并调用他们的Action
            if (towerCellArray != null)
            {
                foreach (var cell in towerCellArray)
                {
                    var startCell = cell as TheFiveStartMapCell;
                    if (startCell != null && startCell.Action != null)
                    {
                        startCell.Action(null);
                    }
                }
            }
            else
            {
                Debug.LogError("塔cell为空");
            }
        };

        StepAction = mapCellBase =>
        {
            Debug.Log("StepAction");
            // 检测范围内单位
            // 如果范围内有单位则继续触发
            var targetList = ClusterManager.Single.GetPositionObjectListByGraphics(graphics);
            if (targetList != null && targetList.Count > 0)
            {
                // 选定目标
            }
        };

    }

    /// <summary>
    /// 设置塔数据
    /// </summary>
    /// <param name="towerData">他单元数据</param>
    public void SetTowerData([NotNull]int[,] towerData)
    {
        height = towerData.GetLength(0);
        wight = towerData.GetLength(1);
        // 初始化本地数据
        towerCellDataArray = towerData;
        towerCellArray = new TheFiveCellBase[height, wight];
        transDirArray = new MultiLinkNode<TheFiveCellBase>[height, wight];
        // 加载地图Cell数据
        // 如果该位置为null则只绘制底板

        // 初始化链接

        // 刷新列表
        RefreshCells();
    }

    /// <summary>
    /// 绘制当前单位
    /// </summary>
    /// <param name="leftdown">地图左下点位置</param>
    /// <param name="unitWidth">地图单位宽度</param>
    public override void Draw(Vector3 leftdown, int unitWidth)
    {
        base.Draw(leftdown, unitWidth);
        // 绘制塔内单位
        DrawTowerCell();

        if (isNotSetGraphics)
        {
            graphics = new CircleGraphics(GameObj.transform.position, 10 * unitWidth);
            isNotSetGraphics = false;
        }

        //StepAction(this);
    }


    /// <summary>
    /// 绘制塔对象Cell
    /// </summary>
    public void DrawTowerCell()
    {
        if (towerCellArray != null)
        {
            // 遍历塔分布数据
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < wight; j++)
                {
                    towerCellArray[i, j].Draw(towerLeftDown, towerUnitWidth);
                }
            }
            // 将对应cell创建到对应位置

            // 如果绘制过了则判断变化, 否则不做操作


        }
    }

    /// <summary>
    /// 重新绘制单位
    /// </summary>
    public void RefreshCells()
    {
        // 遍历塔单元数据, 如果TheFiveCell与数据不同, 则更新该位置的cell
        if (towerCellDataArray != null)
        {
            if (towerCellArray == null)
            {
                towerCellArray = new TheFiveCellBase[height,wight];
            }
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < wight; j++)
                {
                    var val = towerCellDataArray[i, j];
                    var towerCellItem = towerCellArray[i, j];
                    if (towerCellItem == null || towerCellItem.DataId != val)
                    {
                        // 重新创建单元
                        //var newCell = UnitFictory.Single.CreateUnit<TheFiveCellBase>(UnitType.TowerCell, val) as TheFiveCellBase;
                        //if (newCell != null)
                        //{
                        //    newCell.X = j;
                        //    // 反向Y轴位置
                        //    newCell.Y = height - i - 1;
                        //    towerCellArray[i, j] = newCell;
                        //    //Debug.Log(i + "," + j + ":" + newCell.DataId);
                        //}
                        //else
                        //{
                        //    Debug.LogError("TowerCell创建失败:" + val + " x:" + j + " y:" + i);
                        //}
                        //// 销毁原先的单元
                        //if (towerCellItem != null)
                        //{
                        //    UnitFictory.Single.DestoryMapCell(towerCellItem);
                        //    Debug.Log("销毁x:" + j + " y:" + i + "的TowerCell");
                        //}
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取去向cell列表
    /// </summary>
    /// <param name="x">cell位置x</param>
    /// <param name="y">cell位置y</param>
    /// <returns></returns>
    public List<TheFiveCellBase> GetNextTheFiveCellList(int x, int y)
    {
        List<TheFiveCellBase> result = null;

        if (x >= 0 && x <= wight && 
            y >= 0 && y <= height &&
            transDirArray != null)
        {
            var multiMember = transDirArray[x, y];
            if (multiMember != null)
            {
                result = multiMember.GetTargetList();
            }
        }

        return result;
    }

    /// <summary>
    /// 显示物体
    /// </summary>
    public override void Show()
    {
        base.Show();
        // 处理子集cell
        if (towerCellArray != null)
        {
            foreach (var cell in towerCellArray)
            {
                if (cell != null)
                {
                    cell.Show();
                }
            }
        }
    }

    /// <summary>
    /// 隐藏物体
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        // 处理子集cell
        if (towerCellArray != null)
        {
            foreach (var cell in towerCellArray)
            {
                if (cell != null)
                {
                    cell.Hide();
                }
            }
        }
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
/// 多源多响链表节点
/// </summary>
public class MultiLinkNode<T>
{
    /// <summary>
    /// 被保存单位
    /// </summary>
    public T Tobj { get; set; }

    /// <summary>
    /// 来源列表
    /// </summary>
    private readonly List<MultiLinkNode<T>> sourceNodeList = new List<MultiLinkNode<T>>();

    /// <summary>
    /// 去向列表
    /// </summary>
    private readonly List<MultiLinkNode<T>> targetNodeList = new List<MultiLinkNode<T>>();


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="t"></param>
    public MultiLinkNode(T t)
    {
        Tobj = t;
    }

    /// <summary>
    /// 添加源对象
    /// </summary>
    /// <param name="sourceNode">源对象</param>
    public void AddSource([NotNull] MultiLinkNode<T> sourceNode)
    {
        if (!sourceNodeList.Contains(sourceNode))
        {
            sourceNodeList.Add(sourceNode);
        }
    }

    /// <summary>
    /// 添加目标对象
    /// </summary>
    /// <param name="targetNode">目标对象</param>
    public void AddTarget([NotNull] MultiLinkNode<T> targetNode)
    {
        if (!targetNodeList.Contains(targetNode))
        {
            targetNodeList.Add(targetNode);
        }
    }

    /// <summary>
    /// 删除源对象
    /// </summary>
    /// <param name="sourceNode"></param>
    public void RemoveSource([NotNull] MultiLinkNode<T> sourceNode)
    {
        if (sourceNodeList.Contains(sourceNode))
        {
            sourceNodeList.Remove(sourceNode);
        }
    }

    /// <summary>
    /// 删除目标对象
    /// </summary>
    /// <param name="targetNode"></param>
    public void RemoveTarget([NotNull] MultiLinkNode<T> targetNode)
    {
        if (targetNodeList.Contains(targetNode))
        {
            targetNodeList.Remove(targetNode);
        }
    }

    /// <summary>
    /// 获取去列表
    /// </summary>
    /// <returns></returns>
    public List<T> GetTargetList()
    {
        List<T> result = null;
        if (targetNodeList.Count > 0)
        {
            result = new List<T>();
            foreach (var item in targetNodeList)
            {
                result.Add(item.Tobj);
            }
        }
        return result;
    }

    /// <summary>
    /// 获取源列表
    /// </summary>
    /// <returns></returns>
    public List<T> GetSourceList()
    {
        List<T> result = null;
        if (sourceNodeList.Count > 0)
        {
            result = new List<T>();
            foreach (var item in sourceNodeList)
            {
                result.Add(item.Tobj);
            }
        }
        return result;
    } 
}