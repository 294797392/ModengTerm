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
    public static class VTUtils
    {
        private static void BuildPlainText(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count)
        {
            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            if (string.IsNullOrEmpty(text))
            {
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine(text);
            }
        }

        public static string BuildContent(List<List<VTCharacter>> charactersList, int startCharIndex, int endCharIndex, LogFileTypeEnum fileType)
        {
            switch (fileType)
            {
                case LogFileTypeEnum.Text:
                    {
                        if (charactersList.Count == 1)
                        {
                            // 只有一行
                            return VTUtils.CreatePlainText(charactersList[0], startCharIndex, endCharIndex - startCharIndex + 1);
                        }
                        else
                        {
                            StringBuilder builder = new StringBuilder();

                            // 第一行
                            List<VTCharacter> first = charactersList.FirstOrDefault();
                            VTUtils.BuildPlainText(first, builder, startCharIndex, first.Count - startCharIndex);

                            // 中间的行
                            for (int i = 1; i < charactersList.Count - 1; i++)
                            {
                                List<VTCharacter> characters = charactersList[i];
                                VTUtils.BuildPlainText(characters, builder, 0, characters.Count);
                            }

                            // 最后一行
                            List<VTCharacter> last = charactersList.LastOrDefault();
                            VTUtils.BuildPlainText(last, builder, 0, endCharIndex + 1);

                            return builder.ToString();
                        }
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
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

        public static void CopyCharacter(List<VTCharacter> copyFroms, List<VTCharacter> copyTos)
        {
            copyTos.Clear();
            for (int i = 0; i < copyFroms.Count; i++)
            {
                VTCharacter copyFrom = copyFroms[i];
                VTCharacter character = VTCharacter.CreateNull();

                // 拷贝VTCharacter
                VTUtils.CopyAttributeState(copyFrom.AttributeList, character.AttributeList);
                character.Character = copyFrom.Character;
                character.ColumnSize = copyFrom.ColumnSize;
                character.Flags = copyFrom.Flags;

                copyTos.Add(character);
            }
        }

        /// <summary>
        /// 创建带有样式的文本
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static VTFormattedText CreateFormattedText(List<VTCharacter> characters)
        {
            VTFormattedText formattedText = new VTFormattedText();

            for (int i = 0; i < characters.Count; i++)
            {
                VTCharacter character = characters[i];

                formattedText.Text += character.Character;

                foreach (VTextAttributeState attributeState in character.AttributeList)
                {
                    VTextAttribute attribute = formattedText.Attributes.FirstOrDefault(v => v.Attribute == attributeState.Attribute && !v.Closed);

                    if (attributeState.Enabled)
                    {
                        // 启用状态
                        if (attribute == null)
                        {
                            attribute = new VTextAttribute()
                            {
                                Attribute = attributeState.Attribute,
                                StartIndex = i,
                                Parameter = attributeState.Parameter
                            };
                            formattedText.Attributes.Add(attribute);
                        }
                        else
                        {
                            // 颜色比较特殊，有可能连续多次设置不同的颜色
                            if (attributeState.Attribute == VTextAttributes.Background ||
                                attributeState.Attribute == VTextAttributes.Foreground)
                            {
                                // 如果设置的是颜色的话，并且当前字符的颜色和最后一次设置的颜色不一样，那么要先关闭最后一次设置的颜色
                                // attribute是最后一次设置的颜色，attributeState是当前字符的颜色
                                if (attribute.Parameter != attributeState.Parameter)
                                {
                                    attribute.Closed = true;

                                    // 关闭后创建一个新的Attribute
                                    attribute = new VTextAttribute()
                                    {
                                        Attribute = attributeState.Attribute,
                                        StartIndex = i,
                                        Parameter = attributeState.Parameter
                                    };
                                    formattedText.Attributes.Add(attribute);
                                }
                            }
                        }
                        attribute.Count++;
                    }
                    else
                    {
                        // 禁用状态
                        if (attribute != null)
                        {
                            attribute.Closed = true;
                        }
                    }
                }
            }

            return formattedText;
        }

        /// <summary>
        /// 创建裸文本
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string CreatePlainText(IEnumerable<VTCharacter> characters)
        {
            string text = string.Empty;

            foreach (VTCharacter character in characters)
            {
                text += character.Character;
            }

            return text;
        }

        public static string CreatePlainText(List<VTCharacter> characters, int startIndex, int count)
        {
            string text = string.Empty;

            for (int i = 0; i < count; i++)
            {
                text += characters[startIndex + i].Character;
            }

            return text;
        }

        public static string CreatePlainText(List<VTCharacter> characters, int startIndex)
        {
            int count = characters.Count - startIndex;
            return CreatePlainText(characters, startIndex, count);
        }
    }
}