using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Loggering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Logger
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
            get { return keyword; }
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                    NotifyPropertyChanged("Keyword");
                }
            }
        }

        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set
            {
                if (ignoreCase != value)
                {
                    ignoreCase = value;
                    NotifyPropertyChanged("IgnoreCase");
                }
            }
        }

        /// <summary>
        /// 是否使用正则表达式
        /// </summary>
        public bool Regexp
        {
            get { return regexp; }
            set
            {
                if (regexp != value)
                {
                    regexp = value;
                    NotifyPropertyChanged("Regexp");
                }
            }
        }

        /// <summary>
        /// 日志保存路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }

        public ParagraphFormatEnum FileType
        {
            get { return fileType; }
            set
            {
                if (fileType != value)
                {
                    fileType = value;
                    NotifyPropertyChanged("FileType");
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

        //private FilterTypeEnum GetFilterType()
        //{
        //    if (string.IsNullOrEmpty(Keyword))
        //    {
        //        return FilterTypeEnum.None;
        //    }
        //    else
        //    {
        //        if (Regexp)
        //        {
        //            return FilterTypeEnum.Regexp;
        //        }
        //        else
        //        {
        //            return FilterTypeEnum.PlainText;
        //        }
        //    }
        //}

        #endregion

        #region 公开接口

        /// <summary>
        /// 选择日志文件的保存路径
        /// </summary>
        public void BrowseFilePath()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            dialog.FileName = string.Format("{0}_{1}", vt.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)dialog.ShowDialog())
            {
                FilePath = dialog.FileName;

                FileType = FilterIndex2FileType(dialog.FilterIndex);
            }
        }

        #endregion
    }
}
