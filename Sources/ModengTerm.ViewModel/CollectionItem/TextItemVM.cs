using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.CollectionItem
{
    public class TextItemVM : CollectionItemVM
    {
        public string Text { get; set; }

        public List<VTextAttribute> TextAttributes { get; private set; }

        public TextItemVM()
        {
            this.TextAttributes = new List<VTextAttribute>();
        }
    }
}
