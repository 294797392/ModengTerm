using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore.Invocations
{
    /// <summary>
    /// IInvocation
    /// 封装执行控制功能的动作所需的信息
    /// 
    /// 存储要调用的一个控制功能所需的所有信息
    /// 比如：
    ///     设置字符的颜色，控制光标位置，移动光标，设置标题等等
    /// 这个类是最终让客户端执行动作的类，比如移动光标，设置窗口标题，设置字体颜色等等
    /// </summary>
    public interface IInvocation
    {
    }
}