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
    public class VT100InvocationConverter : IInvocationConverter
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VT100InvocationConverter");

        private static readonly Dictionary<Guid, IConverter> ConvertersMap = new Dictionary<Guid, IConverter>()
        {
            { typeof(OSCConverter).GUID, new OSCConverter() }
        };

        /// <summary>
        /// 分隔符
        /// </summary>
        private static readonly byte Delimiter = (byte)';';

        /// <summary>
        /// String Terminator：字符串终结符
        /// 每种终端可能使用不同的String Terminator
        /// 需要在界面做相应配置
        /// VT100使用ascii码的7，是\a
        /// </summary>
        private static readonly byte ST = CharacterUtils.BitCombinations(02, 00);

        public bool Convert(IFormattedCf cf, out ICfInvocation invocation)
        {
            invocation = null;
            IConverter converter;
            if (!ConvertersMap.TryGetValue(cf.GetType().GUID, out converter))
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
                if (!cmd.IndexOf(VT100InvocationConverter.Delimiter, out delimiterIdx))
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
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}