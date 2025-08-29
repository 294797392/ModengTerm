using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Metadatas;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.CreateSession
{
    public class PreferenceNodeVM : TreeNodeViewModel
    {
        /// <summary>
        /// 界面入口类
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 界面实例
        /// </summary>
        public FrameworkElement FrameworkElement { get; set; }

        /// <summary>
        /// 该预选项所属的插件
        /// </summary>
        public AddonMetadata AddonMetadata { get; set; }

        /// <summary>
        /// 配置项的默认值列表
        /// </summary>
        public Dictionary<string, object> DefaultOptions { get; private set; }

        public PreferenceNodeVM(TreeViewModelContext context) :
            base(context)
        {
            this.DefaultOptions = new Dictionary<string, object>();
        }
    }
}
