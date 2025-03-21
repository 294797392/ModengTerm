using ModengTerm.Base.DataModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    public class ModengAPI
    {
        #region 构造方法

        public ModengAPI()
        {

        }

        #endregion

        #region API

        #region SideWindowAPI

        /// <summary>
        /// 新建侧边栏窗口
        /// </summary>
        protected SideWindow CreateSideWindow(SideWindowMetadata swm)
        {

        }

        /// <summary>
        /// 删除侧边栏窗口
        /// </summary>
        protected void DeleteSideWindow(SideWindow window) { }

        /// <summary>
        /// 关闭侧边栏窗口
        /// </summary>
        protected void CloseSideWindow(SideWindow window) { }

        /// <summary>
        /// 打开侧边栏窗口
        /// </summary>
        protected void OpenSideWindow(SideWindow window) { }

        #endregion

        #region SessionAPI

        /// <summary>
        /// 获取当前所有打开的Session
        /// </summary>
        /// <returns></returns>
        protected List<OpenedSessionVM> GetSessions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打开指定的会话
        /// </summary>
        /// <param name="session"></param>
        protected void OpenSession(XTermSession session)
        { }

        /// <summary>
        /// 关闭某个Session
        /// </summary>
        /// <param name="session"></param>
        protected void CloseSession(OpenedSessionVM session)
        { }

        /// <summary>
        /// 选中某个Session
        /// </summary>
        /// <param name="session"></param>
        protected void SelectSession(OpenedSessionVM session)
        { }

        #endregion

        #endregion
    }
}
