﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using DotNEToolkit;
using ModengTerm.Document.Enumerations;

namespace ModengTerm.Document.Utility
{
    public delegate void CreateLineDelegate(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, bool lastLine);

    /// <summary>
    /// 提供终端的工具函数
    /// </summary>
    public static class VTUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTUtils");

        /// <summary>
        /// 所有的嵌入的资源名字
        /// </summary>
        private static List<string> AllResourceNames = new List<string>();
        private static Assembly ResourceAssembly = null;

        private const string HtmlTemplate =
            "<html>" +
            "<head>" +
            "<title>{0}</title>" +
            "</head>" +
            "<body style='background-color:{2};font-size:{3}px;font-family:{4};color:{5};'>{1}</body>" +
            "</html>";

        private static readonly List<VTextAttributes> AllTextAttributes = Enum.GetValues(typeof(VTextAttributes)).Cast<VTextAttributes>().ToList();
        private static readonly Dictionary<string, GifMetadata> GifMetadataMap = new Dictionary<string, GifMetadata>();

        static VTUtils()
        {
        }

        internal static void CreatePlainText(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, bool lastLine)
        {
            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            //if (filter != null)
            //{
            //    if (!filter.Filter(text))
            //    {
            //        return;
            //    }
            //}

            if (lastLine)
            {
                builder.Append(text);
            }
            else
            {
                builder.AppendLine(text);
            }
        }

        internal static void CreateHtml(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, bool lastLine)
        {
            if (characters.Count == 0)
            {
                builder.AppendLine("<br>");
                return;
            }

            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            //if (filter != null)
            //{
            //    if (!filter.Filter(text))
            //    {
            //        return;
            //    }
            //}

            for (int i = 0; i < count; i++)
            {
                VTCharacter character = characters[startIndex + i];

                builder.Append("<span style='");

                if (VTUtils.GetTextAttribute(VTextAttributes.Background, character.Attribute))
                {
                    VTColor vtc = character.Background;
                    string color = vtc.Html;
                    builder.AppendFormat("background-color:{0};", color);
                }

                if (VTUtils.GetTextAttribute(VTextAttributes.Foreground, character.Attribute))
                {
                    VTColor vtc = character.Foreground;
                    string color = vtc.Html;
                    builder.AppendFormat("color:{0};", color);
                }

                if (VTUtils.GetTextAttribute(VTextAttributes.Underline, character.Attribute))
                {
                    builder.Append("text-decoration:underline;");
                }

                builder.AppendFormat("'>{0}</span>", character.Character == ' ' ? "&nbsp" : character.Character.ToString());
            }

            builder.AppendLine("</br>");
        }

        public static CreateLineDelegate GetCreateLineDelegate(ParagraphFormatEnum fileType)
        {
            switch (fileType)
            {
                case ParagraphFormatEnum.HTML: return CreateHtml;
                case ParagraphFormatEnum.PlainText: return CreatePlainText;
                default:
                    throw new NotImplementedException();
            }
        }

        private static object GetAttributeParameter(VTextAttributes textAttributes, VTCharacter character)
        {
            switch (textAttributes)
            {
                case VTextAttributes.Background: return character.Background;
                case VTextAttributes.Foreground: return character.Foreground;
                default:
                    return null;
            }
        }

