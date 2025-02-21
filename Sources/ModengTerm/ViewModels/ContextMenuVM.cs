using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 标题栏菜单ViewModel
    /// 同时也是侧边栏窗格的ViewModel
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
        public PanelAlignEnum PanelAlign { get; private set; }

        public string PanelEntry { get; set; }

        public string PanelVMEntry { get; set; }

        #endregion

        #region 构造方法

        public ContextMenuVM(ContextMenuDefinition definition)
        {
            this.ID = definition.ID;
            this.Name = definition.Name;
            this.PanelEntry = definition.PanelEntry;
            this.PanelVMEntry = definition.PanelVMEntry;
            this.PanelAlign = (PanelAlignEnum)definition.PanelAlign;
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

        public static void Execute(ContextMenuVM contextMenu, OpenedSessionVM openedSessionVM, object invokeObject)
        {
            ContextMenuDefinition menuDefinition = contextMenu.Definition;

            object targetObject = invokeObject;
            List<MethodInfo> methods = null;

            if (!string.IsNullOrEmpty(menuDefinition.HandlerEntry) && !string.IsNullOrEmpty(menuDefinition.MethodName))
            {
                // 先创建点击事件处理器实例
                ClickHandler clickHandler;
                if (!menuItemClickHandlers.TryGetValue(menuDefinition.HandlerEntry, out clickHandler))
                {
                    try
                    {
                        object handlerObject = ConfigFactory<object>.CreateInstance(menuDefinition.HandlerEntry);
                        methods = handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToList();
                        menuItemClickHandlers[menuDefinition.HandlerEntry] = new ClickHandler(handlerObject, methods);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("创建菜单点击事件处理器异常", ex);
                        return;
                    }
                }

                methods = clickHandler.Methods;

                targetObject = clickHandler.Object;
            }
            else
            {
                if (string.IsNullOrEmpty(menuDefinition.MethodName))
                {
                    return;
                }

                string typeName = invokeObject.GetType().ToString();
                ClickHandler clickHandler;
                if (!menuItemClickHandlers.TryGetValue(typeName, out clickHandler))
                {
                    methods = invokeObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToList();
                    clickHandler = new ClickHandler(null, methods);
                    menuItemClickHandlers[typeName] = clickHandler;
                }

                methods = clickHandler.Methods;
            }

            MethodInfo clickMethod = methods.FirstOrDefault(v => v.Name == menuDefinition.MethodName);
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
