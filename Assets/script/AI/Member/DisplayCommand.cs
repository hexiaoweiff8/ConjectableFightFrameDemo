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
        public DisplayCommandType CmdType { get {return DisplayCommandType.Move;} }

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
        /// 移动来源X
        /// </summary>
        public int FromX { get; private set; }

        /// <summary>
        /// 移动来源Y
        /// </summary>
        public int FromY { get; private set; }

        /// <summary>
        /// 初始化移动命令
        /// TODO 添加来源位置
        /// </summary>
        public MoveDisplayCommand(int fromX, int fromY, int toX, int toY, IMember member, IMemberDisplay memberDisplay)
        {
            this.X = toX * BlackBoard.Single.MapBase.UnitWidth;
            this.Y = toY * BlackBoard.Single.MapBase.UnitWidth;
            this.Member = member;
            this.MemberDisplay = memberDisplay;
            FromX = fromX * BlackBoard.Single.MapBase.UnitWidth;
            FromY = fromY * BlackBoard.Single.MapBase.UnitWidth;
            member.X = toX;
            member.Y = toY;
            // 设置启动帧数
            StartFrame = MemberManager.Single.FrameCount;
            TotalFrame = CalculateFrameCount();
        }

        /// <summary>
        /// 计算运行帧数
        /// </summary>
        /// <returns></returns>
        private int CalculateFrameCount()
        {
            var offsetX = Math.Abs(X - FromX);
            var offsetY = Math.Abs(Y - FromY);

            var dis = Math.Sqrt(offsetX*offsetX + offsetY*offsetY);

            var speed = Member.Speed;
            if (speed <= 0)
            {
                speed = 1;
            }
            var time = dis / speed;

            return (int) Math.Ceiling(time);
        }
    }
}
