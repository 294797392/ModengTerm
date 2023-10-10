using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 收藏夹功能
    /// </summary>
    public class VTFavorites
    {
        /// <summary>
        /// 收藏的段落列表
        /// </summary>
        private List<VTParagraph> paragraphs;

        public VTFavorites()
        {
            this.paragraphs = new List<VTParagraph>();
        }


    }
}
