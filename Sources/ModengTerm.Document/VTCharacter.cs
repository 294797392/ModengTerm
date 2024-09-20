using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储终端里一个字符的信息
    /// VTCharacter保存要显示的字符信息和该字符的样式信息
    /// </summary>
    public class VTCharacter
    {
        /// <summary>
        /// 字符
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// 该字符所占用的列数
        /// 一个中文字符占用2列
        /// 每列的宽度是一个英文字符宽度
        /// 如果显示其他国家的语言，需要测试得出一个字符占用几列
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// 该字符的标志位
        /// </summary>
        public VTCharacterFlags Flags { get; set; }

        /// <summary>
        /// 按位存储该字符的属性
        /// 支持的属性请参考VTextAttributes枚举
        /// </summary>
        public int Attribute;

        /// <summary>
        /// 该字符的背景色
        /// </summary>
        public VTColor Background { get; set; }

        /// <summary>
        /// 该字符的前景色
        /// </summary>
        public VTColor Foreground { get; set; }

        #region 构造方法

        private VTCharacter()
        {
        }

        #endregion

        /// <summary>
        /// 创建一个新的字符
        /// </summary>
        /// <param name="ch">要新建的字符</param>
        /// <param name="columnSize">该字符占几列</param>
        /// <param name="attributeState"></param>
        /// <remarks>
        /// 该方法会首先从缓存里取字符，如果缓存里没有空闲字符，那么创建一个新的
        /// </remarks>
        /// <returns></returns>
        public static VTCharacter Create(char ch, int columnSize, VTextAttributeState attributeState = null)
        {
            VTCharacter character = new VTCharacter();
            if (attributeState != null)
            {
                character.Attribute = attributeState.Value;
                character.Background = attributeState.Background;
                character.Foreground = attributeState.Foreground;
            }
            character.Character = ch;
            character.ColumnSize = columnSize;
            character.Flags = VTCharacterFlags.None;
            return character;
        }

        /// <summary>
        /// 从缓冲池里创建一个空的字符
        /// TODO：考虑重命名为CreateSpace或者CreateBlank
        /// </summary>
        /// <returns></returns>
        public static VTCharacter CreateNull()
        {
            return Create(' ', 1, null);
        }
    }
}