using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GTerminalCore
{
    /// <summary>
    /// 管理虚拟终端与远程主机通信的接口
    /// </summary>
    public interface IVTStream
    {
        event Action<object, VTStreamState> StatusChanged;

        /// <summary>
        /// 从数据流中读取一段数据
        /// </summary>
        /// <param name="size">要读取的数据大小</param>
        /// <returns></returns>
        byte[] Read(int size);

        /// <summary>
        /// 从数据流中读取一个字节的数据
        /// </summary>
        /// <returns></returns>
        byte Read();

        /// <summary>
        /// 想数据流里写入一个字节的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Write(byte data);

        /// <summary>
        /// 想数据流里写入一段数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Write(byte[] data);

        /// <summary>
        /// EOF为True时，VideoTerminal会停止解析
        /// 在与远程主机断开连接的时候，应该把这个值设为True，否则为False
        /// </summary>
        bool EOF { get; }
    }
}