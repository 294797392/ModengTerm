using ModengTerm.Base.Definitions;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public delegate void CommandHandlerDelegate(CommandEventArgs e);

    /// <summary>
    /// 插件生命周期管理
    /// 插件事件管理
    /// </summary>
    public abstract class AddonModule
    {
        #region 实例变量

        private Dictionary<string, CommandHandlerDelegate> registerCommands;

        #endregion

        #region 属性

        public string ID { get { return this.Definition.ID; } }

        public AddonDefinition Definition { get; set; }

        #endregion

        #region Internal

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void Initialize()
        {
            this.registerCommands = new Dictionary<string, CommandHandlerDelegate>();

            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

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

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        #endregion
    }
}
