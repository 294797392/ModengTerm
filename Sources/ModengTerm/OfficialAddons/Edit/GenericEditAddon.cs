using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Document.Enumerations;
using System;

namespace ModengTerm.OfficialAddons.Edit
{
    public class GenericEditAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("GenericEditAddon.Paste", Paste);
            this.RegisterCommand("GenericEditAddon.CopySelection", CopySelection);
            this.RegisterCommand("GenericEditAddon.SaveSelection", SaveSelection);
            this.RegisterCommand("GenericEditAddon.SaveViewport", SaveViewport);
            this.RegisterCommand("GenericEditAddon.SaveAll", SaveAll);
            this.RegisterCommand("GenericEditAddon.ClearScreen", ClearScreen);
        }

        protected override void OnDeactive()
        {
        }

        private void Paste(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            string text = System.Windows.Clipboard.GetText();
            shell.Send(text);
        }

        private void CopySelection(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            shell.CopySelection();
        }

        private void SaveSelection(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            SaveToFile(ParagraphTypeEnum.Selected, shell);
        }

        private void SaveViewport(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            SaveToFile(ParagraphTypeEnum.Viewport, shell);
        }

        private void SaveAll(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            SaveToFile(ParagraphTypeEnum.AllDocument, shell);
        }

        private void ClearScreen(CommandArgs e)
        {
            IClientShellTab shell = this.client.GetActiveTab<IClientShellTab>();
            shell.ClearScreen();
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

        private void SaveToFile(ParagraphTypeEnum paragraphType, IClientShellTab shell)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", shell.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if (!(bool)saveFileDialog.ShowDialog())
            {
                return;
            }

            ParagraphFormatEnum fileType = FilterIndex2FileType(saveFileDialog.FilterIndex);
            shell.SaveToFile(paragraphType, fileType, saveFileDialog.FileName);
        }

        #endregion
    }
}
