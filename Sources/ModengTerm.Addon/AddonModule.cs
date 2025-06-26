using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public delegate void AddonCommandDelegate(CommandArgs e);

    /// <summary>
    /// 插件生命周期管理
    /// 插件事件管理
    /// 根据Activitions的值确定插件实例化的时机：
    /// 1. 在应用程序启动的时候就实例化
    /// 2. 在打开某个会话的时候实例化
    /// </summary>
    public abstract class AddonModule
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("AddonModule");

        #endregion

        #region 事件

        #endregion

        #region 实例变量

        private AddonDefinition definition;

        private Dictionary<string, AddonCommandDelegate> registerCommands;

        private List<ISidePanel> sidePanels;
        private List<IOverlayPanel> overlayPanels;

        protected ClientFactory factory;
        protected IClient client;
        protected StorageService storageService;
        protected IClientEventRegistry eventRegistry;

        #endregion

        #region 属性

        public string ID { get; private set; }

        /// <summary>
        /// 插件注册的所有命令
        /// </summary>
        public Dictionary<string, AddonCommandDelegate> RegisteredCommand { get { return this.registerCommands; } }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Active(ActiveContext context)
        {
            this.ID = context.Definition.ID;
            this.factory = context.Factory;
            this.registerCommands = new Dictionary<string, AddonCommandDelegate>();
            this.overlayPanels = new List<IOverlayPanel>();
            this.definition = context.Definition;

            // 此时只是创建了HostPanel的实例，但是还没有真正加到界面上，调用AddSidePanel加到界面上
            this.sidePanels = this.factory.CreateSidePanels(context.Definition.SidePanels);
            this.client = this.factory.GetClient();
            this.storageService = this.factory.GetStorageService();
            this.eventRegistry = this.factory.GetEventRegistry();

            this.OnActive(context);
        }

        public void Deactive()
        {
            this.OnDeactive();

            // Release
            this.registerCommands.Clear();
        }

        public void RaiseCommand(CommandArgs e)
        {
            AddonCommandDelegate handler;
            if (!this.registerCommands.TryGetValue(e.Command, out handler))
            {
                return;
            }

            handler(e);
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 监听某个命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handler"></param>
        protected void RegisterCommand(string command, AddonCommandDelegate handler)
        {
            if (this.registerCommands.ContainsKey(command))
            {
                return;
            }

            this.registerCommands[command] = handler;
        }

        protected ISidePanel GetSidePanel(string id)
        {
            return this.sidePanels.FirstOrDefault(v => v.ID.ToString() == id);
        }

        protected IOverlayPanel CreateOverlayPanel(string id)
        {
            PanelDefinition definition = this.definition.OverlayPanels.FirstOrDefault(v => v.ID == id);
            if (definition == null)
            {
                logger.ErrorFormat("没找到指定的OverlayPanel, {0}", id);
                return null;
            }

            return this.factory.CreateOverlayPanel(definition);
        }

        /// <summary>
        /// 先查找指定的OverlayPanel是否存在，如果不存在则创建并加到指定的shellTab里
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="shellTab"></param>
        /// <returns>OverlayPanel的实例</returns>
        protected IOverlayPanel EnsureOverlayPanel(string panelId, IClientShellTab shellTab)
        {
            IOverlayPanel overlayPanel = shellTab.GetOverlayPanel(panelId);
            if (overlayPanel == null)
            {
                overlayPanel = this.CreateOverlayPanel(panelId);
                if (overlayPanel == null)
                {
                    return null;
                }

                shellTab.AddOverlayPanel(overlayPanel);
            }

            return overlayPanel;
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
