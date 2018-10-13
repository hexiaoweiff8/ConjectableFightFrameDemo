using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 弹道生成工厂
/// </summary>
public class BallisticFactory
{
    /// <summary>
    /// 弹道工厂单例
    /// </summary>
    public static BallisticFactory Single
    {
        get
        {
            if (single == null)
            {
                single = new BallisticFactory();
            }
            return single;
        }
    }

    /// <summary>
    /// 弹道工厂单例引用
    /// </summary>
    private static BallisticFactory single = null;



    /// <summary>
    /// 创建弹道1
    /// 自由飞行, 使用最飞行距离限制
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="direction">飞行起始方向</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="maxDistance">最大飞行距离</param>
    /// <param name="radius">物体半径</param>
    /// <param name="hasGravity">是否受重力影响, 默认是</param>
    /// <param name="grivity">重力系数, 默认9.8</param>
    /// <returns>自由飞行弹道</returns>
    public Ballistic CreateBallistic(GameObject ball, 
        Vector3 startPos,
        Vector3 direction, 
        float speed, 
        float maxDistance, 
        float radius, 
        bool hasGravity = true, 
        float grivity = 9.8f)
    {
        if (ball == null)
        {
            return null;
        }
        var result = CreateBallistic(ball,
            startPos,
            direction,
            speed,
            radius,
            hasGravity,
            grivity);
        result.BallisticArriveTarget = new BallisticArriveTargetForFreeFly(maxDistance);

        // 移动
        result.Move = TrajectoryAlgorithm.Single.GetAlgorithm(TrajectoryAlgorithmType.Line);
        // TODO 飞行方式也放入工厂中组合

        return result;
    }

    /// <summary>
    /// 创建弹道2
    /// 目标点, 到达目标点为
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="direction">飞行起始方向</param>
    /// <param name="targetPos">目标点</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="radius">物体半径</param>
    /// <param name="hasGravity">是否受重力影响</param>
    /// <param name="gravity">重力系数, 默认9.8</param>
    /// <param name="turnaroundWeight">转向权重</param>
    /// <returns>目标点飞行弹道</returns>
    public Ballistic CreateBallistic(GameObject ball, 
        Vector3 startPos,
        Vector3 direction,
        Vector3 targetPos, 
        float speed, 
        float radius, 
        bool hasGravity = true, 
        float gravity = 9.8f,
        float turnaroundWeight = 1,
        TrajectoryAlgorithmType trajectoryType = TrajectoryAlgorithmType.Parabola)
    {
        if (ball == null)
        {
            return null;
        }

        // 创建基础弹道数据
        var result = CreateBallistic(ball,
            startPos,
            direction,
            speed,
            radius,
            hasGravity,
            gravity);
        result.BallisticArriveTarget = new BallisticArriveTargetForPosition(targetPos);

        
        // 移动方法
        result.Move = TrajectoryAlgorithm.Single.GetAlgorithm(trajectoryType);
        return result;
    }

    /// <summary>
    /// 创建弹道3
    /// 目标对象, 到达目标点
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="direction">飞行起始方向</param>
    /// <param name="targetObj">目标对象</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="radius">物体半径</param>
    /// <param name="hasGravity">是否受重力影响</param>
    /// <param name="grivity">重力系数, 默认9.8</param>
    /// <returns>目标点飞行弹道</returns>
    public Ballistic CreateBallistic(GameObject ball, 
        Vector3 startPos, 
        Vector3 direction, 
        GameObject targetObj, 
        float speed, 
        float radius, 
        bool hasGravity = true, 
        float grivity = 9.8f,
        TrajectoryAlgorithmType trajectoryType = TrajectoryAlgorithmType.Parabola)
    {
        if (ball == null)
        {
            return null;
        }
        var result = CreateBallistic(ball,
            startPos,
            direction,
            speed,
            radius,
            hasGravity,
            grivity);
        result.BallisticArriveTarget = new BallisticArriveTargetForObj(targetObj);

        // 移动方法
        result.Move = TrajectoryAlgorithm.Single.GetAlgorithm(trajectoryType);
        // 起始时运动一次, 防止出现一帧原始模型状态
        result.Move(result, result.BallisticArriveTarget);
        return result;
    }


