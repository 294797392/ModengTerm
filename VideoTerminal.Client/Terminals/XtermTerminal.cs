using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminals
{
    /// <summary>
    /// xterm终端，有自己的控制序列
    /// 参考：
    ///     
    /// </summary>
    public class XtermTerminal
    {
        //        ESC ] 0 ; txt BEL 将图标名和窗口标题设为文本.
        //ESC ] 1 ; txt BEL 将图标名设为文本.
        //ESC ] 2 ; txt BEL 将窗口名设为文本.
        //ESC ] 4 6 ; name BEL 改变日志文件名(一般由编译时选项禁止)
        //ESC ] 5 0 ; fn BEL 字体设置为 fn.
        //        ESC F 光标移动到屏幕左下角(由hpLowerleftBugCompat 打开这项设置)
        //ESC l 内存锁定(对于 HP 终端).锁定光标以上的内存.
        //ESC m 内存解锁(对于 HP 终端). 
        //ESC n LS2 调用 G2 字符集.
        //ESC o LS3 调用 G3 字符集.
        //ESC | LS3R 以GR调用 G3 字符集.在xterm上看不到效果.
        //ESC }
        //    LS2R 以GR调用 G3 字符集.在xterm上看不到效果.
        //ESC ~ LS1R 以GR调用 G3 字符集.在xterm上看不到效果.

        //它不识别 ESC % ...


    }
}
