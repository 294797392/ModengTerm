using DotNEToolkit.DataModels;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 存储一个可以回放的文件
    /// 录像文件由多个帧组成
    /// 每一帧分帧头和帧体，帧头是20字节，帧体大小不固定
    /// </summary>
    public class PlaybackFile : ModelBase
    {
        /// <summary>
        /// 保存文件头信息
        /// </summary>
        private class FileHeader
        {
            /// <summary>
            /// 文件格式版本号
            /// </summary>
            public int Version { get; set; }
        }

        #region 常量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("PlaybackFile");

        /// <summary>
        /// 文件头40字节长度
        /// </summary>
        private const int FILE_HEADER_SIZE = 40;

        /// <summary>
        /// 帧头长度
        /// </summary>
        private const int FRAME_HEADER_SIZE = 20;

        #endregion

        #region 实例变量

        private FileStream stream;

        /// <summary>
        /// 缓存文件头信息
        /// </summary>
        private FileHeader header;

        #endregion

        #region 属性

        /// <summary>
        /// 该回放文件所对应的会话信息
        /// 保存回放文件的时候会拷贝一份会话信息
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 该文件里的所有帧
        /// </summary>
        public List<PlaybackFrame> Frames { get; set; }

        /// <summary>
        /// 获取是否到文件结尾了
        /// </summary>
        public bool EndOfFile
        {
            get
            {
                if (this.stream == null)
                {
                    return true;
                }

                return this.stream.Position == this.stream.Length - 1;
            }
        }

        #endregion

        #region 构造方法

        public PlaybackFile()
        {
            this.Frames = new List<PlaybackFrame>();
        }

        #endregion

        /// <summary>
        /// 打开该文件
        /// </summary>
        public int OpenWrite(string filePath)
        {
            if (this.stream != null)
            {
                return ResponseCode.SUCCESS;
            }

            try
            {
                this.stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                // 写文件头
                byte[] magic = new byte[] { (byte)'M', (byte)'P', (byte)'Z' };
                this.stream.Write(magic);

                byte[] version = BitConverter.GetBytes(1);
                this.stream.Write(version);

                this.stream.Seek(FILE_HEADER_SIZE, SeekOrigin.Begin);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("打开回放文件进行写入异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public int OpenRead(string filePath)
        {
            if (this.stream != null)
            {
                logger.ErrorFormat("打开回放文件进行读取失败, 已经被打开了");
                return ResponseCode.FAILED;
            }

            if (!File.Exists(filePath))
            {
                logger.ErrorFormat("打开回放文件进行读取失败, 文件不存在, {0}", filePath);
                return ResponseCode.FAILED;
            }

            try
            {
                this.stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                byte[] headerBytes = new byte[FILE_HEADER_SIZE];
                this.stream.Read(headerBytes, 0, headerBytes.Length);

                // TODO：对文件类型做校验

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("打开回放文件进行读取异常", ex);
                return ResponseCode.FAILED;
            }
        }

        /// <summary>
        /// 关闭该文件
        /// </summary>
        public void Close()
        {
            if (this.stream == null)
            {
                return;
            }

            this.stream.Close();
            this.stream.Dispose();
            this.stream = null;
        }

        /// <summary>
        /// 向回放文件里写入一帧
        /// </summary>
        /// <param name="frame"></param>
        public int WriteFrame(PlaybackFrame frame)
        {
            // 帧头20个字节
            // 0 - 7：时间戳
            // 8 - 11：数据大小
            // 12 - 19：保留

            #region 数据包头

            byte[] frameHeader = new byte[FRAME_HEADER_SIZE];

            // 时间戳，8字节
            byte[] timestampBytes = BitConverter.GetBytes(frame.Timestamp);
            Buffer.BlockCopy(timestampBytes, 0, frameHeader, 0, timestampBytes.Length);

            // 数据大小，4字节
            byte[] sizeBytes = BitConverter.GetBytes(frame.Data.Length);
            Buffer.BlockCopy(sizeBytes, 0, frameHeader, 8, sizeBytes.Length);

            #endregion

            #region 数据包内容

            byte[] frameData = frame.Data;

            #endregion

            byte[] frameBytes = new byte[frameHeader.Length + frameData.Length];
            Buffer.BlockCopy(frameHeader, 0, frameBytes, 0, frameHeader.Length);
            Buffer.BlockCopy(frameData, 0, frameBytes, 20, frameData.Length);

            try
            {
                this.stream.Write(frameBytes);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("写入数据帧异常", ex);
                return ResponseCode.FAILED;
            }
        }

        /// <summary>
        /// 读取下一帧
        /// </summary>
        /// <returns></returns>
        public PlaybackFrame GetNextFrame()
        {
            FileStream stream = this.stream;

            try
            {
                // 读帧头
                byte[] frameHeader = new byte[FRAME_HEADER_SIZE];
                stream.Read(frameHeader, 0, frameHeader.Length);

                // 解析帧头
                long timestamp = BitConverter.ToInt64(frameHeader);
                int dataSize = BitConverter.ToInt32(frameHeader, 8);

                // 读帧数据
                byte[] frameData = new byte[dataSize];
                stream.Read(frameData, 0, frameData.Length);

                PlaybackFrame frame = new PlaybackFrame()
                {
                    Data = frameData,
                    Timestamp = timestamp
                };

                return frame;
            }
            catch (Exception ex)
            {
                logger.Error("读取数据帧异常", ex);
                return null;
            }
        }
    }
}
