using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 连线特效控制器
/// </summary>
public class LineRandererControl : MonoBehaviour
{
    /// <summary>
    /// 连线类型
    /// </summary>
    public int LineType = 0;
    /// <summary>
    /// 释放位置
    /// </summary>
    public Vector3 ReleasePos;

    /// <summary>
    /// 接收位置
    /// </summary>
    public Vector3 ReceivePos;

    /// <summary>
    /// 释放对象
    /// </summary>
    public GameObject ReleaseObj;

    /// <summary>
    /// 接收对象
    /// </summary>
    public GameObject ReceiveObj;



    /// <summary>
    /// 连线特效渲染器
    /// </summary>
    private LineRenderer lineRenderer;


    void Start()
    {
        // 获取lineRanderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            throw new Exception("lineRanderer 不存在, 加载失败.");
        }

        // 设置起始与结束点
        if (LineType == 0)
        {
            lineRenderer.SetPositions(new[] { ReleasePos, ReceivePos });
        }
        else
        {
            lineRenderer.SetPositions(new[] { ReleaseObj.transform.position,
                ReceiveObj.transform.position });
        }
    }


    void Update()
    {
        if (LineType == 1 && lineRenderer != null)
        {
            // 重新设置连线特效的两端位置
            lineRenderer.SetPositions(new[] { ReleaseObj.transform.position,
                ReceiveObj.transform.position });
        }
    }


}