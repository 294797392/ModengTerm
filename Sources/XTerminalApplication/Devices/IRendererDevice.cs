using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalDevice.Interface
{
    public interface IRendererDevice
    {
        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlocks"></param>
        void DrawText(List<VTextBlock> textBlocks);

        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlock"></param>
        void DrawText(VTextBlock textBlock);

        /// <summary>
        /// 删除文本块
        /// </summary>
        /// <param name="textBlocks">要删除的文本块列表</param>
        void DeleteText(List<VTextBlock> textBlocks);

        /// <summary>
        /// 删除文本块
        /// </summary>
        /// <param name="textBlock"></param>
        void DeleteText(VTextBlock textBlock);

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        VTextBlockMetrics MeasureText(VTextBlock textBlock);
    }
}
