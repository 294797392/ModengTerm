using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using System;
using System.Collections.Generic;
using System.Text;
using XTerminal.Document;
using XTerminal.Parser;
using System.Linq;

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

        public static VTextAttributes VTAction2TextAttribute(VTActions actions, out bool enabled)
        {
            enabled = true;

            switch (actions)
            {
                case VTActions.Bold: return VTextAttributes.Bold;
                case VTActions.BoldUnset: enabled = false; return VTextAttributes.Bold;
                case VTActions.Underline: return VTextAttributes.Underline;
                case VTActions.UnderlineUnset: enabled = false; return VTextAttributes.Underline;
                case VTActions.Italics: return VTextAttributes.Italics;
                case VTActions.ItalicsUnset: enabled = false; return VTextAttributes.Italics;
                case VTActions.DoublyUnderlined: return VTextAttributes.DoublyUnderlined;
                case VTActions.DoublyUnderlinedUnset: enabled = false; return VTextAttributes.DoublyUnderlined;
                case VTActions.Background: return VTextAttributes.Background;
                case VTActions.BackgroundUnset: enabled = false; return VTextAttributes.Background;
                case VTActions.Foreground: return VTextAttributes.Foreground;
                case VTActions.ForegroundUnset: enabled = false; return VTextAttributes.Foreground;

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

        public static List<VTextAttributeState> CreateTextAttributeStates()
        {
            List<VTextAttributeState> attributeStates = new List<VTextAttributeState>();

            IEnumerable<VTextAttributes> attributes = Enum.GetValues(typeof(VTextAttributes)).Cast<VTextAttributes>().OrderBy(v => v);

            foreach (VTextAttributes attribute in attributes)
            {
                VTextAttributeState attributeState = new VTextAttributeState(attribute);

                attributeStates.Add(attributeState);
            }

            return attributeStates;
        }

        public static void CopyAttributeState(List<VTextAttributeState> copyFroms, List<VTextAttributeState> copyTos)
        {
            for (int i = 0; i < copyFroms.Count; i++)
            {
                VTextAttributeState copyFrom = copyFroms[i];
                VTextAttributeState copyTo = copyTos[i];

                copyTo.Enabled = copyFrom.Enabled;
                copyTo.Parameter = copyFrom.Parameter;
            }
        }
    }
}

