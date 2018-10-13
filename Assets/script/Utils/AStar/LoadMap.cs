using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 该类创建在战场
/// 由外部调用创建
/// 将初始化方法暴露, 流程具体功能不在此处描述
/// 提供抽象的流程控制
/// </summary>
public class LoadMap : MonoBehaviour
{
    //-------------------------公共属性-----------------------------

    /// <summary>
    /// 单例
    /// </summary>
    public static LoadMap Single = null;

    /// <summary>
    /// 地图底板
    /// </summary>
    public GameObject MapPlane;

    /// <summary>
    /// 障碍物对象, 如果该对象为空则创建cube
    /// </summary>
    public GameObject Obstacler;

    /// <summary>
    /// 障碍物父级
    /// </summary>
    public GameObject ObstaclerList;


    /// <summary>
    /// 是否显示
    /// </summary>
    public bool IsShow;

    /// <summary>
    /// 网格线颜色
    /// </summary>
    public Color LineColor = Color.red;

    /// <summary>
    /// 地图宽度
    /// </summary>
    public int MapWidth { get { return mapWidth; } }

    /// <summary>
    /// 地图高度
    /// </summary>
    public int MapHeight { get { return mapHeight;} }

    /// <summary>
    /// 地图单元宽度
    /// </summary>
    public int UnitWidth { get { return unitWidth;} }


    //-------------------------公共常量-----------------------------


    //-------------------------私有属性-----------------------------
    /// <summary>
    /// 地面map数据
    /// </summary>
    private int[][] surfaceMapData = null;

    /// <summary>
    /// 空中map数据
    /// </summary>
    private int[][] airMapData = null;

    /// <summary>
    /// 地图宽度(X)
    /// </summary>
    private int mapWidth = -1;

    /// <summary>
    /// 地图高度(Y)
    /// </summary>
    private int mapHeight = -1;
    /// <summary>
    /// 单位宽度
    /// </summary>
    private int unitWidth = 1;

    /// <summary>
    /// 地图状态
    /// </summary>
    private Dictionary<long, GameObject> mapStateDic = new Dictionary<long, GameObject>();


    //-------------------计算优化属性---------------------
    /// <summary>
    /// 半地图宽度
    /// </summary>
    private float halfMapWidth;

    /// <summary>
    /// 半地图长度
    /// </summary>
    private float halfMapHight;

    /// <summary>
    /// 地图四角位置
    /// 初始化时计算
    /// </summary>
    private Vector3 leftup = Vector3.zero;
    private Vector3 leftdown = Vector3.zero;
    private Vector3 rightup = Vector3.zero;
    private Vector3 rightdown = Vector3.zero;


    //private List<GameObject> ObstaclerArray = new List<GameObject>();

    void Start()
    {
        IsShow = true;
        Single = this;
    }

    void Awake()
    {
        Start();
    }

    void Update()
    {
        // 画格子
        DrawLine();
    }


    /// <summary>
    /// 初始化 将地图数据传入
    /// </summary>
    /// <param name="map">地图数据</param>
    /// <param name="unitWidth">地图单位宽度</param>
    public void Init(int[][] map, int unitWidth)
    {
        // 设置本地数据
        surfaceMapData = map;
        this.unitWidth = unitWidth;

        // 验证数据
        if (!ValidateData())
        {
            Debug.LogError("参数错误!");
            return;
        }

        // 初始化地图宽度长度
        mapHeight = surfaceMapData.Length;
        mapWidth = surfaceMapData[0].Length;

        // 初始化空中地图
        airMapData = new int[mapHeight][];
        for (var i = 0; i < airMapData.Length; i++)
        {
            airMapData[i] = new int[mapWidth];
            var row = airMapData[i];
            for (var j = 0; j < row.Length; j++)
            {
                row[j] = 0;
            }
        }

        // 初始化优化数据
        halfMapWidth = mapWidth * 0.5f * this.unitWidth;
        halfMapHight = mapHeight * 0.5f * this.unitWidth;

        // 获得起始点
        Vector3 startPosition = MapPlane.transform.position;
        // 初始化四角点
        leftup = new Vector3(-halfMapWidth + startPosition.x, (MapPlane.transform.localScale.y) / 2 + startPosition.y, halfMapHight + startPosition.z);
        leftdown = new Vector3(-halfMapWidth + startPosition.x, (MapPlane.transform.localScale.y) / 2 + startPosition.y, -halfMapHight + startPosition.z);
        rightup = new Vector3(halfMapWidth + startPosition.x, (MapPlane.transform.localScale.y) / 2 + startPosition.y, halfMapHight + startPosition.z);
        rightdown = new Vector3(halfMapWidth + startPosition.x, (MapPlane.transform.localScale.y) / 2 + startPosition.y, -halfMapHight + startPosition.z);
        // 缩放地图
        MapPlane.transform.localScale = new Vector3(mapWidth * unitWidth, 1, mapHeight * unitWidth);
        //RefreshMap();
    }

