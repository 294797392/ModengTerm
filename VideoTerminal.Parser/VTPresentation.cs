using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 存储当前界面展现信息
    /// </summary>
    public class VTPresentation
    {
        /// <summary>
        /// 光标行数
        /// </summary>
        public int CursorRow { get; set; }

        /// <summary>
        /// 光标列数
        /// </summary>
        public int CursorColumn { get; set; }

        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string Text { get; set; }
    }
}
