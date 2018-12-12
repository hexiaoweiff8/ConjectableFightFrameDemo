using Assets.script.AI.Net;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Util;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// Member管理器
    /// 负责控制Member的逻辑与等待
    /// </summary>
    public class MemberManager : SingleItem<MemberManager>, IMemberManager
    {

        /// <summary>
        /// 帧速度
        /// </summary>
        public const int FrameSpeed = 1;

        /// <summary>
        /// 推断模式帧速度
        /// </summary>
        public const int FastFrameSpeed = 60;


        /// <summary>
        /// 当前帧数
        /// </summary>
        public long FrameCount { get { return frameCount; } }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowMode { get; set; }

        /// <summary>
        /// 是否为网络模式
        /// </summary>
        public bool IsNetMode { get; set; }

        /// <summary>
        /// 成员数量
        /// </summary>
        public int MemberCount { get { return memberList.Count; } }

        /// <summary>
        /// 是否为服务端
        /// 如果false则为客户端
        /// 如果true则为服务端
        /// </summary>
        public bool IsServer { get; set; }

        /// <summary>
        /// 是否为是战斗状态
        /// </summary>
        public bool IsFighting { get { return isFighting; } }




        /// <summary>
        /// 当前帧数
        /// </summary>
        private long frameCount = 0;

        /// <summary>
        /// 是否战斗结束
        /// </summary>
        private bool isFighting = false;

        // -------------------------------------客户端部分----------------------------------------

        /// <summary>
        /// member集合
        /// </summary>
        private List<IMember> memberList = new List<IMember>();

        /// <summary>
        /// 客户端收到的操作缓存列表
        /// </summary>
        private List<IOptionCommand> optionCommandList = new List<IOptionCommand>();

        /// <summary>
        /// 客户端缓存命令列表
        /// </summary>
        private FramePacker clientStageFramePacker = new FramePacker();


        // ------------------------------------服务器部分------------------------------------------

        private const float ServerFrameTime = 0.02f;

        /// <summary>
        /// 服务器缓存命令列表
        /// </summary>
        private FramePacker serverStageFramePacker = new FramePacker();

        /// <summary>
        /// 服务器定时器
        /// </summary>
        private Timer serverTimer = new Timer(ServerFrameTime, true);


        // -------------------------------------通用部分--------------------------------------------

        /// <summary>
        /// 反序列化
        /// </summary>
        private BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器

        /// <summary>
        /// 检测战斗结果
        /// </summary>
        private Func<List<IMember>, bool> checkFightEnd = null;


        // -------------------------------------公共方法------------------------------------------

        /// <summary>
        /// 设置检测战斗结束
        /// </summary>
        /// <param name="check"></param>
        public void SetCheckFightEndFunc(Func<List<IMember>, bool> check)
        {
            checkFightEnd = check;
        }

        /// <summary>
        /// 初始化服务器内容
        /// </summary>
        public void InitServer(int serverPort)
        {
            serverTimer.Kill();
            // 启动本地监听
            IsNetMode = true;
            NetManager.Single.StartBind(serverPort, ClientType.TCP);
            NetManager.Single.ServerComputeAction = (bytes) =>
            {
                //Debug.Log("收到消息" + bytes.Length);
                // 处理数据
                // 反序列化
                var stream = new MemoryStream(bytes);
                var packet = binFormat.Deserialize(stream) as FramePacker;
                // 缓存各个单位的操作
                serverStageFramePacker.CommandList.AddRange(packet.CommandList);
            };

            // 启动服务器定时器
            serverTimer = new Timer(ServerFrameTime, true).OnCompleteCallback(() =>
            {
                //Debug.Log("tick!");
                // 定时向所有客户端发送操作
                SendToAll(serverStageFramePacker);
                serverStageFramePacker.CommandList.Clear();
            }).Start();
        }

        /// <summary>
        /// 初始化网络
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void InitNet(string serverIp, int serverPort)
        {
            IsNetMode = true;
            NetManager.Single.Connect(serverIp, serverPort, ClientType.TCP);
            NetManager.Single.ClientComputeAction = (bytes) =>
            {
                // 处理数据
                // 反序列化
                var stream = new MemoryStream(bytes);
                var packet = binFormat.Deserialize(stream) as FramePacker;
                // 解析后循环发送
                foreach (var cmd in packet.CommandList)
                {
                    // 分发操作
                    Dispatch(cmd);
                }

                // 发送缓存操作
                stream = new MemoryStream();
                binFormat.Serialize(stream, clientStageFramePacker);
                //Debug.Log("发送消息" + stream.ToArray().Length);
                NetManager.Single.ClientSend(stream.ToArray(), ClientType.TCP);
                clientStageFramePacker.CommandList.Clear();
            };
            
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void OnceFrame()
        {
            if (isFighting)
            {
                var targetFrame = (ShowMode ? FrameSpeed : FastFrameSpeed) + frameCount;
                while (targetFrame > frameCount)
                {
                    for (var i = 0; i < memberList.Count; i++)
                    {
                        var member = memberList[i];
                        if ((!member.CheckWait(frameCount) && ShowMode) || !ShowMode)
                        {
                            member.OnceFrame(frameCount, BlackBoard.Single);
                        }
                    }
                    frameCount++;

                    if (checkFightEnd != null)
                    {
                        isFighting = checkFightEnd(memberList);
                    }
                    else
                    {
                        // 默认战斗结束检测
                        if (memberList.Count <= 1)
                        {
                            Debug.Log("战斗结束");
                        }
                    }
                }
            }
            // -------------------客户端操作---------------------
            // 执行缓存内容
            for (var i = 0; i < optionCommandList.Count; i++)
            {
                DoCmd(optionCommandList[i]);
            }
            optionCommandList.Clear();

        }

        /// <summary>
        /// 发送操作命令
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCmd(IOptionCommand cmd)
        {
            // 缓存命令, 等待逻辑帧发送
            // 如果收到服务器的逻辑计数帧, 则立即发送
            clientStageFramePacker.CommandList.Add(cmd);
        }

        /// <summary>
        /// 转发给所有单位
        /// </summary>
        public void SendToAll(FramePacker frameData)
        {
            var stream = new MemoryStream();
            binFormat.Serialize(stream, frameData);
            NetManager.Single.ServerSendToAll(stream.ToArray(), ClientType.TCP);
        }

        /// <summary>
        /// 抛出消息
        /// </summary>
        public void Dispatch(IOptionCommand cmd)
        {
            //Debug.Log("消息处理" + cmd.OpType);

            // 将创建事件抛入待处理列表, 解决多线程创建单位问题
            optionCommandList.Add(cmd);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public void DoCmd(IOptionCommand cmd)
        {
            switch (cmd.OpType)
            {
                case OptionType.Create:
                {
                    // 如果是单位创建则直接创建单位
                    // 创建单位
                    // 单位Id
                    var newId = cmd.MemberId;
                    // 判断如果是自己的单位跳过
                    if (memberList.Any((item) => item.Id == newId))
                    {
                        return;
                    }
                    // 初始位置
                    var posX = int.Parse(cmd.Param["posX"]);
                    var posY = int.Parse(cmd.Param["posY"]);
                    // 血量
                    var hp = int.Parse(cmd.Param["hp"]);
                    var memberDisplay = new MemberDisplay(GameObject.CreatePrimitive(PrimitiveType.Capsule));
                    var member = new Member(frameCount, memberDisplay, this, newId);
                    member.X = posX;
                    member.Y = posY;
                    member.Hp = hp;
                    Add(member);
                }
                    break;
                case OptionType.EnterServer:
                {
                    // 单位加入服务器
                    if (IsServer)
                    {
                        Debug.Log("单位加入服务器" + cmd.Param["ip"]);
                            // 单位IP
                            // 单位
                    }
                }
                    break;
                case OptionType.ServerStart:
                    {
                        // 服务器发送的开始命令
                        isFighting = true;
                    }
                    break;
                default:
                {

                    // 处理单位消息
                    for (var i = 0; i < memberList.Count; i++)
                    {
                        if (cmd.MemberId == memberList[i].Id)
                        {
                            memberList[i].Dispatch(cmd);
                        }
                    }
                    }
                    break;
            }

            for (var i = 0; i < memberList.Count; i++)
            {
                if (((Member)memberList[i]).waitingOptionDic.Any((kv) => kv.Value > 0))
                {
                    Debug.LogError("" + memberList[i].Id + "消息未回传");
                }
            }
        }


        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="member"></param>
        public void Add(IMember member)
        {
            memberList.Add(member);
            // 发送创建命令
            member.SendCmd(new Commend(frameCount, member.Id, OptionType.Create)
            {
                Param = new Dictionary<string, string>()
                {
                    { "posX", "" + member.X},
                    { "posY", "" + member.Y},
                    { "hp", "" + member.Hp},
                },
            });
        }

        /// <summary>
        /// 获取一个单位
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMember Get(int index)
        {
            if (memberList.Count > index)
            {
                return memberList[index];
            }

            return null;
        }


        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="member"></param>
        public void Remove(IMember member)
        {
            memberList.Remove(member);
            // 从显示层消除
            member.DisplayMember.Remove();
        }


        /// <summary>
        /// 重置逻辑管理器
        /// </summary>
        public void Reset()
        {
            frameCount = 0;
            memberList.Clear();
            isFighting = false;
            NetManager.Single.Reset();
        }

        // 消息处理
    }

    /// <summary>
    /// 数据黑板
    /// </summary>
    public class BlackBoard : SingleItem<BlackBoard>, IBlackBoard
    {
        /// <summary>
        /// 地图数据
        /// </summary>
        public MapBase MapBase { get; set; }

        /// <summary>
        /// 清理数据
        /// </summary>
        public void Clear()
        {
            MapBase.Clear();
            MapBase = null;
        }
    }

    /// <summary>
    /// 网络打包类
    /// </summary>
    [Serializable]
    public class Commend : IOptionCommand
    {
        /// <summary>
        /// 帧数
        /// </summary>
        public long FrameNum { get; set; }

        /// <summary>
        /// 单位Id
        /// </summary>
        public int MemberId { get; set; }

        // 操作类型
        // 出生
        // 移动
        // 攻击
        // 死亡
        public OptionType OpType { get; set; }
        

        // 操作数据
        // 出生位置, 血量
        // 移动目标位置
        // 攻击目标
        // 死亡标志
        public Dictionary<string, string> Param { get; set; }

        

        /// <summary>
        /// 实例化
        /// </summary>
        public Commend(long frameNum, int memberId, OptionType opType)
        {
            FrameNum = frameNum;
            MemberId = memberId;
            OpType = opType;
            Param = new Dictionary<string, string>();
        }

    }

    /// <summary>
    /// 逻辑帧数据包装
    /// </summary>
    [Serializable]
    public class FramePacker
    {
        public List<IOptionCommand> CommandList = new List<IOptionCommand>();
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OptionType
    {
        None,           // 无操作
        Create,         // 创建单位
        Move,           // 单位移动
        Attack,         // 单位攻击
        Dead,           // 单位死亡
        FightEnd,       // 战斗结束
        EnterServer,    // 加入服务器
        ServerStart,    // 服务器启动
    }

}
