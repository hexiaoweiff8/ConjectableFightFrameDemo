using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.AI.Neural
{
    /// <summary>
    /// 神经网络类
    /// </summary>
    public class NeuralNet
    {
        /// <summary>
        /// 输入层
        /// </summary>
        public NeuronLayer InLayer;

        /// <summary>
        /// 输出层
        /// </summary>
        public NeuronLayer[] HideLayer;

        /// <summary>
        ///  隐层
        /// </summary>
        public NeuronLayer OutLayer;


        /// <summary>
        /// 初始化神经网络
        /// </summary>
        /// <param name="inLayerNodeCount">输入层节点数</param>
        /// <param name="hiddenLayerNodeCount">输出层节点数</param>
        /// <param name="outLayerNodeCount">输出层节点数</param>
        public NeuralNet(int inLayerNodeCount, [NotNull]int[] hiddenLayerNodeCount, int outLayerNodeCount)
        {
            // 输入层
            InLayer = new NeuronLayer(inLayerNodeCount);

            // 隐层
            HideLayer = new NeuronLayer[hiddenLayerNodeCount.Length];

            for (var i = 0; i < hiddenLayerNodeCount.Length; i++)
            {
                HideLayer[i] = new NeuronLayer(hiddenLayerNodeCount[i]);
            }

            // 输出层
            OutLayer = new NeuronLayer(outLayerNodeCount);

            InLayer.ConnectLayer(HideLayer[0]);

            for (var i = 0; i < hiddenLayerNodeCount.Length - 1; i++)
            {
                HideLayer[i].ConnectLayer(HideLayer[i + 1]);
            }

            HideLayer[HideLayer.Length - 1].ConnectLayer(OutLayer);
        }

        /// <summary>
        /// 训练网络
        /// </summary>
        /// <param name="inData">输入数据</param>
        /// <param name="outData">输出数据(期望数据)</param>
        public void Train([NotNull]float[] inData, [NotNull]float[] outData)
        {
            if (InLayer.NodeList.Count != inData.Length || OutLayer.NodeList.Count != outData.Length)
            {
                throw new Exception("输入数据格式与网络不匹配.");
            }

            // 设置输入数据
            for (var i = 0; i < inData.Length; i++)
            {
                InLayer.NodeList[i].InputValue = inData[i];
            }
            // 设置期望值
            for (var i = 0; i < outData.Length; i++)
            {
                OutLayer.NodeList[i].DesireValue = outData[i];
            }

            // 计算值(正向计算)
            InLayer.CalculateValue();
            for (var i = 0; i < HideLayer.Length; i++)
            {
                HideLayer[i].CalculateValue();
            }
            OutLayer.CalculateValue();

            // 计算误差(反向传播, 误差回传)
            OutLayer.CalculateError();
            for (var i = HideLayer.Length - 1; i > 0; i--)
            {
                HideLayer[i].CalculateError();
            }
            InLayer.CalculateError();


            // 计算权重(梯度计算)
            OutLayer.CalculateWeight();
            for (var i = HideLayer.Length - 1; i > 0; i--)
            {
                HideLayer[i].CalculateWeight();
            }
            InLayer.CalculateWeight();


        }


        /// <summary>
        /// 计算值
        /// </summary>
        /// <param name="inData">输入数据</param>
        /// <returns>网络输出数据</returns>
        public float[] Calculate(float[] inData)
        {
            float[] result = new float[OutLayer.NodeList.Count];

            // 输入数据
            for (var i = 0; i < inData.Length; i++)
            {
                InLayer.NodeList[i].InputValue = inData[i];
            }

            // 向前计算
            InLayer.CalculateValue();
            for (var i = 0; i < HideLayer.Length; i++)
            {
                HideLayer[i].CalculateValue();
            }
            OutLayer.CalculateValue();

            // 输出值
            for (var i = 0; i < OutLayer.NodeList.Count; i++)
            {
                result[i] = OutLayer.NodeList[i].Value;
            }

            return result;
        }
    }
}
