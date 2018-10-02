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
        /// 设置光标位置，相对于当前的位置
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
