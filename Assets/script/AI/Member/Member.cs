using System;
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
        public Member(long nowFrame, IMemberDisplay displayMember)
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

                    var path =
                        AStarPathFinding.SearchRoad(
                            BlackBoard.Single.MapBase.GetMapArray(MapManager.MapObstacleLayer),
                            X, Y,
                            targetX, targetY, 1, 1);

                    if (path != null && path.Count > 0)
                    {
                        couldNotPass = false;
                        pathList = new Stack<Node>(path.ToArray());
                    }

                    var index = RandomPacker.Single.GetRangeI(0, MemberManager.Single.MemberCount);

                    // 随机攻击一个目标
                    var targetMember = MemberManager.Single.Get(index);
                    if (targetMember != null)
                    {
                        targetMember.Hp -= 10;
                        UnityEngine.Debug.Log(Id + "攻击Id:" + targetMember.Id + " Hp:" + 10);
                    }
                }

                // 向目标寻路, 如果不可达继续寻路

                var nextNode = pathList.Pop();
                UnityEngine.Debug.Log(Id + " from" + X + "," + Y + " to" + targetX + "," + targetY + "Hp:" + Hp);
                // 跑出显示命令, 并等待显示部分反馈的帧数

                // TODO 发送操作


            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void Dispatch(IOptionCommand cmd)
        {
            if (cmd.OpType == OptionType.Create)
            {
                // 创建单位
                // 单位Id
                var newId = int.Parse(cmd.Param["id"]);
                // 初始位置
                var posX = int.Parse(cmd.Param["posX"]);
                var posY = int.Parse(cmd.Param["posY"]);
                // 血量
                var hp = int.Parse(cmd.Param["hp"]);



            }
            // 进行操作

            switch (cmd.OpType)
            {
                case OptionType.Attack:
                    // 单位攻击
                {
                    // 攻击目标
                    var atkTarId = int.Parse(cmd.Param["tarId"]);
                    // 攻击方式
                    var atkType = int.Parse(cmd.Param["atkType"]);
                    // 攻击伤害
                    var dmg = int.Parse(cmd.Param["dmg"]);
                    Hp -= dmg;
                }
                    break;
                case OptionType.Move:
                    // 单位移动
                {
                    // 移动目标
                    var toX = int.Parse(cmd.Param["toX"]);
                    var toY = int.Parse(cmd.Param["toY"]);
                    // 移动来源
                    var fromX = int.Parse(cmd.Param["fromX"]);
                    var fromY = int.Parse(cmd.Param["fromY"]);
                        // 验证来源
                    if (this.X != fromX || Y != fromY)
                    {
                        UnityEngine.Debug.LogError("数据异常, 刷新位置");
                    }
                    this.Wait(DisplayMember.Do(new MoveDisplayCommand(toX, toY, this, DisplayMember)));
                }
                    break;
                case OptionType.None:
                    // 无操作
                {
                    UnityEngine.Debug.LogError("无操作");
                }
                    break;
                case OptionType.Dead:
                    // 单位死亡
                {
                    // 击杀者
                    var killerId = int.Parse(cmd.Param["killerId"]);
                }
                    break;
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
