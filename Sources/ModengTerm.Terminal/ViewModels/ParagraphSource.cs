using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 段落数据源
    /// </summary>
    public abstract class ParagraphSource
    {
        /// <summary>
        /// 该数据源所对应的会话
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 获取所有段落列表
        /// </summary>
        /// <returns></returns>
        public abstract List<ParagraphVM> GetParagraphs();
    }

    /// <summary>
    /// 对剪贴板历史记录提供数据
    /// </summary>
    public class ClipboardParagraphSource : ParagraphSource
    {
        private VTClipboard clipboard;

        public ClipboardParagraphSource(VTClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public override List<ParagraphVM> GetParagraphs()
        {
            return this.clipboard.HistoryList.Select(v => new ParagraphVM(v)).ToList();
        }
    }

    /// <summary>
    /// 对收藏夹提供数据
    /// </summary>
    public class FavoritesParagraphSource : ParagraphSource
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FavoritesParagraphSource");

        //private ITerminalAgent terminalAgent;

        //public FavoritesParagraphSource(ITerminalAgent terminalAgent)
        //{
            //this.terminalAgent = terminalAgent;
        //}

        public override List<ParagraphVM> GetParagraphs()
        {
            //List<Favorites> favoritesList = this.terminalAgent.GetFavorites(this.Session.ID);
            //if (favoritesList == null)
            //{
            //    logger.ErrorFormat("获取收藏夹列表失败, sessionID = {0}", this.Session.ID);
            //    return new List<ParagraphVM>();
            //}

            //return favoritesList.Select(v => new ParagraphVM(v)).ToList();
            throw new NotImplementedException();
        }
    }

    public class BookmarkParagraphSource : ParagraphSource
    {
        private VTBookmark bookmarkMgr;

        public BookmarkParagraphSource(VTBookmark bookmarkMgr)
        {
            this.bookmarkMgr = bookmarkMgr;
        }

        public override List<ParagraphVM> GetParagraphs()
        {
            return this.bookmarkMgr.Bookmarks.Select(v => new ParagraphVM(v)).ToList();
        }
    }
}
