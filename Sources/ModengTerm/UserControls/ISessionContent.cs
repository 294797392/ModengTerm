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
        /// 当前打开的会话的ViewModel
        /// </summary>
        OpenedSessionVM OpenedSessionVM { get; }

        /// <summary>
        /// 对应的打开的会话
        /// </summary>
        XTermSession Session { get; set; }

        /// <summary>
        /// 控件被完全Loaded的时候调用
        /// </summary>
        /// <returns></returns>
        int Open();

        void Close();
    }
}
