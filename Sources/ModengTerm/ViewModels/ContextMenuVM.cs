using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Windows.Documents;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 当点击Shell会话菜单的时候触发的回调
    /// </summary>
    public delegate void ContextMenuDelegate(ContextMenuVM sender);

    public interface IContextMenuData
    {

    }

    /// <summary>
    /// 会话上下文菜单
    /// </summary>
    public class ContextMenuVM : MenuItemVM
    {
        #region 实例变量

        private bool canChecked;
        private ContextMenuDelegate executeDelegate;

        #endregion

        #region 属性

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> Children { get; set; }

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
        /// 菜单的扩展数据
        /// </summary>
        public IContextMenuData Data { get; private set; }

        #endregion

        #region 构造方法

        public ContextMenuVM()
        {
        }

        public ContextMenuVM(string name)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
        }

        public ContextMenuVM(string name, ContextMenuDelegate execute)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.executeDelegate = execute;
        }

        public ContextMenuVM(string name, ContextMenuDelegate execute, bool canChecked) :
            this(name, execute)
        {
            this.canChecked = canChecked;
        }

        public ContextMenuVM(string name, ContextMenuDelegate execute, IContextMenuData menuData) :
            this(name, execute, true)
        {
            this.Data = menuData;
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
