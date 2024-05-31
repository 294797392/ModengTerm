using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.ViewModels
{
    public class BookmarksVM : ParagraphsVM
    {
        public BookmarksVM(ParagraphSource source, ShellSessionVM videoTerminal) :
            base(source, videoTerminal)
        {
        }
    }
}
