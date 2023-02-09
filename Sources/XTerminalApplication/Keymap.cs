using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalDevice
{
    /// <summary>
    /// 存储不同模式下，键盘按键与终端字节流的映射关系
    /// </summary>
    public abstract class Keymap
    {
        /// <summary>
        /// 映射
        /// </summary>
        /// <param name="evt">系统的输入事件</param>
        /// <returns></returns>
        public abstract byte[] MapKey(VTInputEvent evt);
    }

    /// <summary>
    /// 默认模式下的按键映射
    /// </summary>
    public class DefaultKeymap : Keymap
    {
        private static readonly Dictionary<VTKeys, byte[]> Key2BytesTableCapsLock = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'A' } }, { VTKeys.B, new byte[] { (byte)'B' } }, { VTKeys.C, new byte[] { (byte)'C' } }, { VTKeys.D, new byte[] { (byte)'D' } },
            { VTKeys.E, new byte[] { (byte)'E' } }, { VTKeys.F, new byte[] { (byte)'F' } }, { VTKeys.G, new byte[] { (byte)'G' } }, { VTKeys.H, new byte[] { (byte)'H' } },
            { VTKeys.I, new byte[] { (byte)'I' } }, { VTKeys.J, new byte[] { (byte)'J' } }, { VTKeys.K, new byte[] { (byte)'K' } }, { VTKeys.L, new byte[] { (byte)'L' } },
            { VTKeys.M, new byte[] { (byte)'M' } }, { VTKeys.N, new byte[] { (byte)'N' } }, { VTKeys.O, new byte[] { (byte)'O' } }, { VTKeys.P, new byte[] { (byte)'P' } },
            { VTKeys.Q, new byte[] { (byte)'Q' } }, { VTKeys.R, new byte[] { (byte)'R' } }, { VTKeys.S, new byte[] { (byte)'S' } }, { VTKeys.T, new byte[] { (byte)'T' } },
            { VTKeys.U, new byte[] { (byte)'U' } }, { VTKeys.V, new byte[] { (byte)'V' } }, { VTKeys.W, new byte[] { (byte)'W' } }, { VTKeys.X, new byte[] { (byte)'X' } },
            { VTKeys.Y, new byte[] { (byte)'Y' } }, { VTKeys.Z, new byte[] { (byte)'Z' } },

            { VTKeys.Enter, new byte[] { (byte)'\n' } }, { VTKeys.Spacebar, new byte[] { (byte)' ' } }
        };

        private static readonly Dictionary<VTKeys, byte[]> Key2BytesTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'a' } }, { VTKeys.B, new byte[] { (byte)'b' } }, { VTKeys.C, new byte[] { (byte)'c' } }, { VTKeys.D, new byte[] { (byte)'d' } },
            { VTKeys.E, new byte[] { (byte)'e' } }, { VTKeys.F, new byte[] { (byte)'f' } }, { VTKeys.G, new byte[] { (byte)'g' } }, { VTKeys.H, new byte[] { (byte)'h' } },
            { VTKeys.I, new byte[] { (byte)'i' } }, { VTKeys.J, new byte[] { (byte)'j' } }, { VTKeys.K, new byte[] { (byte)'k' } }, { VTKeys.L, new byte[] { (byte)'l' } },
            { VTKeys.M, new byte[] { (byte)'m' } }, { VTKeys.N, new byte[] { (byte)'n' } }, { VTKeys.O, new byte[] { (byte)'o' } }, { VTKeys.P, new byte[] { (byte)'p' } },
            { VTKeys.Q, new byte[] { (byte)'q' } }, { VTKeys.R, new byte[] { (byte)'r' } }, { VTKeys.S, new byte[] { (byte)'s' } }, { VTKeys.T, new byte[] { (byte)'t' } },
            { VTKeys.U, new byte[] { (byte)'u' } }, { VTKeys.V, new byte[] { (byte)'v' } }, { VTKeys.W, new byte[] { (byte)'w' } }, { VTKeys.X, new byte[] { (byte)'x' } },
            { VTKeys.Y, new byte[] { (byte)'y' } }, { VTKeys.Z, new byte[] { (byte)'z' } },

            { VTKeys.Enter, new byte[] { (byte)'\n' } }, { VTKeys.Spacebar, new byte[] { (byte)' ' } }
        };

        public override byte[] MapKey(VTInputEvent evt)
        {
            if (evt.CapsLock)
            {
                // 大写锁定了
                return Key2BytesTableCapsLock[evt.Key];
            }
            else
            {
                // 大写没锁定
                return Key2BytesTable[evt.Key];
            }
        }
    }
}

