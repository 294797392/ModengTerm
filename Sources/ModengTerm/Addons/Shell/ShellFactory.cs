using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Shell
{
    public static class ShellFactory
    {
        private static AbstractShell shell;

        public static AbstractShell GetShell()
        {
            if (shell == null)
            {
                shell = new DefaultShellImpl();
            }
            return shell;
        }
    }
}
