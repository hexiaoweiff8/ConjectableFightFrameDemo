using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 地图绘制器
/// </summary>
public class MapDrawer : MapDrawerBase
{
    /// <summary>
    /// 单例
    /// </summary>
    public static MapDrawer Single { get { return single; } }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static MapDrawer single = null;

    // -----------------------------公有属性-----------------------------

    /// <summary>
    /// 地图宽度
    /// </summary>
    public int MapWidth { get { return mapBase != null ? mapBase.MapWidth : 0; } }

    /// <summary>
    /// 地图高度
    /// </summary>
    public int MapHeight { get { return mapBase != null ? mapBase.MapHeight : 0; } }

    /// <summary>
    /// 是否已启动
    /// </summary>
    public bool IsStarted { get { return isStarted; } }

    /// <summary>
    /// 单位宽度
    /// </summary>
    public int UnitWidth { get; private set; }

    /// <summary>
    /// 单位父级
    /// </summary>
    public List<Transform> ItemParentList;


    // -----------------------------私有属性-----------------------------

    /// <summary>
    /// 地图数据
    /// </summary>
    private MapBase mapBase = null;


    /// <summary>
    /// 是否已启动
    /// </summary>
    private bool isStarted = false;

    /// <summary>
    /// 绘制范围
    /// </summary>
    private ICollisionGraphics drawGraphics;

    /// <summary>
    /// 历史Rect
    /// 判断Rect变更使用
    /// </summary>
    private ICollisionGraphics historyGraphics;

    /// <summary>
    /// 绘制处理事件
    /// </summary>
    private Action<int, MapCellBase[,]> drawAction = null;


    void Awake()
    {
        // 设置单例
        single = this;
        // 设置切换不销毁
        DontDestroyOnLoad(this);
        // 构建绘制范围变更事件
        // 只绘制地板层, 障碍层
        drawAction = (layer, array) =>
        {
            // 数据宽度
            var width = mapBase.MapWidth;
            // 数据长度
            var height = mapBase.MapHeight;
            var i = 0;
            var j = 0;

            if (layer != MapManager.MapBaseLayer)
            {
                // TODO 判断是否有单元被移动, 被移动的需要被检测

                // 遍历所有单位
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width; j++)
                    {
                        //var key = Utils.GetKey(j, i);
                        var item = array[i, j];
                        if (item != null)
                        {
                            if (drawGraphics.CheckCollision(item.GetGraphics()))
                            {
                                // 刷新范围内单位
                                item.Show();
                            }
                            else
                            {
                                item.Hide();
                            }
                        }
                    }
                }

            }
            else
            {
                // 判断mapBase是否在范围内
                // 遍历所有单位
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width; j++)
                    {
                        //var key = Utils.GetKey(j, i);
                        var item = array[i, j] as MapCell;
                        if (item != null)
                        {
                            if (drawGraphics.CheckCollision(item.GetGraphics()))
                            {
                                item.EnterScreen();
                            }
                            else
                            {
                                item.OutScreen();
                            }
                        }
                    }
                }
            }

        };
    }



    /// <summary>
    /// 帧更新方法
    /// </summary>
    void Update()
    {
        if (mapBase != null && isStarted)
        {
            // 绘制线
        //#if UNITY_EDITOR
        //    mapData.DrawLine();
        //#endif
            Draw();
        }
        // 更新地图单元相对位置
    }


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="mapBaseParam">绘制数据</param>
    /// <param name="mapCenter">地图中心</param>
    /// <param name="graphics">绘制范围</param>
    /// <param name="type">绘制类型</param>
    public void Init([NotNull] MapBase mapBaseParam, Vector3 mapCenter, ICollisionGraphics graphics, int type = 0)
    {
        Clear();
        mapBase = mapBaseParam;
        drawGraphics = graphics == null ? new RectGraphics(Vector2.zero, 0, 0, 0) : graphics;
        UnitWidth = mapBaseParam.UnitWidth;


        ChangeDrawGraphics(graphics);
    }

    /// <summary>
    /// 修改绘制范围
    /// </summary>
    /// <param name="graphics">绘制范围类</param>
    public void ChangeDrawGraphics(ICollisionGraphics graphics)
    {

        drawGraphics = graphics;
    }


    /// <summary>
    /// 绘制全局
    /// </summary>
    public override void Draw()
    {
        Utils.DrawGraphics(drawGraphics, Color.yellow);
        // 当前范围是否移动, 如果移动则更新列表, 如果未移动则使用旧列表数据
        if (drawGraphics == null ||
            historyGraphics == null ||
            drawGraphics.Postion != historyGraphics.Postion ||
            mapBase.NeedDraw)
        {
            // 局部绘制控制(只有地板)
            mapBase.Foreach(drawAction);
            // 全绘制(除了地板)
            mapBase.DrawMap(drawGraphics);
            // 记录绘制范围
            if (historyGraphics == null)
            {
                historyGraphics = drawGraphics.Clone();
            }
            historyGraphics.Copy(drawGraphics);

        }

        // 判断是否有移动单位变更


    }


    /// <summary>
    /// 开始运行
    /// </summary>
    public override void Begin()
    {
        Debug.Log("开始绘制");
        isStarted = true;
    }

    /// <summary>
    /// 停止运行
    /// </summary>
    public override void Stop()
    {
        Debug.Log("停止绘制");
        isStarted = false;
    }

    /// <summary>
    /// 清理数据
    /// </summary>
    public override void Clear()
    {
        Debug.Log("清理数据");
        if (mapBase != null)
        {
            mapBase.Clear();
            mapBase = null;
        }
        Stop();
    }

}


/// <summary>
/// 地图绘制器抽象类
/// </summary>
public abstract class MapDrawerBase : MonoBehaviour
{

    /// <summary>
    /// 绘制全局
    /// </summary>
    public abstract void Draw();

    /// <summary>
    /// 开始运行
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// 停止运行
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// 清理数据
    /// </summary>
    public abstract void Clear();


}


/// <summary>
/// 地图加载器
/// </summary>
public class MapLoader
{
    /// <summary>
    /// 单例
    /// </summary>
    public static MapLoader Single
    {
        get
        {
            if (single == null)
            {
                single = new MapLoader();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static MapLoader single = null;



    /// <summary>
    /// 根据MapId加载地图数据
    /// </summary>
    /// <param name="mapId">map编号</param>
    /// <returns>地图数据类</returns>
    public MapBase LoadMap(int mapId)
    {
        MapBase result = null;

        // 读取地图文件
        // 获得地图宽度高度
        // 遍历内容加载单位
        // 已存在单位加载设置
        //for (var i = 0; i < MapHeight; i++)
        //{
        //    for (var j = 0; j < MapWidth; j++)
        //    {
        //        MapBase.Single.PushMapCell(new MapCell(1), j, i);
        //    }
        //}

        return result;
    }
}