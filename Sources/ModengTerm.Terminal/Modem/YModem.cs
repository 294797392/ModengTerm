using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    public class YModem : ModemBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("YModem");

        #region ModemBase

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePaths">filePaths[0]是文件夹路径</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override int Receive(List<string> filePaths)
        {
            throw new NotImplementedException();
        }

        public override int Send(List<string> filePaths)
        {
            byte[] onebyte = new byte[1];

            int n = this.HostStream.Receive(onebyte);

            if (n <= 0)
            {
                logger.ErrorFormat("startbyte read failed, {0}", n);
                return ResponseCode.FAILED;
            }


            throw new NotImplementedException();
        }

        #endregion
    }
}
