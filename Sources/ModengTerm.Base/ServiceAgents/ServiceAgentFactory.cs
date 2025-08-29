using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.ServiceAgents
{
    public static class ServiceAgentFactory
    {
        private static ServiceAgent serviceAgent;

        /// <summary>
        /// 获取ServiceAgent的实例
        /// </summary>
        /// <returns></returns>
        public static ServiceAgent Get()
        {
            if (serviceAgent == null) 
            {
                serviceAgent = new LocalServiceAgent();
                serviceAgent.Initialize();
            }

            return serviceAgent;
        }
    }
}
