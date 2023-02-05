using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    /// <summary>
    /// 存储不同模式下，键盘按键与终端字节流的映射关系
    /// </summary>
    public abstract class Keymap //: Dictionary<KeyboardMaps, Dictionary<VTKeys, string>>
    {
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mkey"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract byte[] MapKey(VTKeys key, VTModifierKeys mkey, string text);
    }
}

