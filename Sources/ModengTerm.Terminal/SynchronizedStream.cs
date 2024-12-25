using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 拷贝Ssh.Net的代码
    /// </summary>
    public class SynchronizedStream
    {
        #region 实例变量

        private readonly object _sync = new object();
        private byte[] _readBuffer = new byte[16384];
        private int _readHead; // The index from which the data starts in _readBuffer.
        private int _readTail; // The index at which to add new data into _readBuffer.
        private bool _disposed;

        #endregion

        #region 属性

        #endregion

        #region 公开接口

        public void Write(byte[] data, int length)
        {
            lock (_sync)
            {
                // Ensure sufficient buffer space and copy the new data in.

                if (_readBuffer.Length - _readTail >= length)
                {
                    // If there is enough space after _tail for the new data,
                    // then copy the data there.
                    Buffer.BlockCopy(data, 0, _readBuffer, _readTail, length);
                    _readTail += length;
                }
                else
                {
                    // We can't fit the new data after _tail.

                    var newLength = _readTail - _readHead + length;

                    if (newLength <= _readBuffer.Length)
                    {
                        // If there is sufficient space at the start of the buffer,
                        // then move the current data to the start of the buffer.
                        Buffer.BlockCopy(_readBuffer, _readHead, _readBuffer, 0, _readTail - _readHead);
                    }
                    else
                    {
                        // Otherwise, we're gonna need a bigger buffer.
                        var newBuffer = new byte[Math.Max(newLength, _readBuffer.Length * 2)];
                        Buffer.BlockCopy(_readBuffer, _readHead, newBuffer, 0, _readTail - _readHead);
                        _readBuffer = newBuffer;
                    }

                    // Copy the new data into the freed-up space.
                    Buffer.BlockCopy(data, 0, _readBuffer, _readTail - _readHead, length);

                    _readHead = 0;
                    _readTail = newLength;
                }

                Monitor.PulseAll(_sync);
            }
        }

        /// <summary>
        /// 从接收缓冲区里读取指定的数据
        /// </summary>
        /// <param name="buffer">存储收到的数据的缓冲区</param>
        /// <param name="size">要接收的数据长度</param>
        /// <returns>接收到的数据长度</returns>
        public int Read(byte[] buffer, int offset, int count, int timeout)
        {
            lock (_sync)
            {
                while (_readHead == _readTail && !_disposed)
                {
                    if (!Monitor.Wait(_sync, timeout))
                    {
                        return 0;
                    }
                }

                if (_disposed) 
                {
                    return -1;
                }

                var bytesRead = Math.Min(count, _readTail - _readHead);

                Buffer.BlockCopy(_readBuffer, _readHead, buffer, offset, bytesRead);

                _readHead += bytesRead;

                return bytesRead;
            }
        }

        public void Close() 
        {
            lock (_sync)
            {
                _disposed = true;

                Monitor.PulseAll(_sync);
            }
        }

        #endregion
    }
}
