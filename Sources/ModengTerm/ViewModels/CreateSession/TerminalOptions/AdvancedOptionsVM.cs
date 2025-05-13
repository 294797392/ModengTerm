using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.UserControls.TerminalUserControls;
using System.Windows.Media;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.TerminalOptions
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
            get { return this.writeColor; }
            set
            {
                if (this.writeColor != value)
                {
                    this.writeColor = value;
                    this.NotifyPropertyChanged("SendColor");
                }
            }
        }

        public Color RecvColor
        {
            get { return this.readColor; }
            set
            {
                if (this.readColor != value)
                {
                    this.readColor = value;
                    this.NotifyPropertyChanged("RecvColor");
                }
            }
        }

        /// <summary>
        /// 是否渲染输入的数据
        /// </summary>
        public bool RenderWrite
        {
            get { return this.renderWrite; }
            set
            {
                if (this.renderWrite != value) 
                {
                    this.renderWrite = value;
                    this.NotifyPropertyChanged("RenderWrite");
                }
            }
        }

        #endregion

        #region OptionContentVM

        public override void OnInitialize()
        {
            this.RenderModes = new BindableCollection<RenderModeEnum>();
            this.RenderModes.AddRange(VTBaseUtils.GetEnumValues<RenderModeEnum>());
            this.RenderModes.SelectedItem = RenderModeEnum.Default;
            this.SendColor = DrawingUtils.GetColor(OptionDefaultValues.TERM_ADVANCE_SEND_COLOR);
            this.RecvColor = DrawingUtils.GetColor(OptionDefaultValues.TERM_ADVANCE_RECV_COLOR);
            this.RenderWrite = OptionDefaultValues.TERM_ADVANCE_RENDER_WRITE;
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
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_CLICK_TO_CURSOR, this.clickToCursor);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, this.autoCompletionList);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_AUTO_WRAP_MODE, this.autoWrapMode);
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_SEND_COLOR, DrawingUtils.GetRgbKey(this.writeColor));
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_RECV_COLOR, DrawingUtils.GetRgbKey(this.readColor));
            session.SetOption(OptionKeyEnum.TERM_ADVANCE_RENDER_WRITE, this.renderWrite);

            return true;
        }

        #endregion
    }
}
