﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// Member管理器
    /// 负责控制Member的逻辑与等待
    /// </summary>
    public class MemberManager : SingleItem<MemberManager>, IMemberManager
    {

        /// <summary>
        /// 当前帧数
        /// </summary>
        public long FrameCount { get { return frameCount; } }

        /// <summary>
        /// 数据黑板
        /// </summary>
        public BlackBoard BlackBoard { get { return blackBorad; } }


        /// <summary>
        /// 当前帧数
        /// </summary>
        private long frameCount = 0;

        /// <summary>
        /// 数据黑板
        /// </summary>
        public BlackBoard blackBorad = null;

        /// <summary>
        /// member集合
        /// </summary>
        private List<IMember> memberList = new List<IMember>();



        /// <summary>
        /// 实例化
        /// </summary>
        public MemberManager([NotNull]MapBase mapBase)
        {
            blackBorad = new BlackBoard() { MapBase = mapBase };
        }


        /// <summary>
        /// 执行
        /// </summary>
        public void Do()
        {
            for (var i = 0; i < memberList.Count; i++)
            {
                var member = memberList[i];
                if (!member.CheckWait(frameCount))
                {
                    member.Do(frameCount);
                }
            }
            frameCount++;
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
        /// 删除成员
        /// </summary>
        /// <param name="member"></param>
        public void Remove(IMember member)
        {
            memberList.Remove(member);
        }

        /// <summary>
        /// 重置逻辑管理器
        /// </summary>
        public void Reset()
        {
            frameCount = 0;
            memberList.Clear();
        }
    }

    /// <summary>
    /// 数据黑板
    /// </summary>
    public class BlackBoard : IBlackBoard
    {
        /// <summary>
        /// 地图数据
        /// </summary>
        public MapBase MapBase { get; set; }
    }

}
