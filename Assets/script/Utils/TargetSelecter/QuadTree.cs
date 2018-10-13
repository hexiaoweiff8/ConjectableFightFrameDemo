using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 四叉树
/// 用于分割地图
/// 提高碰撞检测效率
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class QuadTree<T> where T : IGraphicsHolder//IGraphical<Rectangle>
{


    /// <summary>
    /// 根节点
    /// </summary>
    public QuadTree<T> root { get; private set; }

    /// <summary>
    /// 当前节点最大单元数量
    /// 如果当前节点为最深节点则不受此数量限制
    /// </summary>
    private int maxItemCount = 10;

    /// <summary>
    /// 最大树深度
    /// </summary>
    private int maxLevel = 4;

    /// <summary>
    /// 当前四叉树所在等级
    /// </summary>
    private int level;

    /// <summary>
    /// 对象列表
    /// </summary>
    private List<T> itemsList;

    /// <summary>
    /// 当前四叉树节点编辑
    /// </summary>
    private RectGraphics rect;

    /// <summary>
    /// 子树节点列表
    /// </summary>
    private QuadTree<T>[] nodes;

    /// <summary>
    /// 节点缓存
    /// </summary>
    private Queue<QuadTree<T>> nodeCache = null;

    /// <summary>
    /// 矩形缓存
    /// </summary>
    private Queue<ICollisionGraphics> rectCache = null;

    /// <summary>
    /// 缓存上次所有节点的位置
    /// </summary>
    private Dictionary<T, Vector2> cachePosition;

    ///// <summary>
    ///// 保存每个单位对应到子节点位置
    ///// </summary>
    //private Dictionary<T, long> itemMap; 


    /// <summary>
    /// 初始化四叉树
    /// </summary>
    /// <param name="level">当前四叉树所在位置</param>
    /// <param name="rect">当前四叉树的位置与宽度大小</param>
    /// <param name="parentNodeCache">节点cache队列</param>
    /// <param name="parentRectCache">图形cache队列</param>
    /// <param name="isRoot">是否为根节点</param>
    public QuadTree(int level, RectGraphics rect, Queue<QuadTree<T>> parentNodeCache = null, Queue<ICollisionGraphics> parentRectCache = null, bool isRoot = true)
    {
        this.level = level;
        itemsList = new List<T>();
        this.rect = rect;
        nodes = new QuadTree<T>[4];
        nodeCache = parentNodeCache ?? new Queue<QuadTree<T>>();
        rectCache = parentRectCache ?? new Queue<ICollisionGraphics>();
        if (isRoot)
        {
            root = this;
            cachePosition = new Dictionary<T, Vector2>();
            //itemMap = new Dictionary<T, long>();
        }
    }


    /// <summary>
    /// 根据rect获取该rect所在的节点
    /// </summary>
    /// <param name="item">对象</param>
    /// <returns>节点编号</returns>
    public int GetIndex(ICollisionGraphics item)
    {
        var result = -1;
        var tmpResult = -1;
        // 获得当前节点rect的中心点
        //var midPointX = this.rect.Postion.x;// + this.rect.Width/2;
        //var midPointY = this.rect.Postion.y;// + this.rect.Height/2;
        
        // 判断如果当前图形边框超出子节点则不放入其中
        var collisionSubNodeCount = 0;
        if (nodes != null && item != null)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                if (node != null && node.GetRectangle().CheckCollision(item))
                {
                    if (collisionSubNodeCount > 0)
                    {
                        break;
                    }
                    tmpResult = i;
                    collisionSubNodeCount++;
                }
            }
            
            if (collisionSubNodeCount == 1)
            {
                // 判断是否超出当前节点外框
                var node = nodes[tmpResult];
                var nodeRect = node.GetRectangle();
                var rect = item.GetExternalRect();
                if (IsInner(item, nodeRect))
                {
                    result = tmpResult;
                }
            }
        }
        
        //var topContians = (item.Postion.y > midPointY); 
        //var bottomContians = (item.Postion.y < midPointY);

        //if (item.Postion.x < midPointX)
        //{
        //    if (topContians)
        //    {
        //        // 左上角
        //        result = 3;
        //    }
        //    else if (bottomContians)
        //    {
        //        // 左下角
        //        result = 2;
        //    }
        //}
        //else if (item.Postion.x > midPointX)
        //{
        //    if (topContians)
        //    {
        //        // 右上角
        //        result = 0;
        //    }
        //    else if (bottomContians)
        //    {
        //        // 右下角
        //        result = 1;
        //    }
        //}

        return result;
    }
    
    /// <summary>
    /// 插入对象
    /// </summary>
    /// <param name="item">对象</param>
    public void Insert(T item)
    {
        // 有子节点
        if (nodes[0] != null)
        {
            if (InsertToSubNode(item, nodes))
            {
                return;
            }
        }
        
        itemsList.Add(item);
        // 判断是否item数量大于maxCount, 并且level小于maxLevel
        if (itemsList.Count > maxItemCount && level < maxLevel)
        {
            // 大于则创建子节点
            if (nodes[0] == null)
            {
                Split();
            }
            // 将节点挨个加入子节点, 不能放入子节点的继续保留
            for (var i = 0; i < itemsList.Count; i++)
            {
                var tmpItem = itemsList[i];
                // TODO 这种方式在插入失败时会一直调用造成性能问题
                if (InsertToSubNode(tmpItem, nodes))
                {
                    // 从当前列表中删除该节点
                    itemsList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    /// <summary>
    /// 插入列表
    /// </summary>
    /// <param name="list">对向列表</param>
    public void Insert(IList<T> list)
    {
        foreach (var item in list)
        {
            Insert(item);
        }
    }

    ///// <summary>
    ///// 删除单位
    ///// </summary>
    ///// <param name="t">被删除单位</param>
    //public void Remove(T t)
    //{
    //    if (t == null)
    //    {
    //        return;
    //    }
    //    // 列表中不存在单位
    //    if (!root.itemMap.ContainsKey(t))
    //    {
    //        return;
    //    }
    //    // 获取四叉树中子节点位置
    //    QuadTree<T> node = this;
    //    var pos = itemMap[t];
    //    long nowPos = 0;
    //    while ((nowPos = pos % 8) < 4)
    //    {
            
    //    }
    //}

    /// <summary>
    /// 返回传入对象可能会有碰撞的对向列表
    /// TODO 分块交界处会有问题
    /// </summary>
    /// <param name="rectangle">碰撞对象</param>
    /// <returns>可能碰撞的列表, 对量性质: 在传入rect所在的最底层自己点的对量+其上各个父级的边缘节点</returns>
    public IList<T> Retrieve(ICollisionGraphics rectangle)
    {
        var result = new List<T>();

        var index = GetIndex(rectangle);
        // 如果未在子节点则从当前节点返回所有对象
        if (index != -1 && nodes[0] != null)
        {
            result.AddRange(nodes[index].Retrieve(rectangle));
        }

        result.AddRange(itemsList);

        return result;
    }


    /// <summary>
    /// 按照矩形返回获取范围内对向列表
    /// TODO 优化
    /// </summary>
    /// <param name="scopeRect">范围rect</param>
    /// <returns></returns>
    public IList<T> GetScope(ICollisionGraphics scopeRect)
    {
        List<T> result = null;
        // 判断与当前四叉树的相交
        if (scopeRect != null)
        {
            result = new List<T>();
            T tmpItem;
            // 遍历当前节点中的对象列表, 是否有相交
            for (var i = 0; i < itemsList.Count; i++)
            {
                tmpItem = itemsList[i];
                if (tmpItem.MyCollisionGraphics.CheckCollision(scopeRect))
                {
                    result.Add(tmpItem);
                }
            }
            if (rect.CheckCollision(scopeRect))
            {
                if (nodes[0] != null)
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (nodes[i].rect.CheckCollision(scopeRect))
                        {
                            result.AddRange(nodes[i].GetScope(scopeRect));
                        }
                    }
                }
                // 划定范围, 获取范围内子对象中符合范围的对象
                // 判断是否与该区域相交 相交则取该区域内对象判断, 并获取其子节点判断是否相交
                // 获取子列表中的对象
                //var log = "";
                //foreach (var item in result)
                //{
                //    log += (item as PositionObject).name + ",";
                //}
                //UnityEngine.Debug.LogError(log);
            }
        }
        return result;
    }

    /// <summary>
    /// 获取当前四叉树的矩形区域
    /// </summary>
    /// <returns></returns>
    public RectGraphics GetRectangle()
    {
        return rect;
    }

    /// <summary>
    /// 获得子树列表
    /// </summary>
    /// <returns></returns>
    public QuadTree<T>[] GetSubNodes()
    {
        return nodes;
    }

    /// <summary>
    /// 获取当前树中的单元列表
    /// </summary>
    /// <returns></returns>
    public IList<T> GetItemList()
    {
        return itemsList;
    }
    
    /// <summary>
    /// 清除四叉树
    /// 已创建对象不会消除(减少GC消耗), clear后会放入对象池中当在读创建时取出重用
    /// </summary>
    public void Clear()
    {
        // 清空列表
        itemsList.Clear();
        // 清空范围
        // rect = null;

        for (var i = 0; i < nodes.Length; i++)
        {
            var quadTree = nodes[i];
            if (quadTree == null) { continue; }
            quadTree.Clear();
            nodes[i] = null;
            // 将列表放入缓存
            nodeCache.Enqueue(quadTree);
            rectCache.Enqueue(quadTree.rect);
        }
    }


    /// <summary>
    /// 判断单位是否完全在当前节点的矩形内
    /// </summary>
    /// <param name="item">被检测单位</param>
    /// <param name="testRect">检测矩形</param>
    /// <returns>被检测单位是否在当前几点的范围内</returns>
    public bool IsInner(ICollisionGraphics item, RectGraphics testRect)
    {
        if (item == null)
        {
            return false;
        }
        // 获得图形的外接矩形
        var itemExternalRect = item.GetExternalRect();
        // 判断是否在范围内
        var itemExternalRectHalfWidth = itemExternalRect.Width*0.5f;
        var nodeRectHalfWidth = testRect.Width * 0.5f;
        var itemExternalRectHalfHeight = itemExternalRect.Height*0.5f;
        var nodeRectHalfHeight = testRect.Height * 0.5f;
        // 是否在范围内
        return itemExternalRect.Postion.x + itemExternalRectHalfWidth <= testRect.Postion.x + nodeRectHalfWidth
            && itemExternalRect.Postion.x - itemExternalRectHalfWidth >= testRect.Postion.x - nodeRectHalfWidth
            && itemExternalRect.Postion.y + itemExternalRectHalfHeight <= testRect.Postion.y + nodeRectHalfHeight
            && itemExternalRect.Postion.y - itemExternalRectHalfHeight >= testRect.Postion.y - nodeRectHalfHeight;
    }


    /// <summary>
    /// 刷新四叉树
    /// 将有位置变动的单位放到其应在的子树中
    /// </summary>
    public void Refresh()
    {
        // 缓存上次所有单位位置, 如果位置发生变化并且没有在上次叶子节点上则重新插入
        // 检查子节点
        if (nodes != null && nodes[0] != null)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                nodes[i].Refresh();
            }
        }
        // 缓存列表
        var cacheList = new List<T>();

        // 检查当前节点的位置是否变动
        for (var i = 0; i < itemsList.Count; i++)
        {
            var item = itemsList[i];
            if (root.cachePosition.ContainsKey(item))
            {
                var position = root.cachePosition[item];
                if (!position.Equals(item.MyCollisionGraphics.Postion) && !IsInner(item.MyCollisionGraphics, rect))
                {
                    // 重新缓存位置
                    root.cachePosition[item] = item.MyCollisionGraphics.Postion;
                    // 重新插入缓存列表等待重新插入列表, 从当前列表删除
                    cacheList.Add(item);
                    itemsList.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                // 插入新数据
                root.cachePosition.Add(item, item.MyCollisionGraphics.Postion);
            }
        }
        if (cacheList.Count > 0)
        {
            // 将缓存列表插入
            root.Insert(cacheList);
            // 清空缓存列表
            //cacheList.Clear();
            // 整理四叉树结构
            ReBuild();
        }
    }


    /// <summary>
    /// 整理四叉树结构
    /// </summary>
    public void ReBuild()
    {
        // 从叶子节点上一层开始检查, 如果数量不够分裂的, 合并子树
        if (nodes != null && nodes[0] != null)
        {
            // 统计数量
            var itemCount = 0;
            var isSecondLevel = true;
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                node.ReBuild();
                itemCount += node.itemsList.Count;
                // 判断子集是否包含子集
                isSecondLevel &= node.nodes[0] == null;
            }
            itemCount += itemsList.Count;
            // 如果数量不足以分裂子节点则合并回收
            if (itemCount < maxItemCount && isSecondLevel)
            {
                // 合并子节点
                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    itemsList.AddRange(node.itemsList);
                    // 删除引用
                    node.Clear();
                    nodes[i] = null;
                }
            }
        }
        // 叶子节点不作操作, 从叶子节点上一级开始操作
    }


    // --------------------------------私有方法---------------------------------


    /// <summary>
    /// 拆分当前四叉树节点, 增加四个字节点
    /// 并将当前节点内的的对象进行分类转移至子节点
    /// </summary>
    private void Split()
    {
        QuadTree<T> node = null;
        RectGraphics subRect = null;
        int subLevel = level + 1;
        for (var i = 0; i < 4; i++)
        {
            subRect = GetSplitRectangle(rect, i, rectCache);
            if (nodeCache.Count != 0)
            {
                node = nodeCache.Dequeue();
                node.level = subLevel;
                node.rect = subRect;
            }
            else
            {
                node = new QuadTree<T>(subLevel, subRect, nodeCache, isRoot: false);
                node.root = root;
            }

            nodes[i] = node;
        }
    }


    /// <summary>
    /// 将对象插入子节点
    /// </summary>
    /// <param name="item"></param>
    /// <param name="subNodes"></param>
    private bool InsertToSubNode(T item, QuadTree<T>[] subNodes)
    {
        var result = false;
        var index = GetIndex(item.MyCollisionGraphics);
        if (index != -1)
        {
            subNodes[index].Insert(item);
            result = true;
        }
        return result;
    }

    /// <summary>
    /// 获得子节点
    /// </summary>
    /// <param name="parentRect">父节点矩形范围</param>
    /// <param name="subRectNum">子节点ID</param>
    /// <param name="rectCacheQueue">矩形对象缓存队列</param>
    /// <returns>子节点矩形范围</returns>
    private static RectGraphics GetSplitRectangle(RectGraphics parentRect, int subRectNum,
        Queue<ICollisionGraphics> rectCacheQueue = null)
    {
        // 获得当前四叉树的一半宽度高度
        var subQuadTreeWidth = parentRect.Width * 0.5f;
        var subQuadTreeHeight = parentRect.Height * 0.5f;
        var subX = 0f;
        var subY = 0f;
        RectGraphics result = null;
        var halfWidth = subQuadTreeWidth*0.5f;
        var halfHeight = subQuadTreeHeight*0.5f;
        switch (subRectNum)
        {
            case 0:
                subX = parentRect.Postion.x + halfWidth;
                subY = parentRect.Postion.y + halfHeight;
                break;
            case 1:
                subX = parentRect.Postion.x + halfWidth;
                subY = parentRect.Postion.y - halfHeight;
                break;
            case 2:
                subX = parentRect.Postion.x - halfWidth;
                subY = parentRect.Postion.y - halfHeight;
                break;
            case 3:
                subX = parentRect.Postion.x - halfWidth;
                subY = parentRect.Postion.y + halfHeight;
                break;
        }

        // 从cache中获取矩形单位并重新设置数据
        if (rectCacheQueue != null && rectCacheQueue.Count > 0)
        {
            result = rectCacheQueue.Dequeue() as RectGraphics;
            if (result != null)
            {
                result.Postion = new Vector2(subX, subY);
                result.Width = subQuadTreeWidth;
                result.Height = subQuadTreeHeight;
                result.Rotation = 0;
            }
        }
        // 如果为空则重新分配矩形数据类
        if(result == null)
        {
            result = new RectGraphics(new Vector2(subX, subY), subQuadTreeWidth, subQuadTreeHeight, 0);
        }

        // result.Postion = new Vector2(subX, subY);
        result.Width = subQuadTreeWidth;
        result.Height = subQuadTreeHeight;

        return result;
    }
}