    /// <summary>
    /// 获取左下位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLeftBottom()
    {
        var localPos = transform.localPosition;
        return new Vector3(localPos.x - mapWidth * unitWidth * 0.5f, localPos.y, localPos.z - mapHeight * unitWidth * 0.5f);
    }

    /// <summary>
    /// 获取中心点
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenter()
    {
        return transform.localPosition;
    }

    /// <summary>
    /// 获取地面地图数据
    /// </summary>
    /// <returns></returns>
    public int[][] GetSurfaceMapData()
    {
        return surfaceMapData;
    }

    /// <summary>
    /// 获取空中地图
    /// </summary>
    /// <returns></returns>
    public int[][] GetAirMapData()
    {
        return airMapData;
    }

    /// <summary>
    /// 将位置映射到敌方位置
    /// </summary>
    /// <param name="pos">被映射位置</param>
    /// <returns>映射到对方阵营位置</returns>
    public Vector3 MapItemToEnemy(Vector3 pos)
    {
        Vector3 result = Vector3.zero;
        
        // 当前位置 + 地图偏移
        var mapAbsoluteWidth = MapWidth*UnitWidth;
        // loadMap的位置*2 - 地图宽度 = 地图偏移
        var mapOffsetX = transform.position.x*2 - mapAbsoluteWidth;

        // 单位位置 = 地图位置 - 当前位置 + 地图偏移
        result = new Vector3(mapAbsoluteWidth - pos.x + mapOffsetX, pos.y, pos.z);

        return result;
    }

    ///// <summary>
    ///// 刷新地图
    ///// </summary>
    //public void RefreshMap()
    //{
    //    //// 验证数据
    //    //if (!ValidateData())
    //    //{
    //    //    Debug.LogError("参数错误!");
    //    //    return;
    //    //}
    //    // 清除障碍物
    //    //CleanObstaclerList();

        

    //    // TODO 不显示逻辑地图障碍物则返回
    //    //if (!IsShow)
    //    //{
    //    //    return null;
    //    //}

    //    //var result = new List<FixtureData>();
    //    //// 验证障碍物, 如果为空则修改为cube
    //    //// 按照map数据构建地图大小

    //    //Vector3 startPosition = MapPlane.transform.position;
    //    //// 创建格子
    //    //for (var row = 0; row < mapData.Length; row++)
    //    //{
    //    //    var oneRow = mapData[row];
    //    //    if (oneRow == null)
    //    //    {
    //    //        continue;
    //    //    }
    //    //    for (int col = 0; col < oneRow.Length; col++)
    //    //    {
    //    //        var cell = oneRow[col];
    //    //        var key = (row << 32) + col;
    //    //        switch (cell)
    //    //        {
    //    //            case Utils.Obstacle:

