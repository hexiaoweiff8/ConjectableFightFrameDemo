using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 地图基类
/// </summary>
public class MapBase
{

    // ------------------------------公共属性----------------------------------

    /// <summary>
    /// 地图宽度
    /// </summary>
    public int MapWidth { get { return mapWidth; } }

    /// <summary>
    /// 地图高度
    /// </summary>
    public int MapHeight { get { return mapHeight; } }

    /// <summary>
    /// 地图单位宽度
    /// </summary>
    public int UnitWidth { get { return unitWidth; } }

    /// <summary>
    /// 地图中心位置
    /// </summary>
    public Vector3 MapCenter { get; private set; }

    /// <summary>
    /// 是否需要绘制
    /// </summary>
    public bool NeedDraw = false;

    /// <summary>
    /// 地图左上点
    /// </summary>
    public Vector2 Leftup { get; private set; }

    /// <summary>
    /// 地图右上点
    /// </summary>
    public Vector2 Rightup { get; private set; }

    /// <summary>
    /// 地图左下点
    /// </summary>
    public Vector2 Leftdown { get; private set; }

    /// <summary>
    /// 地图右下点
    /// </summary>
    public Vector2 Rightdown { get; private set; }


    // ------------------------------私有属性----------------------------------

    ///// <summary>
    ///// 地图底板层
    ///// </summary>
    //private MapCellBase[,] mapCellArray;

    /// <summary>
    /// 地图层字典
    /// </summary>
    private Dictionary<int, MapCellBase[,]> mapCellArrayDic = new Dictionary<int, MapCellBase[,]>();

    /// <summary>
    /// 地图数据字典
    /// </summary>
    private Dictionary<int, int[][]> mapArrayDic = new Dictionary<int, int[][]>();

    /// <summary>
    /// 地图数据分组字典
    /// </summary>
    private Dictionary<int, Dictionary<int, List<MapCellBase>>> mapDataGroupDic = new Dictionary<int, Dictionary<int, List<MapCellBase>>>();

    /// <summary>
    /// 地图高度
    /// </summary> 
    private int mapHeight;

    /// <summary>
    /// 地图宽度
    /// </summary>
    private int mapWidth;

    /// <summary>
    /// 地图单位宽度
    /// </summary>
    private int unitWidth;

    /// <summary>
    /// 地图线绘制颜色
    /// </summary>
    private Color lineColor;



    // ------------------------------公共方法-----------------------------------

    
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHeight">地图高度</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="newCenter">地图中心</param>
    public MapBase(int mapWidth, int mapHeight, Vector3 newCenter, int unitWidth)
    {
        ResetMapPos(newCenter, mapWidth, mapHeight, unitWidth);
        lineColor = Color.red;
    }

    /// <summary>
    /// 添加层数据
    /// </summary>
    /// <param name="mapCellArray">地图层数据</param>
    /// <param name="mapArray">mapDataArray数据</param>
    /// <param name="layer">数据所在层</param>
    public void AddMapCellArray([NotNull]MapCellBase[,] mapCellArray, [NotNull]int[][] mapArray, int layer)
    {
        if (mapCellArrayDic.ContainsKey(layer) || mapArrayDic.ContainsKey(layer))
        {
            throw new Exception("该层已存在:" + layer);
        }
        mapCellArrayDic.Add(layer, mapCellArray);
        mapArrayDic.Add(layer, mapArray);
        // 遍历数据, 添加进分组字典
        foreach (var mapCell in mapCellArray)
        {
            if (mapCell != null)
            {
                AddMapCell(mapCell, layer);
            }
        }
        NeedDraw = true;
    }

    /// <summary>
    /// 添加地图单元
    /// </summary>
    /// <param name="mapCell"></param>
    /// <param name="layer"></param>
    public void AddMapCell([NotNull] MapCellBase mapCell, int layer)
    {
        if (!mapDataGroupDic.ContainsKey(layer))
        {
            mapDataGroupDic.Add(layer, new Dictionary<int, List<MapCellBase>>());
        }
        var groupDic = mapDataGroupDic[layer];
        // 遍历数据, 添加进分组字典
        if (!groupDic.ContainsKey(mapCell.DataId))
        {
            groupDic.Add(mapCell.DataId, new List<MapCellBase>() {mapCell});
        }
        else
        {
            groupDic[mapCell.DataId].Add(mapCell);
        }

        mapCellArrayDic[layer][mapCell.Y, mapCell.X] = mapCell;
        mapArrayDic[layer][mapCell.Y][mapCell.X] = mapCell.DataId;

        NeedDraw = true;
    }

