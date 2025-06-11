using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 维护插件实例状态
    /// </summary>
    public class AddonContext
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AddonContext");

        /// <summary>
        /// 插件实例
        /// </summary>
        private AddonModule instance;

        /// <summary>
        /// 插件定义
        /// </summary>
        private AddonDefinition definition;

        /// <summary>
        /// 插件Id
        /// </summary>
        public string Id { get { return this.definition.ID; } }

        public AddonDefinition Definition { get { return this.definition; } }

        public AddonContext(AddonDefinition definition)
        {
            this.definition = definition;
        }

        private AddonModule EnsureInstance()
        {
            if (this.instance == null)
            {
                AddonModule addonModule = null;

                try
                {
                    addonModule = ConfigFactory<AddonModule>.CreateInstance(this.definition.ClassEntry);
                    addonModule.Definition = this.definition;
                    addonModule.Active(ActiveContext.Default);

                    this.instance = addonModule;
                }
                catch (Exception ex)
                {
                    logger.Error("创建插件实例异常", ex);
                    return null;
                }
            }

            return this.instance;
        }

        /// <summary>
        /// 如果插件订阅了某个事件，则触发指定事件处理器
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public void RaiseCommand(CommandArgs e)
        {
            this.instance.RaiseCommand(e);
        }

        public void RaiseEvent(EventName code, EventArgs args) 
        {
            this.instance.RaiseEvent(code, args);
        }

        public void Active() 
        {
            this.EnsureInstance();
        }
    }
}
