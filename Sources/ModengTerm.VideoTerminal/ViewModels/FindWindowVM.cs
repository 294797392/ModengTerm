using ModengTerm.Base;
using ModengTerm.Terminal.Document;
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
    public enum FindResults
    {
        /// <summary>
        /// 查找成功
        /// </summary>
        Success,

        /// <summary>
        /// 查找失败
        /// </summary>
        Failed,

        /// <summary>
        /// 查找结束
        /// </summary>
        Completed
    }

    /// <summary>
    /// 存储匹配的行
    /// </summary>
    public class MatchedLine
    {
        /// <summary>
        /// 匹配行的物理行号
        /// </summary>
        public int PhysicsRow { get; private set; }

        public List<VTCharacter> Characters { get; private set; }

        /// <summary>
        /// 匹配到的文本索引
        /// </summary>
        public List<int> MatchedIndexs { get; private set; }

        public MatchedLine(IEnumerable<VTCharacter> characters, int physicsRow)
        {
            this.MatchedIndexs = new List<int>();
            this.Characters = new List<VTCharacter>(characters);
            this.PhysicsRow = physicsRow;
        }
    }

    public class FindWindowVM : ViewModelBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindWindowVM");

        #region 实例变量

        private bool ignoreCase;
        private bool regexp;
        private string keyword;
        private VideoTerminal vt;

        /// <summary>
        /// 存储当前要搜索的行数
        /// </summary>
        private List<MatchedLine> matchList;

        /// <summary>
        /// 存储当前搜索到的索引
        /// </summary>
        private int findIndex;

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
                }
            }
        }

        /// <summary>
        /// 存储最后一次的查找结果
        /// </summary>
        public List<MatchedLine> FindResult { get; private set; }

        #endregion

        #region 构造方法

        public FindWindowVM(VideoTerminal vt)
        {
            this.vt = vt;

            this.FindScopeList = new BindableCollection<FindScopes>();
            this.FindScopeList.AddRange(MTermUtils.GetEnumValues<FindScopes>());

            this.FindStartupList = new BindableCollection<FindStartups>();
            this.FindStartupList.AddRange(MTermUtils.GetEnumValues<FindStartups>());

            this.matchList = new List<MatchedLine>();
            this.FindResult = new List<MatchedLine>();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 开始执行查找操作
        /// </summary>
        /// <param name="findOnce">是否找到了一个匹配项就返回</param>
        /// <returns>
        /// 本次搜索结果
        /// 搜索到的匹配行存储在FindResult属性里
        /// </returns>
        public FindResults Find(bool findOnce)
        {
            if (string.IsNullOrEmpty(this.Keyword))
            {
                return FindResults.Success;
            }

            VTScrollback scrollback = this.vt.Scrollback;

            switch (this.FindScopeList.SelectedItem)
            {
                case FindScopes.All:
                    {
                        // 计算一共要搜索的行数
                        int firstRow = scrollback.FirstLine.PhysicsRow;
                        int lastRow = scrollback.LastLine.PhysicsRow;
                        int rows = lastRow - firstRow + 1;

                        List<VTHistoryLine> historyLines;
                        if (!scrollback.TryGetHistories(firstRow, rows, out historyLines))
                        {
                            logger.ErrorFormat("查找失败, 获取历史行数据失败, startRow = {0}, rows = {1}", firstRow, rows);
                            return FindResults.Failed;
                        }

                        this.matchList.AddRange(historyLines.Select(v => new MatchedLine(v.Characters, v.PhysicsRow)));
                        break;
                    }

                case FindScopes.Document:
                    {
                        VTextLine current = this.vt.ActiveDocument.FirstLine;
                        while (current != null)
                        {
                            this.matchList.Add(new MatchedLine(current.Characters, current.PhysicsRow));
                            current = current.NextLine;
                        }
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
                        findIndex = 0;
                        break;
                    }

                case FindStartups.FromEnd:
                    {
                        findIndex = 0;
                        this.matchList.Reverse();
                        break;
                    }

                case FindStartups.CurrentToBegin:
                    {
                        // 从当前位置往上找，用当前内容的最后一行的索引
                        this.findIndex = this.matchList.FindIndex((v => v.PhysicsRow == this.vt.ActiveDocument.LastLine.PhysicsRow));
                        this.matchList.Reverse();
                        break;
                    }

                case FindStartups.CurrentToEnd:
                    {
                        this.findIndex = this.matchList.FindIndex((v => v.PhysicsRow == this.vt.ActiveDocument.FirstLine.PhysicsRow));
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            StringComparison stringComparison = this.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            RegexOptions regexOptions = this.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            for (; this.findIndex < this.matchList.Count; this.findIndex++)
            {
                MatchedLine matchLine = this.matchList[this.findIndex];

                string text = VTUtils.CreatePlainText(matchLine.Characters);

                if (this.Regexp)
                {
                    // 用正则表达式搜索
                    Match match = Regex.Match(text, this.Keyword, regexOptions);
                    if (!match.Success)
                    {
                        continue;
                    }

                    do
                    {
                        matchLine.MatchedIndexs.Add(match.Index);
                    }
                    while ((match = match.NextMatch()) != null);
                }
                else
                {
                    // 直接文本匹配
                    // 注意一行文本里可能会有多个地方匹配，要把所有匹配的地方都找到

                    // 从startIndex开始搜索
                    int startIndex = 0;

                    // 存储匹配的字符索引
                    int matchedIndex = 0;

                    while ((matchedIndex = text.IndexOf(this.Keyword, startIndex, stringComparison)) > 0)
                    {
                        matchLine.MatchedIndexs.Add(matchedIndex);
                    }
                }

                // 如果有匹配项，那么把这一行加入到查找结果里
                if (matchLine.MatchedIndexs.Count > 0)
                {
                    this.FindResult.Add(matchLine);
                }
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}
