using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    /// <summary>
    /// 表示一个监控对象
    /// </summary>
    public abstract class WatchObject : PanelContentVM
    {
        #region 属性

        #endregion

        #region 公开接口

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 执行定时任务
        /// </summary>
        public abstract void Watch(AbstractWatcher watcher);

        #endregion
    }
}
