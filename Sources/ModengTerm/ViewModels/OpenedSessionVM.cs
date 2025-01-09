using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 表示一个被打开的会话
    /// </summary>
    public abstract class OpenedSessionVM : SessionItemVM
    {
        #region 公开事件

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        /// <summary>
        /// 保存当前显示的所有菜单列表
        /// </summary>
        protected List<ContextMenuDefinition> contextMenus;

        #endregion

        #region 属性

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public DependencyObject Content
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// 对应的会话信息
        /// </summary>
        public XTermSession Session { get; private set; }

        /// <summary>
        /// 与会话的连接状态
        /// </summary>
        public SessionStatusEnum Status
        {
            get { return this.status; }
            protected set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        /// <summary>
        /// 该会话的右键菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> ContextMenus { get; private set; }

        /// <summary>
        /// 该会话的标题菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
        }

        #endregion

        #region 公开接口

        public int Open()
        {
            this.ContextMenus = new BindableCollection<ContextMenuVM>();
            this.TitleMenus = new BindableCollection<ContextMenuVM>();

            SessionDefinition sessionDefinition = MTermApp.Context.Manifest.SessionList.FirstOrDefault(v => v.Type == this.Session.Type);
            List<ContextMenuDefinition> menuDefinitions = this.GetContextMenuDefinition();
            List<MenuItemRelation> titleMenuItems = menuDefinitions.Select(v => new MenuItemRelation(v.TitleParentID, v)).ToList();
            this.TitleMenus.AddRange(VTClientUtils.CreateContextMenuVM(titleMenuItems));
            List<MenuItemRelation> contextMenuItems = menuDefinitions.Select(v => new MenuItemRelation(v.ContextParentID, v)).ToList();
            this.ContextMenus.AddRange(VTClientUtils.CreateContextMenuVM(contextMenuItems));

            this.contextMenus = menuDefinitions;

            return this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();
        }

        /// <summary>
        /// 当显示到界面上之后触发
        /// </summary>
        public void Load() { }

        /// <summary>
        /// 当从界面上移除之后触发
        /// </summary>
        public void Unload() { }

        #endregion

        #region 抽象方法

        protected abstract int OnOpen();
        protected abstract void OnClose();

        #endregion

        #region 实例方法

        private List<ContextMenuDefinition> GetContextMenuDefinition()
        {
            List<ContextMenuDefinition> menuDefinitions = new List<ContextMenuDefinition>();

            switch ((SessionTypeEnum)this.Session.Type)
            {
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.Localhost:
                case SessionTypeEnum.AdbShell:
                case SessionTypeEnum.SSH:
                    {
                        menuDefinitions.AddRange(MTermApp.Context.Manifest.TerminalMenus);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            List<ContextMenuDefinition> results = new List<ContextMenuDefinition>();

            foreach (ContextMenuDefinition menuDefinition in menuDefinitions)
            {
                if (menuDefinition.SessionTypes.Count == 0)
                {
                    results.Add(menuDefinition);
                }
                else
                {
                    if (menuDefinition.SessionTypes.Contains(this.Session.Type))
                    {
                        results.Add(menuDefinition);
                    }
                }
            }

            return results;
        }

        #endregion
    }
}
