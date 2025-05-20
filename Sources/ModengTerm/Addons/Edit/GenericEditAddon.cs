using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;

namespace ModengTerm.Addons.Edit
{
    public class EditAddonObject : AddonObject    
    {
        
    }

    public class GenericEditAddon : AddonModule
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("GenericEditAddon.Paste", Paste);
            this.RegisterCommand("GenericEditAddon.CopySelection", CopySelection);
            this.RegisterCommand("GenericEditAddon.SaveSelection", SaveSelection);
            this.RegisterCommand("GenericEditAddon.SaveViewport", SaveViewport);
            this.RegisterCommand("GenericEditAddon.SaveAll", SaveAll);
            this.RegisterCommand("GenericEditAddon.ClearScreen", ClearScreen);
        }

        protected override void OnRelease()
        {
        }

        private void Paste(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            string text = System.Windows.Clipboard.GetText();
            shellSessionVM.SendText(text);
        }

        private void CopySelection(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            shellSessionVM.CopySelection();
        }

        private void SaveSelection(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            SaveToFile(ParagraphTypeEnum.Selected, shellSessionVM);
        }

        private void SaveViewport(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            SaveToFile(ParagraphTypeEnum.Viewport, shellSessionVM);
        }

        private void SaveAll(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            SaveToFile(ParagraphTypeEnum.AllDocument, shellSessionVM);
        }

        private void ClearScreen(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            VTDocument document = shellSessionVM.VideoTerminal.ActiveDocument;
            document.DeleteViewoprt();
            document.SetCursorLogical(0, 0);
            document.RequestInvalidate();
        }

        #region 实例方法

        private ParagraphFormatEnum FilterIndex2FileType(int filterIndex)
        {
            switch (filterIndex)
            {
                case 1: return ParagraphFormatEnum.PlainText;
                case 2: return ParagraphFormatEnum.HTML;

                default:
                    throw new NotImplementedException();
            }
        }

        private void SaveToFile(ParagraphTypeEnum paragraphType, ShellSessionVM shellSession)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", shellSession.Session.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }

            ParagraphFormatEnum fileType = FilterIndex2FileType(saveFileDialog.FilterIndex);
            shellSession.SaveToFile(paragraphType, fileType, saveFileDialog.FileName);
        }

        #endregion
    }
}
