using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.script.AI.Member;
using UnityEngine;
using Assets.script.AI.Net;
using UnityEngine.UI;
using Util;

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

    /// <summary>
    /// 启动服务器按钮
    /// </summary>
    private GameObject startServerBtn;

    /// <summary>
    /// 加入服务器按钮
    /// </summary>
    private GameObject enterServerBtn;

    /// <summary>
    /// 被控制的单位
    /// </summary>
    private IMember controlMember = null;



    void Start()
    {
        startServerBtn = GameObject.Find("Canvas/ButtonStartServer");
        enterServerBtn = GameObject.Find("Canvas/ButtonEnterServer");
        var toggleServer = GameObject.Find("Canvas/IsServer").GetComponent<Toggle>();
        toggleServer.isOn = IsServer;
        startServerBtn.SetActive(IsServer);
        MemberManager.Single.IsServer = IsServer;
    }
    
    void Update()
    {
        // 驱动逻辑与显示
        MemberManager.Single.OnceFrame();
        if (MemberManager.Single.IsFighting)
        {
            DisplayCmdManager.Single.Do();
            // 绘制地图
            BlackBoard.Single.MapBase.DrawLine();
        }
        // 操作
        Control();
    }


    void OnDestroy()
    {
        MemberManager.Single.Reset();
    }


    public void Control()
    {
        if (controlMember != null)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                controlMember.SendCmd(new Commend(MemberManager.Single.FrameCount, controlMember.Id, OptionType.Move)
                {
                    Param = new Dictionary<string, string>()
                        {
                            {"fromX", "" + controlMember.X},
                            {"fromY", "" + controlMember.Y},
                            {"toX", "" + (controlMember.X - 1)},
                            {"toY", "" + controlMember.Y},
                        }
                });
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                controlMember.SendCmd(new Commend(MemberManager.Single.FrameCount, controlMember.Id, OptionType.Move)
                {
                    Param = new Dictionary<string, string>()
                        {
                            {"fromX", "" + controlMember.X},
                            {"fromY", "" + controlMember.Y},
                            {"toX", "" + (controlMember.X + 1)},
                            {"toY", "" + controlMember.Y},
                        }
                });
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                controlMember.SendCmd(new Commend(MemberManager.Single.FrameCount, controlMember.Id, OptionType.Move)
                {
                    Param = new Dictionary<string, string>()
                        {
                            {"fromX", "" + controlMember.X},
                            {"fromY", "" + controlMember.Y},
                            {"toX", "" + controlMember.X},
                            {"toY", "" + (controlMember.Y + 1)},
                        }
                });
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                controlMember.SendCmd(new Commend(MemberManager.Single.FrameCount, controlMember.Id, OptionType.Move)
                {
                    Param = new Dictionary<string, string>()
                        {
                            {"fromX", "" + controlMember.X},
                            {"fromY", "" + controlMember.Y},
                            {"toX", "" + controlMember.X},
                            {"toY", "" + (controlMember.Y - 1)},
                        }
                });
            }
        }
    }

    /// <summary>
    /// 初始化服务器
    /// </summary>
    public void InitServer()
    {

        // ----------------------------系统初始化-----------------------------
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
        MemberManager.Single.InitNet(ServerIp, ServerPort);


        // -----------------------------显示初始化-----------------------------




        // -----------------------------战斗结束检测-------------------------------
        // 设置战斗结束检测
        MemberManager.Single.SetCheckFightEndFunc((memberList) =>
        {
            //if (memberList.Count == 0)
            //{
            //    // 战斗结束
            //    Debug.Log("战斗结束");
            //    return false;
            //}
            // 战斗继续
            return true;
        });
    }


    /// <summary>
    /// 启动服务器
    /// </summary>
    public void StartServer()
    {
        // 发送启动命令
        MemberManager.Single.SendCmd(new Commend(0, 0, OptionType.ServerStart));
    }

    /// <summary>
    /// 加入服务器
    /// </summary>
    public void EnterServer()
    {
        // 发送加入服务器命令
        MemberManager.Single.SendCmd(new Commend(0, 0, OptionType.EnterServer)
        {
            Param =
            {
                { "ip", "" + Utils.GetLocalIP() }
            }
        });
    }

    /// <summary>
    /// 切换是否为服务器
    /// </summary>
    public void ToggleServer()
    {
        IsServer = !IsServer;
        startServerBtn.SetActive(IsServer);
        MemberManager.Single.IsServer = IsServer;
    }

    /// <summary>
    /// 创建单位
    /// </summary>
    public void CreateMember()
    {
        // -----------------------------数据初始化-----------------------------
        // 初始化单位
        RandomPacker.Single.SetSeed((int)DateTime.Now.Ticks);
        var memberDisplay = new MemberDisplay(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        controlMember = new Member(MemberManager.Single.FrameCount, memberDisplay, MemberManager.Single, RandomPacker.Single.GetRangeI(0, 10000));
        controlMember.Hp = 100;
        controlMember.Speed = 1;
        controlMember.IsLocal = true;
        Debug.Log("单位Id" + controlMember.Id);
        MemberManager.Single.Add(controlMember);
    }
}