    /// <summary>
    /// 绘制格子
    /// </summary>
    public void DrawLine()
    {
        // 在底板上画出格子
        // 画四边
        Debug.DrawLine(Leftup, Rightup, lineColor);
        Debug.DrawLine(Leftup, Leftdown, lineColor);
        Debug.DrawLine(Rightdown, Rightup, lineColor);
        Debug.DrawLine(Rightdown, Leftdown, lineColor);

        // 绘制格子
        for (var i = 1; i <= mapWidth; i++)
        {
            Debug.DrawLine(Leftup + new Vector2(i*unitWidth, 0), Leftdown + new Vector2(i*unitWidth, 0), lineColor);
        }
        for (var i = 1; i <= mapHeight; i++)
        {
            Debug.DrawLine(Leftdown + new Vector2(0, i*unitWidth), Rightdown + new Vector2(0, i*unitWidth), lineColor);
        }
    }


    /// <summary>
    /// 绘制地图
    /// </summary>
    public void DrawMap(ICollisionGraphics rect)
    {
        // 遍历地图
        foreach (var kv in mapDataGroupDic)
        {
            foreach (var kv2 in kv.Value)
            {
                foreach (var item in kv2.Value)
                {
                    item.Draw(Leftdown, unitWidth);
                }
            }
        }

        NeedDraw = false;
        // 判断变更
        // 绘制变更
        // 否则跳过
    }


    /// <summary>
    /// 重建地图数据
    /// </summary>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    public void ReBuildMap(int width, int height)
    {
        if (width < 0 || height < 0)
        {
            throw new Exception("地图大小不合法:" + height + "," + width);
        }
        mapWidth = width;
        mapHeight = height;
        mapCellArrayDic.Clear();
    }
    

    /// <summary>
    /// 获取地图CellArray
    /// </summary>
    /// <returns>地图数据</returns>
    public MapCellBase[,] GetMapCellArray(int layer)
    {
        if (!mapCellArrayDic.ContainsKey(layer))
        {
            throw new Exception("不存在地图层:" + layer);
        }
        return mapCellArrayDic[layer];
    }

    /// <summary>
    /// 获取地图Array
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public int[][] GetMapArray(int layer)
    {
        if (!mapArrayDic.ContainsKey(layer))
        {
            throw new Exception("不存在地图层:" + layer);
        }
        return mapArrayDic[layer];
    }

    /// <summary>
    /// 获取地图单元列表
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<MapCellBase> GetMapCellList(int layer, int id)
    {
        List<MapCellBase> result = null;

        if (mapDataGroupDic.ContainsKey(layer))
        {
            if (mapDataGroupDic[layer].ContainsKey(id))
            {
                result = mapDataGroupDic[layer][id];
            }
        }

        return result;
    }

    /// <summary>
    /// 清理数据
    /// </summary>
    public void Clear()
    {
        // TODO 待解决
        //foreach (var kv in mapDataGroupDic)
        //{
        //    foreach (var kv2 in kv.Value)
        //    {
        //        foreach (var item in kv2.Value)
        //        {
        //            UnitFictory.Single.DestoryMapCell(item);
        //        }
        //    }
        //}
        mapArrayDic.Clear();
        mapCellArrayDic.Clear();
    }


    ///// <summary>
    ///// 获取对应位置的Rect
    ///// </summary>
    ///// <param name="posX">位置X</param>
    ///// <param name="posY">位置Y</param>
    ///// <returns></returns>
    //public Rect GetItemRect(int posX, int posY)
    //{
    //    return new Rect(posX, posY, unitWidth, unitWidth);
    //}

    // ------------------------------私有方法-----------------------------------

    /// <summary>
    /// 重置地图位置点
    /// </summary>
    public void ResetMapPos(Vector3 newCenter, int newMapWidth, int newMapHeight, int unitWidth)
    {

        // 验证x与y是否越界
        if (newMapWidth < 0)
        {
            Debug.LogError("地图宽度非法:" + newMapWidth);
            return;
        }
        if (newMapHeight < 0)
        {
            Debug.LogError("地图高度非法:" + newMapHeight);
            return;
        }

        // 重置中心位置与宽高
        MapCenter = newCenter;
        mapWidth = newMapWidth;
        mapHeight = newMapHeight;

        this.unitWidth = unitWidth;
        // 地图宽高
        var width = mapWidth * unitWidth;
        var height = mapHeight * unitWidth;
        //var halfWidht = mapWidth * unitWidth * 0.5f - unitWidth * 0.5f;
        //var halfHeight = mapHeight * unitWidth * 0.5f - unitWidth * 0.5f;
        // 取矩形四角点
        Leftup = new Vector2(MapCenter.x, MapCenter.y + height);
        Rightup = new Vector2(MapCenter.x + width, MapCenter.y + height);
        Leftdown = new Vector2(MapCenter.x, MapCenter.y);
        Rightdown = new Vector2(MapCenter.x + width, MapCenter.y);
    }

    /// <summary>
    /// 遍历每个地图
    /// </summary>
    public void Foreach(Action<int, MapCellBase[,]>  action)
    {
        foreach (var kv in mapCellArrayDic)
        {
            action(kv.Key, kv.Value);
        }
    }
}


/// <summary>
/// 地图单元抽象类
/// </summary>
public abstract class MapCellBase
{

