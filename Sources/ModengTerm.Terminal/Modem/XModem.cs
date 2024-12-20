using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    public class XModem : ModemBase
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        /// <summary>
        /// 1. 用户执行rx
        /// 2. 点击发送文件按钮
        /// 3. 运行Send
        /// </summary>
        /// <returns></returns>
        public override int Send(string filePath)
        {
            return ResponseCode.SUCCESS;
        }

        public override int Receive()
        {
            return ResponseCode.SUCCESS;
        }
    }
}
