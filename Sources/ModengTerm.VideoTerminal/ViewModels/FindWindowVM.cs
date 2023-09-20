using ModengTerm.Base;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Document;

namespace ModengTerm.Terminal.ViewModels
{
    public class FindWindowVM : ViewModelBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindWindowVM");

        private bool ignoreCase;
        private bool regexp;
        private string keyword;
        private bool findAll;
        private VideoTerminal vt;

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
        /// 是否一次性查找所有匹配的行
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

        public FindWindowVM(VideoTerminal vt)
        {
            this.vt = vt;

            this.FindScopeList = new BindableCollection<FindScopes>();
            this.FindScopeList.AddRange(MTermUtils.GetEnumValues<FindScopes>());

            this.FindStartupList = new BindableCollection<FindStartups>();
            this.FindStartupList.AddRange(MTermUtils.GetEnumValues<FindStartups>());
        }

        public List<VTHistoryLine> Find()
        {
            throw new NotImplementedException();
            //VTScrollback scrollback = this.vt.Scrollback;

            //// 要搜索的起始行和结束行
            //int firstRow = 0, lastRow = 0;

            //try
            //{
            //    switch (this.FindScopeList.SelectedItem)
            //    {
            //        case FindScopes.All:
            //            {
            //                firstRow = scrollback.FirstLine.PhysicsRow;
            //                lastRow = scrollback.LastLine.PhysicsRow;
            //                break;
            //            }

            //        case FindScopes.Document:
            //            {
            //                firstRow = this.vt.ActiveDocument.FirstLine.PhysicsRow;
            //                lastRow = this.vt.ActiveDocument.LastLine.PhysicsRow;
            //                break;
            //            }

            //        default:
            //            throw new NotImplementedException();
            //    }

            //    // 一共要搜索的行数
            //    int rows = lastRow - firstRow + 1;

            //    switch (this.FindStartupList.SelectedItem)
            //    {
            //        case FindStartups.FromBegin:
            //            {
            //                // 从头开始查找
            //                scrollback.TryGetHistories(firstRow, rows)

            //                    for (int i = 0; i < rows; i++)
            //                {
            //                    scrollback.TryGetHistories
            //                    }

            //                break;
            //            }

            //        case FindStartups.FromEnd:
            //            {
            //                break;
            //            }

            //        case FindStartups.CurrentToBegin:
            //            {
            //                break;
            //            }

            //        case FindStartups.CurrentToEnd:
            //            {
            //                break;
            //            }

            //        default:
            //            throw new NotImplementedException();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    logger.Error("查找异常", ex);
            //}
            //finally
            //{
            //    this.find = false;
            //}
        }
    }
}
