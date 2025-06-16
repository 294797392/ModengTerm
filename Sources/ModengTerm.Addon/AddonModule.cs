using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public delegate void AddonCommandHandler(CommandArgs e);

    public delegate void AddonEventHandler(HostEvent evType, HostEventArgs evArgs);

    /// <summary>
    /// 插件生命周期管理
    /// 插件事件管理
    /// 根据Activitions的值确定插件实例化的时机：
    /// 1. 在应用程序启动的时候就实例化
    /// 2. 在打开某个会话的时候实例化
    /// </summary>
    public abstract class AddonModule
    {
        #region 事件

        #endregion

        #region 实例变量

        private Dictionary<string, AddonCommandHandler> registerCommands;

        private Dictionary<HostEvent, AddonEventHandler> registeredEvent;

        private List<IHostPanel> panels;

        protected HostFactory factory;
        protected IHostWindow hostWindow;
        protected StorageService storageService;

        #endregion

        #region 属性

        public string ID { get; private set; }

        public Dictionary<string, AddonCommandHandler> RegisteredCommand { get { return this.registerCommands; } }

        public Dictionary<HostEvent, AddonEventHandler> RegisteredEvent { get { return this.registeredEvent; } }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Active(ActiveContext context)
        {
            this.ID = context.Definition.ID;
            this.factory = context.Factory;
            this.registerCommands = new Dictionary<string, AddonCommandHandler>();
            this.registeredEvent = new Dictionary<HostEvent, AddonEventHandler>();

            // 此时只是创建了HostPanel的实例，但是还没有真正加到界面上，调用AddSidePanel加到界面上
            this.panels = this.factory.CreatePanels(context.Definition.Panels);
            this.hostWindow = context.HostWindow;
            this.storageService = context.StorageService;

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
            AddonCommandHandler handler;
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
        protected void RegisterCommand(string command, AddonCommandHandler handler)
        {
            if (this.registerCommands.ContainsKey(command))
            {
                return;
            }

            this.registerCommands[command] = handler;
        }

        /// <summary>
        /// 监听某个事件
        /// </summary>
        /// <param name="evType"></param>
        /// <param name="handler"></param>
        protected void RegisterEvent(HostEvent evType, AddonEventHandler handler)
        {
            if (this.registeredEvent.ContainsKey(evType))
            {
                return;
            }

            this.registeredEvent[evType] = handler;
        }

        protected IHostPanel GetPanel(string id)
        {
            return this.panels.FirstOrDefault(v => v.ID.ToString() == id);
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
