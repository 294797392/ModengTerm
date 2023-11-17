using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using System;
using System.Collections.Generic;
using System.Text;
using XTerminal.Parser;
using System.Linq;
using XTerminal.Base.Definitions;
using System.Reflection;
using System.Windows.Media;
using ModengTerm.Terminal.DataModels;
using XTerminal.Base.Enumerations;
using ModengTerm.Base;
using System.IO;
using System.Windows.Media.Imaging;
using ModengTerm.Base.Enumerations;
using DotNEToolkit;
using ModengTerm.ServiceAgents.DataModels;

namespace ModengTerm.Terminal
{
    public class CreateContentParameter
    {
        public List<List<VTCharacter>> CharactersList { get; set; }

        public int StartCharacterIndex { get; set; }

        public int EndCharacterIndex { get; set; }

        public LogFileTypeEnum ContentType { get; set; }

        public string SessionName { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        public VTypeface Typeface { get; set; }
    }

    public delegate void CreateLineDelegate(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, LoggerFilter filter = null);

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
        private static readonly BitmapImage ErrorBitmapImage = null;

        static VTUtils()
        {
            ErrorBitmapImage = new BitmapImage();
            ErrorBitmapImage.BeginInit();
            ErrorBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            ErrorBitmapImage.UriSource = new Uri("pack://application:,,,/ModengTerm;component/Images/error.png");
            ErrorBitmapImage.EndInit();
        }

        internal static void CreatePlainText(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, LoggerFilter filter = null)
        {
            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            if (filter != null)
            {
                if (!filter.Filter(text))
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine(text);
            }
        }

