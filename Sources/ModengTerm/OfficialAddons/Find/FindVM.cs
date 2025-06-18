using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.EventData;
using ModengTerm.Document.Graphics;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Find
{
    /// <summary>
    /// 搜索ViewModel
    /// 模仿XShell和VS2022的搜索功能
    /// </summary>
    public class FindVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindVM");

        #endregion

        #region 实例变量

        private bool caseSensitive;
        private bool regexp;
        private string keyword;
        private IVideoTerminal videoTerminal;

        /// <summary>
        /// 存储当前命中匹配到的行
        /// </summary>
        private List<VTextRange> matchResult;

        /// <summary>
        /// 查找提示
        /// </summary>
        private string message;

        /// <summary>
        /// 要高亮显示的在matchResult里的元素索引
        /// 可以从下往上找
        /// 也可以从上往下找
        /// </summary>
        private int findIndex;

        /// <summary>
        /// 是否至少执行了一次查找
        /// </summary>
        private bool findOnce;

        private bool upFind;
        private bool downFind;

        private bool findAll;

        /// <summary>
        /// 用来高亮显示匹配结果的矩形
        /// </summary>
        private GraphicsObject mainRectElement;
        private GraphicsObject alternateRectElement;
        private GraphicsObject activeRectElement;

        /// <summary>
        /// 如果某一行内容没有变化，那么不需要重新搜索
        /// 根据渲染计数（Version）来判断VTextLine的内容是否有变化
        /// </summary>
        private Dictionary<VTextLine, int> textLineVersions;

        #endregion

        #region 属性

        /// <summary>
        /// 显示当前查找状态
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// 查找的时候是否区分大小写
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set
            {
                if (caseSensitive != value)
                {
                    caseSensitive = value;
                    NotifyPropertyChanged("CaseSensitive");
                    OnCaseSensitiveChanged();
                }
            }
        }

        /// <summary>
        /// 是否使用正则表达式进行查找
        /// </summary>
        public bool Regexp
        {
            get { return regexp; }
            set
            {
                if (regexp != value)
                {
                    regexp = value;
                    NotifyPropertyChanged("Regexp");
                }
            }
        }

        /// <summary>
        /// 要查找的关键字或者正则表达式
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                    NotifyPropertyChanged("Keyword");
                    OnKeywordChanged(value);
                }
            }
        }

        /// <summary>
        /// 是否查找所有
        /// </summary>
        public bool FindAll
        {
            get { return findAll; }
            set
            {
                if (findAll != value)
                {
                    findAll = value;
                    NotifyPropertyChanged("FindAll");
                }
            }
        }

        /// <summary>
        /// 高亮区域的前景色
        /// 暂时没用
        /// </summary>
        public VTColor HighlightForeground { get; set; }

        /// <summary>
        /// 高亮区域的背景色
        /// </summary>
        public VTColor HighlightBackground { get; set; }

        /// <summary>
        /// 向上查找
        /// </summary>
        public bool UpFind
        {
            get { return upFind; }
            set
            {
                if (upFind != value)
                {
                    upFind = value;
                    NotifyPropertyChanged("UpFind");
                }
            }
        }

        /// <summary>
        /// 向下查找
        /// </summary>
        public bool DownFind
        {
            get { return downFind; }
            set
            {
                if (downFind != value)
                {
                    downFind = value;
                    NotifyPropertyChanged("DownFind");
                }
            }
        }

        #endregion

        #region 构造方法

        public FindVM()
        {
            textLineVersions = new Dictionary<VTextLine, int>();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>是否找到了至少一个匹配项</returns>
        private bool PerformFind(string keyword)
        {
            //if (string.IsNullOrEmpty(keyword))
            //{
            //    matchResult = null;
            //    activeRectElement.Clear();
            //    return false;
            //}

            //List<VTextMatches> matches = FindMatches(keyword);
            //if (matches.Count == 0)
            //{
            //    activeRectElement.Clear();
            //    return false;
            //}

            //HighlightMatches(matches);

            //matchResult = matches;

            //return true;
            return true;
        }

        ///// <summary>
        ///// 匹配一行，如果有匹配成功则返回匹配后的数据
        ///// </summary>
        ///// <param name="textLine">要搜索的行</param>
        ///// <returns></returns>
        //private List<VTextMatches> FindMatches(string keyword, VTextLine textLine)
        //{
        //    return new List<VTextMatches>();
        //    //string text = VTDocUtils.CreatePlainText(textLine.Characters);

        //    //if (Regexp)
        //    //{
        //    //    RegexOptions regexOptions = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

        //    //    // 用正则表达式搜索
        //    //    Match match = Regex.Match(text, keyword, regexOptions);
        //    //    if (!match.Success)
        //    //    {
        //    //        // 没有找到搜索结果
        //    //        return null;
        //    //    }

        //    //    List<VTMatches> vtMatches = new List<VTMatches>();

        //    //    do
        //    //    {
        //    //        vtMatches.Add(new VTMatches(textLine, match.Length, match.Index));
        //    //    }
        //    //    while ((match = match.NextMatch()) != null);

        //    //    return vtMatches;
        //    //}
        //    //else
        //    //{
        //    //    StringComparison stringComparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        //    //    // 直接文本匹配
        //    //    // 注意一行文本里可能会有多个地方匹配，要把所有匹配的地方都找到

        //    //    int startIndex = 0;

        //    //    // 存储匹配的字符索引
        //    //    int matchedIndex = 0;

        //    //    if ((matchedIndex = text.IndexOf(keyword, 0, stringComparison)) == -1)
        //    //    {
        //    //        // 没找到
        //    //        return null;
        //    //    }

        //    //    List<VTMatches> vtMatches = new List<VTMatches>();

        //    //    vtMatches.Add(new VTMatches(textLine, this.keyword.Length, matchedIndex));

        //    //    startIndex = matchedIndex + this.keyword.Length;

        //    //    // 找到了继续找
        //    //    while ((matchedIndex = text.IndexOf(this.keyword, startIndex, stringComparison)) >= 0)
        //    //    {
        //    //        vtMatches.Add(new VTMatches(textLine, this.keyword.Length, matchedIndex));

        //    //        startIndex = matchedIndex + this.keyword.Length;
        //    //    }

        //    //    return vtMatches;
        //    //}
        //}

        ///// <summary>
        ///// 查找当前文档的所有匹配项
        ///// </summary>
        ///// <param name="keyword">要匹配的关键字</param>
        ///// <returns></returns>
        //private List<VTextMatches> FindMatches(string keyword)
        //{
        //    List<VTextMatches> result = new List<VTextMatches>();

        //    VTDocument activeDocument = videoTerminal.ActiveDocument;

        //    VTextLine current = activeDocument.FirstLine;

        //    while (current != null)
        //    {
        //        List<VTextMatches> matches = FindMatches(keyword, current);
        //        if (matches != null)
        //        {
        //            result.AddRange(matches);
        //        }

        //        current = current.NextLine;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 高亮显示匹配的项
        ///// </summary>
        ///// <param name="matches"></param>
        //private void HighlightMatches(List<VTextMatches> matchResult)
        //{
        //    List<VTRect> rectangles = new List<VTRect>();

        //    foreach (VTextMatches matches in matchResult)
        //    {
        //        //VTextLine textLine = matches.TextLine;

        //        //VTextRange textRange = textLine.MeasureTextRange(matches.Index, matches.Length);

        //        //rectangles.Add(new VTRect(textRange.Left, textRange.Top, textRange.Width, textRange.Height));
        //    }

        //    activeRectElement.DrawRectangles(rectangles, null, HighlightBackground);
        //}

        /// <summary>
        /// 当关键字改变的时候触发
        /// </summary>
        private void OnKeywordChanged(string keyword)
        {
            PerformFind(keyword);
        }

        /// <summary>
        /// 当区分大小写选项改变的时候触发
        /// </summary>
        private void OnCaseSensitiveChanged()
        {
            PerformFind(Keyword);
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置要搜索的终端
        /// </summary>
        /// <param name="vt"></param>
        public void SetVideoTerminal(IVideoTerminal vt)
        {
            if (videoTerminal == vt)
            {
                return;
            }

            IVideoTerminal oldTerminal = videoTerminal;
            if (oldTerminal != null)
            {
                // 先释放之前搜索的终端资源
                oldTerminal.MainDocument.Rendering -= MainDocument_Rendering;
                oldTerminal.MainDocument.GraphicsInterface.DeleteDrawingObject(mainRectElement);
                oldTerminal.AlternateDocument.Rendering -= MainDocument_Rendering;
                oldTerminal.AlternateDocument.GraphicsInterface.DeleteDrawingObject(alternateRectElement);
            }

            IVideoTerminal newTerminal = vt;
            newTerminal.MainDocument.Rendering += MainDocument_Rendering;
            mainRectElement = newTerminal.MainDocument.GraphicsInterface.CreateDrawingObject();
            newTerminal.AlternateDocument.Rendering += MainDocument_Rendering;
            alternateRectElement = newTerminal.AlternateDocument.GraphicsInterface.CreateDrawingObject();
            if (newTerminal.IsAlternate)
            {
                activeRectElement = alternateRectElement;
            }
            else
            {
                activeRectElement = mainRectElement;
            }

            // 更新渲染计数缓存
            textLineVersions.Clear();

            videoTerminal = vt;

            if (!string.IsNullOrEmpty(keyword))
            {
                PerformFind(keyword);
            }
        }

        public void Release()
        {
            videoTerminal.MainDocument.Rendering -= MainDocument_Rendering;
            videoTerminal.MainDocument.GraphicsInterface.DeleteDrawingObject(mainRectElement);
            videoTerminal.AlternateDocument.Rendering -= MainDocument_Rendering;
            videoTerminal.AlternateDocument.GraphicsInterface.DeleteDrawingObject(alternateRectElement);
            matchResult = null;
            Message = string.Empty;
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当滚动结束并渲染结束之后触发
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void MainDocument_Rendering(VTDocument document, VTRenderData renderData)
        {
            PerformFind(keyword);
        }

        /// <summary>
        /// 当丢弃行的时候触发
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MainDocument_DiscardLine(VTDocument obj)
        {
            PerformFind(keyword);
        }

        #endregion
    }
}
