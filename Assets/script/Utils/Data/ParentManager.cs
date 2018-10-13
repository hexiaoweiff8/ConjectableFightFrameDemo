using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 父级管理
/// </summary>
public class ParentManager : SingleItem<ParentManager>
{
    /// <summary>
    /// 集群父级
    /// </summary>
    public const string ClusterParent = "ClusterPatent";

    /// <summary>
    /// 建筑父级
    /// </summary>
    public const string BuildingParent = "BuildingParent";

    /// <summary>
    /// 障碍物父级
    /// </summary>
    public const string ObstacleParent = "ObstacleParent";

    /// <summary>
    /// 弹道父级
    /// </summary>
    public const string BallisticParent = "BallisticParent";

    /// <summary>
    /// 缓存父级
    /// </summary>
    public const string PoolParent = "PoolParent";

    /// <summary>
    /// 父级dic
    /// </summary>
    private Dictionary<string, GameObject> parentDic = new Dictionary<string, GameObject>();

    /// <summary>
    /// 获取父级
    /// 如果不存在则创建
    /// </summary>
    /// <param name="key"></param>
    /// <returns>返回一个obj, 不会为null</returns>
    public GameObject GetParent(string key)
    {
        if (!parentDic.ContainsKey(key))
        {
            var parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            parent.name = key;
            parentDic.Add(key, parent);
        }
        else if (parentDic[key] == null)
        {
            var parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            parent.name = key;
            parentDic[key] = parent;
        }
        return parentDic[key];
    }

    /// <summary>
    /// 设置父级
    /// </summary>
    /// <param name="obj">被设置父级对象</param>
    /// <param name="key">被设置父级key</param>
    public void SetParent(GameObject obj, string key)
    {
        if (obj != null && !string.IsNullOrEmpty(key))
        {
            obj.transform.parent = GetParent(key).transform;
        }
    }

    /// <summary>
    /// 将父级Obj放入dic
    /// </summary>
    /// <param name="key"></param>
    /// <param name="parent"></param>
    public void PutParent(string key, GameObject parent)
    {
        if (string.IsNullOrEmpty(key) || parent == null)
        {
            return;
        }
        parentDic.Add(key, parent);
    }

    /// <summary>
    /// 删除父级
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        parentDic.Remove(key);
    }

    /// <summary>
    /// 清空父级
    /// </summary>
    public void Clear()
    {
        foreach (var parent in parentDic)
        {
            GameObject.Destroy(parent.Value);
        }
        parentDic.Clear();
    }
}