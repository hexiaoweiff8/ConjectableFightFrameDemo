using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.script.AI.Net
{
    public class TcpIPClient:IDisposable
    {
        public delegate void OnClose();
        public delegate void OnRecv(byte[] buff);
        public delegate void OnConn(bool isok);

        //主线程和网络线程Socket句柄分开，但指向同实例
        Socket socket = null;

        /// <summary>
        /// 是否已销毁
        /// </summary>
        bool m_IsDispose = false;

        /// <summary>
        ///  是否链接成功
        /// </summary>
        bool m_ConnOK = false;
 
        /// <summary>
        /// 链接关闭事件
        /// </summary>
        public OnClose Event_Close = null;

        /// <summary>
        /// 消息接收事件
        /// </summary>
        public OnRecv Event_Recv = null;

        /// <summary>
        /// 链接成功事件
        /// </summary>
        public OnConn Event_Conn = null;

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnd {
            get
            {
                if (socket != null)
                {
                    return socket.Connected;
                }
                else
                {
                    return false;
                }
            } }

        /// <summary>
        /// 数据接收类
        /// </summary>
        private RecvObject recvObj = new RecvObject();

        /// <summary>
        /// 有效长度
        /// </summary>
        private int recvLen = 0;

        /// <summary>
        /// 流水号测试
        /// </summary>
        private static int lid = 0;

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        public void Connect(String ip, int port)
        {
            if (socket != null || m_IsDispose)
            {
                return;
            }

            IPAddress[] address = Dns.GetHostAddresses(ip);
            if (address.Length > 0 && AddressFamily.InterNetworkV6 == address[0].AddressFamily)
            {
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);//新建一个socket
            }
            else
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//新建一个socket
            }
            socket.BeginConnect(ip, port, ConnectCallback, null);

        }

        /// <summary>
        /// 向服务器发送
        /// </summary>
        /// <param name="buf">被发送数据</param>
        public void Send(byte[] buf)
        {
            if (socket == null || !m_ConnOK)
            {
                return;
            }
            buf = ByteUtils.AddDataHead(buf);
            socket.BeginSend(buf, 0, buf.Length, SocketFlags.None, SendCallback, socket);
        }

        /// <summary>
        /// 销毁并关闭连接
        /// </summary>
        public void Dispose()
        {
            if (m_IsDispose)
            {
                return;
            }
            m_IsDispose = true;
            m_ConnOK = false;
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        private void _BeginReceive()
        {
            socket.BeginReceive(recvObj.Buffer, recvObj.BufLen, recvObj.Buffer.Length - recvObj.BufLen, 0,
                ReceiveCallback, null);
        }


        // ------------------------------回调-------------------------------


        /// <summary>
        /// 建立链接成功回调
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            if (socket == null) return;
            try
            {
                socket.EndConnect(ar);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("链接错误:" + e);
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }
                return;
            }
            // 开始接收消息
            _BeginReceive();
            m_ConnOK = true;
            //回调
            if (Event_Conn != null) Event_Conn(m_ConnOK);
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
            var isClosed = false;

            if (m_IsDispose || socket == null)
            {
                UnityEngine.Debug.Log("链接已被销毁.");
                return;
            }
            try
            {
                // 收到消息
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0) //成功
                {

                    // 获取等待获取消息数组
                    //CLog.Log("收到消息长度:" + bytesRead);
                    //CLog.Log("等待消息长度:" + (recvObj.BufLen - recvObj.ReadPos));
                    recvObj.AddDataLen(bytesRead);
                    // 处理数据
                    ComputeData(recvObj);


                    //启动下一轮接收
                    _BeginReceive();
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
                socket = null;
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }

                m_ConnOK = false;

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
            if (ByteUtils.CouldRead(recvObject.Buffer, recvObject.ReadPos, recvObject.BufLen - recvObject.ReadPos))
            {
                while (ByteUtils.CouldRead(recvObject.Buffer, recvObject.ReadPos, recvObject.BufLen - recvObject.ReadPos))
                {
                    // CLog.Log(recvObject.BufLen + "," + recvObject.ReadPos);
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
                    //CLog.Log("" + dataBody.Length + 4);
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



    /// <summary>
    /// 接收数据类
    /// </summary>
    public class RecvObject
    {
        /// <summary>
        /// 缓冲大小
        /// </summary>
        public const int BufferSize = 1024 * 1024;

        /// <summary>
        /// 缓冲区
        /// </summary>
        public byte[] Buffer = new byte[BufferSize];

        /// <summary>
        /// 有效长度/写入位置
        /// </summary>
        public int BufLen = 0;//有效长度

        /// <summary>
        /// 当前读取位置
        /// </summary>
        public int ReadPos = 0;

        /// <summary>
        /// 读取长度
        /// </summary>
        /// <param name="len"></param>
        public void ReadLength(int len)
        {
            ReadPos += len;
        }

        /// <summary>
        /// 添加数据长度
        /// </summary>
        /// <param name="len"></param>
        public void AddDataLen(int len)
        {
            BufLen += len;
        }

        /// <summary>
        /// 移除无用包
        /// </summary>
        public void Cut()
        {
            if (ReadPos == 0) return;

            //将尾部未处理包前移
            {
                for (int i = ReadPos; i < BufLen; i++)
                {
                    Buffer[i - ReadPos] = Buffer[i];
                }
                // 清空数据
                var zero = (byte)0;
                var start = (BufLen == ReadPos ? 0 : BufLen);
                for (var i = start; i < BufferSize; i++)
                {
                    Buffer[i] = zero;
                }
                BufLen -= ReadPos;
                if (BufLen < 0)
                {
                    BufLen = 0;
                }
                ReadPos = 0;
            }
        }
    }
}