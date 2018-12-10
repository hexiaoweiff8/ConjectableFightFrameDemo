using System.Collections;
using System.Collections.Generic;
using Assets.script.AI.Member;
using UnityEngine;
using Assets.script.AI.Net;

public class FrameSyncDemo : MonoBehaviour
{

    // 创建服务端

    // 创建客户端

    // 创建单位
    // 单位
    // 创建本地单位, 启动网络请求
    // 判断是否为server
    // 注册server事件, 创建单位在server上进行创建


    /// <summary>
    /// 是否为服务器
    /// </summary>
    public bool IsServer = false;

    /// <summary>
    /// 服务器Ip
    /// </summary>
    public string ServerIp = "127.0.0.1";

    /// <summary>
    /// 服务器端口
    /// </summary>
    public int ServerPort = 9999;

    /// <summary>
    /// 客户端端口
    /// </summary>
    public int ClientPort = 9998;

    /// <summary>
    /// 随机数种子
    /// </summary>
    public int RandomSeed = 1;

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



    void Start()
    {
        // 设置游戏帧率
        Application.targetFrameRate = 45;
        RandomPacker.Single.SetSeed(RandomSeed);
        // 初始化地图
        var mapBase = MapManager.Single.GetMapBase(MapId, MapCenter, UnitWidth);

        // 设置显示模式
        MemberManager.Single.ShowMode = true;
        DisplayCmdManager.Single.ShowMode = true;
        // 初始化数据黑板
        BlackBoard.Single.MapBase = mapBase;
        MemberManager.Single.Reset();
        // 初始化服务器
        if (IsServer)
        {
            MemberManager.Single.InitServer(ServerPort);
        }
        MemberManager.Single.InitNet(ServerIp, ServerPort, ClientPort);




        // 初始化单位
        var memberDisplay = new MemberDisplay(GameObject.CreatePrimitive(PrimitiveType.Capsule));
        var member = new Member(MemberManager.Single.FrameCount, memberDisplay, MemberManager.Single);
        member.Hp = 100;
        MemberManager.Single.Add(member);
        //// 初始化单位
        //memberDisplay = new MemberDisplay(GameObject.CreatePrimitive(PrimitiveType.Capsule));
        //member = new Member(MemberManager.Single.FrameCount, memberDisplay);
        //member.Hp = 100;
        //MemberManager.Single.Add(member);

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

        if (IsServer)
        {
            // 建立服务器
        }

    }
    
    void Update()
    {
        // 驱动逻辑与显示
        MemberManager.Single.Do();
        DisplayCmdManager.Single.Do();
        // 绘制地图
        BlackBoard.Single.MapBase.DrawLine();
        // TODO 操作注册单位

        // 单位的操作, 发送消息

    }
}
