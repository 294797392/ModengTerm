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
        public string ClassName { get { return this.Metadata.ClassName; } }

        /// <summary>
        /// 界面实例
        /// </summary>
        public FrameworkElement FrameworkElement { get; set; }

        /// <summary>
        /// 该预选项所属的插件
        /// </summary>
        public AddonMetadata AddonMetadata { get; set; }

        /// <summary>
        /// 配置项的元数据
        /// </summary>
        public PreferenceMetadata Metadata { get; set; }

        public PreferenceNodeVM(TreeViewModelContext context) :
            base(context)
        {
        }
    }
}
