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

namespace ModengTerm.ViewModel.Terminal
{
    public class ParagraphVM : ItemViewModel
    {
        private string content;
        private DateTime creationTime;
        private int firstRow;

        public string Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        public DateTime CreationTime
        {
            get { return creationTime; }
            set
            {
                if (creationTime != value)
                {
                    creationTime = value;
                    NotifyPropertyChanged("CreationTime");
                }
            }
        }

        public int FirstRow
        {
            get { return firstRow; }
            set
            {
                if (firstRow != value)
                {
                    firstRow = value;
                    NotifyPropertyChanged("FirstRow");
                }
            }
        }

        public ParagraphVM(VTParagraph paragraph)
        {
            ID = Guid.NewGuid().ToString();
            Content = paragraph.Content;
            //this.FirstRow = paragraph.FirstPhysicsRow;
        }

        public ParagraphVM(Favorites favorites)
        {
            throw new NotImplementedException();

            ID = Guid.NewGuid().ToString();

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
