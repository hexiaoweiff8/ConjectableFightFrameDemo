using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 圆形图形
/// </summary>
public class CircleGraphics : CollisionGraphics
{

    /// <summary>
    /// 半径
    /// </summary>
    public float Radius { get; set; }



    /// <summary> 
    /// 初始化
    /// </summary>
    /// <param name="position">圆心位置</param>
    /// <param name="radius">圆半径</param>
    public CircleGraphics(Vector2 position, float radius)
    {
        Postion = position;
        Radius = radius;
        graphicType = GraphicType.Circle;
    }

    /// <summary>
    /// 获取正方向外接矩形
    /// </summary>
    /// <returns>外接矩形</returns>
    public override RectGraphics GetExternalRect()
    {
        // 直径
        var d = Radius*2;
        var circle = new RectGraphics(Postion, d, d, 0);
        // Utils.DrawGraphics(circle, Color.white);
        return circle;
    }


    /// <summary>
    /// 复制
    /// </summary>
    /// <returns>被复制出来的单位</returns>
    public override ICollisionGraphics Clone()
    {
        return new CircleGraphics(this.Postion, this.Radius);
    }

    /// <summary>
    /// 拷贝
    /// </summary>
    /// <param name="graphics"></param>
    public override void Copy(ICollisionGraphics graphics)
    {
        if (graphics.GraphicType == GraphicType.Circle)
        {
            var sourceCircle = graphics as CircleGraphics;
            this.Postion = sourceCircle.Postion;
            this.Radius = sourceCircle.Radius;
        }
        else
        {
            Debug.Log("试图拷贝:" + graphics.GraphicType + "类型到圆形");
        }
    }
}
