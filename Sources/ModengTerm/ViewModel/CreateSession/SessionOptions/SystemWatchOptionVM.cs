using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModel.CreateSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.CreateSession.SessionOptions
{
    public class SystemWatchOptionVM : OptionContentVM
    {
        private bool enableAdb;
        private string adbPath;
        private string adbPrompt;
        private int adbLoginTimeout;

        /// <summary>
        /// 更新频率列表
        /// </summary>
        public BindableCollection<WatchFrequencyEnum> WatchFrequencies { get; private set; }

        public bool EnableADB
        {
            get { return enableAdb; }
            set
            {
                if (enableAdb != value)
                {
                    enableAdb = value;
                    this.NotifyPropertyChanged("AdbMonitor");
                }
            }
        }

        public string AdbPrompt
        {
            get { return adbPrompt; }
            set
            {
                if (adbPrompt != value)
                {
                    adbPrompt = value;
                    this.NotifyPropertyChanged("AdbPrompt");
                }
            }
        }

        public string AdbPath
        {
            get { return adbPath; }
            set
            {
                if (adbPath != value)
                {
                    adbPath = value;
                    this.NotifyPropertyChanged("AdbPath");
                }
            }
        }

        public int AdbLoginTimeout
        {
            get { return adbLoginTimeout; }
            set
            {
                if (adbLoginTimeout != value)
                {
                    adbLoginTimeout = value;
                    this.NotifyPropertyChanged("AdbLoginTimeout");
                }
            }
        }

        public override void LoadOptions(XTermSession session)
        {
            WatchFrequencies.SelectedItem = session.GetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, OptionDefaultValues.WATCH_FREQUENCY);
            EnableADB = session.GetOption<bool>(OptionKeyEnum.WATCH_ADB_ENABLED, OptionDefaultValues.WATCH_ADB_ENABLED);
            AdbPath = session.GetOption<string>(OptionKeyEnum.WATCH_ADB_PATH, OptionDefaultValues.WATCH_ADB_PATH);
            AdbLoginTimeout = session.GetOption<int>(OptionKeyEnum.WATCH_ADB_LOGIN_TIMEOUT, OptionDefaultValues.WATCH_ADB_LOGIN_TIMEOUT);
            AdbPrompt = session.GetOption<string>(OptionKeyEnum.WATCH_ADB_PROMPT, OptionDefaultValues.WATCH_ADB_PROMPT);
        }

        public override bool SaveOptions(XTermSession session)
        {
            session.SetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, WatchFrequencies.SelectedItem);
            session.SetOption<bool>(OptionKeyEnum.WATCH_ADB_ENABLED, EnableADB);
            session.SetOption<string>(OptionKeyEnum.WATCH_ADB_PATH, AdbPath);
            session.SetOption<int>(OptionKeyEnum.WATCH_ADB_LOGIN_TIMEOUT, AdbLoginTimeout);
            session.SetOption<string>(OptionKeyEnum.WATCH_ADB_PROMPT, AdbPrompt);

            return true;
        }

        public override void OnInitialize()
        {
            WatchFrequencies = new BindableCollection<WatchFrequencyEnum>();
            WatchFrequencies.AddRange(VTBaseUtils.GetEnumValues<WatchFrequencyEnum>());
            WatchFrequencies.SelectedItem = OptionDefaultValues.WATCH_FREQUENCY;
            AdbPath = OptionDefaultValues.WATCH_ADB_PATH;
            AdbLoginTimeout = OptionDefaultValues.WATCH_ADB_LOGIN_TIMEOUT;
            AdbPrompt = OptionDefaultValues.WATCH_ADB_PROMPT;
            EnableADB = OptionDefaultValues.WATCH_ADB_ENABLED;
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }
    }
}
