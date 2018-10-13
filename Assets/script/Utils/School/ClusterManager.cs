using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 集群管理
/// 集群行为都在这里集中实现
/// </summary>
public class ClusterManager : ILoopItem
{

    public static ClusterManager Single
    {
        get
        {
            if (single == null)
            {
                single = new ClusterManager();
                looperNum = LooperManager.Single.Add(single);
            }
            return single;
        }
    }


    private static ClusterManager single = null;

    // -------------------------公有属性-------------------------------

    /// <summary>
    /// 地图宽度
    /// </summary>
    public float MapWidth;

    /// <summary>
    /// 地图高度
    /// </summary>
    public float MapHeight;

    /// <summary>
    /// 碰撞拥挤权重
    /// </summary>
    public float CollisionWeight = 5f;

    /// <summary>
    /// 单位格子宽度
    /// </summary>
    public int UnitWidth = 1;


    // -------------------------私有属性-------------------------------

    /// <summary>
    /// 极限速度
    /// </summary>
    private float upTopSpeed = 100f;

    /// <summary>
    /// 目标列表
    /// </summary>
    private TargetList<PositionObject> targetList;

    /// <summary>
    /// 已对比碰撞对象ID的列表
    /// </summary>
    private Dictionary<long, bool> areadyCollisionList = new Dictionary<long, bool>();

    /// <summary>
    /// 暂停标志
    /// </summary>
    private bool pause = false;

    /// <summary>
    /// 是否停止标志
    /// </summary>
    private bool isStop = false;

    /// <summary>
    /// 在循环器中的ID编号
    /// </summary>
    private static long looperNum = -1;

    // -----------------------------公有方法------------------------------


    /// <summary>
    /// 单次循环
    /// </summary>
    public void Do()
    {
        if (targetList != null)
        {
            // 刷新四叉树
            targetList.Refresh();
            // 刷新地图对应位置
            //targetList.RebulidMapInfo();
            // 单位移动
            AllMemberMove(targetList.List);
        #if UNITY_EDITOR
            // 绘制四叉树
            DrawQuadTreeLine(targetList.QuadTree);
        #endif
        }
    }

    /// <summary>
    /// 是否执行完毕
    /// </summary>
    /// <returns>是否执行结束标志</returns>
    public bool IsEnd()
    {
        return isStop;
    }

    /// <summary>
    /// 被销毁时执行
    /// </summary>
    public void OnDestroy()
    {
        // TODO 清空数据
        // 防止内存泄漏
    }

    /// <summary>
    /// 加入单位
    /// </summary>
    /// <param name="member">单位</param>
    public void Add(PositionObject member)
    {
        targetList.Add(member);
    }

