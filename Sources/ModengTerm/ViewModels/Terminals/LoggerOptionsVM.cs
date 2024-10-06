using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Loggering;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public class LoggerOptionsVM : ViewModelBase
    {
        #region 实例变量

        private string keyword;
        private bool ignoreCase;
        private bool regexp;
        private string filePath;
        private ParagraphFormatEnum fileType;
        private IVideoTerminal vt;

        #endregion

        #region 属性

        /// <summary>
        /// 要记录的关键字
        /// </summary>
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
        /// 是否忽略大小写
        /// </summary>
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

        /// <summary>
        /// 是否使用正则表达式
        /// </summary>
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

        /// <summary>
        /// 日志保存路径
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        public ParagraphFormatEnum FileType
        {
            get { return this.fileType; }
            set
            {
                if (this.fileType != value)
                {
                    this.fileType = value;
                    this.NotifyPropertyChanged("FileType");
                }
            }
        }

        #endregion

        #region 构造方法

        public LoggerOptionsVM(IVideoTerminal vt)
        {
            this.vt = vt;
        }

        #endregion

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

        private FilterTypeEnum GetFilterType()
        {
            if (string.IsNullOrEmpty(this.Keyword))
            {
                return FilterTypeEnum.None;
            }
            else
            {
                if (this.Regexp)
                {
                    return FilterTypeEnum.Regexp;
                }
                else
                {
                    return FilterTypeEnum.PlainText;
                }
            }
        }

        #endregion

        #region 公开接口

        public LoggerOptions GetOptions()
        {
            if (string.IsNullOrEmpty(this.FilePath))
            {
                MTMessageBox.Info("请选择日志保存路径");
                return null;
            }

            LoggerOptions loggerOptions = new LoggerOptions()
            {
                FilePath = this.FilePath,
                FileType = this.FileType,
                FilterText = this.Keyword,
                IgnoreCase = this.IgnoreCase,
                FilterType = this.GetFilterType()
            };

            return loggerOptions;
        }

        /// <summary>
        /// 选择日志文件的保存路径
        /// </summary>
        public void BrowseFilePath()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            dialog.FileName = String.Format("{0}_{1}", this.vt.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)(dialog.ShowDialog()))
            {
                this.FilePath = dialog.FileName;

                this.FileType = this.FilterIndex2FileType(dialog.FilterIndex);
            }
        }

        #endregion
    }
}
