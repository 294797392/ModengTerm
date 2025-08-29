using DotNEToolkit.DataAccess;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Terminal
{
    /// <summary>
    /// 定义一个可以处理键盘输入的会话
    /// </summary>
    public abstract class InputSessionVM : OpenedSessionVM
    {
        /// <summary>
        /// 处理用户按键输入数据
        /// </summary>
        /// <param name="keyInput">按键输入信息</param>
        public abstract void SendInput(VTKeyboardInput keyInput);

        /// <summary>
        /// 输入原始数据
        /// </summary>
        /// <param name="rawData"></param>
        public abstract void SendRawData(byte[] rawData);

        /// <summary>
        /// 输入纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public abstract void SendText(string text);

        protected InputSessionVM(XTermSession session) :
            base(session)
        {
        }
    }
}
