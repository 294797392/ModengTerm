using AsciiControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions.CfInvocationConverters
{
    /// <summary>
    /// 用于把ControlFunction转换成Invocation的类
    /// 注意，这个类只会转换以ESC开头的ControlFunction
    /// 非ESC开头的ControlFunction会在ControlFunctions类里使用ICfParser进行转换
    /// </summary>
    public interface IConverter
    {
        bool Convert(IFormattedCf cf, out ICfInvocation invocation);
    }
}