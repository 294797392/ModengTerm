using log4net.Repository.Hierarchy;
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
    /// 点击顶部菜单栏或者右键菜单所执行的事件处理器
    /// </summary>
    /// <param name="e"></param>
    public delegate void AddonCommandDelegate(CommandArgs e);

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

        private Dictionary<string, Command> registerCommands;

        private List<IOverlayPanel> overlayPanels;

        protected ClientFactory factory;
        protected IClient client;
        protected IClientStorage storageService;
        protected IClientEventRegistry eventRegistry;

        #endregion

        #region 属性

        public string ID { get; private set; }

        public AddonMetadata Metadata { get { return this.metadata; } }

        /// <summary>
        /// 插件注册的所有命令
        /// </summary>
        public Dictionary<string, Command> RegisteredCommand { get { return this.registerCommands; } }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Active(ActiveContext context)
        {
            this.ID = context.Definition.ID;
            this.factory = context.Factory;
            this.registerCommands = new Dictionary<string, Command>();
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

            // Release
            this.registerCommands.Clear();
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 监听某个命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handler"></param>
        public void RegisterCommand(string command, AddonCommandDelegate handler, object userData = null)
        {
            if (this.registerCommands.ContainsKey(command))
            {
                return;
            }

            Command cmd = new Command()
            {
                Delegate = handler,
                UserData = userData
            };

            this.registerCommands[command] = cmd;
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
