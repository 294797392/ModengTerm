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
        private const string FactoryImpl = "ModengTerm.Addon.ObjectFactoryImpl, ModengTerm";
        private static ObjectFactory factory;

        /// <summary>
        /// 获取窗口实例
        /// </summary>
        /// <returns></returns>
        public abstract IHostWindow GetHostWindow();

        /// <summary>
        /// 获取数据存储服务
        /// </summary>
        /// <returns></returns>
        public abstract StorageService GetStorageService();

        /// <summary>
        /// 获取当前激活的ShellObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetActivePanel<T>() where T : IHostPanel
        {
            IHostWindow window = this.GetHostWindow();

            return window.GetActivePanel<T>();
        }

        public List<IHostPanel> GetAllPanels()
        {
            IHostWindow window = this.GetHostWindow();

            return window.GetAllPanels();
        }

        public List<T> GetAllPanels<T>() where T : IHostPanel
        {
            return this.GetAllPanels().OfType<T>().ToList();
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