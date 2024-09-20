using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Callbacks;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public abstract class ParagraphsVM : ViewModelBase
    {
        #region 实例变量

        private ParagraphSource paragraphSource;
        protected IVideoTerminal videoTerminal;
        protected ShellSessionVM shellSession;

        #endregion

        #region 属性

        public SendToAllTerminalCallback SendToAllTerminalDlg { get; set; }

        public BindableCollection<ParagraphVM> ParagraphList { get; private set; }

        #endregion

        #region 构造方法

        public ParagraphsVM(ParagraphSource source, ShellSessionVM shellSessionVM)
        {
            this.paragraphSource = source;
            this.shellSession = shellSessionVM;
            this.videoTerminal = shellSessionVM.VideoTerminal;

            this.ParagraphList = new BindableCollection<ParagraphVM>();
            this.ParagraphList.AddRange(this.paragraphSource.GetParagraphs());
        }

        #endregion

        #region 实例方法

        private ParagraphVM Sender2ParagraphVM(object sender)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            return frameworkElement.DataContext as ParagraphVM;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 定位到指定的段落
        /// </summary>
        /// <param name="paragraph">要定位到的段落</param>
        public void Locate(object sender, EventArgs args)
        {
            ParagraphVM paragraph = this.Sender2ParagraphVM(sender);

            this.videoTerminal.ScrollTo(paragraph.FirstRow, ScrollOptions.ScrollToTop);
        }

        /// <summary>
        /// 发送到窗口
        /// </summary>
        public void SendToTerminal(object sender, EventArgs args)
        {
            ParagraphVM paragraph = this.Sender2ParagraphVM(sender);

            this.shellSession.SendText(paragraph.Content);
        }

        /// <summary>
        /// 发送到所有窗口
        /// </summary>
        /// <param name="paragraph"></param>
        public void SendToAllTerminal(object sender, EventArgs args)
        {
            ParagraphVM paragraph = this.Sender2ParagraphVM(sender);

            this.SendToAllTerminalDlg(paragraph.Content);
        }

        #endregion
    }
}
