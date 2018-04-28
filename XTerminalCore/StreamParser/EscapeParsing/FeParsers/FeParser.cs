using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    /// <summary>
    /// 解析ControlFunction是ESC类型的的字符串
    /// </summary>
    public abstract class FeParser
    {
        /// <summary>
        /// String Terminator：字符串终结符
        /// 
        /// 每种终端可能使用不同的String Terminator
        /// 需要在界面做相应配置
        /// VT100使用ascii码的7，是\a
        /// </summary>
        public byte ST { get; set; }

        public FeParser()
        {
            this.ST = DefaultValues.ST;
        }

        /// <summary>
        /// 从数据流中解析出一个结构化的ControlFunction
        /// </summary>
        /// <param name="reader">要解析的数据流</param>
        /// <param name="cf">格式化之后的ControlFunction</param>
        /// <returns></returns>
        public abstract bool Parse(StreamReader reader, out IFormattedCf cf);
    }
}