    /// <summary>
    /// 创建基础弹道
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <param name="startPos">起始位置</param>
    /// <param name="direction">飞行起始方向</param>
    /// <param name="speed">飞行速度</param>
    /// <param name="radius">物体半径</param>
    /// <param name="hasGravity">是否受重力影响, 默认是</param>
    /// <param name="grivity">重力系数, 默认9.8</param>
    /// <returns>基础弹道</returns>
    private Ballistic CreateBallistic(GameObject ball, Vector3 startPos,
        Vector3 direction,
        float speed,
        float radius,
        bool hasGravity = true,
        float grivity = 9.8f)
    {
        if (ball == null)
        {
            return null;
        }
        // 设置父级
        ParentManager.Single.SetParent(ball, ParentManager.BallisticParent);
        var result = ball.AddComponent<Ballistic>();

        result.StartPos = startPos;
        result.Position = startPos;
        result.Direction = direction;
        result.Speed = speed;
        result.Radius = radius;
        result.HasGravity = hasGravity;
        result.Gravity = grivity;
        

        return result;
    }
}


/// <summary>
/// 弹道到达目标点判断抽象类
/// </summary>
public abstract class BallisticArriveTarget
{
    /// <summary>
    /// 是否到达目标
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <returns>是否到达</returns>
    public abstract bool IsArrivedTarget(Ballistic ball);
}


/// <summary>
/// 判断是否到达, 目标点方式
/// </summary>
public class BallisticArriveTargetForPosition : BallisticArriveTarget
{
    /// <summary>
    /// 目标地点
    /// </summary>
    public Vector3 TargetPosition { get; set; }

    /// <summary>
    /// 初始化, 将目标位置传入
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public BallisticArriveTargetForPosition(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;
    }

    /// <summary>
    /// 是否到达目标
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <returns>是否到达</returns>
    public override bool IsArrivedTarget(Ballistic ball)
    {
        var result = false;
        if (ball != null)
        {
            // 判断位置+半径是否到达目标
            var nowDir = (TargetPosition - ball.Position);
            if (nowDir.magnitude <= ball.Speed * Time.deltaTime)
            {
                result = true;
            }
        }
        // TODO
        Debug.DrawRay(TargetPosition, Vector3.up * 10);
        return result;
    }
}

/// <summary>
/// 判断是否到达类, 目标对象方式
/// </summary>
public class BallisticArriveTargetForObj : BallisticArriveTarget
{

    /// <summary>
    /// 目标对象
    /// </summary>
    public GameObject TargetObj { get; set; }

    /// <summary>
    /// 最后一次位置
    /// </summary>
    public Vector3 LasttimePos;

    /// <summary>
    /// 初始化目标对象
    /// </summary>
    /// <param name="targetObj"></param>
    public BallisticArriveTargetForObj(GameObject targetObj)
    {
        TargetObj = targetObj;
    }

    /// <summary>
    /// 是否到达目标
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <returns>是否到达</returns>
    public override bool IsArrivedTarget(Ballistic ball)
    {
        var result = false;
        if (ball != null && TargetObj != null)
        {
            // 判断位置+半径是否到达目标
            if (TargetObj != null)
            {
                if ((ball.Position - TargetObj.transform.position).magnitude <= ball.Speed * Time.deltaTime)
                {
                    LasttimePos = TargetObj.transform.position;
                    result = true;
                }
            }
            else
            {
                if ((ball.Position - LasttimePos).magnitude <= ball.Speed * Time.deltaTime)
                {
                    result = true;
                }
            }
        }

        return result;
    }
}


