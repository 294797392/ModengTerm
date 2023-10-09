using ModengTerm.Terminal.Document;
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
    /// <summary>
    /// 发送到所有窗口的委托
    /// </summary>
    /// <param name="text"></param>
    public delegate void SendToAllTerminalDelegate(string text);

    public class ParagraphVM : ItemViewModel
    {
        private VTParagraph paragraph;

        public string Content
        {
            get { return this.paragraph.Content; }
        }

        public DateTime CreationTime
        {
            get { return this.paragraph.CreationTime; }
        }

        public int FirstRow { get { return this.paragraph.FirstPhysicsRow; } }

        public ParagraphVM(VTParagraph paragraph)
        {
            this.ID = Guid.NewGuid().ToString();
            this.paragraph = paragraph;
        }
    }

    /// <summary>
    /// 剪贴板里包含多个历史记录，每个历史记录使用ParagraphVM来表示
    /// </summary>
    public class ClipboardVM : ViewModelBase
    {
        private IVideoTerminal videoTerminal;
        private VTClipboard clipboard;

        public BindableCollection<ParagraphVM> HistoryList { get; private set; }

        public SendToAllTerminalDelegate SendToAllTerminalDlg { get; set; }

        public ClipboardVM(IVideoTerminal vt, VTClipboard clipboard)
        {
            this.videoTerminal = vt;
            this.clipboard = clipboard;
            this.HistoryList = new BindableCollection<ParagraphVM>();

            foreach (VTParagraph paragraph in this.clipboard.HistoryList)
            {
                ParagraphVM paragraphVM = new ParagraphVM(paragraph);
                this.HistoryList.Add(paragraphVM);
            }
        }

        private ParagraphVM Sender2ParagraphVM(object sender) 
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            return frameworkElement.DataContext as ParagraphVM;
        }

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

            this.videoTerminal.SendInput(paragraph.Content);
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
    }
}
