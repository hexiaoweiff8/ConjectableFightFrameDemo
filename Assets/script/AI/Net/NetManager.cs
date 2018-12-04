using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Assets.script.AI.Net
{
    public class NetManager
    {

        public Action<byte[]> ComputeAction;
        /// <summary>
        /// udp网络连接
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// tcp网络连接
        /// </summary>
        private TcpIPClient tcpClient;

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">连接地址</param>
        /// <param name="port">连接端口</param>
        /// <param name="type">连接类型</param>
        public void Connect(string ip, int port, ClientType type = ClientType.UDP)
        {
            switch (type)
            {
                case ClientType.TCP:
                    tcpClient = new TcpIPClient();
                    tcpClient.Connect(ip, port);
                    tcpClient.Event_Recv += OnReceive;
                    break;

                case ClientType.UDP:
                    udpClient = new UdpClient();
                    udpClient.Connect(ip, port);
                    udpClient.Event_Recv += OnReceive;
                    break;
            }
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="buff"></param>
        private void OnReceive(byte[] buff)
        {
            // 解析数据
            if (ComputeAction != null)
            {
                ComputeAction(buff);
            }
            else
            {
                Debug.LogError("未初始化处理消息");
            }
        }
        

    }

    /// <summary>
    /// 网络连接方式
    /// </summary>
    public enum ClientType
    {
        TCP,
        UDP
    }
}
