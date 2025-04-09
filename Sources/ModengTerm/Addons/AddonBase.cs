using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public abstract class AddonBase
    {
        #region 实例变量

        private List<AddonEventTypes> registerEvents;

        #endregion

        #region 属性

        internal AddonDefinition Definition { get; set; }

        #endregion

        #region Internal

        internal void Initialize()
        {
            this.registerEvents = new List<AddonEventTypes>();

            this.OnInitialize();
        }

        internal void Release()
        {
            this.OnRelease();

            // Release
            this.registerEvents.Clear();
        }

        internal void RaiseEvent(AddonEventTypes evt, params object[] evp)
        {
            if (!this.registerEvents.Contains(evt))
            {
                // 没注册事件
                return;
            }

            this.OnEvent(evt, evp);
        }

        #endregion

        #region 受保护方法

        protected void RegisterEvent(AddonEventTypes ev)
        {
            if (this.registerEvents.Contains(ev))
            {
                return;
            }

            this.registerEvents.Add(ev);
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        /// <summary>
        /// 当插件有事件触发的时候调用
        /// </summary>
        /// <param name="ev"></param>
        protected abstract void OnEvent(AddonEventTypes ev, params object[] param);

        #endregion
    }
}
