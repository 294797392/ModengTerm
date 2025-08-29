using DotNEToolkit;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using Newtonsoft.Json;
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
                case SessionTypeEnum.Ssh:
                case SessionTypeEnum.LocalConsole:
                    {
                        return true;
                    }

                case SessionTypeEnum.Sftp:
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

        /// <summary>
        /// 检测当前操作系统是否是Win10或更高版本
        /// </summary>
        /// <returns></returns>
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

        public static string GetSessionTypeName(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.LocalConsole: return "local";
                case SessionTypeEnum.SerialPort: return "serial";
                case SessionTypeEnum.Tcp: return "tcp";
                case SessionTypeEnum.Sftp: return "sftp";
                case SessionTypeEnum.Ssh: return "ssh";
                default:
                    throw new NotImplementedException();
            }
        }

        public static T GetOptions<T>(this Dictionary<string, object> options, string key)
        {
            object value = options[key];

            Type type = typeof(T);

            if (type == typeof(string))
            {
                return (T)options[key];
            }

            if (type.IsClass)
            {
                string svalue = options[key].ToString();
                return JsonConvert.DeserializeObject<T>(svalue);
            }

            return options.GetValue<T>(key);
        }

        public static string ReplaceVariable(string source)
        {
            foreach (KeyValuePair<string, string> kv in VTBaseConsts.VariableName2Value)
            {
                if (source.Contains(kv.Key))
                {
                    return source.Replace(kv.Key, kv.Value);
                }
            }

            return source;
        }
    }
}
