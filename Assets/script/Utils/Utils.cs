
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// 工具类
/// </summary>
public class Utils
{


    /// <summary>
    /// 敌方阵营
    /// </summary>
    public const int EnemyCamp = 2;

    /// <summary>
    /// 我方阵营
    /// </summary>
    public const int MyCamp = 1;

    /// <summary>
    /// 角度转π
    /// </summary>
    public const float AngleToPi = 0.0174532925199433f;

    /// <summary>
    /// π转角度
    /// </summary>
    public const float PiToAngle = 57.2957795130823f;
    /// <summary>
    /// 趋近0值
    /// </summary>
    public static readonly float ApproachZero = 0.00001f;

    /// <summary>
    /// 负趋近0值
    /// </summary>
    public static readonly float ApproachKZero = -0.00001f;

    /// <summary>
    /// 中立阵营
    /// </summary>
    public const int NeutralCamp = 0;

    /// <summary>
    /// DateTime的Ticks时间转换为秒
    /// </summary>
    public const long TicksTimeToSecond = 10000000;

    /// <summary>
    /// 单位大类型-地面单位
    /// </summary>
    public const short GeneralTypeSurface = 1;

    /// <summary>
    /// 单位大类型-空中单位
    /// </summary>
    public const short GeneralTypeAir = 2;

    /// <summary>
    /// 单位大类型-建筑
    /// </summary>
    public const short GeneralTypeBuilding = 3;

    /// <summary>
    /// 选择目标点已选择X
    /// </summary>
    public const string TargetPointSelectorXKey = "SelectedX";

    /// <summary>
    /// 选择目标点已选择Y
    /// </summary>
    public const string TargetPointSelectorYKey = "SelectedY";

    /// <summary>
    /// 特效显示层
    /// </summary>
    public const int EffectLayer = 12;

    /// <summary>
    /// 子弹类型-普通类型
    /// </summary>
    public const int BulletTypeNormal = 1;

    /// <summary>
    /// 子弹类型-其他类型
    /// </summary>
    public const int BulletTypeScope = 2;

    // 1.AOE范围中心点为目标单位、2.AOE范围中心点为发动攻击时目标的座标位置、3.AOE范围中心点为自身、4.AOE范围中心在自身前方
    /// <summary>
    /// 目标对象位置(跟随,圆形)
    /// </summary>
    public const int AOEObjScope = 1;

    /// <summary>
    /// 目标位置(圆形)
    /// </summary>
    public const int AOEPointScope = 2;

    /// <summary>
    /// 自己位置周围(圆形, 扇形)
    /// </summary>
    public const int AOEScope = 3;

    /// <summary>
    /// 自身前方(矩形, 扇形)
    /// </summary>
    public const int AOEForwardScope = 4;





    /// <summary>
    /// 地图文件名称Head
    /// </summary>
    private const string MapNameHead = "MapInfo";

    /// <summary>
    /// 地图文件名称Level部分
    /// </summary>
    private const string MapNameLevel = "_Level";


    //-------------------------静态方法--------------------------

    /// <summary>
    /// 位置转行列
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="position">当前在plane上的位置(区间, 比如0-1 为同一个位置)</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>map中的行列位置 0位x 1位y</returns>
    public static int[] PositionToNum(Vector3 planePosOffset, Vector3 position, float unitWidth, float mapWidth, float mapHight)
    {
        var x = (int)((position.x - planePosOffset.x + mapWidth * unitWidth * 0.5f) / unitWidth);
        var y = (int)((position.z - planePosOffset.z + mapHight * unitWidth * 0.5f) / unitWidth);
        if (x < 0)
        {
            x = 0;
        }
        if (x >= mapWidth)
        {
            x = (int)mapWidth - 1;
        }
        if (y < 0)
        {
            y = 0;
        }
        if (y >= mapHight)
        {
            y = (int)mapHight - 1;
        }
        return new[] { x, y };
    }


    /// <summary>
    /// 行列转位置
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="num">map中的行列位置</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>当前plane对应位置, 固定位置的中心点</returns>
    public static Vector3 NumToPositionV(Vector3 planePosOffset, Vector2 num, float unitWidth, float mapWidth, float mapHight)
    {
        // 中心位置为基准
        var result = new Vector3(
            planePosOffset.x - mapWidth * unitWidth * 0.5f
            + num.x * unitWidth
            + unitWidth * 0.5f, // 统一中心坐标系
            planePosOffset.y,
            planePosOffset.z - mapHight * unitWidth * 0.5f
            + num.y * unitWidth
            + unitWidth * 0.5f); // 统一中心坐标系

        return result;
    }

