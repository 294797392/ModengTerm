using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Session
{
    public enum SessionTreeNodeTypeEnum
    {
        /// <summary>
        /// 该节点是一个分组
        /// </summary>
        Group,

        /// <summary>
        /// 该节点是一个会话
        /// </summary>
        Session,

        /// <summary>
        /// 返回上一级分组
        /// </summary>
        GobackGroup
    }
}
