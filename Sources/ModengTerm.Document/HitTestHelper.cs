using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 提供做命中测试的帮助函数
    /// </summary>
    public static class HitTestHelper
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("HitTestHelper");

        /// <summary>
        /// 根据Y坐标找到包含该Y坐标的历史行
        /// </summary>
        /// <param name="document">要做命中测试的文档</param>
        /// <param name="cursorY">鼠标坐标Y轴数值</param>
        /// <param name="logicalRow">所命中的行是第几行</param>
        /// <returns>找到了就返回找到的历史行，没找到返回null</returns>
        public static VTextLine HitTestVTextLine(VTDocument document, double cursorY, out int logicalRow)
        {
            logicalRow = 0;

            // 当前行的Y偏移量
            VTextLine current = document.FirstLine;

            while (current != null)
            {
                // 当前行的边界框信息
                VTRect lineBounds = current.Bounds;

                // 判断鼠标的Y坐标是否在边界框内
                if (lineBounds.Top <= cursorY && lineBounds.Bottom >= cursorY)
                {
                    // 此时说明找到了鼠标所在行
                    return current;
                }

                logicalRow++;
                current = current.NextLine;
            }

            return null;
        }

        /// <summary>
        /// 根据X坐标找到该坐标下的字符的边界框信息
        /// 因为字符与字符之间有间隙，如果在测量的时候cursorX刚好在两个字符的间隙里，那么此时返回false
        /// </summary>
        /// <param name="textLine">要做字符命中测试的行</param>
        /// <param name="cursorX">要做命中测试的X偏移量</param>
        /// <param name="charIndexHit">被命中的字符的索引</param>
        /// <param name="characterHitRange">被命中的字符的边界框信息</param>
        /// <param name="columnIndexHit">命中的列索引</param>
        /// <returns>命中成功返回true，失败返回false</returns>
        public static void HitTestVTCharacter(VTextLine textLine, double cursorX, out int charIndexHit, out VTextRange characterHitRange, out int columnIndexHit)
        {
            // 命中测试流程是一个一个字符做边界框的判断
            // 先测量第一个字符的边界框，然后判断xPos是否在该边界框里
            // 然后测量第一个和第二个字符的边界框，然后再判断
            // ...以此类推，直到命中为止

            characterHitRange = VTextRange.Empty;

            // 这种计算方式只适用于等宽字体
            columnIndexHit = (int)Math.Min(Math.Floor(cursorX / textLine.Typeface.Width), textLine.OwnerDocument.ViewportColumn - 1);

            //logger.InfoFormat("columnIndexHit = {0}, cursorX = {1}, charWidth = {2}", columnIndexHit, cursorX, textLine.Typeface.Width);

            charIndexHit = textLine.FindCharacterIndex(columnIndexHit);
            if (charIndexHit >= 0)
            {
                characterHitRange = textLine.MeasureCharacter(charIndexHit);
            }
        }
    }
}