    /// <summary>
    /// 行列转位置
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="num">map中的行列位置</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>当前plane对应位置, 固定位置的中心点</returns>
    public static Vector3 NumToPositionH(Vector3 planePosOffset, Vector2 num, float unitWidth, float mapWidth, float mapHight)
    {
        var result = new Vector3(
            planePosOffset.x - mapWidth * unitWidth * 0.5f
            + num.x * unitWidth
            + unitWidth * 0.5f, // 统一中心坐标系
            planePosOffset.y - mapHight * unitWidth * 0.5f
            + num.y * unitWidth
            + unitWidth * 0.5f, // 统一中心坐标系
            planePosOffset.z);

        return result;
    }

    /// <summary>
    /// 行列位置转换列表
    /// </summary>
    /// <param name="planePosOffset">地图底板位置偏移</param>
    /// <param name="nodeList">map中的行列位置列表</param>
    /// <param name="unitWidth">单位宽度</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHight">地图高度</param>
    /// <returns>转换后位置列表</returns>
    public static List<Vector3> NumToPostionByList(Vector3 planePosOffset, IList<Node> nodeList, float unitWidth, float mapWidth, float mapHight)
    {
        List<Vector3> result = null;
        if (nodeList != null && nodeList.Count > 0)
        {
            result = new List<Vector3>();
            foreach (var node in nodeList)
            {
                result.Add(NumToPositionH(planePosOffset, new Vector2(node.X, node.Y), unitWidth, mapWidth, mapHight));
            }
        }
        return result;
    }


