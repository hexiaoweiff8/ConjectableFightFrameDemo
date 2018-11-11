using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 设置检测战斗结束
        /// </summary>
        /// <param name="check"></param>
        public void SetCheckFightEndFunc(Func<List<IMember>, bool> check)
        {
            checkFightEnd = check;
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

}
