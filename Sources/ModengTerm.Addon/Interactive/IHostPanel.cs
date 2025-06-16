using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public interface IHostPanel
    {
        SidePanelDocks Dock { get; }

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
