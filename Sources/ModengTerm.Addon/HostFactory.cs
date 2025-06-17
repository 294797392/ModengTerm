using DotNEToolkit;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactory");
        private const string ImplClass = "ModengTerm.Addon.HostFactoryImpl,ModengTerm";

        private static HostFactory instance;

        public abstract StorageService GetStorageService();

        public abstract IHostWindow GetHostWindow();

        public abstract IHostEventRegistory GetEventRegistory();

        public abstract List<ISidePanel> CreateSidePanels(List<PanelDefinition> definitions);

        public abstract List<IOverlayPanel> CreateOverlayPanels(List<PanelDefinition> definitions);

        public static HostFactory GetFactory() 
        {
            if (instance == null)
            {
                try
                {
                    instance = ConfigFactory<HostFactory>.CreateInstance(ImplClass);
                }
                catch (Exception ex) 
                {
                    logger.Error("创建HostFactory异常", ex);
                    return null;
                }
            }

            return instance;
        }
    }
}