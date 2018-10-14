using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.script.AI.Member
{
    /// <summary>
    /// member的逻辑接口
    /// </summary>
    public interface IMember
    {

        /// <summary>
        /// 执行
        /// </summary>
        void Do(long frame, IBlackBoard blackBoard);

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
    /// member管理器接口
    /// </summary>
    public interface IMemberManager
    {

        /// <summary>
        /// 执行
        /// </summary>
        void Do();

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="member"></param>
        void Add(IMember member);

        /// <summary>
        /// 删除一个单位
        /// </summary>
        /// <param name="id"></param>
        void Remove(IMember member);

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// 显示管理器接口
    /// </summary>
    public interface IMemberDisplayManager
    {

        void Do();




    }

    /// <summary>
    /// 数据黑板接口
    /// </summary>
    public interface IBlackBoard
    {
        MapBase MapBase { get; set; }

    }
}