    /// <summary>
    /// 2转3
    /// </summary>
    /// <param name="vec2"></param>
    /// <returns></returns>
    public static Vector3 V2ToV3(Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y);
    }

    /// <summary>
    /// 获取node的key值
    /// </summary>
    /// <param name="x">位置x</param>
    /// <param name="y">位置y</param>
    /// <returns>key值</returns>
    public static long GetKey(long x, long y)
    {
        var result = (x << 32) + y;
        return result;
    }



    /// <summary>
    /// 表达式树转换字符串对比数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="op"></param>
    /// <returns></returns>
    public static Expression<Func<T, T, bool>> Generate<T>(string op)
    {
        ParameterExpression x = Expression.Parameter(typeof(T), "x");
        ParameterExpression y = Expression.Parameter(typeof(T), "y");
        return Expression.Lambda<Func<T, T, bool>>
        (
            (op.Equals(">")) ? Expression.GreaterThan(x, y) :
                (op.Equals("<")) ? Expression.LessThan(x, y) :
                    (op.Equals(">=")) ? Expression.GreaterThanOrEqual(x, y) :
                        (op.Equals("<=")) ? Expression.LessThanOrEqual(x, y) :
                            (op.Equals("!=")) ? Expression.NotEqual(x, y) :
                                Expression.Equal(x, y),
            x,
            y
        );
    }


    public static Func<T, T, bool> GetCompare<T>(string op)
    {
        return Generate<T>(op).Compile();
    }




    /// <summary>
    /// 计算两个向量角度
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static float GetAngleWithZ(Vector3 vec)
    {
        var result = Vector3.Angle(Vector3.forward, vec);

        result *= (Vector3.Angle(Vector3.right, vec) >= 90 ? -1 : 1);

        return result;
    }

    // ---------------------------文件操作----------------------------

    /// <summary>
    /// 合并文件(字符串文件)
    /// </summary>
    /// <param name="pathList">被合并文件Path列表</param>
    /// <returns>合并后的文件内容</returns>
    public static string CombineFile(List<string> pathList)
    {
        if (pathList == null || pathList.Count == 0)
        {
            return null;
        }
        StringBuilder sb = new StringBuilder();

        foreach (var path in pathList)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                sb.Append(fileInfo.Name);
                sb.Append("%--%");
                sb.Append(LoadFileInfo(fileInfo));
                sb.Append("%---%");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="fileName">文件名</param>
    /// <param name="info">文件内容</param>
    public static void CreateOrOpenFile([NotNull]string path, [NotNull]string fileName, [NotNull]string info)
    {
        StreamWriter sw;
        FileInfo fi = new FileInfo(path + Path.AltDirectorySeparatorChar + fileName);
        sw = fi.CreateText();
        sw.Write(info);
        sw.Close();
    }

    /// <summary>
    /// 生成地图文件名
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <param name="mapLevel">地图层级</param>
    /// <returns>地图文件名</returns>
    public static string GetMapFileNameById(int mapId, int mapLevel)
    {
        return GetMapFileNameById(string.Format("{0:0000}", mapId), mapLevel);
    }

    /// <summary>
    /// 生成地图文件名
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <param name="mapLevel">地图层级</param>
    /// <returns>地图文件名</returns>
    public static string GetMapFileNameById([NotNull]string mapId, int mapLevel)
    {
        return MapNameHead + mapId + MapNameLevel + mapLevel;
    }

    /// <summary>
    /// 获取数据宽高
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Dictionary<string, Vector2> GetDateWH([NotNull]Dictionary<string, int[][]> data)
    {
        var result = new Dictionary<string, Vector2>();

        foreach (var kv in data)
        {
            result.Add(kv.Key, new Vector2(kv.Value[0].Length, kv.Value.Length));
        }

        return result;
    }


    /// <summary>
    /// 分解文件内容
    /// </summary>
    /// <param name="data">被分解数据</param>
    /// <returns>分解后的文件对照表(文件名, 文件内容)</returns>
    public static Dictionary<string, int[][]> DepartFileData([NotNull]string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return null;
        }
        var result = new Dictionary<string, int[][]>();

        // 解析数据
        var filesDataArray = data.Split(new[] { "%---%" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var fileData in filesDataArray)
        {
            var dataDepart = fileData.Split(new[] { "%--%" }, StringSplitOptions.RemoveEmptyEntries);
            if (dataDepart.Length == 2)
            {
                var fileName = dataDepart[0];
                var fileInfo = dataDepart[1];
                result.Add(fileName, DeCodeInfo(fileInfo));
            }
        }
        return result;
    }


    /// <summary>
    /// 解码地图数据
    /// </summary>
    /// <param name="mapInfoJson">地图数据json</param>
    /// <returns>地图数据数组</returns>
    public static int[][] DeCodeInfo([NotNull]string mapInfoJson)
    {
        if (string.IsNullOrEmpty(mapInfoJson))
        {
            return null;
        }

        // 读出数据
        var mapLines = mapInfoJson.Split('\n');

        int[][] mapInfo = new int[mapLines.Length][];
        for (var row = 0; row < mapLines.Length; row++)
        {
            var line = mapLines[row];
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var cells = line.Split(',');
            mapInfo[row] = new int[cells.Length];
            for (int col = 0; col < cells.Length; col++)
            {
                if (string.IsNullOrEmpty(cells[col].Trim()))
                {
                    continue;
                }
                mapInfo[row][col] = int.Parse(cells[col]);
            }
        }

        return mapInfo;
    }



    /// <summary>
    /// 从两个目录中加载文件, 如果persistentDataPath中存在文件则加载, 否则从streamingAssetsPath中加载
    /// </summary>
    /// <param name="path">文件路径(在目录中的结构)</param>
    /// <returns>文件内容</returns>
    public static string LoadFileRotate([NotNull]string path)
    {
        string result = null;
        FileInfo fi = new FileInfo(Path.Combine(Application.persistentDataPath, path));
        if (fi.Exists)
        {
            result = LoadFileInfo(fi);
        }
        else
        {
            fi = new FileInfo(Path.Combine(Application.streamingAssetsPath, path));
            // 继续加载
            if (fi.Exists)
            {
                result = LoadFileInfo(fi);
            }
        }

        return result;
    }



    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>文件内容, 如果文件不存在则返回null</returns>
    public static string LoadFileInfo([NotNull]string path)
    {
        string result = null;

        if (path != null)
        {
            var fi = new FileInfo(path);
            result = LoadFileInfo(fi);
        }

        return result;
    }


    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="fi">文件信息类</param>
    /// <returns>文件内容, 如果文件不存在则返回null</returns>
    public static string LoadFileInfo([NotNull]FileInfo fi)
    {
        string result = null;
        if (fi != null)
        {
            StreamReader sr;
            if (fi.Exists)
            {
                sr = new StreamReader(fi.OpenRead());
                result = sr.ReadToEnd();
                sr.Close();
            }
        }

        return result;
    }
    /// <summary>
    /// 拷贝int二维数组
    /// </summary>
    /// <param name="from">数据源</param>
    /// <param name="to">目标数组</param>
    /// <param name="rowCount">行数</param>
    /// <param name="colCount">列数</param>
    public static void CopyArray([NotNull]int[][] from, out int[][] to, int rowCount, int colCount)
    {
        if (from == null || rowCount <= 0 || colCount <= 0)
        {
            throw new Exception("拷贝数据错误.");
        }
        to = new int[rowCount][];
        for (var i = 0; i < rowCount; i++)
        {
            to[i] = new int[colCount];
            for (var j = 0; j < colCount; j++)
            {
                to[i][j] = from[i][j];
            }
        }
    }

    /// <summary>
    /// 拷贝int一维数组
    /// </summary>
    /// <param name="from">数据源</param>
    /// <param name="to">目标数组</param>
    /// <param name="length">拷贝长度</param>
    public static void CopyArray<T>([NotNull]T[] from, T[] to, int length)
    {
        if (from == null || to == null || length <= 0 || from.Length < length || to.Length < length)
        {
            return;
        }
        for (var i = 0; i < length; i++)
        {
            to[i] = from[i];
        }
    }

    /// <summary>
    /// 创建空int二维数组
    /// </summary>
    /// <param name="h">数组高</param>
    /// <param name="w">数组宽</param>
    /// <param name="val">默认值</param>
    /// <returns>空数组</returns>
    public static int[][] GetEmptyIntArray(int h, int w, int val = 0)
    {
        int[][] result = null;

        if (h > 0 && w > 0)
        {
            result = new int[h][];
            for (var i = 0; i < h; i++)
            {
                result[i] = new int[w];
                for (var j = 0; j < w; j++)
                {
                    result[i][j] = val;
                }
            }
        }

        return result;
    }


    // --------------------------数学-------------------------------

    /// <summary>
    /// 获取列表中最小值
    /// </summary>
    /// <param name="values">数值列表</param>
    /// <returns>最小值</returns>
    public static float MinValue(params float[] values)
    {
        var result = 0f;
        if (values != null || values.Length > 0)
        {
            result = values[0];
            result = values.Concat(new[] { result }).Min();
        }

        return result;
    }




    // ---------------------------图形-------------------------------
    /// <summary>
    /// 计算距离(2D)
    /// </summary>
    /// <param name="x1">位置1x</param>
    /// <param name="y1">位置1y</param>
    /// <param name="x2">位置2x</param>
    /// <param name="y2">位置2y</param>
    /// <returns></returns>
    public static float V2Distance(float x1, float y1, float x2, float y2)
    {
        float tx = x1 - x2;
        float tz = y1 - y2;
        return (float)Math.Sqrt(tx * tx + tz * tz);
    }


    /// <summary>
    /// 求两点间距离2d
    /// </summary>
    /// <param name="x1">点1x</param>
    /// <param name="y1">点1y</param>
    /// <param name="x2">点2x</param>
    /// <param name="y2">点2y</param>
    /// <returns>两点距离</returns>
    public static float GetTwoPointDistance2D(float x1, float y1, float x2, float y2)
    {
        var xOffset = x1 - x2;
        var yOffset = y1 - y2;
        return (float)Math.Sqrt(xOffset * xOffset + yOffset * yOffset);
    }

    ///// <summary>
    ///// 排除Y轴
    ///// </summary>
    ///// <param name="vec3">被排除向量</param>
    ///// <returns>被排除后的数据</returns>
    //public static Vector3 WithOutY(Vector3 vec3)
    //{
    //    return new Vector3(vec3.x, 0, vec3.z);
    //}

    /// <summary>
    /// 排除Z轴
    /// </summary>
    /// <param name="vec3">被排除向量</param>
    /// <returns>被排除后的数据</returns>
    public static Vector3 WithOutZ(Vector3 vec3)
    {
        return new Vector3(vec3.x, vec3.y);
    }

    /// <summary>
    /// 3转2排除Y
    /// </summary>
    /// <param name="vec3"></param>
    /// <returns></returns>
    public static Vector2 V3ToV2WithouY(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }

    /// <summary>
    /// 获取矩形水平检测线
    /// </summary>
    /// <param name="rotation">旋转角度-360-360°</param>
    /// <returns>水平检测线标量</returns>
    public static Vector2 GetHorizonalTestLine(float rotation)
    {
        var angle = rotation * AngleToPi;
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

    /// <summary>
    /// 获取矩形垂直检测线
    /// </summary>
    /// <param name="rotation">旋转角度-360-360°</param>
    /// <param name="radius">检测线长度</param>
    /// <returns>垂直检测线标量</returns>
    public static Vector2 GetVerticalTestLine(float rotation, float radius = 1f)
    {
        var angle = rotation * AngleToPi;
        return new Vector2(-(float)Math.Sin(angle) * radius, (float)Math.Cos(angle) * radius);
    }

    /// <summary>
    /// 获取显示位置
    /// </summary>
    /// <param name="camearPos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static ICollisionGraphics GetShowGraphics(Vector3 camearPos, float width, float height)
    {
        //var cameraPosX = camearPos.x + width * unitWidth * 0.5f - scopeWidth * 0.5f;
        //var cameraPosY = camearPos.y + height * unitWidth * 0.5f - scopeHeight * 0.5f;
        var result = new RectGraphics(new Vector2(camearPos.x, camearPos.y), width, height, 0);
        return result;
    }


    /// <summary>
    /// 绘制图形
    /// </summary>
    /// <param name="graphics">图形对象</param>
    public static void DrawGraphics(ICollisionGraphics graphics, Color color)
    {
        if (!Debug.logger.logEnabled)
        {
            return;
        }
        if (graphics == null)
        {
            return;
        }
        switch (graphics.GraphicType)
        {
            case GraphicType.Circle:
                var circle = graphics as CircleGraphics;
                if (circle != null)
                {
                    DrawCircle(new Vector3(circle.Postion.x, circle.Postion.y), circle.Radius, color);
                }
                break;
            case GraphicType.Rect:
                var rect = graphics as RectGraphics;
                if (rect != null)
                {
                    DrawRect(new Vector3(rect.Postion.x, rect.Postion.y), rect.Width, rect.Height, rect.Rotation, color);
                }
                break;
            case GraphicType.Sector:
                var sector = graphics as SectorGraphics;
                if (sector != null)
                {
                    DrawSector(new Vector3(sector.Postion.x, sector.Postion.y), sector.Radius, sector.Rotation, sector.OpenAngle, color);
                }
                break;
        }
    }

    /// <summary>
    /// 回执矩形
    /// </summary>
    /// <param name="position">举行中心位置</param>
    /// <param name="width">举行宽度</param>
    /// <param name="height">举行高度</param>
    /// <param name="rotation">基于矩形中心旋转角度</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawRect(Vector3 position, float width, float height, float rotation, Color color)
    {
        var angle = rotation * AngleToPi;
        var halfWidth = width * 0.5f;
        var halfHeight = height * 0.5f;

        var sin = (float)(Math.Sin(angle));
        var cos = (float)(Math.Cos(angle));
        var left = (-halfWidth);
        var right = (halfWidth);
        var top = (halfHeight);
        var bottom = (-halfHeight);
        var point1 = new Vector3(left * cos - bottom * sin, left * sin + bottom * cos) + position;
        var point2 = new Vector3(left * cos - top * sin, left * sin + top * cos) + position;
        var point3 = new Vector3(right * cos - top * sin, right * sin + top * cos) + position;
        var point4 = new Vector3(right * cos - bottom * sin, right * sin + bottom * cos) + position;
        Debug.DrawLine(point1, point2, color);
        Debug.DrawLine(point2, point3, color);
        Debug.DrawLine(point3, point4, color);
        Debug.DrawLine(point4, point1, color);
    }

    /// <summary>
    /// 绘制扇形
    /// </summary>
    /// <param name="position">圆心位置</param>
    /// <param name="radius">圆半径</param>
    /// <param name="rotation">扇形旋转角度</param>
    /// <param name="openAngle">扇形开口角度</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawSector(Vector3 position, float radius, float rotation, float openAngle, Color color)
    {

    }

    /// <summary>
    ///  绘制三角形
    /// </summary>
    /// <param name="point1">三角形点1</param>
    /// <param name="point2">三角形点2</param>
    /// <param name="point3">三角形点3</param>
    /// <param name="color">绘制颜色</param>
    public static void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Color color)
    {
        Debug.DrawLine(point1, point2, color);
        Debug.DrawLine(point2, point3, color);
        Debug.DrawLine(point3, point1, color);
    }


    /// <summary>
    /// 绘制圆
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <param name="color">颜色</param>
    public static void DrawCircle(Vector3 position, float radius, Color color)
    {
        // 绘制圆环
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 endPoint = position + new Vector3(x, z);
            if (Math.Abs(theta) < Utils.ApproachZero)
            {
                firstPoint = endPoint;
            }
            else
            {
                Debug.DrawLine(beginPoint, endPoint, color);
            }
            beginPoint = endPoint;
        }

        // 绘制最后一条线段
        Debug.DrawLine(firstPoint, beginPoint, color);

        //Debug.DrawLine(firstPoint, firstPoint + new Vector3(radius, 0, 0), color);
    }



    /// <summary>
    /// 获取本机IP地址
    /// </summary>
    /// <returns>本机IP地址</returns>
    public static string GetLocalIP()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return IpEntry.AddressList[i].ToString();
                }
            }
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}