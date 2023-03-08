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
        public static readonly Keymap Default = new DefaultKeymap();

        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mkey"></param>
        /// <param name="capsLock">capslock按键的状态</param>
        /// <param name="text">输入的中文数据</param>
        /// <returns></returns>
        public abstract byte[] MapKey(VTKeys key, VTModifierKeys mkey, bool capsLock, string text);
    }

    /// <summary>
    /// 默认模式下的按键映射
    /// </summary>
    public class DefaultKeymap : Keymap
    {
        private static readonly Dictionary<VTKeys, byte[]> Key2BytesTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'A' } },{ VTKeys.A, new byte[] { (byte)'A' } },
        };

        public override byte[] MapKey(VTKeys key, VTModifierKeys mkey, bool capsLock, string text)
        {
            switch (mkey)
            {
                case VTModifierKeys.None:
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            if (capsLock)
                            {
                                // 大写
                            }
                            else
                            {
                                // 小写
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

