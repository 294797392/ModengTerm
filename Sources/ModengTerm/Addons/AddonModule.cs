using ModengTerm.Base.Definitions;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public delegate void CommandHandlerDelegate(CommandEventArgs e);

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

        private Dictionary<string, CommandHandlerDelegate> registerCommands;

        #endregion

        #region 属性

        public string ID { get { return this.Definition.ID; } }

        public AddonDefinition Definition { get; set; }

        public AddonObjectStorage ObjectStorage { get; set; }

        /// <summary>
        /// 提供控制主窗口的接口
        /// </summary>
        public IWindowShell Shell { get; private set; }

        #endregion

        #region Internal

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Active(ActiveContext context)
        {
            this.registerCommands = new Dictionary<string, CommandHandlerDelegate>();

            this.OnActive(context);
        }

        public void Deactive()
        {
            this.OnDeactive();

            // Release
            this.registerCommands.Clear();
        }

        public void RaiseCommand(CommandEventArgs context)
        {
            CommandHandlerDelegate handler;
            if (!this.registerCommands.TryGetValue(context.Command, out handler))
            {
                return;
            }

            handler(context);
        }

        #endregion

        #region 受保护方法

        protected void RegisterCommand(string command, CommandHandlerDelegate handler)
        {
            if (this.registerCommands.ContainsKey(command))
            {
                return;
            }

            this.registerCommands[command] = handler;
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

        private void InitializeGlobalPanels() 
        {
            foreach (PanelItemDefinition definition in this.Definition.GlobalPanels)
            {
                PanelVM panelVM = new PanelVM(definition);
                panelVM.ID = definition.ID;
                panelVM.Name = definition.Name;
                panelVM.IconURI = definition.Icon;
                panelVM.ClassName = definition.ClassName;
                panelVM.VMClassName = definition.VMClassName;

                this.Shell.AddPanel(panelVM);
            }
        }

        #endregion
    }
}
