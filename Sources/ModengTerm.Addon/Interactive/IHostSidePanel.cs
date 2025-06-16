using log4net.Repository.Hierarchy;
using ModengTerm.Addon.Extensions;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 指定SidePanel要显示的位置
    /// </summary>
    public enum SidePanelDocks
    {
        /// <summary>
        /// 显示在左边
        /// </summary>
        Left,

        /// <summary>
        /// 显示在右边
        /// </summary>
        Right
    }

    /// <summary>
    /// 向插件公开控制侧边栏的接口
    /// </summary>
    public interface IHostSidePanel : IHostPanel
    {
        /// <summary>
        /// 读取或设置侧边栏显示的位置
        /// </summary>
        SidePanelDocks Dock { get; set; }

        /// <summary>
        /// 获取插件端的Panel对象
        /// </summary>
        SidePanel ClientPanel { get; }
    }
}
