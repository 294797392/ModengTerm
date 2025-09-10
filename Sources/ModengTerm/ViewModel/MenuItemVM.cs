using ModengTerm.Base.Definitions;
using ModengTerm.Base.Metadatas;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 右键菜单和标题栏菜单ViewModel
    /// </summary>
    public class MenuItemVM : ItemViewModel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ContextMenuVM");

        #endregion

        #region 实例变量

        private bool canChecked;

        #endregion

        #region 属性

        /// <summary>
        /// 菜单的元数据信息
        /// </summary>
        public MenuMetadata Metadata { get; private set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public BindableCollection<MenuItemVM> Children { get; private set; }

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

        /// <summary>
        /// 该菜单所触发的命令Key
        /// 
        /// </summary>
        public string CommandKey { get; set; }

        #endregion

        #region 构造方法

        public MenuItemVM(MenuMetadata metadata)
        {
            this.ID = metadata.ID;
            this.Name = metadata.Name;
            this.Children = new BindableCollection<MenuItemVM>();
            this.Metadata = metadata;
            this.IsVisible = true;
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        #endregion
    }
}
