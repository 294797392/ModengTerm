using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Enumerations;

namespace XTerminal
{
    public static class XTermUtils
    {
        private static StringBuilder DocumentBuilder = new StringBuilder();

        private static void BuildLine(VTHistoryLine historyLine, int startIndex, int endIndex, StringBuilder builder, SaveFormatEnum format)
        {
            string text = VDocumentUtils.BuildLine(historyLine.Characters);
            builder.AppendLine(text.Substring(startIndex, endIndex - startIndex + 1));
        }

        public static string BuildDocument(VTHistoryLine startLine, VTHistoryLine endLine, int startCharIndex, int endCharIndex, SaveFormatEnum format)
        {
            DocumentBuilder.Clear();

            // 当前只选中了一行
            if (startLine == endLine)
            {
                BuildLine(startLine, startCharIndex, endCharIndex, DocumentBuilder, format);
                return DocumentBuilder.ToString();
            }

            VTHistoryLine current = startLine;

            while (current != null)
            {
                if (current == startLine)
                {
                    BuildLine(current, startCharIndex, current.Characters.Count - 1, DocumentBuilder, format);
                }
                else if (current == endLine)
                {
                    BuildLine(current, 0, endCharIndex, DocumentBuilder, format);
                    break;
                }
                else
                {
                    BuildLine(current, 0, current.Characters.Count - 1, DocumentBuilder, format);
                }

                current = current.NextLine;
            }

            if (format == SaveFormatEnum.HtmlFormat)
            {
                // TODO：加上HTML的头部和尾部标签，和一些脚本
            }

            return DocumentBuilder.ToString();
        }
    }
}
