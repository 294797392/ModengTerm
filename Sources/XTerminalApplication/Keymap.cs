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
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DefaultKeymap");

        private static readonly Dictionary<VTKeys, byte[]> Key2BytesTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'a' } }, { VTKeys.B, new byte[] { (byte)'b' } }, { VTKeys.C, new byte[] { (byte)'c' } }, { VTKeys.D, new byte[] { (byte)'d' } },
            { VTKeys.E, new byte[] { (byte)'e' } }, { VTKeys.F, new byte[] { (byte)'f' } }, { VTKeys.G, new byte[] { (byte)'g' } }, { VTKeys.H, new byte[] { (byte)'h' } },
            { VTKeys.I, new byte[] { (byte)'i' } }, { VTKeys.J, new byte[] { (byte)'j' } }, { VTKeys.K, new byte[] { (byte)'k' } }, { VTKeys.L, new byte[] { (byte)'l' } },
            { VTKeys.M, new byte[] { (byte)'m' } }, { VTKeys.N, new byte[] { (byte)'n' } }, { VTKeys.O, new byte[] { (byte)'o' } }, { VTKeys.P, new byte[] { (byte)'p' } },
            { VTKeys.Q, new byte[] { (byte)'q' } }, { VTKeys.R, new byte[] { (byte)'r' } }, { VTKeys.S, new byte[] { (byte)'s' } }, { VTKeys.T, new byte[] { (byte)'t' } },
            { VTKeys.U, new byte[] { (byte)'u' } }, { VTKeys.V, new byte[] { (byte)'v' } }, { VTKeys.W, new byte[] { (byte)'w' } }, { VTKeys.X, new byte[] { (byte)'x' } },
            { VTKeys.Y, new byte[] { (byte)'y' } }, { VTKeys.Z, new byte[] { (byte)'z' } },

            { VTKeys.Enter, new byte[] { (byte)'\n' } }, { VTKeys.Space, new byte[] { (byte)' ' } }
        };

        #endregion

        #region 实例变量

        private byte[] capitalBytes;

        #endregion

        #region 构造方法

        public DefaultKeymap()
        {
            this.capitalBytes = new byte[1];
        }

        #endregion

        #region Keymap

        public override byte[] MapKey(VTInputEvent evt)
        {
            byte[] bytes;
            if (!Key2BytesTable.TryGetValue(evt.Key, out bytes))
            {
                logger.ErrorFormat("未找到Key - {0}的映射关系", evt.Key);
                return null;
            }

            // 这里表示输入的是大写字母
            if (evt.Key >= VTKeys.A && evt.Key <= VTKeys.Z && evt.CapsLock)
            {
                capitalBytes[0] = (byte)(bytes[1] - 32);
                return capitalBytes;
            }

            return bytes;
        }

        #endregion
    }
}

