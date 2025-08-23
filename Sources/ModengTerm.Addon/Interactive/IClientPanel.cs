using ModengTerm.Base.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 向插件公开控制Panel的接口
    /// </summary>
    public interface IClientPanel
    {
        /// <summary>
        /// 获取该面板是否是打开状态
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// 打开面板
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭面板
        /// </summary>
        void Close();

        /// <summary>
        /// 面板打开的时候，就关闭
        /// 面板关闭的时候，就打开
        /// </summary>
        void SwitchStatus();
    }

    /// <summary>
    /// 向插件公开SidePanel的接口
    /// </summary>
    public interface ISidePanel : IClientPanel
    {
        /// <summary>
        /// Panel显示的位置
        /// </summary>
        SidePanelDocks Dock { get; }
    }

    /// <summary>
    /// 向插件公开OverlayPanel的接口
    /// </summary>
    public interface IOverlayPanel : IClientPanel
    {
        OverlayPanelDocks Dock { get; set; }
    }
}