    //    //                // 有障碍则在该位置创建障碍物 
    //    //                var isExist = mapStateDic.ContainsKey(key);
    //    //                if (isExist && mapStateDic[key] == null || !isExist)
    //    //                {
    //    //                    var newObstacler = CreateObstacler();
    //    //                    newObstacler.transform.parent = ObstaclerList == null ? null : ObstaclerList.transform;
    //    //                    newObstacler.transform.localScale = new Vector3(unitWidth, unitWidth, unitWidth);
    //    //                    newObstacler.transform.position = Utils.NumToPosition(startPosition, new Vector2(col, row), unitWidth, mapWidth, mapHeight);
    //    //                    mapStateDic[key] = newObstacler;
    //    //                    var fix = newObstacler.AddComponent<FixtureData>();
    //    //                    fix.MemberData = new VOBase()
    //    //                    {
    //    //                        SpaceSet = 1 * UnitWidth
    //    //                    };
    //    //                    fix.transform.localScale = new Vector3(UnitWidth, UnitWidth, UnitWidth);
    //    //                    fix.transform.position = Utils.NumToPosition(transform.position, new Vector2(col, row), UnitWidth, MapWidth, MapHeight);
    //    //                    fix.X = col * UnitWidth - MapWidth * UnitWidth * 0.5f;
    //    //                    fix.Y = row * UnitWidth - MapHeight * UnitWidth * 0.5f;
    //    //                    fix.Diameter = 1 * UnitWidth;
    //    //                    result.Add(fix);
    //    //                }
    //    //                break;
    //    //            case Utils.Accessibility:
    //    //                // 无障碍 如果有则清除障碍
    //    //                if (mapStateDic.ContainsKey(key))
    //    //                {
    //    //                    if (mapStateDic[key] != null)
    //    //                    {
    //    //                        //Destroy(mapStateDic[key]);
    //    //                        mapStateDic[key] = null;
    //    //                    }
    //    //                }
    //    //                break;
    //    //        }
    //    //    }
    //    //}

    //    //return result;
    //}

    //------------------------私有方法----------------------------

    /// <summary>
    /// 在地图上画出网格
    /// </summary>
    private void DrawLine()
    {
        // 不显示逻辑地图则返回
        if (!IsShow)
        {
            return;
        }
        // 在底板上画出格子
        // 画四边
        Debug.DrawLine(leftup, rightup, LineColor);
        Debug.DrawLine(leftup, leftdown, LineColor);
        Debug.DrawLine(rightdown, rightup, LineColor);
        Debug.DrawLine(rightdown, leftdown, LineColor);

        // 获得格数
        var xCount = mapWidth;
        var yCount = mapHeight;

        for (var i = 1; i <= xCount; i++)
        {
            Debug.DrawLine(leftup + new Vector3(i * unitWidth, 0, 0), leftdown + new Vector3(i * unitWidth, 0, 0), LineColor);
        }
        for (var i = 1; i <= yCount; i++)
        {
            Debug.DrawLine(leftdown + new Vector3(0, 0, i * unitWidth), rightdown + new Vector3(0, 0, i * unitWidth), LineColor);
        }
    }


    ///// <summary>
    ///// 创建障碍物对象
    ///// 如果障碍物引用为空则创建cube
    ///// </summary>
    ///// <returns>障碍物引用</returns>
    //private GameObject CreateObstacler()
    //{
    //    if (Obstacler == null)
    //    {
    //        Obstacler = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //        Destroy(Obstacler.GetComponent<BoxCollider>());
    //        Obstacler.name = "Obstacler";
    //        Obstacler.transform.localPosition = leftup;
    //        Obstacler.layer = LayerMask.NameToLayer("Effects");
    //        //string a = LayerMask.LayerToName(8);
    //    }
    //    var result = Instantiate(Obstacler);
    //    ObstaclerArray.Add(result);

    //    return result;
    //}

    ///// <summary>
    ///// 清空障碍物
    ///// </summary>
    //private void CleanObstaclerList()
    //{
    //    if (ObstaclerArray != null && ObstaclerArray.Count > 0)
    //    {
    //        foreach (var ob in ObstaclerArray)
    //        {
    //            Destroy(ob);
    //        }
    //        ObstaclerArray.Clear();
    //    }
    //}

    /// <summary>
    /// 验证数据
    /// </summary>
    /// <returns>是否验证通过</returns>
    private bool ValidateData()
    {

        if (surfaceMapData == null || MapPlane == null)
        {
            return false;
        }

        return true;
    }

}