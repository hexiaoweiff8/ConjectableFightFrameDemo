using System.Collections;
using System.Collections.Generic;
using Assets.script.AI.Member;
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
    /// 创建单位数量
    /// </summary>
    public int MemberCount = 10;

    /// <summary>
    /// 随机数种子
    /// </summary>
    public int RandomSeed = 1;

    /// <summary>
    /// 是否是显示模式
    /// </summary>
    public bool ShowMode = true;
    


    /// <summary>
    /// 启动
    /// </summary>
	void Start () {
        // 设置游戏帧率
        Application.targetFrameRate = 45;
        RandomPacker.Single.SetSeed(RandomSeed);
        // 初始化地图
        var mapBase = MapManager.Single.GetMapBase(MapId, MapCenter, UnitWidth);
        // 设置显示模式
        MemberManager.Single.ShowMode = ShowMode;
        DisplayCmdManager.Single.ShowMode = ShowMode;
        // 初始化数据黑板
        BlackBoard.Single.MapBase = mapBase;

        MemberManager.Single.Reset();

        for (var i = 0; i < MemberCount; i++)
        {
            // 初始化单位
            var memberDisplay = new MemberDisplay(GameObject.CreatePrimitive(PrimitiveType.Capsule));
            var member = new Member(MemberManager.Single.FrameCount, memberDisplay, MemberManager.Single);
            member.Hp = 100;
            MemberManager.Single.Add(member);
        }

        // 设置战斗结束检测
        MemberManager.Single.SetCheckFightEndFunc((memberList) =>
        {
            if (memberList.Count == 0)
            {
                // 战斗结束
                Debug.Log("战斗结束");
                return false;
            }
            // 战斗继续
            return true;
        });
        
	}
	
	/// <summary>
    /// 更新
    /// </summary>
	void Update () {
		// 驱动逻辑与显示
        MemberManager.Single.Do();
        DisplayCmdManager.Single.Do();
        // 绘制地图
        BlackBoard.Single.MapBase.DrawLine();

    }
}
