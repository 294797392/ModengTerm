using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
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

        private bool findOnce;

        /// <summary>
        /// 存储当前命中匹配到的行
        /// </summary>
        private List<VTMatches> matchResult;

        /// <summary>
        /// 查找提示
        /// </summary>
        private string message;

        /// <summary>
        /// 当前查找下一个的索引
        /// 可以从下往上找
        /// 也可以从上往下找
        /// </summary>
        private int findIndex;

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
            get { return this.findOnce; }
            set
            {
                if (this.findOnce != value)
                {
                    this.findOnce = value;
                    this.NotifyPropertyChanged("FindAll");
                }
            }
        }

        /// <summary>
        /// 高亮区域的前景色
        /// </summary>
        public VTColor HighlightForeground { get; set; }

        /// <summary>
        /// 高亮区域的背景色
        /// </summary>
        public VTColor HighlightBackground { get; set; }

        #endregion

        #region 构造方法

        public FindVM(IVideoTerminal vt)
        {
            this.videoTerminal = vt;
            this.videoTerminal.ScrollChanged += VideoTerminal_ScrollChanged;
        }

        #endregion

        #region 实例方法

        private void PerformFind(string keyword)
        {
            // 先恢复文本行的状态
            this.ResetTextLineState();

            if (string.IsNullOrEmpty(keyword))
            {
                this.matchResult = null;
                return;
            }

            List<VTMatches> matches = this.FindMatches(keyword);
            this.HighlightMatches(matches);

            this.matchResult = matches;
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

            VTScreen activeScreen = this.videoTerminal.ActiveScreen;

            VTextLine current = activeScreen.FirstLine;

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
            foreach (VTMatches matches in matchResult)
            {
                VTextLine textLine = matches.TextLine;

                for (int i = 0; i < matches.Length; i++)
                {
                    VTCharacter character = textLine.Characters[i + matches.Index];

                    character.Background = this.HighlightBackground;
                    character.Foreground = this.HighlightForeground;

                    VTUtils.SetTextAttribute(VTextAttributes.Foreground, true, ref character.Attribute);
                    VTUtils.SetTextAttribute(VTextAttributes.Background, true, ref character.Attribute);
                }

                textLine.MakeInvalidate();
                textLine.RequestInvalidate();
            }
        }

        /// <summary>
        /// 把高亮显示的匹配项重置为默认颜色
        /// </summary>
        private void ResetTextLineState()
        {
            if (this.matchResult == null)
            {
                return;
            }

            IEnumerable<VTextLine> textLines = this.matchResult.GroupBy(v => v.TextLine).Select(v => v.Key);
            foreach (VTextLine textLine in textLines)
            {
                VTHistoryLine historyLine;
                if (!this.videoTerminal.TryGetHistoryLine(textLine.PhysicsRow, out historyLine))
                {
                    logger.ErrorFormat("KeywordChanged失败, 没找到文本行对应的历史记录, physicsRow = {0}", textLine.PhysicsRow);
                    continue;
                }

                textLine.SetHistory(historyLine);
                textLine.RequestInvalidate();
            }
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

        private void VideoTerminal_ScrollChanged(IVideoTerminal vt, int oldScrollValue, int newScrollValue)
        {
            this.PerformFind(this.keyword);
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 查找下一个
        /// </summary>
        public void FindNext()
        {

        }

        public void Release()
        {
            this.ResetTextLineState();
            this.matchResult = null;
            this.Message = string.Empty;
        }

        #endregion
    }
}
