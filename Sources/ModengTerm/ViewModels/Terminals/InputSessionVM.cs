using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels.Terminals
{
    /// <summary>
    /// 定义一个可以处理键盘输入的会话
    /// </summary>
    public abstract class InputSessionVM : OpenedSessionVM
    {
        /// <summary>
        /// 是否发送到所有窗口
        /// </summary>
        public bool SendAll { get; set; }

        /// <summary>
        /// 处理用户输入数据
        /// </summary>
        /// <param name="userInput"></param>
        public abstract void SendInput(UserInput userInput);

        /// <summary>
        /// 输入纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public void SendInput(string text)
        {
            UserInput userInput = new UserInput()
            {
                CapsLock = false,
                Text = text,
                Key = ModengTerm.Terminal.VTKeys.GenericText,
                Modifiers = VTModifierKeys.None
            };

            this.SendInput(userInput);
        }

        protected InputSessionVM(XTermSession session) :
            base(session)
        {
        }
    }
}
