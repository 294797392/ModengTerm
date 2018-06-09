using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalControl
{
    public abstract class VideoTerminal : IVideoTerminal
    {
        public event Action<object, byte, byte[]> Action;

        /// <summary>
        /// 是否解析C1字符集
        /// </summary>
        public virtual bool SupportC1Characters { get { return false; } }

        public abstract VTTypeEnum Type { get; }

        public abstract bool OnKeyDown(KeyEventArgs key, out byte[] data);

        public abstract void StartParsing(IVTStream stream);
    }
}