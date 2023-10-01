using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 查找范围
    /// </summary>
    public enum FindScopes
    {
        /// <summary>
        /// 只查找当前显示的内容
        /// </summary>
        Document,

        /// <summary>
        /// 查找所有内容
        /// </summary>
        All
    }

    /// <summary>
    /// 从哪里开始查找
    /// </summary>
    public enum FindStartups
    {
        /// <summary>
        /// 从第一行开始向下查找
        /// </summary>
        FromBegin,

        /// <summary>
        /// 从最后一行开始向上查找
        /// </summary>
        FromEnd,

        /// <summary>
        /// 从当前位置向上查找
        /// </summary>
        CurrentToBegin,

        /// <summary>
        /// 从当前位置向下查找
        /// </summary>
        CurrentToEnd
    }

    /// <summary>
    /// 存储查找参数
    /// </summary>
    public class VTFindOptions
    {
        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; }

        public bool Regexp { get; set; }

        /// <summary>
        /// 要查找的关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 查找范围
        /// </summary>
        public FindScopes Scope { get; set; }

        /// <summary>
        /// 指定从哪里开始查找，怎么查找
        /// </summary>
        public FindStartups Startup { get; set; }

        /// <summary>
        /// 是否一次性把所有的都查找出来
        /// </summary>
        public bool FindAll { get; set; }

        public VTFindOptions()
        {

        }
    }
}
