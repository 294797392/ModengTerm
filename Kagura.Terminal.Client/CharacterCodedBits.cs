using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Terminal.Client
{
    /// <summary>
    /// Ascii编码方式
    /// 1.7位编码
    /// 2.8位编码
    /// 
    /// 每种主机的编码可能会不同，编码不同，解析ControlFunction的方式也不同
    /// </summary>
    public enum CharacterCodedBits
    {
        _7Bit,
        _8Bit
    }
}