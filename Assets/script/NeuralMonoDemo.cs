using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神经网络游戏Demo
/// </summary>
public class NeuralMonoDemo : MonoBehaviour {

    // 初始化网络结构
    // 规则, 随机出现障碍, 单位碰到障碍重置在安全位置,
    // 单位被重置时训练其前进向障碍物反方向
    // 每个单位有攻击范围, 有组件, 可以将其他单位kill, 并将其他单位的组件组装在自己身上
    // 养蛊的方式, 将有用的组件组装在自己身上, 
    // 如何组合, 单位崩溃原则
    // 存活单位放入名人堂
    // 规则制定
    // 前进, 加速, 八方向检测, 逃跑, 追击, 等待, 繁衍, 
    // 神经网络控制状态切换
    // 单位数据导出
    // 

    // 帧同步, 
    // ILRuntime
    // 第一个版本, 躲避障碍
    // 升级神经网络结构, 添加ReLu, 等计算方式
    // 单隐层节点数量粗略估计公式: NH = (NI + Max(No, Nc)) / 2
    // Python ,人脸识别, 人体识别


    // 训练一个单位躲避障碍
    // 单位8输入, 2输出
    // 绘制出数学范围



	void Start () {
        // 创建单位


        // 

	}
	
	void Update () {
		// 启动推测框架
	}
}
