using log4net.Appender;
using ModengTerm.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm
{
    public static class ClientUtils
    {
        /// <summary>
        /// 根据大小自动计算一个用来显示的大小单位
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ConvertToHumanReadableUnit(double bytes, out SizeUnitsEnum unit, int decimals = 2)
        {
            unit = SizeUnitsEnum.KB;

            if (bytes == 0)
            {
                return 0;
            }

            double size = bytes / 1024;

            if (size < 1024)
            {
                unit = SizeUnitsEnum.KB;
                return size;
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = SizeUnitsEnum.MB;
                return Math.Round(size, decimals);
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = SizeUnitsEnum.GB;
                return Math.Round(size, decimals);
            }

            size = size / 1024;
            if (size < 1024)
            {
                unit = SizeUnitsEnum.TB;
                return Math.Round(size, decimals);
            }

            throw new NotImplementedException();
        }

        public static string Unit2Suffix(SizeUnitsEnum unit)
        {
            switch (unit)
            {
                case SizeUnitsEnum.GB: return "GB";
                case SizeUnitsEnum.TB: return "TB";
                case SizeUnitsEnum.KB: return "KB";
                case SizeUnitsEnum.MB: return "MB";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssize"></param>
        /// <param name="size"></param>
        /// <param name="sizeUnit"></param>
        /// <returns>是否更新了</returns>
        public static bool UpdateUnitValue(UnitValue ssize, double size, SizeUnitsEnum sizeUnit = SizeUnitsEnum.Byte)
        {
            if (sizeUnit == SizeUnitsEnum.Byte)
            {
                if (ssize.Bytes == size)
                {
                    // 值是一样的，不用计算
                    return false;
                }
            }

            // 传递进来的字节数
            UInt64 bytes = 0;
            int pow = -1;

            switch (sizeUnit)
            {
                case SizeUnitsEnum.GB: pow = 3; break;
                case SizeUnitsEnum.TB: pow = 4; break;
                case SizeUnitsEnum.KB: pow = 1; break;
                case SizeUnitsEnum.MB: pow = 2; break;
                case SizeUnitsEnum.Byte: bytes = (ulong)size; break;
                default:
                    throw new NotImplementedException();
            }

            if (pow != -1)
            {
                double v = Math.Pow(1024, pow);
                bytes = (ulong)(size * v);
            }

            ssize.Bytes = bytes;
            SizeUnitsEnum newUnit;
            ssize.Value = ClientUtils.ConvertToHumanReadableUnit(bytes, out newUnit);
            ssize.Unit = newUnit;

            return true;
        }
    }
}
