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
        /// 当前帧数
        /// </summary>
        private long frameCount = 0;
        

        /// <summary>
        /// member集合
        /// </summary>
        private List<IMember> memberList = new List<IMember>();
        


        /// <summary>
        /// 执行
        /// </summary>
        public void Do()
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
