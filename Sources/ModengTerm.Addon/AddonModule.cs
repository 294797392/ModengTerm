using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
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

        private List<IOverlayPanel> overlayPanels;

        protected ClientFactory factory;
        protected IClient client;
        protected IClientStorage storageService;
        protected IClientEventRegistry eventRegistry;

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
            this.factory = context.Factory;
            this.overlayPanels = new List<IOverlayPanel>();
            this.metadata = context.Definition;

            this.client = this.factory.GetClient();
            this.storageService = this.factory.GetStorageService();
            this.eventRegistry = this.factory.GetEventRegistry();

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
        /// </summary>
        /// <param name="command">要注册的命令</param>
        /// <param name="delegate">命令执行函数</param>
        /// <param name="userData">回传给回调的用户数据</param>
        public void RegisterCommand(string command, AddonCommandDelegate @delegate, object userData = null)
        {
            // 多个插件可能注册了相同的command，需要保证每个插件注册的command不一致
            string commandKey = AddonUtils.GetCommandKey(this.ID, command);

            this.eventRegistry.RegisterCommand(commandKey, @delegate, userData);
        }

        protected string GetObjectId()
        {
            return Guid.NewGuid().ToString();
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
