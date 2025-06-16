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
    public interface IHostSidePanel
    {
        /// <summary>
        /// 读取或设置侧边栏显示的位置
        /// </summary>
        SidePanelDocks Dock { get; set; }

        object ID { get; }

        string Name { get; }

        /// <summary>
        /// 获取该侧边栏是否是打开状态
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        void Close();

        /// <summary>
        /// 侧边栏打开的时候，就关闭
        /// 侧边栏关闭的时候，就打开
        /// </summary>
        void SwitchStatus();
    }
}
