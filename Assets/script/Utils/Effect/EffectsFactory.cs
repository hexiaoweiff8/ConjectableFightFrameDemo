using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Util;
using Object = UnityEngine.Object;

// ----------------------------------实体对象-----------------------------------------
/// <summary>
/// 特效工厂
/// </summary>
public class EffectsFactory
{
    /// <summary>
    /// 单例
    /// </summary>
    public static EffectsFactory Single
    {
        get
        {
            if (single == null)
            {
                single = new EffectsFactory();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static EffectsFactory single = null;

    /// <summary>
    /// 特效包名称
    /// </summary>
    public const string EffectPackName = "attackeffect";


    /// <summary>
    /// 创建点特效
    /// 特效只在一个位置上
    /// </summary>
    /// <param name="effectKey"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="scale"></param>
    /// <param name="durTime"></param>
    /// <param name="speed"></param>
    /// <param name="completeCallback">完成回调</param>
    /// <param name="effectLayer">特效所在渲染层</param>
    /// <param name="rotate">x,y旋转量</param>
    /// <returns>特效对象</returns>
    public EffectBehaviorAbstract CreatePointEffect(string effectKey,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        float durTime,
        float speed,
        Action completeCallback = null,
        int effectLayer = 0,
        Vector2 rotate = new Vector2())
    {
        EffectBehaviorAbstract result = null;

        result = new PointEffect(effectKey,
            parent,
            position,
            scale,
            durTime,
            speed,
            completeCallback,
            effectLayer,
            rotate);

        return result;
    }

    /// <summary>
    /// 创建点对点特效
    /// 特效会从start按照速度与轨迹飞到end点
    /// </summary>
    /// <returns>特效对象</returns>
    public EffectBehaviorAbstract CreatePointToPointEffect(string effectKey,
        Transform parent,
        Vector3 position,
        Vector3 to,
        Vector3 scale,
        float speed,
        TrajectoryAlgorithmType flytype = TrajectoryAlgorithmType.Line,
        Action completeCallback = null,
        int effectLayer = 0)
    {
        EffectBehaviorAbstract result = null;

        result = new PointToPointEffect(effectKey,
            parent,
            position,
            to,
            scale,
            speed,
            flytype,
            completeCallback,
            effectLayer);

        return result;
    }


    /// <summary>
    /// 创建点对点特效
    /// 特效会从start按照速度与轨迹飞到目标对象位置
    /// </summary>
    /// <returns>特效对象</returns>
    public EffectBehaviorAbstract CreatePointToObjEffect(string effectKey, 
        Transform parent, 
        Vector3 position, 
        GameObject targetObj, 
        Vector3 scale, 
        float speed, 
        TrajectoryAlgorithmType flytype = TrajectoryAlgorithmType.Line, 
        Action completeCallback = null, 
        int effectLayer = 0)
    {
        EffectBehaviorAbstract result = null;

        result = new PointToTargetEffect(effectKey, parent, position, targetObj, scale, speed, flytype, completeCallback , effectLayer);

        return result;
    }

    /// <summary>
    /// 创建连线特效
    /// </summary>
    /// <param name="effectKey">特效Key</param>
    /// <param name="parent">特效父级</param>
    /// <param name="releasePos">起始点</param>
    /// <param name="receivePos">结束点</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="completeCallback">结束回调</param>
    /// <param name="effectLayer">特效渲染层级</param>
    /// <returns></returns>
    public EffectBehaviorAbstract CreateLinerEffect(string effectKey,
        Transform parent,
        Vector3 releasePos,
        Vector3 receivePos,
        float durTime,
        Action completeCallback = null,
        int effectLayer = 0)
    {
        EffectBehaviorAbstract result = null;

        result = new LinerEffect(effectKey, parent, releasePos, receivePos, durTime, completeCallback, effectLayer);

        return result;
    }



    /// <summary>
    /// 创建连线特效
    /// </summary>
    /// <param name="effectKey">特效Key</param>
    /// <param name="parent">特效父级</param>
    /// <param name="receiveObj">结束对象</param>
    /// <param name="durTime">持续时间</param>
    /// <param name="completeCallback">结束回调</param>
    /// <param name="effectLayer">特效渲染层级</param>
    /// <returns></returns>
    public EffectBehaviorAbstract CreateLinerEffect(string effectKey,
        Transform parent,
        GameObject receiveObj,
        float durTime,
        Action completeCallback = null,
        int effectLayer = 0)
    {
        EffectBehaviorAbstract result = null;

        result = new LinerEffect(effectKey, parent, receiveObj, durTime, completeCallback, effectLayer);

        return result;
    }



    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //public EffectBehaviorAbstract CreateScopeEffect()
    //{
    //    EffectBehaviorAbstract result = null;



    //    return result;
    //}
}


/// <summary>
/// 点特效
/// </summary>
public class PointEffect : EffectBehaviorAbstract
{
    /// <summary>
    /// 特效key
    /// </summary>
    private string effectKey;

    /// <summary>
    /// 父级
    /// </summary>
    private Transform parent;

    /// <summary>
    /// 特效出现位置
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// 特效对象缩放
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// 持续时间
    /// </summary>
    private float durTime;

    /// <summary>
    /// 特效扩散速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 特效对象
    /// </summary>
    private GameObject effectObject;

    /// <summary>
    /// 完成回调
    /// </summary>
    private Action completeCallback;

    /// <summary>
    /// 显示层级
    /// </summary>
    private int layer = 0;

    /// <summary>
    /// x轴,y轴旋转
    /// </summary>
    private Vector2 rotate = Vector2.zero;

    /// <summary>
    /// 创建点特效
    /// </summary>
    /// <param name="effectKey">特效key, 可以使路径, 或者AB包中对应的key</param>
    /// <param name="parent">父级</param>
    /// <param name="position">特效出现位置</param>
    /// <param name="scale">特效缩放</param>
    /// <param name="durTime"></param>
    /// <param name="speed">TODO 特效播放速度</param>
    /// <param name="completeCallback">结束回调</param>
    /// <param name="rotate">x,y旋转量</param>
    public PointEffect(string effectKey, Transform parent, Vector3 position, Vector3 scale, float durTime, float speed, Action completeCallback = null, int effectLayer = 0, Vector2 rotate = new Vector2())
    {
        this.effectKey = effectKey;
        this.parent = parent;
        this.position = position;
        this.scale = scale;
        this.durTime = durTime;
        this.speed = speed;
        this.completeCallback = completeCallback;
        this.layer = effectLayer;
        this.rotate = rotate;
    }

    /// <summary>
    /// 开始
    /// </summary>
    public override void Begin()
    {
        // 加载特效预设
        effectObject = PoolLoader.Single.Load(effectKey, EffectsFactory.EffectPackName, parent);
        if (effectObject == null)
        {
            throw new Exception("特效为空, 加载失败:" + effectKey);
        }
        //effectObject = Instantiate(effectObject);
        // 设置数据
        effectObject.transform.localScale = scale;
        effectObject.transform.position = position;
        effectObject.layer = layer;
        // 单位旋转
        if (rotate != Vector2.zero)
        {
            effectObject.transform.Rotate(Vector3.right, rotate.x);
            effectObject.transform.Rotate(Vector3.up, rotate.y);
        }
        // TODO 特效播放速度
        // 特效持续时间
        new Timer(durTime).OnCompleteCallback(() =>
        {
            if (completeCallback != null)
            {
                completeCallback();
            }
            // 回收对象
            PoolLoader.Single.Destory(effectObject);
            //Destroy();
        }).Start();
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public override void Pause()
    {
        if (effectObject != null)
        {
            effectObject.SetActive(false);
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    public override void AntiPause()
    {
        if (effectObject != null)
        {
            effectObject.SetActive(true);
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        if (effectObject != null)
        {
            // 销毁对象
            GameObject.Destroy(effectObject);
            effectObject = null;
        }
    }
}

/// <summary>
/// 点对点特效
/// </summary>
public class PointToPointEffect : EffectBehaviorAbstract
{
    /// <summary>
    /// 特效key
    /// </summary>
    private string effectKey;

    /// <summary>
    /// 父级
    /// </summary>
    private Transform parent;

    /// <summary>
    /// 特效出现位置
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// 特效目标位置
    /// </summary>
    private Vector3 to;

    /// <summary>
    /// 特效对象缩放
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// 特效扩散速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 特效对象
    /// </summary>
    private GameObject effectObject;

    /// <summary>
    /// 弹道
    /// </summary>
    private Ballistic ballistic;

    /// <summary>
    /// 完成回调
    /// </summary>
    private Action completeCallback;

    /// <summary>
    /// 飞行轨迹
    /// </summary>
    private TrajectoryAlgorithmType flyType = TrajectoryAlgorithmType.Line;

    /// <summary>
    /// 显示层级
    /// </summary>
    private int layer = 0;

    /// <summary>
    /// 点对点特效
    /// TODO 加入路径
    /// </summary>
    /// <param name="effectKey">特效key, 可以使路径, 或者AB包中对应的key</param>
    /// <param name="parent">父级</param>
    /// <param name="from">起点</param>
    /// <param name="to">目标点</param>
    /// <param name="scale">缩放</param>
    /// <param name="speed">速度</param>
    /// <param name="completeCallback">完成回调</param>
    public PointToPointEffect(string effectKey, Transform parent, Vector3 from, Vector3 to, Vector3 scale, float speed, TrajectoryAlgorithmType flyType, Action completeCallback = null, int effectLayer = 0)
    {
        this.effectKey = effectKey;
        this.parent = parent;
        this.position = from;
        this.to = to;
        this.scale = scale;
        this.speed = speed;
        this.flyType = flyType;
        this.completeCallback = completeCallback;
        this.layer = effectLayer;
    }


    /// <summary>
    /// 开始移动
    /// </summary>
    public override void Begin()
    {
        // 加载特效预设
        effectObject = PoolLoader.Single.Load(effectKey, EffectsFactory.EffectPackName, parent);
        if (effectObject == null)
        {
            throw new Exception("特效为空, 加载失败.");
        }
        //effectObject = Instantiate(effectObject);
        // 设置数据
        effectObject.transform.localScale = scale;
        effectObject.transform.position = position;
        effectObject.layer = layer;
        // 开始移动
        // 创建弹道
        ballistic = BallisticFactory.Single.CreateBallistic(effectObject, position, to - position,
                       to,
                       speed, 1,false, trajectoryType: flyType);

        ballistic.OnKill = (ballistic1, target) =>
        {
            GameObject.Destroy(effectObject.GetComponent<Ballistic>());
            // 回收单位
            PoolLoader.Single.Destory(effectObject);
        };

        // 运行完成
        ballistic.OnComplete = (a, b) =>
        {
            if (completeCallback != null)
            {
                completeCallback();
            }
            a.Kill();
            //Destroy();
        };
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public override void Pause()
    {
        if (ballistic != null)
        {
            ballistic.Pause();
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    public override void AntiPause()
    {
        if (ballistic != null)
        {
            ballistic.Start();
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        if (ballistic != null)
        {
            GameObject.Destroy(ballistic.gameObject);
        }
    }
}

/// <summary>
/// 点对对象特效
/// </summary>
public class PointToTargetEffect : EffectBehaviorAbstract
{
    /// <summary>
    /// 特效key
    /// </summary>
    private string effectKey;

    /// <summary>
    /// 父级
    /// </summary>
    private Transform parent;

    /// <summary>
    /// 特效出现位置
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// 目标对象
    /// </summary>
    private GameObject targetObj;

    /// <summary>
    /// 特效对象缩放
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// 特效扩散速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 特效对象
    /// </summary>
    private GameObject effectObject;

    /// <summary>
    /// 弹道
    /// </summary>
    private Ballistic ballistic;

    /// <summary>
    /// 完成回调
    /// </summary>
    private Action completeCallback;

    /// <summary>
    /// 飞行轨迹
    /// </summary>
    private TrajectoryAlgorithmType flyType = TrajectoryAlgorithmType.Line;

    /// <summary>
    /// 显示层级
    /// </summary>
    private int layer = 0;

    /// <summary>
    /// 点对点特效
    /// TODO 加入路径
    /// </summary>
    /// <param name="effectKey">特效key, 可以使路径, 或者AB包中对应的key</param>
    /// <param name="parent">父级</param>
    /// <param name="from">起点</param>
    /// <param name="targetObj">目标对象</param>
    /// <param name="scale">缩放</param>
    /// <param name="speed">速度</param>
    /// <param name="completeCallback">完成回调</param>
    public PointToTargetEffect(string effectKey, Transform parent, Vector3 from, GameObject targetObj, Vector3 scale, float speed, TrajectoryAlgorithmType flyType = TrajectoryAlgorithmType.Line, Action completeCallback = null, int effectLayer = 0)
    {
        this.effectKey = effectKey;
        this.parent = parent;
        this.position = from;
        this.targetObj = targetObj;
        this.scale = scale;
        this.speed = speed;
        this.flyType = flyType;
        this.completeCallback = completeCallback;
        this.layer = effectLayer;
    }


    /// <summary>
    /// 开始移动
    /// </summary>
    public override void Begin()
    {
        // 加载特效预设
        effectObject = PoolLoader.Single.Load(effectKey, EffectsFactory.EffectPackName, parent);
        if (effectObject == null)
        {
            throw new Exception("特效为空, 加载失败.");
        }
        //effectObject = Instantiate(effectObject);
        // 设置数据
        effectObject.transform.localScale = scale;
        effectObject.transform.position = position;
        effectObject.layer = layer;
        // 开始移动
        // 创建弹道
        ballistic = BallisticFactory.Single.CreateBallistic(effectObject, position, targetObj.transform.position - position, targetObj, speed, 1, trajectoryType: flyType);

        ballistic.OnKill = (ballistic1, target) =>
        {
            GameObject.Destroy(effectObject.GetComponent<Ballistic>());
            PoolLoader.Single.Destory(effectObject);
        };
        // 运行完成
        ballistic.OnComplete = (a, b) =>
        {
            if (completeCallback != null)
            {
                completeCallback();
            }
            a.Kill();
            //Destroy();
        };
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public override void Pause()
    {
        if (ballistic != null)
        {
            ballistic.Pause();
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    public override void AntiPause()
    {
        if (ballistic != null)
        {
            ballistic.Start();
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        if (ballistic != null)
        {
            GameObject.Destroy(ballistic.gameObject);
        }
    }
}

/// <summary>
/// 连线特效
/// </summary>
public class LinerEffect : EffectBehaviorAbstract
{
    /// <summary>
    /// 特效key
    /// </summary>
    private string effectKey;

    /// <summary>
    /// 父级单位
    /// </summary>
    private Transform parent;

    /// <summary>
    /// 释放位置
    /// </summary>
    private Vector3 releasePos;

    /// <summary>
    /// 接收位置
    /// </summary>
    private Vector3 receivePos;

    /// <summary>
    /// 接收对象
    /// </summary>
    private GameObject receiveObj;

    /// <summary>
    /// 持续时间
    /// </summary>
    private float durTime;

    /// <summary>
    /// 完成回调
    /// </summary>
    private Action completeCallback = null;

    /// <summary>
    /// 特效渲染层级
    /// </summary>
    private int effectLayer = 0;

    /// <summary>
    /// 特效对象
    /// </summary>
    private GameObject effectObject;

    /// <summary>
    /// 连线类型
    /// 0: 点对点
    /// 1: 对象对对象
    /// </summary>
    private int lineType = 0;


    /// <summary>
    /// 初始化连线特效
    /// </summary>
    /// <param name="effectKey"></param>
    /// <param name="parent"></param>
    /// <param name="releasePos"></param>
    /// <param name="receivePos"></param>
    /// <param name="durTime"></param>
    /// <param name="completeCallback"></param>
    /// <param name="effectLayer"></param>
    public LinerEffect(string effectKey,
        Transform parent,
        Vector3 releasePos,
        Vector3 receivePos,
        float durTime,
        Action completeCallback = null,
        int effectLayer = 0)
    {
        this.effectKey = effectKey;
        this.parent = parent;
        this.releasePos = releasePos;
        this.receivePos = receivePos;
        this.durTime = durTime;
        this.completeCallback = completeCallback;
        this.effectLayer = effectLayer;
        lineType = 0;
    }

    /// <summary>
    /// 初始化连线特效
    /// </summary>
    /// <param name="effectKey"></param>
    /// <param name="parent"></param>
    /// <param name="receiveObj"></param>
    /// <param name="durTime"></param>
    /// <param name="completeCallback"></param>
    /// <param name="effectLayer"></param>
    public LinerEffect(string effectKey,
        Transform parent,
        GameObject receiveObj,
        float durTime,
        Action completeCallback = null,
        int effectLayer = 0)
    {
        this.effectKey = effectKey;
        this.parent = parent;
        this.receiveObj = receiveObj;
        this.durTime = durTime;
        this.completeCallback = completeCallback;
        this.effectLayer = effectLayer;
        lineType = 1;
    }


    /// <summary>
    /// 开始效果
    /// </summary>
    public override void Begin()
    {
        // 加载特效预设
        effectObject = PoolLoader.Single.Load(effectKey, EffectsFactory.EffectPackName, parent);
        if (effectObject == null)
        {
            throw new Exception("特效为空, 加载失败.");
        }
        effectObject.transform.localScale = new Vector3(1, 1, 1);
        effectObject.transform.localPosition = Vector3.zero;

        // 获取lineRanderer
        var lineRandererControl = effectObject.GetComponent<LineRandererControl>();
        if (lineRandererControl == null)
        {
            lineRandererControl = effectObject.AddComponent<LineRandererControl>();
        }

        switch (lineType)
        {
            case 0:
                // 设置两点
                lineRandererControl.ReleasePos = releasePos;
                lineRandererControl.ReceivePos = receivePos;
                break;
            case 1:
                // 设置两对象
                lineRandererControl.ReleaseObj = effectObject;
                lineRandererControl.ReceiveObj = receiveObj;
                break;
        }
        lineRandererControl.LineType = lineType;

        // 定时关闭
        new Timer(durTime).OnCompleteCallback(() =>
        {
            PoolLoader.Single.Destory(effectObject);
        }).Start();
    }

    /// <summary>
    /// 暂停效果
    /// </summary>
    public override void Pause()
    {
        // 暂停
    }

    /// <summary>
    /// 继续
    /// </summary>
    public override void AntiPause()
    {
        // 结束暂停
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        PoolLoader.Single.Destory(effectObject);
    }
}



/// <summary>
/// 范围特效
/// </summary>
public class ScopeEffect : EffectBehaviorAbstract
{

    /// <summary>
    /// 开始移动
    /// </summary>
    public override void Begin()
    {

    }

    /// <summary>
    /// 暂停
    /// </summary>
    public override void Pause()
    {

    }
    
    /// <summary>
    /// 继续
    /// </summary>
    public override void AntiPause()
    {
        
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {

    }
}



// ----------------------------------抽象对象-----------------------------------------

/// <summary>
/// 特效对象
/// </summary>
public abstract class EffectBehaviorAbstract : IEffectsBehavior
{
    /// <summary>
    /// 单位唯一自增ID
    /// </summary>
    public int AdditionId
    {
        get { return additionId++; }
    }

    /// <summary>
    /// 单位唯一自增ID
    /// </summary>
    private static int additionId = 1024;
    /// <summary>
    /// 开始移动
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// 暂停
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// 继续
    /// </summary>
    public abstract void AntiPause();

    /// <summary>
    /// 销毁
    /// </summary>
    public abstract void Destroy();
    
}

/// <summary>
/// 特效行为接口
/// </summary>
public interface IEffectsBehavior
{
    /// <summary>
    /// 开始
    /// </summary>
    void Begin();

    /// <summary>
    /// 暂停
    /// </summary>
    void Pause();

    /// <summary>
    /// 继续
    /// </summary>
    void AntiPause();

    /// <summary>
    /// 销毁
    /// </summary>
    void Destroy();
}