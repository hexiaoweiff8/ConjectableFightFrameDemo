using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Assets.script.AI.Net
{
    /// <summary>
    /// UDP服务端
    /// </summary>
    public class UdpServer : IDisposable
    {

        /// <summary>
        /// 消息接收事件
        /// </summary>
        public TcpIPClient.OnRecv Event_Recv = null;


        /// <summary>
        /// 是否在接收
        /// </summary>
        private bool receiving = false;

        /// <summary>
        /// socket链接对象
        /// </summary>
        private Socket socket;
        
        /// <summary>
        /// 数据接收类
        /// </summary>
        private RecvObject recvObj = new RecvObject();

        /// <summary>
        /// 接收线程
        /// </summary>
        private Thread th;



        /// <summary>
        /// 启动本地监听
        /// </summary>
        /// <param name="port">被监听端口</param>
        public void StartBind(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            receiving = true;
            th = new Thread(Recive);
            th.Start();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        public void Send(string ip, int port, byte[] data)
        {
            var point = new IPEndPoint(Dns.GetHostAddresses(ip)[0], port);
            data = ByteUtils.AddDataHead(data);
            socket.SendTo(data, point);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (th != null && th.IsAlive)
            {
                th.Interrupt();
            }
            recvObj = new RecvObject();
        }

        /// <summary>
        /// 启动异步监听
        /// </summary>
        private void Recive()
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            while (receiving)
            {
                var len = socket.ReceiveFrom(recvObj.Buffer, recvObj.BufLen, recvObj.Buffer.Length - recvObj.BufLen, SocketFlags.None, ref endPoint);
                recvObj.AddDataLen(len);
                ReciveCallback(recvObj);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReciveCallback(RecvObject recv)
        {
            try
            {
                // 收到消息
                int bytesRead = recv.BufLen;
                if (bytesRead > 0) //成功
                {
                    // 获取等待获取消息数组
                    recvObj.AddDataLen(bytesRead);
                    // 处理数据
                    ComputeData(recvObj);
                }
                else//网络中断
                {
                    //isClosed = true;
                    //CLog.LogError("网络中断");
                    UnityEngine.Debug.LogError("收到空字节流");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.ToString());
            }

        }



        /// <summary>
        /// 计算数据
        /// </summary>
        /// <param name="recvObject">数据接受类</param>
        private void ComputeData(RecvObject recvObject)
        {
            //CLog.Log("包头读取长度:" + ByteUtils.GetDataLength(recvObject.Buffer) + 4);


            if (ByteUtils.CouldRead(recvObject.Buffer, recvObject.ReadPos, recvObject.BufLen - recvObject.ReadPos))
            {
                while (ByteUtils.CouldRead(recvObject.Buffer, recvObject.ReadPos, recvObject.BufLen - recvObject.ReadPos))
                {
                    // 分离数据
                    SeparatData(recvObject);
                }
                recvObj.Cut();
            }
        }


        /// <summary>
        /// 分离数据
        /// </summary>
        /// <param name="recvObject">被分离数据类</param>
        private void SeparatData(RecvObject recvObject)
        {
            // 单个数据
            byte[] dataBody = null;
            dataBody = ByteUtils.ReadMsg(ref recvObject.Buffer, recvObject.ReadPos, false);
            //回调
            if (Event_Recv != null)
            {
                try
                {
                    recvObject.ReadLength(dataBody.Length + 4);
                    Event_Recv(dataBody);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.ToString());
                }
            }

        }


        public void Dispose()
        {
            Reset();
        }
    }
}
