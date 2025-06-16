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
        /// 获取当前激活的Shell
        /// </summary>
        /// <returns></returns>
        T GetActiveTab<T>() where T : IHostTab;

        /// <summary>
        /// 获取所有类型的会话对象
        /// </summary>
        /// <returns></returns>
        List<IHostTab> GetAllTabs();

        /// <summary>
        /// 获取指定类型的所有会话对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetAllTabs<T>() where T : IHostTab;

        void AddSidePanel(IHostPanel panel);

        void RemoveSidePanel(IHostPanel panel);
    }
}