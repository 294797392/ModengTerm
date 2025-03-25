using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// TODO：尝试把PanelContentVM改造成插件
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PanelContentVM");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        #endregion

        #region MenuContentVM

        public override void OnInitialize()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        public override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 抽象方法

        #endregion
    }

    public abstract class SessionPanelContentVM : MenuContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SessionPanelContentVM");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        #endregion

        #region MenuContentVM

        static int id = 0;

        public override void OnInitialize()
        {
            this.ID = ++id;
        }

        public override void OnLoaded()
        {
            logger.InfoFormat("{0} OnLoaded", this.ID);
        }

        public override void OnUnload()
        {
            logger.InfoFormat("{0} OnUnload", this.ID);
        }

        public override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 抽象方法

        #endregion
    }
}
