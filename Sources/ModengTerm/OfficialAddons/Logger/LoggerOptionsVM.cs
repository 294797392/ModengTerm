using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Document.Enumerations;
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

        public LoggerOptionsVM()
        {
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
        #endregion
    }
}
