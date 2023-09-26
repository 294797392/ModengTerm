using DotNEToolkit.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModengPack
{
    class Program
    {
        static string GetKey(List<string> filePaths)
        {
            List<byte> bytes = new List<byte>();
            filePaths.ForEach((filePath) => 
            {
                bytes.AddRange(File.ReadAllBytes(filePath));
            });

            // 所有文件的crc16
            byte[] crc16 = CRC.CRC16(bytes.ToArray());

            // 所有文件的md5
            MD5 md5Alg = MD5.Create();
            byte[] md5 = md5Alg.ComputeHash(bytes.ToArray());

            throw new NotImplementedException();
        }

        static void Main(string[] args)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            List<string> encryptList = new List<string>();

            encryptList.AddRange(Directory.EnumerateFiles(exePath, "*.exe"));
            encryptList.AddRange(Directory.EnumerateFiles(exePath, "*.dll"));

            byte[] exeBytes = File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName);

            // 读取PE头，加密完后再添加到加密后的数据前面。模拟一个假的dll
            byte[] peHeader = new byte[20];
            Buffer.BlockCopy(exeBytes, 0, peHeader, 0, peHeader.Length);

            // 计算密钥


            foreach (string filePath in encryptList)
            {


                Console.WriteLine(filePath);
            }

            Console.ReadLine();
        }
    }
}
