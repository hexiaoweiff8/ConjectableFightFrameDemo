using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// 网络单位, 操作来自于网络
    /// </summary>
    public class NetMember : IMember
    {  /// <summary>
       /// 单位Id
       /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 默认等待帧数
        /// </summary>
        public const int DefaultWaitFrameCount = 15;
        /// <summary>
        /// 显示单位
        /// </summary>
        public IMemberDisplay DisplayMember { get; set; }

        /// <summary>
        /// 位置X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 位置Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 移动速度
        /// </summary>
        public int Speed { get; set; }


        /// <summary>
        /// 生命值
        /// </summary>
        public int Hp { get; set; }

        /// <summary>
        /// 是否为AI
        /// </summary>
        public bool IsAI { get; set; }


        /// <summary>
        /// id种子
        /// </summary>
        private static int idSeed = 1;


        /// <summary>
        /// 启动帧
        /// </summary>
        private long actionFrame = 0;

        /// <summary>
        /// 路径
        /// </summary>
        private Stack<Node> pathList = null;

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="nowFrame"></param>
        public NetMember(long nowFrame, IMemberDisplay displayMember)
        {
            actionFrame = nowFrame;
            DisplayMember = displayMember;
            Id = idSeed++;
        }
        // 移动-格子

        // 线性同余

        // 简单AI 寻路向目标

        public void Wait(long targetFrame)
        {
            actionFrame = actionFrame + targetFrame;
        }

        /// <summary>
        /// 检测是否是等待状态
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool CheckWait(long frame)
        {
            return frame < actionFrame;
        }


        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="frame"></param>
        public void Do(long frame, IBlackBoard blackBoard)
        {
            actionFrame = frame;

            // 检查是否死亡

            if (Hp <= 0)
            {
                MemberManager.Single.Remove(this);
                UnityEngine.Debug.Log("单位死亡Id:" + Id);
                return;
            }
            
        }

        /// <summary>
        /// 等待
        /// </summary>
        public void Wait()
        {
            Wait(DefaultWaitFrameCount);
        }
    }
}
