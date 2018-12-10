using Assets.script.AI.Net;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

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
        public bool IsServer = false;


        /// <summary>
        /// 当前帧数
        /// </summary>
        private long frameCount = 0;

        /// <summary>
        /// member集合
        /// </summary>
        private List<IMember> memberList = new List<IMember>();

        /// <summary>
        /// 检测战斗结果
        /// </summary>
        private Func<List<IMember>, bool> checkFightEnd = null;

        /// <summary>
        /// 地址端口
        /// </summary>
        private Dictionary<string, int> addressAndPort = new Dictionary<string, int>();

        /// <summary>
        /// 是否战斗结束
        /// </summary>
        private bool isFighting = false;

        /// <summary>
        /// 反序列化
        /// </summary>
        private BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器


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
            // 启动本地监听
            IsNetMode = true;
            NetManager.Single.StartBind(serverPort);
            NetManager.Single.ServerComputeAction = (bytes) =>
            {
                // 处理数据
                // 反序列化
                var stream = new MemoryStream(bytes);
                var packet = binFormat.Deserialize(stream) as Commend;
                // 处理注册消息
                if (packet.OpType == OptionType.Register)
                {
                    // 获取目标的IP, Port
                    var ip = packet.Param["ip"];
                    var port = int.Parse(packet.Param["port"]);
                    // 缓存地址端口
                    addressAndPort.Add(ip, port);
                }
                else
                {
                    // 转发消息
                    SendToAll(packet); 
                }
            };
        }

        /// <summary>
        /// 初始化网络
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="clientPort"></param>
        public void InitNet(string serverIp, int serverPort, int clientPort)
        {
            IsNetMode = true;
            NetManager.Single.Connect(serverIp, serverPort);
            NetManager.Single.ClientComputeAction = (bytes) =>
            {
                // 处理数据
                // 反序列化
                var stream = new MemoryStream(bytes);
                var packet = binFormat.Deserialize(stream) as Commend;
                // 分发操作
                Dispatch(packet);
            };

            // 发送注册消息
            {
                var regCmd = new Commend(0, 0, OptionType.Register);
                regCmd.Param.Add("ip", Utils.GetLocalIP());
                regCmd.Param.Add("port", "" + clientPort);
                var stream = new MemoryStream();
                binFormat.Serialize(stream, regCmd);
                NetManager.Single.ClientSend(stream.ToArray());
            }
        }
        
        /// <summary>
        /// 执行
        /// </summary>
        public void Do()
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
                            member.Do(frameCount, BlackBoard.Single);
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
        }

        /// <summary>
        /// 发送操作命令
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCmd(IOptionCommand cmd)
        {
            // 发送
            var stream = new MemoryStream();
            binFormat.Serialize(stream, cmd);
            NetManager.Single.ClientSend(stream.ToArray());
        }

        /// <summary>
        /// 转发给所有单位
        /// </summary>
        public void SendToAll(IOptionCommand cmd)
        {
            var stream = new MemoryStream();
            binFormat.Serialize(stream, cmd);
            var data = stream.ToArray();
            foreach(var kv in addressAndPort){
                NetManager.Single.ServerUdpSend(kv.Key, kv.Value, data);
            }
        }

        /// <summary>
        /// 抛出消息
        /// </summary>
        public void Dispatch(IOptionCommand cmd)
        {
            // 如果是单位创建则直接创建单位
            if (cmd.OpType == OptionType.Create)
            {
                // 创建单位
                // 单位Id
                var newId = cmd.MemberId;
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
                MemberManager.Single.Add(member);
            }
            else
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
            isFighting = true;
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
    /// 操作类型
    /// </summary>
    public enum OptionType
    {
        None,
        Register,
        Create,
        Move,
        Attack,
        Dead
    }

}
