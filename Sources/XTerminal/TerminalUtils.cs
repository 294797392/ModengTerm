using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Enumerations;

namespace XTerminal
{
    public static class TerminalUtils
    {
        private static StringBuilder ContentBuilder = new StringBuilder();

        private static void BuildContentLine(VTHistoryLine historyLine, int startIndex, int endIndex, StringBuilder builder, SaveFormatEnum format)
        {
            string text = VDocumentUtils.BuildLine(historyLine.Characters);
            builder.AppendLine(text.Substring(startIndex, endIndex - startIndex + 1));
        }

        public static string BuildContent(VTHistoryLine startLine, VTHistoryLine endLine, int startCharIndex, int endCharIndex, SaveFormatEnum format)
        {
            ContentBuilder.Clear();

            // 当前只选中了一行
            if (startLine == endLine)
            {
                BuildContentLine(startLine, startCharIndex, endCharIndex, ContentBuilder, format);
                return ContentBuilder.ToString();
            }

            VTHistoryLine current = startLine;

            while (current != null)
            {
                if (current == startLine)
                {
                    BuildContentLine(current, startCharIndex, current.Characters.Count - 1, ContentBuilder, format);
                }
                else if (current == endLine)
                {
                    BuildContentLine(current, 0, endCharIndex, ContentBuilder, format);
                    break;
                }
                else
                {
                    BuildContentLine(current, 0, current.Characters.Count - 1, ContentBuilder, format);
                }

                current = current.NextLine;
            }

            if (format == SaveFormatEnum.HtmlFormat)
            {
                // TODO：加上HTML的头部和尾部标签，和一些脚本
            }

            return ContentBuilder.ToString();
        }
    }
}
