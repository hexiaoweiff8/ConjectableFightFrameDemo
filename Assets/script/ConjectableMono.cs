using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可推测战斗载体
/// </summary>
public class ConjectableMono : MonoBehaviour
{
    /// <summary>
    /// 地图Id
    /// </summary>
    public int MapId = 1;

    /// <summary>
    /// 地图中心
    /// </summary>
    public Vector3 MapCenter = Vector3.zero;

    /// <summary>
    /// 单位宽度
    /// </summary>
    public int UnitWidth = 1;


    /// <summary>
    /// 启动
    /// </summary>
	void Start () {
        // 设置游戏帧率
        Application.targetFrameRate = 45;
        // 初始化地图
        var mapBase = MapManager.Single.GetMapBase(MapId, MapCenter, UnitWidth);
        // 初始化单位

        // 初始化显示
        // 初始化逻辑
	}
	
	/// <summary>
    /// 更新
    /// </summary>
	void Update () {
		// 驱动逻辑与显示

	}
}
