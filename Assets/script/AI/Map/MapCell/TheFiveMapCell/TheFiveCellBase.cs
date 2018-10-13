using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 五行单元基类
/// </summary>
public class TheFiveCellBase : MapCellBase
{

    /// <summary>
    /// 当前单位的父级
    /// </summary>
    public Tower Tower { get; set; }

    /// <summary>
    /// 当前cell属性
    /// </summary>
    public TheFiveType TheFiveType { get; set; }

    /// <summary>
    /// 五行属性
    /// </summary>
    private TheFiveProperties properties = new TheFiveProperties();


    /// <summary>
    /// 五行属性附加值
    /// </summary>
    private TheFiveProperties additionProperties = new TheFiveProperties();

    /// <summary>
    /// 输出数量
    /// </summary>
    public int ExoprtCount { get; set; }



    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dataId"></param>
    /// <param name="drawLayer"></param>
    public TheFiveCellBase(GameObject obj, int dataId, int drawLayer)
        : base(obj, dataId, drawLayer)
    {
        Action = (mapCell) =>
        {

        };
    }


    /// <summary>
    /// 向外传输
    /// </summary>
    /// <returns>输出自己</returns>
    public TheFiveProperties Export()
    {
        TheFiveProperties result = null;
        if (ExoprtCount > 1)
        {
            // 分裂
            result = (properties + additionProperties) / ExoprtCount;
        }
        else
        {
            result = (properties + additionProperties);
        }

        return result;
    }

    /// <summary>
    /// 吸收来自其他单元的属性
    /// </summary>
    /// <param name="fromCell">来源及cell</param>
    public void Absorb([NotNull]TheFiveCellBase fromCell)
    {
        additionProperties.Plus(fromCell.Export());
        fromCell.ClearProperties();
    }

    /// <summary>
    /// 清理属性数据
    /// </summary>
    public void ClearProperties()
    {
        // 清空数据
        additionProperties = new TheFiveProperties();
    }



    // TODO 重构Draw 以解决绘制问题, draw时根据现实的实际大小将UntiWidth/当前缩放比例


}

/// <summary>
/// 五行属性
/// </summary>
public class TheFiveProperties
{

    /// <summary>
    /// 五行相生相克关系
    /// TODO 从表中获取
    /// </summary>
    public static float[,] TheFiveDiseasesAndInsect { get; private set; }


    /// <summary>
    /// 无属性值
    /// </summary>
    public float NoneValue { get; private set; }

    /// <summary>
    /// 火属性值
    /// </summary>
    public float FireValue { get; private set; }

    /// <summary>
    /// 水属性值
    /// </summary>
    public float WaterValue { get; private set; }

    /// <summary>
    /// 金属性值
    /// </summary>
    public float MetalValue { get; private set; }

    /// <summary>
    /// 木属性值
    /// </summary>
    public float WoodValue { get; private set; }

    /// <summary>
    /// 土属性值
    /// </summary>
    public float EarthValue { get; private set; }

    /// <summary>
    /// 链接次数
    /// </summary>
    public int LinkCount { get; private set; }



    /// <summary>
    /// 吸收来自其他单元的属性
    /// </summary>
    /// <param name="fromCell">来源及cell</param>
    public void Plus([NotNull]TheFiveProperties fromCell)
    {
        // 获取目标值到当前cell中
        // 混合本地值
        NoneValue += fromCell.NoneValue;
        FireValue += fromCell.FireValue;
        WaterValue += fromCell.WaterValue;
        MetalValue += fromCell.MetalValue;
        WoodValue += fromCell.WoodValue;
        EarthValue += fromCell.EarthValue;

        // 如果五属性都有则将其转化为无属性
        var min = Utils.MinValue(FireValue, WaterValue, MetalValue, WoodValue, EarthValue);
        if (min > 0)
        {
            NoneValue += min;
            FireValue -= min;
            WaterValue -= min;
            MetalValue -= min;
            WoodValue -= min;
            EarthValue -= min;
        }
        LinkCount += fromCell.LinkCount;
    }



    /// <summary>
    /// 除操作
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="count">分裂数量</param>
    /// <returns></returns>
    public static TheFiveProperties operator /([NotNull]TheFiveProperties cell, int count)
    {
        return new TheFiveProperties()
        {
            NoneValue = cell.NoneValue / count,
            FireValue = cell.FireValue / count,
            WaterValue = cell.WaterValue / count,
            MetalValue = cell.MetalValue / count,
            WoodValue = cell.WoodValue / count,
            EarthValue = cell.EarthValue / count,
        };
    }

    /// <summary>
    /// 除操作
    /// </summary>
    /// <param name="cell1"></param>
    /// <param name="cell2">分裂数量</param>
    /// <returns></returns>
    public static TheFiveProperties operator +([NotNull]TheFiveProperties cell1, TheFiveProperties cell2)
    {
        return new TheFiveProperties()
        {
            NoneValue = cell1.NoneValue + cell2.NoneValue,
            FireValue = cell1.FireValue + cell2.FireValue,
            WaterValue = cell1.WaterValue + cell2.WaterValue,
            MetalValue = cell1.MetalValue + cell2.MetalValue,
            WoodValue = cell1.WoodValue + cell2.WoodValue,
            EarthValue = cell1.EarthValue + cell2.EarthValue,
        };
    }



    /// <summary>
    /// 设置五行克制关系数据
    /// </summary>
    /// <param name="data"></param>
    public static void SetTheFiveDiseasesAndInsect(float[,] data)
    {
        TheFiveDiseasesAndInsect = data;
    }
}

/// <summary>
/// 五行类型
/// </summary>
public enum TheFiveType
{
    None = 0,   // 无属性
    Fire = 1,   // 火
    Water = 2,  // 水
    Metal = 3,  // 金
    Wood = 4,   // 木
    Earth = 5   // 土
}

