using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.script.AI.Net
{
    /// <summary>
    /// Socket实现的异步TCP服务器
    /// </summary>
    public class AsyncSocketTCPServer : IDisposable
    {
        #region Fields
        /// <summary>
        /// 服务器程序同意的最大客户端连接数
        /// </summary>
        private int _maxClient;

        /// <summary>
        /// 当前的连接的客户端数
        /// </summary>
        private int _clientCount;

        /// <summary>
        /// 服务器使用的异步socket
        /// </summary>
        private Socket _serverSock;

        /// <summary>
        /// 客户端会话列表
        /// </summary>
        private List<AsyncSocketState> _clients;

        private bool disposed = false;

        #endregion

        #region Properties

        /// <summary>
        /// 服务器是否正在执行
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// 监听的IP地址
        /// </summary>
        public IPAddress Address { get; private set; }
        /// <summary>
        /// 监听的port
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 通信使用的编码
        /// </summary>
        public Encoding Encoding { get; set; }


        #endregion

        #region 构造函数

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="listenPort">监听的port</param>
        public AsyncSocketTCPServer(int listenPort)
            : this(IPAddress.Any, listenPort, 1024)
        {
        }

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localEP">监听的终结点</param>
        public AsyncSocketTCPServer(IPEndPoint localEP)
            : this(localEP.Address, localEP.Port, 1024)
        {
        }

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localIPAddress">监听的IP地址</param>
        /// <param name="listenPort">监听的port</param>
        /// <param name="maxClient">最大客户端数量</param>
        public AsyncSocketTCPServer(IPAddress localIPAddress, int listenPort, int maxClient)
        {
            this.Address = localIPAddress;
            this.Port = listenPort;
            this.Encoding = Encoding.Default;

            _maxClient = maxClient;
            _clients = new List<AsyncSocketState>();
            _serverSock = new Socket(localIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region Method

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _serverSock.Bind(new IPEndPoint(this.Address, this.Port));
                _serverSock.Listen(1024);
                _serverSock.BeginAccept(new AsyncCallback(HandleAcceptConnected), _serverSock);
            }
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="backlog">
        /// 服务器所同意的挂起连接序列的最大长度
        /// </param>
        public void Start(int backlog)
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _serverSock.Bind(new IPEndPoint(this.Address, this.Port));
                _serverSock.Listen(backlog);
                _serverSock.BeginAccept(new AsyncCallback(HandleAcceptConnected), _serverSock);
            }
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _serverSock.Close();
                //TODO 关闭对全部客户端的连接

            }
        }

        /// <summary>
        /// 处理客户端连接
        /// </summary>
        /// <param name="ar"></param>
        private void HandleAcceptConnected(IAsyncResult ar)
        {
            if (IsRunning)
            {
                Socket server = (Socket)ar.AsyncState;
                Socket client = server.EndAccept(ar);

                //检查是否达到最大的同意的客户端数目
                if (_clientCount >= _maxClient)
                {
                    //C-TODO 触发事件
                    RaiseOtherException(null);
                }
                else
                {
                    AsyncSocketState state = new AsyncSocketState(client);
                    lock (_clients)
                    {
                        _clients.Add(state);
                        _clientCount++;
                        RaiseClientConnected(state); //触发客户端连接事件
                    }
                    state.RecvDataBuffer = new byte[client.ReceiveBufferSize];
                    //開始接受来自该客户端的数据
                    client.BeginReceive(state.RecvDataBuffer, 0, state.RecvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(HandleDataReceived), state);
                }
                //接受下一个请求
                server.BeginAccept(new AsyncCallback(HandleAcceptConnected), ar.AsyncState);
            }
        }
        /// <summary>
        /// 处理客户端数据
        /// </summary>
        /// <param name="ar"></param>
        private void HandleDataReceived(IAsyncResult ar)
        {
            if (IsRunning)
            {
                AsyncSocketState state = (AsyncSocketState)ar.AsyncState;
                Socket client = state.ClientSocket;
                try
                {
                    //假设两次開始了异步的接收,所以当客户端退出的时候
                    //会两次执行EndReceive
                    int recv = client.EndReceive(ar);
                    if (recv == 0)
                    {
                        //C- TODO 触发事件 (关闭客户端)
                        Close(state);
                        RaiseNetError(state);
                        return;
                    }
                    //TODO 处理已经读取的数据 ps:数据在state的RecvDataBuffer中

                    //C- TODO 触发数据接收事件
                    RaiseDataReceived(state);
                }
                catch (SocketException)
                {
                    //C- TODO 异常处理
                    RaiseNetError(state);
                }
                finally
                {
                    //继续接收来自来客户端的数据
                    client.BeginReceive(state.RecvDataBuffer, 0, state.RecvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(HandleDataReceived), state);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="state">接收数据的客户端会话</param>
        /// <param name="data">数据报文</param>
        public void Send(AsyncSocketState state, byte[] data)
        {
            RaisePrepareSend(state);
            Send(state.ClientSocket, data);
        }

        /// <summary>
        /// 发送数据给所有链接
        /// </summary>
        /// <param name="data"></param>
        public void SendToAll(byte[] data)
        {
            data = ByteUtils.AddDataHead(data);
            foreach (var client in _clients)
            {
                Send(client, data);
            }
        }

        /// <summary>
        /// 异步发送数据至指定的客户端
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="data">报文</param>
        public void Send(Socket client, byte[] data)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP Scoket server has not been started.");

            if (client == null)
                throw new ArgumentNullException("client");

            if (data == null)
                throw new ArgumentNullException("data");
            client.BeginSend(data, 0, data.Length, SocketFlags.None,
             new AsyncCallback(SendDataEnd), client);
        }

        /// <summary>
        /// 发送数据完成处理函数
        /// </summary>
        /// <param name="ar">目标客户端Socket</param>
        private void SendDataEnd(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);
            RaiseCompletedSend(null);
        }
        #endregion

        #region 事件

        /// <summary>
        /// 与客户端的连接已建立事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientConnected;
        /// <summary>
        /// 与客户端的连接已断开事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientDisconnected;

        /// <summary>
        /// 触发客户端连接事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseClientConnected(AsyncSocketState state)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new AsyncSocketEventArgs(state));
            }
        }
        /// <summary>
        /// 触发客户端连接断开事件
        /// </summary>
        /// <param name="client"></param>
        private void RaiseClientDisconnected(Socket client)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, new AsyncSocketEventArgs(new byte[]{}));
            }
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> DataReceived;

        private void RaiseDataReceived(AsyncSocketState state)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 发送数据前的事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> PrepareSend;

        /// <summary>
        /// 触发发送数据前的事件
        /// </summary>
        /// <param name="state"></param>
        private void RaisePrepareSend(AsyncSocketState state)
        {
            if (PrepareSend != null)
            {
                PrepareSend(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 数据发送完成事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> CompletedSend;

        /// <summary>
        /// 触发数据发送完成的事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseCompletedSend(AsyncSocketState state)
        {
            if (CompletedSend != null)
            {
                CompletedSend(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 网络错误事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> NetError;
        /// <summary>
        /// 触发网络错误事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseNetError(AsyncSocketState state)
        {
            if (NetError != null)
            {
                NetError(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 异常事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> OtherException;
        /// <summary>
        /// 触发异常事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseOtherException(AsyncSocketState state, byte[] descrip)
        {
            if (OtherException != null)
            {
                OtherException(this, new AsyncSocketEventArgs(descrip, state));
            }
        }
        private void RaiseOtherException(AsyncSocketState state)
        {
            RaiseOtherException(state, new byte[] {});
        }
        #endregion

        #region Close
        /// <summary>
        /// 关闭一个与客户端之间的会话
        /// </summary>
        /// <param name="state">须要关闭的客户端会话对象</param>
        public void Close(AsyncSocketState state)
        {
            if (state != null)
            {
                state.Datagram = null;
                state.RecvDataBuffer = null;

                _clients.Remove(state);
                _clientCount--;
                //TODO 触发关闭事件
                state.Close();
            }
        }
        /// <summary>
        /// 关闭全部的客户端会话,与全部的客户端连接会断开
        /// </summary>
        public void CloseAllClient()
        {
            foreach (AsyncSocketState client in _clients)
            {
                Close(client);
            }
            _clientCount = 0;
            _clients.Clear();
        }
        #endregion

        #region 释放
        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release 
        /// both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                        if (_serverSock != null)
                        {
                            _serverSock = null;
                        }
                    }
                    catch (SocketException)
                    {
                        //TODO
                        RaiseOtherException(null);
                    }
                }
                disposed = true;
            }
        }
        #endregion
    }

    /// <summary>
    /// 异步SOCKET TCP 中用来存储客户端状态信息的类
    /// </summary>
    public class AsyncSocketState
    {
        #region 字段
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        private byte[] _recvBuffer;

        /// <summary>
        /// 客户端发送到server的报文
        /// 注意:在有些情况下报文可能仅仅是报文的片断而不完整
        /// </summary>
        private string _datagram;

        /// <summary>
        /// 客户端的Socket
        /// </summary>
        private Socket _clientSock;

        #endregion

        #region 属性

        /// <summary>
        /// 接收数据缓冲区 
        /// </summary>
        public byte[] RecvDataBuffer
        {
            get
            {
                return _recvBuffer;
            }
            set
            {
                _recvBuffer = value;
            }
        }

        /// <summary>
        /// 存取会话的报文
        /// </summary>
        public string Datagram
        {
            get
            {
                return _datagram;
            }
            set
            {
                _datagram = value;
            }
        }

        /// <summary>
        /// 获得与客户端会话关联的Socket对象
        /// </summary>
        public Socket ClientSocket
        {
            get
            {
                return _clientSock;

            }
        }


        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cliSock">会话使用的Socket连接</param>
        public AsyncSocketState(Socket cliSock)
        {
            _clientSock = cliSock;
        }

        /// <summary>
        /// 初始化数据缓冲区
        /// </summary>
        public void InitBuffer()
        {
            if (_recvBuffer == null && _clientSock != null)
            {
                _recvBuffer = new byte[_clientSock.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {

            //关闭数据的接受和发送
            _clientSock.Shutdown(SocketShutdown.Both);

            //清理资源
            _clientSock.Close();
        }
    }



    /// <summary>
    /// 异步Socket TCP事件參数类
    /// </summary>
    public class AsyncSocketEventArgs : EventArgs
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public byte[] _msg;

        /// <summary>
        /// client状态封装类
        /// </summary>
        public AsyncSocketState _state;

        /// <summary>
        /// 是否已经处理过了
        /// </summary>
        public bool IsHandled { get; set; }

        public AsyncSocketEventArgs(byte[] msg)
        {
            this._msg = msg;
            IsHandled = false;
        }
        public AsyncSocketEventArgs(AsyncSocketState state)
        {
            this._msg = state.RecvDataBuffer;
            this._state = state;
            IsHandled = false;
        }
        public AsyncSocketEventArgs(byte[] msg, AsyncSocketState state)
        {
            this._msg = msg;
            this._state = state;
            IsHandled = false;
        }
    }
}