        private static Stream GetWallpaperStream(string uri)
        {
            if (ResourceAssembly == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AllResourceNames = assembly.GetManifestResourceNames().ToList();
                ResourceAssembly = assembly;
            }

            string resourceName = AllResourceNames.FirstOrDefault(v => v.Contains(uri));
            if (string.IsNullOrEmpty(resourceName))
            {
                logger.ErrorFormat("GetWallpaperMetadata失败, 资源不存在, uri = {0}", uri);
                return null;
            }

            return ResourceAssembly.GetManifestResourceStream(resourceName);
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

        /// <summary>
        /// 根据列索引获取该列对应的字符的索引
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="column"></param>
        /// <returns>如果不存在则返回-1</returns>
        public static int GetCharacterIndex(IEnumerable<VTCharacter> characters, int column)
        {
            int characterIndex = 0;
            int columns = 0;

            foreach (VTCharacter character in characters)
            {
                if (columns >= column)
                {
                    return characterIndex;
                }

                columns += character.ColumnSize;

                characterIndex++;
            }

            return -1;
        }

        /// <summary>
        /// 创建带有样式的文本
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static VTFormattedText CreateFormattedText(List<VTCharacter> characters, int startIndex, int count)
        {
            VTFormattedText formattedText = new VTFormattedText();

            for (int i = 0; i < count; i++)
            {
                VTCharacter character = characters[startIndex + i];

                formattedText.Text += character.Character;

                foreach (VTextAttributes textAttribute in AllTextAttributes)
                {
                    VTextAttribute attribute = formattedText.Attributes.FirstOrDefault(v => v.Attribute == textAttribute && !v.Closed);

                    if (VTUtils.GetTextAttribute(textAttribute, character.Attribute))
                    {
                        object parameter = GetAttributeParameter(textAttribute, character);

                        // 启用状态
                        if (attribute == null)
                        {
                            attribute = new VTextAttribute()
                            {
                                Attribute = textAttribute,
                                StartIndex = i,
                                Parameter = parameter
                            };
                            formattedText.Attributes.Add(attribute);
                        }
                        else
                        {
                            // 颜色比较特殊，有可能连续多次设置不同的颜色
                            if (textAttribute == VTextAttributes.Background ||
                                textAttribute == VTextAttributes.Foreground)
                            {
                                // 如果设置的是颜色的话，并且当前字符的颜色和最后一次设置的颜色不一样，那么要先关闭最后一次设置的颜色
                                // attribute是最后一次设置的颜色，attributeState是当前字符的颜色

                                //object parameter = GetAttributeParameter(textAttribute, character);

                                if (attribute.Parameter != parameter)
                                {
                                    attribute.Closed = true;

                                    // 关闭后创建一个新的Attribute
                                    attribute = new VTextAttribute()
                                    {
                                        Attribute = textAttribute,
                                        StartIndex = i,
                                        Parameter = parameter
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
                //if (startIndex + i >= characters.Count)
                //{
                //    logger.FatalFormat("{0}, {1},{2}", characters.Count, startIndex, count);
                //    break;
                //}
                //if (startIndex + i < 0) 
                //{
                //    logger.FatalFormat("小于0, {0}, {1},{2}", characters.Count, startIndex, count);
                //    break;
                //}

                text += characters[startIndex + i].Character;
            }

            return text;
        }


        /// <summary>
        /// 按位设置某个字符的某个属性
        /// </summary>
        /// <param name="textAttributes"></param>
        /// <param name="enable"></param>
        /// <param name="attribute"></param>
        public static void SetTextAttribute(VTextAttributes textAttributes, bool enable, ref int attribute)
        {
            switch (textAttributes)
            {
                case VTextAttributes.Background:
                    {
                        attribute = enable ? attribute |= 2 : attribute &= (~2);
                        break;
                    }

                case VTextAttributes.Bold:
                    {
                        attribute = enable ? attribute |= 32 : attribute &= (~32);
                        break;
                    }

                case VTextAttributes.DoublyUnderlined:
                    {
                        attribute = enable ? attribute |= 4 : attribute &= (~4);
                        break;
                    }

                case VTextAttributes.Foreground:
                    {
                        attribute = enable ? attribute |= 1 : attribute &= (~1);
                        break;
                    }

                case VTextAttributes.Italics:
                    {
                        attribute = enable ? attribute |= 8 : attribute &= (~8);
                        break;
                    }

                case VTextAttributes.Underline:
                    {
                        attribute = enable ? attribute |= 16 : attribute &= (~16);
                        break;
                    }

                case VTextAttributes.Faint:
                    {
                        attribute = enable ? attribute |= 64 : attribute &= (~64);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取某个字符属性是否被设置了
        /// </summary>
        /// <param name="textAttribute">要获取的属性</param>
        /// <param name="attribute">按位存储的属性</param>
        /// <returns></returns>
        public static bool GetTextAttribute(VTextAttributes textAttribute, int attribute)
        {
            switch (textAttribute)
            {
                case VTextAttributes.Background:
                    {
                        return (attribute >> 1 & 1) == 1;
                    }

                case VTextAttributes.Bold:
                    {
                        return (attribute >> 5 & 1) == 1;
                    }

                case VTextAttributes.DoublyUnderlined:
                    {
                        return (attribute >> 2 & 1) == 1;
                    }

                case VTextAttributes.Foreground:
                    {
                        return (attribute & 1) == 1;
                    }

                case VTextAttributes.Italics:
                    {
                        return (attribute >> 3 & 1) == 1;
                    }

                case VTextAttributes.Underline:
                    {
                        return (attribute >> 4 & 1) == 1;
                    }

                case VTextAttributes.Faint:
                    {
                        return (attribute >> 6 & 1) == 1;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 根据当前屏幕大小计算终端的自适应大小
        /// </summary>
        /// <param name="displaySize">文档显示区域的大小</param>
        /// <param name="typeface">字体信息</param>
        /// <param name="rowSize">计算出来的终端行数</param>
        /// <param name="colSize">计算出来的终端列数</param>
        public static void CalculateAutoFitSize(VTSize displaySize, VTypeface typeface, out int rowSize, out int colSize)
        {
            // 自适应屏幕大小
            // 计算一共有多少行，和每行之间的间距是多少

            // 终端控件的初始宽度和高度，在打开Session的时候动态设置
            rowSize = (int)Math.Floor(displaySize.Height / typeface.Height);
            colSize = (int)Math.Floor(displaySize.Width / typeface.Width);

            //logger.InfoFormat("计算行数, {0}, {1}, {2}", displaySize.Height, typeface.Height, rowSize);
        }

        public static void GetSegement(List<VTCharacter> characters, int characterIndex, out int startIndex, out int count)
        {
            startIndex = 0;
            count = 0;

            // IsLetterOrDigit也可以判断是否是中文，如果是中文，那么就是true
            if (!char.IsLetterOrDigit(characters[characterIndex].Character))
            {
                startIndex = characterIndex;
                count = 1;
                return;
            }

            // 从characterIndex向左搜索，找到statIndex
            for (int i = characterIndex; i >= 0; i--)
            {
                char c = characters[i].Character;
                if (char.IsLetterOrDigit(c))
                {
                    count++;
                    continue;
                }

                startIndex = i + 1;
                break;
            }

            // 从characterIndex向右搜索，找到count
            for (int i = characterIndex + 1; i < characters.Count; i++)
            {
                char c = characters[i].Character;
                if (char.IsLetterOrDigit(c))
                {
                    count++;
                    continue;
                }

                break;
            }
        }

        /// <summary>
        /// 把字符列表根据指定的分隔符分割成一个个的单词
        /// </summary>
        /// <param name="characters">要分割的字符列表</param>
        /// <param name="splitters">分隔符列表</param>
        /// <param name="result">分割之后的单词列表</param>
        public static void Split(List<VTCharacter> characters, string[] splitters, out string[] result)
        {
            result = null;

            string text = VTUtils.CreatePlainText(characters);
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            result = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
