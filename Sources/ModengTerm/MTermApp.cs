using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Loggering;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Session;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using WPFToolkit.MVVM;

namespace ModengTerm
{
    /// <summary>
    /// 存储整个应用程序都需要使用的通用的数据和方法
    /// </summary>
    public class MTermApp : ModularApp<MTermApp, MTermManifest>, INotifyPropertyChanged
    {
        #region 实例变量

        private OpenedSessionVM selectedOpenedSession;

        private DispatcherTimer drawFrameTimer;

        #endregion

        #region 属性

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; private set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public LoggerManager LoggerManager { get; private set; }

        /// <summary>
        /// 主窗口ViewModel
        /// </summary>
        public MainWindowVM MainWindowVM { get; private set; }

        /// <summary>
        /// 资源管理器树形列表
        /// </summary>
        public SessionTreeVM ResourceManagerTreeVM { get; private set; }

        #endregion

        #region ModularApp

        protected override int OnInitialize()
        {
            this.ServiceAgent = new LocalServiceAgent();
            this.ServiceAgent.Initialize();

            this.LoggerManager = new LoggerManager();
            this.LoggerManager.Initialize();

            VTermApp.Context.ServiceAgent = this.ServiceAgent;
            VTermApp.Context.Initialize("vtermapp.json");

            this.ResourceManagerTreeVM = this.CreateSessionTreeVM(false, true);
            this.ResourceManagerTreeVM.Roots[0].Name = "会话列表";
            this.ResourceManagerTreeVM.ExpandAll();

            // 在最后初始化ViewModel，因为ViewModel里可能会用到ServiceAgent
            this.MainWindowVM = new MainWindowVM();

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        private void LoadSessionGroupNode(SessionGroupVM parentGroup, List<SessionGroup> groups)
        {
            // 先找到该分组的所有子节点
            IEnumerable<SessionGroup> children = groups.Where(v => v.ParentId == parentGroup.ID.ToString());

            foreach (SessionGroup child in children)
            {
                SessionGroupVM groupVM = new SessionGroupVM(parentGroup.Context, parentGroup.Level + 1, child);
                parentGroup.Add(groupVM);
                this.LoadSessionGroupNode(groupVM, groups);
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 创建一个会话树形列表
        /// </summary>
        /// <param name="onlyGroup">树形列表里是否只有分组</param>
        /// <param name="includeRootNode">是否包含无分组节点</param>
        public SessionTreeVM CreateSessionTreeVM(bool onlyGroup = false, bool includeRootNode = false)
        {
            SessionTreeVM sessionTreeVM = new SessionTreeVM();
            TreeViewModelContext context = sessionTreeVM.Context;

            SessionGroupVM rootNode = null;

            if (includeRootNode) 
            {
                rootNode = new SessionGroupVM(context, 0, VTBaseConsts.RootGroup);
                sessionTreeVM.AddRootNode(rootNode);
            }

            List<SessionGroup> groups = this.ServiceAgent.GetSessionGroups();
            IEnumerable<SessionGroup> rootGroups = groups.Where(v => v.ParentId == string.Empty);
            foreach (SessionGroup group in rootGroups)
            {
                SessionGroupVM groupVM = new SessionGroupVM(context, 0, group);
                if (rootNode != null)
                {
                    groupVM.Level = 1;
                    rootNode.Add(groupVM);
                }
                else
                {
                    sessionTreeVM.AddRootNode(groupVM);
                }

                this.LoadSessionGroupNode(groupVM, groups);
            }

            if (!onlyGroup)
            {
                List<XTermSession> sessions = this.ServiceAgent.GetSessions();
                foreach (XTermSession session in sessions)
                {
                    XTermSessionVM sessionVM = new XTermSessionVM(context, 0, session);

                    if (string.IsNullOrEmpty(session.GroupId))
                    {
                        // 如果Session不属于任何分组，那么直接加到根节点
                        if (rootNode != null)
                        {
                            sessionVM.Level = rootNode.Level + 1;
                            rootNode.Add(sessionVM);
                        }
                        else
                        {
                            sessionTreeVM.AddRootNode(sessionVM);
                        }
                    }
                    else
                    {
                        TreeNodeViewModel parentVM;
                        if (!context.TryGetNode(session.GroupId, out parentVM))
                        {
                            logger.FatalFormat("没有找到Session对应的Gorup, {0},{1}", session.ID, session.GroupId);
                            continue;
                        }

                        sessionVM.Level = parentVM.Level + 1;
                        parentVM.Add(sessionVM);
                    }
                }
            }

            return sessionTreeVM;
        }

        #endregion

        #region 实例方法

        //private void ProcessFrame(int elapsed, IFramedElement element)
        //{
        //    element.Elapsed -= elapsed;

        //    if (element.Elapsed <= 0)
        //    {
        //        // 渲染
        //        try
        //        {
        //            element.RequestInvalidate();
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("RequestInvalidate运行异常", ex);
        //        }

        //        element.Elapsed = element.Delay;
        //    }
        //}

        #endregion

        #region 事件处理器

        /// <summary>
        /// 光标闪烁线程
        /// 所有的光标都在这一个线程运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void DrawFrameTimer_Tick(object sender, EventArgs e)
        //{
        //    IEnumerable<ShellSessionVM> vtlist = this.OpenedTerminals;

        //    foreach (ShellSessionVM vt in vtlist)
        //    {
        //        // 如果当前界面上没有显示终端，那么不处理帧
        //        FrameworkElement frameworkElement = vt.Content as FrameworkElement;
        //        if (!frameworkElement.IsLoaded)
        //        {
        //            continue;
        //        }

        //        VTDocument activeDocument = vt.VideoTerminal.ActiveDocument;

        //        int elapsed = this.drawFrameTimer.Interval.Milliseconds;

        //        //this.ProcessFrame(elapsed, activeDocument.Cursor);

        //        //switch (vt.Background.PaperType)
        //        //{
        //        //    case WallpaperTypeEnum.Live:
        //        //        {
        //        //            this.ProcessFrame(elapsed, vt.Background);
        //        //            break;
        //        //        }

        //        //    case WallpaperTypeEnum.Image:
        //        //    case WallpaperTypeEnum.Color:
        //        //        {
        //        //            if (vt.Background.Effect != EffectTypeEnum.None)
        //        //            {
        //        //                this.ProcessFrame(elapsed, vt.Background);
        //        //            }
        //        //            break;
        //        //        }

        //        //    default:
        //        //        {
        //        //            break;
        //        //        }
        //        //}
        //    }
        //}

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
