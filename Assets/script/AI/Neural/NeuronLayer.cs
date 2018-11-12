using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.AI.Neural
{

    /// <summary>
    /// 神经层
    /// </summary>
    public class NeuronLayer
    {
        /// <summary>
        /// 上一层
        /// </summary>
        public NeuronLayer PreLayer;

        /// <summary>
        /// 下一层
        /// </summary>
        public NeuronLayer NextLayer;

        /// <summary>
        /// 节点列表
        /// </summary>
        public List<NeuronNode> NodeList = new List<NeuronNode>();


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="nodeCount">当前层节点数量</param>
        public NeuronLayer(int nodeCount)
        {
            for (var i = 0; i < nodeCount; i++)
            {
                NodeList.Add(new NeuronNode());
            }
        }


        /// <summary>
        /// 连接层
        /// </summary>
        /// <param name="layer">层类</param>
        public void ConnectLayer([NotNull]NeuronLayer layer)
        {
            NextLayer = layer;
            layer.PreLayer = this;
            for (var i = 0; i < NodeList.Count; i++)
            {
                for (var j = 0; j < NextLayer.NodeList.Count; j++)
                {
                    NodeList[i].ConnectNeuron(NextLayer.NodeList[j]);
                }
            }
        }

        /// <summary>
        /// 计算层值
        /// </summary>
        public void CalculateValue()
        {
            for (var i = 0; i < NodeList.Count; i++)
            {
                NodeList[i].UpdateValue();
            }
        }

        /// <summary>
        /// 计算层误差
        /// </summary>
        public void CalculateError()
        {
            for (var i = 0; i < NodeList.Count; i++)
            {
                NodeList[i].UpdateError();
            }
        }

        /// <summary>
        /// 计算层权重
        /// </summary>
        public void CalculateWeight()
        {
            for (var i = 0; i < NodeList.Count; i++)
            {
                NodeList[i].UpdateWeight();
            }
        }
    }
}
