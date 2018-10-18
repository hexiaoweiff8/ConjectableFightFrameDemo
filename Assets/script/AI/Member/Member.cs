﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.script.AI.Member
{

    /// <summary>
    /// 单位成员(逻辑)
    /// </summary>
    public class Member : IMember
    {

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
        public Member(long nowFrame, IMemberDisplay displayMember)
        {
            actionFrame = nowFrame;
            DisplayMember = displayMember;
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

            if (pathList != null && pathList.Count > 0)
            {
                // 继续前进
                var nextNode = pathList.Pop();
                // 跑出显示命令, 并等待显示部分反馈的帧数
                this.Wait(DisplayMember.Do(new MoveDisplayCommand(nextNode.X, nextNode.Y, this, DisplayMember)));
            }
            else
            {

                var width = blackBoard.MapBase.MapWidth;
                var height = blackBoard.MapBase.MapHeight;

                var couldNotPass = true;
                int targetX = 0;
                int targetY = 0;

                while (couldNotPass)
                {
                    // 随机获取目标位置
                    targetX = RandomPacker.Single.GetRangeI(0, width);
                    targetY = RandomPacker.Single.GetRangeI(0, height);

                    var path = AStarPathFinding.SearchRoad(BlackBoard.Single.MapBase.GetMapArray(MapManager.MapObstacleLayer),
                            X, Y,
                            targetX, targetY, 1, 1);

                    if (path != null && path.Count > 0)
                    {
                        couldNotPass = false;
                        pathList = new Stack<Node>(path.ToArray());
                    }
                }

                // 向目标寻路, 如果不可达继续寻路
                
                var nextNode = pathList.Pop();
                UnityEngine.Debug.Log("from" + X + "," + Y + " to" + targetX + "," + targetY);
                // 跑出显示命令, 并等待显示部分反馈的帧数
                this.Wait(DisplayMember.Do(new MoveDisplayCommand(nextNode.X, nextNode.Y, this, DisplayMember)));

            }

            // 重点是结构

            // 性能-拆分结构

            // 单帧任务数

            // 超出任务延帧执行
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
