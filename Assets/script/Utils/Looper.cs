using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 循环器挂载脚本
/// </summary>
public class Looper : MonoBehaviour
{
    public static Looper Single
    {
        get
        {
            if (single == null)
            {
                AutoInstance();
            }
            return single;
        }
    }

    private static Looper single = null;

    /// <summary>
    /// 初始化looper对象
    /// </summary>
    public static void AutoInstance()
    {
        if (single != null)
        {
            return;
        }
        // 创建单位并设置为DontDestroy, 并且挂上Looper脚本
        var looper = new GameObject("Looper");
        GameObject.DontDestroyOnLoad(looper);
        looper.AddComponent<Looper>();
    }

    void Start()
    {
        // 只保留一个
        if (single != null)
        {
            Destroy(gameObject);
        }
        else
        {
            single = this;
        }
    }

    void Update()
    {
        // 执行循环
        LooperManager.Single.DoLoop();
    }
}


/// <summary>
/// 循环器
/// </summary>
/// <typeparam name="T"></typeparam>
public class LooperManager : LooperAbstract<ILoopItem>
{
    /// <summary>
    /// 单例
    /// </summary>
    public static LooperManager Single
    {
        get
        {
            if (single == null)
            {
                Looper.AutoInstance();
                single = new LooperManager();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static LooperManager single = null;
}

/// <summary>
/// 循环器虚类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class LooperAbstract<T> : ILooper<T> where T : ILoopItem
{
    /// <summary>
    /// 执行单位键值列表
    /// </summary>
    private IDictionary<long, T> itemDic = new Dictionary<long, T>();

    /// <summary>
    /// 删除列表
    /// </summary>
    private List<long> delList = new List<long>(); 

    /// <summary>
    /// 待添加列表
    /// </summary>
    private IDictionary<long, T> addList = new Dictionary<long, T>(); 

    /// <summary>
    /// 单位键ID 自增
    /// </summary>
    private static long keyId = 1024;

    /// <summary>
    /// 是否暂停
    /// </summary>
    private static bool isPause = false;


    /// <summary>
    /// 执行列表
    /// </summary>
    public void DoLoop()
    {
        // 暂停功能
        if (isPause)
        {
            return;
        }

        var keySet = itemDic.Keys;
        // 执行列表中的单位
        foreach (var key in keySet)
        {
            var item = itemDic[key];
            if (!item.IsEnd())
            {
                item.Do();
            }
            else
            {
                delList.Add(key);
            }
        }

        // 删除执行完毕对象
        foreach (var removeItem in delList)
        {
            Remove(removeItem);
        }
        // 添加对象
        foreach (var kv in addList)
        {
            itemDic.Add(kv.Key, kv.Value);
        }
        // 清空删除列表
        delList.Clear();
        addList.Clear();
    }

    /// <summary>
    /// 将执行单位加入执行列表
    /// </summary>
    /// <param name="item">执行单位</param>
    /// <returns>执行列表中的key, 如果key为-1则说明已存在列表中, -2则说明传入值为null</returns>
    public long Add(T item)
    {
        if (item == null)
        {
            // 值为空
            throw new Exception("循环器单位item为空.");
        }
        // 判断列表中是否包含
        if (itemDic.Values.Contains(item))
        {
            return -1;
        }
        addList.Add(keyId, item);
        return keyId++;
    }

    /// <summary>
    /// 删除执行单位
    /// </summary>
    /// <param name="key">执行单位key(来自于Add返回的key)</param>
    public void Remove(long key)
    {
        if (Contains(key))
        {
            // 执行onDestroy
            itemDic[key].OnDestroy();
            itemDic.Remove(key);
        }
    }

    /// <summary>
    /// 判断是否包含指定Key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(long key)
    {
        if (itemDic.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// TODO 分等级
    /// </summary>
    /// <param name="level">清除等级</param>
    public void Clear(int level = -1)
    {
        if (level < 0)
        {
            itemDic.Clear();
        }
    }

    /// <summary>
    /// 暂停执行
    /// </summary>
    public void Pause()
    {
        isPause = true;
    }

    /// <summary>
    /// 终止暂停
    /// </summary>
    public void AntiPause()
    {
        isPause = false;
    }
}

///// <summary>
///// 循环器单位工厂
///// </summary>
//public class LooperItemFactory
//{
    
//}

/// <summary>
/// 循环单位实体
/// </summary>
public class LoopItemImpl : ILoopItem
{
    /// <summary>
    /// 执行Action
    /// 赋值该属性则再每帧执行该方法
    /// </summary>
    public Action DoAction { get; set; }

    /// <summary>
    /// 是否执行结束
    /// 赋值该属性则再每帧判断是否为true, 为true则删除该执行
    /// 为false则继续执行
    /// </summary>
    public Func<bool> IsEndFunc { get; set; }

    /// <summary>
    /// 销毁调用
    /// 赋值该属性则在销毁时调用
    /// </summary>
    public Action DoOnDestory { get; set; }

    /// <summary>
    /// 单次循环
    /// </summary>
    public void Do()
    {
        if (DoAction != null)
        {
            DoAction();
        }
    }

    /// <summary>
    /// 是否执行完毕
    /// </summary>
    /// <returns>是否执行完毕</returns>
    public bool IsEnd()
    {
        if (IsEndFunc != null)
        {
            return IsEndFunc();
        }
        return false;
    }

    /// <summary>
    /// 被销毁时执行
    /// </summary>
    public void OnDestroy()
    {
        if (DoOnDestory != null)
        {
            DoOnDestory();
        }
    }
}



/// <summary>
/// 循环器借口
/// </summary>
public interface ILooper<in T> where T : ILoopItem
{
    /// <summary>
    /// 执行循环
    /// </summary>
    void DoLoop();

    /// <summary>
    /// 添加执行单位
    /// </summary>
    /// <param name="item">执行单位</param>
    /// <returns>单位唯一key</returns>
    long Add(T item);

    /// <summary>
    /// 删除执行单位
    /// </summary>
    /// <param name="key">执行单位key</param>
    void Remove(long key);

    /// <summary>
    /// 暂停
    /// </summary>
    void Pause();

    /// <summary>
    /// 结束暂停
    /// </summary>
    void AntiPause();

    /// <summary>
    /// 清空执行列表
    /// TODO 执行单位分等级
    /// 清空可以全清空或清空某一等级的单位
    /// </summary>
    void Clear(int level);

}

/// <summary>
/// 执行单位接口
/// </summary>
public interface ILoopItem
{
    /// <summary>
    /// 单次循环
    /// </summary>
    void Do();

    /// <summary>
    /// 是否执行完毕
    /// </summary>
    /// <returns></returns>
    bool IsEnd();

    /// <summary>
    /// 被销毁时执行
    /// </summary>
    void OnDestroy();

}