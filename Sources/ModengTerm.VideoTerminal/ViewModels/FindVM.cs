using ModengTerm.Base;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Document.Graphics;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Document;

namespace ModengTerm.Terminal.ViewModels
{
    public class FindVM : ViewModelBase
    {
        private class MatchedLine
        {
            public List<VTMatches> Matches { get; private set; }

            public int PhysicsRow { get; set; }

            public MatchedLine(int physicsRow)
            {
                this.PhysicsRow = physicsRow;
                this.Matches = new List<VTMatches>();
            }
        }

        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindVM");

        #region 实例变量

        private bool ignoreCase;
        private bool regexp;
        private string keyword;
        private VideoTerminal videoTerminal;

        /// <summary>
        /// 存储当前要搜索的行数的物理行号
        /// </summary>
        private int startRow;

        /// <summary>
        /// 存储一共要搜索多少行
        /// </summary>
        private int endRow;

        /// <summary>
        /// 存储当前搜索到的行索引
        /// </summary>
        private int findIndex;

        /// <summary>
        /// 是否需要重置查找参数
        /// </summary>
        private bool dirty;

        private StringComparison stringComparison;
        private RegexOptions regexOptions;

        private bool findOnce;

        /// <summary>
        /// 存储当前命中匹配到的行
        /// </summary>
        private List<MatchedLine> matchedLines;

        #endregion

        #region 属性

        public BindableCollection<FindScopes> FindScopeList { get; private set; }

        public BindableCollection<FindStartups> FindStartupList { get; private set; }

        public bool IgnoreCase
        {
            get { return this.ignoreCase; }
            set
            {
                if (this.ignoreCase != value)
                {
                    this.ignoreCase = value;
                    this.NotifyPropertyChanged("IgnoreCase");

                    this.dirty = true;
                }
            }
        }

        public bool Regexp
        {
            get { return this.regexp; }
            set
            {
                if (this.regexp != value)
                {
                    this.regexp = value;
                    this.NotifyPropertyChanged("Regexp");

                    this.dirty = true;
                }
            }
        }

        public string Keyword
        {
            get { return this.keyword; }
            set
            {
                if (this.keyword != value)
                {
                    this.keyword = value;
                    this.NotifyPropertyChanged("Keyword");

                    this.dirty = true;
                }
            }
        }

        /// <summary>
        /// 当前是否是只查找一次就返回
        /// </summary>
        public bool FindOnce
        {
            get { return this.findOnce; }
            set
            {
                if (this.findOnce != value)
                {
                    this.findOnce = value;
                    this.NotifyPropertyChanged("FindOnce");
                    this.dirty = true;
                }
            }
        }

        #endregion

        #region 构造方法

