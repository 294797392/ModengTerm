using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 提供一些文本选择操作的帮助函数
    /// </summary>
    public static class VTextSelectionHelper
    {
        private static readonly VTRect EmptyRect = new VTRect();

        /// <summary>
        /// 根据Y坐标找到包含该Y坐标的历史行
        /// </summary>
        /// <param name="firstLine">要从第几行开始往下找</param>
        /// <param name="yPos">Y坐标偏移量</param>
        /// <param name="offsetY">该行相对于Surface的Y偏移量</param>
        /// <returns>找到了就返回找到的历史行，没找到返回null</returns>
        public static VTHistoryLine HitTestVTextLine(VTHistoryLine firstLine, double yPos, out double offsetY)
        {
            // 当前行的Y偏移量
            offsetY = 0;
            VTHistoryLine historyLine = firstLine;

            while (historyLine != null)
            {
                // 当前行的边界框信息
                VTRect lineBounds = new VTRect(0, offsetY, historyLine.Width, historyLine.Height);

                // 判断鼠标的Y坐标是否在边界框内
                if (lineBounds.Top <= yPos && lineBounds.Bottom >= yPos)
                {
                    // 此时说明找到了鼠标所在行
                    return historyLine;
                }

                // 下一行的Y坐标
                offsetY += lineBounds.Height;

                historyLine = historyLine.NextLine;
            }

            return null;
        }

        /// <summary>
        /// 根据X坐标找到该坐标下的字符的边界框信息
        /// </summary>
        /// <param name="surface">渲染该字符的Surface，用来测量字符</param>
        /// <param name="historyLine">要做字符命中测试的行</param>
        /// <param name="xPos">要做命中测试的X偏移量</param>
        /// <param name="index">被命中的字符的索引</param>
        /// <param name="bounds">被命中的字符的边界框信息</param>
        /// <returns>命中成功返回true，失败返回false</returns>
        public static bool HitTestVTCharacter(ITerminalSurface surface, VTHistoryLine historyLine, double xPos, out int index, out VTRect bounds)
        {
            // 命中测试流程是一个一个字符做边界框的判断
            // 先测量第一个字符的边界框，然后判断xPos是否在该边界框里
            // 然后测量第一个和第二个字符的边界框，然后再判断
            // ...以此类推，直到命中为止

            index = -1;
            bounds = EmptyRect;

            string text = historyLine.Text;
            for (int i = 0; i < text.Length; i++)
            {
                VTRect characterBounds = surface.MeasureCharacter(historyLine, i);

                if (characterBounds.Left <= xPos && characterBounds.Right >= xPos)
                {
                    // 鼠标命中了字符，使用命中的字符的边界框
                    index = i;
                    bounds = characterBounds;
                    return true;
                }
            }

            return false;
        }
    }
}