using DotNEToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;
using WPFToolkit.Utils;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Document.Rendering;
using XTerminal.ServiceAgents;
using XTerminal.Session;
using XTerminal.Session.Definitions;
using XTerminal.UserControls;
using XTerminal.ViewModels;

namespace XTerminal
{
    public class XTermManifest : AppManifest
    {
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        public XTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
        }
    }

    public class XTermApp : ModularApp<XTermApp, XTermManifest>, INotifyPropertyChanged
    {
        private OpenedSessionVM selectedOpenedSession;

        #region 属性

        /// <summary>
        /// 此App不异步初始化
        /// </summary>
        protected override bool AsyncInitializing => false;

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; private set; }

        /// <summary>
        /// 存储当前打开的所有会话列表
        /// </summary>
        public BindableCollection<OpenedSessionVM> OpenedSessionList { get; private set; }

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
            this.ServiceAgent = this.Factory.LookupModule<ServiceAgent>();
            this.OpenedSessionList = new BindableCollection<OpenedSessionVM>();

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 公开接口

        public int OpenSession(XTermSession session, IDrawingCanvasPanel canvasPanel)
        {
            // 新建会话ViewModel
            OpenedSessionVM sessionVM = new OpenedSessionVM(session);
            sessionVM.CanvasPanel = canvasPanel;
            sessionVM.StatusChanged += this.SessionVM_StatusChanged;

            // 打开会话
            sessionVM.Open();

            // 添加到界面上
            this.OpenedSessionList.Add(sessionVM);
            this.SelectedOpenedSession = sessionVM;

            return ResponseCode.SUCCESS;
        }

        public void CloseSession(XTermSession session)
        {
            
        }

        #endregion

        #region 事件处理器

        private void SessionVM_StatusChanged(OpenedSessionVM sessionVM, SessionStatusEnum status)
        {
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
