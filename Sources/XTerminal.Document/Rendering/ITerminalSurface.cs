using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 表示一个用来渲染终端输出的表面
    /// </summary>
    public interface ITerminalSurface
    {
        /// <summary>
        /// 删除并释放渲染对象
        /// </summary>
        void Delete(VTDocumentElement drawable);

        /// <summary>
        /// 画
        /// 如果是文本元素，将对文本进行重新排版并渲染
        /// 排版是比较耗时的操作
        /// </summary>
        /// <param name="drawable"></param>
        void Draw(VTDocumentElement drawable);

        /// <summary>
        /// 根据元素的OffsetX和OffsetY属性，设置元素在Surface中的位置
        /// 而不用重新画，速度要比Draw快
        /// 画文本的速度还是比较慢的，因为需要对文本进行排版，耗时都花在排版上面了
        /// 所以能不排版就最好不排版
        /// </summary>
        /// <param name="drawable"></param>
        void Arrange(VTDocumentElement drawable);

        /// <summary>
        /// 设置元素的透明度
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="opacity"></param>
        void SetOpacity(VTDocumentElement drawable, double opacity);
    }
}
