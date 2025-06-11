using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Addon
{
    public static class ShellFactory
    {
        private static IWindow shell;

        public static IWindow GetWindow()
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
        public static T GetActivePanel<T>() where T : IPanel
        {
            IWindow service = GetWindow();

            return service.GetActivePanel<T>();
        }

        public static List<IPanel> GetAllPanels()
        {
            IWindow window = GetWindow();

            return window.GetAllPanels();
        }
    }
}