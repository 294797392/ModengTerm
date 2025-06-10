using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Addon
{
    public static class ShellFactory
    {
        private static IShellService shell;

        public static IShellService GetService()
        {
            if (shell == null)
            {
                //shell = new ShellServiceImpl();
            }
            return shell;
        }

        /// <summary>
        /// 获取当前激活的ShellObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetActiveShell<T>() where T : IShellObject
        {
            IShellService service = GetService();

            return service.GetActiveShell<T>();
        }

        public static List<T> GetAllShells<T>() where T : IShellObject
        {
            IShellService shell = GetService();

            return shell.GetShellObjects<T>();
        }
    }
}