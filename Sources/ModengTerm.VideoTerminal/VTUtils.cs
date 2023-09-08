using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using System;
using System.Collections.Generic;
using System.Text;
using XTerminal.Document;
using XTerminal.Parser;

namespace ModengTerm.Terminal
{
    internal static class VTUtils
    {
        private static readonly StringBuilder Builder = new StringBuilder();

        private static void BuildLine(VTHistoryLine historyLine, int startIndex, int endIndex, StringBuilder builder, LogFileTypeEnum fileType, LoggerFilter filter = null)
        {
            string text = VDocumentUtils.BuildLine(historyLine.Characters);

            if (filter != null)
            {
                if (!filter.Filter(text))
                {
                    return;
                }
            }

            builder.AppendLine(text.Substring(startIndex, endIndex - startIndex + 1));
        }

        public static string BuildDocument(VTHistoryLine startLine, VTHistoryLine endLine, int startCharIndex, int endCharIndex, LogFileTypeEnum fileType, LoggerFilter filter = null)
        {
            Builder.Clear();

            BuildDocument(startLine, endLine, startCharIndex, endCharIndex, Builder, fileType, filter);

            return Builder.ToString();
        }

        public static void BuildDocument(VTHistoryLine startLine, VTHistoryLine endLine, int startCharIndex, int endCharIndex, StringBuilder builder, LogFileTypeEnum fileType, LoggerFilter filter = null)
        {
            // 当前只选中了一行
            if (startLine == endLine)
            {
                BuildLine(startLine, startCharIndex, endCharIndex, builder, fileType, filter);
                return;
            }

            VTHistoryLine current = startLine;

            while (current != null)
            {
                if (current == startLine)
                {
                    BuildLine(current, startCharIndex, current.Characters.Count - 1, builder, fileType, filter);
                }
                else if (current == endLine)
                {
                    BuildLine(current, 0, endCharIndex, builder, fileType, filter);
                    break;
                }
                else
                {
                    BuildLine(current, 0, current.Characters.Count - 1, builder, fileType, filter);
                }

                current = current.NextLine;
            }
        }


        public static VTextDecorations VTAction2TextDecoration(VTActions actions, out bool unset)
        {
            unset = false;

            switch (actions)
            {
                case VTActions.Bold: return VTextDecorations.Bold;
                case VTActions.BoldUnset: unset = true; return VTextDecorations.Bold;
                case VTActions.Underline: return VTextDecorations.Underline;
                case VTActions.UnderlineUnset: unset = true; return VTextDecorations.Underline;
                case VTActions.Italics: return VTextDecorations.Italics;
                case VTActions.ItalicsUnset: unset = true; return VTextDecorations.Italics;
                case VTActions.DoublyUnderlined: return VTextDecorations.DoublyUnderlined;
                case VTActions.DoublyUnderlinedUnset: unset = true; return VTextDecorations.DoublyUnderlined;
                case VTActions.Background: return VTextDecorations.Background;
                case VTActions.BackgroundUnset: unset = true; return VTextDecorations.Background;
                case VTActions.Foreground: return VTextDecorations.Foreground;
                case VTActions.ForegroundUnset: unset = true; return VTextDecorations.Foreground;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取字符列表一共占多少列
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static int GetColumns(IEnumerable<VTCharacter> characters)
        {
            int columns = 0;

            foreach (VTCharacter character in characters)
            {
                columns += character.ColumnSize;
            }

            return columns;
        }
    }
}

