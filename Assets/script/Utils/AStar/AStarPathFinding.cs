using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A*寻路
/// </summary>
public class AStarPathFinding
{


    public static int[][] closePathMap = null;



    /// <summary>
    /// 无障碍
    /// </summary>
    public const int Accessibility = 0;

    /// <summary>
    /// 障碍物
    /// </summary>
    public const int Obstacle = 1;

    /// <summary>
    /// 已打开
    /// </summary>
    public const int Opened = -1;

    /// <summary>
    /// 已关闭
    /// </summary>
    public const int Closed = -1;

    /// <summary>
    /// 队员
    /// </summary>
    public const int Member = 2;


    /// <summary>
    /// 节点对比方法
    /// </summary>
    private static Func<Node, Node, int> compareTo = (item1, item2) =>
    {
        var f1 = item1.F;
        var f2 = item2.F;
        if (f1 < f2)
        {
            return -1;
        }
        else if (f1 > f2)
        {
            return 1;
        }
        return 0;
    };

    /// <summary>
    /// 寻找路径
    /// </summary>
    /// <param name="map">地图数组</param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <param name="diameterX">物体X轴宽度</param>
    /// <param name="diameterY">物体Y轴宽度</param>
    /// <param name="isJumpPoint">是否为跳跃式点, 如果为true 则路径只会给出拐点处的关键点</param>
    /// <param name="completeCallback">结束回调函数</param>
    /// <returns>返回路径点列表, 如果列表长度为0则没有路径</returns>
    public static IList<Node> SearchRoad(int[][] map, int startX, int startY, int endX, int endY, int diameterX, int diameterY,
        bool isJumpPoint = false, Action completeCallback = null)
    {

        // 结束节点
        Node endNode = null;
        //var now = Time.realtimeSinceStartup;
        // 行列数量
        var rowCount = map.Length;
        var colCount = map[0].Length;

        // 柔化边缘寻路
        endX = FormatPoint(endX, diameterX, colCount);
        endY = FormatPoint(endY, diameterY, rowCount);
        startX = FormatPoint(startX, diameterX, colCount);
        startY = FormatPoint(startY, diameterY, rowCount);
        // 获取可行进目标点
        var targetNode = GetFormatTarget(map, endX, endY, diameterX, diameterY);
        if (targetNode == null)
        {
            return null;
        }
        endX = targetNode.X;
        endY = targetNode.Y;

        // 复制数据, 用于标记关闭列表
        Utils.CopyArray(map,out closePathMap, rowCount, colCount);
        // 二叉堆open列表
        var openBHList = new BinaryHeapList<Node>(compareTo, rowCount, colCount);
        // 起始节点
        var startNode = new Node(startX, startY);
        // 初始化开始节点
        openBHList.Push(startNode);
        // 如果搜索次数大于(w+h) * 4 则停止搜索
        var maxSearchCount = (rowCount + colCount) * 40;

        var counter = 0;
        // 中间变量定义
        // 寻路G值
        float g;
        // 当前被搜索节点
        Node currentPoint;
        
        do
        {
            counter++;
            // 获取最小节点
            currentPoint = openBHList.Pop();
            // 找到路径
            if (currentPoint.X == endX && currentPoint.Y == endY)
            {
                endNode = currentPoint;
                break;
            }

            // 关闭节点
            if (closePathMap[currentPoint.Y][currentPoint.X] == Accessibility)
            {
                closePathMap[currentPoint.Y][currentPoint.X] = Closed;
            }
            // 获取当前节点周围的节点
            // TODO 预加载 刷新地图(多层地图)
            if (currentPoint.Surround == null)
            {
                currentPoint.Surround = SurroundPoint(currentPoint);
            }

            for (var i = 0; i < currentPoint.Surround.Length; i++)
            {
                var surroundPoint = currentPoint.Surround[i];
                // 斜向是否可移动
                // 判断周围节点合理性
                if (!ValidPos(surroundPoint.X, surroundPoint.Y, colCount, rowCount) ||
                    closePathMap[surroundPoint.Y][surroundPoint.X] == Closed ||
                    !IsPassable(map, surroundPoint, diameterX, diameterY))
                {
                    continue;
                }
                // 计算G值 上下左右为10, 四角为14
                g = currentPoint.G + (((currentPoint.X - endX) * (currentPoint.Y - endY)) == 0 ? 1 : 1.414f);

                // 该点是否在开启列表中
                var node = openBHList.OpenArray[surroundPoint.Y][surroundPoint.X];
                if (node == null)
                {
                    // 两点直线最近, 并且靠近障碍物的路线更昂贵
                    var xOff = (endX - surroundPoint.X);
                    var yOff = (endY - surroundPoint.Y);
                    // 求直线斜率(为了让路径更符合人类行为)
                    //k = (yOff / (float)xOff);

                    //var rate = k < 0.5 ? 0.1f : 10f;
                    //Debug.Log(k);
                    //var angle = Vector2.Angle(new Vector2(xOff, yOff), Vector2.right);
                    //var x2Line = Math.Abs(Math.Cos(angle * Utils.AngleToPi));
                    //var y2Line = Math.Abs(Math.Sin(angle * Utils.AngleToPi));
                    //Debug.Log(k + ", " + xOff + ", " + yOff);

                    // 欧几里得启发
                    //(float)Math.Sqrt(xOff * xOff / y2Line + yOff * yOff / x2Line)
                    // 曼哈顿启发
                    //(Math.Abs(xOff) + Math.Abs(yOff)) 
                    surroundPoint.H = (float)Math.Sqrt(xOff * xOff + yOff * yOff) * 4 + (NearObstacleCount(surroundPoint, map, colCount, rowCount) << 3);
                    surroundPoint.G = g;
                    surroundPoint.F = surroundPoint.H + surroundPoint.G;
                    surroundPoint.Parent = currentPoint;
                    openBHList.Push(surroundPoint);
                }
                else // 存在于开启列表, 比较当前的G值与之前的G值大小
                {
                    if (g < node.G)
                    {
                        node.Parent = currentPoint;
                        node.G = g;
                        node.F = g + node.H;
                    }
                    else if (node.Parent == null)
                    {
                        Debug.LogError("没设置父级");
                    }
                }
            }


            // 如果开放列表为空, 则没有通路
            if (openBHList.Count == 0)
            {
                break;
            }

            // 如果搜索次数大于(w+h) * 4 则停止搜索
            if (counter > maxSearchCount)
            {
                //openList = null;
                Debug.Log("无可行路径");
                // TODO 行进到最近能到的位置
                break;
            }
        } while (true);
        
        IList<Node> path = new List<Node>();
        // 如果有可行路径
        if (endNode != null)
        {
            // 将路径回退并放入列表
            var currentNode = endNode;
            path.Add(currentNode);
            while (currentNode.X != startX || currentNode.Y != startY)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
        }
        else
        {
            Debug.Log("无路径, 寻路次数:" + counter);
        }

        // 处理跳点路径
        if (isJumpPoint && path.Count > 0)
        {
            // 如果路径方向发生变化则变化的奇点为拐点放入列表
            var jumpPointPath = new List<Node>();
            var startPoint = path[0];
            jumpPointPath.Add(startPoint);
            var xOff = startPoint.X;
            var yOff = startPoint.Y;
            foreach (var pathPoint in path)
            {
                // 不是第一个元素
                if (pathPoint.Parent == null)
                {
                    continue;
                }

                // x,y差值是否与上一次相同, 不相同则保存拐点
                var nowXOff = pathPoint.X - pathPoint.Parent.X;
                var nowYOff = pathPoint.Y - pathPoint.Parent.Y;
                if (nowXOff != xOff || nowYOff != yOff)
                {
                    xOff = nowXOff;
                    yOff = nowYOff;
                    jumpPointPath.Add(pathPoint);
                }
            }
            path.Clear();
            path = jumpPointPath;

        }

        if (completeCallback != null)
        {
            completeCallback();
        }

        //Debug.Log(string.Format("{0:#.##########}", Time.realtimeSinceStartup - now));
        // 返回路径, 如果路径数量为0 则没有可行路径
        return path;
    }

