using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
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

        public MenuItemDefinition Definition { get; private set; }

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

        #endregion

        #region 构造方法

        public ContextMenuVM(MenuItemDefinition definition)
        {
            this.ID = definition.ID;
            this.Name = definition.Name;
            this.Children = new BindableCollection<ContextMenuVM>();
            this.Definition = definition;
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        #endregion
    }

    public static class ContextMenuHelper
    {
        private class ClickHandler
        {
            public object Object { get; private set; }

            public List<MethodInfo> Methods { get; private set; }

            public ClickHandler(object _object, List<MethodInfo> methods)
            {
                this.Object = _object;
                this.Methods = methods;
            }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ContextMenuHelper");
        private static Dictionary<string, ClickHandler> menuItemClickHandlers = new Dictionary<string, ClickHandler>();

        public static void Execute(ContextMenuVM contextMenu, OpenedSessionVM openedSessionVM)
        {
            MenuItemDefinition menuDefinition = contextMenu.Definition;

            if (string.IsNullOrEmpty(menuDefinition.HandlerEntry) || string.IsNullOrEmpty(menuDefinition.MethodName))
            {
                logger.ErrorFormat("{0}缺少参数", menuDefinition.Name);
                return;
            }

            // 先创建点击事件处理器实例
            ClickHandler clickHandler;
            if (!menuItemClickHandlers.TryGetValue(menuDefinition.HandlerEntry, out clickHandler))
            {
                try
                {
                    object handlerObject = ConfigFactory<object>.CreateInstance(menuDefinition.HandlerEntry);
                    List<MethodInfo> methods = handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToList();
                    clickHandler = new ClickHandler(handlerObject, methods);
                    menuItemClickHandlers[menuDefinition.HandlerEntry] = clickHandler;
                }
                catch (Exception ex)
                {
                    logger.Error("创建菜单点击事件处理器异常", ex);
                    return;
                }
            }

            object targetObject = clickHandler.Object;

            MethodInfo clickMethod = clickHandler.Methods.FirstOrDefault(v => v.Name == menuDefinition.MethodName);
            if (clickMethod == null)
            {
                logger.ErrorFormat("调用点击事件失败, 没找到指定的方法:{0}, entry:{1}", menuDefinition.MethodName, menuDefinition.HandlerEntry);
                return;
            }

            object[] parameters = new object[]
            {
                contextMenu, openedSessionVM
            };

            try
            {
                clickMethod.Invoke(targetObject, parameters);
            }
            catch (Exception ex)
            {
                MTMessageBox.Error("执行失败");
                logger.Error("执行点击事件异常", ex);
            }
        }
    }
}
