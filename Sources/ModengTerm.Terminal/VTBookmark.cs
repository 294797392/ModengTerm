using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 管理书签列表
    /// </summary>
    public class VTBookmark
    {
        private XTermSession session;

        /// <summary>
        /// 书签列表
        /// </summary>
        public List<VTParagraph> Bookmarks { get; private set; }

        public VTBookmark(XTermSession session)
        {
            this.Bookmarks = new List<VTParagraph>();
            this.session = session;
        }

        public void AddBookmark(VTextLine textLine)
        {
            CreateContentParameter createContentParameter = new CreateContentParameter()
            {
                SessionName = this.session.Name,
                StartCharacterIndex = 0,
                EndCharacterIndex = textLine.Characters.Count - 1,
                ContentType = LogFileTypeEnum.PlainText,
                Typeface = textLine.Style,
                CharactersList = new List<List<VTCharacter>>() { textLine.Characters.ToList() }
            };

            VTParagraph paragraph = new VTParagraph()
            {
                StartCharacterIndex = createContentParameter.StartCharacterIndex,
                CharacterList = createContentParameter.CharactersList,
                Content = VTUtils.CreateContent(createContentParameter),
                CreationTime = DateTime.Now,
                EndCharacterIndex = createContentParameter.EndCharacterIndex,
                FirstPhysicsRow = textLine.PhysicsRow,
                IsAlternate = false,
                LastPhysicsRow = textLine.PhysicsRow,
            };

            this.Bookmarks.Add(paragraph);
        }

        public void RemoveBookmark(VTextLine textLine)
        {
            VTParagraph paragraph = this.Bookmarks.FirstOrDefault(v => v.FirstPhysicsRow == textLine.PhysicsRow);
            if (paragraph != null)
            {
                this.Bookmarks.Remove(paragraph);
            }
        }
    }
}