    /// <summary>
    /// 携程寻路, 可以查看寻路过程
    /// </summary>
    /// <param name="map"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <param name="diameterX"></param>
    /// <param name="diameterY"></param>
    /// <returns></returns>
    public static IEnumerator ISearchRoad(int[][] map, int startX, int startY, int endX, int endY, int diameterX, int diameterY)
    {

        // 结束节点
        Node endNode = null;
        // 行列数量
        var rowCount = map.Length;
        var colCount = map[0].Length;
        var halfDiameterX = diameterX/2;
        var halfDiameterY = diameterY/2;

        // 柔化边缘寻路
        if (endX + halfDiameterX >= colCount)
        {
            endX = colCount - halfDiameterX - 1;
        }
        if (endX - halfDiameterX <= 0)
        {
            endX = halfDiameterX + 1;
        }
        if (endY + halfDiameterY >= rowCount)
        {
            endY = rowCount - halfDiameterY - 1;
        }
        if (endY - diameterY <= 0)
        {
            endY = diameterY + 1;
        }

        // 复制数据, 用于标记关闭列表
        Utils.CopyArray(map, out closePathMap, rowCount, colCount);
        // 二叉堆open列表
        var openBHList = new BinaryHeapList<Node>(compareTo, rowCount, colCount);
        // 初始化开始节点
        openBHList.Push(new Node(startX, startY));
        // 如果搜索次数大于(w+h) * 4 则停止搜索
        var maxSearchCount = (rowCount + colCount) * 40;

        // 计算结束偏移
        endX = endX - diameterX / 2;
        endY = endY - diameterY / 2;


        var counter = 0;

        // 寻路G值
        float g;
        // 当前被搜索节点
        Node currentPoint;
        // 开放节点值
        Node node = null;
        // 当前节点与目标点x,y差
        int xOff = 0;
        int yOff = 0;
        // 当前路线斜率
        float k = 0;

        do
        {
            counter++;
            // 获取最小节点
            currentPoint = openBHList.Pop();
            // 找到路径
            if (currentPoint.X == endX && currentPoint.Y == endY)
            {
                endNode = currentPoint;
                break;
            }

            // 关闭节点
            if (closePathMap[currentPoint.Y][currentPoint.X] == Accessibility)
            {
                closePathMap[currentPoint.Y][currentPoint.X] = Closed;
            }
            // 获取当前节点周围的节点
            // TODO 预加载 刷新地图(多层地图)
            if (currentPoint.Surround == null)
            {
                currentPoint.Surround = SurroundPoint(currentPoint);
            }

            foreach (Node surroundPoint in currentPoint.Surround)
            {
                 // 斜向是否可移动
                // 判断周围节点合理性
                if (!ValidPos(surroundPoint.X, surroundPoint.Y, colCount, rowCount) ||
                    closePathMap[surroundPoint.Y][surroundPoint.X] == Closed ||
                    !IsPassable(map, surroundPoint, diameterX, diameterY))
                {
                    continue;
                }
                // 计算G值 上下左右为10, 四角为14
                g = currentPoint.G + (((currentPoint.X - endX) * (currentPoint.Y - endY)) == 0 ? 1 : 1.414f);

                // 该点是否在开启列表中
                node = openBHList.OpenArray[surroundPoint.Y][surroundPoint.X];
                if (node == null)
                {

                    // 两点直线最近, 并且靠近障碍物的路线更昂贵
                    xOff = (endX - surroundPoint.X);
                    yOff = (endY - surroundPoint.Y);
                    // 求直线斜率(为了让路径更符合人类行为)
                    //k = (yOff / (float)xOff);

                    //var rate = k < 0.5 ? 0.1f : 10f;
                    //Debug.Log(k);
                    //var angle = Vector2.Angle(new Vector2(xOff, yOff), Vector2.right);
                    //var x2Line = Math.Abs(Math.Cos(angle * Utils.AngleToPi));
                    //var y2Line = Math.Abs(Math.Sin(angle * Utils.AngleToPi));
                    //Debug.Log(k + ", " + xOff + ", " + yOff);

                    //(float)Math.Sqrt(xOff * xOff / y2Line + yOff * yOff / x2Line)
                    surroundPoint.H = (Math.Abs(xOff) + Math.Abs(yOff)) * 8 + (NearObstacleCount(surroundPoint, map, colCount, rowCount) * 8);
                    surroundPoint.G = g;
                    surroundPoint.F = surroundPoint.H + surroundPoint.G;
                    surroundPoint.Parent = currentPoint;
                    openBHList.Push(surroundPoint);
                }
                else // 存在于开启列表, 比较当前的G值与之前的G值大小
                {
                    if (g < node.G)
                    {
                        node.Parent = currentPoint;
                        node.G = g;
                        node.F = g + node.H;
                    }
                }
            }


            // 如果开放列表为空, 则没有通路
            if (openBHList.Count == 0)
            {
                break;
            }

            // 如果搜索次数大于(w+h) * 4 则停止搜索
            if (counter > maxSearchCount)
            {
                //openList = null;
                Debug.Log("无可行路径");
                // TODO 行进到最近能到的位置
                break;
            }
            yield return new WaitForEndOfFrame();
        } while (true);

        IList<Node> path = new List<Node>();
        // 如果有可行路径
        if (endNode != null)
        {
            // 将路径回退并放入列表
            var currentNode = endNode;
            do
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            } while (currentNode.X != startX || currentNode.Y != startY);
        }
        else
        {
            Debug.Log("无路径, 寻路次数:" + counter);
        }
        yield break;
    }

    /// <summary>
    /// 过得可行进目标点
    /// </summary>
    /// <param name="map">地图数据</param>
    /// <param name="targetX">目标点X</param>
    /// <param name="targetY">目标点Y</param>
    /// <param name="diameterX">单位宽度X</param>
    /// <param name="diameterY">单位高度Y</param>
    /// <returns></returns>
    private static Node GetFormatTarget(int[][] map, int targetX, int targetY, int diameterX, int diameterY)
    {
        var targetNode = new Node(targetX, targetY);
        var counterForFindTarget = 0;
        // 验证起始节点 如果该节点不可达则朝向xy两轴正负方向寻找可前进点
        while (!IsPassable(map, targetNode, diameterX, diameterY))
        {
            var num = counterForFindTarget % 4;
            switch (num)
            {
                case 0:
                    targetNode.X = targetX + counterForFindTarget / 4 + 1;
                    break;
                case 1:
                    targetNode.X = targetX - counterForFindTarget / 4 + 1;
                    break;
                case 2:
                    targetNode.Y = targetY + counterForFindTarget / 4 + 1;
                    break;
                case 3:
                    targetNode.Y = targetY + counterForFindTarget / 4 + 1;
                    break;
            }
            //Debug.Log(targetNode);
            counterForFindTarget++;
            if (counterForFindTarget > 100)
            {
                Debug.LogError("没有路径");
                return null;
            }
        }
        return targetNode;
    }

    /// <summary>
    /// 当前位置是否可以通过
    /// </summary>
    /// <param name="computeMap">地图</param>
    /// <param name="nowNode">当前位置</param>
    /// <param name="diameterX">移动物体X轴高度</param>
    /// <param name="diameterY">移动物体Y轴高度</param>
    /// <returns></returns>
    private static bool IsPassable(int[][] computeMap, Node nowNode, int diameterX, int diameterY)
    {
        //var now = Time.realtimeSinceStartup;
        // 定义 物体位置为左上角(主要指直径大于1的)
        // 验证参数是否合法
        if (diameterX <= 0 || diameterY < 0 || computeMap == null || nowNode == null)
        {
            return false;
        }
        var x = nowNode.X;
        var y = nowNode.Y;
        var halfX = (int)(diameterX * 0.5f);
        var halfY = (int)(diameterY * 0.5f);
        // 只有一点, 判断当前点
        if (halfX == 0 || halfY == 0)
        {
            if (x >= computeMap[0].Length ||
                y >= computeMap.Length ||
                computeMap[y][x] > 0) //== Obstacle)
            {
                return false;
            }
        }
        else
        {
            // TODO 优化方案 差值判断不同区域, 重复区域忽略
            // 遍历直径内的点
            // 优化, 中间忽略, 只判断外圈
            for (var i = -halfX; i < halfX; i++)
            {
                for (var j = -halfY; j < halfY; j++)
                {
                    if (i > 0 && i < diameterX - 1 && j > 0 && j < diameterY - 1)
                    {
                        continue;
                    }
                    var computeX = x + i;
                    var computeY = x + j;
                    // 判断点的位置是否合法
                    // 判断斜向运动时
                    if (computeX < 0 ||
                        computeY < 0 ||
                        computeX >= computeMap[0].Length ||
                        computeY >= computeMap.Length ||
                        computeMap[computeY][computeX] > 0) //== Obstacle)
                    {
                        return false;
                    }
                }
            }
        }


        //Debug.Log(string.Format("{0:#.#######}",Time.realtimeSinceStartup - now));

        return true;
    }

    /// <summary>
    /// 检查节点中是否有障碍物
    /// </summary>
    /// <param name="node">被检查节点</param>
    /// <param name="map">地图信息</param>
    /// <param name="colCount"></param>
    /// <param name="rowCount"></param>
    /// <returns></returns>
    public static int NearObstacleCount(Node node, int[][] map, int colCount, int rowCount)
    {
        var result = 0;
        if (node != null && map != null && ValidPos(node.X, node.Y, colCount, rowCount))
        {
            if (ValidPos(node.X + 1, node.Y, colCount, rowCount) && map[node.Y][node.X + 1] > 0) // == Obstacle)
            {
                result++;
            }
            if (ValidPos(node.X, node.Y + 1, colCount, rowCount) && map[node.Y + 1][node.X] > 0) // == Obstacle)
            {
                result++;
            }
            if (ValidPos(node.X - 1, node.Y, colCount, rowCount) && map[node.Y][node.X - 1] > 0) // == Obstacle)
            {
                result++;
            }
            if (ValidPos(node.X, node.Y - 1, colCount, rowCount) && map[node.Y - 1][node.X] > 0) // == Obstacle)
            {
                result++;
            }
        }
        return result;
    }

    /// <summary>
    /// 获取当前点周围的点
    /// </summary>
    /// <param name="curPoint">当前点</param>
    /// <returns>周围节点的数组</returns>
    private static Node[] SurroundPoint(Node curPoint)
    {
        var x = curPoint.X;
        var y = curPoint.Y;
        return new[]
        {
            //new Node(x - 1, y - 1),
            new Node(x, y - 1),
            //new Node(x + 1, y - 1),
            new Node(x + 1, y),
            //new Node(x + 1, y + 1),
            new Node(x, y + 1),
            //new Node(x - 1, y + 1),
            new Node(x - 1, y)
        };
    }

    /// <summary>
    /// 在字典中获取节点
    /// </summary>
    /// <param name="x">位置x</param>
    /// <param name="y">位置y</param>
    /// <param name="nodeDic">目标字典</param>
    /// <returns>如果存在则返回该节点, 否则返回null</returns>
    private static Node ExistInList(long x, long y, IDictionary<long, Node> nodeDic)
    {
        Node result = null;
        if (x >= 0 && y >= 0 && nodeDic != null)
        {
            long key = Utils.GetKey(x, y);
            if (nodeDic.ContainsKey(key))
            {
                result = nodeDic[key];
            }
        }
        return result;
    }

    /// <summary>
    /// 验证节点位置是否有效
    /// </summary>
    /// <param name="x">位置x</param>
    /// <param name="y">位置y</param>
    /// <param name="mapWidth">地图宽度</param>
    /// <param name="mapHeight">地图高度</param>
    /// <returns>是否有效</returns>
    private static bool ValidPos(int x, int y, int mapWidth, int mapHeight)
    {
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 格式化地图上点
    /// </summary>
    /// <param name="val">被格式化值</param>
    /// <param name="diameter">直径</param>
    /// <param name="max">最大值</param>
    /// <returns>被格式化值</returns>
    private static int FormatPoint(int val, int diameter, int max)
    {
        var result = val;
        if (result + diameter >= max)
        {
            result = max - diameter;
        }
        if (result - diameter <= 0)
        {
            result = diameter >> 1;
        }
        return result;
    }


}

