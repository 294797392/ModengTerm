using ModengTerm.Base.Definitions;
using ModengTerm.Base.ServiceAgents;

namespace ModengTerm.Addons
{
    public delegate void CommandHandlerDelegate();

    public abstract class AddonBase
    {
        #region 实例变量

        private List<AddonEventTypes> registerEvents;
        private Dictionary<string, CommandHandlerDelegate> registerCommands;

        #endregion

        #region 属性

        public string ID { get { return this.Definition.ID; } }

        public AddonDefinition Definition { get; set; }

        #endregion

        #region Internal

        public void Initialize()
        {
            this.registerEvents = new List<AddonEventTypes>();
            this.registerCommands = new Dictionary<string, CommandHandlerDelegate>();

            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

            // Release
            this.registerEvents.Clear();
            this.registerCommands.Clear();
        }

        public void RaiseEvent(AddonEventTypes evt, params object[] evp)
        {
            if (!this.registerEvents.Contains(evt))
            {
                // 没注册事件
                return;
            }

            this.OnEvent(evt, evp);
        }

        public void RaiseCommand(string command)
        {
            CommandHandlerDelegate handler;
            if (!this.registerCommands.TryGetValue(command, out handler)) 
            {
                return;
            }

            handler();
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

        protected void RegisterCommand(string command, CommandHandlerDelegate handler) 
        {
            if (this.registerCommands.ContainsKey(command))
            {
                return;
            }

            this.registerCommands[command] = handler;
        }

        // api
        protected void ShowWindow(string windowUri)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        /// <summary>
        /// 当插件有事件触发的时候调用
        /// </summary>
        /// <param name="ev"></param>
        protected abstract void OnEvent(AddonEventTypes evt, params object[] evp);

        #endregion
    }
}
