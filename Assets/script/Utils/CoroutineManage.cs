using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//协程管理器
public class CoroutineManage : MonoBehaviour
{

    /// <summary>
    /// Update事件代理
    /// </summary>
    public delegate void IUpdate();


    /// <summary>
    /// 单例
    /// </summary>
    public static CoroutineManage Single
    {
        get
        {
            if (single == null)
            {
                single = ParentManager.Single.GetParent("CoroutineManage").AddComponent<CoroutineManage>();
                DontDestroyOnLoad(single.gameObject);
            }
            return single;

        }
    }


    /// <summary>
    /// 单例对象
    /// </summary>
    public static CoroutineManage single = null;


    /// <summary>
    /// 启动协程
    /// </summary>
    /// <param name="it"></param>
    public new void StartCoroutine(IEnumerator it)
    {
        base.StartCoroutine(it);
    }

    /// <summary>
    /// 注册Update事件
    /// </summary>
    /// <param name="iupdate"></param>
    public void RegComponentUpdate(IUpdate iupdate)
    {
        m_NeedRemove.Remove(iupdate);
        m_NeedAdd.Add(iupdate);
    }

    /// <summary>
    /// 注销update事件
    /// </summary>
    /// <param name="iupdate"></param>
    public void UnRegComponentUpdate(IUpdate iupdate)
    {
        m_NeedAdd.Remove(iupdate);
        m_NeedRemove.Add(iupdate);
    }

    public void Update()
    {
        //调用update委托 
        foreach (IUpdate curr in m_UpdateDelegates)
            curr();

        //加入update委托
        {
            foreach (IUpdate curr in m_NeedAdd) m_UpdateDelegates.Add(curr);

            m_NeedAdd.Clear();
        }

        //删除已经无效的update委托
        {
            foreach (IUpdate curr in m_NeedRemove) m_UpdateDelegates.Remove(curr);

            m_NeedRemove.Clear();
        }


    }

    HashSet<IUpdate> m_NeedRemove = new HashSet<IUpdate>();
    HashSet<IUpdate> m_UpdateDelegates = new HashSet<IUpdate>();
    HashSet<IUpdate> m_NeedAdd = new HashSet<IUpdate>();
}