/// <summary>
/// 二叉堆列表
/// </summary>
public class BinaryHeapList<T> where T : Node
{

    /// <summary>
    /// 当前节点值
    /// </summary>
    public T Value { get; private set; }

    /// <summary>
    /// 当前子树的值数量
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// open列表
    /// </summary>
    public Node[][] OpenArray
    {
        get { return openPath; }
    }
    
    /// <summary>
    /// 对比方法
    /// </summary>
    private Func<T, T, int> compTo = null;

    /// <summary>
    /// 单位数组, 用于二叉堆存储
    /// </summary>
    private T[] itemArray = null;

    /// <summary>
    /// 数组位置
    /// </summary>
    private int arrayPos = 0;

    /// <summary>
    /// open列表
    /// </summary>
    private Node[][] openPath = null;

    /// <summary>
    /// 行数
    /// </summary>
    private int rowCount = 0;

    /// <summary>
    /// 列数
    /// </summary>
    private int colCount = 0;

    /// <summary>
    /// 存储长度
    /// </summary>
    private int bufferLength = 1024;


    /// <summary>
    /// 初始化二叉堆列表
    /// </summary>
    /// <param name="compTo">对比大小方法, arg1>arg2返回-1, 反之返回1, 相等返回0</param>
    /// <param name="isOriginal">是否为源</param>
    /// <param name="rowCount">行数</param>
    /// <param name="colCount">列数</param>
    public BinaryHeapList(Func<T, T, int> compTo, int rowCount, int colCount)
    {
        this.compTo = compTo;
        itemArray = new T[bufferLength];
        // 初始化open列表
        openPath = new Node[rowCount][];
        for (var i = 0; i < rowCount; i++)
        {
            openPath[i] = new Node[colCount];
        }
    }

