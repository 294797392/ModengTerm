using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels.Terminals;
using System.Collections.Generic;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class ContextMenuDefinition
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父菜单的ID
        /// 如果为空表示没有父菜单，直接显示到根节点
        /// 如果为-1表示在标题上不显示该菜单
        /// </summary>
        public string TitleParentID { get; set; }

        /// <summary>
        /// 右键菜单的ParentID
        /// 如果为-1表示不显示右键菜单
        /// </summary>
        public string ContextParentID { get; set; }

        public string Name { get; set; }

        public string ClassName { get; set; }

        public string VMClassName { get; set; }

        public ContextMenuDelegate Callback { get; set; }

        /// <summary>
        /// 如果这个菜单需要显示界面，PanelID指定界面显示在哪个Panel里
        /// </summary>
        public string PanelID { get; set; }

        /// <summary>
        /// 支持的会话类型
        /// 如果数量是0，那么表示支持所有会话类型
        /// </summary>
        public List<SessionTypeEnum> SupportedSessionTypes { get; set; } = new List<SessionTypeEnum>();

        public ContextMenuDefinition(string id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public ContextMenuDefinition(string id, string titleParentID, string contextParentID, string name)
        {
            this.ID = id;
            this.TitleParentID = titleParentID;
            this.ContextParentID = contextParentID;
            this.Name = name;
        }

        public ContextMenuDefinition(string id, string titleParentID, string contextParentID, string name, ContextMenuDelegate callback)
        {
            this.ID = id;
            this.TitleParentID = titleParentID;
            this.ContextParentID = contextParentID;
            this.Name = name;
            this.Callback = callback;
        }

        public ContextMenuDefinition(string id, string titleParentID, string contextParentID, string name, string className, string vmClassName, string panelID, ContextMenuDelegate callback)
        {
            this.ID = id;
            this.TitleParentID = titleParentID;
            this.ContextParentID = contextParentID;
            this.Name = name;
            this.ClassName = className;
            this.VMClassName = vmClassName;
            this.Callback = callback;
            this.PanelID = panelID;
        }
    }

    /// <summary>
    /// 当点击Shell会话菜单的时候触发的回调
    /// </summary>
    public delegate void ContextMenuDelegate(ContextMenuVM sender);

    /// <summary>
    /// 标题栏菜单ViewModel
    /// 同时也是侧边栏窗格的ViewModel
    /// </summary>
    public class ContextMenuVM : ItemViewModel
    {
        #region 实例变量

        private bool canChecked;
        private ContextMenuDelegate executeDelegate;

        #endregion

        #region 属性

        public ContextMenuDefinition Definition { get; private set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> Children { get; private set; }

        /// <summary>
        /// 是否可以勾选
        /// </summary>
        public bool CanChecked
        {
            get { return this.canChecked; }
            set
            {
                if (this.canChecked != value)
                {
                    this.canChecked = value;
                    this.NotifyPropertyChanged("CanChecked");
                }
            }
        }

        /// <summary>
        /// 作为侧边栏窗格的ViewModel，它所属的侧边栏窗格容器Id
        /// </summary>
        public string PanelId { get; private set; }

        public string ClassName { get; set; }

        public string VMClassName { get; set; }

        #endregion

        #region 构造方法

        public ContextMenuVM(ContextMenuDefinition definition)
        {
            this.ID = definition.ID;
            this.Name = definition.Name;
            this.ClassName = definition.ClassName;
            this.VMClassName = definition.VMClassName;
            this.PanelId = definition.PanelID;
            this.executeDelegate = definition.Callback;
            this.Children = new BindableCollection<ContextMenuVM>();
            this.Definition = definition;
        }

        #endregion

        #region 实例方法

        private void AddSubItems(List<ContextMenuVM> container, ContextMenuVM parent)
        {
            container.AddRange(parent.Children);

            foreach (ContextMenuVM child in parent.Children)
            {
                if (child.Children == null)
                {
                    continue;
                }

                this.AddSubItems(container, child);
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 当点击菜单的时候执行
        /// </summary>
        public void Execute()
        {
            if (this.executeDelegate == null)
            {
                return;
            }

            this.executeDelegate(this);
        }

        /// <summary>
        /// 递归获取所有子级元素
        /// </summary>
        /// <returns></returns>
        public List<ContextMenuVM> GetChildrenRecursive()
        {
            if (this.Children == null)
            {
                return new List<ContextMenuVM>();
            }

            List<ContextMenuVM> subItems = new List<ContextMenuVM>();

            subItems.AddRange(this.Children);

            foreach (ContextMenuVM rootItem in this.Children)
            {
                if (rootItem.Children == null)
                {
                    continue;
                }

                this.AddSubItems(subItems, rootItem);
            }

            return subItems;
        }

        #endregion
    }
}
