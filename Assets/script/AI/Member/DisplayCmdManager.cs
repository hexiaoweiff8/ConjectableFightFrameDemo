﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// 显示命令管理器
    /// </summary>
    public class DisplayCmdManager : SingleItem<DisplayCmdManager>, IDisplayCmdManager
    {

        /// <summary>
        /// 是否为显示模式
        /// </summary>
        public bool ShowMode { get; set; }

        /// <summary>
        /// 显示命令列表
        /// </summary>
        private List<IDisplayCommand> memberDisplayList = new List<IDisplayCommand>();

        /// <summary>
        /// 命令删除列表
        /// </summary>
        private List<IDisplayCommand> delList = new List<IDisplayCommand>();

        /// <summary>
        /// 执行命令列表
        /// </summary>
        public void Do()
        {
            if (!ShowMode)
            {
                return;
            }
            for (var i = 0; i < memberDisplayList.Count; i++)
            {
                var cmd = memberDisplayList[i];
                switch (cmd.CmdType)
                {
                    case DisplayCommandType.Move:
                        {
                            // 计算移动距离
                            var moveCmd = cmd as MoveDisplayCommand;
                            if (moveCmd != null)
                            {
                                // 计算帧数
                                // 使用帧数做差值进行行进
                                var targetX = moveCmd.X;
                                var targetY = moveCmd.Y;
                                var startX = moveCmd.FromX;
                                var startY = moveCmd.FromY;
                                if (targetX == startX && targetY == startY)
                                {
                                    break;
                                }
                                var startFrame = moveCmd.StartFrame;
                                var nowFrame = MemberManager.Single.FrameCount;
                                var totalFrame = moveCmd.TotalFrame;
                                // 差值求当前位置
                                var process = (nowFrame - startFrame) / (float)totalFrame;
                                var nowX = Mathf.Lerp(startX, targetX, process);
                                var nowY = Mathf.Lerp(startY, targetY, process);
                                // TODO 应封装, 与unity脱离耦合
                                moveCmd.MemberDisplay.Obj.transform.position = new Vector3(nowX, nowY);
                            }
                        }
                        break;

                    case DisplayCommandType.Wait:
                        break;
                }

                // 检测是否执行结束
                if (MemberManager.Single.FrameCount >= cmd.StartFrame + cmd.TotalFrame)
                {
                    delList.Add(cmd);
                }
            }

            // 删除
            foreach (var del in delList)
            {
                memberDisplayList.Remove(del);
            }
            delList.Clear();
        }


        /// <summary>
        /// 添加显示命令
        /// </summary>
        /// <param name="memberDisplay"></param>
        public void Add(IDisplayCommand memberDisplay)
        {
            memberDisplayList.Add(memberDisplay);
        }

        /// <summary>
        /// 删除显示指令
        /// </summary>
        /// <param name="memberDisplay"></param>
        public void Remove(IDisplayCommand memberDisplay)
        {
            memberDisplayList.Remove(memberDisplay);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            memberDisplayList.Clear();
        }
    }

    /// <summary>
    /// 显示管理器接口
    /// </summary>
    public interface IDisplayCmdManager
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Do();

        /// <summary>
        /// 添加显示命令
        /// </summary>
        /// <param name="memberDisplay"></param>
        void Add(IDisplayCommand memberDisplay);

        /// <summary>
        /// 删除显示指令
        /// </summary>
        /// <param name="memberDisplay"></param>
        void Remove(IDisplayCommand memberDisplay);

        /// <summary>
        /// 清理
        /// </summary>
        void Clear();
    }
}
