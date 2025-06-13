using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 向插件公开应用程序主窗口的功能
    /// </summary>
    public interface IHostWindow
    {
        /// <summary>
        /// 打开会话
        /// </summary>
        /// <param name="session"></param>
        /// <param name="addToRecent">是否加入到最近打开的列表里</param>
        void OpenSession(XTermSession session, bool addToRecent = false);

        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        void VisiblePanel(string panelId);

        /// <summary>
        /// 获取当前激活的Shell
        /// </summary>
        /// <returns></returns>
        T GetActivePanel<T>() where T : IHostPanel;

        /// <summary>
        /// 获取指定类型的所有会话Shell对象
        /// </summary>
        /// <returns></returns>
        List<IHostPanel> GetAllPanels();
    }
}