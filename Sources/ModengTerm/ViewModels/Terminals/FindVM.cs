using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Rendering;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 定义查找范围
    /// </summary>
    public enum FindScopes
    {
        AllDocument,
    }

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
        private List<VTMatches> matchResult;

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
        private IDocumentObject mainRectElement;
        private IDocumentObject alternateRectElement;
        private IDocumentObject activeRectElement;

        #endregion

        #region 属性

        /// <summary>
        /// 显示当前查找状态
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// 查找的时候是否区分大小写
        /// </summary>
        public bool CaseSensitive
        {
            get { return this.caseSensitive; }
            set
            {
                if (this.caseSensitive != value)
                {
                    this.caseSensitive = value;
                    this.NotifyPropertyChanged("CaseSensitive");
                    this.OnCaseSensitiveChanged();
                }
            }
        }

        /// <summary>
        /// 是否使用正则表达式进行查找
        /// </summary>
        public bool Regexp
        {
            get { return this.regexp; }
            set
            {
                if (this.regexp != value)
                {
                    this.regexp = value;
                    this.NotifyPropertyChanged("Regexp");
                }
            }
        }

        /// <summary>
        /// 要查找的关键字或者正则表达式
        /// </summary>
        public string Keyword
        {
            get { return this.keyword; }
            set
            {
                if (this.keyword != value)
                {
                    this.keyword = value;
                    this.NotifyPropertyChanged("Keyword");
                    this.OnKeywordChanged(value);
                }
            }
        }

        /// <summary>
        /// 是否查找所有
        /// </summary>
        public bool FindAll
        {
            get { return this.findAll; }
            set
            {
                if (this.findAll != value)
                {
                    this.findAll = value;
                    this.NotifyPropertyChanged("FindAll");
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
            get { return this.upFind; }
            set
            {
                if (this.upFind != value)
                {
                    this.upFind = value;
                    this.NotifyPropertyChanged("UpFind");
                }
            }
        }

        /// <summary>
        /// 向下查找
        /// </summary>
        public bool DownFind
        {
            get { return this.downFind; }
            set
            {
                if (this.downFind != value)
                {
                    this.downFind = value;
                    this.NotifyPropertyChanged("DownFind");
                }
            }
        }

        #endregion

        #region 构造方法

        public FindVM(IVideoTerminal vt)
        {
            this.videoTerminal = vt;

            this.videoTerminal.MainDocument.ScrollChanged += MainDocument_ScrollChanged;
            this.videoTerminal.MainDocument.DiscardLine += MainDocument_DiscardLine;
            this.mainRectElement = this.videoTerminal.MainDocument.Renderer.CreateDrawingObject();
            this.alternateRectElement = this.videoTerminal.AlternateDocument.Renderer.CreateDrawingObject();

            if (this.videoTerminal.IsAlternate)
            {
                this.activeRectElement = this.alternateRectElement;
            }
            else
            {
                this.activeRectElement = this.mainRectElement;
            }
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
            if (string.IsNullOrEmpty(keyword))
            {
                this.matchResult = null;
                this.activeRectElement.Clear();
                return false;
            }

            List<VTMatches> matches = this.FindMatches(keyword);
            if (matches.Count == 0)
            {
                this.activeRectElement.Clear();
                return false;
            }

            this.HighlightMatches(matches);

            this.matchResult = matches;

            return true;
        }

        /// <summary>
        /// 匹配一行，如果有匹配成功则返回匹配后的数据
        /// </summary>
        /// <param name="textLine">要搜索的行</param>
        /// <returns></returns>
        private List<VTMatches> FindMatches(string keyword, VTextLine textLine)
        {
            string text = VTUtils.CreatePlainText(textLine.Characters);

            if (this.Regexp)
            {
                RegexOptions regexOptions = this.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

                // 用正则表达式搜索
                Match match = Regex.Match(text, keyword, regexOptions);
                if (!match.Success)
                {
                    // 没有找到搜索结果
                    return null;
                }

                List<VTMatches> vtMatches = new List<VTMatches>();

                do
                {
                    vtMatches.Add(new VTMatches(textLine, match.Length, match.Index));
                }
                while ((match = match.NextMatch()) != null);

                return vtMatches;
            }
            else
            {
                StringComparison stringComparison = this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                // 直接文本匹配
                // 注意一行文本里可能会有多个地方匹配，要把所有匹配的地方都找到

                int startIndex = 0;

                // 存储匹配的字符索引
                int matchedIndex = 0;

                if ((matchedIndex = text.IndexOf(keyword, 0, stringComparison)) == -1)
                {
                    // 没找到
                    return null;
                }

                List<VTMatches> vtMatches = new List<VTMatches>();

                vtMatches.Add(new VTMatches(textLine, this.keyword.Length, matchedIndex));

                startIndex = matchedIndex + this.keyword.Length;

                // 找到了继续找
                while ((matchedIndex = text.IndexOf(this.keyword, startIndex, stringComparison)) >= 0)
                {
                    vtMatches.Add(new VTMatches(textLine, this.keyword.Length, matchedIndex));

                    startIndex = matchedIndex + this.keyword.Length;
                }

                return vtMatches;
            }
        }

        /// <summary>
        /// 查找当前文档的所有匹配项
        /// </summary>
        /// <param name="keyword">要匹配的关键字</param>
        /// <returns></returns>
        private List<VTMatches> FindMatches(string keyword)
        {
            List<VTMatches> result = new List<VTMatches>();

            VTDocument activeDocument = this.videoTerminal.ActiveDocument;

            VTextLine current = activeDocument.FirstLine;

            while (current != null)
            {
                List<VTMatches> matches = this.FindMatches(keyword, current);
                if (matches != null)
                {
                    result.AddRange(matches);
                }

                current = current.NextLine;
            }

            return result;
        }

        /// <summary>
        /// 高亮显示匹配的项
        /// </summary>
        /// <param name="matches"></param>
        private void HighlightMatches(List<VTMatches> matchResult)
        {
            List<VTRect> rectangles = new List<VTRect>();

            foreach (VTMatches matches in matchResult)
            {
                VTextLine textLine = matches.TextLine;

                VTextRange textRange = textLine.MeasureTextRange(matches.Index, matches.Length);

                rectangles.Add(new VTRect(textRange.Left, textRange.Top, textRange.Width, textRange.Height));
            }

            this.activeRectElement.DrawRectangles(rectangles, null, this.HighlightBackground);
        }

        /// <summary>
        /// 当关键字改变的时候触发
        /// </summary>
        private void OnKeywordChanged(string keyword)
        {
            this.PerformFind(keyword);
        }

        /// <summary>
        /// 当区分大小写选项改变的时候触发
        /// </summary>
        private void OnCaseSensitiveChanged()
        {
            this.PerformFind(this.Keyword);
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当滚动结束并渲染结束之后触发
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void MainDocument_ScrollChanged(VTDocument arg1, VTScrollData arg2)
        {
            this.PerformFind(this.keyword);
        }

        /// <summary>
        /// 当丢弃行的时候触发
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MainDocument_DiscardLine(VTDocument obj)
        {
            this.PerformFind(this.keyword);
        }

        #endregion

        #region 公开接口

        public void Release()
        {
            this.videoTerminal.MainDocument.ScrollChanged -= this.MainDocument_ScrollChanged;
            this.matchResult = null;
            this.Message = string.Empty;
        }

        #endregion
    }
}
