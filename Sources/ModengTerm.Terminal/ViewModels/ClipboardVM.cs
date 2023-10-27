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
    /// 剪贴板里包含多个历史记录，每个历史记录使用ParagraphVM来表示
    /// </summary>
    public class ClipboardVM : ParagraphsVM
    {
        public ClipboardVM(ParagraphSource paragraphSource, ShellSessionVM videoTerminal) : 
            base(paragraphSource, videoTerminal)
        {

        }
    }
}
