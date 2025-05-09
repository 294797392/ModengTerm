using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 提供控制窗口的接口
    /// </summary>
    public class WindowController
    {
        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        public void VisiblePanel(string panelId)
        {
            MTermApp.Context.MainWindowVM.Panel.ChangeVisible(panelId);
        }
    }
}
