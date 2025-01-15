using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.SessionOptions
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
            get { return this.enableAdb; }
            set
            {
                if (this.enableAdb != value)
                {
                    this.enableAdb = value;
                    this.NotifyPropertyChanged("AdbMonitor");
                }
            }
        }

        public string AdbPrompt
        {
            get { return this.adbPrompt; }
            set
            {
                if (this.adbPrompt != value)
                {
                    this.adbPrompt = value;
                    this.NotifyPropertyChanged("AdbPrompt");
                }
            }
        }

        public string AdbPath
        {
            get { return this.adbPath; }
            set
            {
                if (this.adbPath != value)
                {
                    this.adbPath = value;
                    this.NotifyPropertyChanged("AdbPath");
                }
            }
        }

        public int AdbLoginTimeout
        {
            get { return this.adbLoginTimeout; }
            set
            {
                if (this.adbLoginTimeout != value)
                {
                    this.adbLoginTimeout = value;
                    this.NotifyPropertyChanged("AdbLoginTimeout");
                }
            }
        }

        public override void LoadOptions(XTermSession session)
        {
            this.WatchFrequencies.SelectedItem = session.GetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, OptionDefaultValues.WATCH_FREQUENCY);
            this.EnableADB = session.GetOption<bool>(OptionKeyEnum.WATCH_ADB_ENABLED, OptionDefaultValues.WATCH_ADB_ENABLED);
            this.AdbPath = session.GetOption<string>(OptionKeyEnum.WATCH_ADB_PATH, OptionDefaultValues.WATCH_ADB_PATH);
            this.AdbLoginTimeout = session.GetOption<int>(OptionKeyEnum.WATCH_ADB_LOGIN_TIMEOUT, OptionDefaultValues.WATCH_ADB_LOGIN_TIMEOUT);
            this.AdbPrompt = session.GetOption<string>(OptionKeyEnum.WATCH_ADB_PROMPT, OptionDefaultValues.WATCH_ADB_PROMPT);
        }

        public override bool SaveOptions(XTermSession session)
        {
            session.SetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, this.WatchFrequencies.SelectedItem);
            session.SetOption<bool>(OptionKeyEnum.WATCH_ADB_ENABLED, this.EnableADB);
            session.SetOption<string>(OptionKeyEnum.WATCH_ADB_PATH, this.AdbPath);
            session.SetOption<int>(OptionKeyEnum.WATCH_ADB_LOGIN_TIMEOUT, this.AdbLoginTimeout);
            session.SetOption<string>(OptionKeyEnum.WATCH_ADB_PROMPT, this.AdbPrompt);

            return true;
        }

        public override void OnInitialize()
        {
            this.WatchFrequencies = new BindableCollection<WatchFrequencyEnum>();
            this.WatchFrequencies.AddRange(VTBaseUtils.GetEnumValues<WatchFrequencyEnum>());
            this.WatchFrequencies.SelectedItem = OptionDefaultValues.WATCH_FREQUENCY;
            this.AdbPath = OptionDefaultValues.WATCH_ADB_PATH;
            this.AdbLoginTimeout = OptionDefaultValues.WATCH_ADB_LOGIN_TIMEOUT;
            this.AdbPrompt = OptionDefaultValues.WATCH_ADB_PROMPT;
            this.EnableADB = OptionDefaultValues.WATCH_ADB_ENABLED;
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
