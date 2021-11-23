using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace VideoTerminal.Parser
{
    public interface VTScreen
    {
        /// <summary>
        /// 获取或设置光标所在行
        /// </summary>
        int CursorRow { get; set; }

        /// <summary>
        /// 获取或设置光标所在列
        /// </summary>
        int CursorColumn { get; set; }

        /// <summary>
        /// 打印字符/字符串
        /// </summary>
        /// <param name="text"></param>
        void PrintText(string text);

        /// <summary>
        /// 以当前光标位置为原点移动光标
        /// Column：
        ///     正：向右移动
        ///     负：向左移动
        ///     0 ：不移动
        /// Row：
        ///     正：向上移动
        ///     负：向下移动
        ///     0 ：不移动
        /// </summary>
        void MoveCursor(int col, int row);

        /// <summary>
        /// 删除一行数据
        /// </summary>
        /// <param name="startCol">开始行数</param>
        /// <param name="count">要删除的符号个数</param>
        void EraseLine(int startCol, int count);
    }
}