        public FindVM(VideoTerminal vt)
        {
            this.videoTerminal = vt;

            this.FindScopeList = new BindableCollection<FindScopes>();
            this.FindScopeList.AddRange(MTermUtils.GetEnumValues<FindScopes>());

            this.FindStartupList = new BindableCollection<FindStartups>();
            this.FindStartupList.AddRange(MTermUtils.GetEnumValues<FindStartups>());
            this.matchedLines = new List<MatchedLine>();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 匹配一行，如果有匹配成功则返回匹配后的数据
        /// </summary>
        /// <param name="matchIndex">物理行号</param>
        /// <returns></returns>
        private MatchedLine MatchLine(int matchIndex)
        {
            VTHistoryLine historyLine;
            if (!this.videoTerminal.ScrollInfo.TryGetHistory(matchIndex, out historyLine))
            {
                logger.ErrorFormat("查找失败, 没找到对应的行, {0}", matchIndex);
                return null;
            }

            string text = VTUtils.CreatePlainText(historyLine.Characters);

            if (this.Regexp)
            {
                // 用正则表达式搜索
                Match match = Regex.Match(text, this.Keyword, this.regexOptions);
                if (!match.Success)
                {
                    // 没有找到搜索结果
                    return null;
                }

                MatchedLine matchedLine = new MatchedLine(matchIndex);

                do
                {
                    matchedLine.Matches.Add(new VTMatches(match.Length, match.Index));
                }
                while ((match = match.NextMatch()) != null);

                return matchedLine;
            }
            else
            {
                // 直接文本匹配
                // 注意一行文本里可能会有多个地方匹配，要把所有匹配的地方都找到

                int startIndex = 0;

                // 存储匹配的字符索引
                int matchedIndex = 0;

                if ((matchedIndex = text.IndexOf(this.Keyword, 0, this.stringComparison)) == -1)
                {
                    // 没找到
                    return null;
                }

                MatchedLine matchedLine = new MatchedLine(matchIndex);
                matchedLine.Matches.Add(new VTMatches(this.keyword.Length, matchedIndex));

                startIndex = matchedIndex + this.keyword.Length;

                // 找到了继续找
                while ((matchedIndex = text.IndexOf(this.keyword, startIndex, stringComparison)) >= 0)
                {
                    matchedLine.Matches.Add(new VTMatches(this.keyword.Length, matchedIndex));

                    startIndex = matchedIndex + this.keyword.Length;
                }

                return matchedLine;
            }
        }

        /// <summary>
        /// 获取下一个要匹配的行的物理行号
        /// </summary>
        /// <returns>返回-1则表示查找到底了</returns>
        private int GetNextMatchIndex()
        {
            int nextIndex = 0;

            if (this.endRow >= this.startRow)
            {
                // 说明从上往下找
                nextIndex = this.findIndex;
            }
            else
            {
                // 说明从下往上找
                nextIndex = this.endRow - this.findIndex;
            }

            if (nextIndex == this.endRow)
            {
                // 从上往下找找到底了
                this.dirty = true;
                return -1;
            }

            if (nextIndex < 0)
            {
                // 从下往上找找到底了
                this.dirty = true;
                return -1;
            }

            this.findIndex++;

            return nextIndex;
        }

        /// <summary>
        /// 让滚动条滚动到一行处
        /// </summary>
        /// <param name="matches"></param>
        private void ScrollToMatches(MatchedLine matches)
        {
            VTScrollInfo scrollInfo = this.videoTerminal.ScrollInfo;

            int scrollTo = matches.PhysicsRow - this.videoTerminal.RowSize / 2;
            if (scrollTo < scrollInfo.ScrollMin)
            {
                scrollTo = scrollInfo.ScrollMin;
            }
            else if (scrollTo > scrollInfo.ScrollMax)
            {
                scrollTo = scrollInfo.ScrollMax;
            }

            this.videoTerminal.ScrollTo(scrollTo);
        }

        /// <summary>
        /// 高亮显示匹配的项
        /// </summary>
        /// <param name="matches"></param>
        private void HighlightMatches(MatchedLine matches)
        {
            VTextLine textLine = this.videoTerminal.ActiveDocument.FindLine(matches.PhysicsRow);
            textLine.MatchesList = matches.Matches;

            this.videoTerminal.uiSyncContext.Send((state) =>
            {
                textLine.RequestInvalidate();
            }, null);
        }

        #endregion

        #region 公开接口

        public void PerformFind()
        {
            if (string.IsNullOrEmpty(this.Keyword))
            {
                return;
            }

            if (this.dirty)
            {
                VTScrollInfo scrollback = this.videoTerminal.ScrollInfo;
                int firstRow = 0, lastRow = 0, totalRows = 0; // 要搜索的起始行和结束行和要搜索的总行数

                switch (this.FindScopeList.SelectedItem)
                {
                    case FindScopes.All:
                        {
                            // 计算一共要搜索的行数
                            firstRow = scrollback.FirstLine.PhysicsRow;
                            lastRow = scrollback.LastLine.PhysicsRow;
                            totalRows = lastRow - firstRow + 1;
                            break;
                        }

                    case FindScopes.Document:
                        {
                            firstRow = this.videoTerminal.ActiveDocument.FirstLine.PhysicsRow;
                            lastRow = this.videoTerminal.ActiveDocument.LastLine.PhysicsRow;
                            totalRows = lastRow - firstRow + 1;
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                switch (this.FindStartupList.SelectedItem)
                {
                    case FindStartups.FromBegin:
                        {
                            // 从头开始查找
                            this.findIndex = 0;
                            this.startRow = firstRow;
                            this.endRow = lastRow;
                            break;
                        }

                    case FindStartups.FromEnd:
                        {
                            this.findIndex = 0;
                            this.startRow = lastRow;
                            this.endRow = firstRow;
                            break;
                        }

                    case FindStartups.CurrentToBegin:
                        {
                            // 从当前位置往上找，用当前内容的最后一行的索引
                            this.findIndex = 0;
                            this.startRow = lastRow;
                            this.endRow = firstRow;
                            break;
                        }

                    case FindStartups.CurrentToEnd:
                        {
                            this.findIndex = 0;
                            this.startRow = firstRow;
                            this.endRow = lastRow;
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                this.stringComparison = this.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
                this.regexOptions = this.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

                this.dirty = false;
            }

            this.matchedLines.Clear();

            while (true)
            {
                int matchIndex = this.GetNextMatchIndex();
                if (matchIndex < 0)
                {
                    // 到底了
                    if (this.FindOnce)
                    {
                        MTMessageBox.Info("已经找到最后一行了");
                    }
                    else
                    {
                        // 显示查找结果窗口
                    }
                    return;
                }

                MatchedLine matches = this.MatchLine(this.findIndex);
                if (matches == null)
                {
                    // 没找到，继续找
                    continue;
                }
                else
                {
                    if (FindOnce)
                    {
                        this.ScrollToMatches(matches);
                        this.HighlightMatches(matches);
                        break;
                    }
                    else
                    {
                        this.matchedLines.Add(matches);
                    }
                }
            }
        }

        #endregion
    }
}
