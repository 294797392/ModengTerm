using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    /// <summary>
    /// 管理虚拟终端键盘信息
    /// </summary>
    public interface IVTKeyboard
    {
        /// <summary>
        /// 是否带有数字键盘
        /// </summary>
        bool HasMumericKeypad { get; }

        /// <summary>
        /// 是否带有方向键盘
        /// </summary>
        bool HasDirectionsKeypad { get; }

        /// <summary>
        /// 获取当前键盘输入的数据
        /// </summary>
        /// <returns>
        /// 当前键盘的输入数据
        /// 失败或者无输入返回null
        /// </returns>
        byte[] GetCurrentInputData();
    }
}