using ModengTerm.Terminal.Modem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modules
{
    public static class VTModuleFactory
    {
        public static VTModuleBase Create(VTModuleTypes types)
        {
            switch (types)
            {
                case VTModuleTypes.XModem: return new XModem();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class ModuleMgr
    {
        private List<VTModuleBase> moduleList;

        public ModuleMgr()
        {

        }

        #region 公开接口

        public void Initialize()
        {
            this.moduleList = new List<VTModuleBase>();
        }

        public void Release()
        {
            foreach (VTModuleBase module in this.moduleList)
            {
                module.Stop();
            }

            this.moduleList.Clear();
        }

        #endregion

        public T Request<T>(VTModuleTypes type) where T : VTModuleBase
        {
            T module = this.moduleList.OfType<T>().FirstOrDefault(v => v.Type == type);
            if (module == null)
            {
                module = (T)VTModuleFactory.Create(type);
                this.moduleList.Add(module);
            }

            return module;
        }
    }
}