/// <summary>
/// 判断是否到达类, 自由飞方式
/// </summary>
public class BallisticArriveTargetForFreeFly : BallisticArriveTarget
{

    /// <summary>
    /// 自由飞情况下最大飞行距离
    /// </summary>
    public float FreeFlyMaxLength { get; set; }

    /// <summary>
    /// 缓存上次位置
    /// </summary>
    private Vector3 prePostion;

    /// <summary>
    /// 移动距离
    /// </summary>
    private float moveDistance = 0f;

    /// <summary>
    /// 初始化飞行距离
    /// </summary>
    /// <param name="freeFlyMaxLength"></param>
    public BallisticArriveTargetForFreeFly(float freeFlyMaxLength)
    {
        FreeFlyMaxLength = freeFlyMaxLength;
    }


    /// <summary>
    /// 是否到达目标
    /// </summary>
    /// <param name="ball">弹道对象</param>
    /// <returns>是否到达</returns>
    public override bool IsArrivedTarget(Ballistic ball)
    {
        var result = false;
        if (ball != null)
        {
            // 累加移动距离
            if (prePostion != Vector3.zero)
            {
                moveDistance += (ball.Position - prePostion).magnitude;
                if (moveDistance >= FreeFlyMaxLength)
                {
                    result = true;
                }
            }
            else
            {
                prePostion = ball.Position;
            }
        }

        return result;
    }
}

/// <summary>
/// 轨迹算法类
/// </summary>
public class TrajectoryAlgorithm
{
    /// <summary>
    /// 轨迹算法单例
    /// </summary>
    public static TrajectoryAlgorithm Single
    {
        get
        {
            if (single == null)
            {
                single = new TrajectoryAlgorithm();
            }
            return single;
        }
    }

    /// <summary>
    /// 轨迹算法单例
    /// </summary>
    private static TrajectoryAlgorithm single;

