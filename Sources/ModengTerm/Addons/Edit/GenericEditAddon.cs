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

namespace ModengTerm.Addons.Edit
{
    public class GenericEditAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("GenericEditAddon.Paste", ExecutePastCommand);
            RegisterCommand("GenericEditAddon.CopySelection", ExecuteCopyCommand);
            RegisterCommand("GenericEditAddon.SaveSelection", ExecuteSaveSelectionCommand);
            RegisterCommand("GenericEditAddon.SaveViewport", ExecuteSaveViewportCommand);
            RegisterCommand("GenericEditAddon.SaveAll", ExecuteSaveAllCommand);
            RegisterCommand("GenericEditAddon.ClearScreen", ExecuteClearScreen);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecutePastCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //shellSessionVM.Paste();
        }

        private void ExecuteCopyCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //shellSessionVM.CopySelection();
        }

        private void ExecuteSaveSelectionCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //SaveToFile(ParagraphTypeEnum.Selected, shellSessionVM);
        }

        private void ExecuteSaveViewportCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //SaveToFile(ParagraphTypeEnum.Viewport, shellSessionVM);
        }

        private void ExecuteSaveAllCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //SaveToFile(ParagraphTypeEnum.AllDocument, shellSessionVM);
        }

        private void ExecuteClearScreen()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //VTDocument document = shellSessionVM.VideoTerminal.ActiveDocument;
            //document.DeleteViewoprt();
            //document.SetCursorLogical(0, 0);
            //document.RequestInvalidate();
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
