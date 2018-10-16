using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.script.AI.Member
{
    public class MemberDisplay : IMemberDisplay
    {

        /// <summary>
        /// 显示单位
        /// </summary>
        public GameObject Obj { get; set; }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="obj"></param>
        public MemberDisplay(GameObject obj)
        {
            Obj = obj;
        }

        /// <summary>
        /// 执行显示命令
        /// </summary>
        /// <param name="cmd"></param>
        public int Do(IDisplayCommand cmd)
        {
            return DoCommand(cmd);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>等待帧数</returns>
        private int DoCommand(IDisplayCommand cmd)
        {
            // 将命令添加进管理器
            DisplayCmdManager.Single.Add(cmd);

            // 计算运行帧数
            return cmd.TotalFrame;
        }
    }
}
