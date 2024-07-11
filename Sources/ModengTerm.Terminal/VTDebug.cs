using DotNEToolkit;
using log4net.Util;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public enum VTSendTypeEnum
    {
        /// <summary>
        /// 按键盘输入
        /// </summary>
        UserInput,

        /// <summary>
        /// 响应DSR事件
        /// </summary>
        DSR_DeviceStatusReport,

        /// <summary>
        /// 响应DA_DeviceAttributes事件
        /// </summary>
        DA_DeviceAttributes
    }

    /// <summary>
    /// 提供一些记录日志的工具函数
    /// </summary>
    public class VTDebug : SingletonObject<VTDebug>
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDebug");
        private log4net.ILog codeLogger = log4net.LogManager.GetLogger("code");
        private log4net.ILog interactiveLogger = log4net.LogManager.GetLogger("interactive");
        private log4net.ILog receivedLogger = log4net.LogManager.GetLogger("received");
        private log4net.ILog rawReadLogger = log4net.LogManager.GetLogger("rawread");

        private int vttestCodeIndex;

        public VTDebug()
        {
        }

        public void WriteInteractive(VTSendTypeEnum type, byte[] bytes)
        {
            if (!interactiveLogger.IsDebugEnabled)
            {
                return;
            }

            string message = type == VTSendTypeEnum.UserInput ?
                bytes.Select(v => ((char)v).ToString()).Join(",") :
                bytes.Select(v => ((int)v).ToString()).Join(",");

            string log = string.Format("-> [{0},{1}]", type, message);

            interactiveLogger.Info(log);
        }

        public void WriteInteractive(VTSendTypeEnum type, StatusType statusType, byte[] bytes)
        {
            if (!interactiveLogger.IsDebugEnabled)
            {
                return;
            }

            string message = bytes.Select(v => ((int)v).ToString()).Join(",");
            string log = string.Format("-> [{0},{1},{2}]", type, statusType, message);
            interactiveLogger.Info(log);
        }

        public void WriteInteractive(string action, string format, params object[] param)
        {
            if (!interactiveLogger.IsDebugEnabled)
            {
                return;
            }

            string message = string.Format(format, param);
            string log = string.Format("<- [{0},{1}]", action, message);
            interactiveLogger.Info(log);
        }

        public void WriteCode(string action, List<byte> sequence)
        {
            if (!codeLogger.IsDebugEnabled)
            {
                return;
            }

            string varName = string.Format("{0}{1}", action, this.vttestCodeIndex++);
            string codeSequence = string.Empty;

            if (action == "LF")
            {
                codeSequence = "'\\n'";
            }
            else if (action == "CR")
            {
                codeSequence = "'\\r'";
            }
            else
            {
                codeSequence = string.Join(',', sequence.Select(v => string.Format("'{0}'", Convert.ToChar(v))));
            }

            codeSequence += ",'\\0'";

            string log = string.Format("char {0}[] = {{{1}}};printf({2}); // {3}", varName, codeSequence, varName, action);

            this.codeLogger.Info(log);
        }

        public void WriteRawRead(byte[] bytes, int size)
        {
            if (!this.rawReadLogger.IsDebugEnabled)
            {
                return;
            }

            string message = bytes.Take(size).Select(v => ((int)v).ToString()).Join(",");
            this.rawReadLogger.Info(message);
        }

        /// <summary>
        /// 记录从服务器接收到的原始指令和指令对应的参数
        /// </summary>
        /// <param name="action"></param>
        /// <param name="format"></param>
        /// <param name="param"></param>
        public void WriteReceived(string action, string format, params object[] param)
        {
            if (!this.receivedLogger.IsDebugEnabled)
            {
                return;
            }

            string message = string.Format(format, param);
            string log = string.Format("<- [{0},{1}]", action, message);
            this.receivedLogger.Info(log);
        }
    }
}