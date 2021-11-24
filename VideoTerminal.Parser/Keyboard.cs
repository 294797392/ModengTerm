using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 表示终端的键盘按键转换器
    /// </summary>
    public abstract class Keyboard
    {
        /// <summary>
        /// 把系统按键转换成终端字符
        /// </summary>
        /// <param name="systemKey">要转换的系统按键</param>
        /// <returns></returns>
        public abstract char TranslateKey(char systemKey);
    }
}