    /// <summary>
    /// 将单位加入列表, 并根据对比方法将其放到合适位置
    /// </summary>
    /// <param name="item">放入单位</param>
    public void Push(T item)
    {
        var localItem = item;

        if (localItem == null)
        {
            return;
        }

        // 将值插入列表最后位置
        itemArray[arrayPos] = item;
        // 然后向上调整数组
        FilterUp(arrayPos);
        
        arrayPos++;
        Count++;
        // 判断是否空间足够, 不足则重新分配空间
        if (bufferLength - arrayPos <= 1)
        {
            // 缓存体积 * 2
            bufferLength = bufferLength << 1;
            var tmpArray = new T[bufferLength];
            Utils.CopyArray(itemArray, tmpArray, arrayPos);
            itemArray = tmpArray;
            //tmpArray = null;
        }
        // 加入单元列表
        openPath[item.Y][item.X] = item;
    }

    /// <summary>
    /// 获取按照对比方法最大值并从列表中删除
    /// </summary>
    /// <returns>最大值</returns>
    public T Pop()
    {

        // 将数组最上位置弹出, 并将数组最后一位放到第一位
        var lastPos = arrayPos - 1;
        T result = itemArray[0];
        itemArray[0] = itemArray[lastPos];
        // 然后调整数组
        FileterDown(0, lastPos);

        // 从单位列表中删除该单位
        openPath[result.Y][result.X] = null;
        Count--;
        arrayPos--;
        return result;
    }


