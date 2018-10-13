using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.script.AI.Member
{

    /// <summary>
    /// 单位成员
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 执行Ai
        /// </summary>
        public void DoAi()
        {
            // 随机获取目标位置
            // 前进
            var path = AStarPathFinding.SearchRoad(null, 1, 1, 1, 1, 1, 1);

            // 如果到达路径尽头, 重新寻路继续前进
            // 重点是结构

            // 性能-拆分结构

            // 单帧任务数

            // 超出任务延帧执行

        }
        // 移动-格子

        // 线性同余

        // 简单AI 寻路向目标

    }
}
