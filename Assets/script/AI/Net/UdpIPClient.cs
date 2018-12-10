using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.script.AI.Net
{
    public class UdpIPClient : IDisposable
    {

        /// <summary>
        /// 链接关闭事件
        /// </summary>
        public TcpIPClient.OnClose Event_Close = null;

        /// <summary>
        /// 消息接收事件
        /// </summary>
        public TcpIPClient.OnRecv Event_Recv = null;

        /// <summary>
        /// 链接成功事件
        /// </summary>
        public TcpIPClient.OnConn Event_Conn = null;


        /// <summary>
        /// socket链接对象
        /// </summary>
        private Socket socket;

        /// <summary>
        /// 数据接收类
        /// </summary>
        private RecvObject recvObj = new RecvObject();

        /// <summary>
        /// 已连接地址
        /// </summary>
        private string connectingAddress = null;

        /// <summary>
        /// 已连接接口
        /// </summary>
        private int connectingPort = 0;

        /// <summary>
        /// 是否正在接收数据
        /// </summary>
        private bool isReceiving = false;

        /// <summary>
        /// 应收数据长度
        /// </summary>
        private int receivableLength = 0;

        /// <summary>
        /// 消息流水号
        /// </summary>
        private int lid = 0;

        /// <summary>
        /// 链接Socket udp
        /// </summary>
        /// <param name="ip">目标IP</param>
        /// <param name="port">目标端口</param>
        /// <returns>是否链接成功</returns>
        public void Connect(string ip, int port)
        {
            // 关闭已有链接.
            Close();
            //CLog.Log("开始链接:" + ip + ":" + port);

            // 建立链接类
            IPAddress[] address = Dns.GetHostAddresses(ip);
            if (address.Length > 0 && AddressFamily.InterNetworkV6 == address[0].AddressFamily)
            {
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);//新建一个socket
            }
            else
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//新建一个socket
            }

            // 异步请求建立链接
            socket.BeginConnect(address[0], port, (ayResult) =>
            {

                // 保存IP与Port
                connectingAddress = ip;
                connectingPort = port;

                // 开始接收消息
                BeginReceive();
                // 链接回调
                if (Event_Conn != null)
                {
                    Event_Conn(true);
                }
            }, socket);

        }



        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">数据msg</param>
        /// <returns></returns>
        public bool Send(byte[] msg)
        {
            if (socket == null || !socket.Connected)
            {
                UnityEngine.Debug.LogError("请先链接再发送消息");
                return false;
            }
            if (msg == null || msg.Length == 0)
            {
                //CLog.Log("数据为空");
                return false;
            }

            try
            {
                //CLog.Log("数据长度:" + msg.Length);
                // 添加数据头
                msg = ByteUtils.AddDataHead(msg);
                //socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, SendCallback, socket);
                var address = Dns.GetHostAddresses(connectingAddress)[0];
                var point = new IPEndPoint(address, connectingPort);
                socket.SendTo(msg, point);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("发送失败:" + e.Message);
            }

            return false;
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            if (socket != null && socket.Connected)
            {
                // 禁用发送与接收
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                UnityEngine.Debug.Log("链接已关闭");
            }
            socket = null;
        }

        /// <summary>
        /// 销毁方法
        /// </summary>
        public void Dispose()
        {
            Close();
        }


        ///// <summary>
        ///// 接收消息回调
        ///// </summary>
        ///// <param name="ayResult2"></param>
        //private void ReceiveCallback(IAsyncResult ayResult2)
        //{

        //    try
        //    {
        //        // 收到消息
        //        SocketError se;
        //        // 收到的字节数量
        //        var receivedByteCount = socket.EndReceive(ayResult2, out se);
        //        CLog.Log("收到数据, 长度:" + receivedByteCount + ",error:" + se);
        //        if (receivedByteCount > 0)
        //        {
        //            // 保证线程数据安全
        //            lock (receivedBuffer)
        //            {
        //                receivedBuffer = ByteUtils.ConnectByte(receivedBuffer, buffer, 0, receivedByteCount);
        //            }
        //            // 继续接收数据
        //            socket.BeginReceive(buffer, 0, BuffSize, SocketFlags.None, ReceiveCallback, socket);
        //        }
        //        else if (receivedByteCount == 0)
        //        {
        //            CLog.Log("收到0长度数据");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        CLog.Log("数据解析错误:" + e.ToString());
        //    }
        //}


        /// <summary>
        /// 开始接收消息
        /// </summary>
        private void  BeginReceive()
        {
            socket.BeginReceive(recvObj.Buffer, recvObj.BufLen, recvObj.Buffer.Length - recvObj.BufLen, 0, ReceiveCallback, socket);
        }


        /// <summary>
        /// 发送回调
        /// </summary>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (socket == null) return;
                int bytesSend = socket.EndSend(ar);
                //if (bytesSend > 0)
                //{
                //    CLog.Log("发送成功:" + bytesSend);
                //}
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.ToString());
            }
        }


        /// <summary>
        /// 接收消息回调
        /// </summary>
        private void ReceiveCallback(IAsyncResult ar)
        {
            // 需要加入消息容错机制, 防止消息丢失导致的卡死
            var isClosed = false;
            var callbackSocket = (Socket) ar.AsyncState;

            if (callbackSocket == null || !callbackSocket.Connected)
            {
                UnityEngine.Debug.Log("链接已被销毁.");
                return;
            }
            try
            {
                // 收到消息
                int bytesRead = callbackSocket.EndReceive(ar);
                if (bytesRead > 0) //成功
                {

                    // 获取等待获取消息数组
                    //CLog.Log("UDP收到消息长度:" + bytesRead);
                    //CLog.Log("UDP等待消息长度:" + (recvObj.BufLen - recvObj.ReadPos));
                    recvObj.AddDataLen(bytesRead);
                    // 处理数据
                    ComputeData(recvObj);

                    //启动下一轮接收
                    BeginReceive();
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
                isClosed = true;
                UnityEngine.Debug.LogError(ex.ToString());
            }

            // 验证链接状态
            if (isClosed)
            {
                //CLog.Log("关闭链接.");
                callbackSocket.Close();

                //回调
                if (Event_Close != null) Event_Close();

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
    }
}