///// <summary>
///// 矩形类
///// </summary>
//public class Rectangle : GraphicalItem<Rectangle>
//{

//    /// <summary>
//    /// 宽度
//    /// </summary>
//    public float Width { get; set; }

//    /// <summary>
//    /// 高度
//    /// </summary>
//    public float Height { get; set; }

//    public Rectangle()
//    {
        
//    }

//    public Rectangle(float x, float y, float w, float h)
//    {
//        X = x;
//        Y = y;
//        Width = w;
//        Height = h;
//    }

//    /// <summary>
//    /// 检测碰撞
//    /// TODO 优化
//    /// </summary>
//    /// <param name="target">目标</param>
//    /// <returns>是否碰撞</returns>
//    public override bool IsCollision(Rectangle target)
//    {
//        if (target == null || X + Width < target.X || target.X + target.Width < X || Y + Height < target.Y || target.Y + target.Height < Y)
//        {
//            return false;
//        }
//        return true;
//    }


//    public override string ToString()
//    {
//        return string.Format("x:{0}, y:{1}, W:{2}, H:{3}", X, Y, Width, Height);
//    }

    
//}

///// <summary>
///// 图形接口
///// 提供图形反馈
///// </summary>
///// <typeparam name="T">图形类型</typeparam>
//public interface IGraphical<T> where T : GraphicalItem<T>
//{
//    T GetGraphical();
//}


///// <summary>
///// 图形抽象类
///// </summary>
///// <typeparam name="T">目标类型也是图形抽象类</typeparam>
//public abstract class GraphicalItem<T> where T : GraphicalItem<T>
//{

//    /// <summary>
//    /// 位置X
//    /// </summary>
//    public float X { get; set; }

//    /// <summary>
//    /// 位置Y
//    /// </summary>
//    public float Y { get; set; }

//    /// <summary>
//    /// 检测碰撞
//    /// </summary>
//    /// <param name="target">目标对象</param>
//    /// <returns>是否碰撞</returns>
//    public abstract bool IsCollision(T target);
//}