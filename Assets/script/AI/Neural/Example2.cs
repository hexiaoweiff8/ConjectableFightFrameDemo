using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.AI.Neural
{
    public class Example2 : MonoBehaviour
    {
        /// <summary>
        /// 网络类
        /// </summary>
        public NeuralMono NeuralMono;

        /// <summary>
        /// 左侧探针
        /// </summary>
        public GameObject LeftSensor;

        /// <summary>
        /// 右侧探针
        /// </summary>
        public GameObject RightSensor;

        /// <summary>
        /// 上侧探针
        /// </summary>
        public GameObject UpSensor;

        /// <summary>
        /// 下侧探针
        /// </summary>
        public GameObject DownSensor;

        /// <summary>
        /// 训练次数
        /// </summary>
        public int TrainTimes = 10000;


        /// <summary>
        /// 起始位置
        /// </summary>
        private Vector3 startPos;

        /// <summary>
        /// 起始转向
        /// </summary>
        private Quaternion startRotate;


        /// <summary>
        /// 网络输入数据
        /// </summary>
        private float[] inDatas = new float[4];

        /// <summary>
        /// 网络输出数据
        /// </summary>
        private float[] outDatas = new float[2];

        /// <summary>
        /// 训练数据
        /// </summary>
        private float[][] trainDatas = new float[4][];



        void Awake()
        {
            startPos = transform.position;
            startRotate = transform.rotation;

            // 初始化训练数据
            trainDatas[0] = new float[2] { 1, 0.5f };
            trainDatas[1] = new float[2] { 0, 0.5f };
            trainDatas[2] = new float[2] { 0.5f, 1 };
            trainDatas[3] = new float[2] { 0.5f, 0 };
        }


        void Update()
        {
            // 计算当前位置与墙的距离

            // 向左侧探针前方发射射线
            inDatas[0] = GetDistance(LeftSensor);

            // 向右侧探针前方发射射线
            inDatas[1] = GetDistance(RightSensor);

            // 向上侧探针前方发射射线
            inDatas[2] = GetDistance(UpSensor);

            // 向下侧探针前方发射射线
            inDatas[3] = GetDistance(DownSensor);


            for (var i = 0; i < inDatas.Length; i++)
            {
                var inData = inDatas[i];
                if (inData < 0.2)
                {
                    // 如果碰到左侧墙壁
                    // 重置位置
                    transform.position = startPos;
                    transform.rotation = startRotate;
                    // 训练网络
                    for (var j = 0; j < TrainTimes; j++)
                    {
                        NeuralMono.Train(inDatas, trainDatas[i]);
                    }
                    print("←" + inDatas[0] + ",→" + inDatas[1] + ",上" + inDatas[2] + ",下" + inDatas[3]);
                    Debug.LogError("----训练----" + i + "," + trainDatas[i][0] + "," + trainDatas[i][1]);
                    break;
                }
            }


            // 计算网络
            outDatas = NeuralMono.Calculate(inDatas);
            //print("" + outData[0] + "," + outData[1]);

            // 判断转向
            if (outDatas[0] < 0.2f)
            {
                transform.Rotate(0, ((outDatas[0] / 1) * 0.2f) * -50 * Time.deltaTime, 0);
            }
            else if(outDatas[0] > 0.7f)
            {
                transform.Rotate(0, ((outDatas[0] / 1) * 0.2f + 0.8f) * 50 * Time.deltaTime, 0);
            }


            if (outDatas[1] < 0.2f)
            {
                transform.Rotate(((outDatas[0] / 1) * 0.2f) * -50 * Time.deltaTime, 0, 0);
            }
            else if (outDatas[1] > 0.7f)
            {
                transform.Rotate(((outDatas[0] / 1) * 0.2f + 0.8f) * 50 * Time.deltaTime, 0, 0);
            }

            // 位置前进
            transform.Translate(0, 0, 0.2f);


        }


        void OnGUI()
        {
            GUI.color = Color.red;
            // TODO 绘制神经网络的结构
            GUI.Label(new Rect(100, 100, 500, 800), GetNeuronNetStructureStr());
        }


        /// <summary>
        /// 获取神经网络结构字符串
        /// </summary>
        /// <returns></returns>
        private string GetNeuronNetStructureStr()
        {
            StringBuilder result = new StringBuilder();

            result.Append("in:" + inDatas[0] + "," + inDatas[1] + "," + inDatas[2] + "," + inDatas[3]);
            result.Append("-----------------------\n");
            // 遍历input节点
            foreach (var inItem in NeuralMono.NeuralNet.InLayer.NodeList)
            {

                foreach (var axon in inItem.FromList)
                {
                    result.Append(inItem.Value);
                    result.Append("  :  ");
                    result.Append(axon.weight);
                    result.Append(",\n");
                }
            }
            result.Append("-----------------------\n");

            // 遍历hide节点
            foreach (var hiddenLayer in NeuralMono.NeuralNet.HideLayer)
            {
                foreach (var hiddenItem in hiddenLayer.NodeList)
                {
                    foreach (var axon in hiddenItem.FromList)
                    {
                        result.Append(hiddenItem.Value);
                        result.Append("  :  ");
                        result.Append(axon.weight);
                        result.Append(",\n");
                    }
                }
            }
            result.Append("-----------------------\n");

            // 遍历out节点
            foreach (var outItem in NeuralMono.NeuralNet.OutLayer.NodeList)
            {
                foreach (var axon in outItem.FromList)
                {
                    result.Append(outItem.Value);
                    result.Append("  :  ");
                    result.Append(axon.weight);
                    result.Append(",\n");
                }
            }

            result.Append("\n-----------------------");
            result.Append("out:" + outDatas[0] + "," + outDatas[1]);
            return result.ToString();
        }


        private float GetDistance(GameObject obj)
        {
            var result = 0f;

            Ray ray = new Ray(obj.transform.position, obj.transform.forward);
            RaycastHit hit;
            Debug.DrawRay(obj.transform.position, obj.transform.forward, Color.red);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                result = hit.distance;
            }

            return result;
        }
    }
}
