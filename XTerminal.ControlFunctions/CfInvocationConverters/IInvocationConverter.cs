using AsciiControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions.CfInvocationConverters
{
    /// <summary>
    /// 每种终端对ControlFunction里内容的定义可能不同，这里通过一个接口去解析不同终端里的ControlFunction
    /// 
    /// 根据ControlFunction里的内容，
    /// 把<seealso cref="AsciiControlFunctions.IControlFunction"/>转换成一个<seealso cref="AsciiControlFunctions.CfInvocations.ICfInvocation"/>
    /// </summary>
    public interface IInvocationConverter
    {
        bool Convert(IFormattedCf cf, out ICfInvocation invocation);
    }
}