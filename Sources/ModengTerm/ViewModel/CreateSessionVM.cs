using log4net.Appender;
using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.Metadatas;
using ModengTerm.ViewModel.CreateSession;
using ModengTerm.ViewModel.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 新建Session窗口的ViewModel
    /// </summary>
    public class CreateSessionVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionVM");

        #endregion

        #region 实例变量

        private SessionTypeVM selectedSessionType;
        private PreferenceTreeVM optionTreeVM;

        /// <summary>
        /// 当前选择的SessionType -> SessionType要显示的首选项界面列表
        /// </summary>
        private Dictionary<SessionTypeEnum, List<PreferenceNodeVM>> sessionType2Preference;

        #endregion

        #region 属性

        /// <summary>
        /// 当前选择的会话类型
        /// </summary>
        public SessionTypeVM SelectedSessionType
        {
            get { return this.selectedSessionType; }
            set
            {
                if (this.selectedSessionType != value)
                {
                    if (this.selectedSessionType != null)
                    {
                        #region 隐藏之前选择的会话的首选项页面

                        List<PreferenceNodeVM> preferenceItems;
                        if (this.sessionType2Preference.TryGetValue(this.selectedSessionType.Type, out preferenceItems))
                        {
                            foreach (PreferenceNodeVM preferenceItem in preferenceItems)
                            {
                                preferenceItem.IsVisible = false;
                            }
                        }

                        #endregion
                    }

                    if (value != null)
                    {
                        #region 显示选择的会话的首选项页面

                        List<PreferenceNodeVM> preferenceItems;
                        if (this.sessionType2Preference.TryGetValue(value.Type, out preferenceItems))
                        {
                            foreach (PreferenceNodeVM preferenceItem in preferenceItems)
                            {
                                preferenceItem.IsVisible = true;
                            }
                        }

                        #endregion

                        // 选中第一个有界面的节点
                        PreferenceNodeVM node = preferenceItems.FirstOrDefault(v => !string.IsNullOrEmpty(v.ClassName));
                        if (node != null)
                        {
                            node.IsSelected = true;
                        }
                    }

                    this.selectedSessionType = value;
                    this.NotifyPropertyChanged("SelectedSessionType");
                }
            }
        }

        /// <summary>
        /// 会话类型列表
        /// </summary>
        public ObservableCollection<SessionTypeVM> SessionTypeList { get; private set; }

        /// <summary>
        /// 所有的会话分组列表
        /// </summary>
        public SessionTreeVM SessionGroups { get; private set; }

        /// <summary>
        /// 当前显示的配置树形列表ViewModel
        /// </summary>
        public PreferenceTreeVM PreferenceTreeVM
        {
            get { return optionTreeVM; }
            private set
            {
                if (optionTreeVM != value)
                {
                    optionTreeVM = value;
                    this.NotifyPropertyChanged("OptionTreeVM");
                }
            }
        }

        #region 主题相关

        public string ScrollbarThumbColor { get; set; }
        public string ScrollbarButtonColor { get; set; }
        public string ScrollbarTrackColor { get; set; }

        #endregion

        #endregion

        #region 构造方法

        public CreateSessionVM()
        {
            this.Name = "新建会话";
            this.sessionType2Preference = new Dictionary<SessionTypeEnum, List<PreferenceNodeVM>>();

            #region 加载参数树形列表

            // 加载参数树形列表
            this.PreferenceTreeVM = new PreferenceTreeVM();
            this.InitializePreferenceTree();
            this.PreferenceTreeVM.ExpandAll();

            #endregion

            #region 会话类型和会话分组

            this.SessionTypeList = new BindableCollection<SessionTypeVM>();
            foreach (SessionMetadata session in VTBaseConsts.SessionMetadatas)
            {
                this.SessionTypeList.Add(new SessionTypeVM(session));
            }
            // 选择第一个会话
            this.SelectedSessionType = this.SessionTypeList.FirstOrDefault();

            this.SessionGroups = VMUtils.CreateSessionTreeVM(true, true);
            this.SessionGroups.ExpandAll();

            #endregion
        }

        #endregion

        #region 实例方法

        private void InitializePreferenceTree()
        {
            foreach (AddonMetadata metadata in ClientContext.Context.Manifest.Addons)
            {
                if (metadata.Preferences.Count == 0)
                {
                    continue;
                }

                this.LoadChildPreference(metadata, null, metadata.Preferences);
            }
        }

        private void LoadChildPreference(AddonMetadata addonMetadata, PreferenceNodeVM parent, List<PreferenceMetadata> children)
        {
            foreach (PreferenceMetadata metadata in children)
            {
                PreferenceNodeVM pivm = new PreferenceNodeVM(this.PreferenceTreeVM.Context);
                pivm.ID = metadata.ID;
                pivm.Name = metadata.Name;
                pivm.ClassName = metadata.ClassName;
                pivm.AddonMetadata = addonMetadata;
                pivm.IsVisible = false;
                pivm.Level = parent == null ? 0 : parent.Level + 1;

                // 加载树形列表的时候就把配置项里的变量解析掉，这样就不用每次保存的时候再解析一遍了
                foreach (KeyValuePair<string, object> kv in metadata.DefaultOptions)
                {
                    if (kv.Value is string)
                    {
                        // 如果默认配置项是字符串类型，那么判断是否包含变量
                        pivm.DefaultOptions[kv.Key] = VTBaseUtils.ReplaceVariable(kv.Value.ToString());
                    }
                    else
                    {
                        pivm.DefaultOptions[kv.Key] = kv.Value;
                    }
                }

                // 保存不同的会话所需要显示的配置项界面列表
                foreach (SessionTypeEnum scope in metadata.Scopes)
                {
                    List<PreferenceNodeVM> pivms;
                    if (!this.sessionType2Preference.TryGetValue(scope, out pivms))
                    {
                        pivms = new List<PreferenceNodeVM>();
                        this.sessionType2Preference[scope] = pivms;
                    }

                    pivms.Add(pivm);
                }

                // 加载子级配置项
                this.LoadChildPreference(addonMetadata, pivm, metadata.Children);

                // 配置项添加到树里
                if (parent == null)
                {
                    this.PreferenceTreeVM.AddRootNode(pivm);
                }
                else
                {
                    parent.Add(pivm);
                }
            }
        }

        private bool SaveAllOptions(XTermSession session)
        {
            List<PreferenceNodeVM> preferenceNodes = this.sessionType2Preference[this.selectedSessionType.Type];

            foreach (PreferenceNodeVM pNode in preferenceNodes)
            {
                Dictionary<string, object> sessionOptions;
                if (!session.Options.TryGetValue(pNode.AddonMetadata.ID, out sessionOptions))
                {
                    sessionOptions = new Dictionary<string, object>();
                    session.Options[pNode.AddonMetadata.ID] = sessionOptions;
                }

                // 先把所有默认值写入到配置里
                foreach (KeyValuePair<string, object> kv in pNode.DefaultOptions)
                {
                    sessionOptions[kv.Key] = kv.Value;
                }

                IPreferencePanel preferencePanel = pNode.FrameworkElement as IPreferencePanel;
                if (preferencePanel == null)
                {
                    // 如果没有打开过配置项界面，或者配置项界面没有实现IPreferencePanel，那么保存默认配置项
                    continue;
                }

                // 然后把界面上的配置重写到配置里
                Dictionary<string, object> options = preferencePanel.GetOptions();
                if (options == null)
                {
                    return false;
                }

                foreach (KeyValuePair<string, object> kv in options)
                {
                    sessionOptions[kv.Key] = kv.Value;
                }
            }

            return true;
        }

        #endregion

        #region 事件处理器

        #endregion

        public XTermSession GetSession()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                MTMessageBox.Info("请输入会话名称");
                return null;
            }

            SessionTypeVM sessionType = this.SelectedSessionType;
            if (sessionType == null)
            {
                MTMessageBox.Info("请选择会话类型");
                return null;
            }

            string groupId = string.Empty;
            if (SessionGroups.Context.SelectedItem != null &&
                SessionGroups.Context.SelectedItem.Data != VTBaseConsts.RootGroup)
            {
                groupId = SessionGroups.Context.SelectedItem.ID.ToString();
            }

            XTermSession session = new XTermSession();
            session.ID = Guid.NewGuid().ToString();
            session.CreationTime = DateTime.Now;
            session.Name = this.Name;
            session.Type = (int)sessionType.Type;
            session.GroupId = groupId;

            if (!SaveAllOptions(session))
            {
                return null;
            }

            return session;
        }
    }
}
