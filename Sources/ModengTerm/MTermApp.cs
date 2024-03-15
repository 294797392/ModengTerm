using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.ServiceAgents;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFToolkit.MVVM;
using XTerminal.UserControls.OptionsUserControl;

namespace ModengTerm
{
    public class MTermApp : ModularApp<MTermApp, MTermManifest>, INotifyPropertyChanged
    {
        /// <summary>
        /// C#的代码混淆工具不能进行反射，因为会修改类名，所以把要动态创建的控件实例类型写死
        /// </summary>
        public static readonly List<OptionDefinition> TerminalOptionList = new List<OptionDefinition>()
        {
            new OptionDefinition("连接设置", typeof(SSHOptionsUserControl))
            {
                Children = new List<OptionDefinition>()
                {
                    new OptionDefinition("SSH", typeof(SSHOptionsUserControl)),
                    new OptionDefinition("串口", typeof(SerialPortOptionsUserControl))
                }
            },

            new OptionDefinition("终端", typeof(TerminalOptionsUserControl)),

            new OptionDefinition("外观主题", typeof(ThemeOptionsUserControl))
        };

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
        /// 存储当前打开的所有会话列表
        /// </summary>
        public BindableCollection<OpenedSessionVM> OpenedSessionList { get; private set; }

        public BindableCollection<ShellSessionVM> OpenedTerminals { get; private set; }

        /// <summary>
        /// 界面上当前选中的会话
        /// </summary>
        public OpenedSessionVM SelectedOpenedSession
        {
            get { return this.selectedOpenedSession; }
            set
            {
                if (this.selectedOpenedSession != value)
                {
                    this.selectedOpenedSession = value;
                    this.NotifyPropertyChanged("SelectedOpenedSession");
                }
            }
        }

        #endregion

        #region ModularApp

        protected override int OnInitialize()
        {
            this.OpenedSessionList = new BindableCollection<OpenedSessionVM>();
            this.OpenedTerminals = new BindableCollection<ShellSessionVM>();
            this.ServiceAgent = new LocalServiceAgent();
            this.ServiceAgent.Initialize();

            this.LoggerManager = new LoggerManager();
            this.LoggerManager.Initialize();

            // 将打开页面新加到OpenedSessionTab页面上
            this.OpenedSessionList.Add(new OpenSessionVM(null));

            #region 启动后台工作线程

            // 启动光标闪烁线程, 所有的终端共用同一个光标闪烁线程

            this.drawFrameTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.drawFrameTimer.Interval = TimeSpan.FromMilliseconds(MTermConsts.DrawFrameInterval);
            this.drawFrameTimer.Tick += DrawFrameTimer_Tick;
            this.drawFrameTimer.IsEnabled = false;

            #endregion

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public void OpenSession(XTermSession session, ContentControl container)
        {
            // 先初始化UI，等UI显示出来在打开Session
            // 因为初始化终端需要知道当前的界面大小，从而计算行大小和列大小

            ISessionContent content = SessionContentFactory.Create(session);
            OpenedSessionVM viewModel = OpenedSessionVMFactory.Create(session);

            // 给ViewModel赋值
            viewModel.ID = Guid.NewGuid().ToString();
            viewModel.Name = session.Name;
            viewModel.Description = session.Description;
            viewModel.Content = content as DependencyObject;
            viewModel.ServiceAgent = MTermApp.Context.ServiceAgent;

            // 给SessionContent赋值
            content.Session = session;

            UserControl userControl = content as UserControl;
            userControl.DataContext = viewModel;
            userControl.Loaded += SessionContent_Loaded;  // Content完全显示出来会触发这个事件

            container.Content = content;
        }

        public void CloseSession(OpenedSessionVM session)
        {
            ISessionContent content = session.Content as ISessionContent;
            if (MTermUtils.IsTerminal((SessionTypeEnum)content.Session.Type))
            {
                UserControl userControl = content as UserControl;
                this.OpenedTerminals.Remove(userControl.DataContext as ShellSessionVM);
            }
            content.Close();

            this.OpenedSessionList.Remove(session);

            if (session is VideoTerminal)
            {
                if (this.OpenedSessionList.OfType<VideoTerminal>().Count() == 0)
                {
                    this.drawFrameTimer.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 获取所有已经打开了的会话列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OpenedSessionVM> GetOpenedSessions()
        {
            return this.OpenedSessionList.OfType<OpenedSessionVM>();
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
        private void DrawFrameTimer_Tick(object sender, EventArgs e)
        {
            IEnumerable<ShellSessionVM> vtlist = this.OpenedTerminals;

            foreach (ShellSessionVM vt in vtlist)
            {
                // 如果当前界面上没有显示终端，那么不处理帧
                FrameworkElement frameworkElement = vt.Content as FrameworkElement;
                if (!frameworkElement.IsLoaded)
                {
                    continue;
                }

                VTDocument activeDocument = vt.VideoTerminal.ActiveDocument;

                int elapsed = this.drawFrameTimer.Interval.Milliseconds;

                //this.ProcessFrame(elapsed, activeDocument.Cursor);

                //switch (vt.Background.PaperType)
                //{
                //    case WallpaperTypeEnum.Live:
                //        {
                //            this.ProcessFrame(elapsed, vt.Background);
                //            break;
                //        }

                //    case WallpaperTypeEnum.Image:
                //    case WallpaperTypeEnum.Color:
                //        {
                //            if (vt.Background.Effect != EffectTypeEnum.None)
                //            {
                //                this.ProcessFrame(elapsed, vt.Background);
                //            }
                //            break;
                //        }

                //    default:
                //        {
                //            break;
                //        }
                //}
            }
        }

        private void SessionContent_Loaded(object sender, RoutedEventArgs e)
        {
            // 此时所有的界面都加载完了，可以真正打开Session了
            ISessionContent content = sender as ISessionContent;

            // 要反注册事件，不然每次显示界面就会多打开一个VideoTerminal
            UserControl userControl = content as UserControl;
            userControl.Loaded -= this.SessionContent_Loaded;

            int code = content.Open();
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开会话失败, {0}", code);
            }

            // 如果是终端类型的，加入到终端列表里
            // 方便“发送到所有”功能
            if (MTermUtils.IsTerminal((SessionTypeEnum)content.Session.Type))
            {
                this.OpenedTerminals.Add(userControl.DataContext as ShellSessionVM);
            }

            OpenedSessionVM sessionVM = userControl.DataContext as OpenedSessionVM;

            // 添加到界面上，因为最后一个元素是打开Session的TabItem，所以要添加到倒数第二个元素的位置
            this.OpenedSessionList.Insert(this.OpenedSessionList.Count - 1, sessionVM);
            this.SelectedOpenedSession = sessionVM;

            // 启动光标渲染线程
            if (sessionVM is VideoTerminal)
            {
                if (!this.drawFrameTimer.IsEnabled)
                {
                    this.drawFrameTimer.IsEnabled = true;
                }
            }
        }

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
