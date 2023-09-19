using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
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
        /// 如果显示其他国家的语言，需要测试得出一个字符占用几列
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// 该字符的标志位
        /// </summary>
        public VTCharacterFlags Flags { get; set; }

        /// <summary>
        /// 存储该字符的文本属性列表
        /// </summary>
        public List<VTextAttributeState> AttributeList { get; private set; }

        #region 构造方法

        private VTCharacter()
        {
        }

        #endregion

        /// <summary>
        /// 创建一个新的字符
        /// </summary>
        /// <remarks>
        /// 该方法会首先从缓存里取字符，如果缓存里没有空闲字符，那么创建一个新的
        /// </remarks>
        /// <returns></returns>
        public static VTCharacter Create(char ch, int columnSize, VTCharacterFlags flag, List<VTextAttributeState> attributeStates)
        {
            VTCharacter character = new VTCharacter();
            character.AttributeList = VTUtils.CreateTextAttributeStates();
            character.Character = ch;
            character.ColumnSize = columnSize;
            character.Flags = flag;
            if (attributeStates != null)
            {
                VTUtils.CopyAttributeState(attributeStates, character.AttributeList);
            }
            return character;
        }

        /// <summary>
        /// 从缓冲池里创建一个空的字符
        /// </summary>
        /// <returns></returns>
        public static VTCharacter CreateNull()
        {
            return Create(' ', 1, VTCharacterFlags.SingleByteChar, null);
        }
    }
}