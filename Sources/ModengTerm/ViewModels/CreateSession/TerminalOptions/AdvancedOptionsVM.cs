using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.TerminalOptions
{
    public class AdvancedOptionsVM : OptionContentVM
    {
        #region 实例变量

        private bool clickToCursor;
        private bool autoWrapMode;
        private bool autoCompletionList;

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

        #endregion

        #region OptionContentVM

        public override void OnInitialize()
        {
            RenderModes = new BindableCollection<RenderModeEnum>();
            RenderModes.AddRange(MTermUtils.GetEnumValues<RenderModeEnum>());
            RenderModes.SelectedItem = RenderModeEnum.Default;
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

            return true;
        }

        #endregion
    }
}
