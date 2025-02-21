using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Keyboard
{
    public abstract class KeyboardBase
    {
        public abstract byte[] TranslateInput(VTKeyboardInput userInput);

        /// <summary>
        /// 设置当前的光标键模式是否是ApplicationMode
        /// 光标键有两种模式，一种模式是ApplicationMode，另外一种是NormalMode
        /// 光标键就是上下左右键
        /// </summary>
        /// <param name="isApplicationMode"></param>
        public abstract void SetKeypadMode(bool isApplicationMode);

        /// <summary>
        /// 设置当前终端解析数据流的模式
        /// </summary>
        /// <param name="isAnsiMode"></param>
        public abstract void SetAnsiMode(bool isAnsiMode);

        public abstract void SetCursorKeyMode(bool isApplicationMode);
    }
}
