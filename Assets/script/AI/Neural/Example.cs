using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.AI.Neural
{
    /// <summary>
    /// 测试类
    /// </summary>
    public class Example : MonoBehaviour
    {
        /// <summary>
        /// 主相机
        /// </summary>
        public Camera MainCamere;

        /// <summary>
        /// 主地面
        /// </summary>
        public GameObject MainPlane;

        /// <summary>
        /// 球
        /// </summary>
        public GameObject Ball;

        /// <summary>
        /// 球刚体
        /// </summary>
        public Rigidbody BallRigidbody;

        /// <summary>
        /// 神经网络Obj
        /// </summary>
        public NeuralMono NeuralMono;

        /// <summary>
        /// 地图宽
        /// </summary>
        public int Width = 10;

        /// <summary>
        /// 地图长
        /// </summary>
        public int Height = 10;

        /// <summary>
        /// 训练次数
        /// </summary>
        public int TrainTime = 10000;

        /// <summary>
        /// 平台单位长度
        /// </summary>
        public const int PlaneHalfUnit = 5;


        /// <summary>
        /// 位置映射数组
        /// </summary>
        private int[] positionMappingArray;

        /// <summary>
        /// 最后球所在位置
        /// </summary>
        private int lastPos = PlaneHalfUnit / 2;

        /// <summary>
        /// 是否在平台上
        /// </summary>
        private bool onTheTable = false;

        /// <summary>
        /// 球原始位置
        /// </summary>
        private Vector3 ballSourcePosition;

        /// <summary>
        /// 平台原始旋转值
        /// </summary>
        private Quaternion planeRotate;


        /// <summary>
        /// 输入数据
        /// </summary>
        private float[] inData = new float[2];

        /// <summary>
        /// 输出数据
        /// </summary>
        private float[] outData;


        void Awake()
        {
            positionMappingArray = new int[Width];
            // 记录球原始位置
            ballSourcePosition = Ball.transform.position;
            // 记录板子原始角度
            planeRotate = MainPlane.transform.rotation;
        }

        public void OnGUI()
        {
            GUI.color = Color.red;
            // TODO 绘制神经网络的结构
            GUI.Label(new Rect(100, 100, 500, 800), GetNeuronNetStructureStr());
        }




        void Update()
        {
            // 起始位置
            var positon = Ball.transform.position;

            //Ray ray = new Ray();
            //var scale = MainPlane.transform.localScale;

            onTheTable = false;

            // 检测球体位置
            // 射线更新
            //for (var i = 0; i < Height; i++)
            //{
            for (var j = 0; j < Width; j++)
            {
                //// 发射射线
                //ray.direction = new Vector3();
                //ray.origin = new Vector3();
                if (j - PlaneHalfUnit < positon.x && j - PlaneHalfUnit + 1 >= positon.x)
                {
                    positionMappingArray[j] = 1;
                    lastPos = j;
                    onTheTable = true;
                }
                else
                {
                    positionMappingArray[j] = 0;
                }
            }

            // 判断掉到下面
            if (positon.y < -5)
            {
                onTheTable = false;
                if (lastPos > 5)
                {
                    lastPos = positionMappingArray.Length - 1;
                }
                else
                {
                    lastPos = 0;
                }
            }

            //}

            // 转换数据

            if (lastPos == 0)
            {
                lastPos = 1;
            }
            inData[0] = (float)lastPos / positionMappingArray.Length;
            inData[1] = (float)(positionMappingArray.Length - lastPos) / positionMappingArray.Length;

            // 如果球没有在范围内则判断球掉下去了, 从上次掉下去位置判断从哪里掉下去的, 训练网络
            if (!onTheTable)
            {
                // 重置场景, 训练网络
                // 球回归原位
                Ball.transform.position = ballSourcePosition;
                // 清除惯性
                BallRigidbody.velocity = Vector3.zero;
                // 板子回归原位
                MainPlane.transform.rotation = planeRotate;


                var trainTarget = new[] {(lastPos > 5 ? 0 : 1f)};
                for (var i = 0; i < TrainTime; i++)
                {
                    NeuralMono.Train(inData, trainTarget);
                }
            }
            else
            {
                // 输入数据
                outData = NeuralMono.Calculate(inData);
                //Debug.logger.Log(outData[0] + "," + (outData[0] / 1) * 0.2f * -100 * Time.deltaTime + "," + (outData[0] / 1) * 0.2f * 100 * Time.deltaTime);

                // 判断输出
                if (outData[0] < 0.45)
                {
                    // 平台Z轴顺时针旋转
                    MainPlane.transform.Rotate(Vector3.forward, ((outData[0] / 1) * 0.2f + + 0.2f) * 50 * Time.deltaTime);
                }
                else if (outData[0] > 0.55)
                {
                    // 平台Z轴逆时针旋转
                    MainPlane.transform.Rotate(Vector3.forward, ((outData[0] / 1) * 0.2f) * -50 * Time.deltaTime);
                }

                if (Math.Acos(MainPlane.transform.rotation.w) > 90)
                {
                    MainPlane.transform.rotation = new Quaternion(0, 0, 1, 0);
                }
                if (Math.Acos(MainPlane.transform.rotation.w) < -90)
                {
                    MainPlane.transform.rotation = new Quaternion(0, 0, 1, 0);
                }


            }

            //Debug.Log(lastPos + "," + positon.x);
        }


        /// <summary>
        /// 获取神经网络结构字符串
        /// </summary>
        /// <returns></returns>
        private string GetNeuronNetStructureStr()
        {
            if (inData == null || outData == null)
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder();

            result.Append("In:" + inData[0]
                + ":" + inData[1]);
            result.Append("-----------------------\n");
            // 遍历input节点
            foreach (var inItem in NeuralMono.NeuralNet.InLayer.NodeList)
            {

                //foreach (var axon in inItem.TargetList)
                //{
                //    result.Append(inItem.Value);
                //    result.Append(":");
                //    result.Append(axon.weight);
                //    result.Append(" -in- ");
                //}
                result.Append(inItem.Value);
                result.Append("\n");
            }
            result.Append("\n");

            // 遍历hide节点
            foreach (var hiddenLayer in NeuralMono.NeuralNet.HideLayer)
            {
                foreach (var hiddenItem in hiddenLayer.NodeList)
                {
                    //foreach (var axon in hiddenItem.TargetList)
                    //{
                    //    result.Append(hiddenItem.Value);
                    //    result.Append(":");
                    //    result.Append(axon.weight);
                    //    result.Append(" -hide- ");
                    //}
                    result.Append(hiddenItem.Value);
                    result.Append("\n");
                }
            }
            result.Append("\n");

            // 遍历out节点
            foreach (var outItem in NeuralMono.NeuralNet.OutLayer.NodeList)
            {
                //foreach (var axon in outItem.TargetList)
                //{
                //    result.Append(outItem.Value);
                //    result.Append(":");
                //    result.Append(axon.weight);
                //    result.Append(" -out- ");
                //}
                result.Append(outItem.Value);
                result.Append("\n");
            }

            result.Append("-----------------------");
            result.Append("out:" + outData[0]);
            return result.ToString();
        }
    }
}
