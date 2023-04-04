using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.ServiceAgents
{
    /// <summary>
    /// 对本体文件数据库进行管理
    /// </summary>
    public class FileServiceAgent : ServiceAgent
    {
        protected override int OnInitialize()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }
    }
}
