using DotNEToolkit;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
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
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTBaseUtils");

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

        /// <summary>
        /// 判断会话类型是否是终端类型
        /// </summary>
        /// <param name="sessionType"></param>
        /// <returns></returns>
        public static bool IsTerminal(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.Tcp:
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

        //public static VTManifest GetManifest() 
        //{
        //    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");
        //    return JSONHelper.ParseFile<VTManifest>(path);
        //}

        public static System.Windows.Media.Color RgbKey2Color(string rgbKey)
        {
            VTColor vtColor = VTColor.CreateFromRgbKey(rgbKey);
            return System.Windows.Media.Color.FromRgb(vtColor.R, vtColor.G, vtColor.B);
        }

        public static string Color2RgbKey(System.Windows.Media.Color color)
        {
            return string.Format("{0},{1},{2},{3}", color.R, color.G, color.B, color.A);
        }

        public static string GetSessionTypeName(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.Localhost: return "local";
                case SessionTypeEnum.SerialPort: return "serial";
                case SessionTypeEnum.Tcp: return "tcp";
                case SessionTypeEnum.SFTP: return "sftp";
                case SessionTypeEnum.SSH: return "ssh";
                default:
                    throw new NotImplementedException();
            }
        }

        #region UnitValue

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

        #endregion
    }
}