    /// <summary>
    /// 地图单元ID
    /// </summary>
    public virtual long MapCellId { get; private set; }

    /// <summary>
    /// 地图单位类型
    /// </summary>
    public UnitType MapCellType { get; set; }

    /// <summary>
    /// 地图Obj单位
    /// </summary>
    public virtual GameObject GameObj { get; set; }

    /// <summary>
    /// 该单元的数据ID
    /// </summary>
    public virtual int DataId { get; set; }

    /// <summary>
    /// 位置X
    /// </summary>
    public virtual int X { get; set; }

    /// <summary>
    /// 位置Y
    /// </summary>
    public virtual int Y { get; set; }

    /// <summary>
    /// 触发事件
    /// </summary>
    public Action<MapCellBase> Action { get; set; }

    /// <summary>
    /// step事件
    /// </summary>
    public Action<MapCellBase> StepAction { get; set; }

    /// <summary>
    /// 步驱动时间, 如果小于0则不驱动
    /// </summary>
    public float StepIntervalTime = -1;


    /// <summary>
    /// 自增唯一ID
    /// </summary>
    private static long addtionId = 1024;

    /// <summary>
    /// 绘制层级
    /// </summary>
    private int drawLayer = -1;

    /// <summary>
    /// 历史Rect
    /// </summary>
    private ICollisionGraphics historyRect;

    /// <summary>
    /// 历史位置X
    /// </summary>
    private int historyX = -1;

    /// <summary>
    /// 历史位置Y
    /// </summary>
    private int historyY = -1;

    /// <summary>
    /// 最后一次运行时间
    /// </summary>
    private float lastRunTime = -1;

    /// <summary>
    /// 历史位置X
    /// </summary>
    protected int historyXForDraw = -1;

    /// <summary>
    /// 历史位置Y
    /// </summary>
    protected int historyYForDraw = -1;

    /// <summary>
    /// 历史单位宽度
    /// </summary>
    protected int historyUnitWidth = 0;

    /// <summary>
    /// 是否绘制
    /// </summary>
    protected bool isShow = true;




    /// <summary>
    /// 基础初始化
    /// </summary>
    protected MapCellBase(GameObject obj, int dataId, int drawLayer)
    {
        // 当前新类的ID并自增
        MapCellId = addtionId++;
        // 初始化模型
        this.GameObj = obj;
        DataId = dataId;
        this.drawLayer = drawLayer;
    }

    /// <summary>
    /// 绘制方法
    /// </summary>
    public virtual void Draw(Vector3 leftdown, int unitWidth)
    {
        // 判断是否有变动
        if (isShow && GameObj != null)
        {
            CheckScale(unitWidth);
            if (X != historyXForDraw || Y != historyYForDraw)
            {
                GameObj.transform.position = new Vector3(leftdown.x + X * unitWidth,
                    leftdown.y + Y * unitWidth);
                historyXForDraw = X;
                historyYForDraw = Y;
            }
        }
    }

    /// <summary>
    /// 显示物体
    /// </summary>
    public virtual void Show()
    {
        if (GameObj != null)
        {
            GameObj.SetActive(true);
        }
        // 设置显示层级
        isShow = true;
    }

    /// <summary>
    /// 隐藏物体
    /// </summary>
    public virtual void Hide()
    {
        if (GameObj != null)
        {
            GameObj.SetActive(false);
        }
        isShow = false;
    }

    /// <summary>
    /// 获取该位置的Rect
    /// </summary>
    /// <returns>该位置的Rect</returns>
    public virtual ICollisionGraphics GetGraphics()
    {
        // 如果位置有变更则更新Rect
        if (X != historyX || Y != historyY)
        {
            var unitWidth = MapDrawer.Single.UnitWidth;
            historyX = X;
            historyY = Y;
            historyRect = new RectGraphics(new Vector2(X * unitWidth - MapDrawer.Single.MapWidth * unitWidth * 0.5f, Y * unitWidth - MapDrawer.Single.MapHeight * unitWidth * 0.5f), unitWidth, unitWidth, 0);
        }
        return historyRect;
    }
    
    /// <summary>
    /// Tower驱动功能
    /// </summary>
    public virtual void NextActionStep(float nowTime)
    {
        if (StepIntervalTime > 0 && StepAction != null)
        {
            if (nowTime - lastRunTime > StepIntervalTime)
            {
                StepAction(this);
                lastRunTime = nowTime;
            }
        }
    }

    /// <summary>
    /// 检查并重置缩放
    /// </summary>
    /// <param name="unitWidth">单位宽度</param>
    protected void CheckScale(int unitWidth)
    {
        if (unitWidth != historyUnitWidth && GameObj != null)
        {
            // 控制缩放, 每个单位的prefab大小默认是1, 赞找unitWidth的大小进行拉伸
            GameObj.transform.localScale = new Vector3(unitWidth, unitWidth, 1);
            historyUnitWidth = unitWidth;
        }
    }
}
