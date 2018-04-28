using System;
using System.IO;
using System.Threading.Tasks;
using XTerminalCore.InvocationConverting;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    /// <summary>
    /// 终端数据流解析器
    /// </summary>
    public class TerminalStreamParser
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("OutputStreamParser");

        #endregion

        #region 实例变量

        private bool isRunning = false;
        private Stream stream;
        private StreamReader reader;
        private IInvocationConverter InvocationConverter = new XtermInvocationConverter();

        #endregion

        #region 事件

        /// <summary>
        /// 当解析到一个ControlFunction时需要执行的动作
        /// </summary>
        public event Action<IFormattedCf> OnAction;

        /// <summary>
        /// 解析数据流发生错误时触发
        /// </summary>
        public event Action<byte> OnError;

        /// <summary>
        /// 当解析到普通文本时触发
        /// </summary>
        public event Action<char> OnText;

        #endregion

        #region 公开接口

        public void Initialize(Stream stream)
        {
            this.stream = stream;
        }

        public void Start()
        {
            this.isRunning = true;
            this.reader = new StreamReader(this.stream);
            Task.Factory.StartNew(this.ParsingThreadProcess, this.reader);
        }

        public void Stop()
        {
            this.isRunning = false;
            this.reader.Close();
            this.reader.Dispose();
        }

        #endregion

        #region 事件处理器

        private void ParsingThreadProcess(object state)
        {
            StreamReader reader = state as StreamReader;
            while (this.isRunning)
            {
                byte c = (byte)reader.Read();

                IControlFunctionParser parser;
                if (ControlFunctions.IsControlFunction(c, out parser))
                {
                    IFormattedCf controlFunc;
                    if (!parser.Parse(reader, out controlFunc))
                    {
                        this.NotifyError(c);
                    }
                    else
                    {
                        this.NotifyAction(controlFunc);
                    }
                }
                else
                {
                    this.NotifyText((char)c);
                }
            }
        }

        #endregion

        #region 实例方法

        private void NotifyAction(IFormattedCf controlFunc)
        {
            if (this.OnAction != null)
            {
                this.OnAction(controlFunc);
            }
        }

        private void NotifyError(byte controlFunc)
        {
            if (this.OnError != null)
            {
                this.OnError(controlFunc);
            }
        }

        private void NotifyText(char c)
        {
            if (this.OnText != null)
            {
                this.OnText(c);
            }
        }

        #endregion
    }




    ///// <summary>
    ///// 从一串字符中解析出所有ControlFunction从开头到结尾的字符
    ///// 7位编码和8位编码通用
    ///// </summary>
    ///// <param name="chars">要解析的字符</param>
    ///// <returns></returns>
    //public bool Parse(byte[] chars)
    //{
    //    int length = chars.Length;

    //    for (int idx = 0; idx < length; idx++)
    //    {
    //        byte c = chars[idx];
    //        IControlFunctionParser parser;
    //        if (ControlFunctions.IsControlFunction(c, out parser))
    //        {
    //            if (parser == null)
    //            {
    //                throw new NotImplementedException(string.Format("未实现ControlFunction'{0}'的解析器", c));
    //            }

    //            IInvocation currentInvocation;
    //            if (parser == SingleCharacterParser.Instance)
    //            {
    //                // 单字节ControlFunction
    //                SingleCharacterInvocation scInvocation;
    //                scInvocation.Action = chars[idx];
    //                currentInvocation = scInvocation;
    //            }
    //            else
    //            {
    //                IFormattedCf controlFunc;
    //                if (!parser.Parse(chars, idx, out controlFunc))
    //                {
    //                    logger.ErrorFormat("解析ControlFunction'{0}'失败", c);
    //                    return false;
    //                }
    //                logger.InfoFormat("解析成功:{0}", controlFunc);

    //                if (!this.InvocationConverter.Convert(controlFunc, out currentInvocation))
    //                {
    //                    logger.ErrorFormat("ControlFunc转Invocation失败");
    //                    return false;
    //                }
    //                idx += controlFunc.GetSize() - 1;
    //            }
    //            if (this.InvocationParsed != null)
    //            {
    //                this.InvocationParsed(currentInvocation);
    //            }
    //        }
    //        else
    //        {
    //            if (this.CharParsed != null)
    //            {
    //                this.CharParsed((char)c);
    //            }
    //        }
    //    }

    //    return true;
    //}

}