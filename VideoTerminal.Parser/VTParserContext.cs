using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 存储解析器当前的上下文信息
    /// </summary>
    public class VTParserContext
    {
        /// <summary>
        /// 记录上一个解析的字符
        /// </summary>
        public byte PreviousChar { get; set; }

        /// <summary>
        /// 记录当前正在解析的字符
        /// </summary>
        public byte CurrentChar { get; set; }

        /// <summary>
        /// 当前解析到的字符的索引位置
        /// </summary>
        public int CharIndex { get; set; }

        /// <summary>
        /// 收集到的OSC参数
        /// </summary>
        public int OSCParameter { get; set; }

        /// <summary>
        /// 收集到的OSC字符串参数
        /// </summary>
        public StringBuilder OSCString { get; private set; }

        public VTParserContext()
        {
            this.OSCString = new StringBuilder();
        }

        /// <summary>
        /// 清空缓存数据
        /// </summary>
        public void Reset()
        {
            this.OSCParameter = 0;
            this.OSCString.Clear();
        }
    }
}