        internal static void CreateHtml(List<VTCharacter> characters, StringBuilder builder, int startIndex, int count, LoggerFilter filter = null)
        {
            if (characters.Count == 0)
            {
                builder.AppendLine("<br>");
                return;
            }

            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            if (filter != null)
            {
                if (!filter.Filter(text))
                {
                    return;
                }
            }

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

        internal static CreateLineDelegate GetCreateLineDelegate(LogFileTypeEnum fileType)
        {
            switch (fileType)
            {
                case LogFileTypeEnum.HTML: return CreateHtml;
                case LogFileTypeEnum.PlainText: return CreatePlainText;
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
                createLine(charactersList[0], builder, startCharIndex, endCharIndex - startCharIndex + 1);
            }
            else
            {
                // 第一行
                List<VTCharacter> first = charactersList.FirstOrDefault();
                createLine(first, builder, startCharIndex, first.Count - startCharIndex);

                // 中间的行
                for (int i = 1; i < charactersList.Count - 1; i++)
                {
                    List<VTCharacter> characters = charactersList[i];
                    createLine(characters, builder, 0, characters.Count);
                }

                // 最后一行
                List<VTCharacter> last = charactersList.LastOrDefault();
                createLine(last, builder, 0, endCharIndex + 1);
            }

            if (fileType == LogFileTypeEnum.HTML)
            {
                string htmlBackground = VTColor.CreateFromRgbKey(parameter.Typeface.BackgroundColor).Html;
                string htmlForeground = VTColor.CreateFromRgbKey(parameter.Typeface.ForegroundColor).Html;
                return string.Format(HtmlTemplate, parameter.SessionName, builder.ToString(), htmlBackground, 
                    parameter.Typeface.FontSize, parameter.Typeface.FontFamily, htmlForeground);
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

        public static void CopyCharacter(List<VTCharacter> copyFroms, List<VTCharacter> copyTos)
        {
            copyTos.Clear();
            for (int i = 0; i < copyFroms.Count; i++)
            {
                VTCharacter copyFrom = copyFroms[i];
                VTCharacter character = VTCharacter.CreateNull();

                character.Attribute = copyFrom.Attribute;
                character.Background = copyFrom.Background;
                character.Foreground = copyFrom.Foreground;
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

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 根据光标闪烁速度获取闪烁间隔时间
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int GetCursorInterval(VTCursorSpeeds speed)
        {
            switch (speed)
            {
                case VTCursorSpeeds.HighSpeed: return MTermConsts.HighSpeedBlinkInterval;
                case VTCursorSpeeds.LowSpeed: return MTermConsts.LowSpeedBlinkInterval;
                case VTCursorSpeeds.NormalSpeed: return MTermConsts.NormalSpeedBlinkInterval;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 当Wallpaper是动态图的时候，获取动态图的元数据，用来实时渲染
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static GifMetadata GetWallpaperMetadata(string uri)
        {
            GifMetadata gifMetadata;
            if (!GifMetadataMap.TryGetValue(uri, out gifMetadata))
            {
                Stream stream = GetWallpaperStream(uri);
                if (stream == null)
                {
                    return new GifMetadata();
                }

                gifMetadata = GifParser.GetFrames(uri, stream);
                GifMetadataMap[uri] = gifMetadata;
            }
            return gifMetadata;
        }

        /// <summary>
        /// 获取动态背景或静态背景的预览图
        /// </summary>
        /// <param name="paperType">标识是静态图还是动态图</param>
        /// <param name="uri">背景图的路径</param>
        /// <returns></returns>
        public static BitmapSource GetWallpaperThumbnail(WallpaperTypeEnum paperType, string uri)
        {
            switch (paperType)
            {
                case WallpaperTypeEnum.Image:
                    {
                        return GetWallpaperBitmap(uri, 200, 200);
                    }

                case WallpaperTypeEnum.Live:
                    {
                        Stream stream = VTUtils.GetWallpaperStream(uri);
                        if (stream == null)
                        {
                            return ErrorBitmapImage;
                        }

                        return GifParser.GetThumbnail(stream);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 当Wallpaper是静态图的时候，获取静态图
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pixelWidth">设置解码后的图像宽度，减少这个值可以减少内存占用</param>
        /// <param name="pixelHeight">设置解码后的图像高度，减少这个值可以减少内存占用</param>
        /// <returns></returns>
        public static BitmapSource GetWallpaperBitmap(string uri, int pixelWidth = 0, int pixelHeight = 0)
        {
            Stream stream = VTUtils.GetWallpaperStream(uri);
            if (stream == null)
            {
                return ErrorBitmapImage;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.DecodePixelHeight = pixelHeight;
            bitmapImage.DecodePixelWidth = pixelWidth;
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        /// <summary>
        /// 根据当前屏幕大小计算终端的自适应大小
        /// </summary>
        /// <param name="contentSize">文档显示区域的大小</param>
        /// <param name="typeface">字体信息</param>
        /// <param name="rowSize">计算出来的终端行数</param>
        /// <param name="colSize">计算出来的终端列数</param>
        public static void CalculateAutoFitSize(VTSize contentSize, VTypeface typeface, out int rowSize, out int colSize)
        {
            // 自适应屏幕大小
            // 计算一共有多少行，和每行之间的间距是多少

            // 终端控件的初始宽度和高度，在打开Session的时候动态设置
            rowSize = (int)Math.Floor(contentSize.Height / typeface.Height);
            colSize = (int)Math.Floor(contentSize.Width / typeface.SpaceWidth);

            //Console.WriteLine("height = {0}, characterHeight = {1}, rows = {2}", contentArea.Height, typeface.Height, rowSize);
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
        /// 获取终端的清单文件
        /// </summary>
        /// <returns></returns>
        public static TerminalManifest GetManifest()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "manifest.json");

            if (!File.Exists(filePath))
            {
                return new TerminalManifest();
            }

            try
            {
                return JSONHelper.ParseFile<TerminalManifest>(filePath);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("加载终端清单文件异常, {0}, {1}", ex, filePath);
                return new TerminalManifest();
            }
        }

        /// <summary>
        /// 获取回放文件的完整目录
        /// </summary>
        /// <param name="sessionId">回放文件所属的会话Id</param>
        /// <param name="playbackFile">回放文件</param>
        /// <returns></returns>
        public static string GetPlaybackFilePath(string sessionId, PlaybackFile playbackFile)
        {
            string directory = GetPlaybackFileDirectory(sessionId);

            return Path.Combine(directory, playbackFile.Name);
        }

        /// <summary>
        /// 获取某个会话对应的回放文件的存储目录
        /// </summary>
        /// <param name="sessionId">回放文件所属的会话Id</param>
        /// <returns></returns>
        public static string GetPlaybackFileDirectory(string sessionId)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playback", sessionId);
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("创建Playback存储目录异常, {0}, {1}", ex, dir);
                    return string.Empty;
                }
            }

            return dir;
        }
    }
}
