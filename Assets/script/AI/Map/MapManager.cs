using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : SingleItem<MapManager>
{

    /// <summary>
    /// 地图背景层
    /// </summary>
    public const int MapBackLayer = 0;

    /// <summary>
    /// 地图基层
    /// </summary>
    public const int MapBaseLayer = 1;

    /// <summary>
    /// 障碍物层
    /// </summary>
    public const int MapObstacleLayer = 2;

    /// <summary>
    /// npc层
    /// </summary>
    public const int MapNpcLayer = 3;

    /// <summary>
    /// 主角层
    /// </summary>
    public const int MapPlayerLayer = 4;

    /// <summary>
    /// 近景环境层
    /// </summary>
    public const int MapCloseEnvironmentLayer = 5;

    /// <summary>
    /// 加载地图层级数量
    /// </summary>
    public const int LoadMapLevelCount = 3;

    /// <summary>
    /// 出兵点Id
    /// </summary>
    public const int OutMosterPointId = 301;

    /// <summary>
    /// 入兵点Id
    /// </summary>
    public const int InMonsterPointId = 302;

    /// <summary>
    /// 塔位点Id
    /// </summary>
    public const int TowerPointId = 401;

    /// <summary>
    /// 地图文件地址
    /// </summary>
    public const string MapDataFilePath = @"MapData\mapdata";

    /// <summary>
    /// 地图文件数据字典
    /// (地图文件名, 地图数据)
    /// </summary>
    private Dictionary<string, int[][]> mapDataDic = null;

    /// <summary>
    /// 地图宽高数据
    /// </summary>
    private Dictionary<string, Vector2> mapWHDic = null; 

    /// <summary>
    /// 地图数据类字典
    /// </summary>
    private Dictionary<int, MapBase> mapBaseDic = new Dictionary<int, MapBase>();

    /// <summary>
    /// 地图文件是否已加载
    /// </summary>
    private bool isLoaded = false;

    // -----------------文件加载管理--------------------

    /// <summary>
    /// 启动地图
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <param name="mapCenter">地图中心</param>
    /// <param name="unitWidth">地图单位宽度</param>
    /// <param name="drawType">绘制类型</param>
    public MapBase GetMapBase(int mapId, Vector3 mapCenter, int unitWidth, int drawType = 0)
    {
        if (MapDrawer.Single == null)
        {
            throw new Exception("地图绘制器为空.");
        }
        // 验证数据
        if (mapId < 0)
        {
            throw new Exception("地图Id错误:" + mapId);
        }
        if (unitWidth < 0)
        {
            throw new Exception("地图unitWidth错误:" + unitWidth);
        }

        MapBase mapBase = null;
        // 获取地图数据
        if (mapBaseDic.ContainsKey(mapId))
        {
            mapBase = mapBaseDic[mapId];
        }
        else
        {
            mapBase = GetMapInfo(mapId, mapCenter, unitWidth);

            // 缓存地图数据
            mapBaseDic.Add(mapId, mapBase);
        }

        if (mapBase == null)
        {
            throw new Exception("地图数据为空");
        }

        //// 启动绘制
        //MapDrawer.Single.Clear();
        //MapDrawer.Single.Init(mapBase, type: drawType);
        //MapDrawer.Single.ChangeDrawRect(Utils.GetShowRect(mapCenter,
        //    MapDrawer.Single.MapWidth,
        //    MapDrawer.Single.MapHeight,
        //    Screen.width + MapDrawer.Single.UnitWidth * 2,
        //    Screen.height + MapDrawer.Single.UnitWidth * 2,
        //    MapDrawer.Single.UnitWidth));
        //MapDrawer.Single.Begin();

        return mapBase;
    }



    /// <summary>
    /// 清空现有所有单位与层级
    /// </summary>
    public void Clear()
    {
        // 构建销毁方法
        MapDrawer.Single.Clear();
        mapBaseDic.Clear();
    }


    // ----------------------------私有方法---------------------------------

    /// <summary>
    /// 按照ID加载文件, 并且将加载文件缓存
    /// </summary>
    /// <param name="mapId">被加载地图DI</param>
    /// <param name="mapCenter">地图中心点</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <returns>被加载地图内容, 如果不存在返回null</returns>
    private MapBase GetMapInfo(int mapId, Vector3 mapCenter, int unitWidth)
    {
        MapBase result = null;

        if (!isLoaded)
        {
            // 加载文件
            mapDataDic = Utils.DepartFileData(Utils.LoadFileRotate(MapDataFilePath));

            if (mapDataDic == null)
            {
                Debug.LogError("加载失败");
            }
            else
            {
                isLoaded = true;
                mapWHDic = Utils.GetDateWH(mapDataDic);
            }
        }
        if (mapId > 0)
        {
            // 加载地图数据
            var whVector = mapWHDic[Utils.GetMapFileNameById(mapId, 1)];
            result = new MapBase((int)whVector.x, (int)whVector.y, mapCenter, unitWidth);
            for (var level = 1; level <= LoadMapLevelCount; level++)
            {
                // 从缓存中查找, 如果缓存中不存在, 则从文件中加载
                var key = Utils.GetMapFileNameById(mapId, level);
                if (mapDataDic != null && mapDataDic.ContainsKey(key))
                {
                    var mapData = mapDataDic[key];

                    // 添加地图单元数组
                    var mapCellArray = GetCells(mapData, (UnitType)level, result);
                    result.AddMapCellArray(mapCellArray, mapData, level);
                }
                else
                {
                    Debug.LogError("地图不存在 ID:" + mapId);
                }
            }
            // 添加对应的怪层

            //var monsterMapData = Utils.GetEmptyIntArray(result.MapHeight, result.MapWidth);
            //var monsterMapCellArray = GetCells(monsterMapData, UnitType.Tower);
            //result.AddMapCellArray(monsterMapCellArray, monsterMapData, MapPlayerLayer);

            // 添加对应的Tower层
            var towerMapData = Utils.GetEmptyIntArray(result.MapHeight, result.MapWidth);
            var towerMapCellArray = GetCells(towerMapData, UnitType.Tower);
            result.AddMapCellArray(towerMapCellArray, towerMapData, MapPlayerLayer);
        }

        return result;
    }


    /// <summary>
    /// 转换地图数据为地图单位
    /// </summary>
    /// <param name="mapData">地图数据</param>
    /// <param name="layer">层级</param>
    /// <param name="mapBase">地图基类</param>
    /// <returns></returns>
    private static MapCellBase[,] GetCells(int[][] mapData, UnitType layer, MapBase mapBase = null)
    {
        var height = mapData.Length;
        var width = mapData[0].Length;

        var mapCellDataArray = new MapCellBase[height, width];
        // 遍历内容
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (mapData[i][j] > 0)
                {
                    // 加载模型
                    //var mapCell = UnitFictory.Single.CreateUnit<MapCellBase>(layer, mapData[i][j]);
                    //mapCell.X = j;
                    //mapCell.Y = i;
                    //if (mapBase != null)
                    //{
                    //    mapCell.Draw(mapBase.Leftdown, mapBase.UnitWidth);
                    //}
                    // 根据数据加载
                    //mapCellDataArray[i, j] = mapCell;
                }
            }
        }

        return mapCellDataArray;
    }

}