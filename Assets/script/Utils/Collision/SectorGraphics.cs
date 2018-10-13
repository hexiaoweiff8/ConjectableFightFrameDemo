using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 扇形图形
/// </summary>
public class SectorGraphics : CollisionGraphics
{

    /// <summary>
    /// 旋转角度
    /// </summary>
    public float Rotation { get; set; }

    /// <summary>
    /// 半径
    /// </summary>
    public float Radius { get; set; }

    /// <summary>
    /// 扇形打开角度
    /// 0-360
    /// </summary>
    public float OpenAngle { get; set; }



    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="position">圆心位置</param>
    /// <param name="rotation">旋转角度</param>
    /// <param name="radius">圆半径</param>
    /// <param name="openAngle">扇形开口角度</param>
    public SectorGraphics(Vector2 position, float rotation, float radius, float openAngle)
    {
        Postion = position;
        Rotation = rotation;
        Radius = radius;
        OpenAngle = openAngle;
        graphicType = GraphicType.Sector;
    }

    /// <summary>
    /// 获取正方向外接矩形
    /// </summary>
    /// <returns>外接矩形</returns>
    public override RectGraphics GetExternalRect()
    {
        var halfRadius = Radius * 0.5f;
        var angle = Rotation * Utils.AngleToPi;
        // Debug.Log(Math.PI / 180f);
        var x = halfRadius * (float) Math.Sin(angle);
        var y = halfRadius * (float) Math.Cos(angle);
        var rectPos = new Vector2(x, y) + Postion;
        var rect = new RectGraphics(rectPos, x*2, y*2, 0);
        //Utils.DrawGraphics(rect, Color.white);
        return rect;
    }



    /// <summary>
    /// 复制
    /// </summary>
    /// <returns>被复制出来的单位</returns>
    public override ICollisionGraphics Clone()
    {
        return new SectorGraphics(this.Postion, this.Rotation, this.Radius, this.OpenAngle);
    }

    /// <summary>
    /// 拷贝
    /// </summary>
    /// <param name="graphics"></param>
    public override void Copy(ICollisionGraphics graphics)
    {
        if (graphics.GraphicType == GraphicType.Sector)
        {
            var sourceSector = graphics as SectorGraphics;
            this.Postion = sourceSector.Postion;
            this.Rotation = sourceSector.Rotation;
            this.Radius = sourceSector.Radius;
            this.OpenAngle = sourceSector.OpenAngle;
        }
        else
        {
            Debug.Log("试图拷贝:" + graphics.GraphicType + "类型到扇形");
        }
    }
}