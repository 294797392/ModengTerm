using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Utils
{
    /// <summary>
    /// 根据Control Functions for Coded Character Sets Ecma-048.pdf标准，
    /// 提供一些处理ASCII转义字符的工具函数
    /// 这是一个ECMA定义的标准，所有的终端模拟器都会实现这个标准
    /// 参考：
    ///     Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    ///     
    /// 控制序列：
    ///     对于7位编码的ASCII码：
    ///         1.由两个ASCII字符表示一个控制序列的开头，第一个字符是01/11（27），第二个字符在04/00（64） - 05/15（95）之间
    ///     
    ///     对于8位编码的ASCII码：
    ///         1.由08/00（128） - 09/15（159）这之间的1个ASCII字符表示一个控制序列的开头
    ///     
    ///     控制序列的格式是：
    ///         CSI P..P I..I F
    ///         CSI：控制序列字符，在7位编码中，由01/11 05/11两个字符组成；在8位编码中，由09/11单个字符组成
    ///         P..P：参数字符串，由03/00（48） - 03/15（63）之间的字符组成
    ///         I..I：中间字符串，由02/00（32） - 02/15（47）之间的字符组成，后面会跟一个字符串终结字符（F）
    ///         F：结束字符，由04/00（64） - 07/14（126）之间的某个字符表示。07/00 - 07/14之间的字符也是结束符，但是这是留给厂商做实验使用的。注意，带有中间字符串和不带有中间字符串的结束符的含义不一样
    /// </summary>
    public class CharacterUtilis
    {
    }
}