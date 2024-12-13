using ModengTerm.Base.DataModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm
{
    public interface ISessionContent
    {
        /// <summary>
        /// 对应的打开的会话
        /// </summary>
        XTermSession Session { get; set; }

        /// <summary>
        /// 控件被完全显示之后调用
        /// </summary>
        /// <returns></returns>
        int Open(OpenedSessionVM sessionVM);

        void Close();

        /// <summary>
        /// 把输入焦点设置到控件上
        /// </summary>
        /// <returns></returns>
        bool SetInputFocus();
    }
}
