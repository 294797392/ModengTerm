using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 把一个高四位和一个低四位的二进制数组合成一个ascii码
        /// </summary>
        /// <param name="high_bit">十进制表示的高四位</param>
        /// <param name="low_bit">十进制表示的低四位</param>
        /// <returns></returns>
        public static byte BitCombinations(int high_bit, int low_bit)
        {
            string high = Convert.ToString(high_bit, 2).PadLeft(4, '0');
            string low = Convert.ToString(low_bit, 2).PadLeft(4, '0');
            return Convert.ToByte(string.Format("{0}{1}", high, low), 2);
        }

        App()
        {
            byte[] buffer = Encoding.UTF8.GetBytes("黑");
            bool flag1= ICare.Utility.Misc.MiscUtility.GetBit(buffer[0], 5);
            bool flag2 = ICare.Utility.Misc.MiscUtility.GetBit(buffer[0], 6);
            bool flag3 = ICare.Utility.Misc.MiscUtility.GetBit(buffer[0], 7);
            string text = Encoding.UTF8.GetString(new byte[] { 229, 147 });

            //MessageBox.Show(((int)';').ToString());

            string log4netPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml");
            if (File.Exists(log4netPath))
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netPath));

                //MessageBox.Show(Convert.ToInt32("10011011", 2).ToString());
                //MessageBox.Show(((int)'A').ToString());
            }
        }
    }
}