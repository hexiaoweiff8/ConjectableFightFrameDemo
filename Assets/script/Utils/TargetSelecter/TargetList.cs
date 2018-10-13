using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标列表
/// </summary>
/// <typeparam name="T"></typeparam>
public class TargetList<T> where T : IGraphicsHolder//IGraphical<Rectangle>
{
    /// <summary>
    /// 返回全引用列表
    /// </summary>
    public IList<T> List { get { return list; } } 

    /// <summary>
    /// 返回四叉树列表
    /// </summary>
    public QuadTree<T> QuadTree { get { return quadTree; } }

    /// <summary>
    /// 目标总列表
    /// </summary>
    private IList<T> list = null;

    /// <summary>
    /// 四叉树
    /// </summary>
    private QuadTree<T> quadTree = null;

    /// <summary>
    /// 地图信息
    /// </summary>
    private MapBase mapBase = null;


    /// <summary>
    /// 创建目标列表
    /// </summary>
    /// <param name="x">地图位置x</param>
    /// <param name="y">地图位置y</param>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    /// <param name="unitWidht"></param>
    public TargetList(float x, float y, int width, int height, int unitWidht)
    {
        var mapRect = new RectGraphics(new Vector2(x, y), width * unitWidht, height * unitWidht, 0);
        quadTree = new QuadTree<T>(0, mapRect);
        list = new List<T>();
    }

    /// <summary>
    /// 添加单元
    /// </summary>
    /// <param name="t">单元对象, 类型T</param>
    public void Add(T t)
    {
        // 空对象不加入队列
        if (t == null)
        {
            return;
        }
        if (list.Contains(t))
        {
            Debug.LogError("单位在列表中已存在");
            return;
        }
        // 加入全局列表
        list.Add(t);
        // 加入四叉树
        quadTree.Insert(t);
    }

    /// <summary>
    /// 删除单位
    /// </summary>
    /// <param name="t"></param>
    public void Remove(T t)
    {
        // 空对象不加入队列
        if (t == null)
        {
            return;
        }
        list.Remove(t);
        // TODO 目前使用重构建方式解决四叉树删除单位, 有更好方案后替换
        RebuildQuadTree();
    }


    /// <summary>
    /// 根据范围获取对象
    /// </summary>
    /// <param name="graphics">范围对象, 用于判断碰撞</param>
    /// <returns></returns>
    public IList<T> GetListWithRectangle(ICollisionGraphics graphics)
    {
        // 返回范围内的对象列表
        return quadTree.Retrieve(graphics);
    }

    /// <summary>
    /// 重新构建四叉树
    /// 使用情况: 列表中对向位置已变更时
    /// </summary>
    public void RebuildQuadTree()
    {
        quadTree.Clear();
        quadTree.Insert(list);
    }


    public void Refresh()
    {
        quadTree.Refresh();
    }


    //public void RebulidMapInfo()
    //{
    //    if (mapinfo != null)
    //    {
    //        mapinfo.RebuildMapInfo(list);
    //    }
    //}

    /// <summary>
    /// 清理数据
    /// </summary>
    public void Clear()
    {
        if (list != null)
        {
            list.Clear();
        }
        if (quadTree != null)
        {
            quadTree.Clear();
        }
    }

}