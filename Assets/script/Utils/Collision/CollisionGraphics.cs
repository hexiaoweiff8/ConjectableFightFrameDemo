using System;
using UnityEngine;

public abstract class CollisionGraphics : ICollisionGraphics
{

    /// <summary>
    /// 获取图形类型
    /// </summary>
    public GraphicType GraphicType
    {
        get { return graphicType; }
    }

    /// <summary>
    /// 图形所在位置
    /// </summary>
    public Vector2 Postion { get; set; }


    /// <summary>
    /// 图形类型
    /// </summary>
    protected GraphicType graphicType;


    /// <summary>
    /// 检测与其他图形的碰撞
    /// </summary>
    /// <param name="graphics">其他图形对象</param>
    /// <returns>是否碰撞</returns>
    public bool CheckCollision(ICollisionGraphics graphics)
    {
        return CheckCollision(this, graphics);
    }

    /// <summary>
    /// 获取外接矩形
    /// </summary>
    /// <returns></returns>
    public abstract RectGraphics GetExternalRect();

    /// <summary>
    /// 检测是否碰撞
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    /// <returns></returns>
    public static bool CheckCollision(ICollisionGraphics item1, ICollisionGraphics item2)
    {
        var result = false;
        switch (item1.GraphicType)
        {
            case GraphicType.Circle:
                switch (item2.GraphicType)
                {
                    case GraphicType.Circle:
                        // 圆圆碰撞
                        result = CheckCircleAndCircle(item1, item2);
                        break;
                    case GraphicType.Rect:
                        // 检测2 半径到四边距离
                        result = CheckCircleAndRect(item1, item2);
                        break;
                    case GraphicType.Sector:
                        // 检测3 圆与扇形
                        result = CheckSectorAndCircle(item2, item1);
                        break;
                }
                break;

            case GraphicType.Rect:
                switch (item2.GraphicType)
                {
                    case GraphicType.Circle:
                        result = CheckCircleAndRect(item2, item1);
                        break;
                    // 检测2 半径到四边距离
                    case GraphicType.Rect:
                        result = CheckRectAndRect(item2, item1);
                        break;
                    // 检测4 矩形与矩形
                    case GraphicType.Sector:
                        result = CheckSectorAndRect(item2, item1);
                        break;
                        // 检测5 矩形与扇形
                }
                break;

            case GraphicType.Sector:
                switch (item2.GraphicType)
                {
                    case GraphicType.Circle:
                        // 检测3 扇形与圆
                        result = CheckSectorAndCircle(item1, item2);
                        break;
                    case GraphicType.Rect:
                        // 检测5 扇形与矩形
                        result = CheckSectorAndRect(item1, item2);
                        break;
                        //case GraphicType.Sector:
                        //    break;
                        //    // 检测6 扇形与扇形
                }
                break;
        }
        return result;
    }


    /// <summary>
    /// 圆圆碰撞检测
    /// </summary>
    /// <param name="circle1">圆1</param>
    /// <param name="circle2">圆2</param>
    /// <returns>是否碰撞</returns>
    public static bool CheckCircleAndCircle(ICollisionGraphics circle1, ICollisionGraphics circle2)
    {
        var result = false;

        var circleGraphics1 = circle1 as CircleGraphics;
        var circleGraphics2 = circle2 as CircleGraphics;

        if (circleGraphics1 != null && circleGraphics2 != null)
        {
            // 检查两圆的半径与距离大小
            var distance = (circleGraphics1.Postion - circleGraphics2.Postion).magnitude;
            result = distance < circleGraphics1.Radius + circleGraphics2.Radius;
        }

        return result;
    }

