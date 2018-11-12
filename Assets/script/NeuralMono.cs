using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神经网络游戏Demo
/// </summary>
public class NeuralMono : MonoBehaviour {

    // 初始化网络结构
    // 规则, 随机出现障碍, 单位碰到障碍重置在安全位置,
    // 单位被重置时训练其前进向障碍物反方向
    // 每个单位有攻击范围, 有组件, 可以将其他单位kill, 并将其他单位的组件组装在自己身上
    // 养蛊的方式, 将有用的组件组装在自己身上, 
    // 如何组合, 单位崩溃原则
    // 存活单位放入名人堂
    // 规则制定

	void Start () {
        // 创建单位
        // 
	}
	
	void Update () {
		// 启动推测框架
	}
}
