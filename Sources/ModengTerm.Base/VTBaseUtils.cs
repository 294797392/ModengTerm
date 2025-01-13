using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Base
{
    public static class VTBaseUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("MTermUtils");

        public static List<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<T> GetEnumValues<T>(params T[] excludes) where T : Enum
        {
            List<T> values = GetEnumValues<T>();
            if (excludes.Length > 0)
            {
                foreach (T value in excludes)
                {
                    values.Remove(value);
                }
            }
            return values;
        }

        public static bool IsTerminal(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.AdbShell:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Localhost:
                    {
                        return true;
                    }

                case SessionTypeEnum.SFTP:
                    {
                        return false;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public static int Clamp(int val, int minval, int maxval)
        {
            if (val < minval)
            {
                return minval;
            }
            else if (val > maxval)
            {
                return maxval;
            }

            return val;
        }

        /// <summary>
        /// 把一个16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public static bool TryParseHexString(string hex, out byte[] parsed)
        {
            parsed = null;

            if (string.IsNullOrEmpty(hex))
            {
                parsed = new byte[0];
                return true;
            }

            // 先删除所有的0x（如果有的话）
            string lowerHex = hex.ToLower().Replace("0x", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

            // 十六进制字符串的长度必须是2的倍数，2个字符等于1个字节
            if (lowerHex.Length % 2 != 0)
            {
                return false;
            }

            List<byte> values = new List<byte>();

            // 再一个一个字节拆分16进制字符串
            for (int i = 0; i < lowerHex.Length; i += 2)
            {
                byte v;
                if (!byte.TryParse(lowerHex.Substring(i, 2), NumberStyles.HexNumber, null, out v))
                {
                    return false;
                }

                values.Add(v);
            }

            parsed = values.ToArray();

            return true;
        }

        public static bool IsWin10()
        {
            return Environment.OSVersion.Version.Major >= 10;
        }

        /// <summary>
        /// 判断是否是正确的端口号
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsValidNetworkPort(int port)
        {
            return port > 0 && port < 65535;
        }

        public static int GetWatchInterval(WatchFrequencyEnum frequency)
        {
            switch (frequency)
            {
                case WatchFrequencyEnum.Normal: return VTBaseConsts.WatchIntervalNormal;
                case WatchFrequencyEnum.High: return VTBaseConsts.WatchIntervalHigh;
                case WatchFrequencyEnum.Low: return VTBaseConsts.WatchIntervalLow;
                default:
                    throw new NotImplementedException();
            }
        }

        public static string TrimInvalidFileNameChars(string srcFileName)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            foreach (char invalidChar in invalidFileNameChars)
            {
                srcFileName = srcFileName.Replace(invalidChar, '_');
            }

            srcFileName = srcFileName.Replace(" ", "_");

            return srcFileName;
        }

        #region UnitValue

        /// <summary>
        /// 把src转换成可读的值并存储到dest
        /// </summary>
        /// <param name="updateTo"></param>
        /// <param name="updateFrom"></param>
        /// <param name="decimals"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static bool UpdateReadable(UnitValueDouble updateTo, UnitValue64 updateFrom, int decimals = 2)
        {
            // 要转换的值和被转换之前的值没有变化，直接返回
            if (updateTo.FromValue == updateFrom.Value)
            {
                return false;
            }

            updateTo.FromValue = updateFrom.Value;

            // 小于1024，如果再转换那就小于1了，显示不友好
            // 所以直接返回
            if (updateFrom.Value < 1024)
            {
                updateTo.Value = updateFrom.Value;
                updateTo.Unit = updateFrom.Unit;
                return true;
            }

            ulong fromValue = updateFrom.Value;
            double toValue = fromValue / 1024;

            switch (updateFrom.Unit)
            {
                case UnitType.Byte: goto FromBytes;
                case UnitType.KB: goto FromKB;
                case UnitType.MB: goto FromMB;
                case UnitType.GB: goto FromGB;
                default:
                    throw new NotImplementedException();
            }

            FromBytes:
            if (toValue < 1024)
            {
                updateTo.Value = Math.Round(toValue, decimals);
                updateTo.Unit = UnitType.KB;
                return true;
            }

            toValue = toValue / 1024;

            FromKB:
            if (toValue < 1024)
            {
                updateTo.Value = Math.Round(toValue, decimals);
                updateTo.Unit = UnitType.MB;
                return true;
            }

            toValue = toValue / 1024;

            FromMB:
            if (toValue < 1024)
            {
                updateTo.Value = Math.Round(toValue, decimals);
                updateTo.Unit = UnitType.GB;
                return true;
            }

            toValue = toValue / 1024;

            FromGB:
            if (toValue < 1024)
            {
                updateTo.Value = Math.Round(toValue, decimals);
                updateTo.Unit = UnitType.TB;
            }
            else
            {
                // 大于1024TB
                updateTo.Value = Math.Round(toValue, decimals);
                updateTo.Unit = UnitType.TB;
            }

            return true;
        }

        public static void UpdateReadable(UnitValueDouble updateTo, UnitType toUnit, ulong fromValue, UnitType fromUnit, int decimals = 2)
        {
            if (updateTo.Unit == fromUnit && updateTo.FromValue == fromValue)
            {
                return;
            }

            int pow = fromUnit - toUnit;
            double v = Math.Pow(1024, pow);

            updateTo.Value = Math.Round(fromValue * v, decimals);
            updateTo.Unit = toUnit;
        }




        //public static bool UpdateReadable(UnitValue updateTo, double srcSize, UnitType srcUnit = UnitType.Byte)
        //{
        //    if (updateTo.Unit == srcUnit && updateTo.Value == srcSize)
        //    {
        //        // 值是一样的，不用计算
        //        return false;
        //    }

        //    // 传递进来的字节数
        //    double bytes = 0;
        //    int pow = -1;

        //    switch (srcUnit)
        //    {
        //        case UnitType.GB: pow = 3; break;
        //        case UnitType.TB: pow = 4; break;
        //        case UnitType.KB: pow = 1; break;
        //        case UnitType.MB: pow = 2; break;
        //        case UnitType.Byte: bytes = srcSize; break;
        //        default:
        //            throw new NotImplementedException();
        //    }

        //    if (pow != -1)
        //    {
        //        double v = Math.Pow(1024, pow);
        //        bytes = srcSize * v;
        //    }

        //    UnitType newUnit;
        //    updateTo.Value = MTermUtils.ConvertToHumanReadableUnit(bytes, out newUnit);
        //    updateTo.Unit = newUnit;

        //    return true;
        //}



        /// <summary>
        /// 根据大小自动计算一个用来显示的大小单位
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ConvertToHumanReadableUnit(double bytes, out UnitType unit, int decimals = 2)
        {
            unit = UnitType.KB;

            if (bytes == 0)
            {
                return 0;
            }

            double size = bytes / 1024;

            if (size < 1024)
            {
                unit = UnitType.KB;
                return Math.Round(size, decimals);
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = UnitType.MB;
                return Math.Round(size, decimals);
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = UnitType.GB;
                return Math.Round(size, decimals);
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = UnitType.TB;
                return Math.Round(size, decimals);
            }

            throw new NotImplementedException();
        }

        public static string Unit2Suffix(UnitType unit)
        {
            switch (unit)
            {
                case UnitType.Byte: return "Byte";
                case UnitType.GB: return "GB";
                case UnitType.TB: return "TB";
                case UnitType.KB: return "KB";
                case UnitType.MB: return "MB";
                default:
                    throw new NotImplementedException();
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ssize"></param>
        ///// <param name="size"></param>
        ///// <param name="sizeUnit"></param>
        ///// <returns>是否更新了</returns>
        //public static bool UpdateUnitValue(UnitValue ssize, double size, SizeUnitsEnum sizeUnit = SizeUnitsEnum.Byte)
        //{
        //    if (sizeUnit == SizeUnitsEnum.Byte)
        //    {
        //        if (ssize.Bytes == size)
        //        {
        //            // 值是一样的，不用计算
        //            return false;
        //        }
        //    }

        //    // 传递进来的字节数
        //    UInt64 bytes = 0;
        //    int pow = -1;

        //    switch (sizeUnit)
        //    {
        //        case SizeUnitsEnum.GB: pow = 3; break;
        //        case SizeUnitsEnum.TB: pow = 4; break;
        //        case SizeUnitsEnum.KB: pow = 1; break;
        //        case SizeUnitsEnum.MB: pow = 2; break;
        //        case SizeUnitsEnum.Byte: bytes = (ulong)size; break;
        //        default:
        //            throw new NotImplementedException();
        //    }

        //    if (pow != -1)
        //    {
        //        double v = Math.Pow(1024, pow);
        //        bytes = (ulong)(size * v);
        //    }

        //    ssize.Bytes = bytes;
        //    SizeUnitsEnum newUnit;
        //    ssize.Value = MTermUtils.ConvertToHumanReadableUnit(bytes, out newUnit);
        //    ssize.Unit = newUnit;

        //    return true;
        //}

        #endregion
    }
}
