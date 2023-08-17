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
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.Document.Rendering;
using XTerminal.ServiceAgents;
using XTerminal.Session;
using XTerminal.UserControls;
using XTerminal.ViewModels;

namespace XTerminal
{
    public class XTermManifest : AppManifest
    {
        /// <summary>
        /// 支持的会话列表
        /// </summary>
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        /// <summary>
        /// 定义XTerminal支持的文本颜色
        /// </summary>
        [JsonProperty("foreground")]
        public List<ColorDefinition> ForegroundList { get; private set; }

        /// <summary>
        /// XTerminal支持的文字大小列表
        /// </summary>
        [JsonProperty("fontSize")]
        public List<FontSizeDefinition> FontSizeList { get; private set; }

        /// <summary>
        /// 支持的文字样式列表
        /// </summary>
        [JsonProperty("fontFamily")]
        public List<FontFamilyDefinition> FontFamilyList { get; private set; }

        /// <summary>
        /// 主题列表
        /// </summary>
        [JsonProperty("themes")]
        public List<ThemeDefinition> ThemeList { get; private set; }

        [JsonProperty("ftpOptions")]
        public List<OptionDefinition> FTPOptionList { get; private set; }

        [JsonProperty("terminalOptions")]
        public List<OptionDefinition> TerminalOptionList { get; private set; }

        public XTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
            this.ForegroundList = new List<ColorDefinition>();
            this.FontSizeList = new List<FontSizeDefinition>();
            this.FontFamilyList = new List<FontFamilyDefinition>();
            this.ThemeList = new List<ThemeDefinition>();
            this.FTPOptionList = new List<OptionDefinition>();
            this.TerminalOptionList = new List<OptionDefinition>();
        }
    }

    public class XTermApp : ModularApp<XTermApp, XTermManifest>, INotifyPropertyChanged
    {
        #region 实例变量

        private OpenedSessionVM selectedOpenedSession;

        #endregion

        #region 属性

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

        /// <summary>
        /// FTP选项参数树形列表
        /// </summary>
        public OptionTreeVM SFTPOptionsTreeVM { get; private set; }

        /// <summary>
        /// 终端选项参数树形列表
        /// </summary>
        public OptionTreeVM TerminalOptionsTreeVM { get; private set; }

        #endregion

        #region ModularApp

        protected override int OnInitialize()
        {
            this.SFTPOptionsTreeVM = new OptionTreeVM();
            this.TerminalOptionsTreeVM = new OptionTreeVM();
            this.OpenedSessionList = new BindableCollection<OpenedSessionVM>();
            this.ServiceAgent = this.Factory.LookupModule<ServiceAgent>();

            #region 加载选项树形列表

            this.LoadOptionsTree(this.SFTPOptionsTreeVM, this.Manifest.FTPOptionList);
            this.LoadOptionsTree(this.TerminalOptionsTreeVM, this.Manifest.TerminalOptionList);

            #endregion

            // 将打开页面新加到OpenedSessionTab页面上
            this.OpenedSessionList.Add(new OpenSessionVM());

            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        private void LoadChildrenOptions(OptionTreeNodeVM parentNode, List<OptionDefinition> children)
        {
            foreach (OptionDefinition option in children)
            {
                OptionTreeNodeVM vm = new OptionTreeNodeVM(parentNode.Context, option)
                {
                    ID = option.ID,
                    Name = option.Name,
                    EntryClass = option.EntryClass,
                    IsExpanded = true
                };

                parentNode.AddChildNode(vm);

                this.LoadChildrenOptions(vm, option.Children);
            }
        }

        private void LoadOptionsTree(OptionTreeVM treeVM, List<OptionDefinition> options)
        {
            foreach (OptionDefinition option in options)
            {
                OptionTreeNodeVM vm = new OptionTreeNodeVM(treeVM.Context, option)
                {
                    ID = option.ID,
                    Name = option.Name,
                    EntryClass = option.EntryClass,
                    IsExpanded = true
                };

                treeVM.AddRootNode(vm);

                this.LoadChildrenOptions(vm, option.Children);
            }

            // 默认选中第一个节点
            TreeNodeViewModel firstNode = treeVM.Roots.FirstOrDefault();
            if (firstNode != null)
            {
                firstNode.IsSelected = true;
            }
        }

        #endregion

        #region 公开接口

        public OpenedSessionVM OpenSession(XTermSession session)
        {
            SessionContent content = SessionContentFactory.Create(session);

            // 新建会话ViewModel
            OpenedSessionVM sessionVM = this.CreateOpenedSessionVM(session);
            sessionVM.ID = Guid.NewGuid().ToString();
            sessionVM.Name = session.Name;
            sessionVM.Description = session.Description;
            sessionVM.Content = content;
            sessionVM.StatusChanged += this.SessionVM_StatusChanged;
            sessionVM.Open(session);

            content.DataContext = sessionVM;

            // 添加到界面上，因为最后一个元素是打开Session的TabItem，所以要添加到倒数第二个元素的位置
            this.OpenedSessionList.Insert(this.OpenedSessionList.Count - 1, sessionVM);
            this.SelectedOpenedSession = sessionVM;

            return sessionVM;
        }

        public void CloseSession(OpenedSessionVM session)
        {
            session.StatusChanged -= this.SessionVM_StatusChanged;
            session.Close();

            this.OpenedSessionList.Remove(session);
            OpenedSessionVM firstOpenedSession = this.GetOpenedSessions().FirstOrDefault();
            if (firstOpenedSession == null)
            {
                this.OpenSession(XTermDefaultValues.DefaultSession);
            }
            else
            {
                this.SelectedOpenedSession = firstOpenedSession;
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

        private OpenedSessionVM CreateOpenedSessionVM(XTermSession session)
        {
            switch ((SessionTypeEnum)session.SessionType)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Win32CommandLine:
                    {
                        return new TerminalSessionVM();
                    }

                case SessionTypeEnum.SFTP:
                    {
                        return new SFTPSessionVM();
                    }

                default:
                    throw new NotImplementedException();
            }
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
