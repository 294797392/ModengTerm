using ModengTerm.Base.Definitions;
using ModengTerm.Base.Metadatas;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 标题栏菜单ViewModel
    /// </summary>
    public class ContextMenuVM : ItemViewModel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ContextMenuVM");

        #endregion

        #region 实例变量

        private bool canChecked;

        #endregion

        #region 属性

        public MenuMetadata Definition { get; private set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> Children { get; private set; }

        /// <summary>
        /// 是否可以勾选
        /// </summary>
        public bool CanChecked
        {
            get { return canChecked; }
            set
            {
                if (canChecked != value)
                {
                    canChecked = value;
                    this.NotifyPropertyChanged("CanChecked");
                }
            }
        }

        public string AddonId { get { return Definition.AddonId; } }

        public string Command { get { return Definition.Command; } }

        #endregion

        #region 构造方法

        public ContextMenuVM(MenuMetadata definition)
        {
            this.ID = definition.ID;
            this.Name = definition.Name;
            Children = new BindableCollection<ContextMenuVM>();
            Definition = definition;
            this.IsVisible = true;
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        #endregion
    }
}