    /// <summary>
    /// 删除对象
    /// </summary>
    /// <param name="member">被删除对象</param>
    public void Remove(PositionObject member)
    {
        // 将member中的数据清除
        member.Clear();
        targetList.Remove(member);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="x">四叉树位置x</param>
    /// <param name="y">四叉树位置y</param>
    /// <param name="w">四叉树宽度</param>
    /// <param name="h">四叉树高度</param>
    /// <param name="unitw">地面单位格宽度</param>
    public void Init(float x, float y, int w, int h, int unitw)
    {
        targetList = new TargetList<PositionObject>(x, y, w, h, unitw);
        MapHeight = h;
        MapWidth = w;
        UnitWidth = unitw;
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        pause = true;
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void GoOn()
    {
        pause = false;
    }

    ///// <summary>
    ///// 停止并销毁该集群管理
    ///// </summary>
    //private void Stop()
    //{
    //    ClearAll();
    //    isStop = true;
    //    LooperManager.Single.Remove(looperNum);
    //    single = null;
    //}


    /// <summary>
    /// 清除所有组
    /// </summary>
    public void ClearAll()
    {
        // 清除数据
        if (targetList != null)
        {
            foreach (var item in targetList.List)
            {
                item.Clear();
            }
        }
        if (targetList != null)
        {
            targetList.Clear();
        }
    }

    /// <summary>
    /// 获取图形范围内的单位
    /// </summary>
    /// <param name="graphics">图形对象</param>
    /// <returns>范围内单位列表</returns>
    public IList<PositionObject> GetPositionObjectListByGraphics(ICollisionGraphics graphics)
    {
        if (targetList == null)
        {
            return null;
        }
        Utils.DrawGraphics(graphics, Color.white);
        IList<PositionObject> result = targetList.QuadTree.GetScope(graphics);
        return result;
    }

    /// <summary>
    /// 检测范围内单位
    /// </summary>
    /// <param name="pos">检测位置</param>
    /// <param name="range">检测半径</param>
    /// <param name="myCamp">当前单位阵营</param>
    /// <param name="isExceptMyCamp">是否排除己方阵营</param>
    /// <returns>范围内单位</returns>
    public IList<PositionObject> CheckRange(Vector2 pos, float range, int myCamp = -1, bool isExceptMyCamp = false)
    {
        return CheckRange(new CircleGraphics(pos, range), myCamp, isExceptMyCamp);
    }

    /// <summary>
    /// 检测范围内单位
    /// </summary>
    /// <param name="graphics">范围图形</param>
    /// <param name="myCamp">当前单位阵营</param>
    /// <param name="isExceptMyCamp">是否排除己方阵营</param>
    /// <returns>范围内单位</returns>
    public IList<PositionObject> CheckRange(ICollisionGraphics graphics, int myCamp = -1, bool isExceptMyCamp = false)
    {
        var memberInSightScope = GetPositionObjectListByGraphics(graphics);

        IList<PositionObject> list = new List<PositionObject>();
        if (memberInSightScope != null)
        {
            foreach (var member in memberInSightScope)
            {
                // 单位有效
                // 区分空地属性
                // 区分阵营
                if (member != null && 
                    member.AllData.MemberData.CurrentHP > 0
                    && (myCamp == -1
                    || (isExceptMyCamp && member.AllData.MemberData.Camp != myCamp)
                    || (!isExceptMyCamp && member.AllData.MemberData.Camp == myCamp))
                    && member.CouldSelect)
                {
                    list.Add(member);
                }
            }
        }

        return list;
    }


    // ------------------------私有方法--------------------------


    /// <summary>
    /// 所有成员判断组队行进与碰撞
    /// </summary>
    /// <param name="memberList">成员列表</param>
    private void AllMemberMove(IList<PositionObject> memberList)
    {
        // 验证数据有效性
        if (memberList == null || memberList.Count == 0 || pause)
        { return; }

        // 遍历所有成员
        for (var i = 0; i < memberList.Count; i++)
        {
            // 当前成员
            var member = memberList[i];
            if (member is ClusterData)
            {
                OneMemberMove(member as ClusterData);
            }
        }

        // 清空对比列表
        areadyCollisionList.Clear();
    }

    /// <summary>
    /// 可移动单位移动
    /// </summary>
    /// <param name="member">单个单位</param>
    private void OneMemberMove(ClusterData member)
    {
        if (member == null)
        {
            return;
        }
        // 计算周围单位碰撞
        GetCloseMemberGrivity(member);

        if (!member.IsMoving)
        {
            return;
        }
        //// 高度控制
        //var heightDiff = member.transform.position.y - member.Height;
        //if (heightDiff != 0)
        //{
        //    member.transform.position = new Vector3(member.transform.position.x, member.Height,
        //        member.transform.position.z);
        //}

        // 单位状态切换
        ChangeMemberState(member);
        // 当前单位到目标的方向
        Vector3 targetDir = Utils.WithOutZ(member.TargetPos - member.Position);
        // 转向角度
        float rotate = 0f;
        // 标准化目标方向
        Vector3 normalizedTargetDir = targetDir.normalized;
        // 计算后最终方向
        var finalDir = GetGtivity(member);
        Debug.DrawLine(member.Position, member.Position + finalDir, Color.cyan);
        // 当前方向与目标方向夹角
        var angleForTarget = Vector3.Dot(normalizedTargetDir, Utils.WithOutZ(member.Direction));

        // 当前单位位置减去周围单位的位置的和, 与最终方向相加, 这个向量做处理, 只能指向目标方向的左右90°之内, 防止调头
        // 获取周围成员(不论敌友, 包括障碍物)的斥力引力
        // 直线移动防止抖动
        if (angleForTarget < 0.999f)
        {
            // 计算转向
            rotate = Vector3.Dot(finalDir.normalized, member.DirectionRight) * 180;
            if (rotate > 180 || rotate < -180)
            {
                rotate += ((int)rotate / 180) * 180 * (Mathf.Sign(rotate));
            }
        }
        // 转向
        member.Rotate = Vector3.up * rotate * member.RotateSpeed * Time.deltaTime;
        member.Position += member.SpeedDirection * Time.deltaTime;
        // 前进
        Debug.DrawLine(member.Position, member.Position + member.SpeedDirection, Color.white);
        
#if UNITY_EDITOR
        member.Do();
#endif
    }

    /// <summary>
    /// 计算引力
    /// </summary>
    /// <param name="member">队员对象</param>
    /// <returns></returns>
    private Vector3 GetGtivity(ClusterData member)
    {
        var result = Vector3.zero;
        
        // 同队伍聚合
        if (member != null) // && member.Group != null
        {
            var grivity = Utils.WithOutZ(member.TargetPos - member.Position);
            // 如果当前方向与引力方向
            // 速度不稳定问题
            member.SpeedDirection = grivity.normalized * member.MaxSpeed;

            // 加入最大速度限制, 防止溢出
            //member.PhysicsInfo.SpeedDirection *= GetUpTopSpeed(member.PhysicsInfo.SpeedDirection.magnitude);
            result = grivity.normalized * member.MaxSpeed;
        }

        return result;
    }

    /// <summary>
    /// 获取同区域内成员引力斥力
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private void GetCloseMemberGrivity(PositionObject member)
    {
        if (member == null)
        {
            return;
        }
        // 遍历附近单位(不论敌友), 检测碰撞并排除碰撞, (挤开效果), 列表中包含障碍物
        var graphics = member.MyCollisionGraphics;
        var closeMemberList = targetList.QuadTree.GetScope(graphics);
        // 碰撞前进方向
        var collisionThoughDir = Vector3.zero;

        if (closeMemberList != null)
        {
            for (var k = 0; k < closeMemberList.Count; k++)
            {
                var closeMember = closeMemberList[k];
                // 如果是自己或者非同组跳过
                if (closeMember.Equals(member) || member.CollisionGroup != closeMember.CollisionGroup)
                {
                    continue;
                }

                // 计算周围人员的位置
                var diffPosition = member.Position - closeMember.Position;
                // 判断两对象是否已计算过, 如果计算过不再计算
                var compereId1 = Utils.GetKey(member.Id, closeMember.Id);
                var compereId2 = Utils.GetKey(closeMember.Id, member.Id);
                if (!areadyCollisionList.ContainsKey(compereId1) &&
                    !areadyCollisionList.ContainsKey(compereId2))
                {
                    // 获取附近单位的图形
                    var closeGraphics = closeMember.MyCollisionGraphics;
                    // 检测当前单位是否与其有碰撞
                    if (graphics.CheckCollision(closeGraphics))
                    {
                        // TODO 最小距离的获取方式需要抽象, 根据不同图形获取对小距离
                        var minDistance = member.Diameter * UnitWidth * 0.5f + closeMember.Diameter * UnitWidth * 0.5f;
                        // 质量比例
                        var qualityRate = Math.Min(member.Quality / closeMember.Quality, CollisionWeight);
                        // 插入深度
                        var insertDis = minDistance - diffPosition.magnitude;
                        // 基础排斥力
                        if (insertDis > 0)
                        {
                            // 插入深度
                            var diffCollisionThoughDir = diffPosition.normalized * (insertDis) * 0.5f;
                            
                            collisionThoughDir += diffCollisionThoughDir / qualityRate;
                            // 碰撞单位是否可移动
                            if (closeMember.CouldMove)
                            {
                                var offPos = diffCollisionThoughDir*qualityRate;
                                // 直接设置未碰撞位置
                                closeMember.Position -= offPos;
                            }
                            member.Position += diffCollisionThoughDir;
                        }
                        // 影响速度
                        member.SpeedDirection += collisionThoughDir;

                        // 加入最大速度限制, 防止溢出
                        member.SpeedDirection *= GetUpTopSpeed(member.SpeedDirection.magnitude);
                        closeMember.SpeedDirection *= GetUpTopSpeed(closeMember.SpeedDirection.magnitude);
                        // 加入已对比列表
                        areadyCollisionList.Add(compereId1, true);
                        Debug.DrawLine(member.Position, collisionThoughDir + member.Position, Color.green);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 变更单位状态
    /// </summary>
    /// <param name="member"></param>
    private void ChangeMemberState(ClusterData member)
    {
        if (member == null)
        {
            return;
        }

        if (member.State == SchoolItemState.Unstart)
        {
            member.State = SchoolItemState.Moving;
            if (member.Moveing != null)
            {
                member.Moveing(member.MapCellObj);
            }
        }

        if (member.SpeedDirection.magnitude < 1)
        {
            // 开始等待
            if (member.State != SchoolItemState.Waiting && member.State != SchoolItemState.Complete)
            {
                member.State = SchoolItemState.Waiting;
                if (member.Wait != null)
                {
                    member.Wait(member.MapCellObj);
                }
            }
        }
        else
        {
            // 根据角度获得差速, 直线移动最快
            if (member.State != SchoolItemState.Moving && member.State != SchoolItemState.Complete)
            {
                // 结束等待, 开始移动
                member.State = SchoolItemState.Moving;
                if (member.Moveing != null)
                {
                    member.Moveing(member.MapCellObj);
                }
            }
        }

        if (Utils.WithOutZ(member.Position - member.TargetPos).magnitude < (member.Diameter * 0.5f + 0.5f) * UnitWidth)
        {
            if (member.State != SchoolItemState.Complete)
            {
                //member.Group.CompleteMemberCount++;
                // 将单位的下一位置pop出来 如果没有则
                if (!member.PopTarget())
                {
                    // 单位状态修改为complete
                    member.State = SchoolItemState.Complete;
                    // 调用到达
                    if (member.Complete != null) { member.Complete(member.MapCellObj); }
                }
            }
        }
    }


    /// <summary>
    /// 控制极限速度
    /// </summary>
    /// <param name="speed">当前速度</param>
    /// <returns>如果speed超过极限速度则将其置为极限速度系数</returns>
    private float GetUpTopSpeed(float speed)
    {
        var result = 1f;
        if (speed > upTopSpeed)
        {
            result = upTopSpeed / speed;
        }
        return result;
    }


    /// <summary>
    /// 绘制单元位置与四叉树分区情况
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argQuadTree"></param>
    private void DrawQuadTreeLine<T>(QuadTree<T> argQuadTree) where T : IGraphicsHolder
    {
        if (!Debug.logger.logEnabled)
        {
            return;
        }
        var colorForItem = Color.green;
        // 绘制四叉树边框
        Utils.DrawGraphics(argQuadTree.GetRectangle(), Color.white);
        // 遍历四叉树内容
        foreach (var item in argQuadTree.GetItemList())
        {
            // 绘制当前对象
            Utils.DrawGraphics(item.MyCollisionGraphics, colorForItem);
        }

        if (argQuadTree.GetSubNodes()[0] != null)
        {
            foreach (var node in argQuadTree.GetSubNodes())
            {
                DrawQuadTreeLine(node);
            }
        }
    }

}