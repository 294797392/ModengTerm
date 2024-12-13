using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 用一个64位的整数来保存8个8位的整数
    /// 
    /// 用这个类来存储intermediate的目的是可以更方便的对不同参数进行比较
    /// 有些参数是一个字节，但是有些参数是两个字节
    /// 用VTID这个类对多字节参数进行封装
    /// </summary>
    public class VTID
    {
        /// <summary>
        /// 最多能存储8个Intermediate字节，每个Intermediate占用8位
        /// </summary>
        private List<byte> bytes;

        /// <summary>
        /// 获取第index个8位整数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                return this.bytes[index];
            }
        }

        public int Length { get { return this.bytes.Count; } }

        public VTID() 
        {
            this.bytes = new List<byte>();
        }

        /// <summary>
        /// 存储一个intermediate字符
        /// </summary>
        public void AddIntermediate(byte ch)
        {
            this.bytes.Add(ch);
        }

        public void Finalize(byte ch)
        {
            this.bytes.Add(ch);
        }

        public ulong SubSequence(int offset)
        {
            ulong value = 0;
            int shift = 0;

            for (int i = offset; i < this.bytes.Count; i++)
            {
                value = value << shift | this.bytes[i];
                shift++;
            }

            return value;
        }

        public void Clear()
        {
            this.bytes.Clear();
        }
    }
}
