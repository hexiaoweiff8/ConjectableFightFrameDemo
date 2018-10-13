using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;


/// <summary>
/// 加载器
/// </summary>
public class ResourcesLoader
{
    /// <summary>
    /// 单例
    /// </summary>
    public static ResourcesLoader Single
    {
        get
        {
            if (single == null)
            {
                single = new ResourcesLoader();
            }
            return single;
        }
    }

    /// <summary>
    /// 单例对象
    /// </summary>
    private static ResourcesLoader single = null;

    /// <summary>
    /// 加载特效
    /// </summary>
    /// <param name="key">特效路径</param>
    /// <returns>特效对象</returns>
    public GameObject Load(string key)
    {
        var result = (GameObject)Resources.Load(key);
        if (result != null)
        {
            result = Object.Instantiate(result);
        }
        else
        {
            Debug.Log("Resource not exist:" + key);
        }
        return result;
    }


    /// <summary>
    /// 加载特效
    /// </summary>
    /// <param name="key">特效路径</param>
    /// <returns>特效对象</returns>
    public T LoadForType<T>(string key) where T : Object
    {
        var result = (T)Resources.Load(key, typeof(T));
        if (result == null)
        {
            Debug.Log("Resource not exist:" + key);
        }
        return result;
    }

    ///// <summary>
    ///// 加载Ab包资源
    ///// </summary>
    ///// <param name="key"></param>
    ///// <param name="package"></param>
    ///// <returns></returns>
    //public GameObject Load(string key, string package)
    //{
    //    return GameObjectExtension.InstantiateFromPacket(package, key, null);
    //}
}