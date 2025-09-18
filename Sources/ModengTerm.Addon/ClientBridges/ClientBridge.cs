using DotNEToolkit;
using ModengTerm.Addon.ClientBridges.Impl;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addon.ClientBridges
{
    /// <summary>
    /// 提供插件可以调用的客户端接口
    /// </summary>
    internal abstract class ClientBridge
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientBridge");
        private const string ImplClass = "ModengTerm.Addon.ClientBridges.Impl.ClientFactoryImpl,ModengTerm.Addon";

        private static ClientBridge instance;

        public abstract IClientStorage GetStorageService();

        public abstract IClient GetClient();

        public abstract IClientEventRegistry GetEventRegistry();

        public static ClientBridge GetBridge()
        {
            if (instance == null)
            {
                try
                {
                    instance = new ClientFactoryImpl();

                    //instance = ConfigFactory<ClientBridge>.CreateInstance(ImplClass);
                }
                catch (Exception ex)
                {
                    logger.Error("创建ClientBridge异常", ex);
                    return null;
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// 供插件调用的访问客户端功能的接口
    /// </summary>
    public static class Client
    {
        public static IClientEventRegistry GetEventRegistry()
        {
            return ClientBridge.GetBridge().GetEventRegistry();
        }

        public static IClient GetClient()
        {
            return ClientBridge.GetBridge().GetClient();
        }

        public static IClientStorage GetStorage() 
        {
            return ClientBridge.GetBridge().GetStorageService();
        }

        public static T GetActiveTab<T>() where T : IClientTab
        {
            return ClientBridge.GetBridge().GetClient().GetActiveTab<T>();
        }

        public static void RegisterCommand(string commandKey, CommandDelegate @delegate, object userData = null) 
        {
            ClientBridge.GetBridge().GetEventRegistry().RegisterCommand(commandKey, @delegate, userData);
        }
    }
}