    /// <summary>
    /// 获取飞行算法
    /// </summary>
    /// <param name="type">算法类型</param>
    /// <returns>具体算法, 如果列表中不存在该算法类型, 返回null</returns>
    public Action<Ballistic, BallisticArriveTarget> GetAlgorithm(TrajectoryAlgorithmType type)
    {
        Action<Ballistic, BallisticArriveTarget> result = null;
        switch (type)
        {
            // 抛物线移动
            case TrajectoryAlgorithmType.Parabola:
                {
                    #region Parabola
                    result = (ballistic, ballisticArriveTarget) =>
                    {

                        if (ballistic != null && ballisticArriveTarget != null)
                        {
                            var tmpTargetPos = GetTargetPos(ballistic, ballisticArriveTarget);
                            var targetDir = tmpTargetPos - ballistic.StartPos;
                            if (targetDir == Vector3.zero)
                            {
                                targetDir = Vector3.one;
                            }
                            // 判断是否有重力效果
                            if (!ballistic.HasGravity)
                            {
                                var tmpDir = ballistic.Direction.normalized * ballistic.Speed;
                                ballistic.transform.forward = tmpDir;
                                ballistic.Position += tmpDir * Time.deltaTime;
                                // 走直线直奔目标
                                ballistic.Direction = targetDir;
                            }
                            else
                            {
                                // 使用时间计算当前位置
                                var passedTime = ballistic.PassedTime;

                                // xz平面向量
                                //var targetPosWithoutY = new Vector2(tmpTargetPos.x, tmpTargetPos.z);
                                //var theta = Math.Acos(Vector2.Dot(targetPosWithoutY.normalized, targetDir.z < 0 ? Vector2.left : Vector2.right));
                                // 计算位移比例
                                //var xOffsetProportion = (float)Math.Cos(theta + (targetDir.x < 0 ? Math.PI : 0));
                                //var zOffsetProportion = (float)Math.Sin(theta + (targetDir.z < 0 ? Math.PI : 0));
                                var normalizedTargetDir = targetDir.normalized;
                                var xOffsetProportion = normalizedTargetDir.x;
                                var zOffsetProportion = normalizedTargetDir.z;
                                // X位移
                                var offsetX = ballistic.Speed * xOffsetProportion * passedTime + ballistic.StartPos.x;
                                // Z位移
                                var offsetZ = ballistic.Speed * zOffsetProportion * passedTime + ballistic.StartPos.z;

                                // 求起点与目标点的中心点
                                var center = (ballistic.StartPos + tmpTargetPos) * 0.5f;
                                // 向下位移, 与弧线垂直
                                center -= new Vector3(0, 10 * ballistic.Gravity, 0);
                                // 求中心点至两端的向量
                                var centerToStart = ballistic.StartPos - center;
                                var centerToEnd = tmpTargetPos - center;
                                // 使用两向量求差值

                                // Y位移 按照位置设置曲线
                                var radianVec = Vector3.Slerp(centerToStart, centerToEnd, (ballistic.Position - ballistic.StartPos).magnitude / (targetDir.magnitude)) + center;//speed * (float)Math.Sin(ballistic.ShootTheta) * passedTime - gravity * passedTime * passedTime / 2;

                                // 计算新位置
                                var nowPos = Vector3.zero;
                                nowPos.x = offsetX;
                                nowPos.z = offsetZ;
                                nowPos.y = radianVec.y;

                                // 设置新位置
                                ballistic.Position = nowPos;
                                // 累加已飞行时间
                                ballistic.PassedTime += Time.deltaTime;
                            }

                            // 判断如果超过目标则设置为到达目标
                            // 检测位置是否超过目标, 检测目标起始点与结束点的x,z如果与当前位置到结束点的x,z符号不同则判断其超过目标, 设置其位置为目标位置
                            // 当前位置到目标点的向量
                            var nowDir = tmpTargetPos - ballistic.Position;
                            if (Math.Sign(nowDir.x) != Math.Sign(targetDir.x) || Math.Sign(nowDir.z) != Math.Sign(targetDir.z))
                            {
                                // 超过目标位置, 设置其位置为目标位置
                                ballistic.Position = tmpTargetPos;
                            }
                        }

                    };
                    #endregion
                }
                break;
            // 直线移动
            case TrajectoryAlgorithmType.Line:
                {
                    #region Line
                    result = (ballistic, ballisticArriveTarget) =>
                    {
                        if (ballistic != null && ballisticArriveTarget != null)
                        {
                            // 目标点
                            var target = GetTargetPos(ballistic, ballisticArriveTarget);
                            // 计算当前移动
                            var tmpDir = ballistic.Direction.normalized * ballistic.Speed;
                            ballistic.transform.forward = tmpDir;
                            ballistic.Position += tmpDir * Time.deltaTime;
                            ballistic.Direction = (target - ballistic.Position).normalized;
                        }
                    };
                    #endregion
                }
                break;
            // 正弦线移动
            case TrajectoryAlgorithmType.Sine:
                {
                    #region Sine
                    result = (ballistic, ballisticArriveTarget) =>
                    {
                        if (ballistic != null && ballisticArriveTarget != null)
                        {
                            // 目位置
                            var tmpTargetPos = GetTargetPos(ballistic, ballisticArriveTarget);
                            // 目标方向
                            // TODO 移动目标可能会有问题
                            var targetDir = tmpTargetPos - ballistic.StartPos;
                            var nowDir = tmpTargetPos - ballistic.Position;
                            // 使用时间计算当前位置
                            var passedTime = ballistic.PassedTime;

                            // X,Z向目标移动
                            // xz平面向量
                            var targetPosWithoutY = Utils.WithOutZ(tmpTargetPos);
                            var theta = Math.Acos(Vector2.Dot(targetPosWithoutY.normalized, targetDir.z < 0 ? Vector2.left : Vector2.right));
                            // 计算位移比例
                            var xOffsetProportion = (float)Math.Cos(theta + (targetDir.z < 0 ? Math.PI : 0));
                            var zOffsetProportion = (float)Math.Sin(theta + (targetDir.z < 0 ? Math.PI : 0));
                            // X位移
                            var offsetX = ballistic.Speed * xOffsetProportion * passedTime;
                            // Z位移
                            var offsetZ = ballistic.Speed * zOffsetProportion * passedTime;

                            // 计算新位置
                            var nowPos = Vector3.zero;
                            nowPos.x = offsetX;
                            nowPos.z = offsetZ;
                            // Y轴震荡

                            // 求起点与目标点的中心点
                            var center = (ballistic.StartPos + tmpTargetPos) * 0.5f;
                            // 向下位移, 与弧线垂直
                            center -= new Vector3(0, 10 * ballistic.Gravity, 0);
                            // 求中心点至两端的向量
                            var centerToStart = ballistic.StartPos - center;
                            var centerToEnd = tmpTargetPos - center;
                            // 使用两向量求差值

                            // Y位移 按照位置设置曲线
                            var radianVec = Vector3.Slerp(centerToStart, centerToEnd, (ballistic.Position - ballistic.StartPos).magnitude / (targetDir.magnitude)) + center;//speed * (float)Math.Sin(ballistic.ShootTheta) * passedTime - gravity * passedTime * passedTime / 2;

                            // 震荡单位长度
                            var shockLen = 50f;
                            nowPos.y = radianVec.y + (float)Math.Sin((nowDir.magnitude % shockLen) * 2 / shockLen * Math.PI);
                            // X,Z轴震荡

                            // 设置新位置
                            ballistic.Position = nowPos + ballistic.StartPos;
                            // 累加已飞行时间
                            ballistic.PassedTime += Time.deltaTime;

                            if (Math.Sign(nowDir.x) != Math.Sign(targetDir.x) || Math.Sign(nowDir.z) != Math.Sign(targetDir.z))
                            {
                                // 超过目标位置, 设置其位置为目标位置
                                ballistic.Position = tmpTargetPos;
                            }
                        }

                    };
                    #endregion
                }
                break;

        }

        return result;
    }

