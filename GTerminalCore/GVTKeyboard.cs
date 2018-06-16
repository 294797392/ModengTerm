using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalCore
{
    public class GVTKeyboard : IVTKeyboard
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("GVTKeyboard");

        /// <summary>
        /// 获取大写是否锁定
        /// </summary>
        public bool CapsLocked
        {
            get
            {
                byte[] bs = new byte[256];
                WindowsAPI.GetKeyboardState(bs);
                return (bs[0x14] == 1);
            }
        }

        public bool HasDirectionsKeypad
        {
            get
            {
                return true;
            }
        }

        public bool HasMumericKeypad
        {
            get
            {
                return true;
            }
        }

        public byte[] GetCurrentInputData(KeyEventArgs key)
        {
            Dictionary<Key, byte[]> iptTbl = null;

            // 按下了Control键
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                iptTbl = VTInputTbl.ControlTable;
                goto Translate;
            }

            // 按下了Shift
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                iptTbl = VTInputTbl.ShiftTable;
                goto Translate;
            }

            // 打开了大写锁定
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift || this.CapsLocked)
            {
                iptTbl = VTInputTbl.AnsiUpperTable;
                goto Translate;
            }

            // 小写
            iptTbl = VTInputTbl.AnsiLowerTable;

            Translate:
            byte[] data;
            if (!iptTbl.TryGetValue(key.Key, out data))
            {
                logger.ErrorFormat("未定义按键{0}", key.Key);
            }

            return data;
        }
    }
}