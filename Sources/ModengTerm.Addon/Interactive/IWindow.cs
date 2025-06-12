using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 表示应用程序的窗口
    /// </summary>
    public abstract class IWindow
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
        public abstract T GetActivePanel<T>() where T : IPanel;

        /// <summary>
        /// 获取指定类型的所有会话Shell对象
        /// </summary>
        /// <returns></returns>
        public abstract List<IPanel> GetAllPanels();
    }
}