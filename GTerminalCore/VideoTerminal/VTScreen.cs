using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace GardeniaTerminalCore
{
    public interface VTScreen
    {
        /// <summary>
        /// 获取当前光标的位置
        /// </summary>
        TextPointer CurrentCaretPosition { get; }

        /// <summary>
        /// 打印字符/字符串
        /// </summary>
        /// <param name="text"></param>
        void PrintText(string text);

        /// <summary>
        /// 执行退格操作
        /// </summary>
        void Backspace();

        /// <summary>
        /// 删除一个字符
        /// </summary>
        /// <param name="direction">要删除的字符数量</param>
        /// <param name="position">要删除的字符的位置</param>
        void EraseCharAtCaretPosition(int count, TextPointer position);
    }
}