    /// <summary>
    /// 圆方碰撞检测
    /// </summary>
    /// <param name="circle">圆</param>
    /// <param name="rect">方</param>
    /// <returns>是否碰撞</returns>
    public static bool CheckCircleAndRect(ICollisionGraphics circle, ICollisionGraphics rect)
    {
        // 转换格式
        var circleGraphics = circle as CircleGraphics;
        var rectGraphics = rect as RectGraphics;

        if (circleGraphics != null && rectGraphics != null)
        {

            var positionOffset = circleGraphics.Postion - rectGraphics.Postion;
            var axis = rectGraphics.HorizonalAxis;
            var dot = Math.Abs(Vector2.Dot(axis, positionOffset));
            // 映射对角线到四个轴上进行对比
            var projection1 = Math.Abs(Vector2.Dot(axis, rectGraphics.Diagonal1)) * 0.5f;
            var projection2 = Math.Abs(Vector2.Dot(axis, rectGraphics.Diagonal2)) * 0.5f;
            var projection3 = circleGraphics.Radius;

            projection1 = projection1 > projection2 ? projection1 : projection2;

            if (projection1 + projection3 <= dot)
            {
                return false;
            }

            axis = rectGraphics.VerticalAxis;
            dot = Math.Abs(Vector2.Dot(axis, positionOffset));
            // 映射对角线到四个轴上进行对比
            projection1 = Math.Abs(Vector2.Dot(axis, rectGraphics.Diagonal1)) * 0.5f;
            projection2 = Math.Abs(Vector2.Dot(axis, rectGraphics.Diagonal2)) * 0.5f;
            projection3 = circleGraphics.Radius;

            projection1 = projection1 > projection2 ? projection1 : projection2;

            if (projection1 + projection3 <= dot)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 扇圆碰撞检测
    /// </summary>
    /// <param name="sector">扇形</param>
    /// <param name="circle">圆形</param>
    /// <returns></returns>
    public static bool CheckSectorAndCircle(ICollisionGraphics sector, ICollisionGraphics circle)
    {

        var sectorGraphics = sector as SectorGraphics;
        var circleGraphics = circle as CircleGraphics;
        if (sectorGraphics != null && circleGraphics != null)
        {
            // 如果不在扇形圆半径内, 则不相交
            if ((sectorGraphics.Postion - circleGraphics.Postion).magnitude >
                sectorGraphics.Radius + circleGraphics.Radius)
            {
                return false;
            }
            // 获取扇形两角的点位置
            var halfAngle = sectorGraphics.OpenAngle * 0.5f;
            // 两个点相对圆心方向
            var pointForCorner1 = new Vector2((float)Math.Sin(sectorGraphics.Rotation + halfAngle),
                (float)Math.Cos(sectorGraphics.Rotation + halfAngle));
            var pointForCorner2 = new Vector2((float)Math.Sin(sectorGraphics.Rotation - halfAngle),
                (float)Math.Cos(sectorGraphics.Rotation - halfAngle));


            // 如果圆心在扇形角度内则相交
            var distance1 = EvaluatePointToLine(circleGraphics.Postion, pointForCorner1, sectorGraphics.Postion);
            var distance2 = EvaluatePointToLine(circleGraphics.Postion, sectorGraphics.Postion, pointForCorner2);
            if (distance1 >= 0 && distance2 >= 0)
            {
                // 圆心在扇形开口角度内
                return true;
            }
            // 如果与两线相交则相交
            if (CheckCircleAndLine(circle, sectorGraphics.Postion, pointForCorner1) || CheckCircleAndLine(circle, sectorGraphics.Postion, pointForCorner2))
            {
                return true;
            }
        }


        return false;
    }

    /// <summary>
    /// 方方碰撞检测
    /// </summary>
    /// <param name="rect1">方形1</param>
    /// <param name="rect2">方形2</param>
    /// <returns>是否碰撞</returns>
    public static bool CheckRectAndRect(ICollisionGraphics rect1, ICollisionGraphics rect2)
    {

        // 转换类型
        var rectGraphics1 = rect1 as RectGraphics;
        var rectGraphics2 = rect2 as RectGraphics;

        if (rectGraphics1 != null && rectGraphics2 != null)
        {

            var positionOffset = rectGraphics1.Postion - rectGraphics2.Postion;
            // 检测矩形的距离, 如果超过最大检测距离, 则不需要继续监测
            //var r1 = rectGraphics1.Diagonal1.magnitude * 0.5f;
            //var r2 = rectGraphics2.Diagonal1.magnitude * 0.5f;
            //if (r1 + r2 > positionOffset.magnitude)
            //{
            //    return false;
            //}

            //如果没有旋转则使用无旋转碰撞
            if (rectGraphics2.Rotation == 0 && rectGraphics1.Rotation == 0)
            {
                var offPos = rectGraphics2.Postion - rectGraphics1.Postion;

                if (Math.Abs(offPos.x) < (rectGraphics2.Width * 0.5f + rectGraphics1.Width * 0.5f) &&
                    (Math.Abs(offPos.y) < (rectGraphics2.Height * 0.5f + rectGraphics1.Height * 0.5f)))
                {
                    return true;
                }
                return false;
            }


            // 建立投影, 如果在法线上任意两投影不重合, 说明不想交, 否则相交
            var axisArray = new[]
            {
                rectGraphics1.HorizonalAxis,
                rectGraphics1.VerticalAxis,
                rectGraphics2.HorizonalAxis,
                rectGraphics2.VerticalAxis,
            };

            var projection1 = 0f;
            var projection2 = 0f;
            var projection3 = 0f;
            var projection4 = 0f;
            var dot = 0f;
            Vector2 axis;
            for (var i = 0; i < axisArray.Length; i++)
            {
                axis = axisArray[i];
                dot = Math.Abs(Vector2.Dot(axis, positionOffset));
                // 映射对角线到四个轴上进行对比
                projection1 = Math.Abs(Vector2.Dot(axis, rectGraphics1.Diagonal1));
                projection2 = Math.Abs(Vector2.Dot(axis, rectGraphics1.Diagonal2));
                projection3 = Math.Abs(Vector2.Dot(axis, rectGraphics2.Diagonal1));
                projection4 = Math.Abs(Vector2.Dot(axis, rectGraphics2.Diagonal2));

                projection1 = projection1 > projection2 ? projection1 : projection2;
                projection3 = projection3 > projection4 ? projection3 : projection4;


                if (projection1 * 0.5f + projection3 * 0.5f <= dot)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 扇方碰撞检测
    /// </summary>
    /// <param name="sector">扇形</param>
    /// <param name="rect">方形</param>
    /// <returns>是否碰撞</returns>
    public static bool CheckSectorAndRect(ICollisionGraphics sector, ICollisionGraphics rect)
    {
        var sectorGraphics = sector as SectorGraphics;
        var rectGraphics = rect as RectGraphics;
        if (sectorGraphics != null && rectGraphics != null)
        {
            var sectorR = (sectorGraphics.Postion - rectGraphics.Postion).magnitude;
            if (sectorR > sectorGraphics.Radius + rectGraphics.Width && sectorR > sectorGraphics.Radius + rectGraphics.Height)
            {
                return false;
            }
            // TODO 检测举行位置
            // 获取扇形两角的点位置
            var halfAngle = sectorGraphics.OpenAngle * 0.5f;
            // 两个点相对圆心方向
            var pointForCorner1 = new Vector2((float)Math.Sin(sectorGraphics.Rotation + halfAngle),
                (float)Math.Cos(sectorGraphics.Rotation + halfAngle));
            var pointForCorner2 = new Vector2((float)Math.Sin(sectorGraphics.Rotation - halfAngle),
                (float)Math.Cos(sectorGraphics.Rotation - halfAngle));

            var distance1 = EvaluatePointToLine(rectGraphics.Postion, pointForCorner1, sectorGraphics.Postion);
            var distance2 = EvaluatePointToLine(rectGraphics.Postion, sectorGraphics.Postion, pointForCorner2);
            if (distance1 >= 0 && distance2 >= 0)
            {
                // 圆心在扇形开口角度内
                return true;
            }
            //// 如果与两线相交则相交
            //if (CheckCircleAndLine(circle, sectorGraphics.Postion, pointForCorner1) || CheckCircleAndLine(circle, sectorGraphics.Postion, pointForCorner2))
            //{
            //    return true;
            //}
        }

        return false;
    }

    ///// <summary>
    ///// 扇形扇形碰撞检测
    ///// </summary>
    ///// <param name="sector1">扇形1</param>
    ///// <param name="sector2">扇形2</param>
    ///// <returns>是否碰撞</returns>
    //public static bool CheckSectorAndSector(ICollisionGraphics sector1, ICollisionGraphics sector2)
    //{
    //    var result = false;



    //    return result;
    //}


    /// <summary>
    /// 圆与线碰撞
    /// </summary>
    /// <param name="circle">圆形</param>
    /// <param name="lineP1">线点1</param>
    /// <param name="lineP2">线点2</param>
    /// <returns>是否碰撞</returns>
    public static bool CheckCircleAndLine(ICollisionGraphics circle, Vector2 lineP1, Vector2 lineP2)
    {
        var result = false;

        var circleGraphics = circle as CircleGraphics;
        if (circleGraphics != null)
        {
            result = CheckCircleAndLine(circleGraphics.Postion, circleGraphics.Radius, lineP1, lineP2);
        }

        return result;
    }


    public static bool CheckCircleAndLine(Vector2 circlePos, float circleRadius, Vector2 lineP1, Vector2 lineP2)
    {
        var result = false;
        // 线段长度
        var lineVec = lineP2 - lineP1;
        var lineLen = lineVec.magnitude;
        var lineToCircleCenter = circlePos - lineP1;
        var lineDir = lineVec.normalized;
        // 计算圆心到点1线段映射到线段上的映射长度
        var projectionToLine = Vector2.Dot(lineToCircleCenter, lineDir);

        Vector2 nearest;
        if (projectionToLine <= 0)
        {
            nearest = lineP1;
        }
        else if (projectionToLine >= lineLen)
        {
            nearest = lineP2;
        }
        else
        {
            nearest = lineP1 + lineDir * projectionToLine;
        }

        result = (circlePos - nearest).magnitude <= circleRadius;

        return result;
    }

    /// <summary>
    /// 计算点与线的关系
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="lineP1">线点1</param>
    /// <param name="lineP2">线点2</param>
    /// <returns>返回值大于0表示点在线右侧, 等于0表示点在线上, 小于0表示点在线左侧</returns>
    public static float EvaluatePointToLine(Vector2 point, Vector2 lineP1, Vector2 lineP2)
    {
        var a = lineP2.y - lineP1.y;
        var b = lineP1.x - lineP2.x;
        var c = lineP2.x * lineP1.y - lineP1.x * lineP2.y;

        return a * point.x + b * point.y + c;
    }




    /// <summary>
    /// 获取Axis2映射到Axis1的值
    /// </summary>
    /// <param name="axis">映射轴</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="horizonalAxis">横向轴</param>
    /// <param name="verticalAxis">纵向轴</param>
    /// <returns>映射值</returns>
    public static float GetProjectionRaduis(Vector2 axis, float width, float height, Vector2 horizonalAxis, Vector2 verticalAxis)
    {
        // 加入旋转
        var halfWidth = width * 0.5f;
        var halfHeight = height * 0.5f;
        var projectionAxisX = Vector2.Dot(axis, horizonalAxis);
        var projectionAxisY = Vector2.Dot(axis, verticalAxis);

        return halfWidth * projectionAxisX + halfHeight * projectionAxisY;
    }

    /// <summary>
    /// 设置单位的spaceSet空间值
    /// </summary>
    /// <param name="graphics">图形单位</param>
    /// <param name="spaceSet">空间值</param>
    /// <param name="unitWidth">单位宽度</param>
    public static void SetGraphicsSpaceSet(ICollisionGraphics graphics, float spaceSet, int unitWidth = 1)
    {
        if (graphics != null && spaceSet > 0)
        {
            var circle = graphics as CircleGraphics;
            if (circle != null)
            {
                circle.Radius = spaceSet * unitWidth;
                return;
            }
            var rect = graphics as RectGraphics;
            if (rect != null)
            {
                rect.Width = spaceSet * 1.414f * unitWidth;
                rect.Height = spaceSet * 1.414f * unitWidth;
                return;
            }
        }
    }

    /// <summary>
    /// 复制数据
    /// </summary>
    /// <returns>被复制的图形类</returns>
    public abstract ICollisionGraphics Clone();

    /// <summary>
    /// 复制数据到当前类中
    /// </summary>
    /// <param name="graphics">被复制数据</param>
    public abstract void Copy(ICollisionGraphics graphics);
}


