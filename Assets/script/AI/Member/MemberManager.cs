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
        /// 初始化网络
        /// </summary>
        /// <param name="ServerIp"></param>
        /// <param name="ServerPort"></param>
        public void InitNet(string ServerIp, int ServerPort)
        {
            IsNetMode = true;
            NetManager.Single.Connect(ServerIp, ServerPort);
            NetManager.Single.ComputeAction = (bytes) => { 
                // 处理数据
                // 反序列化
                var stream = new MemoryStream(bytes);
                var packet = binFormat.Deserialize(stream) as Packet;
                // 分发操作
                RouteOption(packet);
            };
        }

        /// <summary>
        /// 分发操作
        /// </summary>
        /// <param name="packet"></param>
        public void RouteOption(Packet packet)
        {
            if (packet.OpType == OptionType.Create)
            {
                // 创建单位
                // 单位Id
                var newId = int.Parse(packet.Param["id"]);
                // 初始位置
                var posX = int.Parse(packet.Param["posX"]);
                var posY = int.Parse(packet.Param["posY"]);
                // 血量
                var hp = int.Parse(packet.Param["hp"]);

                

            }
            var targetMember = memberList.Find((member) => member.Id == packet.MemberId);
            if (targetMember != null)
            {
                // 进行操作

                switch (packet.OpType)
                {
                    case OptionType.Attack:
                        // 单位攻击
                        {
                            // 攻击目标
                            var atkTarId = int.Parse(packet.Param["tarId"]);
                            // 攻击方式
                            var atkType = int.Parse(packet.Param["atkType"]);
                            // 攻击伤害
                            var dmg = int.Parse(packet.Param["dmg"]);
                        }
                        break;
                    case OptionType.Move:
                        // 单位移动
                        {
                            // 移动目标
                            var toX = int.Parse(packet.Param["toX"]);
                            var toY = int.Parse(packet.Param["toY"]);
                            // 移动来源
                            var fromX = int.Parse(packet.Param["fromX"]);
                            var fromY = int.Parse(packet.Param["fromY"]);
                        }
                        break;
                    case OptionType.None:
                        // 无操作
                        {
                            Debug.LogError("无操作");
                        }
                        break;
                    case OptionType.Dead:
                        // 单位死亡
                        {
                            // 击杀者
                            var killerId = int.Parse(packet.Param["killerId"]);
                        }
                        break;
                }
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
        /// 添加成员
        /// </summary>
        /// <param name="member"></param>
        public void Add(IMember member)
        {
            memberList.Add(member);
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
    public class Packet
    {
        /// <summary>
        /// 单位Id
        /// </summary>
        public int MemberId;

        // 操作类型
        // 出生
        // 移动
        // 攻击
        // 死亡
        public OptionType OpType = OptionType.None;

        // 操作数据
        // 出生位置, 血量
        // 移动目标位置
        // 攻击目标
        // 死亡标志
        public Dictionary<string, string> Param = new Dictionary<string, string>();

    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OptionType
    {
        None,
        Create,
        Move,
        Attack,
        Dead
    }

}
