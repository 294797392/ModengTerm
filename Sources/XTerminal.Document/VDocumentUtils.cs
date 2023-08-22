using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Parser;

namespace XTerminal.Document
{
    public static class VDocumentUtils
    {
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
                case VTActions.DefaultBackground: unset = true; return VTextDecorations.Background;

                case VTActions.Foreground: return VTextDecorations.Foreground;
                case VTActions.DefaultForeground: unset = true; return VTextDecorations.Foreground;

                default:
                    throw new NotImplementedException();
            }
        }

        public static void GetSegement(string text, int characterIndex, out int startIndex, out int endIndex)
        {
            startIndex = 0;
            endIndex = text.Length - 1;

            if (!char.IsLetterOrDigit(text[characterIndex]))
            {
                startIndex = characterIndex;
                endIndex = characterIndex;
                return;
            }

            for (int i = characterIndex; i >= 0; i--)
            {
                char c = text[i];
                if (char.IsLetterOrDigit(c))
                {
                    continue;
                }

                startIndex = i + 1;
                break;
            }

            for (int i = characterIndex + 1; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetterOrDigit(c))
                {
                    continue;
                }

                endIndex = i - 1;
                break;
            }
        }

        /// <summary>
        /// 把一组VTCharacter转换成一个文本行字符串
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string BuildLine(IEnumerable<VTCharacter> characters)
        {
            string text = string.Empty;

            foreach (VTCharacter character in characters)
            {
                text += character.Character;
            }

            return text;
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
