using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.ViewModels.CollectionItem;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 表示一个集锦，类似于edge里的集锦
    /// 一个集锦可以包含多个文本行
    /// </summary>
    public class CollectionVM : ItemViewModel
    {
        /// <summary>
        /// 该集锦下的所有集锦项
        /// </summary>
        public BindableCollection<CollectionItemVM> Items { get; private set; }

        public CollectionVM()
        {
            this.Items = new BindableCollection<CollectionItemVM>();
        }
    }
}
