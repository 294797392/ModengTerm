using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFunctions.CfInvocations
{
    public struct ModifyWindowInfomationInvocation : ICfInvocation
    {
        public string WindowTitle;
        public string IconName;
    }
}