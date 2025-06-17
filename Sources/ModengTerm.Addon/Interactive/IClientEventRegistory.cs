using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    public delegate void ClientEventDelegate(ClientEvent evType, ClientEventArgs evArgs);

    public delegate void TabEventDelegate(TabEventArgs e);

    public interface IClientEventRegistory
    {
        #region ClientEvent

        /// <summary>
        /// 订阅客户端的事件
        /// </summary>
        /// <param name="evType"></param>
        /// <param name="delegate"></param>
        void SubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate);

        /// <summary>
        /// 取消订阅客户端事件
        /// </summary>
        /// <param name="evType"></param>
        /// <param name="delegate"></param>
        void UnsubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate);

        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="evArgs"></param>
        void PublishEvent(ClientEventArgs evArgs);

        #endregion

        #region TabEvent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab">要订阅的标签页</param>
        /// <param name="evType">要订阅的事件</param>
        /// <param name="evArgs"></param>
        void SubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate);

        void UnsubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate);

        /// <summary>
        /// 取消指定tab的所有订阅的事件
        /// </summary>
        /// <param name="tab"></param>
        void UnsubscribeTabEvent(IClientTab tab);

        void PublishTabEvent(IClientTab tab, TabEventArgs evArgs);

        #endregion
    }
}
