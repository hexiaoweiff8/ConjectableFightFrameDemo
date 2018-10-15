using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.script.AI.Member;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// 移动命令
    /// </summary>
    public class MoveDisplayCommand : IDisplayCommand
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        public DisplayCommandType CmdType { get; set; }

        /// <summary>
        /// 逻辑成员
        /// </summary>
        public IMember Member { get; set; }

        /// <summary>
        /// 显示成员
        /// </summary>
        public IMemberDisplay MemberDisplay { get; set; }


        /// <summary>
        /// 总共执行的帧数
        /// </summary>
        public int TotalFrame { get; set; }

        /// <summary>
        /// 启动帧数
        /// </summary>
        public long StartFrame { get; set; }

        /// <summary>
        /// 移动目标X
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 移动目标Y
        /// </summary>
        public int Y { get; private set; }


        /// <summary>
        /// 初始化移动命令
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MoveDisplayCommand(int x, int y, IMember member, IMemberDisplay memberDisplay)
        {
            this.X = x;
            this.Y = y;
            this.Member = member;
            this.MemberDisplay = memberDisplay;
            // 设置启动帧数
            StartFrame = MemberManager.Single.FrameCount;

        }

        /// <summary>
        /// 计算运行帧数
        /// </summary>
        /// <returns></returns>
        private int CalculateFrameCount()
        {


            return 0;
        }
    }
}
