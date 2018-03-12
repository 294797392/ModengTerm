using ControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFunctions.FeParsers
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
        /// 把一串带有控制功能的字符转换成一个格式化之后的控制功能结构体
        /// </summary>
        /// <param name="chars">第一个字符是Fe Code的字符串</param>
        /// <param name="cf">格式化之后的ControlFunction</param>
        /// <returns></returns>
        public abstract bool Parse(byte[] chars, out IFormattedCf cf);
    }
}