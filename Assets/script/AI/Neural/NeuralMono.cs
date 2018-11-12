using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.AI.Neural;
using UnityEngine;

/// <summary>
/// 神经网路运行器
/// </summary>
public class NeuralMono : MonoBehaviour
{
    /// <summary>
    /// 神经网络
    /// </summary>
    public NeuralNet NeuralNet;

    /// <summary>
    /// 输入层数量
    /// </summary>
    public int InNeuralCount;

    /// <summary>
    /// 隐层数量
    /// </summary>
    public int[] HiddenNeuralCount;

    /// <summary>
    /// 输出层数量
    /// </summary>
    public int OutNeuralCount;


    /// <summary>
    /// 启动
    /// </summary>
    protected virtual void Awake()
    {
        // 实例化神经网络
        NeuralNet = new NeuralNet(InNeuralCount, HiddenNeuralCount, OutNeuralCount);
    }


    /// <summary>
    /// 训练网络
    /// </summary>
    /// <param name="inData">输入数据</param>
    /// <param name="outData">期望结果数据</param>
    public void Train(float[] inData, float[] outData)
    {
        NeuralNet.Train(inData, outData);
    }

    /// <summary>
    /// 计算结果
    /// </summary>
    /// <param name="inData">输入数据</param>
    /// <returns>计算结果</returns>
    public float[] Calculate(float[] inData)
    {
        return NeuralNet.Calculate(inData);
    }

}
