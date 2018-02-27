using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Terminal;

namespace XTerminal.Simulators
{
    /// <summary>
    /// VT100类型的终端模拟器
    /// 1.解析用户输入的原始字符，解析成VT100字符或控制字符，并发送给终端
    /// 2.解析终端发送过来的控制字符或普通字符，并转转成客户端可识别的命令
    /// </summary>
    public class VT100Simulator : AbstractSimulator
    {
        public override void HandleUserInput(KeyInfo key)
        {
            if (key.IsCtrlPressed)
            {

            }
            else
            {

            }
        }

        public override void ProcessReceivedData(byte[] data)
        {
        }
    }
}