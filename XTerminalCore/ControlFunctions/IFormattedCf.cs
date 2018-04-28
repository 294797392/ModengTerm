using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    /// <summary>
    /// IFormattedControlFunction
    /// 存储ControlFunction里的信息
    /// </summary>
    public interface IFormattedCf
    {
        /// <summary>
        /// 获取ControlFunction内容所占的字节数
        /// </summary>
        /// <returns></returns>
        int GetSize();
    }
}