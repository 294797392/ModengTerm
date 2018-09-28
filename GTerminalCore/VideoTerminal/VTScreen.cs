using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public interface VTScreen
    {
        /// <summary>
        /// 打印字符/字符串
        /// </summary>
        /// <param name="text"></param>
        void PrintText(char text);
        void PrintText(string text);

        /// <summary>
        /// 执行退格操作
        /// </summary>
        void Backspace();
    }
}
