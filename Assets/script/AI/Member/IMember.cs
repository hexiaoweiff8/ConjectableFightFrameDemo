﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// member的逻辑接口
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// 单位Id
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// 显示单位
        /// </summary>
        IMemberDisplay DisplayMember { get; set; }

        /// <summary>
        /// 位置X
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// 位置Y
        /// </summary>
        int Y { get; set; }

        /// <summary>
        /// 移动速度
        /// </summary>
        int Speed { get; set; }

        /// <summary>
        /// 生命值
        /// </summary>
        int Hp { get; set; }

        /// <summary>
        /// 执行
        /// </summary>
        void Do(long frame, IBlackBoard blackBoard);

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        void SendCmd(IOptionCommand cmd);

        /// <summary>
        /// 处理操作命令
        /// </summary>
        /// <param name="cmd"></param>
        void Dispatch(IOptionCommand cmd);

        /// <summary>
        /// 等待
        /// 等待帧数
        /// </summary>
        void Wait(long targetFrame);

        /// <summary>
        /// 等待默认帧数
        /// </summary>
        void Wait();

        /// <summary>
        /// 检查是否在等待
        /// </summary>
        /// <returns></returns>
        bool CheckWait(long frame);

    }

    /// <summary>
    /// 单位显示类
    /// </summary>
    public interface IMemberDisplay
    {
        /// <summary>
        /// 显示单位
        /// </summary>
        GameObject Obj { get; set; }

        /// <summary>
        /// 执行显示命令
        /// </summary>
        /// <param name="cmd"></param>
        int Do(IDisplayCommand cmd);


        /// <summary>
        /// 删除显示单位
        /// </summary>
        void Remove();
    }

    /// <summary>
    /// member管理器接口
    /// </summary>
    public interface IMemberManager
    {
        /// <summary>
        /// 帧数
        /// </summary>
        long FrameCount { get; }

        /// <summary>
        /// 成员数量
        /// </summary>
        int MemberCount { get; }

        /// <summary>
        /// 执行
        /// </summary>
        void Do();

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        void SendCmd(IOptionCommand cmd);

        /// <summary>
        /// 处理操作命令
        /// </summary>
        /// <param name="cmd"></param>
        void Dispatch(IOptionCommand cmd);

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="member"></param>
        void Add(IMember member);

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMember Get(int index);

        /// <summary>
        /// 删除一个单位
        /// </summary>
        /// <param name="member"></param>
        void Remove(IMember member);

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();
    }


    /// <summary>
    /// 数据黑板接口
    /// </summary>
    public interface IBlackBoard
    {
        MapBase MapBase { get; set; }

        /// <summary>
        /// 清理
        /// </summary>
        void Clear();

    }


    /// <summary>
    /// 命令借口
    /// </summary>
    public interface IDisplayCommand
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        DisplayCommandType CmdType { get; }

        /// <summary>
        /// 成员逻辑部分
        /// </summary>
        IMember Member { get; set; }

        /// <summary>
        /// 成员显示部分
        /// </summary>
        IMemberDisplay MemberDisplay { get; set; }

        /// <summary>
        /// 总共执行的帧数
        /// </summary>
        int TotalFrame { get; set; }

        /// <summary>
        /// 启动帧数
        /// </summary>
        long StartFrame { get; set; }

    }

    /// <summary>
    /// 操作接口
    /// </summary>
    public interface IOptionCommand
    {

        long FrameNum { get; set; }
        /// <summary>
        /// 单位Id
        /// </summary>
        int MemberId { get; set; }

        // 操作类型
        // 出生
        // 移动
        // 攻击
        // 死亡
        OptionType OpType { get; set; }

        // 操作数据
        // 出生位置, 血量
        // 移动目标位置
        // 攻击目标
        // 死亡标志
        Dictionary<string, string> Param { get; set; }
    }

    /// <summary>
    /// 命令类型
    /// </summary>
    public enum DisplayCommandType
    {
        Move = 1,   // 移动
        Wait = 2,   // 等待

    }
}
