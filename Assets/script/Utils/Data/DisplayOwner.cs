using System;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 显示单位包装对象
/// </summary>
public class DisplayOwner
{
    /// <summary>
    /// 唯一Id
    /// </summary>
    public int Id { get { return ClusterData.AllData.MemberData.ObjID; } }

    ///// <summary>
    ///// 显示对象Obj引用
    ///// </summary>
    //public MapCellBase MapCell { get; set; }

    /// <summary>
    /// 集群数据引用
    /// </summary>
    public PositionObject ClusterData { get; set; }


    ///// <summary>
    ///// 显示对象引用
    ///// </summary>
    //public RanderControl RanderControl { get; set; }

    ///// <summary>
    ///// 对象数据引用
    ///// </summary>
    //public VOBase MemberData { get; set; }



    //private static uint additionId = 0;


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="clusterData"></param>
    public DisplayOwner( [NotNull] PositionObject clusterData)
    {
        //MapCell = mapCell;
        ClusterData = clusterData;
    }


    /// <summary>
    /// 销毁对象
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(ClusterData.MapCell.GameObj);
        CleanData();
    }


    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleanData()
    {
        ClusterManager.Single.Remove(ClusterData);
        ClusterData = null;
        //MFAModelRender = null;
        //MemberData = null;
    }
}