    /// <summary>
    /// 向上调整数组
    /// </summary>
    /// <param name="start">开始调整位置</param>
    private void FilterUp(int start)
    {
        //var current = start;
        // 获取父节点位置
        var parent = (start - 1) >> 1;
        // 当前节点值
        var item = itemArray[start];
        while (start > 0)
        {
            // 对比当前节点与父节点大小
            if (compTo(itemArray[parent], item) < 0)
            {
                // 如果当前节点值(判断)大于父节点则退出循环
                break;
            }
            else
            {
                // 节点上移
                itemArray[start] = itemArray[parent];
                start = parent;
                parent = (parent - 1) >> 1;
            }
        }

        itemArray[start] = item;
    }

    /// <summary>
    /// 向下调整数组
    /// </summary>
    /// <param name="start">调整开始位置</param>
    /// <param name="end">调整结束位置</param>
    private void FileterDown(int start, int end)
    {
        // 当前节点
        //var current = start;
        // 左子节点
        var left = (start << 1) + 1;
        // 当前节点值
        var item = itemArray[start];

        while (left <= end)
        {
            if (left < end && compTo(itemArray[left], itemArray[left + 1]) > 0)
            {
                // 取比较大的子节点
                left++;
            }
            if (compTo(itemArray[left], item) > 0)
            {
                // 如果当前节点值(判断)大于子节点则退出
                break;
            }
            // 节点下移
            itemArray[start] = itemArray[left];
            start = left;
            left = (left << 1) + 1;
        }
        itemArray[start] = item;
    }
}