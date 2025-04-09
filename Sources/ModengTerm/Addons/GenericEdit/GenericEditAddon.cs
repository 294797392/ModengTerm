using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.GenericEdit
{
    public class GenericEditAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("GenericEditAddon.Paste", this.ExecutePastCommand);
            this.RegisterCommand("GenericEditAddon.CopySelection", this.ExecuteCopyCommand);
            this.RegisterCommand("GenericEditAddon.SaveSelection", this.ExecuteSaveSelectionCommand);
            this.RegisterCommand("GenericEditAddon.SaveViewport", this.ExecuteSaveViewportCommand);
            this.RegisterCommand("GenericEditAddon.SaveAll", this.ExecuteSaveAllCommand);
            this.RegisterCommand("GenericEditAddon.ClearScreen", this.ExecuteClearScreen);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecutePastCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            shellSessionVM.Paste();
        }

        private void ExecuteCopyCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            shellSessionVM.CopySelection();
        }

        private void ExecuteSaveSelectionCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            this.SaveToFile(ParagraphTypeEnum.Selected, shellSessionVM);
        }

        private void ExecuteSaveViewportCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            this.SaveToFile(ParagraphTypeEnum.Viewport, shellSessionVM);
        }

        private void ExecuteSaveAllCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            this.SaveToFile(ParagraphTypeEnum.AllDocument, shellSessionVM);
        }

        private void ExecuteClearScreen() 
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
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

            ParagraphFormatEnum fileType = this.FilterIndex2FileType(saveFileDialog.FilterIndex);
            shellSession.SaveToFile(paragraphType, fileType, saveFileDialog.FileName);
        }

        #endregion
    }
}
