using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 对象池加载器
/// </summary>
public class PoolLoader : SingleItem<PoolLoader>
{

    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<string, UnityEngine.Object> pool = new Dictionary<string, UnityEngine.Object>();


    /// <summary>
    /// 加载对象-Res方式
    /// </summary>
    /// <param name="path">被加载路径</param>
    /// <param name="name">单位名称</param>
    /// <param name="parent">单位父级</param>
    /// <returns>对象单位</returns>
    public GameObject Load(string path, string name = null, Transform parent = null)
    {
        GameObject result = null;
        if (pool.ContainsKey(path))
        {
            // 实例化新对象
            result = GameObject.Instantiate((GameObject)pool[path]);
        }
        else
        {
            try
            {
                var obj = ResourcesLoader.Single.Load(path);
                pool.Add(path, obj);
                // 实例化新对象
                result = GameObject.Instantiate(obj);
                // 移出屏幕
                obj.transform.position = new Vector3(9999999, 9999999);

                // 设置父级
                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }
            }
            catch
            {
                Debug.LogError("对象不存在:" + path);
                throw;
            }
        }
        // 设置父级
        if (parent != null)
        {
            result.transform.SetParent(parent);
        }
        if (result != null)
        {
            result.transform.position = Vector3.zero;
        }
        if (name != null)
        {
            result.name = name;
        }
        return result;
    }

    /// <summary>
    /// 泛型加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public T LoadForType<T>(string path) where T : UnityEngine.Object
    {

        T result;
        if (pool.ContainsKey(path))
        {
            // 实例化新对象
            result = (T)pool[path];
        }
        else
        {
            result = ResourcesLoader.Single.LoadForType<T>(path);
            pool.Add(path, result);
        }
        return result;
    }

    ///// <summary>
    ///// 加载对象-AB包方式
    ///// </summary>
    ///// <param name="key">预设名称(带后缀)</param>
    ///// <param name="package">包名称</param>
    ///// <param name="parent">父级</param>
    ///// <returns></returns>
    //public GameObject Load(string key, string package, Transform parent = null)
    //{
    //    GameObject result = null;
    //    if (pool.ContainsKey(key))
    //    {
    //        result = GameObject.Instantiate(pool[key]);
    //        result.transform.parent = parent;
    //        result.SetActive(true);
    //        return result;
    //    }
    //    else
    //    {
    //        var newObj = ResourcesLoader.Single.Load(key, package);
    //        newObj.transform.parent = parent;
    //        pool.Add(key, newObj);
    //        result = GameObject.Instantiate(newObj);
    //        newObj.SetActive(false);
    //    }
    //    if (result == null)
    //    {
    //        throw new Exception("包:" + package + ", key:" + key + "不存在.");
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// 回收对象
    ///// </summary>
    ///// <param name="obj">对象实例</param>
    ///// <param name="clean">清理对象方法 默认为空</param>
    //public void CircleBack(GameObject obj, Action clean = null)
    //{
    //    // 销毁单位
    //    GameObject.Destroy(obj);
    //    if (clean != null)
    //    {
    //        clean();
    //    }


    //}

    /// <summary>
    /// 销毁对象
    /// </summary>
    /// <param name="obj">被销毁对象</param>
    /// <param name="clean">销毁完毕回调</param>
    public void Destory(GameObject obj, Action clean = null)
    {

        // 销毁单位
        GameObject.Destroy(obj);
        if (clean != null)
        {
            clean();
        }
    }

    /// <summary>
    /// 清理数据
    /// </summary>
    public void Clear()
    {
        pool.Clear();
    }
}
