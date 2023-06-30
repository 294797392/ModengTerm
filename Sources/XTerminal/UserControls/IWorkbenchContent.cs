using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.UserControls
{
    public interface IWorkbenchContent
    {
        int Open();

        void Close();
    }
}
