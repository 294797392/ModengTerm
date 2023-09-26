using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using System;
using System.Collections.Generic;
using System.Text;
using XTerminal.Document;
using XTerminal.Parser;
using System.Linq;
using XTerminal.Base.Definitions;
using System.Reflection;
using System.Windows.Media;

namespace ModengTerm.Terminal
{
    public class CreateContentParameter
    {
        public List<List<VTCharacter>> CharactersList { get; set; }

        public int StartCharacterIndex { get; set; }

        public int EndCharacterIndex { get; set; }

        public LogFileTypeEnum ContentType { get; set; }

        public Dictionary<string, string> ColorTable { get; set; }

        public string SessionName { get; set; }

        /// <summary>
        /// 终端背景颜色
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// 终端前景色（文本默认颜色）
        /// </summary>
        public string Foreground { get; set; }

        public string FontFamily { get; set; }

        public double FontSize { get; set; }
    }

    public static class VTUtils
    {
        private delegate void CreateLineDelegate(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, CreateContentParameter parameter);
        private const string HtmlTemplate =
            "<html>" +
            "<head>" +
            "<title>{0}</title>" +
            "</head>" +
            "<body style='background-color:{2};font-size:{3}px;font-family:{4};color:{5};'>{1}</body>" +
            "</html>";

        /// <summary>
        /// VTColor转成HtmlColor
        /// </summary>
        /// <param name="vtc"></param>
        /// <param name="colorTable"></param>
        /// <returns></returns>
        private static string GetHtmlColor(VTColor vtc, Dictionary<string, string> colorTable)
        {
            RgbColor rgbColor = null;

            if (vtc is NamedColor)
            {
                string rgbKey = colorTable[vtc.Name];
                rgbColor = VTColor.CreateFromRgbKey(rgbKey) as RgbColor;
            }
            else if (vtc is RgbColor)
            {
                rgbColor = vtc as RgbColor;
            }

            return rgbColor.Html;
        }

        private static void CreatePlainText(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, CreateContentParameter parameter)
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

        private static void CreateHtml(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, CreateContentParameter parameter)
        {
            if (characters.Count == 0)
            {
                builder.AppendLine("<br>");
                return;
            }

            Dictionary<string, string> colorTable = parameter.ColorTable;

            for (int i = 0; i < count; i++)
            {
                VTCharacter character = characters[i];

                builder.Append("<span style='");

                foreach (VTextAttributeState attribute in character.AttributeList)
                {
                    if (!attribute.Enabled)
                    {
                        continue;
                    }

                    switch (attribute.Attribute)
                    {
                        case VTextAttributes.Background:
                            {
                                VTColor vtc = attribute.Parameter as VTColor;
                                string color = GetHtmlColor(vtc, colorTable);
                                builder.AppendFormat("background-color:{0};", color);
                                break;
                            }

                        case VTextAttributes.Foreground:
                            {
                                VTColor vtc = attribute.Parameter as VTColor;
                                string color = GetHtmlColor(vtc, colorTable);
                                builder.AppendFormat("color:{0};", color);
                                break;
                            }

                        case VTextAttributes.Underline:
                            {
                                builder.Append("text-decoration:underline;");
                                break;
                            }

                        default:
                            break;
                    }
                }

                builder.AppendFormat("'>{0}</span>", character.Character == ' ' ? "&nbsp" : character.Character.ToString());
            }

            builder.AppendLine("</br>");
        }

        private static CreateLineDelegate GetCreateLineDelegate(LogFileTypeEnum fileType)
        {
            switch (fileType)
            {
                case LogFileTypeEnum.HTML: return CreateHtml;
                case LogFileTypeEnum.PlainText: return CreatePlainText;
                default:
                    throw new NotImplementedException();
            }
        }





        public static string CreateContent(CreateContentParameter parameter)
        {
            List<List<VTCharacter>> charactersList = parameter.CharactersList;
            LogFileTypeEnum fileType = parameter.ContentType;
            int startCharIndex = parameter.StartCharacterIndex;
            int endCharIndex = parameter.EndCharacterIndex;

            CreateLineDelegate createLine = VTUtils.GetCreateLineDelegate(fileType);
            StringBuilder builder = new StringBuilder();

            if (charactersList.Count == 1)
            {
                // 只有一行
                createLine(charactersList[0], builder, startCharIndex, endCharIndex - startCharIndex + 1, parameter);
            }
            else
            {
                // 第一行
                List<VTCharacter> first = charactersList.FirstOrDefault();
                createLine(first, builder, startCharIndex, first.Count - startCharIndex, parameter);

                // 中间的行
                for (int i = 1; i < charactersList.Count - 1; i++)
                {
                    List<VTCharacter> characters = charactersList[i];
                    createLine(characters, builder, 0, characters.Count, parameter);
                }

                // 最后一行
                List<VTCharacter> last = charactersList.LastOrDefault();
                createLine(last, builder, 0, endCharIndex + 1, parameter);
            }

            if (fileType == LogFileTypeEnum.HTML)
            {
                string htmlBackground = VTUtils.GetHtmlColor(VTColor.CreateFromRgbKey(parameter.Background), parameter.ColorTable);
                string htmlForeground = VTUtils.GetHtmlColor(VTColor.CreateFromRgbKey(parameter.Foreground), parameter.ColorTable);
                return string.Format(HtmlTemplate, parameter.SessionName, builder.ToString(), htmlBackground, parameter.FontSize, parameter.FontFamily, htmlForeground);
            }

            return builder.ToString();
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
                case VTActions.ReverseVideo: return VTextAttributes.Background;
                case VTActions.ReverseVideoUnset: enabled = false; return VTextAttributes.Background;

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
        public static VTFormattedText CreateFormattedText(List<VTCharacter> characters, int startIndex, int count)
        {
            VTFormattedText formattedText = new VTFormattedText();

            for (int i = 0; i < count; i++)
            {
                VTCharacter character = characters[startIndex + i];

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
                                attributeState.Attribute == VTextAttributes.Foreground ||
                                attributeState.Attribute == VTextAttributes.FontFamily)
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

        public static VTFormattedText CreateFormattedText(List<VTCharacter> characters)
        {
            return VTUtils.CreateFormattedText(characters, 0, characters.Count);
        }

        /// <summary>
        /// 创建裸文本
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string CreatePlainText(List<VTCharacter> characters)
        {
            return CreatePlainText(characters, 0, characters.Count);
        }

        public static string CreatePlainText(List<VTCharacter> characters, int startIndex)
        {
            int count = characters.Count - startIndex;
            return CreatePlainText(characters, startIndex, count);
        }

        public static string CreatePlainText(List<VTCharacter> characters, int startIndex, int count)
        {
            if (characters.Count == 0)
            {
                return string.Empty;
            }

            string text = string.Empty;

            for (int i = 0; i < count; i++)
            {
                text += characters[startIndex + i].Character;
            }

            return text;
        }
    }
}
