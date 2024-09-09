using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public class ParagraphVM : ItemViewModel
    {
        private string content;
        private DateTime creationTime;
        private int firstRow;

        public string Content
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
                }
            }
        }

        public DateTime CreationTime
        {
            get { return this.creationTime; }
            set
            {
                if (this.creationTime != value)
                {
                    this.creationTime = value;
                    this.NotifyPropertyChanged("CreationTime");
                }
            }
        }

        public int FirstRow
        {
            get { return this.firstRow; }
            set
            {
                if (this.firstRow != value)
                {
                    this.firstRow = value;
                    this.NotifyPropertyChanged("FirstRow");
                }
            }
        }

        public ParagraphVM(VTParagraph paragraph)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Content = paragraph.Content;
            //this.FirstRow = paragraph.FirstPhysicsRow;
        }

        public ParagraphVM(Favorites favorites)
        {
            throw new NotImplementedException();

            this.ID = Guid.NewGuid().ToString();

            //CreateContentParameter ccp = new CreateContentParameter()
            //{
            //    StartCharacterIndex = favorites.StartCharacterIndex,
            //    EndCharacterIndex = favorites.EndCharacterIndex,
            //    HistoryLines = favorites.CharacterList,
            //    ContentType = ParagraphFormatEnum.PlainText,
            //    SessionName = String.Empty,
            //    Typeface = favorites.Typeface,
            //};
            //this.Content = VTUtils.CreateContent(ccp);
            //this.CreationTime = favorites.CreationTime;
            //this.FirstRow = -1;
        }
    }
}
