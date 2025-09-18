using ModengTerm.Addon.ClientBridges;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Metadatas;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 插件生命周期管理
    /// 插件事件管理
    /// </summary>
    public abstract class AddonModule
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("AddonModule");

        #endregion

        #region 事件

        #endregion

        #region 实例变量

        private AddonMetadata metadata;

        internal IClient client;
        internal IClientStorage storageService;
        internal IClientEventRegistry eventRegistry;

        #endregion

        #region 属性

        public string ID { get { return this.metadata.ID; } }

        public AddonMetadata Metadata { get { return this.metadata; } }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Active(ActiveContext context)
        {
            this.metadata = context.Definition;

            this.client = Client.GetClient();
            this.storageService = Client.GetStorage();
            this.eventRegistry = Client.GetEventRegistry();

            this.OnActive(context);
        }

        public void Deactive()
        {
            this.OnDeactive();
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 注册命令处理器
        /// 该命令可以通过任意一个会话的菜单触发，开发者可以在处理器里获取到触发该命令的会话
        /// </summary>
        /// <param name="command">要注册的命令</param>
        /// <param name="delegate">命令执行函数</param>
        /// <param name="userData">回传给回调的用户数据</param>
        public void RegisterCommand(string command, CommandDelegate @delegate, object userData = null)
        {
            string commandKey = AddonUtils.GetCommandKey(this.metadata, command);

            this.eventRegistry.RegisterCommand(commandKey, @delegate, userData);
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 当插件被激活的时候触发
        /// </summary>
        protected abstract void OnActive(ActiveContext context);

        /// <summary>
        /// 当插件被注销的时候触发
        /// </summary>
        protected abstract void OnDeactive();

        #endregion

        #region 实例方法

        #endregion
    }
}
