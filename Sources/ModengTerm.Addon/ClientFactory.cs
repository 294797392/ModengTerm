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
    public abstract class ClientFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientFactory");
        private const string ImplClass = "ModengTerm.Addon.ClientFactoryImpl,ModengTerm";

        private static ClientFactory instance;

        public abstract StorageService GetStorageService();

        public abstract IClientWindow GetHostWindow();

        public abstract IClientEventRegistory GetEventRegistory();

        public abstract List<ISidePanel> CreateSidePanels(List<PanelDefinition> definitions);

        public abstract IOverlayPanel CreateOverlayPanel(PanelDefinition definition);

        public static ClientFactory GetFactory() 
        {
            if (instance == null)
            {
                try
                {
                    instance = ConfigFactory<ClientFactory>.CreateInstance(ImplClass);
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