using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalBase
{
    public class VTypeface
    {
        private string hashId;

        public string HashID
        {
            get
            {
                if(string.IsNullOrEmpty(this.hashId))
                {
                    this.hashId = string.Format("{0}_{1}", this.FontFamily, this.FontWeight);
                }
                return this.hashId;
            }
        }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体粗细
        /// </summary>
        public VFontWeight FontWeight { get; set; }

    }
}
