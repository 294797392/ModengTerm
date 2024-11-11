using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm.Terminal.EventArgs
{
    /// <summary>
    /// 当终端打印一个字符的时候触发
    /// </summary>
    public class PrintEventArgs
    {
        /// <summary>
        /// 打印的字符
        /// </summary>
        public char Character { get; set; }
    }
}
