using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ServiceAgents.Crypto
{
    public class AESLocalSecretKeyProvider : SecretKeyProvider
    {
        public override SecretKey GetSecretKey()
        {
            string cpuId = string.Empty;
            string biosSN = string.Empty;
            string diskSN = string.Empty;

            string text = string.Format("{0}_{1}_{2}", cpuId, biosSN, diskSN);

            byte[] toHash = Encoding.UTF8.GetBytes(text);
            byte[] key = SHA256.HashData(toHash);

            return new AESecretKey()
            {
                Key = key,
            };
        }

        //// 获取CPU序列号
        //public static string GetCPUSerialNumber()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        //        string cpuSerialNumber = "";
        //        foreach (ManagementObject mo in searcher.Get())
        //        {
        //            cpuSerialNumber = mo["ProcessorId"].ToString().Trim();
        //            break;
        //        }
        //        return cpuSerialNumber;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        //// 获取主板序列号
        //public static string GetBIOSSerialNumber()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        //        string biosSerialNumber = "";
        //        foreach (ManagementObject mo in searcher.Get())
        //        {
        //            biosSerialNumber = mo.GetPropertyValue("SerialNumber").ToString().Trim();
        //            break;
        //        }
        //        return biosSerialNumber;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        //// 获取硬盘序列号
        //public static string GetHardDiskSerialNumber()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
        //        string hardDiskSerialNumber = "";
        //        foreach (ManagementObject mo in searcher.Get())
        //        {
        //            hardDiskSerialNumber = mo["SerialNumber"].ToString().Trim();
        //            break;
        //        }
        //        return hardDiskSerialNumber;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}
    }
}
