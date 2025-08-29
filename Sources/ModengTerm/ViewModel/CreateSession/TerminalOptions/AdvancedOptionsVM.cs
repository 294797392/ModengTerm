using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.ViewModel.CreateSession;
using System.Windows.Media;
using WPFToolkit.MVVM;
using Color = System.Windows.Media.Color;

namespace ModengTerm.ViewModel.CreateSession.TerminalOptions
{
    public class AdvancedOptionsVM : OptionContentVM
    {
        #region 实例变量

        private bool clickToCursor;
        private bool autoWrapMode;
        private bool autoCompletionList;
        private Color writeColor;
        private Color readColor;
        private bool renderWrite;

        #endregion

        #region 属性

        public BindableCollection<RenderModeEnum> RenderModes { get; private set; }

        /// <summary>
        /// 点击即可将光标移动到该位置
        /// </summary>
        public bool ClickToCursor
        {
            get { return clickToCursor; }
            set
            {
                if (clickToCursor != value)
                {
                    clickToCursor = value;
                    NotifyPropertyChanged("ClickToCursor");
                }
            }
        }

        /// <summary>
        /// 默认是否启用自动换行
        /// </summary>
        public bool AutoWrapMode
        {
            get { return autoWrapMode; }
            set
            {
                if (autoWrapMode != value)
                {
                    autoWrapMode = value;
                    NotifyPropertyChanged("AutoWrapMode");
                }
            }
        }

        /// <summary>
        /// 是否启用自动完成列表
        /// </summary>
        public bool AutoCompletionList
        {
            get { return autoCompletionList; }
            set
            {
                if (autoCompletionList != value)
                {
                    autoCompletionList = value;
                    NotifyPropertyChanged("AutoCompletionList");
                }
            }
        }

        public Color SendColor
        {
            get { return writeColor; }
            set
            {
                if (writeColor != value)
                {
                    writeColor = value;
                    this.NotifyPropertyChanged("SendColor");
                }
            }
        }

        public Color RecvColor
        {
            get { return readColor; }
            set
            {
                if (readColor != value)
                {
                    readColor = value;
                    this.NotifyPropertyChanged("RecvColor");
                }
            }
        }

        /// <summary>
        /// 是否渲染输入的数据
        /// </summary>
        public bool RenderWrite
        {
            get { return renderWrite; }
            set
            {
                if (renderWrite != value)
                {
                    renderWrite = value;
                    this.NotifyPropertyChanged("RenderWrite");
                }
            }
        }

        #endregion

        #region OptionContentVM

        public override void OnInitialize()
        {
            RenderModes = new BindableCollection<RenderModeEnum>();
            RenderModes.AddRange(VTBaseUtils.GetEnumValues<RenderModeEnum>());
            RenderModes.SelectedItem = RenderModeEnum.Default;
            SendColor = VTBaseUtils.RgbKey2Color(OptionDefaultValues.TERM_ADVANCE_SEND_COLOR);
            RecvColor = VTBaseUtils.RgbKey2Color(OptionDefaultValues.TERM_ADVANCE_RECV_COLOR);
            RenderWrite = OptionDefaultValues.TERM_ADVANCE_RENDER_WRITE;
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

        public override void LoadOptions(XTermSession session)
        {
        }

        public override bool SaveOptions(XTermSession session)
        {
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_RENDER_MODE, RenderModes.SelectedItem);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_CLICK_TO_CURSOR, clickToCursor);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, autoCompletionList);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_AUTO_WRAP_MODE, autoWrapMode);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_SEND_COLOR, VTBaseUtils.Color2RgbKey(writeColor));
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_RECV_COLOR, VTBaseUtils.Color2RgbKey(readColor));
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_RENDER_WRITE, renderWrite);

            return true;
        }

        #endregion
    }
}
