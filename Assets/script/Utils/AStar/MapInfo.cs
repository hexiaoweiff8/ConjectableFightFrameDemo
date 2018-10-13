using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 地图信息
/// </summary>
public class MapInfo<T> where T : IGraphicsHolder//IGraphical<Rectangle>
{


    /// <summary>
    /// 地图信息
    /// </summary>
    public int[][] Map
    {
        get { return map; }
        set { map = value; }
    }

    /// <summary>
    /// 地图信息
    /// </summary>
    private int[][] map = null;

    /// <summary>
    /// 地图单位格子跨度
    /// </summary>
    private int unitWidth = 1;

    /// <summary>
    /// 地图位置便宜
    /// </summary>
    private Vector3 planePosition;

    /// <summary>
    /// 地图宽度
    /// </summary>
    private int mapWidth = 0;

    /// <summary>
    /// 地图高度
    /// </summary>
    private int mapHeight = 0;


    /// <summary>
    /// 增加地图信息
    /// </summary>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    /// <param name="map">地图信息</param>
    /// <param name="unitw">地图格子单位宽度</param>
    public void AddMap(int unitw, int width, int height, int[][] map)
    {
        this.map = map;
        this.unitWidth = unitw;
        mapWidth = width;
        mapHeight = height;
        
    }


    /// <summary>
    /// 获取对象位置周围的格子
    /// </summary>
    /// <param name="t">单位</param>
    /// <param name="halfDimension">维数</param>
    /// <returns>返回单位轴为 (维数*2 + 1)平方个单位(包含自身)</returns>
    public Node[] GetAroundPos(T t, int halfDimension)
    {
        Node[] result = null;

        if (t != null && halfDimension > 0)
        {
            var node = GetPos(t);
            if (ValidatePos(node))
            {
                var dimension = halfDimension*2 + 1;
                result = new Node[dimension*dimension];
                int x;
                int y;
                // 获取当前节点的位置
                for (int i = 0; i < dimension; i++)
                {
                    // 求周围xy
                    x = node.X + i % dimension - halfDimension;
                    y = node.Y + i / dimension - halfDimension;
                    
                    result[i] = new Node(x, y);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 获取当前对象所在格子位置
    /// </summary>
    /// <param name="t">单位</param>
    /// <returns>单位当前在地图中对应位置 返回如果为null 则无效, 否则返回一个长度为2的int数组 0位置为x, 1位置为y</returns>
    public Node GetPos(T t)
    {
        Node result = null;

        if (t != null && t.MyCollisionGraphics != null)
        {
            var graphics = t.MyCollisionGraphics;
            var pos = Utils.PositionToNum(planePosition,
                    new Vector3(graphics.Postion.x, 0, graphics.Postion.y),
                    unitWidth, mapWidth, mapHeight);
            result = new Node(pos[0], pos[1]);
        }

        return result;
    }

    /// <summary>
    /// 重新构建地图信息
    /// </summary>
    public void RebuildMapInfo(IList<T> list)
    {
        if (map != null)
        {
            // 将每个对象的位置对应带地图上
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                // 位置转换为下标
                var pos = GetPos(item);
                if (ValidatePos(pos))
                {
                    if (item is ClusterData) {
                        map[pos.Y][pos.X] = AStarPathFinding.Member;
                    }else if (item is FixtureData) {
                        map[pos.Y][pos.X] = AStarPathFinding.Obstacle;
                    } else {
                        map[pos.Y][pos.X] = AStarPathFinding.Accessibility;
                    }
                } else {
                    map[pos.Y][pos.X] = AStarPathFinding.Accessibility;
                }
            }
        }
    }

    /// <summary>
    /// 验证位置有效性
    /// </summary>
    /// <param name="pos">位置信息</param>
    /// <returns>是否有效</returns>
    private bool ValidatePos(Node pos)
    {
        var result = false;
        if (pos != null)
        {
            if (pos.X >= 0 && pos.X < mapWidth && pos.Y >= 0 && pos.Y < mapHeight)
            {
                result = true;
            }
        }
        return result;
    }

}




/// <summary>
/// 地图节点
/// </summary>
public class Node : IComparable<Node>
{
    public Node(int x, int y, int g = 0, int h = 0)
    {
        X = x;
        Y = y;
        G = g;
        H = h;
    }
    // 由get set修改为公共属性, get set在寻路过程中效率有瓶颈
    public int X = 0;
    public int Y = 0;

    public float G = 0;
    public float H = 0;
    public float F = 0;
    public Node Parent = null;

    /// <summary>
    /// 周围节点
    /// </summary>
    public Node[] Surround = null;

    public override string ToString()
    {
        return string.Format("x:{0},y:{1},g:{2},h:{3},f:{4}", X, Y, G, H, F);
    }
    public int CompareTo(Node node)
    {
        if (node.F > F)
        {
            return -1;
        }
        return node.F < F ? 1 : 0;
    }
}