using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.script.AI.Net
{
    /// <summary>
    /// 数据工具
    /// </summary>
    public static class ByteUtils
    {


        /// <summary>
        /// 格式化数据
        /// 给数据加上数据长度
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] AddDataHead(byte[] msg)
        {
            if (msg == null)
            {
                return null;
            }
            var lenthData = ConnectByte(Int32ToBytes(msg.Length), msg, 0, msg.Length);

            return lenthData;
        }

        /// <summary>
        /// 连接二个字节数组
        /// </summary>
        /// <param name='byte1'>第一个数组</param>
        /// <param name='bytes2'>第二个数组</param>
        /// <param name="byte2Index">第二个字节数组起始偏移</param>
        /// <param name="byte2Length">第二个字节数组读取长度</param>
        /// <returns>返回一个 连接好的字节流</returns>
        public static byte[] ConnectByte(byte[] byte1, byte[] bytes2, int byte2Index, int byte2Length)
        {
            var result = new byte[byte1.Length + byte2Length - byte2Index];
            Array.Copy(byte1, 0, result, 0, byte1.Length);
            Array.Copy(bytes2, byte2Index, result, byte1.Length, byte2Length - byte2Index);
            return result;
        }

        /// <summary>
        /// 检查一个数组长度是不是满足要求 网络接收时用
        /// </summary>
        /// <param name='data'>是否符合可读取条件</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="dataLen">数据实际长度</param>
        public static bool CouldRead(byte[] data, int offset = 0, int dataLen = -1)
        {
            if (data.Length < 4)
            {
                return false;
            }
            // 数据长度
            //var len = BitConverter.ToUInt32(data, 0);
            var len = BytesToInt32(data, offset);
            Debug.Log("-" + len + "-" + ((dataLen > 0 ? dataLen : data.Length) - 4));
            // 数据是否达到长度
            return len > 0 && ((dataLen > 0 ? dataLen :data.Length) - 4) >= len;
        }

        /// <summary>
        /// 获取数据长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetDataLength([NotNull]byte[] data)
        {
            if (data.Length < 4)
            {
                return -1;
            }

            var len = BytesToInt32(data, 0);
            return len;
        }

        /// <summary>
        /// 拷贝数组
        /// </summary>
        /// <param name="from">被拷贝数据</param>
        /// <param name="offset">数据起始位置</param>
        /// <param name="length">读取数据长度</param>
        /// <returns></returns>
        public static byte[] Copy([NotNull]byte[] from, int offset, int length)
        {
            var result = new byte[length];

            var target = offset + length;
            if (target > from.Length)
            {
                throw new Exception("数组越界");
            }

            for (var i = offset; i < target; i++)
            {
                result[i - offset] = from[i];
            }

            return result;
        }

        /// <summary>
        /// 从字节数组中读取一条消息
        /// 并将已经读取的数据移除
        /// </summary>
        public static byte[] ReadMsg(ref byte[] msg, int offset = 0, bool isCut = true)
        {
            // 数据长度
            //var len = BitConverter.ToUInt32(msg, 0);
            var len = BytesToInt32(msg, offset);
            if (len <= 0)
            {
                msg = new byte[0];
                return new byte[0];
            }
            var result = new byte[len];
            // 从数组中将一条信息读取
            Array.Copy(msg, 4 + offset, result, 0, len);

            if (isCut)
            {
                // 把原数据 进行剪切
                var c = new byte[msg.Length - len - 4];
                Array.Copy(msg, len + offset + 4, c, 0, c.Length);
                msg = c;
            }
            return result;
        }

        /// <summary>
        /// 裁切子数组
        /// </summary>
        /// <param name="bytes">被裁切数组</param>
        /// <param name="start">裁切起始位置</param>
        /// <param name="length">裁切长度</param>
        /// <returns></returns>
        public static byte[] GetSubBytes(byte[] bytes, int start, int length)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var result = ConnectByte(new byte[0], bytes, start, length);

            return result;
        }

        /// <summary>
        /// int转bytes 大端在前
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] Int32ToBytes(Int32 num)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((num >> 24) & 0xFF);
            src[1] = (byte)((num >> 16) & 0xFF);
            src[2] = (byte)((num >> 8) & 0xFF);
            src[3] = (byte)(num & 0xFF);
            return src;
        }

        /// <summary>
        /// bytes转int 大端在前
        /// 只去数组前四位
        /// </summary>
        /// <param name="data">数据数组</param>
        /// <param name="offset">数据偏移</param>
        /// <returns></returns>
        public static int BytesToInt32(byte[] data, int offset)
        {
            int value;
            value = (int)(((data[offset] & 0xFF) << 24)
                          | ((data[offset + 1] & 0xFF) << 16)
                          | ((data[offset + 2] & 0xFF) << 8)
                          | (data[offset + 3] & 0xFF));
            return value;
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="from">被复制数据</param>
        /// <param name="to">目标</param>
        /// <param name="posFrom">被复制数据位置</param>
        /// <param name="posTo">目标位置</param>
        /// <param name="len">数据长度</param>
        public static void CopyData([NotNull]byte[] from, [NotNull]byte[] to, int posFrom, int posTo, int len)
        {
            for (var i = 0; i < len; i++)
            {
                to[i + posTo] = from[i + posFrom];
            }
        }
    }
}