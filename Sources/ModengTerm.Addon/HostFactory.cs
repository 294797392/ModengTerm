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
    public abstract class HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ObjectFactory");
        private const string FactoryImpl = "ModengTerm.Addon.ObjectFactoryImpl, ModengTerm";
        private static HostFactory factory;

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
        /// 获取当前激活的Tab页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetActiveTab<T>() where T : IHostTab
        {
            IHostWindow window = this.GetHostWindow();

            return window.GetActiveTab<T>();
        }

        public List<IHostTab> GetAllTabs()
        {
            IHostWindow window = this.GetHostWindow();

            return window.GetAllTabs();
        }

        public List<T> GetAllTabs<T>() where T : IHostTab
        {
            return this.GetAllTabs().OfType<T>().ToList();
        }

        /// <summary>
        /// 获取工厂实例
        /// </summary>
        /// <returns></returns>
        public static HostFactory GetFactory() 
        {
            if (factory == null)
            {
                try
                {
                    factory = ConfigFactory<HostFactory>.CreateInstance(FactoryImpl);
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