using AsciiControlFunctions.CfInvocations;
using AsciiControlFunctions.FeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions.CfInvocationConverters
{
    /// <summary>
    /// 根据VT100终端的规则把ControlFunction的内容转换成一个Invocation
    /// 
    /// IConverter.Convert函数中使用的各种变量的含义：
    /// c   The literal character c.
    /// C   A single(required) character.
    /// Ps  A single(usually optional) numeric parameter, composed of one of more digits.
    /// Pm  A multiple numeric parameter composed of any number of single numeric parameters, 
    ///     separated by;  character(s).  Individual values for the parameters are listed with Ps.
    /// Pt  A text parameter composed of printable characters.
    /// 
    /// 参考：
    ///     http://invisible-island.net/xterm/ctlseqs/ctlseqs.html
    /// </summary>
    public class XtermInvocationConverter : IInvocationConverter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VT100InvocationConverter");

        /// <summary>
        /// FormattedCf -> FormattedCfConverter
        /// </summary>
        private static readonly Dictionary<string, IConverter> ConvertersMap = new Dictionary<string, IConverter>()
        {
            { typeof(FormattedCSI).Name, new CSIConverter() },
            { typeof(FormattedOSC).Name, new OSCConverter() }
        };

        /// <summary>
        /// 分隔符
        /// </summary>
        private static readonly byte Delimiter = (byte)';';

        /// <summary>
        /// String Terminator
        /// 终端定义的字符串结束符
        /// </summary>
        private static readonly byte ST = DefaultValues.ST;

        private static readonly Encoding Encoding = Encoding.ASCII;

        public XtermInvocationConverter()
        {
        }

        public bool Convert(IFormattedCf cf, out ICfInvocation invocation)
        {
            invocation = null;
            IConverter converter;
            if (!ConvertersMap.TryGetValue(cf.GetType().Name, out converter))
            {
                logger.ErrorFormat("未实现{0}的Convert", cf);
                return false;
            }

            return converter.Convert(cf, out invocation);
        }

        #region Converter Classes

        private class OSCConverter : IConverter
        {
            public bool Convert(IFormattedCf cf, out ICfInvocation invocation)
            {
                invocation = null;
                var osc = (FormattedOSC)cf;
                byte[] cmd = osc.CommandString;

                #region 解析Ps

                int delimiterIdx = 0;
                if (!cmd.IndexOf(XtermInvocationConverter.Delimiter, out delimiterIdx))
                {
                    logger.ErrorFormat("解析OSC Ps失败, 未找到分隔符");
                    return false;
                }
                byte[] psBytes = new byte[delimiterIdx + 1];
                Array.Copy(cmd, 0, psBytes, 0, psBytes.Length);
                string strPs = Encoding.ASCII.GetString(psBytes);
                int Ps = 0;
                if (!int.TryParse(strPs, out Ps))
                {
                    logger.ErrorFormat("解析OSC Ps失败, 无效的Ps:{0}", strPs);
                    return false;
                }

                #endregion

                #region 解析Pt

                byte[] ptBytes = new byte[cmd.Length - psBytes.Length - 1];
                Array.Copy(cmd, delimiterIdx + 1, ptBytes, 0, ptBytes.Length);
                string Pt = ptBytes.Stringify();

                switch (Ps)
                {
                    case 0:
                        {
                            // Change Icon Name and Window Title to Pt.
                            ModifyWindowInfomationInvocation mwi;
                            mwi.WindowTitle = Pt;
                            mwi.IconName = Pt;
                            invocation = mwi;
                            break;
                        }

                    case 1:
                        {
                            // Change Icon Name to Pt
                            ModifyWindowInfomationInvocation mwi;
                            mwi.WindowTitle = null;
                            mwi.IconName = Pt;
                            invocation = mwi;
                            break;
                        }

                    case 2:
                        {
                            // Change Window Title to Pt
                            ModifyWindowInfomationInvocation mwi;
                            mwi.WindowTitle = Pt;
                            mwi.IconName = null;
                            invocation = mwi;
                            break;
                        }

                    case 3:
                        {
                            // Set X property on top-level window.  Pt should be in the form "prop=value", or just "prop" to delete the property
                            break;
                        }

                    default:
                        throw new NotImplementedException(string.Format("未实现的Ps:{0}", Ps));

                }

                #endregion

                return true;
            }
        }

        private class CSIConverter : IConverter
        {
            public bool Convert(IFormattedCf cf, out ICfInvocation invocation)
            {
                invocation = null;
                var csi = (FormattedCSI)cf;

                byte finalByte = csi.FinalByte.Content;

                if (csi.FinalByte.PrivateUse)
                {
                    throw new NotImplementedException(string.Format("未实现, FormattedCSI Private Use, FinalByte:{0}", finalByte));
                }

                if (csi.FinalByte.WithIntermediateByte0200)
                {
                    throw new NotImplementedException(string.Format("未实现, FormattedCSI WithIntermediateByte0200, FinalByte:{0}", finalByte));
                }

                if (finalByte == FinalByte.CUU || finalByte == FinalByte.CUD ||
                    finalByte == FinalByte.CUB || finalByte == FinalByte.CUF)
                {
                    #region 移动光标操作

                    int times = 1;
                    MoveCursorInvocation mcInvocation;
                    mcInvocation.Direction = FinalByte2Direction(finalByte);
                    mcInvocation.X = 0;
                    mcInvocation.Y = 0;
                    if (csi.ParameterBytes.Length > 0)
                    {
                        if (!csi.ParameterBytes.Numberic(out times))
                        {
                            logger.ErrorFormat("解析光标操作失败");
                            return false;
                        }
                    }
                    mcInvocation.Times = times;
                    invocation = mcInvocation;

                    #endregion
                }
                else if (finalByte == FinalByte.CUP)
                {
                    #region 设置光标位置

                    int row = 1, column = 1;
                    if (csi.ParameterBytes.Length > 0)
                    {
                        string parameter = Encoding.GetString(csi.ParameterBytes);
                        string[] items = parameter.Split((char)Delimiter);
                        if (items.Length != 2)
                        {
                            logger.ErrorFormat("解析设置光标位置失败, parameter:{0}", parameter);
                            return false;
                        }
                        if (!int.TryParse(items[0], out row) || !int.TryParse(items[1], out column))
                        {
                            logger.ErrorFormat("解析设置光标位置失败, parameter:{0}", parameter);
                            return false;
                        }
                    }

                    MoveCursorInvocation mvInvocation;
                    mvInvocation.Direction = MoveCursorInvocation.CursorDirectionEnum.Custom;
                    mvInvocation.X = row;
                    mvInvocation.Y = column;
                    mvInvocation.Times = 0;
                    invocation = mvInvocation;

                    #endregion
                }
                else
                {
                    throw new NotImplementedException(string.Format("未实现FinalByte:{0}", finalByte));
                }

                return true;
            }

            private static MoveCursorInvocation.CursorDirectionEnum FinalByte2Direction(byte finalByte)
            {
                if (finalByte == FinalByte.CUU)
                {
                    return MoveCursorInvocation.CursorDirectionEnum.Up;
                }
                else if (finalByte == FinalByte.CUD)
                {
                    return MoveCursorInvocation.CursorDirectionEnum.Down;
                }
                else if (finalByte == FinalByte.CUB)
                {
                    return MoveCursorInvocation.CursorDirectionEnum.Left;
                }
                else
                {
                    return MoveCursorInvocation.CursorDirectionEnum.Right;
                }
            }
        }

        #endregion
    }
}