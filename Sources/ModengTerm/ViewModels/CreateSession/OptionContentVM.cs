using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession
{
    public abstract class OptionContentVM : MenuContentVM
    {
        /// <summary>
        /// 加载指定会话里的参数
        /// </summary>
        /// <param name="session">要加载的会话</param>
        public abstract void LoadOptions(XTermSession session);

        /// <summary>
        /// 保存参数到指定的Session
        /// </summary>
        /// <param name="session">要保存的会话</param>
        /// <returns></returns>
        public abstract bool SaveOptions(XTermSession session);
    }
}
