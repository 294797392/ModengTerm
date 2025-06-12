using DotNEToolkit;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 提供创建插件可以使用的对象的功能
    /// </summary>
    public abstract class ObjectFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ObjectFactory");
        private const string FactoryImpl = "ModengTerm.Addon.FactoryImpl, ModengTerm";
        private static ObjectFactory factory;

        /// <summary>
        /// 获取窗口实例
        /// </summary>
        /// <returns></returns>
        public abstract IWindow GetWindow();

        /// <summary>
        /// 获取数据存储服务
        /// </summary>
        /// <returns></returns>
        public abstract StorageService GetStorage();

        /// <summary>
        /// 获取当前激活的ShellObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetActivePanel<T>() where T : IPanel
        {
            IWindow window = this.GetWindow();

            return window.GetActivePanel<T>();
        }

        public List<IPanel> GetAllPanels()
        {
            IWindow window = this.GetWindow();

            return window.GetAllPanels();
        }

        public static ObjectFactory GetFactory() 
        {
            if (factory == null)
            {
                try
                {
                    factory = ConfigFactory<ObjectFactory>.CreateInstance(FactoryImpl);
                }
                catch (Exception ex)
                {
                    logger.Error("创建Factory实例异常", ex);
                }
            }
            return factory;
        }
    }
}