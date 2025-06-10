using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Shell
{
    /// <summary>
    /// 提供和界面进行交互的接口
    /// </summary>
    public abstract class IShellService
    {
        /// <summary>
        /// 打开会话
        /// </summary>
        /// <param name="session"></param>
        public abstract void OpenSession(XTermSession session);

        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        public abstract void VisiblePanel(string panelId);

        /// <summary>
        /// 获取当前激活的Shell
        /// </summary>
        /// <returns></returns>
        public abstract T GetActiveShell<T>() where T : IShellObject;

        /// <summary>
        /// 获取指定类型的所有会话Shell对象
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public abstract List<T> GetShellObjects<T>() where T : IShellObject;
    }
}