using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace ModengTerm.OfficialAddons.Logger
{
    public class LoggerAddon : AddonModule
    {
        #region 类变量

        private static log4net.ILog log4netLogger = log4net.LogManager.GetLogger("LoggerAddon");

        #endregion

        #region 实例变量

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext e)
        {
        }

        protected override void OnDeactive()
        {
        }

        #endregion
    }
}
