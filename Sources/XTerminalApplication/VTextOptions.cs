using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminalParser;

namespace XTerminalDevice
{
    public class VTextOptions
    {
        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 默认字体颜色
        /// </summary>
        public VTForeground Foreground { get; set; }

        public VTextOptions()
        {
            this.FontSize = DefaultValues.FontSize;
            this.Foreground = DefaultValues.Foreground;
        }
    }
}