    /// <summary>
    /// 根据类型获取目标点
    /// </summary>
    /// <param name="ball">当前移动对象</param>
    /// <param name="ballisticTarget">对象弹道判断类</param>
    /// <returns>目标点</returns>
    private static Vector3 GetTargetPos(Ballistic ball, BallisticArriveTarget ballisticTarget)
    {
        Vector3 result = Vector3.zero;

        if (ball != null && ballisticTarget != null)
        {

            if (ballisticTarget is BallisticArriveTargetForFreeFly)
            {
                result = ball.Direction.normalized*((BallisticArriveTargetForFreeFly)ballisticTarget).FreeFlyMaxLength;
            }
            else if (ballisticTarget is BallisticArriveTargetForObj)
            {
                var objTarget = ballisticTarget as BallisticArriveTargetForObj;
                if (objTarget.TargetObj != null)
                {
                    result = objTarget.TargetObj.transform.position;
                }
                else
                {
                    result = objTarget.LasttimePos;
                }
            }
            else if (ballisticTarget is BallisticArriveTargetForPosition)
            {
                result = ((BallisticArriveTargetForPosition) ballisticTarget).TargetPosition;
            }
        }

        return result;
    }
}


public class BallisticMoveCollection : ILoopItem
{
    /// <summary>
    /// 弹道运行列表
    /// </summary>
    private IList<Ballistic> ballisticList= new List<Ballistic>(); 


    public void Do()
    {
        //BallisticMoveCollection
    }

    public bool IsEnd()
    {
        return false;
    }

    public void OnDestroy()
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 轨迹算法类型
/// </summary>
public enum TrajectoryAlgorithmType
{
    Parabola = 0, // 抛物线
    Line = 1, // 直线
    Sine = 2, // 正弦线
}